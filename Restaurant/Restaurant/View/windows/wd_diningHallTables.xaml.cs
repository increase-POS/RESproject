using Restaurant.Classes;
using Restaurant.Classes.ApiClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for wd_diningHallTables.xaml
    /// </summary>
    /// 
    public partial class wd_diningHallTables : Window
    {
        Invoice tableInvoice = new Invoice();
        public bool isOk { get; set; }
        public List<Tables> selectedTables = new List<Tables>();
        public wd_diningHallTables()
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
        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return)
                {
                    //Btn_select_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                if (AppSettings.lang.Equals("en"))
                {
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                }
                translate();

                txt_totalCurrency.Text = AppSettings.Currency;
                FillCombo.FillTablesStatus(cb_searchStatus);

                #region loading
                loadingList = new List<keyValueBool>();
                bool isDone = true;
                loadingList.Add(new keyValueBool { key = "loading_tables", value = false });
                loadingList.Add(new keyValueBool { key = "loading_reservations", value = false });
                loadingList.Add(new keyValueBool { key = "loading_fillSectionCombo", value = false });

                loading_tables();
                loading_reservations();
                loading_fillSectionCombo();
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

            txt_title.Text = AppSettings.resourcemanager.GetString("trTables");
            txt_details.Text = AppSettings.resourcemanager.GetString("trDetails");
            txt_nextReservation.Text = AppSettings.resourcemanager.GetString("trNextReservation");
            txt_tableName.Text = AppSettings.resourcemanager.GetString("trTableName");
            txt_tableStatus.Text = AppSettings.resourcemanager.GetString("trTableStatus");
            tb_tableAvailable.Text = AppSettings.resourcemanager.GetString("trTableIsAvailable");
            txt_invoiceCode.Text = AppSettings.resourcemanager.GetString("trInvoiceCode");
            txt_startTime.Text = AppSettings.resourcemanager.GetString("trStartTime");
            txt_endTime.Text = AppSettings.resourcemanager.GetString("trExpectedEndTime");
            txt_invCustomer.Text = AppSettings.resourcemanager.GetString("trCustomer");
            txt_memberShip.Text = AppSettings.resourcemanager.GetString("trMembership");
            txt_waiter.Text = AppSettings.resourcemanager.GetString("trWaiter");
            txt_total.Text = AppSettings.resourcemanager.GetString("trTotal");
            txt_preparingOrders.Text = AppSettings.resourcemanager.GetString("trPreparingOrders");
            txt_date.Text = AppSettings.resourcemanager.GetString("trDate");
            txt_reservStartTime.Text = AppSettings.resourcemanager.GetString("trStartTime");
            txt_reservEndTime.Text = AppSettings.resourcemanager.GetString("trEndTime");
            txt_personsCount.Text = AppSettings.resourcemanager.GetString("trPersonsCount");
   

            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_searchSection, AppSettings.resourcemanager.GetString("trSectionHint"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_searchStatus, AppSettings.resourcemanager.GetString("trStatusHint"));

            txt_statusEmpty.Text = AppSettings.resourcemanager.GetString("trEmpty");
            txt_statusOpen.Text = AppSettings.resourcemanager.GetString("trOpened");
            txt_statusReservated.Text = AppSettings.resourcemanager.GetString("trReserved");


            btn_refresh.ToolTip = AppSettings.resourcemanager.GetString("trRefresh");
            btn_clear.ToolTip = AppSettings.resourcemanager.GetString("trClear");

            txt_tablesContainer.Text = AppSettings.resourcemanager.GetString("trTables");
            txt_reservationsContainer.Text = AppSettings.resourcemanager.GetString("trReservations");

            // btn_save.Content = AppSettings.resourcemanager.GetString("trSave");

        }
        #region loading
        List<keyValueBool> loadingList;
        async Task loading_tables()
        {
            try
            {
                await refreshTablesList();
            }
            catch { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_tables"))
                {
                    item.value = true;
                    break;
                }
            }
        }

        async Task loading_reservations()
        {
            //try
            {
                await refreshReservationsList();
            }
            //catch { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_reservations"))
                {
                    item.value = true;
                    break;
                }
            }
        }

        async Task loading_fillSectionCombo()
        {

            //await refreshReservationsList();

            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_fillSectionCombo"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        
        #endregion
        #region Refresh & Search
        int sectionId = 0;
        async Task Search()
        {
            tablesQuery = tablesList;   
            if (cb_searchSection.SelectedIndex > 0)
            {
                sectionId = (int)cb_searchSection.SelectedValue;
                tablesQuery = tablesQuery.Where(s => s.sectionId == sectionId).ToList();
            }
            else
                sectionId = 0;

            if(cb_searchStatus.SelectedIndex != -1)
                tablesQuery = tablesQuery.Where(s => s.status == cb_searchStatus.SelectedValue.ToString()).ToList();
            BuildTablesDesign();
        }
        async Task refreshTablesList()
        {
            tablesList = await FillCombo.table.GetTablesStatusInfo(MainWindow.branchLogin.branchId,"", "", "");
        }
        async Task refreshReservationsList()
        {
            reservationsList = await reservation.Get(MainWindow.branchLogin.branchId);
        }
        #endregion      
        private void Btn_colse_Click(object sender, RoutedEventArgs e)
        {
            isOk = false;
            this.Close();
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

        #region table
        List<Tables> tablesList = new List<Tables>();
        List<Tables> tablesQuery = new List<Tables>();
        IEnumerable<TablesReservation> reservationsList;
        TablesReservation reservation = new TablesReservation();
        void BuildTablesDesign()
        {
            wp_tablesContainer.Children.Clear();

            foreach (var item in tablesQuery)
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

                if (item.status == "opened" || item.status == "openedReserved")
                    pathTable.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
                else if (item.status == "reserved") 
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

                if (item.status == "opened" || item.status == "openedReserved")
                    itemStatusText.Text = AppSettings.resourcemanager.GetString("trOpened");
                else if (item.status == "reserved")
                    itemStatusText.Text = AppSettings.resourcemanager.GetString("trReserved");
                else
                    itemStatusText.Text = AppSettings.resourcemanager.GetString("trEmpty");
                //itemStatusText.Text = item.status;
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
        async void tableButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string tableName =button.Tag.ToString();
            var table = tablesList.Where(x => x.name == tableName).FirstOrDefault();
            await  showDetails(table);
            MessageBox.Show("Hey you Click me! I'm  " + table.name + " & person Count is " + table.personsCount);
        }

        private async Task showDetails(Tables table)
        {
            
            tb_tableName.Text = table.name;
            tb_tableStatus.Text = table.status;
            switch (table.status)
            {
                case "empty":
                    grid_emptyTableDetails.Visibility = Visibility.Visible;
                    grid_openTableDetails.Visibility = Visibility.Collapsed;
                    break;
                case "opened":
                case "openedReserved":
                    grid_emptyTableDetails.Visibility = Visibility.Collapsed;
                    grid_openTableDetails.Visibility = Visibility.Visible;
                    tableInvoice = await FillCombo.table.GetTableInvoice(table.tableId);

                    tb_InvoiceCode.Text = tableInvoice.invNumber;
                    tb_startTime.Text = tableInvoice.invDate.ToString().Split(' ')[1];
                    TimeSpan startTime = TimeSpan.Parse(tableInvoice.invDate.ToString().Split(' ')[1]);
                    TimeSpan timeStaying = TimeSpan.FromHours(AppSettings.time_staying);
                    tb_endTime.Text = timeStaying.ToString();
                    tb_customerName.Text = tableInvoice.agentName;
                    tb_total.Text = tableInvoice.totalNet.ToString();
                    break;
            }
            #region next reservation
            TablesReservation nextReservation = null;
            foreach (var res in reservationsList)
            {
                var found = res.tables.Where(x => x.tableId == table.tableId && res.reservationDate >= DateTime.Now);
                if (found != null)
                {
                    nextReservation = res;
                    break;
                }
            }
            if(nextReservation != null)
            {
                dp_reservatedTableTitle.Visibility = Visibility.Visible;
                tb_reservCode.Text = nextReservation.code;
                tb_date.Text = nextReservation.reservationDate.ToString().Split(' ')[0];
                tb_reservStartTime.Text = nextReservation.reservationTime.ToString().Split(' ')[1];
                tb_reservEndTime.Text = nextReservation.endTime.ToString().Split(' ')[1];
                tb_personsCount.Text = nextReservation.personsCount.ToString();
                tb_reservCustomerName.Text = nextReservation.customerName;
            }
            else
                dp_reservatedTableTitle.Visibility = Visibility.Collapsed;

            #endregion
        }
        #endregion

        private async void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {//refresh

                HelpClass.StartAwait(grid_main);
                await refreshTablesList();
                await Search();
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #region container

        private void Btn_tablesContainer_Click(object sender, RoutedEventArgs e)
        {
            wp_tablesContainer.Visibility = Visibility.Visible;
            path_tablesContainer.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
            txt_tablesContainer.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;

            grid_reservationsContainer.Visibility = Visibility.Collapsed;
            path_reservationsContainer.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            txt_reservationsContainer.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
        }

        private void Btn_reservationsContainer_Click(object sender, RoutedEventArgs e)
        {
            wp_tablesContainer.Visibility = Visibility.Collapsed;
            path_tablesContainer.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            txt_tablesContainer.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;

            grid_reservationsContainer.Visibility = Visibility.Visible;
            path_reservationsContainer.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
            txt_reservationsContainer.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
        }

        #endregion

        private async void Dg_reservation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
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
                    tb_personsCount.Text = _PersonsCount.ToString();
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
            */
        }
        #region Button In DataGrid

        void cancelRowinDatagridTable(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                    if (vis is DataGridRow)
                    {
                        Tables row = (Tables)dg_tables.SelectedItems[0];
                        selectedTables.Remove(row);
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

        async void confirmRowinDatagridReservation(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);




                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        async void cancelRowinDatagridReservation(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);




                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion
    }
}
