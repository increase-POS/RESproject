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
using Restaurant.Classes;

namespace Restaurant.View.windows
{
    /// <summary>
    /// Interaction logic for wd_items.xaml
    /// </summary>
    public partial class wd_purchaseItems : Window
    {
        public wd_purchaseItems()
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
        //public int selectedItem { get; set; }
        public List<int> selectedItems { get; set; }
        List<Item> items;
        List<Item> itemsQuery;
        Category category = new Category();

        List<int> categoryIds = new List<int>();
        Boolean all = true;
        List<string> categoryNames = new List<string>();
        public byte tglCategoryState = 1;
        public byte tglItemState = 1;
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
        void loading_RefrishItems()
        {
            try
            {
                RefrishItems();
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
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_ucItems);
                grid_ucItems.Opacity = 1;

                // for pagination onTop Always
                btns = new Button[] { btn_firstPage, btn_prevPage, btn_activePage, btn_nextPage, btn_lastPage };
                catigoriesAndItemsView.wdPurchaseItems = this;

                #region translate
                if (MainWindow.lang.Equals("en"))
                {
                    MainWindow.resourcemanager = new ResourceManager("Restaurant.en_file", Assembly.GetExecutingAssembly());
                    grid_ucItems.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    MainWindow.resourcemanager = new ResourceManager("Restaurant.ar_file", Assembly.GetExecutingAssembly());
                    grid_ucItems.FlowDirection = FlowDirection.RightToLeft;
                }
                translate();
                #endregion
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

                Txb_searchitems_TextChanged(null, null);
               
                if (sender != null)
                    HelpClass.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_ucItems);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void translate()
        {
            txt_items.Text = MainWindow.resourcemanager.GetString("trItems");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(txb_searchitems, MainWindow.resourcemanager.GetString("trSearchHint"));
            btn_add.Content = MainWindow.resourcemanager.GetString("trAdd");
        }
        
        private void Btn_colse_Click(object sender, RoutedEventArgs e)
        {
            isActive = false;
            this.Close();
        }

        #region Categor and Item
        #region Refrish Y
   
      
    
        async Task<IEnumerable<Item>> RefrishItems()
        {
            // int branchId = MainWindow.branchLogin.branchId;
            if (FillCombo.purchaseItems == null)
                await FillCombo.RefreshPurchaseItems();

            selectedItems = new List<int>();
            items = FillCombo.purchaseItems.Where( x => x.itemUnitId != null).ToList();

            //if (CardType.Equals("sales"))
            //{
            //    defaultSale = 1;
            //    defaultPurchase = 0;
            //    items = await itemModel.GetSaleOrPurItems(category.categoryId, defaultSale, defaultPurchase, branchId);
            //    FillCombo.salesItems = items.ToList();
            //}
            //else if (CardType.Equals("purchase"))
            //{

            //}
            //else if (CardType.Equals("order"))
            //{
            //    defaultPurchase = 0;
            //    defaultSale = 0;
            //    items = await itemModel.GetSaleOrPurItems(category.categoryId, defaultSale, defaultPurchase, branchId);
            //    FillCombo.salesItems = items.ToList();
            //}
            //else if (CardType.Equals("movement"))
            //{
            //    defaultPurchase = -1;
            //    defaultSale = -1;
            //    items = await itemModel.GetSaleOrPurItems(category.categoryId, defaultSale, defaultPurchase, branchId);
            //    FillCombo.salesItems = items.ToList();
            //}

            //if (defaultSale == 1)
            //    MainWindow.InvoiceGlobalSaleUnitsList = await itemUnitModel.GetForSale();
            //else
            //    MainWindow.InvoiceGlobalItemUnitsList = await itemUnitModel.Getall();
            return items;
        }

        //void RefrishItemsDatagrid(IEnumerable<Item> _items)
        //{
        //    dg_items.ItemsSource = _items;
        //}


        void RefrishItemsCard(IEnumerable<Item> _items)
        {
            grid_itemContainerCard.Children.Clear();
            catigoriesAndItemsView.gridCatigorieItems = grid_itemContainerCard;
            catigoriesAndItemsView.FN_refrishCatalogItem(_items.ToList(), "purchase");
        }
        #endregion
        #region Get Id By Click  Y
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
      
        #region Search Y


        private async void Txb_searchitems_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_ucItems);

                if (FillCombo.purchaseItems is null)
                    await FillCombo.RefreshPurchaseItems();
                txtItemSearch = txb_searchitems.Text.ToLower();
                pageIndex = 1;

                #region
                itemsQuery = items;
                if (categoryIds.Count > 0 && all == false)
                {
                    itemsQuery = itemsQuery.Where(x => x.categoryId != null).ToList().Where(x => categoryIds.Contains((int)x.categoryId)).ToList();
                }

                itemsQuery = itemsQuery.Where(x => (x.code.ToLower().Contains(txtItemSearch) ||
                x.name.ToLower().Contains(txtItemSearch) ||
                x.details.ToLower().Contains(txtItemSearch)
                )).ToList();

