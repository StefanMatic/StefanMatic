using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace TaskRestAPI.Models
{

    /*
     * This class is made with the intent to do all of the underlying 
     * work with the azure table, so the child classess would be more manageble to use
     */
    public class AzureTableEntity: TableEntity
    {
        //connection string
        private readonly CloudStorageAccount _storageAccount =
            CloudStorageAccount.Parse(ConfigurationManager.
                    AppSettings["StorageConnectionString"]);

        public AzureTableEntity()
        {
            var now = DateTime.Now;
            this.Created = now;
            this.Modified = now;
        }

        /// <summary>
        ///     Create Row
        /// </summary>
        /// <returns>AzureTableEntity</returns>
        public AzureTableEntity CreateRow()
        {
            var client = this._storageAccount.CreateCloudTableClient();
            var table = client.GetTableReference(this.TableName);

            table.CreateIfNotExists();

            TableOperation tableOperation = TableOperation.InsertOrReplace(this);
            table.Execute(tableOperation);

            return this;
        }

        /// <summary>
        ///     Get Rows
        /// </summary>
        /// <param name="filter">
        ///     Takes a filter. If the filter is empty, it takes all the entities from the table
        /// </param>
        /// <returns>List<Type></returns>
        public List<T> GetRows<T>(string filter) where T : AzureTableEntity, new()
        {
            var client = this._storageAccount.CreateCloudTableClient();
            var table = client.GetTableReference(this.TableName);

            table.CreateIfNotExists();

            TableQuery<T> query = (filter == string.Empty) ? new TableQuery<T>() : new TableQuery<T>().Where(filter);

            IEnumerable<T> IDataList = table.ExecuteQuery(query);
            List<T> DataList = new List<T>();
            foreach (var singleData in IDataList)
                DataList.Add(singleData);

            return DataList;
        }

        /// <summary>
        ///     Update Row
        /// </summary>
        /// <returns>AzureTableEntity</returns>
        public AzureTableEntity UpdateRow()
        {
            var client = this._storageAccount.CreateCloudTableClient();
            var table = client.GetTableReference(this.TableName);

            table.CreateIfNotExists();

            this.Modified = DateTime.Now;

            //Updates the data if the entity exists. If not, inserts it as new data
            TableOperation tableOperation = TableOperation.InsertOrMerge(this);
            table.Execute(tableOperation);

            Debug.WriteLine(JsonConvert.SerializeObject(this));

            return this;
        }

        /// <summary>
        ///     Delete Row
        /// </summary>
        /// <returns>Nothing</returns>
        public void DeleteRow()
        {
            var client = this._storageAccount.CreateCloudTableClient();
            var table = client.GetTableReference(this.TableName);

            table.CreateIfNotExists();

            //reset the ETag(resource versioning machanism)
            this.ETag = "*";

            TableOperation tableOperation = TableOperation.Delete(this);
            table.Execute(tableOperation);
        }


        //The table name is going to be used internally so we don't want it to be saved in the table storage
        [IgnoreProperty]
        public string TableName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }


}