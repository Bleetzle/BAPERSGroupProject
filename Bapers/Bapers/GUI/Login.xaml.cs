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

        private void credentialChecker(string username, string password)
        {
            bool isfound = false;
            //fucntion used to verify credentials
            if (username == "admin" && password == "pass")
            {
                isfound = true;
            }
            //code for searching database for the username and password

            
            if (isfound)
            {
                GUI.receptionist receptionistwindow = new GUI.receptionist();
                receptionistwindow.Show();
                this.Close();
                //account found, switch to the account portal
                
            }
            else{
                //show error message
                System.Windows.Forms.MessageBox.Show("Error, Account not found");


            }
        }
    }
}
