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

namespace Bapers.GUI.officeManager
{
    /// <summary>
    /// Interaction logic for changeCustomer.xaml
    /// </summary>
    public partial class changeCustomer : Window
    {
        DatabaseConnector db = new DatabaseConnector();
        public changeCustomer()
        {
            InitializeComponent();
            Populate();
        }

        private async void Populate()
        {
            await db.Select("")
        }


        private void standard_checked(object sender, RoutedEventArgs e)
        {
            discount_txt.Visibility = Visibility.Hidden;
            discount_Dropdown.Visibility = Visibility.Hidden;
        }

        private void valued_checked(object sender, RoutedEventArgs e)
        {
            discount_txt.Visibility = Visibility.Visible;
            discount_Dropdown.Visibility = Visibility.Visible;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            Login loginwindow = new Login();
            loginwindow.Show();
            this.Close();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            officeManagerPortal officeManagerPortalWindow = new officeManagerPortal();
            officeManagerPortalWindow.Show();
            this.Close();
        }

        private void changeDiscount_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
