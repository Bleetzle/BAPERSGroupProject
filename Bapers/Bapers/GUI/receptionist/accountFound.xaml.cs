﻿using System;
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

namespace Bapers.GUI
{
    /// <summary>
    /// Interaction logic for accountFound.xaml
    /// </summary>
    public partial class accountFound : Window
    {
        //this pop up is used as an intermediary when account is found and prompts the user if they would like to pay or add jobs to that account after a search is complete
        public accountFound()
        {
            InitializeComponent();
        }

        private void no_Click(object sender, RoutedEventArgs e)
        {
            searchAcc searchaccWindow = new searchAcc();
            searchaccWindow.Show();
            this.Close();
        }

        private void yes_Click(object sender, RoutedEventArgs e)
        {
            addJobs addjobswindow = new addJobs();
            addjobswindow.Show();
            this.Close();
        }

        private void payment_Click(object sender, RoutedEventArgs e)
        {
            payment paymentWindow = new payment();
            paymentWindow.Show();
            this.Close();
        }
    }
}
