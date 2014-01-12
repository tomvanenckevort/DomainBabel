using System.Linq;
using DomainBabel.Test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainBabel.Test
{
    [TestClass]
    public class MicrosoftTranslatorContainerTest
    {
        private ITranslationContainer translatorContainer;

        [TestInitialize]
        public void Init()
        {
            this.translatorContainer = new MockTranslationContainer();
        }

        [TestMethod]
        public void GetLanguages()
        {
            var foundLanguages = this.translatorContainer.LanguagesForTranslation();

            Assert.IsNotNull(foundLanguages);
            CollectionAssert.AreEqual(MockTranslationContainer.Languages, foundLanguages.ToList());
        }

        [TestMethod]
        public void GetTranslations()
        {
            var foundTranslations = this.translatorContainer.Translate("TEST", "en", "nl");

            Assert.IsNotNull(foundTranslations);
            CollectionAssert.AreEqual(MockTranslationContainer.Translations, foundTranslations.ToList());
        }

        [TestMethod]
        public void GetNonExistingTranslations()
        {
            var foundTranslations = this.translatorContainer.Translate("TEST", "en", "de");

            Assert.IsNotNull(foundTranslations);
            Assert.IsTrue(foundTranslations.Count() == 0);
        }
    }
}
