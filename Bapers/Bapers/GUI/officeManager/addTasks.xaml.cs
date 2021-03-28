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
    /// Interaction logic for addTasks.xaml
    /// </summary>
    public partial class addTasks : Window
    {
        DatabaseConnector db = new DatabaseConnector();
        public addTasks()
        {
            InitializeComponent();
            populate();
        }
        private async void populate()
        {
             await db.Select(taskGrid, "SELECT * FROM Tasks;");
        }


        private async void add_Click(object sender, RoutedEventArgs e)
        {
         

            try
            {
                await db.InQuery("INSERT INTO Tasks (task_description,location,task_duration, price,amount)VALUES(@val0, @val1, @val2,@val3,@val4)", description_txtBox.Text, location_txtBox.Text, duration_txtBox.Text, price_txtBox.Text,0);
                MessageBox.Show("Successfully added task");
                description_txtBox.Clear();
                location_txtBox.Clear();
                duration_txtBox.Clear();
                price_txtBox.Clear();
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to add task");
                throw;
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            officeManagerPortal officeManagerWindow = new officeManagerPortal();
            officeManagerWindow.Show();
            this.Close();
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

        private void taskGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void searchChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ontaskChange(object sender, SelectedCellsChangedEventArgs e)
        {

        }
    }
}
