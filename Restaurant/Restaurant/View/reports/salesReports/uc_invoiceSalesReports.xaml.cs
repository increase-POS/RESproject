using LiveCharts;
using LiveCharts.Wpf;
using Restaurant.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using LiveCharts.Helpers;
using Restaurant.View.windows;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using System.Threading;
using System.Resources;
using System.Reflection;
using Restaurant.View.sales;

namespace Restaurant.View.reports.salesReports
{
    /// <summary>
    /// Interaction logic for uc_invoiceSalesReports.xaml
    /// </summary>
    public partial class uc_invoiceSalesReports : UserControl
    {
        private static uc_invoiceSalesReports _instance;
        public static uc_invoiceSalesReports Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new uc_invoiceSalesReports();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_invoiceSalesReports()
        {
            try
            {
                InitializeComponent();
            }
            catch
            { }
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }

        //prin & pdf
        ReportCls reportclass = new ReportCls();
        LocalReport rep = new LocalReport();
        SaveFileDialog saveFileDialog = new SaveFileDialog();

        private int selectedTab = 0;

        Statistics statisticModel = new Statistics();

        List<ItemTransferInvoice> Invoices;
        List<ItemTransferInvoice> InvoicesQuery;

        string searchText = "";

        //for combo boxes
        /*************************/
        Branch selectedBranch;
        Pos selectedPos;
        Agent selectedVendor;
        User selectedUser;

        //List<Branch> comboBranches;
        //List<Pos> comboPoss;
        //List<Agent> comboVendors;
        //List<User> comboUsers;

        ObservableCollection<Branch> comboBrachTemp = new ObservableCollection<Branch>();
        ObservableCollection<Pos> comboPosTemp = new ObservableCollection<Pos>();
        ObservableCollection<Agent> comboVendorTemp = new ObservableCollection<Agent>();
        ObservableCollection<User> comboUserTemp = new ObservableCollection<User>();

        ObservableCollection<Branch> dynamicComboBranches;
        //List<Branch> dynamicComboBranches;
        ObservableCollection<Pos> dynamicComboPoss;
        ObservableCollection<Agent> dynamicComboVendors;
        ObservableCollection<User> dynamicComboUsers;

        Branch branchModel = new Branch();
        Pos posModel = new Pos();
        Agent agentModel = new Agent();
        User userModel = new User();

