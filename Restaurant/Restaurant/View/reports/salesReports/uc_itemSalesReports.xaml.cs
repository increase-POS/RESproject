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

namespace Restaurant.View.reports.salesReports
{
    /// <summary>
    /// Interaction logic for uc_itemSalesReports.xaml
    /// </summary>
    public partial class uc_itemSalesReports : UserControl
    {
        private static uc_itemSalesReports _instance;
        public static uc_itemSalesReports Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new uc_itemSalesReports();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_itemSalesReports()
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

        private int selectedTab = 0;
        Statistics statisticModel = new Statistics();
        List<ItemTransferInvoice> Items;

        //for combo boxes
        /*************************/
        Branch selectedBranch;
        ItemUnitCombo selectedItem;

        List<Branch> comboBranches;
        List<ItemUnitCombo> itemUnitCombos;

        List<Branch> comboBrachTemp = new List<Branch>();
        List<ItemUnitCombo> comboItemTemp = new List<ItemUnitCombo>();

        List<Branch> dynamicComboBranches = new List<Branch>();
        List<ItemUnitCombo> dynamicComboItem = new List<ItemUnitCombo>();

        Branch branchModel = new Branch();
        /*************************/

        List<int> selectedBranchId = new List<int>();

        List<int> selectedItemId = new List<int>();

        // report
        ReportCls reportclass = new ReportCls();
        LocalReport rep = new LocalReport();
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        IEnumerable<ItemTransferInvoice> temp;


        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {//load
            try
            {
               HelpClass.StartAwait(grid_main);

                Items = await statisticModel.GetSaleitem((int)MainWindow.branchLogin.branchId, (int)MainWindow.userLogin.userId);

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

                comboBranches = await branchModel.GetAllWithoutMain("b");

                itemUnitCombos = statisticModel.GetIUComboList(Items);

                fillComboBranches(cb_collect);
                fillComboItemsBranches(cb_ItemsBranches);
                await fillComboItemTypes();
                fillComboItems();

                btn_items_Click(btn_items, null);

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
            tt_items.Content = AppSettings.resourcemanager.GetString("trItems");
            tt_collect.Content = AppSettings.resourcemanager.GetString("trBestSeller");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_ItemsBranches, AppSettings.resourcemanager.GetString("trBranchHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_Items, AppSettings.resourcemanager.GetString("trItemHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_Types, AppSettings.resourcemanager.GetString("trCategorie") + "...");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_collect, AppSettings.resourcemanager.GetString("trBranchHint"));

            chk_allcollect.Content = AppSettings.resourcemanager.GetString("trAll");
            chk_allBranchesItem.Content = AppSettings.resourcemanager.GetString("trAll");
            chk_allItems.Content = AppSettings.resourcemanager.GetString("trAll");
            chk_allTypes.Content = AppSettings.resourcemanager.GetString("trAll");


            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_ItemEndDate, AppSettings.resourcemanager.GetString("trEndDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_ItemStartDate, AppSettings.resourcemanager.GetString("trStartDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_collectEndDate, AppSettings.resourcemanager.GetString("trEndDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_collectStartDate, AppSettings.resourcemanager.GetString("trStartDateHint"));

            MaterialDesignThemes.Wpf.HintAssist.SetHint(txt_search, AppSettings.resourcemanager.GetString("trSearchHint"));
            tt_refresh.Content = AppSettings.resourcemanager.GetString("trRefresh");

            col_number.Header = AppSettings.resourcemanager.GetString("trNo.");
            col_date.Header = AppSettings.resourcemanager.GetString("trDate");
            col_type.Header = AppSettings.resourcemanager.GetString("trCategorie");
            col_branch.Header = AppSettings.resourcemanager.GetString("trBranch");
            col_item.Header = AppSettings.resourcemanager.GetString("trItem");
            col_itQuantity.Header = AppSettings.resourcemanager.GetString("trQTR");
            col_invCount.Header = AppSettings.resourcemanager.GetString("trInvoices");
            col_price.Header = AppSettings.resourcemanager.GetString("trPrice");
            col_total.Header = AppSettings.resourcemanager.GetString("trTotal");
            col_avg.Header = AppSettings.resourcemanager.GetString("trItem") + "/" + AppSettings.resourcemanager.GetString("tr_Invoice");

            tt_report.Content = AppSettings.resourcemanager.GetString("trPdf");
            tt_print.Content = AppSettings.resourcemanager.GetString("trPrint");
            tt_excel.Content = AppSettings.resourcemanager.GetString("trExcel");
            tt_preview.Content = AppSettings.resourcemanager.GetString("trPreview");
            tt_count.Content = AppSettings.resourcemanager.GetString("trCount");

        }

        private void fillComboBranches(ComboBox cb)
        {
            cb.SelectedValuePath = "branchId";
            cb.DisplayMemberPath = "name";
            var lst = Items.GroupBy(i => i.itemId).Select(i => new { i.FirstOrDefault().itemUnitId, i.FirstOrDefault().ITitemName });
            cb.ItemsSource = lst;

        }
        private void fillComboItemsBranches(ComboBox cb)
        {
            cb.SelectedValuePath = "branchId";
            cb.DisplayMemberPath = "name";
            cb.ItemsSource = comboBranches;
        }

        private void fillComboItems()
        {
            var lst = Items;///////where category
            cb_Items.ItemsSource = lst;
            cb_Items.SelectedValuePath = "itemUnitId";
            cb_Items.DisplayMemberPath = "itemUnitName";
        }

        private async Task fillComboItemTypes()
        {
            if (FillCombo.categoriesList is null)
                await FillCombo.RefreshCategory();
            List<Category> newList = FillCombo.categoriesList.Where(x => x.type == "s").ToList();
           
            cb_Types.ItemsSource = newList;
            cb_Types.SelectedValuePath = "categoryId";
            cb_Types.DisplayMemberPath = "name";
        }

        public void fillItemsEvent()
        {
            //temp = fillList(Items, cb_ItemsBranches, cb_Items, chk_itemInvoice, chk_itemReturn, dp_ItemStartDate, dp_ItemEndDate)
            //    .Where(j => (selectedItemId.Count != 0 ? selectedItemId.Contains((int)j.ITitemUnitId) : true));

            //dgInvoice.ItemsSource = temp;

            //txt_count.Text = dgInvoice.Items.Count.ToString();

            //fillPieChart();
            //fillColumnChart();
            //fillRowChart(selectedItemId);
        }

        public void fillCollectEvent()
        {
            temp = fillCollectListBranch(Items, dp_collectStartDate, dp_collectEndDate)
                .Where(j => (selectedBranchId.Count != 0 ? selectedBranchId.Contains((int)j.branchCreatorId) : true));

            dgInvoice.ItemsSource = temp;
            txt_count.Text = dgInvoice.Items.Count.ToString();

            fillPieChartCollect();
            fillColumnChartCollect();
            fillRowChartCollect();
        }

        public void fillCollectEventAll()
        {
            temp = fillCollectListAll(Items, dp_collectStartDate, dp_collectEndDate);

            dgInvoice.ItemsSource = temp;

            txt_count.Text = dgInvoice.Items.Count.ToString();

            fillPieChartCollect();
            fillColumnChartCollect();
            fillRowChartCollect();
        }

        public void paint()
        {
            bdrMain.RenderTransform = Animations.borderAnimation(50, bdrMain, true);

            bdr_items.Background =  Application.Current.Resources["SecondColor"] as SolidColorBrush;
            bdr_collect.Background =  Application.Current.Resources["SecondColor"] as SolidColorBrush;

            path_items.Fill = Brushes.White;
            path_collect.Fill = Brushes.White;
        }

        private void btn_items_Click(object sender, RoutedEventArgs e)
        {//items
            try
            {
               HelpClass.StartAwait(grid_main);

                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), (sender as Button).Tag.ToString());

                txt_search.Text = "";
                selectedTab = 0;

                ReportsHelp.showSelectedStack(grid_stacks, stk_tagsItems);

                paint();
                ReportsHelp.paintTabControlBorder(grid_tabControl, bdr_items);
                path_items.Fill =  Application.Current.Resources["SecondColor"] as SolidColorBrush;

                ReportsHelp.hideAllColumns(dgInvoice);
                //show columns
                grid_items.Visibility = Visibility.Visible;
                grid_collect.Visibility = Visibility.Hidden;
                col_number.Visibility = Visibility.Visible;
                col_date.Visibility = Visibility.Visible;
                col_branch.Visibility = Visibility.Visible;
                col_item.Visibility = Visibility.Visible;
                col_type.Visibility = Visibility.Visible;
                col_itQuantity.Visibility = Visibility.Visible;
                col_price.Visibility = Visibility.Visible;
                col_total.Visibility = Visibility.Visible;

                fillItemsEvent();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }

        }

