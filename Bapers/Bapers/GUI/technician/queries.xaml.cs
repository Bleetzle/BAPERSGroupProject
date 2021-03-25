using System;
using System.Collections.Generic;
using System.Data;
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

namespace Bapers.GUI.technician
{
    /// <summary>
    /// Interaction logic for queries.xaml
    /// </summary>
    public partial class queries : Window
    {
        DatabaseConnector db = new DatabaseConnector();
        string selectedQ = "";
        string selectedStatus = "";

        public queries()
        {
            InitializeComponent();
            PopulateQuestions();
        }

        private async void PopulateQuestions()
        {
            await db.Select(QGrid,
                "SELECT question_id, Jobjob_number, Tasktask_ID, description, COALESCE(null, ' ') AS \"--\", first_name, last_name, status " +
                "FROM Questions, Staff " +
                "WHERE Questions.staff_ID = Staff.Staff_ID " +
                "AND status != \"Archived\" ; ");
        }
        private async void PopulateResponces()
        {
            await db.Select(QGrid,
                "SELECT responces.description, COALESCE(null, ' ') AS \"--\", first_name, last_name " +
                "FROM Responces, Questions, Staff " +
                "WHERE Questions.question_id = @val0 " +
                "AND Questions.question_id = Responces.question_id " +
                "AND Responces.staff_ID = Staff.staff_ID;"
                , selectedQ);
        }
        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            technicianPortal technicianPortalWindow = new technicianPortal();
            technicianPortalWindow.Show();
            this.Close();
        }

        private void refresh_click(object sender, RoutedEventArgs e)
        {
            switch (subTitle.Text)
            {
                case "Responces: ":
                    PopulateResponces();
                    break;
                default:
                    PopulateQuestions(); 
                    break;
            }
        }

        private async void searchChanged(object sender, TextChangedEventArgs e)
        {
            if (searchbox.Text.Equals(""))
            {
                PopulateQuestions();
                return;
            }
            await db.Select(QGrid,
                "SELECT question_id, Jobjob_number, Tasktask_ID, description, COALESCE(null, ' ') AS \"--\", first_name, last_name status " +
                "FROM Questions, Staff " +
                "WHERE Jobjob_number = @val0 " +
                "AND Questions.staff_ID = Staff.Staff_ID " +
                "AND status != \"Archived\" ;"
                , searchbox.Text);
        }

        private void onQChange(object sender, SelectedCellsChangedEventArgs e)
        {
            foreach (var item in e.AddedCells)
            {
                if (item.Column != null)
                {
                    string col = item.Column.Header.ToString();
                    //tmp job num to be able to create the map value

                    //assuming id always appears before the price
                    if (col.Equals("question_id") && item.Column.GetCellContent(item.Item) != null)
                    {
                        selectedQ = (item.Column.GetCellContent(item.Item) as TextBlock).Text;
                    }
                    else if (col.Equals("status") && item.Column.GetCellContent(item.Item) != null)
                    {
                        selectedStatus = (item.Column.GetCellContent(item.Item) as TextBlock).Text;
                    }
                }
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            subTitle.Text = "Responces: ";

            searchbox.Visibility = Visibility.Hidden;
            searchIcon.Visibility = Visibility.Hidden;
            QReturn_btn.Visibility = Visibility.Visible;

            jobIDtxt.Visibility = Visibility.Hidden;
            jobID_txtBox.Visibility = Visibility.Hidden;
            tasktxt.Visibility = Visibility.Hidden;
            taskNumber_txtBox.Visibility = Visibility.Hidden;
            qdesctxt.Visibility = Visibility.Hidden;
            qDescription_textBox.Visibility = Visibility.Hidden;
            addQ_btn.Visibility = Visibility.Hidden;

            rdesctxt.Visibility = Visibility.Visible;
            rDescription_textBox.Visibility = Visibility.Visible;
            addR_btn.Visibility = Visibility.Visible;
            markResolved.Visibility = Visibility.Visible;

            if (!selectedStatus.Equals("Pending"))
            {
                rDescription_textBox.IsEnabled = false;
                addR_btn.IsEnabled = false;
                markResolved.IsEnabled = false;
            }
            else
            {
                rDescription_textBox.IsEnabled = true;
                addR_btn.IsEnabled = true;
                markResolved.IsEnabled = true;
            }


            PopulateResponces();
        }

        private void Qreturn_click(object sender, RoutedEventArgs e)
        {
            subTitle.Text = "Questions: ";

            searchbox.Visibility = Visibility.Visible;
            searchIcon.Visibility = Visibility.Visible;
            QReturn_btn.Visibility = Visibility.Hidden;

            jobIDtxt.Visibility = Visibility.Visible;
            jobID_txtBox.Visibility = Visibility.Visible;
            tasktxt.Visibility = Visibility.Visible;
            taskNumber_txtBox.Visibility = Visibility.Visible;
            qdesctxt.Visibility = Visibility.Visible;
            qDescription_textBox.Visibility = Visibility.Visible;
            addQ_btn.Visibility = Visibility.Visible;

            rdesctxt.Visibility = Visibility.Hidden;
            rDescription_textBox.Visibility = Visibility.Hidden;
            addR_btn.Visibility = Visibility.Hidden;
            markResolved.Visibility = Visibility.Hidden;

            PopulateQuestions();
        }

        private async void addQ_click(object sender, RoutedEventArgs e)
        {
            if (jobID_txtBox.Text.Equals("") || taskNumber_txtBox.Text.Equals("") || qDescription_textBox.Text.Equals(""))
            {
                MessageBox.Show("Please fill in all areas");
                return;
            }

            await db.InQuery("INSERT INTO Questions (Jobjob_number, Tasktask_id, description, staff_ID, status) VALUES (@val0, @val1, @val2, @val3, \"Pending\");", jobID_txtBox.Text, taskNumber_txtBox.Text,qDescription_textBox.Text, myVariables.num);
            MessageBox.Show("Question Added Successfully!");
        }

        private async void addR_click(object sender, RoutedEventArgs e)
        {
            //checks box has been filled
            if (rDescription_textBox.Text.Equals(""))
            {
                MessageBox.Show("Please fill in all areas");
                return;
            }
            //adds response to the database
            await db.InQuery("INSERT INTO Responces (question_id, description, staff_ID) VALUES (@val0,@val1,@val2);", selectedQ, rDescription_textBox.Text, myVariables.num);
            
            //check for marked as complete:
            if ((bool)markResolved.IsChecked)
            {
                //update the question and 
                await db.InQuery("UPDATE Questions SET status = \"Resolved\" WHERE question_id = @val0;", selectedQ);
                var tmp = await db.SelectSingle("SELECT staff_id from Questions WHERE question_id = @val0;", selectedQ);
                //add to the resolved table
                await db.InQuery("INSERT INTO resolvedQuestions(question_id, staff_id, resolvedDateTime) VALUES (@val0, @val1, @val2)", selectedQ, int.Parse(tmp), DateTime.Now);
                MessageBox.Show("Job Resolved Successfully!");
            }
            MessageBox.Show("Response Added Successfully!");
        }
    }
}
