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

        private void search_click(object sender, RoutedEventArgs e)
        {
            //check if any of the inputs are empty
            if (firstname_txtBox.Text.Equals("") || surname_txtBox.Text.Equals("") ||telephone_txtBox.Text.Equals(""))
            {
                MessageBox.Show("Please fill in all boxes!");
                return;
            }

            //constructs a query based on the inputs
            string query = 
                " SELECT *" +
                " FROM Customer" +
                " WHERE first_name = \"" + firstname_txtBox.Text + "\"" +
                " AND last_name = \"" + surname_txtBox.Text + "\"" +
                " AND phone_number = \"" + telephone_txtBox.Text + "\"" +
                ";"
                ;

            //checks if the data for the given query exists
            bool isfound = db.Check(query);

            if (isfound)
            {
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
