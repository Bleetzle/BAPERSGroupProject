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
    /// Interaction logic for searchAcc.xaml
    /// </summary>
    public partial class searchAcc : Window
    {
        DatabaseConnector db = new DatabaseConnector();

        public searchAcc()
        {
            InitializeComponent();
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

        private void back_Click(object sender, RoutedEventArgs e)
        {
            receptionist receptionistwindow = new receptionist();
            receptionistwindow.Show();
            this.Close();
        }

        private async void search_click(object sender, RoutedEventArgs e)
        {
            //check if any of the inputs are empty
            if (firstname_txtBox.Text.Equals("") || surname_txtBox.Text.Equals("") ||telephone_txtBox.Text.Equals("") || email_txtBox.Text.Equals(""))
            {
                MessageBox.Show("Please fill in all boxes!");
                return;
            }

            //constructs a query based on the inputs
            //checks if the data for the given query exists
            bool isfound = await db.Check(
                " SELECT *" +
                " FROM Customer" +
                " WHERE first_name = @val0" +
                " AND last_name = @val1" +
                " AND phone_number = @val2 " +
                " AND email_address = @val3 " +
                ";"
                , firstname_txtBox.Text, surname_txtBox.Text, telephone_txtBox.Text, email_txtBox.Text
                );
            //if account is found
            if (isfound)
            {
                //stores the customer deatails for use in the job adding or payment adding
                myVariables.currfname = firstname_txtBox.Text;
                myVariables.currlname = surname_txtBox.Text;
                myVariables.currnum = telephone_txtBox.Text;
                myVariables.currvalue = await db.SelectSingle(
                    "SELECT customer_status " +
                    "FROM Customer " +
                    "WHERE first_name = @val0 " +
                    "AND last_name = @val1 " +
                    "AND phone_number = @val2 " +
                    ";", firstname_txtBox.Text, surname_txtBox.Text, telephone_txtBox.Text);
                myVariables.currID = await db.SelectSingle(
                    "SELECT account_number " +
                    "FROM Customer " +
                    "WHERE first_name = @val0 " +
                    "AND last_name = @val1 " +
                    "AND phone_number = @val2 " +
                    ";", firstname_txtBox.Text, surname_txtBox.Text, telephone_txtBox.Text);
                //creates window for customer thats found to be used
                accountFound accountfoundwindow = new accountFound();
                accountfoundwindow.Show();
                this.Close();
            }
            else {
                accNotFound accnotfoundWindow = new accNotFound();
                accnotfoundWindow.Show();
                this.Close();
            }
        }
    }
}
