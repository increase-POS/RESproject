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

namespace Restaurant.View.catalog.rawMaterials
{
    /// <summary>
    /// Interaction logic for uc_rawMaterials.xaml
    /// </summary>
    public partial class uc_rawMaterials : UserControl
    {
        private static uc_rawMaterials _instance;
        public static uc_rawMaterials Instance
        {
            get
            {
                _instance = new uc_rawMaterials();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_rawMaterials()
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

        private void Btn_itemsRawMaterials_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_units_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_units.Instance);

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
