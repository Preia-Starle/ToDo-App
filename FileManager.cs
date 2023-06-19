using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assignment6
{
    /// <summary>
    /// Class performing operations related to file management like saving file, opening file or getting information from a file
    /// </summary>
    internal class FileManager
    {
        //hardcoded file version token for validation purposes
        private const string fileVersionToken = "ToDoRe_21";
        //hardcoded file version number for validation purposes
        private const double fileVersionNr = 1.0;

        /// <summary>
        /// Save taskList content into a hardcoded textFile
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="title"></param>
        /// <param name="initialDirectory"></param>
        /// <param name="writeData"></param>
        public bool SaveTaskListToFile(List<Task> taskList, string fileName)
        {
            bool ok = true;
            //initialise the StreamWriter object
            StreamWriter writer = null;

            try
            {
                //write first the fileName, fileVersionToken and Number and taskList.count
                writer = new StreamWriter(fileName);
                writer.WriteLine(fileVersionToken);
                writer.WriteLine(fileVersionNr);
                writer.WriteLine(taskList.Count);

                //then loop through the taskList and write each item and it's attributes
                for (int i = 0; i < taskList.Count; i++)
                {
                    writer.WriteLine(taskList[i].TaskDescription);
                    writer.WriteLine(taskList[i].Priority.ToString());
                    writer.WriteLine(taskList[i].DateAndTime.Year);
                    writer.WriteLine(taskList[i].DateAndTime.Month);
                    writer.WriteLine(taskList[i].DateAndTime.Day);
                    writer.WriteLine(taskList[i].DateAndTime.Hour);
                    writer.WriteLine(taskList[i].DateAndTime.Minute);
                    writer.WriteLine(taskList[i].DateAndTime.Second);

                }
            }
            catch
            {
                ok = false; //if error return false
            }
            finally //execute regardless catch try outcome
            {
                if (writer != null)
                    writer.Close();
            }
            return ok;

        }

        /// <summary>
        /// Open previously saved file and display in the listbox
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool OpenDataFile(List<Task> taskList, string fileName)
        {
            bool ok = true;
            //initialise the StreamReader object
            StreamReader reader = null;

            try
            {
                //if there are items in the listBox
                if (taskList != null)
                {
                    //clear the listBox so it is ready for a new content loaded from the saved file
                    taskList.Clear();
                }

                else
                {
                    //create a new taskList
                    taskList = new List<Task>();
                }

                //set the reader so it reads the hardcoded file's content
                reader = new StreamReader(fileName);

                //read the version
                string versionTest = reader.ReadLine();
                double version = double.Parse(reader.ReadLine());

                //check if the fileVersionToken and Number match with assigned values so we get the correct file
                if ((versionTest == fileVersionToken) && (version == fileVersionNr))
                {
                    //count number of items 
                    int count = int.Parse(reader.ReadLine());

                    //loop through all items and read all attributes for each line
                    for (int i = 0; i < count; i++)
                    {
                        Task task = new Task();
                        task.TaskDescription = reader.ReadLine();
                        task.Priority = (PriorityType)Enum.Parse(typeof(PriorityType), reader.ReadLine());

                        int year = 0, month = 0, day = 0;
                        int hour = 0, minute = 0, second = 0;

                        year = int.Parse(reader.ReadLine());
                        month = int.Parse(reader.ReadLine());
                        day = int.Parse(reader.ReadLine());
                        hour = int.Parse(reader.ReadLine());
                        minute = int.Parse(reader.ReadLine());
                        second = int.Parse(reader.ReadLine());

                        task.DateAndTime = new DateTime(year, month, day, hour, minute, second);

                        taskList.Add(task); //add each read task to the listBox to be displayed on GUI

                    }

                }
                else
                {
                    ok = false; //throw error if problem
                }

            }
            catch //if error ok = false
            {
                ok = false;
            }
            finally //execute regardless the try catch outcome
            {
                if (reader != null)
                    reader.Close();
            }
            return ok;
        }

        /// <summary>
        /// Save data file method taking filter(path to the file), title of the file and the data to be saved as parameters
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="title"></param>
        /// <param name="initialDirectory"></param>
        /// <param name="writeData"></param>
        public void SaveDataFile(string filter, string title, string dataToWrite)
        {
            //1.create a SaveDialog object
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            //2a.filter through the files 
            saveFileDialog1.Filter = filter;

            saveFileDialog1.Title = title;

            //2b.restore what's been previously worked on after the dialog is closed
            saveFileDialog1.RestoreDirectory = true;

            //3.show the SaveFileDialog and if result OK
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //get the file path and name
                string filePath = saveFileDialog1.FileName;

                //write the tasks to the selected file
                //create a new StreamWriter class object, constructor takes two arguments: 1.name of the file, 2. bool value indicating whether to append(true)to the file or overwrite(false)
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    writer.Write(dataToWrite);
                }

            }

        }

        /// <summary>
        /// Open selected file method taking filter(file path) and title as parameters
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public string OpenDataFile(string filter, string title)
        {
            //create an OpenDialog object
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //filter through the files and search
            openFileDialog1.Filter = filter;

            //set the title
            openFileDialog1.Title = title;

            //if result OK
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //return the file
                return openFileDialog1.FileName;
            }

            return null;
        }

        /// <summary>
        /// Gets desired info from the assembly info file (takes filePath and the desired information to be extracted as parameters)
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetAssemblyInfo(string filePath, string key)
        {
            //read the file located by filePath
            using (StreamReader reader = new StreamReader(filePath))
            {
                //call ReadToEnd method to read all
                string assemblyInfo = reader.ReadToEnd();

                //user regular expression to extract desired information (type of information passed in a parameter of the method)
                Match match = Regex.Match(assemblyInfo, $@"\[assembly: Assembly{key}\(""(.+?)""\)\]");

                if (match.Success) //if found
                {

                    //return the captured value of the second group - [1] (passed as a second argument) in the regex pattern (meaning the selected information to be extracted)
                    return match.Groups[1].Value;

                }
                return string.Empty;
            }
        }
    }
}
