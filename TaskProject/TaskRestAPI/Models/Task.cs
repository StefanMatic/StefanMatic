using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace TaskRestAPI.Models
{
    public class Task: AzureTableEntity
    {
        private const string _tableName = "Tasks";

        public Task()
        {
            this.TableName = _tableName;
        }

        public Task(string projectName, string taskName, string description) : this()
        {
            this.Name = taskName;
            this.Description = description;
            this.Id = Guid.NewGuid().ToString();

            PartitionKey = projectName;
            RowKey = this.Id;
        }

        /// <summary>
        ///     Creating a new entity in Azure table storage
        /// </summary>
        /// <returns></returns>
        public Task Create()
        {
            this.CreateRow();
            return this;
        }

        /// <summary>
        ///     Project Exists
        /// </summary>
        /// <returns>bool</returns>
        public bool Exists(string rowKey = null, string partition = null)
        {
            string filter = $"RowKey eq '{rowKey ?? this.RowKey}' and PartitionKey eq '{partition ?? this.PartitionKey}'";
            Task task = this.GetRows<Task>(filter).FirstOrDefault();

            if (task == null)
                return false;
            else
                return true;
        }


        /// <summary>
        ///     Get the wanted entity
        /// </summary>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        public Task Get(string rowKey = null)
        {
            string filter = $"RowKey eq '{rowKey ?? this.RowKey}'";
            return this.GetRows<Task>(filter).FirstOrDefault();
        }

        /// <summary>
        ///     Gets the wanted entity using the partition and row key
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partition"></param>
        /// <returns></returns>
        public Task Get(string rowKey = null, string partition = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"RowKey eq '{rowKey ?? this.RowKey}' ");
            sb.Append($" and PartitionKey eq '{partition ?? this.PartitionKey}'");

            return this.GetRows<Task>(sb.ToString()).FirstOrDefault();
        }

        /// <summary>
        ///     Gets all projects
        /// </summary>
        /// <returns></returns>
        public List<Task> GetAll(string partitionKey = null)
        {
            //We want all the task in the partition where we put all the tasks from one project
            string filter = $"PartitionKey eq '{partitionKey ?? this.PartitionKey}'";
            return this.GetRows<Task>(filter);
        }

        /// <summary>
        ///     Updates the name and description fields of the Task entity
        /// </summary>
        /// <param name="newTask"></param>
        /// <returns></returns>
        public Task Update(Task newTask)
        {
            //Only the code can be modified
            this.Name = newTask.Name;
            this.Description = newTask.Description;

            this.UpdateRow();
            return this;
        }


        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}