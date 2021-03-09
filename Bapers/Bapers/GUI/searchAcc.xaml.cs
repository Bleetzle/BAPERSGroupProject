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

namespace Bapers.GUI
{
    /// <summary>
    /// Interaction logic for searchAcc.xaml
    /// </summary>
    public partial class searchAcc : Window
    {
        public searchAcc()
        {
            InitializeComponent();
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

        private void back_Click(object sender, RoutedEventArgs e)
        {
            receptionist receptionistwindow = new receptionist();
            receptionistwindow.Show();
            this.Close();
        }

        private void search_click(object sender, RoutedEventArgs e)
        {
            bool isfound = true;
            //The code for searching the db for the account goes here



            if (isfound)
            {
                accountFound accountfoundwindow = new accountFound();
                accountfoundwindow.Show();
                this.Close();
            }
            else {
                accNotFound accnotfoundWindow = new accNotFound();
                accnotfoundWindow.Show();
                this.Close();
            }
        }
    }
}
