﻿using netoaster;
using Restaurant.Classes;
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
using System.Windows.Shapes;

namespace Restaurant.View.windows
{
    /// <summary>
    /// Interaction logic for wd_reportCopyCountSetting.xaml
    /// </summary>
    public partial class wd_reportCopyCountSetting : Window
    {

        public wd_reportCopyCountSetting()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            { HelpClass.ExceptionMessage(ex, this); }
        }
        BrushConverter bc = new BrushConverter();

        //print

        string sale_copy_count;
        string pur_copy_count;
        string rep_copy_count;
        SetValues setvalueModel = new SetValues();
        List<SetValues> printList = new List<SetValues>();

        public SetValues sale_copy_countrow = new SetValues();
        public SetValues pur_copy_countrow = new SetValues();
        public SetValues rep_copy_countrow = new SetValues();

        async Task refreshWindow()
        {
            printList = await setvalueModel.GetBySetvalNote("print");

            sale_copy_countrow = printList.Where(X => X.name == "sale_copy_count").FirstOrDefault();
            sale_copy_count = sale_copy_countrow.value;
            pur_copy_countrow = printList.Where(X => X.name == "pur_copy_count").FirstOrDefault();
            pur_copy_count = pur_copy_countrow.value;
            rep_copy_countrow = printList.Where(X => X.name == "rep_copy_count").FirstOrDefault();
            rep_copy_count = rep_copy_countrow.value;

            tb_purCopyCount.Text = pur_copy_count;

            tb_saleCopyCount.Text = sale_copy_count;
            tb_repPrintCount.Text = rep_copy_count;
        }

        private void Btn_colse_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {//load
            /*
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);

                #region translate

                if (winLogIn.lang.Equals("en"))
                {
                    winLogIn.resourcemanager = new ResourceManager("Restaurant.en_file", Assembly.GetExecutingAssembly());
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    winLogIn.resourcemanager = new ResourceManager("Restaurant.ar_file", Assembly.GetExecutingAssembly());
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                }

                translate();
                #endregion

                //
                await refreshWindow();


                if (sender != null)
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
            */
        }


