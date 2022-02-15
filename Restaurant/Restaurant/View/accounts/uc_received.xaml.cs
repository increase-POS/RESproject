using netoaster;
using Restaurant.Classes;
using Restaurant.View.windows;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.IO;
using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
 using System.Windows.Resources;

namespace Restaurant.View.accounts
{
    /// <summary>
    /// Interaction logic for uc_received.xaml
    /// </summary>
    public partial class uc_received : UserControl
    {
        public uc_received()
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
        private static uc_received _instance;
        public static uc_received Instance
        {
            get
            {
                //if (_instance == null)
                _instance = new uc_received();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        CashTransfer cashModel = new CashTransfer();
        CashTransfer cashtrans = new CashTransfer();
        //Bonds bondModel = new Bonds();
        Card cardModel = new Card();
        Agent agentModel = new Agent();
        User userModel = new User();
        ShippingCompanies shCompanyModel = new ShippingCompanies();
        Pos posModel = new Pos();
        IEnumerable<Agent> agents;
        IEnumerable<User> users;
        IEnumerable<ShippingCompanies> shCompanies;
        IEnumerable<Card> cards;
        IEnumerable<CashTransfer> cashesQuery;
        IEnumerable<CashTransfer> cashesQueryExcel;
        IEnumerable<CashTransfer> cashes;
        static private int _SelectedCard = -1;

        public List<Invoice> invoicesLst = new List<Invoice>();
        //print
        ReportCls reportclass = new ReportCls();
        LocalReport rep = new LocalReport();
        SaveFileDialog saveFileDialog = new SaveFileDialog();

        string searchText = "";
        string createPermission = "received_create";
        string reportsPermission = "received_reports";
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
            try
            {
                HelpClass.StartAwait(grid_main);

                requiredControlList = new List<string> { "cash", "depositFrom", "paymentProcessType" };

                #region translate
                if (MainWindow.lang.Equals("en"))
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

                btn_invoices.IsEnabled = false;

                dp_searchStartDate.SelectedDateChanged += this.dp_SelectedStartDateChanged;
                dp_searchEndDate.SelectedDateChanged += this.dp_SelectedEndDateChanged;

                #region fill deposit from combo
                var depositlist = new[] {
                new { Text = MainWindow.resourcemanager.GetString("trVendor")                 , Value = "v" },
                new { Text = MainWindow.resourcemanager.GetString("trCustomer")               , Value = "c" },
                new { Text = MainWindow.resourcemanager.GetString("trUser")                   , Value = "u" },
                new { Text = MainWindow.resourcemanager.GetString("trAdministrativeDeposit")  , Value = "m" },
                new { Text = MainWindow.resourcemanager.GetString("trShippingCompanies")      , Value = "sh" }
                 };
                cb_depositFrom.DisplayMemberPath = "Text";
                cb_depositFrom.SelectedValuePath = "Value";
                cb_depositFrom.ItemsSource = depositlist;
                #endregion

                await fillVendors();

                await fillCustomers();

                await fillUsers();

                await fillShippingCompanies();

                #region fill process type
                var typelist = new[] {
                new { Text = MainWindow.resourcemanager.GetString("trCash")       , Value = "cash" },
                //new { Text = MainWindow.resourcemanager.GetString("trDocument")   , Value = "doc" },
                new { Text = MainWindow.resourcemanager.GetString("trCheque")     , Value = "cheque" },
                new { Text = MainWindow.resourcemanager.GetString("trAnotherPaymentMethods") , Value = "card" },
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

                btn_image.IsEnabled = false;

                await Search();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
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
            txt_baseInformation.Text = MainWindow.resourcemanager.GetString("trTransaferDetails");
            txt_title.Text = MainWindow.resourcemanager.GetString("trReceived");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_search, MainWindow.resourcemanager.GetString("trSearchHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_depositFrom, MainWindow.resourcemanager.GetString("trDepositFromHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_depositorV, MainWindow.resourcemanager.GetString("trDepositorHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_depositorC, MainWindow.resourcemanager.GetString("trDepositorHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_depositorU, MainWindow.resourcemanager.GetString("trDepositorHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_depositorSh, MainWindow.resourcemanager.GetString("trDepositorHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_paymentProcessType, MainWindow.resourcemanager.GetString("trPaymentTypeHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_docNum, MainWindow.resourcemanager.GetString("trDocNumHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_docDate, MainWindow.resourcemanager.GetString("trDocDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_docNumCheque, MainWindow.resourcemanager.GetString("trDocNumHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_docNumCard, MainWindow.resourcemanager.GetString("trProcessNumHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_docDateCheque, MainWindow.resourcemanager.GetString("trDocDateHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_cash, MainWindow.resourcemanager.GetString("trCashHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_notes, MainWindow.resourcemanager.GetString("trNoteHint"));

            chb_all.Content = MainWindow.resourcemanager.GetString("trAll");

            dg_receivedAccounts.Columns[0].Header = MainWindow.resourcemanager.GetString("trTransferNumberTooltip");
            dg_receivedAccounts.Columns[1].Header = MainWindow.resourcemanager.GetString("trDepositor");
            dg_receivedAccounts.Columns[2].Header = MainWindow.resourcemanager.GetString("trPaymentTypeTooltip");
            dg_receivedAccounts.Columns[3].Header = MainWindow.resourcemanager.GetString("trDate");
            dg_receivedAccounts.Columns[4].Header = MainWindow.resourcemanager.GetString("trCashTooltip");

            tt_clear.Content = MainWindow.resourcemanager.GetString("trClear");
            tt_refresh.Content = MainWindow.resourcemanager.GetString("trRefresh");
            tt_report.Content = MainWindow.resourcemanager.GetString("trPdf");
            tt_print.Content = MainWindow.resourcemanager.GetString("trPrint");
            tt_excel.Content = MainWindow.resourcemanager.GetString("trExcel");
            tt_count.Content = MainWindow.resourcemanager.GetString("trCount");

            btn_add.Content = MainWindow.resourcemanager.GetString("trSave");
            txt_image.Text = MainWindow.resourcemanager.GetString("trImage");
            txt_preview.Text = MainWindow.resourcemanager.GetString("trPreview");
            txt_printInvoice.Text = MainWindow.resourcemanager.GetString("trPrint");
            txt_pdf.Text = MainWindow.resourcemanager.GetString("trPdfBtn");

        }
        private void Dg_receivedAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//selection
            try
            {
                HelpClass.StartAwait(grid_main);
                #region clear validate
                //HelpClass.clearComboBoxValidate(cb_depositFrom, p_error_depositFrom);
                //HelpClass.clearComboBoxValidate(cb_depositorV, p_error_depositor);
                //HelpClass.clearComboBoxValidate(cb_depositorC, p_error_depositor);
                //HelpClass.clearComboBoxValidate(cb_depositorU, p_error_depositor);
                //HelpClass.clearComboBoxValidate(cb_depositorSh, p_error_depositor);
                //HelpClass.clearComboBoxValidate(cb_paymentProcessType, p_error_paymentProcessType);
                //HelpClass.clearTextBlockValidate(txt_card, p_error_card);
                //TextBox tbDocDate = (TextBox)dp_docDate.Template.FindName("PART_TextBox", dp_docDate);
                //HelpClass.clearValidate(tb_docNum, p_error_docNum);
                //HelpClass.clearValidate(tb_docNumCard, p_error_docCard);
                //HelpClass.clearValidate(tb_docNumCheque, p_error_docNumCheque);
                //HelpClass.clearValidate(tb_cash, p_error_cash);
                #endregion

                if (dg_receivedAccounts.SelectedIndex != -1)
                {
                    cashtrans = dg_receivedAccounts.SelectedItem as CashTransfer;
                    this.DataContext = cashtrans;
                    if (cashtrans != null)
                    {
                        btn_image.IsEnabled = true;
                        ///////////////////////////
                        btn_add.IsEnabled = false;
                        cb_depositFrom.IsEnabled = false;
                        cb_depositorV.IsEnabled = false;
                        cb_depositorC.IsEnabled = false;
                        cb_depositorU.IsEnabled = false;
                        cb_depositorSh.IsEnabled = false;
                        btn_invoices.IsEnabled = false;
                        cb_paymentProcessType.IsEnabled = false;
                        gd_card.IsEnabled = false;
                        tb_docNum.IsEnabled = false;
                        dp_docDate.IsEnabled = false;
                        tb_docNumCheque.IsEnabled = false;
                        tb_docNumCard.IsEnabled = false;
                        dp_docDateCheque.IsEnabled = false;
                        tb_cash.IsEnabled = false;
                        tb_notes.IsEnabled = false;
                        //////////////////////////
                        //tb_transNum.Text = cashtrans.transNum;

                        //tb_cash.Text = HelpClass.DecTostring(cashtrans.cash);

                        cb_depositFrom.SelectedValue = cashtrans.side;

                        switch (cb_depositFrom.SelectedValue.ToString())
                        {
                            case "v":
                                cb_depositorV.SelectedIndex = -1;
                                try { cb_depositorV.SelectedValue = cashtrans.agentId.Value; }
                                catch { }
                                bdr_depositorC.Visibility = Visibility.Collapsed; //HelpClass.clearComboBoxValidate(cb_depositorC, p_error_depositor);
                                bdr_depositorU.Visibility = Visibility.Collapsed; //HelpClass.clearComboBoxValidate(cb_depositorU, p_error_depositor);
                                bdr_depositorSh.Visibility = Visibility.Collapsed; //HelpClass.clearComboBoxValidate(cb_depositorSh, p_error_depositor);
                                break;
                            case "c":
                                cb_depositorC.SelectedIndex = -1;
                                try { cb_depositorC.SelectedValue = cashtrans.agentId.Value; }
                                catch { }
                                bdr_depositorV.Visibility = Visibility.Collapsed; //HelpClass.clearComboBoxValidate(cb_depositorV, p_error_depositor);
                                bdr_depositorU.Visibility = Visibility.Collapsed; //HelpClass.clearComboBoxValidate(cb_depositorU, p_error_depositor);
                                bdr_depositorSh.Visibility = Visibility.Collapsed; //HelpClass.clearComboBoxValidate(cb_depositorSh, p_error_depositor);
                                break;
                            case "u":
                                cb_depositorU.SelectedIndex = -1;
                                try { cb_depositorU.SelectedValue = cashtrans.userId.Value; }
                                catch { }
                                bdr_depositorV.Visibility = Visibility.Collapsed; //HelpClass.clearComboBoxValidate(cb_depositorV, p_error_depositor);
                                bdr_depositorC.Visibility = Visibility.Collapsed; //HelpClass.clearComboBoxValidate(cb_depositorC, p_error_depositor);
                                bdr_depositorSh.Visibility = Visibility.Collapsed; //HelpClass.clearComboBoxValidate(cb_depositorSh, p_error_depositor);
                                break;
                            case "sh":
                                cb_depositorSh.SelectedIndex = -1;
                                try { cb_depositorSh.SelectedValue = cashtrans.shippingCompanyId.Value; }
                                catch { }
                                bdr_depositorV.Visibility = Visibility.Collapsed; //HelpClass.clearComboBoxValidate(cb_depositorV, p_error_depositor);
                                bdr_depositorC.Visibility = Visibility.Collapsed; //HelpClass.clearComboBoxValidate(cb_depositorC, p_error_depositor);
                                bdr_depositorU.Visibility = Visibility.Collapsed; //HelpClass.clearComboBoxValidate(cb_depositorU, p_error_depositor);
                                break;
                            case "m":
                                bdr_depositorV.Visibility = Visibility.Collapsed; //HelpClass.clearComboBoxValidate(cb_depositorV, p_error_depositor);
                                bdr_depositorC.Visibility = Visibility.Collapsed; //HelpClass.clearComboBoxValidate(cb_depositorC, p_error_depositor);
                                bdr_depositorU.Visibility = Visibility.Collapsed; //HelpClass.clearComboBoxValidate(cb_depositorU, p_error_depositor);
                                bdr_depositorSh.Visibility = Visibility.Collapsed; //HelpClass.clearComboBoxValidate(cb_depositorSh, p_error_depositor);
                                break;
                        }

                        cb_paymentProcessType.SelectedValue = cashtrans.processType;
                        if (cashtrans.cardId != null)
                            _SelectedCard = (int)cashtrans.cardId;
                    }
                }
                    HelpClass.clearValidate(requiredControlList, this);
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
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
        async Task Search()
        {
            try
            {
                if (cashes is null)
                    await RefreshCashesList();
                if (chb_all.IsChecked == false)
                {
                    searchText = tb_search.Text.ToLower();
                    cashesQuery = cashes.Where(s => (s.transNum.ToLower().Contains(searchText)
                    || s.cash.ToString().ToLower().Contains(searchText)
                    )
                    && (s.side == "v" || s.side == "c" || s.side == "u" || s.side == "m" || s.side == "sh")
                    && s.transType == "d"
                    && s.processType != "inv"
                    && s.updateDate.Value.Date >= dp_searchStartDate.SelectedDate.Value.Date
                    && s.updateDate.Value.Date <= dp_searchEndDate.SelectedDate.Value.Date
                    );
                }
                else
                {
                    searchText = tb_search.Text.ToLower();
                    cashesQuery = cashes.Where(s => (s.transNum.ToLower().Contains(searchText)
                    || s.cash.ToString().ToLower().Contains(searchText)
                    )
                    && (s.side == "v" || s.side == "c" || s.side == "u" || s.side == "m" || s.side == "sh")
                    && s.transType == "d"
                    && s.processType != "inv"
                    );
                }


                cashesQueryExcel = cashesQuery.ToList();
                RefreshCashView();
            }
            catch { }
        }

        private async void Btn_add_Click(object sender, RoutedEventArgs e)
        {//save
            try
            {
                HelpClass.StartAwait(grid_main);
                //  string s = "0", s1 = "";
                int s = 0;
                int s1 = 0;
                if (MainWindow.groupObject.HasPermissionAction(createPermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    #region validate

                    //chk empty cash
                    //HelpClass.validateEmptyTextBox(tb_cash, p_error_cash, tt_errorCash, "trEmptyCashToolTip");

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

                    //chk empty cheque num
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
                    //    HelpClass.validateEmptyTextBox(tb_docNumCard, p_error_docCard, tt_docNumCard, "trEmptyProcessNumToolTip");
                    //}
                    //else
                    //{
                    //    HelpClass.clearValidate(tb_docNumCard, p_error_docCard);
                    //}
                    //chk empty deposit from
                    //HelpClass.validateEmptyComboBox(cb_depositFrom, p_error_depositFrom, tt_errorDepositFrom, "trErrorEmptyDepositFromToolTip");

                    //chk empty depositor
                    //if (cb_depositorV.IsVisible)
                    //    HelpClass.validateEmptyComboBox(cb_depositorV, p_error_depositor, tt_errordepositor, "trErrorEmptyDepositorToolTip");
                    //else
                    //    HelpClass.clearComboBoxValidate(cb_depositorV, p_error_depositor);
                    //if (cb_depositorC.IsVisible)
                    //    HelpClass.validateEmptyComboBox(cb_depositorC, p_error_depositor, tt_errordepositor, "trErrorEmptyDepositorToolTip");
                    //else
                    //    HelpClass.clearComboBoxValidate(cb_depositorC, p_error_depositor);
                    //if (cb_depositorU.IsVisible)
                    //    HelpClass.validateEmptyComboBox(cb_depositorU, p_error_depositor, tt_errordepositor, "trErrorEmptyDepositorToolTip");
                    //else
                    //    HelpClass.clearComboBoxValidate(cb_depositorU, p_error_depositor);
                    //if (cb_depositorSh.IsVisible)
                    //    HelpClass.validateEmptyComboBox(cb_depositorSh, p_error_depositor, tt_errordepositor, "trErrorEmptyDepositorToolTip");
                    //else
                    //    HelpClass.clearComboBoxValidate(cb_depositorU, p_error_depositor);

                    //chk empty payment type
                    //HelpClass.validateEmptyComboBox(cb_paymentProcessType, p_error_paymentProcessType, tt_errorpaymentProcessType, "trErrorEmptyPaymentTypeToolTip");

                    //chk empty card 
                    //if (gd_card.IsVisible)
                    //    HelpClass.validateEmptyTextBlock(txt_card, p_error_card, tt_errorCard, "trSelectCreditCard");
                    //else
                    //    HelpClass.clearTextBlockValidate(txt_card, p_error_card);
                    #endregion

                    #region save
                    //if ((!tb_cash.Text.Equals("")) && (!cb_depositFrom.Text.Equals("")) && (!cb_paymentProcessType.Text.Equals("")) &&
                    //(((cb_depositorV.IsVisible) && (!cb_depositorV.Text.Equals(""))) || (!cb_depositorV.IsVisible)) &&
                    //(((cb_depositorC.IsVisible) && (!cb_depositorC.Text.Equals(""))) || (!cb_depositorC.IsVisible)) &&
                    //(((cb_depositorU.IsVisible) && (!cb_depositorU.Text.Equals(""))) || (!cb_depositorU.IsVisible)) &&
                    //(((cb_depositorSh.IsVisible) && (!cb_depositorSh.Text.Equals(""))) || (!cb_depositorSh.IsVisible)) &&
                    //(((grid_cheque.IsVisible) && (!tb_docNumCheque.Text.Equals(""))) || (!grid_cheque.IsVisible)) &&
                    //(((grid_doc.IsVisible) && (!dp_docDate.Text.Equals("")) && (!tb_docNum.Text.Equals(""))) || (!dp_docDate.IsVisible)) &&
                    //(((gd_card.IsVisible) && (!txt_card.Text.Equals(""))) || (!gd_card.IsVisible))
                    //)
                    if (HelpClass.validate(requiredControlList, this) )
                    {
                        string depositor = cb_depositFrom.SelectedValue.ToString();
                        int agentid = 0;

                        CashTransfer cash = new CashTransfer();

                        cash.transType = "d";
                        cash.posId = MainWindow.posLogin.posId;
                        cash.transNum = await cashModel.generateCashNumber(cash.transType + cb_depositFrom.SelectedValue.ToString());
                        cash.cash = decimal.Parse(tb_cash.Text);
                        cash.notes = tb_notes.Text;
                        cash.createUserId = MainWindow.userLogin.userId;
                        cash.side = cb_depositFrom.SelectedValue.ToString();
                        cash.processType = cb_paymentProcessType.SelectedValue.ToString();

                        if (bdr_depositorV.IsVisible)
                        { cash.agentId = Convert.ToInt32(cb_depositorV.SelectedValue); agentid = Convert.ToInt32(cb_depositorV.SelectedValue); }

                        if (bdr_depositorC.IsVisible)
                        {
                            cash.agentId = Convert.ToInt32(cb_depositorC.SelectedValue);
                            agentid = Convert.ToInt32(cb_depositorC.SelectedValue);
                        }

                        if (bdr_depositorU.IsVisible)
                            cash.userId = Convert.ToInt32(cb_depositorU.SelectedValue);

                        if (bdr_depositorSh.IsVisible)
                            cash.shippingCompanyId = Convert.ToInt32(cb_depositorSh.SelectedValue);

                        if (cb_paymentProcessType.SelectedValue.ToString().Equals("card"))
                        {
                            cash.cardId = _SelectedCard;
                            cash.docNum = tb_docNumCard.Text;
                        }

                        if (cb_paymentProcessType.SelectedValue.ToString().Equals("doc"))
                            cash.docNum = tb_docNum.Text;

                        if (cb_paymentProcessType.SelectedValue.ToString().Equals("cheque"))
                            cash.docNum = tb_docNumCheque.Text;

                        if (cb_paymentProcessType.SelectedValue.ToString().Equals("doc"))
                        {
                            //int res = await saveBond(cash.docNum, cash.cash, dp_docDate.SelectedDate.Value, "d");
                            //cash.bondId = res;
                        }

                        if (bdr_depositorV.IsVisible || bdr_depositorC.IsVisible)
                        {
                            if (tb_cash.IsReadOnly)
                                s1 = await cashModel.PayListOfInvoices(cash.agentId.Value, invoicesLst, "feed", cash);
                            else
                                s1 = await cashModel.PayByAmmount(cash.agentId.Value, decimal.Parse(tb_cash.Text), "feed", cash);
                        }
                        else if (bdr_depositorU.IsVisible)
                        {
                            if (tb_cash.IsReadOnly)
                                s1 = await cashModel.PayUserListOfInvoices(cash.userId.Value, invoicesLst, "feed", cash);
                            else
                                s1 = await cashModel.PayUserByAmmount(cash.userId.Value, decimal.Parse(tb_cash.Text), "feed", cash);
                        }
                        else if (bdr_depositorSh.IsVisible)
                        {
                            if (tb_cash.IsReadOnly)
                                s1 = await cashModel.PayShippingCompanyListOfInvoices(cash.shippingCompanyId.Value, invoicesLst, "feed", cash);
                            else
                                s1 = await cashModel.payShippingCompanyByAmount(cash.shippingCompanyId.Value, decimal.Parse(tb_cash.Text), "feed", cash);
                        }
                        else
                            s = await cashModel.Save(cash);

                        if ((!s.Equals("0")) || (!s1.Equals("")) || (s1.Equals("-1")))
                        {
                            if (cb_paymentProcessType.SelectedValue.ToString().Equals("cash"))
                                await calcBalance(cash.cash, depositor, agentid);

                            Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);
                            Clear();
                            await RefreshCashesList();
                            await Search();
                        }
                        else
                            Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                    }
                    #endregion

                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                
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
        private async Task calcBalance(decimal ammount, string depositor, int agentid)
        {
            int s = 0;
            //increase pos balance
            Pos pos = await posModel.getById(MainWindow.posLogin.posId);
            pos.balance += ammount;

            s = await pos.save(pos);
        }

        private async Task calcUserBalance(decimal value, int userId)
        {//balance for user
            User user = await userModel.getUserById(userId);

            if (user.balanceType == 0)
                user.balance += value;
            else
            {
                if (value > user.balance)
                {
                    value -= user.balance;
                    user.balance = value;
                    user.balanceType = 0;
                }
                else
                    user.balance -= value;
            }

            await userModel.save(user);

        }

        private async Task calcShippingComBalance(decimal value, int shippingcompanyId)
        {//balance for shipping company
            ShippingCompanies shCom = await shCompanyModel.GetByID(shippingcompanyId);

            if (shCom.balanceType == 0)
                shCom.balance += value;
            else
            {
                if (value > shCom.balance)
                {
                    value -= shCom.balance;
                    shCom.balance = value;
                    shCom.balanceType = 0;
                }
                else
                    shCom.balance -= value;
            }
            await shCompanyModel.save(shCom);

        }
 

        private void Btn_clear_Click(object sender, RoutedEventArgs e)
        {//clear
            try
            {
                HelpClass.StartAwait(grid_main);

                Clear();
               
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

                if (MainWindow.groupObject.HasPermissionAction(createPermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
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
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        void FN_ExportToExcel()
        {
            var QueryExcel = cashesQueryExcel.AsEnumerable().Select(x => new
            {
                TransNum = x.transNum,
                DepositFrom = x.side,
                Depositor = x.agentName,
                OpperationType = x.processType,
                Cash = x.cash
            });
            var DTForExcel = QueryExcel.ToDataTable();
            DTForExcel.Columns[0].Caption = MainWindow.resourcemanager.GetString("trTransferNumberTooltip");
            DTForExcel.Columns[1].Caption = MainWindow.resourcemanager.GetString("trDepositFrom");
            DTForExcel.Columns[2].Caption = MainWindow.resourcemanager.GetString("trDepositor");
            DTForExcel.Columns[3].Caption = MainWindow.resourcemanager.GetString("trPaymentTypeTooltip");
            DTForExcel.Columns[4].Caption = MainWindow.resourcemanager.GetString("trCashTooltip");

            ExportToExcel.Export(DTForExcel);

        }


        private void Btn_image_Click(object sender, RoutedEventArgs e)
        {//image
            try
            {
                if (MainWindow.groupObject.HasPermissionAction(createPermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    if (cashtrans != null || cashtrans.cashTransId != 0)
                    {
                        wd_uploadImage w = new wd_uploadImage();

                        w.tableName = "cashTransfer";
                        w.tableId = cashtrans.cashTransId;
                        w.docNum = cashtrans.docNum;
                        w.ShowDialog();
                    }
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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
                await RefreshCashesList();
                await Search();
                
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        async Task<IEnumerable<CashTransfer>> RefreshCashesList()
        {
            cashes = await cashModel.GetCashBond("d", "all");
            cashes = cashes.Where(x => (x.processType != "balance")).GroupBy(x => x.transNum).Select(x => new CashTransfer
            {
                cashTransId = x.FirstOrDefault().cashTransId,
                transType = x.FirstOrDefault().transType,
                posId = x.FirstOrDefault().posId,
                userId = x.FirstOrDefault().userId,
                agentId = x.FirstOrDefault().agentId,
                invId = x.FirstOrDefault().invId,
                transNum = x.FirstOrDefault().transNum,
                createDate = x.FirstOrDefault().createDate,
                updateDate = x.FirstOrDefault().updateDate,
                cash = x.Sum(g => g.cash),
                updateUserId = x.FirstOrDefault().updateUserId,
                createUserId = x.FirstOrDefault().createUserId,
                notes = x.FirstOrDefault().notes,
                posIdCreator = x.FirstOrDefault().posIdCreator,
                isConfirm = x.FirstOrDefault().isConfirm,
                cashTransIdSource = x.FirstOrDefault().cashTransIdSource,
                side = x.FirstOrDefault().side,
                docName = x.FirstOrDefault().docName,
                docNum = x.FirstOrDefault().docNum,
                docImage = x.FirstOrDefault().docImage,
                bankId = x.FirstOrDefault().bankId,
                bankName = x.FirstOrDefault().bankName,
                agentName = x.FirstOrDefault().agentName,
                usersName = x.FirstOrDefault().usersName,// side =u
                posName = x.FirstOrDefault().posName,
                posCreatorName = x.FirstOrDefault().posCreatorName,
                processType = x.FirstOrDefault().processType,
                cardId = x.FirstOrDefault().cardId,
                bondId = x.FirstOrDefault().bondId,
                usersLName = x.FirstOrDefault().usersLName,// side =u
                createUserName = x.FirstOrDefault().createUserName,
                createUserLName = x.FirstOrDefault().createUserLName,
                createUserJob = x.FirstOrDefault().createUserJob,
                cardName = x.FirstOrDefault().cardName,
                bondDeserveDate = x.FirstOrDefault().bondDeserveDate,
                bondIsRecieved = x.FirstOrDefault().bondIsRecieved,
                shippingCompanyId = x.FirstOrDefault().shippingCompanyId,
                shippingCompanyName = x.FirstOrDefault().shippingCompanyName


            });
            return cashes;
        }

        void RefreshCashView()
        {
            dg_receivedAccounts.ItemsSource = cashesQuery;
            txt_count.Text = cashesQuery.Count().ToString();
        }

        private void Tb_validateEmptyLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = sender.GetType().Name;
                validateEmpty(name, sender);
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
                //else if ((sender as TextBox).Name == "tb_docNumCard")
                //    HelpClass.validateEmptyTextBox((TextBox)sender, p_error_docCard, tt_errorDocCard, "trEmptyProcessNumToolTip");
            }
            else if (name == "ComboBox")
            {
                if ((sender as ComboBox).Name == "cb_depositFrom")
                    HelpClass.validateEmptyComboBox((ComboBox)sender, p_error_depositFrom, tt_errorDepositFrom, "trErrorEmptyDepositFromToolTip");
                else if ((sender as ComboBox).Name == "cb_depositorV")
                    HelpClass.validateEmptyComboBox((ComboBox)sender, p_error_depositor, tt_errordepositor, "trErrorEmptyDepositorToolTip");
                else if ((sender as ComboBox).Name == "cb_depositorC")
                    HelpClass.validateEmptyComboBox((ComboBox)sender, p_error_depositor, tt_errordepositor, "trErrorEmptyDepositorToolTip");
                else if ((sender as ComboBox).Name == "cb_depositorU")
                    HelpClass.validateEmptyComboBox((ComboBox)sender, p_error_depositor, tt_errordepositor, "trErrorEmptyDepositorToolTip");
                else if ((sender as ComboBox).Name == "cb_depositorSh")
                    HelpClass.validateEmptyComboBox((ComboBox)sender, p_error_depositor, tt_errordepositor, "trErrorEmptyDepositorToolTip");
                else if ((sender as ComboBox).Name == "cb_paymentProcessType")
                    HelpClass.validateEmptyComboBox((ComboBox)sender, p_error_paymentProcessType, tt_errorpaymentProcessType, "trErrorEmptyPaymentTypeToolTip");
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
            try
            {
                string name = sender.GetType().Name;
                validateEmpty(name, sender);
                var txb = sender as TextBox;
                if ((sender as TextBox).Name == "tb_cash")
                    HelpClass.InputJustNumber(ref txb);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }

        }

        private void Cb_paymentProcessType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//type selection
            try
            {
                HelpClass.StartAwait(grid_main);
                switch (cb_paymentProcessType.SelectedIndex)
                {
                    case 0://cash
                        grid_doc.Visibility = Visibility.Collapsed;
                        grid_cheque.Visibility = Visibility.Collapsed;
                        gd_card.Visibility = Visibility.Collapsed;
                        tb_docNumCard.Visibility = Visibility.Collapsed;
                        tb_docNumCheque.Visibility = Visibility.Collapsed;
                        HelpClass.clearValidate(p_error_docCard);
                        HelpClass.clearValidate(p_error_docNum);
                        HelpClass.clearValidate(p_error_docNum);
                        HelpClass.clearValidate(p_error_card);
                        if (grid_doc.IsVisible)
                        {
                            TextBox dpDate = (TextBox)dp_docDate.Template.FindName("PART_TextBox", dp_docDate);
                            HelpClass.clearValidate(p_error_docDate);
                        }
                        if (grid_cheque.IsVisible)
                        {
                            TextBox dpDateCheque = (TextBox)dp_docDateCheque.Template.FindName("PART_TextBox", dp_docDateCheque);
                            HelpClass.clearValidate(p_error_docNumCheque);
                        }
                        break;

                    //case 1://doc
                    //    grid_doc.Visibility = Visibility.Visible;
                    //    grid_cheque.Visibility = Visibility.Collapsed;
                    //    gd_card.Visibility = Visibility.Collapsed;
                    //    tb_docNumCard.Visibility = Visibility.Collapsed;
                    //    tb_docNumCheque.Visibility = Visibility.Collapsed;
                    //    HelpClass.clearValidate(tb_docNumCard, p_error_docCard);
                    //    HelpClass.clearValidate(tb_docNumCheque, p_error_docNum);
                    //    HelpClass.clearTextBlockValidate(txt_card, p_error_card);
                    //    if (grid_cheque.IsVisible)
                    //    {
                    //        TextBox dpDateCheque = (TextBox)dp_docDateCheque.Template.FindName("PART_TextBox", dp_docDateCheque);
                    //        HelpClass.clearValidate(dpDateCheque, p_error_docNumCheque);
                    //    }
                    //    break;

                    //case 2://cheque
                    case 1://cheque
                        grid_doc.Visibility = Visibility.Collapsed;
                        grid_cheque.Visibility = Visibility.Visible;
                        gd_card.Visibility = Visibility.Collapsed;
                        tb_docNumCard.Visibility = Visibility.Collapsed;
                        HelpClass.clearValidate(p_error_docCard);
                        HelpClass.clearValidate(p_error_docNum);
                        HelpClass.clearValidate(p_error_card);
                        if (grid_doc.IsVisible)
                        {
                            TextBox dpDate = (TextBox)dp_docDate.Template.FindName("PART_TextBox", dp_docDate);
                            HelpClass.clearValidate(p_error_docDate);
                        }
                        break;

                    //case 3://card
                    case 2://card
                        grid_doc.Visibility = Visibility.Collapsed;
                        grid_cheque.Visibility = Visibility.Collapsed;
                        gd_card.Visibility = Visibility.Visible;
                        tb_docNumCard.Visibility = Visibility.Visible;
                        HelpClass.clearValidate(p_error_docNum);
                        HelpClass.clearValidate(p_error_docNum);
                        HelpClass.clearValidate(p_error_card);
                        if (grid_doc.IsVisible)
                        {
                            TextBox dpDate = (TextBox)dp_docDate.Template.FindName("PART_TextBox", dp_docDate);
                            HelpClass.clearValidate(p_error_docDate);
                        }
                        if (grid_cheque.IsVisible)
                        {
                            TextBox dpDateCheque = (TextBox)dp_docDateCheque.Template.FindName("PART_TextBox", dp_docDateCheque);
                            HelpClass.clearValidate(p_error_docNumCheque);
                        }
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

        private void Cb_depositFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//deposit selection
         //try
         //{
         //    
         //        HelpClass.StartAwait(grid_main);
         
            btn_invoices.IsEnabled = false;
            switch (cb_depositFrom.SelectedIndex)
            {
                case 0://vendor
                    cb_depositorV.SelectedIndex = -1;
                    bdr_depositorV.Visibility = Visibility.Visible;
                    bdr_depositorC.Visibility = Visibility.Collapsed;
                    btn_invoices.Visibility = Visibility.Visible;
                    bdr_depositorU.Visibility = Visibility.Collapsed;
                    bdr_depositorSh.Visibility = Visibility.Collapsed;
                    requiredControlList = new List<string> { "cash", "depositFrom", "paymentProcessType" , "depositorV" };
                    break;
                case 1://customer
                    cb_depositorC.SelectedIndex = -1;
                    bdr_depositorV.Visibility = Visibility.Collapsed;
                    bdr_depositorC.Visibility = Visibility.Visible;
                    btn_invoices.Visibility = Visibility.Visible;
                    bdr_depositorU.Visibility = Visibility.Collapsed;
                    bdr_depositorSh.Visibility = Visibility.Collapsed;
                    requiredControlList = new List<string> { "cash", "depositFrom", "paymentProcessType", "depositorC" };
                    break;
                case 2://user
                    cb_depositorU.SelectedIndex = -1;
                    bdr_depositorV.Visibility = Visibility.Collapsed;
                    bdr_depositorC.Visibility = Visibility.Collapsed;
                    btn_invoices.Visibility = Visibility.Visible;
                    bdr_depositorU.Visibility = Visibility.Visible;
                    bdr_depositorSh.Visibility = Visibility.Collapsed;
                    requiredControlList = new List<string> { "cash", "depositFrom", "paymentProcessType", "depositorU" };
                    break;
                case 3://other
                    bdr_depositorV.Visibility = Visibility.Collapsed;
                    bdr_depositorC.Visibility = Visibility.Collapsed;
                    btn_invoices.Visibility = Visibility.Collapsed;
                    bdr_depositorU.Visibility = Visibility.Collapsed;
                    bdr_depositorSh.Visibility = Visibility.Collapsed;
                    requiredControlList = new List<string> { "cash", "depositFrom", "paymentProcessType"};
                    //HelpClass.clearComboBoxValidate(cb_depositorV, p_error_depositor);
                    //HelpClass.clearComboBoxValidate(cb_depositorC, p_error_depositor);
                    //HelpClass.clearComboBoxValidate(cb_depositorU, p_error_depositor);
                    //HelpClass.clearComboBoxValidate(cb_depositorSh, p_error_depositor);
                    cb_depositorV.Text = "";
                    cb_depositorC.Text = "";
                    cb_depositorU.Text = "";
                    cb_depositorSh.Text = "";
                    break;
                case 4://shipping company
                    cb_depositorSh.SelectedIndex = -1;
                    bdr_depositorV.Visibility = Visibility.Collapsed;
                    bdr_depositorC.Visibility = Visibility.Collapsed;
                    btn_invoices.Visibility = Visibility.Visible;
                    bdr_depositorU.Visibility = Visibility.Collapsed;
                    bdr_depositorSh.Visibility = Visibility.Visible;
                    requiredControlList = new List<string> { "cash", "depositFrom", "paymentProcessType", "depositorSh" };
                    break;
            }
            
            //    
            //        HelpClass.EndAwait(grid_main);
            //}
            //catch (Exception ex)
            //{
            //    
            //        HelpClass.EndAwait(grid_main);
            //    HelpClass.ExceptionMessage(ex, this);
            //}
        }

        private async Task fillVendors()
        {
            try
            {
                agents = await agentModel.GetActiveForAccount("v", "d");

                cb_depositorV.ItemsSource = agents;
                cb_depositorV.DisplayMemberPath = "name";
                cb_depositorV.SelectedValuePath = "agentId";
            }
            catch { }
        }

        private async Task fillCustomers()
        {
            try
            {
                agents = await agentModel.GetActiveForAccount("c", "d");

                cb_depositorC.ItemsSource = agents;
                cb_depositorC.DisplayMemberPath = "name";
                cb_depositorC.SelectedValuePath = "agentId";
            }
            catch { }
        }

        private async Task fillUsers()
        {
            try
            {
                users = await userModel.GetActiveForAccount("d");

                cb_depositorU.ItemsSource = users;
                cb_depositorU.DisplayMemberPath = "username";
                cb_depositorU.SelectedValuePath = "userId";
            }
            catch { }
        }

        private async Task fillShippingCompanies()
        {
            try
            {
                shCompanies = await shCompanyModel.GetForAccount("d");
                shCompanies = shCompanies.Where(sh => sh.deliveryType != "local");
                cb_depositorSh.ItemsSource = shCompanies;
                cb_depositorSh.DisplayMemberPath = "name";
                cb_depositorSh.SelectedValuePath = "shippingCompanyId";
            }
            catch { }
        }

        private void Btn_printInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(createPermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
                {

                    if (cashtrans.cashTransId > 0)
                    {
                        BuildvoucherReport();
                        LocalReportExtensions.PrintToPrinterbyNameAndCopy(rep, MainWindow.rep_printer_name, short.Parse(MainWindow.rep_print_count));

                    }
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }


        private void Btn_invoices_Click(object sender, RoutedEventArgs e)
        {//invoices
            //try
            //{
                invoicesLst.Clear();

                Window.GetWindow(this).Opacity = 0.2;
                wd_invoicesList w = new wd_invoicesList();

                w.agentId = 0; w.userId = 0; w.shippingCompanyId = 0;

                if (cb_depositFrom.SelectedValue.ToString() == "v")
                    w.agentId = Convert.ToInt32(cb_depositorV.SelectedValue);
                else if (cb_depositFrom.SelectedValue.ToString() == "c")
                    w.agentId = Convert.ToInt32(cb_depositorC.SelectedValue);
                else if (cb_depositFrom.SelectedValue.ToString() == "u")
                    w.userId = Convert.ToInt32(cb_depositorU.SelectedValue);
                else if (cb_depositFrom.SelectedValue.ToString() == "sh")
                    w.shippingCompanyId = Convert.ToInt32(cb_depositorSh.SelectedValue);

                w.invType = "feed";

                w.ShowDialog();
                if (w.isActive)
                {
                    tb_cash.Text = w.sum.ToString();
                    tb_cash.IsReadOnly = true;
                    invoicesLst.AddRange(w.selectedInvoices);
                }

                Window.GetWindow(this).Opacity = 1;
            //}
            //catch (Exception ex)
            //{
                
            //    HelpClass.EndAwait(grid_main);
            //    HelpClass.ExceptionMessage(ex, this);
            //}
        }
        public void BuildvoucherReport()
        {
            string addpath;
            bool isArabic = ReportCls.checkLang();
            if (isArabic)
            {
                //if (MainWindow.docPapersize == "A4")
                //{
                //    addpath = @"\Reports\Account\Ar\ArReciveReportA4.rdlc";
                //}
                //else
                //{
                //    addpath = @"\Reports\Account\Ar\ArReciveReport.rdlc";
                //}
            }
            else
            {
                //if (MainWindow.docPapersize == "A4")
                //{
                //    addpath = @"\Reports\Account\En\ReciveReportA4.rdlc";
                //}
                //else
                //{
                //    addpath = @"\Reports\Account\En\ReciveReport.rdlc";
                //}
            }

            //string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);
            //rep.ReportPath = reppath;
            rep.DataSources.Clear();
            rep.EnableExternalImages = true;
            rep.SetParameters(reportclass.fillPayReport(cashtrans));

            rep.Refresh();
        }
        private void Btn_preview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(createPermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    Window.GetWindow(this).Opacity = 0.2;

                    string pdfpath;
                    pdfpath = @"\Thumb\report\temp.pdf";
                    pdfpath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, pdfpath);

                    //
                    if (cashtrans.cashTransId > 0)
                    {
                        BuildvoucherReport();

                        LocalReportExtensions.ExportToPDF(rep, pdfpath);
                        wd_previewPdf w = new wd_previewPdf();
                        w.pdfPath = pdfpath;
                        if (!string.IsNullOrEmpty(w.pdfPath))
                        {
                            w.ShowDialog();

                            w.wb_pdfWebViewer.Dispose();

                        }
                        else
                            Toaster.ShowError(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                        Window.GetWindow(this).Opacity = 1;
                    }

                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Btn_pdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                if (MainWindow.groupObject.HasPermissionAction(createPermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    if (cashtrans.cashTransId > 0)
                    {
                        BuildvoucherReport();

                        saveFileDialog.Filter = "PDF|*.pdf;";

                        if (saveFileDialog.ShowDialog() == true)
                        {
                            string filepath = saveFileDialog.FileName;

                            LocalReportExtensions.ExportToPDF(rep, filepath);
                        }
                    }
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Cb_depositorV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ((cb_depositorV.SelectedIndex != -1) && (cb_depositorV.IsEnabled))
                    btn_invoices.IsEnabled = true;
                else
                    btn_invoices.IsEnabled = false;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Cb_depositorC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ((cb_depositorC.SelectedIndex != -1) && (cb_depositorC.IsEnabled))
                    btn_invoices.IsEnabled = true;
                else
                    btn_invoices.IsEnabled = false;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Cb_depositorU_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((cb_depositorU.SelectedIndex != -1) && (cb_depositorU.IsEnabled))
                btn_invoices.IsEnabled = true;
            else
                btn_invoices.IsEnabled = false;
        }

        private void Cb_depositorSh_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ((cb_depositorSh.SelectedIndex != -1) && (cb_depositorSh.IsEnabled))
                    btn_invoices.IsEnabled = true;
                else
                    btn_invoices.IsEnabled = false;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }


        private void Tb_EnglishDigit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {//only english and digits
            Regex regex = new Regex("^[a-zA-Z0-9. -_?]*$");
            if (!regex.IsMatch(e.Text))
                e.Handled = true;

        }
        public void BuildReport()
        {
            List<ReportParameter> paramarr = new List<ReportParameter>();

            string addpath;
            bool isArabic = ReportCls.checkLang();
            if (isArabic)
            {
                addpath = @"\Reports\Account\Ar\ArReceiptAccReport.rdlc";
            }
            else addpath = @"\Reports\Account\En\ReceiptAccReport.rdlc";
            string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);

            ReportCls.checkLang();

            clsReports.receivedAccReport(cashesQuery, rep, reppath, paramarr);
            clsReports.setReportLanguage(paramarr);
            clsReports.Header(paramarr);
            clsReports.bankdg(paramarr);

            rep.SetParameters(paramarr);

            rep.Refresh();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {//pdf
            try
            {
                HelpClass.StartAwait(grid_main);
                if (MainWindow.groupObject.HasPermissionAction(reportsPermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
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
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                
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
                if (MainWindow.groupObject.HasPermissionAction(reportsPermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    #region
                    BuildReport();
                    LocalReportExtensions.PrintToPrinterbyNameAndCopy(rep, MainWindow.rep_printer_name, short.Parse(MainWindow.rep_print_count));
                    #endregion
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Btn_preview1_Click(object sender, RoutedEventArgs e)
        {//preview
            try
            {
                HelpClass.StartAwait(grid_main);
                if (MainWindow.groupObject.HasPermissionAction(reportsPermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
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
                        w.ShowDialog();
                        w.wb_pdfWebViewer.Dispose();
                    }
                    Window.GetWindow(this).Opacity = 1;
                    #endregion
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Btn_pieChart_Click(object sender, RoutedEventArgs e)
        {//pie
            try
            {
                HelpClass.StartAwait(grid_main);
                /////////////////////
                if (MainWindow.groupObject.HasPermissionAction(reportsPermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    Window.GetWindow(this).Opacity = 0.2;
                    //cashesQueryExcel = cashesQuery.ToList();
                    win_IvcAccount win = new win_IvcAccount(cashesQuery, 1);
                    win.ShowDialog();
                    Window.GetWindow(this).Opacity = 1;
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                /////////////////////
                
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
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
            this.DataContext = new CashTransfer();
            /////////////////////////
            ///
            btn_add.IsEnabled = true;
            btn_invoices.Visibility = Visibility.Collapsed;
            cb_depositFrom.IsEnabled = true;
            cb_depositorV.IsEnabled = true;
            cb_depositorC.IsEnabled = true;
            cb_depositorU.IsEnabled = true;
            cb_depositorSh.IsEnabled = true;
            btn_invoices.IsEnabled = true;
            cb_paymentProcessType.IsEnabled = true;
            gd_card.IsEnabled = true;
            tb_docNum.IsEnabled = true;
            dp_docDate.IsEnabled = true;
            tb_docNumCheque.IsEnabled = true;
            tb_docNumCard.IsEnabled = true;
            dp_docDateCheque.IsEnabled = true;
            tb_cash.IsEnabled = true;
            tb_notes.IsEnabled = true;

            btn_image.IsEnabled = false;
            /////////////////////////
            ///
            //if (grid_doc.IsVisible)
            //{
            //    TextBox tbDocDate = (TextBox)dp_docDate.Template.FindName("PART_TextBox", dp_docDate);
            //    HelpClass.clearValidate(tbDocDate, p_error_docDate);
            //    dp_docDate.SelectedDate = null;
            //    tb_docNum.Clear();
            //    HelpClass.clearValidate(tb_docNum, p_error_docNum);
            //}
            //if (grid_cheque.IsVisible)
            //{
            //    tb_docNumCheque.Clear();
            //    dp_docDateCheque.SelectedDate = null;
            //    HelpClass.clearValidate(tb_docNumCheque, p_error_docNumCheque);
            //}
            cb_depositFrom.SelectedIndex = -1;
            bdr_depositorV.Visibility = Visibility.Collapsed;
            bdr_depositorC.Visibility = Visibility.Collapsed;
            bdr_depositorU.Visibility = Visibility.Collapsed;
            bdr_depositorSh.Visibility = Visibility.Collapsed;
            gd_card.Visibility = Visibility.Collapsed;
            //p_error_docNumCheque.Visibility = Visibility.Collapsed;
            cb_paymentProcessType.SelectedIndex = -1;
            //tb_cash.Clear();
            //tb_notes.Clear();
            //tb_docNumCard.Clear();
            //tb_docNum.Clear();
            //tb_docNumCheque.Clear();
            //tb_transNum.Text = "";
            tb_cash.IsReadOnly = false;
            grid_doc.Visibility = Visibility.Collapsed;
            tb_docNumCard.Visibility = Visibility.Collapsed;
            grid_cheque.Visibility = Visibility.Collapsed;
            //HelpClass.clearValidate(tb_cash, p_error_cash);
            //HelpClass.clearComboBoxValidate(cb_depositFrom, p_error_depositFrom);
            //HelpClass.clearComboBoxValidate(cb_depositorV, p_error_depositor);
            //HelpClass.clearComboBoxValidate(cb_depositorC, p_error_depositor);
            //HelpClass.clearComboBoxValidate(cb_depositorU, p_error_depositor);
            //HelpClass.clearComboBoxValidate(cb_depositorSh, p_error_depositor);
            //HelpClass.clearComboBoxValidate(cb_paymentProcessType, p_error_paymentProcessType);
            //HelpClass.clearTextBlockValidate(txt_card, p_error_card);
            //HelpClass.clearValidate(tb_docNum, p_error_docNum);
            //HelpClass.clearValidate(tb_docNumCheque, p_error_docNumCheque);
            //HelpClass.clearValidate(tb_docNumCard, p_error_docCard);
            //HelpClass.clearValidate(tb_docNumCheque, p_error_docNumCheque);


            // last 
            HelpClass.clearValidate(requiredControlList, this);
        }
        string input;
        decimal _decimal = 0;
        private void Number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {//only  digits
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
        {//only english and digits
            try
            {
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
