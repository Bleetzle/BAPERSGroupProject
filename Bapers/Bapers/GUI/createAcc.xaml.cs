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
    /// Interaction logic for createAcc.xaml
    /// </summary>
    public partial class createAcc : Window
    {
        public createAcc()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            receptionist receptionistwindow = new receptionist();
            receptionistwindow.Show();
            this.Close();
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            //code for creating a new account goes here


            //after account created..
            accountCreated_popup accountCreated_Popup_window = new accountCreated_popup();
            accountCreated_Popup_window.Show();
            this.Close();
        }
    }
}
