﻿using netoaster;
using Restaurant.Classes;
using Restaurant.Classes.ApiClasses;
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

namespace Restaurant.View.sales.reservations
{
    /// <summary>
    /// Interaction logic for uc_reservationsUpdate.xaml
    /// </summary>
    public partial class uc_reservationsUpdate : UserControl
    {
        public uc_reservationsUpdate()
        {
            try
            {
                InitializeComponent();
                if (System.Windows.SystemParameters.PrimaryScreenWidth >= 1440)
                {
                    txt_deleteButton.Visibility = Visibility.Visible;
                    //txt_addButton.Visibility = Visibility.Visible;
                    txt_updateButton.Visibility = Visibility.Visible;
                    //txt_add_Icon.Visibility = Visibility.Visible;
                    txt_update_Icon.Visibility = Visibility.Visible;
                    txt_delete_Icon.Visibility = Visibility.Visible;
                }
                else if (System.Windows.SystemParameters.PrimaryScreenWidth >= 1360)
                {
                    //txt_add_Icon.Visibility = Visibility.Collapsed;
                    txt_update_Icon.Visibility = Visibility.Collapsed;
                    txt_delete_Icon.Visibility = Visibility.Collapsed;
                    txt_deleteButton.Visibility = Visibility.Visible;
                    //txt_addButton.Visibility = Visibility.Visible;
                    txt_updateButton.Visibility = Visibility.Visible;

                }
                else
                {
                    txt_deleteButton.Visibility = Visibility.Collapsed;
                    //txt_addButton.Visibility = Visibility.Collapsed;
                    txt_updateButton.Visibility = Visibility.Collapsed;
                    //txt_add_Icon.Visibility = Visibility.Visible;
                    txt_update_Icon.Visibility = Visibility.Visible;
                    txt_delete_Icon.Visibility = Visibility.Visible;

                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private static uc_reservationsUpdate _instance;
        public static uc_reservationsUpdate Instance
        {
            get
            {
                //if (_instance == null)
                if(_instance is null)
                _instance = new uc_reservationsUpdate();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        string basicsPermission = "reservationsUpdate_basics";
        IEnumerable<TablesReservation> reservationsList;
        IEnumerable<TablesReservation> reservationsQuery;
        TablesReservation reservation = new TablesReservation();
        List<Tables> selectedTables = new List<Tables>();
        int _PersonsCount = 0;
        string searchText = "";
        public static List<string> requiredControlList;
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }
        #region loading
        List<keyValueBool> loadingList;
        async Task loading_reservaitions()
        {
            //try
            {
                await refreshReservaitionsList();
            }
            //catch { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_reservaitions"))
                {
                    item.value = true;
                    break;
                }
            }
        }
       
        async void loading_fillCustomerCombo()
        {
            try
            {
                await FillCombo.FillComboCustomers(cb_customerId);
            }
            catch (Exception)
            { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_fillCustomerCombo"))
                {
                    item.value = true;
                    break;
                }
            }
        }
      
        #endregion
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {//load
            try
            {
                HelpClass.StartAwait(grid_main);
                requiredControlList = new List<string> { "reservationDate", "reservationStartTime", "reservationEndTime", "personsCount" };
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
                loadingList.Add(new keyValueBool { key = "loading_reservaitions", value = false });
                loadingList.Add(new keyValueBool { key = "loading_fillCustomerCombo", value = false });

                loading_reservaitions();
                loading_fillCustomerCombo();
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
                await Search();

                btn_confirm.IsEnabled = false;
                btn_cancel.IsEnabled = false;

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
            #region datagrid
            dg_reservation.Columns[0].Header = AppSettings.resourcemanager.GetString("trCode");
            dg_reservation.Columns[1].Header = AppSettings.resourcemanager.GetString("trDate");
            dg_reservation.Columns[2].Header = AppSettings.resourcemanager.GetString("trTime");
            dg_reservation.Columns[3].Header = AppSettings.resourcemanager.GetString("trCount");
            dg_reservation.Columns[4].Header = AppSettings.resourcemanager.GetString("trCustomer");
            dg_reservation.Columns[5].Header = AppSettings.resourcemanager.GetString("trStatus");
            #endregion
            txt_title.Text = AppSettings.resourcemanager.GetString("trReservations");
            txt_baseInformation.Text = AppSettings.resourcemanager.GetString("trBaseInformation");
            txt_tables.Text = AppSettings.resourcemanager.GetString("trTables");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_customerId, AppSettings.resourcemanager.GetString("trCustomerHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(dp_reservationDate, AppSettings.resourcemanager.GetString("trReservationDaterHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tp_reservationTime, AppSettings.resourcemanager.GetString("trStartTimeHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tp_reservationEndTime, AppSettings.resourcemanager.GetString("trEndTimeHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_personsCount, AppSettings.resourcemanager.GetString("trPersonsCountHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_notes, AppSettings.resourcemanager.GetString("trNoteHint"));

            btn_refresh.ToolTip = AppSettings.resourcemanager.GetString("trRefresh");
            btn_clear.ToolTip = AppSettings.resourcemanager.GetString("trClear");
            btn_pdf.ToolTip = AppSettings.resourcemanager.GetString("trPdf");
            btn_print.ToolTip = AppSettings.resourcemanager.GetString("trPrint");
            btn_pieChart.ToolTip = AppSettings.resourcemanager.GetString("trPieChart");
            btn_exportToExcel.ToolTip = AppSettings.resourcemanager.GetString("trExcel");
            btn_preview.ToolTip = AppSettings.resourcemanager.GetString("trPreview");
            txt_count.ToolTip = AppSettings.resourcemanager.GetString("trCount");
        }
        #region Update - confirm - Search - Tgl - Clear - DG_SelectionChanged - refresh
       
        private async void Btn_update_Click(object sender, RoutedEventArgs e)
        {//update
            try
            {
                HelpClass.StartAwait(grid_main);
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "update"))
                {
                    if (reservation.reservationId > 0)
                    {
                        if (HelpClass.validate(requiredControlList, this))
                    {
                            bool valid1 = await validateReservationTime();
                            bool valid2 = validatePersonsCount();
                            if (valid1 && valid2)
                        {
                            reservation.branchId = MainWindow.branchLogin.branchId;
                            //reserve.code = await reserve.generateReserveCode("tr", MainWindow.branchLogin.code, MainWindow.branchLogin.branchId);
                            if (cb_customerId.SelectedIndex > 0)
                                reservation.customerId = (int)cb_customerId.SelectedValue;
                            reservation.reservationDate = dp_reservationDate.SelectedDate;

                            #region reservation time period                      
                            DateTime startTime = DateTime.Parse(dp_reservationDate.SelectedDate.ToString().Split(' ')[0]
                                    + ' ' + tp_reservationTime.SelectedTime.ToString().Split(' ')[1]
                                    + ' ' + tp_reservationEndTime.SelectedTime.ToString().Split(' ')[2]);

                            var dateOfStartTime = DateTime.Parse(tp_reservationTime.SelectedTime.ToString().Split(' ')[0]);
                            var dateOfEndTime = DateTime.Parse(tp_reservationEndTime.SelectedTime.ToString().Split(' ')[0]);
                            var difference = (dateOfEndTime - dateOfStartTime).Days;
                            DateTime reservationEndDate = DateTime.Parse(dp_reservationDate.SelectedDate.ToString().Split(' ')[0]);
                            if (difference > 0)
                            {
                                reservationEndDate = DateTime.Parse(dp_reservationDate.SelectedDate.ToString().Split(' ')[0]).AddDays(1);
                            }
                            DateTime endTime = DateTime.Parse(reservationEndDate.ToString().Split(' ')[0]
                                                + ' ' + tp_reservationEndTime.SelectedTime.ToString().Split(' ')[1]
                                                + ' ' + tp_reservationEndTime.SelectedTime.ToString().Split(' ')[2]);

                            reservation.reservationTime = startTime;
                            reservation.endTime = endTime;
                            #endregion
                            reservation.personsCount = int.Parse(tb_personsCount.Text);
                            reservation.notes = tb_notes.Text;
                            //save
                            int res = await reservation.updateReservation(reservation, selectedTables);
                            if (res > 0)
                            {
                                Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);
                                Clear();
                                await refreshReservaitionsList();
                                await Search();

                            }
                            else
                                Toaster.ShowError(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);

                        }
                    }
                    }
                    else
                        Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trSelectItemFirst"), animation: ToasterAnimation.FadeIn);
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
        async Task<Boolean> validateReservationTime()
        {
            bool valid = true;
            string message = "";
            DateTime startTime = DateTime.Parse(dp_reservationDate.SelectedDate.ToString().Split(' ')[0]
                                + ' ' + tp_reservationTime.SelectedTime.ToString().Split(' ')[1]
                                + ' ' + tp_reservationEndTime.SelectedTime.ToString().Split(' ')[2]);

            DateTime endTime = DateTime.Parse(dp_reservationDate.SelectedDate.ToString().Split(' ')[0]
                                + ' ' + tp_reservationEndTime.SelectedTime.ToString().Split(' ')[1]
                                + ' ' + tp_reservationEndTime.SelectedTime.ToString().Split(' ')[2]);
            foreach (Tables tb in selectedTables)
            {
                int notReserved = await FillCombo.table.checkTableAvailabiltiy(tb.tableId, MainWindow.branchLogin.branchId,
                                                         dp_reservationDate.SelectedDate.ToString(),
                                                         startTime.ToString(),
                                                         endTime.ToString(), reservation.reservationId);

                if (notReserved == 0)
                {
                    valid = false;
                    if (message == "")
                        message += tb.name;
                    else
                        message += ", " + tb.name;
                }
                message += " ";
                if (!valid)
                    Toaster.ShowWarning(Window.GetWindow(this), message: message + AppSettings.resourcemanager.GetString("trReserved"), animation: ToasterAnimation.FadeIn);
            }
            return valid;
        }
        bool validatePersonsCount()
        {
            bool valid = true;
            if (int.Parse(tb_personsCount.Text) > _PersonsCount)
            {
                Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trCountMoreTableCapacity"), animation: ToasterAnimation.FadeIn);
                valid = false;
            }
            return valid;
        }
        private async void Btn_confirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {//delete
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "update"))
                {
                    HelpClass.StartAwait(grid_main);
                    int res = await reservation.updateReservationStatus(reservation.reservationId,"confirm",MainWindow.userLogin.userId);
                    if (res > 0)
                    {
                        Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);
                        Clear();
                        await refreshReservaitionsList();
                        await Search();
                    }
                    else
                        Toaster.ShowError(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                    HelpClass.EndAwait(grid_main);
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

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
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "delete"))
                {
                    HelpClass.StartAwait(grid_main);
                    int res = await reservation.updateReservationStatus(reservation.reservationId, "cancle", MainWindow.userLogin.userId);
                    if (res > 0)
                    {
                        reservation.reservationId = 0;
                        Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);
                        Clear();
                        await refreshReservaitionsList();
                        await Search();
                    }
                    else
                        Toaster.ShowError(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                    HelpClass.EndAwait(grid_main);
                }
                else
                    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);

            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        /*
        private async Task activate()
        {//activate
            user.isActive = 1;
            int s = await user.save(user);
            if (s <= 0)
                Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
            else
            {
                Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopActive"), animation: ToasterAnimation.FadeIn);
                await RefreshUsersList();
                await Search();
            }
        }
        */
        #endregion
        #region events
        private void Btn_tables_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                MainWindow.mainWindow.Opacity = 0.2;

                wd_tablesList w = new wd_tablesList();
                w.page = "reservationUpdate";
                w.reservationId = reservation.reservationId;
                w.selectedTables = selectedTables;
                w.ShowDialog();

                if (w.DialogResult == true)
                {
                    _PersonsCount = 0;
                    foreach (var table in w.selectedTables)
                        _PersonsCount += table.personsCount;
                    selectedTables = w.selectedTables;

                    dg_tables.ItemsSource = null;
                    dg_tables.ItemsSource = selectedTables;
                }
                MainWindow.mainWindow.Opacity = 1;
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
                Clear();
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Dg_reservation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                //selection

                if (dg_reservation.SelectedIndex != -1)
                {
                    reservation = new TablesReservation();
                    reservation = dg_reservation.SelectedItem as TablesReservation;
                    this.DataContext = reservation;
                    _PersonsCount = (int)reservation.personsCount;
                   // tb_personsCount.Text = _PersonsCount.ToString();
                    if (reservation.tables.Count != 0)
                    {
                        selectedTables = reservation.tables;                      
                    }
                    dg_tables.ItemsSource = selectedTables;

                    btn_tables.IsEnabled = true;
                    btn_confirm.IsEnabled = true;
                    btn_cancel.IsEnabled = true;

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
                await refreshReservaitionsList();
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
            //search
            if (reservationsList is null)
                await refreshReservaitionsList();
            searchText = tb_search.Text.ToLower();
            reservationsQuery = reservationsList.Where(s => s.code.ToLower().Contains(searchText) ||
            s.status.ToLower().Contains(searchText));
            RefreshReservationsView();
        }
        async Task refreshReservaitionsList()
        {
            reservationsList = await reservation.Get(MainWindow.branchLogin.branchId);
        }
        void RefreshReservationsView()
        {
            dg_reservation.ItemsSource = reservationsQuery;
            txt_count.Text = reservationsQuery.Count().ToString();
        }
        #endregion
        #region validate - clearValidate - textChange - lostFocus - . . . . 
        void Clear()
        {
            this.DataContext = new TablesReservation() ;
            dg_tables.ItemsSource = null;
            _PersonsCount = 0;
           // tb_personsCount.Text = "";
            // last 
            HelpClass.clearValidate(requiredControlList, this);
            btn_tables.IsEnabled = false;
            btn_confirm.IsEnabled = false;
            btn_cancel.IsEnabled = false;
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
        #region Button In DataGrid

        void cancelRowinDatagrid(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                    if (vis is DataGridRow)
                    {
                        Tables row = (Tables)dg_tables.SelectedItems[0];
                        selectedTables.Remove(row);
                        _PersonsCount -= row.personsCount;
                        //tb_personsCount.Text = _PersonsCount.ToString();

                        dg_tables.ItemsSource = null;
                        dg_tables.ItemsSource = selectedTables;
                    }
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
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

                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "report") || SectionData.isAdminPermision())
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

   
    }
}
