﻿using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using netoaster;
using Restaurant.Classes;
using Restaurant.View.windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Windows.Threading;

namespace Restaurant.View.kitchen
{
    /// <summary>
    /// Interaction logic for uc_consumptionRawMaterials.xaml
    /// </summary>
    public partial class uc_consumptionRawMaterials : UserControl
    {
        string createPermission = "consumptionRawMaterials_create";
        string reportsPermission = "consumptionRawMaterials_reports";
        private static uc_consumptionRawMaterials _instance;
        public static uc_consumptionRawMaterials Instance
        {
            get
            {
                //if (_instance == null)
                _instance = new uc_consumptionRawMaterials();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_consumptionRawMaterials()
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
        ObservableCollection<BillDetailsPurchase> billDetails = new ObservableCollection<BillDetailsPurchase>();
        public static bool archived = false;
        public static bool isFromReport = false;
        List<ItemUnit> barcodesList;
        List<ItemUnit> itemUnits;
        public Invoice invoice = new Invoice();
        Invoice generatedInvoice = new Invoice();
        List<Invoice> invoices;
        List<ItemTransfer> invoiceItems;
        List<ItemTransfer> mainInvoiceItems;
        public List<Control> controls;
        static public string _ProcessType = "imd"; //draft import

        static private int _SequenceNum = 0;
        static private int _Count = 0;
        static private int _invoiceId;
        // for barcode
        DateTime _lastKeystroke = new DateTime(0);
        static private string _BarcodeStr = "";
        static private string _SelectedProcess = "";
        static private int _SelectedBranch = -1;
        bool _IsFocused = false;
        private static DispatcherTimer timer;
        Item item = new Item();
        IEnumerable<Item> items;
        public static List<string> requiredControlList;
        private void translate()
        {
            ////////////////////////////////----Order----/////////////////////////////////
            dg_billDetails.Columns[1].Header = MainWindow.resourcemanager.GetString("trNum");
            dg_billDetails.Columns[2].Header = MainWindow.resourcemanager.GetString("trItem");
            dg_billDetails.Columns[3].Header = MainWindow.resourcemanager.GetString("trUnit");
            dg_billDetails.Columns[4].Header = MainWindow.resourcemanager.GetString("trQuantity");

            tt_error_previous.Content = MainWindow.resourcemanager.GetString("trPrevious");
            tt_error_next.Content = MainWindow.resourcemanager.GetString("trNext");


        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);

                MainWindow.mainWindow.KeyDown -= HandleKeyPress;
                timer.Stop();
                if (billDetails.Count > 0 && (_ProcessType == "imd" || _ProcessType == "exd"))
                {
                    #region Accept
                    MainWindow.mainWindow.Opacity = 0.2;
                    wd_acceptCancelPopup w = new wd_acceptCancelPopup();
                    w.contentText = "Do you want save sale invoice in drafts?";
                    w.ShowDialog();
                    MainWindow.mainWindow.Opacity = 1;
                    #endregion
                    if (w.isOk)
                        Btn_newDraft_Click(null, null);
                    else
                        clearProcess();
                }
                else
                    clearProcess();
                Instance = null;
                GC.Collect();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
            }
        }
        public async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);
                requiredControlList = new List<string> { "branch", };

                MainWindow.mainWindow.KeyDown += HandleKeyPress;

                if (MainWindow.lang.Equals("en"))
                {
                    MainWindow.resourcemanager = new ResourceManager("Restaurant.en_file", assembly: Assembly.GetExecutingAssembly());
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    MainWindow.resourcemanager = new ResourceManager("Restaurant.ar_file", Assembly.GetExecutingAssembly());
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                }

                translate();
                setNotifications();
                setTimer();
                await RefrishItems();
                await fillBarcodeList();
                //List all the UIElement in the VisualTree
                controls = new List<Control>();
                FindControl(this.grid_main, controls);

