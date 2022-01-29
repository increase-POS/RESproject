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

namespace Restaurant.View.sales.reservations
{
    /// <summary>
    /// Interaction logic for uc_reservationTable.xaml
    /// </summary>
    public partial class uc_reservationTable : UserControl
     {
        public uc_reservationTable()
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
        private static uc_reservationTable _instance;
        public static uc_reservationTable Instance
        {
            get
            {
                //if (_instance == null)
                _instance = new uc_reservationTable();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        string basicsPermission = "reservationTable_basics";
        //User user = new User();
        //IEnumerable<User> usersQuery;
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
                if (MainWindow.lang.Equals("en"))
                {
                    //MainWindow.resourcemanager = new ResourceManager("Restaurant.en_file", Assembly.GetExecutingAssembly());
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    //MainWindow.resourcemanager = new ResourceManager("Restaurant.ar_file", Assembly.GetExecutingAssembly());
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

                tablesList = new List<Tables>()
            {
                new Tables{ name = "Table-001", personsCount=2, status="empty"},
                new Tables{ name = "Table-002", personsCount=3, status="open"},
                new Tables{ name = "Table-003", personsCount=4, status="reservated"},
                new Tables{ name = "Table-004", personsCount=5, status="empty"},
                new Tables{ name = "Table-005", personsCount=6, status="empty"},
                new Tables{ name = "Table-006", personsCount=7, status="reservated"},
                new Tables{ name = "Table-007", personsCount=8, status="open"},
                new Tables{ name = "Table-008", personsCount=9, status="open"},
                new Tables{ name = "Table-009", personsCount=10, status="reservated"},
                new Tables{ name = "Table-010", personsCount=11, status="empty"},
                new Tables{ name = "Table-011", personsCount=6, status="empty"},
                new Tables{ name = "Table-012", personsCount=12, status="open"},
                new Tables{ name = "Table-013", personsCount=2, status="reservated"},
                new Tables{ name = "Table-014", personsCount=8, status="empty"},
                new Tables{ name = "Table-015", personsCount=3, status="empty"},
                new Tables{ name = "Table-016", personsCount=9, status="reservated"},
                new Tables{ name = "Table-017", personsCount=5, status="open"},
            };
                BuildBillDesign();

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




        }
        #region Add - Update - Delete - Search - Tgl - Clear - DG_SelectionChanged - refresh
        private async void Btn_add_Click(object sender, RoutedEventArgs e)
        { //add
            try
            {
                HelpClass.StartAwait(grid_main);
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "add"))
                {


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
        private void Cb_customerId_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                cb_customerId.ItemsSource = FillCombo.customersList.Where(x => x.name.Contains(cb_customerId.Text));

            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Cb_table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                /*
                string s = _BarcodeStr;
                if (cb_coupon.SelectedIndex != -1)
                {
                    couponModel = coupons.ToList().Find(c => c.cId == (int)cb_coupon.SelectedValue);
                    if (couponModel != null)
                    {
                        s = couponModel.barcode;
                        await dealWithBarcode(s);
                    }
                    cb_coupon.SelectedIndex = -1;
                    cb_coupon.SelectedItem = "";
                }
                */
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Btn_clearTable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                /*
                _Discount = 0;
                selectedCoupons.Clear();
                lst_coupons.Items.Clear();
                cb_coupon.SelectedIndex = -1;
                cb_coupon.SelectedItem = "";
                refreshTotalValue();
                */
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        #region table
        List<Tables> tablesList = new List<Tables>();
        void BuildBillDesign()
        {
            wp_tablesContainer.Children.Clear();
            foreach (var item in tablesList)
            {
                #region button
                Button tableButton = new Button();
                tableButton.Tag = item.name;
                tableButton.Margin = new Thickness(5);
                tableButton.Padding = new Thickness(0);
                tableButton.Background = null;
                tableButton.BorderBrush = null;

                if (item.personsCount <= 2)
                {
                    tableButton.Height = 90;
                    tableButton.Width = 80;
                }
                else if (item.personsCount == 3)
                {
                    tableButton.Height = 130;
                    tableButton.Width = 90;
                }
                else if (item.personsCount == 4)
                {
                    tableButton.Height = 140;
                    tableButton.Width = 100;
                }
                else if (item.personsCount == 5)
                {
                    tableButton.Height = 150;
                    tableButton.Width = 110;
                }
                else if (item.personsCount == 6)
                {
                    tableButton.Height = 160;
                    tableButton.Width = 120;
                }
                else if (item.personsCount == 7)
                {
                    tableButton.Height = 170;
                    tableButton.Width = 130;
                }
                else if (item.personsCount == 8)
                {
                    tableButton.Height = 180;
                    tableButton.Width = 140;
                }
                else if (item.personsCount == 9)
                {
                    tableButton.Height = 190;
                    tableButton.Width = 150;
                }
                else if (item.personsCount > 9)
                {
                    tableButton.Height = 200;
                    tableButton.Width = 160;
                }

                tableButton.Click += tableButton_Click;

                #region Grid Container
                Grid gridContainer = new Grid();
                int rowCount = 3;
                RowDefinition[] rd = new RowDefinition[rowCount];
                for (int i = 0; i < rowCount; i++)
                {
                    rd[i] = new RowDefinition();
                }
                rd[0].Height = new GridLength(1, GridUnitType.Star);
                rd[1].Height = new GridLength(20, GridUnitType.Pixel);
                rd[2].Height = new GridLength(20, GridUnitType.Pixel);
                for (int i = 0; i < rowCount; i++)
                {
                    gridContainer.RowDefinitions.Add(rd[i]);
                }
                /////////////////////////////////////////////////////
                #region Path table
                Path pathTable = new Path();
                pathTable.Stretch = Stretch.Fill;
                pathTable.Margin = new Thickness(5);

                if (item.status == "open")
                    pathTable.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
                else if (item.status == "reservated")
                    pathTable.Fill = Application.Current.Resources["BlueTables"] as SolidColorBrush;
                else
                    pathTable.Fill = Application.Current.Resources["GreenTables"] as SolidColorBrush;

                if (item.personsCount <= 2)
                    pathTable.Data = App.Current.Resources["tablePersons2"] as Geometry;
                else if (item.personsCount == 3)
                    pathTable.Data = App.Current.Resources["tablePersons3"] as Geometry;
                else if (item.personsCount == 4)
                    pathTable.Data = App.Current.Resources["tablePersons4"] as Geometry;
                else if (item.personsCount == 5)
                    pathTable.Data = App.Current.Resources["tablePersons5"] as Geometry;
                else if (item.personsCount == 6)
                    pathTable.Data = App.Current.Resources["tablePersons6"] as Geometry;
                else if (item.personsCount == 7)
                    pathTable.Data = App.Current.Resources["tablePersons7"] as Geometry;
                else if (item.personsCount == 8)
                    pathTable.Data = App.Current.Resources["tablePersons8"] as Geometry;
                else if (item.personsCount == 9)
                    pathTable.Data = App.Current.Resources["tablePersons9"] as Geometry;
                else if (item.personsCount > 9)
                    pathTable.Data = App.Current.Resources["tablePersons9Plus"] as Geometry;

                gridContainer.Children.Add(pathTable);
                #endregion
                #region   personCount 
                if (item.personsCount > 9)
                {
                    var itemPersonCountText = new TextBlock();
                    itemPersonCountText.Text = item.personsCount.ToString();
                    itemPersonCountText.Foreground = Application.Current.Resources["White"] as SolidColorBrush;
                    itemPersonCountText.FontSize = 32;
                    itemPersonCountText.VerticalAlignment = VerticalAlignment.Center;
                    itemPersonCountText.HorizontalAlignment = HorizontalAlignment.Center;
                    gridContainer.Children.Add(itemPersonCountText);
                }
                #endregion
                #region   name
                var itemNameText = new TextBlock();
                itemNameText.Text = item.name;
                itemNameText.VerticalAlignment = VerticalAlignment.Center;
                itemNameText.HorizontalAlignment = HorizontalAlignment.Center;
                itemNameText.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                Grid.SetRow(itemNameText, 1);
                gridContainer.Children.Add(itemNameText);
                #endregion
                #region   status
                var itemStatusText = new TextBlock();
                itemStatusText.Text = item.status;
                itemStatusText.VerticalAlignment = VerticalAlignment.Center;
                itemStatusText.HorizontalAlignment = HorizontalAlignment.Center;
                itemStatusText.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                Grid.SetRow(itemStatusText, 2);
                gridContainer.Children.Add(itemStatusText);
                #endregion
                tableButton.Content = gridContainer;

                #endregion
                wp_tablesContainer.Children.Add(tableButton);
                #endregion
            }
        }
        void tableButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string tableName = button.Tag.ToString();
            var table = tablesList.Where(x => x.name == tableName).FirstOrDefault();
            MessageBox.Show("Hey you Click me! I'm  " + table.name + " & person Count is " + table.personsCount);
        }
        #endregion
    }
}
