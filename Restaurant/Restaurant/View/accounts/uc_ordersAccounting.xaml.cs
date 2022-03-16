using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using netoaster;
using Restaurant.Classes;
using Restaurant.View.windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;

namespace Restaurant.View.accounts
{
    /// <summary>
    /// Interaction logic for uc_ordersAccounting.xaml
    /// </summary>
    public partial class uc_ordersAccounting : UserControl
    {
        public uc_ordersAccounting()
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
        private static uc_ordersAccounting _instance;
        public static uc_ordersAccounting Instance
        {
            get
            {
                //if (_instance == null)
                if(_instance is null)
                    _instance = new uc_ordersAccounting();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        string createPermission = "ordersAccounting_create";
        string reportsPermission = "ordersAccounting_reports";
        string BranchesPermission = "ordersAccounting_allBranches";
        CashTransfer cashModel = new CashTransfer();
        Invoice invoiceModel = new Invoice();
        Branch branchModel = new Branch();
        CashTransfer cashtrans = new CashTransfer();
        Invoice invoice = new Invoice();

        //Bonds bondModel = new Bonds();
        Card cardModel = new Card();
        Agent agentModel = new Agent();
        User userModel = new User();
        Pos posModel = new Pos();
        IEnumerable<Agent> agents;
        IEnumerable<Agent> customers;
        IEnumerable<User> users;
        IEnumerable<Card> cards;
        //IEnumerable<CashTransfer> cashesQuery;
        //IEnumerable<CashTransfer> cashesQueryExcel;
        IEnumerable<Invoice> invoiceQuery;
        IEnumerable<Invoice> invoiceQueryExcel;
        //IEnumerable<CashTransfer> cashes;
        IEnumerable<Invoice> invoices;
        IEnumerable<Branch> branches;
        int agentId, userId;
        string searchText = "";

        static private int _SelectedCard = -1;

        public static List<string> requiredControlList;
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }
        private void Btn_confirm_Click(object sender, RoutedEventArgs e)
        {//confirm

        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {//load
            //try
            //{
            //    HelpClass.StartAwait(grid_main);

                requiredControlList = new List<string> { "cash", "paymentProcessType" };

                #region fill branch combo1
                try
                {
                    branches = await branchModel.GetBranchesActive("b");
                    cb_branch.ItemsSource = branches;
                    cb_branch.DisplayMemberPath = "name";
                    cb_branch.SelectedValuePath = "branchId";
                    cb_branch.SelectedValue = MainWindow.branchLogin.branchId;
                    if (FillCombo.groupObject.HasPermissionAction(BranchesPermission, FillCombo.groupObjects, "one"))
                        cb_branch.IsEnabled = true;
                    else
                        cb_branch.IsEnabled = false;
                }
                catch { }
                #endregion

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

                dp_searchEndDate.SelectedDate = DateTime.Now;
                dp_searchStartDate.SelectedDate = DateTime.Now;

                dp_searchStartDate.SelectedDateChanged += this.dp_SelectedStartDateChanged;
                dp_searchEndDate.SelectedDateChanged += this.dp_SelectedEndDateChanged;

                btn_image.IsEnabled = false;

                btn_save.IsEnabled = false;

                #region fill process type
                var typelist = new[] {
                new { Text = AppSettings.resourcemanager.GetString("trCredit")    , Value = "balance" },
                new { Text = AppSettings.resourcemanager.GetString("trCash")       , Value = "cash" },
                //new { Text = AppSettings.resourcemanager.GetString("trDocument")   , Value = "doc" },
                new { Text = AppSettings.resourcemanager.GetString("trCheque")     , Value = "cheque" },
                new { Text = AppSettings.resourcemanager.GetString("trAnotherPaymentMethods") , Value = "card" }
                 };
                cb_paymentProcessType.DisplayMemberPath = "Text";
                cb_paymentProcessType.SelectedValuePath = "Value";
                cb_paymentProcessType.ItemsSource = typelist;
            #endregion

                #region fill card combo
                try
                {
                    cards = await cardModel.GetAll();
                    InitializeCardsPic(cards);
                }
                catch { }
                #endregion

                #region fill agent combo
                List<Agent> agents = new List<Agent>();
                    List<Agent> customers = new List<Agent>();
                    try
                    {
                        customers = await agentModel.GetAgentsActive("c");
                        agents = await agentModel.GetAgentsActive("v");
                        agents.AddRange(customers);

                        cb_customer.ItemsSource = customers;
                        cb_customer.DisplayMemberPath = "name";
                        cb_customer.SelectedValuePath = "agentId";
                        cb_customer.SelectedIndex = -1;
                    }
                    catch { }
                    #endregion

                #region fill salesman combo
                try
                {
                    users = await userModel.GetUsersActive();
                    cb_salesMan.ItemsSource = users;
                    cb_salesMan.DisplayMemberPath = "username";
                    cb_salesMan.SelectedValuePath = "userId";
                    cb_salesMan.SelectedIndex = -1;
                }
                catch { }
                #endregion

                #region fill status combo
                var statuslist = new[] {
                new { Text = AppSettings.resourcemanager.GetString("trDelivered")  , Value = "rc" },
                new { Text = AppSettings.resourcemanager.GetString("trInDelivery")   , Value = "tr" }
                 };
                cb_state.DisplayMemberPath = "Text";
                cb_state.SelectedValuePath = "Value";
                cb_state.ItemsSource = statuslist;
                #endregion

                await RefreshInvoiceList();
                await Search();
                Clear();

            //    HelpClass.EndAwait(grid_main);
            //}
            //catch (Exception ex)
            //{
            //    HelpClass.EndAwait(grid_main);
            //    HelpClass.ExceptionMessage(ex, this);
            //}
        }

        void InitializeCardsPic(IEnumerable<Card> cards)
        {
            #region cardImageLoad
            dkp_cards.Children.Clear();
            int userCount = 0;
            foreach (var item in cards)
            {
                #region Button
                Button button = new Button();
                button.DataContext = item.name;
                button.Tag = item.cardId;
                button.Padding = new Thickness(0, 0, 0, 0);
                button.Margin = new Thickness(2.5, 5, 2.5, 5);
                button.Background = null;
                button.BorderBrush = null;
                button.Height = 35;
                button.Width = 35;
                button.Click += card_Click;
                //Grid.SetColumn(button, 4);
                #region grid
                Grid grid = new Grid();
                #region 
                Ellipse ellipse = new Ellipse();
                //ellipse.Margin = new Thickness(-5, 0, -5, 0);
                ellipse.StrokeThickness = 1;
                ellipse.Stroke = Application.Current.Resources["MainColorOrange"] as SolidColorBrush;
                ellipse.Height = 35;
                ellipse.Width = 35;
                ellipse.FlowDirection = FlowDirection.LeftToRight;
                ellipse.ToolTip = item.name;
                userImageLoad(ellipse, item.image);
                Grid.SetColumn(ellipse, userCount);
                grid.Children.Add(ellipse);
                #endregion
                #endregion
                button.Content = grid;
                #endregion
                dkp_cards.Children.Add(button);

            }
            #endregion
        }
        void card_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            _SelectedCard = int.Parse(button.Tag.ToString());
            txt_card.Text = button.DataContext.ToString();
            //MessageBox.Show("Hey you Click me! I'm Card: " + _SelectedCard);
        }
        ImageBrush brush = new ImageBrush();
        async void userImageLoad(Ellipse ellipse, string image)
        {
            try
            {
                if (!string.IsNullOrEmpty(image))
                {
                    clearImg(ellipse);

                    byte[] imageBuffer = await cardModel.downloadImage(image); // read this as BLOB from your DB
                    var bitmapImage = new BitmapImage();
                    using (var memoryStream = new System.IO.MemoryStream(imageBuffer))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                    }
                    ellipse.Fill = new ImageBrush(bitmapImage);
                }
                else
                {
                    clearImg(ellipse);
                }
            }
            catch
            {
                clearImg(ellipse);
            }
        }
        private void clearImg(Ellipse ellipse)
        {
            Uri resourceUri = new Uri("pic/no-image-icon-90x90.png", UriKind.Relative);
            StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
            BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
            brush.ImageSource = temp;
            ellipse.Fill = brush;
        }
        private async void dp_SelectedEndDateChanged(object sender, SelectionChangedEventArgs e)
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
        private async void dp_SelectedStartDateChanged(object sender, SelectionChangedEventArgs e)
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
            txt_title.Text = AppSettings.resourcemanager.GetString("trOrders");
            txt_baseInformation.Text = AppSettings.resourcemanager.GetString("trTransaferDetails");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_branch, AppSettings.resourcemanager.GetString("trBranchHint"));

            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_invoiceNum, AppSettings.resourcemanager.GetString("trInvoiceNumberHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_search, AppSettings.resourcemanager.GetString("trSearchHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_paymentProcessType, AppSettings.resourcemanager.GetString("trPaymentTypeHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_docNum, AppSettings.resourcemanager.GetString("trDocNumHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_docDate, AppSettings.resourcemanager.GetString("trDocDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_docNumCheque, AppSettings.resourcemanager.GetString("trDocNumHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_docNumCard, AppSettings.resourcemanager.GetString("trProcessNumHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_docDateCheque, AppSettings.resourcemanager.GetString("trDocDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_cash, AppSettings.resourcemanager.GetString("trCashHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_notes, AppSettings.resourcemanager.GetString("trNoteHint"));
            //MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_card, AppSettings.resourcemanager.GetString("trCardHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_salesMan, AppSettings.resourcemanager.GetString("trSalesManHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_customer, AppSettings.resourcemanager.GetString("trCustomerHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_searchStartDate, AppSettings.resourcemanager.GetString("trSartDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_searchEndDate, AppSettings.resourcemanager.GetString("trEndDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_state, AppSettings.resourcemanager.GetString("trStateHint"));

            dg_orderAccounts.Columns[0].Header = AppSettings.resourcemanager.GetString("trInvoiceNumber");
            dg_orderAccounts.Columns[1].Header = AppSettings.resourcemanager.GetString("trSalesMan");
            dg_orderAccounts.Columns[2].Header = AppSettings.resourcemanager.GetString("trCustomer");
            dg_orderAccounts.Columns[3].Header = AppSettings.resourcemanager.GetString("trDate");
            dg_orderAccounts.Columns[4].Header = AppSettings.resourcemanager.GetString("trCashTooltip");
            dg_orderAccounts.Columns[5].Header = AppSettings.resourcemanager.GetString("trState");
           
            tt_clear.Content = AppSettings.resourcemanager.GetString("trClear");
            tt_refresh.Content = AppSettings.resourcemanager.GetString("trRefresh");
            tt_report.Content = AppSettings.resourcemanager.GetString("trPdf");
            tt_print.Content = AppSettings.resourcemanager.GetString("trPrint");
            tt_excel.Content = AppSettings.resourcemanager.GetString("trExcel");
            tt_count.Content = AppSettings.resourcemanager.GetString("trCount");
            btn_save.Content = AppSettings.resourcemanager.GetString("trSave");

            txt_image.Text = AppSettings.resourcemanager.GetString("trImage");
            txt_preview.Text = AppSettings.resourcemanager.GetString("trPreview");
            txt_print_pay.Text = AppSettings.resourcemanager.GetString("trPrint");
            txt_pdf.Text = AppSettings.resourcemanager.GetString("trPdfBtn");
        }
        private void Dg_orderAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//selection
            //try
            //{
            //    HelpClass.StartAwait(grid_main);

                #region clear validate
                //HelpClass.clearComboBoxValidate(cb_paymentProcessType, p_errorpayment_processType);
                //HelpClass.clearComboBoxValidate(cb_card, p_errorpayment_processType);
                //TextBox tbDocDate = (TextBox)dp_docDate.Template.FindName("PART_TextBox", dp_docDate);
                //HelpClass.clearValidate(tb_docNum, p_error_docNum);
                //HelpClass.clearValidate(tb_cash, p_error_cash);
                #endregion

                if (dg_orderAccounts.SelectedIndex != -1)
                {
                    invoice = dg_orderAccounts.SelectedItem as Invoice;
                    this.DataContext = cashtrans;

                    if (invoice != null)
                    {
                        tb_cash.IsEnabled = true;

                        btn_image.IsEnabled = true;

                        //tb_invoiceNum.Text = invoice.invNumber;

                        agentId = invoice.agentId.Value;

                        userId = invoice.shipUserId.Value;

                        //tb_cash.Text = HelpClass.DecTostring(cashtrans.cash);

                        if (cb_paymentProcessType.SelectedIndex == 0)
                        {
                            tb_cash.Text = invoice.deserved.ToString();
                            tb_cash.IsEnabled = false;
                        }
                        else
                        {
                            tb_cash.IsEnabled = true;
                            tb_cash.Clear();
                        }

                        if (invoice.status == "rc")
                        {
                            btn_save.IsEnabled = false;
                            tb_cash.IsEnabled = false;
                            cb_paymentProcessType.IsEnabled = false;
                            tb_notes.IsEnabled = false;
                            //HelpClass.clearValidate(tb_cash, p_error_cash);
                        }
                        else
                        {
                            btn_save.IsEnabled = true;
                            tb_cash.IsEnabled = true;
                            cb_paymentProcessType.IsEnabled = true;
                            tb_notes.IsEnabled = true;
                        }
                    }
                    else
                    {
                        btn_save.IsEnabled = false;
                        btn_image.IsEnabled = false;
                    }
                }
            //    HelpClass.EndAwait(grid_main);
            //}
            //catch (Exception ex)
            //{
            //    HelpClass.EndAwait(grid_main);
            //    HelpClass.ExceptionMessage(ex, this);
            //}
        }

        async Task Search()
        {
            try
            {
                if (invoices is null)
                    await RefreshInvoiceList();

                if (chb_all.IsChecked == false)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        searchText = tb_search.Text.ToLower();
                        invoiceQuery = invoices.Where(s => (s.invNumber.ToLower().Contains(searchText)
                        || s.shipUserName.ToLower().Contains(searchText)
                        || s.agentName.ToLower().Contains(searchText)
                        || s.totalNet.ToString().ToLower().Contains(searchText)
                        || s.status.ToLower().Contains(searchText)
                        )
                        && s.updateDate.Value.Date >= dp_searchStartDate.SelectedDate.Value.Date
                        && s.updateDate.Value.Date <= dp_searchEndDate.SelectedDate.Value.Date
                        );

                    });
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        searchText = tb_search.Text.ToLower();
                        invoiceQuery = invoices.Where(s => (s.invNumber.ToLower().Contains(searchText)
                        || s.shipUserName.ToLower().Contains(searchText)
                        || s.agentName.ToLower().Contains(searchText)
                        || s.totalNet.ToString().ToLower().Contains(searchText)
                        || s.status.ToLower().Contains(searchText)
                        )
                        );

                    });
                }

