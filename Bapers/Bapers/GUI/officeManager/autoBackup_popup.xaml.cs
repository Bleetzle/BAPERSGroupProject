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

namespace Bapers.GUI.officeManager
{
    /// <summary>
    /// Interaction logic for autoBackup_popup.xaml
    /// </summary>
    public partial class autoBackup_popup : Window
    {
        DatabaseConnector db = new DatabaseConnector();
        string savePath = "";
        public autoBackup_popup(string path)
        {
            InitializeComponent();
            savePath = path;
        }

        private async void Submit(object sender, RoutedEventArgs e)
        {
            //window created when auto backup is selected
            if (timeSpan.Text.Equals(""))
            {
                MessageBox.Show("Please Select an option");
            }
            int val = int.Parse(timeSpan.Text);
            //run query on how many days it should auto back up inbetween
            await db.InQuery("INSERT INTO BackupHistory (backup_date, automatically_backed, DateOfNext, timeSpan, savePath) VALUES (@val0,@val1,@val2,@val3,@val4)", DateTime.Now.Date, true, DateTime.Now.Date.AddDays(val), timeSpan.Text,savePath);

            db.Backup(savePath);
            this.Close();
        }
    }
}
