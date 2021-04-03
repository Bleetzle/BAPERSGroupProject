using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Bapers.GUI
{
    /// <summary>
    /// Interaction logic for payment.xaml
    /// </summary>
    public partial class payment : Window
    {
        DatabaseConnector db = new DatabaseConnector();
        Dictionary<string, float> selectedJobs = new Dictionary<string, float>();
        float subTotal = 0f;

        public payment()
        {
            InitializeComponent();

            //assuming the page is created after searching
            addpaymentfor.Text += " " + myVariables.currfname + " " + myVariables.currlname;
            Populate();

        }

        private async void Populate()
        {
            await db.Select(paymentGrid, 
                "SELECT job_number as ID, deadline as Deadline, special_instructions as Instructions, job_completed as Status, discounted_total as Price " +
                "FROM Job " +
                "WHERE Customeraccount_number = @val0 " +
                "AND Customerphone_number = @val1 " +
                "AND job_status = \"Completed\" "+
                "ORDER BY deadline;"
                , myVariables.currID, myVariables.currnum);
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        //the next two functions are used so the correct gui shown when either of the two is selected
        private void card_Checked(object sender, RoutedEventArgs e)
        {
            cardType.Visibility = Visibility.Visible;
            cardType_txtbox.Visibility = Visibility.Visible; 
            cardNum.Visibility = Visibility.Visible;
            cardNum_txtbox.Visibility = Visibility.Visible;

            expDate.Visibility = Visibility.Visible;
            expDateCal.Visibility = Visibility.Visible;
            cvc.Visibility = Visibility.Visible;
            cvc_txtbox.Visibility = Visibility.Visible;
        }

        private void cash_Checked(object sender, RoutedEventArgs e)
        {
            cardType.Visibility = Visibility.Hidden;
            cardType_txtbox.Visibility = Visibility.Hidden;
            cardNum.Visibility = Visibility.Hidden;
            cardNum_txtbox.Visibility = Visibility.Hidden;
            expDate.Visibility = Visibility.Hidden;
            expDateCal.Visibility = Visibility.Hidden;
            cvc.Visibility = Visibility.Hidden;
            cvc_txtbox.Visibility = Visibility.Hidden;
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            receptionist receptionistwindow = new receptionist();
            receptionistwindow.Show();
            this.Close();
        }

        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private async void pay_Click(object sender, RoutedEventArgs e)
        {
            if (selectedJobs.Count == 0)
            {
                MessageBox.Show("Please select some jobs to pay for");
                return;
            }

            //check if all values are filled in
            if ((bool)card_rdioBtn.IsChecked)
            {
                //check the text boxes
                if (cardType_txtbox.Text.Equals("")|| !expDateCal.SelectedDate.HasValue || cvc_txtbox.Text.Equals(""))
                {
                    MessageBox.Show("Please fill in all details");
                    return;
                }

                //create a payment entry for each job
                foreach (KeyValuePair<string, float> p in selectedJobs.ToList())
                {
                    //stores the payment for each of the jobs
                    await db.InQuery(
                        "INSERT INTO payment (payment_type, payment_amount, Jobjob_number, Customerphone_number, Customeraccount_number, payment_date)" +
                        "Values (@val0, @val1, @val2, @val3, @val4, @val5)"
                        , "Card", p.Value, p.Key, myVariables.currnum, myVariables.currID, DateTime.Now.Date);
                    //gets the payment ID just 
                    var val = await db.SelectSingle(
                        "SELECT payment_id " +
                        "FROM payment " +
                        "WHERE Jobjob_number = @val0"
                        , p.Key);
                    //stores the card details
                    await db.InQuery(
                        "INSERT INTO card (Paymentpayment_id, card_type, expiry_date, CC4digits, CardNum)" +
                        "Values (@val0, @val1, @val2, @val3, @val4)"
                        ,val, cardType_txtbox.Text, expDateCal.SelectedDate.Value.Date, cvc_txtbox.Text, cardNum_txtbox.Text);
                    //archives the Jobs
                    await db.InQuery(
                        "UPDATE job " +
                        "SET job_status = 'Archived' " +
                        "WHERE job_number = @val0; "
                        , p.Key);
                }

                MessageBox.Show("Jobs Sucessfully Paid For");
                //repopulates the tables
                Populate();
            }
            //cash query
            else
            {
                //create a payment entry for each job
                foreach (KeyValuePair<string, float> p in selectedJobs.ToList())
                {
                    //stores the payment for each of the jobs
                    await db.InQuery(
                        "INSERT INTO payment (payment_type, payment_amount, Jobjob_number, Customerphone_number, Customeraccount_number, payment_date)" +
                        "Values (@val0, @val1, @val2, @val3, @val4, @val5)"
                        ,"Cash", p.Value, p.Key, myVariables.currnum, myVariables.currID, DateTime.Now.Date);
                    //archives the Jobs
                    await db.InQuery(
                        "UPDATE job " +
                        "SET job_status = 'Archived' " +
                        "WHERE job_number = @val0; "
                        ,p.Key );
                }

                MessageBox.Show("Jobs Sucessfully Paid For");
                //repopulates the tables
                Populate();
            }
        }

        //adds and removes the job number selected by the user toa list
        private void onChange(object sender, SelectedCellsChangedEventArgs e)
        {       
            string tmp = "";
            float tmp1 = -1; 

            foreach (var item in e.AddedCells)
            {
                if (item.Column != null)
                {
                    string col = item.Column.Header.ToString();
                    //tmp job num to be able to create the map value

                    //assuming job num always appears before the price
                    if (col.Equals("ID") && item.Column.GetCellContent(item.Item) != null)
                    {
                        tmp = (item.Column.GetCellContent(item.Item) as TextBlock).Text;
                    }
                    if (col.Equals("Price") && item.Column.GetCellContent(item.Item) != null)
                    {
                        tmp1 = float.Parse((item.Column.GetCellContent(item.Item) as TextBlock).Text);
                        subTotal += tmp1;
                    }
                    //makes sure a job number has been selected before adding to the list
                    if (!tmp.Equals("") && tmp1 != -1)
                    {
                        selectedJobs.Add(tmp, tmp1);
                        tmp = "";
                        tmp1 = -1;
                    }
                }
            }
            //does it for each of the removed cells
            foreach (var item in e.RemovedCells)
            {
                if (item.Column != null)
                {
                    string col = item.Column.Header.ToString();

                    if (col.Equals("ID") && item.Column.GetCellContent(item.Item) != null)
                    {
                        tmp = (item.Column.GetCellContent(item.Item) as TextBlock).Text;
                    }
                    if (col.Equals("Price") && item.Column.GetCellContent(item.Item) != null)
                    {
                        tmp1 = float.Parse((item.Column.GetCellContent(item.Item) as TextBlock).Text);
                        subTotal -= tmp1;
                    }
                    //makes sure a job number has been deselected before removing to the list
                    if (!tmp.Equals(""))
                    {
                        selectedJobs.Remove(tmp);
                        tmp = "";
                    }
                }
            }

            subTotaltxt.Text = subTotal.ToString();
        }
    }
}
