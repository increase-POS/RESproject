using MaterialDesignThemes.Wpf;
using netoaster;
using POS.View.windows;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using Tulpep.NotificationWindow;

namespace POS.Classes
{
    class SectionData
    {
        public static bool iscodeExist = false;

        public static Agent agentModel = new Agent();
        public static Bonds bondModel = new Bonds();
        public static Branch branchModel = new Branch();
        public static Category categoryModel = new Category();
        public static Pos posModel = new Pos();
        public static Offer offerModel = new Offer();
        public static CashTransfer cashModel = new CashTransfer();

        public static Coupon couponModel = new Coupon();

        public static string code;

        public static BrushConverter bc = new BrushConverter();

        public static ImageBrush brush = new ImageBrush();

        public async static Task<long> genRandomCode(string type)
        {
            Random rnd = new Random();
            long randomNum = rnd.Next(0, 999999999);
            await isCodeExist(randomNum.ToString(), type);
            if (!iscodeExist)
                code = randomNum.ToString();
            else await genRandomCode(type);

            return randomNum;

        }

        public static async Task isCodeExist(string randomNum, string type)
        {
            try
            {
                List<Agent> agents = await agentModel.Get(type);
                Agent agent = new Agent();
                List<string> codes = new List<string>();
                for (int i = 0; i < agents.Count; i++)
                {
                    agent = agents[i];
                    codes.Add(agent.code.Trim());
                }
                if (codes.Contains(randomNum.Trim()))
                    iscodeExist = true;
                else
                    iscodeExist = false;
            }
            catch { }
        }

        public async static Task<long> genRandomCode(string type, string _class)
        {
            Random rnd = new Random();
            long randomNum = rnd.Next(0, 999999999);
            await isCodeExist(randomNum.ToString(), type, _class);
            if (!iscodeExist)
                code = randomNum.ToString();
            else await genRandomCode(type, _class);

            return randomNum;

        }

        public static async Task<bool> isCodeExist(string randomNum, string type, string _class, int id)
        {
            iscodeExist = false;
            try
            {
                List<string> codes = new List<string>();

                if (_class.Equals("Agent"))
                {
                    List<Agent> agents = await agentModel.Get(type);
                    //Agent agent = new Agent();
                    //for (int i = 0; i < agents.Count; i++)
                    //{
                    //    agent = agents[i];
                    //    codes.Add(agent.code.Trim());
                    //}
                    if (agents.Any(a => a.code == randomNum && a.agentId != id))
                        iscodeExist = true;
                    else
                        iscodeExist = false;
                }
                else if (_class.Equals("Branch"))
                {
                    List<Branch> branches = await branchModel.Get(type);

                    //Branch branch = new Branch();
                    //for (int i = 0; i < branches.Count; i++)
                    //{
                    //    branch = branches[i];
                    //    if (branch.branchId != id)
                    //        codes.Add(branch.code.Trim());
                    //}
                    if (branches.Any(b => b.code == randomNum && b.branchId != id))
                        iscodeExist = true;
                    else
                        iscodeExist = false;
                }
                else if (_class.Equals("Category"))
                {
                    List<Category> categories = await categoryModel.GetAllCategories(MainWindow.userID.Value);

                    //Category category = new Category();
                    //for (int i = 0; i < categories.Count; i++)
                    //{
                    //    category = categories[i];
                    //    if (category.categoryId != id)
                    //        codes.Add(category.categoryCode.Trim());
                    //}
                    if (categories.Any(c => c.categoryCode == randomNum && c.categoryId != id))
                        iscodeExist = true;
                    else
                        iscodeExist = false;
                }
                else if (_class.Equals("Pos"))
                {
                    List<Pos> poss = await posModel.Get();

                    //    Pos pos = new Pos();
                    //    for (int i = 0; i < poss.Count; i++)
                    //    {
                    //        pos = poss[i];
                    //        if (pos.posId != id)
                    //            codes.Add(pos.code.Trim());
                    //    }
                    if (poss.Any(p => p.code == randomNum && p.posId != id))
                        iscodeExist = true;
                    else
                        iscodeExist = false;
                }
                //if (codes.Contains(randomNum.Trim()))
                //    iscodeExist = true;
                //else
                //    iscodeExist = false;

            }
            catch { }
            return iscodeExist;
        }
        public static async Task<bool> isCodeExist(string randomNum, string type, string _class)
        {
            iscodeExist = false;
            try
            {
                List<string> codes = new List<string>();

                if (_class.Equals("Agent"))
                {
                    List<Agent> agents = await agentModel.Get(type);
                    Agent agent = new Agent();
                    for (int i = 0; i < agents.Count; i++)
                    {
                        agent = agents[i];
                        codes.Add(agent.code.Trim());
                    }
                }
                else if (_class.Equals("Branch"))
                {
                    List<Branch> branches = await branchModel.Get(type);

                    Branch branch = new Branch();
                    for (int i = 0; i < branches.Count; i++)
                    {
                        branch = branches[i];
                        codes.Add(branch.code.Trim());
                    }
                }
                else if (_class.Equals("Offer"))
                {
                    List<Offer> offers = await offerModel.Get();

                    Offer offer = new Offer();
                    for (int i = 0; i < offers.Count; i++)
                    {
                        offer = offers[i];
                        codes.Add(offer.code.Trim());
                    }
                }

                if (codes.Contains(randomNum.Trim()))
                    iscodeExist = true;
                else
                    iscodeExist = false;

            }
            catch { }
            return iscodeExist;
        }
        public static async Task<bool> CouponCodeNotExist(string randomNum, int id)
        {
            try
            {
                Coupon coupon = new Coupon();
                coupon = await couponModel.Existcode(randomNum);
                if ((coupon.code.Trim() == randomNum.Trim()) && (coupon.cId != id))
                {
                    return false;
                }
                else return true;

            }
            catch { return true; }
        }

