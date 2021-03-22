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

namespace Bapers.GUI.reports
{
    /// <summary>
    /// Interaction logic for viewReport.xaml
    /// </summary>
    public partial class viewReport : Window
    {
        DatabaseConnector db = new DatabaseConnector();

        public viewReport(string reportType, string userID, string timespan, string startDate)
        {
            InitializeComponent();
            populateGrid(reportType, userID, timespan, startDate);
 
        }

        private async void populateGrid(string reportType, string userID, string timespan, string startDate)
        {
            string endDate = "200120"; 

            switch (reportType)
            {
                case "Individual Performance":
                    await db.Select(reportGrid,
                        "SELECT first_name, last_name, tasks.location AS Department, job_completed AS Date, start_time, time_taken, t1.total_time AS Total " +
                        "FROM staff, job, Job_Tasks, Tasks, " +
                        "( SELECT SUM(time_taken) as total_time, Staffstaff_ID AS totalStaffID  " +
                        "FROM Job_Tasks, job  " +
                        "WHERE job_status = \"Completed\"  " +
                        "AND Job_number = Jobjob_number  " +
                        "AND job_completed BETWEEN @val0 AND @val1  " +
                        "GROUP BY Staffstaff_ID) t1  " +
                        "WHERE staff_ID = Staffstaff_ID  " +
                        "AND t1.totalStaffID = staff_ID  " +
                        "AND Job_number = Jobjob_number  " +
                        "AND Taskstask_ID = task_ID  " +
                        "AND job_status = \"Completed\"  " +
                        "AND job_completed BETWEEN @val0 AND @val1; "
                        , startDate, endDate);
                    break;
                case "Summary Performance":
                    await db.Select(reportGrid,
                        "SELECT * " +
                        "FROM v1 " +
                        "WHERE Date BETWEEN @val0 AND @val1 " +
                        "AND Shift = \"Day Shift 1\" " +
                        "UNION " +
                        "SELECT coalesce(NULL, ' '), coalesce(NULL, 'Total'), coalesce(SUM(Copy_room), '0'), coalesce(Sum(Development), '0'), coalesce(Sum(Finishing), '0'), coalesce(Sum(Packing), '0') " +
                        "FROM v1 " +
                        "WHERE DATE BETWEEN @val0 AND @val1 " +
                        "AND Shift = \"Day Shift 1\" " +
                        "UNION " +
                        "SELECT * " +
                        "FROM v1 " +
                        "WHERE Date BETWEEN @val0 AND @val1 " +
                        "AND Shift = \"Day Shift 2\" " +
                        "UNION " +
                        "SELECT coalesce(NULL, ' '), coalesce(NULL, 'Total'), coalesce(SUM(Copy_room), '0'), coalesce(Sum(Development), '0'), coalesce(Sum(Finishing), '0'), coalesce(Sum(Packing), '0') " +
                        "FROM v1 " +
                        "WHERE DATE BETWEEN @val0 AND @val1 " +
                        "AND Shift = \"Day Shift 2\" " +
                        "UNION " +
                        "SELECT * " +
                        "FROM v1 " +
                        "WHERE Date BETWEEN @val0 AND @val1 " +
                        "AND Shift = \"Night Shift 1\" " +
                        "UNION " +
                        "SELECT coalesce(NULL, ' '), coalesce(NULL, 'Total'), coalesce(SUM(Copy_room), '0'), coalesce(Sum(Development), '0'), coalesce(Sum(Finishing), '0'), coalesce(Sum(Packing), '0') " +
                        "FROM v1 " +
                        "WHERE DATE BETWEEN @val0 AND @val1 " +
                        "AND Shift = \"Night Shift 1\" " +
                        "UNION " +
                        "SELECT coalesce(NULL, ' '), coalesce(NULL, ' '), coalesce(NULL, ' '), coalesce(NULL, ' '), coalesce(NULL, ' '), coalesce(NULL, ' ') " +
                        "UNION " +
                        "SELECT coalesce(NULL, ' '), Shift, coalesce(SUM(Copy_room), '0'), coalesce(Sum(Development), '0'), coalesce(Sum(Finishing), '0'), coalesce(Sum(Packing), '0') " +
                        "FROM v1 " +
                        "WHERE DATE BETWEEN @val0 AND @val1 " +
                        "GROUP BY Shift " +
                        "UNION " +
                        "SELECT coalesce(NULL, 'Total '), coalesce(NULL, ' '), coalesce(SUM(Copy_room), '0'), coalesce(Sum(Development), '0'), coalesce(Sum(Finishing), '0'), coalesce(Sum(Packing), '0') " +
                        "FROM v1 " +
                        "WHERE DATE BETWEEN @val0 AND @val1; "
                        , startDate, endDate);
                    break;
                case "Individual":
                    //indivual report does not exist rn 
                    await db.Select(reportGrid,"", userID, startDate, endDate);
                    break;
                default:
                    MessageBox.Show("There was an error");
                    break;
            }
            
        }


        private void printReport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            reportPortal reportPortalWindow = new reportPortal();
            reportPortalWindow.Show();
            this.Close();
        }

        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }
    }
}
