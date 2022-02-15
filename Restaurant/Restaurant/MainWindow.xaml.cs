using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPFTabTip;
using Restaurant;
using Restaurant.Classes;
using Restaurant.View.sectionData;
using Restaurant.View.catalog;
using Restaurant.View.purchase;
using Restaurant.View.storage;
using Restaurant.View.kitchen;
using Restaurant.View.sales;
using Restaurant.View.accounts;
using Restaurant.View.settings;
using Object = Restaurant.Classes.Object;
using Restaurant.View.sectionData.persons;
using Restaurant.View.sectionData.hallDivide;
using Restaurant.View.sectionData.branchesAndStores;
using Restaurant.View.catalog.foods;
using Restaurant.View.storage.storageDivide;
using Restaurant.View.storage.storageOperations;
using Restaurant.View.storage.movementsOperations;
using Restaurant.View.storage.stocktakingOperations;
using Restaurant.View.sales.promotion;
using Restaurant.View.delivery;
using Restaurant.View.sectionData.banksData;
using Restaurant.View.catalog.rawMaterials;
using Restaurant.View.settings.reportsSettings;
using Restaurant.View.sales.reservations;
using Restaurant.View.windows;
using Restaurant.View.settings.emailsGeneral;
using Restaurant.View.reports;
using Restaurant.View.reports.storageReports;
using Restaurant.View.reports.purchaseReports;
using Restaurant.View.reports.salesReports;
using Restaurant.View.reports.accountsReports;