        private void Btn_collect_Click(object sender, RoutedEventArgs e)
        {//collect
            try
            {
                 HelpClass.StartAwait(grid_main);

                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), (sender as Button).Tag.ToString());

                txt_search.Text = "";
                selectedTab = 1;

                ReportsHelp.showSelectedStack(grid_stacks, stk_tagsBranches);

                paint();
                ReportsHelp.paintTabControlBorder(grid_tabControl, bdr_collect);
                path_collect.Fill =  Application.Current.Resources["SecondColor"] as SolidColorBrush;

                ReportsHelp.hideAllColumns(dgInvoice);
                //show columns
                grid_items.Visibility = Visibility.Hidden;
                grid_collect.Visibility = Visibility.Visible;
                col_branch.Visibility = Visibility.Visible;
                col_item.Visibility = Visibility.Visible;
                col_itQuantity.Visibility = Visibility.Visible;
                col_invCount.Visibility = Visibility.Visible;
                col_avg.Visibility = Visibility.Visible;

                if (stk_tagsBranches.Children.Count == 0)
                {
                    fillCollectEventAll();
                }
                else
                {
                    fillCollectEvent();
                }

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        #region Items Events
        private void Chip_OnDeleteItemClick(object sender, RoutedEventArgs e)
        {
            var currentChip = (Chip)sender;
            stk_tagsItems.Children.Remove(currentChip);
            var m = comboItemTemp.Where(j => j.itemUnitId == (Convert.ToInt32(currentChip.Name.Remove(0, 3))));
            dynamicComboItem.Add(m.FirstOrDefault());
            selectedItemId.Remove(Convert.ToInt32(currentChip.Name.Remove(0, 3)));
            if (selectedItemId.Count == 0)
            {
                cb_Items.SelectedItem = null;
            }
            fillItemsEvent();

        }

