using MaterialDesignThemes.Wpf;
using Restaurant.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Resources;
using System.Windows.Shapes;

namespace Restaurant.View.sales
{
    /// <summary>
    /// Interaction logic for uc_diningHall.xaml
    /// </summary>
    public partial class uc_diningHall : UserControl
    {
        private static uc_diningHall _instance;
        public static uc_diningHall Instance
        {
            get
            {
                _instance = new uc_diningHall();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_diningHall()
        {
            try
            {
                InitializeComponent();
            }
            catch
            { }
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // for pagination onTop Always
            btns = new Button[] { btn_firstPage, btn_prevPage, btn_activePage, btn_nextPage, btn_lastPage };
            catigoriesAndItemsView.ucdiningHall = this;
            FillBillDetailsList(0);
            await Search();
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }
        #region  Cards
        CatigoriesAndItemsView catigoriesAndItemsView = new CatigoriesAndItemsView();
        #region Refrish Y
        Item item = new Item();
        IEnumerable<Item> items;
        IEnumerable<Item> itemsQuery;
        async Task<IEnumerable<Item>> RefreshItemsList()
        {
            items = await item.GetAllItems();
            return items;
        }
        void RefrishItemsCard(IEnumerable<Item> _items)
        {
            grid_itemContainerCard.Children.Clear();
            catigoriesAndItemsView.gridCatigorieItems = grid_itemContainerCard;
            catigoriesAndItemsView.FN_refrishCatalogItem(_items.ToList());
        }
        #endregion
        #region Get Id By Click  Y

        public void ChangeItemIdEvent(int itemId)
        {
           
           
        }
        #endregion
        #region Search Y


        /// <summary>
        /// Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async Task Search()
        {
            //search
            try
            {
                    HelpClass.StartAwait(grid_main);

                if (items is null)
                    await RefreshItemsList();
                itemsQuery = items.ToList();
                pageIndex = 1;

                #region
                

                if (btns is null)
                    btns = new Button[] { btn_firstPage, btn_prevPage, btn_activePage, btn_nextPage, btn_lastPage };
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
                #endregion

                    HelpClass.EndAwait(grid_main);
            }
            catch (Exception ex)
            {
                    HelpClass.EndAwait(grid_main);
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion
        #region Pagination Y
        Pagination pagination = new Pagination();
        Button[] btns;
        public int pageIndex = 1;

        private void Tb_pageNumberSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);

                itemsQuery = items.ToList();

                if (tb_pageNumberSearch.Text.Equals(""))
                {
                    pageIndex = 1;
                }
                else if (((itemsQuery.Count() - 1) / 9) + 1 < int.Parse(tb_pageNumberSearch.Text))
                {
                    pageIndex = ((itemsQuery.Count() - 1) / 9) + 1;
                }
                else
                {
                    pageIndex = int.Parse(tb_pageNumberSearch.Text);
                }

                #region
                
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
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


        private void Btn_firstPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);

                pageIndex = 1;
                #region
                itemsQuery = items.ToList();
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
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
        private void Btn_prevPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);

                pageIndex = int.Parse(btn_prevPage.Content.ToString());
                #region
                itemsQuery = items.ToList();

                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
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
        private void Btn_activePage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);

                pageIndex = int.Parse(btn_activePage.Content.ToString());
                #region
                itemsQuery = items.ToList();
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
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
        private void Btn_nextPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);

                pageIndex = int.Parse(btn_nextPage.Content.ToString());
                #region
                itemsQuery = items.ToList();
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
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
        private void Btn_lastPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                    HelpClass.StartAwait(grid_main);

                itemsQuery = items.ToList();
                pageIndex = ((itemsQuery.Count() - 1) / 9) + 1;
                #region
                itemsQuery = items.ToList();
                RefrishItemsCard(pagination.refrishPagination(itemsQuery, pageIndex, btns));
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
        #endregion
        #endregion

        #region invoice
        ObservableCollection<BillDetails> billDetailsList = new ObservableCollection<BillDetails>();