        ObservableCollection<int> selectedBranchId = new ObservableCollection<int>();
        ObservableCollection<int> selectedPosId = new ObservableCollection<int>();
        ObservableCollection<int> selectedVendorsId = new ObservableCollection<int>();
        ObservableCollection<int> selectedUserId = new ObservableCollection<int>();

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {//load
            try
            {
                HelpClass.StartAwait(grid_main);

                #region translate
                if (AppSettings.lang.Equals("en"))
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                else
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                translate();
                #endregion

                fillServices();

                chk_allBranches.IsChecked = true;
                chk_allServices.IsChecked = true;

                btn_branch_Click(btn_branch , null);
               
                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), btn_pos.Tag.ToString());
               
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        #region methods
        private async void callSearch(object sender)
        {
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
        private void translate()
        {
            tt_branch.Content = AppSettings.resourcemanager.GetString("trBranches");
            tt_pos.Content = AppSettings.resourcemanager.GetString("trPOSs");
            tt_vendors.Content = AppSettings.resourcemanager.GetString("trVendors");
            tt_users.Content = AppSettings.resourcemanager.GetString("trUsers");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_branches, AppSettings.resourcemanager.GetString("trBranchHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_sevices, AppSettings.resourcemanager.GetString("trServices")+"...");

            chk_allBranches.Content = AppSettings.resourcemanager.GetString("trAll");
            chk_allServices.Content = AppSettings.resourcemanager.GetString("trAll");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_endDate, AppSettings.resourcemanager.GetString("trEndDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_startDate, AppSettings.resourcemanager.GetString("trStartDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dt_endTime, AppSettings.resourcemanager.GetString("trEndTime") + "...");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dt_startTime, AppSettings.resourcemanager.GetString("trStartTime") + "...");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(txt_search, AppSettings.resourcemanager.GetString("trSearchHint"));
            tt_refresh.Content = AppSettings.resourcemanager.GetString("trRefresh");

            col_No.Header = AppSettings.resourcemanager.GetString("trNo.");
            col_type.Header = AppSettings.resourcemanager.GetString("trType");
            col_date.Header = AppSettings.resourcemanager.GetString("trDate");
            col_branch.Header = AppSettings.resourcemanager.GetString("trBranch");
            col_pos.Header = AppSettings.resourcemanager.GetString("trPOS");
            col_vendor.Header = AppSettings.resourcemanager.GetString("trVendor");
            col_agentCompany.Header = AppSettings.resourcemanager.GetString("trCompany");
            col_user.Header = AppSettings.resourcemanager.GetString("trUser");
            col_count.Header = AppSettings.resourcemanager.GetString("trQTR");
            col_discount.Header = AppSettings.resourcemanager.GetString("trDiscount");
            col_tax.Header = AppSettings.resourcemanager.GetString("trTax");
            col_totalNet.Header = AppSettings.resourcemanager.GetString("trTotal");

            tt_report.Content = AppSettings.resourcemanager.GetString("trPdf");
            tt_print.Content = AppSettings.resourcemanager.GetString("trPrint");
            tt_excel.Content = AppSettings.resourcemanager.GetString("trExcel");
            tt_count.Content = AppSettings.resourcemanager.GetString("trCount");

        }

        private void fillServices()
        {
            //FillCombo.FillInvoiceType(cb_sevices);
            //cb_sevices.SelectedIndex = -1;
            var typelist = new[] {
                new { Text = AppSettings.resourcemanager.GetString("trDiningHallType")       , Value = "s" },
                new { Text = AppSettings.resourcemanager.GetString("trTakeAway")             , Value = "ts" },
                new { Text = AppSettings.resourcemanager.GetString("trSelfService")          , Value = "ss" },
                 };
            cb_sevices.SelectedValuePath = "Value";
            cb_sevices.DisplayMemberPath = "Text";
            cb_sevices.ItemsSource = typelist;
            cb_sevices.SelectedIndex = -1;
        }
        private void fillComboBranches()
        {
            cb_branches.SelectedValuePath = "branchCreatorId";
            cb_branches.DisplayMemberPath = "branchCreatorName";
            var lst = Invoices.GroupBy(i => i.branchCreatorId).Select(i => new { i.FirstOrDefault().branchCreatorName, i.FirstOrDefault().branchCreatorId });
            cb_branches.ItemsSource = lst;
        }

        private void fillComboPos()
        {
            //cb_branches.SelectedValuePath = "posId";
            //cb_branches.DisplayMemberPath = "name";
            //cb_branches.ItemsSource = dynamicComboPoss;
        }

        private void fillComboVendors()
        {
            //cb_branches.SelectedValuePath = "agentId";
            //cb_branches.DisplayMemberPath = "name";
            //cb_branches.ItemsSource = dynamicComboVendors;
        }

        private void fillComboUsers()
        {
            //cb_branches.SelectedValuePath = "userId";
            //cb_branches.DisplayMemberPath = "username";
            //cb_branches.ItemsSource = dynamicComboUsers;
        }

        private static void fillDates(DatePicker startDate, DatePicker endDate, TimePicker startTime, TimePicker endTime)
        {
            try
            {
                if (startDate.SelectedDate != null && startTime.SelectedTime != null)
                {
                    string x = startDate.SelectedDate.Value.Date.ToShortDateString();
                    string y = startTime.SelectedTime.Value.ToShortTimeString();
                    string resultStartTime = x + " " + y;
                    startTime.SelectedTime = DateTime.Parse(resultStartTime);
                    startDate.SelectedDate = DateTime.Parse(resultStartTime);
                }
                if (endDate.SelectedDate != null && endTime.SelectedTime != null)
                {
                    string x = endDate.SelectedDate.Value.Date.ToShortDateString();
                    string y = endTime.SelectedTime.Value.ToShortTimeString();
                    string resultEndTime = x + " " + y;
                    endTime.SelectedTime = DateTime.Parse(resultEndTime);
                    endDate.SelectedDate = DateTime.Parse(resultEndTime);
                }
            }
            catch (Exception ex)
            {
                //HelpClass.ExceptionMessage(ex, this);
            }
        }

        //IEnumerable<ItemTransferInvoice> invLst = null;
        //private IEnumerable<ItemTransferInvoice> fillList(IEnumerable<ItemTransferInvoice> Invoices, CheckBox chkInvoice, CheckBox chkReturn, CheckBox chkDraft
        //   , DatePicker startDate, DatePicker endDate, TimePicker startTime, TimePicker endTime)
        //{
        //    fillDates(startDate, endDate, startTime, endTime);
        //    var result = Invoices.Where(x => (

        //       ((chkDraft.IsChecked == true ? (x.invType == "sd" || x.invType == "sbd") : false)
        //                  || (chkReturn.IsChecked == true ? (x.invType == "sb") : false)
        //                  || (chkInvoice.IsChecked == true ? (x.invType == "s") : false))
        //              && (startDate.SelectedDate != null ? x.invDate >= startDate.SelectedDate : true)
        //              && (endDate.SelectedDate != null ? x.invDate <= endDate.SelectedDate : true)
        //              && (startTime.SelectedTime != null ? x.invDate >= startTime.SelectedTime : true)
        //              && (endTime.SelectedTime != null ? x.invDate <= endTime.SelectedTime : true)));
        //    invLst = result;
        //    return result;
        //}
        IEnumerable<ItemTransferInvoice> invLstRowChart = null;
        private IEnumerable<ItemTransferInvoice> fillRowChartList(IEnumerable<ItemTransferInvoice> Invoices, CheckBox chkInvoice, CheckBox chkReturn, CheckBox chkDraft
          , DatePicker startDate, DatePicker endDate, TimePicker startTime, TimePicker endTime)
        {
            fillDates(startDate, endDate, startTime, endTime);
            var result = Invoices.Where(x => ((txt_search.Text != null ? x.invNumber.Contains(txt_search.Text)
              || x.invType.Contains(txt_search.Text)
              || x.discountType.Contains(txt_search.Text) : true)
              &&
                         (startDate.SelectedDate != null ? x.invDate >= startDate.SelectedDate : true)
                        && (endDate.SelectedDate != null ? x.invDate <= endDate.SelectedDate : true)
                        && (startTime.SelectedTime != null ? x.invDate >= startTime.SelectedTime : true)
                        && (endTime.SelectedTime != null ? x.invDate <= endTime.SelectedTime : true)));
            return result;
        }

        private List<ItemTransferInvoice> converter(List<ItemTransferInvoice> query)
        {
            foreach (var item in query)
            {
                if (item.invType == "p")
                {
                    item.invType = AppSettings.resourcemanager.GetString("trPurchaseInvoice");
                }
                else if (item.invType == "pw")
                {
                    item.invType = AppSettings.resourcemanager.GetString("trPurchaseInvoice");
                }
                else if (item.invType == "pb")
                {
                    item.invType = AppSettings.resourcemanager.GetString("trPurchaseReturnInvoice");
                }
                else if (item.invType == "pd")
                {
                    item.invType = AppSettings.resourcemanager.GetString("trDraftPurchaseBill");
                }
                else if (item.invType == "pbd")
                {
                    item.invType = AppSettings.resourcemanager.GetString("trPurchaseReturnDraft");
                }
            }
            return query;

        }
        private void hideSatacks()
        {
            stk_tagsBranches.Visibility = Visibility.Collapsed;
            stk_tagsItems.Visibility = Visibility.Collapsed;
            stk_tagsPos.Visibility = Visibility.Collapsed;
            stk_tagsUsers.Visibility = Visibility.Collapsed;
            stk_tagsVendors.Visibility = Visibility.Collapsed;
            stk_tagsCoupons.Visibility = Visibility.Collapsed;
            stk_tagsOffers.Visibility = Visibility.Collapsed;
        }

        public void paint()
        {
            //bdrMain.RenderTransform = Animations.borderAnimation(50, bdrMain, true);

            bdr_branch.Background = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            bdr_pos.Background = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            bdr_vendors.Background = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            bdr_users.Background = Application.Current.Resources["SecondColor"] as SolidColorBrush;

            path_branch.Fill = Brushes.White;
            path_pos.Fill = Brushes.White;
            path_vendors.Fill = Brushes.White;
            path_users.Fill = Brushes.White;
        }

        async Task Search()
        {

            if (Invoices is null)
                await RefreshInvoicesList();

            searchText = txt_search.Text.ToLower();
            InvoicesQuery = Invoices
               .Where(s =>
            (
            s.invNumber.ToLower().Contains(searchText)
            ||
            (s.branchCreatorName != null ? s.branchCreatorName.ToLower().Contains(searchText) : false)
            ||
            (s.posName      != null ? s.posName.ToLower().Contains(searchText)      : false)//////?????
            ||
            (s.agentName    != null ? s.agentName.ToLower().Contains(searchText)    : false)//////?????
            ||
            (s.agentCompany != null ? s.agentCompany.ToLower().Contains(searchText) : false)//////?????
            ||
            (s.uUserAccName != null ? s.uUserAccName.ToLower().Contains(searchText) : false)//////?????
            )
            &&
            //branch
            (cb_branches.SelectedIndex != -1    ? s.branchCreatorId == Convert.ToInt32(cb_branches.SelectedValue) : true)
            &&
            //service
            (cb_sevices.SelectedIndex != -1     ? s.invType == cb_sevices.SelectedValue.ToString() : true)
            &&
            //start date
            (dp_startDate.SelectedDate != null  ? s.updateDate >= dp_startDate.SelectedDate : true)
            &&
            //end date
            (dp_endDate.SelectedDate != null    ? s.updateDate <= dp_endDate.SelectedDate : true)
            && 
            //start time
            (dt_startTime.SelectedTime != null  ? s.updateDate >= dt_startTime.SelectedTime : true)///???
            && 
            //end time
            (dt_endTime.SelectedTime != null    ? s.updateDate <= dt_endTime.SelectedTime : true)///???
            ).ToList();

            RefreshInvoicesView();
        }

        void RefreshInvoicesView()
        {
            dgInvoice.ItemsSource = InvoicesQuery;
            txt_count.Text = InvoicesQuery.Count().ToString();

            //hide tax column if region tax equals to 0
            if (!AppSettings.invoiceTax_bool.Value)
                col_tax.Visibility = Visibility.Hidden;
            else
                col_tax.Visibility = Visibility.Visible;

            ObservableCollection<int> selected = new ObservableCollection<int>();
            if (selectedTab == 0)
                selected = selectedBranchId;
            if (selectedTab == 1)
                selected = selectedPosId;
            if (selectedTab == 2)
                selected = selectedVendorsId;
            if (selectedTab == 3)
                selected = selectedUserId;

            fillColumnChart(selected);
            fillPieChart(selected);
            fillRowChart(selected);
        }

        async Task<IEnumerable<ItemTransferInvoice>> RefreshInvoicesList()
        {
            Invoices = await statisticModel.GetSaleitemcount((int)MainWindow.branchLogin.branchId, (int)MainWindow.userLogin.userId);
            return Invoices;
        }
       
        #endregion
      
        #region Events

        #region tabControl
        private void hidAllColumns()
        {
            col_type.Visibility = Visibility.Hidden;
            col_branch.Visibility = Visibility.Hidden;
            col_pos.Visibility = Visibility.Hidden;
            col_vendor.Visibility = Visibility.Hidden;
            col_agentCompany.Visibility = Visibility.Hidden;
            col_user.Visibility = Visibility.Hidden;
            col_discount.Visibility = Visibility.Hidden;
            col_count.Visibility = Visibility.Hidden;
            col_totalNet.Visibility = Visibility.Hidden;
            col_tax.Visibility = Visibility.Hidden;
        }
        private async void btn_branch_Click(object sender, RoutedEventArgs e)
        {//branches
            try
            {
                HelpClass.StartAwait(grid_main);

                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), (sender as Button).Tag.ToString());
                MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_branches, AppSettings.resourcemanager.GetString("trBranchHint"));

