﻿using netoaster;
using Restaurant.Classes;
using Restaurant.View.windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Group = Restaurant.Classes.Group;
using Object = Restaurant.Classes.Object;

namespace Restaurant.View.settings
{
    /// <summary>
    /// Interaction logic for uc_permissions.xaml
    /// </summary>
    public partial class uc_permissions : UserControl
    {
        public uc_permissions()
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
        private static uc_permissions _instance;
        public static uc_permissions Instance
        {
            get
            {
                //if (_instance == null)
                _instance = new uc_permissions();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        string basicsPermission = "permissions_basics";
        string usersPermission = "Permissions_users";
        string _parentObjectName = "categories";
        Group group = new Group();
        IEnumerable<Group> groupsQuery;
        IEnumerable<Group> groups;
        byte tgl_groupState;
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
                requiredControlList = new List<string> { "name" };
                if (MainWindow.lang.Equals("en"))
                {
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                }
                translate();

                Keyboard.Focus(tb_name);
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

            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_search, MainWindow.resourcemanager.GetString("trSearchHint"));
            btn_refresh.ToolTip = MainWindow.resourcemanager.GetString("trRefresh");
            btn_clear.ToolTip = MainWindow.resourcemanager.GetString("trClear");

            txt_addButton.Text = MainWindow.resourcemanager.GetString("trAdd");
            txt_updateButton.Text = MainWindow.resourcemanager.GetString("trUpdate");
            txt_deleteButton.Text = MainWindow.resourcemanager.GetString("trDelete");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_notes, MainWindow.resourcemanager.GetString("trNoteHint"));

            dg_group.Columns[0].Header = MainWindow.resourcemanager.GetString("trName");
            dg_group.Columns[1].Header = MainWindow.resourcemanager.GetString("trNote");
            /*
            txt_dashboard.Text = MainWindow.resourcemanager.GetString("trDashBoard");


            txt_categories.Text = MainWindow.resourcemanager.GetString("trCategories");
            txt_properties.Text = MainWindow.resourcemanager.GetString("trProperties");
            txt_item.Text = MainWindow.resourcemanager.GetString("trItems");
            txt_package.Text = MainWindow.resourcemanager.GetString("trPackage");
            txt_service.Text = MainWindow.resourcemanager.GetString("trService");
            txt_units.Text = MainWindow.resourcemanager.GetString("trUnits");
            txt_storageCost.Text = MainWindow.resourcemanager.GetString("trStorageCost");

            txt_locations.Text = MainWindow.resourcemanager.GetString("trLocation");
            txt_section.Text = MainWindow.resourcemanager.GetString("trSection");
            txt_reciptOfInvoice.Text = MainWindow.resourcemanager.GetString("trInvoice");
            txt_itemsStorage.Text = MainWindow.resourcemanager.GetString("trStorage");
            txt_importExport.Text = MainWindow.resourcemanager.GetString("trMovements");
            txt_itemsDestroy.Text = MainWindow.resourcemanager.GetString("trDestructive");
            txt_shortage.Text = MainWindow.resourcemanager.GetString("trShortage");
            txt_inventory.Text = MainWindow.resourcemanager.GetString("trStocktaking");


            txt_posAccounting.Text = MainWindow.resourcemanager.GetString("trPOS");
            txt_banksAccounting.Text = MainWindow.resourcemanager.GetString("trGroups");
            txt_payments.Text = MainWindow.resourcemanager.GetString("trPayments");
            txt_received.Text = MainWindow.resourcemanager.GetString("trReceived");
            txt_bonds.Text = MainWindow.resourcemanager.GetString("trBonds");
            txt_ordersAccounting.Text = MainWindow.resourcemanager.GetString("trOrders");

            txt_payInvoice.Text = MainWindow.resourcemanager.GetString("trInvoice");
            txt_purchaseOrder.Text = MainWindow.resourcemanager.GetString("trOrders");
            txt_salesStatistic.Text = MainWindow.resourcemanager.GetString("trStatistic");

            txt_reciptInvoice.Text = MainWindow.resourcemanager.GetString("trInvoice");
            txt_coupon.Text = MainWindow.resourcemanager.GetString("trCoupon");
            txt_offer.Text = MainWindow.resourcemanager.GetString("trOffer");

            txt_quotation.Text = MainWindow.resourcemanager.GetString("trQuotations");
            txt_salesOrders.Text = MainWindow.resourcemanager.GetString("trOrders");

            txt_customers.Text = MainWindow.resourcemanager.GetString("trCustomers");
            txt_suppliers.Text = MainWindow.resourcemanager.GetString("trSuppliers");
            txt_users.Text = MainWindow.resourcemanager.GetString("trUsers");
            txt_branches.Text = MainWindow.resourcemanager.GetString("trBranches");
            txt_stores.Text = MainWindow.resourcemanager.GetString("trStores");
            txt_pos.Text = MainWindow.resourcemanager.GetString("trPOS");
            txt_groups.Text = MainWindow.resourcemanager.GetString("trGroups");
            txt_shippingCompany.Text = MainWindow.resourcemanager.GetString("trShipping");

            txt_storageReports.Text = MainWindow.resourcemanager.GetString("trStore");
            txt_purchaseReports.Text = MainWindow.resourcemanager.GetString("trPurchases");
            txt_salesReports.Text = MainWindow.resourcemanager.GetString("trSales");
            txt_accountsReports.Text = MainWindow.resourcemanager.GetString("trAccounting");

            txt_storageAlerts.Text = MainWindow.resourcemanager.GetString("trStore");
            txt_saleAlerts.Text = MainWindow.resourcemanager.GetString("trSales");

            txt_general.Text = MainWindow.resourcemanager.GetString("trGeneral");
            txt_reportsSettings.Text = MainWindow.resourcemanager.GetString("trReports");
            txt_permissions.Text = MainWindow.resourcemanager.GetString("trPermission");
            txt_emailsSetting.Text = MainWindow.resourcemanager.GetString("trEmail");
            txt_emailTemplates.Text = MainWindow.resourcemanager.GetString("trEmailTemplates");
            */

