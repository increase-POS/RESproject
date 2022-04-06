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

namespace Restaurant.View.delivery
{
    /// <summary>
    /// Interaction logic for uc_driversManagement.xaml
    /// </summary>
    public partial class uc_driversManagement : UserControl
    {
        IEnumerable<User> drivers;
        User userModel = new User();
        User driver = new User();
        string searchText = "";
        byte tgl_driverState;
        private static uc_driversManagement _instance;

        public static uc_driversManagement Instance
        {
            get
            {
                if(_instance is null)
                _instance = new uc_driversManagement();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_driversManagement()
        {
            try
            {
                InitializeComponent();
            }
            catch
            { }
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {//load
            try
            {
                HelpClass.StartAwait(grid_main);

                #region translate
                if (AppSettings.lang.Equals("en"))
                {
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                }
                translate();
                #endregion

                await Search();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        #region methods
        async Task Search()
        {
            try
            {
                if (drivers is null)
                    await RefreshDriversList();

                searchText = tb_search.Text.ToLower();
                drivers = drivers.Where(s => (
                   s.name.ToLower().Contains(searchText)
                || s.mobile.ToString().ToLower().Contains(searchText)
                )
                && s.isActive == tgl_driverState
                );

                RefreshDriverView();
            }
            catch { }
        }

        void RefreshDriverView()
        {
            dg_user.ItemsSource = drivers;
        }

        async Task<IEnumerable<User>> RefreshDriversList()
        {
            drivers = await userModel.GetUsersActive();
            drivers = drivers.Where(x => x.job == "deliveryEmployee");

            return drivers;
        }

        private void translate()
        {
        }
        #endregion
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }

        private async void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {//refresh
            try
            {
                HelpClass.StartAwait(grid_main);

                searchText = "";
                tb_search.Text = "";
                await Search();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }

        }

        private async void Tb_search_TextChanged(object sender, TextChangedEventArgs e)
        {//search
            try
            {
                HelpClass.StartAwait(grid_main);

                await Search();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Tgl_isActive_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                if (drivers is null)
                    await RefreshDriversList();
                tgl_driverState = 1;
                await Search();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Tgl_isActive_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                if (drivers is null)
                    await RefreshDriversList();
                tgl_driverState = 0;
                await Search();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Dg_user_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//selection
            try
            {
                HelpClass.StartAwait(grid_main);
                
                if (dg_user.SelectedIndex != -1)
                {
                    driver = dg_user.SelectedItem as User;
                    this.DataContext = driver;
                    if (driver != null)
                    {
                        //btn_stores.IsEnabled = true;
                        //grid_userNameLabel.Visibility = Visibility.Visible;
                        //grid_userNameInput.Visibility = Visibility.Collapsed;

                    }
                }
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