                txt_search.Text = "";
                hideSatacks();
                stk_tagsBranches.Visibility = Visibility.Visible;
                selectedTab = 0;

                paint();
                ReportsHelp.paintTabControlBorder(grid_tabControl, bdr_branch);
                path_branch.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;

                hidAllColumns();
                //show columns
                col_branch.Visibility = Visibility.Visible;
                col_count.Visibility = Visibility.Visible;
                col_totalNet.Visibility = Visibility.Visible;
                col_discount.Visibility = Visibility.Visible;
                col_tax.Visibility = Visibility.Visible;
                col_type.Visibility = Visibility.Visible;

                await Search();
                fillComboBranches();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void btn_pos_Click(object sender, RoutedEventArgs e)
        {//pos
            try
            {
                HelpClass.StartAwait(grid_main);

                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), (sender as Button).Tag.ToString());
                MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_branches, AppSettings.resourcemanager.GetString("trPosHint"));

                txt_search.Text = "";
                hideSatacks();
                stk_tagsPos.Visibility = Visibility.Visible;
                selectedTab = 1;

                paint();
                ReportsHelp.paintTabControlBorder(grid_tabControl, bdr_pos);
                path_pos.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;

                hidAllColumns();
                //show columns
                col_branch.Visibility = Visibility.Visible;
                col_count.Visibility = Visibility.Visible;
                col_pos.Visibility = Visibility.Visible;
                col_totalNet.Visibility = Visibility.Visible;
                col_discount.Visibility = Visibility.Visible;
                col_tax.Visibility = Visibility.Visible;
                col_type.Visibility = Visibility.Visible;

