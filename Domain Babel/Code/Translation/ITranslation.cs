using System;
using System.Collections.ObjectModel;

namespace DomainBabel
{
    /// <summary>
    /// Interface for translation functions class.
    /// </summary>
    public interface ITranslation
    {
        /// <summary>
        /// Gets all the available languages.
        /// </summary>
        /// <returns>Collection of ISO language codes</returns>
        ReadOnlyCollection<string> Languages();

        /// <summary>
        /// Get translation of the specified text from the specified language to English.
        /// </summary>
        /// <param name="searchText">Phrase to translate</param>
        /// <param name="sourceLanguage">Language ISO code to translate from</param>
        /// <returns>Array of found translations</returns>
        string[] GetTranslation(string searchText, string sourceLanguage);
    }
}
