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
using System.Windows.Shapes;

namespace Restaurant.View.windows
{
    /// <summary>
    /// Interaction logic for wd_membershipListForReports.xaml
    /// </summary>
    public partial class wd_membershipListForReports : Window
    {
        string _title = "";
        public string membershipType = "";

        public List<CouponInvoice> CouponInvoiceList = new List<CouponInvoice>();
        public List<ItemTransfer> itemsTransferList = new List<ItemTransfer>();
        public List<InvoicesClass> invoiceClassDiscountList = new List<InvoicesClass>();

        public wd_membershipListForReports()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.Key == Key.Return)
                //{
                //    Btn_select_Click(null, null);
                //}
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
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
                #endregion

                await fillDataGrid();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_colse_Click(object sender, RoutedEventArgs e)
        {

        }

        #region methods
        private void translat()
        {
            txt_title.Text = AppSettings.resourcemanager.GetString(_title);
            //dg_transfers.Columns[0].Header = AppSettings.resourcemanager.GetString("trTransferNumberTooltip");
            //dg_transfers.Columns[1].Header = AppSettings.resourcemanager.GetString("trDepositor");
            //dg_transfers.Columns[2].Header = AppSettings.resourcemanager.GetString("trRecepient");
            //dg_transfers.Columns[3].Header = AppSettings.resourcemanager.GetString("trCashTooltip");

        }


        async Task fillDataGrid()
        {
            hideAllColumns();

            if (membershipType == "c")
            {
                dg_memberships.ItemsSource = CouponInvoiceList;

                //view columns
                col_cCode.Visibility = Visibility.Visible;
                col_cName.Visibility = Visibility.Visible;
                col_cTypeValue.Visibility = Visibility.Visible;

                _title = "trCoupons";

                col_coupon.Width = new GridLength(1, GridUnitType.Star);
            }
            else if (membershipType == "o")
            {
                dg_memberships.ItemsSource = itemsTransferList;

                //view columns
                col_oCode.Visibility = Visibility.Visible;
                col_oName.Visibility = Visibility.Visible;
                col_oTypeValue.Visibility = Visibility.Visible;

                _title = "trOffers";

                col_offer.Width = new GridLength(1, GridUnitType.Star);
            }
            else if (membershipType == "i")
            {
                dg_memberships.ItemsSource = invoiceClassDiscountList;

                //view columns
                col_iName.Visibility = Visibility.Visible;
                col_iTypeValue.Visibility = Visibility.Visible;

                _title = "trInvoicesClasses";

                col_invoice.Width = new GridLength(1, GridUnitType.Star);
            }

            translat();
        }

        private void hideAllColumns()
        {
            col_coupon.Width = new GridLength(0);
            col_offer.Width = new GridLength(0);
            col_invoice.Width = new GridLength(0);

            for (int i = 0; i < dg_memberships.Columns.Count-1; i++)
                dg_memberships.Columns[i].Visibility = Visibility.Hidden;


        }
        #endregion
    }
}
