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

namespace Bapers.GUI.reports
{
    /// <summary>
    /// Interaction logic for reportPortal.xaml
    /// </summary>
    public partial class reportPortal : Window
    {
        public reportPortal()
        {
            InitializeComponent();
            if (myVariables.myStack.Count == 0)
            {
                back_btn.Visibility = Visibility.Hidden;
            }
            else
            {
                back_btn.Visibility = Visibility.Visible;
            }
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

        private void individual_Checked(object sender, RoutedEventArgs e)
        {
            userId_txt.Visibility = Visibility.Visible;
            userId_txtBox.Visibility = Visibility.Visible;
        }

        private void individual_unchekced(object sender, RoutedEventArgs e)
        {
            userId_txt.Visibility = Visibility.Hidden;
            userId_txtBox.Visibility = Visibility.Hidden;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void viewReport_Click(object sender, RoutedEventArgs e)
        {
            viewReport viewReportWindow = new viewReport();
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
                    MessageBox.Show("Something went wrong, History not found");
                    break;
            }
            this.Close();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        //selecttion sometimes doesnt happen depending on where the radio button is pressed. 
        //note tell prince about this
        }
    }
}
