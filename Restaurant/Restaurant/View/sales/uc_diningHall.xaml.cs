using MaterialDesignThemes.Wpf;
using netoaster;
using Restaurant.Classes;
using Restaurant.Classes.ApiClasses;
using Restaurant.View.windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
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
using System.Windows.Threading;
using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Threading;
using System.Text.RegularExpressions;

namespace Restaurant.View.sales
{
    /// <summary>
    /// Interaction logic for uc_diningHall.xaml
    /// </summary>
    public partial class uc_diningHall : UserControl
    {
        private static uc_diningHall _instance;
        string invoicePermission = "saleInvoice_invoice";
        string addRangePermission = "copoun_invoice";
        string addTablePermission = "addTable_invoice";
        #region for report
        long prinvoiceId = 0;
        #endregion
        decimal _DeliveryCost = 0;
        public static uc_diningHall Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new uc_diningHall();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_diningHall()
        {
            try
            {
                InitializeComponent();
            }
            catch
            { }
        }
        public static List<string> catalogMenuList;
        public static List<Button> categoryBtns;
        #region loading
        List<keyValueBool> loadingList;
        async Task loading_items()
        {
            //try
            {
                await refreshItemsList();
            }
            //catch { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_items"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async Task loading_customers()
        {
            //try
            {
                if (FillCombo.customersList == null)
                    await FillCombo.RefreshCustomers();
            }
            //catch { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_customers"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async Task loading_categories()
        {
            //try
            {
                await refrishCategories();
            }
            //catch { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_categories"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async Task refrishCategories()
        {
            if (FillCombo.categoriesList == null)
                await FillCombo.RefreshCategory();
        }
        #endregion
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);
                // for pagination onTop Always
                btns = new Button[] { btn_firstPage, btn_prevPage, btn_activePage, btn_nextPage, btn_lastPage };
                catigoriesAndItemsView.ucdiningHall = this;

                catalogMenuList = new List<string> { "allMenu", "appetizers", "beverages", "fastFood", "mainCourses", "desserts" };
                categoryBtns = new List<Button> { btn_appetizers, btn_beverages, btn_fastFood, btn_mainCourses, btn_desserts };
                #region translate
                changeInvType();
                tb_moneyIcon.Text = AppSettings.Currency;
                tb_moneyIconTotal.Text = AppSettings.Currency;
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

                #region loading
                loadingList = new List<keyValueBool>();
                bool isDone = true;
                loadingList.Add(new keyValueBool { key = "loading_items", value = false });
                loadingList.Add(new keyValueBool { key = "loading_categories", value = false });
                loadingList.Add(new keyValueBool { key = "loading_customers", value = false });

                loading_items();
                loading_categories();
                loading_customers();
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
                #region invoice tax
                if (AppSettings.invoiceTax_bool == false)
                {
                    txt_tax.Visibility = Visibility.Collapsed;
                    tb_tax.Visibility = Visibility.Collapsed;
                }
                else
                    tb_tax.Text = HelpClass.PercentageDecTostring(AppSettings.invoiceTax_decimal);
                #endregion

                tb_moneyIcon.Text = AppSettings.Currency;
                #region notification
                setTimer();
                refreshDraftNotification();
                refreshOrdersNotification();
                #endregion
                HelpClass.activateCategoriesButtons(items, FillCombo.categoriesList, categoryBtns);
                // FillBillDetailsList(0);
                await Search();
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        public async void changeInvType()
        {
            int diningHall = int.Parse(AppSettings.typesOfService_diningHall);
            int takeAway = int.Parse(AppSettings.typesOfService_takeAway);
            int selfService = int.Parse(AppSettings.typesOfService_selfService);
            int countService = diningHall + takeAway + selfService;
            if (countService == 1)
            {
                if (diningHall == 1)
                    AppSettings.invType = "diningHall";
                else if (takeAway == 1)
                    AppSettings.invType = "takeAway";
                else if (selfService == 1)
                    AppSettings.invType = "selfService";
                btn_invType.IsEnabled = false;
            }
            else
            {
                try
                {
                    if (FillCombo.invoiceTypelist is null)
                        FillCombo.FillInvoiceType(new ComboBox());
                    switch (AppSettings.invType)
                    {
                        case "diningHall":
                            if (diningHall == 0)
                                AppSettings.invType = FillCombo.invoiceTypelist.FirstOrDefault().key;
                            break;
                        case "takeAway":
                            if (takeAway == 0)
                                AppSettings.invType = FillCombo.invoiceTypelist.FirstOrDefault().key;
                            break;
                        case "selfService":
                            if (selfService == 0)
                                AppSettings.invType = FillCombo.invoiceTypelist.FirstOrDefault().key;
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // don't move this debug ,yasin
                    HelpClass.ExceptionMessage(ex, this);
                }
            }

            if (AppSettings.invType == "diningHall")
            {
                _InvoiceType = "sd";
                txt_invType.Text = AppSettings.resourcemanager.GetString("trDiningHallType");

                brd_cancel.Visibility = Visibility.Visible;
                btn_cancel.Visibility = Visibility.Visible;
                btn_tables.Visibility = Visibility.Visible;
                btn_kitchen.Visibility = Visibility.Visible;
                btn_waiter.Visibility = Visibility.Visible;


                md_draft.Visibility = Visibility.Collapsed;
                btn_delivery.Visibility = Visibility.Collapsed;
                btn_orderTime.Visibility = Visibility.Collapsed;
                #region enable btns
                if (invoice.invoiceId != 0)
                {

                    btn_waiter.IsEnabled = true;
                    btn_discount.IsEnabled = true;
                    btn_customer.IsEnabled = true;
                    btn_kitchen.IsEnabled = true;

                }
                else
                {
                    btn_waiter.IsEnabled = false;
                    btn_discount.IsEnabled = false;
                    btn_customer.IsEnabled = false;
                    btn_kitchen.IsEnabled = false;
                }
                #endregion

                if (invoice.invType != "sd" && invoice.invType != null)
                    await clear();
            }
            else if (AppSettings.invType == "takeAway" /*|| _InvoiceType == "tsd"*/)
            {
                _InvoiceType = "tsd";
                txt_invType.Text = AppSettings.resourcemanager.GetString("trTakeAway");

                btn_cancel.Visibility = Visibility.Collapsed;
                brd_cancel.Visibility = Visibility.Collapsed;
                btn_tables.Visibility = Visibility.Collapsed;
                btn_kitchen.Visibility = Visibility.Collapsed;
                btn_waiter.Visibility = Visibility.Collapsed;


                md_draft.Visibility = Visibility.Visible;
                btn_delivery.Visibility = Visibility.Visible;
                btn_orderTime.Visibility = Visibility.Visible;

                btn_customer.IsEnabled = true;
                btn_discount.IsEnabled = true;

                refreshDraftNotification();

                if (invoice.invType == "ssd") // transfer  self service draft to take away draft
                {
                    await addInvoice("tsd");
                }
            }
            else if (AppSettings.invType == "selfService" /*|| _InvoiceType == "ssd"*/)
            {
                _InvoiceType = "ssd";

                txt_invType.Text = AppSettings.resourcemanager.GetString("trSelfService");

                btn_cancel.Visibility = Visibility.Collapsed;
                brd_cancel.Visibility = Visibility.Collapsed;
                btn_kitchen.Visibility = Visibility.Collapsed;
                btn_waiter.Visibility = Visibility.Collapsed;
                btn_delivery.Visibility = Visibility.Collapsed;


                md_draft.Visibility = Visibility.Visible;
                btn_orderTime.Visibility = Visibility.Visible;
                btn_tables.Visibility = Visibility.Visible;

                btn_customer.IsEnabled = true;
                btn_discount.IsEnabled = true;

                refreshDraftNotification();
                if (invoice.invType == "tsd") // transfer take away  draft to self service draft
                {
                    await addInvoice("ssd");
                }
            }           

        }
        private void translate()
        {
            txt_orders.Text = AppSettings.resourcemanager.GetString("trOrders");
            txt_allMenu.Text = AppSettings.resourcemanager.GetString("trAll");

            txt_appetizers.Text = AppSettings.resourcemanager.GetString("trAppetizers");
            txt_beverages.Text = AppSettings.resourcemanager.GetString("trBeverages");
            txt_fastFood.Text = AppSettings.resourcemanager.GetString("trFastFood");
            txt_mainCourses.Text = AppSettings.resourcemanager.GetString("trMainCourses");
            txt_desserts.Text = AppSettings.resourcemanager.GetString("trDesserts");

            txt_ordersAlerts.Text = AppSettings.resourcemanager.GetString("trOrders");
            txt_newDraft.Text = AppSettings.resourcemanager.GetString("trNew");
            txt_preview.Text = AppSettings.resourcemanager.GetString("trPreview");
            txt_pdf.Text = AppSettings.resourcemanager.GetString("trPdf");
            txt_printInvoice.Text = AppSettings.resourcemanager.GetString("trPrint");
            txt_subtotal.Text = AppSettings.resourcemanager.GetString("trSubTotal");
            txt_totalDiscount.Text = AppSettings.resourcemanager.GetString("trDiscount");
            txt_tax.Text = AppSettings.resourcemanager.GetString("trTax");
            txt_total.Text = AppSettings.resourcemanager.GetString("trTotal");
            txt_drafts.Text = AppSettings.resourcemanager.GetString("trDraft");


            btn_pay.Content = AppSettings.resourcemanager.GetString("trPay");

            #region

            /*
            txt_discount.Text = AppSettings.resourcemanager.GetString("trDiscount");
            txt_customer.Text = AppSettings.resourcemanager.GetString("trCustomer");
            txt_waiter.Text = AppSettings.resourcemanager.GetString("trWaiter");
            txt_kitchen.Text = AppSettings.resourcemanager.GetString("trKitchen");
            txt_tables.Text = AppSettings.resourcemanager.GetString("trTables");
            txt_orderTime.Text = AppSettings.resourcemanager.GetString("time");
            txt_delivery.Text = AppSettings.resourcemanager.GetString("trDelivery");
            */

            btn_tables.ToolTip = AppSettings.resourcemanager.GetString("trTables");
            btn_kitchen.ToolTip = AppSettings.resourcemanager.GetString("trKitchen");
            btn_waiter.ToolTip = AppSettings.resourcemanager.GetString("trWaiter");
            btn_customer.ToolTip = AppSettings.resourcemanager.GetString("trCustomer");
            btn_discount.ToolTip = AppSettings.resourcemanager.GetString("trDiscount");
            btn_delivery.ToolTip = AppSettings.resourcemanager.GetString("trDelivery");
            btn_orderTime.ToolTip = AppSettings.resourcemanager.GetString("time");

            #endregion
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {

                //HelpClass.StartAwait(grid_main);
                Instance = null;
                GC.Collect();
                //HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                //HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #region  Cards
        CatigoriesAndItemsView catigoriesAndItemsView = new CatigoriesAndItemsView();
        #region Refrish Y
        Item item = new Item();
        List<Item> items;
        IEnumerable<Item> itemsQuery;
        long categoryId = 0;
        long tagId = 0;
        public string _InvoiceType = "sd";
        void RefrishItemsCard(IEnumerable<Item> _items)
        {
            grid_itemContainerCard.Children.Clear();
            catigoriesAndItemsView.gridCatigorieItems = grid_itemContainerCard;
            catigoriesAndItemsView.FN_refrishCatalogItem(_items.ToList(), 3, 5, "sales");
        }
        #endregion
        #region Get Id By Click  Y
        itemsTransferIngredients dishIngredients = new itemsTransferIngredients();
        public async void ChangeItemIdEvent(long itemId)
        {
            try
            {
                if (AppSettings.invType == "diningHall" && selectedTables.Count == 0)
                    Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trChooseTableFirst"), animation: ToasterAnimation.FadeIn);
                else if (item != null)
                {
                    item = items.Where(x => x.itemId == itemId).FirstOrDefault();
                    #region item Ingredients
                    List<itemsTransferIngredients> ingredientsList = await dishIngredients.GetItemIngredients(0,(long)item.itemUnitId);
                    #endregion
                    addRowToBill(item, 1, ingredientsList,new List<ItemTransfer>(),0,false, false);
                   

                    #region save draft invoice
                    #endregion
                }
            }
            catch { }
        }
        #endregion
        #region Search Y - refresh


        /// <summary>
        /// Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async Task Search()
        {
            //search
            try
            {
                //HelpClass.StartAwait(grid_main);

                if (items is null)
                    await refreshItemsList();

                itemsQuery = items.ToList();

                #region search for category
                if (categoryId > 0)
                    itemsQuery = itemsQuery.Where(x => x.categoryId == categoryId).ToList();
                #endregion

                #region search for tag
                if (tagId > 0)
                    itemsQuery = itemsQuery.Where(x => x.tagId == tagId).ToList();
                #endregion

                pageIndex = 1;

                #region


                if (btns is null)
                    btns = new Button[] { btn_firstPage, btn_prevPage, btn_activePage, btn_nextPage, btn_lastPage };
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns, 15));
                #endregion

                //HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                //HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        async Task refreshItemsList()
        {
            DateTime dt = DateTime.Now;
            string day = dt.DayOfWeek.ToString();
            items = await FillCombo.item.GetAllSalesItemsInv(MainWindow.branchLogin.branchId, day.ToLower(), AppSettings.invType, _MemberShipId);
        }
        #endregion
        #region Pagination Y
        Pagination pagination = new Pagination();
        Button[] btns;
        public int pageIndex = 1;

        private void Tb_pageNumberSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);

                itemsQuery = items.ToList();

                if (tb_pageNumberSearch.Text.Equals(""))
                {
                    pageIndex = 1;
                }
                else if (((itemsQuery.Count() - 1) / 9) + 1 < int.Parse(tb_pageNumberSearch.Text))
                {
                    pageIndex = ((itemsQuery.Count() - 1) / 9) + 1;
                }
                else
                {
                    pageIndex = int.Parse(tb_pageNumberSearch.Text);
                }

                #region

                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns, 15));
                #endregion


                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }


        private void Btn_firstPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);

