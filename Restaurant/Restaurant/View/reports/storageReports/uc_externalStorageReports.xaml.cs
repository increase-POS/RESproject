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
using static Restaurant.Classes.Statistics;
using System.Globalization;
using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using System.IO;
using System.Threading;
using Restaurant.View.storage;
using System.Resources;
using System.Reflection;
using Restaurant.View.storage.movementsOperations;

namespace Restaurant.View.reports.storageReports
{
    /// <summary>
    /// Interaction logic for uc_externalStorageReports.xaml
    /// </summary>
    public partial class uc_externalStorageReports : UserControl
    {

        private static uc_externalStorageReports _instance;
        public static uc_externalStorageReports Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new uc_externalStorageReports();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_externalStorageReports()
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

        List<ItemTransferInvoice> itemsTransfer;
        List<ItemTransferInvoice> itemsTransferQuery;

        IEnumerable<ItemTransferInvoice> invCount;

        Statistics statisticModel = new Statistics();
        string searchText = "";
        int selectedTab = 0;


        ReportCls reportclass = new ReportCls();
        LocalReport rep = new LocalReport();
        SaveFileDialog saveFileDialog = new SaveFileDialog();

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

                //await FillCombo.fillComboBranchesAllWithoutMain(cb_branches);

                btn_externalItems_Click(btn_item, null);
               
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
        async Task<IEnumerable<ItemTransferInvoice>> RefreshExternalList()
        {
            itemsTransfer = await statisticModel.GetExternalMov((int)MainWindow.branchLogin.branchId, (int)MainWindow.userLogin.userId);
            fillBranches();
            fillItems();
            fillVendors();
            return itemsTransfer;
        }

        async Task Search()
        {
            try
            {
                if (itemsTransfer == null)
                    await RefreshExternalList();

                searchText = txt_search.Text.ToLower();

                itemsTransferQuery = itemsTransfer
             .Where(s =>
              (
               s.branchName.Contains(searchText) ||
               s.itemName.Contains(searchText) ||
               s.unitName.Contains(searchText) ||
               s.agentName.Contains(searchText) ||
               s.invNumber.Contains(searchText) ||
               s.invType.Contains(searchText)
              )
             &&
             //branchID
            ( cb_branches.SelectedIndex != -1 ? s.branchId == Convert.ToInt32(cb_branches.SelectedValue) : true)
             &&
             //in
            ((chk_in.IsChecked == true   ? (s.invType == "p") : false)
             || 
             //out
             (chk_out.IsChecked == true ? (s.invType == "pb") : false))
             &&
             //item
             (cb_items.SelectedItem != null ? s.itemId == Convert.ToInt32(cb_items.SelectedValue) : true)
             &&
             //unit
             (cb_units.SelectedItem != null ? s.unitId == Convert.ToInt32(cb_units.SelectedValue) : true)
             &&
             //vendor
             (cb_vendor.SelectedItem != null ? s.agentId == Convert.ToInt32(cb_vendor.SelectedValue) : true)
             && 
             //start date
             (dp_startDate.SelectedDate != null ? s.invDate == dp_startDate.SelectedDate : true)
             &&
             //end date
             (dp_endDate.SelectedDate != null ? s.invDate == dp_endDate.SelectedDate : true)

             ).ToList();

             RefreshExternalView();
               
            }
            catch { }
        }

        void RefreshExternalView()
        {
            dgStock.ItemsSource = itemsTransferQuery;
            txt_count.Text = itemsTransferQuery.Count().ToString();

            fillExternalColumnChart();
            fillExternalPieChart();
            fillExternalRowChart();
        }

