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
    /// Interaction logic for queries.xaml
    /// </summary>
    public partial class queries : Window
    {
        public queries()
        {
            InitializeComponent();
        }

        private void new_checked(object sender, RoutedEventArgs e)
        {

        }

        private void resolve_checked(object sender, RoutedEventArgs e)
        {

        }

        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            technicianPortal technicianPortalWindow = new technicianPortal();
            technicianPortalWindow.Show();
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void refresh_click(object sender, RoutedEventArgs e)
        {

        }

        private void submit_click(object sender, RoutedEventArgs e)
        {

        }
    }
}
