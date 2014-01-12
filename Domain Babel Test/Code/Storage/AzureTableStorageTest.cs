using System.Web.Mvc;
using DomainBabel.Test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleInjector;

namespace DomainBabel.Test
{
    [TestClass]
    public class AzureTableStorageTest
    {
        private ITableStorage storage;

        [TestInitialize]
        public void Init()
        {
            var container = new Container();

            container.Register<ICloudTable, MockCloudTable>();
            container.Register<ITableStorage, AzureTableStorage>();

            container.Verify();

            DependencyResolver.SetResolver(container);

            this.storage = DependencyResolver.Current.GetService<ITableStorage>();
        }

        [TestMethod]
        public void TranslationEntityKeys()
        {
            var entity = new TranslationEntity()
                {
                    PartitionKey = "ABC",
                    RowKey = "DEF"
                };

            Assert.AreEqual(entity.PartitionKey, entity.Language);
            Assert.AreEqual(entity.RowKey, entity.Part);
        }

        [TestMethod]
        public void TranslationEntityEmptyTranslations()
        {
            var entity = new TranslationEntity();

            entity.SetTranslations(null);

            Assert.IsNull(entity.Translation);
        }

        [TestMethod]
        public void AddNewTranslation()
        {
            var language = "es";
            var part = "TREE";
            var translations = new string[] { "ÁRBOL" };

            // add translation
            this.storage.AddTranslation(language, part, translations);

            // retrieve translation
            var foundTranslations = this.storage.GetTranslation(language, part);

            CollectionAssert.AreEqual(foundTranslations, translations);
        }

        [TestMethod]
        public void AddNullTranslation()
        {
            var language = "es";
            var part = "TREE";
            var translations = new string[] { "ÁRBOL" };

            // add translation
            this.storage.AddTranslation(null, part, translations);

            this.storage.AddTranslation(language, null, translations);

            // retrieve translation
            var foundTranslations = this.storage.GetTranslation(language, part);

            CollectionAssert.AreEqual(foundTranslations, translations);
        }

        [TestMethod]
        public void UpdateExistingTranslation()
        {
            var language = "nl";
            var part = "TREE";
            var translations = new string[] { "BOOM", "ROOS" };

            // add translation
            this.storage.AddTranslation(language, part, translations);

            // retrieve translation
            var foundTranslations = this.storage.GetTranslation(language, part);

            CollectionAssert.AreEqual(foundTranslations, translations);
        }

        [TestMethod]
        public void GetTranslation()
        {
            var language = "de";
            var part = "TREE";
            var translations = new string[] { "BAUM" };

            // retrieve translation
            var foundTranslations = this.storage.GetTranslation(language, part);

            CollectionAssert.AreEqual(foundTranslations, translations);
        }

        [TestMethod]
        public void GetNonExistingTranslation()
        {
            var language = "it";
            var part = "TREE";

            // retrieve translation
            var foundTranslations = this.storage.GetTranslation(language, part);

            Assert.IsNull(foundTranslations);
        }

        [TestMethod]
        public void GetExpiredTranslation()
        {
            var language = "fr";
            var part = "TREE";

            // retrieve translation
            var foundTranslations = this.storage.GetTranslation(language, part);

            Assert.IsNull(foundTranslations);
        }

        [TestMethod]
        public void GetNullTranslation()
        {
            var language = "en";
            var part = "TREE";

            // retrieve translation
            var foundTranslations = this.storage.GetTranslation(null, part);

            Assert.IsNull(foundTranslations);

            foundTranslations = this.storage.GetTranslation(language, null);

            Assert.IsNull(foundTranslations);

            foundTranslations = this.storage.GetTranslation(null, null);

            Assert.IsNull(foundTranslations);
        }
    }
}
