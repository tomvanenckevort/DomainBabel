using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;

namespace DomainBabel.Test.Mocks
{
    public class MockTranslationContainer : MicrosoftTranslatorContainer
    {
        private bool throwException = false;

        public static List<Language> Languages = new List<Language>()
            {
                new Language() { Code = "nl" },
                new Language() { Code = "en" },
                new Language() { Code = "de" },
                new Language() { Code = "fr" },
                new Language() { Code = "mww" },
                new Language() { Code = "ht" },
                new Language() { Code = "tlh" },
                new Language() { Code = "tlh-Qaak" },
                new Language() { Code = "mt" },
                new Language() { Code = "other" }
            };

        public static List<Translation> Translations = new List<Translation>()
            {
                new Translation() { Text = "ONE" },
                new Translation() { Text = "TWO" },
                new Translation() { Text = "THREE" }
            };

        public static List<Translation> TranslationES = new List<Translation>()
            {
                new Translation() { Text = "ÁRBOL" }
            };

        public static List<Translation> TranslationFR = new List<Translation>()
            {
                new Translation() { Text = "ARBRE" }
            };

        public MockTranslationContainer(bool throwException = false)
        {
            this.throwException = throwException;
        }

        public override IEnumerable<Language> LanguagesForTranslation()
        {
            if (this.throwException)
            {
                throw new DataServiceQueryException();
            }

            return Languages;
        }

        public override IEnumerable<Translation> Translate(string text, string toLanguage, string fromLanguage) 
        {
            if (text == "TEST" && toLanguage == "en" && fromLanguage == "nl")
            {
                return Translations;
            }
            else if (text == "TREE" && toLanguage == "en" && fromLanguage == "es")
            {
                return TranslationES;
            }
            else if (text == "TREE" && toLanguage == "en" && fromLanguage == "fr")
            {
                return TranslationFR;
            }
            else if (string.IsNullOrEmpty(fromLanguage))
            {
                throw new DataServiceQueryException();
            }
            else
            {
                return Enumerable.Empty<Translation>();
            }
        }
    }
}
