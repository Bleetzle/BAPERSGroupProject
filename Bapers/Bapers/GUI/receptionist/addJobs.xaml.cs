using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace Bapers.GUI
{
    /// <summary>
    /// Interaction logic for addJobs.xaml
    /// </summary>
    public partial class addJobs : Window
    {
        DatabaseConnector db = new DatabaseConnector();
        List<string> list = new List<string>();

        public addJobs()
        {
            InitializeComponent();
            populate();

            //assuming the page is created after searching
            addJobFor.Text += " " + myVariables.currfname + " " + myVariables.currlname; 
        }

        private async void populate()
        {
            await db.SelectLists(list, "SELECT task_description FROM Tasks;");
            if (list != null)
            {
                foreach (string s in list)
                {
                    CheckBox cb = new CheckBox();
                    cb.Content = s;
                    cb.Width = tasks_dropDown.Width;
                    tasks_dropDown.Items.Add(cb);
                }
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            receptionist receptionistwindow = new receptionist();
            receptionistwindow.Show();
            this.Close();
        }

        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private async void addJob_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedList = new List<string>();

            //get the Selected tasks
            for (int i = 1; i < tasks_dropDown.Items.Count; i++)
            {
                CheckBox cb = (tasks_dropDown.Items[i] as CheckBox);
                if ((bool)cb.IsChecked)
                    selectedList.Add(cb.Content.ToString());
            }

            if (selectedList.Count.Equals(0) || deadline_date.SelectedDate.Value.Equals(null) || time_dropDown.SelectedIndex.Equals(0))
            {
                MessageBox.Show("Please fill in all required fields");
                return;
            }

            //creates the auto incrementing value for the id
            string jobNum = await db.SelectSingle("SELECT MAX(job_number) FROM job;");
            int num = (int.Parse(jobNum.Substring(1))) + 1;

            //format the deadline
            DateTime selectedDate = new DateTime(
                deadline_date.SelectedDate.Value.Year, deadline_date.SelectedDate.Value.Month, deadline_date.SelectedDate.Value.Day
                ,int.Parse(time_dropDown.Text.Substring(0, 2)), 00, 00);

            //timespans to determine the urgency
            double minsToComplete = 0f;
            double timeTillDeadline = 0f;

            foreach(string s in selectedList)
            {
                var val = await db.SelectSingle("SELECT task_duration FROM Tasks WHERE task_description = @val0;", s);
                minsToComplete += int.Parse(val);
            }

            timeTillDeadline = (selectedDate - DateTime.Now).TotalMinutes;

            //determine if job deadline is less than 6 hour
            string urgency = "Normal";
            if (timeTillDeadline - minsToComplete < 360 )
                urgency = "Urgent";

            //create entry in job table
            await db.InQuery(
            "INSERT INTO Job(job_Number, job_priority, deadline, job_status, special_instructions, job_completed, Customeraccount_number, Customerphone_number) " +
            "VALUES(@val0, @val1 , @val2, @val3, @val4, @val5, @val6, @val7);"
            ,"J" + num , urgency, selectedDate, "uncompleted", specialIn_txtBox.Text, null, myVariables.currID, myVariables.currnum);

            //create entries in job_tasks table
            foreach (string s in selectedList)
            {
                var taskid = await db.SelectSingle("SELECT Task_ID FROM Tasks WHERE task_description = @val0;", s);
                await db.InQuery("INSERT INTO Job_Tasks VALUES (@val0, @val1, @val2, @val3, @val4 )", "J" + num, taskid, null, null, null);
            }

            //populate the table on the page
            await db.Select(jobsGrid,
                "SELECT job_number, job_priority, deadline, job_status, special_instructions " +
                "FROM Job " +
                "WHERE job_number = @val0 " +
                "UNION  " +
                "SELECT coalesce(NULL, '*'), coalesce(NULL, 'Task'), coalesce(NULL, 'Description'), coalesce(NULL, 'Price(£)'), coalesce(NULL, ' ') " +
                "UNION " +
                "SELECT coalesce(NULL, '*'), Taskstask_id, task_description, price, coalesce(NULL, ' ') " +
                "FROM Job_Tasks, tasks " +
                "WHERE Jobjob_number = @val0 " +
                "AND Taskstask_id = task_id; "
                , "J" + num);
            

            System.Windows.Forms.MessageBox.Show("Job has been added successfully");
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void onChange(object sender, SelectionChangedEventArgs e)
        {
            tasks_dropDown.SelectedIndex = 0;
        }
    }
}
