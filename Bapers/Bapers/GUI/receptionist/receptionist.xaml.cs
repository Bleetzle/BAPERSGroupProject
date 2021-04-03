using System.Windows;

namespace Bapers.GUI
{
    /// <summary>
    /// Interaction logic for receptionist.xaml
    /// </summary>
    public partial class receptionist : Window
    {
        
        public receptionist()
        {
            InitializeComponent();
            //checks if there is a history, if there is, backbutton is visible
            if (myVariables.myStack.Count ==0)
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

        private void searchAcc_click(object sender, RoutedEventArgs e)
        {
            searchAcc searchaccWindow = new searchAcc();
            searchaccWindow.Show();
            this.Close();
        }

        private void addPay_click(object sender, RoutedEventArgs e)
        {
            payment paymentWindow = new payment();
            paymentWindow.Show();
            this.Close();
        }

        private void createAcc_click(object sender, RoutedEventArgs e)
        {
            createAcc createaccWindow = new createAcc();
            createaccWindow.Show();
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
    }
}
