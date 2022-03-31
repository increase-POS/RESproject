using netoaster;
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
        CashTransfer cashModel = new CashTransfer();
        IEnumerable<CashTransfer> cashesQuery;
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
            dg_transfers.Columns[0].Header = AppSettings.resourcemanager.GetString("trOrders");
            dg_transfers.Columns[1].Header = AppSettings.resourcemanager.GetString("trTable");
            dg_transfers.Columns[2].Header = AppSettings.resourcemanager.GetString("trWaiter");

        }

        async Task fillDataGrid()
        {
            //cashesQuery = cashesQuery.Where(c => c.posId == MainWindow.posLogin.posId && c.isConfirm == 0);

            //foreach (var c in cashesQuery)
            //{
            //    if (c.transType.Equals("p"))
            //    {
            //        string s = c.posName;
            //        c.posName = c.pos2Name;
            //        c.pos2Name = s;
            //    }
            //}

            //dg_transfers.ItemsSource = cashesQuery;
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
