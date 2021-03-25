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

namespace Bapers.GUI.shiftManager
{
    /// <summary>
    /// Interaction logic for assign.xaml
    /// </summary>
    public partial class assign : Window
    {
        public assign()
        {
            InitializeComponent();
        }

        private void assign_Click(object sender, RoutedEventArgs e)
        {

        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            shiftManager shiftManagerWindow = new shiftManager();
            shiftManagerWindow.Show();
            this.Close();
        }

        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private void onUsersChange(object sender, SelectedCellsChangedEventArgs e)
        {

        }

        private void onTaskChange(object sender, SelectedCellsChangedEventArgs e)
        {

        }
    }
}
