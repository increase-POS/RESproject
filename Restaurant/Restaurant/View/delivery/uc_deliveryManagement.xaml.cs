using netoaster;
using Restaurant.Classes;
using Restaurant.Classes.ApiClasses;
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

namespace Restaurant.View.delivery
{
    /// <summary>
    /// Interaction logic for uc_deliveryManagement.xaml
    /// </summary>
    public partial class uc_deliveryManagement : UserControl
    {
        public uc_deliveryManagement()
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
        private static uc_deliveryManagement _instance;
        public static uc_deliveryManagement Instance
        {
            get
            {
                //if (_instance == null)
                if(_instance is null)
                _instance = new uc_deliveryManagement();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        string basicsPermission = "reservationsUpdate_basics";
        IEnumerable<Invoice> orders;
        IEnumerable<User> drivers;
        User userModel = new User();
        OrderPreparing orderModel = new OrderPreparing();
        Invoice order = new Invoice();
        
        string searchText = "";
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
                requiredControlList = new List<string> { "userId" , "deliveryTime" };

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

                #region fill drivers
                drivers = await userModel.GetUsersActive();
                //cb_userId.ItemsSource = drivers.Where(x => x.job == "deliveryEmployee" && x.driverIsAvailable == 1);
                cb_userId.ItemsSource = drivers.Where(x => x.job == "deliveryEmployee");
                cb_userId.DisplayMemberPath = "fullName";
                cb_userId.SelectedValuePath = "userId";

                cb_searchUser.ItemsSource = cb_userId.ItemsSource;
                cb_searchUser.DisplayMemberPath = "fullName";
                cb_searchUser.SelectedValuePath = "userId";
                #endregion

                chk_allForDelivery.IsChecked = true;

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        #region methods
        async Task Search()
        {
            try
            {
                searchText = tb_search.Text.ToLower();
                if (chk_allForDelivery.IsChecked == true)
                {
                    await RefreshOrdersList("");
                }
                else if (chk_readyForDelivery.IsChecked == true)
                {
                    await RefreshOrdersList("Ready");
                }
                else if (chk_withDeliveryMan.IsChecked == true)
                {
                    await RefreshOrdersList("Collected");
                }
                else if (chk_inTheWay.IsChecked == true)
                {
                    await RefreshOrdersList("InTheWay");
                   
                }
                orders = orders.Where(s => (s.invNumber.Contains(searchText)
                      || s.shipUserName.ToString().Contains(searchText)
                      //|| s.deliveryTime.ToString().Contains(searchText)/////?????????????????
                      )
                  );

                RefreshOrdersView();
            }
            catch { }
        }

        async Task<IEnumerable<Invoice>> RefreshOrdersList(string status)
        {
            orders = await orderModel.GetOrdersWithDelivery(MainWindow.branchLogin.branchId, status);
            return orders;
        }
        void RefreshOrdersView()
        {
            dg_orders.ItemsSource = orders;
            txt_count.Text = orders.Count().ToString();
        }

        void Clear()
        {
            order = new Invoice();
            btn_save.IsEnabled = true;
            btn_save.Content = AppSettings.resourcemanager.GetString("trCollect");
            selectedOrders.Clear();
            this.DataContext = order;

            HelpClass.clearValidate(requiredControlList, this);
        }
        private void translate()
        {
            txt_title.Text = AppSettings.resourcemanager.GetString("trDeliveryManagement");
            txt_baseInformation.Text = AppSettings.resourcemanager.GetString("trUserInformation");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_search, AppSettings.resourcemanager.GetString("trSearchHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_searchUser, AppSettings.resourcemanager.GetString("deliveryMan")+"...");

            chk_allForDelivery.Content = AppSettings.resourcemanager.GetString("trAll");
            chk_readyForDelivery.Content = AppSettings.resourcemanager.GetString("readyForDelivery");
            chk_withDeliveryMan.Content = AppSettings.resourcemanager.GetString("withDeliveryMan");
            chk_inTheWay.Content = AppSettings.resourcemanager.GetString("inTheWay");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_userId, AppSettings.resourcemanager.GetString("deliveryMan") + "...");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_deliveryTime, AppSettings.resourcemanager.GetString("deliveryTime") + "...");
            txt_minutes.Text = AppSettings.resourcemanager.GetString("minute");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_notes, AppSettings.resourcemanager.GetString("trNotes") + "..."); 

            dg_orders.Columns[1].Header = AppSettings.resourcemanager.GetString("trCode");
            dg_orders.Columns[2].Header = AppSettings.resourcemanager.GetString("deliveryMan");
            dg_orders.Columns[3].Header = AppSettings.resourcemanager.GetString("deliveryTime");
            dg_orders.Columns[4].Header = AppSettings.resourcemanager.GetString("trStatus");

            btn_clear.ToolTip = AppSettings.resourcemanager.GetString("trClear");
            tt_refresh.Content = AppSettings.resourcemanager.GetString("trRefresh");
            tt_report.Content = AppSettings.resourcemanager.GetString("trPdf");
            tt_print.Content = AppSettings.resourcemanager.GetString("trPrint");
            tt_excel.Content = AppSettings.resourcemanager.GetString("trExcel");
            tt_count.Content = AppSettings.resourcemanager.GetString("trCount");

            btn_save.Content = AppSettings.resourcemanager.GetString("trSave");
        }
        #endregion

