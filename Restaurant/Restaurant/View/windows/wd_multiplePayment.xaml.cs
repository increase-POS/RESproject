using netoaster;
using POS.Classes;
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
using System.Windows.Resources;
using System.Windows.Shapes;

namespace POS.View.windows
{
    /// <summary>
    /// Interaction logic for wd_multiplePayment.xaml
    /// </summary>
    public partial class wd_multiplePayment : Window
    {
        public wd_multiplePayment()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                SectionData.ExceptionMessage(ex, this);
            }
        }
        static private object _Sender;
        public bool isOk { get; set; }
        public bool isPurchase { get; set; }
        public bool hasCredit { get; set; }
        public decimal creditValue { get; set; }
        ImageBrush brush = new ImageBrush();
        static private string _SelectedPaymentType = "cash";
        static private int _SelectedCard = -1;
        public List<CashTransfer> listPayments = new List<CashTransfer>();
        CashTransfer cashTrasnfer;
        Card cardModel = new Card();
        public IEnumerable<Card> cards;
        public Invoice invoice = new Invoice();
        bool amountIsValid = false;
        private  void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
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


                configurProcessType();

                // get it from invoice
                loading_fillCardCombo();
              
                //////////////////////////
                //invoice.agentId
                //////////////////////////
                tb_moneySympol1.Text = tb_moneySympol2.Text = MainWindow.Currency;
                invoice.paid = 0;
                tb_cash.Text = tb_total.Text = invoice.totalNet.ToString();

            }
            catch (Exception ex)
            {

                SectionData.ExceptionMessage(ex, this);
            }
        }
        private void translate()
        {

            txt_title.Text = MainWindow.resourcemanager.GetString("trMultiplePayment");

            //    MaterialDesignThemes.Wpf.HintAssist.SetHint(txb_search, MainWindow.resourcemanager.GetString("trSearchHint"));
            //    btn_select.Content = MainWindow.resourcemanager.GetString("trSelect");
        }
        private void configurProcessType()
        {
            cb_paymentProcessType.DisplayMemberPath = "Text";
            cb_paymentProcessType.SelectedValuePath = "Value";
            if (invoice.invType.Equals("sbd"))
            {
                var typelist = new[] {
                 new { Text = MainWindow.resourcemanager.GetString("trCash")       , Value = "cash" },
                };
                cb_paymentProcessType.ItemsSource = typelist;
            }
            else
            {
                var typelist = new[] {
                new { Text = MainWindow.resourcemanager.GetString("trCash")       , Value = "cash" },
                new { Text = MainWindow.resourcemanager.GetString("trAnotherPaymentMethods") , Value = "card" },
                 };

                cb_paymentProcessType.ItemsSource = typelist;
            }
            cb_paymentProcessType.SelectedIndex = 0;
        }
        private void Btn_colse_Click(object sender, RoutedEventArgs e)
        {
            isOk = false;
            this.Close();
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
        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return)
                {
                    Btn_save_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                SectionData.ExceptionMessage(ex, this);
            }
        }
        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!isPurchase && 
               (invoice.paid >= invoice.totalNet || (hasCredit == true && creditValue > invoice.totalNet - invoice.paid) || (hasCredit == true && creditValue ==0)))
                {
                    if (invoice.totalNet - invoice.paid > 0)
                    {
                        cashTrasnfer = new CashTransfer();

                        ///////////////////////////////////////////////////
                        cashTrasnfer.agentId = invoice.agentId;
                        cashTrasnfer.invId = invoice.invoiceId;
                        cashTrasnfer.processType = "balance";
                        cashTrasnfer.cash = invoice.totalNet - invoice.paid;
                        listPayments.Add(cashTrasnfer);
                    }
                    isOk = true;
                    this.Close();
                }
                else if(isPurchase && hasCredit == true)
                {
                    if (invoice.totalNet - invoice.paid > 0)
                    {
                        cashTrasnfer = new CashTransfer();

                        ///////////////////////////////////////////////////
                        cashTrasnfer.agentId = invoice.agentId;
                        cashTrasnfer.invId = invoice.invoiceId;
                        cashTrasnfer.processType = "balance";
                        cashTrasnfer.cash = invoice.totalNet - invoice.paid;
                        listPayments.Add(cashTrasnfer);
                    }
                    //lst_payments.Items.Add(s);
                    ///////////////////////////////////////////////////
                    isOk = true;
                    this.Close();
                }
                else // if (invoice.paid < invoice.totalNet && hasCredit == false &&)
                {
                    isOk = false;
                    Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trAmountPaidEqualInvoiceValue"), animation: ToasterAnimation.FadeIn);
                }

            }
            catch (Exception ex)
            {
                SectionData.ExceptionMessage(ex, this);
            }
        }

        private void input_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = sender.GetType().Name;
                if (name == "TextBox")
                {
                    if ((sender as TextBox).Name == "tb_processNum")
                        SectionData.validateEmptyTextBox((TextBox)sender, p_errorProcessNum, tt_errorProcessNum, "trEmptyProcessNumToolTip");
                }
                if (name == "ComboBox")
                {

                    if ((sender as ComboBox).Name == "cb_paymentProcessType")
                        SectionData.validateEmptyComboBox((ComboBox)sender, p_errorpaymentProcessType, tt_errorpaymentProcessType, "trErrorEmptyPaymentTypeToolTip");

                }
                if (name == "TextBlock")
                {

                    if ((sender as TextBlock).Name == "txt_card")
                        SectionData.validateEmptyTextBlock((TextBlock)sender, p_errorCard, tt_errorCard, "trSelectCreditCard");
                }

            }
            catch (Exception ex)
            {
                SectionData.ExceptionMessage(ex, this);
            }
        }
        private void Cb_paymentProcessType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                switch (cb_paymentProcessType.SelectedIndex)
                {
                    case 0://cash
                        gd_card.Visibility = Visibility.Collapsed;
                        tb_processNum.Clear();
                        _SelectedCard = -1;
                        txt_card.Text = "";
                        SectionData.clearTextBlockValidate(txt_card, p_errorCard);
                        SectionData.clearValidate(tb_processNum, p_errorCard);
                        break;
                    case 1://card
                        gd_card.Visibility = Visibility.Visible;
                        break;
                }

            }
            catch (Exception ex)
            {

                SectionData.ExceptionMessage(ex, this);
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
                //button.DataContext = item.name;
                button.DataContext = item;
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
            try
            {
                var button = sender as Button;
                _SelectedCard = int.Parse(button.Tag.ToString());
                //txt_card.Text = button.DataContext.ToString();
                Card card = button.DataContext as Card;
                txt_card.Text = card.name;
                if (card.hasProcessNum.Value)
                    tb_processNum.Visibility = Visibility.Visible;
                else
                    tb_processNum.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                SectionData.ExceptionMessage(ex, this);
            }
        }
        void loading_fillCardCombo()
        {
            try
            {
                InitializeCardsPic(cards);
            }
            catch (Exception)
            { }
        }
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
        private void PreventSpaces(object sender, KeyEventArgs e)
        {
            try
            {
                e.Handled = e.Key == Key.Space;
            }
            catch (Exception ex)
            {
                SectionData.ExceptionMessage(ex, this);
            }
        }
        private void DecimalValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            try
            {
                var regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
                if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                    e.Handled = false;

                else
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                SectionData.ExceptionMessage(ex, this);
            }
        }
        private void Tb_EnglishDigit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {//only english and digits
            Regex regex = new Regex("^[a-zA-Z0-9. -_?]*$");
            if (!regex.IsMatch(e.Text))
                e.Handled = true;
        }
        private void Tb_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _Sender = sender;
                string name = sender.GetType().Name;
                validateEmpty(name, sender);
                var txb = sender as TextBox;
                if ((sender as TextBox).Name == "tb_cash")
                {
                    SectionData.InputJustNumber(ref txb);
                }
            }
            catch (Exception ex)
            {
                SectionData.ExceptionMessage(ex, this);
            }
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
                SectionData.ExceptionMessage(ex, this);
            }
        }
        private void validateEmpty(string name, object sender)
        {
            amountIsValid = true;
            if (name == "TextBox")
            {
                if ((sender as TextBox).Name == "tb_cash")
                    amountIsValid = SectionData.validateEmptyTextBox((TextBox)sender, p_errorCash, tt_errorCash, "trEmptyCashToolTip");
            }

        }
        private void Tb_cash_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {//only decimal
            var regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                e.Handled = false;
            else
                e.Handled = true;
        }
        private void Btn_enter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tb_cash.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#f8f8f8"));
                p_errorCash.Visibility = Visibility.Collapsed;
                //listPayments
                string s = "";
                cashTrasnfer = new CashTransfer();
                try
                {
                    cashTrasnfer.cash = decimal.Parse(tb_cash.Text);
                }
                catch
                {
                    cashTrasnfer.cash = 0;
                }
                if (cashTrasnfer.cash > 0)
                {
                    if (cashTrasnfer.cash + invoice.paid <= invoice.totalNet)
                    {
                        cashTrasnfer.agentId = invoice.agentId;
                        cashTrasnfer.invId = invoice.invoiceId;
                        cashTrasnfer.processType = cb_paymentProcessType.SelectedValue.ToString();
                        if (cb_paymentProcessType.SelectedValue.ToString().Equals("cash"))
                        {
                            //s = cb_paymentProcessType.Text + " : " + cashTrasnfer.cash;
                            s = validateDuplicate(cashTrasnfer.cash.Value);
                        }
                        else if (cb_paymentProcessType.SelectedValue.ToString().Equals("card"))
                        {
                            cashTrasnfer.cardId = _SelectedCard;
                            cashTrasnfer.docNum = tb_processNum.Text;

                            s = txt_card.Text + " : " + cashTrasnfer.cash;
                        }


                        lst_payments.Items.Add(s);
                        listPayments.Add(cashTrasnfer);
                        invoice.paid += cashTrasnfer.cash;
                        txt_sum.Text = invoice.paid.ToString();


                        if (invoice.paid == invoice.totalNet)
                            txt_sum.Foreground = Application.Current.Resources["mediumGreen"] as SolidColorBrush;
                        else
                            txt_sum.Foreground = Application.Current.Resources["mediumRed"] as SolidColorBrush;

                        tb_cash.Text = (invoice.totalNet - invoice.paid).ToString();
                    }
                    else
                    {
                        p_errorCash.Visibility = Visibility.Visible;
                        tt_errorCash.Content = MainWindow.resourcemanager.GetString("trAmountGreaterInvoiceValue");
                        tb_cash.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#15FF0000"));
                    }
                }
                else
                {
                    p_errorCash.Visibility = Visibility.Visible;
                    tt_errorCash.Content = MainWindow.resourcemanager.GetString("trZeroAmmount");
                    tb_cash.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#15FF0000"));
                }
            }
            catch (Exception ex)
            {
                SectionData.ExceptionMessage(ex, this);
            }
        }
        string validateDuplicate(decimal dec)
        {
            try
            {
                string s = "";
                //decimal dec = 0;
                //List<string> str1 = s.ToString().Split(':').ToList<string>();
                //str1[0] = str1[0].Replace(" ", "");
                //dec = Decimal.Parse(str1[0]);
                bool hasDuplicate = false;
                int index = 0;
                foreach (var item in lst_payments.Items)
                {
                    if (item.ToString().Contains(cb_paymentProcessType.Text))
                    {
                        List<string> str = item.ToString().Split(':').ToList<string>();
                        str[1] = str[1].Replace(" ", "");
                        dec += decimal.Parse(str[1]);
                        invoice.paid -= decimal.Parse(str[1]);
                        hasDuplicate = true;
                        break;
                    }
                    index++;
                }
                if (hasDuplicate)
                {
                    listPayments.Remove(listPayments[index]);
                    lst_payments.Items.Remove(lst_payments.Items[index]);
                }

                cashTrasnfer.cash = dec;
                s = cb_paymentProcessType.Text + " : " + cashTrasnfer.cash;
                return s;
            }
            catch (Exception ex)
            {
                SectionData.ExceptionMessage(ex, this);
                return "";
            }
        }
        private void Btn_clearSerial_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lst_payments.Items.Clear();
                listPayments.Clear();
                invoice.paid = 0;
                tb_cash.Text =
                    txt_sum.Text = "0";

            }
            catch (Exception ex)
            {
                SectionData.ExceptionMessage(ex, this);
            }
        }

        private void Lst_payments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (lst_payments.SelectedItem != null)
                {


                    List<string> str = lst_payments.SelectedItem.ToString().Split(':').ToList<string>();
                    str[1] = str[1].Replace(" ", "");
                    invoice.paid -= decimal.Parse(str[1]);
                    txt_sum.Text = invoice.paid.ToString();

                    listPayments.Remove(listPayments[lst_payments.SelectedIndex]);
                    lst_payments.Items.Remove(lst_payments.SelectedItem);

                    if (invoice.paid == invoice.totalNet)
                        txt_sum.Foreground = Application.Current.Resources["mediumGreen"] as SolidColorBrush;
                    else
                        txt_sum.Foreground = Application.Current.Resources["mediumRed"] as SolidColorBrush;

                    tb_cash.Text = (invoice.totalNet - invoice.paid).ToString();
                }
            }
            catch (Exception ex)
            {
                SectionData.ExceptionMessage(ex, this);
            }
        }
    }
}
