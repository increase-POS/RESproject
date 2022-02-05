using netoaster;
using Restaurant.Classes;
using Restaurant.View.windows;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;

namespace Restaurant.View.accounts
{
    /// <summary>
    /// Interaction logic for uc_posTransfers.xaml
    /// </summary>
    public partial class uc_posTransfers : UserControl
    {
        public uc_posTransfers()
        {
            try
            {
                InitializeComponent();
                if (System.Windows.SystemParameters.PrimaryScreenWidth >= 1440)
                {
                    txt_deleteButton.Visibility = Visibility.Visible;
                    txt_addButton.Visibility = Visibility.Visible;
                    txt_updateButton.Visibility = Visibility.Visible;
                    txt_add_Icon.Visibility = Visibility.Visible;
                    txt_update_Icon.Visibility = Visibility.Visible;
                    txt_delete_Icon.Visibility = Visibility.Visible;
                }
                else if (System.Windows.SystemParameters.PrimaryScreenWidth >= 1360)
                {
                    txt_add_Icon.Visibility = Visibility.Collapsed;
                    txt_update_Icon.Visibility = Visibility.Collapsed;
                    txt_delete_Icon.Visibility = Visibility.Collapsed;
                    txt_deleteButton.Visibility = Visibility.Visible;
                    txt_addButton.Visibility = Visibility.Visible;
                    txt_updateButton.Visibility = Visibility.Visible;

                }
                else
                {
                    txt_deleteButton.Visibility = Visibility.Collapsed;
                    txt_addButton.Visibility = Visibility.Collapsed;
                    txt_updateButton.Visibility = Visibility.Collapsed;
                    txt_add_Icon.Visibility = Visibility.Visible;
                    txt_update_Icon.Visibility = Visibility.Visible;
                    txt_delete_Icon.Visibility = Visibility.Visible;

                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private static uc_posTransfers _instance;
        public static uc_posTransfers Instance
        {
            get
            {
                //if (_instance == null)
                _instance = new uc_posTransfers();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        Pos posModel = new Pos();
        Branch branchModel = new Branch();
        IEnumerable<Pos> poss;
        IEnumerable<Branch> branches;
        CashTransfer cashtrans = new CashTransfer();
        CashTransfer cashModel = new CashTransfer();
        IEnumerable<CashTransfer> cashesQuery;
        IEnumerable<CashTransfer> cashesQueryExcel;
        IEnumerable<CashTransfer> cashes;
        string searchText = "";
        CashTransfer cashtrans2 = new CashTransfer();
        CashTransfer cashtrans3 = new CashTransfer();
        IEnumerable<CashTransfer> cashes2;
        string basicsPermission = "posTransfers_basics";
        string transAdminPermission = "posTransfers_transAdmin";

        public static List<string> requiredControlList;
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {//load
            try
            {
                HelpClass.StartAwait(grid_main);
                requiredControlList = new List<string> { "name", "accNumber", "mobile", "phone" };
                if (MainWindow.lang.Equals("en"))
                {
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                }
                translate();
                dp_searchStartDate.SelectedDate = DateTime.Now.Date;
                dp_searchEndDate.SelectedDate = DateTime.Now.Date;

                dp_searchStartDate.SelectedDateChanged += this.dp_SelectedStartDateChanged;
                dp_searchEndDate.SelectedDateChanged += this.dp_SelectedEndDateChanged;

                try
                {
                    poss = await posModel.Get();
                }
                catch { }

                //HelpClass.fillBranches(cb_fromBranch, "bs");
                //cb_fromBranch.SelectedValue = MainWindow.branchID.Value;
                //HelpClass.fillBranches(cb_toBranch, "bs");
                //cb_toBranch.SelectedValue = MainWindow.branchID.Value;


                if (!MainWindow.groupObject.HasPermissionAction(transAdminPermission, MainWindow.groupObjects, "one"))
                {
                    cb_fromBranch.IsEnabled = false;////////////permissions
                    cb_toBranch.IsEnabled = false;/////////////permissions
                }

                #region fill branch combo1
                try
                {
                    branches = await branchModel.GetBranchesActive("b");
                    cb_fromBranch.ItemsSource = branches;
                    cb_fromBranch.DisplayMemberPath = "name";
                    cb_fromBranch.SelectedValuePath = "branchId";
                    cb_fromBranch.SelectedValue = MainWindow.branchLogin.branchId;
                }
                catch { }
                #endregion

                #region fill branch combo2
                try
                {
                    cb_toBranch.ItemsSource = branches;
                    cb_toBranch.DisplayMemberPath = "name";
                    cb_toBranch.SelectedValuePath = "branchId";
                }
                catch { }
                #endregion

                #region fill operation state
                /*
                var dislist = new[] {
                new { Text = MainWindow.resourcemanager.GetString("trUnConfirmed")  , Value = "0" },
                new { Text = MainWindow.resourcemanager.GetString("trWaiting")      , Value = "1" },
                new { Text = MainWindow.resourcemanager.GetString("trConfirmed")    , Value = "2" },
                new { Text = MainWindow.resourcemanager.GetString("trCreatedOper")  , Value = "3" }
                 };
                cb_state.DisplayMemberPath = "Text";
                cb_state.SelectedValuePath = "Value";
                cb_state.ItemsSource = dislist;
                cb_state.SelectedIndex = 0;
                */
                #endregion

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
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_search, MainWindow.resourcemanager.GetString("trSearchHint"));
            txt_title.Text = MainWindow.resourcemanager.GetString("trTransfers");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_cash, MainWindow.resourcemanager.GetString("trCashHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_pos1, MainWindow.resourcemanager.GetString("trFromPosHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_pos2, MainWindow.resourcemanager.GetString("trToPosHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_notes, MainWindow.resourcemanager.GetString("trNoteHint"));
            //MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_state, MainWindow.resourcemanager.GetString("trStateHint"));

            dg_posAccounts.Columns[0].Header = MainWindow.resourcemanager.GetString("trTransferNumberTooltip");
            dg_posAccounts.Columns[1].Header = MainWindow.resourcemanager.GetString("trFromPos");
            dg_posAccounts.Columns[2].Header = MainWindow.resourcemanager.GetString("trToPos");
            dg_posAccounts.Columns[3].Header = MainWindow.resourcemanager.GetString("trOpperationTypeToolTip");
            dg_posAccounts.Columns[4].Header = MainWindow.resourcemanager.GetString("trDate");
            dg_posAccounts.Columns[5].Header = MainWindow.resourcemanager.GetString("trCashTooltip");

            tt_clear.Content = MainWindow.resourcemanager.GetString("trClear");
            tt_refresh.Content = MainWindow.resourcemanager.GetString("trRefresh");
            tt_report.Content = MainWindow.resourcemanager.GetString("trPdf");
            tt_print.Content = MainWindow.resourcemanager.GetString("trPrint");
            tt_excel.Content = MainWindow.resourcemanager.GetString("trExcel");
            tt_count.Content = MainWindow.resourcemanager.GetString("trCount");

            btn_confirm.Content = MainWindow.resourcemanager.GetString("trConfirm");
            btn_add.Content = MainWindow.resourcemanager.GetString("trAdd");
            btn_update.Content = MainWindow.resourcemanager.GetString("trUpdate");
            btn_delete.Content = MainWindow.resourcemanager.GetString("trDelete");

        }
        #region Add - Update - Delete - Search - Tgl - Clear - DG_SelectionChanged - refresh
        private async void Btn_add_Click(object sender, RoutedEventArgs e)
        {//add
            try
            {
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "add") )
                {
                    HelpClass.StartAwait(grid_main);

                    /*
                    #region validate
                    //chk empty cash
                    HelpClass.validateEmptyTextBox(tb_cash, p_errorCash, tt_errorCash, "trEmptyCashToolTip");
                    //chk empty pos1
                    HelpClass.validateEmptyComboBox(cb_pos1, p_errorPos1, tt_errorPos1, "trErrorEmptyFromPosToolTip");
                    //chk empty pos2
                    HelpClass.validateEmptyComboBox(cb_pos2, p_errorPos2, tt_errorPos2, "trErrorEmptyToPosToolTip");
                    //chk if 2 pos is the same
                    bool isSame = false;
                    if (cb_pos1.SelectedValue == cb_pos2.SelectedValue)
                        isSame = true;
                    if ((cb_pos1.SelectedIndex != -1) && (cb_pos2.SelectedIndex != -1) && (cb_pos1.SelectedValue == cb_pos2.SelectedValue))
                    {
                        HelpClass.showComboBoxValidate(cb_pos1, p_errorPos1, tt_errorPos1, "trErrorSamePos");
                        HelpClass.showComboBoxValidate(cb_pos2, p_errorPos2, tt_errorPos2, "trErrorSamePos");
                    }

                    #endregion

                    #region add

                    if ((!tb_cash.Text.Equals("")) && (!cb_pos1.Text.Equals("")) && (!cb_pos2.Text.Equals("")) && !isSame )
                    {
                        Pos pos = await posModel.getById(Convert.ToInt32(cb_pos1.SelectedValue));
                        if (pos.balance < decimal.Parse(tb_cash.Text))
                        { Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopNotEnoughBalance"), animation: ToasterAnimation.FadeIn); }
                        else
                        {
                            //first operation
                            CashTransfer cash1 = new CashTransfer();

                            cash1.transType = "p";//pull
                            cash1.transNum = await cashModel.generateCashNumber(cash1.transType + "p");
                            cash1.cash = decimal.Parse(tb_cash.Text);
                            cash1.createUserId = MainWindow.userID.Value;
                            cash1.notes = tb_note.Text;
                            cash1.posIdCreator = MainWindow.posID.Value;
                            if (Convert.ToInt32(cb_pos1.SelectedValue) == MainWindow.posID)
                                cash1.isConfirm = 1;
                            else cash1.isConfirm = 0;
                            cash1.side = "p";//pos
                            cash1.posId = Convert.ToInt32(cb_pos1.SelectedValue);

                            int s1 = await cashModel.Save(cash1);

                            if (!s1.Equals(0))
                            {
                                //second operation
                                CashTransfer cash2 = new CashTransfer();

                                cash2.transType = "d";//deposite
                                cash2.transNum = await cashModel.generateCashNumber(cash2.transType + "p");
                                cash2.cash = decimal.Parse(tb_cash.Text);
                                cash2.createUserId = MainWindow.userID.Value;
                                cash2.posIdCreator = MainWindow.posID.Value;
                                if (Convert.ToInt32(cb_pos2.SelectedValue) == MainWindow.posID)
                                    cash2.isConfirm = 1;
                                else cash2.isConfirm = 0;
                                cash2.side = "p";//pos
                                cash2.posId = Convert.ToInt32(cb_pos2.SelectedValue);
                                cash2.cashTransIdSource = s1;//id from first operation

                                int s2 = await cashModel.Save(cash2);

                                if (!s2.Equals(0))
                                {
                                    #region notification Object
                                    int pos1 = 0;
                                    int pos2 = 0;
                                    if ((int)cb_pos1.SelectedValue != MainWindow.posID.Value)
                                        pos1 = (int)cb_pos1.SelectedValue;
                                    if ((int)cb_pos2.SelectedValue != MainWindow.posID.Value)
                                        pos2 = (int)cb_pos2.SelectedValue;
                                    Notification not = new Notification()
                                    {
                                        title = "trTransferAlertTilte",
                                        ncontent = "trTransferAlertContent",
                                        msgType = "alert",
                                        createUserId = MainWindow.userID.Value,
                                        updateUserId = MainWindow.userID.Value,
                                    };
                                    if (pos1 != 0)
                                        await not.save(not, (int)cb_pos1.SelectedValue, "accountsAlerts_transfers", cb_pos2.Text, 0, pos1);
                                    if (pos2 != 0)
                                        await not.save(not, (int)cb_pos2.SelectedValue, "accountsAlerts_transfers", cb_pos1.Text, 0, pos2);

                                    #endregion

                                    Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);
                                    Btn_clear_Click(null, null);

                                    await RefreshCashesList();
                                    Tb_search_TextChanged(null, null);
                                }
                                else
                                    Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                            }
                        }
                    }

                    #endregion
                    */
                    HelpClass.EndAwait(grid_main);
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Btn_update_Click(object sender, RoutedEventArgs e)
        {//update
            try
            {
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "update"))
                {
                    HelpClass.StartAwait(grid_main);
                    /*
                    #region validate
                    //chk empty cash
                    HelpClass.validateEmptyTextBox(tb_cash, p_errorCash, tt_errorCash, "trEmptyCashToolTip");
                    //chk empty user
                    HelpClass.validateEmptyComboBox(cb_pos1, p_errorPos1, tt_errorPos1, "trErrorEmptyFromPosToolTip");
                    //chk empty bank
                    HelpClass.validateEmptyComboBox(cb_pos2, p_errorPos2, tt_errorPos2, "trErrorEmptyToPosToolTip");
                    //chk if 2 pos is the same
                    bool isSame = false;
                    if (cb_pos1.SelectedValue == cb_pos2.SelectedValue)
                        isSame = true;
                    if ((cb_pos1.SelectedIndex != -1) && (cb_pos2.SelectedIndex != -1) && (cb_pos1.SelectedValue == cb_pos2.SelectedValue))
                    {
                        HelpClass.showComboBoxValidate(cb_pos1, p_errorPos1, tt_errorPos1, "trErrorSamePos");
                        HelpClass.showComboBoxValidate(cb_pos2, p_errorPos2, tt_errorPos2, "trErrorSamePos");
                    }
                    #endregion

                    #region update
                    if ((!tb_cash.Text.Equals("")) && (!cb_pos1.Text.Equals("")) && (!cb_pos2.Text.Equals("")) && !isSame)
                    {
                        //first operation (pull)
                        cashtrans2.cash = decimal.Parse(tb_cash.Text);
                        cashtrans2.notes = tb_note.Text;
                        cashtrans2.posId = Convert.ToInt32(cb_pos1.SelectedValue);

                        int s1 = await cashModel.Save(cashtrans2);

                        if (!s1.Equals(0))
                        {
                            //second operation (deposit)
                            cashtrans3.cash = decimal.Parse(tb_cash.Text);
                            cashtrans3.posId = Convert.ToInt32(cb_pos2.SelectedValue);
                            cashtrans3.notes = tb_note.Text;

                            int s2 = await cashModel.Save(cashtrans3);

                            if (!s2.Equals(0))
                            {
                                Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);

                                await RefreshCashesList();
                                Tb_search_TextChanged(null, null);
                            }
                            else
                                Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                        }
                    }
                    #endregion
                    */
                    HelpClass.EndAwait(grid_main);
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {//delete
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "delete") )
                {
                HelpClass.StartAwait(grid_main);
                    if (cashtrans.cashTransId != 0)
                    {
                        #region
                        Window.GetWindow(this).Opacity = 0.2;
                        wd_acceptCancelPopup w = new wd_acceptCancelPopup();
                        w.contentText = MainWindow.resourcemanager.GetString("trMessageBoxDelete");
                        w.ShowDialog();
                        Window.GetWindow(this).Opacity = 1;
                        #endregion
                        if (w.isOk)
                        {
                            int b = await cashModel.deletePosTrans(cashtrans.cashTransId);

                            if (b == 1)
                            {
                                Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopDelete"), animation: ToasterAnimation.FadeIn);
                                //clear textBoxs
                                Btn_clear_Click(sender, e);
                            }
                            else if (b == 0)
                                Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopCanNotDeleteRequest"), animation: ToasterAnimation.FadeIn);
                            else
                                Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);

                            await RefreshCashesList();
                            Tb_search_TextChanged(null, null);
                        }
                    }
                    HelpClass.EndAwait(grid_main);
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Btn_confirm_Click(object sender, RoutedEventArgs e)
        {//confirm
            try
            {
                
                if (cashtrans.cashTransId != 0)
                {
                    HelpClass.StartAwait(grid_main);
                    //if another operation not confirmed then just confirm this
                    ////if another operation is confirmed then chk balance before confirm
                    bool confirm = false;
                    if (cashtrans2.cashTransId == cashtrans.cashTransId)//chk which record is selected
                    { if (cashtrans3.isConfirm == 0) confirm = false; else confirm = true; }
                    else//chk which record is selected
                    { if (cashtrans2.isConfirm == 0) confirm = false; else confirm = true; }

                    if (!confirm) await confirmOpr();
                    else
                    {
                        Pos pos = await posModel.getById(cashtrans2.posId.Value);
                        //there is enough balance
                        if (pos.balance >= cashtrans2.cash)
                        {
                            cashtrans2.isConfirm = 1;
                            int s = await cashModel.Save(cashtrans2);
                            s = await cashModel.MovePosCash(cashtrans2.cashTransId, MainWindow.userLogin.userId);
                            //   if (s.Equals("transdone"))//tras done so confirm
                            if (s.Equals(1))//tras done so confirm
                                await confirmOpr();
                            else//error then do not confirm
                                Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);

                    HelpClass.EndAwait(grid_main);
                        }
                        //there is not enough balance
                        else
                            Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopNotEnoughBalance"), animation: ToasterAnimation.FadeIn);
                    }
                }
                
            }
            catch (Exception ex)
            {
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async Task confirmOpr()
        {
            cashtrans.isConfirm = 1;
            int s = await cashModel.Save(cashtrans);
            if (!s.Equals(0))
            {
                await RefreshCashesList();
                Tb_search_TextChanged(null, null);

                Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopConfirm"), animation: ToasterAnimation.FadeIn);

                btn_confirm.Content = MainWindow.resourcemanager.GetString("trIsConfirmed");
                btn_confirm.IsEnabled = false;
            }
        }
        #endregion
        #region events
        private async void Tb_search_TextChanged(object sender, TextChangedEventArgs e)
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
        private async void Dg_posAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//selection
            try
            {
                
                    HelpClass.StartAwait(grid_main);
                /*
                HelpClass.clearValidate(tb_cash, p_errorCash);
                HelpClass.clearComboBoxValidate(cb_pos1, p_errorPos1);
                HelpClass.clearComboBoxValidate(cb_pos2, p_errorPos2);

                if (dg_posAccounts.SelectedIndex != -1)
                {
                    cashtrans = dg_posAccounts.SelectedItem as CashTransfer;
                    this.DataContext = cashtrans;

                    if (cashtrans != null)
                    {
                        tb_cash.Text = HelpClass.DecTostring(cashtrans.cash);

                        //creator pos is login pos
                        if (cashtrans.posIdCreator == MainWindow.posID.Value)
                        {
                            btn_update.IsEnabled = true;
                            btn_delete.IsEnabled = true;
                        }
                        else
                        {
                            btn_update.IsEnabled = false;
                            btn_delete.IsEnabled = false;
                        }
                        //login pos is operation pos
                        if (cashtrans.posId == MainWindow.posID.Value)
                        {
                            if (cashtrans.isConfirm != 1)
                            { btn_confirm.Content = MainWindow.resourcemanager.GetString("trConfirm");
                                btn_confirm.IsEnabled = true; }
                            else
                            { btn_confirm.Content = MainWindow.resourcemanager.GetString("trIsConfirmed");
                                btn_confirm.IsEnabled = false; }
                        }
                        else
                        {

                            btn_confirm.IsEnabled = false;
                            if (cashtrans.isConfirm != 1)
                                btn_confirm.Content = MainWindow.resourcemanager.GetString("trConfirm");
                            else
                                btn_confirm.Content = MainWindow.resourcemanager.GetString("trIsConfirmed");
                        }

                        #region get two pos

                        cashes2 = await cashModel.GetbySourcId("p", cashtrans.cashTransId);
                        //to insure that the pull operation is in cashtrans2 
                        if (cashtrans.transType == "p")
                        {
                            cashtrans2 = cashes2.ToList()[0] as CashTransfer;
                            cashtrans3 = cashes2.ToList()[1] as CashTransfer;
                        }
                        else if (cashtrans.transType == "d")
                        {
                            cashtrans2 = cashes2.ToList()[1] as CashTransfer;
                            cashtrans3 = cashes2.ToList()[0] as CashTransfer;
                        }

                        cb_fromBranch.SelectedValue = (MainWindow.posList.Where(x => x.posId == cashtrans2.posId).FirstOrDefault() as Pos).branchId;
                        cb_pos1.SelectedValue = cashtrans2.posId;

                        cb_toBranch.SelectedValue = (MainWindow.posList.Where(x => x.posId == cashtrans3.posId).FirstOrDefault() as Pos).branchId;
                        cb_pos2.SelectedValue = cashtrans3.posId;



                        if ((cashtrans2.isConfirm == 1) && (cashtrans3.isConfirm == 1))
                            btn_update.IsEnabled = false;
                        //else
                        //    btn_update.IsEnabled = true;
                        #endregion
                    }
                }
                */
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {//refresh

                HelpClass.StartAwait(grid_main);

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
        #endregion
        #region Refresh & Search
        async Task Search()
        {
            //search
            if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "show") )
            {
                try
                {
                    /*
                    if (cashes is null)
                        await RefreshCashesList();
                    searchText = tb_search.Text;
                    if (chb_all.IsChecked == false)
                    {
                        switch (Convert.ToInt32(cb_state.SelectedValue))
                        {
                            case 0://inconfirmed
                                cashesQuery = cashes.Where(s => (s.transNum.Contains(searchText)
                                 || s.transType.Contains(searchText)
                                 || s.cash.ToString().Contains(searchText)
                                 || s.posName.Contains(searchText)
                                 )
                                && s.updateDate.Value.Date <= dp_searchEndDate.SelectedDate.Value.Date
                                && s.updateDate.Value.Date >= dp_searchStartDate.SelectedDate.Value.Date
                                && s.posId == MainWindow.posID.Value
                                && s.isConfirm == 0
                                );
                                break;
                            case 1://waiting
                                cashesQuery = cashes.Where(s => (s.transNum.Contains(searchText)
                                || s.transType.Contains(searchText)
                                || s.cash.ToString().Contains(searchText)
                                || s.posName.Contains(searchText)
                                )
                                && s.updateDate.Value.Date <= dp_searchEndDate.SelectedDate.Value.Date
                                && s.updateDate.Value.Date >= dp_searchStartDate.SelectedDate.Value.Date
                                && s.posId == MainWindow.posID.Value
                                && s.isConfirm == 1
                                //&& another is not confirmed 
                                && s.isConfirm2 == 0
                                );
                                break;
                            case 2://confirmed
                                cashesQuery = cashes.Where(s => (s.transNum.Contains(searchText)
                                || s.transType.Contains(searchText)
                                || s.cash.ToString().Contains(searchText)
                                || s.posName.Contains(searchText)
                                )
                                && s.updateDate.Value.Date <= dp_searchEndDate.SelectedDate.Value.Date
                                && s.updateDate.Value.Date >= dp_searchStartDate.SelectedDate.Value.Date
                                && s.posId == MainWindow.posID.Value
                                && s.isConfirm == 1
                                //&& another is confirmed
                                && s.isConfirm2 == 1
                                );
                                break;
                            case 3://created by me
                                cashesQuery = cashes.Where(s => (s.transNum.Contains(searchText)
                                || s.transType.Contains(searchText)
                                || s.cash.ToString().Contains(searchText)
                                || s.posName.Contains(searchText)
                                )
                                && s.updateDate.Value.Date <= dp_searchEndDate.SelectedDate.Value.Date
                                && s.updateDate.Value.Date >= dp_searchStartDate.SelectedDate.Value.Date
                                && s.posIdCreator == MainWindow.posID.Value
                                );
                                break;
                            default://no select
                                cashesQuery = cashes.Where(s => (s.transNum.Contains(searchText)
                                || s.transType.Contains(searchText)
                                || s.cash.ToString().Contains(searchText)
                                || s.posName.Contains(searchText)
                                )
                               && s.updateDate.Value.Date <= dp_searchEndDate.SelectedDate.Value.Date
                               && s.updateDate.Value.Date >= dp_searchStartDate.SelectedDate.Value.Date
                               );
                                break;
                        }
                    }
                    else
                    {
                        switch (Convert.ToInt32(cb_state.SelectedValue))
                        {
                            case 0://inconfirmed
                                cashesQuery = cashes.Where(s => (s.transNum.Contains(searchText)
                                 || s.transType.Contains(searchText)
                                 || s.cash.ToString().Contains(searchText)
                                 || s.posName.Contains(searchText)
                                 )
                                && s.posId == MainWindow.posID.Value
                                && s.isConfirm == 0
                                );
                                break;
                            case 1://waiting
                                cashesQuery = cashes.Where(s => (s.transNum.Contains(searchText)
                                || s.transType.Contains(searchText)
                                || s.cash.ToString().Contains(searchText)
                                || s.posName.Contains(searchText)
                                )
                                && s.posId == MainWindow.posID.Value
                                && s.isConfirm == 1
                                //&& another is not confirmed 
                                && s.isConfirm2 == 0
                                );
                                break;
                            case 2://confirmed
                                cashesQuery = cashes.Where(s => (s.transNum.Contains(searchText)
                                || s.transType.Contains(searchText)
                                || s.cash.ToString().Contains(searchText)
                                || s.posName.Contains(searchText)
                                )
                                && s.posId == MainWindow.posID.Value
                                && s.isConfirm == 1
                                //&& another is confirmed
                                && s.isConfirm2 == 1
                                );
                                break;
                            case 3://created by me
                                cashesQuery = cashes.Where(s => (s.transNum.Contains(searchText)
                                || s.transType.Contains(searchText)
                                || s.cash.ToString().Contains(searchText)
                                || s.posName.Contains(searchText)
                                )
                                && s.posIdCreator == MainWindow.posID.Value
                                );
                                break;
                            default://no select
                                cashesQuery = cashes.Where(s => (s.transNum.Contains(searchText)
                                || s.transType.Contains(searchText)
                                || s.cash.ToString().Contains(searchText)
                                || s.posName.Contains(searchText)
                                )
                               );
                                break;
                        }
                    }

                    RefreshCashView();
                    cashesQueryExcel = cashesQuery.ToList();
                    txt_count.Text = cashesQuery.Count().ToString();
                    */
                }
                catch { }
            }
        }
        async Task<IEnumerable<CashTransfer>> RefreshCashesList()
        {
            cashes = await cashModel.GetCashTransferForPosAsync("all", "p");
            return cashes;

        }
        void RefreshCashView()
        {
            dg_posAccounts.ItemsSource = cashesQuery;
            txt_count.Text = cashesQuery.Count().ToString();
        }
        #endregion
        #region validate - clearValidate - textChange - lostFocus - . . . . 
        void Clear()
        {
            this.DataContext = new Bank();





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
        #region report
        /*
        //report  parameters
        ReportCls reportclass = new ReportCls();
        LocalReport rep = new LocalReport();
        SaveFileDialog saveFileDialog = new SaveFileDialog();

        // end report parameters
        public void BuildReport()
        {

            List<ReportParameter> paramarr = new List<ReportParameter>();

            string addpath;
            bool isArabic = ReportCls.checkLang();
            if (isArabic)
            {
                addpath = @"\Reports\SectionData\banksData\Ar\ArBank.rdlc";
            }
            else
            {
                addpath = @"\Reports\SectionData\banksData\En\EnBank.rdlc";
            }
            string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);
            clsReports.BanksReport(banksQuery, rep, reppath, paramarr);
            clsReports.setReportLanguage(paramarr);
            clsReports.Header(paramarr);

            rep.SetParameters(paramarr);

            rep.Refresh();

        }
        private void Btn_pdf_Click(object sender, RoutedEventArgs e)
        {

            //pdf
            try
            {

                HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report"))
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

        private void Btn_preview_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report"))
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

        private void Btn_print_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report"))
                {

                    #region
                    BuildReport();
                    LocalReportExtensions.PrintToPrinterbyNameAndCopy(rep, FillCombo.rep_printer_name, FillCombo.rep_print_count == null ? short.Parse("1") : short.Parse(FillCombo.rep_print_count));
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
        
        private void Btn_exportToExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report"))
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
        */
        #endregion
        private void Cb_pos1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//pos1selection
            try
            {
                
                    HelpClass.StartAwait(grid_main);
                int bToId = Convert.ToInt32(cb_toBranch.SelectedValue);
                int pFromId = Convert.ToInt32(cb_pos1.SelectedValue);
                var toPos = poss.Where(p => p.branchId == bToId && p.posId != pFromId);
                cb_pos2.ItemsSource = toPos;
                cb_pos2.DisplayMemberPath = "name";
                cb_pos2.SelectedValuePath = "posId";
                cb_pos2.SelectedIndex = -1;

                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Cb_pos2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void Cb_fromBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//fill pos1
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                int bFromId = Convert.ToInt32(cb_fromBranch.SelectedValue);
                var fromPos = poss.Where(p => p.branchId == bFromId);
                cb_pos1.ItemsSource = fromPos;
                cb_pos1.DisplayMemberPath = "name";
                cb_pos1.SelectedValuePath = "posId";
                cb_pos1.SelectedIndex = -1;

                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Cb_toBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { //fill pos combo2
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                int bToId = Convert.ToInt32(cb_toBranch.SelectedValue);
                int pFromId = Convert.ToInt32(cb_pos1.SelectedValue);
                var toPos = poss.Where(p => p.branchId == bToId && p.posId != pFromId);
                cb_pos2.ItemsSource = toPos;
                cb_pos2.DisplayMemberPath = "name";
                cb_pos2.SelectedValuePath = "posId";
                cb_pos2.SelectedIndex = -1;

                
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
        private async void search_Checking(object sender, RoutedEventArgs e)
        {
            try
            {
                /*
                CheckBox cb = sender as CheckBox;
                if (cb.IsFocused)
                {
                    if (cb.Name == "chk_stored")
                    {
                        chk_freezone.IsChecked = false;
                        chk_locked.IsChecked = false;
                        btn_transfer.Visibility = Visibility.Visible;
                        btn_locked.Visibility = Visibility.Collapsed;
                        dg_itemsStorage.Columns[6].Visibility = Visibility.Collapsed; //make order num column unvisible
                        dg_itemsStorage.Columns[3].Visibility = Visibility.Visible;
                        dg_itemsStorage.Columns[4].Visibility = Visibility.Visible;
                    }
                    else if (cb.Name == "chk_freezone")
                    {
                        chk_stored.IsChecked = false;
                        chk_locked.IsChecked = false;
                        btn_transfer.Visibility = Visibility.Visible;
                        btn_locked.Visibility = Visibility.Collapsed;
                        dg_itemsStorage.Columns[6].Visibility = Visibility.Collapsed; //make order num column unvisible
                        dg_itemsStorage.Columns[3].Visibility = Visibility.Visible;
                        dg_itemsStorage.Columns[4].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        chk_stored.IsChecked = false;
                        chk_freezone.IsChecked = false;
                        btn_locked.Visibility = Visibility.Visible;
                        btn_transfer.Visibility = Visibility.Collapsed;
                        dg_itemsStorage.Columns[6].Visibility = Visibility.Visible; //make order num column visible
                        dg_itemsStorage.Columns[3].Visibility = Visibility.Collapsed;
                        dg_itemsStorage.Columns[4].Visibility = Visibility.Collapsed;
                    }
                }
                HelpClass.StartAwait(grid_main);
                await RefreshItemLocationsList();
                await Search();
                Clear();
                HelpClass.EndAwait(grid_main);
                */
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void chk_uncheck(object sender, RoutedEventArgs e)
        {
            try
            {
                /*
                CheckBox cb = sender as CheckBox;
                if (cb.IsFocused)
                {
                    if (cb.Name == "chk_stored")
                        chk_stored.IsChecked = true;
                    else if (cb.Name == "chk_freezone")
                        chk_freezone.IsChecked = true;
                    else
                        chk_locked.IsChecked = true;
                }
                */
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

    }
}
