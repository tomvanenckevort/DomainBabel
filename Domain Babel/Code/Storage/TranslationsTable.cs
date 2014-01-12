using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;    

namespace DomainBabel
{
    /// <summary>
    /// Cloud table containing translations.
    /// </summary>
    public class TranslationsTable : ICloudTable
    {
        /// <summary>
        /// Table name for Translations table.
        /// </summary>
        private const string TranslationsTableName = "Translations";

        /// <summary>
        /// Storage account instance.
        /// </summary>
        private CloudStorageAccount storageAccount;

        /// <summary>
        /// Table client instance.
        /// </summary>
        private CloudTableClient tableClient;

        /// <summary>
        /// Cloud table instance.
        /// </summary>
        private CloudTable translationsTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationsTable"/> class.
        /// </summary>
        public TranslationsTable()
        {
            this.storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            bool useTableStorage = bool.Parse(ConfigurationManager.AppSettings["UseTableStorage"]);

            if (useTableStorage)
            {
                this.tableClient = this.storageAccount.CreateCloudTableClient();

                this.translationsTable = this.tableClient.GetTableReference(TranslationsTableName);

                // check that table exists
                this.translationsTable.CreateIfNotExists();
            }
        }

        /// <summary>
        /// Executes an operation on the table.
        /// </summary>
        /// <param name="operation">Table operation</param>
        /// <returns>Table result</returns>
        public TableResult Execute(TableOperation operation)
        {
            if (this.translationsTable != null)
            {
                return this.translationsTable.Execute(operation);
            }
            else
            {
                return new TableResult() { HttpStatusCode = 500 };
            }
        }
    }
}