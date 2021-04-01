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
using System.Collections.Generic;
using System.Linq;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;

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

        private async Task<bool> OpenConnection()
        {
            //connecting to the sql connector
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }
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

        private async Task<bool> CloseConnection()
        {
            try
            {
                await connection.CloseAsync();
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
            //select query for sql
            DataTable dataTable = new DataTable();
            try
            { 
                MySqlCommand cmd = new MySqlCommand(q, connection);

                for (int i = 0; i < vals.Length; i++)
                {
                  cmd.Parameters.AddWithValue($"@val{i}", vals[i]);
                }

                if (await OpenConnection() == true)
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        await da.FillAsync(dataTable);
                    }
                    dg.ItemsSource = dataTable.DefaultView;
                    dg.DataContext = dataTable.DefaultView;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An error occurred {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
               await this.CloseConnection();
            }
        }

        //runs a query and fills in a list for the first row of a query
        public async Task SelectLists(List<string> list, string q, params object[] vals)
        {
            DataTable dataTable = new DataTable();

            try
            {
                MySqlCommand cmd = new MySqlCommand(q, connection);

                for (int i = 0; i < vals.Length; i++)
                {
                    cmd.Parameters.AddWithValue($"@val{i}", vals[i]);
                }

                if (await  this.OpenConnection() == true)
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
                await this.CloseConnection();
            }
        }


        //runs a query and selects a single item -- slightly quicker to run than the first method as there is no grid
        public async Task<string> SelectSingle(string q, params object[] vals)
        {
            //this query returns a single row from a specified  sql query
            string value = "null";
            try
            {
                MySqlCommand cmd = new MySqlCommand(q, connection);

                for (int i = 0; i < vals.Length; i++)
                {
                    cmd.Parameters.AddWithValue($"@val{i}", vals[i]);
                }

                if (await this.OpenConnection() == true)
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
                await this.CloseConnection();
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

                if (await this.OpenConnection() == true)
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
                await this.CloseConnection();
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

                if (await OpenConnection() == true)
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
                await this.CloseConnection();
            }
        }

        public void Backup(string path)
        {
            try
            {
                //all data used for creating a backup
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

                //path to the back up
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
                //if making backup fails
                MessageBox.Show(string.Format("Error, unable to Backup. {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //made it so then we can just use a search and select function to senf throught the rest of the name that is unkown to the user
        public void Restore(string path)
        {
            try
            {
                //all data used for restoring from a .sql file
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

        /// <summary>
        /// Generates report and stores it in the given path then returns the string on return so that the report can be opened and viewd if needed;
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<string> generateReport(string reportType, DataGrid dg, string custID, int timespan, DateTime startDate, bool automatic, string path)
        {
            //records Auto generation
            if (automatic)
                await this.InQuery("INSERT INTO ReportHistory (report_date, report_type, automatically_generated, DateOfNext, timeSpan, savePath, userID) VALUES (@val0, @val1, @val2, @val3, @val4, @val5, @val6)", DateTime.Now.Date, reportType, automatic, DateTime.Now.Date.AddDays(timespan), timespan, path, custID);
                            //records generation
            else
                await this.InQuery("INSERT INTO ReportHistory (report_date, report_type, automatically_generated, timespan,userID) VALUES (@val0, @val1, @val2,@val3, @val4)", DateTime.Now.Date, reportType, automatic, timespan, custID);
            
            var endDate = startDate.AddDays(timespan);

            DateTime time = DateTime.Now;
            int year = time.Year;
            int month = time.Month;
            int day = time.Day;
            int hour = time.Hour;
            int minute = time.Minute;
            int second = time.Second;
            int millisecond = time.Millisecond;

            path += year + "-" + month + "-" + day + "-" + hour + "-" + minute + "-" + second + "-" + millisecond + ".pdf";

            //query used to create diffrent report types
            switch (reportType)
            {
                case "Individual Performance":
                    await this.Select(dg,
                        "SELECT first_name, last_name, tasks.location AS Department, job_completed AS Date, start_time, time_taken, t1.total_time AS Total " +
                        "FROM staff, job, Job_Tasks, Tasks, " +
                        "( SELECT SUM(time_taken) as total_time, Staffstaff_ID AS totalStaffID  " +
                        "FROM Job_Tasks, job  " +
                        "WHERE job_status = \"Completed\"  " +
                        "AND Job_number = Jobjob_number  " +
                        "AND job_completed BETWEEN @val0 AND @val1  " +
                        "GROUP BY Staffstaff_ID) t1  " +
                        "WHERE staff_ID = Staffstaff_ID  " +
                        "AND t1.totalStaffID = staff_ID  " +
                        "AND Job_number = Jobjob_number  " +
                        "AND Taskstask_ID = task_ID  " +
                        "AND job_status = \"Completed\"  " +
                        "AND job_completed BETWEEN @val0 AND @val1; "
                        , startDate, endDate);
                    break;
                case "Summary Performance":
                    await this.Select(dg,
                        "SELECT * " +
                        "FROM v1 " +
                        "WHERE Date BETWEEN @val0 AND @val1 " +
                        "AND Shift = \"Day Shift 1\" " +
                        "UNION " +
                        "SELECT coalesce(NULL, ' '), coalesce(NULL, 'Total'), coalesce(SUM(Copy_room), '0'), coalesce(Sum(Development), '0'), coalesce(Sum(Finishing), '0'), coalesce(Sum(Packing), '0') " +
                        "FROM v1 " +
                        "WHERE DATE BETWEEN @val0 AND @val1 " +
                        "AND Shift = \"Day Shift 1\" " +
                        "UNION " +
                        "SELECT * " +
                        "FROM v1 " +
                        "WHERE Date BETWEEN @val0 AND @val1 " +
                        "AND Shift = \"Day Shift 2\" " +
                        "UNION " +
                        "SELECT coalesce(NULL, ' '), coalesce(NULL, 'Total'), coalesce(SUM(Copy_room), '0'), coalesce(Sum(Development), '0'), coalesce(Sum(Finishing), '0'), coalesce(Sum(Packing), '0') " +
                        "FROM v1 " +
                        "WHERE DATE BETWEEN @val0 AND @val1 " +
                        "AND Shift = \"Day Shift 2\" " +
                        "UNION " +
                        "SELECT * " +
                        "FROM v1 " +
                        "WHERE Date BETWEEN @val0 AND @val1 " +
                        "AND Shift = \"Night Shift 1\" " +
                        "UNION " +
                        "SELECT coalesce(NULL, ' '), coalesce(NULL, 'Total'), coalesce(SUM(Copy_room), '0'), coalesce(Sum(Development), '0'), coalesce(Sum(Finishing), '0'), coalesce(Sum(Packing), '0') " +
                        "FROM v1 " +
                        "WHERE DATE BETWEEN @val0 AND @val1 " +
                        "AND Shift = \"Night Shift 1\" " +
                        "UNION " +
                        "SELECT coalesce(NULL, ' '), coalesce(NULL, ' '), coalesce(NULL, ' '), coalesce(NULL, ' '), coalesce(NULL, ' '), coalesce(NULL, ' ') " +
                        "UNION " +
                        "SELECT coalesce(NULL, ' '), Shift, coalesce(SUM(Copy_room), '0'), coalesce(Sum(Development), '0'), coalesce(Sum(Finishing), '0'), coalesce(Sum(Packing), '0') " +
                        "FROM v1 " +
                        "WHERE DATE BETWEEN @val0 AND @val1 " +
                        "GROUP BY Shift " +
                        "UNION " +
                        "SELECT coalesce(NULL, 'Total '), coalesce(NULL, ' '), coalesce(SUM(Copy_room), '0'), coalesce(Sum(Development), '0'), coalesce(Sum(Finishing), '0'), coalesce(Sum(Packing), '0') " +
                        "FROM v1 " +
                        "WHERE DATE BETWEEN @val0 AND @val1; "
                        , startDate, endDate);
                    break;
                case "Individual":
                    await this.Select(dg,
                        "SELECT DISTINCT(job_number), job_priority, job_status, special_instructions AS instructions, job_completed, discounted_total AS price " +
                        "FROM Job, Customer " +
                        "WHERE CustomerAccount_number = account_number " +
                        "AND Customerphone_number = @val0 " +
                        "AND deadline BETWEEN @val1 AND @val2 " +
                        "UNION " +
                        "SELECT coalesce(NULL, 'Total Jobs Booked: '), COUNT(job_number), coalesce(NULL, '---'), coalesce(NULL, '----'), coalesce(NULL, 'Total Paid: '), SUM(discounted_Total) " +
                        "FROM Job,customer " +
                        "WHERE CustomerAccount_number = account_number " +
                        "AND Customerphone_number = @val0 " +
                        "AND deadline BETWEEN @val1 AND @val2 " +
                        ";"
                        , int.Parse(custID), startDate, endDate) ;
                    break;
                default:
                    MessageBox.Show("There was an error");
                    break;
            }
            //printing the query into a pdf document
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Report";
            reportType += " Report:";
            
            // creates extra pages if query is too long
            PdfPage page = document.AddPage();
            page.Height = 842;//842
            page.Width = 590;

            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont font = new XFont("Verdana", 14, XFontStyle.Bold);
            XRect rect = new XRect(new XPoint(), gfx.PageSize);
            rect.Inflate(-10, -15);

            gfx.DrawString(reportType, font, XBrushes.MidnightBlue, rect, XStringFormats.TopCenter);

            XStringFormat format = new XStringFormat();
            format.LineAlignment = XLineAlignment.Far;
            format.Alignment = XStringAlignment.Center;
            font = new XFont("Verdana", 8);

            document.Outlines.Add(reportType, page, true);

            // Text format
            format.LineAlignment = XLineAlignment.Near;
            format.Alignment = XStringAlignment.Near;
            XFont fontParagraph = new XFont("Verdana", 8, XFontStyle.Regular);


            // page structure options
            double lineHeight = 20;
            int marginLeft = 20;
            int marginTop = 100;

            // Row elements
            int el_width = 40;
            int el_height = 15;

            DataTable dt = new DataTable();
            dt = ((DataView)dg.ItemsSource).ToTable();

            List<string> l = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

            CreateTable(page, dt, gfx, format, lineHeight, marginLeft, marginTop, el_height, el_width);

            document.Save(path);
            return path;
        }

        private void CreateTable(PdfPage page, DataTable dt, XGraphics gfx, XStringFormat format, double lineHeight, int marginLeft, int marginTop, int el_height, int el_width)
        {
            //how the printed output would looklike/ gui design for table
            XFont font = new XFont("Verdana", 10, XFontStyle.Bold);

            var tf = new XTextFormatter(gfx);

            int rect_height = 13;

            double gridwidth = page.Width - (marginLeft * 2);

            XSolidBrush rect_style1 = new XSolidBrush(XColors.MidnightBlue);
            XSolidBrush rect_style2 = new XSolidBrush(XColors.LightGray);
            XSolidBrush rect_style3 = new XSolidBrush(XColors.DarkGray);

            //creating the rows and columns
            for (int i = 0; i <= dt.Rows.Count; i++)
            {
                if (i == 0)
                {
                    gfx.DrawRectangle(rect_style1, marginLeft, marginTop, gridwidth, rect_height);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        tf.DrawString(dt.Columns[j].ColumnName, font, XBrushes.White, new XRect(((gridwidth / dt.Columns.Count) * (j + 1)) - el_width, marginTop, (gridwidth / dt.Columns.Count), el_height), format);
                    }
                }
                else
                {
                    var row = dt.Rows[i-1] as DataRow;

                    if ((i-1) % 2 == 1)
                        gfx.DrawRectangle(rect_style2, marginLeft, marginTop + (rect_height * i ) + (i - 1), gridwidth, rect_height);
                    else
                        gfx.DrawRectangle(rect_style3, marginLeft, marginTop + (rect_height * i) + (i - 1), gridwidth, rect_height);

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        tf.DrawString(row[dt.Columns[j].ColumnName].ToString(), font, XBrushes.White, new XRect(((gridwidth / dt.Columns.Count) * (j + 1)) - el_width, marginTop + (rect_height * i + 1) + i, (gridwidth / dt.Columns.Count), el_height), format);
                    }
                }

            }
        }




    }
}
