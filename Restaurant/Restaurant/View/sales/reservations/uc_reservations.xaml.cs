using netoaster;
using Restaurant.Classes;
using Restaurant.View.windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Restaurant.View.sales.reservations
{
    /// <summary>
    /// Interaction logic for uc_reservations.xaml
    /// </summary>
    public partial class uc_reservations : UserControl
    {
        private static uc_reservations _instance;
        public static uc_reservations Instance
        {
            get
            {
                _instance = new uc_reservations();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_reservations()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();

        }

        private void Btn_reservationTable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_reservationTable.Instance);

                Button button = sender as Button;
                MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Btn_reservationsUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_reservationsUpdate.Instance);

                Button button = sender as Button;
                MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
    }
}
