using Microsoft.WindowsAzure.Storage.Table;

namespace DomainBabel
{
    /// <summary>
    /// Interface for Cloud Table class.
    /// </summary>
    public interface ICloudTable
    {
        /// <summary>
        /// Executes an operation on the table.
        /// </summary>
        /// <param name="operation">Table operation</param>
        /// <returns>Table result</returns>
        TableResult Execute(TableOperation operation);
    }
}
