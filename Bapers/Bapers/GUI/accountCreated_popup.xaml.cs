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
    /// Interaction logic for accountCreated_popup.xaml
    /// </summary>
    public partial class accountCreated_popup : Window
    {
        public accountCreated_popup()
        {
            InitializeComponent();
        }

        private void yes_Click(object sender, RoutedEventArgs e)
        {
            //goes to the add jobs window
            addJobs addjobswindow = new addJobs();
            addjobswindow.Show();
            this.Close();
        }

        private void no_Click(object sender, RoutedEventArgs e)
        {
            //sends the user back to the reception portal
            receptionist receptionistwindow = new receptionist();
            receptionistwindow.Show();
            this.Close();
        }
    }
}
