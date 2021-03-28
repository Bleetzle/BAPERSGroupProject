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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bapers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Login : Window
    {

        DatabaseConnector db = new DatabaseConnector();
        public Login()
        {
            myVariables.myStack.Clear();
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void login_Click(object sender, RoutedEventArgs e)
        {
            credentialChecker(username_txtBox.Text, password_txtBox.Password);
            //variables passed from username and password textbox
        }

        private async void credentialChecker(string username, string password)
        {
            if (username_txtBox.Text.Equals("") || password_txtBox.Password.Equals(""))
            {
                MessageBox.Show("Please fill in all areas");
                return;
            }
            //code for searching database for the username and password

            var num = await db.SelectSingle("SELECT userID FROM users WHERE username = @val0 AND pass = @val1", username_txtBox.Text, db.StringToHash(password_txtBox.Password));
            myVariables.num = num;

            //need to change this to also store the information of whos logged in
            if (!num.Equals("null"))
            {
                string role = await db.SelectSingle("SELECT role FROM staff WHERE staff_ID = @val0", num); ;
                myVariables.role = role;


                //check user roles upon logging in
                switch (role)
                {
                    case "Receptionist":
                        GUI.receptionist receptionistwindow = new GUI.receptionist();
                        receptionistwindow.Show();
                        break;
                    case "Technician":
                        //need to check with the id if there is any quesries to remove
                        List<string> qs= new List<string>();
                        await db.SelectLists(qs, "select question_id FROM resolvedQuestions where staff_id = @val0;", myVariables.num);
                        await db.SelectLists(qs, "DELETE FROM resolvedQuestions where staff_id = @val0;", myVariables.num);
                        foreach (string s in qs)
                            await db.InQuery("UPDATE Questions SET status = \"Archived\" WHERE question_id = @val0 ", int.Parse(s));
                        GUI.technician.technicianPortal technicianWindow = new GUI.technician.technicianPortal();
                        technicianWindow.Show();
                        break;
                    case "Shift Manager":
                        GUI.shiftManager.shiftManager shiftManagerWindow = new GUI.shiftManager.shiftManager();
                        shiftManagerWindow.Show();
                        break;
                    case "Office Manager":
                        //check for automatic backup to do
                        var nxtBUDate = await db.SelectSingle("SELECT MAX(DateofNext) FROM BackupHistory;");
                        if (!nxtBUDate.Equals("") && DateTime.Parse(nxtBUDate).Date == DateTime.Now.Date)
                        {
                            var val = await db.SelectSingle("SELECT id, MAX(DateOfNext) FROM BackupHistory GROUP BY id ORDER BY  DateOfNext DESC;");
                            List<string> BUinfo = new List<string>();
                            await db.SelectLists(BUinfo,
                                "SELECT backup_date FROM BackupHistory WHERE id = @val0 " +
                                "UNION SELECT automatically_backed FROM BackupHistory WHERE id = @val0  " +
                                "UNION SELECT DateOfNext FROM BackupHistory WHERE id = @val0 " +
                                "UNION SELECT timeSpan FROM BackupHistory WHERE id = @val0 " +
                                "UNION SELECT savePath FROM BackupHistory WHERE id = @val0; "
                                , val);
                            db.Backup(BUinfo[4]);
                            await db.InQuery("INSERT INTO BackupHistory (backup_date, automatically_backed, DateOfNext, timeSpan, savePath) VALUES (@val0,@val1,@val2,@val3,@val4)"
                                , DateTime.Now.Date, true, DateTime.Now.Date.AddDays(int.Parse(BUinfo[3])), BUinfo[3], BUinfo[4] );
                            MessageBox.Show("Automatic backup created \n Next backup in: " + int.Parse(BUinfo[3]) + "Days");
                        }
                        //check for automatic report to backup
                        var nxtRepDate = await db.SelectSingle("SELECT MAX(DateOfNext) FROM ReportHistory;");
                        if (!nxtRepDate.Equals("") && DateTime.Parse(nxtRepDate).Date == DateTime.Now.Date)
                        {
                            var val = await db.SelectSingle("SELECT id, MAX(DateofNext) FROM ReportHistory GROUP BY id ORDER BY  DateOfNext DESC;");
                            List<string> repinfo = new List<string>();
                            await db.SelectLists(repinfo,
                                "SELECT report_date FROM ReportHistory WHERE id = @val0 " +
                                "UNION SELECT report_type FROM ReportHistory WHERE id = @val0  " +
                                "UNION SELECT automatically_generated FROM ReportHistory WHERE id = @val0 " +
                                "UNION SELECT DateOfNext FROM ReportHistory WHERE id = @val0 " +
                                "UNION SELECT timeSpan FROM ReportHistory WHERE id = @val0 " +
                                "UNION SELECT savePath FROM ReportHistory WHERE id = @val0 " +
                                "UNION SELECT userID FROM ReportHistory WHERE id = @val0; "
                                , val); 

                            await db.generateReport(repinfo[1], new DataGrid(), repinfo[6], int.Parse(repinfo[4]), DateTime.Parse(repinfo[0]),Convert.ToBoolean(int.Parse(repinfo[2])), repinfo[5]);
                            MessageBox.Show("Automatic report created \n Next backup in: " + int.Parse(repinfo[4]) + "Days");
                        }
                      
                        GUI.officeManager.officeManagerPortal officeManagerWindow = new GUI.officeManager.officeManagerPortal();
                        officeManagerWindow.Show();
                        break;
                    default:
                        MessageBox.Show("Something went wrong, no role assigned to user");
                        Login loginWindow = new Login();
                        loginWindow.Show();
                        break;
                }
                this.Close();
                //account found, switch to the account portal
            }
            else
            {
                //show error message
                System.Windows.Forms.MessageBox.Show("Account not found, Please check details are correct");
            }
        }

    }
}
