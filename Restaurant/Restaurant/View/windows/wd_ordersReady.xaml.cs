using netoaster;
using Restaurant.Classes;
using Restaurant.Classes.ApiClasses;
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
    /// Interaction logic for wd_ordersReady.xaml
    /// </summary>
    public partial class wd_ordersReady : Window
    {
        public wd_ordersReady()
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

        public string page = "";
        OrderPreparing preparingOrder = new OrderPreparing();
        List<OrderPreparing> orders = new List<OrderPreparing>();
        List<Invoice> invoices = new List<Invoice>();
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
                translat();
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
        private void translat()
        {
            txt_title.Text = AppSettings.resourcemanager.GetString("trOrders");
            col_orders.Header = AppSettings.resourcemanager.GetString("trCharp");
            col_tables.Header = AppSettings.resourcemanager.GetString("trTable");
            col_waiter.Header = AppSettings.resourcemanager.GetString("trWaiter");
            col_invoices.Header = AppSettings.resourcemanager.GetString("trInvoices");
            col_status.Header = AppSettings.resourcemanager.GetString("trStatus");

        }

        async Task fillDataGrid()  
        {
            if (page == "dinningHall")
            {
                #region visible - unvisible columns
                col_orders.Visibility = Visibility.Visible;
                col_tables.Visibility = Visibility.Visible;
                col_waiter.Visibility = Visibility.Visible;

                col_invoices.Visibility = Visibility.Collapsed;
                col_shiping.Visibility = Visibility.Collapsed;
                col_status.Visibility = Visibility.Collapsed;
                #endregion
                orders = await preparingOrder.GetHallOrdersWithStatus(MainWindow.branchLogin.branchId, "Ready", 24);
                dg_orders.ItemsSource = orders;
            }
            else if(page == "takeAway")
            {
                #region visible - unvisible columns
                col_invoices.Visibility = Visibility.Visible;
                col_status.Visibility = Visibility.Visible;
                col_shiping.Visibility = Visibility.Visible;

                col_orders.Visibility = Visibility.Collapsed;
                col_tables.Visibility = Visibility.Collapsed;
                col_waiter.Visibility = Visibility.Collapsed;
                #endregion

                invoices = await preparingOrder.GetOrdersByTypeWithStatus(MainWindow.branchLogin.branchId,"ts" ,24);
                dg_orders.ItemsSource = invoices;
            }
            else if(page == "selfService")
            {
                #region visible - unvisible columns
                col_invoices.Visibility = Visibility.Visible;
                col_tables.Visibility = Visibility.Visible;
                col_status.Visibility = Visibility.Visible;
                col_shiping.Visibility = Visibility.Visible;

                col_orders.Visibility = Visibility.Collapsed;
                col_waiter.Visibility = Visibility.Collapsed;
                #endregion

                invoices = await preparingOrder.GetOrdersByTypeWithStatus(MainWindow.branchLogin.branchId,"ss", 24);
                dg_orders.ItemsSource = invoices;
            }

         
        }
        private void Btn_colse_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch 
            { }
        }

    }
}
