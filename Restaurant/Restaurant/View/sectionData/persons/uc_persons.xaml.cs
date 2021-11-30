using Restaurant.Classes;
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

namespace Restaurant.View.sectionData.persons
{
    /// <summary>
    /// Interaction logic for uc_persons.xaml
    /// </summary>
    public partial class uc_persons : UserControl
    {
        private static uc_persons _instance;
        public static uc_persons Instance
        {
            get
            {
                _instance = new uc_persons();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_persons()
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

        private void Btn_vendors_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_customers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_customers.Instance);

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
