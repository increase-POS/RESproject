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
        int prinvoiceId = 0;
        #endregion
        decimal _DeliveryCost = 0;

        public static uc_diningHall Instance
        {
            get
            {
                if(_instance is null)
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
                if(FillCombo.customersList == null)
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
            // Clear();

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
                tb_tax.Text = AppSettings.invoiceTax_decimal.ToString();
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
            
        }

       async void changeInvType()
        {
            //values(diningHall - takeAway - selfService)
            if (AppSettings.invType == "diningHall")
            {
                txt_invType.Text = AppSettings.resourcemanager.GetString("trDiningHallType");

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

                if(invoice.invType != "sd" && invoice.invType != null)
                    await clear();
            }
            else if (AppSettings.invType == "takeAway" || _InvoiceType == "tsd") 
            {
                txt_invType.Text = AppSettings.resourcemanager.GetString("trTakeAway");

                btn_cancel.Visibility = Visibility.Collapsed;
                btn_tables.Visibility = Visibility.Collapsed;
                btn_kitchen.Visibility = Visibility.Collapsed;
                btn_waiter.Visibility = Visibility.Collapsed;


                md_draft.Visibility = Visibility.Visible;
                btn_delivery.Visibility = Visibility.Visible;
                btn_orderTime.Visibility = Visibility.Visible;

                btn_customer.IsEnabled = true;
                btn_discount.IsEnabled = true;

                refreshDraftNotification();

                if(invoice.invType == "ssd") // transfer  self service draft to take away draft
                {
                    await addInvoice("tsd");
                }
            }
            else if(AppSettings.invType == "selfService" || _InvoiceType == "ssd")
            {
                txt_invType.Text = AppSettings.resourcemanager.GetString("trSelfService");

                btn_cancel.Visibility = Visibility.Collapsed;
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
            Instance = null;
            GC.Collect();
        }
       
        #region  Cards
        CatigoriesAndItemsView catigoriesAndItemsView = new CatigoriesAndItemsView();
        #region Refrish Y
        Item item = new Item();
        List<Item> items;
        IEnumerable<Item> itemsQuery;
        int categoryId = 0;
        int tagId = 0;
        string _InvoiceType = "sd";
        void RefrishItemsCard(IEnumerable<Item> _items)
        {
            grid_itemContainerCard.Children.Clear();
            catigoriesAndItemsView.gridCatigorieItems = grid_itemContainerCard;
            catigoriesAndItemsView.FN_refrishCatalogItem(_items.ToList(),5, "sales");
        }
        #endregion
        #region Get Id By Click  Y

        public void ChangeItemIdEvent(int itemId)
        {
            try
            {
                if(AppSettings.invType == "diningHall" && selectedTables.Count == 0)
                    Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trChooseTableFirst"), animation: ToasterAnimation.FadeIn);

                else if (item != null)
                {
                    item = items.Where(x => x.itemId == itemId).FirstOrDefault();
                    addRowToBill(item,1);
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
               HelpClass.StartAwait(grid_main);

                //if (items is null)
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

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        async Task refreshItemsList()
        {
            DateTime dt = DateTime.Now;
            string day = dt.DayOfWeek.ToString();
           items = await FillCombo.item.GetAllSalesItemsInv(day.ToLower(), _MemberShipId);
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
                if (sender != null)
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


        private void Btn_firstPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);

                pageIndex = 1;
                #region
                itemsQuery = items.ToList();
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns, 15));
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
        private void Btn_prevPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);

                pageIndex = int.Parse(btn_prevPage.Content.ToString());
                #region
                itemsQuery = items.ToList();

                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns, 15));
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
        private void Btn_activePage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);

                pageIndex = int.Parse(btn_activePage.Content.ToString());
                #region
                itemsQuery = items.ToList();
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns, 15));
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
        private void Btn_nextPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);

                pageIndex = int.Parse(btn_nextPage.Content.ToString());
                #region
                itemsQuery = items.ToList();
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns, 15));
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
        private void Btn_lastPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);

                itemsQuery = items.ToList();
                pageIndex = ((itemsQuery.Count() - 1) / 9) + 1;
                #region
                itemsQuery = items.ToList();
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns, 15));
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
                    Border border = FindControls.FindVisualChildren<Border>(this).Where(x => x.Tag != null && x.Name == "bdr_" + control )
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
                button.Margin = new Thickness(5,0,5,0);

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
        int _MemberShipId = 0;
        List<CouponInvoice> selectedCopouns = new List<CouponInvoice>();
        List<ItemTransfer> invoiceItems = new List<ItemTransfer>();
        List<Tables> selectedTables = new List<Tables>();
        Invoice invoice = new Invoice();
        TablesReservation reservation = new TablesReservation();
        InvoicesClass invoiceMemberShipClass = new InvoicesClass();
        List<InvoicesClass> customerInvClasses = new List<InvoicesClass>();
        private void addRowToBill(Item item, long count)
        {
            decimal total = 0;
            var invoiceItem = billDetailsList.Where(x => x.itemId == item.itemId).FirstOrDefault();
            #region add item to invoice
            if (invoiceItem == null)
            {
                decimal price = 0;
                decimal basicPrice = (decimal)item.basicPrice;
                if (AppSettings.itemsTax_bool == true)
                    price = (decimal)item.priceTax;
                else
                    price = (decimal)item.price;

                int offerId = 0;
                string offerType = "1";
                decimal offerValue = 0;
                if (item.offerId != null)
                {
                    offerId = (int)item.offerId;
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
                    itemUnitId = (int)item.itemUnitId,
                    itemName = item.name,
                    Count = (int)count,
                    Price = (decimal)item.price,
                    basicPrice = basicPrice,
                    Total = (decimal)item.price,
                    offerId = item.offerId,
                    OfferType = offerType,
                    OfferValue = offerValue,
                    forAgents = item.forAgent,
                });
                _SequenceNum++;
            }
            #endregion
            #region item already exist in invoice
            else
            {
                invoiceItem.Count++;
                total = invoiceItem.Price * invoiceItem.Count;
                invoiceItem.Total = total;
            }
            #endregion
            BuildBillDesign();
            refreshTotal();
        }
        void BuildBillDesign()
        {
            //sv_billDetail.Children.Clear();
            #region Grid Container
            Grid gridContainer = new Grid();
            gridContainer.Margin = new Thickness(5);
            //int rowCount = billDetailsList.Count();
            int rowCount = 8;
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
            int colCount = 7;
            ColumnDefinition[] cd = new ColumnDefinition[colCount];
            for (int i = 0; i < colCount; i++)
            {
                cd[i] = new ColumnDefinition();
            }
            cd[0].Width = new GridLength(1, GridUnitType.Auto);
            cd[1].Width = new GridLength(1, GridUnitType.Auto);
            cd[2].Width = new GridLength(1, GridUnitType.Star);
            cd[3].Width = new GridLength(1, GridUnitType.Auto);
            cd[4].Width = new GridLength(1, GridUnitType.Auto);
            cd[5].Width = new GridLength(1, GridUnitType.Auto);
            cd[6].Width = new GridLength(1, GridUnitType.Star);
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
                indexText.Text = (item.index + 1)+".";
                indexText.Tag = ( "index-"+ item.index);
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
                buttonImage.Margin = new Thickness(5 , 10, 5, 10);
                buttonImage.VerticalAlignment = VerticalAlignment.Center;
                buttonImage.HorizontalAlignment = HorizontalAlignment.Center;
                buttonImage.BorderThickness = new Thickness(1);
                buttonImage.BorderBrush = Application.Current.Resources["Grey"] as SolidColorBrush;
                buttonImage.Padding = new Thickness(0);
                buttonImage.FlowDirection = FlowDirection.LeftToRight;
                MaterialDesignThemes.Wpf.ButtonAssist.SetCornerRadius(buttonImage, (new CornerRadius(10)));
                buttonImage.Cursor = Cursors.Arrow;
                //MaterialDesignThemes.Wpf.ShadowAssist.SetDarken(buttonImage, false);
                MaterialDesignThemes.Wpf.ShadowAssist.SetShadowDepth(buttonImage, ShadowDepth.Depth0);
                //MaterialDesignThemes.Wpf.ShadowAssist.SetShadowEdges(buttonImage,ShadowEdges.None);

                bool isModified = HelpClass.chkImgChng(item.image, (DateTime)it.updateDate, Global.TMPItemsFolder);
                if (isModified)
                   HelpClass.getImg("Item",item.image, buttonImage);
                else
                    HelpClass.getLocalImg("Item",item.image, buttonImage);

                Grid.SetRow(buttonImage, item.index);
                Grid.SetColumn(buttonImage, 1);
                gridContainer.Children.Add(buttonImage);
                /////////////////////////////////

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
                Grid.SetColumn(itemNameText, 2);
                gridContainer.Children.Add(itemNameText);
                #endregion
                #region   count
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
                Grid.SetColumn(countText, 4);
                gridContainer.Children.Add(countText);
                /////////////////////////////////

                #endregion
                #region   total
                var totalText = new TextBlock();
                totalText.Text = item.Total.ToString();
                totalText.Tag = "total-" + item.index;
                //totalText.Tag = item.index;
                totalText.Foreground = Application.Current.Resources["Grey"] as SolidColorBrush;
                totalText.Margin = new Thickness(5);
                totalText.VerticalAlignment = VerticalAlignment.Center;
                totalText.HorizontalAlignment = HorizontalAlignment.Right;

                Grid.SetRow(totalText, item.index);
                Grid.SetColumn(totalText, 6);
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
                if (item.Count <2)
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
                Grid.SetColumn(buttonMinus, 3);
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
                Grid.SetColumn(buttonPlus, 5);
                gridContainer.Children.Add(buttonPlus);
                /////////////////////////////////
                #endregion
                _SequenceNum++;
            }
            sv_billDetail.Content = gridContainer;
            
        }
        void buttonPlus_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int index = int.Parse(button.Tag.ToString().Replace("plus-", ""));
            //index--;
            billDetailsList[index].Count++;
            billDetailsList[index].Total = billDetailsList[index].Count * billDetailsList[index].Price;
            editBillRow(index);
            if (billDetailsList[index].Count == 2)
                refreshDeleteButtonInvoice(index,billDetailsList[index].Count);

            BuildBillDesign();
            refreshTotal();
            setKitchenNotification();
        }
        void buttonMinus_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int index = int.Parse(button.Tag.ToString().Replace("minus-", ""));
            int itemUnitId = billDetailsList[index].itemUnitId;
            int sentToKitchenCount = sentInvoiceItems.Where(x => x.itemUnitId == itemUnitId).Select(x => x.Count).Sum();
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
                    billDetailsList[index].Total = billDetailsList[index].Count * billDetailsList[index].Price;
                    editBillRow(index);
                    if (billDetailsList[index].Count == 1)
                        refreshDeleteButtonInvoice(index, billDetailsList[index].Count);
                }

                BuildBillDesign();
                refreshTotal();
                setKitchenNotification();
            }
           
        }
        void refreshDeleteButtonInvoice(int index,int count)
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
        void refreshTotal()
        {
            decimal total = 0;
            #region subtotal
            _Sum = 0;
            foreach(var item in billDetailsList)
            {
                _Sum += item.Total;
            }
            tb_subtotal.Text = _Sum.ToString();
            total = _Sum;
            #endregion

            #region discount
            decimal couponsDiscount = 0;
            decimal totalDiscount = 0;
            if (_Sum > 0)
            {
                #region calculate coupons discount
                if(selectedCopouns != null)
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
                if (manualDiscount !=0)
                {
                    if (_DiscountType == "2")
                        manualDiscount = HelpClass.calcPercentage(_Sum, manualDiscount);
                }
                #endregion

                #region customer invoice classes discount
                foreach (InvoicesClass c in customerInvClasses)
                {
                    if (_Sum >= c.minInvoiceValue && _Sum <= c.maxInvoiceValue)
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

            tb_total.Text = total.ToString();

        }
        void applyInvClassesOnTotal()
        {
            decimal total = 0;
            #region subtotal
            _Sum = 0;
            foreach(var item in billDetailsList)
            {
                _Sum += item.Total;
            }
            tb_subtotal.Text = _Sum.ToString();
            total = _Sum;
            #endregion

            #region discount
            decimal couponsDiscount = 0;
            decimal totalDiscount = 0;
            if (_Sum > 0)
            {
                #region calculate coupons discount
                if(selectedCopouns != null)
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
                if (_ManualDiscount !=0)
                {
                    if (_DiscountType == "2")
                        _ManualDiscount = HelpClass.calcPercentage(_Sum, _ManualDiscount);
                }
                #endregion
                totalDiscount = couponsDiscount + _ManualDiscount;
                //tb_totalDiscount.Text = totalDiscount.ToString();
                tb_totalDiscount.Text = HelpClass.PercentageDecTostring(totalDiscount);              
            }

            total = _Sum - totalDiscount;
            if (totalDiscount > 0)
            {
                txt_totalDiscount.Visibility = Visibility.Visible;
                tb_totalDiscount.Visibility = Visibility.Visible;
                tb_moneyIconDis.Visibility = Visibility.Visible;
                //tb_totalDiscount.Text = totalDiscount.ToString();
            }
            else
            {
                txt_totalDiscount.Visibility = Visibility.Collapsed;
                tb_totalDiscount.Visibility = Visibility.Collapsed;
                tb_moneyIconDis.Visibility = Visibility.Collapsed;
            }
            #endregion

            #region customer invoice classes discount
            foreach (InvoicesClass c in customerInvClasses)
            {
                decimal invClassDiscount = 0;
                if(_Sum >= c.minInvoiceValue && _Sum <= c.maxInvoiceValue)
                {
                    if(c.discountValue != 0)
                    {
                        invClassDiscount = c.discountValue;
                        if (c.discountType == 2)
                            invClassDiscount = HelpClass.calcPercentage(_Sum, invClassDiscount);
                    }

                    total -= invClassDiscount;

                    break;
                }
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

           
            tb_total.Text = total.ToString();

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
            if(AppSettings.invType =="takeAway" || AppSettings.invType =="selfService")
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
                int ordersCount = await FillCombo.invoice.GetCountByCreator(invoiceType, MainWindow.userLogin.userId, duration,hours);
                if (FillCombo.invoice != null && (_InvoiceType == "tsd" || _InvoiceType =="ssd") && invoice != null && invoice.invoiceId != 0 && !isFromReport)
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
                int ordersCount = await preparingOrder.GetHallOrderCount(status, MainWindow.branchLogin.branchId,duration);

                HelpClass.refreshNotification(md_ordersAlerts, ref _OrderCount, ordersCount);
            }
            catch { }
        }
        #endregion
        #region adddraft - addInvoice - cancleInvoice - clear - table names - fillInvoiceInputs - validate invoice values - refresh items price
        private async Task<int> addDraft()
        {
            int res = 0;
            if (AppSettings.invType == "diningHall")
            {
                
                if ( _InvoiceType == "sd" && selectedTables.Count > 0)
                {
                    res = await addInvoice("sd");
                    refreshOrdersNotification();

                }
              
            }
            else if(AppSettings.invType == "takeAway")
            {
                //if (billDetailsList.Count > 0)
                //{
                    if (invoice.invoiceId == 0)
                    {
                        //invoice.invNumber = await invoice.generateInvNumber("tsd", MainWindow.branchLogin.code, MainWindow.branchLogin.branchId);
                        invoice.invNumber = await invoice.generateDialyInvNumber("ssd,ss,tsd,ts", MainWindow.branchLogin.branchId);
                    }

                    res = await addInvoice("tsd");

                    refreshOrdersNotification();
                    refreshDraftNotification();
                //}

            }
            else if(AppSettings.invType == "selfService")
            {
                if (invoice.invoiceId == 0)
                {
                    //invoice.invNumber = await invoice.generateInvNumber("ssd", MainWindow.branchLogin.code, MainWindow.branchLogin.branchId);
                    invoice.invNumber = await invoice.generateDialyInvNumber("ssd,ss,tsd,ts", MainWindow.branchLogin.branchId);

                }
               res =  await addInvoice("ssd");

                refreshDraftNotification();
                refreshOrdersNotification();
            }
            return res;
        }

        async Task<int> addInvoice(string invType)
        {
            try
            {
                #region invoice object
                if (invoice.invoiceId == 0)
                {

                    invoice.createUserId = MainWindow.userLogin.userId;
                    invoice.branchId = MainWindow.branchLogin.branchId;
                    invoice.branchCreatorId = MainWindow.branchLogin.branchId;
                }
                invoice.updateUserId = MainWindow.userLogin.userId;
                invoice.invType = invType;
                invoice.discountValue = _ManualDiscount;
                invoice.discountType = _DiscountType ;
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
               foreach(var item in billDetailsList)
                {
                    itemT = new ItemTransfer();
                    itemT.invoiceId = 0;
                    itemT.quantity = item.Count;
                    itemT.price = item.Price;
                    itemT.itemUnitId = item.itemUnitId;
                    itemT.offerId = item.offerId;
                    itemT.offerType = decimal.Parse(item.OfferType);
                    itemT.offerValue = item.OfferValue;
                    itemT.itemTax = item.Tax;
                    itemT.itemUnitPrice = item.basicPrice;
                    itemT.createUserId = MainWindow.userLogin.userId;

                    invoiceItems.Add(itemT);
                }
                #endregion
                int res = await FillCombo.invoice.saveInvoiceWithItems(invoice, invoiceItems);

                invoice.invoiceId = res;
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
               
                int res = await FillCombo.invoice.saveInvoice(invoice);
                if (res > 0)
                {
                    Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopCanceled"), animation: ToasterAnimation.FadeIn);
                    #region update reservation status to cancle
                    if(invoice.reservationId != null)
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
            _InvoiceType = "sd";
            txt_tableName.Text = "";
            billDetailsList.Clear();
            _Sum = 0;
            _DeliveryCost = 0;
            _ManualDiscount = 0;
            _invoiceClassDiscount = 0;
            _DeliveryDiscount = 0;
            _MemberShipId = 0;
            _DiscountType = "";
            selectedCopouns.Clear() ;
            selectedTables.Clear();
            
            invoice = new Invoice();

            #region return waiter button to default
            //txt_waiter.Text = AppSettings.resourcemanager.GetString("trWaiter");
            txt_waiter.Text = "";
            txt_waiter.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            path_waiter.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
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
            await refreshItemsList();
            BuildBillDesign();
            refreshTotal();
        }

        private void setTablesName()
        {
            string str = "";
            foreach(Tables tbl in selectedTables)
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
                item = items.Where(x => x.itemId == it.itemId).FirstOrDefault();
                addRowToBill(item, it.quantity);
            }
        }
        private void refreshItemsPrice()
        {
            var tempBill = billDetailsList.ToList();
            billDetailsList = new ObservableCollection<BillDetailsSales>();
            foreach (BillDetailsSales it in tempBill)
            {
                item = items.Where(x => x.itemId == it.itemId).FirstOrDefault();
                addRowToBill(item, it.Count);
            }
        }
        async Task fillDiningHallInv()
        {
            //#region set parameters
            //_Sum = (decimal)invoice.total;
            //_DeliveryCost = (decimal)invoice.shippingCost;
            //_deliveryDiscount = (decimal)invoice.shippingCostDiscount;
            //_ManualDiscount = invoice.discountValue;
            //_DiscountType = invoice.discountType;
            //selectedCopouns = await FillCombo.invoice.GetInvoiceCoupons(invoice.invoiceId);

            //#endregion

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
                _MemberShipId = (int)customer.membershipId;
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
            //#region set parameters
            //_Sum = (decimal)invoice.total;
            //_DeliveryCost = (decimal)invoice.shippingCost;
            //_ManualDiscount = invoice.discountValue;
            //_DiscountType = invoice.discountType;
            //selectedCopouns = await FillCombo.invoice.GetInvoiceCoupons(invoice.invoiceId);

            //#endregion

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
                _MemberShipId = (int)customer.membershipId;
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
            //#region set parameters
            //_Sum = (decimal)invoice.total;
            //_DeliveryCost = (decimal)invoice.shippingCost;
            ////_ManualDiscount = invoice.discountValue;
            ////_DiscountType = invoice.discountType;
            ////selectedCopouns = await FillCombo.invoice.GetInvoiceCoupons(invoice.invoiceId);

            //#endregion

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
                _MemberShipId = (int)customer.membershipId;
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
                txt_tableName.Text =invoice.tables[0].name ;
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
                try { itemCountInOrder = kitchenOrders.Where(x => x.itemUnitId == b.itemUnitId).Sum(x => x.quantity); }
                catch { }

                //int remainingCount = (int)b.quantity - itemCountInOrder;

                BillDetailsSales newBillRow = new BillDetailsSales()
                {
                    itemUnitId = (int)b.itemUnitId,
                    itemName = b.itemName,
                    index = index,
                    Count = itemCountInOrder,
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
            foreach (ItemTransfer b in invoiceItems)
            {

                int itemCountInOrder = 0;
                try { itemCountInOrder = kitchenOrders.Where(x => x.itemUnitId == b.itemUnitId).Sum(x => x.quantity); }
                catch { }

                long countInInvoiceItems = billDetailsList.Where(x => x.itemUnitId == b.itemUnitId).Sum(x => x.Count);

                if(countInInvoiceItems == itemCountInOrder) // set btn_kitchen as default
                {
                    txt_kitchen.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                    path_kitchen.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                }
                else
                {
                    txt_kitchen.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                    path_kitchen.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
                    break;
                }
            }
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
                if(invoice.invoiceId != 0 || billDetailsList.Count >0)
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
                if(AppSettings.invType == "diningHall")
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
        private void Btn_invType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                Window.GetWindow(this).Opacity = 0.2;


                wd_selectInvType w = new wd_selectInvType();

                w.ShowDialog();
                if(w.isOk)
                {
                    //MessageBox.Show(w.invTypeValue);
                    //AppSettings.invType = w.invTypeValue;
                    changeInvType();
                   
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
                        int res = await addDraft();

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
                    if(invoice.waiterId != null)
                        w.userId = (int)invoice.waiterId;
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
                        int res = await FillCombo.invoice.saveInvoice(invoice);
                        if(res >0)
                            Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);
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
                    if(invoice.agentId != null)
                    w.customerId =(int) invoice.agentId;
                    w.ShowDialog();
                    if (w.isOk)
                    {
                   
                    customerInvClasses = new List<InvoicesClass>();

                        if (w.customerId > 0 )
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
                            txt_customer.Text = "";

                            txt_delivery.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                            path_delivery.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                            #endregion

                            #endregion

                            #region customer membership Info
                            var customer = FillCombo.customersList.Where(x => x.agentId == w.customerId).FirstOrDefault();
                            if (customer.membershipId != null)
                            {
                                _MemberShipId = (int)customer.membershipId;
                                _DeliveryDiscount = w.deliveryDiscount;
                                if (w.memberShipStatus == "valid")
                                {
                                    customerInvClasses = await invoiceMemberShipClass.GetInvclassByMembershipId(_MemberShipId);

                                }
                            }
                            else
                                _MemberShipId = 0;

                            #region refresh items with new pricel
                            if (w.hasOffers)
                            {
                                await refreshItemsList();
                                Search();
                                refreshItemsPrice();
                                //BuildBillDesign();
                            }
                            #endregion

                            #endregion
                            #region update invoice
                            invoice.agentId = w.customerId;
                            invoice.membershipId = customer.membershipId;
                            refreshTotal();
                            int res = await addDraft();
                            if (res > 0)
                            {
                                // save invoice memberShip discount class
                                if(invoiceMemberShipClass.invClassMemberId != 0)
                                    await FillCombo.invoice.saveMemberShipClassDis(invoiceMemberShipClass,invoice.invoiceId);
                                Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);

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
                        // return button content to default
                        _DeliveryDiscount = 0;
                        _MemberShipId = 0;
                            txt_customer.Text = "";
                        // return foreground color to default
                        txt_customer.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                            path_customer.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;

                        btn_delivery.IsEnabled = false;

                        refreshTotal();

                    }

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

                        refreshTotal();
                    #region change button Color
                    if(w.manualDiscount > 0 || w.selectedCopouns.Count>0)
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
                    int res = await addDraft();
                        if (res > 0)
                        {
                            Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);
                            await FillCombo.invoice.saveInvoiceCoupons(selectedCopouns,invoice.invoiceId,"sd");
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
                    _DeliveryCost = w._DeliveryCost;

                    refreshTotal();
                    #region change button Color
                    if (w.shippingCompanyId > 0)
                    {
                        var company = FillCombo.shippingCompaniesList.Where(x => x.shippingCompanyId == w.shippingCompanyId).FirstOrDefault();

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
                    #endregion

                    #region update invoice
                    invoice.shippingCompanyId = w.shippingCompanyId;
                    invoice.shipUserId = w.shippingUserId;
                    invoice.shippingCost = _DeliveryCost;
                    invoice.realShippingCost = w._RealDeliveryCost;
                    //invoice.total = _Sum;
                    //invoice.totalNet = decimal.Parse(tb_total.Text);
                    //invoice.paid = 0;
                    //invoice.deserved = invoice.totalNet;
                    //invoice.updateUserId = MainWindow.userLogin.userId;

                    int res = await addDraft();

                    if (res > 0)
                    {

                        Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);
                        await FillCombo.invoice.saveInvoiceCoupons(selectedCopouns, invoice.invoiceId, "sd");
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
                if(invoice.orderTime != null)
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

                    int res = await addDraft();

                    if (res > 0)
                    {
                        Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);
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
            if (decimal.Parse(tb_subtotal.Text) == 0)
            {
                Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trErrorTotalIsZeroToolTip"), animation: ToasterAnimation.FadeIn);
                return false;
            }
            return true;
        }

        private decimal getCusAvailableBlnc(Agent customer)
        {
            decimal remain = 0;

            decimal customerBalance = customer.balance;

            if (customer.balanceType == 0)
                remain = decimal.Parse(tb_total.Text) - (decimal)customerBalance;
            else
                remain = (decimal)customer.balance + decimal.Parse(tb_total.Text);
            return remain;
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
                        refreshTotal();

                       
                        #region payment window
                        Window.GetWindow(this).Opacity = 0.2;

                        wd_multiplePayment w = new wd_multiplePayment();
                        w.isPurchase = false;
                        w.invoice.invType = _InvoiceType;
                        w.invoice.totalNet = decimal.Parse(tb_total.Text);
                        w.cards = FillCombo.cardsList;

                        #region customer balance
                        if (invoice.agentId != null)
                        {
                            await FillCombo.RefreshCustomers();

                            var customer = FillCombo.customersList.ToList().Find(b => b.agentId == (int)invoice.agentId && b.isLimited == true);
                            if (customer != null)
                            {
                                decimal remain = 0;
                                if (customer.maxDeserve != 0)
                                    remain = getCusAvailableBlnc(customer);
                                w.hasCredit = true;
                                w.creditValue = remain;
                            }
                            else
                            {
                                w.hasCredit = false;
                                w.creditValue = 0;
                            }
                        }
                        #endregion
                        w.ShowDialog();
                        Window.GetWindow(this).Opacity = 1;
                        #endregion
                        if (w.isOk)
                        {
                           
                            paymentsList = w.listPayments;

                            if (AppSettings.invType == "diningHall")
                                await saveDiningHallInvoice("s");

                            else if (AppSettings.invType == "takeAway")
                                await saveTakeAwayInvoice("ts");

                            else if (AppSettings.invType == "selfService")
                                await saveTakeAwayInvoice("ss");

                        }                      
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
            int res = await addInvoice(invType);
            if (res > 0)
            {
                Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);

                #region savepayment
                
                await FillCombo.invoice.recordPosCashTransfer(invoice, "si");

                
                foreach (var item in paymentsList)
                {
                    await saveConfiguredCashTrans(item);
                    invoice.paid += item.cash;
                    invoice.deserved -= item.cash;
                }
                prinvoiceId = await invoice.saveInvoice(invoice);
                // refresh pos balance
                await MainWindow.refreshBalance();
                #endregion
                //await FillCombo.invoice.saveInvoiceCoupons(selectedCopouns, invoice.invoiceId, "s");
                #region close reservation
                if (invoice.reservationId != null)
                    await reservation.updateReservationStatus((long)invoice.reservationId, "close", MainWindow.userLogin.userId);
                #endregion

                await clear();
            }
            else
                Toaster.ShowError(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
        }

        private async Task saveTakeAwayInvoice(string invType)
        {
            if (invoice.invoiceId == 0)
            {
                //invoice.invNumber = await invoice.generateInvNumber("tsd", MainWindow.branchLogin.code, MainWindow.branchLogin.branchId);
                invoice.invNumber = await invoice.generateDialyInvNumber("ssd,ss,tsd,ts", MainWindow.branchLogin.branchId);
            }

            int res = await addInvoice("ts");
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

            foreach (BillDetailsSales b in billDetailsList)
            {
                ItemOrderPreparing it = new ItemOrderPreparing()
                {
                    itemUnitId = b.itemUnitId,
                    quantity = b.Count,
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

            int res = await preparingOrder.savePreparingOrders(preparingOrder, preparingItemsList, statusObject, MainWindow.branchLogin.branchId);

            if (AppSettings.statusesOfPreparingOrder == "directlyPrint")
            {
                #region save status = Preparing
                statusObject = new orderPreparingStatus();
                statusObject.orderPreparingId = res;
                statusObject.status = "Preparing";
                statusObject.createUserId = MainWindow.userLogin.userId;

                await preparingOrder.updateOrderStatus(statusObject);
                #endregion
                
                #region save status = Ready
                statusObject = new orderPreparingStatus();
                statusObject.orderPreparingId = res;
                statusObject.status = "Ready";
                statusObject.createUserId = MainWindow.userLogin.userId;

                await preparingOrder.updateOrderStatus(statusObject);
                #endregion

                #region save status = Done if no shipping
                if (invoice.shippingCompanyId == null)
                {
                    statusObject = new orderPreparingStatus();
                    statusObject.orderPreparingId = res;
                    statusObject.status = "Done";
                    statusObject.createUserId = MainWindow.userLogin.userId;

                    await preparingOrder.updateOrderStatus(statusObject);
                }
                #endregion
            }
        }

        async Task savePayments()
        {

            foreach (var item in paymentsList)
            {
                await saveConfiguredCashTrans(item);
                invoice.paid += item.cash;
                invoice.deserved -= item.cash;
            }
            prinvoiceId = await invoice.saveInvoice(invoice);
               
        }
        #endregion

    }
}
