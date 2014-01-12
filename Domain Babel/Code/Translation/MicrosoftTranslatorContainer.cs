using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Services.Client;
using System.Net;

namespace DomainBabel 
{    
    /// <summary>
    /// Microsoft Translator container class.
    /// </summary>
    public class MicrosoftTranslatorContainer : DataServiceContext, ITranslationContainer 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftTranslatorContainer"/> class.
        /// </summary>
        public MicrosoftTranslatorContainer() : base() 
        {
            if (ConfigurationManager.AppSettings["MicrosoftTranslationURL"] != null)
            {
                this.BaseUri = new Uri(ConfigurationManager.AppSettings["MicrosoftTranslationURL"]);
            }

            if (ConfigurationManager.AppSettings["MicrosoftTranslationKey"] != null)
            {
                this.Credentials = new NetworkCredential("accountKey", ConfigurationManager.AppSettings["MicrosoftTranslationKey"]);
            }
        }

        /// <summary>
        /// Gets a list of available languages for translation.
        /// </summary>
        /// <returns>List of languages</returns>
        public virtual IEnumerable<Language> LanguagesForTranslation()
        {
            DataServiceQuery<Language> query;

            query = this.CreateQuery<Language>("GetLanguagesForTranslation");

            return query.Execute();
        }
        
        /// <summary>
        /// Translates a specified text from one language to another.
        /// </summary>
        /// <param name="text">The text to translate</param>
        /// <param name="toLanguage">The language code to translate the text into</param>
        /// <param name="fromLanguage">The language code of the translation text</param>
        /// <returns>List of translations</returns>
        public virtual IEnumerable<Translation> Translate(string text, string toLanguage, string fromLanguage) 
        {
            if (text == null) 
            {
                throw new System.ArgumentNullException("text", "Text value cannot be null");
            }

            if (toLanguage == null) 
            {
                throw new System.ArgumentNullException("toLanguage", "To value cannot be null");
            }

            DataServiceQuery<Translation> query;

            query = this.CreateQuery<Translation>("Translate");

            if (text != null) 
            {
                query = query.AddQueryOption("Text", string.Concat("\'", System.Uri.EscapeDataString(text), "\'"));
            }

            if (toLanguage != null) 
            {
                query = query.AddQueryOption("To", string.Concat("\'", System.Uri.EscapeDataString(toLanguage), "\'"));
            }

            if (fromLanguage != null) 
            {
                query = query.AddQueryOption("From", string.Concat("\'", System.Uri.EscapeDataString(fromLanguage), "\'"));
            }

            return query.Execute();
        }
    }
}
