using netoaster;
using Restaurant.Classes;
using Restaurant.View.windows;
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

namespace Restaurant.View.storage.storageOperations
{
    /// <summary>
    /// Interaction logic for uc_itemsStorage.xaml
    /// </summary>
    public partial class uc_itemsStorage : UserControl
    {
        public uc_itemsStorage()
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
        private static uc_itemsStorage _instance;
        public static uc_itemsStorage Instance
        {
            get
            {
                //if (_instance == null)
                _instance = new uc_itemsStorage();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        string transferPermission = "itemsStorage_transfer";
        string reportsPermission = "itemsStorage_reports";
        ItemLocation itemLocation = new ItemLocation();
        IEnumerable<ItemLocation> itemLocationsQuery;
        IEnumerable<ItemLocation> itemLocations;
        byte tgl_branchState;
        string searchText = "";
        public static List<string> requiredControlList;
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {//load
            try
            {
                HelpClass.StartAwait(grid_main);
                requiredControlList = new List<string> { "itemName", "quantity", "sectionId", "locationId" };
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

                await FillCombo.FillComboSections(cb_sectionId);

                btn_transfer.IsEnabled = false;

                Keyboard.Focus(tb_quantity);
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
            ////////////////////////////////----invoice----/////////////////////////////////
            dg_itemsStorage.Columns[0].Header = MainWindow.resourcemanager.GetString("trItemUnit");
            dg_itemsStorage.Columns[1].Header = MainWindow.resourcemanager.GetString("trSectionLocation");
            dg_itemsStorage.Columns[2].Header = MainWindow.resourcemanager.GetString("trQuantity");
            dg_itemsStorage.Columns[3].Header = MainWindow.resourcemanager.GetString("trStartDate");
            dg_itemsStorage.Columns[4].Header = MainWindow.resourcemanager.GetString("trEndDate");
            dg_itemsStorage.Columns[5].Header = MainWindow.resourcemanager.GetString("trNote");
            dg_itemsStorage.Columns[6].Header = MainWindow.resourcemanager.GetString("trOrderNum");

            txt_Location.Text = MainWindow.resourcemanager.GetString("trLocationt");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_itemName, MainWindow.resourcemanager.GetString("trItemNameHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_quantity, MainWindow.resourcemanager.GetString("trQuantityHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_sectionId, MainWindow.resourcemanager.GetString("trSectionHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_locationId, MainWindow.resourcemanager.GetString("trLocationHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_notes, MainWindow.resourcemanager.GetString("trNoteHint"));


            chk_stored.Content = MainWindow.resourcemanager.GetString("trStored");
            chk_freezone.Content = MainWindow.resourcemanager.GetString("trFreeZone");
            chk_locked.Content = MainWindow.resourcemanager.GetString("trReserved");
            btn_transfer.Content = MainWindow.resourcemanager.GetString("trTransfer");
            btn_locked.Content = MainWindow.resourcemanager.GetString("trUnlock");
        }
        #region Add - Update - Delete - Search - Tgl - Clear - DG_SelectionChanged - refresh
        private async void Btn_locked_Click(object sender, RoutedEventArgs e)
        {
            /*
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(transferPermission, MainWindow.groupObjects, "one") )
                {
                    if (dg_itemsStorage.SelectedIndex != -1)
                    {
                        //validateMandatoryInputs();
                        if (itemLocation != null && !tb_quantity.Text.Equals(""))
                        {
                            //int oldLocationId = (int)itemLocation.locationId;
                            //int newLocationId = (int)cb_XYZ.SelectedValue;
                            //if (oldLocationId != newLocationId)
                            //{
                            int quantity = int.Parse(tb_quantity.Text);
                            ItemLocation newLocation = new ItemLocation();
                            newLocation.itemsLocId = itemLocation.itemsLocId;
                            newLocation.itemUnitId = itemLocation.itemUnitId;
                            newLocation.locationId = itemLocation.locationId;
                            newLocation.quantity = quantity;
                            newLocation.startDate = dp_startDate.SelectedDate;
                            newLocation.endDate = dp_endDate.SelectedDate;
                            newLocation.note = tb_notes.Text;
                            newLocation.updateUserId = MainWindow.userID.Value;
                            newLocation.createUserId = MainWindow.userID.Value;
                            int res = await itemLocation.unlockItem(newLocation, MainWindow.branchID.Value);
                            if (res > 0)
                            {
                                Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);

                            }
                            else //HelpClass.popUpResponse("", MainWindow.resourcemanager.GetString("trPopError"));
                                Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);

                            if (chk_stored.IsChecked == true)
                                await refreshItemsLocations();
                            else if (chk_freezone.IsChecked == true)
                                await refreshFreeZoneItemsLocations();
                            else
                            { }

                            clearInputs();
                            //}
                            //else
                            //    Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trTranseToSameLocation"), animation: ToasterAnimation.FadeIn);
                            Tb_search_TextChanged(null, null);
                        }
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
            */
        }
        private void Btn_transfer_Click(object sender, RoutedEventArgs e)
        {
            /*
            // transfer
            try
            {
                HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(transferPermission, MainWindow.groupObjects, "one") )
                {
                    if (dg_itemsStorage.SelectedIndex != -1)
                    {
                        validateMandatoryInputs();
                        if (itemLocation != null &&
                            !tb_quantity.Text.Equals("") && cb_section.SelectedIndex != -1
                            && cb_XYZ.SelectedIndex != -1 && (!itemLocation.itemType.Equals("d") ||
                            (itemLocation.itemType.Equals("d") && dp_startDate.SelectedDate != null && dp_endDate.SelectedDate != null)))
                        {
                            int oldLocationId = (int)itemLocation.locationId;
                            int newLocationId = (int)cb_XYZ.SelectedValue;
                            if (oldLocationId != newLocationId)
                            {
                                int quantity = int.Parse(tb_quantity.Text);
                                ItemLocation newLocation = new ItemLocation();
                                newLocation.itemUnitId = itemLocation.itemUnitId;
                                newLocation.invoiceId = itemLocation.invoiceId;
                                newLocation.locationId = newLocationId;
                                newLocation.quantity = quantity;
                                newLocation.startDate = dp_startDate.SelectedDate;
                                newLocation.endDate = dp_endDate.SelectedDate;
                                newLocation.note = tb_notes.Text;
                                newLocation.updateUserId = MainWindow.userID.Value;
                                newLocation.createUserId = MainWindow.userID.Value;
                                //newLocation.storeCost 
                                int res = await itemLocation.trasnferItem(itemLocation.itemsLocId, newLocation);
                                if (res > 0)
                                {
                                    Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);

                                }
                                else //HelpClass.popUpResponse("", MainWindow.resourcemanager.GetString("trPopError"));
                                    Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);

                                if (chk_stored.IsChecked == true)
                                    await refreshItemsLocations();
                                else if (chk_freezone.IsChecked == true)
                                    await refreshFreeZoneItemsLocations();
                                else
                                { }

                                clearInputs();
                            }
                            else
                                Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trTranseToSameLocation"), animation: ToasterAnimation.FadeIn);
                            Tb_search_TextChanged(null, null);
                        }
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
            */
        }
        #endregion
        #region events
        private async void Cb_sectionId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                    HelpClass.StartAwait(grid_main);
                if (cb_sectionId.SelectedIndex != -1)
                    await FillCombo.FillComboLocationsBySection( cb_locationId , (int)cb_sectionId.SelectedValue);
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void search_Checking(object sender, RoutedEventArgs e)
        {
            /*
            try
            {
                
                    HelpClass.StartAwait(grid_main);
                CheckBox cb = sender as CheckBox;
                if (chk_freezone != null)
                {
                    if (cb.Name == "chk_stored")
                    {
                        chk_freezone.IsChecked = false;
                        chk_locked.IsChecked = false;
                        btn_locked.Visibility = Visibility.Collapsed;
                        dg_itemsStorage.Columns[6].Visibility = Visibility.Collapsed; //make order num column unvisible
                        dg_itemsStorage.Columns[3].Visibility = Visibility.Visible;
                        dg_itemsStorage.Columns[4].Visibility = Visibility.Visible;
                        repTitle2 = "trStored";


                    }
                    else if (cb.Name == "chk_freezone")
                    {
                        chk_stored.IsChecked = false;
                        chk_locked.IsChecked = false;
                        btn_locked.Visibility = Visibility.Collapsed;
                        dg_itemsStorage.Columns[6].Visibility = Visibility.Collapsed; //make order num column unvisible
                        dg_itemsStorage.Columns[3].Visibility = Visibility.Visible;
                        dg_itemsStorage.Columns[4].Visibility = Visibility.Visible;
                        repTitle2 = "trFreeZone";

                    }
                    else
                    {
                        chk_stored.IsChecked = false;
                        chk_freezone.IsChecked = false;
                        btn_locked.Visibility = Visibility.Visible;
                        dg_itemsStorage.Columns[6].Visibility = Visibility.Visible; //make order num column visible
                        dg_itemsStorage.Columns[3].Visibility = Visibility.Collapsed;
                        dg_itemsStorage.Columns[4].Visibility = Visibility.Collapsed;
                        repTitle2 = "trReserved";
                    }
                }
                Tb_search_TextChanged(null, null);
                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
            */
        }
        private void chk_uncheck(object sender, RoutedEventArgs e)
        {

            try
            {
                
                    HelpClass.StartAwait(grid_main);
                CheckBox cb = sender as CheckBox;
                if (cb.IsFocused)
                {
                    if (cb.Name == "chk_stored")
                        chk_stored.IsChecked = true;
                    else if (cb.Name == "chk_freezone")
                        chk_freezone.IsChecked = true;
                    else
                        chk_locked.IsChecked = true;
                }
                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
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
                if (itemLocations is null)
                    await RefreshItemLocationsList();
                tgl_branchState = 1;
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
                if (itemLocations is null)
                    await RefreshItemLocationsList();
                tgl_branchState = 0;
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
        private void Dg_itemsStorage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                //selection
                if (dg_itemsStorage.SelectedIndex != -1)
                {
                    itemLocation = dg_itemsStorage.SelectedItem as ItemLocation;
                    this.DataContext = itemLocation;
                    if (itemLocation != null)
                    {
                        btn_transfer.IsEnabled = true;

                    }
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
        private async void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {//refresh

                HelpClass.StartAwait(grid_main);
                await RefreshItemLocationsList();
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
        async Task Search()
        {
            /*
            //search
            if (itemLocations is null)
                await RefreshItemLocationsList();
            searchText = tb_search.Text.ToLower();
            itemLocationsQuery = itemLocations.Where(s => (s.code.ToLower().Contains(searchText) ||
            s.name.ToLower().Contains(searchText) ||
            s.mobile.ToLower().Contains(searchText)
            ) && s.isActive == tgl_branchState);
            RefreshBranchsView();
            */
        }
        async Task<IEnumerable<ItemLocation>> RefreshItemLocationsList()
        {

            itemLocations = await itemLocation.get(MainWindow.branchLogin.branchId);
            return itemLocations;

        }
        void RefreshBranchsView()
        {
            dg_itemsStorage.ItemsSource = itemLocationsQuery;
            txt_count.Text = itemLocationsQuery.Count().ToString();
        }
        #endregion
        #region validate - clearValidate - textChange - lostFocus - . . . . 
        void Clear()
        {
            this.DataContext = new ItemLocation();

            

            // last 
            HelpClass.clearValidate(requiredControlList, this);
            btn_transfer.IsEnabled = false;
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

                
                    HelpClass.StartAwait(grid_main);
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") )
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
                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
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
                
                    HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") )
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

                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_pieChart_Click(object sender, RoutedEventArgs e)
        {//pie
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") )
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
                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_preview_Click(object sender, RoutedEventArgs e)
        {//preview
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") )
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
                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
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
                
                    HelpClass.StartAwait(grid_main);

                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") )
                {
                    Thread t1 = new Thread(() =>
                    {
                        ExcelPackage();

                    });
                    t1.Start();
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
        */
        #endregion
       
    }
}
