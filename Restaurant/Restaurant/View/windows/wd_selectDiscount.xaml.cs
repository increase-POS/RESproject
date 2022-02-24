using Restaurant.Classes;
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
using System.Windows.Shapes;

namespace Restaurant.View.windows
{
    /// <summary>
    /// Interaction logic for wd_selectDiscount.xaml
    /// </summary>
    public partial class wd_selectDiscount : Window
    {
        public wd_selectDiscount()
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
        public string userJob;

        public bool isOk { get; set; }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {//load
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
        private void Btn_select_Click(object sender, RoutedEventArgs e)
        {


            // if have id return true
            isOk = true;
            //this.Close();
            // else return false
            //isOk = false;
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
      


        private void Cb_coupon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //try
            //{
            //        SectionData.StartAwait(grid_main);
            //    string s = _BarcodeStr;
            //    if (cb_coupon.SelectedIndex != -1)
            //    {
            //        couponModel = coupons.ToList().Find(c => c.cId == (int)cb_coupon.SelectedValue);
            //        if (couponModel != null)
            //        {
            //            s = couponModel.barcode;
            //            await dealWithBarcode(s);
            //        }
            //        cb_coupon.SelectedIndex = -1;
            //        cb_coupon.SelectedItem = "";
            //    }
            //        SectionData.EndAwait(grid_main);
            //}
            //catch (Exception ex)
            //{
            //        SectionData.EndAwait(grid_main);
            //    SectionData.ExceptionMessage(ex, this);
            //}
        }

        private void Btn_clearCoupon_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    _Discount = 0;
            //    selectedCoupons.Clear();
            //    lst_coupons.Items.Clear();
            //    cb_coupon.SelectedIndex = -1;
            //    cb_coupon.SelectedItem = "";
            //    refreshTotalValue();
            //}
            //catch (Exception ex)
            //{
            //    SectionData.ExceptionMessage(ex, this);
            //}
        }
    }
}
