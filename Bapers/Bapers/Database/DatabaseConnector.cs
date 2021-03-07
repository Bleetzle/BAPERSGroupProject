using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace Bapers
{
    class DatabaseConnector 
    {
        private MySqlConnection connection;
       
        public DatabaseConnector()
        {
            Initialise();
        }


        private void Initialise()
        {

            string server = "localhost";
            string database = "BAPERS";
            string username = "root";
            string password = "password123";

            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "USERNAME=" + username + ";" + "PASSWORD=" + password;
            connection = new MySqlConnection(connectionString);
        }

        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number) {
                    default:
                        MessageBox.Show("Unknown Error");
                        break;
                    case 0:
                        MessageBox.Show("Cannot connect to server. Contact administrator");
                        break;
                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        //runs a query
        public void Query(DataGrid dg, string q)
        {
            DataTable dataTable = new DataTable();

            try
            { 
                MySqlCommand cmd = new MySqlCommand(q, connection);
                
                if (this.OpenConnection() == true)
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        da.Fill(dataTable);
                    }
                    dg.DataContext = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An error occurred {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.CloseConnection();
            }
        }


        public void Update()
        {
        }

        public void Delete()
        {
        }

        public void Backup()
        {
        }

        public void Restore()
        {
        }



        //TEMPORARY

        public bool checkConnection()
        {
            CloseConnection();
            return OpenConnection();
        }


    }
}
