using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace DomainBabel
{
    /// <summary>
    /// Interface for Table Storage class.
    /// </summary>
    public interface ITableStorage
    {
        /// <summary>
        /// Executes a table operation.
        /// </summary>
        /// <param name="operation">Operation to execute</param>
        /// <returns>Operation result</returns>
        TableResult Execute(TableOperation operation);

        /// <summary>
        /// Adds or updates a translation to the table.
        /// </summary>
        /// <param name="language">Language translation is from</param>
        /// <param name="part">Text part that is translated</param>
        /// <param name="translations">Translation texts in English</param>
        void AddTranslation(string language, string part, string[] translations);

        /// <summary>
        /// Gets translation from table.
        /// </summary>
        /// <param name="language">Language translation is from</param>
        /// <param name="part">Text part that is translation</param>
        /// <returns>Translation texts in English (or null if not found or expired)</returns>
        string[] GetTranslation(string language, string part);
    }
}
