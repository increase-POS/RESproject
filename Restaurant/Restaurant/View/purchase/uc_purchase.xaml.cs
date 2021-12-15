﻿using Restaurant.Classes;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Restaurant.View.purchase
{
    /// <summary>
    /// Interaction logic for uc_purchase.xaml
    /// </summary>
    public partial class uc_purchase : UserControl
    {
        private static uc_purchase _instance;
        public static uc_purchase Instance
        {
            get
            {
                _instance = new uc_purchase();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_purchase()
        {
            try
            {
                InitializeComponent();
            }
            catch
            { }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }
      
    }
}