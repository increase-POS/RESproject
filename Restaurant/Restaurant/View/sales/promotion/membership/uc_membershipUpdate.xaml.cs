﻿using netoaster;
using Restaurant.Classes;
using Restaurant.View.windows;
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

namespace Restaurant.View.sales.promotion.membership
{
    /// <summary>
    /// Interaction logic for uc_membershipUpdate.xaml
    /// </summary>
    public partial class uc_membershipUpdate : UserControl
    {
        public uc_membershipUpdate()
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
        private static uc_membershipUpdate _instance;
        public static uc_membershipUpdate Instance
        {
            get
            {
                //if (_instance == null)
                if (_instance is null)
                    _instance = new uc_membershipUpdate();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        string basicsPermission = "membershipUpdate_basics";
        Memberships membership = new Memberships();
        IEnumerable<Memberships> membershipsQuery;
        IEnumerable<Memberships> memberships;

        AgentMemberships agMembership = new AgentMemberships();
        IEnumerable<AgentMemberships> agMemberships;

        CouponsMemberships coMembership = new CouponsMemberships();
        IEnumerable<CouponsMemberships> coMemberships;

        MembershipsOffers ofMembership = new MembershipsOffers();
        IEnumerable<MembershipsOffers> ofMemberships;

        InvoicesClassMemberships inMembership = new InvoicesClassMemberships();
        IEnumerable<InvoicesClassMemberships> inMemberships;

        string searchText = "";
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

                #region translate
                if (AppSettings.lang.Equals("en"))
                {
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                }
                translate();
                #endregion

                await RefreshMembershipsList();
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

        #region Refresh & Search
        private string getSubscriptionType(string s)
        {
            string value = "";

            if (s.Equals("f")) value = AppSettings.resourcemanager.GetString("trFree");
            else if (s.Equals("m")) value = AppSettings.resourcemanager.GetString("trMonthly");
            else if (s.Equals("y")) value = AppSettings.resourcemanager.GetString("trYearly");
            else if (s.Equals("o")) value = AppSettings.resourcemanager.GetString("trOnce");

            return value;
        }
        async Task Search()
        {
            if (memberships is null)
                await RefreshMembershipsList();

            searchText = tb_search.Text.ToLower();
            membershipsQuery = memberships
                .Where(s => (
            s.code.ToLower().Contains(searchText) ||
            s.name.ToLower().Contains(searchText)
            )
            )
            ;

            RefreshMembershipsView();
        }
        async Task<IEnumerable<Memberships>> RefreshMembershipsList()
        {
            memberships = await membership.GetAll();
            memberships = memberships.Where(m => m.isActive == 1);
            return memberships;
        }
        void RefreshMembershipsView()
        {
            dg_membership.ItemsSource = membershipsQuery;
        }
        #endregion

        void translate()
        {
            txt_title.Text = AppSettings.resourcemanager.GetString("trMembership");
            txt_details.Text = AppSettings.resourcemanager.GetString("trDetails");
            txt_code.Text = AppSettings.resourcemanager.GetString("trCode");
            txt_name.Text = AppSettings.resourcemanager.GetString("trName");
            txt_customersCount.Text = AppSettings.resourcemanager.GetString("trCustomers");
            txt_couponsCount.Text = AppSettings.resourcemanager.GetString("trCoupons");
            txt_offersCount.Text = AppSettings.resourcemanager.GetString("trOffers");
            txt_invoicesClassesCount.Text = AppSettings.resourcemanager.GetString("trInvoicesClasses");//???
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_search, AppSettings.resourcemanager.GetString("trSearchHint"));

            dg_membership.Columns[0].Header = AppSettings.resourcemanager.GetString("trCode");
            dg_membership.Columns[1].Header = AppSettings.resourcemanager.GetString("trName");

            btn_clear.ToolTip = AppSettings.resourcemanager.GetString("trClear");

            tt_clear.Content = AppSettings.resourcemanager.GetString("trClear");
        }
        #region events
        private async void Tb_search_TextChanged(object sender, TextChangedEventArgs e)
        {//search
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
        private async void Dg_membership_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//selection
            try
            {
                HelpClass.StartAwait(grid_main);
                
                if (dg_membership.SelectedIndex != -1)
                {
                    membership = dg_membership.SelectedItem as Memberships;
                    this.DataContext = membership;
                    if (membership != null)
                    {
                        //customers
                        agMemberships = await agMembership.GetAll();
                        agMemberships = agMemberships.Where(a => a.membershipId == membership.membershipId);
                        tb_customersCount.Text = agMemberships.Count().ToString();
                        //coupons
                        coMemberships = await coMembership.GetAll();
                        coMemberships = coMemberships.Where(c => c.membershipId == membership.membershipId);
                        tb_couponsCount.Text = coMemberships.Count().ToString();
                        //offers
                        //ofMemberships = await ofMembership.GetAll();
                        //ofMemberships = ofMemberships.Where(o => o.membershipId == membership.membershipId);
                        //tb_offersCount.Text = ofMemberships.Count().ToString();
                        //invoices
                        //inMemberships = await inMembership.GetAll();
                        //inMemberships = inMemberships.Where(i => i.membershipId == membership.membershipId);
                        //tb_invoicesClassesCount.Text = inMemberships.Count().ToString();
                    }
                }
                //HelpClass.clearValidate(requiredControlList, this);
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

                tb_search.Text = "";
                searchText = "";
                await RefreshMembershipsList();
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
       
        #region validate - clearValidate - textChange - lostFocus - . . . . 
        void Clear()
        {
            this.DataContext = new Agent();
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

        #endregion

        #region button in datagrid
       

        async void customersRowinDatagrid(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);
                for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                    if (vis is DataGridRow)
                    {
                        Memberships row = (Memberships)dg_membership.SelectedItems[0];

                        try
                        {
                            HelpClass.StartAwait(grid_main);
                            //if (FillCombo.groupObject.HasPermissionAction("membershipCreate_subscriptionFees", FillCombo.groupObjects, "one"))
                            //{
                                Window.GetWindow(this).Opacity = 0.2;

                                wd_agentMembership w = new wd_agentMembership();

                                w.membershipID = membership.membershipId;
                                w.ShowDialog();
                                //if (w.isActive)
                                //{
                                //}

                                Window.GetWindow(this).Opacity = 1;
                            //}
                            //else
                            //    Toaster.ShowInfo(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                            HelpClass.EndAwait(grid_main);
                        }
                        catch (Exception ex)
                        {
                            HelpClass.EndAwait(grid_main);
                            HelpClass.ExceptionMessage(ex, this);
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
        async void couponsRowinDatagrid(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);
                for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                    if (vis is DataGridRow)
                    {
                        Memberships row = (Memberships)dg_membership.SelectedItems[0];



                    }

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        async void offersRowinDatagrid(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);
                for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                    if (vis is DataGridRow)
                    {
                        Memberships row = (Memberships)dg_membership.SelectedItems[0];



                    }

                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        async void invoicesClassesRowinDatagrid(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);
                for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                    if (vis is DataGridRow)
                    {
                        Memberships row = (Memberships)dg_membership.SelectedItems[0];



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
    }
}
