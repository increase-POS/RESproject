using netoaster;
using Restaurant.Classes;
using Restaurant.View.windows;
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

namespace Restaurant.View.catalog.foods
{
    /// <summary>
    /// Interaction logic for uc_foods.xaml
    /// </summary>
    public partial class uc_foods : UserControl
    {
        private static uc_foods _instance;
        public static uc_foods Instance
        {
            get
            {
                _instance = new uc_foods();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_foods()
        {
            try
            {
                InitializeComponent();
            }
            catch
            { }
        }
        string tagsPermission = "foods_tags";
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }

        private void Btn_itemsFoods_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;

                grid_main.Children.Clear();
                grid_main.Children.Add(uc_itemsFoods.Instance);
                uc_itemsFoods.categoryName = button.Tag.ToString();

                //Button button = sender as Button;
                MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_package_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_package.Instance);
                MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_tags_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                //Tags
                if (MainWindow.groupObject.HasPermissionAction(tagsPermission, MainWindow.groupObjects, "one"))
                {
                    Button button = sender as Button;
                    Window.GetWindow(this).Opacity = 0.2;
                    wd_tags w = new wd_tags();
                    w.categoryName = button.Tag.ToString();
                    w.ShowDialog();
                    Window.GetWindow(this).Opacity = 1;
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        
    }
}
