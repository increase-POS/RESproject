using Microsoft.Reporting.WinForms;
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

namespace Restaurant.View.storage.storageOperations
{
    /// <summary>
    /// Interaction logic for uc_storageInvoice.xaml
    /// </summary>
    public partial class uc_storageInvoice : UserControl
    {
        string reciptPermission = "storageInvoice_recipt";
        string returnPermission = "storageInvoice_return";
        string reportsPermission = "storageInvoice_reports";
        string inputsPermission = "storageInvoice_inputs";
        private static uc_storageInvoice _instance;
        public static uc_storageInvoice Instance
        {
            get
            {
                _instance = new uc_storageInvoice();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_storageInvoice()
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
        public static bool isFromReport = false;
        public static bool archived = false;
        Item item = new Item();
        List<ItemUnit> itemUnits;
        public List<Control> controls;
        public Invoice invoice = new Invoice();
        List<ItemTransfer> invoiceItems;
        List<ItemTransfer> mainInvoiceItems;
                
        int prInvoiceId;
        // for barcode
        DateTime _lastKeystroke = new DateTime(0);
        static private string _BarcodeStr = "";
        static private object _Sender;
        bool _IsFocused = false;
        //for invoice details
        static private int _SequenceNum = 0;
        static private int _invoiceId;
        static public string _InvoiceType = "isd"; // immidiatlly in storage draft
        static private decimal _Sum = 0;
        static private decimal _Count = 0;
        #region notification parameters
        private static DispatcherTimer timer;
        int _InvoiceCount = 0;
        int _DraftCount = 0;
        int _ReturnCount = 0;
        #endregion
        private void translate()
        {
            ////////////////////////////////----invoice----/////////////////////////////////
            dg_billDetails.Columns[1].Header = MainWindow.resourcemanager.GetString("trNum");
            dg_billDetails.Columns[2].Header = MainWindow.resourcemanager.GetString("trItem");
            dg_billDetails.Columns[3].Header = MainWindow.resourcemanager.GetString("trUnit");
            dg_billDetails.Columns[4].Header = MainWindow.resourcemanager.GetString("trQTR");
            dg_billDetails.Columns[5].Header = MainWindow.resourcemanager.GetString("trPrice");
            dg_billDetails.Columns[6].Header = MainWindow.resourcemanager.GetString("trTotal");

            txt_invoices.Text = MainWindow.resourcemanager.GetString("trInvoices");
            txt_returnInvoice.Text = MainWindow.resourcemanager.GetString("trReturnInvoices");
            txt_titleDataGridInvoice.Text = MainWindow.resourcemanager.GetString("trDirectEntry");

            tt_error_previous.Content = MainWindow.resourcemanager.GetString("trPrevious");
            tt_error_next.Content = MainWindow.resourcemanager.GetString("trNext");
        }
        #region loading
        List<keyValueBool> loadingList;
        async void loading_RefrishItems()
        {
            try
            {
                await FillCombo.RefreshPurchaseItems();
            }
            catch (Exception)
            { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_RefrishItems"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        #endregion
        public async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                
                    HelpClass.StartAwait(grid_main);
                MainWindow.mainWindow.KeyDown += HandleKeyPress;

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

                //loading_fillBarcodeList();
                setNotifications();
                setTimer();
                controls = new List<Control>();

                #region loading
                loadingList = new List<keyValueBool>();
                bool isDone = true;
                loadingList.Add(new keyValueBool { key = "loading_RefrishItems", value = false });
                loading_RefrishItems();
                do
                {
                    isDone = true;
                    foreach (var item in loadingList)
                    {
                        if (item.value == false)
                        {
                            isDone = false;
                            break;
                        }
                    }
                    if (!isDone)
                    {
                        await Task.Delay(0500);
                    }
                }
                while (!isDone);
                #endregion
                //if (MainWindow.tax == 0)
                //    sp_tax.Visibility = Visibility.Collapsed;
                //else
                //    sp_tax.Visibility = Visibility.Visible;
                controls = new List<Control>();
                FindControl(this.grid_main, controls);
              
                #region Permision
                if (MainWindow.groupObject.HasPermissionAction(reciptPermission, MainWindow.groupObjects, "one"))
                    md_invoiceCount.Visibility = Visibility.Visible;
                else
                    md_invoiceCount.Visibility = Visibility.Collapsed;

                if (MainWindow.groupObject.HasPermissionAction(returnPermission, MainWindow.groupObjects, "one"))
                    md_returnsCount.Visibility = Visibility.Visible;
                else
                    md_returnsCount.Visibility = Visibility.Collapsed;

                if (MainWindow.groupObject.HasPermissionAction(inputsPermission, MainWindow.groupObjects, "one"))
                {
                    md_draft.Visibility = Visibility.Visible;
                    btn_newDraft.Visibility = Visibility.Visible;
                    btn_items.IsEnabled = true;
                }
                else
                {
                    md_draft.Visibility = Visibility.Collapsed;
                    btn_newDraft.Visibility = Visibility.Collapsed;
                    btn_items.IsEnabled = false;
                }
                #endregion

                 HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                MainWindow.mainWindow.KeyDown -= HandleKeyPress;
                timer.Stop();
                saveBeforeExit();

                Instance = null;
                GC.Collect();
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
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
        #region notifications
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
                if (sendert != null)
                    HelpClass.StartAwait(grid_main);
                refreshInvoiceNotification();
                refreshReturnNotification();
                if (sendert != null)
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sendert != null)
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void setNotifications()
        {
            try
            {

                refreshDraftNotification();
                refreshInvoiceNotification();
                refreshReturnNotification();

            }
            catch { }
        }
        private async void refreshDraftNotification()
        {
            try
            {
                string invoiceType = "isd";
                int duration = 2;
                int draftCount = await invoice.GetCountByCreator(invoiceType, MainWindow.userLogin.userId, duration);
                if ((_InvoiceType == "isd") && invoice.invoiceId != 0 && invoice != null && !isFromReport)
                    draftCount--;

                //int previouseCount = 0;
                //if (md_draft.Badge != null && md_draft.Badge.ToString() != "") previouseCount = int.Parse(md_draft.Badge.ToString());

                if (draftCount != _DraftCount)
                {
                    if (draftCount > 9)
                    {
                        md_draft.Badge = "+9";
                    }
                    else if (draftCount == 0) md_draft.Badge = "";
                    else
                        md_draft.Badge = draftCount.ToString();
                }
                _DraftCount = draftCount;

            }
            catch { }
        }
        private async void refreshInvoiceNotification()
        {
            try
            {
                string invoiceType = "pw";
                if (invoice == null)
                    invoice = new Invoice();
                int invoiceCount = await invoice.GetCountBranchInvoices(invoiceType, 0, MainWindow.branchLogin.branchId);
                if (invoice.invType == "pw" && invoice != null)
                    invoiceCount--;

                //int previouseCount = 0;
                //if (md_invoiceCount.Badge != null && md_invoiceCount.Badge.ToString() != "")
                //{
                //    if (md_invoiceCount.Badge.ToString() == "+9")
                //        previouseCount = 10;
                //    else
                //        previouseCount = int.Parse(md_invoiceCount.Badge.ToString());
                //}
                if (invoiceCount != _InvoiceCount)
                {
                    if (invoiceCount > 9)
                    {
                        md_invoiceCount.Badge = "+9";
                    }
                    else if (invoiceCount == 0) md_invoiceCount.Badge = "";
                    else
                        md_invoiceCount.Badge = invoiceCount.ToString();
                }
                _InvoiceCount = invoiceCount;

            }
            catch { }
        }
        private async void refreshReturnNotification()
        {
            try
            {

                string invoiceType = "pbw";
                if (invoice == null)
                    invoice = new Invoice();
                int returnsCount = await invoice.GetCountBranchInvoices(invoiceType, 0, MainWindow.branchLogin.branchId);
                if (invoice.invType == "pbw" && invoice != null)
                    returnsCount--;
                //int previouseCount = 0;
                //if (md_returnsCount.Badge != null && md_returnsCount.Badge.ToString() != "") previouseCount = int.Parse(md_returnsCount.Badge.ToString());

                if (returnsCount != _ReturnCount)
                {
                    if (returnsCount > 9)
                    {
                        md_returnsCount.Badge = "+9";
                    }
                    else if (returnsCount == 0) md_returnsCount.Badge = "";
                    else
                        md_returnsCount.Badge = returnsCount.ToString();
                }
                _ReturnCount = returnsCount;


            }
            catch { }
        }
        #endregion
        #region navigation buttons
        private void navigateBtnActivate()
        {
            int index = FillCombo.invoices.IndexOf(FillCombo.invoices.Where(x => x.invoiceId == _invoiceId).FirstOrDefault());
            if (index == FillCombo.invoices.Count - 1)
                btn_next.IsEnabled = false;
            else
                btn_next.IsEnabled = true;

            if (index == 0)
                btn_previous.IsEnabled = false;
            else
                btn_previous.IsEnabled = true;
        }
        private void clearNavigation()
        {
            _SequenceNum = 0;
            _Count = 0;
            _Sum = 0;
            invoice = new Invoice();
            txt_branch.Text = "_____________";
            txt_invNumber.Text = "";
            billDetails.Clear();
            tb_count.Text = "0";
            tb_total.Text = "0";
            tb_sum.Text = "0";
            btn_items.IsEnabled = true;
            isFromReport = false;
            refrishBillDetails();
        }
        private async Task navigateInvoice(int index)
        {
            try
            {
                clearNavigation();
                invoice = FillCombo.invoices[index];
                _invoiceId = invoice.invoiceId;
                navigateBtnActivate();
                await fillInvoiceInputs(invoice);
            }
            catch (Exception ex)
            {
            }
        }
        private async void Btn_next_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                int index = FillCombo.invoices.IndexOf(FillCombo.invoices.Where(x => x.invoiceId == _invoiceId).FirstOrDefault());
                index++;
                await navigateInvoice(index);

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

                int index = FillCombo.invoices.IndexOf(FillCombo.invoices.Where(x => x.invoiceId == _invoiceId).FirstOrDefault());
                index--;
                await navigateInvoice(index);

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion

        #region save
        private async Task saveBeforeExit()
        {
            if (billDetails.Count > 0 && _InvoiceType == "isd")
            {
                #region Accept
                MainWindow.mainWindow.Opacity = 0.2;
                wd_acceptCancelPopup w = new wd_acceptCancelPopup();
                //w.contentText = MainWindow.resourcemanager.GetString("trMessageBoxActivate");
                w.contentText = MainWindow.resourcemanager.GetString("trSaveInvoiceNotification");
                w.ShowDialog();
                MainWindow.mainWindow.Opacity = 1;
                #endregion
                if (w.isOk)
                {
                    await addInvoice("isd");
                    clearInvoice();
                    _InvoiceType = "isd";
                }
            }
            clearInvoice();

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
        private async Task addInvoice(string invType)
        {
            #region invoice object
            if (invType == "isd" && invoice.invoiceId == 0)
                invoice.invNumber = await invoice.generateInvNumber("isd", MainWindow.branchLogin.code, MainWindow.branchLogin.branchId);
            else
                invoice.invNumber = await invoice.generateInvNumber("is", MainWindow.branchLogin.code, MainWindow.branchLogin.branchId);
            invoice.branchCreatorId = MainWindow.branchLogin.branchId;
            invoice.branchId = MainWindow.branchLogin.branchId;
            invoice.posId = MainWindow.posLogin.posId;
            invoice.invType = invType;
            invoice.total = _Sum;
            invoice.totalNet = decimal.Parse(tb_total.Text);
            invoice.paid = 0;
            invoice.deserved = invoice.totalNet;
            invoice.createUserId = MainWindow.userLogin.userId;
            invoice.cashReturn = 0;
            #endregion
            #region invoice items
            invoiceItems = new List<ItemTransfer>();
            ItemTransfer itemT;
            for (int i = 0; i < billDetails.Count; i++)
            {
                itemT = new ItemTransfer();
                itemT.invoiceId = 0;
                itemT.quantity = billDetails[i].Count;
                itemT.price = billDetails[i].Price;
                itemT.itemUnitId = billDetails[i].itemUnitId;
                itemT.createUserId = MainWindow.userLogin.userId;
                itemT.invoiceId = billDetails[i].OrderId;

                invoiceItems.Add(itemT);
            }
            #endregion
            // save invoice and items in DB
            int invoiceId = await FillCombo.invoice.saveInvoiceWithItems(invoice, invoiceItems);
            prInvoiceId = invoiceId;
            invoice.invoiceId = invoiceId;
            if (invoiceId != 0)
            {
                Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);
                if (invType == "is")
                    FillCombo.invoice.saveAvgPurchasePrice(invoiceItems);
            }
            else
                Toaster.ShowError(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);


        }
        private async Task returnInvoice(string type)
        {
            invoiceItems = new List<ItemTransfer>();
            ItemTransfer itemT;
            decimal returnValue = 0;
            for (int i = 0; i < billDetails.Count; i++)
            {
                itemT = new ItemTransfer();

                itemT.invoiceId = 0;
                itemT.itemName = billDetails[i].Product;
                itemT.itemId = billDetails[i].itemId;
                itemT.unitName = billDetails[i].Unit;
                itemT.quantity = billDetails[i].Count;
                itemT.price = billDetails[i].Price;
                itemT.itemUnitId = billDetails[i].itemUnitId;
                itemT.createUserId = MainWindow.userLogin.userId;

                returnValue += (decimal)itemT.price * (decimal)itemT.quantity;
                invoiceItems.Add(itemT);
            }

            Window.GetWindow(this).Opacity = 0.2;
            wd_transItemsLocation w;
            w = new wd_transItemsLocation();
            w.orderList = invoiceItems;
            if (w.ShowDialog() == true)
            {
                if (w.selectedItemsLocations != null)
                {
                    List<ItemLocation> itemsLocations = w.selectedItemsLocations;
                    List<ItemLocation> readyItemsLoc = new List<ItemLocation>();

                    for (int i = 0; i < itemsLocations.Count; i++)
                    {
                        if (itemsLocations[i].isSelected == true)
                            readyItemsLoc.Add(itemsLocations[i]);
                    }

                    invoice.invType = type;
                    invoice.updateUserId = MainWindow.userLogin.userId;
                    decimal total = 0;
                    // calculate total and totalnet
                    for (int i = 0; i < billDetails.Count; i++)
                    {
                        total += (decimal)billDetails[i].Price * (decimal)billDetails[i].Count;
                    }
                    invoice.total = total;
                    invoice.taxtype = 2;
                    decimal taxValue = HelpClass.calcPercentage(total, (decimal)invoice.tax);
                    invoice.totalNet = total + taxValue;
                    invoice.paid = 0;
                    invoice.deserved = invoice.totalNet;

                    int invoiceId = await FillCombo.invoice.saveInvoiceWithItems(invoice, invoiceItems);
                    if (invoiceId != 0)
                    {
                        await invoice.recordPosCashTransfer(invoice, "pb");
                        await invoice.recordCashTransfer(invoice, "pb");
                        //await FillCombo.invoice.saveInvoiceItems(invoiceItems, invoiceId);

                        #region notification Object
                        Notification not = new Notification()
                        {
                            title = "trExceedMinLimitAlertTilte",
                            ncontent = "trExceedMinLimitAlertContent",
                            msgType = "alert",
                            createDate = DateTime.Now,
                            updateDate = DateTime.Now,
                            createUserId = MainWindow.userLogin.userId,
                            updateUserId = MainWindow.userLogin.userId,
                        };
                        #endregion
                        for (int i = 0; i < readyItemsLoc.Count; i++)
                        {
                            int itemLocId = readyItemsLoc[i].itemsLocId;
                            int quantity = (int)readyItemsLoc[i].quantity;
                            await FillCombo.itemLocation.decreaseItemLocationQuantity(itemLocId, quantity, MainWindow.userLogin.userId, "storageAlerts_minMaxItem", not);
                        }
                        refreshReturnNotification();
                        clearInvoice();
                        Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);
                    }
                    else
                        Toaster.ShowError(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                }
            }
            Window.GetWindow(this).Opacity = 1;
        }
        private async Task receiptInvoice()
        {
            #region notification Object
            Notification not = new Notification()
            {
                title = "trExceedMaxLimitAlertTilte",
                ncontent = "trExceedMaxLimitAlertContent",
                msgType = "alert",
                createUserId = MainWindow.userLogin.userId,
                updateUserId = MainWindow.userLogin.userId,
            };
            #endregion
            invoice.invType = "p";
            invoice.updateUserId = MainWindow.userLogin.userId;
            await FillCombo.invoice.saveInvoice(invoice);
            await FillCombo.itemLocation.recieptInvoice(invoiceItems, MainWindow.branchLogin.branchId, MainWindow.userLogin.userId, "storageAlerts_minMaxItem", not); // increase item quantity in DB
            refreshInvoiceNotification();
            clearInvoice();
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
        private void input_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = sender.GetType().Name;
                if (name == "TextBox")
                {
                }
                else if (name == "ComboBox")
                {
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void clearInvoice()
        {

            _SequenceNum = 0;
            _Count = 0;
            _Sum = 0;
            // _InvoiceType = "pbw";
            _InvoiceType = "isd";
            invoice = new Invoice();
            txt_branch.Text = "_____________";
            txt_invNumber.Text = "";
            billDetails.Clear();
            tb_count.Text = "0";
            tb_total.Text = "0";
            tb_sum.Text = "0";
            btn_items.IsEnabled = true;
            isFromReport = false;
            refrishBillDetails();
            inputEditable();
            if (!isFromReport)
            {
                btn_next.Visibility = Visibility.Collapsed;
                btn_previous.Visibility = Visibility.Collapsed;
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
        private void Tb_taxValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var txb = sender as TextBox;
                if ((sender as TextBox).Name == "tb_taxValue")
                    HelpClass.InputJustNumber(ref txb);
                _Sender = sender;
                refreshTotalValue();
                e.Handled = true;

            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion
        #region billdetails
        public async Task fillInvoiceInputs(Invoice invoice)
        {
            if (_InvoiceType == "isd")
            {
                tb_sum.Visibility = Visibility.Visible;
                txt_sum.Visibility = Visibility.Visible;
                txt_total.Visibility = Visibility.Visible;
                tb_total.Visibility = Visibility.Visible;
            }
            else
            {
                tb_sum.Visibility = Visibility.Collapsed;
                txt_sum.Visibility = Visibility.Collapsed;
                txt_total.Visibility = Visibility.Collapsed;
                tb_total.Visibility = Visibility.Collapsed;
            }
            txt_branch.Text = invoice.branchName;
            txt_invNumber.Text = invoice.invNumber;
            if (invoice.total != null)
                _Sum = (decimal)invoice.total;
            else
                _Sum = 0;
            // build invoice details grid
            await buildInvoiceDetails(invoice.invoiceId);
            refreshTotalValue();
            inputEditable();
        }
        private async Task buildInvoiceDetails(int invoiceId)
        {
            invoiceItems = await FillCombo.invoice.GetInvoicesItems(invoice.invoiceId);
            // build invoice details grid
            _SequenceNum = 0;
            billDetails.Clear();
            _Count = 0;
            foreach (ItemTransfer itemT in invoiceItems)
            {
                _SequenceNum++;
                _Count += (int)itemT.quantity;
                decimal total = (decimal)(itemT.price * itemT.quantity);
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
                    UnitName = itemT.unitName,
                });
            }

            tb_count.Text = _Count.ToString();
            tb_barcode.Focus();

            refrishBillDetails();
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
        }
        void refrishDataGridItems()
        {
            dg_billDetails.ItemsSource = null;
            dg_billDetails.ItemsSource = billDetails;
            dg_billDetails.Items.Refresh();
            DataGrid_CollectionChanged(dg_billDetails, null);

        }
        private void inputEditable()
        {
            if (_InvoiceType == "pw") // wait purchase invoice
            {
                dg_billDetails.Columns[0].Visibility = Visibility.Collapsed; //make delete column unvisible
                dg_billDetails.Columns[4].IsReadOnly = true; //make count read only

            }
            else if (_InvoiceType == "pbw")
            {
                dg_billDetails.Columns[0].Visibility = Visibility.Collapsed; //make delete column visible
                dg_billDetails.Columns[4].IsReadOnly = true; //make count editable
            }
            if (!isFromReport)
            {
                btn_next.Visibility = Visibility.Visible;
                btn_previous.Visibility = Visibility.Visible;
            }
        }
        public async Task ChangeItemIdEvent(int itemId)
        {
            item = FillCombo.purchaseItems.ToList().Find(c => c.itemId == itemId);
            if (item != null)
            {   
                // get item units
                itemUnits = FillCombo.itemUnitList.Where(a => a.itemId == item.itemId).ToList();
                // search for default unit for purchase
                var defaultPurUnit = itemUnits.ToList().Find(c => c.defaultPurchase == 1);
                if (defaultPurUnit != null)
                {
                    int index = billDetails.IndexOf(billDetails.Where(p => p.itemUnitId == defaultPurUnit.itemUnitId && p.OrderId == 0).FirstOrDefault());
                    if (index == -1)//item doesn't exist in bill
                    {
                        // create new row in bill details data grid
                        addRowToBill(item.name, itemId, defaultPurUnit.unitName, defaultPurUnit.itemUnitId, 1, 0, 0);
                    }
                    else // item exist prevoiusly in list
                    {
                        billDetails[index].Count++;
                        billDetails[index].Total = billDetails[index].Count * billDetails[index].Price;

                        _Sum += billDetails[index].Price;
                    }
                }
                else
                {
                    addRowToBill(item.name, itemId, null, 0, 1, 0, 0);
                }

            }
        }
        private void addRowToBill(string itemName, int itemId, string unitName, int itemUnitId, int count, decimal price, decimal total)
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
                Count = 1,
                Price = price,
                Total = total,
            });
            _Sum += total;
        }
        private void refreshTotalValue()
        {
            decimal total = _Sum;
            decimal taxValue = 0;
            //decimal taxInputVal = 0;
            //if (!tb_taxValue.Text.Equals(""))
            //    taxInputVal = decimal.Parse(tb_taxValue.Text);
            //if (total != 0)
            //    taxValue = HelpClass.calcPercentage(total, taxInputVal);

            if (_Sum != 0)
                tb_sum.Text = HelpClass.DecTostring(_Sum);
            else
                tb_sum.Text = "0";
            total = total + taxValue;

            if (total != 0)
                tb_total.Text = HelpClass.DecTostring(total);
            else tb_total.Text = "0";
            tb_count.Text = _SequenceNum.ToString();

        }
        #endregion
        #region data grid pay invoice
        private void Cbm_unitItemDetails_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                //billDetails
                var cmb = sender as ComboBox;
                cmb.SelectedValue = (int)billDetails[0].itemUnitId;

                if (billDetails[0].OrderId != 0)
                    cmb.IsEnabled = false;
                else
                    cmb.IsEnabled = true;

            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Cbm_unitItemDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var cmb = sender as ComboBox;

                if (dg_billDetails.SelectedIndex != -1 && cmb.SelectedValue != null)
                {
                    billDetails[dg_billDetails.SelectedIndex].itemUnitId = (int)cmb.SelectedValue;
                    if (billDetails[dg_billDetails.SelectedIndex].OrderId != 0)
                        cmb.IsEnabled = false;
                    else
                        cmb.IsEnabled = true;
                }

            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
                //return;
            }
        }
        private void DataGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
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

