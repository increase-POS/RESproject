﻿using netoaster;
using Restaurant.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
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
    /// Interaction logic for wd_selectCustomer.xaml
    /// </summary>
    public partial class wd_selectCustomer : Window
    {
        Memberships memberships = new Memberships();
        AgenttoPayCash agentToPayCash = new AgenttoPayCash();
        public wd_selectCustomer() 
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
            isOk = false;
            this.Close();
        }
        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return)
                {
                    Btn_select_Click(btn_select, null);
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        public bool isOk { get; set; }
        public int customerId { get; set; }
        public decimal deliveryDiscount { get; set; }
        public bool hasOffers { get; set; }

        public string memberShipStatus;

        public static List<string> requiredControlList = new List<string>();
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {//load
            try
            {
                HelpClass.StartAwait(grid_main);
                requiredControlList = new List<string> { "customerId" };
                if (AppSettings.lang.Equals("en"))
                {
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                }
                translate();
                await FillCombo.RefreshCustomers();
                await FillCombo.FillComboCustomers(cb_customerId);

                fillInputs();

                if (AppSettings.invType != "")
                    grid_delivery.Visibility = Visibility.Visible;
                else
                    grid_delivery.Visibility = Visibility.Collapsed;

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
            txt_title.Text = AppSettings.resourcemanager.GetString("trCustomers");
            txt_membership.Text = AppSettings.resourcemanager.GetString("trMembership");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_customerId, AppSettings.resourcemanager.GetString("trCustomerHint"));

            txt_code.Text = AppSettings.resourcemanager.GetString("trCode");
            txt_name.Text = AppSettings.resourcemanager.GetString("trName");
            txt_couponsCount.Text = AppSettings.resourcemanager.GetString("trCoupons");
            txt_offersCount.Text = AppSettings.resourcemanager.GetString("trOffers");
            txt_invoicesClassesCount.Text = AppSettings.resourcemanager.GetString("trInvoicesClasses");
            txt_deliveryDetails.Text = AppSettings.resourcemanager.GetString("trDelivery");
           

            btn_select.Content = AppSettings.resourcemanager.GetString("trSelect");

        }

        private void fillInputs()
        {
            if (customerId != 0)
            {
                cb_customerId.SelectedValue = customerId;               
            }
        }
        private async Task fillMemberShipInfo()
        {
            var customer = FillCombo.customersList.Where(x => x.agentId == customerId).FirstOrDefault();
            if (customer.membershipId != null)
            {
                agentToPayCash = await memberships.GetmembershipStateByAgentId(customerId);
                if(agentToPayCash == null)
                {
                    sp_membership.Visibility = Visibility.Collapsed;
                    grid_membershipInctive.Visibility = Visibility.Collapsed;
                }
                else
                {
                    sp_membership.Visibility = Visibility.Visible;

                    //txt_membershipInctive
                    //    membershipStatus
                    #region opacity
                    if (agentToPayCash.membershipStatus == "valid")
                    {
                        sp_membership.Opacity = 1;

                        #region warning expir in 7 days
                        if (agentToPayCash.subscriptionType == "m" || agentToPayCash.subscriptionType == "y")
                        {
                            TimeSpan expireDate = agentToPayCash.endDate.Value - DateTime.Now;
                            //double days = Math.Round(expireDate.TotalDays, MidpointRounding.AwayFromZero);
                            //double days = (expireDate.TotalDays == 0)? (int)expireDate.TotalDays : (int) expireDate.TotalDays +1;
                            double days = (int)expireDate.TotalDays;
                            if (days < 7)
                            {
                                grid_membershipInctive.Visibility = Visibility.Visible;
                                TimeSpan expire = agentToPayCash.endDate.Value - DateTime.Now;
                                double expireDays = Math.Round(expire.TotalDays, MidpointRounding.AwayFromZero);
                                //double expireDays = (expire.TotalDays == 0) ? (int)expire.TotalDays : (int)expire.TotalDays + 1;

                                txt_membershipInctive.Text = AppSettings.resourcemanager.GetString("subscriptionWillBeExpiredIn") + " "
                                    + ((expireDays == 0) ? AppSettings.resourcemanager.GetString("thisDay")
                                    : (expireDays + 1).ToString() + " " + AppSettings.resourcemanager.GetString("trDay"));
                            }
                            else
                            {
                                grid_membershipInctive.Visibility = Visibility.Collapsed;
                                txt_membershipInctive.Text = "";
                            }
                        }
                        else
                        {
                            grid_membershipInctive.Visibility = Visibility.Collapsed;
                            txt_membershipInctive.Text = "";
                        }
                        #endregion
                        memberShipStatus = "valid";
                    }
                    else
                    {
                        sp_membership.Opacity = 0.4;
                        grid_membershipInctive.Visibility = Visibility.Visible;

                        if (agentToPayCash.membershipStatus == "notpayed")
                            memberShipStatus = "notPayed";
                        else if (agentToPayCash.membershipStatus == "expired")
                            memberShipStatus = "subscriptionHasExpired";
                        else
                            memberShipStatus = "notActive";

                    }
                    #endregion
                    #region data context
                    this.DataContext = agentToPayCash;
                    if (agentToPayCash.offersCount > 0)
                        hasOffers = true;
                    #region delivery
                    if (agentToPayCash.isFreeDelivery)
                    {
                        tb_deliveryDetails.Text = AppSettings.resourcemanager.GetString("trFree");
                        deliveryDiscount = 100;
                    }
                    else
                    {
                        tb_deliveryDetails.Text = agentToPayCash.deliveryDiscountPercent + " %";
                        deliveryDiscount = agentToPayCash.deliveryDiscountPercent;
                    }
             
                    #endregion
                    #endregion
                }
            }
            else
            {
                agentToPayCash = new AgenttoPayCash();
                this.DataContext = agentToPayCash;
                tb_deliveryDetails.Text = agentToPayCash.deliveryDiscountPercent + " %";
            }
        }
        private void Btn_select_Click(object sender, RoutedEventArgs e)
        {

            if (HelpClass.validate(requiredControlList, this))
            {
                // if have id return true
                isOk = true;
                customerId =(int) cb_customerId.SelectedValue;
               
                this.Close();
            }
        }

        private async void Cb_customerId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cb_customerId.SelectedIndex != -1)
            {
                customerId = (int)cb_customerId.SelectedValue;
                await fillMemberShipInfo();
            }
        }
        private async void Btn_addCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                HelpClass.StartAwait(grid_main);
                Window.GetWindow(this).Opacity = 0.2;
                wd_updateVendor w = new wd_updateVendor();
                //// pass agent id to update windows
                w.agent.agentId = 0;
                w.type = "c";
                w.ShowDialog();
                Window.GetWindow(this).Opacity = 1;
                if (w.isOk == true)
                {
                    Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopSave"), animation: ToasterAnimation.FadeIn);
                    await FillCombo.RefreshCustomers();
                    await FillCombo.FillComboCustomers(cb_customerId);
                }

                HelpClass.EndAwait(grid_main);

            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Btn_updateCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cb_customerId.SelectedIndex != -1)
                {
                    HelpClass.StartAwait(grid_main);
                    Window.GetWindow(this).Opacity = 0.2;
                    wd_updateVendor w = new wd_updateVendor();
                    //// pass agent id to update windows
                    w.agent.agentId = (int)cb_customerId.SelectedValue;
                    w.type = "c";
                    w.ShowDialog();
                    Window.GetWindow(this).Opacity = 1;
                    if (w.isOk == true)
                    {
                        Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopSave"), animation: ToasterAnimation.FadeIn);
                        await FillCombo.RefreshCustomers();
                        //await FillCombo.FillComboCustomers(cb_customerId);
                    }

                    HelpClass.EndAwait(grid_main);

                }
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }

        }

    }
}