                await Search();
                fillComboPos();
               
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void btn_vendors_Click(object sender, RoutedEventArgs e)
        {//vendor
            try
            {
                HelpClass.StartAwait(grid_main);

                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), (sender as Button).Tag.ToString());
                MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_branches, AppSettings.resourcemanager.GetString("trVendorHint"));

                txt_search.Text = "";
                hideSatacks();
                stk_tagsVendors.Visibility = Visibility.Visible;
                selectedTab = 2;

                paint();
                ReportsHelp.paintTabControlBorder(grid_tabControl, bdr_vendors);
                path_vendors.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;

                hidAllColumns();
                //show columns
                col_branch.Visibility = Visibility.Visible;
                col_vendor.Visibility = Visibility.Visible;
                col_agentCompany.Visibility = Visibility.Visible;
                col_discount.Visibility = Visibility.Visible;
                col_count.Visibility = Visibility.Visible;
                col_totalNet.Visibility = Visibility.Visible;
                col_tax.Visibility = Visibility.Visible;
                col_type.Visibility = Visibility.Visible;

                await Search();
                fillComboVendors();

               HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void btn_users_Click(object sender, RoutedEventArgs e)
        {//users
            try
            {
                HelpClass.StartAwait(grid_main);

                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), (sender as Button).Tag.ToString());
                MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_branches, AppSettings.resourcemanager.GetString("trUserHint"));

                txt_search.Text = "";
                hideSatacks();
                stk_tagsUsers.Visibility = Visibility.Visible;
                selectedTab = 3;

                paint();
                ReportsHelp.paintTabControlBorder(grid_tabControl, bdr_users);
                path_users.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;

                hidAllColumns();
                //show columns
                col_branch.Visibility = Visibility.Visible;
                col_discount.Visibility = Visibility.Visible;
                col_pos.Visibility = Visibility.Visible;
                col_user.Visibility = Visibility.Visible;
                col_totalNet.Visibility = Visibility.Visible;
                col_tax.Visibility = Visibility.Visible;
                col_type.Visibility = Visibility.Visible;

                await Search();
                fillComboUsers();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        #endregion

        private void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                cb_branches.SelectedItem = null;
                chk_allBranches.IsChecked = false;
                cb_sevices.SelectedItem = null;
                chk_allServices.IsChecked = false;
                dp_endDate.SelectedDate = null;
                dp_startDate.SelectedDate = null;
                dt_startTime.SelectedTime = null;
                dt_endTime.SelectedTime = null;

                txt_search.Text = "";
               
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void dp_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            callSearch(sender);
        }

        private void dt_SelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            callSearch(sender);
        }

        private async void Chk_allBranches_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                cb_branches.SelectedIndex = -1;
                cb_branches.IsEnabled = false;

                await Search();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Chk_allBranches_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                cb_branches.IsEnabled = true;

                await Search();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void cb_branches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);

                if (cb_branches.SelectedItem != null)
                {
                    if (selectedTab == 0)
                    {
                        if (stk_tagsBranches.Children.Count < 5)
                        {
                            int bId = (int)cb_branches.SelectedValue;

                            selectedBranch = await branchModel.getBranchById(bId);
                            var b = new MaterialDesignThemes.Wpf.Chip()
                            {
                                Content = selectedBranch.name,
                                Name = "btn" + selectedBranch.branchId.ToString(),
                                IsDeletable = true,
                                Margin = new Thickness(5, 0, 5, 0)
                            };
                            b.DeleteClick += Chip_OnDeleteClick;
                            stk_tagsBranches.Children.Add(b);
                            comboBrachTemp.Add(selectedBranch);
                            selectedBranchId.Add(selectedBranch.branchId);
                            dynamicComboBranches.Remove(selectedBranch);
                        }
                    }
                    if (selectedTab == 1)
                    {
                        if (stk_tagsPos.Children.Count < 5)
                        {
                            int pId = (int)cb_branches.SelectedValue;

                            selectedPos = await posModel.getById(pId);
                            var p = new MaterialDesignThemes.Wpf.Chip()
                            {
                                Content = selectedPos.name,
                                Name = "btn" + selectedPos.posId.ToString(),
                                IsDeletable = true,
                                Margin = new Thickness(5, 0, 5, 0)
                            };
                            p.DeleteClick += Chip_OnDeleteClick;
                            stk_tagsPos.Children.Add(p);
                            comboPosTemp.Add(selectedPos);
                            selectedPosId.Add(selectedPos.posId);
                            dynamicComboPoss.Remove(selectedPos);
                        }
                    }
                    if (selectedTab == 2)
                    {
                        if (stk_tagsVendors.Children.Count < 5)
                        {
                            int aId = (int)cb_branches.SelectedValue;

                            selectedVendor = await agentModel.getAgentById(aId);
                            var a = new MaterialDesignThemes.Wpf.Chip()
                            {
                                Content = selectedVendor.name,
                                Name = "btn" + selectedVendor.agentId.ToString(),
                                IsDeletable = true,
                                Margin = new Thickness(5, 0, 5, 0)
                            };
                            a.DeleteClick += Chip_OnDeleteClick;
                            stk_tagsVendors.Children.Add(a);
                            comboVendorTemp.Add(selectedVendor);
                            selectedVendorsId.Add(selectedVendor.agentId);
                            dynamicComboVendors.Remove(selectedVendor);
                        }
                    }
                    if (selectedTab == 3)
                    {
                        if (stk_tagsUsers.Children.Count < 5)
                        {
                            int uId = (int)cb_branches.SelectedValue;

                            selectedUser = await userModel.getUserById(uId);
                            var u = new MaterialDesignThemes.Wpf.Chip()
                            {
                                Content = selectedUser.name,
                                Name = "btn" + selectedUser.userId.ToString(),
                                IsDeletable = true,
                                Margin = new Thickness(5, 0, 5, 0)
                            };
                            u.DeleteClick += Chip_OnDeleteClick;
                            stk_tagsUsers.Children.Add(u);
                            comboUserTemp.Add(selectedUser);
                            selectedUserId.Add(selectedUser.userId);
                            dynamicComboUsers.Remove(selectedUser);
                        }
                    }

                    await Search();

                }

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Chk_allServices_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                cb_sevices.SelectedIndex = -1;
                cb_sevices.IsEnabled = false;

                await Search();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Chk_allServices_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                cb_sevices.IsEnabled = true;

                await Search();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Cb_sevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            callSearch(sender);
        }

        IEnumerable<ItemTransferInvoice> itemTransfers = null;

        private void Txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {//search
            callSearch(sender);
        }
        Invoice invoice;
        private async void DgInvoice_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {//double click
            try
            {
                HelpClass.StartAwait(grid_main);
                invoice = new Invoice();
                if (dgInvoice.SelectedIndex != -1)
                {
                    ItemTransferInvoice item = dgInvoice.SelectedItem as ItemTransferInvoice;
                    if (item.invoiceId > 0)
                    {
                        Invoice invoice = new Invoice();
                        invoice = await invoice.GetByInvoiceId(item.invoiceId);

                        MainWindow.mainWindow.Btn_sales_Click(MainWindow.mainWindow.btn_sales, null);
                        MainWindow.mainWindow.initializationMainTrack("invoiceSales");/////???????????
                        MainWindow.mainWindow.grid_main.Children.Clear();
                        MainWindow.mainWindow.grid_main.Children.Add(uc_diningHall.Instance);
                        uc_diningHall.Instance.invoice = invoice;
                        uc_diningHall.Instance._InvoiceType = invoice.invType;
                        uc_diningHall.Instance.changeInvType();
                        uc_diningHall.isFromReport = true;
                        await uc_diningHall.Instance.fillInvoiceInputs(invoice);
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

        private async void Chip_OnDeleteClick(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                var currentChip = (Chip)sender;
                if (selectedTab == 0)
                {
                    stk_tagsBranches.Children.Remove(currentChip);
                    var m = comboBrachTemp.Where(j => j.branchId == (Convert.ToInt32(currentChip.Name.Remove(0, 3))));
                    dynamicComboBranches.Add(m.FirstOrDefault());
                    selectedBranchId.Remove(Convert.ToInt32(currentChip.Name.Remove(0, 3)));
                    if (selectedBranchId.Count == 0)
                    {
                        cb_branches.SelectedItem = null;
                    }
                }
                else if (selectedTab == 1)
                {
                    stk_tagsPos.Children.Remove(currentChip);
                    var m = comboPosTemp.Where(j => j.posId == (Convert.ToInt32(currentChip.Name.Remove(0, 3))));
                    dynamicComboPoss.Add(m.FirstOrDefault());
                    selectedPosId.Remove(Convert.ToInt32(currentChip.Name.Remove(0, 3)));
                    if (selectedPosId.Count == 0)
                    {
                        cb_branches.SelectedItem = null;
                    }
                }
                else if (selectedTab == 2)
                {
                    stk_tagsVendors.Children.Remove(currentChip);
                    var m = comboVendorTemp.Where(j => j.agentId == (Convert.ToInt32(currentChip.Name.Remove(0, 3))));
                    dynamicComboVendors.Add(m.FirstOrDefault());
                    selectedVendorsId.Remove(Convert.ToInt32(currentChip.Name.Remove(0, 3)));
                    if (selectedVendorsId.Count == 0)
                    {
                        cb_branches.SelectedItem = null;
                    }
                }
                else if (selectedTab == 3)
                {
                    stk_tagsUsers.Children.Remove(currentChip);
                    var m = comboUserTemp.Where(j => j.userId == (Convert.ToInt32(currentChip.Name.Remove(0, 3))));
                    dynamicComboUsers.Add(m.FirstOrDefault());
                    selectedUserId.Remove(Convert.ToInt32(currentChip.Name.Remove(0, 3)));
                    if (selectedUserId.Count == 0)
                    {
                        cb_branches.SelectedItem = null;
                    }
                }
                await Search();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        
        #endregion

        #region charts
        private void fillPieChart(ObservableCollection<int> stackedButton)
        {
            List<string> titles = new List<string>();
            IEnumerable<int> x = null;

            var temp = InvoicesQuery;

            if (selectedTab == 0)
            {
                temp = temp.Where(j => (selectedBranchId.Count != 0 ? stackedButton.Contains((int)j.branchCreatorId) : true)).ToList();
                var titleTemp = temp.GroupBy(m => m.branchCreatorName);
                titles.AddRange(titleTemp.Select(jj => jj.Key));
                var result = temp.GroupBy(s => s.branchCreatorId).Select(s => new { branchCreatorId = s.Key, count = s.Count() });
                x = result.Select(m => m.count);
            }
            else if (selectedTab == 1)
            {
                temp = temp.Where(j => (selectedPosId.Count != 0 ? stackedButton.Contains((int)j.posId) : true)).ToList();
                var titleTemp = temp.GroupBy(m => new { m.posName, m.posId });
                titles.AddRange(titleTemp.Select(jj => jj.Key.posName));
                var result = temp.GroupBy(s => s.posId).Select(s => new { posId = s.Key, count = s.Count() });
                x = result.Select(m => m.count);
            }
            else if (selectedTab == 2)
            {
                temp = temp.Where(j => (selectedVendorsId.Count != 0 ? stackedButton.Contains((int)j.agentId) : true)).ToList();
                var titleTemp = temp.GroupBy(m => m.agentName);
                titles.AddRange(titleTemp.Select(jj => jj.Key));
                var result = temp.GroupBy(s => s.agentId).Select(s => new { agentId = s.Key, count = s.Count() });
                x = result.Select(m => m.count);
            }
            else if (selectedTab == 3)
            {
                temp = temp.Where(j => (selectedUserId.Count != 0 ? stackedButton.Contains((int)j.updateUserId) : true)).ToList();
                var titleTemp = temp.GroupBy(m => m.cUserAccName);
                titles.AddRange(titleTemp.Select(jj => jj.Key));
                var result = temp.GroupBy(s => s.updateUserId).Select(s => new { updateUserId = s.Key, count = s.Count() });
                x = result.Select(m => m.count);
            }

            SeriesCollection piechartData = new SeriesCollection();
            int xCount = 6;
            if (x.Count() <= 6) xCount = x.Count();
            for (int i = 0; i < xCount; i++)
            {
                List<int> final = new List<int>();

                final.Add(x.ToList().Skip(i).FirstOrDefault());
                piechartData.Add(
                  new PieSeries
                  {
                      Values = final.AsChartValues(),
                      Title = titles.Skip(i).FirstOrDefault(),
                      DataLabels = true,
                  }
              );
            }
            if (x.Count() > 6)
            {
                int finalSum = 0;
                for (int i = 6; i < x.Count(); i++)
                {
                    finalSum = finalSum + x.ToList().Skip(i).FirstOrDefault();
                }
                if (finalSum != 0)
                {
                    List<int> final = new List<int>();

                    final.Add(finalSum);
                    piechartData.Add(
                      new PieSeries
                      {
                          Values = final.AsChartValues(),
                          Title = AppSettings.resourcemanager.GetString("trOthers"),
                          DataLabels = true,
                      }
                  );
                }
            }
            chart1.Series = piechartData;
        }

        private void fillColumnChart(ObservableCollection<int> stackedButton)
        {
            axcolumn.Labels = new List<string>();
            List<string> names = new List<string>();
            IEnumerable<int> x = null;
            IEnumerable<int> y = null;
            IEnumerable<int> z = null;
           
            var temp = InvoicesQuery;
            if (selectedTab == 0)
            {
                temp = temp.Where(j => (selectedBranchId.Count != 0 ? stackedButton.Contains((int)j.branchCreatorId) : true)).ToList();
                var result = temp.GroupBy(s => s.branchCreatorId).Select(s => new
                {
                    branchCreatorId = s.Key,
                    countP  = s.Where(m => m.invType == "s").Count(),
                    countPb = s.Where(m => m.invType == "ts").Count(),
                    countD  = s.Where(m => m.invType == "ss").Count()
                });
                x = result.Select(m => m.countP);
                y = result.Select(m => m.countPb);
                z = result.Select(m => m.countD);
                var tempName = temp.GroupBy(s => s.branchCreatorName).Select(s => new
                {
                    uUserName = s.Key
                });
                names.AddRange(tempName.Select(nn => nn.uUserName));
            }
            else if (selectedTab == 1)
            {
                temp = temp.Where(j => (selectedPosId.Count != 0 ? stackedButton.Contains((int)j.posId) : true)).ToList();
                var result = temp.GroupBy(s => s.posId).Select(s => new
                {
                    posId = s.Key,
                    countP  = s.Where(m => m.invType == "s").Count(),
                    countPb = s.Where(m => m.invType == "ts").Count(),
                    countD  = s.Where(m => m.invType == "ss").Count()
                });
                x = result.Select(m => m.countP);
                y = result.Select(m => m.countPb);
                z = result.Select(m => m.countD);
                var tempName = temp.GroupBy(s => s.posName).Select(s => new
                {
                    uUserName = s.Key + "/" + s.FirstOrDefault().branchCreatorName
                });
                names.AddRange(tempName.Select(nn => nn.uUserName));
            }
            else if (selectedTab == 2)
            {
                temp = temp.Where(j => (selectedVendorsId.Count != 0 ? stackedButton.Contains((int)j.agentId) : true)).ToList();
                var result = temp.GroupBy(s => s.agentId).Select(s => new
                {
                    agentId = s.Key,
                    countP  = s.Where(m => m.invType == "s").Count(),
                    countPb = s.Where(m => m.invType == "ts").Count(),
                    countD  = s.Where(m => m.invType == "ss").Count()

                });
                x = result.Select(m => m.countP);
                y = result.Select(m => m.countPb);
                z = result.Select(m => m.countD);
                var tempName = temp.GroupBy(s => s.agentName).Select(s => new
                {
                    uUserName = s.Key
                });
                names.AddRange(tempName.Select(nn => nn.uUserName));
            }
            else if (selectedTab == 3)
            {
                temp = temp.Where(j => (selectedUserId.Count != 0 ? stackedButton.Contains((int)j.updateUserId) : true)).ToList();
                var result = temp.GroupBy(s => s.updateUserId).Select(s => new
                {
                    updateUserId = s.Key,
                    countP  = s.Where(m => m.invType == "s").Count(),
                    countPb = s.Where(m => m.invType == "ts").Count(),
                    countD  = s.Where(m => m.invType == "ss").Count()

                });
                x = result.Select(m => m.countP);
                y = result.Select(m => m.countPb);
                z = result.Select(m => m.countD);
                var tempName = temp.GroupBy(s => s.uUserAccName).Select(s => new
                {
                    uUserName = s.Key
                });
                names.AddRange(tempName.Select(nn => nn.uUserName));
            }

            SeriesCollection columnChartData = new SeriesCollection();
            List<int> cP = new List<int>();
            List<int> cPb = new List<int>();
            List<int> cD = new List<int>();
            List<string> titles = new List<string>()
            {
                AppSettings.resourcemanager.GetString("trDiningHallType"),
                AppSettings.resourcemanager.GetString("trTakeAway"),
                AppSettings.resourcemanager.GetString("trSelfService")
            };
          
            int xCount = 6;
            if (x.Count() <= 6) xCount = x.Count();

            for (int i = 0; i < xCount; i++)
            {
                cP.Add(x.ToList().Skip(i).FirstOrDefault());
                cPb.Add(y.ToList().Skip(i).FirstOrDefault());
                cD.Add(z.ToList().Skip(i).FirstOrDefault());
                axcolumn.Labels.Add(names.ToList().Skip(i).FirstOrDefault());
            }
            if (x.Count() > 6)
            {
                int cPSum = 0, cPbSum = 0, cDSum = 0;
                for (int i = 6; i < x.Count(); i++)
                {
                    cPbSum = cPSum + x.ToList().Skip(i).FirstOrDefault();
                    cPbSum = cPbSum + y.ToList().Skip(i).FirstOrDefault();
                    cDSum = cDSum + z.ToList().Skip(i).FirstOrDefault();
                }
                if (!((cPbSum == 0) && (cPbSum == 0) && (cDSum == 0)))
                {
                    cP.Add(cPSum);
                    cPb.Add(cPbSum);
                    cD.Add(cDSum);
                    axcolumn.Labels.Add(AppSettings.resourcemanager.GetString("trOthers"));
                }
            }
            //3 فوق بعض
            columnChartData.Add(
            new StackedColumnSeries
            {
                Values = cP.AsChartValues(),
                Title = titles[0],
                DataLabels = true,
            });
            columnChartData.Add(
           new StackedColumnSeries
           {
               Values = cPb.AsChartValues(),
               Title = titles[1],
               DataLabels = true,
           });
            columnChartData.Add(
           new StackedColumnSeries
           {
               Values = cD.AsChartValues(),
               Title = titles[2],
               DataLabels = true,
           });

            DataContext = this;
            cartesianChart.Series = columnChartData;
        }

        private void fillRowChart(ObservableCollection<int> stackedButton)
        {//////?????????????????????
            MyAxis.Labels = new List<string>();
            List<string> names = new List<string>();
            IEnumerable<decimal> sTemp = null;
            IEnumerable<decimal> tsTemp = null;
            IEnumerable<decimal> ssTemp = null;
            IEnumerable<decimal> resultTemp = null;

            var temp = InvoicesQuery;
            if (selectedTab == 0)
            {
                temp = temp.Where(j => (selectedBranchId.Count != 0 ? stackedButton.Contains((int)j.branchCreatorId) : true)).ToList();
                var result = temp.GroupBy(s => s.branchCreatorId).Select(s => new
                {
                    branchCreatorId = s.Key,
                    totals = s.Where(x => x.invType == "s").Sum(x => x.totalNet),
                    totalts = s.Where(x => x.invType == "ts").Sum(x => x.totalNet),
                    totalss = s.Where(x => x.invType == "ss").Sum(x => x.totalNet)
                }

             );
                //var resultTotal = result.Select(x => new { x.branchCreatorId, total = x.totalP - x.totalPb }).ToList();
                sTemp = result.Select(x => (decimal)x.totals);
                tsTemp = result.Select(x => (decimal)x.totalts);
                ssTemp = result.Select(x => (decimal)x.totalss);
                //resultTemp = result.Select(x => (decimal)x.totalP - (decimal)x.totalPb);
                var tempName = temp.GroupBy(s => s.branchCreatorName).Select(s => new
                {
                    name = s.Key
                });
                names.AddRange(tempName.Select(nn => nn.name));
            }
            if (selectedTab == 1)
            {
                temp = temp.Where(j => (selectedPosId.Count != 0 ? stackedButton.Contains((int)j.posId) : true)).ToList();
                var result = temp.GroupBy(s => s.posId).Select(s => new
                {
                    posId = s.Key,
                    totals = s.Where(x => x.invType == "s").Sum(x => x.totalNet),
                    totalts = s.Where(x => x.invType == "ts").Sum(x => x.totalNet),
                    totalss = s.Where(x => x.invType == "ss").Sum(x => x.totalNet)
                }
             );
                //var resultTotal = result.Select(x => new { x.posId, total = x.totalP - x.totalPb }).ToList();
                sTemp = result.Select(x => (decimal)x.totals);
                tsTemp = result.Select(x => (decimal)x.totalts);
                ssTemp = result.Select(x => (decimal)x.totalss);
                //resultTemp = result.Select(x => (decimal)x.totalP - (decimal)x.totalPb);
                var tempName = temp.GroupBy(s => s.posName).Select(s => new
                {
                    name = s.Key
                });
                names.AddRange(tempName.Select(nn => nn.name));
            }
            if (selectedTab == 2)
            {
                temp = temp.Where(j => (selectedVendorsId.Count != 0 ? stackedButton.Contains((int)j.agentId) : true)).ToList();
                var result = temp.GroupBy(s => s.agentId).Select(s => new
                {
                    agentId = s.Key,
                    totals = s.Where(x => x.invType == "s").Sum(x => x.totalNet),
                    totalts = s.Where(x => x.invType == "ts").Sum(x => x.totalNet),
                    totalss = s.Where(x => x.invType == "ss").Sum(x => x.totalNet)
                }
             );
                //var resultTotal = result.Select(x => new { x.agentId, total = x.totalP - x.totalPb }).ToList();
                sTemp = result.Select(x => (decimal)x.totals);
                tsTemp = result.Select(x => (decimal)x.totalts);
                ssTemp = result.Select(x => (decimal)x.totalss);
                //resultTemp = result.Select(x => (decimal)x.totalP - (decimal)x.totalPb);
                var tempName = temp.GroupBy(s => s.agentName).Select(s => new
                {
                    name = s.Key
                });
                names.AddRange(tempName.Select(nn => nn.name));
            }
            if (selectedTab == 3)
            {
                temp = temp.Where(j => (selectedUserId.Count != 0 ? stackedButton.Contains((int)j.updateUserId) : true)).ToList();
                var result = temp.GroupBy(s => s.updateUserId).Select(s => new
                {
                    updateUserId = s.Key,
                    totals = s.Where(x => x.invType == "s").Sum(x => x.totalNet),
                    totalts = s.Where(x => x.invType == "ts").Sum(x => x.totalNet),
                    totalss = s.Where(x => x.invType == "ss").Sum(x => x.totalNet)
                }
             );
                //var resultTotal = result.Select(x => new { x.updateUserId, total = x.totalP - x.totalPb }).ToList();
                sTemp = result.Select(x => (decimal)x.totals);
                tsTemp = result.Select(x => (decimal)x.totalts);
                ssTemp = result.Select(x => (decimal)x.totalss);
                //resultTemp = result.Select(x => (decimal)x.totalP - (decimal)x.totalPb);
                var tempName = temp.GroupBy(s => s.uUserAccName).Select(s => new
                {
                    name = s.Key
                });
                names.AddRange(tempName.Select(nn => nn.name));
            }

            SeriesCollection rowChartData = new SeriesCollection();
            List<decimal> dinningHall = new List<decimal>();
            List<decimal> takeAway = new List<decimal>();
            List<decimal> selfService = new List<decimal>();
            List<string> titles = new List<string>()
            {
                AppSettings.resourcemanager.GetString("trDiningHallType"),
                AppSettings.resourcemanager.GetString("trTakeAway"),
                AppSettings.resourcemanager.GetString("trSelfService")
            };
          
            int xCount = 0;
            if (sTemp.Count() <= 6) xCount = sTemp.Count();
            for (int i = 0; i < xCount; i++)
            {
                dinningHall.Add(sTemp.ToList().Skip(i).FirstOrDefault());
                takeAway.Add(tsTemp.ToList().Skip(i).FirstOrDefault());
                selfService.Add(ssTemp.ToList().Skip(i).FirstOrDefault());
                MyAxis.Labels.Add(names.ToList().Skip(i).FirstOrDefault());
            }
            if (sTemp.Count() > 6)
            {
                decimal dinningSum = 0, takeSum = 0, selfSum = 0;
                for (int i = 6; i < sTemp.Count(); i++)
                {
                    dinningSum = dinningSum + sTemp.ToList().Skip(i).FirstOrDefault();
                    takeSum = takeSum + tsTemp.ToList().Skip(i).FirstOrDefault();
                    selfSum = selfSum + ssTemp.ToList().Skip(i).FirstOrDefault();
                }
                if (!((dinningSum == 0) && (takeSum == 0) && (selfSum == 0)))
                {
                    dinningHall.Add(dinningSum);
                    takeAway.Add(takeSum);
                    selfService.Add(selfSum);
                    MyAxis.Labels.Add(AppSettings.resourcemanager.GetString("trOthers"));
                }
            }

            rowChartData.Add(
          new LineSeries
          {
              Values = dinningHall.AsChartValues(),
              Title = titles[0]
          });
            rowChartData.Add(
         new LineSeries
         {
             Values = takeAway.AsChartValues(),
             Title = titles[1]
         });
            rowChartData.Add(
        new LineSeries
        {
            Values = selfService.AsChartValues(),
            Title = titles[2]

        });
            DataContext = this;
            rowChart.Series = rowChartData;
        }

        #endregion

        #region reports
        private List<ItemTransferInvoice> fillPdfList(ComboBox comboBox, ObservableCollection<int> stackedButton)
        {
            List<ItemTransferInvoice> list = new List<ItemTransferInvoice>();

            //var temp = invLstRowChart;
            //if (selectedTab == 0)
            //{
            //    temp = temp.Where(j => (selectedBranchId.Count != 0 ? stackedButton.Contains((int)j.branchCreatorId) : true));
            //    list = temp.ToList();
            //}
            //if (selectedTab == 1)
            //{
            //    temp = temp.Where(j => (selectedPosId.Count != 0 ? stackedButton.Contains((int)j.posId) : true));
            //    list = temp.ToList();
            //}
            //if (selectedTab == 3)
            //{
            //    temp = temp.Where(j => (selectedUserId.Count != 0 ? stackedButton.Contains((int)j.updateUserId) : true));
            //    list = temp.ToList();
            //}

            return list;

        }
        public List<ItemTransferInvoice> filltoprint()
        {
            List<ItemTransferInvoice> xx = new List<ItemTransferInvoice>();
            //if (selectedTab == 0)
            //{
            //    xx = fillPdfList(cb_branches, selectedBranchId);
            //}
            //else if (selectedTab == 1)
            //{
            //    xx = fillPdfList(cb_branches, selectedPosId);
            //}
            //else if (selectedTab == 2)
            //{
            //    xx = fillPdfList(cb_branches, selectedVendorsId);
            //}
            //else if (selectedTab == 3)
            //{
            //    xx = fillPdfList(cb_branches, selectedUserId);
            //}

            return xx;
        }
        public void BuildReport()
        {
            List<ReportParameter> paramarr = new List<ReportParameter>();
            string addpath = "";
            string firstTitle = "invoice";
            string secondTitle = "";
            string subTitle = "";
            string Title = "";


            bool isArabic = ReportCls.checkLang();
            if (isArabic)
            {
                if (selectedTab == 0)
                {
                    addpath = @"\Reports\StatisticReport\Sale\Ar\ArPurSts.rdlc";
                    secondTitle = "branch";
                    subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);
                }
                else if (selectedTab == 1)
                {
                    addpath = @"\Reports\StatisticReport\Sale\Ar\ArPurPosSts.rdlc";
                    secondTitle = "pos";
                    subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);
                }
                else if (selectedTab == 2)
                {
                    addpath = @"\Reports\StatisticReport\Sale\Ar\ArPurVendorSts.rdlc";
                    secondTitle = "customers";
                    subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);

                }
                else
                {
                    addpath = @"\Reports\StatisticReport\Sale\Ar\ArPurUserSts.rdlc";
                    secondTitle = "users";
                    subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);

                }

            }
            else
            {
                //english
                if (selectedTab == 0)
                {
                    addpath = @"\Reports\StatisticReport\Sale\En\EnPurSts.rdlc";
                    secondTitle = "branch";
                    subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);
                }
                else if (selectedTab == 1)
                {
                    addpath = @"\Reports\StatisticReport\Sale\En\EnPurPosSts.rdlc";
                    secondTitle = "pos";
                    subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);
                }
                else if (selectedTab == 2)
                {
                    addpath = @"\Reports\StatisticReport\Sale\En\EnPurVendorSts.rdlc";
                    secondTitle = "customers";
                    subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);
                }
                else
                {
                    addpath = @"\Reports\StatisticReport\Sale\En\EnPurUserSts.rdlc";
                    secondTitle = "users";
                    subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);
                }

            }

            string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);

            ReportCls.checkLang();
            Title = AppSettings.resourcemanagerreport.GetString("trSalesReport") + " / " + subTitle;
            paramarr.Add(new ReportParameter("trTitle", Title));

            clsReports.SaleInvoiceStsReport(itemTransfers, rep, reppath, paramarr);
            clsReports.setReportLanguage(paramarr);
            clsReports.Header(paramarr);

            rep.SetParameters(paramarr);

            rep.Refresh();

        }
        private void Btn_pdf_Click(object sender, RoutedEventArgs e)
        {//pdf
            try
            {

                HelpClass.StartAwait(grid_main);

                #region
                BuildReport();

                saveFileDialog.Filter = "PDF|*.pdf;";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filepath = saveFileDialog.FileName;
                    LocalReportExtensions.ExportToPDF(rep, filepath);
                }
                #endregion


                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_print_Click(object sender, RoutedEventArgs e)
        {//print
            try
            {

                HelpClass.StartAwait(grid_main);

                #region
                BuildReport();

                LocalReportExtensions.PrintToPrinterbyNameAndCopy(rep, AppSettings.rep_printer_name, short.Parse(AppSettings.rep_print_count));
                #endregion


                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_exportToExcel_Click(object sender, RoutedEventArgs e)
        {//excel
            try
            {
               
                HelpClass.StartAwait(grid_main);

                #region
                //  Thread t1 = new Thread(() =>
                //{

                BuildReport();
                this.Dispatcher.Invoke(() =>
                {
                    saveFileDialog.Filter = "EXCEL|*.xls;";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string filepath = saveFileDialog.FileName;
                        LocalReportExtensions.ExportToExcel(rep, filepath);
                    }
                });

                //});
                //  t1.Start();
                #endregion

               
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
               
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_preview_Click(object sender, RoutedEventArgs e)
        {//preview
            try
            {
               
                HelpClass.StartAwait(grid_main);

                #region
                Window.GetWindow(this).Opacity = 0.2;
                string pdfpath = "";




                pdfpath = @"\Thumb\report\temp.pdf";
                pdfpath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, pdfpath);
                BuildReport();



                LocalReportExtensions.ExportToPDF(rep, pdfpath);
                wd_previewPdf w = new wd_previewPdf();
                w.pdfPath = pdfpath;
                if (!string.IsNullOrEmpty(w.pdfPath))
                {
                    w.ShowDialog();
                    w.wb_pdfWebViewer.Dispose();
                }
                Window.GetWindow(this).Opacity = 1;
                #endregion

               
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
               
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        #endregion

       
    }
}
