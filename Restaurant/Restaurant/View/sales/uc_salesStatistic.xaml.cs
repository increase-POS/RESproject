﻿using LiveCharts;
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
using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using System.IO;
using netoaster;
using System.Threading;
using Restaurant.View.sales;
using Restaurant.View.windows;
using System.Resources;
using System.Reflection;

namespace Restaurant.View.sales
{
    /// <summary>
    /// Interaction logic for uc_salesStatistic.xaml
    /// </summary>
    public partial class uc_salesStatistic : UserControl
    {
        private static uc_salesStatistic _instance;
        public static uc_salesStatistic Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new uc_salesStatistic();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_salesStatistic()
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
        private Statistics statisticModel = new Statistics();

        IEnumerable<ItemTransferInvoice> itemTrasferInvoices;
        IEnumerable<ItemTransferInvoice> itemTrasferInvoicesQuery;
        string searchText = "";
        // report
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

                fillServices();

                chk_allServices.IsChecked = true;
                dp_invoiceDate.SelectedDate = DateTime.Now;

                Btn_Invoice_Click(btn_invoice, null);

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        #region methods
        private void fillServices()
        {
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
            tt_invoice.Content = AppSettings.resourcemanager.GetString("trInvoices");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_invoiceDate, AppSettings.resourcemanager.GetString("trDate"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_sevices, AppSettings.resourcemanager.GetString("typesOfService") + "...");

            chk_allServices.Content = AppSettings.resourcemanager.GetString("trAll");

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
            itemTrasferInvoices = await statisticModel.GetUserdailyinvoice((int)MainWindow.branchLogin.branchId, (int)MainWindow.userLogin.userId);
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
            s.branchCreatorName.ToString().ToLower().Contains(searchText)
            ||
            s.posName.ToString().ToLower().Contains(searchText)
            ||
            s.invType.ToString().ToLower().Contains(searchText)
            )
            &&
            //service
            (cb_sevices.SelectedIndex != -1 ? s.invType == cb_sevices.SelectedValue.ToString() : true)
            &&
            //date
            (dp_invoiceDate.SelectedDate != null ? s.updateDate.Value.Date.ToShortDateString() == dp_invoiceDate.SelectedDate.Value.Date.ToShortDateString() : true)
            );

            RefreshIemTrasferInvoicesView();

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

