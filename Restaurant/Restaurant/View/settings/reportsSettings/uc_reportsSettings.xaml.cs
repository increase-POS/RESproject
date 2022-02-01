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
using System.Resources;
using System.Reflection;
using netoaster;


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

        public class Replang
        {
            public int langId { get; set; }
            public string lang { get; set; }
            public string trlang { get; set; }
            public Nullable<int> isDefault { get; set; }

        }

        SetValues setvalueModel = new SetValues();
        List<SetValues> replangList = new List<SetValues>();
        SetValues replangrow = new SetValues();
        static SettingCls setModel = new SettingCls();
        static SettingCls set = new SettingCls();
        static SetValues valueModel = new SetValues();
        static SetValues printCount = new SetValues();
        static int printCountId = 0;
        List<Replang> langcomboList = new List<Replang>();


        async Task fillRepLang()
        {
            langcomboList = new List<Replang>();
            replangList = await setvalueModel.GetBySetName("report_lang");
            foreach (var reprow in replangList)
            {
                //  trEnglish resourcemanager.GetString("trMenu");
                //trArabic
                Replang comborow = new Replang();
                comborow.langId = reprow.valId;
                comborow.lang = reprow.value;

                if (reprow.value == "ar")
                {
                    comborow.trlang = MainWindow.resourcemanager.GetString("trArabic");
                }
                else if (reprow.value == "en")
                {
                    comborow.trlang = MainWindow.resourcemanager.GetString("trEnglish");
                }
                else
                {
                    comborow.trlang = "";
                }

                langcomboList.Add(comborow);
            }
            cb_reportlang.ItemsSource = langcomboList;
            cb_reportlang.DisplayMemberPath = "trlang";
            cb_reportlang.SelectedValuePath = "langId";
            replangrow = replangList.Where(r => r.isDefault == 1).FirstOrDefault();
            cb_reportlang.SelectedValue = replangrow.valId;
        }

        public static async Task<SetValues> getDefaultPrintCount()
        {
            List<SettingCls> settingsCls = await setModel.GetAll();
            List<SetValues> settingsValues = await valueModel.GetAll();
            set = settingsCls.Where(s => s.name == "Allow_print_inv_count").FirstOrDefault<SettingCls>();
            printCountId = set.settingId;
            printCount = settingsValues.Where(i => i.settingId == printCountId).FirstOrDefault();
            return printCount;
        }

        private async void   UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);
                MainWindow.mainWindow.initializationMainTrack(this.Tag.ToString() );


                #region translate
                if (MainWindow.lang.Equals("en"))
                {
                    MainWindow.resourcemanager = new ResourceManager("Restaurant.en_file", Assembly.GetExecutingAssembly());
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                }
                else
                {
                    MainWindow.resourcemanager = new ResourceManager("Restaurant.ar_file", Assembly.GetExecutingAssembly());
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                }

                translate();
                #endregion

                ///naji code
                ///
                await fillRepLang();
                #region get default print count
                await getDefaultPrintCount();
                if (printCount != null)
                    tb_printCount.Text = printCount.value;
                #endregion

                if (sender != null)
                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
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


        private void translate()
        {
            txt_printCount.Text = MainWindow.resourcemanager.GetString("trPrintCount");
            tb_printCount.Text = MainWindow.resourcemanager.GetString("trPrintCount");

            txt_reportlang.Text = MainWindow.resourcemanager.GetString("trLanguage");

            txt_systmSetting.Text = MainWindow.resourcemanager.GetString("trSystemSetting");
            txt_systmSettingHint.Text = MainWindow.resourcemanager.GetString("trSystemSetting") + "...";

            txt_printerSetting.Text = MainWindow.resourcemanager.GetString("trPrinterSettings");
            txt_printerSettingHint.Text = MainWindow.resourcemanager.GetString("trPrinterSettings") + "...";

            txt_copyCount.Text = MainWindow.resourcemanager.GetString("trCopyCount");
            txt_copyCountHint.Text = MainWindow.resourcemanager.GetString("trCopyCount") + "...";

            txt_printCount.Text = MainWindow.resourcemanager.GetString("trPrintCount");
        }

    
        private async void Btn_reportlang_Click(object sender, RoutedEventArgs e)
        {

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
                    await FillCombo.GetReportlang();
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

        private async void Btn_printCount_Click(object sender, RoutedEventArgs e)
        {
            
            //save print count
            try
            {
                
                    HelpClass.StartAwait(grid_main);

                //  HelpClass.validateEmptyTextBox(tb_printCount, p_errorPrintCount, tt_errorPrintCount, "trEmptyPrintCount");
                HelpClass.clearValidate(p_error_printCount);
                if (tb_printCount.Text.Equals(""))
                {
                    HelpClass.SetValidate( p_error_printCount,"trEmptyPrintCount");
                }
              else  if (int.Parse(tb_printCount.Text)<=0 )
                {
                    HelpClass.SetValidate(p_error_printCount, "trMustBeMoreThanZero");
                }
                else
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
                        FillCombo.Allow_print_inv_count = printCount.value;

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
