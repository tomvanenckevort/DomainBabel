using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace DomainBabel
{
    /// <summary>
    /// Wrapper class for calling Azure Table Storage functions.
    /// </summary>
    public class AzureTableStorage : ITableStorage
    {
        /// <summary>
        /// Translations table instance.
        /// </summary>
        private ICloudTable translationsTable;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTableStorage"/> class.
        /// </summary>
        /// <param name="translationsTable">Translations table</param>
        public AzureTableStorage(ICloudTable translationsTable)
        {
            this.translationsTable = translationsTable;
        }

        /// <summary>
        /// Executes a table operation.
        /// </summary>
        /// <param name="operation">Operation to execute</param>
        /// <returns>Operation result</returns>
        public virtual TableResult Execute(TableOperation operation)
        {
            return this.translationsTable.Execute(operation);
        }

        /// <summary>
        /// Adds or updates a translation to the table.
        /// </summary>
        /// <param name="language">Language translation is from</param>
        /// <param name="part">Text part that is translated</param>
        /// <param name="translations">Translation texts in English</param>
        public void AddTranslation(string language, string part, string[] translations)
        {
            if (language == null || part == null)
            {
                return;
            }

            var entity = new TranslationEntity()
            {
                Language = language,
                Part = part
            };

            entity.SetTranslations(translations);

            var insertOperation = TableOperation.InsertOrReplace(entity);

            this.Execute(insertOperation);
        }

        /// <summary>
        /// Gets translation from table.
        /// </summary>
        /// <param name="language">Language translation is from</param>
        /// <param name="part">Text part that is translation</param>
        /// <returns>Translation texts in English (or null if not found or expired)</returns>
        public string[] GetTranslation(string language, string part)
        {
            if (language == null || part == null)
            {
                return null;
            }

            var retrieveOperation = TableOperation.Retrieve<TranslationEntity>(language, part);

            var result = this.Execute(retrieveOperation);

            if (result.Result != null)
            {
                var entity = (TranslationEntity)result.Result;

                if (DateTime.UtcNow >= entity.Timestamp.AddDays(30))
                {
                    // translation 'expired'
                    return null;
                }

                return entity.Translation.ToArray();
            }
            else
            {
                return null;
            }
        }
    }
}