            fillColumnChart();
            fillPieChart();
            fillRowChart();
        }

        #endregion
        
        private async void Btn_Invoice_Click(object sender, RoutedEventArgs e)
        {//invoice tab
            //try
            //{
            //    HelpClass.StartAwait(grid_main);

                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), (sender as Button).Tag.ToString());
                txt_search.Text = "";

                ReportsHelp.paintTabControlBorder(grid_tabControl, bdr_invoice);
                path_invoice.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;

                await Search();
                rowToHide.Height = rowToShow.Height;

            //    HelpClass.EndAwait(grid_main);
            //}
            //catch (Exception ex)
            //{
            //    HelpClass.EndAwait(grid_main);
            //    HelpClass.ExceptionMessage(ex, this);
            //}
        }

        #region events
        private async void Dp_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {//select date
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

        private void Txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {//search
            callSearch(sender);
        }

        private void Chk_allServices_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                cb_sevices.SelectedIndex = -1;
                cb_sevices.IsEnabled = false;

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
        #endregion

        #region charts
        private void fillPieChart()
        {
            List<string> titles = new List<string>();
            List<int> resultList = new List<int>();
            titles.Clear();

            var result = itemTrasferInvoicesQuery.Where(s => s.invType == "s" || s.invType == "ts" || s.invType == "ss")
                .GroupBy(s => new { s.invType })
                .Select(s => new
                {
                   count = s.Count(),
                    type = s.FirstOrDefault().invType,
                });
            resultList = result.Select(m => m.count).ToList();
            titles = result.Select(m => m.type).ToList();
            for (int t = 0; t < titles.Count; t++)
            {
                string s = "";
                switch (titles[t])
                {
                    case "s": s = AppSettings.resourcemanager.GetString("trDiningHallType"); break;
                    case "ts": s = AppSettings.resourcemanager.GetString("trTakeAway"); break;
                    case "ss": s = AppSettings.resourcemanager.GetString("trSelfService"); break;
                }
                titles[t] = s;
            }

            SeriesCollection piechartData = new SeriesCollection();
            for (int i = 0; i < resultList.Count(); i++)
            {
                List<int> final = new List<int>();
                List<string> lable = new List<string>();

                final.Add(resultList.Skip(i).FirstOrDefault());
                lable = titles;
                piechartData.Add(
                  new PieSeries
                  {
                      Values = final.AsChartValues(),
                      Title = lable.Skip(i).FirstOrDefault(),
                      DataLabels = true,
                  }
              );

            }
            chart1.Series = piechartData;
        }

        private void fillColumnChart()
        {
           // axcolumn.Labels = new List<string>();
           // List<string> names = new List<string>();
           // IEnumerable<int> x = null;//invoice
           // IEnumerable<int> y = null;//draft invoice
           // IEnumerable<int> z = null;

           // var temp = itemTrasferInvoicesQuery;
           // var result = temp.GroupBy(s => s.createUserId).Select(s => new
           // {
           //     updateUserId = s.Key,
           //     countP = s.Where(m => m.invType == "s").Count(),
           //     countPb = s.Where(m => m.invType == "ts").Count(),
           //     countD = s.Where(m => m.invType == "ss" ).Count()

           // });

           // x = result.Select(m => m.countP);
           // y = result.Select(m => m.countPb);
           // z = result.Select(m => m.countD);
           // var tempName = temp.GroupBy(s => s.uUserAccName).Select(s => new
           // {
           //     uUserName = s.Key
           // });
           // names.AddRange(tempName.Select(nn => nn.uUserName));

           // List<string> lable = new List<string>();
           // SeriesCollection columnChartData = new SeriesCollection();
           // List<int> cP = new List<int>();
           // List<int> cPb = new List<int>();
           // List<int> cD = new List<int>();
           // List<string> titles = new List<string>()
           // {
           //     AppSettings.resourcemanager.GetString("trDiningHallType"),
           //     AppSettings.resourcemanager.GetString("trTakeAway"),
           //     AppSettings.resourcemanager.GetString("trSelfService")
           // };
           // int xCount = 0;
           // if (x.Count() <= 6) xCount = x.Count();
           // else xCount = 6;
           // for (int i = 0; i < xCount; i++)
           // {
           //     cP.Add(x.ToList().Skip(i).FirstOrDefault());
           //     cPb.Add(y.ToList().Skip(i).FirstOrDefault());
           //     cD.Add(z.ToList().Skip(i).FirstOrDefault());
           //     axcolumn.Labels.Add(names.ToList().Skip(i).FirstOrDefault());
           // }
           // if (x.Count() > 6)
           // {
           //     int cPSum = 0, cPbSum = 0, cDSum = 0;
           //     for (int i = 6; i < x.Count(); i++)
           //     {
           //         cPSum = cPSum + x.ToList().Skip(i).FirstOrDefault();
           //         cPbSum = cPbSum + y.ToList().Skip(i).FirstOrDefault();
           //         cDSum = cDSum + z.ToList().Skip(i).FirstOrDefault();
           //     }
           //     if (!((cPSum == 0) && (cPbSum == 0) && (cDSum == 0)))
           //     {
           //         cP.Add(cPSum);
           //         cPb.Add(cPbSum);
           //         cD.Add(cDSum);
           //         axcolumn.Labels.Add(AppSettings.resourcemanager.GetString("trOthers"));
           //     }
           // }
           // //3 فوق بعض
           // columnChartData.Add(
           // new StackedColumnSeries
           // {
           //     Values = cP.AsChartValues(),
           //     Title = titles[0],
           //     DataLabels = true,
           // });
           // columnChartData.Add(
           //new StackedColumnSeries
           //{
           //    Values = cPb.AsChartValues(),
           //    Title = titles[1],
           //    DataLabels = true,
           //});
           // columnChartData.Add(
           //new StackedColumnSeries
           //{
           //    Values = cD.AsChartValues(),
           //    Title = titles[2],
           //    DataLabels = true,
           //});

           // DataContext = this;
           // cartesianChart.Series = columnChartData;
        }

        private void fillRowChart()
        {
        //    MyAxis.Labels = new List<string>();
        //    List<string> names = new List<string>();
        //    IEnumerable<decimal> pTemp = null;
        //    IEnumerable<decimal> pbTemp = null;
        //    IEnumerable<decimal> resultTemp = null;

        //    var temp = itemTrasferInvoicesQuery;
        //    var result = temp.GroupBy(s => s.createUserId).Select(s => new
        //    {
        //        updateUserId = s.Key,
        //        totalP = s.Where(x => x.invType == "s").Sum(x => x.totalNet),
        //        totalPb = s.Where(x => x.invType == "ts").Sum(x => x.totalNet),
        //        resultTemp = s.Where(x => x.invType == "ss").Sum(x => x.totalNet)
        //    }
        // );

        //    //var resultTotal = result.Select(x => new { x.updateUserId, total = x.totalP - x.totalPb }).ToList();
        //    pTemp = result.Select(x => (decimal)x.totalP);
        //    pbTemp = result.Select(x => (decimal)x.totalPb);
        //    //resultTemp = result.Select(x => (decimal)x.totalP - (decimal)x.totalPb);
        //    resultTemp = result.Select(x => (decimal)x.resultTemp);
        //    var tempName = temp.GroupBy(s => s.uUserAccName).Select(s => new
        //    {
        //        uUserName = s.Key
        //    });
        //    names.AddRange(tempName.Select(nn => nn.uUserName));

        //    SeriesCollection rowChartData = new SeriesCollection();
        //    List<decimal> purchase = new List<decimal>();
        //    List<decimal> returns = new List<decimal>();
        //    List<decimal> sub = new List<decimal>();
        //    List<string> titles = new List<string>()
        //    {
        //         AppSettings.resourcemanager.GetString("trDiningHallType"),
        //        AppSettings.resourcemanager.GetString("trTakeAway"),
        //        AppSettings.resourcemanager.GetString("trSelfService")
        //    };
        //    for (int i = 0; i < pTemp.Count(); i++)
        //    {
        //        purchase.Add(pTemp.ToList().Skip(i).FirstOrDefault());
        //        returns.Add(pbTemp.ToList().Skip(i).FirstOrDefault());
        //        sub.Add(resultTemp.ToList().Skip(i).FirstOrDefault());
        //        MyAxis.Labels.Add(names.ToList().Skip(i).FirstOrDefault());
        //    }

        //    rowChartData.Add(
        //  new LineSeries
        //  {
        //      Values = purchase.AsChartValues(),
        //      Title = titles[0]
        //  });
        //    rowChartData.Add(
        // new LineSeries
        // {
        //     Values = returns.AsChartValues(),
        //     Title = titles[1]
        // });
        //    rowChartData.Add(
        //new LineSeries
        //{
        //    Values = sub.AsChartValues(),
        //    Title = titles[2]

        //});
        //    DataContext = this;
        //    rowChart.Series = rowChartData;
        }

        #endregion

        #region reports
        public void BuildReport()
        {
            List<ReportParameter> paramarr = new List<ReportParameter>();

            string addpath;
            bool isArabic = ReportCls.checkLang();
            if (isArabic)
            {
                addpath = @"\Reports\Sale\Ar\dailySale.rdlc";
            }
            else
                addpath = @"\Reports\Sale\En\dailySale.rdlc";
            string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);

            ReportCls.checkLang();

            clsReports.SaledailyReport(itemTrasferInvoicesQuery, rep, reppath, paramarr);
            clsReports.setReportLanguage(paramarr);
            clsReports.Header(paramarr);

            rep.SetParameters(paramarr);

            rep.Refresh();
        }
        public void pdfdaily()
        {

            BuildReport();

            this.Dispatcher.Invoke(() =>
            {
                saveFileDialog.Filter = "PDF|*.pdf;";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filepath = saveFileDialog.FileName;
                    LocalReportExtensions.ExportToPDF(rep, filepath);
                }
            });
        }

        private void Btn_pdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                //if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") || HelpClass.isAdminPermision())
                //{
                /////////////////////////////////////
                Thread t1 = new Thread(() =>
                {
                    pdfdaily();
                });
                t1.Start();
                //////////////////////////////////////
                //}
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
        public void printDaily()
        {
            BuildReport();

            this.Dispatcher.Invoke(() =>
            {
                LocalReportExtensions.PrintToPrinterbyNameAndCopy(rep, AppSettings.rep_printer_name, short.Parse(AppSettings.rep_print_count));
            });
        }
        private void Btn_print_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                //if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") || HelpClass.isAdminPermision())
                //{
                /////////////////////////////////////
                Thread t1 = new Thread(() =>
                {
                    printDaily();
                });
                t1.Start();
                //////////////////////////////////////

                //}
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

        public void ExcelDaily()
        {

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
        }
        private void Btn_exportToExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                //if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") || HelpClass.isAdminPermision())
                //{
                //Thread t1 = new Thread(() =>
                //{
                ExcelDaily();

                //});
                //t1.Start();
                //}
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

        private void Btn_preview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                //if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") || HelpClass.isAdminPermision())
                //{
                #region
                Window.GetWindow(this).Opacity = 0.2;
                /////////////////////
                string pdfpath = "";
                pdfpath = @"\Thumb\report\temp.pdf";
                pdfpath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, pdfpath);
                BuildReport();
                LocalReportExtensions.ExportToPDF(rep, pdfpath);
                ///////////////////
                wd_previewPdf w = new wd_previewPdf();
                w.pdfPath = pdfpath;
                if (!string.IsNullOrEmpty(w.pdfPath))
                {
                    w.ShowDialog();
                    w.wb_pdfWebViewer.Dispose();
                }
                Window.GetWindow(this).Opacity = 1;
                #endregion
                //}
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

        #endregion


      
    }
}
