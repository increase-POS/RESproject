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
using System.Windows.Shapes;


namespace Restaurant.View.accounts
{
    /// <summary>
    /// Interaction logic for uc_banksAccounting.xaml
    /// </summary>
    public partial class uc_banksAccounting : UserControl
    {
        public uc_banksAccounting()
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
        private static uc_banksAccounting _instance;
        public static uc_banksAccounting Instance
        {
            get
            {
                //if (_instance == null)
                _instance = new uc_banksAccounting();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        CashTransfer cashtrans = new CashTransfer();
        CashTransfer cashModel = new CashTransfer();
        Pos posModel = new Pos();
        IEnumerable<CashTransfer> cashesQueryExcel;
        IEnumerable<CashTransfer> cashesQuery;
        IEnumerable<CashTransfer> cashes;
        string searchText = "";

        User userModel = new User();
        IEnumerable<User> users;

        Branch branchModel = new Branch();
        //IEnumerable<Branch> branches;

        Bank bankModel = new Bank();
        IEnumerable<Bank> banksQueryExcel;
        IEnumerable<Bank> banksQuery;
        IEnumerable<Bank> banks;

        BrushConverter bc = new BrushConverter();

        wd_acceptUser w = new wd_acceptUser();
        string createPermission = "banksAccounting_create";
        string reportsPermission = "banksAccounting_reports";

        public static List<string> requiredControlList;
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }
        private void translate()
        {
            txt_baseInformation.Text = MainWindow.resourcemanager.GetString("trTransaferDetails");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_search, MainWindow.resourcemanager.GetString("trSearchHint"));
            txt_title.Text = MainWindow.resourcemanager.GetString("trBankAccounts");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_cash, MainWindow.resourcemanager.GetString("trCashHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_depositNumber, MainWindow.resourcemanager.GetString("trDepositeNumHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_opperationType, MainWindow.resourcemanager.GetString("trOpperationTypeHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_user, MainWindow.resourcemanager.GetString("trUserHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_bank, MainWindow.resourcemanager.GetString("trBankHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_notes, MainWindow.resourcemanager.GetString("trNoteHint"));
            try
            {
                if (cb_opperationType.SelectedValue.ToString() == "d")
                    btn_add.Content = MainWindow.resourcemanager.GetString("trDeposit");
                else if (cb_opperationType.SelectedValue.ToString() == "p")
                    btn_add.Content = MainWindow.resourcemanager.GetString("trPull");
            }
            catch { btn_add.Content = MainWindow.resourcemanager.GetString("trSave"); }
            dg_bankAccounts.Columns[0].Header = MainWindow.resourcemanager.GetString("trTransferNumberTooltip");
            dg_bankAccounts.Columns[1].Header = MainWindow.resourcemanager.GetString("trBank");
            dg_bankAccounts.Columns[2].Header = MainWindow.resourcemanager.GetString("trDepositeNumTooltip");
            dg_bankAccounts.Columns[3].Header = MainWindow.resourcemanager.GetString("trDate");
            dg_bankAccounts.Columns[4].Header = MainWindow.resourcemanager.GetString("trCashTooltip");

            tt_clear.Content = MainWindow.resourcemanager.GetString("trClear");
            tt_refresh.Content = MainWindow.resourcemanager.GetString("trRefresh");
            tt_report.Content = MainWindow.resourcemanager.GetString("trPdf");
            tt_print.Content = MainWindow.resourcemanager.GetString("trPrint");
            tt_excel.Content = MainWindow.resourcemanager.GetString("trExcel");
            tt_count.Content = MainWindow.resourcemanager.GetString("trCount");

            btn_image.Content = MainWindow.resourcemanager.GetString("trImage");
            btn_preview.Content = MainWindow.resourcemanager.GetString("trPreview");
            btn_printInvoice.Content = MainWindow.resourcemanager.GetString("trPrint");
            btn_pdf.Content = MainWindow.resourcemanager.GetString("trPdfBtn");
        }
        private void Btn_confirm_Click(object sender, RoutedEventArgs e)
        {//confirm

        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {//load
            try
            {
               
                    HelpClass.StartAwait(grid_main);

                btn_add.IsEnabled = true;
                dp_searchEndDate.SelectedDate = DateTime.Now;
                dp_searchStartDate.SelectedDate = DateTime.Now;

                #region fill operation type
                var dislist = new[] {
            new { Text = MainWindow.resourcemanager.GetString("trDeposit"), Value = "d" },
            new { Text = MainWindow.resourcemanager.GetString("trPull"), Value = "p" },
             };

                cb_opperationType.DisplayMemberPath = "Text";
                cb_opperationType.SelectedValuePath = "Value";
                cb_opperationType.ItemsSource = dislist;
                #endregion

                #region fill users combo
                try
                {
                    users = await userModel.GetUsersActive();
                    cb_user.ItemsSource = users;
                    cb_user.DisplayMemberPath = "username";
                    cb_user.SelectedValuePath = "userId";
                    cb_user.SelectedIndex = -1;
                }
                catch { }
                #endregion

                #region fill banks combo
                try
                {
                    banks = await bankModel.Get();
                    banksQuery = banks.Where(s => s.isActive == 1);
                    cb_bank.ItemsSource = banksQuery;
                    cb_bank.DisplayMemberPath = "name";
                    cb_bank.SelectedValuePath = "bankId";
                    cb_bank.SelectedIndex = -1;
                }
                catch { }
                #endregion

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

                #region Style Date
                /////////////////////////////////////////////////////////////
                HelpClass.defaultDatePickerStyle(dp_searchStartDate);
                HelpClass.defaultDatePickerStyle(dp_searchEndDate);
                /////////////////////////////////////////////////////////////
                #endregion

                #region prevent editting on date and time
                //TextBox tbStartDate = (TextBox)dp_searchStartDate.Template.FindName("PART_TextBox", dp_searchStartDate);
                //tbStartDate.IsReadOnly = true;
                //TextBox tbEndDate = (TextBox)dp_searchEndDate.Template.FindName("PART_TextBox", dp_searchEndDate);
                //tbEndDate.IsReadOnly = true;
                #endregion

                dp_searchStartDate.SelectedDateChanged += this.dp_SelectedStartDateChanged;
                dp_searchEndDate.SelectedDateChanged += this.dp_SelectedEndDateChanged;

                btn_image.IsEnabled = false;

                Tb_search_TextChanged(null, null);

               
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

                await RefreshCashesList();
                Tb_search_TextChanged(null, null);

               
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
               
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void dp_SelectedEndDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
               
                    HelpClass.StartAwait(grid_main);

                await RefreshCashesList();
                Tb_search_TextChanged(null, null);

               
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
               
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Dg_bankAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//selection
            /*
            try
            {
               
                    HelpClass.StartAwait(grid_main);
                HelpClass.clearValidate(tb_cash, p_error_cash);
                HelpClass.clearValidate(tb_depositNumber, p_error_depositNumber);
                HelpClass.clearComboBoxValidate(cb_opperationType, p_error_opperationType);
                HelpClass.clearComboBoxValidate(cb_user, p_error_user);
                HelpClass.clearComboBoxValidate(cb_bank, p_error_bank);
                if (dg_bankAccounts.SelectedIndex != -1)
                {
                    cashtrans = dg_bankAccounts.SelectedItem as CashTransfer;
                    this.DataContext = cashtrans;

                    if (cashtrans != null)
                    {
                        btn_image.IsEnabled = true;

                        tb_cash.Text = HelpClass.DecTostring(cashtrans.cash);

                        cb_opperationType.SelectedValue = cashtrans.transType;
                        cb_user.SelectedValue = cashtrans.userId;
                        cb_bank.SelectedValue = cashtrans.bankId;
                        p_confirmUser.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#2BB673"));
                        if (string.IsNullOrEmpty(cashtrans.docNum))
                        {
                            cb_opperationType.IsEnabled = false;
                            cb_user.IsEnabled = false;
                            cb_bank.IsEnabled = false;
                            tb_cash.IsEnabled = false;
                            tb_depositNumber.IsEnabled = true;
                            tb_notes.IsEnabled = false;
                            btn_add.IsEnabled = true;
                            btn_add.Content = MainWindow.resourcemanager.GetString("trCompletion");
                        }
                        else
                        {
                            cb_opperationType.IsEnabled = false;
                            cb_user.IsEnabled = false;
                            cb_bank.IsEnabled = false;
                            tb_cash.IsEnabled = false;
                            tb_depositNumber.IsEnabled = false;
                            tb_notes.IsEnabled = false;
                            btn_add.IsEnabled = false;
                            btn_add.Content = MainWindow.resourcemanager.GetString("trCompleted");
                        }

                    }
                }
               
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
               
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
            */
        }

        private async void Tb_search_TextChanged(object sender, TextChangedEventArgs e)
        {//search
            try
            {
               
                    HelpClass.StartAwait(grid_main);

                try
                {
                    if (cashes is null)
                        await RefreshCashesList();

                    if (chb_all.IsChecked == false)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            searchText = tb_search.Text.ToLower();
                            cashesQuery = cashes.Where(s => (s.transNum.ToLower().Contains(searchText)
                            || s.cash.ToString().ToLower().Contains(searchText)
                            || s.bankName.ToLower().Contains(searchText)
                            || s.docNum.Contains(searchText)
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
                            cashesQuery = cashes.Where(s => (s.transNum.ToLower().Contains(searchText)
                            || s.cash.ToString().ToLower().Contains(searchText)
                            || s.bankName.ToLower().Contains(searchText)
                            || s.docNum.Contains(searchText)
                            )
                            );
                        });
                    }
                    RefreshCashView();
                    cashesQueryExcel = cashesQuery.ToList();
                }
                catch { }

               
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
               
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Btn_add_Click(object sender, RoutedEventArgs e)
        {//save
            try
            {
               
                    HelpClass.StartAwait(grid_main);
                /*
                if (MainWindow.groupObject.HasPermissionAction(createPermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    if (cashtrans.cashTransId == 0)
                    {
                        #region validate
                        //chk empty cash
                        HelpClass.validateEmptyTextBox(tb_cash, p_error_cash, tt_errorCash, "trEmptyCashToolTip");
                        //chk empty dicount type
                        HelpClass.validateEmptyComboBox(cb_opperationType, p_error_opperationType, tt_errorOpperationType, "trErrorEmptyOpperationTypeToolTip");
                        //chk empty user
                        HelpClass.validateEmptyComboBox(cb_user, p_error_user, tt_errorUser, "trErrorEmptyUserToolTip");
                        //chk empty bank
                        HelpClass.validateEmptyComboBox(cb_bank, p_error_bank, tt_errorBank, "trErrorEmptyBankToolTip");
                        //chk user confirmation
                        bool isuserConfirmed = w.isOk;
                        #endregion

                        #region add
                        if ((!tb_cash.Text.Equals("")) &&
                            (!cb_opperationType.Text.Equals("")) && (!cb_user.Text.Equals("")) &&
                            (!cb_bank.Text.Equals("")) &&
                            (isuserConfirmed)
                            )

                        {
                            CashTransfer cash = new CashTransfer();

                            cash.transType = cb_opperationType.SelectedValue.ToString();
                            cash.userId = Convert.ToInt32(cb_user.SelectedValue);
                            try
                            {
                                cash.transNum = await cashModel.generateCashNumber(cb_opperationType.SelectedValue.ToString() + "bn");
                            }
                            catch { }
                            cash.cash = decimal.Parse(tb_cash.Text);
                            cash.createUserId = MainWindow.userLogin.userId;
                            cash.notes = tb_notes.Text;
                            cash.posId = MainWindow.posLogin.posId;
                            cash.side = "bn";
                            cash.isConfirm = 0;
                            cash.bankId = Convert.ToInt32(cb_bank.SelectedValue);

                            int s = await cashModel.Save(cash);

                            if (!s.Equals(0))
                            {

                                Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);
                                Btn_clear_Click(null, null);

                                dg_bankAccounts.ItemsSource = await RefreshCashesList();
                                Tb_search_TextChanged(null, null);
                            }
                            else
                                Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                        }
                    }
                    else
                    {
                        if (cashtrans.isConfirm == 0)
                        {
                            //chk empty deposite number
                            HelpClass.validateEmptyTextBox(tb_depositNumber, p_error_depositNumber, tt_errorDepositNumber, "trEmptyDepositNumberToolTip");
                            if (!tb_depositNumber.Text.Equals(""))
                            {
                                cashtrans.isConfirm = 1;
                                cashtrans.docNum = tb_depositNumber.Text;

                                int s = await cashModel.Save(cashtrans);

                                if (!s.Equals(0))
                                {
                                    Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trCompleted"), animation: ToasterAnimation.FadeIn);
                                    btn_add.IsEnabled = false;
                                    btn_add.Content = MainWindow.resourcemanager.GetString("trCompleted");

                                    dg_bankAccounts.ItemsSource = await RefreshCashesList();
                                    Tb_search_TextChanged(null, null);

                                    decimal ammount = cashtrans.cash;
                                    if (cashtrans.transType.Equals("d")) ammount *= -1;
                                    await calcBalance(ammount);
                                }
                                else
                                    Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                            }
                        }
                        #endregion
                    }
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                */
               
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
               
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async Task calcBalance(decimal ammount)
        {
            Pos pos = await posModel.getById(MainWindow.posLogin.posId);

            pos.balance += ammount;

            int s = await posModel.save(pos);

        }

        private void Btn_update_Click(object sender, RoutedEventArgs e)
        {//update

        }

        private void Btn_delete_Click(object sender, RoutedEventArgs e)
        {//delete

        }

        private async void Btn_clear_Click(object sender, RoutedEventArgs e)
        {//clear
            /*
            try
            {
               
                    HelpClass.StartAwait(grid_main);

                cashtrans.cashTransId = 0;
                try
                { tb_transNum.Text = await HelpClass.generateNumber(Convert.ToChar(cb_opperationType.SelectedValue), "bn"); }
                catch { }
                btn_add.IsEnabled = true;
                cb_opperationType.IsEnabled = true;
                cb_user.IsEnabled = true;
                cb_bank.IsEnabled = true;
                tb_cash.IsEnabled = true;
                tb_depositNumber.IsEnabled = false;
                tb_notes.IsEnabled = true;
                btn_image.IsEnabled = false;

                tb_cash.Clear();
                tb_depositNumber.Clear();
                cb_opperationType.SelectedIndex = -1;
                cb_bank.SelectedIndex = -1;
                cb_user.SelectedIndex = -1;
                tb_notes.Clear();

                HelpClass.clearValidate(tb_cash, p_error_cash);
                HelpClass.clearValidate(tb_depositNumber, p_error_depositNumber);
                HelpClass.clearComboBoxValidate(cb_opperationType, p_error_opperationType);
                HelpClass.clearComboBoxValidate(cb_user, p_error_user);
                HelpClass.clearComboBoxValidate(cb_bank, p_error_bank);
                p_confirmUser.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E65B65"));

               
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
               
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
            */
        }

        private void Btn_exportToExcel_Click(object sender, RoutedEventArgs e)
        {//excel
            try
            {
               
                    HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(reportsPermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
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
            var QueryExcel = cashesQuery.AsEnumerable().Select(x => new
            {
                Tranfer_Number = x.transNum,
                Bank = x.bankId,
                DipRecNum = x.docNum,
                Cash = x.cash
            });
            var DTForExcel = QueryExcel.ToDataTable();
            DTForExcel.Columns[0].Caption = MainWindow.resourcemanager.GetString("trTransferNum");
            DTForExcel.Columns[1].Caption = MainWindow.resourcemanager.GetString("trBank");
            DTForExcel.Columns[2].Caption = MainWindow.resourcemanager.GetString("trDepositeReceiptNum");
            DTForExcel.Columns[3].Caption = MainWindow.resourcemanager.GetString("trCash");

            ExportToExcel.Export(DTForExcel);
        }

        private void Btn_image_Click(object sender, RoutedEventArgs e)
        {//image
            try
            {
               
                    HelpClass.StartAwait(grid_main);

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
               
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
               
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {//refresh
            try
            {
               
                    HelpClass.StartAwait(grid_main);

                await RefreshCashesList();
                Tb_search_TextChanged(null, null);

               
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
               
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void PreventSpaces(object sender, KeyEventArgs e)
        {
            e.Handled = e.Key == Key.Space;
        }

        private void Tb_cash_PreviewTextInput(object sender, TextCompositionEventArgs e)
        { //only decimal
            var regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                e.Handled = false;

            else
                e.Handled = true;

        }

        private void Tb_depositNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {//only int
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

        }

        private void validateEmpty(string name, object sender)
        {
            /*
            if (name == "TextBox")
            {
                if ((sender as TextBox).Name == "tb_cash")
                    HelpClass.validateEmptyTextBox((TextBox)sender, p_error_cash, tt_errorCash, "trEmptyCashToolTip");
                else if ((sender as TextBox).Name == "tb_depositNumber")
                    HelpClass.validateEmptyTextBox((TextBox)sender, p_error_depositNumber, tt_errorDepositNumber, "trEmptyDepositeNumberToolTip");
            }
            else if (name == "ComboBox")
            {
                if ((sender as ComboBox).Name == "cb_opperationType")
                    HelpClass.validateEmptyComboBox((ComboBox)sender, p_error_opperationType, tt_errorOpperationType, "trErrorEmptyOpperationTypeToolTip");
                else if ((sender as ComboBox).Name == "cb_user")
                    HelpClass.validateEmptyComboBox((ComboBox)sender, p_error_user, tt_errorUser, "trErrorEmptyUserToolTip");
                else if ((sender as ComboBox).Name == "cb_bank")
                    HelpClass.validateEmptyComboBox((ComboBox)sender, p_error_bank, tt_errorBank, "trErrorEmptyBankToolTip");
            }
            */
        }

        private void Tb_validateEmptyTextChange(object sender, TextChangedEventArgs e)
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


        private void Tb_validateEmptyTextChangeCash(object sender, TextChangedEventArgs e)
        {
            try
            {
                string name = sender.GetType().Name;
                validateEmpty(name, sender);
                var txb = sender as TextBox;
                if ((sender as TextBox).Name == "tb_cash")
                    HelpClass.InputJustNumber(ref txb);
                Cb_user_SelectionChanged(null, null);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }

        }

        private void Tb_validateEmptyLostFocusCash(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = sender.GetType().Name;
                validateEmpty(name, sender);
                Cb_user_SelectionChanged(null, null);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        async Task<IEnumerable<CashTransfer>> RefreshCashesList()
        {
            cashes = await cashModel.GetCashTransferAsync("all", "bn");
            return cashes;
        }
        void RefreshCashView()
        {
            dg_bankAccounts.ItemsSource = cashesQuery;
            txt_count.Text = cashesQuery.Count().ToString();
        }

        private void OnlyInt(object sender, TextCompositionEventArgs e)
        {
            //only int
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

        }

        private void Cb_opperationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cb_opperationType.SelectedValue.ToString() == "d")
                    btn_add.Content = MainWindow.resourcemanager.GetString("trDeposit");
                else if (cb_opperationType.SelectedValue.ToString() == "p")
                    btn_add.Content = MainWindow.resourcemanager.GetString("trPull");
                else
                    btn_add.Content = MainWindow.resourcemanager.GetString("trSave");
            }
            catch (Exception ex)
            {
                //    HelpClass.ExceptionMessage(ex, this);
                btn_add.Content = MainWindow.resourcemanager.GetString("trSave");
            }
        }

        private void Btn_confirmUser_Click(object sender, RoutedEventArgs e)
        {//confirm user
            try
            {
                Window.GetWindow(this).Opacity = 0.2;

                w.tb_userName.Text = cb_user.Text;
                w.userID = Convert.ToInt32(cb_user.SelectedValue);
                w.ShowDialog();

                Window.GetWindow(this).Opacity = 1;

                if (w.isOk == true)
                    p_confirmUser.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#2BB673"));
                else p_confirmUser.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E65B65"));
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Cb_user_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//select user
            try
            {
                if ((cb_user.SelectedIndex != -1) && (!tb_cash.Text.Equals("")))
                    btn_confirmUser.IsEnabled = true;
                else
                    btn_confirmUser.IsEnabled = false;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        ReportCls reportclass = new ReportCls();
        LocalReport rep = new LocalReport();
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        public void BuildReport()
        {
            List<ReportParameter> paramarr = new List<ReportParameter>();

            string addpath;
            bool isArabic = ReportCls.checkLang();
            if (isArabic)
            {
                addpath = @"\Reports\Account\Ar\ArBankAccReport.rdlc";
            }
            else addpath = @"\Reports\Account\EN\BankAccReport.rdlc";
            string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);

            ReportCls.checkLang();
            foreach (var r in cashesQuery)
            {
                r.cash = decimal.Parse(HelpClass.DecTostring(r.cash));
            }
            clsReports.bankAccReport(cashesQuery, rep, reppath, paramarr);
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
            /*
            try
            {
               
                    HelpClass.StartAwait(grid_main);
                /////////////////////
                if (MainWindow.groupObject.HasPermissionAction(reportsPermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    Window.GetWindow(this).Opacity = 0.2;
                    win_lvc win = new win_lvc(banksQuery, 2);
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
            */

        }
        public void getBankData(List<ReportParameter> paramarr)
        {
            string pay = "";
            string processType = "";
            //   string cardname = "";
            // string caprocesstype = "";
            string title = "";
            string userName = "";
            string bankName = "";
            string trNum = "";
            string trOperation = "";
            ////////////////////////////////
            ///

            if (cashtrans.transType == "p")
            {
                // title = MainWindow.resourcemanagerreport.GetString("trPayVocher")
                title = MainWindow.resourcemanagerreport.GetString("trVoucher");
                trOperation = MainWindow.resourcemanagerreport.GetString("trDeposit");
                trNum = MainWindow.resourcemanagerreport.GetString("trDepositeNum");
            }

            else
            {
                // title = MainWindow.resourcemanagerreport.GetString("trReceiptVoucher");
                title = MainWindow.resourcemanagerreport.GetString("trVoucher");

                trOperation = MainWindow.resourcemanagerreport.GetString("trReceiptOperation");
                trNum = MainWindow.resourcemanagerreport.GetString("trRecieptNum");
            }

            userName = cashtrans.usersName + " " + cashtrans.usersLName;

            bankName = cashtrans.bankName;

            ////////////////////////////////////

            processType = MainWindow.resourcemanagerreport.GetString("trCash");
            paramarr.Add(new ReportParameter("title", title));
            paramarr.Add(new ReportParameter("docnum", cashtrans.docNum));
            paramarr.Add(new ReportParameter("bondNumber", tb_transNum.Text));
            //  paramarr.Add(new ReportParameter("deserveDate", HelpClass.DateToString(bond.deserveDate)));

            //  paramarr.Add(new ReportParameter("isRecieved", bond.isRecieved.ToString()));
            paramarr.Add(new ReportParameter("trPay", pay));
            // paramarr.Add(new ReportParameter("ctside", bond.ctside));
            paramarr.Add(new ReportParameter("sideName", bankName));
            paramarr.Add(new ReportParameter("trProcessType", processType));
            // paramarr.Add(new ReportParameter("cardName", cardname));
            paramarr.Add(new ReportParameter("transType", cashtrans.transType));//ok
                                                                                //  paramarr.Add(new ReportParameter("type", caprocesstype));
            paramarr.Add(new ReportParameter("user_name", userName));//ok

            paramarr.Add(new ReportParameter("date", reportclass.DateToString(cashtrans.updateDate)));//ok
            paramarr.Add(new ReportParameter("currency", MainWindow.Currency));//ok
            paramarr.Add(new ReportParameter("amount_in_words", reportclass.ConvertAmountToWords(cashtrans.cash)));//ok
            paramarr.Add(new ReportParameter("job", "Employee"));

            paramarr.Add(new ReportParameter("trNum", trNum));//ok
            paramarr.Add(new ReportParameter("amount", HelpClass.DecTostring(cashtrans.cash)));

            paramarr.Add(new ReportParameter("trOperation", trOperation));//ok


        }


        public void buildBankDocReport()
        {
          /*
            List<ReportParameter> paramarr = new List<ReportParameter>();

            string addpath;
            bool isArabic = ReportCls.checkLang();
            // bond.type
            if (isArabic)
            {
                if (MainWindow.docPapersize == "A4")
                {
                    addpath = @"\Reports\Account\Ar\ArBankDocA4.rdlc";
                }
                else//A5
                {
                    addpath = @"\Reports\Account\Ar\ArBankDoc.rdlc";
                }
            }
            else

            {
                if (MainWindow.docPapersize == "A4")
                {
                    addpath = @"\Reports\Account\EN\BankDocA4.rdlc";
                }
                else//A5
                {
                    addpath = @"\Reports\Account\EN\BankDoc.rdlc";
                }

            }
            string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);

            ReportCls.checkLang();
            getBankData(paramarr);
            clsReports.bondsDocReport(rep, reppath, paramarr);
            clsReports.setReportLanguage(paramarr);
            clsReports.Header(paramarr);

            rep.SetParameters(paramarr);
            rep.Refresh();
            */
        }
        private void Btn_printInvoice_Click(object sender, RoutedEventArgs e)
        {// doc
            try
            {
               
                    HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(reportsPermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    #region

                    if (cashtrans != null)
                    {
                        buildBankDocReport();
                        LocalReportExtensions.PrintToPrinterbyNameAndCopy(rep, MainWindow.rep_printer_name, short.Parse(MainWindow.rep_print_count));

                    }
                    else
                    {
                        Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trBondNotReceived"), animation: ToasterAnimation.FadeIn);
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

        private void Btn_pdf_Click(object sender, RoutedEventArgs e)
        {//doc
            try
            {
               
                    HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(reportsPermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    #region
                    if (!string.IsNullOrEmpty(cashtrans.docNum) && cashtrans.userId != null)
                    {
                        buildBankDocReport();
                        saveFileDialog.Filter = "PDF|*.pdf;";
                        if (saveFileDialog.ShowDialog() == true)
                        {
                            string filepath = saveFileDialog.FileName;
                            LocalReportExtensions.ExportToPDF(rep, filepath);
                        }
                    }
                    else
                    {
                        Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trProcessNotConfirmed"), animation: ToasterAnimation.FadeIn);
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

        private void Btn_preview_Click(object sender, RoutedEventArgs e)
        {//doc
            try
            {
               
                    HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(reportsPermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    #region

                    if (!string.IsNullOrEmpty(cashtrans.docNum) && cashtrans.userId != null)
                    {
                        Window.GetWindow(this).Opacity = 0.2;
                        string pdfpath = "";
                        //
                        pdfpath = @"\Thumb\report\temp.pdf";
                        pdfpath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, pdfpath);

                        buildBankDocReport();
                        LocalReportExtensions.ExportToPDF(rep, pdfpath);
                        wd_previewPdf w = new wd_previewPdf();
                        w.pdfPath = pdfpath;
                        if (!string.IsNullOrEmpty(w.pdfPath))
                        {
                            w.ShowDialog();
                            w.wb_pdfWebViewer.Dispose();
                        }
                        Window.GetWindow(this).Opacity = 1;
                    }
                    else
                    {
                        Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trProcessNotConfirmed"), animation: ToasterAnimation.FadeIn);
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
            this.DataContext = new Agent();



            // last 
            HelpClass.clearValidate(requiredControlList, this);
        }
        string input;
        decimal _decimal = 0;
        private void Number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {


                //only  digits
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