        private void cb_Items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_Items.SelectedItem != null)
            {
                if (stk_tagsItems.Children.Count < 5)
                {
                    selectedItem = cb_Items.SelectedItem as ItemUnitCombo;
                    var b = new MaterialDesignThemes.Wpf.Chip()
                    {
                        Content = selectedItem.itemUnitName,
                        Name = "btn" + selectedItem.itemUnitId.ToString(),
                        IsDeletable = true,
                        Margin = new Thickness(5, 0, 5, 0)
                    };
                    b.DeleteClick += Chip_OnDeleteItemClick;
                    stk_tagsItems.Children.Add(b);
                    comboItemTemp.Add(selectedItem);
                    selectedItemId.Add(selectedItem.itemUnitId);
                    dynamicComboItem.Remove(selectedItem);
                    fillItemsEvent();
                }
            }
        }

        private void chk_allItems_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 
                    HelpClass.StartAwait(grid_main); if (cb_Items.IsEnabled == true)
                {
                    cb_Items.SelectedItem = null;
                    cb_Items.IsEnabled = false;
                    for (int i = 0; i < comboItemTemp.Count; i++)
                    {
                        dynamicComboItem.Add(comboItemTemp.Skip(i).FirstOrDefault());
                    }
                    comboItemTemp.Clear();
                    stk_tagsItems.Children.Clear();
                    selectedItemId.Clear();
                }
                else
                {
                    cb_Items.IsEnabled = true;
                }

                fillItemsEvent();
                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void fillItemsEventCall(object sender)
        {
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                fillItemsEvent();

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void fillCollectEventsCall(object sender)
        {
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                fillCollectEvent();

                if (stk_tagsBranches.Children.Count == 0)
                {
                    fillCollectEventAll();
                }

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void chk_Checked(object sender, RoutedEventArgs e)
        {
            fillItemsEventCall(sender);
        }

        private void dp_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            fillItemsEventCall(sender);
        }

        private void dt_SelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            fillItemsEventCall(sender);
        }

        private void Cb_ItemsBranches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fillItemsEvent();
        }

        private void Chk_allBranchesItem_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                cb_ItemsBranches.SelectedItem = null;
                cb_ItemsBranches.IsEnabled = false;

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }

        }

        private void Chk_allBranchesItem_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                cb_ItemsBranches.IsEnabled = true;

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }

        }

        private void Chk_allItems_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                cb_Items.SelectedItem = null;
                cb_Items.IsEnabled = false;
                for (int i = 0; i < comboItemTemp.Count; i++)
                {
                    dynamicComboItem.Add(comboItemTemp.Skip(i).FirstOrDefault());
                }
                comboItemTemp.Clear();
                stk_tagsItems.Children.Clear();
                selectedItemId.Clear();
                fillItemsEvent();

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Chk_allItems_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                cb_Items.IsEnabled = true;
                fillItemsEvent();

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Chk_allTypes_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                cb_Types.SelectedItem = null;
                cb_Types.IsEnabled = false;

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Chk_allTypes_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                cb_Types.IsEnabled = true;

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion

        #region Charts
        private void fillPieChart()
        {
            List<string> titles = new List<string>();
            List<int> x = new List<int>();
            titles.Clear();

            var temp = itemList
             .Where(j => (selectedItemId.Count != 0 ? selectedItemId.Contains((int)j.ITitemUnitId) : true));
            var titleTemp = temp.GroupBy(jj => jj.ITitemUnitId)
             .Select(g => new ItemUnitCombo
             {
                 itemUnitId = (int)g.FirstOrDefault().ITitemUnitId,
                 itemUnitName = g.FirstOrDefault().ITitemName + "-" + g.FirstOrDefault().ITunitName
             }).ToList();
            titles.AddRange(titleTemp.Select(jj => jj.itemUnitName));
            var result = temp.GroupBy(s => s.ITitemUnitId).Select(s => new
            {
                ITitemUnitId = s.Key,
                count = s.Count()
            });
            x = result.Select(m => m.count).ToList();
            int count = x.Count();
            SeriesCollection piechartData = new SeriesCollection();
            for (int i = 0; i < count; i++)
            {
                List<long> final = new List<long>();
                List<string> lable = new List<string>();
                if (i < 5)
                {
                    final.Add(x.Max());
                    lable.Add(titles.Skip(i).FirstOrDefault());
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
                  );
                    break;
                }

            }
            chart1.Series = piechartData;
        }

        private void fillColumnChart()
        {
            axcolumn.Labels = new List<string>();
            List<string> names = new List<string>();
            List<int> x = new List<int>();
            IEnumerable<int> y = null;

            var temp = itemList
                   .Where(j => (selectedItemId.Count != 0 ? selectedItemId.Contains((int)j.ITitemUnitId) : true));
            var result = temp.GroupBy(s => s.ITitemUnitId).Select(s => new
            {
                ITitemUnitId = s.Key,
                countP = s.Where(m => m.invType == "s").Count(),
                countPb = s.Where(m => m.invType == "sb").Count(),

            });
            x = result.Select(m => m.countP).ToList();
            y = result.Select(m => m.countPb);
            var tempName = temp.GroupBy(jj => jj.ITitemUnitId)
             .Select(g => new ItemUnitCombo { itemUnitId = (int)g.FirstOrDefault().ITitemUnitId, itemUnitName = g.FirstOrDefault().ITitemName + "-" + g.FirstOrDefault().ITunitName }).ToList();
            names.AddRange(tempName.Select(nn => nn.itemUnitName));

            List<string> lable = new List<string>();
            SeriesCollection columnChartData = new SeriesCollection();
            List<int> cP = new List<int>();
            List<int> cPb = new List<int>();
            List<int> cD = new List<int>();
            List<string> titles = new List<string>()
            {
                AppSettings.resourcemanager.GetString("trSales"),
                AppSettings.resourcemanager.GetString("trReturned"),
                AppSettings.resourcemanager.GetString("trDraft"),
            };
            int count = x.Count();
            for (int i = 0; i < count; i++)
            {
                if (i < 5)
                {
                    cP.Add(x.Max());
                    x.Remove(x.Max());
                }
                else
                {
                    cP.Add(x.Sum());
                    break;
                }
                axcolumn.Labels.Add(names.ToList().Skip(i).FirstOrDefault());
            }
            axcolumn.Labels.Add(AppSettings.resourcemanager.GetString("trOthers"));

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


            DataContext = this;
            cartesianChart.Series = columnChartData;
        }

        private void fillRowChart(List<int> stackedButton)
        {
            MyAxis.Labels = new List<string>();
            List<string> names = new List<string>();
            IEnumerable<decimal> pTemp = null;
            IEnumerable<decimal> pbTemp = null;
            IEnumerable<decimal> resultTemp = null;

            //var temp = fillRowChartList(Items, chk_itemInvoice, chk_itemReturn, chk_itemDrafs, dp_ItemStartDate, dp_ItemEndDate, dt_itemStartTime, dt_ItemEndTime);
            var temp = Items;
            temp = temp.Where(j => (selectedItemId.Count != 0 ? stackedButton.Contains((int)j.ITitemUnitId) : true)).ToList();
            var result = temp.GroupBy(s => s.ITitemUnitId).Select(s => new
            {
                ITitemUnitId = s.Key,
                totalP = s.Where(x => x.invType == "s").Sum(x => x.totalNet),
                totalPb = s.Where(x => x.invType == "sb").Sum(x => x.totalNet),
            }
         );
            var resultTotal = result.Select(x => new { x.ITitemUnitId, total = x.totalP - x.totalPb }).ToList();
            pTemp = result.Select(x => (decimal)x.totalP);
            pbTemp = result.Select(x => (decimal)x.totalPb);
            resultTemp = result.Select(x => (decimal)x.totalP - (decimal)x.totalPb);
            var tempName = temp.GroupBy(jj => jj.ITitemUnitId)
             .Select(g => new ItemUnitCombo { itemUnitId = (int)g.FirstOrDefault().ITitemUnitId, itemUnitName = g.FirstOrDefault().ITitemName + "-" + g.FirstOrDefault().ITunitName }).ToList();
            names.AddRange(tempName.Select(nn => nn.itemUnitName));
            /********************************************************************************/
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

            int xCount = 0;
            if (pTemp.Count() <= 6) xCount = pTemp.Count();

            for (int i = 0; i < xCount; i++)
            {
                purchase.Add(pTemp.ToList().Skip(i).FirstOrDefault());
                returns.Add(pbTemp.ToList().Skip(i).FirstOrDefault());
                sub.Add(resultTemp.ToList().Skip(i).FirstOrDefault());
                MyAxis.Labels.Add(names.ToList().Skip(i).FirstOrDefault());
            }
            if (pTemp.Count() > 6)
            {
                decimal purchaseSum = 0, returnsSum = 0, subSum = 0;
                for (int i = 0; i < xCount; i++)
                {
                    purchaseSum = purchaseSum + pTemp.ToList().Skip(i).FirstOrDefault();
                    returnsSum = returnsSum + pbTemp.ToList().Skip(i).FirstOrDefault();
                    subSum = subSum + resultTemp.ToList().Skip(i).FirstOrDefault();
                }
                if (!((purchaseSum == 0) && (returnsSum == 0) && (subSum == 0)))
                {
                    purchase.Add(purchaseSum);
                    returns.Add(returnsSum);
                    sub.Add(subSum);
                    MyAxis.Labels.Add(AppSettings.resourcemanager.GetString("trOthers"));
                }
            }
            rowChartData.Add(
          new LineSeries
          {
              Values = purchase.AsChartValues(),
              Title = titles[0]
          });
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
        #endregion

        #region Collect Events
        private void Dp_colletSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            fillCollectEventsCall(sender);
        }

        private void Chip_OnDeleteBranchClick(object sender, RoutedEventArgs e)
        {
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                var currentChip = (Chip)sender;
                stk_tagsBranches.Children.Remove(currentChip);
                var m = comboBrachTemp.Where(j => j.branchId == (Convert.ToInt32(currentChip.Name.Remove(0, 3))));
                dynamicComboBranches.Add(m.FirstOrDefault());
                selectedBranchId.Remove(Convert.ToInt32(currentChip.Name.Remove(0, 3)));
                if (selectedBranchId.Count == 0)
                {
                    cb_collect.SelectedItem = null;
                }

                if (stk_tagsBranches.Children.Count == 0)
                {
                    fillCollectEventAll();
                }
                else
                {
                    fillCollectEvent();
                }

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Cb_collect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                if (cb_collect.SelectedItem != null)
                {
                    if (stk_tagsBranches.Children.Count < 5)
                    {
                        selectedBranch = cb_collect.SelectedItem as Branch;
                        var b = new MaterialDesignThemes.Wpf.Chip()
                        {
                            Content = selectedBranch.name,
                            Name = "btn" + selectedBranch.branchId.ToString(),
                            IsDeletable = true,
                            Margin = new Thickness(5, 0, 5, 0)
                        };
                        b.DeleteClick += Chip_OnDeleteBranchClick;
                        stk_tagsBranches.Children.Add(b);
                        comboBrachTemp.Add(selectedBranch);
                        selectedBranchId.Add(selectedBranch.branchId);
                        dynamicComboBranches.Remove(selectedBranch);
                        fillCollectEvent();
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

        private void Chk_allcollect_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                cb_collect.SelectedItem = null;
                cb_collect.IsEnabled = false;
                for (int i = 0; i < comboBrachTemp.Count; i++)
                {
                    dynamicComboBranches.Add(comboBrachTemp.Skip(i).FirstOrDefault());
                }
                comboBrachTemp.Clear();
                stk_tagsBranches.Children.Clear();
                selectedBranchId.Clear();
                fillCollectEvent();
                if (stk_tagsBranches.Children.Count == 0)
                {
                    fillCollectEventAll();
                }
                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Chk_allcollect_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                cb_collect.IsEnabled = true;
                fillCollectEvent();
                if (stk_tagsBranches.Children.Count == 0)
                {
                    fillCollectEventAll();
                }
                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion

        #region fillLists
        List<ItemTransferInvoice> itemList = null;
        private List<ItemTransferInvoice> fillList(IEnumerable<ItemTransferInvoice> Invoices, ComboBox cbBranch, ComboBox cbitem, CheckBox chkInvoice, CheckBox chkReturn, DatePicker startDate, DatePicker endDate)
        {
            var selectedBranch = cbBranch.SelectedItem as Branch;
            var selectedItemUnit = cbitem.SelectedItem as ItemUnitCombo;

            var result = Invoices.Where(x => (

           ((chkReturn.IsChecked == true ? (x.invType == "sb") : false) || (chkInvoice.IsChecked == true ? (x.invType == "s") : false))
                      && (cbBranch.SelectedItem != null ? x.branchCreatorId == selectedBranch.branchId : true)
                      && (cb_Items.SelectedItem != null ? x.itemUnitId == selectedItemUnit.itemUnitId : true)
                      && (cb_Types.SelectedItem != null ? x.ITtype == cb_Types.SelectedValue.ToString() : true)
                      && (startDate.SelectedDate != null ? x.invDate >= startDate.SelectedDate : true)
                      && (endDate.SelectedDate != null ? x.invDate <= endDate.SelectedDate : true)));

            itemList = result.ToList();
            return result.ToList();
        }

        private IEnumerable<ItemTransferInvoice> fillRowChartList(IEnumerable<ItemTransferInvoice> Invoices, CheckBox chkInvoice, CheckBox chkReturn, CheckBox chkDraft, DatePicker startDate, DatePicker endDate, TimePicker startTime, TimePicker endTime)
        {
            var result = Invoices.Where(x => (
                         (startDate.SelectedDate != null ? x.invDate >= startDate.SelectedDate : true)
                        && (endDate.SelectedDate != null ? x.invDate <= endDate.SelectedDate : true)));

            return result;
        }
        List<ItemTransferInvoice> collectListBranch = null;
        private List<ItemTransferInvoice> fillCollectListBranch(IEnumerable<ItemTransferInvoice> Invoices, DatePicker startDate, DatePicker endDate)
        {
            var temp = Invoices
                .Where(x =>
                 (startDate.SelectedDate != null ? x.updateDate >= startDate.SelectedDate : true)
                && (endDate.SelectedDate != null ? x.updateDate <= endDate.SelectedDate : true))
                .GroupBy(obj => new
                {
                    obj.branchCreatorId,
                    obj.ITitemUnitId
                }).Select(obj => new ItemTransferInvoice
                {
                    branchCreatorId = obj.FirstOrDefault().branchCreatorId,
                    branchCreatorName = obj.FirstOrDefault().branchCreatorName,
                    itemUnitId = obj.FirstOrDefault().itemUnitId,
                    ItemUnits = obj.FirstOrDefault().ItemUnits,
                    invoiceId = obj.FirstOrDefault().invoiceId,
                    ITquantity = obj.Sum(x => x.ITquantity),
                    ITitemName = obj.FirstOrDefault().ITitemName,
                    ITitemId = obj.FirstOrDefault().ITitemId,
                    ITunitName = obj.FirstOrDefault().ITunitName,
                    ITunitId = obj.FirstOrDefault().ITunitId,
                    ITupdateDate = obj.FirstOrDefault().ITupdateDate,
                    itemAvg = obj.Average(x => x.ITquantity),
                    count = obj.Count()
                }).OrderByDescending(obj => obj.ITquantity);

            collectListBranch = temp.ToList();
            return temp.ToList();
        }
        List<ItemTransferInvoice> collectListAll = null;
        private List<ItemTransferInvoice> fillCollectListAll(IEnumerable<ItemTransferInvoice> Invoices, DatePicker startDate, DatePicker endDate)
        {
            var temp = Invoices
                .Where(x =>
                 (startDate.SelectedDate != null ? x.updateDate >= startDate.SelectedDate : true)
                && (endDate.SelectedDate != null ? x.updateDate <= endDate.SelectedDate : true))
                .GroupBy(obj => new
                {
                    obj.ITitemUnitId
                }).Select(obj => new ItemTransferInvoice
                {
                    branchCreatorId = obj.FirstOrDefault().branchCreatorId,
                    branchCreatorName = "All Branches",
                    itemUnitId = obj.FirstOrDefault().itemUnitId,
                    ItemUnits = obj.FirstOrDefault().ItemUnits,
                    invoiceId = obj.FirstOrDefault().invoiceId,
                    ITquantity = obj.Sum(x => x.ITquantity),
                    ITitemName = obj.FirstOrDefault().ITitemName,
                    ITitemId = obj.FirstOrDefault().ITitemId,
                    ITunitName = obj.FirstOrDefault().ITunitName,
                    ITunitId = obj.FirstOrDefault().ITunitId,
                    itemAvg = obj.Average(x => x.ITquantity),
                    ITupdateDate = obj.FirstOrDefault().ITupdateDate,
                    count = obj.Count()
                }).OrderByDescending(obj => obj.ITquantity);

            collectListAll = temp.ToList();
            return temp.ToList();
        }
        #endregion

        #region General Events
        private void Txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {//search
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                IEnumerable<ItemTransferInvoice> tempSearch = itemList;

                if (selectedTab == 0)
                {
                    tempSearch = itemList.Where(s =>
                                                s.invNumber.ToLower().Contains(txt_search.Text) ||
                                                 s.branchCreatorName.ToLower().Contains(txt_search.Text) ||
                                                 s.ITitemName.ToLower().Contains(txt_search.Text) ||
                                                 s.ITunitName.ToLower().Contains(txt_search.Text) ||
                                                 s.ITquantity.ToString().ToLower().Contains(txt_search.Text) ||
                                                 s.ITprice.ToString().ToLower().Contains(txt_search.Text) ||
                                                 s.subTotal.ToString().ToLower().Contains(txt_search.Text)
                                                 );

                    fillPieChart();
                    fillColumnChart();
                    fillRowChart(selectedItemId);
                }
                else if (selectedTab == 1)
                {
                    if (stk_tagsBranches.Children.Count == 0)
                    {
                        tempSearch = collectListAll.Where(s =>
                                                     s.branchCreatorName.ToLower().Contains(txt_search.Text) ||
                                                     s.ITitemName.ToLower().Contains(txt_search.Text) ||
                                                     s.ITunitName.ToLower().Contains(txt_search.Text) ||
                                                     s.count.ToString().ToLower().Contains(txt_search.Text) ||
                                                     s.ITquantity.ToString().ToLower().Contains(txt_search.Text) ||
                                                     s.itemAvg.ToString().ToLower().Contains(txt_search.Text)
                                                     );
                    }
                    else
                    {
                        tempSearch = collectListBranch.Where(s =>
                                                    s.branchCreatorName.ToLower().Contains(txt_search.Text) ||
                                                    s.ITitemName.ToLower().Contains(txt_search.Text) ||
                                                    s.ITunitName.ToLower().Contains(txt_search.Text) ||
                                                    s.ITquantity.ToString().ToLower().Contains(txt_search.Text) ||
                                                    s.count.ToString().ToLower().Contains(txt_search.Text) ||
                                                    s.itemAvg.ToString().ToLower().Contains(txt_search.Text)
                                                    );

                    }
                    fillPieChartCollect();
                    fillColumnChartCollect();
                    fillRowChartCollect();
                }

                dgInvoice.ItemsSource = tempSearch;
                txt_count.Text = dgInvoice.Items.Count.ToString();

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {//refresh
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                if (selectedTab == 0)
                {
                    chk_allBranchesItem.IsChecked = true;
                    chk_allItems.IsChecked = true;
                    dp_ItemStartDate.SelectedDate = null;
                    dp_ItemEndDate.SelectedDate = null;

                    fillItemsEvent();
                }
                else if (selectedTab == 1)
                {
                    dp_collectEndDate.SelectedDate = null;
                    dp_collectStartDate.SelectedDate = null;
                    chk_allcollect.IsChecked = true;
                    stk_tagsBranches.Children.Clear();
                    fillCollectEventAll();
                }

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void btn_settings_Click(object sender, RoutedEventArgs e)
        {//settings

        }

        private void Btn_exportToExcel_Click(object sender, RoutedEventArgs e)
        {//excel
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                //Thread t1 = new Thread(() =>
                //{
                ExcelSalesItems();
                //});
                //t1.Start();

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void ExcelSalesItems()
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

        private void Btn_print_Click(object sender, RoutedEventArgs e)
        {//print
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                /////////////////////////////////////
                Thread t1 = new Thread(() =>
                {
                    printSalesItems();
                });
                t1.Start();
                //////////////////////////////////////

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void printSalesItems()
        {
            BuildReport();

            this.Dispatcher.Invoke(() =>
            {
                LocalReportExtensions.PrintToPrinterbyNameAndCopy(rep, AppSettings.rep_printer_name, short.Parse(AppSettings.rep_print_count));
            });
        }

        private void Btn_pdf_Click(object sender, RoutedEventArgs e)
        {//pdf
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                /////////////////////////////////////
                Thread t1 = new Thread(() =>
                {
                    pdfSelesItems();
                });
                t1.Start();
                //////////////////////////////////////

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void pdfSelesItems()
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

        private void BuildReport()
        {
            List<ReportParameter> paramarr = new List<ReportParameter>();

            string addpath = "";
            string itemtype = "";

            string firstTitle = "saleItems";
            string secondTitle = "";
            string subTitle = "";
            string Title = "";

            bool isArabic = ReportCls.checkLang();
            if (isArabic)
            {

                if (selectedTab == 0)
                {

                    this.Dispatcher.Invoke(() =>
                    {
                        if (chk_allTypes.IsChecked == true || cb_Types.SelectedValue == null)
                        {
                            itemtype = "";
                        }
                        else
                        {
                            itemtype = clsReports.itemTypeConverter(cb_Types.SelectedValue.ToString());
                        }
                        secondTitle = "items";
                        subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);

                        Title = AppSettings.resourcemanagerreport.GetString("trSalesReport");
                        paramarr.Add(new ReportParameter("trTitle", Title));
                    });


                    // cb_Types.
                    addpath = @"\Reports\StatisticReport\Sale\Item\Ar\ArItem.rdlc";
                    //  paramarr.Add(new ReportParameter("trCode", AppSettings.resourcemanagerreport.GetString("trCode")));
                }
                else if (selectedTab == 1)
                {
                    addpath = @"\Reports\StatisticReport\Sale\Item\Ar\ArBestSel.rdlc";
                    secondTitle = "BestSeller";
                    subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);
                    Title = AppSettings.resourcemanagerreport.GetString("trSalesReport") + " / " + subTitle;
                    paramarr.Add(new ReportParameter("trTitle", Title));

                }
            }
            else
            {
                if (selectedTab == 0)
                {
                    addpath = @"\Reports\StatisticReport\Sale\Item\En\EnItem.rdlc";
                    this.Dispatcher.Invoke(() =>
                    {
                        if (chk_allTypes.IsChecked == true || cb_Types.SelectedValue == null)
                        {
                            itemtype = "";
                        }
                        else
                        {
                            itemtype = clsReports.itemTypeConverter(cb_Types.SelectedValue.ToString());
                        }
                        secondTitle = "items";
                        subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);
                        Title = AppSettings.resourcemanagerreport.GetString("trSalesReport");
                        paramarr.Add(new ReportParameter("trTitle", Title));
                    });

                }
                else if (selectedTab == 1)
                {
                    addpath = @"\Reports\StatisticReport\Sale\Item\En\EnBestSel.rdlc";

                    secondTitle = "BestSeller";
                    subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);
                    Title = AppSettings.resourcemanagerreport.GetString("trSalesReport") + " / " + subTitle;
                    paramarr.Add(new ReportParameter("trTitle", Title));
                }
            }
            string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);

            ReportCls.checkLang();


            //cb_Types cb_Types.SelectedValue.ToString() itemtypeconverter //chk_allTypes
            clsReports.saleitemStsReport(temp, rep, reppath, paramarr);
            clsReports.setReportLanguage(paramarr);
            clsReports.Header(paramarr);
            paramarr.Add(new ReportParameter("itemtype", itemtype));

            rep.SetParameters(paramarr);

            rep.Refresh();
        }

        private void Btn_preview_Click(object sender, RoutedEventArgs e)
        {//preview
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

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

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion

        #region collect charts
        private void fillPieChartCollect()
        {
            List<string> titles = new List<string>();
            List<long> x = new List<long>();
            titles.Clear();
            var temp = collectListAll;
            if (stk_tagsBranches.Children.Count > 0)
            {
                temp = collectListBranch
                 .Where(j => (selectedBranchId.Count != 0 ? selectedBranchId.Contains((int)j.branchCreatorId) : true)).ToList();
            }
            var titleTemp = temp.Select(obj => obj.ITitemUnitName1);
            titles.AddRange(titleTemp);
            x = temp.Select(m => (long)m.ITquantity).ToList();
            int count = x.Count();
            SeriesCollection piechartData = new SeriesCollection();
            for (int i = 0; i < count; i++)
            {
                List<long> final = new List<long>();
                List<string> lable = new List<string>();
                if (i < 5)
                {
                    final.Add(x.Max());
                    lable.Add(titles.Skip(i).FirstOrDefault());
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
                  );
                    break;
                }

            }
            chart1.Series = piechartData;
        }

        private void fillColumnChartCollect()
        {
            axcolumn.Labels = new List<string>();
            List<string> names = new List<string>();
            List<int> x = new List<int>();

            var temp = collectListAll;
            if (stk_tagsBranches.Children.Count > 0)
            {
                temp = collectListBranch
                 .Where(j => (selectedBranchId.Count != 0 ? selectedBranchId.Contains((int)j.branchCreatorId) : true)).ToList();
            }

            x = temp.Select(m => m.count).ToList();
            var tempName = temp.OrderByDescending(obj => obj.count).Select(obj => obj.ITitemUnitName1);
            names.AddRange(tempName);

            SeriesCollection columnChartData = new SeriesCollection();
            List<int> cP = new List<int>();
            List<int> cPb = new List<int>();
            List<int> cD = new List<int>();
            List<string> titles = new List<string>()
            {
               AppSettings.resourcemanager.GetString("trInvoices")
            };
            int count = x.Count();
            for (int i = 0; i < count; i++)
            {
                if (i < 5)
                {
                    cP.Add(x.Max());
                    x.Remove(x.Max());
                }
                else
                {
                    cP.Add(x.Sum());
                    break;
                }
                axcolumn.Labels.Add(names.ToList().Skip(i).FirstOrDefault());

            }
            axcolumn.Labels.Add(AppSettings.resourcemanager.GetString("trOthers"));
            //3 فوق بعض
            columnChartData.Add(
            new ColumnSeries
            {
                Values = cP.AsChartValues(),
                Title = titles[0],
                DataLabels = true,
            });

            DataContext = this;
            cartesianChart.Series = columnChartData;
        }

        private void fillRowChartCollect()
        {
            int endYear = DateTime.Now.Year;
            int startYear = endYear - 1;
            int startMonth = DateTime.Now.Month;
            int endMonth = startMonth;
            if (dp_collectStartDate.SelectedDate != null && dp_collectEndDate.SelectedDate != null)
            {
                startYear = dp_collectStartDate.SelectedDate.Value.Year;
                endYear = dp_collectEndDate.SelectedDate.Value.Year;
                startMonth = dp_collectStartDate.SelectedDate.Value.Month;
                endMonth = dp_collectEndDate.SelectedDate.Value.Month;
            }
            MyAxis.Labels = new List<string>();
            List<string> names = new List<string>();
            List<CashTransferSts> resultList = new List<CashTransferSts>();

            var temp = collectListAll;
            if (stk_tagsBranches.Children.Count > 0)
            {
                temp = collectListBranch
                 .Where(j => (selectedBranchId.Count != 0 ? selectedBranchId.Contains((int)j.branchCreatorId) : true)).ToList();
            }

            SeriesCollection rowChartData = new SeriesCollection();
            var tempName = temp.Select(s => s.IupdateDate);
            names.Add("x");

            List<string> lable = new List<string>();
            SeriesCollection columnChartData = new SeriesCollection();
            List<long> cash = new List<long>();

            if (endYear - startYear <= 1)
            {
                for (int year = startYear; year <= endYear; year++)
                {
                    for (int month = startMonth; month <= 12; month++)
                    {
                        var firstOfThisMonth = new DateTime(year, month, 1);
                        var firstOfNextMonth = firstOfThisMonth.AddMonths(1);
                        var drawCash = temp.ToList().Where(c => c.ITupdateDate > firstOfThisMonth && c.ITupdateDate <= firstOfNextMonth).Sum(obj => (long)obj.ITquantity);
                        cash.Add(drawCash);
                        MyAxis.Labels.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) + "/" + year);

                        if (year == endYear && month == endMonth)
                        {
                            break;
                        }
                        if (month == 12)
                        {
                            startMonth = 1;
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int year = startYear; year <= endYear; year++)
                {
                    var firstOfThisYear = new DateTime(year, 1, 1);
                    var firstOfNextMYear = firstOfThisYear.AddYears(1);
                    var drawCash = temp.ToList().Where(c => c.ITupdateDate > firstOfThisYear && c.ITupdateDate <= firstOfNextMYear).Sum(obj => (long)obj.ITquantity);
                    cash.Add(drawCash);
                    MyAxis.Labels.Add(year.ToString());
                }
            }
            rowChartData.Add(
          new LineSeries
          {
              Values = cash.AsChartValues(),
              Title = AppSettings.resourcemanager.GetString("trQuantity")
          });

            DataContext = this;
            rowChart.Series = rowChartData;
        }


        #endregion

    }
}
