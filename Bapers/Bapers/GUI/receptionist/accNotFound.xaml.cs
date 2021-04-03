using System.Windows;

namespace Bapers.GUI
{
    /// <summary>
    /// Interaction logic for accNotFound.xaml
    /// </summary>
    public partial class accNotFound : Window
    {

        //this popup box is used as an intermediary when account is not found through search
        public accNotFound()
        {
            InitializeComponent();
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {
            searchAcc searchaccWindow = new searchAcc();
            searchaccWindow.Show();
            this.Close();
        }

        private void createAcc_Click(object sender, RoutedEventArgs e)
        {
            createAcc createaccWindow = new createAcc();
            createaccWindow.Show();
            this.Close();
        }
    }
}