        #region Refresh - Clear - Search - Select - Save
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
        private async void Dg_orders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//selection
            try
            {
                HelpClass.StartAwait(grid_main);
                if (dg_orders.SelectedIndex != -1)
                {
                    order = dg_orders.SelectedItem as Invoice;
                    this.DataContext = order;
                    if (order != null)
                    {
                        
                        //refreshSaveBtnText
                        if (chk_readyForDelivery.IsChecked == true)
                            btn_save.Content = AppSettings.resourcemanager.GetString("trCollect");
                        else if (chk_withDeliveryMan.IsChecked == true)
                            btn_save.Content = AppSettings.resourcemanager.GetString("inTheWay");
                        else if (chk_withDeliveryMan.IsChecked == true)
                        {
                            btn_save.Content = AppSettings.resourcemanager.GetString("trDone");
                            btn_save.IsEnabled = false;
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
        private async void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {//refresh
            try
            {
                HelpClass.StartAwait(grid_main);

                searchText = "";
                tb_search.Text = "";
                chk_readyForDelivery.IsChecked = true;
                await Search();
                
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Btn_save_Click(object sender, RoutedEventArgs e)
        {//save
            try
            {
                //if (FillCombo.groupObject.HasPermissionAction(basicsPermission, FillCombo.groupObjects, "add"))
                {
                    HelpClass.StartAwait(grid_main);
                    #region add
                    if (HelpClass.validate(requiredControlList, this))
                    {
                        orderPreparingStatus ops = new orderPreparingStatus();

                        if(chk_readyForDelivery.IsChecked == true)
                            ops.status = "Collected";
                        else if (chk_withDeliveryMan.IsChecked == true)
                            ops.status = "InTheWay";
                        else if (chk_withDeliveryMan.IsChecked == true)
                            ops.status = "Done";

                        ops.createUserId = MainWindow.userLogin.userId;
                        ops.updateUserId = MainWindow.userLogin.userId;
                        ops.notes = tb_notes.Text;
                        ops.isActive = 1;

                        int res = await orderModel.EditInvoiceOrdersStatus(order.invoiceId,(int)order.shipUserId, ops);

                        if (!res.Equals(0))
                        {
                            Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);
                            //refreshSaveBtnText
                            if (chk_readyForDelivery.IsChecked == true)
                                btn_save.Content = AppSettings.resourcemanager.GetString("trCollect");
                            else if (chk_withDeliveryMan.IsChecked == true)
                                btn_save.Content = AppSettings.resourcemanager.GetString("inTheWay");
                            else if (chk_withDeliveryMan.IsChecked == true)
                            {
                                btn_save.Content = AppSettings.resourcemanager.GetString("trDone");
                                btn_save.IsEnabled = false;
                            }

                            await Search();
                        }
                        else
                            Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                    }
                    
                    #endregion
                    
                    HelpClass.EndAwait(grid_main);
                }
                //else
                //    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
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

                if (FillCombo.groupObject.HasPermissionAction(basicsPermission, FillCombo.groupObjects, "report") || SectionData.isAdminPermision())
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

                if (FillCombo.groupObject.HasPermissionAction(basicsPermission, FillCombo.groupObjects, "report") || SectionData.isAdminPermision())
                {
                    #region
                    Window.GetWindow(this).Opacity = 0.2;
                    win_lvcCatalog win = new win_lvcCatalog(itemsQuery, 3);
                    // // w.ShowInTaskbar = false;
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

                if (FillCombo.groupObject.HasPermissionAction(basicsPermission, FillCombo.groupObjects, "report") || SectionData.isAdminPermision())
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
                    // w.ShowInTaskbar = false;
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

                if (FillCombo.groupObject.HasPermissionAction(basicsPermission, FillCombo.groupObjects, "report") || SectionData.isAdminPermision())
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

        #region events
        List<Invoice> selectedOrders = new List<Invoice>();
        private void FieldDataGridChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox cb = sender as CheckBox;
                Invoice selectedOrder = dg_orders.SelectedItem as Invoice;
                selectedOrders.Add(selectedOrder);

            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void FieldDataGridUnchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox cb = sender as CheckBox;
                var index = dg_orders.SelectedIndex;
                Invoice selectedOrder = dg_orders.SelectedItem as Invoice;
                selectedOrders.Remove(selectedOrder);

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
                CheckBox cb = sender as CheckBox;
                if (cb.IsChecked == true)
                {
                    if (cb.Name == "chk_allForDelivery")
                    {
                        chk_readyForDelivery.IsChecked = false;
                        chk_withDeliveryMan.IsChecked = false;
                        chk_inTheWay.IsChecked = false;
                    }
                    else if (cb.Name == "chk_readyForDelivery")
                    {
                        chk_allForDelivery.IsChecked = false;
                        chk_withDeliveryMan.IsChecked = false;
                        chk_inTheWay.IsChecked = false;
                    }
                    else if (cb.Name == "chk_withDeliveryMan")
                    {
                        chk_allForDelivery.IsChecked = false;
                        chk_readyForDelivery.IsChecked = false;
                        chk_inTheWay.IsChecked = false;
                    }
                    else if (cb.Name == "chk_inTheWay")
                    {
                        chk_allForDelivery.IsChecked = false;
                        chk_readyForDelivery.IsChecked = false;
                        chk_withDeliveryMan.IsChecked = false;
                    }
                }
                HelpClass.StartAwait(grid_main);

                Clear();
                await Search();

                HelpClass.EndAwait(grid_main);
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
                CheckBox cb = sender as CheckBox;
                if (cb.IsFocused)
                {
                    if (cb.Name == "chk_allForDelivery")
                        chk_allForDelivery.IsChecked = true;
                    else if (cb.Name == "chk_readyForDelivery")
                        chk_readyForDelivery.IsChecked = true;
                    else if (cb.Name == "chk_withDeliveryMan")
                        chk_withDeliveryMan.IsChecked = true;
                    else if (cb.Name == "chk_inTheWay")
                        chk_inTheWay.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
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
