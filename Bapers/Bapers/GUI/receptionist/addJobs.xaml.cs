using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
            //await db.Select(taskGrid, "SELECT task_id, task_description, price, COALESCE(null, ' ') AS Amount FROM Tasks ORDER BY task_id;");

            //foreach (System.Data.DataRowView dr in taskGrid.ItemsSource)
            //{
            //    foreach (System.Data.DataColumn dc in taskGrid.ItemsSource)
            //    {
            //        if (dc.ColumnName.Equals("Amount"))
            //        {

            //        }
            //    }
            //}
            await db.Select(taskGrid,"SELECT * FROM tasks" );
            
            //await db.SelectLists(list, "SELECT task_description FROM Tasks;");

            if (list != null)
            {
                foreach (string s in list)
                {

                    //adds checkboxes
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
            List<int> amount = new List<int>();

            //get the Selected tasks on drop down 
            //for (int i = 1; i < tasks_dropDown.Items.Count; i++)
            //{
            //    CheckBox cb = (tasks_dropDown.Items[i] as CheckBox);
            //    if ((bool)cb.IsChecked)
            //        selectedList.Add(cb.Content.ToString());
            //}

            int amountChanged = 0;

            //finds all the rows which has more than 0 in the amount

            foreach (System.Data.DataRowView dr in taskGrid.ItemsSource)
            {
                if (dr.Row.Field<int>("amount") > 0)
                {
                amountChanged++;
                selectedList.Add(dr.Row.Field<string>("task_description"));
                amount.Add(dr.Row.Field<int>("amount"));
                }
            }

            if (amountChanged == 0 || deadline_date.SelectedDate.Value.Equals(null) || time_dropDown.SelectedIndex.Equals(0))
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

            var tasksCost = new Dictionary<string,float>(); ;

            for (int i = 0; i < selectedList.Count(); i++)
            {
                //get the time to complete all tasks
                var val = await db.SelectSingle("SELECT task_duration FROM Tasks WHERE task_description = @val0;", selectedList[i]);
                minsToComplete += int.Parse(val) * amount[i];

                //get the cost of each task
                var val1 = await db.SelectSingle("SELECT task_id FROM Tasks WHERE task_description = @val0;", selectedList[i]);
                var val2 = await db.SelectSingle("SELECT price FROM Tasks WHERE task_description = @val0;", selectedList[i]);
                tasksCost.Add(val1, float.Parse(val2) * amount[i]);
            }
            
            //determine if job deadline is less than 6 hour
            timeTillDeadline = (selectedDate - DateTime.Now).TotalMinutes;
            string urgency = "Normal";
            if (timeTillDeadline < 360)
            {
                urgency = "Urgent"; 
                if (timeTillDeadline < 180)
                {
                    foreach (string f in tasksCost.Keys.ToList())
                        tasksCost[f] *= 2;
                    if (timeTillDeadline< 60)
                    {
                        foreach (string f in tasksCost.Keys.ToList())
                            tasksCost[f] *= 2;
                    }
                }
            }
           
            var tmpList = tasksCost;

            float totalPrice = 0f;


            switch (await db.SelectSingle(
                "SELECT discount_plan " +
                "FROM discount " +
                "WHERE Customeraccount_number = @val0; "
                , myVariables.currID)
                )
            {
                case "Fixed":
                    //removes the amount from the total cost
                    foreach (float f in tasksCost.Values)
                        totalPrice += f;
                    string val = await db.SelectSingle("SELECT discount_rate FROM fixed_discount WHERE DiscountCustomeraccount_number = @val0", myVariables.currID);
                    totalPrice -= (totalPrice * (float.Parse(val)/100));
                    break;
                case "Variable":
                    //grabs the discount and removes it from each price
                    foreach (KeyValuePair<string, float> p in tmpList.ToList())
                    {
                        string val1 = await db.SelectSingle("SELECT discount_rate FROM variable_discount WHERE DiscountCustomeraccount_number = @val0 AND task_type = @val1", myVariables.currID, p.Key);
                        tasksCost[p.Key] -= p.Value * (float.Parse(val1)/100); 
                    }
                    //takes all the discounted prices and adds them together
                    foreach(float f in tasksCost.Values)
                        totalPrice += f;
                    break;
                case "Flexible":
                    //grabs the month range from today
                    var todays_date = DateTime.Now.Date;
                    var lastmonth_date = todays_date.Subtract(TimeSpan.FromDays(30));
                    //grabs the money spent by the customer in the last 30 
                    var monthlyTotal = 
                        await db.SelectSingle(
                        "SELECT SUM(payment_amount) " +
                        "FROM payment " +
                        "WHERE Customeraccount_number = @val0 " +
                        "AND Customerphone_number = @val1 " +
                        "AND payment_date BETWEEN @val2 AND @val3; "
                        , myVariables.currID, int.Parse(myVariables.currnum), lastmonth_date, todays_date);
                    if (monthlyTotal.Equals(""))
                        monthlyTotal = "0";
                    //grab the discount rate based on the monthly discount
                    float rate = float.Parse(await db.SelectSingle(
                        "SELECT discount_rate " +
                        "FROM flexible_discount " +
                        "WHERE DiscountCustomeraccount_number = @val0 " +
                        "AND @val1 BETWEEN  lower AND upper; "
                        , myVariables.currID, float.Parse(monthlyTotal)));

                    //uses the rate gotten to apply the discount to the total price
                    foreach (float f in tasksCost.Values)
                            totalPrice += f;
                    totalPrice -= (totalPrice * (rate/100));
                    break;
                default:
                    foreach (float f in tasksCost.Values)
                        totalPrice += f;
                    break;
            }

            //create entry in job table
            await db.InQuery(
            "INSERT INTO Job(job_Number, job_priority, deadline, job_status, special_instructions, job_completed, Customeraccount_number, Customerphone_number, discounted_total) " +
            "VALUES(@val0, @val1 , @val2, @val3, @val4, @val5, @val6, @val7, @val8);"
            ,"J" + num , urgency, selectedDate, "uncompleted", specialIn_txtBox.Text, null, myVariables.currID, myVariables.currnum, totalPrice);


            for(int i = 0; i< selectedList.Count(); i++)
            {
                var taskid = await db.SelectSingle("SELECT Task_ID FROM Tasks WHERE task_description = @val0;", selectedList[i]);
                await db.InQuery("INSERT INTO Job_Tasks VALUES (@val0, @val1, @val2, @val3, @val4, @val5)", "J" + num, taskid, null, null, null,amount[i]);
            }

            //populate the table on the page
            await db.Select(jobsGrid,
                "SELECT job_number, job_priority, deadline, job_status, special_instructions AS Amount, coalesce(NULL, ' ') AS Total " +
                "FROM Job " +
                "WHERE job_number = @val0 " +
                "UNION  " +
                "SELECT coalesce(NULL, '*'), coalesce(NULL, 'Task'), coalesce(NULL, 'Description'), coalesce(NULL, 'Price(£)'), coalesce(NULL, ' '),coalesce(NULL, ' ') " +
                "UNION " +
                "SELECT coalesce(NULL, '*'), Taskstask_id, task_description, price, Job_Tasks.amount, coalesce(NULL, 'fill') " +
                "FROM Job_Tasks, tasks " +
                "WHERE Jobjob_number = @val0 " +
                "AND Taskstask_id = task_id " +
                "UNION " +
                "SELECT coalesce(NULL, 'Sub Total'), coalesce(NULL, '*'), coalesce(NULL, '*'),coalesce(NULL, '*'), coalesce(NULL, '*'), discounted_total " +
                "FROM Job " +
                "WHERE job_number = @val0 " +
                "UNION " +
                "SELECT coalesce(NULL, 'Total: '), coalesce(NULL, ' '), coalesce(NULL, ' '), coalesce(NULL, 'Values Added Tax (20%)'), coalesce(NULL, 'tax'), coalesce(NULL, 'total'); "
                , "J" + num);

            foreach (System.Data.DataRowView dr in jobsGrid.ItemsSource)
            {
                if (dr.Row.Field<string>(5).Equals("fill")){
                    dr.Row.SetField<string>(5, tasksCost[dr.Row.Field<string>("job_priority")].ToString() );
                }
                if (dr.Row.Field<string>(4).Equals("tax"))
                {
                    dr.Row.SetField<string>(4, (Math.Round(totalPrice * 0.2, 2)).ToString());
                }
                if (dr.Row.Field<string>(5).Equals("total"))
                {
                    dr.Row.SetField<string>(5, (Math.Round(totalPrice * 1.2, 2) ).ToString());
                }
            }
            System.Windows.Forms.MessageBox.Show("Job has been added successfully");
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void onChange(object sender, SelectionChangedEventArgs e)
        {
            tasks_dropDown.SelectedIndex = 0;
        }

        private void taskGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ontaskChange(object sender, SelectedCellsChangedEventArgs e)
        {

        }
    }
}
