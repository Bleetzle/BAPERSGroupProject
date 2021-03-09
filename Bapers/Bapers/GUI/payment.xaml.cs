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
    /// Interaction logic for payment.xaml
    /// </summary>
    public partial class payment : Window
    {
        public payment()
        {
            InitializeComponent();
        }



        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void card_Checked(object sender, RoutedEventArgs e)
        {
            cardType.Visibility = Visibility.Visible;
            cardType_txtbox.Visibility = Visibility.Visible;
            expDate.Visibility = Visibility.Visible;
            expDate_txtbox.Visibility = Visibility.Visible;
            cvc.Visibility = Visibility.Visible;
            cvc_txtbox.Visibility = Visibility.Visible;
        }

        private void cash_Checked(object sender, RoutedEventArgs e)
        {
            cardType.Visibility = Visibility.Hidden;
            cardType_txtbox.Visibility = Visibility.Hidden;
            expDate.Visibility = Visibility.Hidden;
            expDate_txtbox.Visibility = Visibility.Hidden;
            cvc.Visibility = Visibility.Hidden;
            cvc_txtbox.Visibility = Visibility.Hidden;


        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            receptionist receptionistwindow = new receptionist();
            receptionistwindow.Show();
            this.Close();
        }

        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private void addPayment_Click(object sender, RoutedEventArgs e)
        {
            //code for adding payment details to the databse goes here
        }

        private void addJobs_Click(object sender, RoutedEventArgs e)
        {
            //first needs to check if customer is a valued or just a normal customer.
            //if normal make the add jobs turn invisible after one button press
        }

        private void pay_Click(object sender, RoutedEventArgs e)
        {
            //to print the reciept to pay
        }
    }
}
