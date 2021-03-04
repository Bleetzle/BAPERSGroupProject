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

namespace Bapers.GUI
{
    /// <summary>
    /// Interaction logic for receptionist.xaml
    /// </summary>
    public partial class receptionist : Window
    {
        public receptionist()
        {
            InitializeComponent();
        }

        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private void searchAcc_click(object sender, RoutedEventArgs e)
        {

        }

        private void addPay_click(object sender, RoutedEventArgs e)
        {

        }
    }
}
