using Restaurant.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Restaurant.View.windows
{
    /// <summary>
    /// Interaction logic for wd_extraItemsList.xaml
    /// </summary>
    public partial class wd_extraItemsList : Window
    {
        public bool isActive;
        string searchText = "";
        public string txtItemSearch;
        public long itemId { get; set; }
        List<Item> allExtrasSource = new List<Item>();
        List<Item> selectedExtrasSource = new List<Item>();

        List<Item> allExtras = new List<Item>();
        List<Item> selectedExtras = new List<Item>();

        Item itemModel = new Item();
        ItemExtra extraModel = new ItemExtra();
        ItemExtra itemExtraModel = new ItemExtra();

        List<ItemExtra> newList = new List<ItemExtra>();
        List<Item> extraQuery = new List<Item>();

        public wd_extraItemsList()
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

        private static wd_extraItemsList _instance;
        public static wd_extraItemsList Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new wd_extraItemsList();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {//load
            try
            {
                HelpClass.StartAwait(grid_extraList);

                #region translate
                if (AppSettings.lang.Equals("en"))
                {
                    grid_extraList.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    grid_extraList.FlowDirection = FlowDirection.RightToLeft;
                }

                translat();
                #endregion

                allExtrasSource = await itemModel.GetAllExtras();
                selectedExtrasSource = await itemExtraModel.GetExtraByItemId(itemId);

                allExtras.AddRange(allExtrasSource);
                selectedExtras.AddRange(selectedExtrasSource);

                //remove selected items from all items
                foreach (Item i in selectedExtras)
                {
                    itemModel = new Item();
                    itemModel = allExtras.Where(ex => ex.itemId == i.itemId).FirstOrDefault();
                    if(allExtras.Contains(itemModel))
                        allExtras.Remove(itemModel);
                }

                dg_allItems.ItemsSource = allExtras;
                dg_allItems.SelectedValuePath = "itemId";
                dg_allItems.DisplayMemberPath = "name";

                dg_selectedItems.ItemsSource = selectedExtras;
                dg_selectedItems.SelectedValuePath = "itemId";
                dg_selectedItems.DisplayMemberPath = "name";

                HelpClass.EndAwait(grid_extraList);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_extraList);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        #region methods
        private void translat()
        {
            MaterialDesignThemes.Wpf.HintAssist.SetHint(txb_searchitems, AppSettings.resourcemanager.GetString("trSearchHint"));

            btn_save.Content = AppSettings.resourcemanager.GetString("trSave");

            dg_allItems.Columns[0].Header = AppSettings.resourcemanager.GetString("trItem");
            dg_selectedItems.Columns[0].Header = AppSettings.resourcemanager.GetString("trItem");

            txt_title.Text = AppSettings.resourcemanager.GetString("trOffer");
            txt_items.Text = AppSettings.resourcemanager.GetString("trItems");
            txt_selectedItems.Text = AppSettings.resourcemanager.GetString("trSelectedItems");

            tt_search.Content = AppSettings.resourcemanager.GetString("trSearch");
            tt_selectAllItem.Content = AppSettings.resourcemanager.GetString("trSelectAllItems");
            tt_unselectAllItem.Content = AppSettings.resourcemanager.GetString("trUnSelectAllItems");
            tt_selectItem.Content = AppSettings.resourcemanager.GetString("trSelectOneItem");
            tt_unselectItem.Content = AppSettings.resourcemanager.GetString("trUnSelectOneItem");

        }
        #endregion

        #region events
        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return)
                {
                    Btn_save_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }
        private void Btn_colse_Click(object sender, RoutedEventArgs e)
        {
            //isActive = false;
            this.Close();
        }
        private void Grid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            //// Have to do this in the unusual case where the border of the cell gets selected.
            //// and causes a crash 'EditItem is not allowed'
            //e.Cancel = true;
        }


        #endregion

        #region search - select
        private void Txb_searchitems_TextChanged(object sender, TextChangedEventArgs e)
        {//search
            try
            {
                txtItemSearch = txb_searchitems.Text.ToLower();

                searchText = txb_searchitems.Text;
                extraQuery = allExtras.Where(s => s.name.Contains(searchText)).ToList();
                dg_allItems.ItemsSource = extraQuery;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Dg_allItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Btn_selectedItem_Click(null, null);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Btn_selectedAll_Click(object sender, RoutedEventArgs e)
        {//select all
            try
            {
                int x = allExtras.Count;
                for (int i = 0; i < x; i++)
                {
                    dg_allItems.SelectedIndex = 0;
                    Btn_selectedItem_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }

        }

        private void Btn_selectedItem_Click(object sender, RoutedEventArgs e)
        {//select item
            try
            {
                itemModel = dg_allItems.SelectedItem as Item;
                if (itemModel != null)
                {
                    ItemExtra ie = new ItemExtra();
                    ie.itemId = itemId;
                    ie.extraId = itemModel.itemId;
                    ie.notes = "";
                    ie.createUserId = MainWindow.userLogin.userId;

                    newList.Add(ie);
                    allExtras.Remove(itemModel);
                    selectedExtras.Add(itemModel);

                    dg_allItems.ItemsSource = allExtras;
                    dg_selectedItems.ItemsSource = selectedExtras;

                    dg_allItems.Items.Refresh();
                    dg_selectedItems.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }


        }

        private void Btn_unSelectedItem_Click(object sender, RoutedEventArgs e)
        {//unselect item
            try
            {
                itemModel = dg_selectedItems.SelectedItem as Item;
                ItemExtra i = new ItemExtra();
                if (itemModel != null)
                {
                    i = newList.Where(s =>  s.extraId == itemModel.itemId).FirstOrDefault();

                    newList.Remove(i);
                    allExtras.Add(itemModel);
                    selectedExtras.Remove(itemModel);

                    dg_allItems.ItemsSource = allExtras;
                    dg_selectedItems.ItemsSource = selectedExtras;

                    dg_allItems.Items.Refresh();
                    dg_selectedItems.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }


        }

        private void Btn_unSelectedAll_Click(object sender, RoutedEventArgs e)
        {//unselect all
            try
            {
                int x = selectedExtras.Count;
                for (int i = 0; i < x; i++)
                {
                    dg_selectedItems.SelectedIndex = 0;
                    Btn_unSelectedItem_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Dg_selectedItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Btn_unSelectedItem_Click(null, null);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        #endregion

        private async void Btn_save_Click(object sender, RoutedEventArgs e)
        {//save
            try
            {
                HelpClass.StartAwait(grid_extraList);

                int s = await extraModel.UpdateExtraByItemId(itemId, newList, MainWindow.userLogin.userId);

                isActive = true;
                this.Close();

                HelpClass.EndAwait(grid_extraList);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_extraList);
                HelpClass.ExceptionMessage(ex, this);
            }


        }

    }
}
