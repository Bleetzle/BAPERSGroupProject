using System.Windows;
using MySql.Data.MySqlClient;

namespace Bapers
{
    class DatabaseConnector
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string username;
        private string password;

        public DatabaseConnector()
        {
            Initialise();
        }


        private void Initialise()
        {
            server = "localhost";
            database = "BAPERS";
            username = "root";
            password = "password123";

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

        public void Insert()
        {
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
    }
}