                pageIndex = 1;
                #region
                itemsQuery = items.ToList();
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns, 15));
                #endregion


                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_prevPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);

                pageIndex = int.Parse(btn_prevPage.Content.ToString());
                #region
                itemsQuery = items.ToList();

                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns, 15));
                #endregion


                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_activePage_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);

                pageIndex = int.Parse(btn_activePage.Content.ToString());
                #region
                itemsQuery = items.ToList();
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns, 15));
                #endregion


                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_nextPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);

                pageIndex = int.Parse(btn_nextPage.Content.ToString());
                #region
                itemsQuery = items.ToList();
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns, 15));
                #endregion


                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_lastPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);

                itemsQuery = items.ToList();
                pageIndex = ((itemsQuery.Count() - 1) / 9) + 1;
                #region
                itemsQuery = items.ToList();
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns, 15));
                #endregion


                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion
        #endregion
        #region catalogMenu
        private async void catalogMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tagId = 0;
                string senderTag = (sender as Button).Tag.ToString();
                if (senderTag != "allMenu")
                    categoryId = FillCombo.categoriesList.Where(x => x.categoryCode == senderTag).FirstOrDefault().categoryId;
                else
                    categoryId = -1;
                #region refresh colors
                foreach (var control in catalogMenuList)
                {
                    Border border = FindControls.FindVisualChildren<Border>(this).Where(x => x.Tag != null && x.Name == "bdr_" + control)
                        .FirstOrDefault();
                    if (border.Tag.ToString() == senderTag)
                        border.Background = Application.Current.Resources["MainColor"] as SolidColorBrush;
                    else
                        border.Background = Application.Current.Resources["White"] as SolidColorBrush;
                }
                foreach (var control in catalogMenuList)
                {

                    Path path = FindControls.FindVisualChildren<Path>(this).Where(x => x.Tag != null && x.Name == "path_" + control)
                        .FirstOrDefault();
                    if (path.Tag.ToString() == senderTag)
                        path.Fill = Application.Current.Resources["White"] as SolidColorBrush;
                    else
                        path.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
                }
                foreach (var control in catalogMenuList)
                {
                    TextBlock textBlock = FindControls.FindVisualChildren<TextBlock>(this).Where(x => x.Tag != null && x.Name == "txt_" + control)
                        .FirstOrDefault();
                    if (textBlock.Tag.ToString() == senderTag)
                        textBlock.Foreground = Application.Current.Resources["White"] as SolidColorBrush;
                    else
                        textBlock.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                }
                #endregion
                refreshCatalogTags(senderTag);
                await Search();
            }
            catch { }
        }
        public static List<Tag> tagsList;
        async void refreshCatalogTags(string tag)
        {
            tagsList = await FillCombo.tag.Get(categoryId);
            tagsList = tagsList.Where(x => x.isActive == 1).ToList();

            if (tagsList.Count > 1)
            {
                Tag allTag = new Tag();
                allTag.tagName = AppSettings.resourcemanager.GetString("trAll");
                allTag.tagId = 0;
                tagsList.Add(allTag);
            }

            sp_menuTags.Children.Clear();
            foreach (var item in tagsList)
            {
                #region  
                Button button = new Button();
                button.Content = item.tagName;
                button.Tag = "catalogTags-" + item.tagName;
                button.FontSize = 10;
                button.Height = 25;
                button.Padding = new Thickness(5, 0, 5, 0);
                MaterialDesignThemes.Wpf.ButtonAssist.SetCornerRadius(button, (new CornerRadius(7)));
                button.Margin = new Thickness(5, 0, 5, 0);

                if (item.tagName == AppSettings.resourcemanager.GetString("trAll"))
                {
                    button.Foreground = Application.Current.Resources["White"] as SolidColorBrush;
                    button.Background = Application.Current.Resources["MainColor"] as SolidColorBrush;
                }
                else
                {
                    button.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                    button.Background = Application.Current.Resources["White"] as SolidColorBrush;
                }

                button.BorderBrush = Application.Current.Resources["MainColor"] as SolidColorBrush;
                button.Click += buttonCatalogTags_Click;


                sp_menuTags.Children.Add(button);
                /////////////////////////////////

                #endregion
            }
        }
        async void buttonCatalogTags_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string senderTag = (sender as Button).Tag.ToString();
                #region refresh colors
                foreach (var control in tagsList)
                {
                    Button button = FindControls.FindVisualChildren<Button>(this).Where(x => x.Tag != null && x.Tag.ToString() == "catalogTags-" + control.tagName)
                         .FirstOrDefault();
                    if (button.Tag.ToString() == senderTag)
                    {
                        button.Foreground = Application.Current.Resources["White"] as SolidColorBrush;
                        button.Background = Application.Current.Resources["MainColor"] as SolidColorBrush;
                        tagId = control.tagId;
                    }
                    else
                    {
                        button.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                        button.Background = Application.Current.Resources["White"] as SolidColorBrush;
                    }
                }
                #endregion
                await Search();
            }
            catch { }
        }
        #endregion
        #region invoice

        ObservableCollection<BillDetailsSales> billDetailsList = new ObservableCollection<BillDetailsSales>();
        int _SequenceNum = 0;
        decimal _Sum = 0;
        decimal _ManualDiscount = 0;
        decimal _invoiceClassDiscount = 0;
        decimal _DeliveryDiscount = 0;
        string _DiscountType = "";
        long _MemberShipId = 0;
        List<CouponInvoice> selectedCopouns = new List<CouponInvoice>();
        List<ItemTransfer> invoiceItems = new List<ItemTransfer>();
        List<Tables> selectedTables = new List<Tables>();
        public Invoice invoice = new Invoice();
        TablesReservation reservation = new TablesReservation();
        InvoicesClass invoiceMemberShipClass = new InvoicesClass();
        List<InvoicesClass> customerInvClasses = new List<InvoicesClass>();
        private async void addRowToBill(Item item, long count,List<itemsTransferIngredients> ingredients,List<ItemTransfer> itemExtras,long itemTransferId,bool isExtra, bool isList = false)
        {
            decimal total = 0;
           // var invoiceItem = billDetailsList.Where(x => x.itemId == item.itemId).FirstOrDefault();
            #region add item to invoice
            //if (invoiceItem == null)
            {
                decimal price = 0;
                decimal basicPrice = (decimal)item.basicPrice;
                if (AppSettings.itemsTax_bool == true)
                    price = (decimal)item.priceTax;
                else
                    price = (decimal)item.price;

                long offerId = 0;
                string offerType = "1";
                decimal offerValue = 0;
                if (item.offerId != null)
                {
                    offerId = (long)item.offerId;
                    offerType = item.discountType;
                    offerValue = (decimal)item.discountValue;
                }
                // increase sequence for each read
               
                total = (decimal)item.price;
                billDetailsList.Add(new BillDetailsSales()
                {
                    index = _SequenceNum,
                    image = item.image,
                    itemId = item.itemId,
                    itemUnitId = (long)item.itemUnitId,
                    itemName = item.name,
                    Count = (int)count,
                    Price = (decimal)item.price,
                    basicPrice = basicPrice,
                    Total = ((decimal)item.price * (int)count),
                    offerId = item.offerId,
                    OfferType = offerType,
                    OfferValue = offerValue,
                    forAgents = item.forAgent,
                    itemsIngredients = ingredients,
                    itemExtras = itemExtras,
                    isExtra = isExtra,
                    itemsTransId = itemTransferId,
                });
                _SequenceNum++;
            }
            #endregion
            //#region item already exist in invoice
            //else
            //{
            //    invoiceItem.Count++;
            //    total = invoiceItem.Price * invoiceItem.Count;
            //    invoiceItem.Total = total;
            //}
            //#endregion
            await refreshTotal();
            BuildBillDesign();
            if (isList == false)
                setKitchenNotification();
        }
        void BuildBillDesign()
        {
            //sv_billDetail.Children.Clear();
            #region Grid Container
            Grid gridContainer = new Grid();
            gridContainer.Margin = new Thickness(5);
            //int rowCount = billDetailsList.Count();
            int rowCount = billDetailsList.Count;
            RowDefinition[] rd = new RowDefinition[rowCount];
            for (int i = 0; i < rowCount; i++)
            {
                rd[i] = new RowDefinition();
                rd[i].Height = new GridLength(1, GridUnitType.Auto);
            }
            for (int i = 0; i < rowCount; i++)
            {
                gridContainer.RowDefinitions.Add(rd[i]);
            }
            /////////////////////////////////////////////////////
            int colCount = 8;
            ColumnDefinition[] cd = new ColumnDefinition[colCount];
            for (int i = 0; i < colCount; i++)
            {
                cd[i] = new ColumnDefinition();
            }
            cd[0].Width = new GridLength(1, GridUnitType.Auto);
            cd[1].Width = new GridLength(1, GridUnitType.Auto);
            cd[2].Width = new GridLength(1, GridUnitType.Auto);
            cd[3].Width = new GridLength(1, GridUnitType.Star);
            cd[4].Width = new GridLength(1, GridUnitType.Auto);
            cd[5].Width = new GridLength(35, GridUnitType.Pixel);
            cd[6].Width = new GridLength(1, GridUnitType.Auto);
            cd[7].Width = new GridLength(1, GridUnitType.Star);
            for (int i = 0; i < colCount; i++)
            {
                gridContainer.ColumnDefinitions.Add(cd[i]);
            }
            /////////////////////////////////////////////////////



            #endregion
            _SequenceNum = 0;
            foreach (var item in billDetailsList)
            {
                var it = items.Where(x => x.itemId == item.itemId).FirstOrDefault();
                item.index = _SequenceNum;
                #region   index
                var indexText = new TextBlock();
                indexText.Text = (item.index + 1) + ".";
                indexText.Tag = ("index-" + item.index);
                //indexText.Tag = item.index;
                indexText.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                indexText.FontSize = 14;
                indexText.FontWeight = FontWeights.Bold;
                indexText.Margin = new Thickness(5);
                indexText.VerticalAlignment = VerticalAlignment.Center;
                indexText.HorizontalAlignment = HorizontalAlignment.Center;

                Grid.SetRow(indexText, item.index);
                Grid.SetColumn(indexText, 0);
                gridContainer.Children.Add(indexText);
                /////////////////////////////////

                #endregion
                #region   image
                Button buttonImage = new Button();
                buttonImage.Tag = "image-" + item.index;
                //buttonImage.Tag = item.index;
                buttonImage.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
                buttonImage.Height =
                buttonImage.Width = 40;
                buttonImage.Margin = new Thickness(5, 10, 5, 10);
                buttonImage.VerticalAlignment = VerticalAlignment.Center;
                buttonImage.HorizontalAlignment = HorizontalAlignment.Center;
                buttonImage.BorderThickness = new Thickness(1);
                buttonImage.BorderBrush = Application.Current.Resources["Grey"] as SolidColorBrush;
                buttonImage.Padding = new Thickness(0);
                buttonImage.FlowDirection = FlowDirection.LeftToRight;
                MaterialDesignThemes.Wpf.ButtonAssist.SetCornerRadius(buttonImage, (new CornerRadius(10)));
                //buttonImage.Cursor = Cursors.Arrow;
                //MaterialDesignThemes.Wpf.ShadowAssist.SetDarken(buttonImage, false);
                MaterialDesignThemes.Wpf.ShadowAssist.SetShadowDepth(buttonImage, ShadowDepth.Depth0);
                //MaterialDesignThemes.Wpf.ShadowAssist.SetShadowEdges(buttonImage,ShadowEdges.None);

                bool isModified = HelpClass.chkImgChng(item.image, (DateTime)it.updateDate, Global.TMPItemsFolder);
                if (isModified)
                    HelpClass.getImg("Item", item.image, buttonImage);
                else
                    HelpClass.getLocalImg("Item", item.image, buttonImage);

                buttonImage.Click += buttonItemImage_Click;
                Grid.SetRow(buttonImage, item.index);
                Grid.SetColumn(buttonImage, 1);
                gridContainer.Children.Add(buttonImage);
                /////////////////////////////////

                #endregion
                #region IsExtra
                Path itemIsExtraPath = new Path();
                itemIsExtraPath.Tag = "isExtra-" + item.index;
                if (item.isExtra)
                    itemIsExtraPath.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
                else
                    itemIsExtraPath.Fill = Application.Current.Resources["Grey"] as SolidColorBrush;
                itemIsExtraPath.Data = App.Current.Resources["StarIconGeometry"] as Geometry;
                itemIsExtraPath.Stretch = Stretch.Fill;
                itemIsExtraPath.FlowDirection = FlowDirection.LeftToRight;
                itemIsExtraPath.Margin = new Thickness(1);
                itemIsExtraPath.Width = 10;
                itemIsExtraPath.Height = 10;
                Grid.SetRow(itemIsExtraPath, item.index);
                Grid.SetColumn(itemIsExtraPath, 2);
                gridContainer.Children.Add(itemIsExtraPath);
                #endregion
                #region   name
                var itemNameText = new TextBlock();
                itemNameText.Text = item.itemName;
                itemNameText.Tag = "name-" + item.index;
                //itemNameText.Tag = item.index;
                itemNameText.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                itemNameText.FontSize = 14;
                itemNameText.Margin = new Thickness(5);
                itemNameText.VerticalAlignment = VerticalAlignment.Center;
                itemNameText.HorizontalAlignment = HorizontalAlignment.Left;
                itemNameText.FontWeight = FontWeights.Bold;

                Grid.SetRow(itemNameText, item.index);
                Grid.SetColumn(itemNameText, 3);
                gridContainer.Children.Add(itemNameText);
                #endregion
                #region   count
                 /*
                 var countText = new TextBlock();
                 countText.Text = item.Count.ToString();
                 countText.Tag = "count-" + item.index;
                 //countText.Tag = item.index;
                 countText.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                 countText.FontSize = 14;
                 countText.FontWeight = FontWeights.Bold;
                 countText.Margin = new Thickness(5);
                 countText.VerticalAlignment = VerticalAlignment.Center;
                 countText.HorizontalAlignment = HorizontalAlignment.Center;

                 Grid.SetRow(countText, item.index);
                 Grid.SetColumn(countText, 5);
                 gridContainer.Children.Add(countText);
                 /////////////////////////////////
                 */
                Border countBorder = new Border();
                countBorder.Tag = "count-" + item.index;
                countBorder.CornerRadius = new CornerRadius(10);
                countBorder.Margin = new Thickness(5);
                countBorder.BorderThickness = new Thickness(1);
                countBorder.BorderBrush = Application.Current.Resources["LightGrey"] as SolidColorBrush;
                countBorder.Height = 32;

                #region   count

                var countTextBox = new TextBox();
                countTextBox.Text = item.Count.ToString();
                //countBorder.Name = "countTextBox-" + item.index;
                countTextBox.Tag = "countTextBox-" + item.index;
                countTextBox.Margin = new Thickness(4,0,4,0);
                countTextBox.BorderThickness = new Thickness(0);
                countTextBox.Foreground = Application.Current.Resources["ThickGrey"] as SolidColorBrush;
                countTextBox.MaxLength = 2;
                countTextBox.Height = 30;
                countTextBox.Padding = new Thickness(0, 7.5, 0, 0);
                countTextBox.HorizontalAlignment = HorizontalAlignment.Center;
                countTextBox.VerticalAlignment = VerticalAlignment.Center;
                MaterialDesignThemes.Wpf.TextFieldAssist.SetCharacterCounterStyle(countTextBox , null);
                //countTextBox.TextChanged += inviceItemCount_TextChange;
                countTextBox.PreviewTextInput += NumberInt_PreviewTextInput;
                countTextBox.PreviewKeyDown += Spaces_PreviewKeyDown;
                countTextBox.LostFocus += inviceItemCount_LostFocus;

                countBorder.Child = countTextBox;
                #endregion
                Grid.SetRow(countBorder, item.index);
                Grid.SetColumn(countBorder, 5);
                gridContainer.Children.Add(countBorder);

                #endregion
                #region   total
                var totalText = new TextBlock();
                totalText.Text = HelpClass.DecTostring( item.Total);
                totalText.Tag = "total-" + item.index;
                //totalText.Tag = item.index;
                totalText.Foreground = Application.Current.Resources["Grey"] as SolidColorBrush;
                totalText.Margin = new Thickness(5);
                totalText.VerticalAlignment = VerticalAlignment.Center;
                totalText.HorizontalAlignment = HorizontalAlignment.Right;

                Grid.SetRow(totalText, item.index);
                Grid.SetColumn(totalText, 7);
                gridContainer.Children.Add(totalText);
                /////////////////////////////////

                #endregion
                #region   minus
                Button buttonMinus = new Button();
                buttonMinus.Tag = "minus-" + item.index;
                //buttonMinus.Tag = item.index;
                buttonMinus.Margin = new Thickness(2.5);
                buttonMinus.BorderThickness = new Thickness(0);
                buttonMinus.Height =
                buttonMinus.Width = 30;
                buttonMinus.Padding = new Thickness(0);
                MaterialDesignThemes.Wpf.ButtonAssist.SetCornerRadius(buttonMinus, (new CornerRadius(5)));
                if (item.Count < 2)
                    buttonMinus.Background = Application.Current.Resources["MainColor"] as SolidColorBrush;
                else
                    buttonMinus.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#DFDFDF"));
                #region materialDesign
                var MinusPackIcon = new PackIcon();
                MinusPackIcon.Tag = "minusPackIcon-" + item.index;
                if (item.Count < 2)
                {
                    MinusPackIcon.Foreground = Application.Current.Resources["White"] as SolidColorBrush;
                    MinusPackIcon.Kind = PackIconKind.Delete;
                    MinusPackIcon.Height =
                    MinusPackIcon.Width = 20;
                }
                else
                {
                    MinusPackIcon.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                    MinusPackIcon.Kind = PackIconKind.Minus;
                    MinusPackIcon.Height =
               MinusPackIcon.Width = 25;
                }

                #endregion
                buttonMinus.Content = MinusPackIcon;
                buttonMinus.Click += buttonMinus_Click;

                Grid.SetRow(buttonMinus, item.index);
                Grid.SetColumn(buttonMinus, 4);
                gridContainer.Children.Add(buttonMinus);
                /////////////////////////////////

                #endregion
                #region   plus
                Button buttonPlus = new Button();
                buttonPlus.Tag = "plus-" + item.index;
                //buttonPlus.Tag = item.index;
                buttonPlus.Margin = new Thickness(2.5);
                buttonPlus.BorderThickness = new Thickness(0);
                buttonPlus.Height =
                buttonPlus.Width = 30;
                buttonPlus.Padding = new Thickness(0);
                buttonPlus.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#DFDFDF"));
                MaterialDesignThemes.Wpf.ButtonAssist.SetCornerRadius(buttonPlus, (new CornerRadius(5)));
                #region materialDesign
                var PlusPackIcon = new PackIcon();
                PlusPackIcon.Tag = "plusPackIcon-" + item.index;
                PlusPackIcon.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                PlusPackIcon.Height =
                PlusPackIcon.Width = 25;
                PlusPackIcon.Kind = PackIconKind.Plus;
                #endregion
                buttonPlus.Content = PlusPackIcon;
                buttonPlus.Click += buttonPlus_Click;
                Grid.SetRow(buttonPlus, item.index);
                Grid.SetColumn(buttonPlus, 6);
                gridContainer.Children.Add(buttonPlus);
                /////////////////////////////////
                #endregion
                _SequenceNum++;
            }
            sv_billDetail.Content = gridContainer;

        }
        async void buttonItemImage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int index = int.Parse(button.Tag.ToString().Replace("image-", ""));
           
            try
            {
                HelpClass.StartAwait(grid_main);
                Window.GetWindow(this).Opacity = 0.2;
                wd_invoiceItemIngredients w = new wd_invoiceItemIngredients();
                w.itemsIngredients =  billDetailsList[index].itemsIngredients;
                w.itemExtras = billDetailsList[index].itemExtras;
                w.itemId = billDetailsList[index].itemId;
                w.sentToKitchen = billDetailsList[index].sentToKitchen;
                w.ShowDialog();
                
                    
                setStarColor(index);
                await refreshTotal();
                BuildBillDesign();




                Window.GetWindow(this).Opacity = 1;
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }

        }
        void setStarColor(int index)
        {
            var ingredientEdit = billDetailsList[index].itemsIngredients.Where(x => x.isActive == 0).FirstOrDefault();
            if (billDetailsList[index].itemExtras.Count >0 || ingredientEdit != null )
                billDetailsList[index].isExtra = true;
            else
                billDetailsList[index].isExtra = false;


            List<Path> isExtraPathList = FindControls.FindVisualChildren<Path>(this)
                 .Where(x => x.Tag != null && x.Tag.ToString().Contains("isExtra-" + index)  ).ToList();
            if(isExtraPathList.Count> 0)
            {
                Path isExtraPath = isExtraPathList.FirstOrDefault();
                //yasin
                if (billDetailsList[index].isExtra)
                    isExtraPath.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
                else
                    isExtraPath.Fill = Application.Current.Resources["Grey"] as SolidColorBrush;
            }
           
        }
        async void buttonPlus_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int index = int.Parse(button.Tag.ToString().Replace("plus-", ""));
            //index--;
            if(billDetailsList[index].Count < 99)
            {

            billDetailsList[index].Count++;
            //billDetailsList[index].Total = billDetailsList[index].Count * billDetailsList[index].Price;
            editBillRow(index);
            if (billDetailsList[index].Count == 2)
                refreshDeleteButtonInvoice(index, billDetailsList[index].Count);

                await refreshTotal();
            BuildBillDesign();
                setKitchenNotification();
            }
        }
        async void   buttonMinus_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int index = int.Parse(button.Tag.ToString().Replace("minus-", ""));

            long itemUnitId = billDetailsList[index].itemUnitId;
            long itemTransId = billDetailsList[index].itemsTransId;
            int sentToKitchenCount = sentInvoiceItems.Where(x => x.itemUnitId == itemUnitId && x.itemsTransId == itemTransId ).Select(x => x.Count).Sum();
            // index--;
            if (billDetailsList[index].Count <= sentToKitchenCount)
            {
                Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trItemCannotDelete"), animation: ToasterAnimation.FadeIn);
            }
            else
            {
                if (billDetailsList[index].Count < 2)
                {
                    billDetailsList.Remove(billDetailsList[index]);
                    int counter = 0;
                    foreach (var item in billDetailsList)
                    {
                        item.index = counter;
                        counter++;
                    }

                }
                else
                {
                    billDetailsList[index].Count--;
                    //billDetailsList[index].Total = billDetailsList[index].Count * billDetailsList[index].Price;
                    editBillRow(index);
                    if (billDetailsList[index].Count == 1)
                        refreshDeleteButtonInvoice(index, billDetailsList[index].Count);
                }

                await refreshTotal();
                BuildBillDesign();
                setKitchenNotification();
            }

        }
        void refreshDeleteButtonInvoice(int index, int count)
        {
            Button buttonMinus = new Button();
            var buttonMinuslist = FindControls.FindVisualChildren<Button>(this).Where(x => x.Tag != null);
            foreach (var item in buttonMinuslist)
            {
                if (item.Tag.ToString() == ("minus-" + index))
                {
                    buttonMinus = item;
                    break;
                }

            }

            PackIcon MinusPackIcon = new PackIcon();
            var MinusPackIconlist = FindControls.FindVisualChildren<PackIcon>(this).Where(x => x.Tag != null);
            foreach (var item in MinusPackIconlist)
            {
                if (item.Tag.ToString() == ("minusPackIcon-" + index))
                {
                    MinusPackIcon = item;
                    break;
                }
            }

            if (count == 1)
                buttonMinus.Background = Application.Current.Resources["MainColor"] as SolidColorBrush;
            else if (count == 2)
                buttonMinus.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#DFDFDF"));

            if (count == 1)
            {
                MinusPackIcon.Foreground = Application.Current.Resources["White"] as SolidColorBrush;
                MinusPackIcon.Kind = PackIconKind.Delete;
                MinusPackIcon.Height =
                MinusPackIcon.Width = 20;
            }
            else if (count == 2)
            {
                MinusPackIcon.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                MinusPackIcon.Kind = PackIconKind.Minus;
                MinusPackIcon.Height =
                MinusPackIcon.Width = 25;
            }
        }
        void editBillRow(int index)
        {
            var textBlocklist = FindControls.FindVisualChildren<TextBlock>(this).Where(x => x.Tag != null);
            foreach (var item in textBlocklist)
            {
                if (item.Tag.ToString() == ("count-" + index))
                {
                    item.Text = billDetailsList[index].Count.ToString();
                }
                else if (item.Tag.ToString() == ("total-" + index))
                {
                    item.Text = (billDetailsList[index].Count * billDetailsList[index].Price).ToString();
                }
            }


        }
        async Task refreshTotal()
        {
           
           decimal total = 0;
            #region subtotal
            _Sum = 0;
            //foreach (var item in billDetailsList)
            //{
            //    _Sum += item.Total;

            //    foreach (var it in item.itemExtras)
            //    {

            //        _Sum += it.price * it.quantity * item.Count;
            //    }
            //}

            decimal totalItemExtras = 0;
            foreach (var item in billDetailsList)
            {

                foreach (var itemEx in item.itemExtras)
                {
                    totalItemExtras += itemEx.price * itemEx.quantity;
                }

                item.Total = (item.Price + totalItemExtras) * item.Count;

                _Sum += item.Total;
                totalItemExtras = 0;
            }


            tb_subtotal.Text = HelpClass.DecTostring( _Sum);
            total = _Sum;
            #endregion

            #region discount
            decimal couponsDiscount = 0;
            decimal totalDiscount = 0;
            if (_Sum > 0)
            {
                #region calculate coupons discount
                if (selectedCopouns != null)
                {
                    foreach (CouponInvoice coupon in selectedCopouns)
                    {
                        string discountType = coupon.discountType.ToString();
                        decimal discountValue = (decimal)coupon.discountValue;
                        if (discountType == "2")
                            discountValue = HelpClass.calcPercentage(_Sum, discountValue);
                        couponsDiscount += discountValue;
                    }
                }
                #endregion
                #region manaula discount 
                decimal manualDiscount = _ManualDiscount;
                if (manualDiscount != 0)
                {
                    if (_DiscountType == "2")
                        manualDiscount = HelpClass.calcPercentage(_Sum, manualDiscount);
                }
                #endregion

                #region customer invoice classes discount
                foreach (InvoicesClass c in customerInvClasses)
                {
                    if (_Sum >= c.minInvoiceValue && (_Sum <= c.maxInvoiceValue || c.maxInvoiceValue == 0))
                    {
                        if (c.discountValue != 0)
                        {
                            _invoiceClassDiscount = c.discountValue;
                            if (c.discountType == 2)
                                _invoiceClassDiscount = HelpClass.calcPercentage(_Sum, _invoiceClassDiscount);
                        }
                        invoiceMemberShipClass = c;
                        break;
                    }
                }
                if (invoiceMemberShipClass.invClassMemberId != 0)
                    await FillCombo.invoice.saveMemberShipClassDis(invoiceMemberShipClass, invoice.invoiceId);
                #endregion

                totalDiscount = couponsDiscount + manualDiscount + _invoiceClassDiscount;
                //tb_totalDiscount.Text = totalDiscount.ToString();
                tb_totalDiscount.Text = HelpClass.PercentageDecTostring(totalDiscount);
            }

            total = _Sum - totalDiscount;

            #endregion



            #region hid - display discount 
            if (totalDiscount > 0)
            {
                txt_totalDiscount.Visibility = Visibility.Visible;
                tb_totalDiscount.Visibility = Visibility.Visible;
                tb_moneyIconDis.Visibility = Visibility.Visible;
            }
            else
            {
                txt_totalDiscount.Visibility = Visibility.Collapsed;
                tb_totalDiscount.Visibility = Visibility.Collapsed;
                tb_moneyIconDis.Visibility = Visibility.Collapsed;
            }
            #endregion


            #region invoice tax value 
            decimal taxValue = 0;
            if (AppSettings.invoiceTax_bool == true)
            {
                try
                {
                    taxValue = HelpClass.calcPercentage(total, decimal.Parse(tb_tax.Text));
                }
                catch { }
            }
            total += taxValue;
            #endregion
            #region delivery cost
            decimal deliveryCostAfterDisc = _DeliveryCost;

            if (_DeliveryDiscount > 0)
            {
                if (_DeliveryDiscount == 100)
                    deliveryCostAfterDisc = 0;
                else
                    deliveryCostAfterDisc = HelpClass.calcPercentage(_DeliveryCost, _DeliveryDiscount);
            }

            //total += _DeliveryCost;
            total += deliveryCostAfterDisc;

            if (_DeliveryCost > 0)
            {
                txt_deliveryVal.Visibility = Visibility.Visible;
                tb_delivery.Visibility = Visibility.Visible;
                tb_moneyIconDelivery.Visibility = Visibility.Visible;

                tb_delivery.Text = deliveryCostAfterDisc.ToString();
            }
            else
            {
                txt_deliveryVal.Visibility = Visibility.Collapsed;
                tb_delivery.Visibility = Visibility.Collapsed;
                tb_moneyIconDelivery.Visibility = Visibility.Collapsed;
            }
            #endregion

            tb_total.Text = HelpClass.DecTostring( total);

        }
        /*
        private void inviceItemCount_TextChange(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            try
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = "1";
                }
                else
                {
                    if (int.Parse(textBox.Text) <= 0)
                        textBox.Text = "1";
                }
            }
            catch
            {
                textBox.Text = "1";
            }
            int newCount = int.Parse(textBox.Text);
            int index = int.Parse(textBox.Tag.ToString().Replace("countTextBox-", ""));
                ////////// Plus
            if(billDetailsList[index].Count < newCount)
            {
                billDetailsList[index].Count = newCount;
                //billDetailsList[index].Total = billDetailsList[index].Count * billDetailsList[index].Price;
                editBillRow(index);
                //if (billDetailsList[index].Count == 2)
                    refreshDeleteButtonInvoice(index, billDetailsList[index].Count);

              
            }
                /////////// Minus
            else if (billDetailsList[index].Count > newCount)
            {
                long itemUnitId = billDetailsList[index].itemUnitId;
                long itemTransId = billDetailsList[index].itemsTransId;
                int sentToKitchenCount = sentInvoiceItems.Where(x => x.itemUnitId == itemUnitId && x.itemsTransId == itemTransId).Select(x => x.Count).Sum();
                if (billDetailsList[index].Count <= sentToKitchenCount)
                    Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trItemCannotDelete"), animation: ToasterAnimation.FadeIn);
                else
                {
                        billDetailsList[index].Count = newCount;
                        //billDetailsList[index].Total = billDetailsList[index].Count * billDetailsList[index].Price;
                        editBillRow(index);
                        //if (billDetailsList[index].Count == 1)
                            refreshDeleteButtonInvoice(index, billDetailsList[index].Count);
                }
            }

            BuildBillDesign();
            await  refreshTotal();
            setKitchenNotification();
            textBox.Focus();
        }
        */
        private async void inviceItemCount_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            try
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = "1";
                }
                else
                {
                    if (int.Parse(textBox.Text) <= 0)
                        textBox.Text = "1";
                }
            }
            catch
            {
                textBox.Text = "1";
            }
            int newCount = int.Parse(textBox.Text);
            int index = int.Parse(textBox.Tag.ToString().Replace("countTextBox-", ""));
            ////////// Plus
            if (billDetailsList[index].Count < newCount)
            {
                billDetailsList[index].Count = newCount;
                //billDetailsList[index].Total = billDetailsList[index].Count * billDetailsList[index].Price;
                editBillRow(index);
                //if (billDetailsList[index].Count == 2)
                refreshDeleteButtonInvoice(index, billDetailsList[index].Count);


            }
            /////////// Minus
            else if (billDetailsList[index].Count > newCount)
            {
                long itemUnitId = billDetailsList[index].itemUnitId;
                long itemTransId = billDetailsList[index].itemsTransId;
                int sentToKitchenCount = sentInvoiceItems.Where(x => x.itemUnitId == itemUnitId && x.itemsTransId == itemTransId).Select(x => x.Count).Sum();
                if (billDetailsList[index].Count <= sentToKitchenCount)
                    Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trItemCannotDelete"), animation: ToasterAnimation.FadeIn);
                else
                {
                    billDetailsList[index].Count = newCount;
                    //billDetailsList[index].Total = billDetailsList[index].Count * billDetailsList[index].Price;
                    editBillRow(index);
                    //if (billDetailsList[index].Count == 1)
                    refreshDeleteButtonInvoice(index, billDetailsList[index].Count);
                }
            }

            await refreshTotal();
            BuildBillDesign();
            setKitchenNotification();
            textBox.Focus();
        }

        #endregion
        #region timer to refresh notifications
        private static DispatcherTimer timer;
        int _DraftCount = 0;
        int _OrderCount = 0;
        public static bool isFromReport = false;
        private void setTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(30);
            timer.Tick += timer_Tick;
            timer.Start();
        }
        private async void timer_Tick(object sendert, EventArgs et)
        {
            try
            {
                await refreshOrdersNotification();
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void setNotifications()
        {
            if (AppSettings.invType == "takeAway" || AppSettings.invType == "selfService")
                refreshDraftNotification();

            refreshOrdersNotification();
        }
        async Task refreshDraftNotification()
        {
            try
            {
                string invoiceType = "";
                invoiceType = "tsd,ssd";

                int duration = 0;
                int hours = 24;
                int ordersCount = await FillCombo.invoice.GetCountByCreator(invoiceType, MainWindow.userLogin.userId, duration, hours);
                if (FillCombo.invoice != null && (_InvoiceType == "tsd" || _InvoiceType == "ssd") && invoice != null && invoice.invoiceId != 0 && !isFromReport)
                    ordersCount--;

                HelpClass.refreshNotification(md_draft, ref _DraftCount, ordersCount);
            }
            catch { }
        }

        async Task refreshOrdersNotification()
        {
            try
            {
                string status = "Ready";
                int duration = 24; //hours
                int ordersCount = await preparingOrder.GetHallOrderCount(status, MainWindow.branchLogin.branchId, duration);

                HelpClass.refreshNotification(md_ordersAlerts, ref _OrderCount, ordersCount);
            }
            catch { }
        }
        #endregion
        #region adddraft - addInvoice - cancleInvoice - clear - table names - fillInvoiceInputs  - refresh items price
        private async Task<long> addDraft()
        {
            long res = 0;
            if (AppSettings.invType == "diningHall")
            {

                if (_InvoiceType == "sd" && selectedTables.Count > 0)
                {
                    res = await addInvoice("sd");
                    refreshOrdersNotification();

                }

            }
            else if (AppSettings.invType == "takeAway")
            {
                //if (billDetailsList.Count > 0)
                //{
                if (invoice.invoiceId == 0)
                {
                    invoice.invNumber = await invoice.generateDialyInvNumber("ssd,ss,tsd,ts,sd,s", MainWindow.branchLogin.branchId);
                }

                res = await addInvoice("tsd");

                refreshOrdersNotification();
                refreshDraftNotification();
                //}

            }
            else if (AppSettings.invType == "selfService")
            {
                if (invoice.invoiceId == 0)
                {
                    //invoice.invNumber = await invoice.generateInvNumber("ssd", MainWindow.branchLogin.code, MainWindow.branchLogin.branchId);
                    invoice.invNumber = await invoice.generateDialyInvNumber("ssd,ss,tsd,ts,sd,s", MainWindow.branchLogin.branchId);

                }
                res = await addInvoice("ssd");

                refreshDraftNotification();
                refreshOrdersNotification();
            }

            #region inv items
            billDetailsList = new ObservableCollection<BillDetailsSales>();
            invoiceItems = await FillCombo.invoice.GetInvoicesItems(res);

            fillInvoiceItems();
            #endregion

            return res;
        }

        async Task<long> addInvoice(string invType)
        {
            try
            {
                #region invoice object
                if (invoice.invoiceId == 0)
                {

                    invoice.createUserId = MainWindow.userLogin.userId;
                    invoice.branchId = MainWindow.branchLogin.branchId;
                    invoice.branchCreatorId = MainWindow.branchLogin.branchId;
                    invoice.posId = MainWindow.posLogin.posId;
                }
                invoice.updateUserId = MainWindow.userLogin.userId;
                invoice.invType = invType;
                invoice.discountValue = _ManualDiscount;
                invoice.discountType = _DiscountType;
                invoice.shippingCostDiscount = _DeliveryDiscount;
                invoice.total = _Sum;
                invoice.totalNet = decimal.Parse(tb_total.Text);
                invoice.paid = 0;
                invoice.deserved = invoice.totalNet;
                if (tb_tax.Text != "")
                    invoice.tax = decimal.Parse(tb_tax.Text);
                else
                    invoice.tax = 0;


                #endregion
                #region items transfer
                invoiceItems = new List<ItemTransfer>();
                ItemTransfer itemT;
                foreach (var item in billDetailsList)
                {
                    #region check item with no extras and edits

                    var it = invoiceItems.Where(x => x.itemUnitId == item.itemUnitId && x.isExtra == false).FirstOrDefault();
                    #endregion
                    if (it != null)
                    {
                        it.quantity += item.Count;
                    }
                    else
                    {
                        itemT = new ItemTransfer();
                        itemT.invoiceId = 0;
                        itemT.itemsTransId = item.itemsTransId;
                        itemT.quantity = item.Count;
                        itemT.price = item.Price;
                        itemT.itemUnitId = item.itemUnitId;
                        itemT.offerId = item.offerId;
                        itemT.offerType = decimal.Parse(item.OfferType);
                        itemT.offerValue = item.OfferValue;
                        itemT.itemTax = item.Tax;
                        itemT.itemUnitPrice = item.basicPrice;
                        itemT.createUserId = MainWindow.userLogin.userId;
                        itemT.forAgents = item.forAgents;
                        itemT.itemsIngredients = item.itemsIngredients;
                        itemT.itemExtras = item.itemExtras;
                        itemT.isExtra = item.isExtra;
                        invoiceItems.Add(itemT);

                    }
                }
                #endregion
                //long res = await FillCombo.invoice.saveInvoiceWithItems(invoice, invoiceItems);
                long res = await FillCombo.invoice.saveSalesInvoice(invoice, invoiceItems);

                invoice.invoiceId = res;
                prinvoiceId = invoice.invoiceId;
                return res;
            }
            catch
            {
                return 0;
            }
        }
        async Task cancleInvoice(string invType)
        {
            try
            {
                invoice.invType = invType;

                var res = await FillCombo.invoice.saveInvoice(invoice);
                if (res > 0)
                {
                    Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopCanceled"), animation: ToasterAnimation.FadeIn);
                    #region update reservation status to cancle
                    if (invoice.reservationId != null)
                    {
                        await reservation.updateReservationStatus((long)invoice.reservationId, "cancle", MainWindow.userLogin.userId);
                    }
                    #endregion
                }
                else
                    Toaster.ShowError(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
            }
            catch
            {

            }
        }
        async Task clear()
        {
            // _InvoiceType = "sd";
            txt_tableName.Text = "";
            billDetailsList.Clear();
            _Sum = 0;
            _DeliveryCost = 0;
            _ManualDiscount = 0;
            _invoiceClassDiscount = 0;
            _DeliveryDiscount = 0;
            _MemberShipId = 0;
            _DiscountType = "";
            selectedCopouns.Clear();
            selectedTables.Clear();

            invoice = new Invoice();

            #region return waiter button to default
            //txt_waiter.Text = AppSettings.resourcemanager.GetString("trWaiter");
            txt_waiter.Text = "";
            txt_waiter.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            path_waiter.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            #endregion
            #region return delivery button to default
            txt_delivery.Text = "";
            txt_delivery.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            path_delivery.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            #endregion
            #region return customer button to default
            //txt_customer.Text = AppSettings.resourcemanager.GetString("trCustomer");
            txt_customer.Text = "";
            txt_customer.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            path_customer.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            #endregion
            #region return discount button to default
            txt_discount.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            path_discount.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            #endregion
            #region return delivery button to default
            //txt_customer.Text = AppSettings.resourcemanager.GetString("trDelivery");
            txt_customer.Text = "";

            txt_delivery.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            path_delivery.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            #endregion
            #region return orderTime button to default
            txt_orderTime.Text = "";
            txt_orderTime.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            path_orderTime.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            #endregion  
            #region return kitchen button to default
            txt_kitchen.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            path_kitchen.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            #endregion 

            #region enable- disable buttons
            btn_delivery.IsEnabled = false;
            #endregion
            //last
            changeInvType();
            #region refresh items with new price
            await refreshItemsList();
            Search();
            refreshItemsPrice();
            #endregion
            await refreshTotal();
            BuildBillDesign();
        }

        private void setTablesName()
        {
            string str = "";
            foreach (Tables tbl in selectedTables)
            {
                if (str == "")
                    str += tbl.name;
                else
                    str += " - " + tbl.name;
            }
            txt_tableName.Text = AppSettings.resourcemanager.GetString("trTables") + ": " + str;
        }
        public async Task fillInvoiceInputs(Invoice invoice)
        {
            #region inv items
            billDetailsList = new ObservableCollection<BillDetailsSales>();
            invoiceItems = await FillCombo.invoice.GetInvoicesItems(invoice.invoiceId);

            fillInvoiceItems();
            setKitchenNotification();
            #endregion

            #region set parameters
            _Sum = (decimal)invoice.total;
            _DeliveryCost = (decimal)invoice.shippingCost;
            try
            {
                _DeliveryDiscount = (decimal)invoice.shippingCostDiscount;
            }
            catch { }
            _ManualDiscount = invoice.discountValue;
            _DiscountType = invoice.discountType;
            selectedCopouns = await FillCombo.invoice.GetInvoiceCoupons(invoice.invoiceId);

            #endregion

            #region get customer memberShip
            if (invoice.agentId != null)
            {
                var customer = FillCombo.customersList.Where(x => x.agentId == invoice.agentId).FirstOrDefault();
                if (customer.membershipId != null)
                {
                    _MemberShipId = (long)customer.membershipId;
                    Memberships memberships = new Memberships();
                    AgenttoPayCash agentToPayCash = new AgenttoPayCash();
                    agentToPayCash = await memberships.GetmembershipStateByAgentId((long)invoice.agentId);
                    if (agentToPayCash.membershipStatus == "valid")
                    {
                        customerInvClasses = await invoiceMemberShipClass.GetInvclassByMembershipId(_MemberShipId);
                    }
                    else
                        customerInvClasses = new List<InvoicesClass>();
                }
                else
                    _MemberShipId = 0;

               
            }
            else
                _MemberShipId = 0;

            await refreshItemsList();
            Search();
            refreshItemsPrice();
            #endregion

            if (AppSettings.invType == "diningHall")
                await fillDiningHallInv();
            else if (AppSettings.invType == "takeAway")
                await fillTakeAwayInv();
            else if (AppSettings.invType == "selfService")
                await fillSelfServiceInv();

        }
        private void fillInvoiceItems()
        {
            foreach (ItemTransfer it in invoiceItems)
            {
                #region isExtra
                itemsTransferIngredients ingredientEdit = null;
                if (it.itemsIngredients.Count != 0)
                    ingredientEdit = it.itemsIngredients.Where(x => x.isActive == 0).FirstOrDefault();
                if (it.itemExtras.Count > 0 || ingredientEdit != null)
                {
                   it.isExtra = true;
                }
                #endregion
                item = items.Where(x => x.itemId == it.itemId).FirstOrDefault();
                if(item != null)
                    addRowToBill(item, it.quantity, it.itemsIngredients,it.itemExtras,it.itemsTransId,it.isExtra,true);
            }
           
        }
        private void refreshItemsPrice()
        {
            var tempBill = billDetailsList.ToList();
            billDetailsList = new ObservableCollection<BillDetailsSales>();
            foreach (BillDetailsSales it in tempBill)
            {
                item = items.Where(x => x.itemId == it.itemId).FirstOrDefault();
                addRowToBill(item, it.Count,it.itemsIngredients, it.itemExtras,it.itemsTransId,it.isExtra,false);
            }
        }
        async Task fillDiningHallInv()
        {
            #region text values and colors

            if (_ManualDiscount > 0 || selectedCopouns.Count > 0)
            {
                txt_discount.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                path_discount.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
            }
            else
            {
                txt_discount.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                path_discount.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            }

            if (invoice.waiterId != null)
            {
                if (FillCombo.usersList == null)
                    await FillCombo.RefreshUsers();
                var user = FillCombo.usersList.Where(x => x.userId == invoice.waiterId).FirstOrDefault();
                txt_waiter.Text = user.name;

                txt_waiter.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                path_waiter.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
            }
            else
            {
                //txt_waiter.Text = AppSettings.resourcemanager.GetString("trWaiter");
                txt_waiter.Text = "";
                txt_waiter.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                path_waiter.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            }

            if (invoice.agentId != null)
            {
                var customer = FillCombo.customersList.Where(x => x.agentId == invoice.agentId).FirstOrDefault();
                if (customer.membershipId != null)
                    _MemberShipId = (long)customer.membershipId;
                else
                    _MemberShipId = 0;
                txt_customer.Text = customer.name;

                txt_customer.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                path_customer.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
            }
            else
            {
                //txt_customer.Text = AppSettings.resourcemanager.GetString("trCustomer");
                txt_customer.Text = "";
                txt_customer.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                path_customer.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            }

            if (invoice.total != 0)
                tb_subtotal.Text = HelpClass.DecTostring(invoice.total);
            else
                tb_subtotal.Text = "0";

            if (invoice.totalNet != 0)
                tb_total.Text = HelpClass.DecTostring(invoice.totalNet);
            else tb_total.Text = "0";

            if (invoice.tax != 0)
                tb_tax.Text = HelpClass.PercentageDecTostring(invoice.tax);
            else
                tb_tax.Text = "0";
            #endregion

            #region invoice items
            BuildBillDesign();

            if (invoiceItems.Count == 0)
                btn_cancel.IsEnabled = true;
            #endregion

            #region items sent to kitchen
            await refreshSentToKitchenItems();
            #endregion

            setKitchenNotification();
        }

        async Task fillTakeAwayInv()
        {

            #region text values and colors

            if (_ManualDiscount > 0 || selectedCopouns.Count > 0)
            {
                txt_discount.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                path_discount.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
            }
            else
            {
                txt_discount.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                path_discount.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            }

            if (invoice.shippingCompanyId != null)
            {
                if (FillCombo.shippingCompaniesList == null)
                    await FillCombo.RefreshShippingCompanies();

                var company = FillCombo.shippingCompaniesList.Where(x => x.shippingCompanyId == invoice.shippingCompanyId).FirstOrDefault();

                txt_delivery.Text = company.name;
                txt_delivery.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                path_delivery.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
            }
            else
            {
                txt_delivery.Text = "";
                txt_delivery.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                path_delivery.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            }

            if (invoice.agentId != null)
            {
                var customer = FillCombo.customersList.Where(x => x.agentId == invoice.agentId).FirstOrDefault();
                if (customer.membershipId != null)
                    _MemberShipId = (long)customer.membershipId;
                else
                    _MemberShipId = 0;
                txt_customer.Text = customer.name;

                txt_customer.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                path_customer.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;

                btn_delivery.IsEnabled = true;
            }
            else
            {
                txt_customer.Text = "";
                txt_customer.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                path_customer.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;

                btn_delivery.IsEnabled = false;
            }

            #region change order time Color and value
            if (invoice.orderTime != null)
            {
                txt_orderTime.Text = invoice.orderTime.ToString().Split(' ')[1] + ' ' + invoice.orderTime.ToString().Split(' ')[2];
                txt_orderTime.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                path_orderTime.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
            }
            else
            {
                txt_orderTime.Text = "";
                txt_orderTime.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                path_orderTime.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            }
            #endregion

            if (invoice.total != 0)
                tb_subtotal.Text = HelpClass.DecTostring(invoice.total);
            else
                tb_subtotal.Text = "0";

            if (invoice.totalNet != 0)
                tb_total.Text = HelpClass.DecTostring(invoice.totalNet);
            else tb_total.Text = "0";

            if (invoice.tax != 0)
                tb_tax.Text = HelpClass.PercentageDecTostring(invoice.tax);
            else
                tb_tax.Text = "0";
            #endregion

            #region invoice items
            BuildBillDesign();

            #endregion
        }
        async Task fillSelfServiceInv()
        {

            #region text values and colors

            if (_ManualDiscount > 0 || selectedCopouns.Count > 0)
            {
                txt_discount.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                path_discount.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
            }
            else
            {
                txt_discount.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                path_discount.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            }

            //if (invoice.shippingCompanyId != null)
            //{
            //    if (FillCombo.shippingCompaniesList == null)
            //        await FillCombo.RefreshShippingCompanies();

            //    var company = FillCombo.shippingCompaniesList.Where(x => x.shippingCompanyId == invoice.shippingCompanyId).FirstOrDefault();

            //    txt_delivery.Text = company.name;
            //    txt_delivery.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
            //    path_delivery.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
            //}
            //else
            //{
            //    txt_delivery.Text = "";
            //    txt_delivery.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            //    path_delivery.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            //}

            if (invoice.agentId != null)
            {
                var customer = FillCombo.customersList.Where(x => x.agentId == invoice.agentId).FirstOrDefault();
                if (customer.membershipId != null)
                    _MemberShipId = (long)customer.membershipId;
                else
                    _MemberShipId = 0;
                txt_customer.Text = customer.name;

                txt_customer.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                path_customer.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;

                btn_delivery.IsEnabled = true;
            }
            else
            {
                txt_customer.Text = "";
                txt_customer.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                path_customer.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;

                btn_delivery.IsEnabled = false;

            }

            #region change order time Color and value
            if (invoice.orderTime != null)
            {
                txt_orderTime.Text = invoice.orderTime.ToString().Split(' ')[1] + ' ' + invoice.orderTime.ToString().Split(' ')[2];
                txt_orderTime.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                path_orderTime.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
            }
            else
            {
                txt_orderTime.Text = "";
                txt_orderTime.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                path_orderTime.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            }
            #endregion

            #region change table Color and value
            if (invoice.tables.Count > 0)
            {
                txt_tableName.Text = invoice.tables[0].name;
            }
            #endregion

            if (invoice.total != 0)
                tb_subtotal.Text = HelpClass.DecTostring(invoice.total);
            else
                tb_subtotal.Text = "0";

            if (invoice.totalNet != 0)
                tb_total.Text = HelpClass.DecTostring(invoice.totalNet);
            else tb_total.Text = "0";

            if (invoice.tax != 0)
                tb_tax.Text = HelpClass.PercentageDecTostring(invoice.tax);
            else
                tb_tax.Text = "0";
            #endregion

            #region invoice items
            BuildBillDesign();

            #endregion
        }

        #endregion
        #region kitchen items
        List<BillDetailsSales> sentInvoiceItems = new List<BillDetailsSales>();
        List<OrderPreparing> kitchenOrders = new List<OrderPreparing>();
        OrderPreparing preparingOrder = new OrderPreparing();
        async Task refreshSentToKitchenItems()
        {
            kitchenOrders = await preparingOrder.GetInvoicePreparingOrders(invoice.invoiceId);

            sentInvoiceItems = new List<BillDetailsSales>();
            int index = 1;
            foreach (ItemTransfer b in invoiceItems)
            {

                int itemCountInOrder = 0;
                try { itemCountInOrder = kitchenOrders.Where(x => x.itemUnitId == b.itemUnitId && x.itemsTransId == b.itemsTransId).Sum(x => x.quantity); }
                catch { }

                bool sentToKitchen = false;
                if (itemCountInOrder > 0)
                    sentToKitchen = true;

                b.sentToKitchen = sentToKitchen;
               
                BillDetailsSales newBillRow = new BillDetailsSales()
                {
                    itemUnitId = (long)b.itemUnitId,
                    itemName = b.itemName,
                    index = index,
                    Count = itemCountInOrder,
                    itemsTransId = b.itemsTransId,
                    sentToKitchen = sentToKitchen,
                };
                index++;
                sentInvoiceItems.Add(newBillRow);
            }

            #region enable cancle button if no items sent to kitchen
            if (kitchenOrders.Count == 0)
                btn_cancel.IsEnabled = true;
            else
                btn_cancel.IsEnabled = false;
            #endregion
        }

        private void setKitchenNotification()
        {
            List<ItemOrderPreparing> preparingItemsList = new List<ItemOrderPreparing>();

            foreach (BillDetailsSales b in billDetailsList)
            {
                var sentItem = sentInvoiceItems.Where(x => x.itemUnitId == b.itemUnitId && x.itemsTransId == b.itemsTransId).FirstOrDefault();
                int sentCount = 0;
                if (sentItem != null)
                {
                    sentCount = sentItem.Count;
                    b.sentToKitchen = true;
                }
                if (b.Count != sentCount)
                {
                    ItemOrderPreparing it = new ItemOrderPreparing()
                    {
                        itemUnitId = b.itemUnitId,
                        quantity = b.Count - sentCount,
                        itemsTransId = b.itemsTransId,
                        createUserId = MainWindow.userLogin.userId,
                    };
                    preparingItemsList.Add(it);
                }


            }
            if (preparingItemsList.Count == 0) // set btn_kitchen as default
            {
                txt_kitchen.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                path_kitchen.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            }
            else
            {
                txt_kitchen.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                path_kitchen.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
            }


            //foreach (ItemTransfer b in invoiceItems)
            //{

            //    int itemCountInOrder = 0;
            //    try { itemCountInOrder = kitchenOrders.Where(x => x.itemUnitId == b.itemUnitId).Sum(x => x.quantity); }
            //    catch { }

            //    long countInInvoiceItems = billDetailsList.Where(x => x.itemUnitId == b.itemUnitId).Sum(x => x.Count);

            //    if(countInInvoiceItems == itemCountInOrder) // set btn_kitchen as default
            //    {
            //        txt_kitchen.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            //        path_kitchen.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            //    }
            //    else
            //    {
            //        txt_kitchen.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
            //        path_kitchen.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
            //        break;
            //    }
            //}
        }
        #endregion
        #region buttons: new - orders - tables - customers - waiter - kitchen 

        private async void Btn_newDraft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                //if (FillCombo.groupObject.HasPermissionAction(invoicePermission, FillCombo.groupObjects, "one"))
                //{
                if (invoice.invoiceId != 0 || billDetailsList.Count > 0)
                    await addDraft();
                await clear();
                //}
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
                string invoiceType = "";

                invoiceType = "tsd, ssd";

                Window.GetWindow(this).Opacity = 0.2;
                wd_invoice w = new wd_invoice();

                w.invoiceType = invoiceType;
                w.userId = MainWindow.userLogin.userId;
                //w.duration = 2; // view drafts which created during 2 last days 
                w.hours = 24; // view drafts which created during 24 hours
                w.icon = "drafts";
                w.page = "takeAway";
                w.title = AppSettings.resourcemanager.GetString("trDrafts");

                if (w.ShowDialog() == true)
                {
                    await clear();
                    if (w.invoice != null)
                    {
                        invoice = w.invoice;
                        _InvoiceType = invoice.invType;
                        if (_InvoiceType == "ssd")
                            AppSettings.invType = "selfService";
                        else
                            AppSettings.invType = "takeAway";
                        changeInvType();
                        isFromReport = false;
                        await fillInvoiceInputs(invoice);
                        setNotifications();

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

        private void Btn_ordersAlerts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                Window.GetWindow(this).Opacity = 0.2;


                wd_ordersReady w = new wd_ordersReady();
                if (AppSettings.invType == "diningHall")
                    w.page = "dinningHall";
                else if (AppSettings.invType == "takeAway")
                    w.page = "takeAway";
                else if (AppSettings.invType == "selfService")
                    w.page = "selfService";
                w.ShowDialog();



                Window.GetWindow(this).Opacity = 1;
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                //if (FillCombo.groupObject.HasPermissionAction(invoicePermission, FillCombo.groupObjects, "one"))
                //{
                await cancleInvoice("sc");
                await clear();
                //}
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Btn_invType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                Window.GetWindow(this).Opacity = 0.2;


                wd_selectInvType w = new wd_selectInvType();

                w.ShowDialog();
                if (w.isOk)
                {
                    changeInvType();

                    #region refresh items with new price
                    await refreshItemsList();
                    Search();
                    refreshItemsPrice();
                    #endregion
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
        private async void Btn_tables_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                // if (FillCombo.groupObject.HasPermissionAction(addTablePermission, FillCombo.groupObjects, "one") || HelpClass.isAdminPermision())
                // {

                Window.GetWindow(this).Opacity = 0.2;

                await addDraft();// save invoice
                if (AppSettings.invType == "diningHall")
                {

                    #region invType = diningHall
                    wd_diningHallTables w = new wd_diningHallTables();
                    w.ShowDialog();
                    if (w.isOk == true)
                    {
                        selectedTables = w.selectedTables;
                        invoice = w.invoice;

                        #region enable btns
                        btn_waiter.IsEnabled = true;
                        btn_discount.IsEnabled = true;
                        btn_customer.IsEnabled = true;
                        btn_kitchen.IsEnabled = true;
                        #endregion

                        setTablesName();
                        await fillInvoiceInputs(invoice);
                    }

                    #endregion
                }
                else if (AppSettings.invType == "selfService")
                {
                    #region self-service
                    wd_selectTable w = new wd_selectTable();
                    //w.tableId = 0;
                    w.ShowDialog();
                    if (w.isOk)
                    {
                        #region table name
                        txt_tableName.Text = w.table.name;
                        #endregion

                        #region update invoice
                        List<Tables> tbl = new List<Tables>();
                        tbl.Add(w.table);
                        var res = await addDraft();

                        if (res > 0)
                        {

                            Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);
                            // set table to invoice
                            await FillCombo.invoice.updateInvoiceTables(invoice.invoiceId, tbl);
                        }
                        else
                            Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                        #endregion

                    }
                    #endregion
                }
                //}
                //else
                //    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                Window.GetWindow(this).Opacity = 1;
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Btn_kitchen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                //if (FillCombo.groupObject.HasPermissionAction(addRangePermission, FillCombo.groupObjects, "one") || HelpClass.isAdminPermision())
                //{
                Window.GetWindow(this).Opacity = 0.2;
                await addDraft();// save invoice

                wd_diningHallKitchen w = new wd_diningHallKitchen();

                w.invoiceItemsList = billDetailsList.ToList();
                w.invoiceId = invoice.invoiceId;
                w.ShowDialog();

                await refreshSentToKitchenItems();
                setKitchenNotification();
                Window.GetWindow(this).Opacity = 1;
                //}
                //else
                //    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Btn_waiter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                //if (FillCombo.groupObject.HasPermissionAction(addRangePermission, FillCombo.groupObjects, "one") || HelpClass.isAdminPermision())
                //{
                Window.GetWindow(this).Opacity = 0.2;
                await addDraft();// save invoice

                wd_selectUser w = new wd_selectUser();
                w.userJob = "waiter";
                if (invoice.waiterId != null)
                    w.userId = (long)invoice.waiterId;
                w.ShowDialog();
                if (w.isOk)
                {
                    invoice.waiterId = w.userId;
                    if (w.userId > 0)
                    {
                        //string userName = FillCombo.usersList.Where(x => x.createUserId == w.userId).Select(x => x.name).Single();
                        string userName = FillCombo.usersList.Where(x => x.userId == w.userId).Select(x => x.name).Single();
                        // change button content
                        txt_waiter.Text = userName;
                        // change foreground color
                        txt_waiter.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                        path_waiter.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;

                    }
                    else
                    {
                        // return button content to default
                        //txt_waiter.Text = AppSettings.resourcemanager.GetString("trWaiter");
                        txt_waiter.Text = "";
                        // return foreground color to default
                        txt_waiter.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                        path_waiter.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                    }
                    long res = await FillCombo.invoice.saveInvoice(invoice);
                    if (res > 0)
                    {
                        //Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);
                    }
                    else
                        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);

                }
                Window.GetWindow(this).Opacity = 1;
                //}
                //else
                //    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Btn_customer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                //if (FillCombo.groupObject.HasPermissionAction(addRangePermission, FillCombo.groupObjects, "one") || HelpClass.isAdminPermision())
                //{
                Window.GetWindow(this).Opacity = 0.2;
                await addDraft();// save invoice


                wd_selectCustomer w = new wd_selectCustomer();
                if (invoice.agentId != null)
                    w.customerId = (long)invoice.agentId;
                w.ShowDialog();
                if (w.isOk)
                {
                    customerInvClasses = new List<InvoicesClass>();

                    if (w.customerId > 0)
                    {
                        if (invoice.agentId != w.customerId)
                        {
                            #region clear prevoius client related values (coupons - delivery - invoiceMemberShipClass)
                            selectedCopouns = new List<CouponInvoice>();
                            _DeliveryCost = 0;
                            invoice.shippingCompanyId = null;
                            invoice.shipUserId = null;
                            invoice.shippingCost = 0;
                            invoice.realShippingCost = 0;
                            invoiceMemberShipClass = new InvoicesClass();

                            await FillCombo.invoice.clearInvoiceCouponsAndClasses(invoice.invoiceId);

                            #region return delivery button to default
                            //txt_customer.Text = "";
                            txt_delivery.Text = "";
                            txt_delivery.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                            path_delivery.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                            #endregion

                            #endregion

                            #region customer membership Info
                            var customer = FillCombo.customersList.Where(x => x.agentId == w.customerId).FirstOrDefault();
                            if (customer.membershipId != null)
                            {
                                _MemberShipId = (long)customer.membershipId;
                                _DeliveryDiscount = w.deliveryDiscount;
                                if (w.memberShipStatus == "valid")
                                {
                                    customerInvClasses = await invoiceMemberShipClass.GetInvclassByMembershipId(_MemberShipId);

                                }
                            }
                            else
                                _MemberShipId = 0;
                        

                            #endregion
                            #region update invoice
                            invoice.agentId = w.customerId;
                            invoice.membershipId = customer.membershipId;
                            await refreshTotal();
                            var res = await addDraft();
                            if (res > 0)
                            {
                                // save invoice memberShip discount class
                                //if (invoiceMemberShipClass.invClassMemberId != 0)
                                //    await FillCombo.invoice.saveMemberShipClassDis(invoiceMemberShipClass, invoice.invoiceId);
                            }
                            else
                                Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                            #endregion
                            // test if id chnage
                            // change button content
                            txt_customer.Text = customer.name.ToString();
                            // change foreground color
                            txt_customer.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                            path_customer.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;

                            btn_delivery.IsEnabled = true;
                         }
                    }
                    else
                    {
                        #region clear prevoius client related values (coupons - delivery - invoiceMemberShipClass)
                        selectedCopouns = new List<CouponInvoice>();
                        _DeliveryCost = 0;

                        invoice.agentId = null;
                        invoice.shippingCompanyId = null;
                        invoice.shipUserId = null;
                        invoice.shippingCost = 0;
                        invoice.realShippingCost = 0;
                        invoice.membershipId = null;
                        invoiceMemberShipClass = new InvoicesClass();

                        await FillCombo.invoice.clearInvoiceCouponsAndClasses(invoice.invoiceId);
                        var res = await addDraft();
                        #endregion
                        // return button content to default
                        _DeliveryDiscount = 0;
                        _MemberShipId = 0;
                        txt_customer.Text = "";
                        // return foreground color to default
                        txt_customer.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                        path_customer.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;

                        #region return delivery button to default
                        txt_delivery.Text = "";
                        txt_delivery.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                        path_delivery.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                        btn_delivery.IsEnabled = false;

                        #endregion

                        await refreshTotal();
                    }

                    #region refresh items with new price
                    await refreshItemsList();
                    await Search();
                    refreshItemsPrice();
                    #endregion
                }
                Window.GetWindow(this).Opacity = 1;
                //}
                //else
                //    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Btn_discount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                //if (FillCombo.groupObject.HasPermissionAction(addRangePermission, FillCombo.groupObjects, "one") || HelpClass.isAdminPermision())
                //{
                Window.GetWindow(this).Opacity = 0.2;
                await addDraft();// save invoice

                wd_selectDiscount w = new wd_selectDiscount();
                w._Sum = _Sum;
                w.selectedCopouns = selectedCopouns;
                w.manualDiscount = _ManualDiscount;
                w.discountType = _DiscountType;
                w.memberShipId = _MemberShipId;
                w.ShowDialog();
                if (w.isOk)
                {
                    _ManualDiscount = w.manualDiscount;
                    _DiscountType = w.discountType;
                    selectedCopouns = w.selectedCopouns;

                    await refreshTotal();
                    #region change button Color
                    if (w.manualDiscount > 0 || w.selectedCopouns.Count > 0)
                    {
                        txt_discount.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                        path_discount.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
                    }
                    else
                    {
                        txt_discount.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                        path_discount.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                    }
                    #endregion
                    #region update invoice
                    //invoice.discountValue = _ManualDiscount;
                    //    invoice.discountType = _DiscountType;

                    //    invoice.total = _Sum;
                    //    invoice.totalNet = decimal.Parse(tb_total.Text);
                    //    invoice.paid = 0;
                    //    invoice.deserved = invoice.totalNet;
                    //    invoice.updateUserId = MainWindow.userLogin.userId;

                    //int res = await FillCombo.invoice.saveInvoice(invoice);
                    var res = await addDraft();
                    if (res > 0)
                    {
                        //Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);
                        await FillCombo.invoice.saveInvoiceCoupons(selectedCopouns, invoice.invoiceId, "sd");
                    }
                    else
                        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                    #endregion
                }
                Window.GetWindow(this).Opacity = 1;
                //}
                //else
                //    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Btn_delivery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                Window.GetWindow(this).Opacity = 0.2;
                await addDraft();// save invoice

                wd_selectDelivery w = new wd_selectDelivery();
                w.customerId = invoice.agentId;
                w.shippingCompanyId = invoice.shippingCompanyId;
                w.shippingUserId = invoice.shipUserId;
                w.ShowDialog();
                if (w.isOk)
                {
                    long? shippingCompanyId = null;
                    long? shipUserId = null;
                    decimal realDeliveryCost = 0;
                    if (w.shippingCompanyId > 0)
                    {
                        shippingCompanyId = w.shippingCompanyId;
                        shipUserId = w.shippingUserId;
                        realDeliveryCost = w._RealDeliveryCost;
                        _DeliveryCost = w._DeliveryCost;


                        //await  refreshTotal();
                        #region change button Color
                        var company = FillCombo.shippingCompaniesList.Where(x => x.shippingCompanyId == w.shippingCompanyId).FirstOrDefault();

                        txt_delivery.Text = company.name;
                        txt_delivery.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                        path_delivery.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;

                        #endregion

                        
                    }
                    else
                    {
                        _DeliveryCost = 0;
                        #region change button Color
                        txt_delivery.Text = "";
                        txt_delivery.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                        path_delivery.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                        #endregion
                    }
                    await refreshTotal();

                    #region update invoice
                    invoice.shippingCompanyId = shippingCompanyId;
                    invoice.shipUserId = shipUserId;
                    invoice.shippingCost = _DeliveryCost;
                    invoice.realShippingCost = realDeliveryCost;

                    var res = await addDraft();

                    if (res > 0)
                    {
                        //await FillCombo.invoice.saveInvoiceCoupons(selectedCopouns, invoice.invoiceId, "sd");
                    }
                    else
                        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                    #endregion
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
        private async void Btn_orderTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                Window.GetWindow(this).Opacity = 0.2;


                wd_selectTime w = new wd_selectTime();
                if (invoice.orderTime != null)
                    w.orderTime = (DateTime)invoice.orderTime;
                w.ShowDialog();
                if (w.isOk)
                {
                    #region change button Color
                    txt_orderTime.Text = w.orderTime.ToString().Split(' ')[1] + ' ' + w.orderTime.ToString().Split(' ')[2];
                    txt_orderTime.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                    path_orderTime.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
                    #endregion

                    #region update invoice
                    invoice.orderTime = w.orderTime;

                    var res = await addDraft();

                    if (res > 0)
                    {
                        //Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);
                    }
                    else
                        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                    #endregion

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

        #endregion
        #region PAY
        List<CashTransfer> paymentsList = new List<CashTransfer>();
        private async Task<bool> validateInvoiceValues()
        {
            #region check total
            if (decimal.Parse(tb_subtotal.Text) == 0)
            {
                Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trErrorTotalIsZeroToolTip"), animation: ToasterAnimation.FadeIn);
                return false;
            }
            #endregion
            #region check discount value 
            decimal validDiscountValue = HelpClass.calcPercentage(_Sum, AppSettings.maxDiscount);
            if (decimal.Parse(tb_totalDiscount.Text) > validDiscountValue)
            {
                Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trErrorMaxDiscountToolTip"), animation: ToasterAnimation.FadeIn);

                return false;
            }
            #endregion
            return true;
        }

        private decimal getCusAvailableBlnc(Agent customer)
        {
            decimal maxCredit = 0;

            decimal customerBalance = customer.balance;

            if (customer.balanceType == 0)
                maxCredit = customer.maxDeserve  + (decimal)customerBalance;
            else
            {
                maxCredit = customer.maxDeserve - (decimal)customerBalance;
                if (maxCredit < 0)
                    maxCredit = 0;
            }
            return maxCredit;
        }
        private async Task saveConfiguredCashTrans(CashTransfer cashTransfer)
        {
            switch (cashTransfer.processType)
            {
                case "cash":// cash: update pos balance   
                    MainWindow.posLogin.balance += invoice.totalNet;
                    await MainWindow.posLogin.save(MainWindow.posLogin);

                    cashTransfer.transType = "d"; //deposit
                    cashTransfer.posId = MainWindow.posLogin.posId;
                    cashTransfer.agentId = invoice.agentId;
                    cashTransfer.invId = invoice.invoiceId;
                    cashTransfer.transNum = await cashTransfer.generateCashNumber("dc");
                    cashTransfer.side = "c"; // customer                    
                    cashTransfer.createUserId = MainWindow.userLogin.userId;
                    await cashTransfer.Save(cashTransfer); //add cash transfer   
                    break;
                case "balance":// balance: update customer balance
                               //if (cb_company.SelectedIndex != -1 && companyModel.deliveryType.Equals("com"))
                               //    await invoice.recordComSpecificPaidCash(invoice, "si", cashTransfer);
                               //else
                    await invoice.recordConfiguredAgentCash(invoice, "si", cashTransfer);
                    break;
                case "card": // card
                    cashTransfer.transType = "d"; //deposit
                    cashTransfer.posId = MainWindow.posLogin.posId;
                    cashTransfer.agentId = invoice.agentId;
                    cashTransfer.invId = invoice.invoiceId;
                    cashTransfer.transNum = await cashTransfer.generateCashNumber("dc");
                    cashTransfer.side = "c"; // customer
                    cashTransfer.createUserId = MainWindow.userLogin.userId;
                    await cashTransfer.Save(cashTransfer); //add cash transfer  
                    break;
                case "admin": // admin
                    cashTransfer.transType = "d"; //deposit
                    cashTransfer.posId = MainWindow.posLogin.posId;
                    cashTransfer.agentId = invoice.agentId;
                    cashTransfer.invId = invoice.invoiceId;
                    cashTransfer.transNum = await cashTransfer.generateCashNumber("dc");
                    cashTransfer.side = "c"; // customer
                    cashTransfer.createUserId = MainWindow.userLogin.userId;
                    await cashTransfer.Save(cashTransfer); //add cash transfer  
                    break;
            }
        }
        private async void Btn_pay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                //if (FillCombo.groupObject.HasPermissionAction(invoicePermission, FillCombo.groupObjects, "one"))
                //{
                if (MainWindow.posLogin.boxState == "o") // box is open
                {
                    if (await validateInvoiceValues())
                    {
                        await refreshTotal();
                        bool multipleValid = true;
                        decimal remain = 0;

                        #region payment window
                        if (invoice.shippingCompanyId == null)// no shipping
                        {
                            Window.GetWindow(this).Opacity = 0.2;

                            wd_multiplePayment w = new wd_multiplePayment();
                            w.isPurchase = false;
                            w.windowOfSourceName = this.Tag.ToString();
                            //w.invoice = new Invoice();
                            w.invoice.invType = _InvoiceType;
                            w.invoice.totalNet = decimal.Parse(tb_total.Text);
                            w.cards = FillCombo.cardsList;
                            //w.invoice = invoice;

                            #region customer balance
                            if (invoice.agentId != null)
                            {
                                await FillCombo.RefreshCustomers();

                                var customer = FillCombo.customersList.ToList().Find(b => b.agentId == (long)invoice.agentId && b.isLimited == true);
                                if (customer != null)
                                {
                                    if (customer.isLimited)
                                    {
                                        decimal maxCredit = 0;
                                        if (customer.maxDeserve != 0)
                                            maxCredit = getCusAvailableBlnc(customer);
                                        w.agent = customer;
                                        w.hasCredit = true;
                                        w.maxCredit = maxCredit;
                                    }
                                    else
                                    {
                                        w.hasCredit = false;
                                        w.maxCredit = 0;
                                    }
                                }
                                else
                                {
                                    w.hasCredit = false;
                                    w.maxCredit = 0;
                                }
                            }
                            #endregion
                            w.ShowDialog();
                            Window.GetWindow(this).Opacity = 1;
                            multipleValid = w.isOk;
                            paymentsList = w.listPayments;
                            remain = w.theRemine;
                            #endregion
                        }
                        if (multipleValid)
                        {
                            invoice.cashReturn = remain;
                            invoice.invDate = DateTime.Now;

                            if (AppSettings.invType == "diningHall")
                            {
                                invoice.invBarcode = invoice.genarateSaleInvBarcode(MainWindow.branchLogin.code, invoice.invNumber);

                                await saveDiningHallInvoice("s");
                            }
                            else if (AppSettings.invType == "takeAway")
                            {
                                if (invoice.invoiceId == 0)
                                {
                                    invoice.invNumber = await invoice.generateDialyInvNumber("ssd,ss,tsd,ts,sd,s", MainWindow.branchLogin.branchId);
                                }
                                invoice.invBarcode = invoice.genarateSaleInvBarcode(MainWindow.branchLogin.code, invoice.invNumber);
                                await saveTakeAwayInvoice("ts");
                            }
                            else if (AppSettings.invType == "selfService")
                            {
                                if (invoice.invoiceId == 0)
                                {
                                    invoice.invNumber = await invoice.generateDialyInvNumber("ssd,ss,tsd,ts,sd,s", MainWindow.branchLogin.branchId);
                                }
                                invoice.invBarcode = invoice.genarateSaleInvBarcode(MainWindow.branchLogin.code, invoice.invNumber);
                                await saveTakeAwayInvoice("ss");
                            }
                            /// print

                            {
                                await refreshItemsList();
                                Search();
                                refreshItemsPrice();
                            }
                            ///print 
                            #region print
                            //thread  
                            if (prinvoiceId > 0)
                            {
                                //Thread t1 = new Thread(async () =>
                                //{
                                    prInvoice = await invoiceModel.GetByInvoiceId(prinvoiceId);
                                if (prInvoice.invType == "s" || prInvoice.invType == "ss" || prInvoice.invType == "ts")
                                {

                                    if (AppSettings.print_on_save_sale == "1")
                                    {
                                        // printInvoice();
                                        //Thread t1 = new Thread(async() =>
                                        //{
                                           await  printInvoice(prinvoiceId);
                                        //});
                                        //t1.Start();
                                    }
                                    if (AppSettings.print_kitchen_on_sale == "1" && (prInvoice.invType == "ss" || prInvoice.invType == "ts"))
                                    {
                                        prOrderPreparingList = await preparingOrder.GetOrdersforPrintByInvoiceId(prinvoiceId);

                                        // printInvoice();
                                       // Thread t2 = new Thread(() =>
                                       //{

                                           printInvoiceInkitchen(prinvoiceId, prOrderPreparingList);
                                       //});
                                       // t2.Start();
                                    }
                                    if (AppSettings.email_on_save_sale == "1")
                                    {
                                        //sendsaleEmail();
                                        //Thread t3 = new Thread(() =>
                                        //{
                                            sendsaleEmail(prinvoiceId);
                                        //});
                                        //t3.Start();
                                    }
                                }
                            //});
                            //t1.Start();
                        }
                            #endregion
                        }
                        paymentsList = new List<CashTransfer>();
                    }
                }
                else //box is closed
                {
                    Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trBoxIsClosed"), animation: ToasterAnimation.FadeIn);
                }
                //}
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        #endregion
        #region save invoice according to invType
        private async Task saveDiningHallInvoice(string invType)
        {
            var res = await addInvoice(invType);
            if (res > 0)
            {
                Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);

                #region savepayment

                await FillCombo.invoice.recordPosCashTransfer(invoice, "si");

                await savePayments();

                // refresh pos balance
                await MainWindow.refreshBalance();
                #endregion
                //await FillCombo.invoice.saveInvoiceCoupons(selectedCopouns, invoice.invoiceId, "s");
                #region close reservation
                if (invoice.reservationId != null)
                    await reservation.updateReservationStatus((long)invoice.reservationId, "close", MainWindow.userLogin.userId);
                #endregion

                #region send orders to kitchen
                await sendOrdersToKitchen();
                #endregion
                await clear();
            }
            else
                Toaster.ShowError(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
        }

        private async Task saveTakeAwayInvoice(string invType)
        {


            var res = await addInvoice(invType);
            if (res > 0)
            {
                Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);

                invoice.invoiceId = res;

                #region send orders to kitchen
                await saveOrdersPreparing();
                #endregion

                #region savepayment 
                await FillCombo.invoice.recordPosCashTransfer(invoice, "si");
                if (invoice.shippingCompanyId == null || (invoice.shippingCompanyId != null && invoice.shipUserId == null)) //if no shipping company
                {
                    await savePayments();
                }
                // refresh pos balance
                await MainWindow.refreshBalance();
                #endregion

                await clear();
                refreshDraftNotification();
            }
            else
                Toaster.ShowError(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
        }
        async Task saveOrdersPreparing()
        {
            #region order Items

            List<ItemOrderPreparing> preparingItemsList = new List<ItemOrderPreparing>();

            #region inv items
            billDetailsList = new ObservableCollection<BillDetailsSales>();
            invoiceItems = await FillCombo.invoice.GetInvoicesItems(invoice.invoiceId);

            fillInvoiceItems();
            #endregion

            foreach (BillDetailsSales b in billDetailsList)
            {
                ItemOrderPreparing it = new ItemOrderPreparing()
                {
                    itemUnitId = b.itemUnitId,
                    quantity = b.Count,
                    itemsTransId = b.itemsTransId,
                    createUserId = MainWindow.userLogin.userId,
                };
                preparingItemsList.Add(it);
            }
            #endregion

            #region preparing order object
            preparingOrder = new OrderPreparing();
            preparingOrder.invoiceId = invoice.invoiceId;
            preparingOrder.preparingTime = 0;
            preparingOrder.createUserId = MainWindow.userLogin.userId;
            #endregion

            #region order status object
            orderPreparingStatus statusObject = new orderPreparingStatus();
            statusObject.status = "Listed";
            statusObject.createUserId = MainWindow.userLogin.userId;
            #endregion

            long res = await preparingOrder.savePreparingOrders(preparingOrder, preparingItemsList, statusObject, MainWindow.branchLogin.branchId, AppSettings.statusesOfPreparingOrder);

          
        }

        async Task sendOrdersToKitchen()
        {
            #region  print parameters
            ReportCls reportclass = new ReportCls();
            clsReports clsreport = new clsReports();
            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            List<OrderPreparing> prOrderPreparingList = new List<OrderPreparing>();
            List<OrderPreparing> OrderListBeforesave = new List<OrderPreparing>();
            List<OrderPreparing> OrderListAftersave = new List<OrderPreparing>();
            #endregion

            #region order Items

            List<ItemOrderPreparing> preparingItemsList = new List<ItemOrderPreparing>();

            #region inv items
            billDetailsList = new ObservableCollection<BillDetailsSales>();
            invoiceItems = await FillCombo.invoice.GetInvoicesItems(invoice.invoiceId);

            fillInvoiceItems();
            #endregion
            foreach (BillDetailsSales b in billDetailsList)
            {
                var sentItem = sentInvoiceItems.Where(x => x.itemUnitId == b.itemUnitId && x.itemsTransId == b.itemsTransId).FirstOrDefault();
                int sentCount = 0;
                if (sentItem != null)
                {
                    sentCount = sentItem.Count;
                }
                if (b.Count != sentCount)
                {
                    ItemOrderPreparing it = new ItemOrderPreparing()
                    {
                        itemUnitId = b.itemUnitId,
                        quantity = b.Count - sentCount,
                        createUserId = MainWindow.userLogin.userId,
                        itemsTransId = b.itemsTransId,
                    };
                    preparingItemsList.Add(it);
                }
            }
            #endregion

            if (preparingItemsList.Count > 0)
            {
                #region preparing order object
                preparingOrder = new OrderPreparing();
                preparingOrder.invoiceId = invoice.invoiceId;
                preparingOrder.preparingTime = 0;
                preparingOrder.createUserId = MainWindow.userLogin.userId;
                #endregion

                #region order status object
                orderPreparingStatus statusObject = new orderPreparingStatus();
                statusObject.status = "Listed";
                statusObject.createUserId = MainWindow.userLogin.userId;
                #endregion
                // OrderListbeforesave
                prinvoiceId = invoice.invoiceId;
                if (AppSettings.print_kitchen_on_sale == "1")
                {
                    if (invoice.invoiceId > 0)
                    {
                        prinvoiceId = invoice.invoiceId;
                        OrderListBeforesave = await preparingOrder.GetOrdersforPrintByInvoiceId(prinvoiceId);

                    }
                }
                var res = await preparingOrder.savePreparingOrders(preparingOrder, preparingItemsList, statusObject, MainWindow.branchLogin.branchId, AppSettings.statusesOfPreparingOrder);
                
                //list after  save
                if (AppSettings.print_kitchen_on_sale == "1")
                {

                    OrderListAftersave = await preparingOrder.GetOrdersforPrintByInvoiceId(prinvoiceId);

                    prOrderPreparingList = clsreport.newKitchenorderList(OrderListBeforesave, OrderListAftersave);

                    //print

                    // prOrderPreparingList = await preparingOrder.GetOrdersByInvoiceId(prinvoiceId);

                    //Thread t2 = new Thread(() =>
                    //{
                      //  send all
                       printInvoiceInkitchen(prinvoiceId, prOrderPreparingList);
                    //});
                    //t2.Start();
                }
            }

        }

        async Task savePayments()
        {

            foreach (var item in paymentsList)
            {
                await saveConfiguredCashTrans(item);
                // yasin code
                if (item.processType != "balance")
                {
                    invoice.paid += item.cash;
                    invoice.deserved -= item.cash;
                }
            }
            prinvoiceId = await invoice.saveInvoice(invoice);

        }
        #endregion
        #region report

        // for report
        ReportCls reportclass = new ReportCls();
        LocalReport rep = new LocalReport();
        ItemUnitOffer offer = new ItemUnitOffer();
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        public static int width;
        public static int itemscount;
        public static int height;
        Invoice prInvoice = new Invoice();

        User userModel = new User();
        Branch branchModel = new Branch();
        Invoice invoiceModel = new Invoice();
        List<PayedInvclass> mailpayedList = new List<PayedInvclass>();
        //shipping
        ShippingCompanies shippingcomp = new ShippingCompanies();
        User shipinguser = new User();
        List<OrderPreparing> prOrderPreparingList = new List<OrderPreparing>();

        /*
         * 
          clsReports classreport = new clsReports();
                        resultmessage resmsg = new resultmessage();
                        resmsg = await classreport.pdfSaleInvoice(row.invoiceId, "pdf");
                        if (resmsg.result != "")
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString(resmsg.result), animation: ToasterAnimation.FadeIn);

                            });
                        }
         * */
        public async Task<string> SaveSalepdf(long invoiceId)
        {
            //for email
            string pdfpath = "";
            string folderpath = reportclass.PathUp(System.IO.Directory.GetCurrentDirectory(), 2, pdfpath) + @"\Thumb\report\";
            ReportCls.clearFolder(folderpath);

            pdfpath = @"\Thumb\report\File" + DateTime.Now.ToFileTime().ToString() + ".pdf";
            pdfpath = reportclass.PathUp(System.IO.Directory.GetCurrentDirectory(), 2, pdfpath);

            clsReports classreport = new clsReports();
            resultmessage resmsg = new resultmessage();
            resmsg = await classreport.pdfSaleInvoice(invoiceId, "emailpdf");

            if (resmsg.result != "")
            {
                this.Dispatcher.Invoke(() =>
                {
                    Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString(resmsg.result), animation: ToasterAnimation.FadeIn);

                });
            }
            else
            {
                resmsg.rep.Refresh();
                if (AppSettings.salePaperSize != "A4")
                {
                    LocalReportExtensions.customExportToPDF(resmsg.rep, pdfpath, resmsg.width, resmsg.height);
                }
                else
                {
                    LocalReportExtensions.ExportToPDF(resmsg.rep, pdfpath);
                }
                // LocalReportExtensions.ExportToPDF(resmsg.rep, pdfpath);
            }

            return pdfpath;




        }
        //public async Task<string> SaveSalepdf()
        //{
        //    //for email
        //    List<ReportParameter> paramarr = new List<ReportParameter>();
        //    string pdfpath = "";

        //    //
        //    if (prInvoice.invType == "pd" || prInvoice.invType == "sd" || prInvoice.invType == "qd"
        //                               || prInvoice.invType == "sbd" || prInvoice.invType == "pbd"
        //                               || prInvoice.invType == "ord" || prInvoice.invType == "imd" || prInvoice.invType == "exd")
        //    {
        //        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPrintDraftInvoice"), animation: ToasterAnimation.FadeIn);
        //    }
        //    else
        //    {


        //        if (prInvoice.invoiceId > 0)
        //        {
        //            ////////////////////////
        //            string folderpath = reportclass.PathUp(System.IO.Directory.GetCurrentDirectory(), 2, pdfpath) + @"\Thumb\report\";
        //            ReportCls.clearFolder(folderpath);

        //            pdfpath = @"\Thumb\report\File" + DateTime.Now.ToFileTime().ToString() + ".pdf";
        //            pdfpath = reportclass.PathUp(System.IO.Directory.GetCurrentDirectory(), 2, pdfpath);

        //            User employ = new User();
        //            employ = await userModel.getUserById((long)prInvoice.updateUserId);
        //            prInvoice.uuserName = employ.name;
        //            prInvoice.uuserLast = employ.lastname;

        //            if (prInvoice.agentId != null)
        //            {
        //                Agent agentinv = new Agent();

        //                //  agentinv = customerInvClasses.Where(X => X.agentId == prInvoice.agentId).FirstOrDefault();

        //                agentinv = await agentinv.getAgentById((long)prInvoice.agentId);
        //                prInvoice.agentCode = agentinv.code;
        //                //new lines
        //                prInvoice.agentName = agentinv.name;
        //                prInvoice.agentCompany = agentinv.company;
        //            }
        //            else
        //            {
        //                prInvoice.agentCode = "-";
        //                prInvoice.agentName = "-";
        //                prInvoice.agentCompany = "-";
        //            }
        //            string reppath = reportclass.GetreceiptInvoiceRdlcpath(prInvoice, 0);
        //            ReportCls.checkLang();
        //            Branch branch = new Branch();
        //            branch = await branchModel.getBranchById((long)prInvoice.branchCreatorId);
        //            if (branch.branchId > 0)
        //            {
        //                prInvoice.branchName = branch.name;
        //            }

        //            decimal totaltax = 0;
        //            foreach (var i in invoiceItems)
        //            {
        //                i.price = decimal.Parse(HelpClass.DecTostring(i.price));
        //                if (i.itemTax != null)
        //                {
        //                    totaltax += (decimal)i.itemTax;

        //                }

        //                i.subTotal = decimal.Parse(HelpClass.DecTostring(i.price * i.quantity));
        //            }
        //            if (totaltax > 0 && prInvoice.invType != "sbd" && prInvoice.invType != "sb")
        //            {
        //                paramarr.Add(new ReportParameter("itemtax_note", AppSettings.itemtax_note.Trim()));
        //                paramarr.Add(new ReportParameter("hasItemTax", "1"));

        //            }
        //            else
        //            {
        //                // paramarr.Add(new ReportParameter("itemtax_note", AppSettings.itemtax_note.Trim()));
        //                paramarr.Add(new ReportParameter("hasItemTax", "0"));
        //            }


        //            clsReports.purchaseInvoiceReport(invoiceItems, rep, reppath);
        //            clsReports.setReportLanguage(paramarr);
        //            clsReports.Header(paramarr);
        //            paramarr.Add(new ReportParameter("isSaved", "y"));
        //            paramarr = reportclass.fillSaleInvReport(prInvoice, paramarr, shippingcomp);

        //            // multiplePaytable(paramarr);
        //            if ((prInvoice.invType == "s" || prInvoice.invType == "sd" || prInvoice.invType == "sbd" || prInvoice.invType == "sb"))
        //            {
        //                CashTransfer cachModel = new CashTransfer();
        //                List<PayedInvclass> payedList = new List<PayedInvclass>();
        //                payedList = await cachModel.GetPayedByInvId(prInvoice.invoiceId);
        //                mailpayedList = payedList;
        //                decimal sump = payedList.Sum(x => x.cash);
        //                decimal deservd = (decimal)prInvoice.totalNet - sump;
        //                //convertter
        //                foreach (var p in payedList)
        //                {
        //                    p.cash = decimal.Parse(reportclass.DecTostring(p.cash));
        //                }
        //                paramarr.Add(new ReportParameter("cashTr", AppSettings.resourcemanagerreport.GetString("trCashType")));

        //                paramarr.Add(new ReportParameter("sumP", reportclass.DecTostring(sump)));
        //                paramarr.Add(new ReportParameter("deserved", reportclass.DecTostring(deservd)));
        //                rep.DataSources.Add(new ReportDataSource("DataSetPayedInvclass", payedList));


        //            }
        //            //if (invoice.invType == "s" )
        //            //{
        //            //    CashTransfer cachModel = new CashTransfer();
        //            //    List<PayedInvclass> payedList = new List<PayedInvclass>();
        //            //    payedList = await cachModel.GetPayedByInvId(invoice.invoiceId);
        //            //    rep.DataSources.Add(new ReportDataSource("DataSetPayedInvclass", payedList));
        //            //}


        //            rep.SetParameters(paramarr);
        //            rep.Refresh();

        //            LocalReportExtensions.ExportToPDF(rep, pdfpath);

        //        }

        //    }
        //    return pdfpath;
        //}

        private async void Btn_pdf_Click(object sender, RoutedEventArgs e)
        {//pdf
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);

                /////////////////////////////////////
                //Thread t1 = new Thread(() =>
                //{
                //    await pdfPurInvoice(invoice.invoiceId);

                clsReports classrep = new clsReports();
                resultmessage resmsg = new resultmessage();
                resmsg = await classrep.pdfSaleInvoice(invoice.invoiceId, "pdf");
                if (resmsg.result != "")
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString(resmsg.result), animation: ToasterAnimation.FadeIn);

                    });
                }
                //});
                //t1.Start();
                //////////////////////////////////////
                if (sender != null)
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        public async Task pdfPurInvoice(long invoiceId)
        {
            try
            {

                Invoice prInvoice = new Invoice();

                List<ItemTransfer> invoiceItems = new List<ItemTransfer>();
                int itemscount;
                Invoice invoiceModel = new Invoice();
                ReportCls reportclass = new ReportCls();
                reportsize rs = new reportsize();
                LocalReport rep = new LocalReport();
                rs.rep = rep;
                //if (prinvoiceId != 0)
                //    prInvoice = await invoiceModel.GetByInvoiceId(prinvoiceId);
                //else
                prInvoice = await invoiceModel.GetByInvoiceId(invoiceId);

                ///
                if (int.Parse(AppSettings.Allow_print_inv_count) <= prInvoice.printedcount)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trYouExceedLimit"), animation: ToasterAnimation.FadeIn);

                    });
                }
                else
                {


                    //if (prInvoice.invType == "pd" || prInvoice.invType == "sd"
                    //         || prInvoice.invType == "sbd" || prInvoice.invType == "pbd" ||
                    //         prInvoice.invType == "ssd" || prInvoice.invType == "tsd")
                    //{
                    //    this.Dispatcher.Invoke(() =>
                    //    {
                    //        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPrintDraftInvoice"), animation: ToasterAnimation.FadeIn);
                    //    });
                    //}
                    //else
                    {
                        //////////////////////////////////////////
                        List<ReportParameter> paramarr = new List<ReportParameter>();


                        if (prInvoice.invoiceId > 0)
                        {
                            #region fill invoice data

                            //items
                            invoiceItems = await invoiceModel.GetInvoicesItems(prInvoice.invoiceId);
                            itemscount = invoiceItems.Count();

                            rs = reportclass.GetreceiptInvoiceRdlcpath(prInvoice, 1, AppSettings.salePaperSize, itemscount, rs.rep);
                            //rs.rep;
                            //rs.width;
                            //rs.height;
                            //user


                            User employ = new User();
                            if (FillCombo.usersList != null)
                            {
                                employ = FillCombo.usersList.Where(X => X.userId == (long)prInvoice.updateUserId).FirstOrDefault();
                            }
                            else
                            {
                                employ = await employ.getUserById((long)prInvoice.updateUserId);
                            }

                            prInvoice.uuserName = employ.name;
                            prInvoice.uuserLast = employ.lastname;
                            //agent
                            if (prInvoice.agentId != null)
                            {
                                Agent agentinv = new Agent();

                                // agentinv = customers.Where(X => X.agentId == prInvoice.agentId).FirstOrDefault();

                                if (FillCombo.customersList != null)
                                {
                                    agentinv = FillCombo.customersList.Where(X => X.agentId == (long)prInvoice.agentId).FirstOrDefault();
                                }
                                else
                                {
                                    agentinv = await agentinv.getAgentById((long)prInvoice.agentId);
                                }

                                prInvoice.agentCode = agentinv.code;
                                //new lines
                                prInvoice.agentName = agentinv.name;
                                prInvoice.agentCompany = agentinv.company;

                            }
                            else
                            {
                                prInvoice.agentCode = "-";
                                prInvoice.agentName = "-";
                                prInvoice.agentCompany = "-";
                            }
                            //branch
                            Branch branch = new Branch();
                            if (FillCombo.branchsList != null)
                            {
                                branch = FillCombo.branchsList.Where(X => X.branchId == (long)prInvoice.branchCreatorId).FirstOrDefault();

                            }
                            else
                            {
                                branch = await branch.getBranchById((long)prInvoice.branchCreatorId);
                            }

                            if (branch.branchId > 0)
                            {
                                prInvoice.branchName = branch.name;
                            }

                            ReportCls.checkLang();
                            //shipping
                            ShippingCompanies shippingcom = new ShippingCompanies();

                            if (prInvoice.shippingCompanyId > 0)
                            {
                                if (FillCombo.shippingCompaniesList != null)
                                {
                                    shippingcom = FillCombo.shippingCompaniesList.Where(X => X.shippingCompanyId == (long)prInvoice.shippingCompanyId).FirstOrDefault();

                                }
                                else
                                {
                                    shippingcom = await shippingcom.GetByID((long)prInvoice.shippingCompanyId);
                                }

                            }
                            User shipuser = new User();
                            if (prInvoice.shipUserId > 0)
                            {
                                shipuser = await shipuser.getUserById((long)prInvoice.shipUserId);
                            }
                            prInvoice.shipUserName = shipuser.name + " " + shipuser.lastname;
                            //end shipping
                            //items subTotal & itemTax
                            decimal totaltax = 0;
                            foreach (var i in invoiceItems)
                            {
                                i.price = decimal.Parse(HelpClass.DecTostring(i.price));
                                if (i.itemTax != null)
                                {
                                    totaltax += (decimal)i.itemTax;

                                }
                                i.subTotal = decimal.Parse(HelpClass.DecTostring(i.price * i.quantity));

                            }

                            if (totaltax > 0 && prInvoice.invType != "sbd" && prInvoice.invType != "sb")
                            {
                                paramarr.Add(new ReportParameter("itemtax_note", AppSettings.itemtax_note.Trim()));
                                paramarr.Add(new ReportParameter("hasItemTax", "1"));

                            }
                            else
                            {
                                // paramarr.Add(new ReportParameter("itemtax_note", AppSettings.itemtax_note.Trim()));
                                paramarr.Add(new ReportParameter("hasItemTax", "0"));
                            }
                            //

                            clsReports.purchaseInvoiceReport(invoiceItems, rs.rep, rs.rep.ReportPath);

                            clsReports.setReportLanguage(paramarr);
                            clsReports.Header(paramarr);
                            paramarr.Add(new ReportParameter("isSaved", "y"));
                            paramarr = reportclass.fillSaleInvReport(prInvoice, paramarr, shippingcom);
                            //   multiplePaytable(paramarr);
                            // payment methods

                            if (prInvoice.invType == "s" || prInvoice.invType == "sd" || prInvoice.invType == "sbd" || prInvoice.invType == "sb"
                                || prInvoice.invType == "ts" || prInvoice.invType == "ss" || prInvoice.invType == "tsd" || prInvoice.invType == "ssd"
                                )
                            {
                                CashTransfer cachModel = new CashTransfer();
                                List<PayedInvclass> payedList = new List<PayedInvclass>();
                                payedList = await cachModel.GetPayedByInvId(prInvoice.invoiceId);
                                mailpayedList = payedList;
                                decimal sump = payedList.Sum(x => x.cash);
                                decimal deservd = (decimal)prInvoice.totalNet - sump;
                                //convertter
                                foreach (var p in payedList)
                                {
                                    p.cash = decimal.Parse(reportclass.DecTostring(p.cash));
                                }
                                paramarr.Add(new ReportParameter("cashTr", AppSettings.resourcemanagerreport.GetString("trCashType")));

                                paramarr.Add(new ReportParameter("sumP", reportclass.DecTostring(sump)));
                                paramarr.Add(new ReportParameter("deserved", reportclass.DecTostring(deservd)));
                                rs.rep.DataSources.Add(new ReportDataSource("DataSetPayedInvclass", payedList));


                            }


                            rs.rep.SetParameters(paramarr);

                            rs.rep.Refresh();
                            #endregion
                            /////////////////////////////////////////////////////////
                            saveFileDialog.Filter = "PDF|*.pdf;";
                            bool? savdialog = false;
                            this.Dispatcher.Invoke(() =>
                            {
                                savdialog = saveFileDialog.ShowDialog();

                            });


                            if (savdialog == true)
                            {

                                string filepath = saveFileDialog.FileName;

                                //copy count
                                if (prInvoice.invType == "s" || prInvoice.invType == "sb" || prInvoice.invType == "p"
                                    || prInvoice.invType == "pb"
                                    || prInvoice.invType == "ss" || prInvoice.invType == "ts")
                                {

                                    paramarr.Add(new ReportParameter("isOrginal", false.ToString()));


                                    //if (i > 1)
                                    //{
                                    //    // update paramarr->isOrginal
                                    //    foreach (var item in paramarr.Where(x => x.Name == "isOrginal").ToList())
                                    //    {
                                    //        StringCollection myCol = new StringCollection();
                                    //        myCol.Add(prInvoice.isOrginal.ToString());
                                    //        item.Values = myCol;


                                    //    }
                                    //    //end update

                                    //}
                                    rs.rep.SetParameters(paramarr);

                                    rs.rep.Refresh();


                                    if (int.Parse(AppSettings.Allow_print_inv_count) > prInvoice.printedcount)
                                    {

                                        this.Dispatcher.Invoke(() =>
                                        {
                                            //LocalReportExtensions.ExportToPDF(rep, filepath);
                                            if (AppSettings.salePaperSize != "A4")
                                            {
                                                LocalReportExtensions.customExportToPDF(rs.rep, filepath, rs.width, rs.height);
                                            }
                                            else
                                            {
                                                LocalReportExtensions.ExportToPDF(rs.rep, filepath);
                                            }

                                        });


                                        int res = 0;

                                        res = await invoiceModel.updateprintstat(prInvoice.invoiceId, 1, false, true);



                                        prInvoice.printedcount = prInvoice.printedcount + 1;

                                        prInvoice.isOrginal = false;


                                    }
                                    else
                                    {
                                        this.Dispatcher.Invoke(() =>
                                        {
                                            Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trYouExceedLimit"), animation: ToasterAnimation.FadeIn);

                                        });

                                    }


                                }
                                else
                                {

                                    this.Dispatcher.Invoke(() =>
                                    {
                                        //LocalReportExtensions.ExportToPDF(rep, filepath);
                                        if (AppSettings.salePaperSize != "A4")
                                        {
                                            LocalReportExtensions.customExportToPDF(rs.rep, filepath, rs.width, rs.height);
                                        }
                                        else
                                        {
                                            LocalReportExtensions.ExportToPDF(rs.rep, filepath);
                                        }

                                    });

                                }
                                // end copy count



                            }


                        }
                    }
                }

            }
            catch (Exception ex)
            {
                this.Dispatcher.Invoke(() =>
                {
                    Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("notCompleted"), animation: ToasterAnimation.FadeIn);

                });
            }


        }
        public async void multiplePaytable(List<ReportParameter> paramarr)
        {
            if ((prInvoice.invType == "s" || prInvoice.invType == "sd" || prInvoice.invType == "sbd" || prInvoice.invType == "sb"))
            {
                CashTransfer cachModel = new CashTransfer();
                List<PayedInvclass> payedList = new List<PayedInvclass>();
                payedList = await cachModel.GetPayedByInvId(prInvoice.invoiceId);
                mailpayedList = payedList;
                decimal sump = payedList.Sum(x => x.cash);
                decimal deservd = (decimal)prInvoice.totalNet - sump;
                //convertter
                foreach (var p in payedList)
                {
                    p.cash = decimal.Parse(reportclass.DecTostring(p.cash));
                }
                paramarr.Add(new ReportParameter("cashTr", AppSettings.resourcemanagerreport.GetString("trCashType")));

                paramarr.Add(new ReportParameter("sumP", reportclass.DecTostring(sump)));
                paramarr.Add(new ReportParameter("deserved", reportclass.DecTostring(deservd)));
                rep.DataSources.Add(new ReportDataSource("DataSetPayedInvclass", payedList));


            }
        }
        public async Task printInvoice(long invoiceId)
        {
            try
            {
                clsReports classreport = new clsReports();
                resultmessage resmsg = new resultmessage();
                resmsg = await classreport.pdfSaleInvoice(invoiceId, "directprint");
                Invoice prInvoice = new Invoice();
                if (resmsg.result != "")
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString(resmsg.result), animation: ToasterAnimation.FadeIn);

                    });
                }
                else
                {
                    prInvoice = resmsg.prInvoice;
                    // heereee
                    //  string result = "";
                    Invoice invoiceModel = new Invoice();
                    // Start  print
                    //copy count
                    if (prInvoice.invType == "s" || prInvoice.invType == "sb" || prInvoice.invType == "p" || prInvoice.invType == "pb" || prInvoice.invType == "ts"
                        || prInvoice.invType == "ss")
                    {

                        resmsg.paramarr.Add(new ReportParameter("isOrginal", prInvoice.isOrginal.ToString()));

                        for (int i = 1; i <= short.Parse(AppSettings.sale_copy_count); i++)
                        {
                            if (i > 1)
                            {
                                // update paramarr->isOrginal
                                foreach (var item in resmsg.paramarr.Where(x => x.Name == "isOrginal").ToList())
                                {
                                    StringCollection myCol = new StringCollection();
                                    myCol.Add(prInvoice.isOrginal.ToString());
                                    item.Values = myCol;


                                }
                                //end update

                            }
                            resmsg.rs.rep.SetParameters(resmsg.paramarr);

                            resmsg.rs.rep.Refresh();


                            if (int.Parse(AppSettings.Allow_print_inv_count) > prInvoice.printedcount)
                            {

                                this.Dispatcher.Invoke(() =>
                                {
                                    if (AppSettings.salePaperSize == "A4")
                                    {

                                        LocalReportExtensionsPrint.PrintToPrinterbyNameAndCopy(resmsg.rs.rep, AppSettings.sale_printer_name, 1);

                                    }
                                    else
                                    {
                                        LocalReportExtensionsPrint.customPrintToPrinter(resmsg.rs.rep, AppSettings.sale_printer_name, 1, resmsg.rs.width, resmsg.rs.height);

                                    }

                                });


                                int res = 0;
                                res = await invoiceModel.updateprintstat(prInvoice.invoiceId, 1, false, true);
                                prInvoice.printedcount = prInvoice.printedcount + 1;

                                prInvoice.isOrginal = false;


                            }
                            else
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trYouExceedLimit"), animation: ToasterAnimation.FadeIn);
                                });
                                //result = "trYouExceedLimit";


                            }

                        }

                    }
                    else
                    {

                        this.Dispatcher.Invoke(() =>
                        {

                            if (AppSettings.salePaperSize == "A4")
                            {

                                LocalReportExtensions.PrintToPrinterbyNameAndCopy(resmsg.rs.rep, AppSettings.sale_printer_name, short.Parse(AppSettings.sale_copy_count));

                            }
                            else
                            {
                                LocalReportExtensions.customPrintToPrinter(resmsg.rs.rep, AppSettings.sale_printer_name, short.Parse(AppSettings.sale_copy_count), resmsg.rs.width, resmsg.rs.height);

                            }


                        });

                    }
                    // end copy count
                    //endprint


                }


            }
            catch (Exception ex)
            {
                ex.ToString();
                this.Dispatcher.Invoke(() =>
                {
                    Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("notCompleted"), animation: ToasterAnimation.FadeIn);

                });
            }


        }
        public  void printInvoiceInkitchen(long invoiceId, List<OrderPreparing> OrderPreparingList)
        {
            try
            {
                //   prInvoice = new Invoice();

                if (invoiceId > 0)
                {
                    reportsize rs = reportclass.PrintPrepOrder(OrderPreparingList);
                    this.Dispatcher.Invoke(() =>
                    {
                        if (AppSettings.kitchenPaperSize == "A4")
                        {
                            //   LocalReportExtensionsPrint locrepext = new LocalReportExtensionsPrint();
                            LocalReportExtensionsPrint.PrintToPrinterbyNameAndCopy(rs.rep, AppSettings.kitchen_printer_name, short.Parse(AppSettings.kitchen_copy_count));

                        }
                        else
                        {

                            //  LocalReportExtensionsPrint locrepext = new LocalReportExtensionsPrint();
                            LocalReportExtensionsPrint.customPrintToPrinter(rs.rep, AppSettings.kitchen_printer_name, short.Parse(AppSettings.kitchen_copy_count), rs.width, rs.height);

                        }

                    });
                    //foreach (OrderPreparing row in OrderPreparingList)
                    //{
                    //    List<OrderPreparing> templist = new List<OrderPreparing>();
                    //    templist.Add(row);
                    //    printPrepOrder(templist);
                    //}

                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPrintEmptyInvoice"), animation: ToasterAnimation.FadeIn);

                    });
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
                this.Dispatcher.Invoke( async () =>
                {
                  await Task.Delay(500);
                    Toaster.ShowWarning(Window.GetWindow(this), message: "Not completed", animation: ToasterAnimation.FadeIn);
                });
            }


        }

        //public  reportsize PrintPrepOrder(List<OrderPreparing> OrderPreparingList)
        //{
        //    List<ReportParameter> paramarr = new List<ReportParameter>();

        //    #region fill invoice data
        //    reportsize rs = new reportsize();
        // //LocalReport rep = new LocalReport();
        //   rs = reportclass.GetKitchenRdlcpath(AppSettings.kitchenPaperSize, OrderPreparingList.Count(), rs.rep);
        //   //rs.rep;
        //   // rs.width;
        //   //rs.height;


        //    ReportCls.checkLang();

        //    clsReports.PreparingOrdersPrint(OrderPreparingList.ToList(), rs.rep, paramarr);
        //    clsReports.setReportLanguage(paramarr);
        //    clsReports.Header(paramarr);
        //    paramarr.Add(new ReportParameter("isSaved", "y"));

        //    rs.rep.SetParameters(paramarr);

        //    rs.rep.Refresh();
        //    #endregion
        //    //copy count
        //    string invType = OrderPreparingList.FirstOrDefault().invType;
        //    //if (invType == "s" || invType == "sb" || invType == "ts"
        //    //    || invType == "ss")
        //    //{

        //        paramarr.Add(new ReportParameter("isOrginal","true"));

        //    return rs;
        //        //kitchen



        //    //}
        //    //else
        //    //{



        //    //}
        //    // end copy count

        //}
        private async void Btn_printInvoice_Click(object sender, RoutedEventArgs e)
        {//print
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);

                ////////////////
                //Thread t1 = new Thread(() =>
                //{
                await printInvoice(invoice.invoiceId);
                //});
                //t1.Start();
                /////////////////


                if (sender != null)
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }

        }

        private async void Btn_preview_Click(object sender, RoutedEventArgs e)
        {//preview
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);

                #region
                if (invoice.invoiceId > 0)
                {
                    prInvoice = await invoiceModel.GetByInvoiceId(invoice.invoiceId);
                    if (int.Parse(AppSettings.Allow_print_inv_count) <= prInvoice.printedcount)
                    {
                        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trYouExceedLimit"), animation: ToasterAnimation.FadeIn);

                    }
                    else
                    {

                        Window.GetWindow(this).Opacity = 0.2;

                        List<ReportParameter> paramarr = new List<ReportParameter>();
                        string pdfpath = "";

                        ////////////////////////
                        string folderpath = reportclass.PathUp(System.IO.Directory.GetCurrentDirectory(), 2, pdfpath) + @"\Thumb\report\";
                        ReportCls.clearFolder(folderpath);

                        pdfpath = @"\Thumb\report\Temp" + DateTime.Now.ToFileTime().ToString() + ".pdf";
                        pdfpath = reportclass.PathUp(System.IO.Directory.GetCurrentDirectory(), 2, pdfpath);
                        //////////////////////////////////
                        #region fill invoice data

                        //items
                        invoiceItems = await invoiceModel.GetInvoicesItems(prInvoice.invoiceId);
                        itemscount = invoiceItems.Count();
                        string reppath = reportclass.GetreceiptInvoiceRdlcpath(prInvoice);
                        //user
                        User employ = new User();
                        if (FillCombo.usersList != null)
                        {
                            employ = FillCombo.usersList.Where(X => X.userId == (long)prInvoice.updateUserId).FirstOrDefault();
                        }
                        else
                        {
                            employ = await userModel.getUserById((long)prInvoice.updateUserId);
                        }

                        prInvoice.uuserName = employ.name;
                        prInvoice.uuserLast = employ.lastname;
                        //agent
                        if (prInvoice.agentId != null)
                        {
                            Agent agentinv = new Agent();

                            // agentinv = customers.Where(X => X.agentId == prInvoice.agentId).FirstOrDefault();

                            if (FillCombo.customersList != null)
                            {
                                agentinv = FillCombo.customersList.Where(X => X.agentId == (long)prInvoice.agentId).FirstOrDefault();
                            }
                            else
                            {
                                agentinv = await agentinv.getAgentById((long)prInvoice.agentId);
                            }

                            prInvoice.agentCode = agentinv.code;
                            //new lines
                            prInvoice.agentName = agentinv.name;
                            prInvoice.agentCompany = agentinv.company;

                        }
                        else
                        {
                            prInvoice.agentCode = "-";
                            prInvoice.agentName = "-";
                            prInvoice.agentCompany = "-";
                        }
                        //branch
                        Branch branch = new Branch();
                        if (FillCombo.branchsList != null)
                        {
                            branch = FillCombo.branchsList.Where(X => X.branchId == (long)prInvoice.branchCreatorId).FirstOrDefault();

                        }
                        else
                        {
                            branch = await branchModel.getBranchById((long)prInvoice.branchCreatorId);
                        }

                        if (branch.branchId > 0)
                        {
                            prInvoice.branchName = branch.name;
                        }

                        ReportCls.checkLang();
                        //shipping
                        ShippingCompanies shippingcom = new ShippingCompanies();

                        if (prInvoice.shippingCompanyId > 0)
                        {
                            if (FillCombo.shippingCompaniesList != null)
                            {
                                shippingcom = FillCombo.shippingCompaniesList.Where(X => X.shippingCompanyId == (long)prInvoice.shippingCompanyId).FirstOrDefault();

                            }
                            else
                            {
                                shippingcom = await shippingcom.GetByID((long)prInvoice.shippingCompanyId);
                            }

                        }
                        User shipuser = new User();
                        if (prInvoice.shipUserId > 0)
                        {
                            shipuser = await userModel.getUserById((long)prInvoice.shipUserId);
                        }
                        prInvoice.shipUserName = shipuser.name + " " + shipuser.lastname;
                        //end shipping
                        //items subTotal & itemTax
                        decimal totaltax = 0;
                        foreach (var i in invoiceItems)
                        {
                            i.price = decimal.Parse(HelpClass.DecTostring(i.price));
                            if (i.itemTax != null)
                            {
                                totaltax += (decimal)i.itemTax;

                            }
                            i.subTotal = decimal.Parse(HelpClass.DecTostring(i.price * i.quantity));

                        }

                        if (totaltax > 0 && prInvoice.invType != "sbd" && prInvoice.invType != "sb")
                        {
                            paramarr.Add(new ReportParameter("itemtax_note", AppSettings.itemtax_note.Trim()));
                            paramarr.Add(new ReportParameter("hasItemTax", "1"));

                        }
                        else
                        {
                            // paramarr.Add(new ReportParameter("itemtax_note", AppSettings.itemtax_note.Trim()));
                            paramarr.Add(new ReportParameter("hasItemTax", "0"));
                        }
                        //

                        clsReports.purchaseInvoiceReport(invoiceItems, rep, reppath);

                        clsReports.setReportLanguage(paramarr);
                        clsReports.Header(paramarr);
                        paramarr.Add(new ReportParameter("isSaved", "y"));
                        paramarr = reportclass.fillSaleInvReport(prInvoice, paramarr, shippingcom);
                        //   multiplePaytable(paramarr);
                        // payment methods

                        if (prInvoice.invType == "s" || prInvoice.invType == "sd" || prInvoice.invType == "sbd" || prInvoice.invType == "sb"
                            || prInvoice.invType == "ts" || prInvoice.invType == "ss" || prInvoice.invType == "tsd" || prInvoice.invType == "ssd"
                            )
                        {
                            CashTransfer cachModel = new CashTransfer();
                            List<PayedInvclass> payedList = new List<PayedInvclass>();
                            payedList = await cachModel.GetPayedByInvId(prInvoice.invoiceId);
                            mailpayedList = payedList;
                            decimal sump = payedList.Sum(x => x.cash);
                            decimal deservd = (decimal)prInvoice.totalNet - sump;
                            //convertter
                            foreach (var p in payedList)
                            {
                                p.cash = decimal.Parse(reportclass.DecTostring(p.cash));
                            }
                            paramarr.Add(new ReportParameter("cashTr", AppSettings.resourcemanagerreport.GetString("trCashType")));

                            paramarr.Add(new ReportParameter("sumP", reportclass.DecTostring(sump)));
                            paramarr.Add(new ReportParameter("deserved", reportclass.DecTostring(deservd)));
                            rep.DataSources.Add(new ReportDataSource("DataSetPayedInvclass", payedList));


                        }


                        rep.SetParameters(paramarr);

                        rep.Refresh();
                        #endregion
                        //

                        // start preview
                        //copy count
                        if (prInvoice.invType == "s" || prInvoice.invType == "sb" || prInvoice.invType == "p" || prInvoice.invType == "pb" || prInvoice.invType == "ts" || prInvoice.invType == "ss")
                        {

                            //   paramarr.Add(new ReportParameter("isOrginal", prInvoice.isOrginal.ToString()));
                            // update paramarr->isOrginal
                            foreach (var item in paramarr.Where(x => x.Name == "isOrginal").ToList())
                            {
                                StringCollection myCol = new StringCollection();
                                myCol.Add(prInvoice.isOrginal.ToString());
                                item.Values = myCol;


                            }
                            //end update
                            paramarr.Add(new ReportParameter("isOrginal", false.ToString()));

                            rep.SetParameters(paramarr);

                            rep.Refresh();
                            /////////////////////////
                            if (int.Parse(AppSettings.Allow_print_inv_count) > prInvoice.printedcount)
                            {

                                if (prInvoice.invType == "s" && AppSettings.salePaperSize != "A4")
                                {
                                    LocalReportExtensions.customExportToPDF(rep, pdfpath, width, height);
                                }
                                else
                                {
                                    LocalReportExtensions.ExportToPDF(rep, pdfpath);
                                }


                                int res = 0;

                                res = await invoiceModel.updateprintstat(prInvoice.invoiceId, 1, false, true);



                                prInvoice.printedcount = prInvoice.printedcount + 1;

                                prInvoice.isOrginal = false;


                            }
                            else
                            {
                                Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trYouExceedLimit"), animation: ToasterAnimation.FadeIn);
                            }


                        }
                        else
                        {

                            if (prInvoice.invType == "s" && AppSettings.salePaperSize != "A4")
                            {
                                LocalReportExtensions.customExportToPDF(rep, pdfpath, width, height);
                            }
                            else
                            {
                                LocalReportExtensions.ExportToPDF(rep, pdfpath);
                            }

                        }
                        // end copy count

                        //end previw

                        wd_previewPdf w = new wd_previewPdf();
                        w.pdfPath = pdfpath;
                        if (!string.IsNullOrEmpty(w.pdfPath))
                        {
                            w.ShowDialog();

                            w.wb_pdfWebViewer.Dispose();

                        }
                        else
                            Toaster.ShowError(Window.GetWindow(this), message: "", animation: ToasterAnimation.FadeIn);
                        Window.GetWindow(this).Opacity = 1;

                    }
                }
                else
                {


                    //     Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trSaveInvoiceToPreview"), animation: ToasterAnimation.FadeIn);
                    ////
                    Invoice tmpinvoice = new Invoice();
                    tmpinvoice.invType = "sd";
                    if (txt_customer.Text != null && txt_customer.Text != "")
                    {

                        tmpinvoice.agentId = (long)invoice.agentId;
                    }


                    tmpinvoice.branchCreatorId = MainWindow.branchLogin.branchId;
                    tmpinvoice.branchId = MainWindow.branchLogin.branchId;
                    tmpinvoice.totalNet = decimal.Parse(tb_total.Text);
                    tmpinvoice.deserved = tmpinvoice.totalNet;
                    tmpinvoice.discountValue = invoice.discountValue;
                    tmpinvoice.tax = decimal.Parse(tb_tax.Text);
                    tmpinvoice.total = decimal.Parse(tb_subtotal.Text);
                    tmpinvoice.invDate = DateTime.Now;

                    //   tmpinvoice.deservedDate = dp_desrvedDate.SelectedDate;
                    tmpinvoice.updateDate = DateTime.Now;
                    tmpinvoice.invTime = new TimeSpan();
                    //                         tmpinvoice.vendorInvDate=

                    // prInvoice = await invoiceModel.GetByInvoiceId(invoice.invoiceId);
                    //if (int.Parse(AppSettings.Allow_print_inv_count) <= prInvoice.printedcount)
                    //{
                    //    Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trYouExceedLimit"), animation: ToasterAnimation.FadeIn);

                    //}
                    //else
                    {

                        Window.GetWindow(this).Opacity = 0.2;

                        List<ReportParameter> paramarr = new List<ReportParameter>();
                        string pdfpath = "";

                        ////////////////////////
                        string folderpath = reportclass.PathUp(System.IO.Directory.GetCurrentDirectory(), 2, pdfpath) + @"\Thumb\report\";
                        ReportCls.clearFolder(folderpath);

                        pdfpath = @"\Thumb\report\Temp" + DateTime.Now.ToFileTime().ToString() + ".pdf";
                        pdfpath = reportclass.PathUp(System.IO.Directory.GetCurrentDirectory(), 2, pdfpath);
                        //////////////////////////////////
                        List<ItemTransfer> invoiceItems = new List<ItemTransfer>();
                        ItemTransfer itemtemp = new ItemTransfer();
                        decimal totaltax = 0;
                        //foreach (var billrow in billDetails)
                        //{
                        //    itemtemp = new ItemTransfer();
                        //    itemtemp.itemsTransId = billrow.ID;
                        //    itemtemp.itemName = billrow.Product;
                        //    itemtemp.unitName = billrow.Unit;
                        //    itemtemp.quantity = billrow.Count;

                        //    itemtemp.price = decimal.Parse(HelpClass.DecTostring(billrow.Price));

                        //    totaltax += billrow.Tax;


                        //    // itemtemp.t = billrow.Total;
                        //    invoiceItems.Add(itemtemp);



                        //}
                        ItemUnit tempiu = new ItemUnit();
                        Item tempI = new Item();

                        for (int i = 0; i < billDetailsList.Count; i++)
                        {
                            itemtemp = new ItemTransfer();
                            //itemtemp.invoiceId = 0;
                            itemtemp.quantity = billDetailsList[i].Count;
                            itemtemp.price = decimal.Parse(HelpClass.DecTostring(billDetailsList[i].Price));

                            itemtemp.itemUnitId = billDetailsList[i].itemUnitId;
                            //  tempiu = InvoiceGlobalSaleUnitsList.Where(x => x.itemUnitId == billDetails[i].itemUnitId).FirstOrDefault();
                            // tempI = InvoiceGlobalSaleUnitsList.Where(X => X.itemUnitId == billDetailsList[i].itemUnitId).FirstOrDefault();


                            itemtemp.unitName = tempI.unitName;

                            //itemtemp.offerId = billDetails[i].offerId == null ? 0 : billDetails[i].offerId;
                            //itemtemp.offerType = decimal.Parse(billDetails[i].OfferType);
                            //itemtemp.offerValue = billDetails[i].OfferValue;
                            itemtemp.itemTax = decimal.Parse(HelpClass.DecTostring(billDetailsList[i].Tax));
                            itemtemp.itemUnitPrice = decimal.Parse(HelpClass.DecTostring(billDetailsList[i].basicPrice));
                            //string serialNum = "";
                            //if (billDetailsList[i].serialList != null)
                            //{
                            //    List<string> lst = billDetailsList[i].serialList.ToList();
                            //    for (int j = 0; j < lst.Count; j++)
                            //    {
                            //        serialNum += lst[j];
                            //        if (j != lst.Count - 1)
                            //            serialNum += ",";
                            //    }
                            //}
                            // itemtemp.itemSerial = serialNum;
                            itemtemp.createUserId = MainWindow.userLogin.userId;
                            itemtemp.itemName = billDetailsList[i].itemName;

                            totaltax += billDetailsList[i].Tax;
                            invoiceItems.Add(itemtemp);
                        }

                        //   invoiceItems = await invoiceModel.GetInvoicesItems(prInvoice.invoiceId);
                        itemscount = invoiceItems.Count();
                        string reppath = reportclass.GetreceiptInvoiceRdlcpath(tmpinvoice);
                        // AppSettings.userLogin
                        User employ = new User();
                        employ = MainWindow.userLogin;
                        tmpinvoice.uuserName = employ.name;
                        tmpinvoice.uuserLast = employ.lastname;

                        //  invoiceItems = await invoiceModel.GetInvoicesItems(tmpinvoice.invoiceId);
                        if (tmpinvoice.agentId != null && tmpinvoice.agentId > 0)
                        {
                            Agent agentinv = new Agent();
                            //  agentinv = customers.Where(X => X.agentId == tmpinvoice.agentId).FirstOrDefault();
                            agentinv = await agentinv.getAgentById((long)prInvoice.agentId);
                            tmpinvoice.agentCode = agentinv.code;
                            //new lines
                            tmpinvoice.agentName = agentinv.name;
                            tmpinvoice.agentCompany = agentinv.company;
                        }
                        else
                        {
                            tmpinvoice.agentCode = "-";
                            tmpinvoice.agentName = "-";
                            tmpinvoice.agentCompany = "-";
                        }
                        //branch name
                        Branch branch = new Branch();
                        branch = MainWindow.branchLogin;
                        if (branch.branchId > 0)
                        {
                            tmpinvoice.branchName = branch.name;
                        }

                        //  invoiceItems = await invoiceModel.GetInvoicesItems(prInvoice.invoiceId);
                        ReportCls.checkLang();


                        if (totaltax > 0)
                        {
                            paramarr.Add(new ReportParameter("itemtax_note", AppSettings.itemtax_note.Trim()));
                            paramarr.Add(new ReportParameter("hasItemTax", "1"));

                        }
                        else
                        {
                            // paramarr.Add(new ReportParameter("itemtax_note", AppSettings.itemtax_note.Trim()));
                            paramarr.Add(new ReportParameter("hasItemTax", "0"));
                        }
                        clsReports.purchaseInvoiceReport(invoiceItems, rep, reppath);
                        clsReports.setReportLanguage(paramarr);
                        clsReports.Header(paramarr);
                        paramarr.Add(new ReportParameter("isSaved", "n"));
                        paramarr = reportclass.fillSaleInvReport(tmpinvoice, paramarr, shippingcomp);

                        if (tmpinvoice.invType == "pd" || tmpinvoice.invType == "sd" || tmpinvoice.invType == "qd"
     || tmpinvoice.invType == "sbd" || tmpinvoice.invType == "pbd"
     || tmpinvoice.invType == "ord" || tmpinvoice.invType == "imd" || tmpinvoice.invType == "exd")
                        {
                            paramarr.Add(new ReportParameter("isOrginal", true.ToString()));
                        }
                        else
                        {
                            paramarr.Add(new ReportParameter("isOrginal", false.ToString()));
                        }
                        if ((tmpinvoice.invType == "s" || tmpinvoice.invType == "sd" || tmpinvoice.invType == "sbd" || tmpinvoice.invType == "sb"))
                        {
                            CashTransfer cachModel = new CashTransfer();
                            PayedInvclass PayedInvtemp = new PayedInvclass();

                            List<PayedInvclass> payedList = new List<PayedInvclass>();
                            decimal sump = 0;

                            ////paymentsList
                            //if (cb_paymentProcessType.SelectedIndex != -1)
                            //{
                            //    switch (cb_paymentProcessType.SelectedValue.ToString())
                            //    {
                            //        case "cash":
                            //            {
                            //                PayedInvtemp.processType = "cash";
                            //                sump = decimal.Parse(tb_cashPaid.Text);
                            //            }
                            //            break;
                            //        case "balance":
                            //            {
                            //                PayedInvtemp.processType = "balance";
                            //                sump = decimal.Parse(tb_cashPaid.Text);
                            //            }
                            //            break;
                            //        case "card":
                            //            {
                            //                PayedInvtemp.processType = "card";
                            //                sump = decimal.Parse(tb_cashPaid.Text);
                            //            }
                            //            break;
                            //        case "multiple":
                            //            {
                            //                PayedInvtemp.processType = "multiple";
                            //            }
                            //            break;

                            //    }
                            //}
                            //
                            // payedList = await cachModel.GetPayedByInvId(prInvoice.invoiceId);
                            sump = 0;
                            payedList.Add(PayedInvtemp);

                            //  sump = payedList.Sum(x => x.cash).Value;
                            //  decimal deservd = (decimal)tmpinvoice.totalNet - sump;
                            paramarr.Add(new ReportParameter("cashTr", AppSettings.resourcemanagerreport.GetString("trCashType")));

                            paramarr.Add(new ReportParameter("sumP", reportclass.DecTostring(sump)));

                            paramarr.Add(new ReportParameter("trDraftInv", AppSettings.resourcemanagerreport.GetString("trDraft")));
                            //  paramarr.Add(new ReportParameter("deserved", reportclass.DecTostring(deservd)));
                            rep.DataSources.Add(new ReportDataSource("DataSetPayedInvclass", payedList));


                        }


                        rep.SetParameters(paramarr);
                        rep.Refresh();



                        ////copy count
                        //if (tmpinvoice.invType == "s" || tmpinvoice.invType == "sb" || tmpinvoice.invType == "p" || tmpinvoice.invType == "pb")
                        //{

                        //    //   paramarr.Add(new ReportParameter("isOrginal", prInvoice.isOrginal.ToString()));
                        //    // update paramarr->isOrginal
                        //    foreach (var item in paramarr.Where(x => x.Name == "isOrginal").ToList())
                        //    {
                        //        StringCollection myCol = new StringCollection();
                        //        myCol.Add(tmpinvoice.isOrginal.ToString());
                        //        item.Values = myCol;


                        //    }
                        //end update
                        //paramarr.Add(new ReportParameter("isOrginal", false.ToString()));

                        rep.SetParameters(paramarr);

                        rep.Refresh();

                        //if (int.Parse(AppSettings.Allow_print_inv_count) > tmpinvoice.printedcount)
                        //{

                        if (tmpinvoice.invType == "s" && AppSettings.salePaperSize != "A4")
                        {
                            LocalReportExtensions.customExportToPDF(rep, pdfpath, width, height);
                        }
                        else
                        {
                            LocalReportExtensions.ExportToPDF(rep, pdfpath);
                        }




                        wd_previewPdf w = new wd_previewPdf();
                        w.pdfPath = pdfpath;
                        if (!string.IsNullOrEmpty(w.pdfPath))
                        {
                            w.ShowDialog();

                            w.wb_pdfWebViewer.Dispose();

                        }
                        else
                            Toaster.ShowError(Window.GetWindow(this), message: "", animation: ToasterAnimation.FadeIn);
                        Window.GetWindow(this).Opacity = 1;

                    }



                    //////



                }
                #endregion



                if (sender != null)
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }


        public async void sendsaleEmail(long invoiceId)
        {
            try
            {
                //
                prInvoice = new Invoice();
                //if (isdirect)
                //{
                prInvoice = await invoiceModel.GetByInvoiceId(invoiceId);
                //}
                //else
                //{
                //    prInvoice = await invoiceModel.GetByInvoiceId(invoice.invoiceId);
                //}
                if (prInvoice.invoiceId > 0)
                {
                    //  prInvoice = new Invoice();
                    //  Invoice tomailInvoice = new Invoice();
                    //if (prinvoiceId != 0)
                    //    prInvoice = await invoiceModel.GetByInvoiceId(prinvoiceId);
                    //else
                    //  prInvoice = await invoiceModel.GetByInvoiceId(invoice.invoiceId);
                    //   tomailInvoice = prInvoice;
                    decimal? discountval = 0;
                    string discounttype = "";
                    discountval = prInvoice.discountValue;
                    discounttype = prInvoice.discountType;
                    if (prInvoice.invType == "pd" || prInvoice.invType == "sd" || prInvoice.invType == "qd"
                    || prInvoice.invType == "sbd" || prInvoice.invType == "pbd"
                    || prInvoice.invType == "ord" || prInvoice.invType == "imd" || prInvoice.invType == "exd")
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trCanNotSendDraftInvoice"), animation: ToasterAnimation.FadeIn);
                        });
                    }
                    else
                    {
                        invoiceItems = await invoiceModel.GetInvoicesItems(prInvoice.invoiceId);
                        //extra && ingredient
                        #region items
                        //items subTotal & itemTax
                        decimal totaltax = 0;
                        List<ItemTransfer> tempitemlist = new List<ItemTransfer>();
                        bool isArabic = ReportCls.checkLang();
                        foreach (var i in invoiceItems)
                        {
                            ItemTransfer tempobject = new ItemTransfer();

                            i.price = decimal.Parse(HelpClass.DecTostring(i.price));
                            if (i.itemTax != null)
                            {
                                totaltax += (decimal)i.itemTax;

                            }
                            i.subTotal = decimal.Parse(HelpClass.DecTostring(i.price * i.quantity));

                            //   i.isExtra = false;
                            tempobject = i;

                            //add without ingridient
                            if (i.itemsIngredients.Where(x => x.isBasic == false && x.isActive == 0).Count() > 0)
                            {
                                string alling = "";
                                List<itemsTransferIngredients> inglist = i.itemsIngredients.Where(x => x.isActive == 0 && x.isBasic == false).ToList();

                                for (int k = 0; k < inglist.Count(); k++)
                                {
                                    if (isArabic)
                                    {
                                        if (k == 0)
                                        {
                                            alling = inglist[k].DishIngredientName;
                                        }
                                        else
                                        {
                                            alling = alling + "," + inglist[k].DishIngredientName;
                                        }
                                    }
                                    else
                                    {
                                        if (k == 0)
                                        {
                                            alling = " -" + AppSettings.resourcemanagerreport.GetString("without") + " : " + inglist[k].DishIngredientName;
                                        }
                                        else
                                        {
                                            alling = alling + ", " + inglist[k].DishIngredientName;
                                        }
                                    }


                                }
                                if (isArabic)
                                {

                                    tempobject.itemName = tempobject.itemName + " -" + AppSettings.resourcemanagerreport.GetString("without") + ":" + alling;

                                }
                                else
                                {
                                    tempobject.itemName = tempobject.itemName + alling;
                                }

                            }

                            //end add without ingridient
                            tempitemlist.Add(tempobject);
                            //add extra row
                            if (i.itemExtras.Count() > 0)
                            {


                                //   string extname=" - "+ AppSettings.resourcemanagerreport.GetString("extra")+": ";
                                for (int j = 0; j < i.itemExtras.Count(); j++)
                                {
                                    tempobject = new ItemTransfer();
                                    tempobject = i.itemExtras[j];
                                    tempobject.itemName = AppSettings.resourcemanagerreport.GetString("extra") + ": " + tempobject.itemName;
                                    tempobject.price = decimal.Parse(HelpClass.DecTostring(tempobject.price));
                                    if (tempobject.itemTax != null)
                                    {
                                        totaltax += (decimal)tempobject.itemTax;

                                    }
                                    tempobject.subTotal = decimal.Parse(HelpClass.DecTostring(tempobject.price * tempobject.quantity));


                                
                                    tempitemlist.Add(tempobject);

                                }
                             
                            }
                            //end add extra row

                        }
                        invoiceItems = tempitemlist.ToList();
                        #endregion

                        //end extra and ingredient
                        SysEmails email = new SysEmails();
                        EmailClass mailtosend = new EmailClass();
                        email = await email.GetByBranchIdandSide(MainWindow.branchLogin.branchId, "sales");
                        Agent toAgent = new Agent();
                        toAgent = FillCombo.customersList.Where(x => x.agentId == prInvoice.agentId).FirstOrDefault();

                        if (toAgent == null || toAgent.agentId == 0)
                        {
                            //edit warning message to customer
                            //this.Dispatcher.Invoke(() =>
                            //{
                            //    Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trTheCustomerHasNoEmail"), animation: ToasterAnimation.FadeIn);
                            //});

                        }
                        else
                        {
                            //  int? itemcount = invoiceItems.Count();
                            if (email.emailId == 0)
                                this.Dispatcher.Invoke(() =>
                                {
                                    Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trNoEmailForThisDept"), animation: ToasterAnimation.FadeIn);
                                });
                            else
                            {
                                if (prInvoice.invoiceId == 0)
                                {
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trThereIsNoOrderToSen"), animation: ToasterAnimation.FadeIn);
                                    });
                                }

                                else
                                {
                                    if (invoiceItems == null || invoiceItems.Count() == 0)
                                    {
                                        this.Dispatcher.Invoke(() =>
                                        {
                                            Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trThereIsNoItemsToSend"), animation: ToasterAnimation.FadeIn);
                                        });
                                    }

                                    else
                                    {

                                        if (toAgent.email.Trim() == "")
                                        {
                                            this.Dispatcher.Invoke(() =>
                                            {
                                                Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trTheCustomerHasNoEmail"), animation: ToasterAnimation.FadeIn);
                                            });
                                        }

                                        else
                                        {
                                            SetValues setvmodel = new SetValues();

                                            List<SetValues> setvlist = new List<SetValues>();
                                            if (prInvoice.invType == "s" || prInvoice.invType == "ss" || prInvoice.invType == "ts")
                                            {
                                                setvlist = await setvmodel.GetBySetName("sale_email_temp");
                                            }
                                            else if (prInvoice.invType == "or" || prInvoice.invType == "ors")
                                            {
                                                setvlist = await setvmodel.GetBySetName("sale_order_email_temp");
                                            }
                                            else if (prInvoice.invType == "q" || prInvoice.invType == "qs")
                                            {
                                                setvlist = await setvmodel.GetBySetName("quotation_email_temp");
                                            }
                                            else
                                            {
                                                setvlist = await setvmodel.GetBySetName("sale_email_temp");
                                            }

                                            //shipping

                                            if (prInvoice.shippingCompanyId > 0)
                                            {
                                                shippingcomp = await shippingcomp.GetByID((long)prInvoice.shippingCompanyId);
                                            }

                                            if (prInvoice.shipUserId > 0)
                                            {
                                                shipinguser = await userModel.getUserById((long)prInvoice.shipUserId);
                                            }
                                            prInvoice.shipUserName = shipinguser.name + " " + shipinguser.lastname;
                                            //end shipping

                                            string pdfpath = await SaveSalepdf(prInvoice.invoiceId);

                                            prInvoice.discountValue = (decimal)discountval;
                                            prInvoice.discountType = discounttype;
                                            //pay
                                            List<PayedInvclass> mailpayedList = new List<PayedInvclass>();

                                            CashTransfer cachModel = new CashTransfer();
                                            mailpayedList = await cachModel.GetPayedByInvId(prInvoice.invoiceId);
                                            //mailpayedList = payedList;
                                            decimal sump = mailpayedList.Sum(x => x.cash);
                                            decimal deservd = (decimal)prInvoice.totalNet - sump;
                                            //convertter
                                            foreach (var p in mailpayedList)
                                            {
                                                p.cash = decimal.Parse(reportclass.DecTostring(p.cash));
                                            }
                                            //
                                            //shipping
                                            ShippingCompanies shippingcom = new ShippingCompanies();

                                            if (prInvoice.shippingCompanyId > 0)
                                            {
                                                if (FillCombo.shippingCompaniesList != null)
                                                {
                                                    shippingcom = FillCombo.shippingCompaniesList.Where(X => X.shippingCompanyId == (long)prInvoice.shippingCompanyId).FirstOrDefault();

                                                }
                                                else
                                                {
                                                    shippingcom = await shippingcom.GetByID((long)prInvoice.shippingCompanyId);
                                                }
                                                // prInvoice.shippingCompanyName = shippingcom.name;
                                                prInvoice.shippingCompanyName = clsReports.shippingCompanyNameConvert(shippingcom.name);

                                            }
                                            User shipuser = new User();
                                            if (prInvoice.shipUserId > 0)
                                            {
                                                shipuser = await shipuser.getUserById((long)prInvoice.shipUserId);
                                            }
                                            prInvoice.shipUserName = shipuser.name + " " + shipuser.lastname;

                                            //end shipping
                                            mailtosend = mailtosend.fillSaleTempData(prInvoice, invoiceItems, mailpayedList, email, toAgent, setvlist);

                                            mailtosend.AddAttachTolist(pdfpath);
                                            string msg = "";
                                            this.Dispatcher.Invoke(() =>
                                            {
                                             msg = mailtosend.Sendmail();// temp comment
                                                if (msg == "Failure sending mail.")
                                                {
                                                    // msg = "No Internet connection";

                                                    Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trNoConnection"), animation: ToasterAnimation.FadeIn);
                                                }
                                                else if (msg == "mailsent")
                                                    Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trMailSent"), animation: ToasterAnimation.FadeIn);
                                                else
                                                    Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trMailNotSent"), animation: ToasterAnimation.FadeIn);
                                            });

                                        }
                                    }
                                }
                            }
                        }

                    }
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trThereIsNoItemsToSend"), animation: ToasterAnimation.FadeIn);
                    });
                }


                //

            }
            catch (Exception ex)
            {
                this.Dispatcher.Invoke(() =>
                {
                    Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trCannotSendEmail"), animation: ToasterAnimation.FadeIn);
                });
            }
        }
        //private void Btn_emailMessage_Click(object sender, RoutedEventArgs e)
        //{//email
        //    try
        //    {
        //        if (sender != null)
        //            HelpClass.StartAwait(grid_main);

        //        if (AppSettings.groupObject.HasPermissionAction(sendEmailPermission, AppSettings.groupObjects, "one"))
        //        {

        //            //await sendsaleEmail();
        //            ////
        //            Thread t1 = new Thread(() =>
        //            {
        //                sendsaleEmail(invoice.invoiceId);
        //            });
        //            t1.Start();
        //            ////
        //        }

        //        else
        //            Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
        //        if (sender != null)
        //            HelpClass.EndAwait(grid_main);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (sender != null)
        //            HelpClass.EndAwait(grid_main);
        //        HelpClass.ExceptionMessage(ex, this);
        //    }
        //}

        private async void Btn_printCount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);

                int result = 0;

                if (invoice.invoiceId > 0)
                {
                    result = await invoiceModel.updateprintstat(invoice.invoiceId, -1, true, true);


                    if (result > 0)
                    {
                        Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);
                    }
                    else
                    {
                        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                    }

                }
                else
                {
                    Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trChooseInvoiceToolTip"), animation: ToasterAnimation.FadeIn);
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
        }

        #endregion


        #region validate - clearValidate - textChange - lostFocus - . . . . 

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
        private void NumberInt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                //only  digits
                TextBox textBox = sender as TextBox;
                HelpClass.InputJustNumber(ref textBox);
                //if (textBox.Tag.ToString() == "int")
                {
                    Regex regex = new Regex("[^0-9]");
                    e.Handled = regex.IsMatch(e.Text);
                }
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

        #endregion
         
    }
}
