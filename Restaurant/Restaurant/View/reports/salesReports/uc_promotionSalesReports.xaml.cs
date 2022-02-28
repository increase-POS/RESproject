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
    /// Interaction logic for uc_promotionSalesReports.xaml
    /// </summary>
    public partial class uc_promotionSalesReports : UserControl
    {
        private static uc_promotionSalesReports _instance;
        public static uc_promotionSalesReports Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new uc_promotionSalesReports();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_promotionSalesReports()
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

        List<ItemTransferInvoice> coupons;
        List<ItemTransferInvoice> Offers;

        //for combo boxes
        /*************************/
        CouponCombo selectedCouon;
        OfferCombo selectedOffer;

        List<CouponCombo> comboCoupon;
        List<OfferCombo> comboOffer;

        ObservableCollection<CouponCombo> comboCouponTemp = new ObservableCollection<CouponCombo>();
        ObservableCollection<OfferCombo> comboOfferTemp = new ObservableCollection<OfferCombo>();

        ObservableCollection<CouponCombo> dynamicComboCoupon;
        ObservableCollection<OfferCombo> dynamicComboOffer;

        /*************************/

        ObservableCollection<int> selectedcouponId = new ObservableCollection<int>();
        ObservableCollection<int> selectedOfferId = new ObservableCollection<int>();

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {//load
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                coupons = await statisticModel.GetSalecoupon((int)MainWindow.branchLogin.branchId, (int)MainWindow.userLogin.userId);

                Offers = await statisticModel.GetPromoOffer((int)MainWindow.branchLogin.branchId, (int)MainWindow.userLogin.userId);

                #region translate
                if (AppSettings.lang.Equals("en"))
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                else
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                translate();
                #endregion

                comboCoupon = statisticModel.GetCopComboList(coupons);
                comboOffer = statisticModel.GetOfferComboList(Offers);

                dynamicComboCoupon = new ObservableCollection<CouponCombo>(comboCoupon);
                dynamicComboOffer = new ObservableCollection<OfferCombo>(comboOffer);

                fillComboCoupon();

                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), btn_coupons.Tag.ToString());
                MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_Coupons, AppSettings.resourcemanager.GetString("trCoponHint"));
                paint();
                ReportsHelp.paintTabControlBorder(grid_tabControl, bdr_coupon);
                path_coupons.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;

                chk_couponInvoice.IsChecked = true;

                hidAllColumns();
                col_code.Visibility = Visibility.Visible;
                col_coupon.Visibility = Visibility.Visible;
                col_couponType.Visibility = Visibility.Visible;
                col_coupoValue.Visibility = Visibility.Visible;
                col_couponTotalValue.Visibility = Visibility.Visible;

                 
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
            tt_coupon.Content = AppSettings.resourcemanager.GetString("trCoupons");
            tt_offers.Content = AppSettings.resourcemanager.GetString("trOffer");

            chk_couponInvoice.Content = AppSettings.resourcemanager.GetString("tr_Invoice");
            chk_couponReturn.Content = AppSettings.resourcemanager.GetString("trReturned");
            chk_couponDrafs.Content = AppSettings.resourcemanager.GetString("trDraft");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_Coupons, AppSettings.resourcemanager.GetString("trCouponHint"));

            chk_allCoupon.Content = AppSettings.resourcemanager.GetString("trAll");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_couponEndDate, AppSettings.resourcemanager.GetString("trEndDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_couponStartDate, AppSettings.resourcemanager.GetString("trStartDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dt_couponEndTime, AppSettings.resourcemanager.GetString("trEndTime") + "...");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dt_couponStartTime, AppSettings.resourcemanager.GetString("trStartTime") + "...");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(txt_search, AppSettings.resourcemanager.GetString("trSearchHint"));
            tt_refresh.Content = AppSettings.resourcemanager.GetString("trRefresh");

            col_No.Header = AppSettings.resourcemanager.GetString("trNo.");
            col_couponType.Header = AppSettings.resourcemanager.GetString("trType");
            col_offersType.Header = AppSettings.resourcemanager.GetString("trType");
            col_date.Header = AppSettings.resourcemanager.GetString("trDate");
            col_coupon.Header = AppSettings.resourcemanager.GetString("trCoupon");
            col_code.Header = AppSettings.resourcemanager.GetString("trCode");
            col_coupoValue.Header = AppSettings.resourcemanager.GetString("trValue");
            col_offersValue.Header = AppSettings.resourcemanager.GetString("trValue");
            col_couponTotalValue.Header = AppSettings.resourcemanager.GetString("trDiscount");
            col_offersTotalValue.Header = AppSettings.resourcemanager.GetString("trDiscount");
            col_offers.Header = AppSettings.resourcemanager.GetString("trOffer");
            col_item.Header = AppSettings.resourcemanager.GetString("trItem");
            col_price.Header = AppSettings.resourcemanager.GetString("trPrice");


            col_offerCode.Header = AppSettings.resourcemanager.GetString("trCode");
            col_itQuantity.Header = AppSettings.resourcemanager.GetString("trQTR");
            col_total.Header = AppSettings.resourcemanager.GetString("trTotal");

            tt_report.Content = AppSettings.resourcemanager.GetString("trPdf");
            tt_print.Content = AppSettings.resourcemanager.GetString("trPrint");
            tt_excel.Content = AppSettings.resourcemanager.GetString("trExcel");
            tt_count.Content = AppSettings.resourcemanager.GetString("trCount");

        }

        private void fillComboCoupon()
        {
            cb_Coupons.SelectedValuePath = "Copcid";
            cb_Coupons.DisplayMemberPath = "Copname";
            cb_Coupons.ItemsSource = dynamicComboCoupon;
        }

        private void fillComboOffer()
        {
            cb_Coupons.SelectedValuePath = "OofferId";
            cb_Coupons.DisplayMemberPath = "Oname";
            cb_Coupons.ItemsSource = dynamicComboOffer;
        }
        private static void fillDates(DatePicker startDate, DatePicker endDate, TimePicker startTime, TimePicker endTime)
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
        IEnumerable<ItemTransferInvoice> invLst = null;
        private IEnumerable<ItemTransferInvoice> fillList(IEnumerable<ItemTransferInvoice> Invoices, CheckBox chkInvoice, CheckBox chkReturn, CheckBox chkDraft
           , DatePicker startDate, DatePicker endDate, TimePicker startTime, TimePicker endTime)
        {
            fillDates(startDate, endDate, startTime, endTime);
            var result = Invoices.Where(x => (

               ((chkDraft.IsChecked == true ? (x.invType == "sd" || x.invType == "sbd") : false)
                          || (chkReturn.IsChecked == true ? (x.invType == "sb") : false)
                          || (chkInvoice.IsChecked == true ? (x.invType == "s") : false))
                      && (startDate.SelectedDate != null ? x.invDate >= startDate.SelectedDate : true)
                      && (endDate.SelectedDate != null ? x.invDate <= endDate.SelectedDate : true)
                      && (startTime.SelectedTime != null ? x.invDate >= startTime.SelectedTime : true)
                      && (endTime.SelectedTime != null ? x.invDate <= endTime.SelectedTime : true)));

            invLst = result;
            return result;
        }

        IEnumerable<ItemTransferInvoice> invRowChartLst = null;
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

            invRowChartLst = result;
            return result;
        }

        private void fillPieChart(ObservableCollection<int> stackedButton)
        {
            List<string> titles = new List<string>();
            IEnumerable<int> x = null;

            var temp = invLst;

            if (selectedTab == 0)
            {
                titles.Clear();
                temp = temp.Where(j => (selectedcouponId.Count != 0 ? stackedButton.Contains((int)j.CopcId) : true));
                var titleTemp = temp.GroupBy(m => m.Copname);
                titles.AddRange(titleTemp.Select(jj => jj.Key));
                var result = temp.GroupBy(s => s.CopcId).Select(s => new { CopcId = s.Key, count = s.Count() });
                x = result.Select(m => m.count);
            }

            else if (selectedTab == 1)
            {
                titles.Clear();
                temp = temp.Where(j => (selectedOfferId.Count != 0 ? stackedButton.Contains((int)j.OofferId) : true));
                var titleTemp = temp.GroupBy(m => m.Oname);
                titles.AddRange(titleTemp.Select(jj => jj.Key));
                var result = temp.GroupBy(s => s.OofferId).Select(s => new { OofferId = s.Key, count = s.Count() });
                x = result.Select(m => m.count);
            }

            SeriesCollection piechartData = new SeriesCollection();
            int xCount = 0;
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

            var temp = invLst;

            if (selectedTab == 0)
            {
                temp = temp.Where(j => (selectedcouponId.Count != 0 ? stackedButton.Contains((int)j.CopcId) : true));
                var result = temp.GroupBy(s => s.CopcId).Select(s => new
                {
                    CopcId = s.Key,
                    countP = s.Where(m => m.invType == "s").Count(),
                    countPb = s.Where(m => m.invType == "sb").Count(),
                    countD = s.Where(m => m.invType == "sd" || m.invType == "sbd").Count()

                });
                x = result.Select(m => m.countP);
                y = result.Select(m => m.countPb);
                z = result.Select(m => m.countD);
                var tempName = temp.GroupBy(s => s.Copname).Select(s => new
                {
                    uUserName = s.Key
                });
                names.AddRange(tempName.Select(nn => nn.uUserName));
            }

            else if (selectedTab == 1)
            {
                temp = temp.Where(j => (selectedOfferId.Count != 0 ? stackedButton.Contains((int)j.OofferId) : true));
                var result = temp.GroupBy(s => s.OofferId).Select(s => new
                {
                    CopcId = s.Key,
                    countP = s.Where(m => m.invType == "s").Count(),
                    countPb = s.Where(m => m.invType == "sb").Count(),
                    countD = s.Where(m => m.invType == "sd" || m.invType == "sbd").Count()

                });
                x = result.Select(m => m.countP);
                y = result.Select(m => m.countPb);
                z = result.Select(m => m.countD);
                var tempName = temp.GroupBy(s => s.Oname).Select(s => new
                {
                    uUserName = s.Key
                });
                names.AddRange(tempName.Select(nn => nn.uUserName));
            }

            List<string> lable = new List<string>();
            SeriesCollection columnChartData = new SeriesCollection();
            List<int> cP = new List<int>();
            List<int> cPb = new List<int>();
            List<int> cD = new List<int>();
            List<string> titles = new List<string>()
            {
                AppSettings.resourcemanager.GetString("trSales"),
                AppSettings.resourcemanager.GetString("trReturned"),
                AppSettings.resourcemanager.GetString("trDraft")
            };
            int xCount = 0;
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
                for (int i = 0; i < xCount; i++)
                {
                    cPSum = cPSum + x.ToList().Skip(i).FirstOrDefault();
                    cPbSum = cPbSum + y.ToList().Skip(i).FirstOrDefault();
                    cDSum = cDSum + z.ToList().Skip(i).FirstOrDefault();
                }
                if (!((cPSum == 0) && (cPbSum == 0) && (cDSum == 0)))
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

        public void fillEvent()
        {
            if (selectedTab == 0)
            {
                itemTransfers = fillList(coupons, chk_couponInvoice, chk_couponReturn, chk_couponDrafs, dp_couponStartDate, dp_couponEndDate, dt_couponStartTime, dt_couponEndTime).Where(j => (selectedcouponId.Count != 0 ? selectedcouponId.Contains((int)j.CopcId) : true));
                fillRowChartList(coupons, chk_couponInvoice, chk_couponReturn, chk_couponDrafs, dp_couponStartDate, dp_couponEndDate, dt_couponStartTime, dt_couponEndTime).Where(j => (selectedcouponId.Count != 0 ? selectedcouponId.Contains((int)j.CopcId) : true));
            }
            else if (selectedTab == 1)
            {
                itemTransfers = fillList(Offers, chk_couponInvoice, chk_couponReturn, chk_couponDrafs, dp_couponStartDate, dp_couponEndDate, dt_couponStartTime, dt_couponEndTime).Where(j => (selectedOfferId.Count != 0 ? selectedOfferId.Contains((int)j.OofferId) : true));
                fillRowChartList(Offers, chk_couponInvoice, chk_couponReturn, chk_couponDrafs, dp_couponStartDate, dp_couponEndDate, dt_couponStartTime, dt_couponEndTime).Where(j => (selectedOfferId.Count != 0 ? selectedOfferId.Contains((int)j.OofferId) : true));
            }
            dgInvoice.ItemsSource = itemTransfers;
            txt_count.Text = dgInvoice.Items.Count.ToString();

            ObservableCollection<int> selected = new ObservableCollection<int>();
            if (selectedTab == 0)
                selected = selectedcouponId;
            else if (selectedTab == 1)
                selected = selectedOfferId;

            fillPieChart(selected);
            fillColumnChart(selected);
            fillRowChartCoupAndOffer(cb_Coupons, selected);
        }

        #region Functions
        private void hideSatacks()
        {
            stk_tagsCoupons.Visibility = Visibility.Collapsed;
            stk_tagsOffers.Visibility = Visibility.Collapsed;
        }

        public void paint()
        {
            //bdrMain.RenderTransform = Animations.borderAnimation(50, bdrMain, true);

            bdr_coupon.Background = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            bdr_offers.Background = Application.Current.Resources["SecondColor"] as SolidColorBrush;

            path_coupons.Fill = Brushes.White;
            path_offers.Fill = Brushes.White;

        }

        #endregion

        #region Events
        #region tabControl
        private void hidAllColumns()
        {
            col_code.Visibility = Visibility.Hidden;
            col_offerCode.Visibility = Visibility.Hidden;
            col_item.Visibility = Visibility.Hidden;
            col_itQuantity.Visibility = Visibility.Hidden;
            col_total.Visibility = Visibility.Hidden;
            col_coupon.Visibility = Visibility.Hidden;
            col_offers.Visibility = Visibility.Hidden;
            col_coupoValue.Visibility = Visibility.Hidden;
            col_couponType.Visibility = Visibility.Hidden;
            col_offersType.Visibility = Visibility.Hidden;
            col_offersValue.Visibility = Visibility.Hidden;
            col_offersTotalValue.Visibility = Visibility.Hidden;
            col_couponTotalValue.Visibility = Visibility.Hidden;
            col_price.Visibility = Visibility.Hidden;
        }

        private void btn_coupons_Click(object sender, RoutedEventArgs e)
        {//copouns
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), (sender as Button).Tag.ToString());
                MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_Coupons, AppSettings.resourcemanager.GetString("trCoponHint"));
                selectedTab = 0;
                txt_search.Text = "";
                hideSatacks();
                stk_tagsCoupons.Visibility = Visibility.Visible;

                paint();
                ReportsHelp.paintTabControlBorder(grid_tabControl, bdr_coupon);
                path_coupons.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;

                fillComboCoupon();
                fillEvent();

                hidAllColumns();
                col_code.Visibility = Visibility.Visible;
                col_coupon.Visibility = Visibility.Visible;
                col_couponType.Visibility = Visibility.Visible;
                col_coupoValue.Visibility = Visibility.Visible;
                col_couponTotalValue.Visibility = Visibility.Visible;

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void btn_offers_Click(object sender, RoutedEventArgs e)
        {//offers
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                HelpClass.ReportTabTitle(txt_tabTitle, this.Tag.ToString(), (sender as Button).Tag.ToString());
                MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_Coupons, AppSettings.resourcemanager.GetString("trOfferHint"));
                selectedTab = 1;
                txt_search.Text = "";
                hideSatacks();
                stk_tagsOffers.Visibility = Visibility.Visible;

                paint();
                ReportsHelp.paintTabControlBorder(grid_tabControl, bdr_offers);
                path_offers.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;

                fillComboOffer();
                fillEvent();

                hidAllColumns();

                col_offerCode.Visibility = Visibility.Visible;
                col_offers.Visibility = Visibility.Visible;
                col_offersType.Visibility = Visibility.Visible;
                col_offersValue.Visibility = Visibility.Visible;
                col_item.Visibility = Visibility.Visible;
                col_price.Visibility = Visibility.Visible;
                col_itQuantity.Visibility = Visibility.Visible;
                col_offersTotalValue.Visibility = Visibility.Visible;
                col_total.Visibility = Visibility.Visible;




                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion

        #region refreshButton
        private void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {//refresh
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                cb_Coupons.SelectedItem = null;
                chk_couponDrafs.IsChecked = false;
                chk_couponReturn.IsChecked = false;
                chk_couponInvoice.IsChecked = true;
                dp_couponEndDate.SelectedDate = null;
                dp_couponStartDate.SelectedDate = null;
                dt_couponStartTime.SelectedTime = null;
                dt_couponEndTime.SelectedTime = null;
                chk_allCoupon.IsChecked = false;
                cb_Coupons.IsEnabled = true;

                if (selectedTab == 0)
                {
                    for (int i = 0; i < comboCouponTemp.Count; i++)
                    {
                        dynamicComboCoupon.Add(comboCouponTemp.Skip(i).FirstOrDefault());
                    }
                    comboCouponTemp.Clear();
                    stk_tagsCoupons.Children.Clear();
                    selectedcouponId.Clear();
                    fillEvent();
                }

                else if (selectedTab == 1)
                {
                    for (int i = 0; i < comboOfferTemp.Count; i++)
                    {
                        dynamicComboOffer.Add(comboOfferTemp.Skip(i).FirstOrDefault());
                    }
                    comboOfferTemp.Clear();
                    stk_tagsOffers.Children.Clear();
                    selectedOfferId.Clear();
                    fillEvent();
                }
                txt_search.Text = "";

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion

        #region settings
        private void btn_settings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 
                    HelpClass.StartAwait(grid_main);
                List<string> Headers = new List<string>();
                List<string> Headers1 = new List<string>();
                foreach (var item in dgInvoice.Columns)
                {
                    Headers.Add(item.Header.ToString());
                }

                winControlPanel win = new winControlPanel(Headers);

                if (win.ShowDialog() == false)
                {
                    Headers1.Clear();
                    Headers1.AddRange(win.newHeaderResult);
                }
                for (int i = 0; i < Headers1.Count; i++)
                {
                    if (dgInvoice.Columns[i].Header.ToString() == Headers1[i])
                    {
                        dgInvoice.Columns[i].Visibility = Visibility.Visible;
                    }
                    else
                        dgInvoice.Columns[i].Visibility = Visibility.Hidden;
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

        #endregion

        private void Chip_OnDeleteClick(object sender, RoutedEventArgs e)
        {
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                var currentChip = (Chip)sender;
                if (selectedTab == 0)
                {
                    stk_tagsCoupons.Children.Remove(currentChip);
                    var m = comboCouponTemp.Where(j => j.Copcid == (Convert.ToInt32(currentChip.Name.Remove(0, 3))));
                    dynamicComboCoupon.Add(m.FirstOrDefault());
                    selectedcouponId.Remove(Convert.ToInt32(currentChip.Name.Remove(0, 3)));
                    if (selectedcouponId.Count == 0)
                    {
                        cb_Coupons.SelectedItem = null;
                    }
                }
                else if (selectedTab == 1)
                {
                    stk_tagsOffers.Children.Remove(currentChip);
                    var m = comboOfferTemp.Where(j => j.OofferId == (Convert.ToInt32(currentChip.Name.Remove(0, 3))));
                    dynamicComboOffer.Add(m.FirstOrDefault());
                    selectedOfferId.Remove(Convert.ToInt32(currentChip.Name.Remove(0, 3)));
                    if (selectedOfferId.Count == 0)
                    {
                        cb_Coupons.SelectedItem = null;
                    }
                }
                fillEvent();

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void fillEventCall(object sender)
        {
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                fillEvent();

                 
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
            fillEventCall(sender);
        }

        private void dp_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            fillEventCall(sender);
        }

        private void dt_SelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            fillEventCall(sender);
        }

        bool isClicked = true;
        private void chk_allCoupon_Click(object sender, RoutedEventArgs e)
        {//all coupons
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                if (isClicked)
                {
                    cb_Coupons.SelectedItem = null;
                    cb_Coupons.IsEnabled = false;
                    isClicked = false;

                    if (selectedTab == 0)
                    {
                        for (int i = 0; i < comboCouponTemp.Count; i++)
                        {
                            dynamicComboCoupon.Add(comboCouponTemp.Skip(i).FirstOrDefault());
                        }
                        comboCouponTemp.Clear();
                        stk_tagsCoupons.Children.Clear();
                        selectedcouponId.Clear();
                    }
                    else if (selectedTab == 1)
                    {
                        for (int i = 0; i < comboOfferTemp.Count; i++)
                        {
                            dynamicComboOffer.Add(comboOfferTemp.Skip(i).FirstOrDefault());
                        }
                        comboOfferTemp.Clear();
                        stk_tagsOffers.Children.Clear();
                        selectedOfferId.Clear();
                    }
                }
                else
                {
                    cb_Coupons.IsEnabled = true;
                    isClicked = true;
                }

                fillEvent();

                 
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                 
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void cb_Coupons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                if (cb_Coupons.SelectedItem != null)
                {
                    if (selectedTab == 0)
                    {
                        if (stk_tagsCoupons.Children.Count < 5)
                        {
                            selectedCouon = cb_Coupons.SelectedItem as CouponCombo;
                            var b = new MaterialDesignThemes.Wpf.Chip()
                            {
                                Content = selectedCouon.Copname,
                                Name = "btn" + selectedCouon.Copcid.ToString(),
                                IsDeletable = true,
                                Margin = new Thickness(5, 0, 5, 0)
                            };
                            b.DeleteClick += Chip_OnDeleteClick;
                            stk_tagsCoupons.Children.Add(b);
                            comboCouponTemp.Add(selectedCouon);
                            selectedcouponId.Add(selectedCouon.Copcid);
                            dynamicComboCoupon.Remove(selectedCouon);
                        }
                    }
                    else if (selectedTab == 1)
                    {
                        if (stk_tagsOffers.Children.Count < 5)
                        {
                            selectedOffer = cb_Coupons.SelectedItem as OfferCombo;
                            var b = new MaterialDesignThemes.Wpf.Chip()
                            {
                                Content = selectedOffer.Oname,
                                Name = "btn" + selectedOffer.OofferId.ToString(),
                                IsDeletable = true,
                                Margin = new Thickness(5, 0, 5, 0)
                            };
                            b.DeleteClick += Chip_OnDeleteClick;
                            stk_tagsOffers.Children.Add(b);
                            comboOfferTemp.Add(selectedOffer);
                            selectedOfferId.Add(selectedOffer.OofferId);
                            dynamicComboOffer.Remove(selectedOffer);
                        }
                    }
                    fillEvent();
                }
                 
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

        private void fillRowChartCoupAndOffer(ComboBox comboBox, ObservableCollection<int> stackedButton)
        {
            MyAxis.Labels = new List<string>();
            List<string> names = new List<string>();
            IEnumerable<decimal> pTemp = null;

            var temp = invRowChartLst;
            if (selectedTab == 0)
            {
                names.Clear();
                temp = temp.Where(j => (selectedcouponId.Count != 0 ? stackedButton.Contains((int)j.CopcId) : true));
                var result = temp.GroupBy(s => new { s.CopcId }).Select(s => new
                {
                    CopcId = s.FirstOrDefault().CopcId,
                    copName = s.FirstOrDefault().Copname,
                    invId = s.FirstOrDefault().invoiceId,
                    copTotalValue = s.Sum(x => x.couponTotalValue)
                }
                );


                var name = temp.GroupBy(s => s.invoiceId).Select(s => new
                {
                    uUserName = s.FirstOrDefault().Copname
                });
                names.AddRange(name.Select(nn => nn.uUserName));
                pTemp = result.Select(x => (decimal)x.copTotalValue);
            }
            if (selectedTab == 1)
            {
                names.Clear();
                temp = temp.Where(j => (selectedOfferId.Count != 0 ? stackedButton.Contains((int)j.OofferId) : true));
                var result1 = temp.GroupBy(s => new { s.OofferId, s.ITitemUnitId }).Select(s => new
                {
                    offerId = s.FirstOrDefault().OofferId,
                    offerName = s.FirstOrDefault().Oname,
                    itemId = s.FirstOrDefault().ITitemUnitId,
                    offerTotalValue = s.Sum(x => x.offerTotalValue)
                }
             );

                var name = result1.GroupBy(s => s.offerId).Select(s => new
                {
                    uUserName = s.FirstOrDefault().offerName,
                    offerTotalValue = s.Sum(x => x.offerTotalValue)
                });
                names.AddRange(name.Select(nn => nn.uUserName));
                pTemp = name.Select(x => (decimal)x.offerTotalValue);
            }

            SeriesCollection rowChartData = new SeriesCollection();
            List<decimal> purchase = new List<decimal>();
            List<decimal> returns = new List<decimal>();
            List<decimal> sub = new List<decimal>();
            List<string> titles = new List<string>()
            {
                AppSettings.resourcemanager.GetString("trNetSales"),
                AppSettings.resourcemanager.GetString("trTotalReturn"),
                AppSettings.resourcemanager.GetString("trSum"),
            };
            int xCount = 0;
            if (pTemp.Count() <= 6) xCount = pTemp.Count();
            for (int i = 0; i < xCount; i++)
            {
                purchase.Add(pTemp.ToList().Skip(i).FirstOrDefault());
                MyAxis.Labels.Add(names.ToList().Skip(i).FirstOrDefault());
            }

            if (pTemp.Count() > 6)
            {
                decimal purchaseSum = 0;
                for (int i = xCount; i < pTemp.Count(); i++)
                {
                    purchaseSum = purchaseSum + pTemp.ToList().Skip(i).FirstOrDefault();
                }
                if (purchaseSum != 0)
                {
                    purchase.Add(purchaseSum);
                    MyAxis.Labels.Add(AppSettings.resourcemanager.GetString("trOthers"));
                }
            }
            rowChartData.Add(
                  new LineSeries
                  {
                      Values = purchase.AsChartValues(),
                      Title = titles[0]
                  });

            DataContext = this;
            rowChart.Series = rowChartData;
        }

        private void BuildReport()
        {
            List<ReportParameter> paramarr = new List<ReportParameter>();
            List<ItemTransferInvoice> query = new List<ItemTransferInvoice>();

            string addpath = "";

            string firstTitle = "promotion";
            string secondTitle = "";
            string subTitle = "";
            string Title = "";

            bool isArabic = ReportCls.checkLang();
            if (isArabic)
            {
                if (selectedTab == 0)
                {
                    addpath = @"\Reports\StatisticReport\Sale\Promotion\Ar\ArCoupons.rdlc";
                    secondTitle = "coupon";
                    subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);
                }
                else
                {
                    addpath = @"\Reports\StatisticReport\Sale\Promotion\Ar\ArOffers.rdlc";
                    secondTitle = "offers";
                    subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);
                }
            }
            else
            {
                //english
                if (selectedTab == 0)
                {
                    addpath = @"\Reports\StatisticReport\Sale\Promotion\En\EnCoupons.rdlc";
                    secondTitle = "coupon";
                    subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);
                }
                else
                {
                    addpath = @"\Reports\StatisticReport\Sale\Promotion\En\EnOffers.rdlc";
                    secondTitle = "offers";
                    subTitle = clsReports.ReportTabTitle(firstTitle, secondTitle);

                }
            }

            string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);

            ReportCls.checkLang();
            Title = AppSettings.resourcemanagerreport.GetString("trSalesReport") + " / " + subTitle;
            paramarr.Add(new ReportParameter("trTitle", Title));
            clsReports.SalePromoStsReport(itemTransfers, rep, reppath, paramarr);
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
        IEnumerable<ItemTransferInvoice> itemTransfers = null;

        private void Txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {//search
            try
            {
                 
                    HelpClass.StartAwait(grid_main);

                if (selectedTab == 0)
                {
                    itemTransfers = fillList(coupons, chk_couponInvoice, chk_couponReturn, chk_couponDrafs, dp_couponStartDate, dp_couponEndDate, dt_couponStartTime, dt_couponEndTime).Where(j => (selectedcouponId.Count != 0 ? selectedcouponId.Contains((int)j.CopcId) : true));
                    dgInvoice.ItemsSource = itemTransfers
                        .Where(s => (s.Copname.Contains(txt_search.Text) ||

          s.invNumber.Contains(txt_search.Text)
          ));
                }
                else if (selectedTab == 1)
                {
                    itemTransfers = fillList(Offers, chk_couponInvoice, chk_couponReturn, chk_couponDrafs, dp_couponStartDate, dp_couponStartDate, dt_couponStartTime, dt_couponEndTime).Where(j => (selectedOfferId.Count != 0 ? selectedOfferId.Contains((int)j.OofferId) : true));

                    dgInvoice.ItemsSource = itemTransfers
                        .Where(s => (s.Oname.Contains(txt_search.Text) ||
          s.invNumber.Contains(txt_search.Text)
          ));
                }

                txt_count.Text = dgInvoice.Items.Count.ToString();

                 
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
                //    {
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
                //    });
                //t1.Start();
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

    }
}
