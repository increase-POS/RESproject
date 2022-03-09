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
            tb_moneyIcon.Text = AppSettings.Currency;
            tb_moneyIconTotal.Text = AppSettings.Currency;
            if (AppSettings.lang.Equals("en"))
            {
                AppSettings.resourcemanager = new ResourceManager("Restaurant.en_file", Assembly.GetExecutingAssembly());
                grid_main.FlowDirection = FlowDirection.LeftToRight;
            }
            else
            {
                AppSettings.resourcemanager = new ResourceManager("Restaurant.ar_file", Assembly.GetExecutingAssembly());
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

            loading_items();
            loading_categories();
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
            refreshOrdersNotification();
            #endregion
            HelpClass.activateCategoriesButtons(items, FillCombo.categoriesList, categoryBtns);
           // FillBillDetailsList(0);
            await Search();
            
        }
        private void translate()
        {
            txt_orders.Text = AppSettings.resourcemanager.GetString("trOrders");         
            txt_allMenu.Text = AppSettings.resourcemanager.GetString("trAll");
            txt_ordersAlerts.Text = AppSettings.resourcemanager.GetString("trOrders");
            txt_newDraft.Text = AppSettings.resourcemanager.GetString("trNew");
            txt_preview.Text = AppSettings.resourcemanager.GetString("trPreview");
            txt_pdf.Text = AppSettings.resourcemanager.GetString("trPdf");
            txt_printInvoice.Text = AppSettings.resourcemanager.GetString("trPrint");
            txt_subtotal.Text = AppSettings.resourcemanager.GetString("trSubTotal");
            txt_totalDiscount.Text = AppSettings.resourcemanager.GetString("trDiscount");
            txt_tax.Text = AppSettings.resourcemanager.GetString("trTax");
            txt_total.Text = AppSettings.resourcemanager.GetString("trTotal");
            txt_discount.Text = AppSettings.resourcemanager.GetString("trDiscount");
            txt_customer.Text = AppSettings.resourcemanager.GetString("trCustomer");
            txt_waiter.Text = AppSettings.resourcemanager.GetString("trWaiter");
            txt_kitchen.Text = AppSettings.resourcemanager.GetString("trKitchen");
            txt_tables.Text = AppSettings.resourcemanager.GetString("trTables");
                   
            btn_pay.Content = AppSettings.resourcemanager.GetString("trPay");

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
                if (item != null && selectedTables.Count>0)
                {
                    item = items.Where(x => x.itemId == itemId).FirstOrDefault();
                    addRowToBill(item,1);
                }
                else if(selectedTables.Count == 0)
                    Toaster.ShowWarning(Window.GetWindow(this), message:  AppSettings.resourcemanager.GetString("trChooseTableFirst"), animation: ToasterAnimation.FadeIn);

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
           items = await FillCombo.item.GetAllSalesItemsInv(day.ToLower());
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
            Tag allTag = new Tag();
            allTag.tagName = AppSettings.resourcemanager.GetString("trAll");
            allTag.tagId = 0;
            tagsList.Add(allTag);
            sp_menuTags.Children.Clear();
            foreach (var item in tagsList)
            {
                #region  
                Button button = new Button();
                button.Content = item.tagName;
                button.Tag = "catalogTags-" + item.tagName;
                button.FontSize = 10;
                button.Height = 25;
                button.Padding = new Thickness(5);
                MaterialDesignThemes.Wpf.ButtonAssist.SetCornerRadius(button, (new CornerRadius(7)));
                button.Margin = new Thickness(5,0,5,0);
                button.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                button.Background = Application.Current.Resources["White"] as SolidColorBrush;
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
        string _DiscountType = "";
        List<CouponInvoice> selectedCopouns = new List<CouponInvoice>();
        List<ItemTransfer> invoiceItems = new List<ItemTransfer>();
        List<Tables> selectedTables = new List<Tables>();
        Invoice invoice = new Invoice();
        TablesReservation reservation = new TablesReservation();
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
        }
        void buttonMinus_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int index = int.Parse(button.Tag.ToString().Replace("minus-", ""));
           // index--;
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
                if (_ManualDiscount !=0)
                {
                    if (_DiscountType == "2")
                        _ManualDiscount = HelpClass.calcPercentage(_Sum, _ManualDiscount);
                }
                #endregion
                totalDiscount = couponsDiscount + _ManualDiscount;
                tb_totalDiscount.Text = totalDiscount.ToString();
            }

            total = _Sum - totalDiscount;
            if (totalDiscount > 0)
            {
                txt_totalDiscount.Visibility = Visibility.Visible;
                tb_totalDiscount.Visibility = Visibility.Visible;
                tb_moneyIconDis.Visibility = Visibility.Visible;
                tb_totalDiscount.Text = totalDiscount.ToString();
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
    

            tb_total.Text = total.ToString();

        }
        #endregion
        #region timer to refresh notifications
        private static DispatcherTimer timer;
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
        async Task refreshOrdersNotification()
        {
            try
            {
                string invoiceType = "sd";
                int duration = 1;
                int ordersCount = await FillCombo.invoice.GetCountByCreator(invoiceType, MainWindow.userLogin.userId, duration);
                if (FillCombo.invoice != null && _InvoiceType == "sd" && FillCombo.invoice != null && FillCombo.invoice.invoiceId != 0 && !isFromReport)
                    ordersCount--;

                HelpClass.refreshNotification(md_ordersAlerts, ref _OrderCount, ordersCount);
            }
            catch { }
        }
        #endregion
        #region adddraft - addInvoice - cancleInvoice - clear - table names - fillInvoiceInputs
        private async Task addDraft()
        {
            if (billDetailsList.Count > 0 && _InvoiceType == "sd" && selectedTables.Count > 0)
            {
                #region Accept
                MainWindow.mainWindow.Opacity = 0.2;
                wd_acceptCancelPopup w = new wd_acceptCancelPopup();
                w.contentText = AppSettings.resourcemanager.GetString("trSaveInvoiceNotification");
                w.ShowDialog();
                MainWindow.mainWindow.Opacity = 1;
                #endregion
                if (w.isOk)
                {
                    await addInvoice("sd");
                    clear();
                    refreshOrdersNotification();
                }
                else
                {
                    clear();
                }
            }
            else
                clear();
        }

        async Task addInvoice(string invType)
        {
            try
            {
                #region invoice object
                invoice.invType = invType;
                invoice.discountValue = _ManualDiscount;
                invoice.discountType = _DiscountType ;

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
                if(res > 0)
                    Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);
                else
                    Toaster.ShowError(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
            }
            catch
            {

            }
        }
        async Task cancleInvoice(string invType)
        {
            try
            {
                invoice.invType = invType;
               
                int res = await FillCombo.invoice.saveInvoice(invoice);
                if(res > 0)
                    Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopCanceled"), animation: ToasterAnimation.FadeIn);
                else
                    Toaster.ShowError(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
            }
            catch
            {

            }
        }
        void clear()
        {
            _InvoiceType = "sd";
            txt_tableName.Text = "";
            billDetailsList.Clear();
            _Sum = 0;
            _ManualDiscount = 0;
            _DiscountType = "";
            selectedCopouns.Clear() ;
            selectedTables.Clear();
            invoice = new Invoice();
            txt_waiter.Text = AppSettings.resourcemanager.GetString("trWaiter");

            //last
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
            #region set parameters
            _Sum = (decimal)invoice.total;
            _ManualDiscount = invoice.discountValue;
            _DiscountType = invoice.discountType;
            selectedCopouns = await FillCombo.invoice.GetInvoiceCoupons(invoice.invoiceId);
            if(invoice.waiterId != null)
                txt_waiter.Text = AppSettings.resourcemanager.GetString("trChangeWaiter");
            #endregion

            #region text values
            if (invoice.total != 0)
                tb_subtotal.Text = HelpClass.DecTostring(invoice.total);
            else
                tb_subtotal.Text = "0";

            if (invoice.totalNet != 0)
                tb_total.Text = HelpClass.DecTostring(invoice.totalNet);
            else tb_total.Text = "0";

            if (invoice.tax != 0) 
                tb_tax.Text = HelpClass.DecTostring(invoice.tax);
            else
                tb_tax.Text = "0";
            #endregion

            #region invoice items
            invoiceItems = await FillCombo.invoice.GetInvoicesItems(invoice.invoiceId);
            foreach(ItemTransfer it in invoiceItems)
            {
                item = items.Where(x => x.itemId == it.itemId).FirstOrDefault();
                addRowToBill(item, it.quantity);
            }
            #endregion
        }
        #endregion
        #region buttons new - orders - tables - customers - waiter - kitchen

        private async void Btn_newDraft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                if (FillCombo.groupObject.HasPermissionAction(invoicePermission, FillCombo.groupObjects, "one"))
                {
                    await addDraft();
                }
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Btn_pay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                if (FillCombo.groupObject.HasPermissionAction(invoicePermission, FillCombo.groupObjects, "one"))
                {
                    if (selectedTables.Count == 0)
                        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trChooseTableFirst"), animation: ToasterAnimation.FadeIn);
                    else if(billDetailsList.Count > 0)
                    {
                        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trAddInvoiceWithoutItems"), animation: ToasterAnimation.FadeIn);
                    }
                    else if (invoice.invoiceId != 0)
                    {
                        await addInvoice("s");
                        await FillCombo.invoice.saveInvoiceCoupons(selectedCopouns, invoice.invoiceId, "s");
                        if (invoice.reservationId != null)
                            await reservation.updateReservationStatus((long)invoice.reservationId,"close",MainWindow.userLogin.userId);
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
        private async void Btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                if (FillCombo.groupObject.HasPermissionAction(invoicePermission, FillCombo.groupObjects, "one"))
                {
                    await cancleInvoice("sc");
                }
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

                if (FillCombo.groupObject.HasPermissionAction(addTablePermission, FillCombo.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    Window.GetWindow(this).Opacity = 0.2;
                    wd_diningHallTables w = new wd_diningHallTables();
                    w.ShowDialog();
                    if (w.isOk == true)
                    {
                        selectedTables = w.selectedTables;
                        invoice = w.invoice;


                        setTablesName();
                        await fillInvoiceInputs(invoice);
                    }
                    Window.GetWindow(this).Opacity = 1;
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_kitchen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                //if (FillCombo.groupObject.HasPermissionAction(addRangePermission, FillCombo.groupObjects, "one") || HelpClass.isAdminPermision())
                //{
                Window.GetWindow(this).Opacity = 0.2;
                wd_diningHallKitchen w = new wd_diningHallKitchen();
                w.ShowDialog();
                Window.GetWindow(this).Opacity = 1;
                // }
                //else
                //    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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

               if (FillCombo.groupObject.HasPermissionAction(addRangePermission, FillCombo.groupObjects, "one") || HelpClass.isAdminPermision())
               {
                    Window.GetWindow(this).Opacity = 0.2;
                    wd_selectUser w = new wd_selectUser();
                    w.userJob = "waiter";
                    if(invoice.waiterId != null)
                        w.userId = (int)invoice.waiterId;
                    w.ShowDialog();
                    if (w.isOk)
                    {
                        if(w.userId > 0)
                        {
                                invoice.waiterId = w.userId;
                                txt_waiter.Text = AppSettings.resourcemanager.GetString("trChangeWaiter");
                            // change button content
                            // change foreground color

                        }
                        else
                        {                           
                        // return button content to default
                        // return foreground color to default
                        }
                        int res = await FillCombo.invoice.saveInvoice(invoice);
                        if(res >0)
                            Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);
                        else
                            Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);

                        }
                        Window.GetWindow(this).Opacity = 1;
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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

                if (FillCombo.groupObject.HasPermissionAction(addRangePermission, FillCombo.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    Window.GetWindow(this).Opacity = 0.2;
                    wd_selectCustomer w = new wd_selectCustomer();
                    if(invoice.agentId != null)
                    w.customerId =(int) invoice.agentId;
                    w.ShowDialog();
                    if (w.isOk)
                    {
                        if (w.customerId > 0)
                        {
                            invoice.agentId = w.customerId;
                            invoice.updateUserId = MainWindow.userLogin.userId;

                            int res = await FillCombo.invoice.saveInvoice(invoice);
                            if (res > 0)
                            {
                                Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);

                            }
                            else
                                Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                            // test if id chnage
                            // change button content
                            // change foreground color

                        }
                        else
                        {
                            // return button content to default
                            // return foreground color to default
                        }
                    }
                    Window.GetWindow(this).Opacity = 1;
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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

                if (FillCombo.groupObject.HasPermissionAction(addRangePermission, FillCombo.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    Window.GetWindow(this).Opacity = 0.2;
                    wd_selectDiscount w = new wd_selectDiscount();
                    w._Sum = _Sum;
                    w.selectedCopouns = selectedCopouns;
                    w.manualDiscount = _ManualDiscount;
                    w.discountType = _DiscountType;
                    w.ShowDialog();
                    if (w.isOk)
                    {
                        _ManualDiscount = w.manualDiscount;
                        _DiscountType = w.discountType;
                        selectedCopouns = w.selectedCopouns;

                        refreshTotal();
                        #region update invoice
                        invoice.discountValue = _ManualDiscount;
                        invoice.discountType = _DiscountType;

                        invoice.total = _Sum;
                        invoice.totalNet = decimal.Parse(tb_total.Text);
                        invoice.paid = 0;
                        invoice.deserved = invoice.totalNet;
                        invoice.updateUserId = MainWindow.userLogin.userId;

                        int res = await FillCombo.invoice.saveInvoice(invoice);
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
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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