                invoiceQueryExcel = invoiceQuery.ToList();
                RefreshInvoiceView();
            }
            catch { }

        }
        private async void Tb_search_TextChanged(object sender, TextChangedEventArgs e)
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
        /*
        private async Task<int> saveBond(string num, decimal ammount, Nullable<DateTime> date, string type)
        {
            Bonds bond = new Bonds();
            bond.number = num;
            bond.amount = ammount;
            bond.deserveDate = date;
            bond.type = type;
            bond.isRecieved = 0;
            bond.createUserId = MainWindow.userID.Value;
            int s = await bondModel.Save(bond);
            return s;
        }
        */
        private async Task calcBalance(decimal ammount)
        {
            int s = 0;
            //increase pos balance
            Pos pos = await posModel.getById(MainWindow.posLogin.posId);
            pos.balance += ammount;

            s = await pos.save(pos);
        }
        private void Btn_update_Click(object sender, RoutedEventArgs e)
        {//update

        }
        private void Btn_delete_Click(object sender, RoutedEventArgs e)
        {//delete

        }
        //void Clear()
        //{
        //    cb_paymentProcessType.IsEnabled = true;
        //    cb_card.IsEnabled = true;
        //    tb_docNum.IsEnabled = true;
        //    dp_docDate.IsEnabled = true;
        //    tb_docNumCheque.IsEnabled = true;
        //    tb_docNumCard.IsEnabled = true;
        //    dp_docDateCheque.IsEnabled = true;
        //    tb_cash.IsEnabled = true;
        //    tb_notes.IsEnabled = true;

        //    btn_image.IsEnabled = false;
        //    /////////////////////////
        //    ///
        //    //if (grid_doc.IsVisible)
        //    //{
        //    //    TextBox tbDocDate = (TextBox)dp_docDate.Template.FindName("PART_TextBox", dp_docDate);
        //    //    HelpClass.clearValidate(tbDocDate, p_error_docDate);
        //    //    dp_docDate.SelectedDate = null;
        //    //    tb_docNum.Clear();
        //    //    HelpClass.clearValidate(tb_docNum, p_error_docNum);
        //    //}
        //    //if (grid_cheque.IsVisible)
        //    //{
        //    //    tb_docNumCheque.Clear();
        //    //    dp_docDateCheque.SelectedDate = null;
        //    //}
        //    cb_card.Visibility = Visibility.Collapsed;
        //    cb_paymentProcessType.SelectedIndex = -1;
        //    //tb_cash.Clear();
        //    //tb_notes.Clear();
        //    //tb_docNumCard.Clear();
        //    //tb_docNum.Clear();
        //    //tb_docNumCheque.Clear();
        //    //tb_invoiceNum.Text = "";
        //    tb_cash.IsReadOnly = false;
        //    grid_doc.Visibility = Visibility.Collapsed;
        //    tb_docNumCard.Visibility = Visibility.Collapsed;
        //    grid_cheque.Visibility = Visibility.Collapsed;
        //    //HelpClass.clearValidate(tb_cash, p_error_cash);
        //    //HelpClass.clearComboBoxValidate(cb_paymentProcessType, p_errorpayment_processType);
        //    //HelpClass.clearComboBoxValidate(cb_card, p_error_card);
        //    //HelpClass.clearValidate(tb_docNumCard, p_error_docCard);
        //    //HelpClass.clearValidate(tb_docNum, p_error_docNum);
        //    //HelpClass.clearValidate(tb_docNum, p_error_docNum);
        //    //HelpClass.clearValidate(tb_docNumCheque, p_error_docNumCheque);
        //}
        private void Btn_clear_Click(object sender, RoutedEventArgs e)
        {//clear
         //try
         //{

            HelpClass.StartAwait(grid_main);

            Clear();

            HelpClass.EndAwait(grid_main);
            //}
            //catch (Exception ex)
            //{
            //    HelpClass.EndAwait(grid_main);
            //    HelpClass.ExceptionMessage(ex, this);
            //}
        }

        private void Btn_exportToExcel_Click(object sender, RoutedEventArgs e)
        {//excel
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);
                if (FillCombo.groupObject.HasPermissionAction(reportsPermission, FillCombo.groupObjects, "one"))
                {
                    #region
                    //Thread t1 = new Thread(() =>
                    //{
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
                    //t1.Start();
                    #endregion
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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
        void FN_ExportToExcel()
        {
            var QueryExcel = invoiceQuery.AsEnumerable().Select(x => new
            {
                InvoiceNumber = x.invNumber,
                SalesMan = x.shipUserName,
                Customer = x.agentName,
                Cash = x.totalNet,
                Status = x.status
            });
            var DTForExcel = QueryExcel.ToDataTable();
            DTForExcel.Columns[0].Caption = AppSettings.resourcemanager.GetString("trInvoiceNumber");
            DTForExcel.Columns[1].Caption = AppSettings.resourcemanager.GetString("trSalesMan");
            DTForExcel.Columns[2].Caption = AppSettings.resourcemanager.GetString("trCustomer");
            DTForExcel.Columns[3].Caption = AppSettings.resourcemanager.GetString("trCashTooltip");
            DTForExcel.Columns[4].Caption = AppSettings.resourcemanager.GetString("trState");

            ExportToExcel.Export(DTForExcel);

        }
        private void Btn_image_Click(object sender, RoutedEventArgs e)
        {//image
            try
            {
                if (FillCombo.groupObject.HasPermissionAction(reportsPermission, FillCombo.groupObjects, "one"))
                {
                    if (cashtrans != null || cashtrans.cashTransId != 0)
                    {
                        wd_uploadImage w = new wd_uploadImage();

                        w.tableName = "invoices";
                        w.tableId = invoice.invoiceId;
                        w.docNum = invoice.invNumber;
                    // w.ShowInTaskbar = false;
                        w.ShowDialog();
                    }
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {//refresh
            try
            {
                HelpClass.StartAwait(grid_main);

                searchText = "";
                tb_search.Text = "";
                await RefreshInvoiceList();
                await Search();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        async Task<IEnumerable<Invoice>> RefreshInvoiceList()
        {
            invoices = await invoiceModel.getOrdersForPay(Convert.ToInt32(cb_branch.SelectedValue));
            return invoices;

        }
        void RefreshInvoiceView()
        {
            dg_orderAccounts.ItemsSource = invoiceQuery;
            txt_count.Text = invoiceQuery.Count().ToString();
        }
        private void Tb_validateEmptyLostFocus(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    string name = sender.GetType().Name;
            //    validateEmpty(name, sender);
            //}
            //catch (Exception ex)
            //{
            //    HelpClass.ExceptionMessage(ex, this);
            //}

            try
            {
                HelpClass.validate(requiredControlList, this);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void validateEmpty(string name, object sender)
        {
           /*
            if (name == "TextBox")
            {
                if ((sender as TextBox).Name == "tb_cash")
                    HelpClass.validateEmptyTextBox((TextBox)sender, p_error_cash, tt_errorCash, "trEmptyCashToolTip");
                else if ((sender as TextBox).Name == "tb_docNum")
                    HelpClass.validateEmptyTextBox((TextBox)sender, p_error_docNum, tt_errorDocNum, "trEmptyDocNumToolTip");
                else if ((sender as TextBox).Name == "tb_docNumCheque")
                    HelpClass.validateEmptyTextBox((TextBox)sender, p_error_docNumCheque, tt_errorDocNumCheque, "trEmptyDocNumToolTip");
                else if ((sender as TextBox).Name == "tb_docNumCard")
                    HelpClass.validateEmptyTextBox((TextBox)sender, p_error_docCard, tt_errorDocCard, "trEmptyProcessNumToolTip");
            }
            else if (name == "ComboBox")
            {
                if ((sender as ComboBox).Name == "cb_paymentProcessType")
                    HelpClass.validateEmptyComboBox((ComboBox)sender, p_errorpayment_processType, tt_errorpaymentProcessType, "trErrorEmptyPaymentTypeToolTip");
                else if ((sender as ComboBox).Name == "cb_card")
                    HelpClass.validateEmptyComboBox((ComboBox)sender, p_error_card, tt_errorCard, "trEmptyCardTooltip");
            }
            else if (name == "DatePicker")
            {
                if ((sender as DatePicker).Name == "dp_docDate")
                    HelpClass.validateEmptyDatePicker((DatePicker)sender, p_error_docDate, tt_errorDocDate, "trEmptyDocDateToolTip");
                if ((sender as DatePicker).Name == "dp_docDateCheque")
                    HelpClass.validateEmptyDatePicker((DatePicker)sender, p_error_docDateCheque, tt_errorDocDateCheque, "trEmptyDocDateToolTip");
            }
            */
        }
        private void PreventSpaces(object sender, KeyEventArgs e)
        {
            e.Handled = e.Key == Key.Space;
        }
        private void Tb_docNum_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {//only int
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void Tb_cash_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {//only decimal
            var regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                e.Handled = false;
            else
                e.Handled = true;
        }
        private void Tb_validateEmptyTextChange(object sender, TextChangedEventArgs e)
        {
            //try
            //{
            //    string name = sender.GetType().Name;
            //    validateEmpty(name, sender);
            //    var txb = sender as TextBox;
            //    if ((sender as TextBox).Name == "tb_cash")
            //        HelpClass.InputJustNumber(ref txb);
            //}
            //catch (Exception ex)
            //{
            //    HelpClass.ExceptionMessage(ex, this);
            //}

            try
            {
                HelpClass.validate(requiredControlList, this);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        string processType = "";
        private void Cb_paymentProcessType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//type selection
            try
            {
                HelpClass.StartAwait(grid_main);
                
                switch (cb_paymentProcessType.SelectedIndex)
                {
                    case 0://balance
                        grid_doc.Visibility = Visibility.Collapsed;
                        grid_cheque.Visibility = Visibility.Collapsed;
                        gd_card.Visibility = Visibility.Collapsed;
                        bdr_docNumCard.Visibility = Visibility.Collapsed;
                        HelpClass.clearValidate(p_error_docNumCard);
                        HelpClass.clearValidate(p_error_docNum);
                        HelpClass.clearValidate(p_error_docNum);
                        HelpClass.clearValidate(p_error_card);
                        if (grid_doc.IsVisible)
                            HelpClass.clearValidate(p_error_docDate);
                        if (grid_cheque.IsVisible)
                            HelpClass.clearValidate(p_error_docNumCheque);
                        if (invoice.invoiceId != 0)
                        {
                            tb_cash.Text = invoice.deserved.ToString();
                            tb_cash.IsEnabled = false;
                        }
                        processType = "1";
                        requiredControlList = new List<string> { "cash", "paymentProcessType" };
                        break;

                    case 1://cash
                        grid_doc.Visibility = Visibility.Collapsed;
                        grid_cheque.Visibility = Visibility.Collapsed;
                        gd_card.Visibility = Visibility.Collapsed;
                        bdr_docNumCard.Visibility = Visibility.Collapsed;
                        HelpClass.clearValidate(p_error_docNumCard);
                        HelpClass.clearValidate(p_error_docNum);
                        HelpClass.clearValidate(p_error_docNum);
                        HelpClass.clearValidate(p_error_card);

                        if (grid_doc.IsVisible)
                            HelpClass.clearValidate(p_error_docDate);

                        if (grid_cheque.IsVisible)
                            HelpClass.clearValidate(p_error_docNumCheque);

                        tb_cash.IsEnabled = true;
                        tb_cash.Clear();
                        HelpClass.clearValidate(p_error_cash);
                        processType = "0";
                        requiredControlList = new List<string> { "cash", "paymentProcessType" };
                        break;

                    //case 2://doc
                    //    grid_doc.Visibility = Visibility.Visible;
                    //    grid_cheque.Visibility = Visibility.Collapsed;
                    //    cb_card.Visibility = Visibility.Collapsed;
                    //    tb_docNumCard.Visibility = Visibility.Collapsed;
                    //    HelpClass.clearValidate(tb_docNumCard, p_error_docCard);
                    //    HelpClass.clearValidate(tb_docNumCheque, p_error_docNum);
                    //    HelpClass.clearComboBoxValidate(cb_card, p_error_card);
                    //    if (grid_cheque.IsVisible)
                    //    {
                    //        TextBox dpDateCheque = (TextBox)dp_docDateCheque.Template.FindName("PART_TextBox", dp_docDateCheque);
                    //        HelpClass.clearValidate(dpDateCheque, p_error_docNumCheque);
                    //    }
                    //    tb_cash.IsEnabled = true;
                    //    tb_cash.Clear();
                    //    HelpClass.clearValidate(tb_cash, p_error_cash);
                    //    processType = "0";
                    //    break;

                    //case 3://cheque
                    case 2://cheque
                        grid_doc.Visibility = Visibility.Collapsed;
                        grid_cheque.Visibility = Visibility.Visible;
                        gd_card.Visibility = Visibility.Collapsed;
                        bdr_docNumCard.Visibility = Visibility.Collapsed;
                        HelpClass.clearValidate(p_error_docNumCard);
                        HelpClass.clearValidate(p_error_docNum);
                        HelpClass.clearValidate(p_error_card);

                        if (grid_doc.IsVisible)
                            HelpClass.clearValidate(p_error_docDate);

                        tb_cash.IsEnabled = true;
                        tb_cash.Clear();
                        HelpClass.clearValidate(p_error_cash);
                        processType = "0";
                        requiredControlList = new List<string> { "cash", "paymentProcessType" , "docNumCheque" };
                        break;

                    //case 4://card
                    case 3://card
                        grid_doc.Visibility = Visibility.Collapsed;
                        grid_cheque.Visibility = Visibility.Collapsed;
                        gd_card.Visibility = Visibility.Visible;
                        bdr_docNumCard.Visibility = Visibility.Visible;
                        HelpClass.clearValidate(p_error_docNum);
                        HelpClass.clearValidate(p_error_docNum);
                        HelpClass.clearValidate(p_error_card);

                        if (grid_doc.IsVisible)
                            HelpClass.clearValidate(p_error_docDate);

                        if (grid_cheque.IsVisible)
                            HelpClass.clearValidate(p_error_docNumCheque);

                        tb_cash.IsEnabled = true;
                        tb_cash.Clear();
                        HelpClass.clearValidate(p_error_cash);
                        processType = "0";
                        requiredControlList = new List<string> { "cash", "paymentProcessType", "docNumCard" };
                        break;
                }

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async Task fillCustomers()
        {
            agents = await agentModel.GetAgentsActive("c");
            cb_customer.ItemsSource = agents;
            cb_customer.DisplayMemberPath = "name";
            cb_customer.SelectedValuePath = "agentId";
            cb_salesMan.SelectedIndex = -1;
        }
        private async Task fillUsers()
        {
            users = await userModel.GetUsersActive();

            cb_salesMan.ItemsSource = users;
            cb_salesMan.DisplayMemberPath = "username";
            cb_salesMan.SelectedValuePath = "userId";
            cb_salesMan.SelectedIndex = -1;
        }
        private void Btn_printInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (FillCombo.groupObject.HasPermissionAction(reportsPermission, FillCombo.groupObjects, "one"))
            {

            }
            else
                Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
        }
        private void Btn_invoices_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Cb_salesMan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//select salesman
            try
            {
                HelpClass.StartAwait(grid_main);

                invoiceQuery = invoiceQuery.Where(u => u.shipUserId == Convert.ToInt32(cb_salesMan.SelectedValue));
                invoiceQueryExcel = invoiceQuery;
                RefreshInvoiceView();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Cb_customer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//select agent
            try
            {
                HelpClass.StartAwait(grid_main);

                invoiceQuery = invoiceQuery.Where(c => c.agentId == Convert.ToInt32(cb_customer.SelectedValue));
                invoiceQueryExcel = invoiceQuery;
                RefreshInvoiceView();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Cb_state_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//select state
            try
            {
                HelpClass.StartAwait(grid_main);

                invoiceQuery = invoiceQuery.Where(s => s.status == cb_state.SelectedValue.ToString());
                invoiceQueryExcel = invoiceQuery;
                RefreshInvoiceView();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Cb_branch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//select branch
        }
        private async void Btn_save_Click(object sender, RoutedEventArgs e)
        {//save
            try
            {
                HelpClass.StartAwait(grid_main);

                if (FillCombo.groupObject.HasPermissionAction(createPermission, FillCombo.groupObjects, "one"))
                {
                    #region validate
                    //chk empty cash
                    bool acceptedAmmount = true;

                    //if (tb_cash.Text.Equals(""))
                    //    HelpClass.validateEmptyTextBox(tb_cash, p_error_cash, tt_errorCash, "trEmptyCashToolTip");
                    //else
                    if (!tb_cash.Text.Equals(""))
                    {
                        decimal ammount = decimal.Parse(tb_cash.Text);

                        if (ammount == 0)
                        {
                            acceptedAmmount = false;
                            HelpClass.SetValidate(p_error_cash,  "trZeroAmmount");
                        }
                        else if (ammount > invoice.deserved)
                        {
                            acceptedAmmount = false;
                            HelpClass.SetValidate(p_error_cash, "trGreaterAmmount");
                        }
                    }
                    //chk empty doc num
                    //if (grid_doc.IsVisible)
                    //{
                    //    TextBox dpDate = (TextBox)dp_docDate.Template.FindName("PART_TextBox", dp_docDate);
                    //    HelpClass.validateEmptyTextBox(tb_docNum, p_error_docNum, tt_errorDocNum, "trEmptyDocNumToolTip");
                    //    HelpClass.validateEmptyTextBox(dpDate, p_error_docDate, tt_errorDocDate, "trEmptyDocDateToolTip");
                    //}
                    //else
                    //{
                    //}

                    ////chk empty cheque num
                    //if (grid_cheque.IsVisible)
                    //{
                    //    HelpClass.validateEmptyTextBox(tb_docNumCheque, p_error_docNumCheque, tt_errorDocNumCheque, "trEmptyDocNumToolTip");
                    //}
                    //else
                    //{
                    //}
                    ////chk empty process num
                    //if (tb_docNumCard.IsVisible)
                    //{
                    //    HelpClass.validateEmptyTextBox(tb_docNumCard, p_error_docCard, tt_errorDocCard, "trEmptyProcessNumToolTip");
                    //}
                    //else
                    //{
                    //    HelpClass.clearValidate(tb_docNumCard, p_error_docCard);
                    //}
                    ////chk empty payment type
                    //HelpClass.validateEmptyComboBox(cb_paymentProcessType, p_errorpayment_processType, tt_errorpaymentProcessType, "trErrorEmptyPaymentTypeToolTip");

                    ////chk empty card 
                    //if (cb_card.IsVisible)
                    //    HelpClass.validateEmptyComboBox(cb_card, p_error_card, tt_errorCard, "trEmptyCardTooltip");
                    //else
                    //    HelpClass.clearComboBoxValidate(cb_card, p_error_card);

                    #endregion

                    #region save

                    //if ((!tb_cash.Text.Equals("")) && (!cb_paymentProcessType.Text.Equals("")) &&
                    // (((grid_cheque.IsVisible) && (!tb_docNumCheque.Text.Equals(""))) || (!grid_cheque.IsVisible)) &&
                    // (((grid_doc.IsVisible) && (!dp_docDate.Text.Equals("")) && (!tb_docNum.Text.Equals(""))) || (!dp_docDate.IsVisible)) &&
                    // (((tb_docNumCard.IsVisible) && (!tb_docNumCard.Text.Equals(""))) || (!tb_docNumCard.IsVisible)) &&
                    // (((cb_card.IsVisible) && (!cb_card.Text.Equals(""))) || (!cb_card.IsVisible)) &&
                    //    acceptedAmmount
                    // )
                    if (HelpClass.validate(requiredControlList, this) && acceptedAmmount)
                    {
                        CashTransfer cash = new CashTransfer();

                        cash.transType = "d";
                        cash.posId = MainWindow.posLogin.posId;
                        cash.transNum = await cashModel.generateCashNumber(cash.transType + "c");
                        cash.cash = decimal.Parse(tb_cash.Text);
                        cash.notes = tb_notes.Text;
                        cash.createUserId = MainWindow.userLogin.userId;
                        cash.side = "c";
                        cash.processType = cb_paymentProcessType.SelectedValue.ToString();

                        cash.agentId = agentId;

                        cash.userId = userId;

                        if (cb_paymentProcessType.SelectedValue.ToString().Equals("card"))
                        {
                            //cash.cardId = Convert.ToInt32(cb_card.SelectedValue);
                            cash.cardId = _SelectedCard;
                            cash.docNum = tb_docNumCard.Text;
                        }
                        if (cb_paymentProcessType.SelectedValue.ToString().Equals("doc"))
                            cash.docNum = await cashModel.generateDocNumber("pbnd");

                        if (cb_paymentProcessType.SelectedValue.ToString().Equals("cheque"))
                            cash.docNum = tb_docNumCheque.Text;

                        if (cb_paymentProcessType.SelectedValue.ToString().Equals("doc"))
                        {
                            //int res = await saveBond(cash.docNum, cash.cash, dp_docDate.SelectedDate.Value, "d");
                            //cash.bondId = res;
                        }

                        int s = await cashModel.payOrderInvoice(invoice.invoiceId, invoice.invStatusId, cash.cash, processType, cash);

                        if (!s.Equals(0))
                        {
                            if (cb_paymentProcessType.SelectedValue.ToString().Equals("cash"))
                                await calcBalance(decimal.Parse(tb_cash.Text));

                            Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);

                            Clear();
                            await RefreshInvoiceList();
                            await Search();
                        }
                        else
                        {
                            Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                        }
                    }
                    #endregion
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        public void BuildReport()
        {
            List<ReportParameter> paramarr = new List<ReportParameter>();

            string addpath;
            bool isArabic = ReportCls.checkLang();
            if (isArabic)
            {
                addpath = @"\Reports\Account\Ar\ArOrderAccReport.rdlc";
            }
            else addpath = @"\Reports\Account\En\OrderAccReport.rdlc";
            string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);

            ReportCls.checkLang();

            //clsReports.orderReport(invoiceQuery, rep, reppath);
            clsReports.orderReport(invoiceQuery, rep, reppath, paramarr);
            clsReports.setReportLanguage(paramarr);
            clsReports.Header(paramarr);

            rep.SetParameters(paramarr);
            rep.Refresh();

        }
        ReportCls reportclass = new ReportCls();
        LocalReport rep = new LocalReport();
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        private void Btn_print_Click(object sender, RoutedEventArgs e)
        {//print
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);
                if (FillCombo.groupObject.HasPermissionAction(reportsPermission, FillCombo.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    #region
                    BuildReport();
                    LocalReportExtensions.PrintToPrinterbyNameAndCopy(rep, FillCombo.rep_printer_name, short.Parse(FillCombo.rep_print_count));
                    #endregion
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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
        private void Btn_preview1_Click(object sender, RoutedEventArgs e)
        {//preview
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);
                /////////////////////
                if (FillCombo.groupObject.HasPermissionAction(reportsPermission, FillCombo.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    #region
                    Window.GetWindow(this).Opacity = 0.2;
                    string pdfpath = "";


                    //
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
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                /////////////////////
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
        private void Btn_pieChart_Click(object sender, RoutedEventArgs e)
        {//pie
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);
                /////////////////////
                if (FillCombo.groupObject.HasPermissionAction(reportsPermission, FillCombo.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    Window.GetWindow(this).Opacity = 0.2;
                    win_IvcAccount win = new win_IvcAccount(invoiceQuery ,2 );
                    // // w.ShowInTaskbar = false;
                    win.ShowDialog();
                    Window.GetWindow(this).Opacity = 1;
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                /////////////////////
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


        private void Button_Click(object sender, RoutedEventArgs e)
        {//pdf
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);
                /////////////////////
                if (FillCombo.groupObject.HasPermissionAction(reportsPermission, FillCombo.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    #region
                    BuildReport();

                    saveFileDialog.Filter = "PDF|*.pdf;";

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string filepath = saveFileDialog.FileName;
                        LocalReportExtensions.ExportToPDF(rep, filepath);
                    }
                    #endregion
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                /////////////////////
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
        private async void Chb_all_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                dp_searchStartDate.IsEnabled =
            dp_searchEndDate.IsEnabled = false;
                Btn_refresh_Click(btn_refresh, null);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Chb_all_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                dp_searchStartDate.IsEnabled =
                dp_searchEndDate.IsEnabled = true;

                Btn_refresh_Click(btn_refresh, null);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #region validate - clearValidate - textChange - lostFocus - . . . . 
        void Clear()
        {
            this.DataContext = new Invoice();

            // last 
            HelpClass.clearValidate(requiredControlList, this);
        }
        string input;
        decimal _decimal = 0;
        private void Number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        { //only  digits
            try
            {
                TextBox textBox = sender as TextBox;
                HelpClass.InputJustNumber(ref textBox);
                if (textBox.Tag.ToString() == "int")
                {
                    Regex regex = new Regex("[^0-9]");
                    e.Handled = regex.IsMatch(e.Text);
                }
                else if (textBox.Tag.ToString() == "decimal")
                {
                    input = e.Text;
                    e.Handled = !decimal.TryParse(textBox.Text + input, out _decimal);

                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Code_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                //only english and digits
                Regex regex = new Regex("^[a-zA-Z0-9. -_?]*$");
                if (!regex.IsMatch(e.Text))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }

        }
        private void Spaces_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                e.Handled = e.Key == Key.Space;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void ValidateEmpty_TextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                HelpClass.validate(requiredControlList, this);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void validateEmpty_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.validate(requiredControlList, this);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        #endregion
    }
}
