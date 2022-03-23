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
using System.Windows.Shapes;


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
        //CashTransfer cashModel = new CashTransfer();
        AgentMembershipCash subscription = new AgentMembershipCash();
        string searchText = "";
        byte tgl_subscriptionState;
        ReportCls reportclass = new ReportCls();
        LocalReport rep = new LocalReport();
        SaveFileDialog saveFileDialog = new SaveFileDialog();

        string createPermission = "subscriptions_create";
        string reportsPermission = "subscriptions_reports";

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

                requiredControlList = new List<string> { "customerId" , "monthsCount" , "amount" , "paymentProcessType" };
                
                //expirDate

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

                FillCombo.FillDefaultPayType_cashChequeCard(cb_paymentProcessType);

                #region fillCustomers
                cb_customerId.ItemsSource = await subscription.GetAgentToPay();
                cb_customerId.DisplayMemberPath = "agentName";
                cb_customerId.SelectedValuePath = "agentId";
                cb_customerId.SelectedIndex = -1;
                #endregion

                Keyboard.Focus(cb_customerId);

                await RefreshSubscriptionsList();
                await Search();

                Clear();

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
            txt_title.Text = AppSettings.resourcemanager.GetString("trSubscription");
            txt_baseInformation.Text = AppSettings.resourcemanager.GetString("trBaseInformation");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_search, AppSettings.resourcemanager.GetString("trSearchHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_transNum, AppSettings.resourcemanager.GetString("trTransNumberHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_customerId, AppSettings.resourcemanager.GetString("trCustomerHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_monthsCount, AppSettings.resourcemanager.GetString("trMonthCount")+"...");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_amount, AppSettings.resourcemanager.GetString("trAmountHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_paymentProcessType, AppSettings.resourcemanager.GetString("trPaymentProcessType"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_notes, AppSettings.resourcemanager.GetString("trNoteHint"));

            dg_subscription.Columns[0].Header = AppSettings.resourcemanager.GetString("trTransNumber");
            dg_subscription.Columns[1].Header = AppSettings.resourcemanager.GetString("trCustomer");
            dg_subscription.Columns[2].Header = AppSettings.resourcemanager.GetString("trSubscriptionType");
            dg_subscription.Columns[3].Header = AppSettings.resourcemanager.GetString("trExpireDate");
            dg_subscription.Columns[4].Header = AppSettings.resourcemanager.GetString("trAmount");

            btn_clear.ToolTip = AppSettings.resourcemanager.GetString("trClear");

            tt_clear.Content = AppSettings.resourcemanager.GetString("trClear");
            tt_report.Content = AppSettings.resourcemanager.GetString("trPdf");
            tt_excel.Content = AppSettings.resourcemanager.GetString("trExcel");
            tt_count.Content = AppSettings.resourcemanager.GetString("trCount");

            btn_save.Content = AppSettings.resourcemanager.GetString("trSave");
        }

        #region Add - Update - Delete - Search - Tgl - Clear - DG_SelectionChanged - refresh
        private async void Btn_add_Click(object sender, RoutedEventArgs e)
        {//add
            /*
            try
            {
                if (FillCombo.groupObject.HasPermissionAction(basicsPermission, FillCombo.groupObjects, "add") || HelpClass.isAdminPermision())
                {
                    HelpClass.StartAwait(grid_main);



                    agent = new Agent();
                    if (HelpClass.validate(requiredControlList, this) && HelpClass.IsValidEmail(this))
                    {
                        //deserve
                        decimal maxDeserveValue = 0;
                        if (!tb_upperLimit.Text.Equals(""))
                            maxDeserveValue = decimal.Parse(tb_upperLimit.Text);

                        //payType
                        string payType = "";
                        if (cb_payType.SelectedIndex != -1)
                            payType = cb_payType.SelectedValue.ToString();

                        //tb_code.Text = await agent.generateCodeNumber("c");
                        agent.code = await agent.generateCodeNumber("c");
                        agent.name = tb_name.Text;
                        agent.company = tb_company.Text;
                        agent.address = tb_address.Text;
                        agent.email = tb_email.Text;
                        agent.mobile = cb_areaMobile.Text + "-" + tb_mobile.Text;
                        if (!tb_phone.Text.Equals(""))
                            agent.phone = cb_areaPhone.Text + "-" + cb_areaPhoneLocal.Text + "-" + tb_phone.Text;
                        if (!tb_fax.Text.Equals(""))
                            agent.fax = cb_areaFax.Text + "-" + cb_areaFaxLocal.Text + "-" + tb_fax.Text;
                        agent.type = "c";
                        agent.accType = "";
                        agent.balance = 0;
                        agent.payType = payType;
                        agent.isLimited = (bool)tgl_hasCredit.IsChecked;
                        agent.createUserId = MainWindow.userLogin.userId;
                        agent.updateUserId = MainWindow.userLogin.userId;
                        agent.notes = tb_notes.Text;
                        agent.isActive = 1;
                        agent.maxDeserve = maxDeserveValue;

                        int s = await agent.save(agent);
                        if (s <= 0)
                            Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                        else
                        {
                            Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);

                            if (isImgPressed)
                            {
                                int agentId = s;
                                string b = await agent.uploadImage(imgFileName,
                                    Md5Encription.MD5Hash("Inc-m" + agentId.ToString()), agentId);
                                agent.image = b;
                                isImgPressed = false;
                            }

                            Clear();
                            await RefreshCustomersList();
                            await Search();
                        }
                    }
                    HelpClass.EndAwait(grid_main);
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
            */
        }
        private async void Btn_update_Click(object sender, RoutedEventArgs e)
        {//update
         /*
                     try
                     {
                         if (FillCombo.groupObject.HasPermissionAction(basicsPermission, FillCombo.groupObjects, "update") || HelpClass.isAdminPermision())
                         {
                             HelpClass.StartAwait(grid_main);
                             if (HelpClass.validate(requiredControlList, this) && HelpClass.IsValidEmail(this))
                             {
                                 //deserve
                                 decimal maxDeserveValue = 0;
                                 if (!tb_upperLimit.Text.Equals(""))
                                     maxDeserveValue = decimal.Parse(tb_upperLimit.Text);

                                 //payType
                                 string payType = "";
                                 if (cb_payType.SelectedIndex != -1)
                                     payType = cb_payType.SelectedValue.ToString();

                                 //agent.code = "Us-000001";
                                 //agent.custname = tb_custname.Text;
                                 //tb_code.Text = await agent.generateCodeNumber("c");
                                 //agent.code = await agent.generateCodeNumber("c");
                                 agent.name = tb_name.Text;
                                 agent.company = tb_company.Text;
                                 agent.email = tb_email.Text;
                                 agent.address = tb_address.Text;
                                 agent.mobile = cb_areaMobile.Text + "-" + tb_mobile.Text;
                                 if (!tb_phone.Text.Equals(""))
                                     agent.phone = cb_areaPhone.Text + "-" + cb_areaPhoneLocal.Text + "-" + tb_phone.Text;
                                 if (!tb_fax.Text.Equals(""))
                                     agent.fax = cb_areaFax.Text + "-" + cb_areaFaxLocal.Text + "-" + tb_fax.Text;
                                 agent.payType = payType;
                                 agent.isLimited = (bool)tgl_hasCredit.IsChecked;
                                 agent.updateUserId = MainWindow.userLogin.userId;
                                 agent.notes = tb_notes.Text;
                                 agent.maxDeserve = maxDeserveValue;

                                 int s = await agent.save(agent);
                                 if (s <= 0)
                                     Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                                 else
                                 {
                                     Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);
                                     await RefreshCustomersList();
                                     await Search();
                                     if (isImgPressed)
                                     {
                                         int agentId = s;
                                         string b = await agent.uploadImage(imgFileName, Md5Encription.MD5Hash("Inc-m" + agentId.ToString()), agentId);
                                         agent.image = b;
                                         isImgPressed = false;
                                         if (!b.Equals(""))
                                         {
                                             await getImg();
                                         }
                                         else
                                         {
                                             HelpClass.clearImg(btn_image);
                                         }
                                     }
                                 }
                             }
                             HelpClass.EndAwait(grid_main);
                         }
                         else
                             Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

                     }
                     catch (Exception ex)
                     {
                         HelpClass.EndAwait(grid_main);
                         HelpClass.ExceptionMessage(ex, this);
                     }
                     */
        }
        private async void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            /*
                        try
                        {//delete
                            if (FillCombo.groupObject.HasPermissionAction(basicsPermission, FillCombo.groupObjects, "delete") || HelpClass.isAdminPermision())
                            {
                                HelpClass.StartAwait(grid_main);
                                if (agent.agentId != 0)
                                {
                                    if ((!agent.canDelete) && (agent.isActive == 0))
                                    {
                                        #region
                                        Window.GetWindow(this).Opacity = 0.2;
                                        wd_acceptCancelPopup w = new wd_acceptCancelPopup();
                                        w.contentText = AppSettings.resourcemanager.GetString("trMessageBoxActivate");
                                        w.ShowDialog();
                                        Window.GetWindow(this).Opacity = 1;
                                        #endregion
                                        if (w.isOk)
                                            await activate();
                                    }
                                    else
                                    {
                                        #region
                                        Window.GetWindow(this).Opacity = 0.2;
                                        wd_acceptCancelPopup w = new wd_acceptCancelPopup();
                                        if (agent.canDelete)
                                            w.contentText = AppSettings.resourcemanager.GetString("trMessageBoxDelete");
                                        if (!agent.canDelete)
                                            w.contentText = AppSettings.resourcemanager.GetString("trMessageBoxDeactivate");
                                        w.ShowDialog();
                                        Window.GetWindow(this).Opacity = 1;
                                        #endregion
                                        if (w.isOk)
                                        {
                                            string popupContent = "";
                                            if (agent.canDelete) popupContent = AppSettings.resourcemanager.GetString("trPopDelete");
                                            if ((!agent.canDelete) && (agent.isActive == 1)) popupContent = AppSettings.resourcemanager.GetString("trPopInActive");

                                            int s = await agent.delete(agent.agentId, MainWindow.userLogin.userId, agent.canDelete);
                                            if (s < 0)
                                                Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                                            else
                                            {
                                                Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopDelete"), animation: ToasterAnimation.FadeIn);

                                                await RefreshCustomersList();
                                                await Search();
                                                Clear();
                                            }
                                        }
                                    }
                                }
                                HelpClass.EndAwait(grid_main);
                            }
                            else
                                Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

                        }
                        catch (Exception ex)
                        {
                            HelpClass.EndAwait(grid_main);
                            HelpClass.ExceptionMessage(ex, this);
                        }
                        */
        }
        private async Task activate()
        {//activate
         /*
                     agent.isActive = 1;
                     int s = await agent.save(agent);
                     if (s <= 0)
                         Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                     else
                     {
                         Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopActive"), animation: ToasterAnimation.FadeIn);
                         await RefreshCustomersList();
                         await Search();
                     }
                     */
        }
        #endregion

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
            try
            {
                HelpClass.StartAwait(grid_main);
         
                if (dg_subscription.SelectedIndex != -1)
                {
                    subscription = dg_subscription.SelectedItem as AgentMembershipCash;
                    this.DataContext = subscription;
                    if (subscription != null)
                    {
                        switch(subscription.subscriptionType)
                        {
                            case "m":
                                bdr_monthCount.Visibility = Visibility.Visible;
                                MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_monthsCount, AppSettings.resourcemanager.GetString("trYearCount") + "...");
                                fillMonthCount();
                                break;
                            case "y":
                                bdr_monthCount.Visibility = Visibility.Visible;
                                MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_monthsCount, AppSettings.resourcemanager.GetString("trMonthCount") + "...");
                                fillMonthCount();
                                break;
                            case "o":
                                bdr_monthCount.Visibility = Visibility.Collapsed;
                                cb_monthsCount.SelectedIndex = -1;
                                cb_monthsCount.ItemsSource = null;
                                break;
                        }
                       
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

        private async void fillMonthCount()
        {
            SubscriptionFees subFee = new SubscriptionFees();
            List<SubscriptionFees> subFees = await subFee.GetAll();
            subFees = subFees.Where(s => s.membershipId == subscription.membershipId).ToList();
            cb_monthsCount.DisplayMemberPath = "monthsCount";
            cb_monthsCount.SelectedValuePath = "subscriptionFeesId";
            cb_monthsCount.ItemsSource = subFees;
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

            subscriptionsQuery = subscriptions
                .Where(s => 
            (
            s.transNum.ToLower().Contains(searchText) ||
            s.agentName.ToLower().Contains(searchText) ||
            s.subscriptionType.ToLower().Contains(searchText)||
            //s.subscriptionType.ToLower().Contains(searchText)||expireDate
            s.Amount.ToString().ToLower().Contains(searchText)
            ) 
            && 
            s.isActive == tgl_subscriptionState);

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
        void Clear()
        {
            this.DataContext = new Agent();
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
        // report
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
                addpath = @"\Reports\Sale\Ar\PackageReport.rdlc";
            }
            else
                addpath = @"\Reports\Sale\En\PackageReport.rdlc";
            string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);

            ReportCls.checkLang();

            clsReports.packageReport(itemsQuery, rep, reppath, paramarr);
            clsReports.setReportLanguage(paramarr);
            clsReports.Header(paramarr);

            rep.SetParameters(paramarr);

            rep.Refresh();
        }
        public void pdfpackage()
        {

            BuildReport();

            this.Dispatcher.Invoke(() =>
            {
                saveFileDialog.Filter = "PDF|*.pdf;";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filepath = saveFileDialog.FileName;
                    LocalReportExtensions.ExportToPDF(rep, filepath);
                }
            });
        }
        private void Btn_pdf_Click(object sender, RoutedEventArgs e)
        {//pdf
            try
            {

                if (sender != null)
                    SectionData.StartAwait(grid_main);
                if (FillCombo.groupObject.HasPermissionAction(basicsPermission, FillCombo.groupObjects, "report") || SectionData.isAdminPermision())
                {
                    /////////////////////////////////////
                    Thread t1 = new Thread(() =>
                    {
                        pdfpackage();
                    });
                    t1.Start();
                    //////////////////////////////////////
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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
        public void printpackage()
        {
            BuildReport();

            this.Dispatcher.Invoke(() =>
            {
                LocalReportExtensions.PrintToPrinterbyNameAndCopy(rep, MainWindow.rep_printer_name, short.Parse(MainWindow.rep_print_count));
            });
        }
        private void Btn_print_Click(object sender, RoutedEventArgs e)
        {//print
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_main);

                if (FillCombo.groupObject.HasPermissionAction(reportsPermission, FillCombo.groupObjects, "one") )
                {
                    /////////////////////////////////////
                    Thread t1 = new Thread(() =>
                    {
                        printpackage();
                    });
                    t1.Start();
                    //////////////////////////////////////

                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

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
        private void Btn_pieChart_Click(object sender, RoutedEventArgs e)
        {//pie
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_main);

                if (FillCombo.groupObject.HasPermissionAction(reportsPermission, FillCombo.groupObjects, "one"))
                {
                    #region
                    Window.GetWindow(this).Opacity = 0.2;
                    win_lvcCatalog win = new win_lvcCatalog(itemsQuery, 3);
                    win.ShowDialog();
                    Window.GetWindow(this).Opacity = 1;
                    #endregion
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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
        private void Btn_preview_Click(object sender, RoutedEventArgs e)
        {//preview
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_main);

                if (FillCombo.groupObject.HasPermissionAction(reportsPermission, FillCombo.groupObjects, "one"))
                {
                    #region
                    Window.GetWindow(this).Opacity = 0.2;
                    /////////////////////
                    string pdfpath = "";
                    pdfpath = @"\Thumb\report\temp.pdf";
                    pdfpath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, pdfpath);
                    BuildReport();
                    LocalReportExtensions.ExportToPDF(rep, pdfpath);
                    ///////////////////
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
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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
        public void ExcelPackage()
        {

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
        }
        private void Btn_exportToExcel_Click(object sender, RoutedEventArgs e)
        {//excel
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_main);

                if (FillCombo.groupObject.HasPermissionAction(reportsPermission, FillCombo.groupObjects, "one"))
                {
                    Thread t1 = new Thread(() =>
                    {
                        ExcelPackage();

                    });
                    t1.Start();
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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
        */
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

        private void Cb_paymentProcessType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            try
            {
                HelpClass.StartAwait(grid_main);
                TimeSpan elapsed = (DateTime.Now - _lastKeystroke);
                if (elapsed.TotalMilliseconds > 100 && cb_paymentProcessType.SelectedIndex != -1)
                {
                    _SelectedPaymentType = cb_paymentProcessType.SelectedValue.ToString();
                }
                else
                {
                    cb_paymentProcessType.SelectedValue = _SelectedPaymentType;
                }

                switch (cb_paymentProcessType.SelectedIndex)
                {
                    case 0://cash
                        gd_card.Visibility = Visibility.Collapsed;
                        tb_processNum.Clear();
                        _SelectedCard = -1;
                        txt_card.Text = "";
                        dp_desrvedDate.IsEnabled = false;
                        // update validate list
                        requiredControlList = new List<string> { };
                        break;
                    case 1:// balance
                        gd_card.Visibility = Visibility.Collapsed;
                        dp_desrvedDate.IsEnabled = true;
                        tb_processNum.Clear();
                        _SelectedCard = -1;
                        // update validate list
                        requiredControlList = new List<string> { "vendor", "invoiceNumber", "desrvedDate" };
                        break;
                    case 2://card
                        dp_desrvedDate.IsEnabled = false;
                        gd_card.Visibility = Visibility.Visible;
                        // update validate list
                        requiredControlList = new List<string> { "card" };
                        break;
                    case 3://multiple
                        gd_card.Visibility = Visibility.Collapsed;
                        tb_processNum.Clear();
                        _SelectedCard = -1;
                        txt_card.Text = "";
                        dp_desrvedDate.IsEnabled = true;
                        // update validate list
                        requiredControlList = new List<string> { };
                        break;

                }
                HelpClass.validate(requiredControlList, this);
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
            */
        }

        private async void Btn_save_Click(object sender, RoutedEventArgs e)
        {//save
            try
            {
                HelpClass.StartAwait(grid_main);

                if (FillCombo.groupObject.HasPermissionAction(createPermission, FillCombo.groupObjects, "one"))
                {
                    //subscription = new AgentMembershipCash();
                    if (HelpClass.validate(requiredControlList, this))
                    {
                        CashTransfer cashtrans = new CashTransfer();

                        int s = await cashtrans.Save(cashtrans);
                        //save cashTransfer
                        /*
                         public int cashTransId { get; set; }
        public string transType { get; set; }
        public string transNum { get; set; }
        public decimal cash { get; set; }
        public Nullable<int> updateUserId { get; set; }
        public Nullable<int> createUserId { get; set; }
        public string notes { get; set; }
        public string side { get; set; }
        public string docName { get; set; }
        public string docNum { get; set; }
        public string agentName { get; set; }
        public string processType { get; set; }
        public Nullable<int> cardId { get; set; }
        public string cardName { get; set; }// processType=card
                         */
                        if (!s.Equals(0))
                        {
                            //save agentCash
                            subscription.Amount = decimal.Parse(tb_amount.Text);
                            subscription.monthsCount = (int)cb_monthsCount.SelectedValue;
                            subscription.cashTransId = cashtrans.cashTransId;
                            subscription.transNum = cashtrans.transNum;
                            subscription.transType = cashtrans.processType;//??????????

                            int res = await subscription.save(subscription);

                            if (res <= 0)
                                Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                            else
                            {
                                Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);

                                Clear();
                                await RefreshSubscriptionsList();
                                await Search();
                            }
                        }
                        /*AgentCash
            public Nullable<int> subscriptionFeesId { get; set; }
            public Nullable<int> cashTransId { get; set; }
            public Nullable<int> membershipId { get; set; }
            public Nullable<int> agentId { get; set; }
            public Nullable<System.DateTime> startDate { get; set; }
            public Nullable<System.DateTime> EndDate { get; set; }
            public string notes { get; set; }
            public Nullable<System.DateTime> createDate { get; set; }
            public Nullable<System.DateTime> updateDate { get; set; }
            public Nullable<int> createUserId { get; set; }
            public Nullable<int> updateUserId { get; set; }
            public byte isActive { get; set; }
            public decimal Amount { get; set; }
            public bool canDelete { get; set; }
            public Nullable<int> monthsCount { get; set; }
            public string agentName { get; set; }
            public string agentcode { get; set; }
            public string agentcompany { get; set; }
            public string agenttype { get; set; }
            public string membershipName { get; set; }
            public string membershipcode { get; set; }
            public string transType { get; set; }
            public string transNum { get; set; }
            public Nullable<System.DateTime> payDate { get; set; }
            public byte membershipisActive { get; set; }
            public int agentMembershipCashId { get; set; }
            public string subscriptionType { get; set; }

                         */
                    }
                    else
                        Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

                    HelpClass.EndAwait(grid_main);
                }
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Chb_all_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Chb_all_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private async void Cb_customerId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                AgenttoPayCash a = cb_customerId.SelectedItem as AgenttoPayCash;
                if (a != null)
                {
                    if ((a.subscriptionType == "o") || (a.subscriptionType == "f"))
                    {
                        cb_monthsCount.SelectedIndex = -1;
                        bdr_monthCount.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        SubscriptionFees subFee = new SubscriptionFees();
                        List<SubscriptionFees> subFees = await subFee.GetAll();
                        subFees = subFees.Where(s => s.membershipId == a.membershipId).ToList();
                        cb_monthsCount.DisplayMemberPath = "monthsCount";
                        cb_monthsCount.SelectedValuePath = "subscriptionFeesId";
                        cb_monthsCount.ItemsSource = subFees;

                        if (a.subscriptionType == "m")
                            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_monthsCount, AppSettings.resourcemanager.GetString("trMonthCount") + "...");
                        else if(a.subscriptionType == "y")
                            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_monthsCount, AppSettings.resourcemanager.GetString("trYearCount") + "...");
                    }
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
    }
}
