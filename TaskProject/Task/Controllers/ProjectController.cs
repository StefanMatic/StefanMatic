using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TaskRestAPI.Controllers;
using TaskRestAPI.Models;
using TaskProject.Models;

namespace TaskProject.Controllers
{
    public class ProjectController : Controller
    {
        private IProjectRepository _projectRepository;

        public ProjectController()
        {
            _projectRepository = new ProjectRepository();
        }

        /// <summary>
        ///     List all projects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var projects = await _projectRepository.getAllProjects();
            if (projects != null)
                return View(projects);
            else
                return HttpNotFound();
        }


        /// <summary>
        ///     List all the details of a project
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public async Task<ActionResult> Details(string rowKey, string partitionKey)
        {
            var project = await _projectRepository.getProject(rowKey, partitionKey);
            if (project != null)
                return View(project);
            else
                return RedirectToAction("Index");
        }


        //When implementing the Create method using the PRG pattern, we make two Create methods. The first one has an empty signiture(gives the user the 
        //view where they can give all the necessary records to create an entity)
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //The antiFrogerytoken is made by the form and checked on the server side because of hacker attacks.Every httpPost action should use it
        public async Task<ActionResult> Create(FormCollection collection)
        {
            try
            {
                Project newProject = new Project(collection["Name"], collection["Code"]);

                bool createFlag = await _projectRepository.createProject(newProject);
                if (createFlag)
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $@"Unable to create record: {ex.Message}");
                return View();
            }

            return RedirectToAction("Index");
        }


        /// <summary>
        ///     Edit a selected project
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Edit(string rowKey, string partitionKey)
        {
            var project = await _projectRepository.getProject(rowKey, partitionKey);
            if (project!= null)
                return View(project);
            else
                return RedirectToAction("Index");
        }

        /// <summary>
        ///     Get the results of the modified project
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partitionKey"></param>
        /// <param name="modifiedProject"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string rowKey, string partitionKey, Project modifiedProject)
        {
            try
            {
                // Check if the model is valid
                if (!ModelState.IsValid)
                    return View(modifiedProject);

                bool editFlag = await _projectRepository.updateProject(rowKey, modifiedProject);
                if (editFlag)
                    return RedirectToAction("Index");
                else
                    return View(modifiedProject);
            }
            catch
            {
                return View(modifiedProject);
            }
        }


        /// <summary>
        ///     Delete the project idetified with the row and partitioning key
        /// </summary>
        /// <param name="rowKey"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Delete(string rowKey, string partitionKey)
        {
            var project = await _projectRepository.getProject(rowKey, partitionKey);
            if (project != null)
                return View(project);
            else
                return HttpNotFound();
        }

        /// <summary>
        ///     Delete the givent project
        /// </summary>
        /// <param name="deleteProject"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Project deleteProject)
        {
            try
            {
                bool deleteFlag = await _projectRepository.deleteProject(deleteProject);
                if (deleteFlag)
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to create record: {ex.Message}");
            }

            return View();
        }
    }
}
