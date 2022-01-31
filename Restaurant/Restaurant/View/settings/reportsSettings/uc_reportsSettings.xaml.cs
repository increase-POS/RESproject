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

namespace Restaurant.View.settings.reportsSettings
{
    /// <summary>
    /// Interaction logic for uc_reportsSettings.xaml
    /// </summary>
    public partial class uc_reportsSettings : UserControl
    {
        private static uc_reportsSettings _instance;
        public static uc_reportsSettings Instance
        {
            get
            {
                _instance = new uc_reportsSettings();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_reportsSettings()
        {
            try
            {
                InitializeComponent();
            }
            catch
            { }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }

        #region validate - clearValidate - textChange - lostFocus - . . . . 

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


        #endregion

        private void Btn_reportlang_Click(object sender, RoutedEventArgs e)
        {
            /*
            try
            {
                //  string msg = "";
                int msg = 0;
                if (cb_reportlang.SelectedIndex != -1)
                {
                    replangrow = replangList.Where(r => r.valId == (int)cb_reportlang.SelectedValue).FirstOrDefault();
                    replangrow.isDefault = 1;
                    msg = await setvalueModel.Save(replangrow);
                    //  replangrow.valId=
                    await MainWindow.GetReportlang();
                    await fillRepLang();
                    if (msg > 0)
                    {
                        Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopSave"), animation: ToasterAnimation.FadeIn);
                    }
                    else
                    {
                        Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                    }
                }
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
            */
        }

        private void Btn_systmSetting_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                
                    HelpClass.StartAwait(grid_main);
                //if (MainWindow.groupObject.HasPermissionAction(companySettingsPermission, MainWindow.groupObjects, "one") )
                //{
                Window.GetWindow(this).Opacity = 0.2;
                wd_reportSystmSetting w = new wd_reportSystmSetting();
                w.ShowDialog();
                Window.GetWindow(this).Opacity = 1;
                //}
                //else
                //    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
            
        }

        private void Btn_printerSetting_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                
                    HelpClass.StartAwait(grid_main);


                //if (MainWindow.groupObject.HasPermissionAction(companySettingsPermission, MainWindow.groupObjects, "one") )
                //{
                Window.GetWindow(this).Opacity = 0.2;
                wd_reportPrinterSetting w = new wd_reportPrinterSetting();
                w.ShowDialog();
                Window.GetWindow(this).Opacity = 1;
                //}
                //else
                //    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
            
        }

        private void Btn_copyCount_Click(object sender, RoutedEventArgs e)
        {
            
                        try
                        {

                HelpClass.StartAwait(grid_main);
                            //if (MainWindow.groupObject.HasPermissionAction(companySettingsPermission, MainWindow.groupObjects, "one") )
                            //{
                            Window.GetWindow(this).Opacity = 0.2;
                            wd_reportCopyCountSetting w = new wd_reportCopyCountSetting();
                            w.ShowDialog();
                            Window.GetWindow(this).Opacity = 1;
                            //}
                            //else
                            //    Toaster.ShowInfo(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trdontHavePermission"), animation: ToasterAnimation.FadeIn);
                            
                                HelpClass.EndAwait(grid_main);
                        }
                        catch (Exception ex)
                        {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
                        }
        }

        private void Btn_printCount_Click(object sender, RoutedEventArgs e)
        {
            /*
            //save print count
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                HelpClass.validateEmptyTextBox(tb_printCount, p_errorPrintCount, tt_errorPrintCount, "trEmptyPrintCount");
                if (!tb_printCount.Text.Equals(""))
                {

                    if (printCount == null)
                        printCount = new SetValues();
                    printCount.value = tb_printCount.Text;
                    printCount.isSystem = 1;
                    printCount.isDefault = 1;
                    printCount.settingId = printCountId;

                    int s = await valueModel.Save(printCount);
                    if (!s.Equals(0))
                    {
                        //update tax in main window
                        MainWindow.Allow_print_inv_count = printCount.value;

                        Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopSave"), animation: ToasterAnimation.FadeIn);
                    }
                    else
                        Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
                }

                
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
            */
        }
        #region Numeric 

        private int _numValue_printCount = 0;
        public int NumValue_printCount
        {
            get {
                if (int.TryParse(tb_printCount.Text, out _numValue_printCount))
                    _numValue_printCount = int.Parse(tb_printCount.Text);
                return _numValue_printCount;
            }
            set
            {
                _numValue_printCount = value;
                tb_printCount.Text = value.ToString();
            }
        }

        private void Btn_cmdUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NumValue_printCount++;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Btn_cmdDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NumValue_printCount > 0)
                    NumValue_printCount--;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        #endregion
      
    }
}
