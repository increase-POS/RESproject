using MaterialDesignThemes.Wpf;
using netoaster;
using Restaurant.Classes;
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

namespace Restaurant.View.sales
{
    /// <summary>
    /// Interaction logic for uc_diningHall.xaml
    /// </summary>
    public partial class uc_diningHall : UserControl
    {
        private static uc_diningHall _instance;
        string invoicePermission = "saleInvoice_invoice";

        public static uc_diningHall Instance
        {
            get
            {
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
            tb_moneyIcon.Text = MainWindow.Currency;
            tb_moneyIconTotal.Text = MainWindow.Currency;
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
            HelpClass.activateCategoriesButtons(items, FillCombo.categoriesList, categoryBtns);
           // FillBillDetailsList(0);
            await Search();        
        }
        private void translate()
        {
            txt_orders.Text = MainWindow.resourcemanager.GetString("trOrders");         
            txt_allMenu.Text = MainWindow.resourcemanager.GetString("trAll");
            txt_ordersAlerts.Text = MainWindow.resourcemanager.GetString("trOrders");
            txt_newDraft.Text = MainWindow.resourcemanager.GetString("trNew");
            txt_preview.Text = MainWindow.resourcemanager.GetString("trPreview");
            txt_pdf.Text = MainWindow.resourcemanager.GetString("trPdf");
            txt_printInvoice.Text = MainWindow.resourcemanager.GetString("trPrint");
            txt_subtotal.Text = MainWindow.resourcemanager.GetString("trSubTotal");
            txt_totalDiscount.Text = MainWindow.resourcemanager.GetString("trDiscount");
            txt_tax.Text = MainWindow.resourcemanager.GetString("trTax");
            txt_total.Text = MainWindow.resourcemanager.GetString("trTotal");
            txt_discount.Text = MainWindow.resourcemanager.GetString("trDiscount");
            txt_customer.Text = MainWindow.resourcemanager.GetString("trCustomer");
            txt_waiter.Text = MainWindow.resourcemanager.GetString("trWaiter");
            txt_kitchen.Text = MainWindow.resourcemanager.GetString("trKitchen");
            txt_tables.Text = MainWindow.resourcemanager.GetString("trTables");
                   
            btn_pay.Content = MainWindow.resourcemanager.GetString("trPay");

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
            catigoriesAndItemsView.FN_refrishCatalogItem(_items.ToList(), "sales");
        }
        #endregion
        #region Get Id By Click  Y

        public void ChangeItemIdEvent(int itemId)
        {
            try
            {
                if (item != null)
                {
                    item = items.Where(x => x.itemId == itemId).FirstOrDefault();
                    addRowToBill(item);
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
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
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
                
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
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
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
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

                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
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
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
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
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
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
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
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
            allTag.tagName = MainWindow.resourcemanager.GetString("trAll");
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
        List<ItemTransfer> invoiceItems = new List<ItemTransfer>();
        private void addRowToBill(Item item)
        {
            decimal total = 0;
            var invoiceItem = billDetailsList.Where(x => x.itemId == item.itemId).FirstOrDefault();
            #region add item to invoice
            if (invoiceItem == null)
            {
                decimal price = 0;
                decimal basicPrice = (decimal)item.price;
                //if (FillCombo.itemsTax_bool == true)
                //    price = (decimal)item.priceTax;
                //else
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
                _SequenceNum++;
                total = (decimal)item.price;
                billDetailsList.Add(new BillDetailsSales()
                {
                    index = _SequenceNum,
                    image = item.image,
                    itemId = item.itemId,
                    itemUnitId = (int)item.itemUnitId,
                    itemName = item.name,
                    Count = 1,
                    Price = (decimal)item.price,
                    basicPrice = basicPrice,
                    Total = (decimal)item.price,
                    offerId = item.offerId,
                    OfferType = offerType,
                    OfferValue = offerValue,
                });
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
        private async Task FillBillDetailsList(int invoiceId)
        {
            ////get invoice items
            //invoiceItems = await invoiceModel.GetInvoicesItems(invoiceId);
            //// build invoice details grid
            billDetailsList.Clear();
            //test
            Random r = new Random();
            //int rInt = r.Next(0, 100);
                for (int i = 0; i < 7; i++)
                {

                 int   _Count = r.Next(1, 10);
                Decimal _Price = r.Next(0, 10000);
                billDetailsList.Add(new BillDetailsSales()
                {
                    index = i,
                    image = "/pic/hamburger.jfif",
                    itemId = (i + 1),
                    itemName = "name - " + (i + 1),
                    Count = _Count,
                    Price = _Price,
                    Total = _Count * _Price,
                    offerId =0,
                });
            }
            //
            //int indexer = 0;
            //foreach (ItemTransfer itemT in invoiceItems)
            //{
            //    decimal total = (decimal)(itemT.price * itemT.quantity);
            //    billDetailsList.Add(new BillDetails()
            //    {
            //        ID = indexer,
            //        Product = itemT.itemName,
            //        itemId = (int)itemT.itemId,
            //        Unit = itemT.itemUnitId.ToString(),
            //        itemUnitId = (int)itemT.itemUnitId,
            //        Count = (int)itemT.quantity,
            //        Price = (decimal)itemT.price,
            //        Total = total,
            //        serialList = serialNumLst,
            //        type = itemT.itemType,
            //        valid = isValid,
            //        offerId = itemT.offerId,
            //    });
            //    indexer++;
            //}
            BuildBillDesign();
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
            foreach (var item in billDetailsList)
            {
                var it = items.Where(x => x.itemId == item.itemId).FirstOrDefault();
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
            }
            sv_billDetail.Content = gridContainer;
        }
        void buttonPlus_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int index = int.Parse(button.Tag.ToString().Replace("plus-", ""));
            index--;
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
            index--;
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
            #region subtotal
            _Sum = 0;
            foreach(var item in billDetailsList)
            {
                _Sum += item.Total;
            }
            tb_subtotal.Text = _Sum.ToString();
            #endregion

            #region total
            decimal total = _Sum;
            tb_total.Text = _Sum.ToString();
            #endregion
        }
        #endregion

        #region adddraft - addInvoice - clear
        private async Task addDraft()
        {
            if (billDetailsList.Count > 0 && _InvoiceType == "sd")
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
                    await addInvoice("sd");
                    clear();
                    //refreshDraftNotification();
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
                FillCombo.invoice.invNumber = await FillCombo.invoice.generateInvNumber("sd", MainWindow.branchLogin.code, MainWindow.branchLogin.branchId);
                FillCombo.invoice.branchCreatorId = MainWindow.branchLogin.branchId;
                FillCombo.invoice.branchId = MainWindow.branchLogin.branchId;
                FillCombo.invoice.posId = MainWindow.posLogin.posId;
                FillCombo.invoice.invType = invType;
                if (!tb_totalDiscount.Text.Equals(""))
                    FillCombo.invoice.discountValue = decimal.Parse(tb_totalDiscount.Text);
                FillCombo.invoice.discountType = "1";

                FillCombo.invoice.total = _Sum;
                FillCombo.invoice.totalNet = decimal.Parse(tb_total.Text);
                FillCombo.invoice.paid = 0;
                FillCombo.invoice.deserved = FillCombo.invoice.totalNet;
                if (tb_tax.Text != "")
                    FillCombo.invoice.tax = decimal.Parse(tb_tax.Text);
                else
                    FillCombo.invoice.tax = 0;
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
                int res = await FillCombo.invoice.saveInvoiceWithItems(FillCombo.invoice, invoiceItems);
                if(res > 0)
                    Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);
                else
                    Toaster.ShowError(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
            }
            catch
            {

            }
        }
        void clear()
        {
            _InvoiceType = "sd";
            billDetailsList.Clear();
            FillCombo.invoice = new Invoice();

            //last
            BuildBillDesign();
            refreshTotal();
        }
        #endregion
        #region buttons new - orders - tables - customers - waiter - kitchen

        private async void Btn_newDraft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                if (MainWindow.groupObject.HasPermissionAction(invoicePermission, MainWindow.groupObjects, "one"))
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
        private void Btn_tables_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                //if (MainWindow.groupObject.HasPermissionAction(addRangePermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
                //{
                    Window.GetWindow(this).Opacity = 0.2;
                    wd_diningHallTables w = new wd_diningHallTables();
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
        private void Btn_kitchen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                //if (MainWindow.groupObject.HasPermissionAction(addRangePermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
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
        private void Btn_waiter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                //if (MainWindow.groupObject.HasPermissionAction(addRangePermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
                //{
                Window.GetWindow(this).Opacity = 0.2;
                wd_selectUser w = new wd_selectUser();
                w.userJob = "waiter";
                w.ShowDialog();
                if (w.isOk)
                {
                    if(w.userId > 0)
                    {
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

        private void Btn_customer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                //if (MainWindow.groupObject.HasPermissionAction(addRangePermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
                //{
                Window.GetWindow(this).Opacity = 0.2;
                wd_selectCustomer w = new wd_selectCustomer();
                w.ShowDialog();
                if (w.isOk)
                {
                    if (w.customerId > 0)
                    {
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

        private void Btn_discount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                //if (MainWindow.groupObject.HasPermissionAction(addRangePermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
                //{
                Window.GetWindow(this).Opacity = 0.2;
                wd_selectDiscount w = new wd_selectDiscount();
                w.ShowDialog();
                if (w.isOk)
                {
                     
                }
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
        #endregion


    }
}
