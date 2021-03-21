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
    /// Interaction logic for shiftManager.xaml
    /// </summary>
    public partial class shiftManager : Window
    {
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
    }
}