        public static async Task<bool> chkIfCouponBarCodeIsExist(string randomNum, int id)
        {
            Coupon coupon = new Coupon();
            coupon = await couponModel.getCouponByBarCode(randomNum);
            try
            {
                if ((coupon.barcode.Trim() == randomNum.Trim()) && (coupon.cId != id))
                {
                    return true;
                }
                else return false;
            }
            catch { return false; }
        }


        public static bool IsValid(string txt)
        {//for email
            Regex regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                   RegexOptions.CultureInvariant | RegexOptions.Singleline);
            bool isValidEmail = regex.IsMatch(txt);

            if (!isValidEmail) return false;

            else return true;

        }
        public static void SetError(Control c, Path p_error, ToolTip tt_error, string tr)
        {
            p_error.Visibility = Visibility.Visible;
            tt_error.Content = MainWindow.resourcemanager.GetString(tr);
            c.Background = (Brush)bc.ConvertFrom("#15FF0000");
        }
        public static bool validateEmptyTextBlock(TextBlock txt, Path p_error, ToolTip tt_error, string tr)
        {
            bool isValid = true;
            if (txt.Text.Equals(""))
            {
                p_error.Visibility = Visibility.Visible;
                tt_error.Content = MainWindow.resourcemanager.GetString(tr);
                txt.Background = (Brush)bc.ConvertFrom("#15FF0000");
                isValid = false;
            }
            else
            {
                txt.Background = (Brush)bc.ConvertFrom("#f8f8f8");
                p_error.Visibility = Visibility.Collapsed;
            }
            return isValid;
        }
        public static bool validateEmptyTextBox(TextBox tb, Path p_error, ToolTip tt_error, string tr)
        {
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                p_error.Visibility = Visibility.Visible;
                tt_error.Content = MainWindow.resourcemanager.GetString(tr);
                tb.Background = (Brush)bc.ConvertFrom("#15FF0000");
                isValid = false;
            }
            else
            {
                tb.Background = (Brush)bc.ConvertFrom("#f8f8f8");
                p_error.Visibility = Visibility.Collapsed;
            }
            return isValid;
        }
        public static bool validateEmptyTextBox_setupFirstPos(TextBox tb, Path p_error, ToolTip tt_error, string tr)
        {
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(tb.Text) )
            {
                p_error.Visibility = Visibility.Visible;
                tt_error.Content = wd_setupFirstPos.resourcemanager.GetString(tr);
                tb.Background = (Brush)bc.ConvertFrom("#15FF0000");
                isValid = false;
            }
            else
            {
                tb.Background = (Brush)bc.ConvertFrom("#f8f8f8");
                p_error.Visibility = Visibility.Collapsed;
            }
            return isValid;
        }
        public static bool validateEmptyComboBox(ComboBox cb, Path p_error, ToolTip tt_error, string tr)
        {
            bool isValid = true;

            if (cb.SelectedIndex == -1)
            {
                p_error.Visibility = Visibility.Visible;
                tt_error.Content = MainWindow.resourcemanager.GetString(tr);
                cb.Background = (Brush)bc.ConvertFrom("#15FF0000");
                isValid = false;
            }
            else
            {
                cb.Background = (Brush)bc.ConvertFrom("#f8f8f8");
                p_error.Visibility = Visibility.Collapsed;

            }
            return isValid;
        }
        
        public static bool validateEmptyComboBox_setupFirstPos(ComboBox cb, Path p_error, ToolTip tt_error, string tr)
        {
            bool isValid = true;

            if (cb.SelectedIndex == -1)
            {
                p_error.Visibility = Visibility.Visible;
                tt_error.Content = wd_setupFirstPos.resourcemanager.GetString(tr);
                cb.Background = (Brush)bc.ConvertFrom("#15FF0000");
                isValid = false;
            }
            else
            {
                cb.Background = (Brush)bc.ConvertFrom("#f8f8f8");
                p_error.Visibility = Visibility.Collapsed;

            }
            return isValid;
        }
        public static bool validateEmptyPassword(PasswordBox pb, Path p_error, ToolTip tt_error, string tr)
        {
            bool isValid = true;

            if (pb.Password.Equals(""))
            {
                SectionData.showPasswordValidate(pb, p_error, tt_error, "trEmptyPasswordToolTip");
                p_error.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                SectionData.clearPasswordValidate(pb, p_error);
                p_error.Visibility = Visibility.Collapsed;
            }
            return isValid;
        }
        public static bool validateEmptyPassword_setupFirstPos(PasswordBox pb, Path p_error, ToolTip tt_error, string tr)
        {
            bool isValid = true;

            if (pb.Password.Equals(""))
            {
                SectionData.showPasswordValidate_setupFirstPos(pb, p_error, tt_error, "trEmptyPasswordToolTip");
                p_error.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                SectionData.clearPasswordValidate(pb, p_error);
                p_error.Visibility = Visibility.Collapsed;
            }
            return isValid;
        }
        
        public static bool validateEmail(TextBox tb, Path p_error, ToolTip tt_error)
        {
            bool isValid = true;
            if (!tb.Text.Equals(""))
            {
                if (!ValidatorExtensions.IsValid(tb.Text))
                {
                    p_error.Visibility = Visibility.Visible;
                    tt_error.Content = MainWindow.resourcemanager.GetString("trErrorEmailToolTip");
                    tb.Background = (Brush)bc.ConvertFrom("#15FF0000");
                    isValid = false;
                }
                else
                {
                    p_error.Visibility = Visibility.Collapsed;
                    tb.Background = (Brush)bc.ConvertFrom("#f8f8f8");
                    isValid = true;
                }
            }
            return isValid;
        }
        public static bool validateEmptyDatePicker(DatePicker dp, Path p_error, ToolTip tt_error, string tr)
        {
            bool isValid = true;
            TextBox tb = (TextBox)dp.Template.FindName("PART_TextBox", dp);
            if (tb.Text.Trim().Equals(""))
            {
                p_error.Visibility = Visibility.Visible;
                tt_error.Content = MainWindow.resourcemanager.GetString(tr);
                tb.Background = (Brush)bc.ConvertFrom("#15FF0000");
                isValid = false;
            }
            else
            {
                tb.Background = (Brush)bc.ConvertFrom("#f8f8f8");
                p_error.Visibility = Visibility.Collapsed;
            }
            return isValid;

        }
        public static void validateSmalThanDateNowDatePicker(DatePicker dp, Path p_error, ToolTip tt_error, string tr)
        {
            TextBox tb = (TextBox)dp.Template.FindName("PART_TextBox", dp);
            if (dp.SelectedDate < DateTime.Now)
            {
                p_error.Visibility = Visibility.Visible;
                tt_error.Content = MainWindow.resourcemanager.GetString(tr);
                tb.Background = (Brush)bc.ConvertFrom("#15FF0000");
            }
            else
            {
                tb.Background = (Brush)bc.ConvertFrom("#f8f8f8");
                p_error.Visibility = Visibility.Collapsed;
            }
        }
        public static void clearValidate(TextBox tb, Path p_error)
        {
            tb.Background = (Brush)bc.ConvertFrom("#f8f8f8");
            p_error.Visibility = Visibility.Collapsed;
        }
        public static void clearPasswordValidate(PasswordBox pb, Path p_error)
        {
            pb.Background = (Brush)bc.ConvertFrom("#f8f8f8");
            p_error.Visibility = Visibility.Collapsed;
        }
        public static void clearTextBlockValidate(TextBlock txt, Path p_error)
        {
            txt.Background = (Brush)bc.ConvertFrom("#f8f8f8");
            p_error.Visibility = Visibility.Collapsed;
        }
        public static void clearComboBoxValidate(ComboBox cb, Path p_error)
        {
            cb.Background = (Brush)bc.ConvertFrom("#f8f8f8");
            p_error.Visibility = Visibility.Collapsed;
        }
        public static void showTextBoxValidate(TextBox tb, Path p_error, ToolTip tt_error, string tr)
        {
            p_error.Visibility = Visibility.Visible;
            tt_error.Content = MainWindow.resourcemanager.GetString(tr);
            tb.Background = (Brush)bc.ConvertFrom("#15FF0000");
        }
        public static void showPasswordValidate(PasswordBox tb, Path p_error, ToolTip tt_error, string tr)
        {
            p_error.Visibility = Visibility.Visible;
            tt_error.Content = MainWindow.resourcemanager.GetString(tr);
            tb.Background = (Brush)bc.ConvertFrom("#15FF0000");
        }
        
            public static void showPasswordValidate_setupFirstPos(PasswordBox tb, Path p_error, ToolTip tt_error, string tr)
        {
            p_error.Visibility = Visibility.Visible;
            tt_error.Content = wd_setupFirstPos.resourcemanager.GetString(tr);
            tb.Background = (Brush)bc.ConvertFrom("#15FF0000");
        }
        public static void showComboBoxValidate(ComboBox cb, Path p_error, ToolTip tt_error, string tr)
        {
            p_error.Visibility = Visibility.Visible;
            tt_error.Content = MainWindow.resourcemanager.GetString(tr);
            cb.Background = (Brush)bc.ConvertFrom("#15FF0000");
        }

        public static void showDatePickerValidate(DatePicker dp, Path p_error, ToolTip tt_error, string tr)
        {
            TextBox tb = (TextBox)dp.Template.FindName("PART_TextBox", dp);

            p_error.Visibility = Visibility.Visible;
            tt_error.Content = MainWindow.resourcemanager.GetString(tr);
            tb.Background = (Brush)bc.ConvertFrom("#15FF0000");
        }

        public static void showTimePickerValidate(TimePicker tp, Path p_error, ToolTip tt_error, string tr)
        {
            TextBox tb = (TextBox)tp.Template.FindName("PART_TextBox", tp);

            p_error.Visibility = Visibility.Visible;
            tt_error.Content = MainWindow.resourcemanager.GetString(tr);
            tb.Background = (Brush)bc.ConvertFrom("#15FF0000");
        }





        public static void validateDuplicateCode(TextBox tb, Path p_error, ToolTip tt_error, string tr)
        {
            p_error.Visibility = Visibility.Visible;
            tt_error.Content = MainWindow.resourcemanager.GetString(tr);
            tb.Background = (Brush)bc.ConvertFrom("#15FF0000");
        }

        public static void getMobile(string _mobile, ComboBox _area, TextBox _tb)
        {//mobile
            if ((_mobile != null))
            {
                string area = _mobile;
                string[] pharr = area.Split('-');
                int j = 0;
                string phone = "";

                foreach (string strpart in pharr)
                {
                    if (j == 0)
                    {
                        area = strpart;
                    }
                    else
                    {
                        phone = phone + strpart;
                    }
                    j++;
                }

                _area.Text = area;

                _tb.Text = phone.ToString();
            }
            else
            {
                _area.SelectedIndex = -1;
                _tb.Clear();
            }
        }

        public static void getPhone(string _phone, ComboBox _area, ComboBox _areaLocal, TextBox _tb)
        {//phone
            if ((_phone != null))
            {
                string area = _phone;
                string[] pharr = area.Split('-');
                int j = 0;
                string phone = "";
                string areaLocal = "";
                foreach (string strpart in pharr)
                {
                    if (j == 0)
                        area = strpart;
                    else if (j == 1)
                        areaLocal = strpart;
                    else
                        phone = phone + strpart;
                    j++;
                }

                _area.Text = area;
                _areaLocal.Text = areaLocal;
                _tb.Text = phone.ToString();
            }
            else
            {
                _area.SelectedIndex = -1;
                _areaLocal.SelectedIndex = -1;
                _tb.Clear();
            }
        }

        public static async void getImg(string type, string imageUri, Button button)
        {
            try
            {

                //if (string.IsNullOrEmpty(category.image))
                //{
                //    SectionData.clearImg(button);
                //}
                //else
                //{

                if (type.Equals("Category"))
                {
                    Category category = new Category();
                    byte[] imageBuffer = await category.downloadImage(imageUri); // read this as BLOB from your DB
                    var bitmapImage = new BitmapImage();
                    using (var memoryStream = new System.IO.MemoryStream(imageBuffer))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                    }
                    button.Background = new ImageBrush(bitmapImage);
                }
                else if (type.Equals("Item"))
                {
                    Item item = new Item();
                    byte[] imageBuffer = await item.downloadImage(imageUri); // read this as BLOB from your DB
                    var bitmapImage = new BitmapImage();
                    using (var memoryStream = new System.IO.MemoryStream(imageBuffer))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                    }
                    button.Background = new ImageBrush(bitmapImage);
                }
                else if (type.Equals("User"))
                {
                    User user = new User();
                    byte[] imageBuffer = await user.downloadImage(imageUri); // read this as BLOB from your DB
                    var bitmapImage = new BitmapImage();
                    using (var memoryStream = new System.IO.MemoryStream(imageBuffer))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                    }
                    button.Background = new ImageBrush(bitmapImage);
                }
                else if (type.Equals("Agent"))
                {
                    Agent agent = new Agent();
                    byte[] imageBuffer = await agent.downloadImage(imageUri); // read this as BLOB from your DB
                    var bitmapImage = new BitmapImage();
                    using (var memoryStream = new System.IO.MemoryStream(imageBuffer))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                    }
                    button.Background = new ImageBrush(bitmapImage);
                }

                //}
            }
            catch
            {
                clearImg(button);
            }
        }
        public static async void getLocalImg(string type, string imageUri, Button button)
        {
            try
            {

                if (type.Equals("Category"))
                {
                    Category category = new Category();
                    byte[] imageBuffer =  readLocalImage(imageUri, Global.TMPFolder); 
                    var bitmapImage = new BitmapImage();
                    using (var memoryStream = new System.IO.MemoryStream(imageBuffer))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                    }
                    button.Background = new ImageBrush(bitmapImage);
                }
                else if (type.Equals("Item"))
                {
                    Item item = new Item();
                    byte[] imageBuffer =  readLocalImage(imageUri, Global.TMPItemsFolder);
                    var bitmapImage = new BitmapImage();
                    using (var memoryStream = new System.IO.MemoryStream(imageBuffer))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                    }
                    button.Background = new ImageBrush(bitmapImage);
                }
                else if (type.Equals("User"))
                {
                    User user = new User();
                    byte[] imageBuffer =  readLocalImage(imageUri, Global.TMPUsersFolder); 
                    var bitmapImage = new BitmapImage();
                    using (var memoryStream = new System.IO.MemoryStream(imageBuffer))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                    }
                    button.Background = new ImageBrush(bitmapImage);
                }
                else if (type.Equals("Agent"))
                {
                    Agent agent = new Agent();
                    byte[] imageBuffer =  readLocalImage(imageUri, Global.TMPAgentsFolder);
                    var bitmapImage = new BitmapImage();
                    using (var memoryStream = new System.IO.MemoryStream(imageBuffer))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();
                    }
                    button.Background = new ImageBrush(bitmapImage);
                }

                //}
            }
            catch
            {
                clearImg(button);
            }
        }
        public static bool chkImgChng(string imageName, DateTime updateDate, string TMPFolder)
        {
            string dir = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
            string tmpPath = System.IO.Path.Combine(dir, Global.TMPFolder);
            tmpPath = System.IO.Path.Combine(tmpPath, imageName);
            DateTime mofdifyDate;
            if (!System.IO.File.Exists(tmpPath))
            {
                return true;
            }
            else
            {
                mofdifyDate = System.IO.File.GetLastWriteTime(tmpPath);
                if (mofdifyDate < updateDate)
                    return true;
            }
            return false;
        }
        public static byte[] readLocalImage(string imageName, string TMPFolder)
        {
            byte[] data = null;
            string dir =System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
            string tmpPath = System.IO.Path.Combine(dir, TMPFolder);
            if (!System.IO.Directory.Exists(tmpPath))
                System.IO.Directory.CreateDirectory(tmpPath);
            tmpPath = System.IO.Path.Combine(tmpPath, imageName);
            if (System.IO.File.Exists(tmpPath))
            {
                // Load file meta data with FileInfo
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(tmpPath);
                // The byte[] to save the data in
                data = new byte[fileInfo.Length];
                using (var stream = new System.IO.FileStream(tmpPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    stream.Read(data, 0, data.Length);
                }
                // Delete the temporary file
               // fileInfo.Delete();
            }
            return data;
        }
        public static void clearImg(Button img)
        {
            Uri resourceUri = new Uri("pic/no-image-icon-125x125.png", UriKind.Relative);
            StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);

            BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
            brush.ImageSource = temp;
            img.Background = brush;
        }
        public static decimal calcPercentage(decimal value, decimal percentage)
        {
            decimal percentageVal = (value * percentage) / 100;

            return percentageVal;
        }
        public static void defaultDatePickerStyle(DatePicker dp)
        {
            dp.Loaded += delegate
            {

                var textBox1 = (TextBox)dp.Template.FindName("PART_TextBox", dp);
                if (textBox1 != null)
                {
                    textBox1.Background = dp.Background;
                    textBox1.BorderThickness = dp.BorderThickness;
                }
            };
        }

        public static async Task<string> generateNumber(char opperationType, string side)
        {
            List<CashTransfer> cashes = new List<CashTransfer>();
            Branch b = new Branch();
            b = await branchModel.getBranchById(MainWindow.branchID.Value);

            string str1 = b.code;

            string str2 = opperationType + side; ;

            string str3 = "";
            cashes = await cashModel.GetCashTransferAsync(Convert.ToString(opperationType), side);
            str3 = (cashes.Count() + 1).ToString();

            return str1 + str2 + str3;
        }

        public static async Task<string> generateNumberBond(char opperationType, string side)
        {
            IEnumerable<CashTransfer> cashes;
            //IEnumerable<CashTransfer> cashesQuery;

            Branch b = new Branch();
            b = await branchModel.getBranchById(MainWindow.branchID.Value);

            string str1 = b.code;

            string str2 = opperationType + side;

            string str3 = "";
            cashes = await cashModel.GetCashTransferAsync(Convert.ToString(opperationType), "all");
            cashes = cashes.Where(s => s.processType == "doc");

            str3 = (cashes.Count() + 1).ToString();

            return str1 + str2 + str3;
        }

        static public void searchInComboBox(ComboBox cbm)
        {
            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(cbm.Items);
            itemsViewOriginal.Filter = ((o) =>
            {
                if (String.IsNullOrEmpty(cbm.Text)) return true;
                else
                {
                    if (((string)o).Contains(cbm.Text)) return true;
                    else return false;
                }
            });
            itemsViewOriginal.Refresh();
        }
        static public bool isAdminPermision()
        {
            //if (MainWindow.userLogin.userId == 1 || MainWindow.userLogin.userId == 2)
            if (MainWindow.userLogin.isAdmin == true)
                return true;
            return false;
        }

        static List<Branch> branches;
        static List<Branch> branchesWithAll;
        static List<Branch> branchesWithoutMain;
        static public async Task fillBranches(ComboBox combo, string type = "")
        { 
            if (isAdminPermision())
                branches = await branchModel.GetAll();
            else
                branches = await branchModel.BranchesByBranchandUser(MainWindow.branchID.Value, MainWindow.userLogin.userId);

            combo.ItemsSource = branches.Where(b => b.type != type && b.branchId != 1);
            combo.SelectedValuePath = "branchId";
            combo.DisplayMemberPath = "name";
            combo.SelectedIndex = -1;
        }
        static public async Task fillBranchesWithoutCurrent(ComboBox combo, int currentBranchId, string type = "")
        {
            if (isAdminPermision())
                branches = await branchModel.GetAll();
            else
                branches = await branchModel.BranchesByBranchandUser(MainWindow.branchID.Value, MainWindow.userLogin.userId);

            branchModel = branches.Where(s => s.branchId == currentBranchId).FirstOrDefault<Branch>();
            branches.Remove(branchModel);
            var br = new Branch();
            br.branchId = 0;
            br.name = "---";
            branches.Insert(0, br);
            combo.ItemsSource = branches.Where(b => b.type != type && b.branchId != 1);
            combo.SelectedValuePath = "branchId";
            combo.DisplayMemberPath = "name";
            combo.SelectedIndex = -1;
        }
        static public async Task fillWithoutCurrent(ComboBox combo, int currentBranchId, string type = "")
        {
            if (isAdminPermision())
                branches = await branchModel.GetAll();
            else
                branches = await branchModel.BranchesByBranchandUser(MainWindow.branchID.Value, MainWindow.userLogin.userId);

            branchModel = branches.Where(s => s.branchId == currentBranchId).FirstOrDefault<Branch>();
            branches.Remove(branchModel);
            combo.ItemsSource = branches.Where(b => b.type != type && b.branchId != 1);
            combo.SelectedValuePath = "branchId";
            combo.DisplayMemberPath = "name";
            combo.SelectedIndex = -1;
        }
        static public async Task fillBranchesWithAll(ComboBox combo, string type = "")
        {
            if (isAdminPermision())
                branches = await branchModel.GetAll();
            else
                branches = await branchModel.BranchesByBranchandUser(MainWindow.branchID.Value, MainWindow.userLogin.userId);

            branchesWithAll = branches.ToList();
            Branch branch = new Branch();
            branch.name = "All";
            branch.branchId = 0;
            branchesWithAll.Insert(0, branch);

            combo.ItemsSource = branchesWithAll.Where(b => b.type != type && b.branchId != 1);
            combo.SelectedValuePath = "branchId";
            combo.DisplayMemberPath = "name";
            combo.SelectedIndex = -1;
        }

        static public async Task fillBranchesWithoutMain(ComboBox combo)
        {
            if(branchesWithoutMain == null)
                branchesWithoutMain = await branchModel.GetAllWithoutMain("all");

            combo.ItemsSource = branchesWithoutMain;
            combo.SelectedValuePath = "branchId";
            combo.DisplayMemberPath = "name";
            combo.SelectedIndex = -1;
        }
        static public void FillDefaultPayType(ComboBox cmb)
        {
            #region fill process type
            var typelist = new[] {
                new { Text = MainWindow.resourcemanager.GetString("trCash")       , Value = "cash" },
                new { Text = MainWindow.resourcemanager.GetString("trCredit") , Value = "balance" },
                new { Text = MainWindow.resourcemanager.GetString("trAnotherPaymentMethods") , Value = "card" },
                new { Text = MainWindow.resourcemanager.GetString("trMultiplePayment") , Value = "multiple" }, 
                //new { Text = MainWindow.resourcemanager.GetString("trDocument")   , Value = "doc" },
                //new { Text = MainWindow.resourcemanager.GetString("trCheque")     , Value = "cheque" },
                 };
            cmb.DisplayMemberPath = "Text";
            cmb.SelectedValuePath = "Value";
            cmb.ItemsSource = typelist;
            #endregion
        }
        /// <summary>
        /// لمنع  الصفر بالبداية
        /// </summary>
        /// <param name="txb"></param>
        static public void InputJustNumber(ref TextBox txb)
        {
            if (txb.Text.Count() == 2 && txb.Text == "00")
            {
                string myString = txb.Text;
                myString = Regex.Replace(myString, "00", "0");
                txb.Text = myString;
                txb.Select(txb.Text.Length, 0);
                txb.Focus();
            }
        }

        static async public void ExceptionMessage(Exception ex, object window)
        {
            try
            {

                //Message
                if (ex.HResult == -2146233088)
                    Toaster.ShowError(window as Window, message: MainWindow.resourcemanager.GetString("trNoConnection"), animation: ToasterAnimation.FadeIn);
                else if(ex.HResult != -2147467261)
                    Toaster.ShowError(window as Window, message: ex.HResult + " || " + ex.Message, animation: ToasterAnimation.FadeIn);

                //- 2146233088     An error occurred while sending the request.
                //-2147467261    Void MoveNext()
                if (ex.HResult != -2146233088 &&   ex.HResult != -2147467261)
                { 
                    ErrorClass errorClass = new ErrorClass();
                    errorClass.num = ex.HResult.ToString();
                    errorClass.msg = ex.Message;
                    errorClass.stackTrace = ex.StackTrace;
                    errorClass.targetSite = ex.TargetSite.ToString();
                    errorClass.posId = MainWindow.posID;
                    errorClass.branchId = MainWindow.branchID;
                    errorClass.createUserId = MainWindow.userLogin.userId;
                    await errorClass.save(errorClass);
                }
            }
            catch
            {

            }
        }
        static public void StartAwait(Grid grid)
        {
            grid.IsEnabled = false;
            grid.Opacity = 0.6;
            MahApps.Metro.Controls.ProgressRing progressRing = new MahApps.Metro.Controls.ProgressRing();
            progressRing.Name = "prg_awaitRing";
            progressRing.Foreground = App.Current.Resources["MainColorBlue"] as Brush;
            progressRing.IsActive = true;
            Grid.SetRowSpan(progressRing, 10);
            Grid.SetColumnSpan(progressRing, 10);
            grid.Children.Add(progressRing);
        }
        static public void EndAwait(Grid grid)
        {
            MahApps.Metro.Controls.ProgressRing progressRing = FindControls.FindVisualChildren<MahApps.Metro.Controls.ProgressRing>(grid)
                .Where(x => x.Name == "prg_awaitRing").FirstOrDefault();
            grid.Children.Remove(progressRing);

            var progressRingList = FindControls.FindVisualChildren<MahApps.Metro.Controls.ProgressRing>(grid)
                 .Where(x => x.Name == "prg_awaitRing");
            if (progressRingList.Count() == 0)
            {
                    grid.IsEnabled = true;
                    grid.Opacity = 1;
            }
        }

        static SetValues valueModel = new SetValues();
        static UserSetValues uSetValueModel = new UserSetValues();
        static List<UserSetValues> usValues = new List<UserSetValues>();
        static UserSetValues usMenu = new UserSetValues();

        static public async Task<string> getUserMenuIsOpen(int userId)
        {
            List<SetValues> menuIsOpenValues = new List<SetValues>();
            menuIsOpenValues = await valueModel.GetBySetName("menuIsOpen");
            usValues = await uSetValueModel.GetAll();
            if (usValues.Count > 0)
            {
                var curUserValues = usValues.Where(c => c.userId == userId);

                if (curUserValues.Count() > 0)
                {
                    foreach (var l in curUserValues)
                        if (menuIsOpenValues.Any(c => c.valId == l.valId))
                        {
                            usMenu = l;
                        }
                    if (usMenu.id != 0)
                    {
                        var menu = await valueModel.GetByID(usMenu.valId.Value);

                        return menu.value;
                    }
                    else return "-1";
                }
                else return "-1";
            }

            else return "-1";
        }
        static UserSetValues usMenuIsOpen = new UserSetValues();
        static UserSetValues usValueModel = new UserSetValues();
        static SetValues valueMedel = new SetValues();
        static List<SetValues> menuValues = new List<SetValues>();
        static public async Task<int> getOpenValueId()
        {
            menuValues = await valueMedel.GetAll();
            SetValues openValue = menuValues.Where(o => o.value == "open").FirstOrDefault();
            return openValue.valId;
        }
        static public async Task<int> getCloseValueId()
        {
            menuValues = await valueMedel.GetAll();
            SetValues closeValue = menuValues.Where(o => o.value == "close").FirstOrDefault();
            return closeValue.valId;
        }
        static public async Task saveMenuState(int valId)
        {
            int oId = await getOpenValueId();
            int cId = await getCloseValueId();
            string m = await SectionData.getUserMenuIsOpen(MainWindow.userID.Value);
            var menus = await usValueModel.GetAll();
            usMenuIsOpen = menus.Where(x => x.userId == MainWindow.userID.Value && (x.valId == oId || x.valId == cId)).FirstOrDefault();
            if (m.Equals("-1"))
                usMenuIsOpen = new UserSetValues();

            usMenuIsOpen.userId = MainWindow.userID;
            usMenuIsOpen.valId = valId;
            usMenuIsOpen.createUserId = MainWindow.userID;
          //  string s = await usValueModel.Save(usMenuIsOpen);
          int s = await usValueModel.Save(usMenuIsOpen);
            if (!s.Equals(0))
            {
                //update menu in main window
                SetValues v = await valueMedel.GetByID(valId);
                MainWindow.menuIsOpen = v.value;
                ////save to user settings
                Properties.Settings.Default.menuIsOpen = v.value;
                Properties.Settings.Default.Save();

            }
        }
        public static string translate(string str)
        {
            string _str = "";


            #region  mainWindow
            if (str == "home")
                _str = MainWindow.resourcemanager.GetString("trHome");
            else if (str == "catalog")
                _str = MainWindow.resourcemanager.GetString("trCatalog");
            else if (str == "storage")
                _str = MainWindow.resourcemanager.GetString("trStore");
            else if (str == "purchase")
                _str = MainWindow.resourcemanager.GetString("trPurchases");
            else if (str == "sales")
                _str = MainWindow.resourcemanager.GetString("trSales");
            else if (str == "accounts")
                _str = MainWindow.resourcemanager.GetString("trAccounting");
            else if (str == "reports")
                _str = MainWindow.resourcemanager.GetString("trReports");
            else if (str == "sectionData")
                _str = MainWindow.resourcemanager.GetString("trSectionData");
            else if (str == "settings")
                _str = MainWindow.resourcemanager.GetString("trSettings");
            #endregion

            #region  storage
            if (str == "locations")
                _str = MainWindow.resourcemanager.GetString("trLocation");
            else if (str == "section")
                _str = MainWindow.resourcemanager.GetString("trSection");
            else if (str == "reciptOfInvoice")
                _str = MainWindow.resourcemanager.GetString("trInvoice");
            else if (str == "itemsStorage")
                _str = MainWindow.resourcemanager.GetString("trStorage");
            else if (str == "importExport")
                _str = MainWindow.resourcemanager.GetString("trMovements");
            else if (str == "itemsDestroy")
                _str = MainWindow.resourcemanager.GetString("trDestructive");
            else if (str == "shorstre")
                _str = MainWindow.resourcemanager.GetString("trShorstre");
            else if (str == "inventory")
                _str = MainWindow.resourcemanager.GetString("trStocktaking");
            else if (str == "storageStatistic")
                _str = MainWindow.resourcemanager.GetString("trStatistic");
            #endregion
            #region  Account
            else if (str == "posAccounting")
                _str = MainWindow.resourcemanager.GetString("trPOS");
            else if (str == "payments")
                _str = MainWindow.resourcemanager.GetString("trPayments");
            else if (str == "received")
                _str = MainWindow.resourcemanager.GetString("trReceived");
            else if (str == "bonds")
                _str = MainWindow.resourcemanager.GetString("trBonds");
            else if (str == "banksAccounting")
                _str = MainWindow.resourcemanager.GetString("trBanks");
            else if (str == "ordersAccounting")
                _str = MainWindow.resourcemanager.GetString("trOrders");
            else if (str == "subscriptions")
                _str = MainWindow.resourcemanager.GetString("trSubscriptions");
            else if (str == "accountsStatistic")
                _str = MainWindow.resourcemanager.GetString("trStatistic");
            #endregion
            #region  catalog
            else if (str == "categories")
                _str = MainWindow.resourcemanager.GetString("trCategories");
            else if (str == "item")
                _str = MainWindow.resourcemanager.GetString("trItems");
            else if (str == "service")
                _str = MainWindow.resourcemanager.GetString("trService");
            else if (str == "package")
                _str = MainWindow.resourcemanager.GetString("trPackage");
            else if (str == "properties")
                _str = MainWindow.resourcemanager.GetString("trProperties");
            else if (str == "units")
                _str = MainWindow.resourcemanager.GetString("trUnits");
            else if (str == "storageCost")
                _str = MainWindow.resourcemanager.GetString("trStorageCost");
            #endregion
            #region  purchase
            if (str == "payInvoice")
                _str = MainWindow.resourcemanager.GetString("trInvoice");
            else if (str == "purchaseOrder")
                _str = MainWindow.resourcemanager.GetString("trOrders");
            else if (str == "purchaseStatistic")
                _str = MainWindow.resourcemanager.GetString("trStatistic");
            #endregion
            #region  sales
            if (str == "reciptInvoice")
                _str = MainWindow.resourcemanager.GetString("trInvoice");
            else if (str == "coupon")
                _str = MainWindow.resourcemanager.GetString("trCoupon");
            else if (str == "offer")
                _str = MainWindow.resourcemanager.GetString("trOffer");
           
            else if (str == "quotation")
                _str = MainWindow.resourcemanager.GetString("trQuotations");
            else if (str == "salesOrders")
                _str = MainWindow.resourcemanager.GetString("trOrders");
            else if (str == "medals")
                _str = MainWindow.resourcemanager.GetString("trMedals");
            else if (str == "membership")
                _str = MainWindow.resourcemanager.GetString("trMembership");
            else if (str == "salesStatistic")
                _str = MainWindow.resourcemanager.GetString("trStatistic");
            #endregion
            #region  sectionData
            if (str == "suppliers")
                _str = MainWindow.resourcemanager.GetString("trSuppliers");
            else if (str == "customers")
                _str = MainWindow.resourcemanager.GetString("trCustomers");
            else if (str == "users")
                _str = MainWindow.resourcemanager.GetString("trUsers");
            else if (str == "branches")
                _str = MainWindow.resourcemanager.GetString("trBranches");
            else if (str == "stores")
                _str = MainWindow.resourcemanager.GetString("trStores");
            else if (str == "pos")
                _str = MainWindow.resourcemanager.GetString("trPOS");
            else if (str == "banks")
                _str = MainWindow.resourcemanager.GetString("trBanks");
            else if (str == "cards")
                _str = MainWindow.resourcemanager.GetString("trCard");
            else if (str == "shippingCompany")
                _str = MainWindow.resourcemanager.GetString("trShippingCompanies");
            #endregion
            #region  settings
            if (str == "general")
                _str = MainWindow.resourcemanager.GetString("trGeneral");
            else if (str == "reportsSettings")
                _str = MainWindow.resourcemanager.GetString("trReports");
            else if (str == "permissions")
                _str = MainWindow.resourcemanager.GetString("trPermission");
            else if (str == "emailsSetting")
                _str = MainWindow.resourcemanager.GetString("trEmail");
            else if (str == "emailTemplates")
                _str = MainWindow.resourcemanager.GetString("trEmailTemplates");
            #endregion


            return _str;

        }

        public static string DateToString(DateTime? date)
        {
            string sdate = "";
            if (date != null)
            {
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

        public static string DateTodbString(DateTime? date)
        {
            string sdate = "";
            if (date != null)
            {

                //"yyyy'-'MM'-'dd'T'HH':'mm':'ss"
                sdate = date.Value.ToString("yyyy'-'MM'-'dd");
                      
                
            }

            return sdate;
        }
        public static bool chkPasswordLength(string password)
        {
            bool b = false;
            if (password.Length < 6)
                b = true;
            return b;
        }

        public static string DecTostring(decimal? dec)
        {
            string sdc = "0";
            if (dec == null)
            {

            }
            else
            {
                decimal dc = decimal.Parse(dec.ToString());

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

        public static void ReportTabTitle(TextBlock textBlock, string firstTitle, string secondTitle)
        {

            //////////////////////////////////////////////////////////////////////////////
            if (firstTitle == "invoice")
                firstTitle = MainWindow.resourcemanager.GetString("trInvoices");
            else if (firstTitle == "quotation")
                firstTitle = MainWindow.resourcemanager.GetString("trQuotations");
            else if (firstTitle == "promotion")
                firstTitle = MainWindow.resourcemanager.GetString("trPromotion");
            else if (firstTitle == "internal")
                firstTitle = MainWindow.resourcemanager.GetString("trInternal");
            else if (firstTitle == "external")
                firstTitle = MainWindow.resourcemanager.GetString("trExternal");
            else if (firstTitle == "banksReport")
                firstTitle = MainWindow.resourcemanager.GetString("trBanks");
            else if (firstTitle == "destroied")
                firstTitle = MainWindow.resourcemanager.GetString("trDestructives");
            else if (firstTitle == "usersReport")
                firstTitle = MainWindow.resourcemanager.GetString("trUsers");
            else if (firstTitle == "storageReports")
                firstTitle = MainWindow.resourcemanager.GetString("trStorage");
            else if (firstTitle == "stocktaking")
                firstTitle = MainWindow.resourcemanager.GetString("trStocktaking");
            else if (firstTitle == "stock")
                firstTitle = MainWindow.resourcemanager.GetString("trStock");
            else if (firstTitle == "saleOrders" || firstTitle == "purchaseOrders")
                firstTitle = MainWindow.resourcemanager.GetString("trOrders");
            else if (firstTitle == "saleItems" || firstTitle == "purchaseItem")
                firstTitle = MainWindow.resourcemanager.GetString("trItems");
            else if (firstTitle == "recipientReport")
                firstTitle = MainWindow.resourcemanager.GetString("trRecepient");
            else if (firstTitle == "accountStatement")
                firstTitle = MainWindow.resourcemanager.GetString("trAccountStatement");
            else if (firstTitle == "paymentsReport")
                firstTitle = MainWindow.resourcemanager.GetString("trPayments");
            else if (firstTitle == "posReports")
                firstTitle = MainWindow.resourcemanager.GetString("trPOS");
            else if (firstTitle == "dailySalesStatistic")
                firstTitle = MainWindow.resourcemanager.GetString("trDailySales");
            else if (firstTitle == "accountProfits")
                firstTitle = MainWindow.resourcemanager.GetString("trProfits");
            else if (firstTitle == "accountFund")
                firstTitle = MainWindow.resourcemanager.GetString("trCashBalance");

            //////////////////////////////////////////////////////////////////////////////

            if (secondTitle == "branch")
                secondTitle = MainWindow.resourcemanager.GetString("trBranches");
            else if (secondTitle == "pos")
                secondTitle = MainWindow.resourcemanager.GetString("trPOS");
            else if (secondTitle == "vendors"|| secondTitle == "vendor")
                secondTitle = MainWindow.resourcemanager.GetString("trVendors");
            else if (secondTitle == "customers" || secondTitle == "customer")
                secondTitle = MainWindow.resourcemanager.GetString("trCustomers");
            else if (secondTitle == "users" || secondTitle == "user")
                secondTitle = MainWindow.resourcemanager.GetString("trUsers");
            else if (secondTitle == "items" || secondTitle == "item")
                secondTitle = MainWindow.resourcemanager.GetString("trItems");
            else if (secondTitle == "coupon")
                secondTitle = MainWindow.resourcemanager.GetString("trCoupon");
            else if (secondTitle == "offers")
                secondTitle = MainWindow.resourcemanager.GetString("trOffer");
            else if (secondTitle == "invoice")
                secondTitle = MainWindow.resourcemanager.GetString("tr_Invoice");
            else if (secondTitle == "order")
                secondTitle = MainWindow.resourcemanager.GetString("trOrders");
            else if (secondTitle == "quotation")
                secondTitle = MainWindow.resourcemanager.GetString("trQuotations");
            else if (secondTitle == "operator")
                secondTitle = MainWindow.resourcemanager.GetString("trOperator");
            else if (secondTitle == "payments")
                secondTitle = MainWindow.resourcemanager.GetString("trPayments");
            else if (secondTitle == "recipient")
                secondTitle = MainWindow.resourcemanager.GetString("trRecepient");
            else if (secondTitle == "destroied") 
                 secondTitle = MainWindow.resourcemanager.GetString("trDestructives");
            else if (secondTitle == "agent")
                secondTitle = MainWindow.resourcemanager.GetString("trCustomers");
            else if (secondTitle == "stock")
                secondTitle = MainWindow.resourcemanager.GetString("trStock");
            else if (secondTitle == "external")
                secondTitle = MainWindow.resourcemanager.GetString("trExternal");
            else if (secondTitle == "internal")
                secondTitle = MainWindow.resourcemanager.GetString("trInternal");
            else if (secondTitle == "stocktaking")
                secondTitle = MainWindow.resourcemanager.GetString("trStocktaking");
            else if (secondTitle == "archives")
                secondTitle = MainWindow.resourcemanager.GetString("trArchive");
            else if (secondTitle == "shortfalls")
                secondTitle = MainWindow.resourcemanager.GetString("trShortages");
            else if (secondTitle == "location")
                secondTitle = MainWindow.resourcemanager.GetString("trLocation");
            else if (secondTitle == "collect")
                secondTitle = MainWindow.resourcemanager.GetString("trCollect");
            else if (secondTitle == "shipping")
                secondTitle = MainWindow.resourcemanager.GetString("trShipping");
            else if (secondTitle == "salary")
                secondTitle = MainWindow.resourcemanager.GetString("trSalary");
            else if (secondTitle == "generalExpenses")
                secondTitle = MainWindow.resourcemanager.GetString("trGeneralExpenses");
            else if (secondTitle == "administrativePull")
                secondTitle = MainWindow.resourcemanager.GetString("trAdministrativePull");
            //////////////////////////////////////////////////////////////////////////////

            textBlock.Text = firstTitle + " / " + secondTitle;

        }
    }
}
