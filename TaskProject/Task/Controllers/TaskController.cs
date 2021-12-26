using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TaskRestAPI.Controllers;
using TaskProject.Models;
using ProjectTask = TaskRestAPI.Models.Task;

namespace TaskProject.Controllers
{
    public class TaskController : Controller
    {
        private ITaskRepository _taskRepository;

        public TaskController()
        {
            //init the necessary reporitory
            _taskRepository = new TaskRepository();
        }

        /// <summary>
        ///     Get all tasks for a project
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Index(string projectName)
        {
            var allTasks = await _taskRepository.getAllTasks(projectName);
            if (allTasks != null)
            {
                ViewBag.ProjectName = projectName;
                return View(allTasks);
            }
            else
                return HttpNotFound();
        }

        /// <summary>
        ///     Get details of the task
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public async Task<ActionResult> Details(string rowKey, string partitionKey)
        {
            //Get the project with the given id
            var task = await _taskRepository.getTask(rowKey, partitionKey);
            if (task != null)
                return View(task);
            else
                //Return to the window where we can see all of the tasks
                return RedirectToAction("Index", new { projectName = partitionKey });
        }

        // GET: Project/Create
        //When implementing the Create method using the PRG pattern, we make two Create methods. The first one has an empty signiture(gives the user the 
        //view where they can give all the necessary records to create an entity)
        public ActionResult Create(string projectName)
        {
            //Moramo da prosledimo Project name
            ViewBag.ProjectName = projectName;
            return View();
        }

        /// <summary>
        ///     Create a new task
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        //The antiFrogerytoken is made by the form and checked on the server side because of hacker attacks.Every httpPost action should use it
        public async Task<ActionResult> Create(FormCollection collection)
        {
            try
            {
                ProjectTask newTask = new ProjectTask(collection["PartitionKey"], collection["Name"], collection["Description"]);

                bool createFlag = await _taskRepository.createTask(newTask);

                if (createFlag)
                    return RedirectToAction("Index", new { projectName = newTask.PartitionKey });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $@"Unable to create record: {ex.Message}");
                return View();
            }

            return RedirectToAction("Index", new { projectName = collection["ProjectName"] });
        }


        /// <summary>
        ///     Edit a taks identified with the row and partition key
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Edit(string rowKey, string partitionKey)
        {
            var task = await _taskRepository.getTask(rowKey, partitionKey);

            if (task != null)
                return View(task);
            else
                return RedirectToAction("Index", new { projectName = partitionKey });
        }


        /// <summary>
        ///     Make the modifications given by the user
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partitionKey"></param>
        /// <param name="modifiedTask"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string rowKey, string partitionKey, ProjectTask modifiedTask)
        {
            try
            {
                // Check if the model is valid
                if (!ModelState.IsValid)
                    return View(modifiedTask);

                bool deleteFlag = await _taskRepository.updateTask(rowKey, modifiedTask);
                if (deleteFlag)
                    return RedirectToAction("Index", new { projectName = partitionKey });
                else
                    return View(modifiedTask); 
            }
            catch
            {
                return View(modifiedTask);
            }
        }

        /// <summary>
        ///     Delete task identified by the row and partition key
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Delete(string rowKey, string partitionKey)
        {
            var task = await _taskRepository.getTask(rowKey, partitionKey);
            if (task != null)
                return View(task);
            else
                return HttpNotFound();
        }

        /// <summary>
        ///     Delete task
        /// </summary>
        /// <param name="deleteTask"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(ProjectTask deleteTask)
        {
            try
            {
                bool deleteFlag = await _taskRepository.deleteTask(deleteTask);
                if (deleteFlag)
                    return RedirectToAction("Index", new { projectName = deleteTask.PartitionKey });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to create record: {ex.Message}");
            }

            return View();
        }
    }
}