                HelpClass.EndAwait(grid_main);
                tb_barcode.Focus();
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        public void FindControl(DependencyObject root, List<Control> controls)
        {
            controls.Clear();
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var control = current as Control;
                if (control != null && control.IsTabStop)
                {
                    controls.Add(control);
                }
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(current); i++)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    if (child != null)
                    {
                        queue.Enqueue(child);
                    }
                }
            }
        }
        #region timer to refresh notifications
        private void setTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(30);
            timer.Tick += timer_Tick;
            timer.Start();
        }
        void timer_Tick(object sendert, EventArgs et)
        {
            try
            {
                setNotifications();
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion
        #region notifications
        private void setNotifications()
        {
            try
            {
                refreshDraftNotification();
            }
            catch (Exception ex)
            {
                //HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void refreshDraftNotification()
        {
            try
            {
                string invoiceType = "imd ,exd";
                int duration = 2;
                int draftCount = await invoice.GetCountByCreator(invoiceType, MainWindow.userLogin.userId, duration);
                if ((invoice.invType == "imd" || invoice.invType == "exd"))
                    draftCount--;

                int previouseCount = 0;
                if (md_draftsCount.Badge != null && md_draftsCount.Badge.ToString() != "") previouseCount = int.Parse(md_draftsCount.Badge.ToString());

                if (draftCount != previouseCount)
                {
                    if (draftCount > 9)
                    {
                        draftCount = 9;
                        md_draftsCount.Badge = "+" + draftCount.ToString();
                    }
                    else if (draftCount == 0) md_draftsCount.Badge = "";
                    else
                        md_draftsCount.Badge = draftCount.ToString();
                }
            }
            catch { }
        }

        #endregion
        #region navigation buttons
        private void navigateBtnActivate()
        {
            int index = invoices.IndexOf(invoices.Where(x => x.invoiceId == _invoiceId).FirstOrDefault());
            if (index == invoices.Count - 1)
                btn_next.IsEnabled = false;
            else
                btn_next.IsEnabled = true;

            if (index == 0)
                btn_previous.IsEnabled = false;
            else
                btn_previous.IsEnabled = true;
        }
        private async void Btn_next_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);

                int index = invoices.IndexOf(invoices.Where(x => x.invoiceId == _invoiceId).FirstOrDefault());
                index++;
                clearProcess();
                invoice = invoices[index];
                _ProcessType = invoice.invType;
                _invoiceId = invoice.invoiceId;
                navigateBtnActivate();
                await fillOrderInputs(invoice);

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Btn_previous_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);

                int index = invoices.IndexOf(invoices.Where(x => x.invoiceId == _invoiceId).FirstOrDefault());
                index--;
                clearProcess();
                invoice = invoices[index];
                _ProcessType = invoice.invType;
                _invoiceId = invoice.invoiceId;
                navigateBtnActivate();
                await fillOrderInputs(invoice);


                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion
        #region barcode
        // read item from barcode
        private async void HandleKeyPress(object sender, KeyEventArgs e)
        {
            try
            {
                if (!_IsFocused)
                {
                    Control c = CheckActiveControl();
                    if (c == null)
                        tb_barcode.Focus();
                    _IsFocused = true;
                }

                HelpClass.StartAwait(grid_main);
                if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
                {
                    switch (e.Key)
                    {
                        case Key.P:
                            //handle P key
                            Btn_printInvoice_Click(null, null);
                            break;
                        case Key.S:
                            //handle S key
                            Btn_save_Click(btn_save, null);
                            break;
                        case Key.I:
                            //handle S key
                            Btn_items_Click(null, null);
                            break;
                    }
                }
                TimeSpan elapsed = (DateTime.Now - _lastKeystroke);
                if (elapsed.TotalMilliseconds > 50)
                {
                    _BarcodeStr = "";
                }

                string digit = "";
                // record keystroke & timestamp 
                if (e.Key >= Key.D0 && e.Key <= Key.D9)
                {
                    //digit pressed!         
                    digit = e.Key.ToString().Substring(1);
                    // = "1" when D1 is pressed
                }
                else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                {
                    digit = e.Key.ToString().Substring(6); // = "1" when NumPad1 is pressed
                }
                else if (e.Key >= Key.A && e.Key <= Key.Z)
                    digit = e.Key.ToString();
                else if (e.Key == Key.OemMinus)
                {
                    digit = "-";
                }
                _BarcodeStr += digit;
                _lastKeystroke = DateTime.Now;
                // process barcode

                if (e.Key.ToString() == "Return" && _BarcodeStr != "")
                {
                    await dealWithBarcode(_BarcodeStr);
                    e.Handled = true;
                }
                _BarcodeStr = "";

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        public Control CheckActiveControl()
        {
            for (int i = 0; i < controls.Count; i++)
            {
                Control c = controls[i];
                if (c.IsFocused)
                {
                    return c;
                }
            }
            return null;
        }
        private async Task dealWithBarcode(string barcode)
        {
            tb_barcode.Text = barcode;
            // get item matches barcode
            if (barcodesList != null)
            {
                ItemUnit unit1 = barcodesList.ToList().Find(c => c.barcode == tb_barcode.Text.Trim());

                // get item matches the barcode
                if (unit1 != null)
                {
                    int itemId = (int)unit1.itemId;
                    if (unit1.itemId != 0)
                    {
                        int index = billDetails.IndexOf(billDetails.Where(p => p.itemUnitId == unit1.itemUnitId).FirstOrDefault());

                        if (index == -1)//item doesn't exist in bill
                        {
                            // get item units
                            itemUnits = await FillCombo.itemUnit.GetItemUnits(itemId);
                            //get item from list
                            item = items.ToList().Find(i => i.itemId == itemId);

                            int count = 1;
                            _Count++;

                            addRowToBill(item.name, item.itemId, unit1.mainUnit, unit1.itemUnitId, count);
                        }
                        else // item exist prevoiusly in list
                        {
                            billDetails[index].Count++;
                            _Count++;
                        }
                        refrishBillDetails();
                    }
                }
                else
                {
                    Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trErrorItemNotFoundToolTip"), animation: ToasterAnimation.FadeIn);
                }
            }
            tb_barcode.Clear();
        }
        private async void Tb_barcode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);

                if (e.Key == Key.Return)
                {
                    string barcode = "";
                    if (_BarcodeStr.Length < 13)
                    {
                        barcode = tb_barcode.Text;
                        await dealWithBarcode(barcode);
                    }
                }

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        async Task fillBarcodeList()
        {
            barcodesList = await FillCombo.itemUnit.Getall();
        }
        #endregion
        #region save
        private async void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            /*
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                if (
                            (
                                (_ProcessType == "im" || _ProcessType == "imd")
                            &&
                                 (MainWindow.groupObject.HasPermissionAction(importPermission, MainWindow.groupObjects, "one"))
                            )
                        ||
                            (
                                (_ProcessType == "ex" || _ProcessType == "exd" || _ProcessType == "exw")
                            &&
                                (MainWindow.groupObject.HasPermissionAction(exportPermission, MainWindow.groupObjects, "one"))
                            )
                   )
                {
                    bool valid = await validateOrder();
                    if (valid)
                    {
                        wd_transItemsLocation w;
                        switch (_ProcessType)
                        {
                            case "exw":
                            case "exd":
                                Window.GetWindow(this).Opacity = 0.2;
                                w = new wd_transItemsLocation();
                                List<ItemTransfer> orderList = new List<ItemTransfer>();
                                List<int> ordersIds = new List<int>();
                                foreach (BillDetailsPurchase d in billDetails)
                                {
                                    if (d.Count == 0)
                                    {
                                        Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trErrorQuantIsZeroToolTip"), animation: ToasterAnimation.FadeIn);
                                        
                                            HelpClass.EndAwait(grid_main);
                                        return;
                                    }
                                    else
                                    {
                                        orderList.Add(new ItemTransfer()
                                        {
                                            itemName = d.Product,
                                            itemId = d.itemId,
                                            unitName = d.Unit,
                                            itemUnitId = d.itemUnitId,
                                            quantity = d.Count,
                                            invoiceId = d.OrderId,
                                        });
                                        ordersIds.Add(d.OrderId);
                                    }
                                }
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
                                        #region notification Object
                                        Notification not = new Notification()
                                        {
                                            title = "trExceedMaxLimitAlertTilte",
                                            ncontent = "trExceedMaxLimitAlertContent",
                                            msgType = "alert",
                                            createDate = DateTime.Now,
                                            updateDate = DateTime.Now,
                                            createUserId = MainWindow.userLogin.userId,
                                            updateUserId = MainWindow.userLogin.userId,
                                        };
                                        #endregion
                                        await FillCombo.itemLocation.recieptOrder(readyItemsLoc, orderList, (int)cb_branch.SelectedValue, MainWindow.userLogin.userId, "storageAlerts_minMaxItem", not);
                                        await save();
                                    }
                                }
                                Window.GetWindow(this).Opacity = 1;
                                break;
                            case "emw":
                                //process transfer items
                                await save();
                                break;
                            default:
                                await save();
                                break;
                        }
                        setNotifications();

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
            */
        }

        #endregion
        #region events


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            try
            {

                Regex regex = new Regex("[^0-9]+");
                e.Handled = regex.IsMatch(e.Text);
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
        private void space_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                TextBox textBox = sender as TextBox;
                HelpClass.InputJustNumber(ref textBox);
                e.Handled = e.Key == Key.Space;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
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
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion
        #region report
        ReportCls reportclass = new ReportCls();
        LocalReport rep = new LocalReport();
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        private void Btn_preview_Click(object sender, RoutedEventArgs e)
        {//preview
            try
            {

                HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(reportsPermission, MainWindow.groupObjects, "one"))
                {
                    #region
                    if (invoiceItems != null)
                    {
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
                    }
                    //////////////////////////////////////
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
        private void BuildReport()
        {
            List<ReportParameter> paramarr = new List<ReportParameter>();

            string addpath;
            bool isArabic = ReportCls.checkLang();
            if (isArabic)
            {//ItemsExport
                addpath = @"\Reports\Store\Ar\ArItemsExportReport.rdlc";
            }
            else
                addpath = @"\Reports\Store\En\ItemsExportReport.rdlc";
            string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);

            ReportCls.checkLang();

            clsReports.ItemsExportReport(invoiceItems, rep, reppath, paramarr);
            clsReports.setReportLanguage(paramarr);
            clsReports.Header(paramarr);

            rep.SetParameters(paramarr);

            rep.Refresh();
        }
        private void Btn_printInvoice_Click(object sender, RoutedEventArgs e)
        {//print
            try
            {

                HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(reportsPermission, MainWindow.groupObjects, "one"))
                {
                    /////////////////////////////////////
                    ///  
                    if (invoiceItems != null)
                    {
                        Thread t1 = new Thread(() =>
                        {
                            printExport();
                        });
                        t1.Start();
                    }
                    //////////////////////////////////////
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
        private void printExport()
        {
            BuildReport();

            this.Dispatcher.Invoke(() =>
            {
                LocalReportExtensions.PrintToPrinterbyNameAndCopy(rep, FillCombo.rep_printer_name, short.Parse(FillCombo.rep_print_count));
            });
        }
        private void Btn_pdf_Click(object sender, RoutedEventArgs e)
        {//pdf
            try
            {

                HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(reportsPermission, MainWindow.groupObjects, "one"))
                {
                    if (invoiceItems != null)
                    {
                        Thread t1 = new Thread(() =>
                        {
                            pdfExport();
                        });
                        t1.Start();
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
        private void pdfExport()
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
        #endregion
        #region btn
        private async void Btn_orders_Click(object sender, RoutedEventArgs e)
        {
            /*
                        try
                        {

                                HelpClass.StartAwait(grid_main);


                            Window.GetWindow(this).Opacity = 0.2;
                            wd_invoice w = new wd_invoice();

                            if ((
                                   MainWindow.groupObject.HasPermissionAction(importPermission, MainWindow.groupObjects, "one")

                                   ) &&
                                 (
                                 MainWindow.groupObject.HasPermissionAction(exportPermission, MainWindow.groupObjects, "one")

                                 ))
                                w.invoiceType = "im ,ex";
                            else if (MainWindow.groupObject.HasPermissionAction(importPermission, MainWindow.groupObjects, "one") )
                                w.invoiceType = "im";
                            else if (MainWindow.groupObject.HasPermissionAction(exportPermission, MainWindow.groupObjects, "one") )
                                w.invoiceType = "ex";



                            w.condition = "order";
                            w.title = MainWindow.resourcemanager.GetString("trOrders");
                            w.branchId = MainWindow.branchLogin.branchId;

                            if (w.ShowDialog() == true)
                            {
                                if (w.invoice != null)
                                {
                                    invoice = w.invoice;
                                    _ProcessType = invoice.invType;
                                    _invoiceId = invoice.invoiceId;
                                    isFromReport = false;
                                    archived = false;
                                    setNotifications();
                                    await fillOrderInputs(invoice);
                                    if (_ProcessType == "im")// set title to bill
                                    {
                                        //  mainInvoiceItems = invoiceItems;

                                    }
                                    else if (_ProcessType == "ex")
                                    {
                                        //   mainInvoiceItems = await invoiceModel.GetInvoicesItems(invoice.invoiceMainId.Value);

                                    }
                                    invoices = await invoice.getBranchInvoices(w.invoiceType, 0, MainWindow.branchLogin.branchId);
                                    navigateBtnActivate();
                                }
                            }
                            Window.GetWindow(this).Opacity = 1;

                                HelpClass.EndAwait(grid_main);
                        }
                        catch (Exception ex)
                        {

                                HelpClass.EndAwait(grid_main);
                            HelpClass.ExceptionMessage(ex, this);
                        }
            */
        }
        private async void Btn_ordersWait_Click(object sender, RoutedEventArgs e)
        {
            /*
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(exportPermission, MainWindow.groupObjects, "one") )
                {
                    Window.GetWindow(this).Opacity = 0.2;
                    wd_invoice w = new wd_invoice();

                    w.invoiceType = "exw";
                    w.title = MainWindow.resourcemanager.GetString("trOrders");
                    w.branchId = MainWindow.branchLogin.branchId;

                    if (w.ShowDialog() == true)
                    {
                        if (w.invoice != null)
                        {
                            invoice = w.invoice;
                            _ProcessType = invoice.invType;
                            _invoiceId = invoice.invoiceId;
                            setNotifications();
                            await fillOrderInputs(invoice);
                            invoices = await invoice.getBranchInvoices(w.invoiceType, 0, MainWindow.branchLogin.branchId);
                            navigateBtnActivate();
                        }
                    }
                    Window.GetWindow(this).Opacity = 1;
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
            */
        }
        private async void Btn_items_Click(object sender, RoutedEventArgs e)
        {
            /*
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                //items

                Window.GetWindow(this).Opacity = 0.2;
                wd_items w = new wd_items();
                w.CardType = "movement";
                w.ShowDialog();
                if (w.isActive)
                {
                    // ChangeItemIdEvent(w.selectedItem);
                    for (int i = 0; i < w.selectedItems.Count; i++)
                    {
                        int itemId = w.selectedItems[i];
                        await ChangeItemIdEvent(itemId);
                    }
                    refrishBillDetails();
                }

                Window.GetWindow(this).Opacity = 1;
                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
            */
        }
        private async void Btn_newDraft_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);

                if (billDetails.Count != 0)
                {
                    //await saveDraft();
                    setNotifications();
                }


                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Btn_draft_Click(object sender, RoutedEventArgs e)
        {
            /*
                        try
                        {

                                HelpClass.StartAwait(grid_main);

                            Window.GetWindow(this).Opacity = 0.2;
                            wd_invoice w = new wd_invoice();
                            string invoiceType = "imd ,exd";
                            int duration = 2;
                            w.invoiceType = invoiceType;
                            w.userId = MainWindow.userLogin.userId;
                            w.duration = duration; // view drafts which updated during 2 last days 
                            w.title = MainWindow.resourcemanager.GetString("trDrafts");
                            // w.branchId = MainWindow.branchLogin.branchId;

                            if (w.ShowDialog() == true)
                            {
                                if (w.invoice != null)
                                {
                                    invoice = w.invoice;
                                    _ProcessType = invoice.invType;
                                    _invoiceId = invoice.invoiceId;
                                    isFromReport = false;
                                    archived = false;
                                    setNotifications();
                                    await fillOrderInputs(invoice);
                                    if (_ProcessType == "imd")// set title to bill
                                    {
                                        //  mainInvoiceItems = invoiceItems;

                                    }
                                    else if (_ProcessType == "exd")
                                    {
                                        //   mainInvoiceItems = await invoiceModel.GetInvoicesItems(invoice.invoiceMainId.Value);

                                    }
                                    invoices = await invoice.GetInvoicesByCreator(invoiceType, MainWindow.userLogin.userId, duration);
                                    navigateBtnActivate();
                                }
                            }
                            Window.GetWindow(this).Opacity = 1;

                                HelpClass.EndAwait(grid_main);
                        }
                        catch (Exception ex)
                        {

                                HelpClass.EndAwait(grid_main);
                            HelpClass.ExceptionMessage(ex, this);
                        }
                        */
        }

        #endregion
        #region DataGrid
        void deleteRowFromOrderItems(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);

                for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                    if (vis is DataGridRow)
                    {
                        BillDetailsPurchase row = (BillDetailsPurchase)dg_billDetails.SelectedItems[0];
                        int index = dg_billDetails.SelectedIndex;

                        // remove item from bill
                        billDetails.RemoveAt(index);

                        ObservableCollection<BillDetailsPurchase> data = (ObservableCollection<BillDetailsPurchase>)dg_billDetails.ItemsSource;
                        data.Remove(row);

                        // calculate new total
                        //refreshTotalValue();
                    }
                _SequenceNum = 0;
                for (int i = 0; i < billDetails.Count; i++)
                {
                    _SequenceNum++;
                    billDetails[i].ID = _SequenceNum;
                }
                refrishBillDetails();

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        void deleteRowFromInvoiceItems(object sender, RoutedEventArgs e)
        {
            try
            {

                for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                    if (vis is DataGridRow)
                    {
                        BillDetailsPurchase row = (BillDetailsPurchase)dg_billDetails.SelectedItems[0];
                        int index = dg_billDetails.SelectedIndex;
                        // calculate new sum
                        _Count -= row.Count;

                        // remove item from bill
                        billDetails.RemoveAt(index);

                        ObservableCollection<BillDetailsPurchase> data = (ObservableCollection<BillDetailsPurchase>)dg_billDetails.ItemsSource;
                        data.Remove(row);

                        tb_count.Text = _Count.ToString();
                    }
                for (int i = 0; i < billDetails.Count; i++)
                {
                    _SequenceNum++;
                    billDetails[i].ID = _SequenceNum;
                }

            }
            catch (Exception ex)
            {

                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Dg_billDetails_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {/*
            try
            {
                //
                //    HelpClass.StartAwait(grid_main);

                TextBox t = e.EditingElement as TextBox;  // Assumes columns are all TextBoxes
                var columnName = e.Column.Header.ToString();

                BillDetailsPurchase row = e.Row.Item as BillDetailsPurchase;
                int index = billDetails.IndexOf(billDetails.Where(p => p.itemUnitId == row.itemUnitId && p.OrderId == row.OrderId).FirstOrDefault());

                TimeSpan elapsed = (DateTime.Now - _lastKeystroke);
                if (elapsed.TotalMilliseconds < 100)
                {
                    if (columnName == MainWindow.resourcemanager.GetString("trQuantity"))
                        t.Text = billDetails[index].Count.ToString();
                }
                else
                {
                    int availableAmount = 0;

                    int oldCount = 0;
                    if (!t.Text.Equals(""))
                        oldCount = int.Parse(t.Text);
                    else
                        oldCount = 0;
                    int newCount = 0;
                    //"tb_amont"
                    if (columnName == MainWindow.resourcemanager.GetString("trQuantity"))
                    {
                        if (_ProcessType == "exd")
                        {
                            availableAmount = await getAvailableAmount(row.itemId, row.itemUnitId, MainWindow.branchLogin.branchId, row.ID);
                            if (availableAmount < oldCount)
                            {

                                Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trErrorAmountNotAvailableToolTip"), animation: ToasterAnimation.FadeIn);
                                newCount = newCount + availableAmount;
                                t.Text = availableAmount.ToString();
                            }
                            else
                            {
                                if (!t.Text.Equals(""))
                                    newCount = int.Parse(t.Text);
                                else
                                    newCount = 0;
                                if (newCount < 0)
                                {
                                    newCount = 0;
                                    t.Text = "0";
                                }
                            }
                        }
                        else
                        {
                            if (!t.Text.Equals(""))
                                newCount = int.Parse(t.Text);
                            else
                                newCount = 0;
                        }
                    }
                    else
                        newCount = row.Count;

                    if (row.OrderId != 0)
                    {
                        ItemTransfer item = mainInvoiceItems.ToList().Find(i => i.itemUnitId == row.itemUnitId && i.invoiceId == row.OrderId);
                        if (newCount > item.quantity)
                        {
                            // return old value 
                            t.Text = item.quantity.ToString();

                            newCount = (int)item.quantity;
                            Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trErrorAmountIncreaseToolTip"), animation: ToasterAnimation.FadeIn);
                        }
                    }


                    _Count -= oldCount;
                    _Count += newCount;

                    //  refresh count text box
                    tb_count.Text = _Count.ToString();

                    // update item in billdetails           
                    billDetails[index].Count = (int)newCount;
                    if (invoiceItems != null)
                        invoiceItems[index].quantity = (int)newCount;
                }
                //
                //    HelpClass.EndAwait(grid_main);
                //refrishDataGridItems();

            }
            catch (Exception ex)
            {
                //
                //    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
            */
        }
        private void DataGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);


                //billDetails
                int count = 0;
                foreach (var item in billDetails)
                {
                    if (dg_billDetails.Items.Count != 0)
                    {
                        if (dg_billDetails.Items.Count > 1)
                        {
                            DataGridCell cell = null;
                            try
                            {
                                cell = DataGridHelper.GetCell(dg_billDetails, count, 3);
                            }
                            catch
                            { }
                            if (cell != null)
                            {
                                var cp = (ContentPresenter)cell.Content;
                                var combo = (ComboBox)cp.ContentTemplate.FindName("cbm_unitItemDetails", cp);
                                //var combo = (combo)cell.Content;
                                combo.SelectedValue = (int)item.itemUnitId;
                            }
                        }
                    }
                    count++;
                }


                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Cbm_unitItemDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                var cmb = sender as ComboBox;

                if (dg_billDetails.SelectedIndex != -1 && cmb != null)
                    billDetails[dg_billDetails.SelectedIndex].itemUnitId = (int)cmb.SelectedValue;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Cbm_unitItemDetails_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {

                //billDetails
                if (billDetails.Count == 1)
                {
                    var cmb = sender as ComboBox;
                    cmb.SelectedValue = (int)billDetails[0].itemUnitId;
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Dg_billDetails_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _IsFocused = true;
        }
        #endregion
        #region billdetails
        async Task RefrishItems()
        {
            items = await FillCombo.item.Get();
        }
        private void addRowToBill(string itemName, int itemId, string unitName, int itemUnitId, int count)
        {
            // increase sequence for each read
            _SequenceNum++;

            billDetails.Add(new BillDetailsPurchase()
            {
                ID = _SequenceNum,
                Product = item.name,
                itemId = item.itemId,
                Unit = unitName,
                itemUnitId = itemUnitId,
                Count = count,
            });

        }
        bool firstTimeForDatagrid = true;
        async void refrishBillDetails()
        {
            dg_billDetails.ItemsSource = null;
            dg_billDetails.ItemsSource = billDetails;
            if (firstTimeForDatagrid)
            {
                HelpClass.StartAwait(grid_main);
                await Task.Delay(1000);
                dg_billDetails.Items.Refresh();
                firstTimeForDatagrid = false;
                HelpClass.EndAwait(grid_main);
            }
            DataGrid_CollectionChanged(dg_billDetails, null);
            tb_count.Text = _Count.ToString();

        }
        void refrishDataGridItems()
        {
            dg_billDetails.ItemsSource = null;
            dg_billDetails.ItemsSource = billDetails;
            dg_billDetails.Items.Refresh();
            DataGrid_CollectionChanged(dg_billDetails, null);

        }
        public async Task ChangeItemIdEvent(int itemId)
        {
            if (items != null) item = items.ToList().Find(c => c.itemId == itemId);

            if (item != null)
            {
                this.DataContext = item;

                // get item units
                itemUnits = await FillCombo.itemUnit.GetItemUnits(item.itemId);
                // search for default unit for purchase
                var defaultPurUnit = itemUnits.ToList().Find(c => c.defaultPurchase == 1);
                if (defaultPurUnit != null)
                {
                    // create new row in bill details data grid
                    addRowToBill(item.name, itemId, defaultPurUnit.mainUnit, defaultPurUnit.itemUnitId, 1);

                    //_Count++;
                    //refrishBillDetails();
                }
                else
                {
                    addRowToBill(item.name, itemId, null, 0, 1);
                }
                _Count++;
            }
        }
        private void clearProcess()
        {
            _Count = 0;
            _SequenceNum = 0;
            _SelectedBranch = -1;
            _SelectedProcess = "imd";
            _ProcessType = "imd";
            isFromReport = false;
            archived = false;
            invoice = new Invoice();
            generatedInvoice = new Invoice();
            tb_barcode.Clear();
            billDetails.Clear();

            refrishBillDetails();
            inputEditable();
            btn_next.Visibility = Visibility.Collapsed;
            btn_previous.Visibility = Visibility.Collapsed;

            // last 
            HelpClass.clearValidate(requiredControlList, this);
        }
        public async Task fillOrderInputs(Invoice invoice)
        {
            if (invoice.invoiceMainId == null)
                generatedInvoice = await invoice.getgeneratedInvoice(invoice.invoiceId);
            else
                generatedInvoice = await invoice.GetByInvoiceId((int)invoice.invoiceMainId);
            _Count = invoice.itemsCount;
            tb_count.Text = _Count.ToString();



            // build invoice details grid
            await buildInvoiceDetails();

            inputEditable();
        }
        private async Task buildInvoiceDetails()
        {
            //get invoice items
            invoiceItems = await invoice.GetInvoicesItems(invoice.invoiceId);
            // build invoice details grid
            _SequenceNum = 0;
            billDetails.Clear();
            foreach (ItemTransfer itemT in invoiceItems)
            {
                _SequenceNum++;
                decimal total = (decimal)(itemT.price * itemT.quantity);
                int orderId = 0;
                if (itemT.invoiceId != null)
                    orderId = (int)itemT.invoiceId;
                billDetails.Add(new BillDetailsPurchase()
                {
                    ID = _SequenceNum,
                    Product = itemT.itemName,
                    itemId = (int)itemT.itemId,
                    Unit = itemT.itemUnitId.ToString(),
                    itemUnitId = (int)itemT.itemUnitId,
                    Count = (int)itemT.quantity,
                    Price = (decimal)itemT.price,
                    Total = total,
                    OrderId = orderId,
                });
            }
            tb_barcode.Focus();

            refrishBillDetails();
        }
        private void inputEditable()
        {

            /*
            if (_ProcessType == "imd" || _ProcessType == "exd") // return invoice
            {
                dg_billDetails.Columns[0].Visibility = Visibility.Visible; //make delete hidden
                dg_billDetails.Columns[4].IsReadOnly = false; //make count read only
                tb_barcode.IsEnabled = true;
                btn_save.IsEnabled = true;
            }
            else if (_ProcessType == "im" || _ProcessType == "ex")
            {
                dg_billDetails.Columns[0].Visibility = Visibility.Collapsed; //make delete hidden
                dg_billDetails.Columns[4].IsReadOnly = true; //make count read only
                tb_barcode.IsEnabled = false;
                btn_save.IsEnabled = false;

            }
            else if (_ProcessType == "imw")
            {
                dg_billDetails.Columns[0].Visibility = Visibility.Collapsed; //make delete hidden
                dg_billDetails.Columns[4].IsReadOnly = true; //make count read only
                tb_barcode.IsEnabled = false;
                btn_save.IsEnabled = true;
            }
            else if (_ProcessType == "exw")
            {
                dg_billDetails.Columns[0].Visibility = Visibility.Visible; //make delete hidden
                dg_billDetails.Columns[4].IsReadOnly = false; //make count read only
                tb_barcode.IsEnabled = false;
                btn_save.IsEnabled = true;
            }
            */
            if (!isFromReport)
            {
                btn_next.Visibility = Visibility.Visible;
                btn_previous.Visibility = Visibility.Visible;
            }
        }
        #endregion
    }
}