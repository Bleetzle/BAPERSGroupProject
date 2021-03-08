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


namespace Bapers
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DatabaseChecker : Window 
    {
        DatabaseConnector db = new DatabaseConnector();


        public DatabaseChecker()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // db.Select(dataGrid,"SELECT * FROM Job");
            //db.Backup();
            db.Restore("2021-3-8-14-13-35-750");
        }
    }
}
