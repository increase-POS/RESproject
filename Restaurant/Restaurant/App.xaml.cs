﻿using Restaurant.View.windows;
using Restaurant.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Restaurant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                //if (config.AppSettings.Settings["posId"] == null)
                //{
                //    wd_setupFirstPos logIn = new wd_setupFirstPos();
                //    logIn.Show();
                //}
                //else
                {

                    wd_logIn logIn = new wd_logIn();
                    //MainWindow logIn = new MainWindow();
                    logIn.Show();
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
    }
}
