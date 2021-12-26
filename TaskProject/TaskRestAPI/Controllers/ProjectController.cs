using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaskRestAPI.Models;

namespace TaskRestAPI.Controllers
{
    [RoutePrefix("api/Project")]
    public class ProjectController : ApiController
    {
        /// <summary>
        ///     Gets all the project entities
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("")]
        public IHttpActionResult GetAll()
        {
            List<Project> allProjects = new Project().GetAll();
            if (allProjects != null)
                return Ok(allProjects);
            else
                return NotFound();
        }

        /// <summary>
        ///     Get the project with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            Project myProject = new Project().Get(id);
            if (myProject != null)
                return Ok(myProject);
            else
                return NotFound();
        }

        /// <summary>
        ///     Get a project using the partition key and id
        /// </summary>
        /// <param name="partition"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{partition}/{id}", Name ="DisplayProject")]
        public IHttpActionResult Get(string partition, string id)
        {
            Project myProject = new Project().Get(id, partition);
            if (myProject != null)
                return Ok(myProject);
            else
                return NotFound();
        }       

        /// <summary>
        ///     
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        [HttpPost, Route("create")]
        public IHttpActionResult Post([FromBody]Project project)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            Project newProject = new Project(project.Name, project.Code);
            newProject.Create();

            return Ok(newProject);
        }


        /// <summary>
        ///     Update the project with the newProject object
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newProject"></param>
        /// <returns></returns>
        [HttpPut, Route("update/{id}")]
        public IHttpActionResult Put(string id, [FromBody] Project newProject)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            //Used to cut down hacker attacks in the situation stat they want to access it through URL
            if (id != newProject.RowKey)
                return BadRequest();

            //Check if the new project even exists
            if (!newProject.Exists(newProject.RowKey, newProject.PartitionKey))
                return NotFound();

            Project updatedProject = newProject.Get(newProject.RowKey, newProject.PartitionKey);
            updatedProject.Update(newProject);

            //Returning the newly made project and the status OK
            return Ok(newProject);
        }

        /// <summary>
        ///     Delete the project using row and partition key
        /// </summary>
        /// <param name="RowKey"></param>
        /// <param name="PartitionKey"></param>
        /// <returns></returns>
        [HttpDelete, Route("delete/{RowKey}/{PartitionKey}")]
        public IHttpActionResult Delete(string RowKey, string PartitionKey)
        {
            Project projectToDelete = new Project().Get(RowKey, PartitionKey);
            if (projectToDelete == null)
                return NotFound();

            projectToDelete.DeleteRow();

            return Ok(projectToDelete);
        }

        /// <summary>
        ///     Delete project using id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deleteProject"></param>
        /// <returns></returns>
        [HttpDelete, Route("delete/{id}")]
        public IHttpActionResult Delete(string id, [FromBody] Project deleteProject)
        {
            if (id != deleteProject.Id)
                return BadRequest();

            deleteProject.DeleteRow();

            return Ok(deleteProject);
        }
    }
}
