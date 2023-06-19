using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment6
{
    /// <summary>
    /// Class defining a list of tasks and performing operations on the list like changing and deleting tasks
    /// </summary>
    internal class TaskManager
    {
        private List<Task> tasks;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TaskManager()
        {
            this.tasks = new List<Task>();
        }

        /// <summary>
        /// Cosntructor taking the task list as a parameter
        /// </summary>
        /// <param name="tasks"></param>
        public TaskManager(List<Task> tasks)
        {
            this.tasks = tasks;
        }

        /// <summary>
        /// Get all tasks
        /// </summary>
        /// <returns></returns>
        public List<Task> GetAllTasks()
        {
            return tasks;
        }

        /// <summary>
        /// Get task on a specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Task GetTask(int index)
        {
            //validate index
            if (CheckIndex(index)) //if true
            {
                return tasks[index];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Check whether index exists = is between 0 and tasks count
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool CheckIndex(int index)
        {
            if ((index >= 0) && (index < tasks.Count)) //if task index between 0 (inclusive)and current number of tasks in the list
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Replace old task with new task
        /// </summary>
        /// <param name="dateAndTime"></param>
        /// <param name="taskDescription"></param>
        /// <param name="priority"></param>
        /// <param name="index"></param>
        public void ChangeTask(Task task, int index)
        {
            //validate index
            if (CheckIndex(index))
            {
                tasks.RemoveAt(index);
                Task newTask = new Task(task);
                tasks.Insert(index, newTask);
            }
        }

        /// <summary>
        /// Delete task at a given index
        /// </summary>
        /// <param name="index"></param>
        public void DeleteTask(int index)
        {
            if (CheckIndex(index))
            {
                tasks.RemoveAt(index);
            }
        }

        /// <summary>
        /// Read data from a specified file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool ReadDataFromFile(string fileName)
        {
            FileManager fileManager = new FileManager();

            return fileManager.OpenDataFile(tasks, fileName);
        }

        /// <summary>
        /// Save data into a specified file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool WriteDataToFile(string fileName)
        {
            FileManager fileManager = new FileManager();

            return fileManager.SaveTaskListToFile(tasks, fileName);
        }


    }
}
