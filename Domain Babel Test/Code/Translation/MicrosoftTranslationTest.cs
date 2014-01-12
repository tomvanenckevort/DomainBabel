using System.Linq;
using System.Web.Mvc;
using DomainBabel.Test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleInjector;

namespace DomainBabel.Test.Code.Translation
{
    [TestClass]
    public class MicrosoftTranslationTest
    {
        private ITranslation translation;

        [TestInitialize]
        public void Init()
        {
            var container = new Container();

            container.Register<ICloudTable, MockCloudTable>();
            container.Register<ITableStorage, AzureTableStorage>();
            container.RegisterSingle<ITranslationContainer>(new MockTranslationContainer());
            container.Register<ITranslation, MicrosoftTranslation>();

            container.Verify();

            DependencyResolver.SetResolver(container);

            this.translation = DependencyResolver.Current.GetService<ITranslation>();
        }

        [TestMethod]
        public void GetLanguages()
        {
            var foundLanguages = this.translation.Languages();

            Assert.IsNotNull(foundLanguages);
            CollectionAssert.AreEqual(MockTranslationContainer.Languages
                                        .Select(l => l.Code)
                                        .Except(new string[] { "mt", "tlh", "tlh-Qaak" })
                                        .ToList(), foundLanguages);
        }

        [TestMethod]
        public void GetExceptionLanguages()
        {
            this.translation = new MicrosoftTranslation(DependencyResolver.Current.GetService<ITableStorage>(), new MockTranslationContainer(true));

            var foundLanguages = this.translation.Languages();

            Assert.IsNull(foundLanguages);
        }

        [TestMethod]
        public void GetTranslationFromStorage()
        {
            var translations = this.translation.GetTranslation("TREE", "nl");

            Assert.IsNotNull(translations);
            Assert.IsTrue(translations.Contains("BOOM"));
        }

        [TestMethod]
        public void GetTranslationFromService()
        {
            var translations = this.translation.GetTranslation("TREE", "es");

            Assert.IsNotNull(translations);
            CollectionAssert.AreEqual(MockTranslationContainer.TranslationES.Select(t => t.Text).ToArray(), translations);
        }

        [TestMethod]
        public void GetExpiredTranslationFromService()
        {
            var translations = this.translation.GetTranslation("TREE", "fr");

            Assert.IsNotNull(translations);
            CollectionAssert.AreEqual(MockTranslationContainer.TranslationFR.Select(t => t.Text).ToArray(), translations);
        }

        [TestMethod]
        public void GetExceptionFromService()
        {
            var translations = this.translation.GetTranslation("TREE", "");

            Assert.IsNull(translations);
        }

        [TestMethod]
        public void GetNonExistingTranslation()
        {
            var translations = this.translation.GetTranslation("TREE", "dk");

            Assert.IsNotNull(translations);
            Assert.IsTrue(translations.Count() == 0);
        }
    }
}
