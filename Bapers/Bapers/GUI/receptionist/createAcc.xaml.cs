using System.Windows;
using System.Windows.Controls;

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

        private async void create_Click(object sender, RoutedEventArgs e)
        {
            //input validation
            if (firstname_txtBox.Text.Equals("") || surname_txtBox.Text.Equals("")||telephone_txtBox.Text.Equals("")||compName_txtBox.Text.Equals("")||address_txtBox.Text.Equals(""))
            {
                MessageBox.Show("Please fill in all areas!");
                return;
            }
            //need to check it does not exist already


            //creates the auto incrementing value for the id
            string accNum = await db.SelectSingle("SELECT MAX(account_number) FROM Customer;");
            int num = ( int.Parse( accNum.Substring(3) ) ) + 1;

            //uses the input and the created id to create a new account
            await db.InQuery(
                "INSERT INTO Customer (account_number, first_name, last_name, phone_number, company_name, address, email_address, customer_status)" +
                "VALUES (@val0, @val1, @val2, @val3, @val4, @val5, @val6, @val7)"
                ,"ACC" + num, firstname_txtBox.Text, surname_txtBox.Text, telephone_txtBox.Text, compName_txtBox.Text, address_txtBox.Text,email_txtBox.Text, "standard"
                );


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

            //after account created..
            accountCreated_popup accountCreated_Popup_window = new accountCreated_popup();
            accountCreated_Popup_window.Show();
            this.Close();
        }
    }
}
