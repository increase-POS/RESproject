using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Reporting.WinForms;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Collections;
namespace POS.Classes
{
    class ReportCls
    {
      List<CurrencyInfo> currencies = new List<CurrencyInfo>();
        public static void clearFolder(string FolderName)
        {
            string filename = "";
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                filename = fi.FullName;
                if (!FileIsLocked(filename))
                {
                    fi.Delete();
                }

            }


        }


        public static bool FileIsLocked(string strFullFileName)
        {
            bool blnReturn = false;
            FileStream fs = null;

            try
            {
                fs = File.Open(strFullFileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
                fs.Close();
            }
            catch (IOException ex)
            {
                blnReturn = true;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return blnReturn;

        }
        public void Fillcurrency()
        {

            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Kuwait));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Saudi_Arabia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Oman));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.United_Arab_Emirates));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Qatar));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Bahrain));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Iraq));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Lebanon));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Syria));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Yemen));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Jordan));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Algeria));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Egypt));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Tunisia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Sudan));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Morocco));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Libya));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Somalia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Turkey));


        }

        public string PathUp(string path, int levelnum, string addtopath)
        {
            int pos1 = 0;
            for (int i = 1; i <= levelnum; i++)
            {
                pos1 = path.LastIndexOf("\\");
                path = path.Substring(0, pos1);
            }

            string newPath = path + addtopath;
            return newPath;
            //addtopath = addtopath.Substring(1);
            //return addtopath;
        }

        public string TimeToString(TimeSpan? time)
        {

            TimeSpan ts = TimeSpan.Parse(time.ToString());
            // @"hh\:mm\:ss"
            string stime = ts.ToString(@"hh\:mm");
            return stime;
        }

        public string DateToString(DateTime? date)
        {
            string sdate = "";
            if (date != null)
            {
                //DateTime ts = DateTime.Parse(date.ToString());
                // @"hh\:mm\:ss"
                //sdate = ts.ToString(@"d/M/yyyy");
                DateTimeFormatInfo dtfi = DateTimeFormatInfo.CurrentInfo;

                switch (MainWindow.dateFormat)
                {
                    case "ShortDatePattern":
                        sdate = date.Value.ToString(dtfi.ShortDatePattern);
                        break;
                    case "LongDatePattern":
                        sdate = date.Value.ToString(dtfi.LongDatePattern);
                        break;
                    case "MonthDayPattern":
                        sdate = date.Value.ToString(dtfi.MonthDayPattern);
                        break;
                    case "YearMonthPattern":
                        sdate = date.Value.ToString(dtfi.YearMonthPattern);
                        break;
                    default:
                        sdate = date.Value.ToString(dtfi.ShortDatePattern);
                        break;
                }
            }

            return sdate;
        }
        public static string DateToStringPatern(DateTime? date)
        {
            string sdate = "";
            if (date != null)
            {
                //DateTime ts = DateTime.Parse(date.ToString());
                // @"hh\:mm\:ss"
                //sdate = ts.ToString(@"d/M/yyyy");
                DateTimeFormatInfo dtfi = DateTimeFormatInfo.CurrentInfo;

                switch (MainWindow.dateFormat)
                {
                    case "ShortDatePattern":
                        sdate = date.Value.ToString(@"dd/MM/yyyy");
                        break;
                    case "LongDatePattern":
                        sdate = date.Value.ToString(@"dddd, MMMM d, yyyy");
                        break;
                    case "MonthDayPattern":
                        sdate = date.Value.ToString(@"MMMM dd");
                        break;
                    case "YearMonthPattern":
                        sdate = date.Value.ToString(@"MMMM yyyy");
                        break;
                    default:
                        sdate = date.Value.ToString(@"dd/MM/yyyy");
                        break;
                }
            }

            return sdate;
        }


        public string DecTostring(decimal? dec)
        {
            string sdc = "0";
            if (dec == null)
            {

            }
            else
            {
                decimal dc = decimal.Parse(dec.ToString());

                //sdc = dc.ToString("0.00");
                switch (MainWindow.accuracy)
                {
                    case "0":
                        sdc = string.Format("{0:F0}", dc);
                        break;
                    case "1":
                        sdc = string.Format("{0:F1}", dc);
                        break;
                    case "2":
                        sdc = string.Format("{0:F2}", dc);
                        break;
                    case "3":
                        sdc = string.Format("{0:F3}", dc);
                        break;
                    default:
                        sdc = string.Format("{0:F1}", dc);
                        break;
                }

            }


            return sdc;
        }

        public string BarcodeToImage(string barcodeStr, string imagename)
        {
            // create encoding object
            Zen.Barcode.Code128BarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
            string addpath = @"\Thumb\" + imagename + ".png";
            string imgpath = this.PathUp(Directory.GetCurrentDirectory(), 2, addpath);
            if (File.Exists(imgpath))
            {
                File.Delete(imgpath);
            }
            if (barcodeStr != "")
            {
                System.Drawing.Bitmap serial_bitmap = (System.Drawing.Bitmap)barcode.Draw(barcodeStr, 60);
                // System.Drawing.ImageConverter ic = new System.Drawing.ImageConverter();

                serial_bitmap.Save(imgpath);

                //  generate bitmap
                //  img_barcode.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(serial_bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            else
            {

                imgpath = "";
            }
            if (File.Exists(imgpath))
            {
                return imgpath;
            }
            else
            {
                return "";
            }


        }
        public decimal percentValue(decimal? value, decimal? percent)
        {
            decimal? perval = (value * percent / 100);
            return (decimal)perval;
        }

        public string BarcodeToImage28(string barcodeStr, string imagename)
        {
            // create encoding object
            Zen.Barcode.Code128BarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
            string addpath = @"\Thumb\" + imagename + ".png";
            string imgpath = this.PathUp(Directory.GetCurrentDirectory(), 2, addpath);
            if (File.Exists(imgpath))
            {
                File.Delete(imgpath);
            }
            if (barcodeStr != "")
            {
                System.Drawing.Bitmap serial_bitmap = (System.Drawing.Bitmap)barcode.Draw(barcodeStr, 60);
                // System.Drawing.ImageConverter ic = new System.Drawing.ImageConverter();

                serial_bitmap.Save(imgpath);

                //  generate bitmap
                //  img_barcode.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(serial_bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            else
            {

                imgpath = "";
            }
            if (File.Exists(imgpath))
            {
                return imgpath;
            }
            else
            {
                return "";
            }


        }
        public static bool checkLang()
        {
            bool isArabic;
            if (MainWindow.Reportlang.Equals("en"))
            {
                MainWindow.resourcemanagerreport = new ResourceManager("POS.en_file", Assembly.GetExecutingAssembly());
                isArabic = false;
            }
            else
            {
                MainWindow.resourcemanagerreport = new ResourceManager("POS.ar_file", Assembly.GetExecutingAssembly());
                isArabic = true;
            }
            return isArabic;
        }

        public List<ReportParameter> fillPayReport(CashTransfer cashtrans)
        {
            bool isArabic = checkLang();
            Fillcurrency();
            string title;
            if (cashtrans.transType == "p")
                title = MainWindow.resourcemanagerreport.GetString("trPayVocher");
            else
                title = MainWindow.resourcemanagerreport.GetString("trReceiptVoucher");


            string company_name = MainWindow.companyName;
            string comapny_address = MainWindow.Address;
            string company_phone = MainWindow.Phone;
            string company_fax = MainWindow.Fax;
            string company_email = MainWindow.Email;
            //   string company_logo_img = GetLogoImagePath();
            //string amount = cashtrans.cash.ToString();
            string amount = DecTostring(cashtrans.cash);

            string voucher_num = cashtrans.transNum.ToString();
            string type = "";
            string isCash = "0";
            string trans_num_txt = "";

            string check_num = cashtrans.docNum;
            //string date = cashtrans.createDate.ToString();
            string date = DateToString(cashtrans.createDate);
            string from = "";
            string amount_in_words = "";
            string purpose = "";
            string recived_by = "";
            string user_name = cashtrans.createUserName + " " + cashtrans.createUserLName;
            string job = "Employee"; //cashtrans.createUserJob;
            string pay_to;

            if (cashtrans.side == "u")
            {
                pay_to = cashtrans.usersName + " " + cashtrans.usersLName;

            }
            else if (cashtrans.side == "v" || cashtrans.side == "c")
            {
                pay_to = cashtrans.agentName;
            }
            else if (cashtrans.side == "sh")
            {
                pay_to = cashtrans.shippingCompanyName;
            }
            else
            {
                pay_to = "";
            }
            if (cashtrans.processType == "cheque")
            {

                type = MainWindow.resourcemanagerreport.GetString("trCheque");
                if (isArabic)
                {
                    trans_num_txt = "رقم الشيك:";
                }
                else
                {
                    trans_num_txt = "Cheque Num:";
                }

                //    MainWindow.resourcemanagerreport.GetString("trCheque");
            }
            else if (cashtrans.processType == "card")
            {
                type = cashtrans.cardName;

                if (isArabic)
                {
                    trans_num_txt = "رقم العملية:";
                }
                else
                {
                    trans_num_txt = "Transfer Num:";
                }


                // card name and number
            }
            else if (cashtrans.processType == "cash")
            {
                type = "Cash";
                isCash = "1";

            }
            else if (cashtrans.processType == "doc")
            {
                if (isArabic)
                {
                    type = "مستند";
                    trans_num_txt = "رقم المستند:";
                }
                else
                {
                    type = "Document";
                    trans_num_txt = "Document Num:";
                }




            }
            /////
            try
            {

                int id = MainWindow.CurrencyId;
                ToWord toWord = new ToWord(Convert.ToDecimal(amount), currencies[id]);

                if (isArabic)
                {
                    amount_in_words = toWord.ConvertToArabic();
                    // cashtrans.cash
                }
                else
                {
                    amount_in_words = toWord.ConvertToEnglish(); ;
                }

            }
            catch (Exception ex)
            {
                amount_in_words = String.Empty;

            }

            //  rep.DataSources.Add(new ReportDataSource("DataSetBank", banksQuery));

            List<ReportParameter> paramarr = new List<ReportParameter>();
            paramarr.Add(new ReportParameter("lang", MainWindow.Reportlang));
            paramarr.Add(new ReportParameter("title", title));
            paramarr.Add(new ReportParameter("company_name", company_name));
            paramarr.Add(new ReportParameter("comapny_address", comapny_address));
            paramarr.Add(new ReportParameter("company_phone", company_phone));
            paramarr.Add(new ReportParameter("company_fax", company_fax));
            paramarr.Add(new ReportParameter("company_email", company_email));
            paramarr.Add(new ReportParameter("company_logo_img", "file:\\" + GetLogoImagePath()));
            paramarr.Add(new ReportParameter("amount", amount));
            paramarr.Add(new ReportParameter("voucher_num", voucher_num));
            paramarr.Add(new ReportParameter("type", type));
            paramarr.Add(new ReportParameter("check_num", check_num));
            paramarr.Add(new ReportParameter("date", date));
            paramarr.Add(new ReportParameter("from", from));


            paramarr.Add(new ReportParameter("amount_in_words", amount_in_words));
            paramarr.Add(new ReportParameter("purpose", purpose));
            paramarr.Add(new ReportParameter("recived_by", recived_by));
            paramarr.Add(new ReportParameter("purpose", purpose));
            paramarr.Add(new ReportParameter("user_name", user_name));
            paramarr.Add(new ReportParameter("pay_to", pay_to));
            paramarr.Add(new ReportParameter("job", job));
            paramarr.Add(new ReportParameter("isCash", isCash));
            paramarr.Add(new ReportParameter("trans_num_txt", trans_num_txt));
            return paramarr;
        }
        public string ConvertAmountToWords(Nullable<decimal>  amount)
        {
            Fillcurrency();
            string amount_in_words = "";
            try
            {

                bool isArabic;
                int id = MainWindow.CurrencyId;
                ToWord toWord = new ToWord(Convert.ToDecimal(amount), currencies[id]);
                isArabic = checkLang();
                if (isArabic)
                {
                    amount_in_words = toWord.ConvertToArabic();
                    // cashtrans.cash
                }
                else
                {
                    amount_in_words = toWord.ConvertToEnglish(); ;
                }

            }
            catch (Exception ex)
            {
                amount_in_words = String.Empty;

            }
            return amount_in_words;

        }
        public static string NumberToWordsEN(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWordsEN(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWordsEN(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWordsEN(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWordsEN(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
        public static string NumberToWordsAR(int number)
        {
            if (number == 0)
                return "صفر";

            if (number < 0)
                return "ناقص " + NumberToWordsAR(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWordsAR(number / 1000000) + " مليون ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWordsAR(number / 1000) + " الف ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWordsAR(number / 100) + " مئة ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "و ";

                var unitsMap = new[] { "صفر", "واحد", "اثنان", "ثلاثة", "اربعة", "خمسة", "ستة", "سبعة", "ثمانية", "تسعة", "عشرة", "احدى عشر", "اثنا عشر", "ثلاثة عشر", "اربعة عشر", "خمسة عشر", "ستة عشر", "سبعة عشر", "ثمانية عشر", "تسعة عشر" };
                var tensMap = new[] { "صفر", "عشرة", "عشرون", "ثلاثون", "اربعون", "خمسون", "ستون", "سبعون", "ثمانون", "تسعون" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }

        public string GetLogoImagePath()
        {
            string imageName = MainWindow.logoImage;
            string dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            string tmpPath = Path.Combine(dir, @"Thumb\setting");
            tmpPath = Path.Combine(tmpPath, imageName);
            /*
                  
                  
             * */

            //string addpath = @"\Thumb\setting\" ;

            return tmpPath;
        }

        //

        public string GetPath(string localpath)
        {
            //string imageName = MainWindow.logoImage;
            string dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            string tmpPath = Path.Combine(dir, localpath);



            //string addpath = @"\Thumb\setting\" ;

            return tmpPath;
        }

        public string ReadFile(string localpath)
        {
            string path = GetPath(localpath);
            StreamReader str = new StreamReader(path);
            string content = str.ReadToEnd();
            str.Close();
            return content;
        }

        public string GetpayInvoiceRdlcpath(Invoice invoice)
        {
            string addpath;
            bool isArabic = ReportCls.checkLang();
            if (isArabic)
            {
                if (invoice.invType == "or" || invoice.invType == "po" || invoice.invType == "pod")
                {
                    addpath = @"\Reports\Purchase\Ar\ArInvPurOrderReport.rdlc";
                }
                else
                {
                    addpath = @"\Reports\Purchase\Ar\ArInvPurReport.rdlc";
                }

            }
            else
            {
                if (invoice.invType == "or" || invoice.invType == "po" || invoice.invType == "pod")
                {
                    addpath = @"\Reports\Purchase\En\InvPurOrderReport.rdlc";
                }
                else
                {
                    addpath = @"\Reports\Purchase\En\InvPurReport.rdlc";
                }
            }


            //

            string reppath = PathUp(Directory.GetCurrentDirectory(), 2, addpath);
            return reppath;
        }
        public int GetpageHeight(int itemcount, int repheight)
        {
            // int repheight = 457;
            int tableheight = 33 * itemcount;// 33 is cell height


            int totalheight = repheight + tableheight;
            return totalheight;

        }
        public string GetreceiptInvoiceRdlcpath(Invoice invoice)
        {
            string addpath;
            bool isArabic = checkLang();
            if (isArabic)
            {

                if (invoice.invType == "q" || invoice.invType == "qd")
                {
                    addpath = @"\Reports\Sale\Ar\ArInvPurQtReport.rdlc";
                }
                else if (invoice.invType == "or" || invoice.invType == "ord")
                {
                    addpath = @"\Reports\Sale\Ar\ArInvPurOrderReport.rdlc";
                }
                else
                {

                    if (MainWindow.salePaperSize == "10cm")
                    {
                        addpath = @"\Reports\Sale\Ar\LargeSaleReport.rdlc";
                        View.uc_receiptInvoice.width = 400;//400 =10cm
                        View.uc_receiptInvoice.height = GetpageHeight(View.uc_receiptInvoice.itemscount, 500);

                    }
                    else if (MainWindow.salePaperSize == "8cm")
                    {
                        addpath = @"\Reports\Sale\Ar\MediumSaleReport.rdlc";
                        View.uc_receiptInvoice.width = 315;//315 =8cm
                        View.uc_receiptInvoice.height = GetpageHeight(View.uc_receiptInvoice.itemscount, 500);


                    }
                    else if (MainWindow.salePaperSize == "5.7cm")
                    {
                        addpath = @"\Reports\Sale\Ar\SmallSaleReport.rdlc";
                        View.uc_receiptInvoice.width = 224;//224 =5.7cm
                        View.uc_receiptInvoice.height = GetpageHeight(View.uc_receiptInvoice.itemscount, 460);

                    }
                    else //MainWindow.salePaperSize == "A4"
                    {

                        addpath = @"\Reports\Sale\Ar\ArInvPurReport.rdlc";
                    }

                    //   addpath = @"\Reports\Sale\Ar\LargeSaleReport.rdlc";
                    //   addpath = @"\Reports\Sale\Ar\MediumSaleReport.rdlc";
                    //   addpath = @"\Reports\Sale\Ar\SmallSaleReport.rdlc";
                }

            }
            else
            {
                if (invoice.invType == "q" || invoice.invType == "qd")
                {
                    addpath = @"\Reports\Sale\En\InvPurQtReport.rdlc";
                }
                else if (invoice.invType == "or" || invoice.invType == "ord")
                {
                    addpath = @"\Reports\Sale\En\InvPurOrderReport.rdlc";
                }
                else
                {
                    if (MainWindow.salePaperSize == "10cm")
                    {
                        addpath = @"\Reports\Sale\En\LargeSaleReport.rdlc";
                        View.uc_receiptInvoice.width = 400;//400 =10cm
                        View.uc_receiptInvoice.height = GetpageHeight(View.uc_receiptInvoice.itemscount, 500);

                    }
                    else if (MainWindow.salePaperSize == "8cm")
                    {
                        addpath = @"\Reports\Sale\En\MediumSaleReport.rdlc";
                        View.uc_receiptInvoice.width = 315;//315 =8cm
                        View.uc_receiptInvoice.height = GetpageHeight(View.uc_receiptInvoice.itemscount, 500);

                    }
                    else if (MainWindow.salePaperSize == "5.7cm")
                    {
                        addpath = @"\Reports\Sale\En\SmallSaleReport.rdlc";
                        View.uc_receiptInvoice.width = 224;//224 =5.7cm
                        View.uc_receiptInvoice.height = GetpageHeight(View.uc_receiptInvoice.itemscount, 460);

                    }
                    else //MainWindow.salePaperSize == "A4"
                    {

                        addpath = @"\Reports\Sale\En\InvPurReport.rdlc";
                    }
                    //  addpath = @"\Reports\Sale\En\InvPurReport.rdlc";
                    //    addpath = @"\Reports\Sale\En\LargeSaleReport.rdlc";
                    //   addpath = @"\Reports\Sale\En\MediumSaleReport.rdlc";
                    // addpath = @"\Reports\Sale\En\SmallSaleReport.rdlc";
                }

            }


            //

            string reppath = PathUp(Directory.GetCurrentDirectory(), 2, addpath);
            return reppath;
        }

        public string GetreceiptInvoiceRdlcpath(Invoice invoice, int isPreview)
        {
            string addpath;
            bool isArabic = checkLang();
            if (isArabic)
            {

                if ((invoice.invType == "q" || invoice.invType == "qd"))
                {
                    addpath = @"\Reports\Sale\Ar\ArInvPurQtReport.rdlc";
                }
                else if (invoice.invType == "or" || invoice.invType == "ord")
                {
                    addpath = @"\Reports\Sale\Ar\ArInvPurOrderReport.rdlc";
                }
                else
                {

                    if (MainWindow.salePaperSize == "10cm" && isPreview == 1)
                    {
                        addpath = @"\Reports\Sale\Ar\LargeSaleReport.rdlc";
                        View.uc_receiptInvoice.width = 400;//400 =10cm
                        View.uc_receiptInvoice.height = GetpageHeight(View.uc_receiptInvoice.itemscount, 500);

                    }
                    else if (MainWindow.salePaperSize == "8cm" && isPreview == 1)
                    {
                        addpath = @"\Reports\Sale\Ar\MediumSaleReport.rdlc";
                        View.uc_receiptInvoice.width = 315;//315 =8cm
                        View.uc_receiptInvoice.height = GetpageHeight(View.uc_receiptInvoice.itemscount, 500);


                    }
                    else if (MainWindow.salePaperSize == "5.7cm" && isPreview == 1)
                    {
                        addpath = @"\Reports\Sale\Ar\SmallSaleReport.rdlc";
                        View.uc_receiptInvoice.width = 224;//224 =5.7cm
                        View.uc_receiptInvoice.height = GetpageHeight(View.uc_receiptInvoice.itemscount, 460);

                    }
                    else //MainWindow.salePaperSize == "A4"
                    {

                        addpath = @"\Reports\Sale\Ar\ArInvPurReport.rdlc";
                    }

                    //   addpath = @"\Reports\Sale\Ar\LargeSaleReport.rdlc";
                    //   addpath = @"\Reports\Sale\Ar\MediumSaleReport.rdlc";
                    //   addpath = @"\Reports\Sale\Ar\SmallSaleReport.rdlc";
                }

            }
            else
            {
                if (invoice.invType == "q" || invoice.invType == "qd")
                {
                    addpath = @"\Reports\Sale\En\InvPurQtReport.rdlc";
                }
                else if (invoice.invType == "or" || invoice.invType == "ord")
                {
                    addpath = @"\Reports\Sale\En\InvPurOrderReport.rdlc";
                }
                else
                {
                    if (MainWindow.salePaperSize == "10cm" && isPreview == 1)
                    {
                        addpath = @"\Reports\Sale\En\LargeSaleReport.rdlc";
                        View.uc_receiptInvoice.width = 400;//400 =10cm
                        View.uc_receiptInvoice.height = GetpageHeight(View.uc_receiptInvoice.itemscount, 500);

                    }
                    else if (MainWindow.salePaperSize == "8cm" && isPreview == 1)
                    {
                        addpath = @"\Reports\Sale\En\MediumSaleReport.rdlc";
                        View.uc_receiptInvoice.width = 315;//315 =8cm
                        View.uc_receiptInvoice.height = GetpageHeight(View.uc_receiptInvoice.itemscount, 500);

                    }
                    else if (MainWindow.salePaperSize == "5.7cm" && isPreview == 1)
                    {
                        addpath = @"\Reports\Sale\En\SmallSaleReport.rdlc";
                        View.uc_receiptInvoice.width = 224;//224 =5.7cm
                        View.uc_receiptInvoice.height = GetpageHeight(View.uc_receiptInvoice.itemscount, 460);

                    }
                    else //MainWindow.salePaperSize == "A4"
                    {

                        addpath = @"\Reports\Sale\En\InvPurReport.rdlc";
                    }
                    //  addpath = @"\Reports\Sale\En\InvPurReport.rdlc";
                    //    addpath = @"\Reports\Sale\En\LargeSaleReport.rdlc";
                    //   addpath = @"\Reports\Sale\En\MediumSaleReport.rdlc";
                    // addpath = @"\Reports\Sale\En\SmallSaleReport.rdlc";
                }

            }


            //

            string reppath = PathUp(Directory.GetCurrentDirectory(), 2, addpath);
            return reppath;
        }
        public List<ReportParameter> fillPurInvReport(Invoice invoice, List<ReportParameter> paramarr)
        {
            checkLang();

            decimal disval = calcpercentval(invoice.discountType, invoice.discountValue, invoice.total);
            decimal totalafterdis;
            if (invoice.total != null)
            {
                totalafterdis = (decimal)invoice.total - disval;
            }
            else
            {
                totalafterdis = 0;
            }
            string userName = invoice.uuserName + " " + invoice.uuserLast;
            // string agentName = (invoice.agentCompany != null || invoice.agentCompany != "") ? invoice.agentCompany.Trim()
            //    : ((invoice.agentName != null || invoice.agentName != "") ? invoice.agentName.Trim() : "-");
            string agentName = "-";

            //    decimal taxval = calcpercentval("2", invoice.tax, totalafterdis);
            // decimal totalnet = totalafterdis + taxval;

            //  rep.DataSources.Add(new ReportDataSource("DataSetBank", banksQuery));

            //discountType
            paramarr.Add(new ReportParameter("invNumber", invoice.invNumber == null ? "-" : invoice.invNumber.ToString()));//paramarr[6]
            paramarr.Add(new ReportParameter("invoiceId", invoice.invoiceId.ToString()));



            paramarr.Add(new ReportParameter("invDate", DateToString(invoice.invDate) == null ? "-" : DateToString(invoice.invDate)));
            paramarr.Add(new ReportParameter("invTime", TimeToString(invoice.invTime)));
            paramarr.Add(new ReportParameter("vendorInvNum", invoice.agentCode == "-" ? "-" : invoice.agentCode.ToString()));
            paramarr.Add(new ReportParameter("agentName", agentName));
            paramarr.Add(new ReportParameter("total", DecTostring(invoice.total) == null ? "-" : DecTostring(invoice.total)));

            //  paramarr.Add(new ReportParameter("discountValue", DecTostring(disval) == null ? "-" : DecTostring(disval)));
            paramarr.Add(new ReportParameter("discountValue", invoice.discountValue == null ? "0" : DecTostring(invoice.discountValue)));
            paramarr.Add(new ReportParameter("discountType", invoice.discountType == null ? "1" : invoice.discountType.ToString()));

            paramarr.Add(new ReportParameter("totalNet", DecTostring(invoice.totalNet) == null ? "-" : DecTostring(invoice.totalNet)));
            paramarr.Add(new ReportParameter("paid", DecTostring(invoice.paid) == null ? "-" : DecTostring(invoice.paid)));
            paramarr.Add(new ReportParameter("deserved", DecTostring(invoice.deserved) == null ? "-" : DecTostring(invoice.deserved)));
            //paramarr.Add(new ReportParameter("deservedDate", invoice.deservedDate.ToString() == null ? "-" : invoice.deservedDate.ToString()));
            paramarr.Add(new ReportParameter("deservedDate", invoice.deservedDate.ToString() == null ? "-" : DateToString(invoice.deservedDate)));

            paramarr.Add(new ReportParameter("tax", DecTostring(invoice.tax) == null ? "0" : DecTostring(invoice.tax)));
            string invNum = invoice.invNumber == null ? "-" : invoice.invNumber.ToString();
            paramarr.Add(new ReportParameter("barcodeimage", "file:\\" + BarcodeToImage(invNum, "invnum")));
            paramarr.Add(new ReportParameter("Currency", MainWindow.Currency));
            paramarr.Add(new ReportParameter("logoImage", "file:\\" + GetLogoImagePath()));
            paramarr.Add(new ReportParameter("branchName", invoice.branchName == null ? "-" : invoice.branchName));
            paramarr.Add(new ReportParameter("userName", userName.Trim()));
            if (invoice.invType == "pd" || invoice.invType == "sd" || invoice.invType == "qd"
                    || invoice.invType == "sbd" || invoice.invType == "pbd" || invoice.invType == "pod"
                    || invoice.invType == "ord" || invoice.invType == "imd" || invoice.invType == "exd")
            {

                paramarr.Add(new ReportParameter("watermark", "1"));
            }
            else
            {
                paramarr.Add(new ReportParameter("watermark", "0"));
            }


            return paramarr;
        }


        public decimal calcpercentval(string discountType, decimal? discountValue, decimal? total)
        {

            decimal disval;
            if (discountValue == null || discountValue == 0)
            {
                disval = 0;

            }
            else if (discountValue > 0)
            {

                if (discountType == null || discountType == "-1" || discountType == "0" || discountType == "1")
                {
                    disval = (decimal)discountValue;
                }
                else

                {//percent
                    if (total == null || total == 0)
                    {
                        disval = 0;
                    }
                    else
                    {
                        disval = percentValue(total, discountValue);
                    }
                }
            }
            else
            {
                disval = 0;
            }

            return disval;
        }
        public List<ReportParameter> fillSaleInvReport(Invoice invoice, List<ReportParameter> paramarr)
        {
            checkLang();

            string agentName = (invoice.agentCompany != null || invoice.agentCompany != "") ? invoice.agentCompany.Trim()
            : ((invoice.agentName != null || invoice.agentName != "") ? invoice.agentName.Trim() : "-");
            string userName = invoice.uuserName + " " + invoice.uuserLast;

            //  rep.DataSources.Add(new ReportDataSource("DataSetBank", banksQuery));
            decimal disval = calcpercentval(invoice.discountType, invoice.discountValue, invoice.total);
            decimal totalafterdis;
            if (invoice.total != null)
            {
                totalafterdis = (decimal)invoice.total - disval;
            }
            else
            {
                totalafterdis = 0;
            }

            // discountType
            //  decimal taxval = calcpercentval("2", invoice.tax, totalafterdis);

            // decimal totalnet = totalafterdis + taxval;
            //  percentValue(decimal ? value, decimal ? percent);

            paramarr.Add(new ReportParameter("Notes", (invoice.notes == null || invoice.notes == "") ? "-" : invoice.notes.Trim()));
            paramarr.Add(new ReportParameter("invNumber", (invoice.invNumber == null || invoice.invNumber == "") ? "-" : invoice.invNumber.ToString()));//paramarr[6]
            paramarr.Add(new ReportParameter("invoiceId", invoice.invoiceId.ToString()));

            paramarr.Add(new ReportParameter("invDate", DateToString(invoice.updateDate) == null ? "-" : DateToString(invoice.invDate)));
            paramarr.Add(new ReportParameter("invTime", TimeToString(invoice.invTime)));
            paramarr.Add(new ReportParameter("vendorInvNum", invoice.agentCode == "-" ? "-" : invoice.agentCode.ToString()));
            paramarr.Add(new ReportParameter("agentName", agentName.Trim()));
            paramarr.Add(new ReportParameter("total", DecTostring(invoice.total) == null ? "-" : DecTostring(invoice.total)));



            //  paramarr.Add(new ReportParameter("discountValue", DecTostring(disval) == null ? "-" : DecTostring(disval)));
            paramarr.Add(new ReportParameter("discountValue", invoice.discountValue == null ? "0" : DecTostring(invoice.discountValue)));
            paramarr.Add(new ReportParameter("discountType", invoice.discountType == null ? "1" : invoice.discountType.ToString()));

            paramarr.Add(new ReportParameter("totalNet", DecTostring(invoice.totalNet) == null ? "-" : DecTostring(invoice.totalNet)));
            paramarr.Add(new ReportParameter("paid", DecTostring(invoice.paid) == null ? "-" : DecTostring(invoice.paid)));
            paramarr.Add(new ReportParameter("deserved", DecTostring(invoice.deserved) == null ? "-" : DecTostring(invoice.deserved)));
            //paramarr.Add(new ReportParameter("deservedDate", invoice.deservedDate.ToString() == null ? "-" : invoice.deservedDate.ToString()));
            paramarr.Add(new ReportParameter("deservedDate", invoice.deservedDate.ToString() == null ? "-" : DateToString(invoice.deservedDate)));


            paramarr.Add(new ReportParameter("tax", DecTostring(invoice.tax) == null ? "0" : DecTostring(invoice.tax)));
            string invNum = invoice.invNumber == null ? "-" : invoice.invNumber.ToString();
            paramarr.Add(new ReportParameter("barcodeimage", "file:\\" + BarcodeToImage(invNum, "invnum")));
            paramarr.Add(new ReportParameter("Currency", MainWindow.Currency));
            paramarr.Add(new ReportParameter("branchName", invoice.branchName == null ? "-" : invoice.branchName));
            paramarr.Add(new ReportParameter("userName", userName.Trim()));
            paramarr.Add(new ReportParameter("logoImage", "file:\\" + GetLogoImagePath()));
            if (invoice.invType == "pd" || invoice.invType == "sd" || invoice.invType == "qd"
                        || invoice.invType == "sbd" || invoice.invType == "pbd" || invoice.invType == "pod"
                        || invoice.invType == "ord" || invoice.invType == "imd" || invoice.invType == "exd")
            {

                paramarr.Add(new ReportParameter("watermark", "1"));
            }
            else
            {
                paramarr.Add(new ReportParameter("watermark", "0"));
            }


            return paramarr;
        }
        public static List<ItemTransferInvoice> converter(List<ItemTransferInvoice> query)
        {
            foreach (ItemTransferInvoice item in query)
            {
                if (item.invType == "p")
                {
                    item.invType = MainWindow.resourcemanagerreport.GetString("trPurchaseInvoice");
                }
                else if (item.invType == "pw")
                {
                    item.invType = MainWindow.resourcemanagerreport.GetString("trPurchaseInvoice");
                }
                else if (item.invType == "pb")
                {
                    item.invType = MainWindow.resourcemanagerreport.GetString("trPurchaseReturnInvoice");
                }
                else if (item.invType == "pd")
                {
                    item.invType = MainWindow.resourcemanagerreport.GetString("trDraftPurchaseBill");
                }
                else if (item.invType == "pbd")
                {
                    item.invType = MainWindow.resourcemanagerreport.GetString("trPurchaseReturnDraft");
                }
            }
            return query;

        }


        /////////
        ///


        public bool encodefile(string source, string dest)
        {
            try
            {
              
                byte[] arr = File.ReadAllBytes(source);
              
                arr = Encrypt(arr);

                File.WriteAllBytes(dest, arr);
                return true;
            }
            catch
            {
                return false;
            }

        }

   

        public static byte[] Encrypt(byte[] ordinary)
        {
            BitArray bits = ToBits(ordinary);
            BitArray LHH = SubBits(bits, 0, bits.Length / 2);
            BitArray RHH = SubBits(bits, bits.Length / 2, bits.Length / 2);
            BitArray XorH = LHH.Xor(RHH);
            RHH = RHH.Not();
            XorH = XorH.Not();
            BitArray encr = ConcateBits(XorH, RHH);
            byte[] b = new byte[encr.Length / 8];
            encr.CopyTo(b, 0);
            return b;
        }


        private static BitArray ToBits(byte[] Bytes)
        {
            BitArray bits = new BitArray(Bytes);
            return bits;
        }
        private static BitArray SubBits(BitArray Bits, int Start, int Length)
        {
            BitArray half = new BitArray(Length);
            for (int i = 0; i < half.Length; i++)
                half[i] = Bits[i + Start];
            return half;
        }
        private static BitArray ConcateBits(BitArray LHH, BitArray RHH)
        {
            BitArray bits = new BitArray(LHH.Length + RHH.Length);
            for (int i = 0; i < LHH.Length; i++)
                bits[i] = LHH[i];
            for (int i = 0; i < RHH.Length; i++)
                bits[i + LHH.Length] = RHH[i];
            return bits;
        }
        public void DelFile(string fileName)
        {
          
                bool inuse = false;

                inuse = IsFileInUse(fileName);
                if (inuse == false)
                {
                    File.Delete(fileName);
                }

         
           



        }

        private bool IsFileInUse(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                //throw new ArgumentException("'path' cannot be null or empty.", "path");
                return true;
            }


            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite)) { }
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }


        //////////





    }
}

