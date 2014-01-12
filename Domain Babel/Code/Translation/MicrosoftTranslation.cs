using System.Collections.ObjectModel;
using System.Data.Services.Client;
using System.Linq;

namespace DomainBabel
{
    /// <summary>
    /// Wrapper class for calling Microsoft Translate functions.
    /// </summary>
    public class MicrosoftTranslation : ITranslation
    {
        /// <summary>
        /// Instance of the <see cref="ITableStorage"/> class.
        /// </summary>
        private ITableStorage tableStorage;

        /// <summary>
        /// Instance of the <see cref="ITranslationContainer"/> class.
        /// </summary>
        private ITranslationContainer client;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftTranslation"/> class.
        /// </summary>
        /// <param name="tableStorage">Table Storage class</param>
        /// <param name="client">Translation container class</param>
        public MicrosoftTranslation(ITableStorage tableStorage, ITranslationContainer client)
        {
            this.tableStorage = tableStorage;
            this.client = client;
        }

        /// <summary>
        /// Gets all the available languages.
        /// </summary>
        /// <returns>Collection of ISO language codes</returns>
        public ReadOnlyCollection<string> Languages()
        {
            try
            {
                // exclude languages that don't support the Latin alphabet
                var languageList = this.client.LanguagesForTranslation()
                                    .ToList()
                                    .Where(l => l.Code != "mt" &&
                                                l.Code != "tlh" &&
                                                l.Code != "tlh-Qaak")
                                    .Select(l => l.Code)
                                    .ToList();

                return new ReadOnlyCollection<string>(languageList);
            }
            catch (DataServiceQueryException)
            {
                return null;
            }
        }

        /// <summary>
        /// Get translation of the specified text from the specified language to English.
        /// </summary>
        /// <param name="searchText">Phrase to translate</param>
        /// <param name="sourceLanguage">Language ISO code to translate from</param>
        /// <returns>Array of found translations</returns>
        public string[] GetTranslation(string searchText, string sourceLanguage)
        {
            // check first if there is a valid translation in storage
            var translations = this.tableStorage.GetTranslation(sourceLanguage, searchText);

            if (translations != null)
            {
                return translations;
            }

            // not found, try translation API
            try
            {
                var foundTranslations = this.client.Translate(searchText, "en", sourceLanguage)
                                            .ToList()
                                            .Select(t => t.Text)
                                            .Where(t => t.ToUpperInvariant() != searchText.ToUpperInvariant())
                                            .ToArray();

                if (foundTranslations != null)
                {
                    // update table storage
                    this.tableStorage.AddTranslation(sourceLanguage, searchText, foundTranslations);
                }

                return foundTranslations;
            }
            catch (DataServiceQueryException)
            {
                return null;
            }
        }
    }
}