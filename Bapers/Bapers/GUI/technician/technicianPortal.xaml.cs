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

        private void refresh_click(object sender, RoutedEventArgs e)
        {
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
