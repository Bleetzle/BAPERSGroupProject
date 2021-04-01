using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bapers.GUI.technician
{
    /// <summary>
    /// Interaction logic for technicianPortal.xaml
    /// </summary>
    public partial class technicianPortal : Window 
    {
        DatabaseConnector db = new DatabaseConnector();
        string selectedJob = "";
        string selectedTaskID = "";
        string selectedTaskTime = "";
        DataView data;

        public technicianPortal()
        {
            InitializeComponent();
            PopulateJobs();
            data = (DataView)jobsGrid.ItemsSource;

        }


        private async void PopulateJobs()
        {
            //shows all jobs onto the datagrid that the technician needs to complete
            await db.Select(jobsGrid,
                "SELECT DISTINCT(job_Number), job_priority, deadline, special_instructions " +
                "FROM job, job_tasks " +
                "WHERE job_number = Jobjob_number " +
                "AND Staffstaff_ID = @val0"
                , myVariables.num);
        }
        private async void PopulateTasks()
        {
            if ((bool)showComplete.IsChecked)
            {
                //gets all the current tasks
                await db.Select(tasksGrid,
                    "SELECT Taskstask_ID, task_description, start_time, time_taken " +
                    "FROM Job_Tasks, Tasks, Job " +
                    "WHERE Taskstask_ID = task_id " +
                    "AND job_number = @val0 " +
                    "AND Jobjob_number = job_number " +
                    "AND job_status != \"Archived\"" +
                    "AND Staffstaff_ID = @val1 "
                    , selectedJob, myVariables.num);
            }
            else
            {
                await db.Select(tasksGrid,
                    "SELECT Taskstask_ID, task_description, start_time, time_taken " +
                    "FROM Job_Tasks, Tasks, Job " +
                    "WHERE Taskstask_ID = task_id " +
                    "AND job_number = @val0 " +
                    "AND Jobjob_number = job_number " +
                    "AND job_status != \"Archived\" " +
                    "AND Staffstaff_ID = @val1 " +
                    "AND time_taken is null ; "
                    , selectedJob, myVariables.num);
            }
        }

        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private void query_click(object sender, RoutedEventArgs e)
        {
            //takes techincian to the query tab
            queries queriesWindow = new queries();
            queriesWindow.Show();
            this.Close();

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //requery
        }

        private async void start_Click(object sender, RoutedEventArgs e)
        {
            if (!selectedTaskTime.Equals(""))
            {
                MessageBoxResult confirmResult = MessageBox.Show("Are you sure you want to override the start time?", "Confirm Overide", MessageBoxButton.YesNo);
                if (confirmResult == MessageBoxResult.No)
                    return;
            }  
            //sets the start time and sets the time taken to zero
            await db.InQuery("UPDATE Job_Tasks SET start_time = @val0 WHERE Jobjob_number = @val1 AND Taskstask_ID = @val2;", DateTime.Now.TimeOfDay, selectedJob, selectedTaskID);
            await db.InQuery("UPDATE Job_Tasks SET time_taken = NULL WHERE Jobjob_number = @val0 AND Taskstask_ID = @val1;", selectedJob, selectedTaskID);

            PopulateTasks();
            
            
        }

        private async void complete_Click(object sender, RoutedEventArgs e)
        {
            //checks the task was started
            if (selectedTaskTime.Equals(""))
            {
                MessageBox.Show("Please begin the task before setting to complete");
                return;
            }
            //sets the start time

            TimeSpan spent = DateTime.Now - DateTime.Parse(selectedTaskTime);
            await db.InQuery("UPDATE Job_Tasks SET time_taken = @val0 WHERE Jobjob_number = @val1 AND Taskstask_ID = @val2;", (int)spent.TotalMinutes, selectedJob, selectedTaskID);
            
            //check if the job is complete after the completion of the job
            if (!await db.Check("SELECT * FROM job_tasks WHERE Jobjob_number = @val0 AND time_taken is null;", selectedJob))
            {
                //finds data, then nothing else:
                //task was the last of that job so therfore the job is now complete...
                await db.InQuery("UPDATE Job SET job_status = \"Completed\" WHERE job_number = @val0;", selectedJob);
                await db.InQuery("UPDATE Job SET job_completed = @val0 WHERE job_number = @val1;", DateTime.Now, selectedJob);
            }
            PopulateTasks();

        }

        private void onJobChange(object sender, SelectedCellsChangedEventArgs e)
        {
            foreach (var item in e.AddedCells)
            {
                if (item.Column != null)
                {
                    string col = item.Column.Header.ToString();
                    //tmp job num to be able to create the map value

                    //assuming job num always appears before the price
                    if (col.Equals("job_Number") && item.Column.GetCellContent(item.Item) != null)
                    {
                        selectedJob = (item.Column.GetCellContent(item.Item) as TextBlock).Text;
                        PopulateTasks();
                    }
                }
            }
        }

        private void onTaskChange(object sender, SelectedCellsChangedEventArgs e)
        {
            foreach (var item in e.AddedCells)
            {
                if (item.Column != null)
                {
                    string col = item.Column.Header.ToString();
                    //tmp job num to be able to create the map value

                    //assuming id always appears before the price
                    if (col.Equals("Taskstask_ID") && item.Column.GetCellContent(item.Item) != null)
                    {
                        selectedTaskID = (item.Column.GetCellContent(item.Item) as TextBlock).Text;
                    }
                    if (col.Equals("start_time") && item.Column.GetCellContent(item.Item) != null)
                    {
                        selectedTaskTime = (item.Column.GetCellContent(item.Item) as TextBlock).Text;
                    }
                }
            }
        }

        private void searchChanged(object sender, TextChangedEventArgs e)
        {
            //for searchbox
            if (data != null)
            {
                if (!searchbox.Text.Equals("") && !searchbox.Text.Equals("Search..."))
                {
                    string searchstring = searchbox.Text;
                        data.RowFilter =
                        "job_number LIKE '%" + searchstring.ToString()
                        + "%' OR job_priority LIKE '%" + searchstring.ToString()
                        + "%' OR special_instructions LIKE '%" + searchstring.ToString()
                        + "%'";
                    jobsGrid.ItemsSource = data;
                }
                else
                {
                    PopulateJobs();
                }
            }
            // DISTINCT(job_Number), job_priority, deadline, special_instructions
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PopulateTasks();

        }

        private void jobsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
