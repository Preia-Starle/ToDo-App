using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assignment6
{
    /// <summary>
    /// Starting object to provide an interface for inputting and displaying information related to tasks
    /// </summary>
    public partial class MainForm : Form
    {
        //instance variables
        private Task task = new Task();

        private List<Task> tasks = new List<Task>();

        private TaskManager taskManager;

        //default txt file to be used to save and display data(located in the same directory as applications EXE file)
        string fileName = Application.StartupPath + "\\Tasks.txt";

        //instance variables for timer
        private Timer timer;
        private TimeSpan elapsedTime;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            InitializeGUI();

        }

        /// <summary>
        /// Set defaults on form load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text = "ToDo Reminder by Kate Arvay";
            lblTimer.Text = string.Empty;

        }

        /// <summary>
        /// Set defaults for GUI after form loaded
        /// </summary>
        private void InitializeGUI()
        {
            //1.DATETIME PICKER
            //call a method on dateTimePicker control to set up it's properties 
            CreateDateTimePicker(dateTimePicker1);

            //2.TIMER
            //create new timer object
            timer = new Timer();
            //set inerval to 1s
            timer.Interval = 1000;
            //handle elapsed time and display on label
            timer.Tick += Timer_Tick;
            //start the timer
            timer.Start();

            //initialise a list for priorities
            List<string> priorities = new List<string>();

            //loop through the priorities  
            foreach (PriorityType iteratedPriority in Enum.GetValues(typeof(PriorityType)))
            {
                //call a method to replace any underscores with whitespaces
                string priority = task.GetPriorityName(iteratedPriority);

                //add modified priority name to the previously declared list
                priorities.Add(priority);

            }

            //set as a datasource for the priorities combo box so the text appears without underscores
            cmbBoxPriority.DataSource = priorities;
            //display priority on index 0 (Select a priority)
            cmbBoxPriority.SelectedIndex = 0;

            //clear from text
            txtTaskDescription.Text = string.Empty;
            lstToDoList.Items.Clear();

            //call a method to disable change and edit buttons until user actively selects item from listbox
            DisableButtons();

        }

        /// <summary>
        /// Measure elapsed time and display in the label on GUI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            lblTimer.Text = elapsedTime.ToString(@"hh\:mm\:ss");
        }


        /// <summary>
        /// Update listbox GUI based on the current content of the task list object
        /// </summary>
        private void UpdateGUI()
        {
            //refresh
            lstToDoList.Items.Clear();

            //loop through the task list and add the items to the listbox
            foreach (Task iteratedTask in taskManager.GetAllTasks())
            {
                if (iteratedTask != null)
                {
                    //get the values and format
                    string date = iteratedTask.DateAndTime.ToString("dd-MMM-yyyy");
                    string time = iteratedTask.DateAndTime.ToString("hh:mm tt");
                    string priority = iteratedTask.GetPriorityName(iteratedTask.Priority);
                    string taskDescription = iteratedTask.TaskDescription;
                    string displayString = string.Format("{0, -25} {1, -25} {2, -30} {3, -30}", date, time, priority, taskDescription); //display in columns

                    //display on the GUI
                    lstToDoList.Items.Add(displayString);
                }

            }
            //set defaults for buttons until the item from the listbox is selected
            DisableButtons();

        }

        /// <summary>
        /// Set the dateTimePicker properties
        /// </summary>
        /// <param name="dateTimePicker"></param>
        public void CreateDateTimePicker(DateTimePicker dateTimePicker)
        {
            //set the min and max date
            dateTimePicker1.MinDate = DateTime.Today; //min date is now (task refer always to the future)
            dateTimePicker.MaxDate = new DateTime(2050, 12, 31); //max date

            //set the custom format
            dateTimePicker1.CustomFormat = "yyyy-MM-dd  HH:mm";
            dateTimePicker1.Format = DateTimePickerFormat.Custom;

            //display the control as an up-down control
            dateTimePicker1.ShowUpDown = true;

        }

        /// <summary>
        /// Add the new task to the task list and text file and display on the GUIs listbox on Add button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            //1.get the values from the input boxes
            DateTime dateTime = dateTimePicker1.Value;
            //set back underscores so the priority type matches with the enum content
            PriorityType taskPriority = (PriorityType)Enum.Parse(typeof(PriorityType), cmbBoxPriority.SelectedItem.ToString().Replace(" ", "_"), true);
            string taskDescription = txtTaskDescription.Text;


            //2.create new task object with the input details
            Task newTask = new Task(dateTime, taskDescription, taskPriority);

            //3.validate input 
            if (newTask.CheckData())
            {
                //4.add new task to the list of tasks
                tasks.Add(newTask);

                //5.create a new task manager object and pass the list
                taskManager = new TaskManager(tasks);

                //6.update the listbox
                UpdateGUI();

                //7.clear input fields to defaults
                SetUpInputFieldsToDeafults();

            }
            else
            {
                MessageBox.Show("Please provide input for all fields to save the item.");
            }

        }

        /// <summary>
        /// Reset input fields to defaults
        /// </summary>
        private void SetUpInputFieldsToDeafults()
        {
            txtTaskDescription.Clear();
            cmbBoxPriority.SelectedIndex = 0;
            dateTimePicker1.Value = DateTime.Now;
        }

        /// <summary>
        /// Check input fields and return true/false if not empty
        /// </summary>
        /// <returns></returns>
        private bool AreInputFieldsNotEmpty()
        {
            return !string.IsNullOrEmpty(txtTaskDescription.Text) && !string.IsNullOrEmpty(cmbBoxPriority.Text);
        }

        /// <summary>
        /// Reset the program as at start-up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //reset the GUI to defaults
            InitializeGUI();
            //recreate the task manager
            taskManager = new TaskManager();
            //call UpdateGUI() to iterate through the task list (so to make sure it is actually empty and recreated a new)
            UpdateGUI();
        }

        /// <summary>
        /// Open the selected text file with data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openDataFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //check if taskManager not empty
            if (taskManager == null) //if empty
            {
                //display error message
                MessageBox.Show("There are no saved tasks. First provide tasks and save them to be able to display them in the listbox.", "Error");

            }
            else //if not empty proceed with reading the data
            {
                string errMessage = "Something went wrong when opening the file.";

                //call method to read file data and saved it's return value to bool variable ok
                bool ok = taskManager.ReadDataFromFile(fileName);

                //inform user that this action will overwrite the current listbox content
                this.DialogResult = MessageBox.Show("This action will overwrite the current content of the task list. Are you sure you want to continue?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (this.DialogResult == DialogResult.Yes) //if yes selected
                {
                    //check ok value
                    if (!ok) //if ok false
                    {
                        MessageBox.Show(errMessage); //display error message
                    }
                    else
                    {
                        UpdateGUI(); //updateGUI

                    }

                }
                else
                {
                    this.DialogResult = DialogResult.None; //otherwise do nothing
                }
            }

        }

        /// <summary>
        /// Save items from listbox to a hardcoded txt file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveDataFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //check if listbox not empty
            if (lstToDoList.Items.Count == 0) //if empty
            {
                MessageBox.Show("There are no tasks in the list. Please add tasks to save them.", "Error"); //display error message
            }
            else //otherwise proceed with saving
            {
                string errMessage = "Something went wrong when saving data to file.";

                //call the method to save data to file and check if it executed correctly by assigning its return value to a bool variable
                bool ok = taskManager.WriteDataToFile(fileName);

                //check the value of ok
                if (!ok) //if false
                {
                    MessageBox.Show(errMessage); //display error message
                }
                else //if true
                {
                    MessageBox.Show("Data saved to file:" + Environment.NewLine + fileName); //show user where the file was saved
                }
            }
        }

        /// <summary>
        /// Show a messagebox to user to confirm exit, close application if yes, do nothing if no clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //show message box with yes/no options
            this.DialogResult = MessageBox.Show("Are you sure you want to exit the application?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (this.DialogResult == DialogResult.Yes) //if yes selected
            {
                this.Close(); //this form close
            }
            else
            {
                this.DialogResult = DialogResult.None; //otherwise remain open
            }
        }

        /// <summary>
        /// Change the selected task - replace with a new input on Change button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChange_Click(object sender, EventArgs e)
        {
            //validate index
            if (AreInputFieldsNotEmpty())
            {
                int selectedTaskIndex = lstToDoList.SelectedIndex;

                if (selectedTaskIndex != -1)
                {
                    //get the task on selected index
                    Task selectedTask = taskManager.GetTask(selectedTaskIndex);
                    //copy for editing
                    Task taskCopy = new Task(selectedTask);

                    //get the values from input boxes
                    DateTime dateTime = dateTimePicker1.Value;
                    PriorityType taskPriority = (PriorityType)Enum.Parse(typeof(PriorityType), cmbBoxPriority.SelectedItem.ToString().Replace(" ", "_"), true);
                    string taskDescription = txtTaskDescription.Text;

                    taskCopy = new Task(dateTime, taskDescription, taskPriority);

                    //validate if input fields not empty
                    if (taskCopy.CheckData())
                    {
                        //replace the task
                        taskManager.ChangeTask(taskCopy, selectedTaskIndex);
                        //update GUI
                        UpdateGUI();
                        //clear the input fields
                        SetUpInputFieldsToDeafults();
                        //enable adding
                        btnAdd.Enabled = true;
                        //disable change and delete buttons
                        DisableButtons();


                    }
                    else
                    {
                        MessageBox.Show("Please provide input for all fields to save the item.");
                    }


                }

            }
        }

        /// <summary>
        /// Copy selected item values inside the input boxes to be edited and enable change and delete buttons to allow for changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstToDoList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //enable change and delete options once user selects an item
            EnableButtons();
            //disable add button once that happens(user must unselect to be able to cancel editing, clear fields and make a fresh add)
            btnAdd.Enabled = false;

            //get the selected index
            int selectedIndex = (int)lstToDoList.SelectedIndex;
            //validate index (make sure the click is not just inside the listbox but actual item is selected)
            if (selectedIndex >= 0)
            {
                if (task.EditMode == true && AreInputFieldsNotEmpty())
                {
                    //the user clicked on a different task while editing
                    //in that case cancel editing and show details of the clicked task instead
                    SetUpInputFieldsToDeafults();
                    task.EditMode = false;
                }
                //validate
                if (selectedIndex != -1)
                {
                    //get selected task
                    Task selectedTask = taskManager.GetTask(selectedIndex);

                    //copy values of selected item into the inout fields to be edited
                    dateTimePicker1.Value = selectedTask.DateAndTime;
                    cmbBoxPriority.SelectedItem = selectedTask.GetPriorityName(selectedTask.Priority);
                    txtTaskDescription.Text = selectedTask.TaskDescription;

                }

            }

        }

        /// <summary>
        /// Enable change and delete buttons
        /// </summary>
        private void EnableButtons()
        {
            btnChange.Enabled = true;
            btnDelete.Enabled = true;
        }

        /// <summary>
        /// Disable change and delete buttons
        /// </summary>
        private void DisableButtons()
        {
            btnChange.Enabled = false;
            btnDelete.Enabled = false;
        }

        /// <summary>
        /// Delete the selected task after user confirms in MessageBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            //get the selected index
            int selectedIndex = (int)lstToDoList.SelectedIndex;

            //validate index (make sure the click is not just inside the listbox but actual item is selected)
            if (selectedIndex >= 0)
            {
                //show message box with yes/no options
                this.DialogResult = MessageBox.Show("Are you sure you want to delete this item?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (this.DialogResult == DialogResult.Yes) //if yes selected
                {
                    taskManager.DeleteTask(selectedIndex); //delete task
                    SetUpInputFieldsToDeafults();
                    UpdateGUI(); //refresh GUI
                    btnAdd.Enabled = true;
                }
                else
                {
                    this.DialogResult = DialogResult.None; //otherwise do nothing
                }

            }
        }

        /// <summary>
        /// Display tooltip for DateTime picker on mouse hover event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePicker1_MouseHover(object sender, EventArgs e)
        {
            dateTimeToolTip = new ToolTip();
            dateTimeToolTip.SetToolTip(dateTimePicker1, "Format is yyyy-MM-dd  HH:mm. Set up by clicking on the day, month, year or hour and press arrows up or down or write numbers dierctly.");

        }

        /// <summary>
        /// Open a Message Box after click on About item in Menu strip and display information about the application with an image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //get path to the assembly
            string assemblyInfoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Properties", "AssemblyInfo.cs");
            //initiate an instance of the fileManager object
            FileManager fileManager = new FileManager();
            //set the box title
            string boxTitle = "About";
            //get the desired info from the assembly
            string title = fileManager.GetAssemblyInfo(assemblyInfoPath, "Title");
            string description = fileManager.GetAssemblyInfo(assemblyInfoPath, "Description");
            string version = fileManager.GetAssemblyInfo(assemblyInfoPath, "Version");
            //get the path to the image
            string filePath = Application.StartupPath + "\\wallpaperflare.com_wallpaper.jpg";
            //create a new image object and assign the value of the selected image
            Image image = Image.FromFile(filePath);
            //set the text to be displayed
            string labelText = $"Title: {title}\n\nDescription: {description} \n\nVersion: {version}";

            //set the icon
            string iconFilePath = Application.StartupPath + "\\list_todo_icon_177852.ico";
            Icon icon = new Icon(iconFilePath);

            //call a method to create a custom message box and fill it with selected title, text and image
            CreateCustomMessageBox(boxTitle, labelText, image, icon);

        }

        /// <summary>
        /// Print the content of the listbox on Print click in the Menu Strip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //check if taskManager empty
            if (lstToDoList.Items.Count == 0)
            {
                MessageBox.Show("You have no tasks to print. Please add some tasks to the task list first.", "Error");
            }
            else
            {
                //call method to print listbox content
                PrintListBoxContent(lstToDoList);
            }

        }

        /// <summary>
        /// Create a custom box to display text and picture
        /// </summary>
        /// <param name="text"></param>
        /// <param name="image"></param>
        public void CreateCustomMessageBox(string title, string text, Image image, Icon icon)
        {
            //create a new form to display image and text
            Form customMessageBox = new Form();
            //set the icon
            customMessageBox.Icon = icon;
            //set the title of the box
            customMessageBox.Text = title;
            //set the size
            customMessageBox.ClientSize = new Size(400, 200);

            //create a new control to display the image
            PictureBox pictureBox = new PictureBox();
            //set pictureBox attributes
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Image = image;
            pictureBox.Top = 0;
            pictureBox.Left = 0;
            pictureBox.Size = new Size(150, 150);
            //add the picturebox control to the form
            customMessageBox.Controls.Add(pictureBox);

            //create new label control to display text
            Label label = new Label();
            //set label attributes
            label.AutoSize = true;
            label.Text = text;
            label.Left = pictureBox.Right + 10;
            label.Top = pictureBox.Top + 10;
            label.MaximumSize = new Size(customMessageBox.ClientSize.Width - pictureBox.Right - 20, 0);
            //add the label control to the form
            customMessageBox.Controls.Add(label);

            customMessageBox.ShowDialog();

        }

        /// <summary>
        /// Print listbox content with selected printing method
        /// </summary>
        /// <param name="listBoxToPrint"></param>
        public void PrintListBoxContent(ListBox listBoxToPrint)
        {
            //create instance of the PrintDocument class
            PrintDocument printDocument = new PrintDocument();

            //set up the print dialog
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument;

            //show the print dialog to allow user to select a printer
            if (printDialog.ShowDialog() == DialogResult.OK) //if OK
            {
                //handle the PrintDocuments PrintPage event
                printDocument.PrintPage += (s, ev) =>
                {
                    //set up the font and brush to draw text
                    Font font = new Font("Arial", 12);
                    SolidBrush brush = new SolidBrush(Color.Black);

                    //set up the initial position of drawing
                    int x = ev.MarginBounds.Left;
                    int y = ev.MarginBounds.Top;

                    //loop through the lstbox content and draw on to the page
                    for (int i = 0; i < listBoxToPrint.Items.Count; i++)
                    {
                        //get the current item and draw onto page
                        string itemText = listBoxToPrint.GetItemText(listBoxToPrint.Items[i]);
                        ev.Graphics.DrawString(itemText, font, brush, x, y); //draw each item onto the page
                        y += 20; //advance the position of next item to draw so it appears below the first one
                    }

                };

                printDocument.Print(); //print document
            }
        }

        /// <summary>
        /// Unselect selected if clicked somewhere else than the listbox item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstToDoList_MouseUp(object sender, MouseEventArgs e)
        {
            int index = lstToDoList.IndexFromPoint(e.Location); //return coordinates of the mouse pointer

            if (index == ListBox.NoMatches) //if mouse clicked outside any list item
            {
                lstToDoList.ClearSelected(); //unselect selected
                task.EditMode = false; //cancel editing
                btnAdd.Enabled = true; //enable adding
                SetUpInputFieldsToDeafults(); //set up input fields to defaults = empty for input
                DisableButtons(); //disable change an delete buttons
            }
        }

        /// <summary>
        /// Save items from a listbox to a selected file in user's computer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //initiate an instance of the fileManager object
            FileManager fileManager = new FileManager();

            //initiate variables to be passed to SaveDataFile method
            string dataToWrite = string.Empty; //initiate a variable that will be filled with tasks
            string filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"; //filter to find .txt files
            string title = "Save task as"; //declare a title variable

            if (lstToDoList.Items.Count != 0)
            {
                foreach (Task newTask in taskManager.GetAllTasks())
                {
                    //convert to a human readable format and save into the dataToWrite variable
                    dataToWrite += ($"Date:{newTask.DateAndTime.ToString("dd-MMM-yyyy")}, Time: {newTask.DateAndTime.ToString("HH:mm")}, Priority: {newTask.GetPriorityName(newTask.Priority)}, Task Description: {newTask.TaskDescription}\n");
                }

                fileManager.SaveDataFile(filter, title, dataToWrite);
            }
            else
            {
                MessageBox.Show("Cannot save empty task list. Please add tasks.", "Error");
            }

        }

        /// <summary>
        /// Open selected file from folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //initiate an instance of the fileManager object
            FileManager fileManager = new FileManager();
            //filter through the files and search for txt extension
            string filter = "Text files (* .txt)|*.txt|All files (*.*)|*.*";
            //set the title of the dialog
            string title = "Open tasks file";
            //get file path
            string filePath = fileManager.OpenDataFile(filter, title);

            if (!string.IsNullOrEmpty(filePath))
            {
                //open the file usingthe default associated application
                System.Diagnostics.Process.Start(filePath);
            }

        }
    }
}
