﻿using LiveCharts;
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
    /// Interaction logic for uc_spendingRequestsKitchenReport.xaml
    /// </summary>
    public partial class uc_spendingRequestsKitchenReport : UserControl
    {
        IEnumerable<ItemTransferInvoice> pspendingRequests;
        Statistics statisticsModel = new Statistics();
        IEnumerable<ItemTransferInvoice> pspendingRequestsQuery;

        string searchText = "";
        int selectedTab = 0;
        //report
        ReportCls reportclass = new ReportCls();
        LocalReport rep = new LocalReport();
        SaveFileDialog saveFileDialog = new SaveFileDialog();

        private static uc_spendingRequestsKitchenReport _instance;
        public static uc_spendingRequestsKitchenReport Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new uc_spendingRequestsKitchenReport();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_spendingRequestsKitchenReport()
        {
            try
            {
                InitializeComponent();
            }
            catch
            { }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {//load
            try
            {
                #region translate
                if (AppSettings.lang.Equals("en"))
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                else
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                translate();
                #endregion

                Btn_invoice_Click(btn_invoice, null);

                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), btn_invoice.Tag.ToString());

            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_invoice_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_item_Click(object sender, RoutedEventArgs e)
        {

        }
        #region methods
        private void translate()
        {
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_startDate, AppSettings.resourcemanager.GetString("trStartDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_endDate, AppSettings.resourcemanager.GetString("trEndDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(txt_search, AppSettings.resourcemanager.GetString("trSearchHint"));

            chk_allBranches.Content = AppSettings.resourcemanager.GetString("trAll");

            tt_invoice.Content = AppSettings.resourcemanager.GetString("trInvoices");
            tt_item.Content = AppSettings.resourcemanager.GetString("trItems");

            col_invNum.Header = AppSettings.resourcemanager.GetString("trNum");
            col_invType.Header = AppSettings.resourcemanager.GetString("trType");
            col_invDate.Header = AppSettings.resourcemanager.GetString("trDate");
            col_invTotal.Header = AppSettings.resourcemanager.GetString("trTotal");
            col_itemName.Header = AppSettings.resourcemanager.GetString("trItem");
            col_unitName.Header = AppSettings.resourcemanager.GetString("trUnit");
            col_quantity.Header = AppSettings.resourcemanager.GetString("trQTR");
            col_branch.Header = AppSettings.resourcemanager.GetString("trBranch");
            col_pos.Header = AppSettings.resourcemanager.GetString("trPOS");
            col_invoiceProfit.Header = AppSettings.resourcemanager.GetString("trProfits");
            col_itemProfit.Header = AppSettings.resourcemanager.GetString("trProfits");

            tt_refresh.Content = AppSettings.resourcemanager.GetString("trRefresh");
            tt_report.Content = AppSettings.resourcemanager.GetString("trPdf");
            tt_print.Content = AppSettings.resourcemanager.GetString("trPrint");
            tt_excel.Content = AppSettings.resourcemanager.GetString("trExcel");
            tt_count.Content = AppSettings.resourcemanager.GetString("trCount");

            txt_total.Text = AppSettings.resourcemanager.GetString("trTotal");
        }

        async Task<IEnumerable<ItemTransferInvoice>> RefreshItemUnitInvoiceProfit()
        {
            //if (selectedTab == 0)
            //    profits = await statisticsModel.GetInvoiceProfit(MainWindow.branchLogin.branchId, MainWindow.userLogin.userId);
            //else if (selectedTab == 1)
            //    profits = await statisticsModel.GetItemProfit(MainWindow.branchLogin.branchId, MainWindow.userLogin.userId);

            //return profits;
            return pspendingRequests;
        }

        IEnumerable<ItemUnitInvoiceProfit> profitsTemp = null;

        async Task Search()
        {
            await RefreshItemUnitInvoiceProfit();

            searchText = txt_search.Text.ToLower();

            //profitsTemp = profits.Where(p =>
            //(dp_startDate.SelectedDate != null ? p.updateDate >= dp_startDate.SelectedDate : true)
            //&&
            ////end date
            //(dp_endDate.SelectedDate != null ? p.updateDate <= dp_endDate.SelectedDate : true)
            //);

            //if (selectedTab == 0) await SearchInvoice();
            //else if (selectedTab == 1) await SearchItem();

            RefreshProfitsView();
            fillCombo1();
            fillColumnChart();
            fillPieChart();
            fillRowChart();
        }

        async Task SearchInvoice()
        {
            //profitsQuery = profitsTemp
            //.Where(s =>
            //(
            //s.invNumber.ToLower().Contains(searchText)
            //||
            //s.totalNet.ToString().ToLower().Contains(searchText)
            //||
            //s.invType.ToLower().Contains(searchText)
            //||
            //s.ITitemName.ToLower().Contains(searchText)
            //||
            //s.ITunitName.ToLower().Contains(searchText)
            //||
            //s.ITquantity.ToString().ToLower().Contains(searchText)
            //||
            //s.ITprice.ToString().ToLower().Contains(searchText)
            //)
            //&&
            ////branchID/itemID
            //(
            //    cb_branches.SelectedIndex != -1 ? s.branchCreatorId == Convert.ToInt32(cb_branches.SelectedValue) : true
            //)
            //&&
            ////posID/unitID
            //(
            //    cb_pos.SelectedIndex != -1 ? s.posId == Convert.ToInt32(cb_pos.SelectedValue) : true
            //));

            //profitsQueryExcel = profitsQuery.ToList();
        }

        async Task SearchItem()
        {
            //var quantities = profitsTemp.GroupBy(s => s.ITitemUnitId).Select(inv => new {
            //    ITquantity = inv.Sum(p => p.ITquantity.Value),
            //    itemunitProfit = inv.Sum(p => p.itemunitProfit)
            //}).ToList();

            //profitsTemp = profitsTemp.GroupBy(s => s.ITitemUnitId).SelectMany(inv => inv.Take(1)).ToList();

            //profitsQuery = profitsTemp
            //.Where(s =>
            //(
            //s.invNumber.ToLower().Contains(searchText)
            //||
            //s.totalNet.ToString().ToLower().Contains(searchText)
            //||
            //s.invType.ToLower().Contains(searchText)
            //||
            //s.ITitemName.ToLower().Contains(searchText)
            //||
            //s.ITunitName.ToLower().Contains(searchText)
            //||
            //s.ITquantity.ToString().ToLower().Contains(searchText)
            //||
            //s.ITprice.ToString().ToLower().Contains(searchText)
            //)
            //&&
            ////branchID/itemID
            //(
            //    cb_branches.SelectedIndex != -1 ? s.ITitemId == Convert.ToInt32(cb_branches.SelectedValue) : true
            //)
            //&&
            ////posID/unitID
            //(
            //    cb_pos.SelectedIndex != -1 ? s.ITunitId == Convert.ToInt32(cb_pos.SelectedValue) : true)
            //);

            //int i = 0;
            //foreach (var x in profitsQuery)
            //{
            //    x.ITquantity = quantities[i].ITquantity;
            //    x.itemunitProfit = quantities[i].itemunitProfit;
            //    i++;
            //}
            //profitsQueryExcel = profitsQuery.ToList();
        }



        void RefreshProfitsView()
        {
            //dgFund.ItemsSource = profitsQuery;
            //txt_count.Text = profitsQuery.Count().ToString();

            //decimal total = 0;
            //if (selectedTab == 0)
            //    total = profitsQuery.Select(b => b.invoiceProfit).Sum();
            //else
            //    total = profitsQuery.Select(b => b.itemunitProfit).Sum();

            //tb_total.Text = HelpClass.DecTostring(total);
        }

        private void fillCombo1()
        {
            //if (selectedTab == 0)
            //{
            //    MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_branches, AppSettings.resourcemanager.GetString("trBranchHint"));
            //    cb_branches.SelectedValuePath = "branchCreatorId";
            //    cb_branches.DisplayMemberPath = "branchCreatorName";
            //    cb_branches.ItemsSource = profits.GroupBy(g => g.branchCreatorId).Select(i => new { i.FirstOrDefault().branchCreatorName, i.FirstOrDefault().branchCreatorId });
            //}
            //else if (selectedTab == 1)
            //{
            //    MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_branches, AppSettings.resourcemanager.GetString("trItemHint"));
            //    cb_branches.SelectedValuePath = "ITitemId";
            //    cb_branches.DisplayMemberPath = "ITitemName";
            //    cb_branches.ItemsSource = profits.GroupBy(g => g.ITitemId).Select(i => new { i.FirstOrDefault().ITitemId, i.FirstOrDefault().ITitemName });
            //}
        }

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

        private void Txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region charts
        private void fillColumnChart()
        {
            //axcolumn.Labels = new List<string>();
            //List<string> names = new List<string>();
            //List<decimal> profit = new List<decimal>();

            //var temp = profitsQuery;

            //int count = 0;
            ////invoice
            //if (selectedTab == 0)
            //{
            //    var tempName = temp.GroupBy(s => s.posId).Select(s => new
            //    {
            //        posName = s.FirstOrDefault().posName + "/" + s.FirstOrDefault().branchCreatorName
            //    });
            //    count = tempName.Count();
            //    names.AddRange(tempName.Select(nn => nn.posName));

            //    var tempProfit = temp.GroupBy(s => s.posId).Select(s => new
            //    {
            //        profit = s.Sum(p => decimal.Parse(HelpClass.DecTostring(p.invoiceProfit)))
            //    });

            //    profit.AddRange(tempProfit.Select(nn => nn.profit));
            //}
            ////item
            //else if (selectedTab == 1)
            //{
            //    var tempName = temp.GroupBy(s => s.ITitemUnitId).Select(s => new
            //    {
            //        name = s.FirstOrDefault().ITitemName + "/" + s.FirstOrDefault().ITunitName
            //    });
            //    count = tempName.Count();
            //    names.AddRange(tempName.Select(nn => nn.name));

            //    var tempProfit = temp.GroupBy(s => s.ITitemId).Select(s => new
            //    {
            //        profit = s.Sum(p => decimal.Parse(HelpClass.DecTostring(p.itemunitProfit)))
            //    });

            //    profit.AddRange(tempProfit.Select(nn => nn.profit));
            //}
            //List<string> lable = new List<string>();
            //SeriesCollection columnChartData = new SeriesCollection();

            //List<decimal> cWon = new List<decimal>();
            //List<decimal> cLoss = new List<decimal>();

            //List<string> titles = new List<string>()
            //{
            //   AppSettings.resourcemanager.GetString("trProfit") ,
            //   AppSettings.resourcemanager.GetString("trLoss")
            //};
            //int x = 6;
            //if (count <= 6) x = count;
            //for (int i = 0; i < x; i++)
            //{
            //    if (profit.ToList().Skip(i).FirstOrDefault() > 0)
            //    {
            //        cWon.Add(profit.ToList().Skip(i).FirstOrDefault());
            //        cLoss.Add(0);
            //    }
            //    else
            //    {
            //        cWon.Add(0);
            //        cLoss.Add(-1 * profit.ToList().Skip(i).FirstOrDefault());
            //    }
            //    axcolumn.Labels.Add(names.ToList().Skip(i).FirstOrDefault());
            //}

            //if (count > 6)
            //{
            //    decimal profitSum = 0;
            //    for (int i = 6; i < count; i++)
            //    {
            //        profitSum = profitSum + profit.ToList().Skip(i).FirstOrDefault();
            //    }
            //    if (!((profitSum == 0)))
            //    {
            //        if (profitSum > 0)
            //        {
            //            cWon.Add(profitSum);
            //            cLoss.Add(0);
            //        }
            //        else
            //        {
            //            cWon.Add(0);
            //            cLoss.Add(-1 * profitSum);
            //        }

            //        axcolumn.Labels.Add(AppSettings.resourcemanager.GetString("trOthers"));
            //    }
            //}
            //columnChartData.Add(
            //    new StackedColumnSeries
            //    {
            //        Values = cWon.AsChartValues(),
            //        Title = titles[0],
            //        DataLabels = true,
            //    });
            //columnChartData.Add(
            //   new StackedColumnSeries
            //   {
            //       Values = cLoss.AsChartValues(),
            //       Title = titles[1],
            //       DataLabels = true,
            //   });
            //DataContext = this;
            //cartesianChart.Series = columnChartData;
        }

        private void fillPieChart()
        {
            //List<string> titles = new List<string>();
            //List<string> finalTitles = new List<string>();
            //IEnumerable<decimal> x = null;

            //var temp = profitsQuery;
            //int count = 0;
            //if (selectedTab == 0)
            //{
            //    var titleTemp = temp.GroupBy(m => m.branchCreatorName);
            //    titles.AddRange(titleTemp.Select(jj => jj.Key));

            //    var result = temp.GroupBy(s => s.branchCreatorId).Select(s => new
            //    {
            //        branchCreatorId = s.Key,
            //        profit = s.Sum(p => p.invoiceProfit)
            //    });
            //    x = result.Select(m => decimal.Parse(HelpClass.DecTostring(m.profit)));
            //    count = x.Count();
            //}
            //else if (selectedTab == 1)
            //{
            //    var titleTemp = temp.GroupBy(m => m.ITitemId).Select(d => new
            //    {
            //        ITitemId = d.Key,
            //        name = d.FirstOrDefault().ITitemName
            //    }
            //    );
            //    titles.AddRange(titleTemp.Select(jj => jj.name));

            //    var result = temp.GroupBy(s => s.ITitemId).Select(s => new
            //    {
            //        ITitemUnitId = s.Key,
            //        profit = s.Sum(p => p.itemunitProfit)
            //    });

            //    x = result.Select(m => decimal.Parse(HelpClass.DecTostring(m.profit)));
            //    count = x.Count();
            //}
            //SeriesCollection piechartData = new SeriesCollection();

            //int xCount = 6;
            //if (count < 6) xCount = count;

            //for (int i = 0; i < xCount; i++)
            //{
            //    List<decimal> final = new List<decimal>();

            //    if (x.ToList().Skip(i).FirstOrDefault() > 0)
            //    {
            //        final.Add(x.ToList().Skip(i).FirstOrDefault());
            //        finalTitles.Add(titles[i]);

            //        piechartData.Add(
            //       new PieSeries
            //       {
            //           Values = final.AsChartValues(),
            //           Title = finalTitles.Skip(i).FirstOrDefault(),
            //           DataLabels = true,
            //       }
            //    );
            //    }
            //}

            //if (count > 6)
            //{
            //    decimal finalSum = 0;

            //    for (int i = 6; i < count; i++)
            //    {
            //        finalSum = finalSum + x.ToList().Skip(i).FirstOrDefault();
            //    }

            //    List<decimal> final = new List<decimal>();
            //    List<string> lable = new List<string>();

            //    if (finalSum > 0)
            //        final.Add(finalSum);

            //    piechartData.Add(
            //    new PieSeries
            //    {
            //        Values = final.AsChartValues(),
            //        Title = AppSettings.resourcemanager.GetString("trOthers"),
            //        DataLabels = true,
            //    }
            //    );
            //}

            //chart1.Series = piechartData;
        }

        private void fillRowChart()
        {
            //MyAxis.Labels = new List<string>();
            //List<string> names = new List<string>();
            //List<int> ids = new List<int>();
            //List<int> otherIds = new List<int>();

            //List<ItemUnitInvoiceProfit> resultList = new List<ItemUnitInvoiceProfit>();
            //SeriesCollection rowChartData = new SeriesCollection();

            //if (selectedTab == 0)
            //{
            //    var tempName = profitsQuery.GroupBy(s => new { s.branchCreatorId }).Select(s => new
            //    {
            //        id = s.Key,
            //        name = s.FirstOrDefault().branchCreatorName
            //    });
            //    names.AddRange(tempName.Select(nn => nn.name.ToString()));
            //    ids.AddRange(tempName.Select(mm => mm.id.branchCreatorId.Value));
            //}
            //else if (selectedTab == 1)
            //{
            //    var tempName = profitsQuery.GroupBy(s => new { s.ITitemId }).Select(s => new
            //    {
            //        id = s.Key,
            //        name = s.FirstOrDefault().ITitemName
            //    });
            //    names.AddRange(tempName.Select(nn => nn.name.ToString()));
            //    ids.AddRange(tempName.Select(mm => mm.id.ITitemId.Value));
            //}

            //int x = 6;
            //if (names.Count() < 6) x = names.Count();
            //for (int i = 0; i < x; i++)
            //{
            //    drawPoints(names[i], ids[i], rowChartData, 'n', otherIds);
            //}
            ////others
            //if (names.Count() > 6)
            //{
            //    for (int i = names.Count - x; i < names.Count; i++)
            //        otherIds.Add(ids[i]);
            //    drawPoints(AppSettings.resourcemanager.GetString("trOthers"), 0, rowChartData, 'o', otherIds);
            //}

            //DataContext = this;
            //rowChart.Series = rowChartData;
        }

        private void drawPoints(string name, int id, SeriesCollection rowChartData, char ch, List<int> otherIds)
        {
            //int endYear = DateTime.Now.Year;
            //int startYear = endYear - 1;
            //int startMonth = DateTime.Now.Month;
            //int endMonth = startMonth;
            //if (dp_startDate.SelectedDate != null && dp_endDate.SelectedDate != null)
            //{
            //    startYear = dp_startDate.SelectedDate.Value.Year;
            //    endYear = dp_endDate.SelectedDate.Value.Year;
            //    startMonth = dp_startDate.SelectedDate.Value.Month;
            //    endMonth = dp_endDate.SelectedDate.Value.Month;
            //}
            //SeriesCollection columnChartData = new SeriesCollection();
            //List<decimal> profitLst = new List<decimal>();

            //if (endYear - startYear <= 1)
            //{
            //    for (int year = startYear; year <= endYear; year++)
            //    {
            //        for (int month = startMonth; month <= 12; month++)
            //        {
            //            var firstOfThisMonth = new DateTime(year, month, 1);
            //            var firstOfNextMonth = firstOfThisMonth.AddMonths(1);

            //            if (selectedTab == 0)
            //            {
            //                if (ch == 'n')
            //                {
            //                    var drawProfit = profitsQuery.ToList().Where(c => c.updateDate > firstOfThisMonth && c.updateDate <= firstOfNextMonth && c.branchCreatorId.Value == id)
            //                                                  .Select(b => b.invoiceProfit).Sum();

            //                    profitLst.Add(decimal.Parse(HelpClass.DecTostring(drawProfit)));
            //                }
            //                else if (ch == 'o')
            //                {
            //                    decimal sum = 0;
            //                    for (int i = 0; i < otherIds.Count; i++)
            //                    {
            //                        var drawProfit = profitsQuery.ToList().Where(c => c.updateDate > firstOfThisMonth && c.updateDate <= firstOfNextMonth && c.branchCreatorId.Value == otherIds[i])
            //                                                 .Select(b => b.invoiceProfit).Sum();
            //                        sum = sum + drawProfit;
            //                    }
            //                    profitLst.Add(decimal.Parse(HelpClass.DecTostring(sum)));
            //                }
            //            }
            //            else if (selectedTab == 1)
            //            {
            //                if (ch == 'n')
            //                {
            //                    var drawProfit = profitsQuery.ToList().Where(c => c.updateDate > firstOfThisMonth && c.updateDate <= firstOfNextMonth && c.ITitemId.Value == id)
            //                                                  .Select(b => b.itemunitProfit).Sum();

            //                    profitLst.Add(decimal.Parse(HelpClass.DecTostring(drawProfit)));
            //                }
            //                else if (ch == 'o')
            //                {
            //                    decimal sum = 0;
            //                    for (int i = 0; i < otherIds.Count; i++)
            //                    {
            //                        var drawProfit = profitsQuery.ToList().Where(c => c.updateDate > firstOfThisMonth && c.updateDate <= firstOfNextMonth && c.ITitemId.Value == otherIds[i])
            //                                                 .Select(b => b.itemunitProfit).Sum();
            //                        sum = sum + drawProfit;
            //                    }
            //                    profitLst.Add(decimal.Parse(HelpClass.DecTostring(sum)));
            //                }
            //            }
            //            MyAxis.Labels.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) + "/" + year);

            //            if (year == endYear && month == endMonth)
            //            {
            //                break;
            //            }
            //            if (month == 12)
            //            {
            //                startMonth = 1;
            //                break;
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    for (int year = startYear; year <= endYear; year++)
            //    {
            //        var firstOfThisYear = new DateTime(year, 1, 1);
            //        var firstOfNextMYear = firstOfThisYear.AddYears(1);

            //        if (selectedTab == 0)
            //        {
            //            if (ch == 'n')
            //            {
            //                var drawProfit = profitsQuery.ToList().Where(c => c.updateDate > firstOfThisYear && c.updateDate <= firstOfNextMYear && c.branchCreatorId.Value == id)
            //                                               .Select(b => b.invoiceProfit).Sum();

            //                profitLst.Add(decimal.Parse(HelpClass.DecTostring(drawProfit)));
            //            }
            //            else if (ch == 'o')
            //            {
            //                decimal sum = 0;
            //                for (int i = 0; i < otherIds.Count; i++)
            //                {
            //                    var drawProfit = profitsQuery.ToList().Where(c => c.updateDate > firstOfThisYear && c.updateDate <= firstOfNextMYear && c.branchCreatorId.Value == otherIds[i])
            //                                              .Select(b => b.invoiceProfit).Sum();
            //                    sum = sum + drawProfit;
            //                }
            //                profitLst.Add(decimal.Parse(HelpClass.DecTostring(sum)));
            //            }
            //        }
            //        else if (selectedTab == 1)
            //        {
            //            if (ch == 'n')
            //            {
            //                var drawProfit = profitsQuery.ToList().Where(c => c.updateDate > firstOfThisYear && c.updateDate <= firstOfNextMYear && c.ITitemId.Value == id)
            //                                               .Select(b => b.itemunitProfit).Sum();

            //                profitLst.Add(decimal.Parse(HelpClass.DecTostring(drawProfit)));
            //            }
            //            else if (ch == 'o')
            //            {
            //                decimal sum = 0;
            //                for (int i = 0; i < otherIds.Count; i++)
            //                {
            //                    var drawProfit = profitsQuery.ToList().Where(c => c.updateDate > firstOfThisYear && c.updateDate <= firstOfNextMYear && c.ITitemId.Value == otherIds[i])
            //                                               .Select(b => b.itemunitProfit).Sum();
            //                    sum = sum + drawProfit;
            //                }
            //                profitLst.Add(decimal.Parse(HelpClass.DecTostring(sum)));
            //            }
            //        }
            //        MyAxis.Labels.Add(year.ToString());
            //    }
            //}

            //rowChartData.Add(
            //            new LineSeries
            //            {
            //                Values = profitLst.AsChartValues(),
            //                Title = name
            //            });


        }

        #endregion
        #region reports
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
                if (sender != null)
                    HelpClass.StartAwait(grid_main);
                List<ItemTransferInvoice> query = new List<ItemTransferInvoice>();

                #region
                BuildReport();

                LocalReportExtensions.PrintToPrinterbyNameAndCopy(rep, AppSettings.rep_printer_name, short.Parse(AppSettings.rep_print_count));

                #endregion

                if (sender != null)
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }

        }

        private void Btn_exportToExcel_Click(object sender, RoutedEventArgs e)
        {//excel
            try
            {
                if (sender != null)
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

                if (sender != null)
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }

        }

        private void Btn_preview_Click(object sender, RoutedEventArgs e)
        {//preview
            try
            {
                if (sender != null)
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

                if (sender != null)
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
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
                if (selectedTab == 0)
                {
                    addpath = @"\Reports\StatisticReport\Accounts\Profit\Ar\Profit.rdlc";
                    secondTitle = "invoice";
                }
                else
                {
                    addpath = @"\Reports\StatisticReport\Accounts\Profit\Ar\ProfitItem.rdlc";
                    secondTitle = "items";
                }

                //Reports\StatisticReport\Sale\Daily\Ar
            }
            else
            {
                if (selectedTab == 0)
                {
                    addpath = @"\Reports\StatisticReport\Accounts\Profit\En\Profit.rdlc";
                    secondTitle = "invoice";
                }
                else
                {
                    addpath = @"\Reports\StatisticReport\Accounts\Profit\En\ProfitItem.rdlc";
                    secondTitle = "items";
                }
            }

            string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);

            ReportCls.checkLang();
            subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);
            Title = AppSettings.resourcemanagerreport.GetString("trAccounting") + " / " + subTitle;
            paramarr.Add(new ReportParameter("trTitle", Title));

            // IEnumerable<ItemUnitInvoiceProfit>
            //clsReports.ProfitReport(profitsQuery, rep, reppath, paramarr);
            paramarr.Add(new ReportParameter("totalBalance", tb_total.Text));

            clsReports.setReportLanguage(paramarr);
            clsReports.Header(paramarr);

            rep.SetParameters(paramarr);

            rep.Refresh();
        }

        #endregion
    }
}