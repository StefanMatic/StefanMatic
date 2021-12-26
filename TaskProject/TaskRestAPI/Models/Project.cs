using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace TaskRestAPI.Models
{
    public class Project: AzureTableEntity
    {
        private const string _tableName = "Projects";

        public Project()
        {
            this.TableName = _tableName;
        }

        public Project(string name, string code): this()
        {
            this.Name = name;
            this.Code = code;
            this.Id = Guid.NewGuid().ToString();

            PartitionKey = this.Name;
            RowKey = this.Id;
        }

        /// <summary>
        ///     Create an entity in the table using this object
        /// </summary>
        /// <returns></returns>
        public Project Create()
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
            Project project = this.GetRows<Project>(filter).FirstOrDefault();
            if (project == null)
                return false;
            else
                return true;
        }


        /// <summary>
        ///     Get the wanted entity
        /// </summary>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        public Project Get(string rowKey=null)
        {
            string filter = $"RowKey eq '{rowKey ?? this.RowKey}'";
            return this.GetRows<Project>(filter).FirstOrDefault();
        }

        /// <summary>
        ///     Gets the wanted entity using the partition and row key
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partition"></param>
        /// <returns></returns>
        public Project Get(string rowKey = null, string partition = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"RowKey eq '{rowKey ?? this.RowKey}' ");
            sb.Append($" and PartitionKey eq '{partition ?? this.PartitionKey}'");
 
            return this.GetRows<Project>(sb.ToString()).FirstOrDefault();
        }


        /// <summary>
        ///     Gets all projects
        /// </summary>
        /// <returns></returns>
        public List<Project> GetAll()
        {
            return this.GetRows<Project>(string.Empty);
        }

        /// <summary>
        ///     Updates the code value of the project entity
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Project Update(Project project)
        {
            //Only the code can be modified
            this.Code = project.Code;

            this.UpdateRow();
            return this;
        }

        
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }

    }
}