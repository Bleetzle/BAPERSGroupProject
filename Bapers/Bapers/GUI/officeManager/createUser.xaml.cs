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
    /// Interaction logic for createUser.xaml
    /// </summary>
    public partial class createUser : Window
    {
        DatabaseConnector db = new DatabaseConnector();
        public createUser()
        {
            InitializeComponent();
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

        private async void createAccount_Click(object sender, RoutedEventArgs e)
        {
            bool shiftChecked = false;
            string shift_type = "Day Shift 1";
            string location = null;
            RadioButton typeItem = (RadioButton)role_Dropdown.SelectedItem;

            if ((bool)day_checkBox.IsChecked || (bool)night_checkBox.IsChecked)
            {
                shiftChecked = true;
            }

            if (username_txtBox.Text.Equals("") || 
                password_txtBox.Text.Equals("") || 
                firstname_txtBox.Text.Equals("") || 
                typeItem.Content.ToString().Equals("null") || 
                lastname_txtBox.Text.Equals("") || 
                !shiftChecked)
            {
                MessageBox.Show("Please fill in all areas");
                return;
            }

            //if (role_Dropdown.SelectedItem.Equals("Techinician"))
            //{
            //    if (location_txtBox.Text.Equals(""))
            //    {
            //        MessageBox.Show("Please fill in all areas");
            //        return;
            //    }else
            //    {
            //        location = location_txtBox.Text;
            //    }
            //}
                 
            if (firstname_txtBox.Text.Equals("Night"))
                shift_type = "Night Shift 1";
            
            await db.InQuery( 
                "INSERT INTO staff (first_name, last_name, role, shift_type, location) " +
                "VALUES (@val0, @val1, @val2, @val3, @val4)",
                firstname_txtBox.Text, lastname_txtBox.Text, typeItem.Content.ToString(), shift_type, location
                );
            await db.InQuery(
                "INSERT INTO users (username, pass, UserID) " +
                "VALUES (@val0, @val1, " +
                    "(SELECT staff_ID " +
                    "FROM staff " +
                    "WHERE first_name = @val2 " +
                    "AND last_name = @val3) " +
                    ")",
                username_txtBox.Text, db.StringToHash(password_txtBox.Text), firstname_txtBox.Text, lastname_txtBox.Text
                );
            MessageBox.Show("Account created successfully");
        }
    }
}
