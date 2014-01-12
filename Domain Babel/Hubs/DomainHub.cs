using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;

namespace DomainBabel
{
    /// <summary>
    /// SignalR hub class for managing client searches.
    /// </summary>
    public class DomainHub : Hub
    {
        /// <summary>
        /// List of connection IDs for currently connected clients.
        /// </summary>
        private static readonly HashSet<string> Connections = new HashSet<string>();

        /// <summary>
        /// Translation functions.
        /// </summary>
        private readonly ITranslation translation;

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainHub"/> class.
        /// </summary>
        public DomainHub()
        {
            this.translation = DependencyResolver.Current.GetService<ITranslation>();
        }

        /// <summary>
        /// Called when connection connects to the hub.
        /// </summary>
        /// <returns>Task to execute</returns>
        public override Task OnConnected()
        {
            Connections.Add(Context.ConnectionId);

            return base.OnConnected();
        }

        /// <summary>
        /// Called when connection reconnects to the hub.
        /// </summary>
        /// <returns>Task to execute</returns>
        public override Task OnReconnected()
        {
            Connections.Add(Context.ConnectionId);

            return base.OnReconnected();
        }

        /// <summary>
        /// Called when connection disconnects from the hub.
        /// </summary>
        /// <returns>Task to execute</returns>
        public override Task OnDisconnected()
        {
            Connections.Remove(Context.ConnectionId);

            return base.OnDisconnected();
        }

        /// <summary>
        /// Start searching for translations for the specified search text.
        /// </summary>
        /// <param name="searchText">Search text used for finding translations</param>
        /// <returns>Async operation</returns>
        public async Task Search(string searchText)
        {
            if (searchText == null || searchText.LastIndexOf('.') < 0)
            {
                return;
            }

            // remove the domain extension
            searchText = searchText.Remove(searchText.LastIndexOf('.'));

            // trim off any spaces
            searchText = searchText.Trim().ToUpperInvariant();

            // split it in parts to translate
            List<string> searchTextParts = new List<string>();

            for (int i = 3; i <= searchText.Length; i++)
            {
                for (int j = 0; j <= (searchText.Length - i); j++)
                {
                    searchTextParts.Add(searchText.Substring(j, i));
                }
            }

            if (searchTextParts.Count == 0)
            {
                return;
            }

            // get available languages
            var languages = this.translation.Languages();

            if (languages == null)
            {
                return;
            }

            try
            {
                this.CheckConnection();

                Clients.Caller.LanguageCount(languages.Count);

                var translateTasks = languages.Select(language => this.SearchLanguage(language, searchTextParts));
            
                await Task.WhenAll(translateTasks)
                            .ContinueWith(t =>
                            {
                                // disconnect client when done
                                this.CheckConnection();

                                Clients.Caller.Stop();
                            });
            }
            catch (OperationCanceledException) 
            { 
                // search has been canceled.
            }
        }

        /// <summary>
        /// Searches for each text part through specified language.
        /// </summary>
        /// <param name="language">Language ISO code</param>
        /// <param name="searchTextParts">List of text parts to search through</param>
        /// <returns>Async operation</returns>
        private async Task SearchLanguage(string language, List<string> searchTextParts)
        {
            // get full language name
            string languageFullName = null;

            try
            {
                languageFullName = new CultureInfo(language).EnglishName;
            }
            catch (CultureNotFoundException)
            {
                switch (language)
                {
                    case "mww":
                        languageFullName = "Hmong Daw";
                        break;
                    case "ht":
                        languageFullName = "Haitian";
                        break;
                    default:
                        // fall back to language code
                        languageFullName = language.ToUpperInvariant();
                        break;
                }
            }

            // get translations for each language
            var searchTasks = searchTextParts.Select(async part => await this.SearchTextPart(part, language, languageFullName));

            await Task.WhenAll(searchTasks);
        }

        /// <summary>
        /// Searches for text part in specified language.
        /// </summary>
        /// <param name="part">Text part</param>
        /// <param name="language">Language ISO code</param>
        /// <param name="languageFullName">Language full name</param>
        /// <returns>Async operation</returns>
        private async Task SearchTextPart(string part, string language, string languageFullName)
        {
            var translations = this.translation.GetTranslation(part, language);

            if (translations != null)
            {
                foreach (var translation in translations)
                {
                    this.CheckConnection();

                    // send found translation back to client
                    Clients.Caller.AddResult(part, languageFullName, translation);
                }
            }

            await Task.Yield();
        }

        /// <summary>
        /// Checks whether client is still connected and cancels operation if not.
        /// </summary>
        private void CheckConnection()
        {
            if (!Connections.Contains(Context.ConnectionId))
            {
                throw new OperationCanceledException();
            }
        }
    }
}