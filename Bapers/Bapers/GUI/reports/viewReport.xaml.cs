using System;
using System.Windows;

namespace Bapers.GUI.reports
{
    /// <summary>
    /// Interaction logic for viewReport.xaml
    /// </summary>
    ///
    public partial class viewReport : Window
    {
        DatabaseConnector db = new DatabaseConnector();
        string filePath = "";

        public viewReport(string reportType, string userID, string timespan, DateTime startDate, bool automatic, string path)
        {
            InitializeComponent();
            populateGrid(reportType, userID, timespan, startDate, automatic, path);
        }

        private async void populateGrid(string reportType, string userID, string timespan, DateTime srtDate, bool automatic, string path)
        {

            filePath = path += "\\" + reportType.Replace(" ", "") + "Report";

            var startDate = srtDate.Date;

            filePath = await db.generateReport(reportType, reportGrid, userID, int.Parse(timespan), startDate, automatic, path);
            //records generation
            await db.InQuery("INSERT INTO ReportHistory (report_date, report_type, automatically_generated, userID) VALUES (@val0, @val1, @val2, @val3)", DateTime.Now.Date, reportType, false, userID);
        }


        private void printReport_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@filePath);
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