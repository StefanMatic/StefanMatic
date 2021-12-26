using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectTask = TaskRestAPI.Models.Task;

namespace TaskProject.Models
{
    public interface ITaskRepository
    {
        Task<List<ProjectTask>> getAllTasks(string projectName);
        Task<ProjectTask> getTask(string rowKey, string partitionKey);
        Task<bool> createTask(ProjectTask newTask);
        Task<bool> updateTask(string rowKey, ProjectTask modifiedTask);
        Task<bool> deleteTask(ProjectTask deleteTask);
    }
}
