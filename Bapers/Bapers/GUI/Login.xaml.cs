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
            bool isfound = false;
            //code for searching database for the username and password

            isfound = await db.Check("SELECT * FROM users WHERE username = @val0 AND pass = @val1", username_txtBox.Text, db.StringToHash(password_txtBox.Password));

            //need to change this to also store the information of whos logged in
            if (isfound)
            {
                GUI.receptionist receptionistwindow = new GUI.receptionist();
                receptionistwindow.Show();
                this.Close();
                //account found, switch to the account portal
            }
            else{
                //show error message
                System.Windows.Forms.MessageBox.Show("Account not found, Please check details are correct");
            }
        }





    }
}
