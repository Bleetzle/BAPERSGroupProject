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
    /// Interaction logic for reportPortal.xaml
    /// </summary>
    public partial class reportPortal : Window
    {
        public reportPortal()
        {
            InitializeComponent();
        }

 

        private void individual_click(object sender, RoutedEventArgs e)
        {
            individual individualWindow = new individual();
            individualWindow.Show();
            this.Close();
        }

        private void individualP_click(object sender, RoutedEventArgs e)
        {
            individualPeformance individualPeformanceWindow = new individualPeformance();
            individualPeformanceWindow.Show();
            this.Close();
        }

        private void summP_click(object sender, RoutedEventArgs e)
        {
            summaryPerformace summaryPerformaceWindow = new summaryPerformace();
            summaryPerformaceWindow.Show();
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
