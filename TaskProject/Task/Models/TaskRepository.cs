using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TaskRestAPI.Models;
using ProjectTask = TaskRestAPI.Models.Task;

namespace TaskProject.Models
{
    public class TaskRepository : ITaskRepository
    {
        private readonly string _baseUrl = @"http://akvelon.azurewebsites.net/api/Task";


        /// <summary>
        ///     Create a new task entity record
        /// </summary>
        /// <param name="newTask"></param>
        /// <returns></returns>
        public async Task<bool> createTask(ProjectTask newTask)
        {
            var client = new HttpClient();
            string jsonProject = JsonConvert.SerializeObject(newTask);
            var response = await client.PostAsync(_baseUrl + "/create", new StringContent(jsonProject, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;
        }

        /// <summary>
        ///     Delete a specific entity
        /// </summary>
        /// <param name="deleteTask"></param>
        /// <returns></returns>
        public async Task<bool> deleteTask(ProjectTask deleteTask)
        {
            //Get the project with the given id
            var client = new HttpClient();
            var response = await client.DeleteAsync($"{_baseUrl}/delete/{deleteTask.PartitionKey}/{deleteTask.RowKey}");

            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;
        }

        /// <summary>
        ///     Get all tasks for a given project
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public async Task<List<ProjectTask>> getAllTasks(string projectName)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}/allTasks/{projectName}");

            if (response.IsSuccessStatusCode)
            {
                var allTasks = JsonConvert.DeserializeObject<List<ProjectTask>>(await response.Content.ReadAsStringAsync());
                return allTasks;
            }
            else
                return null;
        }

        /// <summary>
        ///     Get a specific task entity
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public async Task<ProjectTask> getTask(string rowKey, string partitionKey)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}/{partitionKey}/{rowKey}");

            if (response.IsSuccessStatusCode)
            {
                var task = JsonConvert.DeserializeObject<ProjectTask>(await response.Content.ReadAsStringAsync());
                return task;
            }
            return null;
        }

        /// <summary>
        ///     Update a specific task entity
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="modifiedTask"></param>
        /// <returns></returns>
        public async Task<bool> updateTask(string rowKey, ProjectTask modifiedTask)
        {
            var client = new HttpClient();
            string jsonProject = JsonConvert.SerializeObject(modifiedTask);
            var response = await client.PutAsync($"{_baseUrl}/update/{rowKey}", new StringContent(jsonProject, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;
        }
    }
}