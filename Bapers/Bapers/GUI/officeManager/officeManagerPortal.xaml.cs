using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bapers.GUI.officeManager
{
    /// <summary>
    /// Interaction logic for officeManagerPortal.xaml
    /// </summary>
    public partial class officeManagerPortal : Window
    {
        DatabaseConnector db = new DatabaseConnector();
        public officeManagerPortal()
        {
            myVariables.myStack.Push("Office Manager");
            InitializeComponent();
            deadlineChecker();
        }

        private async void deadlineChecker()
        {
            bool deadline = await db.Check("select job_Number, deadline, job_status from job where job_status = 'Uncompleted' AND deadline between curdate() AND date_format(curdate() + 4, '%Y-%m-%d')");
            if (deadline)
            {
                var list1 = new List<string>();
                var list2 = new List<string>();
                await db.SelectLists(list1, "select job_Number from job where job_status = 'Uncompleted' AND deadline between curdate() AND date_format(curdate() + 4, '%Y-%m-%d')");
                await db.SelectLists(list2, "select deadline from job where job_status = 'Uncompleted' AND deadline between curdate() AND date_format(curdate() + 4, '%Y-%m-%d')");
                string notification= "Deadline coming soon for \n";
               for(int i = 0; i < list1.Count(); i++)
                {
                    notification = notification + list1[i] + "   Due at: " + list2[i] + "\n";
                }
                System.Windows.Forms.MessageBox.Show(notification);
            }
            
        }

        private void create_click(object sender, RoutedEventArgs e)
        {
            createUser createUserWindwo = new createUser();
            createUserWindwo.Show();
            this.Close();
        }

        private void changeCustomer_click(object sender, RoutedEventArgs e)
        {
            changeCustomer changeCustomerWindow = new changeCustomer();
            changeCustomerWindow.Show();
            this.Close();
        }

        private void reception_click(object sender, RoutedEventArgs e)
        {
            receptionist receptionistWindwo = new receptionist();
            receptionistWindwo.Show();
            this.Close();
        }

        private void report_click(object sender, RoutedEventArgs e)
        {
            reports.reportPortal reportPortalWindow = new reports.reportPortal();
            reportPortalWindow.Show();
            this.Close();
        }

        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            Login loginwindow = new Login();
            loginwindow.Show();
            this.Close();
        }

        private void backup_click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Chose a folder to store";

                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string path = fbd.SelectedPath;
                    db.Backup(path);
                }
            }  
        }

        private void restore_click(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string path = openFileDialog.FileName;
                    db.Restore(path);
                }
            }
        }

        private void addtasks_click(object sender, RoutedEventArgs e)
        {
            addTasks addtasksWindow = new addTasks();
            addtasksWindow.Show();
            this.Close();

        }
    }
}
