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
    /// Interaction logic for individual.xaml
    /// </summary>
    public partial class individual : Window
    {
        public individual()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void viewReport_Click(object sender, RoutedEventArgs e)
        {
            viewReport viewReportWindow = new viewReport();
            viewReportWindow.Show();
            this.Close();
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