                                if (item.OrderId != 0)
                                    combo.IsEnabled = false;
                                else
                                    combo.IsEnabled = true;
                            }
                        }
                    }
                    count++;
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Dg_billDetails_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            int column = dg_billDetails.CurrentCell.Column.DisplayIndex;
            if (dg_billDetails.SelectedIndex != -1 && column == 3)
                if (billDetails[dg_billDetails.SelectedIndex].OrderId != 0)
                    e.Cancel = true;
        }
        private void Dg_billDetails_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                TextBox t = e.EditingElement as TextBox;  // Assumes columns are all TextBoxes
                var columnName = e.Column.Header.ToString();

                BillDetailsPurchase row = e.Row.Item as BillDetailsPurchase;
                int index = billDetails.IndexOf(billDetails.Where(p => p.itemUnitId == row.itemUnitId && p.OrderId == row.OrderId).FirstOrDefault());

                TimeSpan elapsed = (DateTime.Now - _lastKeystroke);
                if (elapsed.TotalMilliseconds < 100)
                {
                    if (columnName == MainWindow.resourcemanager.GetString("trQTR"))
                        t.Text = billDetails[index].Count.ToString();
                    else if (columnName == MainWindow.resourcemanager.GetString("trPrice"))
                        t.Text = HelpClass.DecTostring(billDetails[index].Price);

                }
                else
                {
                    int oldCount = 0;
                    long newCount = 0;
                    decimal newPrice = 0;
                    decimal oldPrice = 0;
                    //"tb_amont"
                    if (columnName == MainWindow.resourcemanager.GetString("trQTR"))
                        newCount = int.Parse(t.Text);
                    else
                        newCount = row.Count;
                    if (newCount < 0)
                    {
                        newCount = 0;
                        t.Text = "0";
                    }
                    oldCount = row.Count;

                    if (_InvoiceType == "pbw")
                    {
                        ItemTransfer item = mainInvoiceItems.ToList().Find(i => i.itemUnitId == row.itemUnitId);
                        if (newCount > item.quantity)
                        {
                            // return old value 
                            t.Text = item.quantity.ToString();

                            newCount = (long)item.quantity;
                            Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trErrorAmountIncreaseToolTip"), animation: ToasterAnimation.FadeIn);
                        }
                    }
                    _Count -= oldCount;
                    _Count += newCount;
                    tb_count.Text = _Count.ToString();


                    if (columnName == MainWindow.resourcemanager.GetString("trPrice") && !t.Text.Equals(""))
                        newPrice = decimal.Parse(t.Text);
                    else
                        newPrice = row.Price;

                    oldPrice = row.Price;

                    // old total for changed item
                    decimal total = oldPrice * oldCount;
                    _Sum -= total;

                    // new total for changed item
                    total = newCount * newPrice;
                    _Sum += total;
                    //refresh total cell
                    TextBlock tb = dg_billDetails.Columns[6].GetCellContent(dg_billDetails.Items[index]) as TextBlock;
                    tb.Text = HelpClass.DecTostring(total);
                    //  refresh sum and total text box
                    refreshTotalValue();
                    // update item in billdetails           
                    billDetails[index].Count = (int)newCount;
                    billDetails[index].Price = newPrice;
                    billDetails[index].Total = total;
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
        void deleteRowFromInvoiceItems(object sender, RoutedEventArgs e)
        {
            try
            {
                for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                    if (vis is DataGridRow)
                    {
                        BillDetailsPurchase row = (BillDetailsPurchase)dg_billDetails.SelectedItems[0];
                        int index = dg_billDetails.SelectedIndex;

                        // remove item from bill
                        billDetails.RemoveAt(index);

                        ObservableCollection<BillDetailsPurchase> data = (ObservableCollection<BillDetailsPurchase>)dg_billDetails.ItemsSource;
                        data.Remove(row);
                    }
                _SequenceNum = 0;
                _Sum = 0;
                for (int i = 0; i < billDetails.Count; i++)
                {
                    _SequenceNum++;
                    billDetails[i].ID = _SequenceNum;
                    _Sum += billDetails[i].Total;
                    _Count = _SequenceNum;
                }
                // calculate new total
                refreshTotalValue();
            }
            catch (Exception ex)
            {
                //
                //    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion
        #region btn
        private async void Btn_returnInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                if (MainWindow.groupObject.HasPermissionAction(returnPermission, MainWindow.groupObjects, "one") )
                {
                    Window.GetWindow(this).Opacity = 0.2;

                    wd_invoice w = new wd_invoice();

                    #region window parameter
                    string invoiceType = "pbw";
                    w.invoiceType = invoiceType;
                    w.invoiceStatus = "return";
                    w.branchId = MainWindow.branchLogin.branchId;
                    w.icon = "retInvoice";
                    w.page = "storageInv";
                    w.title = MainWindow.resourcemanager.GetString("trReturnInvoices");
                    #endregion
                    if (w.ShowDialog() == true)
                    {
                        btn_items.IsEnabled = false;
                        if (w.invoice != null)
                        {
                            invoice = w.invoice;
                            _invoiceId = invoice.invoiceId;
                            _InvoiceType = invoice.invType;
                            isFromReport = false;
                            setNotifications();
                            // set title to bill
                            txt_titleDataGridInvoice.Text = MainWindow.resourcemanager.GetString("trReturnedInvoice");
                            btn_save.Content = MainWindow.resourcemanager.GetString("trReturn");
                            // orange #FFA926 red #D22A17
                            txt_titleDataGridInvoice.Foreground = Application.Current.Resources["MainColorRed"] as SolidColorBrush;
                            await fillInvoiceInputs(invoice);
                            mainInvoiceItems = invoiceItems;
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
        }
        private async void Btn_newDraft_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);

                if (billDetails.Count > 0 && _InvoiceType == "isd")
                {

                    #region Accept
                    MainWindow.mainWindow.Opacity = 0.2;
                    wd_acceptCancelPopup w = new wd_acceptCancelPopup();
                    w.contentText = MainWindow.resourcemanager.GetString("trSaveInvoiceNotification");
                    w.ShowDialog();
                    MainWindow.mainWindow.Opacity = 1;
                    #endregion
                    if (w.isOk)
                    {
                        await addInvoice(_InvoiceType);
                        clearInvoice();
                        refreshDraftNotification();
                    }
                    else
                        clearInvoice();
                }
                else
                    clearInvoice();

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
            try
            {
                HelpClass.StartAwait(grid_main);

                Window.GetWindow(this).Opacity = 0.2;
                wd_invoice w = new wd_invoice();
                string invoiceType = "isd";
                int duration = 2;
                w.invoiceType = invoiceType;
                w.userId = MainWindow.userLogin.userId;
                w.duration = duration; // view drafts which created during 2 last days 
                w.icon = "drafts";
                w.page = "storageInv";
                w.title = MainWindow.resourcemanager.GetString("trDrafts");

                if (w.ShowDialog() == true)
                {
                    if (w.invoice != null)
                    {
                        invoice = w.invoice;
                        _InvoiceType = invoice.invType;
                        _invoiceId = invoice.invoiceId;
                        isFromReport = false;
                        await fillInvoiceInputs(invoice);
                        setNotifications();
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
        }
        private async void Btn_items_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                Window.GetWindow(this).Opacity = 0.2;
                wd_purchaseItems w = new wd_purchaseItems();
                w.ShowDialog();
                if (w.isActive)
                {
                    for (int i = 0; i < w.selectedItems.Count; i++)
                    {
                        int itemId = w.selectedItems[i];
                        await ChangeItemIdEvent(itemId);
                    }
                    refreshTotalValue();
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
        }
        private async void Btn_invoices_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                if (MainWindow.groupObject.HasPermissionAction(reciptPermission, MainWindow.groupObjects, "one") )
                {
                    Window.GetWindow(this).Opacity = 0.2;

                    wd_invoice w = new wd_invoice();
                    // sale invoices
                    string invoiceType = "pw";
                    w.invoiceType = invoiceType;
                    w.branchId = MainWindow.branchLogin.branchId;
                    w.icon = "purInvoices";
                    w.page = "storageInv";
                    w.title = MainWindow.resourcemanager.GetString("trInvoices");

                    if (w.ShowDialog() == true)
                    {
                        btn_items.IsEnabled = false;
                        if (w.invoice != null)
                        {
                            invoice = w.invoice;
                            _InvoiceType = invoice.invType;
                            _invoiceId = invoice.invoiceId;
                            isFromReport = false;
                            setNotifications();
                            // set title to bill
                            txt_titleDataGridInvoice.Text = MainWindow.resourcemanager.GetString("trPurchaseInvoice");
                            txt_titleDataGridInvoice.Foreground = Application.Current.Resources["MainColorBlue"] as SolidColorBrush;
                            btn_save.Content = MainWindow.resourcemanager.GetString("trStoreBtn");
                            await fillInvoiceInputs(invoice);
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
        }
        private async void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                if (billDetails.Count > 0)
                {
                    if (_InvoiceType == "pw") //p  wait purchase invoice
                        await receiptInvoice();
                    else if (_InvoiceType == "pbw")
                        await returnInvoice("pb");
                    else if (_InvoiceType == "isd" || _InvoiceType == "is")
                    {
                        await addInvoice("is");
                        #region notification Object
                        Notification not = new Notification()
                        {
                            title = "trExceedMaxLimitAlertTilte",
                            ncontent = "trExceedMaxLimitAlertContent",
                            msgType = "alert",
                            createUserId = MainWindow.userLogin.userId,
                            updateUserId = MainWindow.userLogin.userId,
                        };
                        #endregion
                        await FillCombo.itemLocation.recieptInvoice(invoiceItems, MainWindow.branchLogin.branchId, MainWindow.userLogin.userId, "storageAlerts_minMaxItem", not); // increase item quantity in DB
                        clearInvoice();
                        refreshDraftNotification();
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
        #endregion
        #region report

        ReportCls reportclass = new ReportCls();
        LocalReport rep = new LocalReport();
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        
        private void BuildReport()
        {
            List<ReportParameter> paramarr = new List<ReportParameter>();

            string addpath;
            bool isArabic = ReportCls.checkLang();
            if (isArabic)
            {
                addpath = @"\Reports\Storage\storageOperations\Ar\ArStorageInvoice.rdlc";
            }
            else
            {
                addpath = @"\Reports\Storage\storageOperations\En\EnStorageInvoice.rdlc";
            }
            string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);
            //D:\myproj\posproject5\Restaurant\Restaurant\View\storage\storageOperations\uc_ItemsStorage.xaml.cs
       

            clsReports.StorageInvoice(invoiceItems, rep, reppath, paramarr);
            clsReports.setReportLanguage(paramarr);
            clsReports.Header(paramarr);

            rep.SetParameters(paramarr);

            rep.Refresh();
        }
        private void PrintRep()
        {
            BuildReport();

            this.Dispatcher.Invoke(() =>
            {
                LocalReportExtensions.PrintToPrinterbyNameAndCopy(rep, FillCombo.rep_printer_name, FillCombo.rep_print_count == null ? short.Parse("1") : short.Parse(FillCombo.rep_print_count));
            });
        }
        private void PdfRep()
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
                            PdfRep();
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
        private void Btn_printInvoice_Click(object sender, RoutedEventArgs e)
        {//print
            try
            {

                HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(reportsPermission, MainWindow.groupObjects, "one"))
                {

                    if (invoiceItems != null)
                    {
                        Thread t1 = new Thread(() =>
                        {
                            PrintRep();
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
        #endregion
        #region Barcode
        //async Task fillBarcodeList()
        //{
        //    barcodesList = await FillCombo.itemUnit.getAllBarcodes();
        //}
        //async void loading_fillBarcodeList()
        //{
        //    try
        //    {
        //        await fillBarcodeList();
        //    }
        //    catch (Exception)
        //    { }
        //    foreach (var item in loadingList)
        //    {
        //        if (item.key.Equals("loading_fillBarcodeList"))
        //        {
        //            item.value = true;
        //            break;
        //        }
        //    }
        //}
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
                TimeSpan elapsed = (DateTime.Now - _lastKeystroke);
                if (elapsed.TotalMilliseconds > 150)
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
                    if (_Sender != null)
                    {
                        TextBox tb = _Sender as TextBox;
                        if (tb != null)
                        {
                            if (tb.Name == "tb_taxValue" )// remove barcode from text box
                            {
                                string tbString = tb.Text;
                                string newStr = "";
                                int startIndex = tbString.IndexOf(_BarcodeStr);
                                if (startIndex != -1)
                                    newStr = tbString.Remove(startIndex, _BarcodeStr.Length);

                                tb.Text = newStr;
                            }
                        }
                    }
                    _IsFocused = false;
                    await dealWithBarcode(_BarcodeStr);
                    tb_barcode.Text = _BarcodeStr;
                    _BarcodeStr = "";
                    
                    e.Handled = true;
                }
                _Sender = null;
                tb_barcode.Clear();
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
        private async Task dealWithBarcode(string barcode)
        {
            int codeindex = barcode.IndexOf("-");
            string prefix = "";
            if (codeindex >= 0)
                prefix = barcode.Substring(0, codeindex);
            prefix = prefix.ToLower();
            barcode = barcode.ToLower();
            switch (prefix)
            {
                case "is":// this barcode for invoice
                    clearInvoice();
                    invoice = await FillCombo.invoice.GetInvoicesByNum(barcode, MainWindow.branchLogin.branchId);
                    // _InvoiceType = invoice.invType;
                    await fillInvoiceInputs(invoice);
                    break;
                default: // if barcode for item
                   // get item matches barcode
                    if (FillCombo.itemUnitList != null && _InvoiceType == "isd")
                    {
                        ItemUnit unit1 = FillCombo.itemUnitList.Find(c => c.barcode == barcode.Trim());

                        // get item matches the barcode
                        if (unit1 != null)
                        {
                            int itemId = (int)unit1.itemId;
                            if (unit1.itemId != 0)
                            {
                                int index = billDetails.IndexOf(billDetails.Where(p => p.itemUnitId == unit1.itemUnitId && p.OrderId == 0).FirstOrDefault());

                                if (index == -1)//item doesn't exist in bill
                                {
                                    // get item units
                                    itemUnits = await FillCombo.itemUnit.GetItemUnits(itemId);
                                    //get item from list
                                    item = FillCombo.purchaseItems.Find(i => i.itemId == itemId);

                                    int count = 1;
                                    decimal price = 0; //?????
                                    decimal total = count * price;
                                    addRowToBill(item.name, item.itemId, unit1.mainUnit, unit1.itemUnitId, count, price, total);
                                }
                                else // item exist prevoiusly in list
                                {
                                    billDetails[index].Count++;
                                    billDetails[index].Total = billDetails[index].Count * billDetails[index].Price;

                                    _Sum += billDetails[index].Price;

                                }
                                refreshTotalValue();
                                refrishBillDetails();
                            }
                        }
                        else
                        {
                            Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trErrorItemNotFoundToolTip"), animation: ToasterAnimation.FadeIn);
                        }
                    }
                    break;
            }
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
        #endregion
    }

}
