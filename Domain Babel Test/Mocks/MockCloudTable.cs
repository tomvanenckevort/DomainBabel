using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace DomainBabel.Test.Mocks
{
    public class MockCloudTable : ICloudTable
    {
        public static IList<TranslationEntity> Entities = new List<TranslationEntity>()
            {
                new TranslationEntity() { Language = "nl", Part = "TREE", Translations = "BOOM", Timestamp = DateTimeOffset.Now.AddDays(-1) },
                new TranslationEntity() { Language = "de", Part = "TREE", Translations = "BAUM", Timestamp = DateTimeOffset.Now.AddDays(-2) },
                new TranslationEntity() { Language = "fr", Part = "TREE", Translations = "ARBRE", Timestamp = DateTimeOffset.Now.AddDays(-35) }
            };
            
        public TableResult Execute(TableOperation operation)
        {
 	        var result = new TableResult();

            var propEntityType = typeof(TableOperation).GetProperty("Entity", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var entity = (TranslationEntity)propEntityType.GetValue(operation);

            var propRetrievePartitionKeyType = typeof(TableOperation).GetProperty("RetrievePartitionKey", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var retrievePartitionKey = (string)propRetrievePartitionKeyType.GetValue(operation);

            var propRetrieveRowKeyType = typeof(TableOperation).GetProperty("RetrieveRowKey", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var retrieveRowKey = (string)propRetrieveRowKeyType.GetValue(operation);

            var propOperationType = typeof(TableOperation).GetProperty("OperationType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var operationType = (TableOperationType)propOperationType.GetValue(operation);

            switch (operationType)
            {
                case TableOperationType.Insert:
                case TableOperationType.InsertOrMerge:
                case TableOperationType.InsertOrReplace:
                    entity.Timestamp = DateTimeOffset.UtcNow;

                    var existingEntity = Entities.FirstOrDefault(t => t.PartitionKey == entity.PartitionKey && t.RowKey == entity.RowKey);

                    if (existingEntity != null)
                    {
                        Entities.Remove(existingEntity);
                    }

                    Entities.Add(entity);

                    result.HttpStatusCode = (int)HttpStatusCode.OK;
                    result.Result = entity;
                    break;
                case TableOperationType.Delete:
                    Entities.Remove(entity);
                    result.HttpStatusCode = (int)HttpStatusCode.OK;
                    break;
                case TableOperationType.Retrieve:
                    result.Result = Entities.FirstOrDefault(t => t.PartitionKey == retrievePartitionKey && t.RowKey == retrieveRowKey);
                    result.HttpStatusCode = (int)(result.Result != null ? HttpStatusCode.OK : HttpStatusCode.NotFound);
                    break;
                default:
                    result.HttpStatusCode = (int)HttpStatusCode.NotFound;
                    break;
            }

            return result;
        }
    }
}