        public class BillDetails
        {
            public int index { get; set; }
            public string image { get; set; }
            public int itemId { get; set; }
            public string itemName { get; set; }
            public int Count { get; set; }
            public decimal Price { get; set; }
            public decimal Total { get; set; }
            public decimal Tax { get; set; }
            public int? offerId { get; set; }
           
        }
        private async Task FillBillDetailsList(int invoiceId)
        {
            ////get invoice items
            //invoiceItems = await invoiceModel.GetInvoicesItems(invoiceId);
            //// build invoice details grid
            billDetailsList.Clear();
            //test
            Random r = new Random();
            //int rInt = r.Next(0, 100);
                for (int i = 0; i < 13; i++)
                {

                 int   _Count = r.Next(0, 25);
                Decimal _Price = (r.Next(0, 10000)) / 100;
                billDetailsList.Add(new BillDetails()
                {
                    index = i,
                    image = "/pic/90408.jpg",
                    itemId = (i + 1),
                    itemName = "name - " + (i + 1),
                    Count = _Count,
                    Price = _Price,
                    Total = _Count * _Price,
                    offerId =0,
                });
            }
            //
            //int indexer = 0;
            //foreach (ItemTransfer itemT in invoiceItems)
            //{
            //    decimal total = (decimal)(itemT.price * itemT.quantity);
            //    billDetailsList.Add(new BillDetails()
            //    {
            //        ID = indexer,
            //        Product = itemT.itemName,
            //        itemId = (int)itemT.itemId,
            //        Unit = itemT.itemUnitId.ToString(),
            //        itemUnitId = (int)itemT.itemUnitId,
            //        Count = (int)itemT.quantity,
            //        Price = (decimal)itemT.price,
            //        Total = total,
            //        serialList = serialNumLst,
            //        type = itemT.itemType,
            //        valid = isValid,
            //        offerId = itemT.offerId,
            //    });
            //    indexer++;
            //}
            BuildBillDesign();
        }
        void BuildBillDesign()
        {
            //sv_billDetail.Children.Clear();
            #region Grid Container
            Grid gridContainer = new Grid();
            gridContainer.Margin = new Thickness(5);
            //int rowCount = billDetailsList.Count();
            int rowCount = 13;
            RowDefinition[] rd = new RowDefinition[rowCount];
            for (int i = 0; i < rowCount; i++)
            {
                rd[i] = new RowDefinition();
                rd[i].Height = new GridLength(1, GridUnitType.Auto);
            }
            for (int i = 0; i < rowCount; i++)
            {
                gridContainer.RowDefinitions.Add(rd[i]);
            }
            /////////////////////////////////////////////////////
            int colCount = 7;
            ColumnDefinition[] cd = new ColumnDefinition[colCount];
            for (int i = 0; i < colCount; i++)
            {
                cd[i] = new ColumnDefinition();
            }
            cd[0].Width = new GridLength(1, GridUnitType.Auto);
            cd[1].Width = new GridLength(1, GridUnitType.Auto);
            cd[2].Width = new GridLength(1, GridUnitType.Star);
            cd[3].Width = new GridLength(1, GridUnitType.Auto);
            cd[4].Width = new GridLength(1, GridUnitType.Auto);
            cd[5].Width = new GridLength(1, GridUnitType.Auto);
            cd[6].Width = new GridLength(1, GridUnitType.Star);
            for (int i = 0; i < colCount; i++)
            {
                gridContainer.ColumnDefinitions.Add(cd[i]);
            }
            /////////////////////////////////////////////////////



            #endregion
            foreach (var item in billDetailsList)
            {
                #region   index
                var indexText = new TextBlock();
                indexText.Text = (item.index + 1)+".";
                indexText.Tag = item.index;
                indexText.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                indexText.FontSize = 14;
                indexText.FontWeight = FontWeights.Bold;
                indexText.Margin = new Thickness(5);
                indexText.VerticalAlignment = VerticalAlignment.Center;
                indexText.HorizontalAlignment = HorizontalAlignment.Center;

                Grid.SetRow(indexText, item.index);
                Grid.SetColumn(indexText, 0);
                gridContainer.Children.Add(indexText);
                /////////////////////////////////

                #endregion
                #region   image
                Button buttonImage = new Button();
                buttonImage.Tag = item.index;
                buttonImage.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
                buttonImage.Height =  
                buttonImage.Width = 40;
                buttonImage.Margin = new Thickness(5 , 10, 5, 10);
                buttonImage.VerticalAlignment = VerticalAlignment.Center;
                buttonImage.HorizontalAlignment = HorizontalAlignment.Center;
                buttonImage.BorderThickness = new Thickness(1);
                buttonImage.BorderBrush = Application.Current.Resources["Grey"] as SolidColorBrush;
                buttonImage.Padding = new Thickness(0);
                buttonImage.FlowDirection = FlowDirection.LeftToRight;
                MaterialDesignThemes.Wpf.ButtonAssist.SetCornerRadius(buttonImage, (new CornerRadius(10)));
                //bool isModified = HelpClass.chkImgChng(cardViewitem.item.image, (DateTime)cardViewitem.item.updateDate, Global.TMPItemsFolder);
                //if (isModified)
                //    HelpClass.getImg("Item", cardViewitem.item.image, buttonImage);
                //else
                //    HelpClass.getLocalImg("Item", cardViewitem.item.image, buttonImage);
                fillEllipse(buttonImage);

                Grid.SetRow(buttonImage, item.index);
                Grid.SetColumn(buttonImage, 1);
                gridContainer.Children.Add(buttonImage);
                /////////////////////////////////

                #endregion
                #region   name
                var itemNameText = new TextBlock();
                itemNameText.Text = item.itemName;
                itemNameText.Tag = item.index;
                itemNameText.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                itemNameText.FontSize = 14;
                itemNameText.Margin = new Thickness(5);
                itemNameText.VerticalAlignment = VerticalAlignment.Center;
                itemNameText.HorizontalAlignment = HorizontalAlignment.Left;
                itemNameText.FontWeight = FontWeights.Bold;

                Grid.SetRow(itemNameText, item.index);
                Grid.SetColumn(itemNameText, 2);
                gridContainer.Children.Add(itemNameText);
                #endregion
                #region   Count
                var countText = new TextBlock();
                countText.Text = item.Count.ToString();
                countText.Tag = item.index;
                countText.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                countText.FontSize = 14;
                countText.FontWeight = FontWeights.Bold;
                countText.Margin = new Thickness(5);
                countText.VerticalAlignment = VerticalAlignment.Center;
                countText.HorizontalAlignment = HorizontalAlignment.Center;

                Grid.SetRow(countText, item.index);
                Grid.SetColumn(countText, 4);
                gridContainer.Children.Add(countText);
                /////////////////////////////////

                #endregion
                #region   Total
                var totalText = new TextBlock();
                totalText.Text = item.Total.ToString();
                totalText.Tag = item.index;
                totalText.Foreground = Application.Current.Resources["Grey"] as SolidColorBrush;
                totalText.Margin = new Thickness(5);
                totalText.VerticalAlignment = VerticalAlignment.Center;
                totalText.HorizontalAlignment = HorizontalAlignment.Right;

                Grid.SetRow(totalText, item.index);
                Grid.SetColumn(totalText, 6);
                gridContainer.Children.Add(totalText);
                /////////////////////////////////

                #endregion

                #region   Minus
                //                         Width = "25" Height = "25" Kind = "Minus" />

                Button buttonMinus = new Button();
                buttonMinus.Tag = item.index;
                buttonMinus.Margin = new Thickness(2.5);
                buttonMinus.BorderThickness = new Thickness(0);
                buttonMinus.Height =
                buttonMinus.Width = 30;
                buttonMinus.Padding = new Thickness(0);
                buttonMinus.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#DFDFDF"));

                #region materialDesign
                var MinusPackIcon = new PackIcon();
                MinusPackIcon.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
                MinusPackIcon.Height =
                MinusPackIcon.Width = 30;
                MinusPackIcon.Kind = PackIconKind.Minus;
                #endregion
                buttonMinus.Content = MinusPackIcon;

                Grid.SetRow(buttonMinus, item.index);
                Grid.SetColumn(buttonMinus, 3);
                gridContainer.Children.Add(buttonMinus);
                /////////////////////////////////

                #endregion
                /*

            <!--#region  1-->
                                
                                
                                
                                <Button Tag="ssssssss" Grid.Column="5"    Margin="2.5"
                                    Height="25" Width="25"  Padding="0"
                                        Background="#DFDFDF"   BorderThickness="0">
                                    <materialDesign:PackIcon   Foreground="{StaticResource SecondColor}"
                                         Width="25" Height="25" Kind="Plus" />
                                </Button>
                               
                                <!--#endregion-->
                  
                 * */
            }
            sv_billDetail.Content = gridContainer;

        }

        void fillEllipse(Button img)
        {
            try
            {
                ImageBrush myBrush = new ImageBrush();
                Uri resourceUri = new Uri("/pic/90408.jpg", UriKind.Relative);
                StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
                BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
                myBrush.ImageSource = temp;
                img.Background = myBrush;
            }
            catch
            { }
        }
       
        #endregion
    }
}
