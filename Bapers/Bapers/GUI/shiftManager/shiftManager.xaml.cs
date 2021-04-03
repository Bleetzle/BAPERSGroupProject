using System.Windows;


namespace Bapers.GUI.shiftManager
{
    /// <summary>
    /// Interaction logic for shiftManager.xaml
    /// </summary>
    public partial class shiftManager : Window
    {
        //portal for when a shiftmanager logs in
        public shiftManager()
        {
            myVariables.myStack.Push("Shift Manager");
            InitializeComponent();
        }

        private void reception_click(object sender, RoutedEventArgs e)
        {
            receptionist receptionistWindow = new receptionist();
            receptionistWindow.Show();
            this.Close();
        }

        private void report_click(object sender, RoutedEventArgs e)
        {
            reports.reportPortal reportPortalWindow = new reports.reportPortal();
            reportPortalWindow.Show();
            this.Close();
           
        }

        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private void assign_click(object sender, RoutedEventArgs e)
        {
            //used for assigning tasks
            assign assignWindow = new assign();
            assignWindow.Show();
            this.Close();
        }
    }
}
