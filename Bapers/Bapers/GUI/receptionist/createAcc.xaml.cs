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
    /// Interaction logic for createAcc.xaml
    /// </summary>
    public partial class createAcc : Window
    {
        DatabaseConnector db = new DatabaseConnector();

        public createAcc()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
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
            receptionist receptionistwindow = new receptionist();
            receptionistwindow.Show();
            this.Close();
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {

            if (firstname_txtBox.Text.Equals("") || surname_txtBox.Text.Equals("")||telephone_txtBox.Text.Equals("")||compName_txtBox.Text.Equals("")||address_txtBox.Text.Equals(""))
            {
                MessageBox.Show("Please fill in all areas!");
                return;
            }


            string accNum = db.SelectSingle("SELECT MAX(account_number) FROM Customer;");
            int num = ( int.Parse( accNum.Substring(3) ) ) + 1;


            string query =
                "INSERT INTO Customer (account_number, first_name, last_name,phone_number, company_name, address, customer_status)" +
                "VALUES ( " + 
                "\"ACC" + num + "\"," +
                "\"" + firstname_txtBox.Text + "\"," +
                "\"" + surname_txtBox.Text + "\", " +
                "\"" + telephone_txtBox.Text + "\", " +
                "\"" + compName_txtBox.Text + "\", " +
                "\"" + address_txtBox.Text + "\", " +
                " \"standard\"" +
                ");";

            db.InQuery(query);

            //after account created..
            accountCreated_popup accountCreated_Popup_window = new accountCreated_popup();
            accountCreated_Popup_window.Show();
            this.Close();
        }
    }
}
