using Restaurant.Classes;
using Restaurant.View.storage.stocktakingOperations;
using Restaurant.View.storage.movementsOperations;
using Restaurant.View.storage.storageDivide;
using Restaurant.View.storage.storageOperations;
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

namespace Restaurant.View.storage
{
    /// <summary>
    /// Interaction logic for uc_storage.xaml
    /// </summary>
    public partial class uc_storage : UserControl
    {
        private static uc_storage _instance;
        public static uc_storage Instance
        {
            get
            {
                _instance = new uc_storage();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_storage()
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
        
        private void Btn_storageDivide_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_storageDivide.Instance);

                Button button = sender as Button;
                //initializationMainTrack(button.Tag.ToString(), 0);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
         

        private void Btn_storageOperations_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_storageOperations.Instance);

                Button button = sender as Button;
                //initializationMainTrack(button.Tag.ToString(), 0);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Btn_movementsOperations_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_movementsOperations.Instance);

                Button button = sender as Button;
                //initializationMainTrack(button.Tag.ToString(), 0);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

       

        private void Btn_stocktakingOperations_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_stocktakingOperations.Instance);

                Button button = sender as Button;
                //initializationMainTrack(button.Tag.ToString(), 0);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
    }
}
