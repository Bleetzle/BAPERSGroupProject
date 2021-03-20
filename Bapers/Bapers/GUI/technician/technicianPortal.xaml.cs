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

namespace Bapers.GUI.technician
{
    /// <summary>
    /// Interaction logic for technicianPortal.xaml
    /// </summary>
    public partial class technicianPortal : Window
    {

        DatabaseConnector db = new DatabaseConnector();
        public technicianPortal()
        {
            InitializeComponent();
        }

        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private async void refresh_click(object sender, RoutedEventArgs e)
        {
            if (jobID_txtBox.Text.Equals("") || taskNumber_txtBox.Text.Equals(""))
            {
                MessageBox.Show("Please fill in all areas");
                return;
            }

            if (await db.Check("" +
                "SELECT * " +
                "FROM job_Tasks " +
                "WHERE Jobjob_number = @val0 " +
                "AND Taskstask_ID = @val1 " +
                ";"
                , jobID_txtBox.Text, taskNumber_txtBox.Text)
                )
            {
                await db.Select(tasksGrid,
                   "SELECT Jobjob_number as JobNumber, Taskstask_ID as TaskID, start_time, NULL AS price " +
                   "FROM Job_Tasks " +
                   "WHERE Jobjob_number = @val0 " +
                   "AND Taskstask_ID = @val1 " +
                   "UNION " +
                   "SELECT task_description, location, task_duration, price " +
                   "FROM Tasks, Job_Tasks " +
                   "WHERE task_id = @val1 " +
                   "AND task_id = Taskstask_ID " +
                   ";"
                   , jobID_txtBox.Text, taskNumber_txtBox.Text);
            }
            else
            {
                MessageBox.Show("The Job ID and task number combination canot be found, please check your information");
            }
            //refreshes the tasks completed
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
        }


        private void complete_Click(object sender, RoutedEventArgs e)
        {
            //when technician completes a task
        }

    }
}
