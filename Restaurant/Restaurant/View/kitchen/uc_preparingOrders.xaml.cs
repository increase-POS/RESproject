﻿using netoaster;
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

        string basicsPermission = "preparingOrders_basics";
        OrderPreparing preparingOrder = new OrderPreparing();
        List<OrderPreparing> orders = new List<OrderPreparing>();
        //IEnumerable<User> users;
        //byte tgl_userState;
        string searchText = "";
        public static List<string> requiredControlList;
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }
        #region loading
        List<keyValueBool> loadingList;
        //async void loading_fillCustomerCombo()
        //{
        //    try
        //    {
        //        await FillCombo.FillComboCustomers(cb_customerId);
        //    }
        //    catch (Exception)
        //    { }
        //    foreach (var item in loadingList)
        //    {
        //        if (item.key.Equals("loading_fillCustomerCombo"))
        //        {
        //            item.value = true;
        //            break;
        //        }
        //    }
        //}
        #endregion
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {//load
            try
            {
                HelpClass.StartAwait(grid_main);
                requiredControlList = new List<string> { "" };
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
                //loadingList.Add(new keyValueBool { key = "loading_fillCustomerCombo", value = false });


                //loading_fillCustomerCombo();
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
                await refreshPreparingOrders();

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

            #region dg_orders
            dg_orders.Columns[1].Header = "";
            dg_orders.Columns[2].Header = AppSettings.resourcemanager.GetString("trRemainingTime");
            dg_orders.Columns[3].Header = AppSettings.resourcemanager.GetString("trStatus");
            dg_orders.Columns[4].Header = AppSettings.resourcemanager.GetString("trNote");
            #endregion

            txt_title.Text = AppSettings.resourcemanager.GetString("trOrder");
            txt_details.Text = AppSettings.resourcemanager.GetString("trDetails");
            txt_items.Text = AppSettings.resourcemanager.GetString("trItems");
            //txt_byMinute.Text = AppSettings.resourcemanager.GetString("trDetails");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_notes, AppSettings.resourcemanager.GetString("trNoteHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_search, AppSettings.resourcemanager.GetString("trSearchHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_searchStatus, AppSettings.resourcemanager.GetString("trStatusHint"));
           // MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_searchCatalog, AppSettings.resourcemanager.GetString("trSearchHint"));

            btn_save.Content = AppSettings.resourcemanager.GetString("trSave");
        }
        #region loading
        async Task refreshPreparingOrders()
        {
            string status = "Listed, Cooking, Cooked";
            orders = await preparingOrder.GetPreparingOrdersWithStatus(MainWindow.branchLogin.branchId,status);
            dg_orders.ItemsSource = orders;
        }
        #endregion
        #region Add - Update - Delete - Search - Tgl - Clear - DG_SelectionChanged - refresh
        private async void Btn_save_Click(object sender, RoutedEventArgs e)
        { //add
            try
            {
                HelpClass.StartAwait(grid_main);
                if (FillCombo.groupObject.HasPermissionAction(basicsPermission, FillCombo.groupObjects, "add"))
                {


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
        #region events
        private async void Tb_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                //await Search();
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
                /*
                await RefreshUsersList();
                await Search();
                */
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
            if (users is null)
                await RefreshUsersList();
            searchText = tb_search.Text.ToLower();
            usersQuery = users.Where(s => (s.name.ToLower().Contains(searchText) ||
            s.job.ToLower().Contains(searchText)
            ) && s.isActive == tgl_userState);
            RefreshCustomersView();
        }
        async Task<IEnumerable<User>> RefreshUsersList()
        {

            users = await user.Get();
            return users;

        }
        void RefreshCustomersView()
        {
            dg_user.ItemsSource = usersQuery;
            txt_count.Text = usersQuery.Count().ToString();
        }
        */
        #endregion
        #region validate - clearValidate - textChange - lostFocus - . . . . 
        void Clear()
        {
            //user = new User();
            //user.workHours = "0";
            //this.DataContext = user;




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
        class itemsOrder
        {
            public int sequence { get; set; }
            public string name { get; set; }
            public int count { get; set; }
        }
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

       
    }
}
