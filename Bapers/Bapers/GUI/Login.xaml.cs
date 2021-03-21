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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bapers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Login : Window
    {

        DatabaseConnector db = new DatabaseConnector();
        public Login()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        
        private void login_Click(object sender, RoutedEventArgs e)
        {
            credentialChecker(username_txtBox.Text, password_txtBox.Password);
            //variables passed from username and password textbox
        }

        private async void credentialChecker(string username, string password)
        {
            if (username_txtBox.Text.Equals("") || password_txtBox.Password.Equals(""))
            {
                MessageBox.Show("Please fill in all areas");
                return;
            }


            bool isfound = false;
            //code for searching database for the username and password

            var num = await db.SelectSingle("SELECT userID FROM users WHERE username = @val0 AND pass = @val1", username_txtBox.Text, db.StringToHash(password_txtBox.Password));

            //need to change this to also store the information of whos logged in
            if (!num.Equals("null"))
            {
                string role = await db.SelectSingle("SELECT role FROM staff WHERE staff_ID = @val0", num); ;
                myVariables.role = role;

                //check user roles upon logging in
                switch (role) {
                    case "Receptionist":
                        GUI.receptionist receptionistwindow = new GUI.receptionist();
                        receptionistwindow.Show();
                        this.Close();
                        break;
                    case "Technician":
                        GUI.technician.technicianPortal technicianWindow = new GUI.technician.technicianPortal();
                        technicianWindow.Show();
                        this.Close();
                        break;
                    default:
                        MessageBox.Show("Something went wrong, no role assigned to user");
                        break;
                }
                                //account found, switch to the account portal
            }
            else{
                //show error message
                System.Windows.Forms.MessageBox.Show("Account not found, Please check details are correct");
            }
        }

    }
}
