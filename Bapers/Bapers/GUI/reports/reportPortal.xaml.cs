using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Bapers.GUI.reports
{
    /// <summary>
    /// Interaction logic for reportPortal.xaml
    /// </summary>
    public partial class reportPortal : Window
    {
        DatabaseConnector db = new DatabaseConnector();
        public reportPortal()
        {
            InitializeComponent();
            //back button is only visible since its accessed by two accounts
            if (myVariables.myStack.Count == 0)
            {
                back_btn.Visibility = Visibility.Hidden;
            }
            else
            {
                back_btn.Visibility = Visibility.Visible;
            }
            custNo_txt.Visibility = Visibility.Hidden;
            custNo_txtBox.Visibility = Visibility.Hidden;
        }


        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        

        

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void viewReport_Click(object sender, RoutedEventArgs e)
        {
            bool automatic = false;
            string path = "";
            //input validation
            if (reportType_comboBox.Text.Equals("") || timeSpan.Text.Equals("") || start_date.SelectedDate == null)
            {
                System.Windows.MessageBox.Show("Please fill in all areas");
                return;
            }

            if ((bool)autoGen_checkbox.IsChecked)
            {
                automatic = true;
                //store the date in the database with path to folder
            }

            //gets a directory to store the files in
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Chose a folder to store reports";
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    path = fbd.SelectedPath;
                }
            }
            
            viewReport viewReportWindow = new viewReport(reportType_comboBox.Text, custNo_txtBox.Text, timeSpan.Text, start_date.SelectedDate.Value, automatic, path);
            
            viewReportWindow.Show();
            this.Close();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            switch (myVariables.myStack.Pop())
            {
                case "Shift Manager":
                    shiftManager.shiftManager shiftManagerWindow = new shiftManager.shiftManager();
                    shiftManagerWindow.Show();
                    break;
                case "Office Manager":
                    officeManager.officeManagerPortal officeManagerWindow = new officeManager.officeManagerPortal();
                    officeManagerWindow.Show();
                    break;
                default:
                    System.Windows.MessageBox.Show("Something went wrong, History not found");
                    break;
            }
            this.Close();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
    
        }

        

        private void onChange(object sender, System.EventArgs e)
        {
            if (reportType_comboBox.Text == "Individual")
            {
                custNo_txt.Visibility = Visibility.Visible;
                custNo_txtBox.Visibility = Visibility.Visible;
            }
            else
            {
                custNo_txt.Visibility = Visibility.Hidden;
                custNo_txtBox.Visibility = Visibility.Hidden;
            }
        }

        private void onChange(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