            dg_permissions.Columns[0].Header = MainWindow.resourcemanager.GetString("trPermission");
            dg_permissions.Columns[1].Header = MainWindow.resourcemanager.GetString("trShow");
            dg_permissions.Columns[2].Header = MainWindow.resourcemanager.GetString("trAdd");
            dg_permissions.Columns[3].Header = MainWindow.resourcemanager.GetString("trUpdate");
            dg_permissions.Columns[4].Header = MainWindow.resourcemanager.GetString("trDelete");
            dg_permissions.Columns[5].Header = MainWindow.resourcemanager.GetString("trReports");


            txt_title.Text = MainWindow.resourcemanager.GetString("trPermission");
            txt_groupDetails.Text = MainWindow.resourcemanager.GetString("trDetails");
            txt_groups.Text = MainWindow.resourcemanager.GetString("trGroups");


        }
        #region Add - Update - Delete - Search - Tgl - Clear - DG_SelectionChanged - refresh
        private async void Btn_add_Click(object sender, RoutedEventArgs e)
        {//add
            try
            {
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "add") || HelpClass.isAdminPermision())
                {
                    HelpClass.StartAwait(grid_main);

                    group = new Group();
                    if (HelpClass.validate(requiredControlList, this))
                    {

                        bool isGroupExist = await chkDuplicateGroup();
                        if (isGroupExist)
                        {
                            Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopGroupExist"), animation: ToasterAnimation.FadeIn);
                            #region Tooltip_name
                            p_error_name.Visibility = Visibility.Visible;
                            ToolTip toolTip_name = new ToolTip();
                            toolTip_name.Content = MainWindow.resourcemanager.GetString("trDuplicateCodeToolTip");
                            toolTip_name.Style = Application.Current.Resources["ToolTipError"] as Style;
                            p_error_name.ToolTip = toolTip_name;
                            #endregion
                        }
                        else
                        {
                            group.name = tb_name.Text;
                            group.createUserId = MainWindow.userLogin.userId;
                            group.updateUserId = MainWindow.userLogin.userId;
                            group.notes = tb_notes.Text;
                            group.isActive = 1;

                            int s = await group.Save(group);
                            if (s <= 0)
                                Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                            else
                            {
                                Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopAdd"), animation: ToasterAnimation.FadeIn);



                                Clear();
                                await RefreshGroupsList();
                                await Search();
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
        private async void Btn_update_Click(object sender, RoutedEventArgs e)
        {//update
            try
            {
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "update") || HelpClass.isAdminPermision())
                {
                    HelpClass.StartAwait(grid_main);
                    if (HelpClass.validate(requiredControlList, this))
                    {
                        bool isGroupExist = await chkDuplicateGroup();
                        if (isGroupExist)
                        {
                            Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopGroupExist"), animation: ToasterAnimation.FadeIn);
                            #region Tooltip_name
                            p_error_name.Visibility = Visibility.Visible;
                            ToolTip toolTip_name = new ToolTip();
                            toolTip_name.Content = MainWindow.resourcemanager.GetString("trDuplicateCodeToolTip");
                            toolTip_name.Style = Application.Current.Resources["ToolTipError"] as Style;
                            p_error_name.ToolTip = toolTip_name;
                            #endregion
                        }
                        else
                        {
                            group.name = tb_name.Text;
                            group.updateUserId = MainWindow.userLogin.userId;
                            group.notes = tb_notes.Text;

                            int s = await group.Save(group);
                            if (s <= 0)
                                Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                            else
                            {
                                group.groupId = s;
                                Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);
                                await RefreshGroupsList();
                                await Search();

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
        private async void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {//delete
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "delete") || HelpClass.isAdminPermision())
                {
                    HelpClass.StartAwait(grid_main);
                    if (group.groupId != 0)
                    {
                        if ((!group.canDelete) && (group.isActive == 0))
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
                            if (group.canDelete)
                                w.contentText = MainWindow.resourcemanager.GetString("trMessageBoxDelete");
                            if (!group.canDelete)
                                w.contentText = MainWindow.resourcemanager.GetString("trMessageBoxDeactivate");
                            w.ShowDialog();
                            Window.GetWindow(this).Opacity = 1;
                            #endregion
                            if (w.isOk)
                            {
                                string popupContent = "";
                                if (group.canDelete) popupContent = MainWindow.resourcemanager.GetString("trPopDelete");
                                if ((!group.canDelete) && (group.isActive == 1)) popupContent = MainWindow.resourcemanager.GetString("trPopInActive");

                                int s = await group.Delete(group.groupId, MainWindow.userLogin.userId, group.canDelete);
                                if (s < 0)
                                    Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                                else
                                {
                                    Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopDelete"), animation: ToasterAnimation.FadeIn);

                                    await RefreshGroupsList();
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
            group.isActive = 1;
            int s = await group.Save(group);
            if (s <= 0)
                Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
            else
            {
                Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopActive"), animation: ToasterAnimation.FadeIn);
                await RefreshGroupsList();
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
                if (groups is null)
                    await RefreshGroupsList();
                tgl_groupState = 1;
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
                if (groups is null)
                    await RefreshGroupsList();
                tgl_groupState = 0;
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
        private void Dg_group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                //selection
                if (dg_group.SelectedIndex != -1)
                {
                    group = dg_group.SelectedItem as Group;
                    this.DataContext = group;
                    if (group != null)
                    {
                        #region delete
                        if (group.canDelete)
                            btn_delete.Content = MainWindow.resourcemanager.GetString("trDelete");
                        else
                        {
                            if (group.isActive == 0)
                                btn_delete.Content = MainWindow.resourcemanager.GetString("trActive");
                            else
                                btn_delete.Content = MainWindow.resourcemanager.GetString("trInActive");
                        }
                        #endregion
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
        {//refresh
            try
            {

                HelpClass.StartAwait(grid_main);

                searchText = "";
                tb_search.Text = "";
                await RefreshGroupsList();
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
            if (groups is null)
                await RefreshGroupsList();
            searchText = tb_search.Text.ToLower();
            groupsQuery = groups.Where(s => (s.name.ToLower().Contains(searchText)
            ) && s.isActive == tgl_groupState);
            RefreshCustomersView();
        }
        async Task<IEnumerable<Group>> RefreshGroupsList()
        {
            groups = await group.GetAll();
            return groups;
        }
        void RefreshCustomersView()
        {
            dg_group.ItemsSource = groupsQuery;
        }
        #endregion
        #region validate - clearValidate - textChange - lostFocus - . . . . 
        void Clear()
        {
            this.DataContext = new Group();



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


        private async Task<bool> chkDuplicateGroup()
        {
            bool b = false;


            List<Group> groups = await group.GetAll();
            Group group1 = new Group();

            for (int i = 0; i < groups.Count(); i++)
            {
                group1 = groups[i];
                if ((group1.name.Equals(tb_name.Text.Trim())) &&
                    (group1.groupId != group.groupId))
                { b = true; break; }
            }

            return b;
        }
        #region tabs
        private void btn_tabs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;
                paint(button.Tag.ToString());
                //isEnabledButtons(button.Tag.ToString());
                //grid_home.Visibility = Visibility.Visible;
                //btn_home.Opacity = 1;
                List<Object> list =   Object.findChildrenList(button.Tag.ToString(), FillCombo.objectsList);
                string s = "";
                foreach (var item in list)
                {
                    s += item.name+" \n";
                }
                MessageBox.Show(s);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        //private void isEnabledButtons(string tag)
        //{
        //    // btn
        //    List<Button> tabsButtonsList = FindControls.FindVisualChildren<Button>(this)
        //        .Where(x =>  x.Tag != null).ToList();
        //    foreach (var item in tabsButtonsList)
        //    {
        //        if (item.Tag.ToString() == tag)
        //        {
        //            item.IsEnabled = false;
        //            item.Opacity = 1;
        //        }
        //        else
        //            item.IsEnabled = true;
        //    }
        //    //btn_home.IsEnabled = true;
        //    //btn_catalog.IsEnabled = true;
        //    //btn_store.IsEnabled = true;
        //    //btn_sale.IsEnabled = true;
        //    //btn_purchase.IsEnabled = true;
        //    //btn_sectionData.IsEnabled = true;
        //    //btn_reports.IsEnabled = true;
        //    //btn_settings.IsEnabled = true;
        //    //btn_alerts.IsEnabled = true;
        //    //btn_account.IsEnabled = true;
        //}
        public void paint(string tag)
        {
            bdrMain.RenderTransform = Animations.borderAnimation(50, bdrMain, true);

            // bdr
            List<Border> tabsBordersList = FindControls.FindVisualChildren<Border>(this)
                .Where(x => x.Tag != null).ToList();
            foreach (var item in tabsBordersList)
            {
                if (item.Tag.ToString() == tag)
                    item.Background = Application.Current.Resources["White"] as SolidColorBrush;
                else
                    item.Background = Application.Current.Resources["SecondColor"] as SolidColorBrush; 
            }
            // path
            List<Path> tabsPathsList = FindControls.FindVisualChildren<Path>(this)
                .Where(x => x.Tag != null).ToList();
            foreach (var item in tabsPathsList)
            {
                if (item.Tag.ToString() == tag)
                    item.Fill = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                else
                    item.Fill = Application.Current.Resources["White"] as SolidColorBrush;
            }
           

            /*
            grid_home.Visibility = Visibility.Hidden;
            grid_catalog.Visibility = Visibility.Hidden;
            grid_store.Visibility = Visibility.Hidden;
            grid_purchase.Visibility = Visibility.Hidden;
            grid_kitchin.Visibility = Visibility.Hidden;
            grid_sales.Visibility = Visibility.Hidden;
            grid_charts.Visibility = Visibility.Hidden;
            grid_data.Visibility = Visibility.Hidden;
            grid_settings.Visibility = Visibility.Hidden;
            grid_alerts.Visibility = Visibility.Hidden;
            grid_account.Visibility = Visibility.Hidden;
            */
        }
        #endregion
        #region   secondLevel
        /*
        void BuildObjectsDesign(List<Object> objectsChildren)
        {
            grid_secondLevel.Children.Clear();

            //   < Border  BorderBrush = "#E8E8E8" Grid.RowSpan = "3"
            //    BorderThickness = "1" Margin = "0,10" >

            //      < Border Grid.Column = "3" BorderBrush = "#E8E8E8" Grid.RowSpan = "3"
            //    BorderThickness = "1" Margin = "0,10" >

            //      < Border Grid.Column = "5" BorderBrush = "#E8E8E8" Grid.RowSpan = "3"
            //    BorderThickness = "1" Margin = "0,10" >
            Border border;
            for (int i = 0; i < 3; i++)
            {
                border = new Border();
                Grid.SetColumn(border, (i*2)+1);
                border.BorderBrush = Application.Current.Resources["veryLightGrey"] as SolidColorBrush;


                itemNameText.Text = item.name;
                itemNameText.VerticalAlignment = VerticalAlignment.Center;
                itemNameText.HorizontalAlignment = HorizontalAlignment.Center;
                gridContainer.Children.Add(itemNameText);

                tableButton.Tag = item.name;
                tableButton.Margin = new Thickness(5);
                tableButton.Padding = new Thickness(0);
                tableButton.Background = null;
                tableButton.BorderBrush = null;
            }
           

            foreach (var item in objectsChildren)
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
        */
        private void btn_secondLevelClick(object sender, RoutedEventArgs e)
        {
            try
            {
                
                HelpClass.StartAwait(grid_main);
                Button button = sender as Button;
                paintSecondLevel();
                foreach (Path path in FindControls.FindVisualChildren<Path>(this))
                {
                    // do something with tb here
                    if (path.Name == "path_" + button.Tag)
                    {
                        path.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#178DD2"));
                        break;
                    }
                }
                foreach (TextBlock textBlock in FindControls.FindVisualChildren<TextBlock>(this))
                {
                    if (textBlock.Name == "txt_" + button.Tag)
                    {
                        textBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#178DD2"));
                        break;
                    }
                }

                _parentObjectName = button.Tag.ToString();
                Tb_search_TextChanged(null, null);

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        public void paintSecondLevel()
        {
            paintHome();
            /*
            paintCatalog();
            paintStore();
            paintPurchase();
            paintSale();
            paintAccounts();
            paintSectionData();
            paintSettings();
            paintAlerts();
            paintReports();
            */
        }

        public void paintHome()
        {
            /*
            path_dashboard.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#67686d"));
            txt_dashboard.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#67686d"));
            */
        }
        #endregion
        #region groupObjects
        /*
        IEnumerable<GroupObject> groupObjectsQuery;
        IEnumerable<GroupObject> groupObjects;
        void RefreshGroupObjectsView()
        {
            dg_permissions.ItemsSource = groupObjectsQuery;
        }
        private async void Tb_search_TextChanged(object sender, TextChangedEventArgs e)
        {//search
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_main);
                if (groupObjects is null)
                    await RefreshGroupObjectList();
                searchText = tb_searchGroup.Text;
                groupObjectsQuery = groupObjects.Where(s => s.groupId == group.groupId
                && s.objectType != "basic" && s.parentObjectName == _parentObjectName);
                RefreshGroupObjectsView();
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
        private async void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    SectionData.StartAwait(grid_main);
                await RefreshGroupObjectList();
                Tb_search_TextChanged(null, null);
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
        async Task<IEnumerable<GroupObject>> RefreshGroupObjectList()
        {
            groupObjects = await groupObject.GetAll();
            return groupObjects;
        }
        */

        #endregion
        private void Btn_usersList_Click(object sender, RoutedEventArgs e)
        {
            /*
            try
            {
                
                    HelpClass.StartAwait(grid_main);
                if (MainWindow.groupObject.HasPermissionAction(usersPermission, MainWindow.groupObjects, "one") || HelpClass.isAdminPermision())
                {
                    if (group.groupId > 0)
                    {
                        Window.GetWindow(this).Opacity = 0.2;
                        wd_usersList w = new wd_usersList();
                        w.groupId = group.groupId;

                        w.ShowDialog();

                        Window.GetWindow(this).Opacity = 1;
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

        private async void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            /*
            try
            {
                
                    HelpClass.StartAwait(grid_main);
                if (MainWindow.groupObject.HasPermissionAction(basicsPermission, MainWindow.groupObjects, "update") || HelpClass.isAdminPermision())
                {
                    int s = 0;
                    foreach (var item in groupObjectsQuery)
                    {
                        s = await groupObject.Save(item);
                    }
                    if (!s.Equals(0))
                    {
                        //addObjects(int.Parse(s));
                        Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopUpdate"), animation: ToasterAnimation.FadeIn);
                        Btn_clear_Click(null, null);
                    }
                    else
                        Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);

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
    }
}
