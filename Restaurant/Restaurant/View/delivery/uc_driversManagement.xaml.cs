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

        async Task refreshDriverSectors()
        {
            driverSectors = await residentialSector.GetResSectorsByUserId(driver.userId);
            tb_sectorsCount.Text = driverSectors.Count.ToString();
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
            txt_title.Text = AppSettings.resourcemanager.GetString("trDriversManagement");
            txt_details.Text = AppSettings.resourcemanager.GetString("trDetails");
            txt_active.Text = AppSettings.resourcemanager.GetString("trActive");
            txt_userName.Text = AppSettings.resourcemanager.GetString("trUserName");
            txt_driverName.Text = AppSettings.resourcemanager.GetString("trDriver");
            txt_mobile.Text = AppSettings.resourcemanager.GetString("trMobile");
            txt_sectorsCount.Text = AppSettings.resourcemanager.GetString("trResidentialSectors");
            txt_ordersCount.Text = AppSettings.resourcemanager.GetString("trOrders");
            txt_status.Text = AppSettings.resourcemanager.GetString("trStatus");

            txt_preview.Text = AppSettings.resourcemanager.GetString("trPreview");
            txt_print.Text = AppSettings.resourcemanager.GetString("trPrint");
            txt_residentialSectors.Text = AppSettings.resourcemanager.GetString("trResidentialSectors");
            txt_activeInactive.Text = AppSettings.resourcemanager.GetString("activate");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_search, AppSettings.resourcemanager.GetString("trSearchHint"));

            dg_user.Columns[0].Header = AppSettings.resourcemanager.GetString("trUserName");
            dg_user.Columns[1].Header = AppSettings.resourcemanager.GetString("trName");
            dg_user.Columns[2].Header = AppSettings.resourcemanager.GetString("trMobile");
            dg_user.Columns[3].Header = AppSettings.resourcemanager.GetString("trStatus");

            tt_refresh.Content = AppSettings.resourcemanager.GetString("trRefresh");

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
        int selectedDriver = 0;
        List<ResidentialSectors> driverSectors = new List<ResidentialSectors>();
        ResidentialSectors residentialSector = new ResidentialSectors();

        
        private async void Dg_user_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//selection
            try
            {
                HelpClass.StartAwait(grid_main);
                
                if (dg_user.SelectedIndex != -1)
                {
                    driver = dg_user.SelectedItem as User;
                    selectedDriver = dg_user.SelectedIndex;
                    this.DataContext = driver;
                    if (driver != null)
                    {
                        if (driver.driverIsAvailable == 0)
                            txt_activeInactive.Text = AppSettings.resourcemanager.GetString("activate");
                        else
                            txt_activeInactive.Text = AppSettings.resourcemanager.GetString("deActivate");

                        await refreshDriverSectors();

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

        private async void Btn_residentialSectors_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                //if (FillCombo.groupObject.HasPermissionAction(storesPermission, FillCombo.groupObjects, "one"))
                {
                    Window.GetWindow(this).Opacity = 0.2;

                    wd_residentialSectorsList w = new wd_residentialSectorsList();
                    w.driverId = driver.userId;
                    w.ShowDialog();

                    Window.GetWindow(this).Opacity = 1;

                    await refreshDriverSectors();
                }
                //else
                //    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Btn_activeInactive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                //if (FillCombo.groupObject.HasPermissionAction(storesPermission, FillCombo.groupObjects, "one"))
                {
                    string resultStr = "";
                    if (driver.driverIsAvailable == 0)
                    {
                        driver.driverIsAvailable = 1;
                        resultStr = "popActivation";
                    }
                    else
                    {
                        driver.driverIsAvailable = 0;
                        resultStr = "popDeActivation";
                    }

                    int s = await driver.save(driver);
                    if (s <= 0)
                        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                    else
                    {
                        Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString(resultStr), animation: ToasterAnimation.FadeIn);
                        await RefreshDriversList();
                        await Search();
                        dg_user.SelectedIndex = selectedDriver;
                    }
                }
                //else
                //    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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
