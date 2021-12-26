using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskRestAPI.Models;

namespace TaskProject.Models
{
    public interface IProjectRepository
    {
        Task<List<Project>> getAllProjects();
        Task<Project> getProject(string rowKey, string partitionKey);
        Task<bool> createProject(Project newProject);
        Task<bool> updateProject(string rowKey, Project modifiedProject);
        Task<bool> deleteProject(Project deleteProject);

    }
}