namespace Restaurant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ResourceManager resourcemanager;
        public static ResourceManager resourcemanagerreport;
        bool menuState = false;
        //ToolTip="{Binding Properties.Settings.Default.Lang}"
        public static string firstPath = "";
        public static string secondPath = "";
        public static string first = "";
        public static string second = "";
        public static string lang = "ar";
        public static string Reportlang = "ar";
        public static string companyName;
        public static string Email;
        public static string Fax;
        public static string Mobile;
        public static string Address;
        //public static CountryCode Region;
        public static string Currency;
        public static int CurrencyId;
        public static string Phone;
        internal static User userLogin; 
        internal static Pos posLogin; 
        internal static Branch branchLogin; 
        //internal static Pos posLogIn;
        bool isHome = false;
        internal static int? isInvTax;
        internal static decimal? tax;
        internal static int? itemCost;
        internal static string dateFormat;
        internal static string accuracy;
        internal static decimal? StorageCost;
        public static int Idletime = 5;
        public static int threadtime = 5;
        public static string menuIsOpen = "close";
        //public static List<ItemUnitUser> itemUnitsUsers = new List<ItemUnitUser>();
        //public static ItemUnitUser itemUnitsUser = new ItemUnitUser();
        
        /////////// print setting////////////////////
        //public static string sale_copy_count;
        //public static string pur_copy_count;
        //public static string print_on_save_sale;
        //public static string print_on_save_pur;
        //public static string email_on_save_sale;
        //public static string email_on_save_pur;
        public static string rep_printer_name;
        //public static string sale_printer_name;
        //public static string salePaperSize;
        public static string rep_print_count;
        //public static string docPapersize;
        //public static string Allow_print_inv_count;
        /////////////////////////////////////////////
       
        Object objectModel = new Object();
        List<Object> listObjects = new List<Object>();
        static public GroupObject groupObject = new GroupObject();
        static public List<GroupObject> groupObjects = new List<GroupObject>();
        //static SettingCls setModel = new SettingCls();
        //static SetValues valueModel = new SetValues();
        static int nameId, addressId, emailId, mobileId, phoneId, faxId, logoId, taxId;
        public static string logoImage;


        ImageBrush myBrush = new ImageBrush();
        //NotificationUser notificationUser = new NotificationUser();

        public static DispatcherTimer timer;
        DispatcherTimer idletimer;//  logout timer
        DispatcherTimer threadtimer;//  repeat timer for check other login
        DispatcherTimer notTimer;//  repeat timer for notifications
                                 // print setting
       
        public static Boolean go_out = false;
      

        //internal static int? posID=1;
        //static public List<Item> InvoiceglobalItemsList = new List<Item>();
        static public List<ItemUnit> InvoiceGlobalItemUnitsList = new List<ItemUnit>();
        //static public List<Item> InvoiceglobalSaleUnitsList = new List<Item>();


        //public ItemUnit globalItemUnit = new ItemUnit();
        //public List<ItemUnit> globalItemUnitsList = new List<ItemUnit>();
        //static public Unit saleUnit = new Unit();

   

        static public MainWindow mainWindow;
        public MainWindow()
        {
            try
            {
               
                InitializeComponent();
                //userLogin = new User();
                //userLogin.userId = 1;
                mainWindow = this;
                windowFlowDirection();
                
            }
            catch (Exception ex)
            {
                //HelpClass.ExceptionMessage(ex, this);
            }

        }

        void windowFlowDirection()
        {
            #region translate
            if (lang.Equals("en"))
            {
                resourcemanager = new ResourceManager("Restaurant.en_file", Assembly.GetExecutingAssembly());
                grid_mainWindow.FlowDirection = FlowDirection.LeftToRight;
            }
            else
            {
                resourcemanager = new ResourceManager("Restaurant.ar_file", Assembly.GetExecutingAssembly());
                grid_mainWindow.FlowDirection = FlowDirection.RightToLeft;
            }
            #endregion
        }

        #region loading
        List<keyValueBool> loadingList;
        async void loading_listObjects()
        {
            //get tax
            try
            {
                listObjects = await objectModel.GetAll();
            }
            catch
            {
                
            }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_listObjects"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async void loading_getGroupObjects()
        {
            try
            {
                groupObjects = await groupObject.GetUserpermission(userLogin.userId);
            }
            catch (Exception)
            { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_getGroupObjects"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async void loading_globalItemUnitsList()
        {
            try
            {
                await FillCombo.RefreshItemUnit();
            }
            catch (Exception)
            { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_globalItemUnitsList"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        #region FillCombo
        async void loading_RefreshBranches()
        {
            try
            {
                await FillCombo.RefreshBranches();
            }
            catch (Exception)
            { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_RefreshBranches"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async void loading_RefreshBranchesAllWithoutMain()
        {
            try
            {
                await FillCombo.RefreshBranchesAllWithoutMain();
            }
            catch (Exception)
            { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_RefreshBranchesAllWithoutMain"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async void loading_RefreshByBranchandUser()
        {
            try
            {
                await FillCombo.RefreshByBranchandUser();
            }
            catch (Exception)
            { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_RefreshByBranchandUser"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async void loading_RefreshCategory()
        {
            try
            {
                await FillCombo.RefreshCategory();
            }
            catch (Exception)
            { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_RefreshCategory"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async void loading_RefreshUnit()
        {
            try
            {
                await FillCombo.RefreshUnit();
            }
            catch (Exception)
            { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_RefreshUnit"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async void loading_RefreshVendors()
        {
            try
            {
                await FillCombo.RefreshVendors();
            }
            catch (Exception)
            { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_RefreshVendors"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async void loading_RefreshCards()
        {
            try
            {
                await FillCombo.RefreshCards();
            }
            catch (Exception)
            { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_RefreshCards"))
                {
                    item.value = true;
                    break;
                }
            }
        }


        #endregion



        /*
        async void loading_getUserPath()
        {
            #region get user path
            try
            {
                UserSetValues uSetValueModel = new UserSetValues();
                List<UserSetValues> lst = await uSetValueModel.GetAll();

                SetValues setValueModel = new SetValues();

                List<SetValues> setVLst = await setValueModel.GetBySetName("user_path");
                if (setVLst.Count > 0)
                {
                    int firstId = setVLst[0].valId;
                    int secondId = setVLst[1].valId;
                    firstPath = lst.Where(u => u.valId == firstId && u.userId == userID).FirstOrDefault().note;
                    secondPath = lst.Where(u => u.valId == secondId && u.userId == userID).FirstOrDefault().note;
                }
                else
                {
                    firstPath = "";
                    secondPath = "";
                }
            }
            catch
            {
                firstPath = "";
                secondPath = "";
            }
            #endregion
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_getUserPath"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async void loading_getTax()
        {
            //get tax
            try
            {
                tax = decimal.Parse(await getDefaultTax());
            }
            catch
            {
                tax = 0;
            }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_getTax"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async void loading_getItemCost()
        {
            //get item cost
            try
            {
                itemCost = int.Parse(await getDefaultItemCost());
            }
            catch
            {
                itemCost = 0;
            }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_getItemCost"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async void loading_getPrintCount()
        {
            //get print count
            try
            {
                Allow_print_inv_count = await getDefaultPrintCount();
            }
            catch
            {
                Allow_print_inv_count = "1";
            }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_getPrintCount"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async void loading_getDateForm()
        {
            //get dateform
            try
            {
                dateFormat = await getDefaultDateForm();
            }
            catch
            {
                dateFormat = "ShortDatePattern";
            }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_getDateForm"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async void loading_getRegionAndCurrency()
        {
            //get region and currency
            try
            {
                CountryCode c = await getDefaultRegion();
                Region = c;
                Currency = c.currency;
                CurrencyId = c.currencyId;
                txt_cashSympol.Text = MainWindow.Currency;

            }
            catch
            {

            }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_getRegionAndCurrency"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async void loading_getStorageCost()
        {
            //get storage cost
            try
            {
                StorageCost = decimal.Parse(await getDefaultStorageCost());
            }
            catch
            {
                StorageCost = 0;
            }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_getStorageCost"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async void loading_getAccurac()
        {
            //get accuracy
            try
            {
                accuracy = await getDefaultAccuracy();
            }
            catch
            {
                accuracy = "1";
            }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_getAccurac"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        async void loading_getUserPersonalInfo()
        {
            #region user personal info
            txt_userName.Text = userLogin.name;
            txt_userJob.Text = userLogin.job;
            try
            {
                if (!string.IsNullOrEmpty(userLogin.image))
                {
                    byte[] imageBuffer = await userModel.downloadImage(userLogin.image); // read this as BLOB from your DB

                    var bitmapImage = new BitmapImage();

                    using (var memoryStream = new System.IO.MemoryStream(imageBuffer))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                    }

                    img_userLogin.Fill = new ImageBrush(bitmapImage);
                }
                else
                {
                    clearImg();
                }
            }
            catch
            {
                clearImg();
            }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_getUserPersonalInfo"))
                {
                    item.value = true;
                    break;
                }
            }
            #endregion
        }
        async void loading_getItemUnitsUsers()
        {
            try
            {
                itemUnitsUsers = await itemUnitsUser.GetByUserId(userLogin.userId);
            }
            catch (Exception)
            { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_getItemUnitsUsers"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        
        async void loading_getDefaultSystemInfo()
        {
            try
            {
                List<SettingCls> settingsCls = await setModel.GetAll();
                List<SetValues> settingsValues = await valueModel.GetAll();
                SettingCls set = new SettingCls();
                SetValues setV = new SetValues();
                List<char> charsToRemove = new List<char>() { '@', '_', ',', '.', '-' };
                #region get company name
                Thread t1 = new Thread(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        //get company name
                        set = settingsCls.Where(s => s.name == "com_name").FirstOrDefault<SettingCls>();
                        nameId = set.settingId;
                        setV = settingsValues.Where(i => i.settingId == nameId).FirstOrDefault();
                        if (setV != null)
                            companyName = setV.value;

                    });
                });
                t1.Start();
                #endregion

                #region  get company address
                Thread t2 = new Thread(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        //get company address
                        set = settingsCls.Where(s => s.name == "com_address").FirstOrDefault<SettingCls>();
                        addressId = set.settingId;
                        setV = settingsValues.Where(i => i.settingId == addressId).FirstOrDefault();
                        if (setV != null)
                            Address = setV.value;
                    });
                });
                t2.Start();
                #endregion

                #region  get company email
                Thread t3 = new Thread(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        //get company email
                        set = settingsCls.Where(s => s.name == "com_email").FirstOrDefault<SettingCls>();
                        emailId = set.settingId;
                        setV = settingsValues.Where(i => i.settingId == emailId).FirstOrDefault();
                        if (setV != null)
                            Email = setV.value;
                    });
                });
                t3.Start();
                #endregion

                #region  get company mobile
                Thread t4 = new Thread(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        //get company mobile
                        set = settingsCls.Where(s => s.name == "com_mobile").FirstOrDefault<SettingCls>();
                        mobileId = set.settingId;
                        setV = settingsValues.Where(i => i.settingId == mobileId).FirstOrDefault();
                        if (setV != null)
                        {
                            charsToRemove.ForEach(x => setV.value = setV.value.Replace(x.ToString(), String.Empty));
                            Mobile = setV.value;
                        }
                    });
                });
                t4.Start();
                #endregion

                #region  get company phone
                Thread t5 = new Thread(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        //get company phone
                        set = settingsCls.Where(s => s.name == "com_phone").FirstOrDefault<SettingCls>();
                        phoneId = set.settingId;
                        setV = settingsValues.Where(i => i.settingId == phoneId).FirstOrDefault();
                        if (setV != null)
                        {
                            charsToRemove.ForEach(x => setV.value = setV.value.Replace(x.ToString(), String.Empty));
                            Phone = setV.value;
                        }
                    });
                });
                t5.Start();
                #endregion

                #region  get company fax
                Thread t6 = new Thread(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        //get company fax
                        set = settingsCls.Where(s => s.name == "com_fax").FirstOrDefault<SettingCls>();
                        faxId = set.settingId;
                        setV = settingsValues.Where(i => i.settingId == faxId).FirstOrDefault();
                        if (setV != null)
                        {
                            charsToRemove.ForEach(x => setV.value = setV.value.Replace(x.ToString(), String.Empty));
                            Fax = setV.value;
                        }
                    });
                });
                t6.Start();
                #endregion

                #region   get company logo
                //get company logo
                set = settingsCls.Where(s => s.name == "com_logo").FirstOrDefault<SettingCls>();
                logoId = set.settingId;
                setV = settingsValues.Where(i => i.settingId == logoId).FirstOrDefault();
                if (setV != null)
                {
                    logoImage = setV.value;
                    await setV.getImg(logoImage);
                }
                #endregion
            }
            catch (Exception)
            { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_getDefaultSystemInfo"))
                {
                    item.value = true;
                    break;
                }
            }

        }
        async void loading_getprintSitting()
        {
            try
            {
                await getprintSitting();
            }
            catch (Exception)
            { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_getprintSitting"))
                {
                    item.value = true;
                    break;
                }
            }
        }
       
       
        async void loading_POSList()
        {
            try
            {
                posList = await posLogIn.Get();
            }
            catch (Exception)
            { }
            foreach (var item in loadingList)
            {
                if (item.key.Equals("loading_POSList"))
                {
                    item.value = true;
                    break;
                }
            }
        }
        */
        #endregion

        public async void Window_Loaded(object sender, RoutedEventArgs e)
        {//load
            try
            {
                    HelpClass.StartAwait(grid_mainWindow);
                #region bonni
#pragma warning disable CS0436 // Type conflicts with imported type
                TabTipAutomation.IgnoreHardwareKeyboard = HardwareKeyboardIgnoreOptions.IgnoreAll;
                #pragma warning restore CS0436 // Type conflicts with imported type
                #pragma warning disable CS0436 // Type conflicts with imported type
                #pragma warning restore CS0436 // Type conflicts with imported type
                #pragma warning disable CS0436 // Type conflicts with imported type
                TabTipAutomation.ExceptionCatched += TabTipAutomationOnTest;
                #pragma warning restore CS0436 // Type conflicts with imported type
                this.Height = SystemParameters.MaximizedPrimaryScreenHeight;
                //this.Width = SystemParameters.MaximizedPrimaryScreenHeight;
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += timer_Tick;
                timer.Start();

                // idle timer
                idletimer = new DispatcherTimer();
                idletimer.Interval = TimeSpan.FromMinutes(Idletime);
                idletimer.Tick += timer_Idle;
                idletimer.Start();


                //thread
                threadtimer = new DispatcherTimer();
                threadtimer.Interval = TimeSpan.FromSeconds(threadtime);
                threadtimer.Tick += timer_Thread;
                threadtimer.Start();




                #endregion
                translate();
                #region loading
                loadingList = new List<keyValueBool>();
                bool isDone = true;
                loadingList.Add(new keyValueBool { key = "loading_listObjects", value = false });
                loadingList.Add(new keyValueBool { key = "loading_getGroupObjects", value = false });
                loadingList.Add(new keyValueBool { key = "loading_globalItemUnitsList", value = false });
                loadingList.Add(new keyValueBool { key = "loading_RefreshBranches", value = false });
                loadingList.Add(new keyValueBool { key = "loading_RefreshBranchesAllWithoutMain", value = false });
                loadingList.Add(new keyValueBool { key = "loading_RefreshByBranchandUser", value = false });
                loadingList.Add(new keyValueBool { key = "loading_RefreshCategory", value = false });
                loadingList.Add(new keyValueBool { key = "loading_RefreshUnit", value = false });
                loadingList.Add(new keyValueBool { key = "loading_RefreshVendors", value = false });
                loadingList.Add(new keyValueBool { key = "loading_RefreshCards", value = false });

                //loadingList.Add(new keyValueBool { key = "loading_getUserPath", value = false });
                //loadingList.Add(new keyValueBool { key = "loading_getTax", value = false });
                //loadingList.Add(new keyValueBool { key = "loading_getItemCost", value = false });
                //loadingList.Add(new keyValueBool { key = "loading_getDateForm", value = false });
                //loadingList.Add(new keyValueBool { key = "loading_getRegionAndCurrency", value = false });
                //loadingList.Add(new keyValueBool { key = "loading_getStorageCost", value = false });
                //loadingList.Add(new keyValueBool { key = "loading_getAccurac", value = false });
                //loadingList.Add(new keyValueBool { key = "loading_getUserPersonalInfo", value = false });
                //loadingList.Add(new keyValueBool { key = "loading_getDefaultSystemInfo", value = false });
                //loadingList.Add(new keyValueBool { key = "loading_getItemUnitsUsers", value = false });
                //loadingList.Add(new keyValueBool { key = "loading_getprintSitting", value = false });
                //loadingList.Add(new keyValueBool { key = "loading_POSList", value = false });
                //loadingList.Add(new keyValueBool { key = "loading_getPrintCount", value = false });

                loading_listObjects();
                loading_getGroupObjects();
                loading_globalItemUnitsList();
                loading_RefreshBranches();
                loading_RefreshBranchesAllWithoutMain();
                loading_RefreshByBranchandUser();
                loading_RefreshCategory();
                loading_RefreshUnit();
                loading_RefreshVendors();
                loading_RefreshCards();
              await  FillCombo.getprintSitting();

                //loading_getUserPath();
                //loading_getTax();
                //loading_getItemCost();
                //loading_getDateForm();
                //loading_getRegionAndCurrency();
                //loading_getStorageCost();
                //loading_getAccurac();
                //loading_getItemUnitsUsers();
                //loading_getUserPersonalInfo();
                //loading_getDefaultSystemInfo();
                //loading_getprintSitting();
                //loading_POSList();
                //loading_getPrintCount();
                do
                {
                    isDone = true;
                    foreach (var item in loadingList)
                    {
                        if (item.value == false)
                        {
                            isDone = false;
                            break;
                        }
                    }
                    if (!isDone)
                    {
                        //MessageBox.Show("not done");
                        //string s = "";
                        //foreach (var item in loadingList)
                        //{
                        //    s += item.name + " - " + item.value + "\n";
                        //}
                        //MessageBox.Show(s);
                        await Task.Delay(0500);
                        //MessageBox.Show("do");
                    }
                }
                while (!isDone);
                #endregion

                #region notifications 
                //setNotifications();
                setTimer();
                #endregion

                permission();

                //SelectAllText
                EventManager.RegisterClassHandler(typeof(System.Windows.Controls.TextBox), System.Windows.Controls.TextBox.GotKeyboardFocusEvent, new RoutedEventHandler(SelectAllText));

                //lang = "ar";

                //SetNotificationsLocation();

                if (sender != null)
                    HelpClass.EndAwait(grid_mainWindow);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_mainWindow);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        //public void SetNotificationsLocation()
        //{
        //    #region notifications location
        //    //Point position = BTN_notifications.PointToScreen(new Point(0d, 0d)),
        //    //controlPosition = this.PointToScreen(new Point(0d, 0d));
        //    //position.X -= controlPosition.X;
        //    //position.Y -= controlPosition.Y;
        //    //position.X -= 100;
        //    //bdrMain.Margin = new Thickness(0, 70, position.X, 0);
        //    #endregion
        //    #region notifications location
        //    /*
        //   Point positionBtnMinimize = BTN_Minimize.PointToScreen(new Point(0d, 0d)),
        //   positionBtnUserImage = btn_userImage.PointToScreen(new Point(0d, 0d)),
        //   controlPositionBtnMinimize = this.PointToScreen(new Point(0d, 0d)),
        //   controlPositionBtnUserImage = this.PointToScreen(new Point(0d, 0d));
        //   positionBtnMinimize.X -= controlPositionBtnMinimize.X;
        //   positionBtnUserImage.X -= controlPositionBtnUserImage.X;
        //   Double position;
        //   if (positionBtnMinimize.X > positionBtnUserImage.X)
        //   position = positionBtnMinimize.X - positionBtnUserImage.X;
        //   else 
        //   position =   positionBtnUserImage.X - positionBtnMinimize.X;
        //   var thickness = bdrMain.Margin;
        //   bdrMain.Margin = new Thickness(0, 70, thickness.Right + position - 25, 0);
        //   //if(lang.Equals("en"))
        //   //bdrMain.Margin = new Thickness(0, 70, thickness.Right + position - 5 , 0);
        //   //else
        //   //  bdrMain.Margin = new Thickness(0, 70, thickness.Right + position - 10, 0);
        //   */
        //    #endregion
        //    #region notifications location
        //    //Point position = BTN_notifications.PointToScreen(new Point(0d, 0d)),
        //    //controlPosition = this.PointToScreen(new Point(0d, 0d));
        //    //position.X -= controlPosition.X;
        //    //Canvas.SetTop(bdrMain, position.X);
        //    ////bdrMain.Margin = new Thickness(0, 70, position.X, 0);
        //    #endregion
        //    #region notifications location
        //    var thickness = bdrMain.Margin;
        //    bdrMain.Margin = new Thickness(0, 70, thickness.Right + sp_userName.ActualWidth, 0);
        //    #endregion
        //}
        void SelectAllText(object sender, RoutedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            if (textBox != null)
                if (!textBox.IsReadOnly)
                    textBox.SelectAll();
        }
        /*
        public static bool loadingDefaultPath(string first, string second)
        {
            bool load = false;
            if (!string.IsNullOrEmpty(first) && !string.IsNullOrEmpty(second))
            {
                foreach (Button button in FindControls.FindVisualChildren<Button>(MainWindow.mainWindow))
                {
                    if (button.Tag != null)
                        if (button.Tag.ToString() == first && MainWindow.groupObject.HasPermission(first, MainWindow.groupObjects))
                        {
                            button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            load = true;
                            break;
                        }
                }

                if (first == "home")
                    loadingSecondLevel(second, uc_home.Instance);
                if (first == "catalog")
                    loadingSecondLevel(second, UC_catalog.Instance);
                if (first == "storage")
                    loadingSecondLevel(second, Restaurant.View.uc_storage.Instance);
                if (first == "purchase")
                    loadingSecondLevel(second, uc_purchases.Instance);
                if (first == "sales")
                    loadingSecondLevel(second, uc_sales.Instance);
                if (first == "accounts")
                    loadingSecondLevel(second, uc_accounts.Instance);
                if (first == "reports")
                    loadingSecondLevel(second, uc_reports.Instance);
                if (first == "sectionData")
                    loadingSecondLevel(second, UC_SectionData.Instance);
                if (first == "settings")
                    loadingSecondLevel(second, uc_settings.Instance);

            }
            return load;
        }
        */
        static void loadingSecondLevel(string second, UserControl userControl)
        {
            userControl.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
            var button = userControl.FindName("btn_" + second) as Button;
            button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        void permission()
        {
            /*
            bool loadWindow = false;
            loadWindow = loadingDefaultPath(firstPath, secondPath);
            if (!HelpClass.isAdminPermision())
                foreach (Button button in FindControls.FindVisualChildren<Button>(this))
                {
                    if (button.Tag != null)
                        if (MainWindow.groupObject.HasPermission(button.Tag.ToString(), MainWindow.groupObjects))
                        {
                            button.Visibility = Visibility.Visible;
                            if (!loadWindow)
                            {
                                button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                loadWindow = true;
                            }
                        }
                        else button.Visibility = Visibility.Collapsed;
                }
            else
                if (!loadWindow)
                BTN_Home_Click(BTN_home, null);
            */
        }
        #region notifications
        private void setTimer()
        {
            notTimer = new DispatcherTimer();
            notTimer.Interval = TimeSpan.FromSeconds(30);
            notTimer.Tick += notTimer_Tick;
            notTimer.Start();
        }
        private void notTimer_Tick(object sendert, EventArgs et)
        {
            try
            {
                //setNotifications();
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }



        }
        /*
        private async void setNotifications()
        {
            try
            {
                await refreshNotificationCount();
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async Task refreshNotificationCount()
        {
            int notCount = await notificationUser.GetCountByUserId(userID.Value, "alert", posID.Value);

            int previouseCount = 0;
            if (md_notificationCount.Badge != null && md_notificationCount.Badge.ToString() != "") previouseCount = int.Parse(md_notificationCount.Badge.ToString());

            if (notCount != previouseCount)
            {
                if (notCount > 9)
                {
                    notCount = 9;
                    md_notificationCount.Badge = "+" + notCount.ToString();
                }
                else if (notCount == 0) md_notificationCount.Badge = "";
                else
                    md_notificationCount.Badge = notCount.ToString();
            }
        }
        */
        #endregion
        void timer_Idle(object sender, EventArgs e)
        {

            try
            {
                if (IdleClass.IdleTime.Minutes >= Idletime)
                {
                    BTN_logOut_Click(null, null);
                    idletimer.Stop();
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }

        }
        async void timer_Thread(object sendert, EventArgs et)
        {
            try
            {
                //  User thruser = new User();
                //UsersLogs thrlog = new UsersLogs();

                //thrlog = await thrlog.GetByID((int)userLogInID);
                // check go_out == true do logout()
                //if (thrlog.sOutDate != null)
                if (go_out)
                {
                    BTN_logOut_Click(null, null);
                    threadtimer.Stop();
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }


            //try
            //{
            //    posLogIn = await posLogIn.getById(posID.Value);
            //    txt_cashValue.Text = HelpClass.DecTostring(posLogIn.balance);
            //    txt_cashSympol.Text = MainWindow.Currency;
            //}
            //catch (Exception ex)
            //{
            //    HelpClass.ExceptionMessage(ex, this);
            //}


        }
        void timer_Tick(object sender, EventArgs e)
        {
            try
            {

                txtTime.Text = DateTime.Now.ToShortTimeString();
                txtDate.Text = DateTime.Now.ToShortDateString();


            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void TabTipAutomationOnTest(Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
        //private static List<string> QueryWmiKeyboards()
        //{
        //    using (var searcher = new ManagementObjectSearcher(new SelectQuery("Win32_Keyboard")))
        //    using (var result = searcher.Get())
        //    {
        //        return result
        //            .Cast<ManagementBaseObject>()
        //            .SelectMany(keyboard =>
        //                keyboard.Properties
        //                    .Cast<PropertyData>()
        //                    .Where(k => k.Name == "Description")
        //                    .Select(k => k.Value as string))
        //            .ToList();
        //    }
        //}
        void FN_tooltipVisibility(Button btn)
        {
            ToolTip T = btn.ToolTip as ToolTip;
            if (T.Visibility == Visibility.Visible)
                T.Visibility = Visibility.Hidden;
            else T.Visibility = Visibility.Visible;
        }
        private async void BTN_logOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_mainWindow);

                await close();

                Application.Current.Shutdown();
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);

                if (sender != null)
                    HelpClass.EndAwait(grid_mainWindow);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_mainWindow);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        async Task close()
        {
            //log out
            //update lognin record
            if (!go_out)
            {
                //await updateLogninRecord();
            }
            timer.Stop();
            idletimer.Stop();
            threadtimer.Stop();
        }
        private async void BTN_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_mainWindow);
                await close();

                Application.Current.Shutdown();

                if (sender != null)
                    HelpClass.EndAwait(grid_mainWindow);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_mainWindow);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void BTN_Minimize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.WindowState = System.Windows.WindowState.Minimized;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        void colorTextRefreash(TextBlock txt)
        {
            txt_home.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            txt_catalog.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            txt_storage.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            txt_purchases.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            txt_sales.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            txt_kitchen.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            txt_delivery.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            txt_accounts.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            txt_reports.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            txt_sectiondata.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            txt_settings.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));

            txt.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
        }
        void fn_ColorIconRefreash(Path p)
        {
            path_iconSettings.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            path_iconSectionData.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            path_iconReports.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            path_iconAccounts.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            path_iconSales.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            path_iconKitchen.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            path_iconDelivery.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            path_iconPurchases.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            path_iconStorage.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            path_iconCatalog.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));
            path_iconHome.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FEDFB7"));

            p.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
        }
        public void translate()
        {
            tt_menu.Content = resourcemanager.GetString("trMenu");
            tt_home.Content = resourcemanager.GetString("trHome");
            txt_home.Text = resourcemanager.GetString("trHome");
            tt_catalog.Content = resourcemanager.GetString("trCatalog");
            txt_catalog.Text = resourcemanager.GetString("trCatalog");
            tt_storage.Content = resourcemanager.GetString("trStore");
            txt_storage.Text = resourcemanager.GetString("trStore");
            tt_purchase.Content = resourcemanager.GetString("trPurchases");
            txt_purchases.Text = resourcemanager.GetString("trPurchases");
            tt_sales.Content = resourcemanager.GetString("trSales");
            txt_sales.Text = resourcemanager.GetString("trSales");
            tt_accounts.Content = resourcemanager.GetString("trAccounting");
            txt_accounts.Text = resourcemanager.GetString("trAccounting");
            tt_reports.Content = resourcemanager.GetString("trReports");
            txt_reports.Text = resourcemanager.GetString("trReports");
            tt_sectionData.Content = resourcemanager.GetString("trSectionData");
            txt_sectiondata.Text = resourcemanager.GetString("trSectionData");
            tt_settings.Content = resourcemanager.GetString("trSettings");
            txt_settings.Text = resourcemanager.GetString("trSettings");
            txt_cashTitle.Text = resourcemanager.GetString("trBalance");

            mi_changePassword.Header = resourcemanager.GetString("trChangePassword");
            BTN_logOut.Header = resourcemanager.GetString("trLogOut");

            txt_notifications.Text = resourcemanager.GetString("trNotifications");
            txt_noNoti.Text = resourcemanager.GetString("trNoNotifications");
            btn_showAll.Content = resourcemanager.GetString("trShowAll");
        }

        //فتح
        private void BTN_Menu_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (!menuState)
                {
                    Storyboard sb = this.FindResource("Storyboard1") as Storyboard;
                    sb.Begin();
                    menuState = true;
                }
                else
                {
                    Storyboard sb = this.FindResource("Storyboard2") as Storyboard;
                    sb.Begin();
                    menuState = false;
                }


                #region tooltipVisibility
                FN_tooltipVisibility(BTN_menu);
                FN_tooltipVisibility(btn_home);
                FN_tooltipVisibility(btn_catalog);
                FN_tooltipVisibility(btn_storage);
                FN_tooltipVisibility(btn_purchase);
                FN_tooltipVisibility(btn_sales);
                FN_tooltipVisibility(btn_reports);
                FN_tooltipVisibility(btn_accounts);
                FN_tooltipVisibility(btn_sectionData);
                FN_tooltipVisibility(btn_settings);
                #endregion


            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        void fn_pathOpenCollapsed()
        {
            path_openCatalog.Visibility = Visibility.Collapsed;
            path_openStorage.Visibility = Visibility.Collapsed;
            path_openPurchases.Visibility = Visibility.Collapsed;
            path_openSales.Visibility = Visibility.Collapsed;
            path_openReports.Visibility = Visibility.Collapsed;
            path_openSectionData.Visibility = Visibility.Collapsed;
            path_openSettings.Visibility = Visibility.Collapsed;
            path_openHome.Visibility = Visibility.Collapsed;
            path_openAccounts.Visibility = Visibility.Collapsed;
            path_openKitchen.Visibility = Visibility.Collapsed;
            path_openDelivery.Visibility = Visibility.Collapsed;

        }
        void FN_pathVisible(Rectangle p)
        {
            fn_pathOpenCollapsed();
            p.Visibility = Visibility.Visible;
        }




        private void btn_Keyboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (TabTip.Close())
                {
            #pragma warning disable CS0436 // Type conflicts with imported type
                    TabTip.OpenUndockedAndStartPoolingForClosedEvent();
            #pragma warning restore CS0436 // Type conflicts with imported type
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }

        }
        /*
        User userModel = new User();
        UsersLogs userLogsModel = new UsersLogs();
        async Task<bool> updateLogninRecord()
        {
            //update lognin record
            UsersLogs userLog = new UsersLogs();
            userLog = await userLogsModel.GetByID(userLogInID.Value);

            await userLogsModel.Save(userLog);

            //update user record
            userLogin.isOnline = 0;
            await userModel.save(userLogin);


            return true;
        }
        */
        
        private void Btn_home_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                colorTextRefreash(txt_home);
                FN_pathVisible(path_openHome);
                fn_ColorIconRefreash(path_iconHome);
                grid_main.Children.Clear();
                //grid_main.Children.Add(uc_home.Instance);
                //if (isHome)
                //{
                //    uc_home.Instance.timerAnimation();
                //    isHome = false;
                //}
                Button button = sender as Button;
                MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }

        }
        private void Btn_catalog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                colorTextRefreash(txt_catalog);
                FN_pathVisible(path_openCatalog);
                fn_ColorIconRefreash(path_iconCatalog);
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_catalog.Instance);

                isHome = true;
                Button button = sender as Button;
                MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_purchase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                colorTextRefreash(txt_purchases);
                FN_pathVisible(path_openPurchases);
                fn_ColorIconRefreash(path_iconPurchases);
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_purchase.Instance);

                isHome = true;
                Button button = sender as Button;
                MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_storage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                colorTextRefreash(txt_storage);
                FN_pathVisible(path_openStorage);
                fn_ColorIconRefreash(path_iconStorage);
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_storage.Instance);

                isHome = true;
                Button button = sender as Button;
                MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }

        }
        private void Btn_kitchen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                colorTextRefreash(txt_kitchen);
                FN_pathVisible(path_openKitchen);
                fn_ColorIconRefreash(path_iconKitchen);
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_kitchen.Instance);

                isHome = true;
                Button button = sender as Button;
                MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_sales_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                colorTextRefreash(txt_sales);
                FN_pathVisible(path_openSales);
                fn_ColorIconRefreash(path_iconSales);
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_sales.Instance);

                isHome = true;
                Button button = sender as Button;
                MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_delivery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                colorTextRefreash(txt_delivery);
                FN_pathVisible(path_openDelivery);
                fn_ColorIconRefreash(path_iconDelivery);
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_delivery.Instance);

                isHome = true;
                Button button = sender as Button;
                MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_accounts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                colorTextRefreash(txt_accounts);
                FN_pathVisible(path_openAccounts);
                fn_ColorIconRefreash(path_iconAccounts);
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_accounts.Instance);

                isHome = true;
                Button button = sender as Button;
                MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_reports_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                colorTextRefreash(txt_reports);
                FN_pathVisible(path_openReports);
                fn_ColorIconRefreash(path_iconReports);
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_reports.Instance);

                isHome = true;
                Button button = sender as Button;
                MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_settings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                colorTextRefreash(txt_settings);
                FN_pathVisible(path_openSettings);
                fn_ColorIconRefreash(path_iconSettings);
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_settings.Instance);

                isHome = true;
                Button button = sender as Button;
                MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Btn_SectionData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                colorTextRefreash(txt_sectiondata);
                FN_pathVisible(path_openSectionData);
                fn_ColorIconRefreash(path_iconSectionData);
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_sectionData.Instance);

                isHome = true;
                Button button = sender as Button;
                initializationMainTrack(button.Tag.ToString());
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        /*
        
        private void BTN_catalog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                colorTextRefreash(txt_catalog);
                FN_pathVisible(path_openCatalog);
                fn_ColorIconRefreash(path_iconCatalog);
                grid_main.Children.Clear();
                grid_main.Children.Add(UC_catalog.Instance);
                isHome = true;
                Button button = sender as Button;
                initializationMainTrack(button.Tag.ToString(), 0);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        public void BTN_purchases_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                colorTextRefreash(txt_purchases);
                FN_pathVisible(path_openPurchases);
                fn_ColorIconRefreash(path_iconPurchases);
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_purchases.Instance);
                isHome = true;
                Button button = sender as Button;
                initializationMainTrack(button.Tag.ToString(), 0);

            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        public void BTN_sales_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                colorTextRefreash(txt_sales);
                FN_pathVisible(path_openSales);
                fn_ColorIconRefreash(path_iconSales);
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_sales.Instance);
                isHome = true;
                Button button = sender as Button;
                initializationMainTrack(button.Tag.ToString(), 0);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void BTN_accounts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                colorTextRefreash(txt_accounting);
                FN_pathVisible(path_openAccount);
                fn_ColorIconRefreash(path_iconAccounts);
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_accounts.Instance);
                isHome = true;
                Button button = sender as Button;
                initializationMainTrack(button.Tag.ToString(), 0);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void BTN_reports_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                colorTextRefreash(txt_reports);
                FN_pathVisible(path_openReports);
                fn_ColorIconRefreash(path_iconReports);
                isHome = true;
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_reports.Instance);
                Button button = sender as Button;
                initializationMainTrack(button.Tag.ToString(), 0);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        public void BTN_settings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                colorTextRefreash(txt_settings);
                FN_pathVisible(path_openSettings);
                fn_ColorIconRefreash(path_iconSettings);
                isHome = true;
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_settings.Instance);
                Button button = sender as Button;
                initializationMainTrack(button.Tag.ToString(), 0);
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void BTN_storage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;
                initializationMainTrack(button.Tag.ToString(), 0);
                colorTextRefreash(txt_storage);
                FN_pathVisible(path_openStorage);
                fn_ColorIconRefreash(path_iconStorage);
                grid_main.Children.Clear();
                grid_main.Children.Add(View.uc_storage.Instance);
                isHome = true;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        */
      
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_mainWindow);
                await close();
                Application.Current.Shutdown();

                if (sender != null)
                    HelpClass.EndAwait(grid_mainWindow);
            }
            catch (Exception ex)
            {
                if (sender != null)
                    HelpClass.EndAwait(grid_mainWindow);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        private async void Mi_changePassword_Click(object sender, RoutedEventArgs e)
        {//change password
            try
            {

                //Window.GetWindow(this).Opacity = 0.2;
                //wd_changePassword w = new wd_changePassword();
                //w.ShowDialog();
                //Window.GetWindow(this).Opacity = 1;

                //userLogin = await userModel.getUserById(w.userID);

            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        /*
        SetValues v = new SetValues();
        async Task<string> getDefaultStorageCost()
        {
            v = await uc_general.getDefaultCost();
            if (v != null)
                return v.value;
            else
                return "";
        }
        async Task<string> getDefaultLanguage()
        {
            UserSetValues v = await uc_general.getDefaultLanguage();
            SetValues sVModel = new SetValues();
            SetValues sValue = new SetValues();
            sValue = await sVModel.GetByID(v.valId.Value);
            if (sValue != null)
                return sValue.value;
            else
                return "";
        }
        async Task<string> getDefaultTax()
        {
            v = await uc_general.getDefaultTax();
            if (v != null)
                return v.value;
            else
                return "";
        }
        async Task<string> getDefaultItemCost()
        {
            v = await uc_general.getDefaultItemCost();
            if (v != null)
                return v.value;
            else
                return "";
        }
        async Task<string> getDefaultPrintCount()
        {
            v = await uc_general.getDefaultPrintCount();
            if (v != null)
                return v.value;
            else
                return "";
        }
        async Task<string> getDefaultAccuracy()
        {
            v = await uc_general.getDefaultAccuracy();
            if (v != null)
                return v.value;
            else
                return "";
        }
        async Task<string> getDefaultDateForm()
        {
            v = await uc_general.getDefaultDateForm();
            if (v != null)
                return v.value;
            else
                return "";
        }
        async Task<CountryCode> getDefaultRegion()
        {
            CountryCode c = await uc_general.getDefaultRegion();
            if (c != null)
                return c;
            else
                return null;
        }
        */
        private void Mi_more_Click(object sender, RoutedEventArgs e)
        {

        }
        public static string GetUntilOrEmpty(string text, string stopAt)
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return String.Empty;
        }
        #region  Notification
        List<NotificationUser> notifications;
        NotificationUser notificationUser = new NotificationUser();
        private async void BTN_notifications_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (bdrMain.Visibility == Visibility.Collapsed)
                {
                    bdrMain.Visibility = Visibility.Visible;
                    bdrMain.RenderTransform = Animations.borderAnimation(-25, bdrMain, true);
                    notifications = await notificationUser.GetByUserId(userLogin.userId, "alert", posLogin.posId);
                    IEnumerable<NotificationUser> orderdNotifications = notifications.OrderByDescending(x => x.createDate);
                    await notificationUser.setAsRead(userLogin.userId, posLogin.posId, "alert");
                    md_notificationCount.Badge = "";
                    if (notifications.Count == 0)
                    {
                        grd_notifications.Visibility = Visibility.Collapsed;
                        txt_noNoti.Visibility = Visibility.Visible;
                    }

                    else
                    {
                        grd_notifications.Visibility = Visibility.Visible;
                        txt_noNoti.Visibility = Visibility.Collapsed;

                        txt_firstNotiTitle.Text = resourcemanager.GetString(orderdNotifications.Select(obj => obj.title).FirstOrDefault());

                        txt_firstNotiContent.Text = GetUntilOrEmpty(orderdNotifications.Select(obj => obj.ncontent).FirstOrDefault(), ":")
                          + " : " +
                          resourcemanager.GetString(orderdNotifications.Select(obj => obj.ncontent).FirstOrDefault().Substring(orderdNotifications.Select(obj => obj.ncontent).FirstOrDefault().LastIndexOf(':') + 1));

                        txt_firstNotiDate.Text = orderdNotifications.Select(obj => obj.createDate).FirstOrDefault().ToString();

                        if (notifications.Count > 1)
                        {
                            txt_2NotiTitle.Text = resourcemanager.GetString(orderdNotifications.Select(obj => obj.title).Skip(1).FirstOrDefault());
                            txt_2NotiContent.Text = GetUntilOrEmpty(orderdNotifications.Select(obj => obj.ncontent).Skip(1).FirstOrDefault(), ":")
                          + " : " + resourcemanager.GetString(orderdNotifications.Select(obj => obj.ncontent).Skip(1).FirstOrDefault().Substring(orderdNotifications.Select(obj => obj.ncontent).Skip(1).FirstOrDefault().LastIndexOf(':') + 1));

                            txt_2NotiDate.Text = orderdNotifications.Select(obj => obj.createDate).Skip(1).FirstOrDefault().ToString();

                        }
                        if (notifications.Count > 2)
                        {
                            txt_3NotiTitle.Text = resourcemanager.GetString(orderdNotifications.Select(obj => obj.title).Skip(2).FirstOrDefault());
                            txt_3NotiContent.Text = GetUntilOrEmpty(orderdNotifications.Select(obj => obj.ncontent).Skip(2).FirstOrDefault(), ":")
                          + " : " + resourcemanager.GetString(orderdNotifications.Select(obj => obj.ncontent).Skip(2).FirstOrDefault().Substring(orderdNotifications.Select(obj => obj.ncontent).Skip(2).FirstOrDefault().LastIndexOf(':') + 1));

                            txt_3NotiDate.Text = orderdNotifications.Select(obj => obj.createDate).Skip(2).FirstOrDefault().ToString();

                        }
                        if (notifications.Count > 3)
                        {
                            txt_4NotiTitle.Text = resourcemanager.GetString(orderdNotifications.Select(obj => obj.title).Skip(3).FirstOrDefault());
                            txt_4NotiContent.Text = GetUntilOrEmpty(orderdNotifications.Select(obj => obj.ncontent).Skip(3).FirstOrDefault(), ":")
                          + " : " + resourcemanager.GetString(orderdNotifications.Select(obj => obj.ncontent).Skip(3).FirstOrDefault().Substring(orderdNotifications.Select(obj => obj.ncontent).Skip(3).FirstOrDefault().LastIndexOf(':') + 1));

                            txt_4NotiDate.Text = orderdNotifications.Select(obj => obj.createDate).Skip(3).FirstOrDefault().ToString();

                        }
                        if (notifications.Count > 4)
                        {
                            txt_5NotiTitle.Text = resourcemanager.GetString(orderdNotifications.Select(obj => obj.title).Skip(4).FirstOrDefault());
                            txt_5NotiContent.Text = GetUntilOrEmpty(orderdNotifications.Select(obj => obj.ncontent).Skip(4).FirstOrDefault(), ":")
                          + " : " + resourcemanager.GetString(orderdNotifications.Select(obj => obj.ncontent).Skip(4).FirstOrDefault().Substring(orderdNotifications.Select(obj => obj.ncontent).Skip(4).FirstOrDefault().LastIndexOf(':') + 1));

                            txt_5NotiDate.Text = orderdNotifications.Select(obj => obj.createDate).Skip(4).FirstOrDefault().ToString();

                        }
                    }

                }
                else
                {
                    bdrMain.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Btn_showAll_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Opacity = 0.2;
            wd_notifications w = new wd_notifications();
            w.notifications = notifications;
            w.ShowDialog();
            Window.GetWindow(this).Opacity = 1;
        }
        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {

                bdr_showAll.Visibility = Visibility.Visible;

            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {

                bdr_showAll.Visibility = Visibility.Hidden;

            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                bdrMain.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        #endregion

        

        private void Btn_info_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Window.GetWindow(this).Opacity = 0.2;
                //wd_info w = new wd_info();
                //w.ShowDialog();
                //Window.GetWindow(this).Opacity = 1;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
       
        private void Btn_userImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Window.GetWindow(this).Opacity = 0.2;
                //wd_userInfo w = new wd_userInfo();
                //w.ShowDialog();
                //Window.GetWindow(this).Opacity = 1;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private void clearImg()
        {
            Uri resourceUri = new Uri("pic/no-image-icon-90x90.png", UriKind.Relative);
            StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);

            BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
            myBrush.ImageSource = temp;
            img_userLogin.Fill = myBrush;

        }
        #region Main Path
        public void initializationMainTrack(string tag)
        {
            //sp_mainPath
            sp_mainPath.Children.Clear();
            List<Object> _listObjects = new List<Object>();
            _listObjects = objectModel.GetParents(listObjects, tag);
            int counter = 1;
            bool isLast = false;
            foreach (var item in _listObjects)
            {
                if (counter == _listObjects.Count)
                    isLast = true;
                else
                    isLast = false;
                sp_mainPath.Children.Add(initializationMainButton(item, isLast));
                counter++;
            }
        }
        Button initializationMainButton(Object _object, bool isLast)
        {
            Button button = new Button();
            button.Content = ">" + MainWindow.resourcemanager.GetString(_object.translate);
            button.Tag = _object.name;
            button.Click += MainButton_Click;
            button.Background = null;
            button.Margin = new Thickness(5, 0, 0, 0);
            button.BorderThickness = new Thickness(0);
            button.Padding = new Thickness(0);
            button.FontSize = 16;
            if (isLast)
                button.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
            else
                button.Foreground = Application.Current.Resources["Grey"] as SolidColorBrush;
            return button;
        }
        void MainButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            initializationMainTrack(button.Tag.ToString());
            loadPath(button.Tag.ToString());

        }
        void loadPath(string tag)
        {
            grid_main.Children.Clear();
            switch (tag)
            {
                //2
                //case "home":
                //    grid_main.Children.Add(uc_home.Instance);
                //    break;
                case "catalog":
                    grid_main.Children.Add(uc_catalog.Instance);
                    break;
                case "purchase":
                    grid_main.Children.Add(uc_purchase.Instance);
                    break;
                case "storage":
                    grid_main.Children.Add(uc_storage.Instance);
                    break;
                case "kitchen":
                    grid_main.Children.Add(uc_kitchen.Instance);
                    break;
                case "sales":
                    grid_main.Children.Add(uc_sales.Instance);
                    break;
                case "delivery":
                    grid_main.Children.Add(uc_delivery.Instance);
                    break;
                case "accounts":
                    grid_main.Children.Add(uc_accounts.Instance);
                    break;
                case "reports":
                    grid_main.Children.Add(uc_reports.Instance);
                    break;
                case "sectionData":
                    grid_main.Children.Add(uc_sectionData.Instance);
                    break;
                //12
                case "settings":
                    grid_main.Children.Add(uc_settings.Instance);
                    break;
                case "hallDivide":
                    grid_main.Children.Add(uc_hallDivide.Instance);
                    break;
                case "persons":
                    grid_main.Children.Add(uc_persons.Instance);
                    break;
                case "branchesAndStores":
                    grid_main.Children.Add(uc_branchesAndStores.Instance);
                    break;
                case "banksData":
                    grid_main.Children.Add(uc_banksData.Instance);
                    break;
                case "tables":
                    grid_main.Children.Add(uc_tables.Instance);
                    break;
                case "hallSections":
                    grid_main.Children.Add(uc_hallSections.Instance);
                    break;
                case "vendors":
                    grid_main.Children.Add(uc_vendors.Instance);
                    break;
                case "customers":
                    grid_main.Children.Add(uc_customers.Instance);
                    break;
                case "users":
                    grid_main.Children.Add(uc_users.Instance);
                    break;
                //22
                case "branches":
                    grid_main.Children.Add(uc_branches.Instance);
                    break;
                case "stores":
                    grid_main.Children.Add(uc_stores.Instance);
                    break;
                case "pos":
                    grid_main.Children.Add(uc_pos.Instance);
                    break;
                case "banks":
                    grid_main.Children.Add(uc_banks.Instance);
                    break;
                case "cards":
                    grid_main.Children.Add(uc_cards.Instance);
                    break;
                case "rawMaterials":
                    grid_main.Children.Add(uc_rawMaterials.Instance);
                    break;
                case "foods":
                    grid_main.Children.Add(uc_foods.Instance);
                    break;
                case "appetizers":
                    grid_main.Children.Add(uc_itemsFoods.Instance);
                    uc_itemsFoods.categoryName = "appetizers";
                    break;
                case "beverages":
                    grid_main.Children.Add(uc_itemsFoods.Instance);
                    uc_itemsFoods.categoryName = "beverages";
                    break;
                case "fastFood":
                    grid_main.Children.Add(uc_itemsFoods.Instance);
                    uc_itemsFoods.categoryName = "fastFood";
                    break;
                    //32
                case "mainCourses":
                    grid_main.Children.Add(uc_itemsFoods.Instance);
                    uc_itemsFoods.categoryName = "mainCourses";
                    break;
                case "desserts":
                    grid_main.Children.Add(uc_itemsFoods.Instance);
                    uc_itemsFoods.categoryName = "desserts";
                    break;
                case "payInvoice":
                    grid_main.Children.Add(uc_payInvoice.Instance);
                    break;
                case "purchaseOrder":
                    grid_main.Children.Add(uc_purchaseOrder.Instance);
                    break;
                //case "purchaseStatistic":
                //    grid_main.Children.Add(uc_purchaseStatistic.Instance);
                //    break;
                case "storageDivide":
                    grid_main.Children.Add(uc_storageDivide.Instance);
                    break;
                case "storageOperations":
                    grid_main.Children.Add(uc_storageOperations.Instance);
                    break;
                case "movementsOperations":
                    grid_main.Children.Add(uc_movementsOperations.Instance);
                    break;
                case "stocktakingOperations":
                    grid_main.Children.Add(uc_stocktakingOperations.Instance);
                    break;
                case "locations":
                    grid_main.Children.Add(uc_locations.Instance);
                    break;
                //42
                case "storageSections":
                    grid_main.Children.Add(uc_storageSections.Instance);
                    break;
                case "storageCost":
                    grid_main.Children.Add(uc_storageCost.Instance);
                    break;
                case "storageInvoice":
                    grid_main.Children.Add(uc_storageInvoice.Instance);
                    break;
                case "itemsStorage":
                    grid_main.Children.Add(uc_itemsStorage.Instance);
                    break;
                case "storageMovements":
                    grid_main.Children.Add(uc_storageMovements.Instance);
                    break;
                case "spendingOrder":
                    grid_main.Children.Add(uc_spendingOrder.Instance);
                    break;
                case "itemsShortage":
                    grid_main.Children.Add(uc_itemsShortage.Instance);
                    break;
                case "itemsDestructive":
                    grid_main.Children.Add(uc_itemsDestructive.Instance);
                    break;
                case "stocktaking":
                    grid_main.Children.Add(uc_stocktaking.Instance);
                    break;
                case "preparingOrders":
                    grid_main.Children.Add(uc_preparingOrders.Instance);
                    break;
                //52
                case "spendingRequest":
                    grid_main.Children.Add(uc_spendingRequest.Instance);
                    break;
                case "posTransfers":
                    grid_main.Children.Add(uc_posTransfers.Instance);
                    break;
                case "payments":
                    grid_main.Children.Add(uc_payments.Instance);
                    break;
                case "received":
                    grid_main.Children.Add(uc_received.Instance);
                    break;
                case "banksAccounting":
                    grid_main.Children.Add(uc_banksAccounting.Instance);
                    break;
                //case "accountsStatistic":
                //    grid_main.Children.Add(uc_accountsStatistic.Instance);
                //    break;
                case "subscriptions":
                    grid_main.Children.Add(uc_subscriptions.Instance);
                    break;
                case "ordersAccounting":
                    grid_main.Children.Add(uc_ordersAccounting.Instance);
                    break;
                //case "general":
                //    grid_main.Children.Add(uc_general.Instance);
                //    break;
                case "reportsSettings":
                    grid_main.Children.Add(uc_reportsSettings.Instance);
                    break;
                //62
                //case "permissions":
                //    grid_main.Children.Add(uc_permissions.Instance);
                //    break;
                //case "emailSettings":
                //    grid_main.Children.Add(uc_emailSettings.Instance);
                //    break;
                //case "smsSettings":
                //    grid_main.Children.Add(uc_smsSettings.Instance);
                //    break;
                case "promotion":
                    grid_main.Children.Add(uc_promotion.Instance);
                    break;
                case "reservations":
                    grid_main.Children.Add(uc_reservations.Instance);
                    break;
                case "diningHall":
                    grid_main.Children.Add(uc_diningHall.Instance);
                    break;
                //case "takeAway":
                //    grid_main.Children.Add(uc_takeAway.Instance);
                //    break;
                //case "salesStatistic":
                //    grid_main.Children.Add(uc_salesStatistic.Instance);
                //    break;
                case "membership":
                    grid_main.Children.Add(uc_membership.Instance);
                    break;
                case "coupon":
                    grid_main.Children.Add(uc_coupon.Instance);
                    break;
                //72
                case "offer":
                    grid_main.Children.Add(uc_offer.Instance);
                    break;
                //case "quotation":
                //    grid_main.Children.Add(uc_quotation.Instance);
                //    break;
                //case "medals":
                //    grid_main.Children.Add(uc_medals.Instance);
                //    break;

                //75  package
                case "package":
                    grid_main.Children.Add(uc_package.Instance);
                    break;
                //76  deliveryManagement
                case "deliveryManagement":
                    grid_main.Children.Add(uc_deliveryManagement.Instance);
                    break;
                //77  shippingCompanies
                case "shippingCompanies":
                    grid_main.Children.Add(uc_shippingCompanies.Instance);
                    break;
                //78  itemsRawMaterials
                case "itemsRawMaterials":
                    grid_main.Children.Add(uc_itemsRawMaterials.Instance);
                    break;
                //79  units
                case "units":
                    grid_main.Children.Add(uc_units.Instance);
                    break;
                //80  menuSettings
                case "menuSettings":
                    grid_main.Children.Add(uc_menuSettings.Instance);
                    break;
                //81  itemsCosting
                case "itemsCosting":
                    grid_main.Children.Add(uc_itemsCosting.Instance);
                    break;
                //82  consumptionRawMaterials
                case "consumptionRawMaterials":
                    grid_main.Children.Add(uc_consumptionRawMaterials.Instance);
                    break;
                //83  reservationTable
                case "reservationTable":
                    grid_main.Children.Add(uc_reservationTable.Instance);
                    break;
                //84  reservationsUpdate
                case "reservationsUpdate":
                    grid_main.Children.Add(uc_reservationsUpdate.Instance);
                    break;
                //85  residentialSectors
                case "residentialSectors":
                    grid_main.Children.Add(uc_residentialSectors.Instance);
                    break;
                //86  emailsGeneral
                case "emailsGeneral":
                    grid_main.Children.Add(uc_emailsGeneral.Instance);
                    break;
                //87  emailsSetting
                case "emailsSetting":
                    grid_main.Children.Add(uc_emailsSetting.Instance);
                    break;
                //88  emailsTamplates
                case "emailsTamplates":
                    grid_main.Children.Add(uc_emailsTamplates.Instance);
                    break;
                //89  driversManagement
                case "driversManagement":
                    grid_main.Children.Add(uc_driversManagement.Instance);
                    break;
                //90  categorie
                case "categorie":
                    grid_main.Children.Add(uc_categorie.Instance);
                    break;

                //91  alerts
                //92  storageAlerts
                //93  kitchenAlerts
                //94  saleAlerts
                //95  accountsAlerts
                //96  storageAlerts_minMaxItem
                //97  storageAlerts_ImpExp
                //98  storageAlerts_ctreatePurchaseInvoice
                //99  storageAlerts_ctreatePurchaseReturnInvoice
                //100 storageAlerts_spendingOrderApprove
                //101 kitchenAlerts_spendingOrderRequest
                //102 saleAlerts_executeOrder

                //103 storageReports
                case "storageReports":
                    grid_main.Children.Add(uc_storageReports.Instance);
                    break;
                //104 purchaseReports
                case "purchaseReports":
                    grid_main.Children.Add(uc_purchaseReports.Instance);
                    break;
                //105 salesReports
                case "salesReports":
                    grid_main.Children.Add(uc_salesReports.Instance);
                    break;
                //106 accountsReports
                case "accountsReports":
                    grid_main.Children.Add(uc_accountsReports.Instance);
                    break;
                //107 stockStorageReports
                //case "stockStorageReports":
                //    grid_main.Children.Add(uc_stockStorageReports.Instance);
                //    break;
                //108 externalStorageReports
                //case "externalStorageReports":
                //    grid_main.Children.Add(uc_externalStorageReports.Instance);
                //    break;
                //109 internalStorageReports
                //case "internalStorageReports":
                //    grid_main.Children.Add(uc_internalStorageReports.Instance);
                //    break;
                //110 directStorageReports
                //case "directStorageReports":
                //    grid_main.Children.Add(uc_directStorageReports.Instance);
                //    break;
                //111 stocktakingStorageReports
                //case "stocktakingStorageReports":
                //    grid_main.Children.Add(uc_stocktakingStorageReports.Instance);
                //    break;
                //112 destroiedStorageReports
                //case "destroiedStorageReports":
                //    grid_main.Children.Add(uc_destroiedStorageReports.Instance);
                //    break;
                //113 invoicePurchaseReports
                //case "invoicePurchaseReports":
                //    grid_main.Children.Add(uc_invoicePurchaseReports.Instance);
                //    break;
                //114 itemPurchaseReports
                //case "itemPurchaseReports":
                //    grid_main.Children.Add(uc_itemPurchaseReports.Instance);
                //    break;
                //115 orderPurchaseReports
                //case "orderPurchaseReports":
                //    grid_main.Children.Add(uc_orderPurchaseReports.Instance);
                //    break;
                //116 invoiceSalesReports
                //case "invoiceSalesReports":
                //    grid_main.Children.Add(uc_invoiceSalesReports.Instance);
                //    break;
                //117 itemSalesReports
                //case "itemSalesReports":
                //    grid_main.Children.Add(uc_itemSalesReports.Instance);
                //    break;
                //118 promotionSalesReports
                //case "promotionSalesReports":
                //    grid_main.Children.Add(uc_promotionSalesReports.Instance);
                //    break;
                //119 orderSalesReports
                //case "orderSalesReports":
                //    grid_main.Children.Add(uc_orderSalesReports.Instance);
                //    break;
                //120 quotationSalesReports
                //case "quotationSalesReports":
                //    grid_main.Children.Add(uc_quotationSalesReports.Instance);
                //    break;
                //121 dailySalesReports
                //case "dailySalesReports":
                //    grid_main.Children.Add(uc_dailySalesReports.Instance);
                //    break;
                //122 paymentsAccountsReports
                //case "paymentsAccountsReports":
                //    grid_main.Children.Add(uc_paymentsAccountsReports.Instance);
                //    break;
                //123 recipientAccountsReports
                //case "recipientAccountsReports":
                //    grid_main.Children.Add(uc_recipientAccountsReports.Instance);
                //    break;
                //124 bankAccountsReports
                //case "bankAccountsReports":
                //    grid_main.Children.Add(uc_bankAccountsReports.Instance);
                //    break;
                //125 posAccountsReports
                //case "posAccountsReports":
                //    grid_main.Children.Add(uc_posAccountsReports.Instance);
                //    break;
                //126 statementAccountsReports
                //case "statementAccountsReports":
                //    grid_main.Children.Add(uc_statementAccountsReports.Instance);
                //    break;
                //127 fundAccountsReports
                //case "fundAccountsReports":
                //    grid_main.Children.Add(uc_fundAccountsReports.Instance);
                //    break;
                //128 profitsAccountsReports
                //case "profitsAccountsReports":
                //    grid_main.Children.Add(uc_profitsAccountsReports.Instance);
                //    break;

                default:
                    return;

            }
        }
        #endregion
    }
}
