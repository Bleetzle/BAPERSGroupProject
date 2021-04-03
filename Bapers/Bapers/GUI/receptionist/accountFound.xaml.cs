using System.Windows;

namespace Bapers.GUI
{
    /// <summary>
    /// Interaction logic for accountFound.xaml
    /// </summary>
    public partial class accountFound : Window
    {
        //this pop up is used as an intermediary when account is found and prompts the user if they would like to pay or add jobs to that account after a search is complete
        public accountFound()
        {
            InitializeComponent();
        }

        private void no_Click(object sender, RoutedEventArgs e)
        {
            searchAcc searchaccWindow = new searchAcc();
            searchaccWindow.Show();
            this.Close();
        }

        private void yes_Click(object sender, RoutedEventArgs e)
        {
            addJobs addjobswindow = new addJobs();
            addjobswindow.Show();
            this.Close();
        }

        private void payment_Click(object sender, RoutedEventArgs e)
        {
            payment paymentWindow = new payment();
            paymentWindow.Show();
            this.Close();
        }
    }
}
