using Bapers.GUI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Bapers
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //add all the possible windows here::
            //GUI.payment paymentwindow = new GUI.payment();
            //paymentwindow.Show();
            Login loginWindow = new Login();
            //loginWindow.Show();

            createAcc accCreation = new createAcc();
            accCreation.Show();

            DatabaseChecker dbWindow = new DatabaseChecker();
            //dbWindow.Show();

        }
    }
}
