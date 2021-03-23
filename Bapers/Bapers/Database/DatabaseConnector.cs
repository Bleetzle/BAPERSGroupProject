using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;

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
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["BAPERS"].ConnectionString;
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
        public async Task Select(DataGrid dg, string q, params object[] vals)
        {
            DataTable dataTable = new DataTable();

            try
            { 
                MySqlCommand cmd = new MySqlCommand(q, connection);

                for (int i = 0; i < vals.Length; i++)
                {
                  cmd.Parameters.AddWithValue($"@val{i}", vals[i]);
                }

                if (this.OpenConnection() == true)
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        await da.FillAsync(dataTable);
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

        //runs a query and fills in a list for the first row of a query
        public async Task SelectLists(List<string> list, string q)
        {
            DataTable dataTable = new DataTable();

            try
            {
                MySqlCommand cmd = new MySqlCommand(q, connection);

                if (this.OpenConnection() == true)
                {

                    using (MySqlDataReader dr = (MySqlDataReader)await cmd.ExecuteReaderAsync() )
                    {
                        
                        if (dr.HasRows)
                        {
                            while(await dr.ReadAsync())
                            {
                                list.Add(dr.GetString(0));   
                            }
                        }
                    }
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


        //runs a query and selects a single item -- slightly quicker to run than the first method as there is no grid
        public async Task<string> SelectSingle(string q, params object[] vals)
        {
            string value = "null";
            try
            {
                MySqlCommand cmd = new MySqlCommand(q, connection);

                for (int i = 0; i < vals.Length; i++)
                {
                    cmd.Parameters.AddWithValue($"@val{i}", vals[i]);
                }

                if (this.OpenConnection() == true)
                {
                    var val = await cmd.ExecuteScalarAsync();
                    if (val != null)
                        value = val.ToString();
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

        ///runs a query and returns true or false based on if the query finds data 
        ///does not return the data found, just ot chek if a table search exists before searching or it, 
        ///takes more tiem if they exists and less time if they dont exist
        public async Task<bool> Check(string q, params object[] vals)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(q, connection);

                for (int i = 0; i < vals.Length; i++)
                {
                    cmd.Parameters.AddWithValue($"@val{i}", vals[i]);
                }

                if (this.OpenConnection() == true)
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    return reader.HasRows;
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

        //method to hash a given string using a hashing algorithm
        public string StringToHash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();

            byte[] bytes = md5Provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            foreach(byte b in bytes)
            {
                hash.Append(b.ToString("x2"));
            }
            return hash.ToString();
        }

        //runs a query to insert data into the database
        public async Task InQuery(string q, params object[] vals)
        {
            try
            { 
                MySqlCommand cmd = new MySqlCommand(q, connection);
                
                for (int i = 0; i < vals.Length; i++)
                {
                    cmd.Parameters.AddWithValue($"@val{i}", vals[i]);
                }

                if (this.OpenConnection() == true)
                {
                   await cmd.ExecuteNonQueryAsync();
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

        public void Backup(string path)
        {
            try
            {
                string username = System.Configuration.ConfigurationManager.AppSettings.Get("USERNAME");
                string password = System.Configuration.ConfigurationManager.AppSettings.Get("PASSWORD");
                string server = System.Configuration.ConfigurationManager.AppSettings.Get("SERVER");
                string database = System.Configuration.ConfigurationManager.AppSettings.Get("DATABASE");

                DateTime time = DateTime.Now;
                int year = time.Year;
                int month = time.Month;
                int day = time.Day;
                int hour = time.Hour;
                int minute = time.Minute;
                int second = time.Second;
                int millisecond = time.Millisecond;

                path += @"\\MySqlBackup" + year + "-" + month + "-" + day + "-" + hour + "-" + minute + "-" + second + "-" + millisecond + ".sql";
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
        public void Restore(string path)
        {
            try
            {
                string username = System.Configuration.ConfigurationManager.AppSettings.Get("USERNAME");
                string password = System.Configuration.ConfigurationManager.AppSettings.Get("PASSWORD");
                string server = System.Configuration.ConfigurationManager.AppSettings.Get("SERVER");
                string database = System.Configuration.ConfigurationManager.AppSettings.Get("DATABASE");

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
