using System;
using System.Collections.Generic;
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

namespace Bapers.GUI.shiftManager
{
    /// <summary>
    /// Interaction logic for assign.xaml
    /// </summary>
    public partial class assign : Window
    {
        DatabaseConnector db = new DatabaseConnector();
        string selectedStaffID = "";
        string selectedStaffLoc = "";

        string selectedJobID = "";
        string selectedTaskID = "";

        public assign()
        {
            InitializeComponent();

            Populate();

        }

        private async void Populate()
        {
            await db.Select(userGrid,
                "SELECT * " +
                "FROM Staff " +
                "WHERE role = \"Technician\";");

            await db.Select(tasksGrid,
                "SELECT Jobjob_number, Taskstask_id, Staffstaff_ID " +
                "FROM Job_Tasks " +
                "WHERE Staffstaff_id is null;");
            countTasks();
        }

        private async void countTasks()
        {
            tasksLeft.Text = await db.SelectSingle("SELECT COUNT(Jobjob_number) " +
                "FROM Job_Tasks " +
                "WHERE Staffstaff_id is null; ");
        }
        private async void FilterTasks()
        {
            await db.Select(tasksGrid,
                "SELECT Jobjob_number, Taskstask_id, Staffstaff_ID " +
                "FROM Job_Tasks, tasks " +
                "WHERE Staffstaff_id is null " +
                "AND Taskstask_ID = task_ID " +
                "AND location = @val0; "
                ,selectedStaffLoc);
        }

        private async void assign_Click(object sender, RoutedEventArgs e)
        {
            if(selectedStaffID.Equals("") || selectedTaskID.Equals("") || selectedJobID.Equals(""))
            {
                MessageBox.Show("Please select both a User and a Task");
                return;
            }

            await db.InQuery("UPDATE job_tasks SET Staffstaff_ID = @val0 WHERE Jobjob_number = @val1 AND Taskstask_ID = @val2;",selectedStaffID, selectedJobID, selectedTaskID);
            countTasks();
            FilterTasks();
            selectedJobID = "";
            selectedTaskID = "";
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            shiftManager shiftManagerWindow = new shiftManager();
            shiftManagerWindow.Show();
            this.Close();
        }

        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private void onUsersChange(object sender, SelectedCellsChangedEventArgs e)
        {
            foreach (var item in e.AddedCells)
            {
                if (item.Column != null)
                {
                    string col = item.Column.Header.ToString();
                    //tmp job num to be able to create the map value

                    //assuming id always appears before the price
                    if (col.Equals("staff_ID") && item.Column.GetCellContent(item.Item) != null)
                    {
                        selectedStaffID = (item.Column.GetCellContent(item.Item) as TextBlock).Text;
                       
                    }
                    else if (col.Equals("location") && item.Column.GetCellContent(item.Item) != null)
                    {
                        selectedStaffLoc = (item.Column.GetCellContent(item.Item) as TextBlock).Text;
                    }
                }
            }
            FilterTasks();
        }

        private void onTaskChange(object sender, SelectedCellsChangedEventArgs e)
        {
            foreach (var item in e.AddedCells)
            {
                if (item.Column != null)
                {
                    string col = item.Column.Header.ToString();
                    //tmp job num to be able to create the map value

                    
                    if (col.Equals("Jobjob_number") && item.Column.GetCellContent(item.Item) != null)
                    {
                        selectedJobID = (item.Column.GetCellContent(item.Item) as TextBlock).Text;
                    }
                    else if (col.Equals("Taskstask_id") && item.Column.GetCellContent(item.Item) != null)
                    {
                        selectedTaskID = (item.Column.GetCellContent(item.Item) as TextBlock).Text;
                    }

                }
            }
        }
    }
}
