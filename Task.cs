using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment6
{
    /// <summary>
    /// Class defining task object with date and time, task description and priority type fields and containing some helper methods related to task object
    /// </summary>
    internal class Task
    {
        //instance variables
        private DateTime dateAndTime;
        private string taskDescription;
        private PriorityType priority;

        private bool editMode = false;


        /// <summary>
        /// Default contsructor
        /// </summary>
        public Task()
        {

        }

        /// <summary>
        /// Constructor taking task fields as parameters 
        /// </summary>
        /// <param name="dateAndTime"></param>
        /// <param name="taskDescription"></param>
        /// <param name="priority"></param>
        public Task(DateTime dateAndTime, string taskDescription, PriorityType priority)
        {
            DateAndTime = dateAndTime;
            TaskDescription = taskDescription;
            Priority = priority;
            DateAndTime = dateAndTime;
            TaskDescription = taskDescription;
            Priority = priority;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="theOther"></param>
        public Task(Task theOther)
        {
            this.DateAndTime = theOther.DateAndTime;
            this.TaskDescription = theOther.TaskDescription;
            this.Priority = theOther.Priority;
        }

        /// <summary>
        /// Property giving read and write access to the date and time variable
        /// </summary>
        public DateTime DateAndTime
        {
            get
            {
                return dateAndTime;
            }
            set
            {
                dateAndTime = value;
            }
        }

        /// <summary>
        /// Property giving read and write access to the to do task variable
        /// </summary>
        public string TaskDescription
        {
            get
            {
                return taskDescription;
            }
            set
            {
                taskDescription = value;
            }
        }

        /// <summary>
        /// Property giving read and write access to the priority variable
        /// </summary>
        public PriorityType Priority
        {
            get
            {
                return priority;
            }
            set
            {
                priority = value;
            }
        }

        /// <summary>
        /// Property giving read and write access into the edit mode variable
        /// </summary>
        /// <remarks>
        /// To flag if in edit mode or not
        /// </remarks>
        public bool EditMode
        {
            get
            {
                return editMode;
            }
            set
            {
                editMode = value;
            }
        }

        /// <summary>
        /// Replace underscores in the priority names with a whitespace for a more human readable format
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public string GetPriorityName(PriorityType priority)
        {
            string priorityName = priority.ToString();
            return priorityName.Replace("_", " ");
        }

        /// <summary>
        /// Check that the input is not empty
        /// </summary>
        /// <returns></returns>
        public bool CheckData()
        {
            if ((dateAndTime != null) && !string.IsNullOrEmpty(taskDescription) && !Priority.Equals(PriorityType.Select_priority))
            {
                return true;
            }
            else
            {
                return false;
            }
        }



    }
}
