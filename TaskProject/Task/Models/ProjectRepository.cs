using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TaskRestAPI.Models;

namespace TaskProject.Models
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly string _baseUrl = @"http://akvelon.azurewebsites.net/api/Project";


        /// <summary>
        ///     save the given entitiy to the table storage
        /// </summary>
        /// <param name="newProject"></param>
        /// <returns></returns>
        public async Task<bool> createProject(Project newProject)
        {
            var client = new HttpClient();
            string jsonProject = JsonConvert.SerializeObject(newProject);
            var response = await client.PostAsync(_baseUrl + "/create", new StringContent(jsonProject, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;
        }

        /// <summary>
        ///     Delete an existing entity
        /// </summary>
        /// <param name="deleteProject"></param>
        /// <returns></returns>
        public async Task<bool> deleteProject(Project deleteProject)
        {
            //Get the project with the given id
            var client = new HttpClient();
            var response = await client.DeleteAsync($"{_baseUrl}/delete/{deleteProject.RowKey}/{deleteProject.PartitionKey}");

            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;
        }

        /// <summary>
        ///     Get all projects
        /// </summary>
        /// <returns></returns>
        public async Task<List<Project>> getAllProjects()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(_baseUrl);

            if (response.IsSuccessStatusCode)
            {
                var projects = JsonConvert.DeserializeObject<List<Project>>(await response.Content.ReadAsStringAsync());
                return projects;
            }

            return null;
        }

        /// <summary>
        ///     Get the project with the given row and partition key
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public async Task<Project> getProject(string rowKey, string partitionKey)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}/{partitionKey}/{rowKey}");

            if (response.IsSuccessStatusCode)
            {
                var project = JsonConvert.DeserializeObject<Project>(await response.Content.ReadAsStringAsync());
                return project;
            }

            return null;
        }

        /// <summary>
        ///     Update the existing record with the given project entity
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="modifiedProject"></param>
        /// <returns></returns>
        public async Task<bool> updateProject(string rowKey, Project modifiedProject)
        {
            //Update the project entity using the rest API
            var client = new HttpClient();
            string jsonProject = JsonConvert.SerializeObject(modifiedProject);
            var response = await client.PutAsync($"{_baseUrl}/update/{rowKey}", new StringContent(jsonProject, Encoding.UTF8, "application/json"));

            //if theo peration is successfull go back to index page with all of the projects
            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;
        }

    }
}