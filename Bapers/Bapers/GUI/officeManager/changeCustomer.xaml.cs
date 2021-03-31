using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    /// Interaction logic for changeCustomer.xaml
    /// </summary>
    public partial class changeCustomer : Window
    {
        DatabaseConnector db = new DatabaseConnector();
        string selectedAcc = "";
        DataView data;

        public changeCustomer()
        {
            InitializeComponent();
            Populate();
            valued_radioBtn.IsEnabled = false;
            standard_radioBtn.IsEnabled = false;
            discount_Dropdown.IsEnabled = false;
            changeDiscount_btn.IsEnabled = false;
            data = (DataView)custGrid.ItemsSource;
        }

        private async void Populate()
        {
            await db.Select(custGrid, "SELECT account_number, first_name, last_name, email_address, phone_number, address, company_name FROM Customer");

            await db.Select(variGrid, "SELECT task_id, task_description, NULLIF(discount_rate, discount_rate) AS discount_rate FROM variable_discount, tasks WHERE task_type = task_id");

            await db.Select(flexGrid, "SELECT lower, upper, discount_rate " +
                                            "FROM flexible_discount " +
                                            "WHERE DiscountCustomeraccount_number IS NULL; "
                                            );
        }


        private void standard_checked(object sender, RoutedEventArgs e)
        {
            discount_txt.Visibility = Visibility.Hidden;
            discount_Dropdown.Visibility = Visibility.Hidden;
            discount_Dropdown.SelectedIndex = -1;
        }

        private void valued_checked(object sender, RoutedEventArgs e)
        {
            discount_txt.Visibility = Visibility.Visible;
            discount_Dropdown.Visibility = Visibility.Visible;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

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

        private void searchChanged(object sender, TextChangedEventArgs e)
        {
            if (data != null)
            {
                if (!searchbox.Text.Equals("") && !searchbox.Text.Equals("Search..."))
                {
                    string searchstring = searchbox.Text;
                    int convert;
                    int.TryParse(searchstring, out convert);
                    if (convert != 0)
                        data.RowFilter = " phone_number = " + searchstring 
                           + "";
                    else
                    {
                        data.RowFilter =
                        "account_number LIKE '%" + searchstring.ToString()
                        + "%' OR first_name LIKE '%" + searchstring.ToString()
                        + "%' OR last_name LIKE '%" + searchstring.ToString()
                        + "%' OR address LIKE '%" + searchstring.ToString()
                        + "%' OR email_address LIKE '%" + searchstring.ToString()
                        + "%' OR company_name LIKE '%" + searchstring.ToString()
                        + "%'";
                    }
                    custGrid.ItemsSource = data;
                }
                else
                {
                    Populate();
                }
            }
            // UserID, username, first_name, last_name, role, shift_type, location
        }

        /// <summary>
        /// whenever there is a change in the customer table, then the function is called to check all the details of the customer and adapt the page accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void onCustChange(object sender, SelectedCellsChangedEventArgs e)
        {
            if (!changeDiscount_btn.IsEnabled)
            {
                valued_radioBtn.IsEnabled = true;
                standard_radioBtn.IsEnabled = true;
                discount_Dropdown.IsEnabled = true;
                changeDiscount_btn.IsEnabled = true;
            }

            //requiries the variable discount fird to reset it
            await db.Select(variGrid, "SELECT task_id, task_description, NULLIF(discount_rate, discount_rate) AS discount_rate FROM variable_discount, tasks WHERE task_type = task_id");
            //resets the values for the fleible discount grid
            if (flexGrid.ItemsSource != null)
            {
                DataTable dt2 = new DataTable();
                dt2 = ((DataView)flexGrid.ItemsSource).ToTable();
                dt2.Rows.Clear();
                flexGrid.ItemsSource = dt2.DefaultView;
                flexGrid.DataContext = dt2.DefaultView;
            }


            foreach (var item in e.AddedCells)
            {
                if (item.Column != null)
                {
                    string col = item.Column.Header.ToString();
                    //tmp job num to be able to create the map value

                    //assuming job num always appears before the price
                    if (col.Equals("account_number") && item.Column.GetCellContent(item.Item) != null)
                    {
                        selectedAcc = (item.Column.GetCellContent(item.Item) as TextBlock).Text;
                        //change the discount information
                        switch (await db.SelectSingle("SELECT customer_status FROM customer WHERE account_number = @val0", selectedAcc))
                        {
                            case "standard":
                                standard_radioBtn.IsChecked = true;
                                break;
                            case "valued":
                                valued_radioBtn.IsChecked = true;
                                switch(await db.SelectSingle("SELECT discount_plan FROM discount WHERE Customeraccount_number = @val0", selectedAcc)){
                                    case "Fixed":
                                        discount_Dropdown.SelectedIndex = 0;
                                        fixed_txtBox.Text = await db.SelectSingle(
                                            "SELECT discount_rate " +
                                            "FROM fixed_discount " +
                                            "WHERE DiscountCustomeraccount_number = @val0"
                                            , selectedAcc);
                                        break;
                                    case "Variable":
                                        discount_Dropdown.SelectedIndex = 1;
                                        await db.Select(variGrid,
                                            "SELECT task_id, task_description, discount_rate " +
                                            "FROM variable_discount, tasks " +
                                            "WHERE task_type = task_id " +
                                            "AND DiscountCustomeraccount_number = @val0; "
                                            ,selectedAcc);
                                        break;
                                    case "Flexible":
                                        discount_Dropdown.SelectedIndex = 2;
                                        await db.Select(flexGrid,
                                            "SELECT  lower, upper, discount_rate " +
                                            "FROM flexible_discount " +
                                            "WHERE DiscountCustomeraccount_number = @val0; "
                                            , selectedAcc);
                                        break;
                                    default:
                                        discount_Dropdown.SelectedIndex = -1;
                                        break;
                                }
                                break;
                            default:
                                break;
                        }

                    }
                }
            }
        }

        /// <summary>
        /// When the discount changes which is caused either by the user or by the system when pressing on a customer, see onCustChange() then the page cahnges layout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onDiscountChange(object sender, SelectionChangedEventArgs e)
        {
            fixedtxt.Visibility = Visibility.Hidden;
            fixed_txtBox.Visibility = Visibility.Hidden;
            variGrid.Visibility = Visibility.Hidden;
            flexGrid.Visibility = Visibility.Hidden;

            switch (discount_Dropdown.SelectedIndex)
            {
                case 0:
                    fixedtxt.Visibility = Visibility.Visible;
                    fixed_txtBox.Visibility = Visibility.Visible;
                    break;
                case 1:
                    variGrid.Visibility = Visibility.Visible;
                    break;
                case 2:
                    flexGrid.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        //saves all changes to the data 
        private async void saveChanges(object sender, RoutedEventArgs e)
        {
            if (selectedAcc.Equals(""))
            {
                MessageBox.Show("Please select an account and change details to save");
                return;
            }
            if ((bool)valued_radioBtn.IsChecked) {
                switch (discount_Dropdown.Text)
                {
                    case "Fixed":
                        if (fixed_txtBox.Text.Equals(""))
                        {
                            MessageBox.Show("Please input a discount value");
                            return;
                        }
                        break;
                    case "Variable":
                        foreach (System.Data.DataRowView dr in variGrid.ItemsSource)
                        {
                            var str = dr.Row.Field<string>("discount_rate");
                            if (str == null )
                            {
                                MessageBox.Show("Please insert missing discount values");
                                return;
                            }
                        }
                        break;
                    case "Flexible":
                        if (flexGrid.Items.Count == 1)
                        {
                            MessageBox.Show("Please insert some boundaries");
                            return;
                        }
                        break;
                    default:
                        MessageBox.Show("Please Select a discount type");
                        return;
                }
            }


            //save the account details if changed
            custGrid.CommitEdit();
            foreach(System.Data.DataRowView dr in custGrid.ItemsSource)
            {
                await db.InQuery(
                    "UPDATE Customer " +
                    "SET first_name = @val0, " +
                    "   last_name = @val1, " +
                    "   phone_number = @val2, " +
                    "   address = @val3, " +
                    "   company_name = @val4, " +
                    "   email_adress = @val5 " +
                    "WHERE account_number = @val6; "
                    , dr.Row.Field<string>("first_name")
                    , dr.Row.Field<string>("last_name")
                    , dr.Row.Field<int>("phone_number")
                    , dr.Row.Field<string>("address") 
                    , dr.Row.Field<string>("company_name")
                    , dr.Row.Field<string>("email_adress")
                    , dr.Row.Field<string>("account_number"));  
                
                //account_number, first_name, last_name, phone_number, address, company_name
            }


            //check the discount type and see if it was the same as the selected one
            //if same, then check for changes to values
            var val = await db.SelectSingle("SELECT customer_status FROM customer WHERE account_number = @val0", selectedAcc);
            var disc = await db.SelectSingle("SELECT discount_plan FROM discount WHERE Customeraccount_number = @val0", selectedAcc);
            //delete any discount that could be attached to the customer

            //if was originally a valued customer, then will delete the discount attached to that customer
            switch (val)
            {
                case "valued":
                    switch (disc)
                    {
                        case "Fixed":
                            await db.InQuery(
                                "DELETE " +
                                "FROM fixed_discount " +
                                "WHERE DiscountCustomeraccount_number = @val0"
                                , selectedAcc);
                            break;
                        case "Variable":
                            await db.InQuery(
                                "DELETE " +
                                "FROM variable_discount " +
                                "WHERE DiscountCustomeraccount_number = @val0; "
                                , selectedAcc);
                            break;
                        case "Flexible":
                            await db.InQuery(
                                "DELETE " +
                                "FROM flexible_discount " +
                                "WHERE DiscountCustomeraccount_number = @val0; "
                                , selectedAcc);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            //after deleting children, delting the parent table
            await db.InQuery("DELETE FROM discount WHERE Customeraccount_number = @val0;", selectedAcc);


            if ((bool)standard_radioBtn.IsChecked)
            {
                await db.InQuery("UPDATE Customer SET customer_status = \"standard\" WHERE account_number = @val0;", selectedAcc);
            }
            else
            {
                //sets them to valued
                await db.InQuery("UPDATE Customer SET customer_status = \"valued\" WHERE account_number = @val0;", selectedAcc);
                await db.InQuery("INSERT INTO discount (discount_plan, Customeraccount_number)  VALUES (@val0, @val1)", discount_Dropdown.Text, selectedAcc);

                switch (discount_Dropdown.Text)
                {
                    case "Fixed":
                        await db.InQuery(
                            "INSERT INTO fixed_discount (DiscountCustomeraccount_number, discount_rate) " +
                            "VALUES (@val0, @val1)"
                            , selectedAcc, fixed_txtBox.Text);
                        break;
                    case "Variable":
                        foreach (System.Data.DataRowView dr in variGrid.ItemsSource)
                        {
                            await db.InQuery(
                            "INSERT INTO variable_discount (DiscountCustomeraccount_number, task_type, discount_rate) " +
                            "VALUES (@val0, @val1, @val2); "
                            , selectedAcc
                            , dr.Row.Field<int>("task_id")
                            , dr.Row.Field<string>("discount_rate")
                            );
                        }
                        break;
                    case "Flexible":
                        int bnum = 1;
                        foreach (System.Data.DataRowView dr in flexGrid.ItemsSource)
                        {
                            await db.InQuery(
                            "INSERT INTO flexible_discount (band_number, DiscountCustomeraccount_number, discount_rate, lower, upper) " +
                            "VALUES (@val0, @val1, @val2, @val3,@val4); "
                            , bnum
                            , selectedAcc
                            , dr.Row.Field<int>("discount_rate")
                            , dr.Row.Field<int>("lower")
                            , dr.Row.Field<int>("upper")
                            );
                            bnum++;
                        }
                        break;
                    default:
                        break;
                }                
            }
            MessageBox.Show("Save successfull");
        }

        private void custGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