        private void translate()
        {
            /*
            txt_title.Text = winLogIn.resourcemanager.GetString("trCopyCount");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_purCopyCount, MainWindow.resourcemanager.GetString("trPurchasesCopyCount"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_saleCopyCount, MainWindow.resourcemanager.GetString("trSalesCopyCount"));
            MaterialDesignThemes.Wpf.HintAssist.SetHint(tb_repPrintCount, MainWindow.resourcemanager.GetString("trReportsCopyCount"));

            btn_save.Content = winLogIn.resourcemanager.GetString("trSave");
            */
        }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return)
                {
                    //Btn_save_Click(null, null);
                }
            }
            catch (Exception ex)
            { HelpClass.ExceptionMessage(ex, this); }
        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                this.Visibility = Visibility.Hidden;
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
            catch (Exception ex)
            {
                //HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            /*

            int msg;
            sale_copy_countrow.value = (string)tb_saleCopyCount.Text;
            pur_copy_countrow.value = (string)tb_purCopyCount.Text;
            rep_copy_countrow.value = (string)tb_repPrintCount.Text;
            if (int.Parse(sale_copy_countrow.value) <=0 || int.Parse(pur_copy_countrow.value) <= 0|| int.Parse(rep_copy_countrow.value)<=0)
            {
                Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trMustBeMoreThanZero"), animation: ToasterAnimation.FadeIn);
            }
            else
            {
            
                msg = await setvalueModel.Save(sale_copy_countrow);
                msg = await setvalueModel.Save(pur_copy_countrow);
                msg = await setvalueModel.Save(rep_copy_countrow);

                await refreshWindow();
                await MainWindow.Getprintparameter();
                if (msg > 0)
                {
                    Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopSave"), animation: ToasterAnimation.FadeIn);
                    await Task.Delay(1500);
                    this.Close();
                }
                else
                {
                    Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                }
            }
          */

        }

        private void Tb_validateEmptyLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = sender.GetType().Name;
                validateEmpty(name, sender);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void validateEmpty(string name, object sender)
        {
            /*
            try
            {
                if (name == "TextBox")
                {
                    if ((sender as TextBox).Name == "tb_purCopyCount")
                        HelpClass.validateEmptyTextBox((TextBox)sender, p_errorPurCopyCount, tt_errorPurCopyCount, "trEmptyError");
                    else if ((sender as TextBox).Name == "tb_saleCopyCount")
                        HelpClass.validateEmptyTextBox((TextBox)sender, p_errorSaleCopyCount, tt_errorSaleCopyCount, "trEmptyError");
                    else if ((sender as TextBox).Name == "tb_repPrintCount")
                        HelpClass.validateEmptyTextBox((TextBox)sender, p_errorRepPrintCount, tt_errorRepPrintCount, "trEmptyError");
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
            */
        }


        private void Tb_count_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox textBox = sender as TextBox;
                if (textBox == null)
                {
                    return;
                }



                if (textBox.Name == "tb_purCopyCount" || textBox.Name == "tb_saleCopyCount" || textBox.Name == "tb_repPrintCount")
                    HelpClass.InputJustNumber(ref textBox);

                //if (textBox.Name == "tb_purCopyCount")
                //{
                //    if (int.TryParse(textBox.Text, out _numPurCopyCount))
                //        numPurCopyCount = int.Parse(textBox.Text);
                //}
                //else if (textBox.Name == "tb_saleCopyCount")
                //{
                //    if (int.TryParse(textBox.Text, out _numSaleCopyCount))
                //        numSaleCopyCount = int.Parse(textBox.Text);
                //}
                //else if (textBox.Name == "tb_repPrintCount")
                //{
                //    if (int.TryParse(textBox.Text, out _numRepPrintCount))
                //        numRepPrintCount = int.Parse(textBox.Text);
                //}

            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Tb_count_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                Regex regex = new Regex("[^0-9]+");
                e.Handled = regex.IsMatch(e.Text);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Tb_PreventSpaces(object sender, KeyEventArgs e)
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
        #region NumericCount

      

        private int _numValue_purCopyCount = 1;
        public int numValue_purCopyCount
        {
            get {
                if (int.TryParse(tb_purCopyCount.Text, out _numValue_purCopyCount))
                    _numValue_purCopyCount = int.Parse(tb_purCopyCount.Text);
                return _numValue_purCopyCount; }
            set
            {
                _numValue_purCopyCount = value;
                tb_purCopyCount.Text = value.ToString();
            }
        }


        private int _numValue_saleCopyCount = 1;
        public int numValue_saleCopyCount
        {
            get {
                if (int.TryParse(tb_saleCopyCount.Text, out _numValue_saleCopyCount))
                    _numValue_saleCopyCount = int.Parse(tb_saleCopyCount.Text);
                return _numValue_saleCopyCount; }
            set
            {
                _numValue_saleCopyCount = value;
                tb_saleCopyCount.Text = value.ToString();
            }
        }


        private int _numValue_repPrintCount = 1;
        public int numValue_repPrintCount
        {
            get {
                if (int.TryParse(tb_repPrintCount.Text, out _numValue_repPrintCount))
                    _numValue_repPrintCount = int.Parse(tb_repPrintCount.Text);
                return _numValue_repPrintCount; }
            set
            {
                _numValue_repPrintCount = value;
                tb_repPrintCount.Text = value.ToString();
            }
        }
        private void Btn_countDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;

                if (button.Tag.ToString() == "purCopyCount" && numValue_purCopyCount > 0)
                    numValue_purCopyCount--;
                else if (button.Tag.ToString() == "saleCopyCount" && numValue_saleCopyCount > 0)
                    numValue_saleCopyCount--;
                else if (button.Tag.ToString() == "repPrintCount" && numValue_repPrintCount > 0)
                    numValue_repPrintCount--;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Btn_countUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;
                if (button.Tag.ToString() == "purCopyCount")
                    numValue_purCopyCount++;
                else if (button.Tag.ToString() == "saleCopyCount")
                    numValue_saleCopyCount++;
                else if (button.Tag.ToString() == "repPrintCount")
                    numValue_repPrintCount++;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion
    }
}