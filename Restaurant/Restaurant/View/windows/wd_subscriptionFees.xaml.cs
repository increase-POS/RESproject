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
    /// Interaction logic for wd_subscriptionFees.xaml
    /// </summary>
    public partial class wd_subscriptionFees : Window
    {
        public wd_subscriptionFees()
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
        private void Btn_colse_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                this.Close();
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
                    //Btn_confirmation_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Btn_add_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Btn_update_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Dg_subscriptionFees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                #region translate
                if (AppSettings.lang.Equals("en"))
                {
                    grid_subscriptionFees.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    grid_subscriptionFees.FlowDirection = FlowDirection.RightToLeft;
                }

                translat();
                #endregion
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void translat()
        {
            txt_Title.Text = AppSettings.resourcemanager.GetString("trSubscriptionFees");

            dg_subscriptionFees.Columns[0].Header = AppSettings.resourcemanager.GetString("trMonthCount");
            dg_subscriptionFees.Columns[1].Header = AppSettings.resourcemanager.GetString("trAmount");

            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_monthsCount, AppSettings.resourcemanager.GetString("trMonthCountHint"));//
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_amount, AppSettings.resourcemanager.GetString("trAmountHint"));//

            tt_add_Button.Content = AppSettings.resourcemanager.GetString("trAdd");
            tt_update_Button.Content = AppSettings.resourcemanager.GetString("trUpdate");
            tt_delete_Button.Content = AppSettings.resourcemanager.GetString("trDelete");

        }

      
    }
}
