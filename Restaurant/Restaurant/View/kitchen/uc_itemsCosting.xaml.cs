using Restaurant.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Restaurant.View.kitchen
{
    /// <summary>
    /// Interaction logic for uc_itemsCosting.xaml
    /// </summary>
    public partial class uc_itemsCosting : UserControl
    {
        private static uc_itemsCosting _instance;
        public static uc_itemsCosting Instance
        {
            get
            {
                _instance = new uc_itemsCosting();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_itemsCosting()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            catalogMenuList = new List<string> { "allMenu", "appetizers", "beverages", "fastFood", "mainCourses", "desserts" };
            List<ItemUnit> itemUnitsList = new List<ItemUnit> {
                new ItemUnit { itemName = "Cheese", purchasePrice=800, price=1150, priceWithService=1200 ,isActive = 1},
                new ItemUnit { itemName = "Egg", purchasePrice=600, price=850, priceWithService=900 ,isActive = 1},
                new ItemUnit { itemName = "Butter", purchasePrice=1200, price=1400, priceWithService=1500,isActive = 1 },
                new ItemUnit { itemName = "Yogurt", purchasePrice=450, price=450, priceWithService=500,isActive = 1 },
                new ItemUnit { itemName = "Hamburger", purchasePrice=300, price=350, priceWithService=350 ,isActive = 1}
            };
            dg_items.ItemsSource = itemUnitsList;
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }
        #region catalogMenu
        public static List<string> catalogMenuList;

        private void catalogMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               
                string senderTag = (sender as Button).Tag.ToString();
                #region refresh colors
                foreach (var control in catalogMenuList)
                {
                    Border border = FindControls.FindVisualChildren<Border>(this).Where(x => x.Tag != null && x.Name == "bdr_" + control)
                        .FirstOrDefault();
                    if (border.Tag.ToString() == senderTag)
                        border.Background = Application.Current.Resources["MainColor"] as SolidColorBrush;
                    else
                        border.Background = Application.Current.Resources["White"] as SolidColorBrush;
                }
                foreach (var control in catalogMenuList)
                {

                    Path path = FindControls.FindVisualChildren<Path>(this).Where(x => x.Tag != null && x.Name == "path_" + control)
                        .FirstOrDefault();
                    if (path.Tag.ToString() == senderTag)
                        path.Fill = Application.Current.Resources["White"] as SolidColorBrush;
                    else
                        path.Fill = Application.Current.Resources["MainColor"] as SolidColorBrush;
                }
                foreach (var control in catalogMenuList)
                {
                    TextBlock textBlock = FindControls.FindVisualChildren<TextBlock>(this).Where(x => x.Tag != null && x.Name == "txt_" + control)
                        .FirstOrDefault();
                    if (textBlock.Tag.ToString() == senderTag)
                        textBlock.Foreground = Application.Current.Resources["White"] as SolidColorBrush;
                    else
                        textBlock.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
                }
                #endregion
                //refreshCatalogTags(senderTag);
            }
            catch { }
        }

        #endregion

        #region events
        private async void Tb_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);
                //await Search();
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Tgl_isActive_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                /*
                HelpClass.StartAwait(grid_main);
                if (tables is null)
                    await RefreshTablesList();
                tgl_tableState = 1;
                await Search();
                HelpClass.EndAwait(grid_main);
                */
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Tgl_isActive_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                /*
                HelpClass.StartAwait(grid_main);
                if (tables is null)
                    await RefreshTablesList();
                tgl_tableState = 0;
                await Search();
                HelpClass.EndAwait(grid_main);
                */
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        //private void Btn_clear_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        HelpClass.StartAwait(grid_main);
        //        Clear();
        //        HelpClass.EndAwait(grid_main);
        //    }
        //    catch (Exception ex)
        //    {

        //        HelpClass.EndAwait(grid_main);
        //        HelpClass.ExceptionMessage(ex, this);
        //    }
        //}
        private async void Dg_items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                HelpClass.StartAwait(grid_main);


                //HelpClass.clearValidate(requiredControlList, this);
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async void Btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {//refresh

                HelpClass.StartAwait(grid_main);
                /*
                await RefreshUsersList();
                await Search();
                */
                HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {

                HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion
        #region Refresh & Search
        /*
        async Task Search()
        {
            //search
            if (users is null)
                await RefreshUsersList();
            searchText = tb_search.Text.ToLower();
            usersQuery = users.Where(s => (s.name.ToLower().Contains(searchText) ||
            s.job.ToLower().Contains(searchText)
            ) && s.isActive == tgl_userState);
            RefreshCustomersView();
        }
        async Task<IEnumerable<User>> RefreshUsersList()
        {

            users = await user.Get();
            return users;

        }
        void RefreshCustomersView()
        {
            dg_user.ItemsSource = usersQuery;
            txt_count.Text = usersQuery.Count().ToString();
        }
        */
        #endregion
       


        
    }
}
