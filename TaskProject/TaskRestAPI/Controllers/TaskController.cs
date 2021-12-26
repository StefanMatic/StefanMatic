using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaskRestAPI.Models;

namespace TaskRestAPI.Controllers
{   
    [RoutePrefix("api/Task")]
    public class TaskController : ApiController
    {
        /// <summary>
        ///     Getting all of the tasks for a specific project
        /// </summary>
        /// <param name="partitionKey">Project name</param>
        /// <returns></returns>
        [HttpGet, Route("allTasks/{partitionKey}")]
        public IHttpActionResult GetAll(string partitionKey)
        {
            List<Task> allTasks = new Task().GetAll(partitionKey);
            if (allTasks != null)
                return Ok(allTasks);
            else
                return NotFound();
        }

        /// <summary>
        ///     Get Task only by RowKey. This is not as effective as it would be if we were to use both the partition and row key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            Task myTask = new Task().Get(id);
            if (myTask != null)
                return Ok(myTask);
            else
                return NotFound();
        }

        /// <summary>
        ///     Get Task using the partition and row key
        /// </summary>
        /// <param name="partition"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("{partition}/{id}", Name = "DisplayTask")]
        public IHttpActionResult Get(string partition, string id)
        {
            Task myTask = new Task().Get(id, partition);
            if (myTask != null)
                return Ok(myTask);
            else
                return NotFound();
        }


        /// <summary>
        ///     Creating a new task
        /// </summary>
        /// <param name="newTask"></param>
        /// <returns></returns>
        [HttpPost, Route("create")]
        public IHttpActionResult Post([FromBody]Task newTask)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            newTask.Create();
            return Ok(newTask);
        }


        /// <summary>
        ///     Modifing an already existing task
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifiedTask"></param>
        /// <returns></returns>
        [HttpPut, Route("update/{id}")]
        public IHttpActionResult Put(string id, [FromBody] Task modifiedTask)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            //Used to cut down hacker attacks in the situation stat they want to access it through URL
            if (id != modifiedTask.Id)
                return BadRequest();

            //Check if the new task even exists
            if (!modifiedTask.Exists(modifiedTask.RowKey, modifiedTask.PartitionKey))
                return NotFound();

            Task oldTask = new Task().Get(modifiedTask.RowKey, modifiedTask.PartitionKey);
            oldTask.Update(modifiedTask);

            //Returning the newly made task and the status OK
            return Ok(modifiedTask);
        }

        /// <summary>
        ///     Deleting a task using row and partition key
        /// </summary>
        /// <param name="RowKey"></param>
        /// <param name="PartitionKey"></param>
        /// <returns></returns>
        [HttpDelete, Route("delete/{PartitionKey}/{RowKey}")]
        public IHttpActionResult Delete(string partitionKey, string rowKey)
        {
            Task taskToDelete = new Task().Get(rowKey, partitionKey);
            if (taskToDelete == null)
                return NotFound();

            taskToDelete.DeleteRow();

            return Ok(taskToDelete);
        }

        /// <summary>
        ///     Deleting a task with the proper checking if id matches the id of the task to be deleted
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deleteTask"></param>
        /// <returns></returns>
        [HttpDelete, Route("delete/{id}")]
        public IHttpActionResult Delete(string id, [FromBody] Task deleteTask)
        {
            if (id != deleteTask.Id)
                return BadRequest();

            deleteTask.DeleteRow();

            return Ok(deleteTask);
        }
    }
}

