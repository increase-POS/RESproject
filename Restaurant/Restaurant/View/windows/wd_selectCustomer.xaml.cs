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
        AgentMemberships agentMemberships = new AgentMemberships();
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
            catch
            { }
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
        public int customerId;
        public bool isOk { get; set; }
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
                await FillCombo.FillComboCustomers(cb_customerId);

                fillInputs();
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
            //MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_coupon, AppSettings.resourcemanager.GetString("trCouponHint"));

            btn_select.Content = AppSettings.resourcemanager.GetString("trSelect");
        }

        private void fillInputs()
        {
            if (customerId != 0)
                cb_customerId.SelectedValue = customerId;
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
                agentMemberships = await agentMemberships.GetAgentMemberShip(customerId);
            }
        }
    }
}
