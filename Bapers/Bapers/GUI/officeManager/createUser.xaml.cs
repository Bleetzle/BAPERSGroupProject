using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Data;
using System.ComponentModel;

namespace Bapers.GUI.officeManager
{
    /// <summary>
    /// Interaction logic for createUser.xaml
    /// </summary>
    public partial class createUser : Window
    {
        //connects to the databse
        DatabaseConnector db = new DatabaseConnector();
        //current selected user
        string selectedUserID = "";
        DataView data;

        public createUser()
        {
            InitializeComponent();
            location_txtBox.Visibility = Visibility.Hidden;
            locName.Visibility = Visibility.Hidden;
            Populate();
            data = (DataView)userGrid.ItemsSource;
        }

        private async void Populate()
        {
            //gets all the current users from the database
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
            //default shift if nothing is checked
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
            //gets thr role from the drop down box
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
            //adds values from textbox if its a night shift
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
            //gets role from dropdown bar
            var role = (role_Dropdown.SelectedItem as ListBoxItem).Content;

            if (role.Equals("Technician"))
            {
                //location boxes for technician is needed when created
                location_txtBox.Visibility = Visibility.Visible;
                locName.Visibility = Visibility.Visible;
            }
            else
            {
                //else hide the boxes
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
                //update user account
                await db.InQuery(
                    "UPDATE Users " +
                    "SET username = @val0 " +
                    "WHERE userID = @val1; "
                    , dr.Row.Field<string>("username")
                    , dr.Row.Field<int>("userID")
                );
                //update staff account
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
            //search box to search through the users
            if (data != null)
            {
                if (!searchbox.Text.Equals("") && !searchbox.Text.Equals("Search..."))
                {
                    //updates database to show searched query
                    string searchstring =  searchbox.Text;
                    int convert;
                    int.TryParse(searchstring, out convert);
                    if (convert != 0)
                        data.RowFilter = " UserID = " + searchstring + "";
                    else
                    {
                        data.RowFilter =
                        "first_name LIKE '%" + searchstring.ToString()
                        + "%' OR last_name LIKE '%" + searchstring.ToString()
                        + "%' OR role LIKE '%" + searchstring.ToString()
                        + "%' OR shift_type LIKE '%" + searchstring.ToString()
                        + "%' OR location LIKE '%" + searchstring.ToString()
                        + "%'";
                    }
                    userGrid.ItemsSource = data;
                }
                else
                {
                    Populate();
                }
            }
            // UserID, username, first_name, last_name, role, shift_type, location
        }

        private async void delete_Click(object sender, RoutedEventArgs e)
        {
            //deleting users from the database
            //if no user is selected
            if (selectedUserID.Equals(""))
            {
                MessageBox.Show("No user selected");
                return;
            }

            if (!selectedUserID.Equals(""))
            {
                //confirm to delete user
                MessageBoxResult confirmResult = MessageBox.Show("Are you sure you want to delete the account?", "Confirm Delete", MessageBoxButton.YesNo);
                if (confirmResult == MessageBoxResult.No)
                    return;
            }
            //all remove all their tasks
            await db.InQuery("UPDATE Job_tasks SET Staffstaff_ID = null WHERE Staffstaff_ID = @val0;", int.Parse(selectedUserID));
            //remove any questions
            await db.InQuery("DELETE FROM Responces WHERE staff_id = @val0;", int.Parse(selectedUserID));
            //remove any responces
            await db.InQuery("DELETE FROM Questions WHERE staff_id = @val0;", int.Parse(selectedUserID));
             //remove user account
            await db.InQuery("DELETE FROM Users WHERE userId = @val0;", int.Parse(selectedUserID));
            //remove the staff 
            await db.InQuery("DELETE FROM staff WHERE staff_id = @val0;", int.Parse(selectedUserID));
           

            Populate();

        }

        private void gridSelectChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            foreach (var item in e.AddedCells)
            {
                if (item.Column != null)
                {
                    string col = item.Column.Header.ToString();
                    //tmp job num to be able to create the map value

                    //assuming job num always appears before the price
                    if (col.Equals("UserID") && item.Column.GetCellContent(item.Item) != null)
                    {
                        selectedUserID = (item.Column.GetCellContent(item.Item) as TextBlock).Text;
                    }
                }
            }
        }
    }
}
