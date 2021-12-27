using POS.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
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

namespace POS.View.windows
{
    /// <summary>
    /// Interaction logic for wd_cashTransfer.xaml
    /// </summary>
    public partial class wd_cashTransfer : Window
    {
        public wd_cashTransfer()
        {
            try
            {
                InitializeComponent();
            }
            catch(Exception ex)
            {
                SectionData.ExceptionMessage(ex,this);
            }
        }

        public int invId;
        public decimal invPaid;
        public decimal invTotal;
        public string title;
        CashTransfer cashModel = new CashTransfer();
        IEnumerable<CashTransfer> cashes;
        IEnumerable<CashTransfer> cashesQuery; 
        string searchText = "";
        private  void Window_Loaded(object sender, RoutedEventArgs e)
        {//load
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_main);

                #region translate
                if (MainWindow.lang.Equals("en"))
                {
                    MainWindow.resourcemanager = new ResourceManager("POS.en_file", Assembly.GetExecutingAssembly());
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    MainWindow.resourcemanager = new ResourceManager("POS.ar_file", Assembly.GetExecutingAssembly());
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                }

                translate();
                #endregion

                Txb_search_TextChanged(null, null);

                if (sender != null)
                    SectionData.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_main);
                SectionData.ExceptionMessage(ex, this);
            }
        }

        private void translate()
        {
            txt_accounts.Text = title;
            txt_paid.Text = MainWindow.resourcemanager.GetString("trCashPaid");
            txt_total.Text = MainWindow.resourcemanager.GetString("trOf") ;
            MaterialDesignThemes.Wpf.HintAssist.SetHint(txb_search, MainWindow.resourcemanager.GetString("trSearchHint"));
            dg_accounts.Columns[0].Header = MainWindow.resourcemanager.GetString("trTransferNumberTooltip");
            dg_accounts.Columns[1].Header = MainWindow.resourcemanager.GetString("trDate");
            dg_accounts.Columns[2].Header = MainWindow.resourcemanager.GetString("trPaymentTypeTooltip");
            dg_accounts.Columns[3].Header = MainWindow.resourcemanager.GetString("trCashTooltip");
            btn_pay.Content = MainWindow.resourcemanager.GetString("trPay");
            btn_colse.ToolTip = MainWindow.resourcemanager.GetString("trClose");
        }

        async Task<IEnumerable<CashTransfer>> RefreshCashesList()
        {
            cashes = await cashModel.GetListByInvId(invId);
            return cashes;

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (Exception ex)
            {
                //SectionData.ExceptionMessage(ex, this);
            }
        }

        private async void Txb_search_TextChanged(object sender, TextChangedEventArgs e)
        {//search
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_main);

                if (cashes is null)
                    await RefreshCashesList();

                this.Dispatcher.Invoke(() =>
                {
                    searchText = txb_search.Text.ToLower();
                    cashesQuery = cashes.Where(s => (s.transNum.ToLower().Contains(searchText)
                                                  || s.cash.ToString().ToLower().Contains(searchText)));

                });

                RefreshCashView();

                if (sender != null)
                    SectionData.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_main);
                SectionData.ExceptionMessage(ex, this);
            }
        }

        void RefreshCashView()
        {
            dg_accounts.ItemsSource = cashesQuery;
            tb_paid.Text = invPaid.ToString();
            tb_total.Text = invTotal.ToString(); 
        }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_main);

                if (e.Key == Key.Return)
                {
                    Btn_pay_Click(null, null);
                }
                if (sender != null)
                    SectionData.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_main);
                SectionData.ExceptionMessage(ex, this);
            }
        }

        private void Btn_colse_Click(object sender, RoutedEventArgs e)
        {//close
            this.Close();
        }

        private void Btn_pay_Click(object sender, RoutedEventArgs e)
        {//pay
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_main);

                //
                //enter your code here
                //

                if (sender != null)
                    SectionData.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_main);
                SectionData.ExceptionMessage(ex, this);
            }
        }
    }
}
