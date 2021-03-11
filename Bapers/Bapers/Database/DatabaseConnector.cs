using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace Bapers
{
    class DatabaseConnector 
    {
        private MySqlConnection connection;
        private string server = "localhost";
        private string database = "BAPERS";
        private string username = "root";
        private string password = "password123";


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

        //runs a query and fills in a grid
        public void Select(DataGrid dg, string q)
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

        //runs a query and fills in a grid
        public string SelectSingle(string q)
        {
            string value = " ";
            try
            {
                MySqlCommand cmd = new MySqlCommand(q, connection);

                if (this.OpenConnection() == true)
                {
                    value = cmd.ExecuteScalar().ToString();
              
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
            return value;
        }


        //runs a query and returns true or false based on if the query finds data
        public bool Check(string q)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(q, connection);
                
                if (this.OpenConnection() == true)
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                        return true;
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
            return false;
        }

        //runs a query to insert data into the database
        public void InQuery(string q)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(q, connection);

                if (this.OpenConnection() == true)
                {
                    cmd.ExecuteNonQuery();
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

        public void Backup()
        {
            try
            {
                DateTime time = DateTime.Now;
                int year = time.Year;
                int month = time.Month;
                int day = time.Day;
                int hour = time.Hour;
                int minute = time.Minute;
                int second = time.Second;
                int millisecond = time.Millisecond;

                string path; 
                path = @"C:\\DatabaseBackups\\MySqlBackup" + year + "-" + month + "-" + day + "-" + hour + "-" + minute + "-" + second + "-" + millisecond + ".sql";
                StreamWriter file = new StreamWriter(path);

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = @"C:\\Program Files\\MySQL\\MySQL Server 8.0\\bin\\mysqldump.exe";
                psi.RedirectStandardInput = false;
                psi.RedirectStandardOutput = true;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", username, password, server, database);
                psi.UseShellExecute = false;

                Process process = Process.Start(psi);
                string output;
                output = process.StandardOutput.ReadToEnd();
                file.WriteLine(output);
                process.WaitForExit();


                file.Close();
                process.Close();
                
                MessageBox.Show("Backup Success!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error, unable to Backup. {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //made it so then we can just use a search and select function to senf throught the rest of the name that is unkown to the user
        public void Restore(string p)
        {
            try
            {
                string path = @"C:\\DatabaseBackups\\MySqlBackup" + p + ".sql";
                StreamReader file = new StreamReader(path);
                string input = file.ReadToEnd();
                file.Close();

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = @"C:\\Program Files\\MySQL\\MySQL Server 8.0\\bin\\mysql.exe";
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = false;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", username, password, server, database);
                psi.UseShellExecute = false;

                Process process = Process.Start(psi);
                process.StandardInput.WriteLine(input);
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();
                MessageBox.Show("Restore Success!");
            }
            catch (IOException ex)
            {
                MessageBox.Show(string.Format("Error, unable to Restore. \n Backup with that filename does not exist. \n {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
