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

namespace Restaurant.View.storage.stocktakingOperations
{
    /// <summary>
    /// Interaction logic for uc_itemsDestructive.xaml
    /// </summary>
    public partial class uc_itemsDestructive : UserControl
    {
        public uc_itemsDestructive()
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
        private static uc_itemsShortage _instance;
        public static uc_itemsShortage Instance
        {
            get
            {
                //if (_instance == null)
                _instance = new uc_itemsShortage();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        string destroyPermission = "itemsShortage_destroy";
        string reportsPermission = "itemsShortage_reports";

        InventoryItemLocation invItemLoc = new InventoryItemLocation();
        IEnumerable<InventoryItemLocation> invItemLocsQuery;
        IEnumerable<InventoryItemLocation> invItemLocs;
        byte tgl_invItemLocState;
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
                requiredControlList = new List<string> { "amount", "fallCause" };
                if (MainWindow.lang.Equals("en"))
                {
                    MainWindow.resourcemanager = new ResourceManager("Restaurant.en_file", Assembly.GetExecutingAssembly());
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    MainWindow.resourcemanager = new ResourceManager("Restaurant.ar_file", Assembly.GetExecutingAssembly());
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                }
                translate();

                Keyboard.Focus(tb_amount);
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


        }
        #region Add - Update - Delete - Search - Tgl - Clear - DG_SelectionChanged - refresh

        private async void Btn_destroy_Click(object sender, RoutedEventArgs e)
        {
            /*
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(destroyPermission, MainWindow.groupObjects, "one") )
                {
                    bool valid = validateDistroy();
                    if (valid)
                    {
                        int itemUnitId = 0;
                        int itemId = 0;
                        int invoiceId = 0;
                        string serialNum = "";
                        if (invItemLoc.id != 0)
                        {
                            itemUnitId = invItemLoc.itemUnitId;
                            itemId = invItemLoc.itemId;
                        }
                        else
                        {
                            itemUnitId = (int)cb_unit.SelectedValue;
                            itemId = (int)cb_item.SelectedValue;
                        }
                        if (_ItemType == "sn")
                            serialNum = tb_serialNum.Text;
                        invItemLoc.cause = tb_reasonOfDestroy.Text;
                        invItemLoc.notes = tb_notes.Text;
                        if (lst_serials.Items.Count > 0)
                        {
                            for (int j = 0; j < lst_serials.Items.Count; j++)
                            {
                                serialNum += lst_serials.Items[j];
                                if (j != lst_serials.Items.Count - 1)
                                    serialNum += ",";
                            }
                        }
                        // decimal price = await invoiceModel.GetAvgItemPrice(itemUnitId, itemId);
                        decimal price = 0;
                        decimal total = 0;

                        #region invoice Object
                        invoiceModel.invNumber = await invoiceModel.generateInvNumber("ds", branchModel.code, MainWindow.branchID.Value);
                        invoiceModel.branchCreatorId = MainWindow.branchID.Value;
                        invoiceModel.posId = MainWindow.posID.Value;
                        invoiceModel.createUserId = MainWindow.userID.Value;
                        invoiceModel.invType = "d"; // destroy                      
                        invoiceModel.paid = 0;
                        invoiceModel.deserved = invoiceModel.totalNet;
                        invoiceModel.notes = tb_notes.Text;
                        if (cb_user.SelectedIndex != -1 && cb_user.SelectedIndex != 0)
                            invoiceModel.userId = (int)cb_user.SelectedValue;
                        #endregion
                        List<ItemTransfer> orderList = new List<ItemTransfer>();
                        #region notification Object
                        Notification not = new Notification()
                        {
                            title = "trExceedMinLimitAlertTilte",
                            ncontent = "trExceedMinLimitAlertContent",
                            msgType = "alert",
                            createDate = DateTime.Now,
                            updateDate = DateTime.Now,
                            createUserId = MainWindow.userID.Value,
                            updateUserId = MainWindow.userID.Value,
                        };
                        #endregion
                        if (invItemLoc.id != 0)
                        {
                            price = (decimal)invItemLoc.avgPurchasePrice;
                            total = price * int.Parse(tb_amount.Text);
                            invoiceModel.total = total;
                            invoiceModel.totalNet = total;
                            //int amount = await itemLocationModel.getAmountByItemLocId((int)invItemLoc.itemLocationId);
                            //if (amount >= invItemLoc.amountDestroyed)
                            //{
                            orderList.Add(new ItemTransfer()
                            {
                                itemName = invItemLoc.itemName,
                                itemId = invItemLoc.itemId,
                                unitName = invItemLoc.unitName,
                                itemUnitId = invItemLoc.itemUnitId,
                                quantity = invItemLoc.amountDestroyed,
                                itemSerial = serialNum,
                                price = price,
                                invoiceId = 0,
                                inventoryItemLocId = invItemLoc.id,
                                createUserId = MainWindow.userID,
                            });
                            invoiceId = await invoiceModel.saveInvoice(invoiceModel);
                            if (invoiceId != 0)
                            {
                                invoiceModel.invoiceId = invoiceId;
                                await invoiceModel.saveInvoiceItems(orderList, invoiceId);
                                await invItemLoc.distroyItem(invItemLoc);
                                if (cb_user.SelectedIndex != -1 && cb_user.SelectedIndex != 0)
                                    await recordCash(invoiceModel);

                                await itemLocationModel.decreaseItemLocationQuantity((int)invItemLoc.itemLocationId, (int)invItemLoc.amountDestroyed, MainWindow.userID.Value, "storageAlerts_minMaxItem", not);
                                await refreshDestroyDetails();
                                Btn_clear_Click(null, null);
                                Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);
                            }
                            else
                                Toaster.ShowError(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);

                            //}
                            //else
                            //{
                            //    Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trDestroyAmountMoreExist"), animation: ToasterAnimation.FadeIn);
                            //}
                        }
                        else
                        {
                            var avgPrice = items.Where(x => x.itemId == (int)cb_item.SelectedValue).Select(x => x.avgPurchasePrice).Single();
                            if (avgPrice != null)
                                price = (decimal)avgPrice;
                            total = price * int.Parse(tb_amount.Text);
                            invoiceModel.total = total;
                            invoiceModel.totalNet = total;
                            orderList.Add(new ItemTransfer()
                            {
                                itemName = cb_item.SelectedItem.ToString(),
                                itemId = (int)cb_item.SelectedValue,
                                unitName = cb_unit.SelectedItem.ToString(),
                                itemUnitId = (int)cb_unit.SelectedValue,
                                quantity = long.Parse(tb_amount.Text),
                                itemSerial = serialNum,
                                price = price,
                                cause = tb_reasonOfDestroy.Text,
                                invoiceId = 0,
                                createUserId = MainWindow.userID,
                            });
                            // اتلاف عنصر يدوياً بدون جرد
                            Window.GetWindow(this).Opacity = 0.2;
                            wd_transItemsLocation w;
                            w = new wd_transItemsLocation();
                            w.orderList = orderList;
                            if (w.ShowDialog() == true)
                            {
                                if (w.selectedItemsLocations != null)
                                {
                                    List<ItemLocation> itemsLocations = w.selectedItemsLocations;
                                    List<ItemLocation> readyItemsLoc = new List<ItemLocation>();

                                    // _ProcessType ="ex";
                                    for (int i = 0; i < itemsLocations.Count; i++)
                                    {
                                        if (itemsLocations[i].isSelected == true)
                                            readyItemsLoc.Add(itemsLocations[i]);
                                    }

                                    invoiceId = await invoiceModel.saveInvoice(invoiceModel);
                                    if (invoiceId != 0)
                                    {
                                        await invoiceModel.saveInvoiceItems(orderList, invoiceId);
                                        for (int i = 0; i < readyItemsLoc.Count; i++)
                                        {
                                            int itemLocId = readyItemsLoc[i].itemsLocId;
                                            int quantity = (int)readyItemsLoc[i].quantity;
                                            await itemLocationModel.decreaseItemLocationQuantity(itemLocId, quantity, MainWindow.userID.Value, "storageAlerts_minMaxItem", not);
                                        }
                                        Btn_clear_Click(null, null);
                                        Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);
                                    }
                                    else
                                        Toaster.ShowError(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                                }
                            }
                            Window.GetWindow(this).Opacity = 1;
                        }
                    }
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                if (sender != null)
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
            */
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
        private async void Tgl_isActive_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                if (invItemLocs is null)
                    await RefreshInventoryItemLocationsList();
                tgl_invItemLocState = 1;
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
                if (invItemLocs is null)
                    await RefreshInventoryItemLocationsList();
                tgl_invItemLocState = 0;
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
        private void Dg_invItemLoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                //selection
                if (dg_invItemLoc.SelectedIndex != -1)
                {
                    invItemLoc = dg_invItemLoc.SelectedItem as InventoryItemLocation;
                    this.DataContext = invItemLoc;
                    if (invItemLoc != null)
                    {

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
        {
            try
            {//refresh

                HelpClass.StartAwait(grid_main);
                await RefreshInventoryItemLocationsList();
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
            if (invItemLocs is null)
                await RefreshInventoryItemLocationsList();
            //searchText = tb_search.Text.ToLower();
            //invItemLocsQuery = invItemLocs.Where(s => (s.name.ToLower().Contains(searchText) ||
            //s.accNumber.ToLower().Contains(searchText)) && s.isActive == tgl_invItemLocState);
            RefreshInventoryItemLocationsView();
        }
        async Task<IEnumerable<InventoryItemLocation>> RefreshInventoryItemLocationsList()
        {
            //invItemLocs = await invItemLoc.Get();
            return invItemLocs;
        }
        void RefreshInventoryItemLocationsView()
        {
            dg_invItemLoc.ItemsSource = invItemLocsQuery;
            txt_count.Text = invItemLocsQuery.Count().ToString();
        }
        #endregion
        #region validate - clearValidate - textChange - lostFocus - . . . . 
        void Clear()
        {
            this.DataContext = new InventoryItemLocation();


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
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") || SectionData.isAdminPermision())
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
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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

                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") || SectionData.isAdminPermision())
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
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

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

                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") || SectionData.isAdminPermision())
                {
                    #region
                    Window.GetWindow(this).Opacity = 0.2;
                    win_lvcCatalog win = new win_lvcCatalog(itemsQuery, 3);
                    win.ShowDialog();
                    Window.GetWindow(this).Opacity = 1;
                    #endregion
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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

                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") || SectionData.isAdminPermision())
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
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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

                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") || SectionData.isAdminPermision())
                {
                    Thread t1 = new Thread(() =>
                    {
                        ExcelPackage();

                    });
                    t1.Start();
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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
        private void Tgl_manually_Checked(object sender, RoutedEventArgs e)
        {
            /*
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_main);

                tglManuallyChecking();
                if (sender != null)
                    SectionData.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_main);
                SectionData.ExceptionMessage(ex, this);
            }
            */
        }

        private void Tgl_manually_Unchecked(object sender, RoutedEventArgs e)
        {
            /*
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_main);

                tglManuallyChecking();
                if (sender != null)
                    SectionData.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_main);
                SectionData.ExceptionMessage(ex, this);
            }
            */
        }

        private void Tb_serialNum_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);

                if (e.Key == Key.Return && !tb_amount.Text.Equals(""))
                {
                    string s = tb_serialNum.Text;
                    int itemCount = int.Parse(tb_amount.Text);

                    if (!s.Equals(""))
                    {
                        int found = lst_serials.Items.IndexOf(s);

                        if (_serialCount == itemCount)
                        {
                            Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trWarningItemCountIs:") + " " + itemCount, animation: ToasterAnimation.FadeIn);
                        }
                        else if (found == -1)
                        {
                            lst_serials.Items.Add(tb_serialNum.Text);
                            _serialCount++;
                        }
                        else
                            Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trWarningSerialExists"), animation: ToasterAnimation.FadeIn);
                    }
                    tb_serialNum.Clear();
                }
                if (sender != null)
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
            */
        }
    }
}
