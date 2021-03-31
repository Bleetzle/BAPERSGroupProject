using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Data;

namespace Bapers.GUI.officeManager
{
    /// <summary>
    /// Interaction logic for createUser.xaml
    /// </summary>
    public partial class createUser : Window
    {
        DatabaseConnector db = new DatabaseConnector();
        float selectedUserID = "";

        public createUser()
        {
            InitializeComponent();
            location_txtBox.Visibility = Visibility.Hidden;
            locName.Visibility = Visibility.Hidden;
            Populate();
        }

        private async void Populate()
        {
            await db.Select(userGrid, "SELECT UserID, username, first_name, last_name, role, shift_type, location FROM Users, staff WHERE userID = staff_ID");
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
            
            if ((bool)day_checkBox.IsChecked || (bool)night_checkBox.IsChecked)
            {
                shiftChecked = true;
            }
            //check if any area is empty
            if (username_txtBox.Text.Equals("") || 
                password_txtBox.Text.Equals("") || 
                firstname_txtBox.Text.Equals("") ||
                role_Dropdown.SelectedIndex.Equals(0) || 
                lastname_txtBox.Text.Equals("") || 
                !shiftChecked)
            {
                MessageBox.Show("Please fill in all areas");
                return;
            }
            var role = (role_Dropdown.SelectedItem as ListBoxItem).Content;

            //check if the location is empty, only done if technician as ther other roles do not hava a location
            if (role.Equals("Technician"))
            {
                if (location_txtBox.Text.Equals(""))
                {
                    MessageBox.Show("Please fill in all areas");
                    return;
                }
                else
                {
                    location = location_txtBox.Text;
                }
            }

            if (firstname_txtBox.Text.Equals("Night"))
                shift_type = "Night Shift 1";
            
            await db.InQuery( 
                "INSERT INTO staff (first_name, last_name, role, shift_type, location) " +
                "VALUES (@val0, @val1, @val2, @val3, @val4)",
                firstname_txtBox.Text, lastname_txtBox.Text, role, shift_type, location
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
            Populate();
        }

        private void onChange(object sender, SelectionChangedEventArgs e)
        {
            var role = (role_Dropdown.SelectedItem as ListBoxItem).Content;

            if (role.Equals("Technician"))
            {
                location_txtBox.Visibility = Visibility.Visible;
                locName.Visibility = Visibility.Visible;
            }
            else
            {
                location_txtBox.Visibility = Visibility.Hidden;
                locName.Visibility = Visibility.Hidden;
            }
        }

        private async void save_Click(object sender, RoutedEventArgs e)
        {
            //save the account details if changed
            userGrid.CommitEdit();
            foreach (System.Data.DataRowView dr in userGrid.ItemsSource)
            {
                //update user acc
                await db.InQuery(
                    "UPDATE Users " +
                    "SET username = @val0 " +
                    "WHERE userID = @val1; "
                    , dr.Row.Field<string>("username")
                    , dr.Row.Field<int>("userID")
                );
                //update staff
                await db.InQuery(
                    "UPDATE Staff " +
                    "SET first_name = @val0, " +
                    "last_name = @val1, " +
                    "role = @val2, " +
                    "shift_type = @val3, " +
                    "location = @val4 " +
                    "WHERE staff_ID = @val5; "
                    , dr.Row.Field<string>("first_name")
                    , dr.Row.Field<string>("last_name")
                    , dr.Row.Field<string>("role")
                    , dr.Row.Field<string>("shift_type")
                    , dr.Row.Field<string>("location")
                    , dr.Row.Field<int>("userID")
                );
            }
            MessageBox.Show("Details saved");
            Populate();
        }

        private void searchChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedUserID.Equals(""))
            {
                MessageBox.Show("No user selected");
            }

            if (!selectedUserID.Equals(""))
            {
                MessageBoxResult confirmResult = MessageBox.Show("Are you sure you want to delete the account?", "Confirm Delete", MessageBoxButton.YesNo);
                if (confirmResult == MessageBoxResult.No)
                    return;
            }
            


        }

        private void gridSelectChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
