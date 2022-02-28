using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
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
using System.IO;
using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using System.Threading;
using Restaurant.View.windows;
using System.Resources;
using System.Reflection;
using static Restaurant.Classes.Statistics;

namespace Restaurant.View.reports.salesReports
{
    /// <summary>
    /// Interaction logic for uc_dailySalesReports.xaml
    /// </summary>
    public partial class uc_dailySalesReports : UserControl
    {
        private static uc_dailySalesReports _instance;
        public static uc_dailySalesReports Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new uc_dailySalesReports();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_dailySalesReports()
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

        IEnumerable<ItemTransferInvoice> itemTrasferInvoices;
        Statistics statisticsModel = new Statistics();
        IEnumerable<ItemTransferInvoice> itemTrasferInvoicesQuery;
        IEnumerable<ItemTransferInvoice> itemTrasferInvoicesQueryExcel;
        string searchText = "";
        int selectedTab = 0;
        //prin & pdf
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
                {
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                }
                translate();
                #endregion

                dp_invoiceDate.SelectedDate = DateTime.Now;
                chk_invoice.IsChecked = true;

                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), btn_invoice.Tag.ToString());

                
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
            tt_invoice.Content = AppSettings.resourcemanager.GetString("trInvoices");
            tt_order.Content = AppSettings.resourcemanager.GetString("trOrders");
            tt_quotation.Content = AppSettings.resourcemanager.GetString("trQuotations_");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_invoiceDate, AppSettings.resourcemanager.GetString("trDate"));

            chk_invoice.Content = AppSettings.resourcemanager.GetString("trInvoice");
            chk_return.Content = AppSettings.resourcemanager.GetString("trReturn");
            chk_drafs.Content = AppSettings.resourcemanager.GetString("trDraft");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_branches, AppSettings.resourcemanager.GetString("trBranchHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_pos, AppSettings.resourcemanager.GetString("trPosHint"));

            chk_allBranches.Content = AppSettings.resourcemanager.GetString("trAll");
            chk_allPos.Content = AppSettings.resourcemanager.GetString("trAll");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(txt_search, AppSettings.resourcemanager.GetString("trSearchHint"));
            tt_refresh.Content = AppSettings.resourcemanager.GetString("trRefresh");

            col_No.Header = AppSettings.resourcemanager.GetString("trNum");
            col_type.Header = AppSettings.resourcemanager.GetString("trType");
            col_branch.Header = AppSettings.resourcemanager.GetString("trBranch");
            col_pos.Header = AppSettings.resourcemanager.GetString("trPOS");
            col_discount.Header = AppSettings.resourcemanager.GetString("trDiscount");
            col_tax.Header = AppSettings.resourcemanager.GetString("trTax");
            col_totalNet.Header = AppSettings.resourcemanager.GetString("trTotal");
            col_processType.Header = AppSettings.resourcemanager.GetString("trPaymentMethods");

            tt_report.Content = AppSettings.resourcemanager.GetString("trPdf");
            tt_print.Content = AppSettings.resourcemanager.GetString("trPrint");
            tt_excel.Content = AppSettings.resourcemanager.GetString("trExcel");
            tt_count.Content = AppSettings.resourcemanager.GetString("trCount");

        }

        async Task<IEnumerable<ItemTransferInvoice>> RefreshItemTransferInvoiceList()
        {
            itemTrasferInvoices = await statisticsModel.Getdailyinvoice(MainWindow.branchLogin.branchId, MainWindow.userLogin.userId, HelpClass.DateTodbString(dp_invoiceDate.SelectedDate.Value.Date));
            return itemTrasferInvoices;

        }

        async Task Search()
        {

            if (itemTrasferInvoices is null)
                await RefreshItemTransferInvoiceList();

            searchText = txt_search.Text.ToLower();
            itemTrasferInvoicesQuery = itemTrasferInvoices
                .Where(s =>
            (
            s.invNumber.ToLower().Contains(searchText)
            ||
            s.tax.ToString().ToLower().Contains(searchText)
            )
            &&
            (//invType
                (
                    selectedTab == 0 //invoice
                    ?
                    (chk_invoice.IsChecked == true ? s.invType == "s" : false)
                    ||
                    (chk_return.IsChecked == true ? s.invType == "sb" : false)
                    ||
                    (chk_drafs.IsChecked == true ? s.invType == "sd" || s.invType == "sbd" : false)
                    : false
                )
                ||
                (
                    selectedTab == 1 //order
                    ?
                    (chk_invoice.IsChecked == true ? s.invType == "or" : false)
                    ||
                    (chk_return.IsChecked == true ? s.invType == "ors" : false)
                    ||
                    (chk_drafs.IsChecked == true ? s.invType == "ord" : false)
                    : false
                )
                ||
                (
                    selectedTab == 2 //quotation
                    ?
                    (chk_invoice.IsChecked == true ? s.invType == "q" : false)
                    ||
                    (chk_return.IsChecked == true ? s.invType == "qs" : false)
                    ||
                    (chk_drafs.IsChecked == true ? s.invType == "qd" : false)
                    : false
                )
            )
            &&
            //branchID
            (cb_branches.SelectedIndex != -1 ? s.branchCreatorId == Convert.ToInt32(cb_branches.SelectedValue) : true)
            &&
            //posID
            (cb_pos.SelectedIndex != -1 ? s.posId == Convert.ToInt32(cb_pos.SelectedValue) : true)
            );

            itemTrasferInvoicesQueryExcel = itemTrasferInvoicesQuery.ToList();
            RefreshIemTrasferInvoicesView();
            fillBranches();
            fillColumnChart();
            fillPieChart();
            fillRowChart();
        }

        void RefreshIemTrasferInvoicesView()
        {
            //hide tax column if region tax equals to 0
            if (!AppSettings.invoiceTax_bool.Value)
                col_tax.Visibility = Visibility.Hidden;
            else
                col_tax.Visibility = Visibility.Visible;

            dgInvoice.ItemsSource = itemTrasferInvoicesQuery;
            txt_count.Text = itemTrasferInvoicesQuery.Count().ToString();
        }

        private void fillBranches()
        {
            cb_branches.SelectedValuePath = "branchCreatorId";
            cb_branches.DisplayMemberPath = "branchCreatorName";
            cb_branches.ItemsSource = itemTrasferInvoices.Select(i => new { i.branchCreatorName, i.branchCreatorId }).Distinct();
        }
        private async void Btn_Invoice_Click(object sender, RoutedEventArgs e)
        {//invoice tab
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), (sender as Button).Tag.ToString());
                selectedTab = 0;
                txt_search.Text = "";

                chk_invoice.Content = AppSettings.resourcemanager.GetString("tr_Invoice");
                chk_return.Content = AppSettings.resourcemanager.GetString("trReturn");

                path_order.Fill = Brushes.White;
                path_quotation.Fill = Brushes.White;
                bdrMain.RenderTransform = Animations.borderAnimation(50, bdrMain, true);
                ReportsHelp.paintTabControlBorder(grid_tabControl, bdr_invoice);
                path_invoice.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;

                await Search();
                rowToHide.Height = rowToShow.Height;

                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Btn_order_Click(object sender, RoutedEventArgs e)
        {//order tab
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), (sender as Button).Tag.ToString());

                selectedTab = 1;
                txt_search.Text = "";

                chk_invoice.Content = AppSettings.resourcemanager.GetString("trOrder");
                chk_return.Content = AppSettings.resourcemanager.GetString("trSaved");

                path_invoice.Fill = Brushes.White;
                path_quotation.Fill = Brushes.White;
                bdrMain.RenderTransform = Animations.borderAnimation(50, bdrMain, true);
                ReportsHelp.paintTabControlBorder(grid_tabControl, bdr_order);
                path_order.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;

                await Search();
                rowToHide.Height = new GridLength(0);

                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }


        }
        private async void Btn_quotation_Click(object sender, RoutedEventArgs e)
        {//quotation tab
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), (sender as Button).Tag.ToString());

                selectedTab = 2;
                txt_search.Text = "";

                chk_invoice.Content = AppSettings.resourcemanager.GetString("trQuotation");
                chk_return.Content = AppSettings.resourcemanager.GetString("trSaved");

                path_invoice.Fill = Brushes.White;
                path_order.Fill = Brushes.White;
                bdrMain.RenderTransform = Animations.borderAnimation(50, bdrMain, true);
                ReportsHelp.paintTabControlBorder(grid_tabControl, bdr_quotation);
                path_quotation.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;

                await Search();
                rowToHide.Height = new GridLength(0);

                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void RefreshView_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {//select date
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                await RefreshItemTransferInvoiceList();
                await Search();
                fillBranches();

                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void RefreshViewCheckbox(object sender, RoutedEventArgs e)
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
        private void fillPieChart()
        {
            List<string> titles = new List<string>();
            IEnumerable<int> x = null;

            titles.Clear();
            var temp = itemTrasferInvoicesQuery;
            var titleTemp = temp.GroupBy(m => m.branchCreatorName);
            titles.AddRange(titleTemp.Select(jj => jj.Key));
            var result = temp.GroupBy(s => s.branchCreatorId).Select(s => new { branchCreatorId = s.Key, count = s.Count() });
            x = result.Select(m => m.count);

            SeriesCollection piechartData = new SeriesCollection();
            int xCount = x.Count();
            if (x.Count() <= 6) xCount = x.Count();
            else xCount = 6;
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
                for (int i = 6; i < x.Count(); i++)
                {
                    List<int> final = new List<int>();
                    List<string> lable = new List<string>();

                    final.Add(x.ToList().Skip(i).FirstOrDefault());
                    if (final.Count > 0)
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

        private void fillColumnChart()
        {
            axcolumn.Labels = new List<string>();
            List<string> names = new List<string>();
            IEnumerable<int> x = null;
            IEnumerable<int> y = null;
            IEnumerable<int> z = null;

            string trChk1 = "", trChk2 = "", condition1 = "", condition2 = "", condition3 = "", condition4 = "";

            if (selectedTab == 0)
            { trChk1 = "tr_Sales"; trChk2 = "trReturned"; condition1 = "s"; condition2 = "sb"; condition3 = "sd"; condition4 = "sbd"; }
            else if (selectedTab == 1)
            { trChk1 = "trOrder"; trChk2 = "trSaved"; condition1 = "or"; condition2 = "ors"; condition3 = "ord"; condition4 = "ord"; }
            else if (selectedTab == 2)
            { trChk1 = "trQuotation"; trChk2 = "trSaved"; condition1 = "q"; condition2 = "qs"; condition3 = "qd"; condition4 = "qd"; }

            var temp = itemTrasferInvoicesQuery;
            var result = temp.GroupBy(s => s.branchCreatorId).Select(s => new
            {
                branchCreatorId = s.Key,
                countS = s.Where(m => m.invType == condition1).Count(),
                countSb = s.Where(m => m.invType == condition2).Count(),
                countSd = s.Where(m => (m.invType == condition3) || (m.invType == condition4)).Count()
            });
            x = result.Select(m => m.countS);
            y = result.Select(m => m.countSb);
            z = result.Select(m => m.countSd);
            var tempName = temp.GroupBy(s => s.branchCreatorName).Select(s => new
            {
                uUserName = s.Key
            });
            names.AddRange(tempName.Select(nn => nn.uUserName));

            List<string> lable = new List<string>();
            SeriesCollection columnChartData = new SeriesCollection();
            List<int> cS = new List<int>();
            List<int> cSb = new List<int>();
            List<int> cSd = new List<int>();

            List<string> titles = new List<string>()
            {

                AppSettings.resourcemanager.GetString(trChk1),
                AppSettings.resourcemanager.GetString(trChk2),
                AppSettings.resourcemanager.GetString("trDraft")
            };
            int xCount;
            if (x.Count() <= 6) xCount = x.Count();
            else xCount = 6;
            for (int i = 0; i < xCount; i++)
            {
                cS.Add(x.ToList().Skip(i).FirstOrDefault());
                cSb.Add(y.ToList().Skip(i).FirstOrDefault());
                cSd.Add(z.ToList().Skip(i).FirstOrDefault());
                axcolumn.Labels.Add(names.ToList().Skip(i).FirstOrDefault());
            }
            if (x.Count() > 6)
            {
                int cSSum = 0, cSbSum = 0, cSdSum = 0;
                for (int i = 6; i < x.Count(); i++)
                {
                    cSSum = cSSum + x.ToList().Skip(i).FirstOrDefault();
                    cSbSum = cSbSum + y.ToList().Skip(i).FirstOrDefault();
                    cSdSum = cSdSum + z.ToList().Skip(i).FirstOrDefault();
                }
                if (!((cSSum == 0) && (cSbSum == 0) && (cSdSum == 0)))
                {
                    cS.Add(cSSum);
                    cSb.Add(cSbSum);
                    cSd.Add(cSdSum);
                    axcolumn.Labels.Add("trOthers");
                }
            }

            //3 فوق بعض
            columnChartData.Add(
            new StackedColumnSeries
            {
                Values = cS.AsChartValues(),
                Title = titles[0],
                DataLabels = true,
            });
            columnChartData.Add(
           new StackedColumnSeries
           {
               Values = cSb.AsChartValues(),
               Title = titles[1],
               DataLabels = true,
           });
            columnChartData.Add(
           new StackedColumnSeries
           {
               Values = cSd.AsChartValues(),
               Title = titles[2],
               DataLabels = true,
           });

            DataContext = this;
            cartesianChart.Series = columnChartData;
        }

        private void fillRowChart()
        {
            MyAxis.Labels = new List<string>();
            List<string> names = new List<string>();
            IEnumerable<decimal> pTemp = null;
            IEnumerable<decimal> pbTemp = null;
            IEnumerable<decimal> resultTemp = null;

            var temp = itemTrasferInvoicesQuery;
            var result = temp.GroupBy(s => s.branchCreatorId).Select(s => new
            {
                branchCreatorId = s.Key,
                totalS = s.Where(x => x.invType == "s").Sum(x => x.totalNet),
                totalSb = s.Where(x => x.invType == "sb").Sum(x => x.totalNet)
            }
            );
            var resultTotal = result.Select(x => new { x.branchCreatorId, total = x.totalS - x.totalSb }).ToList();
            pTemp = result.Select(x => (decimal)x.totalS);
            pbTemp = result.Select(x => (decimal)x.totalSb);
            resultTemp = result.Select(x => (decimal)x.totalS);
            var tempName = temp.GroupBy(s => s.branchCreatorName).Select(s => new
            {
                uUserName = s.Key
            });
            names.AddRange(tempName.Select(nn => nn.uUserName));

            SeriesCollection rowChartData = new SeriesCollection();
            List<decimal> purchase = new List<decimal>();
            List<decimal> returns = new List<decimal>();
            List<decimal> sub = new List<decimal>();
            List<string> titles = new List<string>()
            {
                AppSettings.resourcemanager.GetString("trNetSales"),
                AppSettings.resourcemanager.GetString("trTotalReturn"),
                AppSettings.resourcemanager.GetString("trTotalSales")
            };
            for (int i = 0; i < pbTemp.Count(); i++)
            {
                purchase.Add(pTemp.ToList().Skip(i).FirstOrDefault());
                returns.Add(pbTemp.ToList().Skip(i).FirstOrDefault());
                sub.Add(resultTemp.ToList().Skip(i).FirstOrDefault());
                MyAxis.Labels.Add(names.ToList().Skip(i).FirstOrDefault());
            }

            rowChartData.Add(
          new LineSeries
          {
              Values = purchase.AsChartValues(),
              Title = titles[0]
          }); ;
            rowChartData.Add(
         new LineSeries
         {
             Values = returns.AsChartValues(),
             Title = titles[1]
         });
            rowChartData.Add(
        new LineSeries
        {
            Values = sub.AsChartValues(),
            Title = titles[2]

        });
            DataContext = this;
            rowChart.Series = rowChartData;
        }

        private async void cb_branches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                fillPos(Convert.ToInt32(cb_branches.SelectedValue));

                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void fillPos(int bID)
        {
            cb_pos.SelectedValuePath = "posId";
            cb_pos.DisplayMemberPath = "posName";
            cb_pos.ItemsSource = itemTrasferInvoicesQuery.Where(t => t.branchCreatorId == bID)
                                                         .Select(i => new
                                                         {
                                                             i.posName,
                                                             i.posId
                                                         }).Distinct();
        }

        private void BuildReport()
        {
            List<ReportParameter> paramarr = new List<ReportParameter>();

            string addpath;
            string firstTitle = "dailySalesStatistic";
            string secondTitle = "";
            string subTitle = "";
            string Title = "";

            bool isArabic = ReportCls.checkLang();
            if (isArabic)
            {
                addpath = @"\Reports\StatisticReport\Sale\Daily\Ar\dailySale.rdlc";



                if (selectedTab == 0)
                {
                    secondTitle = "invoice";
                }
                else if (selectedTab == 1)
                {
                    secondTitle = "order";
                }
                else
                {
                    //  selectedTab == 2
                    secondTitle = "quotation";

                }
                subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);
            }
            else
            {
                addpath = @"\Reports\StatisticReport\Sale\Daily\En\dailySale.rdlc";
                if (selectedTab == 0)
                {
                    secondTitle = "invoice";
                }
                else if (selectedTab == 1)
                {
                    secondTitle = "order";
                }
                else
                {
                    //  selectedTab == 2
                    secondTitle = "quotation";

                }
                subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);
            }

            string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);

            ReportCls.checkLang();

            Title = AppSettings.resourcemanagerreport.GetString("trSalesReport") + " / " + subTitle;
            paramarr.Add(new ReportParameter("trTitle", Title));

            clsReports.SaledailyReport(itemTrasferInvoicesQuery, rep, reppath, paramarr);
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

        private async void Chk_allBranches_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                cb_branches.SelectedIndex = -1;
                cb_branches.IsEnabled = false;

                
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

                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Chk_allPos_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                cb_pos.SelectedIndex = -1;
                cb_pos.IsEnabled = false;


                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Chk_allPos_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                cb_pos.IsEnabled = true;

                
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

                await RefreshItemTransferInvoiceList();
                await Search();

                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Cb_pos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                await RefreshItemTransferInvoiceList();
                await Search();

                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }


        Invoice invoice;
        private async void DgInvoice_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                
                    HelpClass.StartAwait(grid_main);
                invoice = new Invoice();
                if (dgInvoice.SelectedIndex != -1)
                {

                    ItemTransferInvoice item = dgInvoice.SelectedItem as ItemTransferInvoice;
                    if (item.invoiceId > 0)
                    {
                        /*
                        invoice = await invoice.GetByInvoiceId(item.invoiceId);
                        MainWindow.mainWindow.BTN_sales_Click(MainWindow.mainWindow.btn_sales, null);
                        uc_sales.Instance.UserControl_Loaded(null, null);
                        uc_sales.Instance.Btn_receiptInvoice_Click(uc_sales.Instance.btn_reciptInvoice, null);
                        uc_receiptInvoice.Instance.UserControl_Loaded(null, null);
                        uc_receiptInvoice._InvoiceType = invoice.invType;
                        uc_receiptInvoice.Instance.invoice = invoice;
                        uc_receiptInvoice.isFromReport = true;
                        if (item.archived == 0)
                            uc_receiptInvoice.archived = false;
                        else
                            uc_receiptInvoice.archived = true;
                        await uc_receiptInvoice.Instance.fillInvoiceInputs(invoice);
                        */
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
