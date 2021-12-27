using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using netoaster;
using POS.Classes;

namespace POS.View.windows
{
    /// <summary>
    /// Interaction logic for wd_items.xaml
    /// </summary>
    public partial class wd_items : Window
    {
        public wd_items()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                SectionData.ExceptionMessage(ex, this);
            }
        }
        /// <summary>
        /// item is select
        /// </summary>
        /// 
        public string CardType { get; set; }
        public int selectedItem { get; set; }
        public List<int> selectedItems { get; set; }
        Item itemModel = new Item();
        Category categoryModel = new Category();
        Unit unitModel = new Unit();
        Property propertyModel = new Property();
        PropertiesItems propItemsModel = new PropertiesItems();
        ItemsProp itemsPropModel = new ItemsProp();
        Serial serialModel = new Serial();
        ItemUnit itemUnitModel = new ItemUnit();
        Service serviceModel = new Service();
        IEnumerable<Category> categoriesQuery;
        IEnumerable<Item> items;
        IEnumerable<Item> itemsQuery;
        Category category = new Category();
        // item object
        Item item = new Item();

        List<int> categoryIds = new List<int>();
        List<string> categoryNames = new List<string>();
        List<Category> categories;
        public byte tglCategoryState = 1;
        public byte tglItemState = 1;
        int? categoryParentId = 0;
        public string txtItemSearch;
        CatigoriesAndItemsView catigoriesAndItemsView = new CatigoriesAndItemsView();

        public bool isActive;

        #region loading
        public class loadingThread
        {
            public string name { get; set; }
            public bool value { get; set; }
        }
        List<keyValueBool> loadingList;
        async void loading_RefrishItems()
        {
            try
            {
                await RefrishItems();
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
        async void loading_RefrishCategories()
        {
            try
            {
                await RefrishCategories();
            }
            catch (Exception)
            { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_RefrishCategories"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async void loading_RefrishCategoriesCard()
        {
            try
            {
                await RefrishCategoriesCard();
            }
            catch (Exception)
            { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_RefrishCategoriesCard"))
                {
                    item.value = true;
                    break;
                }
            }
        }


        #endregion
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_ucItems);
                grid_ucItems.Opacity = 1;

                // for pagination onTop Always
                btns = new Button[] { btn_firstPage, btn_prevPage, btn_activePage, btn_nextPage, btn_lastPage };
                catigoriesAndItemsView.wdItems = this;

                #region translate
                if (MainWindow.lang.Equals("en"))
                {
                    MainWindow.resourcemanager = new ResourceManager("POS.en_file", Assembly.GetExecutingAssembly());
                    grid_ucItems.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    MainWindow.resourcemanager = new ResourceManager("POS.ar_file", Assembly.GetExecutingAssembly());
                    grid_ucItems.FlowDirection = FlowDirection.RightToLeft;
                }
                translate();
                #endregion
                #region loading
                loadingList = new List<keyValueBool>();
                bool isDone = true;
                loadingList.Add(new keyValueBool { key = "loading_RefrishItems", value = false });
                loadingList.Add(new keyValueBool { key = "loading_RefrishCategories", value = false });
                loadingList.Add(new keyValueBool { key = "loading_RefrishCategoriesCard", value = false });


                loading_RefrishItems();
                loading_RefrishCategories();
                loading_RefrishCategoriesCard();
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
                        //MessageBox.Show("not done");
                        //string s = "";
                        //foreach (var item in loadingList)
                        //{
                        //    s += item.name + " - " + item.value + "\n";
                        //}
                        //MessageBox.Show(s);
                        await Task.Delay(0500);
                        //MessageBox.Show("do");
                    }
                }
                while (!isDone);
                #endregion


                Txb_searchitems_TextChanged(null, null);
               
                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
                SectionData.ExceptionMessage(ex, this);
            }
        }
        private void translate()
        {
            txt_items.Text = MainWindow.resourcemanager.GetString("trItems");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(txb_searchitems, MainWindow.resourcemanager.GetString("trSearchHint"));
            btn_add.Content = MainWindow.resourcemanager.GetString("trAdd");
            tt_grid.Content = MainWindow.resourcemanager.GetString("trViewGrid");
            tt_items.Content = MainWindow.resourcemanager.GetString("trViewItems");

        }
        
        private void Btn_colse_Click(object sender, RoutedEventArgs e)
        {
            isActive = false;
            this.Close();
        }

        #region Categor and Item
        #region Refrish Y
        /// <summary>
        /// Category
        /// </summary>
        /// <returns></returns>
        async Task<IEnumerable<Category>> RefrishCategories()
        {
            categories = await categoryModel.GetAllCategories(MainWindow.userID.Value);
            return categories;
        }
        async Task RefrishCategoriesCard()
        {
            if (categories is null)
                await RefrishCategories();
            categoriesQuery = categories.Where(x => x.isActive == tglCategoryState && x.parentId == categoryParentId);
            catigoriesAndItemsView.gridCatigories = grid_categoryCards;
            generateCoulmnCategoriesGrid(categoriesQuery.Count());
            catigoriesAndItemsView.FN_refrishCatalogCard(categoriesQuery.ToList(), -1);
        }
        void generateCoulmnCategoriesGrid(int column)
        {
            #region
            grid_categoryCards.ColumnDefinitions.Clear();
            ColumnDefinition[] cd = new ColumnDefinition[column];
            for (int i = 0; i < column; i++)
            {
                cd[i] = new ColumnDefinition();
                cd[i].Width = new GridLength(85, GridUnitType.Pixel);
                grid_categoryCards.ColumnDefinitions.Add(cd[i]);
            }
            #endregion
        }
        /// <summary>
        /// Item
        /// </summary>
        /// <returns></returns>

        async Task<IEnumerable<Item>> RefrishItems()
        {
            short defaultSale = 0;
            short defaultPurchase = 0;
            int branchId = MainWindow.branchID.Value;
            selectedItems = new List<int>();
            if (CardType.Equals("sales"))
            {
                defaultSale = 1;
                defaultPurchase = 0;
            }
            else if (CardType.Equals("purchase"))
            {
                defaultPurchase = 1;
                defaultSale = 0;
            }
            else if (CardType.Equals("order"))
            {
                defaultPurchase = 0;
                defaultSale = 0;
            }
            else if (CardType.Equals("movement"))
            {
                defaultPurchase = -1;
                defaultSale = -1;
            }
            items = await itemModel.GetSaleOrPurItems(category.categoryId,defaultSale,defaultPurchase,branchId);
            MainWindow.InvoiceGlobalItemsList = items.ToList();
            if (defaultSale == 1)
                MainWindow.InvoiceGlobalSaleUnitsList = await itemUnitModel.GetForSale();
            else
                MainWindow.InvoiceGlobalItemUnitsList = await itemUnitModel.Getall();
            return items;
        }

        void RefrishItemsDatagrid(IEnumerable<Item> _items)
        {
            dg_items.ItemsSource = _items;
        }


        void RefrishItemsCard(IEnumerable<Item> _items)
        {
            grid_itemContainerCard.Children.Clear();
            catigoriesAndItemsView.gridCatigorieItems = grid_itemContainerCard;
            catigoriesAndItemsView.FN_refrishCatalogItem(_items.ToList(), "en", CardType);
        }
        #endregion
        #region Get Id By Click  Y
        private void dg_items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_ucItems);

                if (dg_items.SelectedIndex != -1)
            {
                item = dg_items.SelectedItem as Item;
                int isExist = selectedItems.IndexOf(item.itemId);
                if (isExist == -1)
                {
                    var b = new MaterialDesignThemes.Wpf.Chip()
                    {
                        Content = item.name,
                        Name = "btn" + item.itemId,
                        IsDeletable = true,
                        Margin = new Thickness(5, 5, 5, 5)
                    };
                    b.DeleteClick += Chip_OnDeleteClick;
                    lst_items.Children.Add(b);
                    selectedItems.Add(item.itemId);
                }

            }
                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
                SectionData.ExceptionMessage(ex, this);
            }

        }
        public async Task ChangeCategoryIdEvent(int categoryId)
        {
            category = categories.ToList().Find(c => c.categoryId == categoryId);

            if (categories.Where(x =>
            x.isActive == tglCategoryState && x.parentId == category.categoryId).Count() != 0)
            {
                categoryParentId = category.categoryId;
                await RefrishCategoriesCard();
            }

            await generateTrack(categoryId);
            await RefrishItems();
            Txb_searchitems_TextChanged(null, null);
        }
        private void Chip_OnDeleteClick(object sender, RoutedEventArgs e)
        {
            var currentChip = (Chip)sender;
            lst_items.Children.Remove(currentChip);
            selectedItems.Remove(Convert.ToInt32(currentChip.Name.Remove(0, 3)));
        }
        public void ChangeItemIdEvent(int itemId)
        {
            int isExist = -1;
            if (selectedItems != null)
                isExist = selectedItems.IndexOf(itemId);
            var item = items.ToList().Find(x => x.itemId == itemId);
            if (isExist == -1)
            {
                var b = new MaterialDesignThemes.Wpf.Chip()
                {
                    Content = item.name,
                    Name = "btn" + item.itemId,
                    IsDeletable = true,
                    Margin = new Thickness(5, 5, 5, 5)
                };
                b.DeleteClick += Chip_OnDeleteClick;
                lst_items.Children.Add(b);
                selectedItems.Add(itemId);
            }
        }

        #endregion
        #region Grid Definition
        /*
        ColumnDefinition[] c;
        RowDefinition[] r;
        Grid gridItemContainerCard = new Grid();
        int[] count;
        void CreateGridCardContainer()
        {
            gridItemContainerCard.Name = "grid_itemContainerCard";
            gridItemContainerCard.Background = null;
            Grid.SetColumnSpan(gridItemContainerCard, 2);
            count = CatigoriesAndItemsView.itemsRowColumnCount(1, 3);
            c = new ColumnDefinition[count[1]];
            for (int i = 0; i < count[1]; i++)
            {
                //ColumnDefinition c1 = new ColumnDefinition();
                c[i] = new ColumnDefinition();
                c[i].Width = new GridLength(1, GridUnitType.Star);
                gridItemContainerCard.ColumnDefinitions.Add(c[i]);
            }
            r = new RowDefinition[count[0]];
            for (int i = 0; i < count[0]; i++)
            {
                r[i] = new RowDefinition();
                r[i].Height = new GridLength(1, GridUnitType.Star);
                gridItemContainerCard.RowDefinitions.Add(r[i]);
            }


            grid_itemContainerCard.Children.Clear();
            grid_itemContainerCard.Children.Add(gridItemContainerCard);
        }
        */
        #endregion
      
        #region Switch Card/DataGrid Y

        private void Btn_itemsInCards_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                grid_itemsDatagrid.Visibility = Visibility.Collapsed;
                grid_itemCards.Visibility = Visibility.Visible;
                path_itemsInCards.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#178DD2"));
                path_itemsInGrid.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#4e4e4e"));

                Txb_searchitems_TextChanged(null, null);
            }
            catch (Exception ex)
            {
                SectionData.ExceptionMessage(ex, this);
            }
        }

        private void Btn_itemsInGrid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                grid_itemCards.Visibility = Visibility.Collapsed;
                grid_itemsDatagrid.Visibility = Visibility.Visible;
                path_itemsInGrid.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#178DD2"));
                path_itemsInCards.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#4e4e4e"));

                Txb_searchitems_TextChanged(null, null);
            }
            catch (Exception ex)
            {
                SectionData.ExceptionMessage(ex, this);
            }
        }
        #endregion
        #region Search Y


        /// <summary>
        /// Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Txb_searchitems_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_ucItems);

                if (items is null)
                await RefrishItems();
                txtItemSearch = txb_searchitems.Text.ToLower();
                pageIndex = 1;

                #region
                itemsQuery = items.Where(x => (x.code.ToLower().Contains(txtItemSearch) ||
                x.name.ToLower().Contains(txtItemSearch) ||
                x.details.ToLower().Contains(txtItemSearch)
                ));

                if (btns is null)
                    btns = new Button[] { btn_firstPage, btn_prevPage, btn_activePage, btn_nextPage, btn_lastPage };
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
                #endregion
                RefrishItemsDatagrid(itemsQuery);

                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
                SectionData.ExceptionMessage(ex, this);
            }
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
                    SectionData.StartAwait(grid_ucItems);

                itemsQuery = items.Where(x => x.isActive == tglItemState);

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
                itemsQuery = items.Where(x => (x.code.ToLower().Contains(txtItemSearch) ||
                x.name.ToLower().Contains(txtItemSearch) ||
                x.details.ToLower().Contains(txtItemSearch)
                ) && x.isActive == tglItemState);
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
                #endregion

                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
                SectionData.ExceptionMessage(ex, this);
            }
        }


        private void Btn_firstPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_ucItems);

                pageIndex = 1;
                #region
                itemsQuery = items.Where(x => (x.code.ToLower().Contains(txtItemSearch) ||
                x.name.ToLower().Contains(txtItemSearch) ||
                x.details.ToLower().Contains(txtItemSearch)
                ) && x.isActive == tglItemState);
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
                #endregion

                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
                SectionData.ExceptionMessage(ex, this);
            }
        }
        private void Btn_prevPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_ucItems);

                pageIndex = int.Parse(btn_prevPage.Content.ToString());
                #region
                itemsQuery = items.Where(x => (x.code.ToLower().Contains(txtItemSearch) ||
                x.name.ToLower().Contains(txtItemSearch) ||
                x.details.ToLower().Contains(txtItemSearch)
                ) && x.isActive == tglItemState);
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
                #endregion

                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
                SectionData.ExceptionMessage(ex, this);
            }
        }
        private void Btn_activePage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_ucItems);

                pageIndex = int.Parse(btn_activePage.Content.ToString());
                #region
                itemsQuery = items.Where(x => (x.code.ToLower().Contains(txtItemSearch) ||
                x.name.ToLower().Contains(txtItemSearch) ||
                x.details.ToLower().Contains(txtItemSearch)
                ) && x.isActive == tglItemState);
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
                #endregion

                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
                SectionData.ExceptionMessage(ex, this);
            }
        }
        private void Btn_nextPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_ucItems);

                pageIndex = int.Parse(btn_nextPage.Content.ToString());
                #region
                itemsQuery = items.Where(x => (x.code.ToLower().Contains(txtItemSearch) ||
                x.name.ToLower().Contains(txtItemSearch) ||
                x.details.ToLower().Contains(txtItemSearch)
                ) && x.isActive == tglItemState);
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
                #endregion

                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
                SectionData.ExceptionMessage(ex, this);
            }
        }
        private void Btn_lastPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_ucItems);

                itemsQuery = items.Where(x => x.isActive == tglCategoryState);
                pageIndex = ((itemsQuery.Count() - 1) / 9) + 1;
                #region
                itemsQuery = items.Where(x => (x.code.ToLower().Contains(txtItemSearch) ||
                x.name.ToLower().Contains(txtItemSearch) ||
                x.details.ToLower().Contains(txtItemSearch)
                ) && x.isActive == tglItemState);
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
                #endregion

                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
                SectionData.ExceptionMessage(ex, this);
            }
        }
        #endregion
        #region categoryPathControl Y

        async Task generateTrack(int categorypaPathId)
        {
            grid_categoryControlPath.Children.Clear();
            IEnumerable<Category> categoriesPath = await
            categoryModel.GetCategoryTreeByID(categorypaPathId);

            int count = 0;
            foreach (var item in categoriesPath.Reverse())
            {
                if (categories.Where(x => x.parentId == item.categoryId).Count() != 0)
                {
                    Button b = new Button();
                    b.Content = " > " + item.name + " ";
                    b.Padding = new Thickness(0);
                    b.Margin = new Thickness(0);
                    b.Background = null;
                    b.BorderThickness = new Thickness(0);
                    b.FontFamily = Application.Current.Resources["Font-cairo-light"] as FontFamily;
                    b.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#6e6e6e"));
                    b.FontSize = 14;
                    Grid.SetColumn(b, count);
                    b.DataContext = item;
                    b.Name = "category" + item.categoryId;
                    b.Tag = item.categoryId;
                    b.Click += new RoutedEventHandler( getCategoryIdFromPath);
                    count++;
                    grid_categoryControlPath.Children.Add(b);
                }
            }
            

        }
        private async void getCategoryIdFromPath(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_ucItems);
                Button b = (Button)sender;

                if (!string.IsNullOrEmpty(b.Tag.ToString()))
                {
                    await generateTrack(int.Parse(b.Tag.ToString()));
                    categoryParentId = int.Parse(b.Tag.ToString());
                    await RefrishCategoriesCard();


                    //category.categoryId = int.Parse(b.Tag.ToString());

                }
            
                await RefrishItems();
                Txb_searchitems_TextChanged(null, null);
                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
                SectionData.ExceptionMessage(ex, this);
            }

        }
        private async void Btn_getAllCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_ucItems);
                categoryParentId = 0;
                await RefrishCategoriesCard();
                grid_categoryControlPath.Children.Clear();
                //category.categoryId = 0;
                //await RefrishItems();
                #region
                short defaultSale = 0;
                short defaultPurchase = 0;
                int branchId = MainWindow.branchID.Value;
                selectedItems = new List<int>();
                if (CardType.Equals("sales"))
                {
                    defaultSale = 1;
                    defaultPurchase = 0;
                }
                else if (CardType.Equals("purchase"))
                {
                    defaultPurchase = 1;
                    defaultSale = 0;
                }
                else if (CardType.Equals("order"))
                {
                    defaultPurchase = 0;
                    defaultSale = 0;
                }
                else if (CardType.Equals("movement"))
                {
                    defaultPurchase = -1;
                    defaultSale = -1;
                }
                items = await itemModel.GetSaleOrPurItems(0, defaultSale, defaultPurchase, branchId);
                MainWindow.InvoiceGlobalItemsList = items.ToList();
                if (defaultSale == 1)
                    MainWindow.InvoiceGlobalSaleUnitsList = await itemUnitModel.GetForSale();
                else
                    MainWindow.InvoiceGlobalItemUnitsList = await itemUnitModel.Getall();
                #endregion
                Txb_searchitems_TextChanged(null, null);

                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
                SectionData.ExceptionMessage(ex, this);
            }

        }

        #endregion

        #endregion

        private async void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_ucItems);

                await RefrishItems();

                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_ucItems);
                SectionData.ExceptionMessage(ex, this);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (Exception ex)
            {
                //SectionData.ExceptionMessage(ex, this);
            }
        }

        private void Btn_add_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                if (selectedItems.Count > 0)
                {
                    isActive = true;
                    this.Close();
                }
                else
                    Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trErrorAmountIncreaseToolTip"), animation: ToasterAnimation.FadeIn);
            }
            catch (Exception ex)
            {
                SectionData.ExceptionMessage(ex, this);
            }
        }

    }
}
