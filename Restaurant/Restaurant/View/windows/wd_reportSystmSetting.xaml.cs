using netoaster;
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
    /// Interaction logic for wd_reportSystmSetting.xaml
    /// </summary>
    public partial class wd_reportSystmSetting : Window
    {
        
        public wd_reportSystmSetting()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            { HelpClass.ExceptionMessage(ex, this); }
        }
        BrushConverter bc = new BrushConverter();

        // print
        SetValues print_on_save_salerow = new SetValues();
        SetValues print_on_save_purrow = new SetValues();
        SetValues email_on_save_salerow = new SetValues();
        SetValues email_on_save_purrow = new SetValues();
        SetValues setvalueModel = new SetValues();
        string print_on_save_sale;
        string print_on_save_pur;
        string email_on_save_sale;
        string email_on_save_pur;

        List<SetValues> printList = new List<SetValues>();

        async Task Getprintparameter()
        {

            printList = await setvalueModel.GetBySetvalNote("print");
      

          

            print_on_save_salerow = printList.Where(X => X.name == "print_on_save_sale").FirstOrDefault();
            print_on_save_sale = print_on_save_salerow.value;

            print_on_save_purrow = printList.Where(X => X.name == "print_on_save_pur").FirstOrDefault();
            print_on_save_pur = print_on_save_purrow.value;

            email_on_save_salerow = printList.Where(X => X.name == "email_on_save_sale").FirstOrDefault();
            email_on_save_sale = email_on_save_salerow.value;

            email_on_save_purrow = printList.Where(X => X.name == "email_on_save_pur").FirstOrDefault();
            email_on_save_pur = email_on_save_purrow.value;
           
            if (print_on_save_pur == "1")
            {
                tgl_printOnSavePur.IsChecked = true;
            }
            else
            {
                tgl_printOnSavePur.IsChecked = false;
            }
            //
            if (print_on_save_sale == "1")
            {
                tgl_printOnSaveSale.IsChecked = true;
            }
            else
            {
                tgl_printOnSaveSale.IsChecked = false;
            }
            //
            if (email_on_save_pur == "1")
            {
                tgl_emailOnSavePur.IsChecked = true;
            }
            else
            {
                tgl_emailOnSavePur.IsChecked = false;
            }
            //////////////////
            if (email_on_save_sale == "1")
            {
                tgl_emailOnSaveSale.IsChecked = true;
            }
            else
            {
                tgl_emailOnSaveSale.IsChecked = false;
            }

           /*
            tgl_printOnSaveSale
            tgl_emailOnSavePur
            tgl_emailOnSaveSale
           */
          
        }

        private void Btn_colse_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {//load
           
            try
            {

                if (sender != null)
                    HelpClass.StartAwait(grid_main);

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

                //code
                await Getprintparameter();


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


        private void translate()
        {
         
            txt_title.Text = MainWindow.resourcemanager.GetString("trSystemSetting");

            txt_printOnSavePur.Text = MainWindow.resourcemanager.GetString("trPrintOnSavePurchase");
            txt_emailOnSavePur.Text = MainWindow.resourcemanager.GetString("trEmailOnSavePurchase");
            txt_printOnSaveSale.Text = MainWindow.resourcemanager.GetString("trPrintOnSaveSale");
            txt_emailOnSaveSale.Text = MainWindow.resourcemanager.GetString("trEmailOnSaveSale");

            btn_save.Content = MainWindow.resourcemanager.GetString("trSave");
 
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

        List<SettingCls> set = new List<SettingCls>();

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
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private async void Btn_save_Click(object sender, RoutedEventArgs e)
        {
           
          //  string msg = "";
            int msg = 0;
            if ((bool)tgl_printOnSavePur.IsChecked)
            {
                print_on_save_purrow.value = "1";

            }
            else
            {
                print_on_save_purrow.value = "0";
            }
            if ((bool)tgl_printOnSaveSale.IsChecked)
            {
                print_on_save_salerow.value = "1";

            }
            else
            {
                print_on_save_salerow.value = "0";
            }
            if ((bool)tgl_emailOnSavePur.IsChecked)
            {
                email_on_save_purrow.value = "1";

            }
            else
            {
                email_on_save_purrow.value = "0";
            }
            if ((bool)tgl_emailOnSaveSale.IsChecked)
            {
                email_on_save_salerow.value = "1";

            }
            else
            {
                email_on_save_salerow.value = "0";
            }
            msg = await setvalueModel.Save(print_on_save_purrow);
            msg = await setvalueModel.Save(print_on_save_salerow);
            msg = await setvalueModel.Save(email_on_save_purrow);
            msg = await setvalueModel.Save(email_on_save_salerow);


            await Getprintparameter();
            await FillCombo.Getprintparameter();
            if (msg > 0)
            {
                Toaster.ShowSuccess(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopSave"), animation: ToasterAnimation.FadeIn);
                await Task.Delay(1500);
                this.Close();
            }
            
            else
                Toaster.ShowWarning(Window.GetWindow(this), message: MainWindow.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
        
        }
    }
}
