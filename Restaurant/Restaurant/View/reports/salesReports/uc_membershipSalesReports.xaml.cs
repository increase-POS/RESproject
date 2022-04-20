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

namespace Restaurant.View.reports.salesReports
{
    /// <summary>
    /// Interaction logic for uc_membershipSalesReports.xaml
    /// </summary>
    public partial class uc_membershipSalesReports : UserControl
    {

        private static uc_membershipSalesReports _instance;
        public static uc_membershipSalesReports Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new uc_membershipSalesReports();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public uc_membershipSalesReports()
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
            //try
            //{
            #region translate
            if (AppSettings.lang.Equals("en"))
                grid_main.FlowDirection = FlowDirection.LeftToRight;
            else
                grid_main.FlowDirection = FlowDirection.RightToLeft;
            translate();
            #endregion

            //chk_allCategories.IsChecked = true;

            //HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), btn_preparingOrders.Tag.ToString());

            //}
            //catch (Exception ex)
            //{
            //    HelpClass.ExceptionMessage(ex, this);
            //}
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }

        #region methods
        private void translate()
        {
            //MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_startDate, AppSettings.resourcemanager.GetString("trStartDateHint"));
            //MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_endDate, AppSettings.resourcemanager.GetString("trEndDateHint"));
            //MaterialDesignThemes.Wpf.HintAssist.SetHint(txt_search, AppSettings.resourcemanager.GetString("trSearchHint"));
            //MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_branches, AppSettings.resourcemanager.GetString("trBranch") + "...");
            //MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_category, AppSettings.resourcemanager.GetString("trCategorie") + "...");

            //chk_allBranches.Content = AppSettings.resourcemanager.GetString("trAll");
            //chk_allCategories.Content = AppSettings.resourcemanager.GetString("trAll");

            //tt_preparingOrder.Content = AppSettings.resourcemanager.GetString("trPreparingOrders");

            //col_orderNum.Header = AppSettings.resourcemanager.GetString("trNum");
            //col_invNum.Header = AppSettings.resourcemanager.GetString("trInvoiceNumber");
            //col_date.Header = AppSettings.resourcemanager.GetString("trDate");
            //col_itemName.Header = AppSettings.resourcemanager.GetString("trItem");
            //col_quantity.Header = AppSettings.resourcemanager.GetString("trQTR");
            //col_branch.Header = AppSettings.resourcemanager.GetString("trBranch");
            //col_category.Header = AppSettings.resourcemanager.GetString("trCategorie");
            //col_tag.Header = AppSettings.resourcemanager.GetString("trTag");
            //col_status.Header = AppSettings.resourcemanager.GetString("trStatus");
            //col_duration.Header = AppSettings.resourcemanager.GetString("duration");

            tt_refresh.Content = AppSettings.resourcemanager.GetString("trRefresh");
            tt_report.Content = AppSettings.resourcemanager.GetString("trPdf");
            tt_print.Content = AppSettings.resourcemanager.GetString("trPrint");
            tt_preview.Content = AppSettings.resourcemanager.GetString("trPreview");
            tt_excel.Content = AppSettings.resourcemanager.GetString("trExcel");
            tt_count.Content = AppSettings.resourcemanager.GetString("trCount");
        }

        //async Task Search()
        //{

        //    if (orders is null)
        //        await RefreshPreparingOredersList();

        //    searchText = txt_search.Text.ToLower();
        //    ordersQuery = orders
        //        .Where(s =>
        //    (
        //     s.invNumber.ToLower().Contains(searchText)
        //    ||
        //    s.branchName.ToLower().Contains(searchText)
        //    ||
        //    s.categoryName.ToLower().Contains(searchText)
        //    ||
        //    s.itemName.ToLower().Contains(searchText)
        //    ||
        //    s.status.ToLower().Contains(searchText)
        //    ||
        //    (s.tagName != null ? s.tagName.ToLower().Contains(searchText) : false)
        //    )
        //    &&
        //    //branch
        //    (cb_branches.SelectedIndex != -1 ? s.branchId == Convert.ToInt32(cb_branches.SelectedValue) : false)
        //    &&
        //    //category
        //    (cb_category.SelectedIndex != -1 ? s.categoryId == Convert.ToInt32(cb_category.SelectedValue) : true)
        //    &&
        //    //start date
        //    (dp_startDate.SelectedDate != null ? s.createDate >= dp_startDate.SelectedDate : true)
        //    &&
        //    //end date
        //    (dp_endDate.SelectedDate != null ? s.createDate <= dp_endDate.SelectedDate : true)
        //    );

        //    RefreshPreparingOrdersView();

        //    fillColumnChart();
        //    fillPieChart();
        //    fillRowChart();

        //}

        //void RefreshPreparingOrdersView()
        //{
        //    dg_orders.ItemsSource = ordersQuery;
        //    txt_count.Text = ordersQuery.Count().ToString();
        //}

        //async Task<IEnumerable<OrderPreparingSTS>> RefreshPreparingOredersList()
        //{
        //    orders = await statisticsModel.GetPreparingOrders(MainWindow.branchLogin.branchId, MainWindow.userLogin.userId);
        //    fillBranches();
        //    fillCategories();
        //    return orders;
        //}

        //private async void callSearch(object sender)
        //{
        //    try
        //    {
        //        HelpClass.StartAwait(grid_main);

        //        await Search();

        //        HelpClass.EndAwait(grid_main);
        //    }
        //    catch (Exception ex)
        //    {
        //        HelpClass.EndAwait(grid_main);
        //        HelpClass.ExceptionMessage(ex, this);
        //    }
        //}
        //private void fillBranches()
        //{
        //    cb_branches.SelectedValuePath = "branchId";
        //    cb_branches.DisplayMemberPath = "branchName";
        //    cb_branches.ItemsSource = orders.Select(i => new { i.branchName, i.branchId }).Distinct();
        //}
        //private void fillCategories()
        //{
        //    cb_category.SelectedValuePath = "categoryId";
        //    cb_category.DisplayMemberPath = "categoryName";
        //    cb_category.ItemsSource = orders.Select(i => new { i.categoryName, i.categoryId }).Distinct();
        //}
        #endregion

        #region events
        private void RefreshView_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cb_branches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Chk_allBranches_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Chk_allBranches_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void Cb_membership_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Chk_allMemberships_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Chk_allMemberships_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void Txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region print datagrid btns
        private void pdfRowinDatagrid(object sender, RoutedEventArgs e)
        {

        }

        private void printRowinDatagrid(object sender, RoutedEventArgs e)
        {

        }

        private void excelRowinDatagrid(object sender, RoutedEventArgs e)
        {

        }

        private void previewRowinDatagrid(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region membership datagrid btns
        private void customerRowinDatagrid(object sender, RoutedEventArgs e)
        {

        }

        private void customersRowinDatagrid(object sender, RoutedEventArgs e)
        {

        }

        private void couponsRowinDatagrid(object sender, RoutedEventArgs e)
        {

        }

        private void offersRowinDatagrid(object sender, RoutedEventArgs e)
        {

        }

        private void invoicesClassesRowinDatagrid(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region charts
        #endregion

        #region reports
        private void Btn_pdf_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_print_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_exportToExcel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_preview_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion


    }
}
