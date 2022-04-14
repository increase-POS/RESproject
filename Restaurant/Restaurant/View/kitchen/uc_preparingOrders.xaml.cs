using netoaster;
using Restaurant.Classes;
using Restaurant.Classes.ApiClasses;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Restaurant.View.kitchen
{
    /// <summary>
    /// Interaction logic for uc_preparingOrders.xaml
    /// </summary>
    public partial class uc_preparingOrders : UserControl
    {
        public uc_preparingOrders()
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
        private static uc_preparingOrders _instance;
        public static uc_preparingOrders Instance
        {
            get
            {
                //if (_instance == null)
                if(_instance is null)
                _instance = new uc_preparingOrders();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        string updatePermission = "preparingOrders_update";
        OrderPreparing preparingOrder = new OrderPreparing();
        List<OrderPreparing> orders = new List<OrderPreparing>();
        List<OrderPreparing> ordersQuery = new List<OrderPreparing>();

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
                requiredControlList = new List<string> { "preparingTime" };

                if (AppSettings.lang.Equals("en"))
                {
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                }
                translate();
                #region loading
                loadingList = new List<keyValueBool>();
                bool isDone = true;
                loadingList.Add(new keyValueBool { key = "loading_orders", value = false });
                loadingList.Add(new keyValueBool { key = "loading_salesItems", value = false });

                loading_orders();
                loading_salesItems();
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
               
                FillCombo.FillInvoiceTypeWithDefault(cb_searchInvType);
                FillCombo.FillPreparingOrderStatusWithDefault(cb_searchStatus);

                await Search();
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
            
            // Title
            if (!string.IsNullOrWhiteSpace(FillCombo.objectsList.Where(x => x.name == this.Tag.ToString()).FirstOrDefault().translate))
                txt_title.Text = AppSettings.resourcemanager.GetString(
               FillCombo.objectsList.Where(x => x.name == this.Tag.ToString()).FirstOrDefault().translate
               );
            #region dg_orders
            col_orderNum.Header = AppSettings.resourcemanager.GetString("trOrderCharp");
            dg_orders.Columns[1].Header = AppSettings.resourcemanager.GetString("trInvoiceCharp");
            dg_orders.Columns[2].Header = AppSettings.resourcemanager.GetString("trRemainingTime");
            dg_orders.Columns[3].Header = AppSettings.resourcemanager.GetString("trStatus");
            dg_orders.Columns[4].Header = AppSettings.resourcemanager.GetString("trNote");
            col_table.Header = AppSettings.resourcemanager.GetString("trTable");
            #endregion

            //txt_title.Text = AppSettings.resourcemanager.GetString("trOrder");
            txt_details.Text = AppSettings.resourcemanager.GetString("trDetails");
            txt_items.Text = AppSettings.resourcemanager.GetString("trItems");
            txt_minute.Text = AppSettings.resourcemanager.GetString("trMinute");
            txt_tablesTitle.Text = AppSettings.resourcemanager.GetString("trTables");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_preparingTime, AppSettings.resourcemanager.GetString("trPreparingTime") + "...");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_notes, AppSettings.resourcemanager.GetString("trNoteHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_search, AppSettings.resourcemanager.GetString("trSearchHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_searchStatus, AppSettings.resourcemanager.GetString("trStatusHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_searchCatalog, AppSettings.resourcemanager.GetString("trCategorie") + "...");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_searchInvType, AppSettings.resourcemanager.GetString("") + "...");

            btn_save.Content = AppSettings.resourcemanager.GetString("trSave");
        }
        #region loading
        List<keyValueBool> loadingList;
        async void loading_orders()
        {
            //get orders
            //try
            //{
                await refreshPreparingOrders();
            //}
            //catch
            //{

            //}
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_orders"))
                {
                    item.value = true;
                    break;
                }
            }
        }
       async void loading_salesItems()
        {
            try
            {
                //await FillCombo.FillComboSalesItemsWithDefault(cb_searchCatalog);
                await FillCombo.FillCategorySale(cb_searchCatalog);
                cb_searchCatalog.SelectedIndex = 0;
            }
            catch { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_salesItems"))
                {
                    item.value = true;
                    break;
                }
            }
        }
       
        #endregion
        #region Add - Update - Delete  - Tgl - Clear - DG_SelectionChanged - refresh
        private async void Btn_save_Click(object sender, RoutedEventArgs e)
        { //add
            try
            {
                HelpClass.StartAwait(grid_main);
                if (FillCombo.groupObject.HasPermissionAction(updatePermission, FillCombo.groupObjects, "one"))
                {
                    if (HelpClass.validate(requiredControlList, this))
                    {
                        await saveOrderPreparing();
                    }
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
        async Task saveOrderPreparing()
        {          
            #region order status object
            orderPreparingStatus statusObject = new orderPreparingStatus();
            statusObject.orderPreparingId = preparingOrder.orderPreparingId;
            statusObject.notes = tb_notes.Text;
            statusObject.createUserId = MainWindow.userLogin.userId;
            #endregion

            int res = 0;
            switch (preparingOrder.status)
            {
                case "Listed":
                    #region preparing order object
                    preparingOrder.preparingTime = decimal.Parse(tb_preparingTime.Text);
                    preparingOrder.notes = tb_notes.Text;
                    #endregion
                    statusObject.status = "Preparing";

                    res = await preparingOrder.editPreparingOrderAndStatus(preparingOrder,statusObject);
                    break;
                case "Preparing":
                    statusObject.status = "Ready";

                    res = await preparingOrder.updateOrderStatus(statusObject);
                    break;
                case "Ready":
                    statusObject.status = "Done";

                    res = await preparingOrder.updateOrderStatus(statusObject);
                    break;
            }

            if (res > 0)
            {
                Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);

                clear();
                await refreshPreparingOrders();
                await Search();
            }
            else
                Toaster.ShowError(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
        }

        private void clear()
        {
            try
            {
                preparingOrder = new OrderPreparing();
                this.DataContext = preparingOrder;
                dg_orders.SelectedIndex = -1;
                itemsList = new List<ItemOrderPreparing>();
                BuildOrderItemsDesign();
                btn_save.Content = AppSettings.resourcemanager.GetString("trSave");
                btn_save.IsEnabled = false;
            }
            catch { }
        }



        #endregion
        #region events
        private async void Cb_search_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void Btn_clear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                clear();
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Dg_orders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                //selection

                if (dg_orders.SelectedIndex != -1)
                {
                    preparingOrder = new OrderPreparing();
                    preparingOrder = dg_orders.SelectedItem as OrderPreparing;

                    this.DataContext = preparingOrder;

                    itemsList = preparingOrder.items;
                    BuildOrderItemsDesign();

                    inputEditable(preparingOrder.status);

                    //btn_save.IsEnabled = true;

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
               
                await refreshPreparingOrders();
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
        async Task refreshPreparingOrders()
        {
            orders = await preparingOrder.GetPreparingOrdersWithStatus(MainWindow.branchLogin.branchId, "",24);          
        }
        async Task Search()
        {
            //search
            if (orders is null)
                await refreshPreparingOrders();
            ordersQuery = orders.ToList();

            searchText = tb_search.Text.ToLower();
            ordersQuery = ordersQuery.Where(s => s.orderNum.ToLower().Contains(searchText) ).ToList();

            #region seacrch in catalog
            if (cb_searchCatalog.SelectedIndex > 0)
                ordersQuery = ordersQuery.Where(c => c.items.Where(p => p.categoryId == (int)cb_searchCatalog.SelectedValue).Any()).ToList();
            #endregion 
            #region seacrch status
            if (cb_searchStatus.SelectedIndex >0)
                ordersQuery = ordersQuery.Where(c => c.status == cb_searchStatus.SelectedValue.ToString()).ToList();
            #endregion

            #region search invoice type
            if(cb_searchInvType.SelectedIndex > 0)
            {
                List<string> invoiceTypes;

                if (cb_searchInvType.SelectedValue.ToString() == "diningHall")
                    invoiceTypes = new List<string>(){"s","sd" };

                else if(cb_searchInvType.SelectedValue.ToString() == "takeAway")
                    invoiceTypes = new List<string>() { "ts" };

                else
                    invoiceTypes = new List<string>() { "ss" }; // self service

                ordersQuery = ordersQuery.Where(c => invoiceTypes.Contains( c.invType)).ToList();
            }
            #endregion
            dg_orders.ItemsSource = ordersQuery;
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

                if (sender != null)
                    SectionData.StartAwait(grid_main);
                if (FillCombo.groupObject.HasPermissionAction(basicsPermission, FillCombo.groupObjects, "report") || SectionData.isAdminPermision())
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
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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

                if (FillCombo.groupObject.HasPermissionAction(basicsPermission, FillCombo.groupObjects, "report") || SectionData.isAdminPermision())
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
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

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

                if (FillCombo.groupObject.HasPermissionAction(basicsPermission, FillCombo.groupObjects, "report") || SectionData.isAdminPermision())
                {
                    #region
                    Window.GetWindow(this).Opacity = 0.2;
                    win_lvcCatalog win = new win_lvcCatalog(itemsQuery, 3);
                    win.ShowDialog();
                    Window.GetWindow(this).Opacity = 1;
                    #endregion
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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

                if (FillCombo.groupObject.HasPermissionAction(basicsPermission, FillCombo.groupObjects, "report") || SectionData.isAdminPermision())
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
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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

                if (FillCombo.groupObject.HasPermissionAction(basicsPermission, FillCombo.groupObjects, "report") || SectionData.isAdminPermision())
                {
                    Thread t1 = new Thread(() =>
                    {
                        ExcelPackage();

                    });
                    t1.Start();
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
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
        #region items
   
        List<ItemOrderPreparing> itemsList = new List<ItemOrderPreparing>();
        void BuildOrderItemsDesign()
        {
            sp_items.Children.Clear();
            foreach (var item in itemsList)
            {
                #region Grid Container
                Grid gridContainer = new Grid();
                int colCount = 3;
                ColumnDefinition[] cd = new ColumnDefinition[colCount];
                for (int i = 0; i < colCount; i++)
                {
                    cd[i] = new ColumnDefinition();
                }
                cd[0].Width = new GridLength(1, GridUnitType.Auto);
                cd[1].Width = new GridLength(1, GridUnitType.Star);
                cd[2].Width = new GridLength(1, GridUnitType.Auto);
                for (int i = 0; i < colCount; i++)
                {
                    gridContainer.ColumnDefinitions.Add(cd[i]);
                }
                /////////////////////////////////////////////////////
                #region   sequence
                var itemSequenceText = new TextBlock();
                itemSequenceText.Text = item.sequence + ".";
                itemSequenceText.Margin = new Thickness(5);
                itemSequenceText.Foreground = Application.Current.Resources["ThickGrey"] as SolidColorBrush;
                itemSequenceText.FontWeight = FontWeights.SemiBold;
                itemSequenceText.VerticalAlignment = VerticalAlignment.Center;
                itemSequenceText.HorizontalAlignment = HorizontalAlignment.Left;
                Grid.SetColumn(itemSequenceText, 0);
               
                gridContainer.Children.Add(itemSequenceText);
                #endregion
                #region   name
                var itemNameText = new TextBlock();
                itemNameText.Text = item.itemName;
                itemNameText.Margin = new Thickness(5);
                itemNameText.Foreground = Application.Current.Resources["ThickGrey"] as SolidColorBrush;
                //itemNameText.FontWeight = FontWeights.SemiBold;
                itemNameText.VerticalAlignment = VerticalAlignment.Center;
                itemNameText.HorizontalAlignment = HorizontalAlignment.Left;
                Grid.SetColumn(itemNameText, 1);

                gridContainer.Children.Add(itemNameText);
                #endregion
                #region   count
                var itemCountText = new TextBlock();
                itemCountText.Text = item.quantity.ToString();
                itemCountText.Margin = new Thickness(5, 5 ,10 , 5);
                itemCountText.Foreground = Application.Current.Resources["ThickGrey"] as SolidColorBrush;
                //itemCountText.FontWeight = FontWeights.SemiBold;
                itemCountText.VerticalAlignment = VerticalAlignment.Center;
                itemCountText.HorizontalAlignment = HorizontalAlignment.Left;
                Grid.SetColumn(itemCountText, 2);

                gridContainer.Children.Add(itemCountText);
                #endregion
                #endregion
                sp_items.Children.Add(gridContainer);
            }
        }



        #endregion

        #region inputEditable
        private void inputEditable(string status)
        {
            switch(status)
            {
                case "Listed":
                    gd_preparingTime.Visibility = Visibility.Visible;
                    btn_save.Content = AppSettings.resourcemanager.GetString("trPreparing");
                    btn_save.IsEnabled = true;
                    break;
                case "Preparing":
                    gd_preparingTime.Visibility = Visibility.Collapsed;
                    btn_save.Content = AppSettings.resourcemanager.GetString("trReady");
                    btn_save.IsEnabled = true;
                    break;
                case "Ready":
                    gd_preparingTime.Visibility = Visibility.Collapsed;
                    btn_save.Content = AppSettings.resourcemanager.GetString("trDone");

                    if(preparingOrder.shippingCompanyId != null) // order is take away (make done in delivery managment)
                        btn_save.IsEnabled = false;
                    else
                        btn_save.IsEnabled = true;

                    break;
                case "Done":
                    gd_preparingTime.Visibility = Visibility.Collapsed;
                    btn_save.Content = AppSettings.resourcemanager.GetString("trSave");
                    btn_save.IsEnabled = false;
                    break;
            }
        }
        #endregion

        
    }
}