                if (btns is null)
                    btns = new Button[] { btn_firstPage, btn_prevPage, btn_activePage, btn_nextPage, btn_lastPage };
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
                #endregion
                //RefrishItemsDatagrid(itemsQuery);

                if (sender != null)
                    HelpClass.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_ucItems);
                HelpClass.ExceptionMessage(ex, this);
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
                    HelpClass.StartAwait(grid_ucItems);

                itemsQuery = items.Where(x => x.isActive == tglItemState).ToList();

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
                ) && x.isActive == tglItemState).ToList();
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
                #endregion

                if (sender != null)
                    HelpClass.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_ucItems);
                HelpClass.ExceptionMessage(ex, this);
            }
        }


        private void Btn_firstPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_ucItems);

                pageIndex = 1;
                #region
                itemsQuery = items.Where(x => (x.code.ToLower().Contains(txtItemSearch) ||
                x.name.ToLower().Contains(txtItemSearch) ||
                x.details.ToLower().Contains(txtItemSearch)
                ) && x.isActive == tglItemState).ToList();
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
                #endregion

                if (sender != null)
                    HelpClass.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_ucItems);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_prevPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_ucItems);

                pageIndex = int.Parse(btn_prevPage.Content.ToString());
                #region
                itemsQuery = items.Where(x => (x.code.ToLower().Contains(txtItemSearch) ||
                x.name.ToLower().Contains(txtItemSearch) ||
                x.details.ToLower().Contains(txtItemSearch)
                ) && x.isActive == tglItemState).ToList();
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
                #endregion

                if (sender != null)
                    HelpClass.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_ucItems);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_activePage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_ucItems);

                pageIndex = int.Parse(btn_activePage.Content.ToString());
                #region
                itemsQuery = items.Where(x => (x.code.ToLower().Contains(txtItemSearch) ||
                x.name.ToLower().Contains(txtItemSearch) ||
                x.details.ToLower().Contains(txtItemSearch)
                ) && x.isActive == tglItemState).ToList();
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
                #endregion

                if (sender != null)
                    HelpClass.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_ucItems);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_nextPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_ucItems);

                pageIndex = int.Parse(btn_nextPage.Content.ToString());
                #region
                itemsQuery = items.Where(x => (x.code.ToLower().Contains(txtItemSearch) ||
                x.name.ToLower().Contains(txtItemSearch) ||
                x.details.ToLower().Contains(txtItemSearch)
                ) && x.isActive == tglItemState).ToList();
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
                #endregion

                if (sender != null)
                    HelpClass.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_ucItems);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_lastPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_ucItems);

                itemsQuery = items.Where(x => x.isActive == tglCategoryState).ToList();
                pageIndex = ((itemsQuery.Count() - 1) / 9) + 1;
                #region
                itemsQuery = items.Where(x => (x.code.ToLower().Contains(txtItemSearch) ||
                x.name.ToLower().Contains(txtItemSearch) ||
                x.details.ToLower().Contains(txtItemSearch)
                ) && x.isActive == tglItemState).ToList();
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
                #endregion

                if (sender != null)
                    HelpClass.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_ucItems);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion
      

        #endregion

        private async void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_ucItems);
                await FillCombo.RefreshPurchaseItems();
                RefrishItems();
                Txb_searchitems_TextChanged(null, null);
                if (sender != null)
                    HelpClass.EndAwait(grid_ucItems);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_ucItems);
                HelpClass.ExceptionMessage(ex, this);
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
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void categories_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox.Tag.ToString() == "allCategories")
            {
                chb_rawMaterials.IsChecked =
                chb_vegetables.IsChecked =
                chb_meat.IsChecked =
                chb_drinks.IsChecked = false;

                chb_rawMaterials.IsEnabled =
                chb_vegetables.IsEnabled =
                chb_meat.IsEnabled =
                chb_drinks.IsEnabled = false;

                categoryIds = new List<int>();
                all = true;
            }
            else
            {
                all = false;
                categoryIds.Add(FillCombo.GetCategoryId(checkBox.Tag.ToString()));
            }
            Txb_searchitems_TextChanged(null, null);
        }

        private void categories_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox.Tag.ToString() == "allCategories")
            {
                chb_rawMaterials.IsChecked =
                chb_vegetables.IsChecked =
                chb_meat.IsChecked =
                chb_drinks.IsChecked = false;

                chb_rawMaterials.IsEnabled =
                chb_vegetables.IsEnabled =
                chb_meat.IsEnabled =
                chb_drinks.IsEnabled = true;

                categoryIds = new List<int>();
                all = true;
            }
            else
            {                
                categoryIds.Remove(FillCombo.GetCategoryId(checkBox.Tag.ToString()));
            }
            Txb_searchitems_TextChanged(null, null);
        }
    }
}
