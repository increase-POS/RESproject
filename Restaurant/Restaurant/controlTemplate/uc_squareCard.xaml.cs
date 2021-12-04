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

namespace Restaurant.controlTemplate
{
    /// <summary>
    /// Interaction logic for uc_squareCard.xaml
    /// </summary>
    public partial class uc_squareCard : UserControl
    {
        public uc_squareCard()
        {
            InitializeComponent();
        }
        public int contentId { get; set; }
        public CardViewItems cardViewitem { get; set; }
        public uc_squareCard(CardViewItems _CardViewitems)
        {
            InitializeComponent();
            cardViewitem = _CardViewitems;
        }
        void CreateItemCard()
        {

            #region Grid Container
            Grid gridContainer = new Grid();
            //int rowCount = 3;
            //RowDefinition[] rd = new RowDefinition[4];
            //for (int i = 0; i < rowCount; i++)
            //{
            //    rd[i] = new RowDefinition();
            //}
            //rd[0].Height = new GridLength(5, GridUnitType.Star);
            //rd[1].Height = new GridLength(25, GridUnitType.Auto);
            //rd[2].Height = new GridLength(25, GridUnitType.Auto);
            //for (int i = 0; i < rowCount; i++)
            //{
            //    gridContainer.RowDefinitions.Add(rd[i]);
            //}
            /////////////////////////////////////////////////////
            //if (this.ActualHeight != 0)
            //    gridContainer.Height = this.ActualHeight ;
            //if (this.ActualHeight != 0)
            //    gridContainer.Width = this.ActualWidth  ;
            /////////////////////////////////////////////////////
            
            brd_main.Child = gridContainer;
            if (brd_main.ActualHeight > brd_main.ActualWidth)
                brd_main.Height = brd_main.ActualWidth;
            else
                brd_main.Width = brd_main.ActualHeight;

            #endregion
            grid_main.FlowDirection = FlowDirection.LeftToRight;
            #region Image
            Item item = new Item();
            Button buttonImage = new Button();
            buttonImage.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
            //buttonImage.Height = (gridContainer.Height / 1.1) - 7.5;
            //buttonImage.Width = ((gridContainer.Width / 2.2) / 1.2) - 7.5;
            buttonImage.Height = brd_main.ActualHeight -2;
            buttonImage.Width = buttonImage.Height;
            buttonImage.BorderThickness = new Thickness(0);
            buttonImage.Padding = new Thickness(0);
            buttonImage.FlowDirection = FlowDirection.LeftToRight;
            MaterialDesignThemes.Wpf.ButtonAssist.SetCornerRadius(buttonImage, (new CornerRadius(10)));
            bool isModified = HelpClass.chkImgChng(cardViewitem.item.image, (DateTime)cardViewitem.item.updateDate, Global.TMPItemsFolder);
            if (isModified)
                HelpClass.getImg("Item", cardViewitem.item.image, buttonImage);
            else
                HelpClass.getLocalImg("Item", cardViewitem.item.image, buttonImage);
            //Grid grid_image = new Grid();
            //grid_image.Height = buttonImage.Height - 2;
            //grid_image.Width = buttonImage.Width - 1;
            //grid_image.Children.Add(buttonImage);

            gridContainer.Children.Add(buttonImage);

            //////////////
            #endregion

            #region   Title
            var titleText = new TextBlock();
            titleText.Text = cardViewitem.item.name;
            //titleText.FontSize = 12;
            //titleText.FontFamily = App.Current.Resources["Font-cairo-bold"] as FontFamily;
            titleText.Margin = new Thickness(1 ,5 ,1, 1);
            titleText.FontWeight = FontWeights.Bold;
            titleText.VerticalAlignment = VerticalAlignment.Center;
            titleText.HorizontalAlignment = HorizontalAlignment.Center;
            //titleText.TextWrapping = TextWrapping.Wrap;
            titleText.Foreground = Application.Current.Resources["MainColor"] as SolidColorBrush;
            Grid.SetRow(titleText, 1);
            /////////////////////////////////

            #endregion
            #region  subTitle
            var subTitleText = new TextBlock();
            try
            {
                subTitleText.Text = HelpClass.DecTostring(cardViewitem.item.priceTax);
            }
            catch
            {
                subTitleText.Text = "";
            }
            subTitleText.Margin = new Thickness(1);
            //subTitleText.FontWeight = FontWeights.Regular;
            subTitleText.VerticalAlignment = VerticalAlignment.Center;
            subTitleText.HorizontalAlignment = HorizontalAlignment.Center;
            //subTitleText.FontSize = 10;
            //subTitleText.TextWrapping = TextWrapping.Wrap;
            subTitleText.Foreground = Application.Current.Resources["SecondColor"] as SolidColorBrush;
            Grid.SetRow(subTitleText, 2);
            /////////////////////////////////

            #endregion

            if (cardViewitem.item.isNew == 1)
            {
               
                #region Path newLabel
                Path pathNewLabel = new Path();
                Grid.SetRowSpan(pathNewLabel, 4);
                pathNewLabel.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#D20707"));
                pathNewLabel.Stretch = Stretch.Fill;
                Grid.SetColumnSpan(pathNewLabel, 2);
                //pathStar.Height = 18;
                //pathStar.Width = 54;
                //pathNewLabel.VerticalAlignment = VerticalAlignment.Bottom;
                //pathNewLabel.HorizontalAlignment = HorizontalAlignment.Right;
                pathNewLabel.FlowDirection = FlowDirection.LeftToRight;
                //pathNewLabel.Margin = new Thickness(7.5);
                pathNewLabel.Data = App.Current.Resources["newBlock"] as Geometry;
                pathNewLabel.Width = gridContainer.Width / 6.5;
                pathNewLabel.Height = pathNewLabel.Width / 3;
                #region Text
                Path pathNewLabelText = new Path();
                Grid.SetRowSpan(pathNewLabelText, 4);
                pathNewLabelText.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFD00"));
                pathNewLabelText.Stretch = Stretch.Fill;
                Grid.SetColumnSpan(pathNewLabelText, 2);

                //pathNewLabelText.VerticalAlignment = VerticalAlignment.Bottom;
                //pathNewLabelText.HorizontalAlignment = HorizontalAlignment.Right;
                pathNewLabelText.FlowDirection = FlowDirection.LeftToRight;
                //pathNewLabelText.Margin = new Thickness(7.5);
                //pathNewLabelText.Margin = new Thickness(0, 0, 12.5, 10);
                pathNewLabelText.Data = App.Current.Resources["newText"] as Geometry;
                //pathStar.Height = 18;
                //pathStar.Width = 54;
                pathNewLabelText.Width = gridContainer.Width / 10;
                pathNewLabelText.Height = pathNewLabelText.Width / 3;
                #endregion
                #endregion

                Grid gridNewContainer = new Grid();
                Grid.SetRowSpan(gridNewContainer, 4);
                Grid.SetColumnSpan(gridNewContainer, 2);
                gridNewContainer.VerticalAlignment = VerticalAlignment.Bottom;
                gridNewContainer.HorizontalAlignment = HorizontalAlignment.Right;
                gridNewContainer.Margin = new Thickness(7.5);

                gridNewContainer.Children.Add(pathNewLabel);
                gridNewContainer.Children.Add(pathNewLabelText);

                gridContainer.Children.Add(gridNewContainer);

                //gridContainer.Children.Add(pathNewLabel);
                //gridContainer.Children.Add(pathNewLabelText);
            }
            if (cardViewitem.item.isOffer == 1)
            {
                #region Path offerLabel
                //string dataStar = "";
                Path pathOfferLabel = new Path();
                Grid.SetColumnSpan(pathOfferLabel, 2);
                Grid.SetRowSpan(pathOfferLabel, 4);
                pathOfferLabel.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#D20707"));
                pathOfferLabel.VerticalAlignment = VerticalAlignment.Top;
                pathOfferLabel.Stretch = Stretch.Fill;
                //   Height = "16" Width = "86" 
                pathOfferLabel.Height = pathOfferLabel.Width = gridContainer.Width / 4.5;
                pathOfferLabel.FlowDirection = FlowDirection.LeftToRight;
                pathOfferLabel.HorizontalAlignment = HorizontalAlignment.Right;

                if (MainWindow.lang.Equals("ar"))
                {
                    pathOfferLabel.Data = App.Current.Resources["offerLabelArTopLeft"] as Geometry;
                }
                else
                {
                    pathOfferLabel.Data = App.Current.Resources["offerLabelEnTopRight"] as Geometry;
                }

                #region Text
                Path pathOfferLabelText = new Path();
                Grid.SetColumnSpan(pathOfferLabelText, 2);
                Grid.SetRowSpan(pathOfferLabelText, 4);
                pathOfferLabelText.FlowDirection = FlowDirection.LeftToRight;
                pathOfferLabelText.VerticalAlignment = VerticalAlignment.Top;
                pathOfferLabelText.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFD00"));
                pathOfferLabelText.Stretch = Stretch.Fill;
                pathOfferLabelText.HorizontalAlignment = HorizontalAlignment.Right;
                if (MainWindow.lang.Equals("ar"))
                {
                    pathOfferLabelText.Height = pathOfferLabelText.Width = gridContainer.Width / 7;
                    pathOfferLabelText.Margin = new Thickness(0, 4, 4, 0);
                    pathOfferLabelText.Data = App.Current.Resources["offerLabelArTopLeft_Text"] as Geometry;
                }
                else

                {
                    pathOfferLabelText.Height = pathOfferLabelText.Width = gridContainer.Width / 6.5;
                    pathOfferLabelText.Margin = new Thickness(0, 2.5, 2.5, 0);
                    pathOfferLabelText.Data = App.Current.Resources["offerLabelEnTopRight_Text"] as Geometry;
                }

                #endregion
                #endregion
                gridContainer.Children.Add(pathOfferLabel);
                gridContainer.Children.Add(pathOfferLabelText);
            }
            //if (cardViewitem.item.itemCount > 0)
            //{
            //    this.ToolTip = MainWindow.resourcemanager.GetString("trCount: ") + cardViewitem.item.itemCount + " " + cardViewitem.item.unitName;
            //}
            //gridContainer.Children.Add(titleText);
            //gridContainer.Children.Add(subTitleText);
            grid_main.Children.Add(titleText);
            grid_main.Children.Add(subTitleText);
        }
        void InitializeControls()
        {
            //if (cardViewitem.cardType == "User")
            //    CreateUserCard(cardViewitem.cardType, cardViewitem.user.name, cardViewitem.user.job, cardViewitem.user.mobile, cardViewitem.user.image);
            //else if (cardViewitem.cardType == "Agent")
            //    CreateUserCard(cardViewitem.cardType, cardViewitem.agent.name, cardViewitem.agent.company, cardViewitem.agent.mobile, cardViewitem.agent.image);
            //else
                CreateItemCard();

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;
            InitializeControls();
        }
        #region squareCardBorderBrush
        public static readonly DependencyProperty squareCardBorderBrushDependencyProperty = DependencyProperty.Register("squareCardBorderBrush",
            typeof(string),
            typeof(uc_squareCard),
            new PropertyMetadata("DEFAULT"));
        public string squareCardBorderBrush
        {
            set
            { SetValue(squareCardBorderBrushDependencyProperty, value); }
            get
            { return (string)GetValue(squareCardBorderBrushDependencyProperty); }
        }
        #endregion
    }
}
