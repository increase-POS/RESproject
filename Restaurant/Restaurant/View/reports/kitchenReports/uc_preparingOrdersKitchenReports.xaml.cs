using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using Restaurant.Classes;
using Restaurant.View.windows;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Restaurant.View.reports.kitchenReports
{
    /// <summary>
    /// Interaction logic for uc_preparingOrdersKitchenReports.xaml
    /// </summary>
    public partial class uc_preparingOrdersKitchenReports : UserControl
    {
        IEnumerable<OrderPreparingSTS> orders;
        Statistics statisticsModel = new Statistics();
        IEnumerable<OrderPreparingSTS> ordersQuery;
        
        //prin & pdf
        ReportCls reportclass = new ReportCls();
        LocalReport rep = new LocalReport();
        SaveFileDialog saveFileDialog = new SaveFileDialog();

        string searchText = "";

        private static uc_preparingOrdersKitchenReports _instance;
        public static uc_preparingOrdersKitchenReports Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new uc_preparingOrdersKitchenReports();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_preparingOrdersKitchenReports()
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

                chk_allBranches.IsChecked = true;
                chk_allCategories.IsChecked = true;

                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), btn_preparingOrders.Tag.ToString());

            //}
            //catch (Exception ex)
            //{
            //    HelpClass.ExceptionMessage(ex, this);
            //}
        }

        #region methods
        private void translate()
        {
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_startDate, AppSettings.resourcemanager.GetString("trStartDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_endDate, AppSettings.resourcemanager.GetString("trEndDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(txt_search, AppSettings.resourcemanager.GetString("trSearchHint"));

            chk_allBranches.Content = AppSettings.resourcemanager.GetString("trAll");
            chk_allCategories.Content = AppSettings.resourcemanager.GetString("trAll");

            tt_preparingOrder.Content = AppSettings.resourcemanager.GetString("trPreparingOrders");

            col_invNum.Header = AppSettings.resourcemanager.GetString("trNum");
            col_date.Header = AppSettings.resourcemanager.GetString("trDate");
            col_itemName.Header = AppSettings.resourcemanager.GetString("trItem");
            col_quantity.Header = AppSettings.resourcemanager.GetString("trQTR");
            col_branch.Header = AppSettings.resourcemanager.GetString("trBranch");
            col_category.Header = AppSettings.resourcemanager.GetString("trCategorie");
            col_tag.Header = AppSettings.resourcemanager.GetString("trTag");
            col_status.Header = AppSettings.resourcemanager.GetString("trStatus");
            col_duration.Header = AppSettings.resourcemanager.GetString("duration");

            tt_refresh.Content = AppSettings.resourcemanager.GetString("trRefresh");
            tt_report.Content = AppSettings.resourcemanager.GetString("trPdf");
            tt_print.Content = AppSettings.resourcemanager.GetString("trPrint");
            tt_excel.Content = AppSettings.resourcemanager.GetString("trExcel");
            tt_count.Content = AppSettings.resourcemanager.GetString("trCount");
        }

        async Task Search()
        {

            if (orders is null)
                await RefreshPreparingOredersList();

            searchText = txt_search.Text.ToLower();
            ordersQuery = orders
                .Where(s =>
            (
             s.invNumber.ToLower().Contains(searchText)
            ||
            s.branchName.ToLower().Contains(searchText)
            ||
            s.categoryName.ToLower().Contains(searchText)
            ||
            s.itemName.ToLower().Contains(searchText)
            ||
            s.status.ToLower().Contains(searchText)
            ||
            (s.tagName != null ? s.tagName.ToLower().Contains(searchText) : false)
            )
            &&
            //branch
            (cb_branches.SelectedIndex != -1 ? s.branchId == Convert.ToInt32(cb_branches.SelectedValue) : true)
            &&
            //category
            (cb_category.SelectedIndex != -1 ? s.categoryId == Convert.ToInt32(cb_category.SelectedValue) : true)
            &&
            //start date
            (dp_startDate.SelectedDate != null ? s.createDate >= dp_startDate.SelectedDate : true)
            &&
            //end date
            (dp_endDate.SelectedDate != null ? s.createDate <= dp_endDate.SelectedDate : true)
            );

            RefreshPreparingOrdersView();
            fillBranches();
            fillCategories();
            fillColumnChart();
            fillPieChart();

        }

        void RefreshPreparingOrdersView()
        {
            dg_orders.ItemsSource = ordersQuery;
            txt_count.Text = ordersQuery.Count().ToString();
        }

        async Task<IEnumerable<OrderPreparingSTS>> RefreshPreparingOredersList()
        {
            orders = await statisticsModel.GetPreparingOrders(MainWindow.branchLogin.branchId, MainWindow.userLogin.userId);
            return orders;
        }

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
        private void fillBranches()
        {
            cb_branches.SelectedValuePath = "branchId";
            cb_branches.DisplayMemberPath = "branchName";
            cb_branches.ItemsSource = orders.Select(i => new { i.branchName, i.branchId }).Distinct();
        }
        private void fillCategories()
        {
            cb_category.SelectedValuePath = "categoryId";
            cb_category.DisplayMemberPath = "categoryName";
            cb_category.ItemsSource = orders.Select(i => new { i.categoryName, i.categoryId }).Distinct();
        }
        #endregion

        #region events
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }

        private void RefreshView_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            callSearch(sender);
        }

        private async void cb_branches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//select branch
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

        private void Chk_allBranches_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                cb_branches.SelectedIndex = -1;
                cb_branches.IsEnabled = false;

                chk_allCategories.IsChecked = true;

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

        private async void Cb_category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//select category
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

        private async void Chk_allCategories_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                cb_category.SelectedIndex = -1;
                cb_category.IsEnabled = false;

                await Search();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Chk_allCategories_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                cb_category.IsEnabled = true;

                await Search();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Txt_search_TextChanged(object sender, TextChangedEventArgs e)
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

        private async void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {//refresh
            try
            {
                HelpClass.StartAwait(grid_main);

                searchText = "";
                txt_search.Text = "";
                await RefreshPreparingOredersList();
                dp_startDate.SelectedDate = null;
                dp_endDate.SelectedDate = null;
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
        private void fillColumnChart()
        {
            //axcolumn.Labels = new List<string>();
            //List<string> names = new List<string>();
            //List<decimal> balances = new List<decimal>();

            ////var temp = balancesQuery;
            //var result = balancesQuery.GroupBy(s => s.posId).Select(s => new
            //{
            //    posId = s.Key,
            //});

            //var tempName = balancesQuery.GroupBy(s => s.posName + "/" + s.branchName).Select(s => new
            //{
            //    posName = s.Key
            //});
            //names.AddRange(tempName.Select(nn => nn.posName));

            //var tempBalance = balancesQuery.GroupBy(s => s.balance).Select(s => new
            //{
            //    balance = s.Key
            //});
            //balances.AddRange(tempBalance.Select(nn => decimal.Parse(HelpClass.DecTostring(nn.balance.Value))));

            //List<string> lable = new List<string>();
            //SeriesCollection columnChartData = new SeriesCollection();
            //List<decimal> cS = new List<decimal>();

            //List<string> titles = new List<string>()
            //{
            //   AppSettings.resourcemanager.GetString("tr_Balance")
            //};
            //int x = 6;
            //if (names.Count() <= 6) x = names.Count();

            //for (int i = 0; i < x; i++)
            //{
            //    cS.Add(balances.ToList().Skip(i).FirstOrDefault());
            //    axcolumn.Labels.Add(names.ToList().Skip(i).FirstOrDefault());
            //}

            //if (names.Count() > 6)
            //{
            //    decimal balanceSum = 0;
            //    for (int i = 6; i < names.Count(); i++)
            //        balanceSum = balanceSum + balances.ToList().Skip(i).FirstOrDefault();

            //    if (balanceSum != 0)
            //        cS.Add(balanceSum);

            //    axcolumn.Labels.Add(AppSettings.resourcemanager.GetString("trOthers"));
            //}

            //columnChartData.Add(
            //new StackedColumnSeries
            //{
            //    Values = cS.AsChartValues(),
            //    Title = titles[0],
            //    DataLabels = true,
            //});

            //DataContext = this;
            //cartesianChart.Series = columnChartData;
        }

        private void fillPieChart()
        {
            //List<string> titles = new List<string>();
            //IEnumerable<int> x = null;
            //IEnumerable<decimal> balances = null;

            //titles.Clear();

            ////var temp = balancesQuery;
            //var titleTemp = balancesQuery.GroupBy(m => m.branchName);
            //titles.AddRange(titleTemp.Select(jj => jj.Key));
            //var result = balancesQuery.GroupBy(s => s.branchId)
            //            .Select(
            //                g => new
            //                {
            //                    branchId = g.Key,
            //                    balance = g.Sum(s => s.balance),
            //                    count = g.Count()
            //                });
            //balances = result.Select(m => decimal.Parse(HelpClass.DecTostring(m.balance.Value)));

            //SeriesCollection piechartData = new SeriesCollection();
            //for (int i = 0; i < balances.Count(); i++)
            //{
            //    List<decimal> final = new List<decimal>();
            //    List<string> lable = new List<string>();
            //    final.Add(balances.ToList().Skip(i).FirstOrDefault());
            //    piechartData.Add(
            //      new PieSeries
            //      {
            //          Values = final.AsChartValues(),
            //          Title = titles.Skip(i).FirstOrDefault(),
            //          DataLabels = true,
            //      }
            //  );
            //}
            //chart1.Series = piechartData;
        }

        private void fillRowChart()
        {

        }

        #endregion

        #region report
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
            }
        }

        private void Btn_print_Click(object sender, RoutedEventArgs e)
        {//print
            try
            {
                HelpClass.StartAwait(grid_main);
                List<ItemTransferInvoice> query = new List<ItemTransferInvoice>();

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
                List<ItemTransferInvoice> query = new List<ItemTransferInvoice>();

                #region
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
                    // w.ShowInTaskbar = false;
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
        private void BuildReport()
        {
            List<ReportParameter> paramarr = new List<ReportParameter>();

            string addpath;

            string firstTitle = "accountProfits";
            string secondTitle = "";
            string subTitle = "";
            string Title = "";

            bool isArabic = ReportCls.checkLang();
            if (isArabic)
            {
                addpath = @"\Reports\StatisticReport\Accounts\Profit\Ar\ProfitItem.rdlc";
                secondTitle = "items";

                //Reports\StatisticReport\Sale\Daily\Ar
            }
            else
            {
                addpath = @"\Reports\StatisticReport\Accounts\Profit\En\ProfitItem.rdlc";
                secondTitle = "items";
            }

            string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);

            ReportCls.checkLang();
            subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);
            Title = AppSettings.resourcemanagerreport.GetString("trAccounting") + " / " + subTitle;
            paramarr.Add(new ReportParameter("trTitle", Title));

            // IEnumerable<ItemUnitInvoiceProfit>
            //clsReports.ProfitReport(ordersQuery, rep, reppath, paramarr);
            //paramarr.Add(new ReportParameter("totalBalance", tb_total.Text));

            clsReports.setReportLanguage(paramarr);
            clsReports.Header(paramarr);

            rep.SetParameters(paramarr);

            rep.Refresh();
        }


        #endregion
    }
}
