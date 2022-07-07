using netoaster;
using Restaurant.Classes;
using Restaurant.Classes.ApiClasses;
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
using System.Windows.Shapes;

namespace Restaurant.View.windows
{
    /// <summary>
    /// Interaction logic for wd_invoiceItemIngredients.xaml
    /// </summary>
    public partial class wd_invoiceItemIngredients : Window
    {
        public wd_invoiceItemIngredients()
        {
            try
            {
                InitializeComponent();
                if (System.Windows.SystemParameters.PrimaryScreenWidth >= 1440)
                {
                    txt_deleteButton.Visibility = Visibility.Visible;
                    txt_addButton.Visibility = Visibility.Visible;
                    txt_updateButton.Visibility = Visibility.Visible;
                    txt_add_Icon.Visibility = Visibility.Visible;
                    txt_update_Icon.Visibility = Visibility.Visible;
                    txt_delete_Icon.Visibility = Visibility.Visible;
                }
                else if (System.Windows.SystemParameters.PrimaryScreenWidth >= 1360)
                {
                    txt_add_Icon.Visibility = Visibility.Collapsed;
                    txt_update_Icon.Visibility = Visibility.Collapsed;
                    txt_delete_Icon.Visibility = Visibility.Collapsed;
                    txt_deleteButton.Visibility = Visibility.Visible;
                    txt_addButton.Visibility = Visibility.Visible;
                    txt_updateButton.Visibility = Visibility.Visible;

                }
                else
                {
                    txt_deleteButton.Visibility = Visibility.Collapsed;
                    txt_addButton.Visibility = Visibility.Collapsed;
                    txt_updateButton.Visibility = Visibility.Collapsed;
                    txt_add_Icon.Visibility = Visibility.Visible;
                    txt_update_Icon.Visibility = Visibility.Visible;
                    txt_delete_Icon.Visibility = Visibility.Visible;

                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        public bool isOpend = false;

        public long itemId;
        public List<itemsTransferIngredients> itemsIngredients;
        public List<ItemTransfer> itemExtras;
        public bool sentToKitchen;

        Item item = new Item();

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }
        private void Btn_colse_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return)
                {
                    //Btn_save_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }


        public string categoryName;
        long categoryId;

        ItemTransfer itemTransfer = new ItemTransfer();


        public static List<string> requiredControlList;
        List<Item> extras;

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {//load
            try
            {
                HelpClass.StartAwait(grid_main);

                
                requiredControlList = new List<string> { "extraItemId", "count" };
                if (AppSettings.lang.Equals("en"))
                {
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                }
                translate();

                setButtonEnabled();

                dg_ingredient.ItemsSource = itemsIngredients;
                RefreshExtrasView();
                await fillExtrasCombo();
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
            txt_title.Text = AppSettings.resourcemanager.GetString("trTag");
            //txt_baseInformation.Text = AppSettings.resourcemanager.GetString("trBaseInformation");
            //MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_tagName, AppSettings.resourcemanager.GetString("trNameHint"));
            //MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_notes, AppSettings.resourcemanager.GetString("trNote") + "...");

            btn_add.Content = AppSettings.resourcemanager.GetString("trAdd");
            btn_update.Content = AppSettings.resourcemanager.GetString("trUpdate");
            btn_delete.Content = AppSettings.resourcemanager.GetString("trDelete");
            btn_clear.ToolTip = AppSettings.resourcemanager.GetString("trClear");

            dg_tag.Columns[0].Header = AppSettings.resourcemanager.GetString("trName");
            dg_tag.Columns[1].Header = AppSettings.resourcemanager.GetString("trCount");
        }

        void setButtonEnabled()
        {
            if(sentToKitchen)
            {
                btn_add.IsEnabled = false;
                btn_update.IsEnabled = false;
                btn_delete.IsEnabled = false;

                col_activate.Visibility = Visibility.Collapsed;
            }
        }
        async Task fillExtrasCombo()
        {
            extras = await FillCombo.item.GetItemExtras(itemId, AppSettings.invType);

            foreach (var e in extras)
                e.name = e.name + " - " + e.price;

            cb_extraItemId.ItemsSource = extras;
            cb_extraItemId.DisplayMemberPath = "name";
            cb_extraItemId.SelectedValuePath = "itemUnitId";
        }
        #region Add - Update - Delete - Search - Tgl - Clear - DG_SelectionChanged - refresh
        private async void Btn_add_Click(object sender, RoutedEventArgs e)
        {//add
            try
            {

                HelpClass.StartAwait(grid_main);
                itemTransfer = new ItemTransfer();
                if (HelpClass.validate(requiredControlList, this) && HelpClass.IsValidEmail(this))
                {
                    if (int.Parse(tb_count.Text) == 0)
                        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trErrorQuantIsZeroToolTip"), animation: ToasterAnimation.FadeIn);
                    else
                    {
                        var it = itemExtras.Where(x => x.itemUnitId == (long)cb_extraItemId.SelectedValue).FirstOrDefault();

                        if (it == null)
                        {
                            item = extras.Where(x => x.itemUnitId == (long)cb_extraItemId.SelectedValue).FirstOrDefault();
                            itemTransfer = new ItemTransfer();
                            itemTransfer.invoiceId = 0;
                            itemTransfer.quantity = int.Parse(tb_count.Text);
                            itemTransfer.price = (decimal)item.price;
                            itemTransfer.itemUnitId = item.itemUnitId;
                            itemTransfer.itemName = item.name;
                            long offerId = 0;
                            string offerType = "1";
                            decimal offerValue = 0;
                            if (item.offerId != null)
                            {
                                offerId = (long)item.offerId;
                                offerType = item.discountType;
                                offerValue = (decimal)item.discountValue;
                            }
                            itemTransfer.offerId = offerId;
                            itemTransfer.offerType = decimal.Parse(offerType);
                            itemTransfer.offerValue = offerValue;
                            //itemTransfer.itemTax = item.Tax;
                            itemTransfer.itemUnitPrice = item.basicPrice;
                            itemTransfer.createUserId = MainWindow.userLogin.userId;
                            itemTransfer.forAgents = item.forAgent;

                            itemExtras.Add(itemTransfer);
                        }
                        else
                        {
                            it.quantity += int.Parse(tb_count.Text);
                        }
                        RefreshExtrasView();
                        Clear();
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
        private async void Btn_update_Click(object sender, RoutedEventArgs e)
        {//update
            try
            {
                HelpClass.StartAwait(grid_main);
                if (HelpClass.validate(requiredControlList, this) && HelpClass.IsValidEmail(this))
                {
                    itemTransfer.quantity = int.Parse(tb_count.Text);
                    RefreshExtrasView();
                    Clear();
                }
                HelpClass.EndAwait(grid_main);

            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {//delete
                HelpClass.StartAwait(grid_main);
                if (itemTransfer.itemName != "")
                {
                    itemExtras.Remove(itemTransfer);
                    RefreshExtrasView();
                    Clear();
                }
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async Task activate()
        {//activate
            //itemTransfer.isActive = 1;
            //var s = await itemTransfer.Save(itemTransfer);
            //if (s <= 0)
            //    Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
            //else
            //{
            //    Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopActive"), animation: ToasterAnimation.FadeIn);
            //    await RefreshTagsList();
            //    await Search();
            //}
        }
        #endregion
        #region events
       
       
       
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
        private async void Dg_tag_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                //selection
                if (dg_tag.SelectedIndex != -1)
                {
                    itemTransfer = dg_tag.SelectedItem as ItemTransfer;
                    this.DataContext = itemTransfer;
                    
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
       
        #endregion
        #region Refresh & Search
       // async Task Search()
       // {
            //search
            //if (tags is null)
            //    await RefreshTagsList();
            //tagsQuery = tags;
            //RefreshTagsView();
       // }
        //async Task<IEnumerable<Tag>> RefreshTagsList()
        //{
        //    tags = await tag.Get(categoryId);
        //    // tags = tags.Where(x => x.categoryId == categoryId);
        //    return tags;
        //}
        void RefreshExtrasView()
        {
            dg_tag.ItemsSource = null;
            dg_tag.ItemsSource = itemExtras;
        }
        #endregion
        #region validate - clearValidate - textChange - lostFocus - . . . . 
        void Clear()
        {
            itemTransfer = new ItemTransfer();
            itemTransfer.quantity = 1;
            this.DataContext = itemTransfer;

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

        async void activateRowinDatagrid(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);

                for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                    if (vis is DataGridRow)
                    {
                        itemsTransferIngredients row = (itemsTransferIngredients)dg_ingredient.SelectedItems[0];
                        if (row.isActive == 1 && row.isBasic == false)
                        {
                            row.isActive = 0;
                        }
                        else if(row.isActive == 0)
                            row.isActive = 1;
                        else
                            Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("ingredientIsBasic"), animation: ToasterAnimation.FadeIn);

                        dg_ingredient.ItemsSource = null;
                        dg_ingredient.ItemsSource = itemsIngredients;
                    }

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Cb_extraItemId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