        private void translate()
        {
            tt_item.Content = AppSettings.resourcemanager.GetString("trItems");
            tt_agent.Content = AppSettings.resourcemanager.GetString("trVendors");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_branches, AppSettings.resourcemanager.GetString("trBranch/StoreHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_items, AppSettings.resourcemanager.GetString("trItemHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_units, AppSettings.resourcemanager.GetString("trUnitHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_vendor, AppSettings.resourcemanager.GetString("trVendorHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_startDate, AppSettings.resourcemanager.GetString("trStartDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_endDate, AppSettings.resourcemanager.GetString("trEndDateHint"));

            chk_allBranches.Content = AppSettings.resourcemanager.GetString("trAll");
            chk_allItems.Content = AppSettings.resourcemanager.GetString("trAll");
            chk_allUnits.Content = AppSettings.resourcemanager.GetString("trAll");
            chk_in.Content = AppSettings.resourcemanager.GetString("trIn");
            chk_out.Content = AppSettings.resourcemanager.GetString("trOut");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(txt_search, AppSettings.resourcemanager.GetString("trSearchHint"));
            tt_refresh.Content = AppSettings.resourcemanager.GetString("trRefresh");

            col_invNumber.Header = AppSettings.resourcemanager.GetString("trNo.");
            col_invTypeNumber.Header = AppSettings.resourcemanager.GetString("trType");
            col_invDate.Header = AppSettings.resourcemanager.GetString("trDate");
            col_branch.Header = AppSettings.resourcemanager.GetString("trBranch");
            col_item.Header = AppSettings.resourcemanager.GetString("trItem");
            col_unit.Header = AppSettings.resourcemanager.GetString("trUnit");
            col_agentType.Header = AppSettings.resourcemanager.GetString("trAgentType");
            col_agent.Header = AppSettings.resourcemanager.GetString("trName");
            col_itemUnits.Header = AppSettings.resourcemanager.GetString("trItemUnit");
            col_agentTypeAgent.Header = AppSettings.resourcemanager.GetString("trType")+"-"+ AppSettings.resourcemanager.GetString("trName");
            col_quantity.Header = AppSettings.resourcemanager.GetString("trQTR");

            tt_report.Content = AppSettings.resourcemanager.GetString("trPdf");
            tt_print.Content = AppSettings.resourcemanager.GetString("trPrint");
            tt_excel.Content = AppSettings.resourcemanager.GetString("trExcel");
            tt_preview.Content = AppSettings.resourcemanager.GetString("trPreview");
            tt_count.Content = AppSettings.resourcemanager.GetString("trCount");

        }

        private void showSelectedTabColumn()
        {
            hideAllColumn();

            if (selectedTab == 0)
            {
                col_branch.Visibility = Visibility.Visible;
                col_item.Visibility = Visibility.Visible;
                col_unit.Visibility = Visibility.Visible;
                col_quantity.Visibility = Visibility.Visible;
                col_agentTypeAgent.Visibility = Visibility.Visible;
                col_invTypeNumber.Visibility = Visibility.Visible;
                col_invNumber.Visibility = Visibility.Visible;
                col_invDate.Visibility = Visibility.Visible;
            }
            else if (selectedTab == 1)
            {
                col_branch.Visibility = Visibility.Visible;
                col_itemUnits.Visibility = Visibility.Visible;
                col_agent.Visibility = Visibility.Visible;
                col_quantity.Visibility = Visibility.Visible;
                col_agentType.Visibility = Visibility.Visible;
                col_invTypeNumber.Visibility = Visibility.Visible;
                col_invNumber.Visibility = Visibility.Visible;
                col_invDate.Visibility = Visibility.Visible;
            }
        }

        private void hideAllColumn()
        {
            col_branch.Visibility = Visibility.Hidden;
            col_item.Visibility = Visibility.Hidden;
            col_unit.Visibility = Visibility.Hidden;
            col_quantity.Visibility = Visibility.Hidden;
            col_itemUnits.Visibility = Visibility.Hidden;
            col_invNumber.Visibility = Visibility.Hidden;
            col_invTypeNumber.Visibility = Visibility.Hidden;
            col_agentType.Visibility = Visibility.Hidden;
            col_agent.Visibility = Visibility.Hidden;
            col_agentTypeAgent.Visibility = Visibility.Hidden;
        }

        private void fillBranches()
        {
            cb_branches.SelectedValuePath = "branchId";
            cb_branches.DisplayMemberPath = "branchName";
            cb_branches.ItemsSource = itemsTransfer.GroupBy(g => g.branchId).Select(i => new { i.FirstOrDefault().branchName, i.FirstOrDefault().branchId });
        }
        private void fillItems()
        {
            cb_items.SelectedValuePath = "itemId";
            cb_items.DisplayMemberPath = "itemName";
            cb_items.ItemsSource = itemsTransfer.GroupBy(g => g.itemId).Select(i => new { i.FirstOrDefault().itemName, i.FirstOrDefault().itemId });
        }
        private void fillUnits()
        {
            cb_units.SelectedValuePath = "unitId";
            cb_units.DisplayMemberPath = "unitName";
            cb_units.ItemsSource = itemsTransfer.Where(i => i.itemId == (int)cb_items.SelectedValue)
                                                .GroupBy(g => g.unitId)
                                                .Select(i => new { i.FirstOrDefault().unitName, i.FirstOrDefault().unitId });
        }
        private void fillVendors()
        {
            cb_vendor.SelectedValuePath = "agentId";
            cb_vendor.DisplayMemberPath = "AgentNameAgent";
            cb_vendor.ItemsSource = itemsTransfer.GroupBy(g => g.agentId).Select(i => new { i.FirstOrDefault().AgentNameAgent, i.FirstOrDefault().agentId });
        }
        #endregion

        #region events
        private async void btn_externalItems_Click(object sender, RoutedEventArgs e)
        {//items
            try
            {
                HelpClass.StartAwait(grid_main);

                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), (sender as Button).Tag.ToString());

                selectedTab = 0;
                txt_search.Text = "";

                path_agent.Fill = Brushes.White;
                //bdrMain.RenderTransform = Animations.borderAnimation(50, bdrMain, true);
                ReportsHelp.paintTabControlBorder(grid_tabControl, bdr_item);
                path_item.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;

                showSelectedTabColumn();
                row_agent.Height = new GridLength(0);
                row_item.Height = row_branch.Height;

                chk_in.IsChecked = true;
                chk_out.IsChecked = true;
                chk_allBranches.IsChecked = true;
                chk_allItems.IsChecked = true;
                chk_allUnits.IsChecked = true;

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void btn_externalAgents_Click(object sender, RoutedEventArgs e)
        {//agents
            try
            {
                HelpClass.StartAwait(grid_main);

                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), (sender as Button).Tag.ToString());

                selectedTab = 1;
                txt_search.Text = "";

                path_item.Fill = Brushes.White;
                //bdrMain.RenderTransform = Animations.borderAnimation(50, bdrMain, true);
                ReportsHelp.paintTabControlBorder(grid_tabControl, bdr_agent);
                path_agent.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;

                showSelectedTabColumn();
                row_item.Height = new GridLength(0);
                row_agent.Height = row_branch.Height;

                chk_in.IsChecked = true;
                chk_out.IsChecked = true;
                chk_allBranches.IsChecked = true;
                chk_allItems.IsChecked = true;
                chk_allVendors.IsChecked = true;

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void chk_externalItemsAllBranches_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                cb_branches.IsEnabled = false;
                cb_branches.SelectedItem = null;

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {//search
            callSearch(sender);
        }
        private void selectionChanged(object sender, RoutedEventArgs e)
        {
            callSearch(sender);
        }
        private void cb_externalItemsBranches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            callSearch(sender);
        }
        private void chk_externalItemsAllItems_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                cb_items.IsEnabled = false;
                cb_items.SelectedItem = null;
                chk_allUnits.IsChecked = true;

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void cb_externalItemsItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                cb_units.IsEnabled = true;
                chk_allUnits.IsEnabled = true;

                fillUnits();

                await Search();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            callSearch(sender);
        }
        private void chk_externalItemsAllUnits_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                cb_units.IsEnabled = false;
                cb_units.SelectedItem = null;

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void chk_externalItemsAllUnits_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                cb_units.IsEnabled = true;

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void chk_externalItemsAllBranches_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                cb_branches.IsEnabled = true;

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {//refresh
            try
            {
                HelpClass.StartAwait(grid_main);

                chk_in.IsChecked = true;
                chk_out.IsChecked = true;
                chk_allBranches.IsChecked = true;
                chk_allItems.IsChecked = true;
                chk_allBranches.IsChecked = true;
                dp_startDate.SelectedDate = null;
                dp_endDate.SelectedDate = null;
                searchText = "";
                txt_search.Text = "";

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void chk_externalItemsAllItems_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                cb_items.IsEnabled = true;
                chk_allUnits.IsChecked = false;

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void chk_externalAgentsAllCustomers_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                cb_vendor.IsEnabled = false;
                cb_vendor.SelectedItem = null;

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void chk_externalAgentsAllCustomers_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                cb_vendor.IsEnabled = true;

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion

        #region reports
        public void BuildReport()
        {
            List<ReportParameter> paramarr = new List<ReportParameter>();

            string addpath = "";
            string firstTitle = "external";
            string secondTitle = "";
            string subTitle = "";
            string Title = "";

            bool isArabic = ReportCls.checkLang();
            if (isArabic)
            {
                if (selectedTab == 0)
                {
                    addpath = @"\Reports\StatisticReport\Storage\Ar\ArExternalItem.rdlc";
                    secondTitle = "items";

                }
                else if (selectedTab == 1)
                {
                    addpath = @"\Reports\StatisticReport\Storage\Ar\ArExternalAgent.rdlc";
                    secondTitle = "customers";
                }
            }
            else
            {
                if (selectedTab == 0)
                {
                    addpath = @"\Reports\StatisticReport\Storage\En\EnExternalItem.rdlc";
                    secondTitle = "items";
                }
                else if (selectedTab == 1)
                {
                    addpath = @"\Reports\StatisticReport\Storage\En\EnExternalAgent.rdlc";
                    secondTitle = "customers";
                }
            }
            string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);

            ReportCls.checkLang();
            subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);
            Title = AppSettings.resourcemanagerreport.GetString("trStorageReport") + " / " + subTitle;
            paramarr.Add(new ReportParameter("trTitle", Title));

            //itemTransferInvoiceExternal
            clsReports.itemTransferInvoiceExternal(itemsTransferQuery, rep, reppath, paramarr);

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
                LocalReportExtensions.PrintToPrinter(rep);
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
                //Thread t1 = new Thread(() =>
                //  {
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

        #region charts
        private void fillExternalRowChart()
        {
            MyAxis.Labels = new List<string>();
            List<string> names = new List<string>();
            List<int> pTemp = new List<int>();
            List<int> pbTemp = new List<int>();

            var result = itemsTransferQuery.GroupBy(x => new { x.branchId, x.invoiceId }).Select(x => new ItemTransferInvoice
            {
                invType = x.FirstOrDefault().invType,
                branchId = x.FirstOrDefault().branchId,
                branchName = x.FirstOrDefault().branchName
            }) ;

            invCount = result.GroupBy(x => x.branchId).Select(x => new ItemTransferInvoice
            {
                PCount = x.Where(g => g.invType == "p").Count(),
                PbCount = x.Where(g => g.invType == "pb").Count(),
                branchName = x.FirstOrDefault().branchName
            });

            for (int i = 0; i < invCount.Count(); i++)
            {
                pTemp.Add(invCount.ToList().Skip(i).FirstOrDefault().PCount);
                pbTemp.Add(invCount.ToList().Skip(i).FirstOrDefault().PbCount);
            }
           
            names.AddRange(invCount.Select(nn => nn.branchName));
            for (int i = 0; i < pTemp.Count(); i++)
            {
                MyAxis.Labels.Add(names.ToList().Skip(i).FirstOrDefault());
            }

            SeriesCollection rowChartData = new SeriesCollection();
           

            rowChartData.Add(
          new LineSeries
          {
              Values = pTemp.AsChartValues(),
              Title = AppSettings.resourcemanager.GetString("tr_Purchases")

          }); ;
          rowChartData.Add(
          new LineSeries
          {
              Values = pbTemp.AsChartValues(),
              Title = AppSettings.resourcemanager.GetString("trPurchaseReturnInvoice")
          });
           
            rowChart.Series = rowChartData;
            DataContext = this;
        }

        private void fillExternalColumnChart()
        {
            axcolumn.Labels = new List<string>();
            List<string> names = new List<string>();

            var res = itemsTransferQuery.GroupBy(x => new { x.AgentNameAgent }).Select(x => new 
            {
                agentId = x.FirstOrDefault().agentId,
                agentName = x.FirstOrDefault().AgentNameAgent,
                iCount = x.Where(m => m.invType == "p").Count(),
                rCount = x.Where(m => m.invType == "pb").Count()
            });

            names.AddRange(res.Select(nn => nn.agentName));

            List<string> lable = new List<string>();
            SeriesCollection columnChartData = new SeriesCollection();
            List<int> cP = new List<int>();
            List<int> cPb = new List<int>();

            int xCount = 6;
            if (res.Count() <= 6) xCount = res.Count();

            for (int i = 0; i < xCount; i++)
            {
                cP.Add(res.ToList().Skip(i).FirstOrDefault().iCount);
                cPb.Add(res.ToList().Skip(i).FirstOrDefault().rCount);
                axcolumn.Labels.Add(names.ToList().Skip(i).FirstOrDefault());
            }
            if (res.Count() > 6)
            {
                int _ivoice = 0, _return = 0;
                for (int i = 6; i < res.Count(); i++)
                {
                    _ivoice = _ivoice + res.ToList().Skip(i).FirstOrDefault().iCount;
                    _return = _return + res.ToList().Skip(i).FirstOrDefault().rCount;
                }
                if (!((_ivoice == 0) && (_return == 0)))
                {
                    cP.Add(_ivoice);
                    cPb.Add(_return);
                    axcolumn.Labels.Add(AppSettings.resourcemanager.GetString("trOthers"));
                }
            }
            columnChartData.Add(
            new StackedColumnSeries
            {
                Values = cP.AsChartValues(),
                DataLabels = true,
                Title = AppSettings.resourcemanager.GetString("tr_Invoice")
            }); ;
            columnChartData.Add(
            new StackedColumnSeries
            {
                Values = cPb.AsChartValues(),
                DataLabels = true,
                Title = AppSettings.resourcemanager.GetString("trReturned")
            });

            DataContext = this;
            cartesianChart.Series = columnChartData;
        }

        private void fillExternalPieChart()
        {
            List<string> titles = new List<string>();
            List<string> titles1 = new List<string>();
            List<decimal> x = new List<decimal>();
            titles.Clear();
            titles1.Clear();

            var result = itemsTransferQuery
                .GroupBy(s => new { s.itemId, s.unitId })
                .Select(s => new ItemTransferInvoice
                {
                    itemId = s.FirstOrDefault().itemId,
                    unitId = s.FirstOrDefault().unitId,
                    quantity = s.Sum(g => g.quantity),
                    itemName = s.FirstOrDefault().itemName,
                    unitName = s.FirstOrDefault().unitName,
                }).OrderByDescending(s => s.quantity);
            x = result.Select(m => (decimal)m.quantity).ToList();
            titles = result.Select(m => m.itemName).ToList();
            titles1 = result.Select(m => m.unitName).ToList();
            int count = x.Count();
            SeriesCollection piechartData = new SeriesCollection();
            for (int i = 0; i < count; i++)
            {
                List<decimal> final = new List<decimal>();
                List<string> lable = new List<string>();
                if (i < 5)
                {
                    final.Add(x.Max());
                    lable.Add(titles.Skip(i).FirstOrDefault() + titles1.Skip(i).FirstOrDefault());
                    piechartData.Add(
                      new PieSeries
                      {
                          Values = final.AsChartValues(),
                          Title = lable.FirstOrDefault(),
                          DataLabels = true,
                      }
                  );
                    x.Remove(x.Max());
                }
                else
                {
                    final.Add(x.Sum());
                    piechartData.Add(
                      new PieSeries
                      {
                          Values = final.AsChartValues(),
                          Title = AppSettings.resourcemanager.GetString("trOthers"),
                          DataLabels = true,
                      }
                  ); ;
                    break;
                }

            }
            chart1.Series = piechartData;
        }

        #endregion

       
    }
}
