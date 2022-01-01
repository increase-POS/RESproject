using netoaster;
using Restaurant.Classes;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using Restaurant.View.windows;

namespace Restaurant.View.catalog.rawMaterials
{
    /// <summary>
    /// Interaction logic for uc_itemsRawMaterials.xaml
    /// </summary>
    public partial class uc_itemsRawMaterials : UserControl
    {
        public uc_itemsRawMaterials()
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
        private static uc_itemsRawMaterials _instance;
        public static uc_itemsRawMaterials Instance
        {
            get
            {
                //if (_instance == null)
                _instance = new uc_itemsRawMaterials();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        string basicsPermission = "itemsRawMaterials_basics";
        string unitsPermission = "itemsRawMaterials_units";
        byte tgl_itemState;
        string searchText = "";
        public static List<string> requiredControlList;
        //List<Unit> units;
        Unit unit = new Unit();
        #region for barcode
        DateTime _lastKeystroke = new DateTime(0);
        static private string _BarcodeStr = "";
        static private object _Sender;
        #endregion
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.KeyDown -= HandleKeyPress;
            Instance = null;
            GC.Collect();
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {//load
            try
            {
                HelpClass.StartAwait(grid_main);
                MainWindow.mainWindow.KeyDown += HandleKeyPress;
                // for pagination onTop Always
                btns = new Button[] { btn_firstPage, btn_prevPage, btn_activePage, btn_nextPage, btn_lastPage };
                catigoriesAndItemsView.ucitemsRawMaterials = this;
                btn_units.IsEnabled = false;

                requiredControlList = new List<string> { "code", "name"  };
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
              
                FillCombo.FillCategoryPurchase(cb_categoryId);
                FillCombo.FillItemTypePurchase(cb_type);
                FillCombo.FillUnits(cb_maxUnitId);
                FillCombo.FillUnits(cb_minUnitId);
                Keyboard.Focus(tb_code);
                await RefreshItemsList();
                await Search();
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

            txt_baseInformation.Text = MainWindow.resourcemanager.GetString("trBaseInformation");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_details, MainWindow.resourcemanager.GetString("trDetailsHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_name, MainWindow.resourcemanager.GetString("trNameHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_code, MainWindow.resourcemanager.GetString("trCode"));


            txt_contentInformatin.Text = MainWindow.resourcemanager.GetString("trMoreInformation");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_categoryId, MainWindow.resourcemanager.GetString("trSelectCategorieHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_taxes, MainWindow.resourcemanager.GetString("trTax") + "...");


            txt_minAndMax.Text = MainWindow.resourcemanager.GetString("trMinAndMaxOfItem");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_min, MainWindow.resourcemanager.GetString("trMinHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_max, MainWindow.resourcemanager.GetString("trMaxHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_minUnitId, MainWindow.resourcemanager.GetString("trSelectUnitHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_maxUnitId, MainWindow.resourcemanager.GetString("trSelectUnitHint"));

            btn_add.Content = MainWindow.resourcemanager.GetString("trAdd");
            btn_update.Content = MainWindow.resourcemanager.GetString("trUpdate");
            btn_delete.Content = MainWindow.resourcemanager.GetString("trDelete");
            btn_clear.ToolTip = MainWindow.resourcemanager.GetString("trClear");

          
        


        }
        #region Add - Update - Delete - Search - Tgl - Clear - DG_SelectionChanged - refresh
        private async void Btn_add_Click(object sender, RoutedEventArgs e)
        {//add
            try
            {
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "add") || HelpClass.isAdminPermision())
                {
                    HelpClass.StartAwait(grid_main);

                    item = new Item();
                    if (HelpClass.validate(requiredControlList, this))
                    {
                        Boolean codeAvailable = await checkCodeAvailabiltiy();

                        int? categoryId = null;
                        if (cb_categoryId.SelectedIndex != -1)
                            categoryId = (int) cb_categoryId.SelectedValue;

                        int min = 0;
                        int max = 0;
                        decimal taxes = 0;

                        if (tb_min.Text != "")
                            min = int.Parse(tb_min.Text);
                        if (tb_max.Text != "")
                            max = int.Parse(tb_max.Text);
                        if (tb_taxes.Text != "")
                            taxes = decimal.Parse(tb_taxes.Text);

                        Nullable<int> minUnitId = null;
                        if (cb_minUnitId.SelectedIndex != -1)
                            minUnitId = (int)cb_minUnitId.SelectedValue;
                        Nullable<int> maxUnitId = null;
                        if (cb_maxUnitId.SelectedIndex != -1)
                            maxUnitId = (int)cb_maxUnitId.SelectedValue;

                        item = new Item();
                        item.code = tb_code.Text;
                        item.name = tb_name.Text;
                        item.details = tb_details.Text;
                        item.image = "";
                        item.taxes = taxes;
                        item.isActive = 1;
                        item.min = min;
                        item.max = max;
                        item.categoryId = categoryId;
                        item.createUserId = MainWindow.userLogin.userId;
                        item.updateUserId = MainWindow.userLogin.userId;
                        item.minUnitId = minUnitId;
                        item.maxUnitId = maxUnitId;
                        item.type = cb_type.SelectedValue.ToString();
                        int res = await item.save(item);
                        if (res == -1)// إظهار رسالة الترقية
                            Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopUpgrade"), animation: ToasterAnimation.FadeIn);

                        else if (res == 0) // an error occure
                            Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                        else
                        {
                            item.itemId = res;
                            Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);

                            int itemId = res;

                            if (openFileDialog.FileName != "")
                                await item.uploadImage(openFileDialog.FileName, Md5Encription.MD5Hash("Inc-m" + itemId.ToString()), itemId);
                            Clear();
                            await RefreshItemsList();
                            await Search();
                        }
                    }
                    HelpClass.EndAwait(grid_main);
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "update") )
                {
                    HelpClass.StartAwait(grid_main);
                    if (HelpClass.validate(requiredControlList, this))
                    {
                        Boolean codeAvailable = await checkCodeAvailabiltiy(item.code);

                        int? categoryId = null;
                        if (cb_categoryId.SelectedIndex != -1)
                            categoryId = (int)cb_categoryId.SelectedValue;

                        int min = 0;
                        int max = 0;
                        decimal taxes = 0;
                        if (tb_min.Text != "")
                            min = int.Parse(tb_min.Text);
                        if (tb_max.Text != "")
                            max = int.Parse(tb_max.Text);
                        if (tb_taxes.Text != "")
                            taxes = decimal.Parse(tb_taxes.Text);

                        Nullable<int> minUnitId = null;
                        if (cb_minUnitId.SelectedIndex != -1)
                            minUnitId = (int)cb_minUnitId.SelectedValue;
                        Nullable<int> maxUnitId = null;
                        if (cb_maxUnitId.SelectedIndex != -1)
                            maxUnitId = (int)cb_maxUnitId.SelectedValue;
                        item.code = tb_code.Text;
                        item.name = tb_name.Text;
                        item.details = tb_details.Text;
                        item.taxes = taxes;
                        item.min = min;
                        item.max = max;
                        item.categoryId = categoryId;
                        item.updateUserId = MainWindow.userLogin.userId;
                        item.minUnitId = minUnitId;
                        item.maxUnitId = maxUnitId;
                        item.type = cb_type.SelectedValue.ToString();

                        int res = await item.save(item);
                        if (res == -1)// إظهار رسالة الترقية
                            Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopUpgrade"), animation: ToasterAnimation.FadeIn);

                        else if (res == 0) // an error occure
                            Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                        else
                        {
                            item.itemId = res;
                            Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);

                            int itemId = res;
                            if (openFileDialog.FileName != "")
                                await item.uploadImage(openFileDialog.FileName, Md5Encription.MD5Hash("Inc-m" + itemId.ToString()), itemId);

                            Clear();
                            await RefreshItemsList();
                            await Search();


                        }

                     

                    }
                    HelpClass.EndAwait(grid_main);
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

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
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "delete") || HelpClass.isAdminPermision())
                {
                    HelpClass.StartAwait(grid_main);
                    if (item.itemId != 0)
                    {
                        if ((!item.canDelete) && (item.isActive == 0))
                        {
                            #region
                            Window.GetWindow(this).Opacity = 0.2;
                            wd_acceptCancelPopup w = new wd_acceptCancelPopup();
                            w.contentText = MainWindow.resourcemanager.GetString("trMessageBoxActivate");
                            w.ShowDialog();
                            Window.GetWindow(this).Opacity = 1;
                            #endregion
                            if (w.isOk)
                                await activate();
                        }
                        else
                        {
                            #region
                            Window.GetWindow(this).Opacity = 0.2;
                            wd_acceptCancelPopup w = new wd_acceptCancelPopup();
                            if (item.canDelete)
                                w.contentText = MainWindow.resourcemanager.GetString("trMessageBoxDelete");
                            if (!item.canDelete)
                                w.contentText = MainWindow.resourcemanager.GetString("trMessageBoxDeactivate");
                            w.ShowDialog();
                            Window.GetWindow(this).Opacity = 1;
                            #endregion
                            if (w.isOk)
                            {
                                string popupContent = "";
                                if (item.canDelete) popupContent = MainWindow.resourcemanager.GetString("trPopDelete");
                                if ((!item.canDelete) && (item.isActive == 1)) popupContent = MainWindow.resourcemanager.GetString("trPopInActive");

                                int s = await item.delete(item.itemId, MainWindow.userLogin.userId, item.canDelete);
                                if (s < 0)
                                    Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                                else
                                {
                                    Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopDelete"), animation: ToasterAnimation.FadeIn);

                                    await RefreshItemsList();
                                    await Search();
                                    Clear();
                                }
                            }
                        }
                    }
                    HelpClass.EndAwait(grid_main);
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async Task activate()
        {//activate
            item.isActive = 1;
            int s = await item.save(item);
            if (s <= 0)
                Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
            else
            {
                Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopActive"), animation: ToasterAnimation.FadeIn);
                await RefreshItemsList();
                await Search();
            }
        }
        #endregion
        #region events
        private async void Tb_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                await Search();
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Tgl_isActive_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                if (items is null)
                    await RefreshItemsList();
                tgl_itemState = 1;
                await Search();
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Tgl_isActive_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                if (items is null)
                    await RefreshItemsList();
                tgl_itemState = 0;
                await Search();
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
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
        private async void HandleKeyPress(object sender, KeyEventArgs e)
        {
            try
                {
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
                _BarcodeStr += digit;
                _lastKeystroke = DateTime.Now;

                // process barcode 
                if (e.Key.ToString() == "Return" && _BarcodeStr != "")
                {
                    // get item matches barcode
                    if (FillCombo.itemUnitList != null)
                    {
                         var ob = FillCombo.itemUnitList.ToList().Find(c => c.barcode == _BarcodeStr);
                        if (ob != null)
                        {
                            int itemId = (int) ob.itemId;
                            ChangeItemIdEvent(itemId);
                        }
                        else
                        {
                            Clear();
                            Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trErrorItemNotFoundToolTip"), animation: ToasterAnimation.FadeIn);
                        }
                    }
                    _BarcodeStr = "";
                }
            }
            catch (Exception ex)
            {
                _BarcodeStr = "";
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        //private async void Dg_item_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        HelpClass.StartAwait(grid_main);
        //        //selection
        //        if (dg_item.SelectedIndex != -1)
        //        {
        //            item = dg_item.SelectedItem as Item;
        //            this.DataContext = item;
        //            if (item != null)
        //            {
        //                await getImg();
        //                #region delete
        //                if (item.canDelete)
        //                    btn_delete.Content = MainWindow.resourcemanager.GetString("trDelete");
        //                else
        //                {
        //                    if (item.isActive == 0)
        //                        btn_delete.Content = MainWindow.resourcemanager.GetString("trActive");
        //                    else
        //                        btn_delete.Content = MainWindow.resourcemanager.GetString("trInActive");
        //                }
        //                #endregion
        //                HelpClass.getMobile(item.mobile, cb_areaMobile, tb_mobile);
        //                HelpClass.getPhone(item.phone, cb_areaPhone, cb_areaPhoneLocal, tb_phone);
        //                HelpClass.getPhone(item.fax, cb_areaFax, cb_areaFaxLocal, tb_fax);
        //            }
        //        }
        //        HelpClass.clearValidate(requiredControlList, this);
        //        HelpClass.EndAwait(grid_main);
        //    }
        //    catch (Exception ex)
        //    {
        //        HelpClass.EndAwait(grid_main);
        //        HelpClass.ExceptionMessage(ex, this);
        //    }
        //}
        private async void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {//refresh

                HelpClass.StartAwait(grid_main);
                await RefreshItemsList();
                await Search();
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

        /*
        async Task Search()
        {
            //search
            if (items is null)
                await RefreshItemsList();
            searchText = tb_search.Text.ToLower();
            itemsQuery = items.Where(s => (s.code.ToLower().Contains(searchText) ||
            s.name.ToLower().Contains(searchText) ||
            s.mobile.ToLower().Contains(searchText)
            ) && s.isActive == tgl_itemState);
            RefreshCustomersView();
        }
        async Task<IEnumerable<Item>> RefreshItemsList()
        {
            items = await item.Get("c");
            return items;
        }
        void RefreshCustomersView()
        {
            dg_item.ItemsSource = itemsQuery;
            txt_count.Text = itemsQuery.Count().ToString();
        }
        */
        #endregion
        #region validate - clearValidate - textChange - lostFocus - . . . . 
        void Clear()
        {
            item = new Item();
            item.taxes = 0;
            item.min = 0;
            item.max = 0;
            this.DataContext = item;       
            #region image
            HelpClass.clearImg(btn_image);
            #endregion
            btn_units.IsEnabled = false;
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
        #region Image
        string imgFileName = "pic/no-image-icon-125x125.png";
        bool isImgPressed = false;
        OpenFileDialog openFileDialog = new OpenFileDialog();
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        private void Btn_image_Click(object sender, RoutedEventArgs e)
        {
            //select image
            try
            {
                HelpClass.StartAwait(grid_main);
                isImgPressed = true;
                openFileDialog.Filter = "Images|*.png;*.jpg;*.bmp;*.jpeg;*.jfif";
                if (openFileDialog.ShowDialog() == true)
                {
                    HelpClass.imageBrush = new ImageBrush();
                    HelpClass.imageBrush.ImageSource = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Relative));
                    btn_image.Background = HelpClass.imageBrush;
                    imgFileName = openFileDialog.FileName;
                }
                else
                { }
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async Task getImg()
        {
            if (string.IsNullOrEmpty(item.image))
            {
                HelpClass.clearImg(btn_image);
            }
            else
            {
                byte[] imageBuffer = await item.downloadImage(item.image); // read this as BLOB from your DB

                var bitmapImage = new BitmapImage();
                if (imageBuffer != null)
                {
                    using (var memoryStream = new MemoryStream(imageBuffer))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                    }

                    btn_image.Background = new ImageBrush(bitmapImage);
                    // configure trmporary path
                    string dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                    string tmpPath = System.IO.Path.Combine(dir, Global.TMPItemsFolder);
                    tmpPath = System.IO.Path.Combine(tmpPath, item.image);
                    openFileDialog.FileName = tmpPath;
                }
                else
                    HelpClass.clearImg(btn_image);
            }
        }
        #endregion
        #region report
        /*
        // report
        ReportCls reportclass = new ReportCls();
        LocalReport rep = new LocalReport();
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        public void BuildReport()
        {
            List<ReportParameter> paramarr = new List<ReportParameter>();

            string addpath;
            bool isArabic = ReportCls.checkLang();
            if (isArabic)
            {
                addpath = @"\Reports\Sale\Ar\PackageReport.rdlc";
            }
            else
                addpath = @"\Reports\Sale\En\PackageReport.rdlc";
            string reppath = reportclass.PathUp(Directory.GetCurrentDirectory(), 2, addpath);

            ReportCls.checkLang();

            clsReports.packageReport(itemsQuery, rep, reppath, paramarr);
            clsReports.setReportLanguage(paramarr);
            clsReports.Header(paramarr);

            rep.SetParameters(paramarr);

            rep.Refresh();
        }
        public void pdfpackage()
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
        private void Btn_pdf_Click(object sender, RoutedEventArgs e)
        {//pdf
            try
            {

                if (sender != null)
                    SectionData.StartAwait(grid_main);
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") || SectionData.isAdminPermision())
                {
                    /////////////////////////////////////
                    Thread t1 = new Thread(() =>
                    {
                        pdfpackage();
                    });
                    t1.Start();
                    //////////////////////////////////////
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                if (sender != null)
                    SectionData.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_main);
                SectionData.ExceptionMessage(ex, this);
            }
        }
        public void printpackage()
        {
            BuildReport();

            this.Dispatcher.Invoke(() =>
            {
                LocalReportExtensions.PrintToPrinterbyNameAndCopy(rep, MainWindow.rep_printer_name, short.Parse(MainWindow.rep_print_count));
            });
        }
        private void Btn_print_Click(object sender, RoutedEventArgs e)
        {//print
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") || SectionData.isAdminPermision())
                {
                    /////////////////////////////////////
                    Thread t1 = new Thread(() =>
                    {
                        printpackage();
                    });
                    t1.Start();
                    //////////////////////////////////////

                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

                if (sender != null)
                    SectionData.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_main);
                SectionData.ExceptionMessage(ex, this);
            }
        }
        private void Btn_pieChart_Click(object sender, RoutedEventArgs e)
        {//pie
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") || SectionData.isAdminPermision())
                {
                    #region
                    Window.GetWindow(this).Opacity = 0.2;
                    win_lvcCatalog win = new win_lvcCatalog(itemsQuery, 3);
                    win.ShowDialog();
                    Window.GetWindow(this).Opacity = 1;
                    #endregion
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                if (sender != null)
                    SectionData.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_main);
                SectionData.ExceptionMessage(ex, this);
            }
        }
        private void Btn_preview_Click(object sender, RoutedEventArgs e)
        {//preview
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") || SectionData.isAdminPermision())
                {
                    #region
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
                    #endregion
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                if (sender != null)
                    SectionData.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_main);
                SectionData.ExceptionMessage(ex, this);
            }
        }
        public void ExcelPackage()
        {

            BuildReport();

            this.Dispatcher.Invoke(() =>
            {
                saveFileDialog.Filter = "EXCEL|*.xls;";
                if (saveFileDialog.ShowDialog() == true)
                {
                    string filepath = saveFileDialog.FileName;
                    LocalReportExtensions.ExportToExcel(rep, filepath);
                }
            });
        }
        private void Btn_exportToExcel_Click(object sender, RoutedEventArgs e)
        {//excel
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") || SectionData.isAdminPermision())
                {
                    Thread t1 = new Thread(() =>
                    {
                        ExcelPackage();

                    });
                    t1.Start();
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                if (sender != null)
                    SectionData.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    SectionData.EndAwait(grid_main);
                SectionData.ExceptionMessage(ex, this);
            }
        }
        */
        #endregion
        #region  Cards
        CatigoriesAndItemsView catigoriesAndItemsView = new CatigoriesAndItemsView();
        #region Refrish Y
        Item item = new Item();
        IEnumerable<Item> items;
        IEnumerable<Item> itemsQuery;
        async Task<IEnumerable<Item>> RefreshItemsList()
        {
            items = await item.Get();
            return items;
        }
        void RefrishItemsCard(IEnumerable<Item> _items)
        {
            grid_itemContainerCard.Children.Clear();
            catigoriesAndItemsView.gridCatigorieItems = grid_itemContainerCard;
            catigoriesAndItemsView.FN_refrishCatalogItem(_items.ToList());
        }
        #endregion
        #region Get Id By Click  Y

        public void ChangeItemIdEvent(int itemId)
        {
            item = items.Where(x => x.itemId == itemId).FirstOrDefault();
            this.DataContext = item;
            btn_units.IsEnabled = true;
        }
        #endregion
        #region Search Y


        async Task Search()
        {
            //search
            try
            {
                HelpClass.StartAwait(grid_main);

                if (items is null)
                    await RefreshItemsList();
                searchText = tb_search.Text;
                itemsQuery = items.
                Where(x => (x.code.ToLower().Contains(searchText) ||
                    x.name.ToLower().Contains(searchText) ||
                    x.details.ToLower().Contains(searchText)
                    // || x.categoryName.ToLower().Contains(searchText)
                    ) && x.isActive == tgl_itemState);
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
        private async Task<Boolean> checkCodeAvailabiltiy(string oldCode = "")
        {

            List<string> itemsCodes = await item.GetItemsCodes();
            string code = tb_code.Text;
            var match = "";
            if (code != oldCode && itemsCodes != null)
                match = itemsCodes.FirstOrDefault(stringToCheck => stringToCheck.Contains(code));

            if (match != "" && match != null)
            {
                Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trDuplicateCodeToolTip"), animation: ToasterAnimation.FadeIn);
                #region Tooltip_code
                p_error_code.Visibility = Visibility.Visible;
                ToolTip toolTip_code = new ToolTip();
                toolTip_code.Content = MainWindow.resourcemanager.GetString("trDuplicateCodeToolTip");
                toolTip_code.Style = Application.Current.Resources["ToolTipError"] as Style;
                p_error_code.ToolTip = toolTip_code;
                #endregion
                return false;
            }
            else
            {
                HelpClass.clearValidate( p_error_code);
                return true;
            }
        }
        private void Btn_units_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                //units
                if (MainWindow.groupObject.HasPermissionAction(unitsPermission, MainWindow.groupObjects, "one"))
                {
                    Window.GetWindow(this).Opacity = 0.2;
                    wd_units w = new wd_units();
                    w.item = item;
                    //w.units = units;
                    w.ShowDialog();
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
    }
}
