using System;
using System.Collections.Generic;

namespace DomainBabel
{
    /// <summary>
    /// Interface for translation container class.
    /// </summary>
    public interface ITranslationContainer
    {
        /// <summary>
        /// Gets a list of available languages for translation.
        /// </summary>
        /// <returns>List of languages</returns>
        IEnumerable<Language> LanguagesForTranslation();

        /// <summary>
        /// Translates a specified text from one language to another.
        /// </summary>
        /// <param name="text">The text to translate</param>
        /// <param name="toLanguage">The language code to translate the text into</param>
        /// <param name="fromLanguage">The language code of the translation text</param>
        /// <returns>List of translations</returns>
        IEnumerable<Translation> Translate(string text, string toLanguage, string fromLanguage);
    }
}
