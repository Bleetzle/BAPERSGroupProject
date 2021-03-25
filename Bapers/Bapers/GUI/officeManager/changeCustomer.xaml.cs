using System;
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

        public changeCustomer()
        {
            InitializeComponent();
            Populate();
        }

        private async void Populate()
        {
            await db.Select(custGrid, "SELECT account_number, first_name, last_name, phone_number, address, company_name FROM Customer");
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
            if (custGrid.ItemsSource != null) {

                //((DataView)custGrid.ItemsSource).RowFilter = searchbox.Text;

            }
        }

        /// <summary>
        /// whenever there is a change in the customer table, then the function is called to check all the details of the customer and adapt the page accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void onCustChange(object sender, SelectedCellsChangedEventArgs e)
        {
            if (variGrid.ItemsSource != null) {
                DataTable dt1 = new DataTable();
                dt1 = ((DataView)variGrid.ItemsSource).ToTable();
                dt1.Rows.Clear();
                variGrid.ItemsSource = dt1.DefaultView;
                variGrid.DataContext = dt1.DefaultView;
            }
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
                                            "SELECT band_number, lower, upper, discount_rate " +
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
        private void saveChanges(object sender, RoutedEventArgs e)
        {
            //do the save
        }
    }
}
