﻿using Restaurant.Classes;
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

using System.Drawing.Printing;
using netoaster;

namespace Restaurant.View.windows
{
    /// <summary>
    /// Interaction logic for wd_reportPrinterSetting.xaml
    /// </summary>
    public partial class wd_reportPrinterSetting : Window
    {

        public wd_reportPrinterSetting()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            { HelpClass.ExceptionMessage(ex, this); }
        }


        BrushConverter bc = new BrushConverter();

        // printer
        
        List<Papersize> papersizeList = new List<Papersize>();
        List<Printers> printersList = new List<Printers>();
        Printers repprinterRow = new Printers();
        Printers salprinterRow = new Printers();
        PosSetting possettingModel = new PosSetting();

        int saleInvPrinterId;
        int reportPrinterId;
        int salepapersizeId;
        int docpapersizeId;

        Printers printerModel = new Printers();
        Papersize papersizeModel = new Papersize();
      

        public List<Printers> getsystemPrinters()
        {
            Printers printermodel = new Printers();

            List<Printers> printerList = new List<Printers>();

            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                printermodel = new Printers();

                printermodel.name = (string)PrinterSettings.InstalledPrinters[i];

                printerList.Add(printermodel);

            }
            return printerList;
        }

      
        public async Task<PosSetting> GetdefaultposSetting(PosSetting oldsetting)
        {
            Papersize papersizeModel = new Papersize();
            Printers printerModel = new Printers();
          

            PosSetting defpossetting = new PosSetting();
            defpossetting = oldsetting;
            //defpossetting.posId = oldsetting.posId;
            //defpossetting.posSettingId = oldsetting.posSettingId;

            //defpossetting.posSerial = oldsetting.posSerial;


            //defpossetting.posSettingId = oldsetting.posSettingId;

            //defpossetting.salprinterId = oldsetting.salprinterId;


            string printname = possettingModel.getdefaultPrinters();

            Printers defpr = new Printers();

            defpr.name = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(printname));
            if (oldsetting.saleInvPrinterId == null)
            {

                defpr.printFor = "sal";
                int saleInvPrinterId = await printerModel.Save(defpr);
                defpossetting.saleInvPrinterId = saleInvPrinterId;
                defpossetting.salname = defpr.name;
                defpossetting.salprintFor = "sal";
                defpossetting.salprinterId = saleInvPrinterId;

            }
            if (oldsetting.reportPrinterId == null)
            {
                defpr.printFor = "doc";//"doc"
                int reportPrinterId = await printerModel.Save(defpr);
                defpossetting.reportPrinterId = reportPrinterId;
                defpossetting.repname = defpr.name;


            }



            papersizeList = await papersizeModel.GetAll();
         
          
            if (oldsetting.saleInvPapersizeId == null)
            {
                int salsizeid = papersizeList.Where(x => x.printfor.Contains("sal") && x.sizeValue == "A4").FirstOrDefault().sizeId;
                defpossetting.saleInvPapersizeId = salsizeid;
                defpossetting.saleSizeValue = "A4";
            

            }

            if (oldsetting.docPapersizeId == null)
            {
                int docsizeid = papersizeList.Where(x => x.printfor.Contains("doc") && x.sizeValue == "A5").FirstOrDefault().sizeId;
                defpossetting.docPapersizeId = docsizeid;
                defpossetting.docPapersize = "A5";

            }

            //   defpossetting.saleInvPrinterId=
            //  defpossetting.reportPrinterId

            return defpossetting;

        }
        async Task GetposSetting()
        {

            possettingModel = await possettingModel.GetByposId((int)MainWindow.posLogin.posId);

            possettingModel = await GetdefaultposSetting(possettingModel);
          //  papersizeList = await papersizeModel.GetAll();

            if (possettingModel is null || possettingModel.posSettingId <= 0)
            {

                //possettingModel = new PosSetting();
                //possettingModel =await GetdefaultposSetting(possettingModel);

            }
            else
            {


                reportPrinterId = (int)possettingModel.reportPrinterId;
                saleInvPrinterId = (int)possettingModel.saleInvPrinterId;

                salepapersizeId = (int)possettingModel.saleInvPapersizeId;
                if (possettingModel.docPapersizeId != null && possettingModel.docPapersizeId > 0)
                {
                    docpapersizeId = (int)possettingModel.docPapersizeId;
                    //   docpapersizerow = await papersizeModel.GetByID(docpapersizeId);
                }

                if (reportPrinterId > 0)
                    repprinterRow = await printerModel.GetByID(reportPrinterId);
                if (saleInvPrinterId > 0)
                    salprinterRow = await printerModel.GetByID(saleInvPrinterId);



            }


        }
        public void fillcb_repname()
        {
            cb_repname.ItemsSource = printersList;
            cb_repname.DisplayMemberPath = "name";
            cb_repname.SelectedValuePath = "name";

            if (repprinterRow.printerId > 0)
            {
                cb_repname.SelectedValue = Encoding.UTF8.GetString(Convert.FromBase64String(repprinterRow.name));

            }

        }

        public void fillcb_salname()
        {
            cb_salname.ItemsSource = printersList;
            cb_salname.DisplayMemberPath = "name";
            cb_salname.SelectedValuePath = "name";
            if (salprinterRow.printerId > 0)
            {
                cb_salname.SelectedValue = (string)Encoding.UTF8.GetString(Convert.FromBase64String(salprinterRow.name));

            }

        }
        //
        public void fillcb_docpapersize()
        {
            cb_docpapersize.ItemsSource = papersizeList.Where(x => x.printfor.Contains("doc"));
            cb_docpapersize.DisplayMemberPath = "paperSize1";
            cb_docpapersize.SelectedValuePath = "sizeId";
            if (docpapersizeId > 0)
            {

                cb_docpapersize.SelectedValue = docpapersizeId;

            }

        }
        public void fillcb_saleInvPaperSize()
        {

            cb_saleInvPaperSize.ItemsSource = papersizeList.Where(x => x.printfor.Contains("sal"));
            // var paperList = papersizeList.Where(x => x.printfor.Contains("sal"));
            cb_saleInvPaperSize.DisplayMemberPath = "paperSize1";
            cb_saleInvPaperSize.SelectedValuePath = "sizeId";
            if (salepapersizeId > 0)
            {
                cb_saleInvPaperSize.SelectedValue = salepapersizeId;
            }


        }

        async Task refreshWindow()
        {

            printersList = getsystemPrinters();
            await GetposSetting();
            fillcb_salname();
            fillcb_repname();
            fillcb_docpapersize();



            fillcb_saleInvPaperSize();

        }
        public async Task<string> Saveprinters()
        {
            PosSetting posscls = new PosSetting();
            // string msg = "";
            int msg = 0;
            string printern = (string)cb_repname.SelectedValue;


            if (printern != null && printern != "")
            {
                repprinterRow.name = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(printern));

            }
            else
            {
                printern = possettingModel.getdefaultPrinters();
                repprinterRow.name = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(printern));
            }
            repprinterRow.printFor = "rep";
            repprinterRow.createUserId = MainWindow.userLogin.userId;

            msg = await printerModel.Save(repprinterRow);
            // reportPrinterId = int.Parse(msg);
            reportPrinterId = msg;


            printern = (string)cb_salname.SelectedValue;
            if (printern != null && printern != "")
            {
                salprinterRow.name = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(printern));

            }
            else
            {
                printern = possettingModel.getdefaultPrinters();
                salprinterRow.name = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(printern));
            }





            salprinterRow.printFor = "sal";
            salprinterRow.createUserId = MainWindow.userLogin.userId;

            msg = await printerModel.Save(salprinterRow);
            // saleInvPrinterId = int.Parse(msg);
            saleInvPrinterId = msg;



            possettingModel.posId = MainWindow.posLogin.posId;
            possettingModel.reportPrinterId = reportPrinterId;
            possettingModel.saleInvPrinterId = saleInvPrinterId;
            ////
            int salsizeid = papersizeList.Where(x => x.printfor.Contains("sal") && x.sizeValue == "A4").FirstOrDefault().sizeId;
            int docsizeid = papersizeList.Where(x => x.printfor.Contains("doc") && x.sizeValue == "A5").FirstOrDefault().sizeId;
            string psize = "";
            if (cb_saleInvPaperSize.SelectedIndex != -1)
            {
                psize = (string)cb_saleInvPaperSize.SelectedValue.ToString();
                if (psize != null && psize != "")
                {
                    possettingModel.saleInvPapersizeId = int.Parse(psize);
                }
                else
                {
                    possettingModel.saleInvPapersizeId = salsizeid;
                }

            }
            else
            {
                possettingModel.saleInvPapersizeId = salsizeid;
            }



            // possettingModel.saleInvPapersizeId = (int)cb_saleInvPaperSize.SelectedValue;

            if (cb_docpapersize.SelectedIndex != -1)
            {
                psize = (string)cb_docpapersize.SelectedValue.ToString();

                if (psize != null && psize != "")
                {
                    possettingModel.docPapersizeId = int.Parse(psize);
                }
                else
                {
                    possettingModel.docPapersizeId = docsizeid;
                }

            }
            else
            {
                possettingModel.docPapersizeId = docsizeid;
            }


            // possettingModel.docPapersizeId = (int)cb_docpapersize.SelectedValue;

            posscls.posSettingId = possettingModel.posSettingId;
            posscls.posId = possettingModel.posId;
            posscls.saleInvPrinterId = possettingModel.saleInvPrinterId;
            posscls.reportPrinterId = possettingModel.reportPrinterId;

            posscls.saleInvPapersizeId = possettingModel.saleInvPapersizeId;
            posscls.docPapersizeId = possettingModel.docPapersizeId;



            // msg = await possettingModel.Save(posscls);
            msg = await possettingModel.Save(posscls);
            return msg.ToString();
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
          
        }


        private void translate()
        {
           
            txt_title.Text = AppSettings.resourcemanager.GetString("trPrinterSettings");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_repname, AppSettings.resourcemanager.GetString("trReportPrinterName")+"...");//
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_salname, AppSettings.resourcemanager.GetString("trReportSalesName") + "...");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_saleInvPaperSize, AppSettings.resourcemanager.GetString("trSalesPaperSize") + "...");
            MaterialDesignThemes.Wpf.HintAssist.SetHint(cb_docpapersize, AppSettings.resourcemanager.GetString("trDocPaperSize") + "...");

            btn_save.Content = AppSettings.resourcemanager.GetString("trSave");
        
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
                //HelpClass.ExceptionMessage(ex, this);
            }
        }



        private async void Btn_save_Click(object sender, RoutedEventArgs e)
        { 
            string msg = "";
            msg = await Saveprinters();

            await refreshWindow();
            await FillCombo.getPrintersNames();
            if (int.Parse(msg) > 0)
            {
                Toaster.ShowSuccess(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopSave"), animation: ToasterAnimation.FadeIn);
                await Task.Delay(1500);
                this.Close();
            }
            else
            {
                Toaster.ShowWarning(Window.GetWindow(this), message: AppSettings.resourcemanager.GetString("trPopError"), animation: ToasterAnimation.FadeIn);
            }

             

        }
    }
}
