using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using netoaster;
using Restaurant.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
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
using System.IO;
using Restaurant.View.windows;

namespace Restaurant.View.accounts
{
    /// <summary>
    /// Interaction logic for uc_subscriptions.xaml
    /// </summary>
    public partial class uc_subscriptions : UserControl
     {
        public uc_subscriptions()
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

        private static uc_subscriptions _instance;
        public static uc_subscriptions Instance
        {
            get
            {
                if(_instance is null)
                    _instance = new uc_subscriptions();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        IEnumerable<AgentMembershipCash> subscriptions;
        IEnumerable<AgentMembershipCash> subscriptionsQuery;
        //IEnumerable<SubscriptionFees> subscriptionsQueryExcel;
        CashTransfer cashtrans = new CashTransfer();
        AgentMembershipCash subscription = new AgentMembershipCash();
        string searchText = "";
        byte tgl_subscriptionState;
        ReportCls reportclass = new ReportCls();
        LocalReport rep = new LocalReport();
        SaveFileDialog saveFileDialog = new SaveFileDialog();

        string createPermission = "subscriptions_create";
        string reportsPermission = "subscriptions_reports";

        public static List<string> requiredControlList;

        #region card
        List<Button> cardBtnList = new List<Button>();
        List<Ellipse> cardEllipseList = new List<Ellipse>();
        IEnumerable<Card> cards;
        Card cardModel = new Card();
        bool hasProcessNum = false;

        static private int _SelectedCard = -1;

        void InitializeCardsPic(IEnumerable<Card> cards)
        {
            #region cardImageLoad
            dkp_cards.Children.Clear();
            int userCount = 0;
            foreach (var item in cards)
            {
                #region Button
                Button button = new Button();
                button.DataContext = item;
                button.Tag = item.cardId;
                button.Padding = new Thickness(0, 0, 0, 0);
                button.Margin = new Thickness(2.5, 5, 2.5, 5);
                button.Background = null;
                button.BorderBrush = null;
                button.Height = 35;
                button.Width = 35;
                button.Click += card_Click;

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
                ellipse.Tag = item.cardId;
                userImageLoad(ellipse, item.image);
                Grid.SetColumn(ellipse, userCount);
                grid.Children.Add(ellipse);
                cardEllipseList.Add(ellipse);
                #endregion
                #endregion

                button.Content = grid;
                #endregion
                dkp_cards.Children.Add(button);
                cardBtnList.Add(button);

            }
            #endregion
        }

        void card_Click(object sender, RoutedEventArgs e)
        {
            HelpClass.clearValidate(requiredControlList, this);
            var button = sender as Button;
            _SelectedCard = int.Parse(button.Tag.ToString());

            Card card = button.DataContext as Card;

            txt_card.Text = card.name;
            
            if (card.hasProcessNum)
            {
                tb_processNum.Visibility = Visibility.Visible;
                hasProcessNum = true;
                requiredControlList = new List<string> { "customerId", "monthsCount", "amount", "paymentProcessType", "processNum" };
            }
            else
            {
                tb_processNum.Visibility = Visibility.Collapsed;
                hasProcessNum = false;
                requiredControlList = new List<string> { "customerId", "monthsCount", "amount", "paymentProcessType" };
            }
            //set border color
            foreach (var el in cardEllipseList)
            {
                if ((int)el.Tag == (int)button.Tag)
                    el.Stroke = Application.Current.Resources["MainColorBlue"] as SolidColorBrush;
                else
                    el.Stroke = Application.Current.Resources["MainColorOrange"] as SolidColorBrush;
            }
            HelpClass.validate(requiredControlList, this);
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
        #endregion

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {//load
            //try
            //{
            //    HelpClass.StartAwait(grid_main);

                requiredControlList = new List<string> { "customerId" , "monthsCount" , "amount" , "paymentProcessType" };

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
                dp_searchEndDate.SelectedDateChanged += this.dp_SelectedStartDateChanged;

                FillCombo.FillDefaultPayType_cashChequeCard(cb_paymentProcessType);

                await fillCustomersToPay();

                #region fill card combo
                try
                {
                    cards = await cardModel.GetAll();
                    InitializeCardsPic(cards);
                }
                catch { }
                #endregion

                Keyboard.Focus(cb_customerId);

                await RefreshSubscriptionsList();
                await Search();

                await Clear();

            //    HelpClass.EndAwait(grid_main);
            //}
            //catch (Exception ex)
            //{
            //    HelpClass.EndAwait(grid_main);
            //    HelpClass.ExceptionMessage(ex, this);
            //}
        }

        async Task fillCustomersToPay()
        {
            cb_customerId.ItemsSource = await subscription.GetAgentToPay();
            cb_customerId.DisplayMemberPath = "agentName";
            cb_customerId.SelectedValuePath = "agentId";
            cb_customerId.SelectedIndex = -1;
        }
        private async void dp_SelectedStartDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //try
            //{
            //    HelpClass.StartAwait(grid_main);

                await RefreshSubscriptionsList();
                await Search();

            //    HelpClass.EndAwait(grid_main);
            //}
            //catch (Exception ex)
            //{
            //    HelpClass.EndAwait(grid_main);
            //    HelpClass.ExceptionMessage(ex, this);
            //}
        }

        private void translate()
        {
            txt_title.Text = AppSettings.resourcemanager.GetString("trSubscription");
            txt_baseInformation.Text = AppSettings.resourcemanager.GetString("trBaseInformation");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_search, AppSettings.resourcemanager.GetString("trSearchHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_transNum, AppSettings.resourcemanager.GetString("trTransNumberHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_customerId, AppSettings.resourcemanager.GetString("trCustomerHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_monthsCount, AppSettings.resourcemanager.GetString("trMonthCount")+"...");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_amount, AppSettings.resourcemanager.GetString("trAmountHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_paymentProcessType, AppSettings.resourcemanager.GetString("trPaymentProcessType"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_notes, AppSettings.resourcemanager.GetString("trNoteHint"));

            dg_subscription.Columns[0].Header = AppSettings.resourcemanager.GetString("trTransferNumberTooltip");
            dg_subscription.Columns[1].Header = AppSettings.resourcemanager.GetString("trCustomer");
            dg_subscription.Columns[2].Header = AppSettings.resourcemanager.GetString("trSubscriptionType");
            dg_subscription.Columns[3].Header = AppSettings.resourcemanager.GetString("trExpireDate");
            dg_subscription.Columns[4].Header = AppSettings.resourcemanager.GetString("trAmount");

            btn_clear.ToolTip = AppSettings.resourcemanager.GetString("trClear");

            tt_report.Content = AppSettings.resourcemanager.GetString("trPdf");
            tt_excel.Content = AppSettings.resourcemanager.GetString("trExcel");
            tt_count.Content = AppSettings.resourcemanager.GetString("trCount");

            btn_save.Content = AppSettings.resourcemanager.GetString("trSave");
        }

        #region events
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
        private async void Tgl_isActive_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                if (subscriptions is null)
                    await RefreshSubscriptionsList();
                tgl_subscriptionState = 1;
                await Search();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Tgl_isActive_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                if (subscriptions is null)
                    await RefreshSubscriptionsList();
                tgl_subscriptionState = 0;
                await Search();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_clear_Click(object sender, RoutedEventArgs e)
        {
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
        private async void Dg_subscription_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//selection
            //try
            //{
            //    HelpClass.StartAwait(grid_main);
         
                if (dg_subscription.SelectedIndex != -1)
                {
                    subscription = dg_subscription.SelectedItem as AgentMembershipCash;
                    btn_save.IsEnabled = false;
                    if (subscription != null)
                    {
                        if (subscription.subscriptionType.Equals("o"))
                        {
                            cb_customerId.Visibility = Visibility.Collapsed;
                            tb_customer.Visibility = Visibility.Visible;
                            tb_customer.Text = subscription.agentName;
                            tb_amount.Text = subscription.Amount.ToString();
                            cb_monthsCount.SelectedIndex = -1;
                            cb_monthsCount.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            cb_customerId.Visibility = Visibility.Visible;
                            tb_customer.Visibility = Visibility.Collapsed;
                            tb_customer.Clear();
                            cb_monthsCount.Visibility = Visibility.Visible;
                            cb_customerId.SelectedValue = subscription.agentId;
                            //Cb_customerId_SelectionChanged(cb_customerId,null);
                            cb_monthsCount.SelectedValue = subscription.monthsCount;
                            //Cb_monthsCount_SelectionChanged(cb_monthsCount , null);
                        }
                        btn_save.IsEnabled = false;

                        //ag = cb_customerId.SelectedItem as AgenttoPayCash;

                        cb_paymentProcessType.SelectedValue = subscription.processType;
                        try
                        {
                            cashtrans = await cashtrans.GetByID(subscription.cashTransId.Value);
                        }
                        catch { }


                        tb_discount.Text = subscription.discountValue.ToString();

                        tb_processNum.Text = subscription.docNum;

                        tb_docNumCheque.Text = subscription.docNum;
                    }
                }
                HelpClass.clearValidate(requiredControlList, this);
            //    HelpClass.EndAwait(grid_main);
            //}
            //catch (Exception ex)
            //{
            //    HelpClass.EndAwait(grid_main);
            //    HelpClass.ExceptionMessage(ex, this);
            //}
        }

        private async void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {//refresh
            try
            {
                HelpClass.StartAwait(grid_main);

                tb_search.Text = "";
                searchText = "";
                await RefreshSubscriptionsList();
                await Search();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion

        #region Refresh & Search
        async Task Search()
        {
            if (subscriptions is null)
                await RefreshSubscriptionsList();

            searchText = tb_search.Text.ToLower();

            if (chb_all.IsChecked == false)
            {
                subscriptionsQuery = subscriptions
                .Where(s =>
                (
                s.transNum.ToLower().Contains(searchText) ||
                s.agentName.ToLower().Contains(searchText) ||
                s.subscriptionType.ToLower().Contains(searchText) ||
                s.updateDate.ToString().ToLower().Contains(searchText) ||
                s.Amount.ToString().ToLower().Contains(searchText)
                )
                &&
                s.isActive == tgl_subscriptionState
                && s.updateDate.Value.Date >= dp_searchStartDate.SelectedDate.Value.Date
                && s.updateDate.Value.Date <= dp_searchEndDate.SelectedDate.Value.Date
                );
            }
            else
            {
                subscriptionsQuery = subscriptions
               .Where(s =>
               (
               s.transNum.ToLower().Contains(searchText) ||
               s.agentName.ToLower().Contains(searchText) ||
               s.subscriptionType.ToLower().Contains(searchText) ||
               s.updateDate.ToString().ToLower().Contains(searchText) ||
               s.Amount.ToString().ToLower().Contains(searchText)
               )
               &&
               s.isActive == tgl_subscriptionState
               );
            }
            RefreshCustomersView();
        }
        async Task<IEnumerable<AgentMembershipCash>> RefreshSubscriptionsList()
        {
            subscriptions = await subscription.GetAll();
            subscriptions = subscriptions.Where(s => s.subscriptionType != "f");
            return subscriptions;
        }
        void RefreshCustomersView()
        {
            dg_subscription.ItemsSource = subscriptionsQuery;
            txt_count.Text = subscriptionsQuery.Count().ToString();
        }
        #endregion

        #region validate - clearValidate - textChange - lostFocus - . . . . 
        async Task Clear()
        {
            cb_customerId.IsEnabled = true ;
            cb_customerId.SelectedIndex = -1;
            cb_paymentProcessType.SelectedIndex = -1;
            btn_save.IsEnabled = true;
            tb_discount.Clear();
            //clear border color
            foreach (var el in cardEllipseList)
            {
                el.Stroke = Application.Current.Resources["MainColorOrange"] as SolidColorBrush;
            }
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

        decimal _discount = 0 , totalNet = 0;
        private void ValidateEmpty_TextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                HelpClass.validate(requiredControlList, this);
                {
                    string name = sender.GetType().Name;
                    if (name == "TextBox")
                        if ((sender as TextBox).Name == "tb_discount")
                        try
                        {
                            if (!tb_discount.Text.Equals(""))
                                _discount = decimal.Parse(tb_discount.Text);
                            totalNet = decimal.Parse(tb_amount.Text) - _discount;
                            txt_total.Text = totalNet.ToString();
                        }
                        catch (Exception ex)
                        {
                        }
                }
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


        #region barcode
        /*List<ItemUnit> barcodesList;
        static private int _InternalCounter = 0;
        private Boolean checkBarcodeValidity(string barcode)
        {
            if (FillCombo.itemUnitList != null)
            {
                var exist = FillCombo.itemUnitList.Where(x => x.barcode == barcode && x.itemId != item.itemId).FirstOrDefault();
                if (exist != null)
                    return false;
                else
                    return true;
            }
            return true;
        }
        */
        private void Tb_barcode_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            try
            {
                TextBox tb = (TextBox)sender;
                string barCode = tb_barcode.Text;
                if (e.Key == Key.Return && barCode.Length == 13)
                {
                    if (isBarcodeCorrect(barCode) == false)
                    {
                        item.barcode = "";
                        this.DataContext = item;

                    }
                    else
                        drawBarcode(barCode);
                }
                else if (barCode.Length == 13 || barCode.Length == 12)
                    drawBarcode(barCode);
                else
                    drawBarcode("");
            }
            catch (Exception ex)
            {

                HelpClass.ExceptionMessage(ex, this);
            }
            */
        }
        /*
        private bool isBarcodeCorrect(string barCode)
        {
            char checkDigit;
            char[] barcodeData;

            char cd = barCode[0];
            barCode = barCode.Substring(1);
            barcodeData = barCode.ToCharArray();
            checkDigit = Mod10CheckDigit(barcodeData);

            if (checkDigit != cd)
            {
                Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trErrorBarcodeToolTip"), animation: ToasterAnimation.FadeIn);
                return false;
            }
            else
                return true;
        }
        public static char Mod10CheckDigit(char[] data)
        {
            // Start the checksum calculation from the right most position.
            int factor = 3;
            int weight = 0;
            int length = data.Length;

            for (int i = 0; i <= length - 1; i++)
            {
                weight += (data[i] - '0') * factor;
                factor = (factor == 3) ? 1 : 3;
            }

            return (char)(((10 - (weight % 10)) % 10) + '0');

        }
        private void drawBarcode(string barcodeStr)
        {
            try
            {
                // configur check sum metrics
                BarcodeSymbology s = BarcodeSymbology.CodeEan13;

                BarcodeDraw drawObject = BarcodeDrawFactory.GetSymbology(s);

                BarcodeMetrics barcodeMetrics = drawObject.GetDefaultMetrics(60);
                barcodeMetrics.Scale = 2;

                if (barcodeStr != "")
                {
                    if (barcodeStr.Length == 13)
                        barcodeStr = barcodeStr.Substring(1);//remove check sum from barcode string
                    var barcodeImage = drawObject.Draw(barcodeStr, barcodeMetrics);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        barcodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] imageBytes = ms.ToArray();

                        img_barcode.Source = ImageProcess.ByteToImage(imageBytes);
                    }
                }
                else
                    img_barcode.Source = null;
            }
            catch { img_barcode.Source = null; }

        }
        private async void generateBarcode()
        {
            string barcodeString = "";
            barcodeString = generateRandomBarcode();
            if (FillCombo.itemUnitList != null)
            {
                if (!checkBarcodeValidity(barcodeString))
                    barcodeString = generateRandomBarcode();
            }
            item.barcode = barcodeString;
            this.DataContext = item;
            //tb_barcode.Text = barcodeString;
            HelpClass.validateEmpty("trErrorEmptyBarcodeToolTip", p_error_barcode);
            drawBarcode(tb_barcode.Text);
        }
        static public string generateRandomBarcode()
        {
            var now = DateTime.Now;

            var days = (int)(now - new DateTime(2000, 1, 1)).TotalDays;
            var seconds = (int)(now - DateTime.Today).TotalSeconds;

            var counter = _InternalCounter++ % 100;
            string randomBarcode = days.ToString("00000") + seconds.ToString("00000") + counter.ToString("00");
            char[] barcodeData = randomBarcode.ToCharArray();
            char checkDigit = Mod10CheckDigit(barcodeData);
            return checkDigit + randomBarcode;

        }
        */
        #endregion

        private void Cb_customerId_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                cb_customerId.ItemsSource = FillCombo.customersList.Where(x => x.name.Contains(cb_customerId.Text));

            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        string _docNum = "";
        private async void Cb_paymentProcessType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //try
            //{
                //    HelpClass.StartAwait(grid_main);
                //TimeSpan elapsed = (DateTime.Now - _lastKeystroke);
                //if (elapsed.TotalMilliseconds > 100 && cb_paymentProcessType.SelectedIndex != -1)
                //{
                //    _SelectedPaymentType = cb_paymentProcessType.SelectedValue.ToString();
                //}
                //else
                //{
                //    cb_paymentProcessType.SelectedValue = _SelectedPaymentType;
                //}
                HelpClass.clearValidate(requiredControlList, this);
                if (requiredControlList.Contains("docNumCheque"))
                    requiredControlList.Remove("docNumCheque");
                if (requiredControlList.Contains("processNum"))
                    requiredControlList.Remove("processNum");
                if (requiredControlList.Contains("card"))
                    requiredControlList.Remove("card");
                switch (cb_paymentProcessType.SelectedIndex)
                {
                    case 0://cash
                        bdr_cheque.Visibility = Visibility.Collapsed;
                        tb_docNumCheque.Clear();
                        dp_docDateCheque.SelectedDate = null;
                        bdr_card.Visibility = Visibility.Collapsed;
                        _SelectedCard = 0;
                        _docNum = "";
                        tb_processNum.Clear();
                        txt_card.Text = "";
                        //requiredControlList = new List<string> { "customerId", "monthsCount", "amount", "paymentProcessType" };
                        break;

                    case 1://cheque
                        bdr_cheque.Visibility = Visibility.Visible;
                        bdr_card.Visibility = Visibility.Collapsed;
                        _SelectedCard = -1;
                        tb_processNum.Clear();
                        txt_card.Text = "";
                        if (!requiredControlList.Contains("docNumCheque"))
                            requiredControlList.Add("docNumCheque");
                        break;

                    case 2://card
                        bdr_cheque.Visibility = Visibility.Collapsed;
                        bdr_card.Visibility = Visibility.Visible;

                        if (!requiredControlList.Contains("processNum"))
                            requiredControlList.Add("processNum");
                        if (!requiredControlList.Contains("card"))
                            requiredControlList.Add("card");
                        try
                        {
                            if (subscription.cardId != null)
                            {
                                Button btn = cardBtnList.Where(c => (int)c.Tag == subscription.cardId.Value).FirstOrDefault();
                                card_Click(btn, null);
                            }
                        }
                        catch { }
                        break;
                    case -1:
                        bdr_cheque.Visibility = Visibility.Collapsed;
                        tb_docNumCheque.Clear();
                        dp_docDateCheque.SelectedDate = null;
                        bdr_card.Visibility = Visibility.Collapsed;
                        _SelectedCard = 0;
                        _docNum = "";
                        tb_processNum.Clear();
                        txt_card.Text = "";
                        requiredControlList = new List<string> { "customerId", "monthsCount", "amount", "paymentProcessType" };
                        break;
            }

                HelpClass.validate(requiredControlList, this);

            //    HelpClass.EndAwait(grid_main);
            //}
            //catch (Exception ex)
            //{

            //    HelpClass.EndAwait(grid_main);
            //    HelpClass.ExceptionMessage(ex, this);
            //}
        }

        private async void Btn_save_Click(object sender, RoutedEventArgs e)
        {//save
            try
            {
                HelpClass.StartAwait(grid_main);

                if (FillCombo.groupObject.HasPermissionAction(createPermission, FillCombo.groupObjects, "one"))
                {
                    
                    if (HelpClass.validate(requiredControlList, this))
                    {
                        #region cashTransfer

                        if (bdr_cheque.Visibility == Visibility.Visible)
                            _docNum = tb_docNumCheque.Text;
                        else if(bdr_card.Visibility == Visibility.Visible)
                            _docNum = tb_processNum.Text;

                        CashTransfer cashtrans = new CashTransfer();

                        cashtrans.transType = "d";
                        cashtrans.posId = MainWindow.posLogin.posId;
                        cashtrans.userId = null;
                        cashtrans.agentId = (int)cb_customerId.SelectedValue;
                        cashtrans.invId = null;
                        cashtrans.transNum = await cashtrans.generateCashNumber(cashtrans.transType + "c");////????????
                        cashtrans.cash = decimal.Parse(tb_amount.Text);
                        cashtrans.createUserId = MainWindow.userLogin.userId;
                        cashtrans.updateUserId = MainWindow.userLogin.userId;
                        cashtrans.notes = "";
                        cashtrans.posIdCreator = null;
                        cashtrans.isConfirm = 0;
                        cashtrans.cashTransIdSource = null;
                        cashtrans.side = "c";
                        cashtrans.docName = "";
                        cashtrans.docNum = _docNum;
                        cashtrans.docImage = "";
                        cashtrans.bankId = null;
                        cashtrans.bankName = "";
                        cashtrans.agentName = null;
                        cashtrans.usersName = null;
                        cashtrans.processType = cb_paymentProcessType.SelectedValue.ToString();
                        if(bdr_card.Visibility == Visibility.Visible)
                            cashtrans.cardId = _SelectedCard;
                        cashtrans.shippingCompanyId = null;
                        #endregion

                        #region agentCash
                        subscription = new AgentMembershipCash();

                        subscription.agentMembershipsId = 0;
                        subscription.subscriptionFeesId = _subscriptionFeesId;
                        subscription.cashTransId = cashtrans.cashTransId;
                        subscription.membershipId = ag.membershipId;
                        subscription.agentId = (int)cb_customerId.SelectedValue;
                        subscription.startDate = ag.startDate;
                        subscription.EndDate = ag.updateDate;
                        subscription.notes = "";
                        subscription.createUserId = MainWindow.userLogin.userId;
                        subscription.updateUserId = MainWindow.userLogin.userId;
                        subscription.isActive = 1;
                        subscription.Amount = decimal.Parse(tb_amount.Text);
                        try
                        {
                            subscription.discountValue = decimal.Parse(tb_discount.Text);
                        }
                        catch
                        {
                            subscription.discountValue = 0;
                        }
                        subscription.total = decimal.Parse(txt_total.Text);
                        subscription.monthsCount = _monthCount;
                        subscription.agentName = ag.agentName;
                        subscription.agentcode = ag.agentcode;
                        subscription.agentcompany = ag.agentcompany;
                        subscription.agenttype = ag.agenttype;
                        subscription.membershipName = ag.membershipName;
                        subscription.membershipcode = ag.membershipcode;
                        subscription.transType = cashtrans.transType;
                        subscription.transNum = cashtrans.transNum;
                        subscription.membershipisActive = ag.membershipisActive;
                        subscription.subscriptionType = ag.subscriptionType;
                        #endregion

                        int res = await subscription.Savepay(subscription, cashtrans);

                        if (res <= 0)
                            Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                        else
                        {
                            Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);

                            
                            await RefreshSubscriptionsList();
                            await Search();
                        }
                      
                    }
                    
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

        private void Chb_all_Checked(object sender, RoutedEventArgs e)
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

        AgenttoPayCash ag = new AgenttoPayCash();

        private void Cb_monthsCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//select month
            try
            {
                subFee = cb_monthsCount.SelectedItem as SubscriptionFees;
                if (subFee != null)
                {
                    //this.DataContext = 
                    txt_total.Text = subFee.Amount.ToString();
                    tb_amount.Text = subFee.Amount.ToString();
                    _monthCount = (int)cb_monthsCount.SelectedValue;
                    _subscriptionFeesId = subFee.subscriptionFeesId;
                }
                else
                {
                    tb_amount.Text = "";
                    txt_total.Text = "0";
                    _monthCount = 0;
                    _subscriptionFeesId = 0;
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        SubscriptionFees subFee = new SubscriptionFees();
        IEnumerable<SubscriptionFees> subFees ;
        int _monthCount = 0 , _subscriptionFeesId;

     

        private async void Cb_customerId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//select customer
            //try
            //{
                #region clear
                tb_amount.Text = "";
                txt_total.Text = "0";
                tb_discount.Text = "";
                cb_paymentProcessType.SelectedIndex = -1;
                cb_monthsCount.SelectedIndex = -1;
                HelpClass.clearValidate(requiredControlList, this);
                #endregion

                ag = cb_customerId.SelectedItem as AgenttoPayCash;
                if (ag != null)
                {
                    btn_save.IsEnabled = true;
                    if ((ag.subscriptionType == "o") || (ag.subscriptionType == "f"))
                    {
                        cb_monthsCount.SelectedIndex = -1;
                        bdr_monthCount.Visibility = Visibility.Collapsed;
                        _monthCount = 0;
                        tb_amount.Text = ag.Amount.ToString();
                        txt_total.Text = tb_amount.Text;
                        _subscriptionFeesId = ag.subscriptionFeesId.Value;
                        if (requiredControlList.Contains("monthsCount"))
                            requiredControlList.Remove("monthsCount");
                    }
                    else
                    {
                        if (!requiredControlList.Contains("monthsCount"))
                            requiredControlList.Add("monthsCount");
                        bdr_monthCount.Visibility = Visibility.Visible;

                        #region fill month count 
                        subFee = new SubscriptionFees();
                        subFees = await subFee.GetAll();
                        subFees = subFees.Where(s => s.membershipId == ag.membershipId).ToList();
                        string str = "";
                        if (ag.subscriptionType == "m")
                        {
                            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_monthsCount, AppSettings.resourcemanager.GetString("trMonthCount") + "...");
                            str = AppSettings.resourcemanager.GetString("trMonth");
                        }
                        else if (ag.subscriptionType == "y")
                        {
                            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_monthsCount, AppSettings.resourcemanager.GetString("trYearCount") + "...");
                            str = AppSettings.resourcemanager.GetString("trYear");
                        }

                        foreach (var m in subFees)
                        {
                            m.notes = m.monthsCount + " " + str;
                        }

                        cb_monthsCount.DisplayMemberPath = "notes";
                        cb_monthsCount.SelectedValuePath = "monthsCount";
                        cb_monthsCount.ItemsSource = subFees;
                        #endregion
                    }
                }

            //}
            //catch (Exception ex)
            //{
            //    HelpClass.ExceptionMessage(ex, this);
            //}
        }


        #region report
        public void BuildReport()
        {
            List<ReportParameter> paramarr = new List<ReportParameter>();

            string addpath;
            bool isArabic = ReportCls.checkLang();
            if (isArabic)
            {
                addpath = @"\Reports\Account\report\Ar\ArSubscription.rdlc";
            }
            else
            {
                addpath = @"\Reports\Account\report\En\EnSubscription.rdlc";
            }
            string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);

            ReportCls.checkLang();

            clsReports.SubscriptionAcc(subscriptionsQuery, rep, reppath, paramarr);
            clsReports.setReportLanguage(paramarr);
            clsReports.Header(paramarr);
            clsReports.bankdg(paramarr);

            rep.SetParameters(paramarr);

            rep.Refresh();
        }

        private void Btn_print_Click(object sender, RoutedEventArgs e)
        {
            //print
            try
            {
                HelpClass.StartAwait(grid_main);
                if (FillCombo.groupObject.HasPermissionAction(reportsPermission, FillCombo.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    #region
                    BuildReport();
                    LocalReportExtensions.PrintToPrinterbyNameAndCopy(rep, AppSettings.rep_printer_name, short.Parse(AppSettings.rep_print_count));
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

        private void Btn_exportToExcel_Click(object sender, RoutedEventArgs e)
        {
            //excel
            try
            {
                HelpClass.StartAwait(grid_main);

                if (FillCombo.groupObject.HasPermissionAction(createPermission, FillCombo.groupObjects, "one") || HelpClass.isAdminPermision())
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
            //preview
            try
            {
                HelpClass.StartAwait(grid_main);
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
            //pdf
            try
            {
                HelpClass.StartAwait(grid_main);
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
               // HelpClass.StartAwait(grid_main);
               // /////////////////////
               // if (FillCombo.groupObject.HasPermissionAction(reportsPermission, FillCombo.groupObjects, "one") || HelpClass.isAdminPermision())
               // {
               //     Window.GetWindow(this).Opacity = 0.2;
               //     //cashesQueryExcel = cashesQuery.ToList();
               //win_IvcAccount win = new win_IvcAccount(subscriptionsQuery, 1);
               //     // // w.ShowInTaskbar = false;
               //     win.ShowDialog();
               //     Window.GetWindow(this).Opacity = 1;
               // }
               // else
               //     Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
               // /////////////////////

               // HelpClass.EndAwait(grid_main);
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
