﻿using Restaurant.Classes;
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
            //if (true)
            {
               
                #region Path newLabel
                Path pathNewLabel = new Path();
                pathNewLabel.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#D20707"));
                pathNewLabel.Stretch = Stretch.Fill;
                pathNewLabel.FlowDirection = FlowDirection.LeftToRight;
                pathNewLabel.Data = App.Current.Resources["rectangleBlock"] as Geometry;
                pathNewLabel.Width = brd_main.ActualHeight / 2.5;
                pathNewLabel.Height = pathNewLabel.Width / 3;
                #region Text
                Path pathNewLabelText = new Path();
                pathNewLabelText.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFD00"));
                pathNewLabelText.Stretch = Stretch.Fill;
                pathNewLabelText.FlowDirection = FlowDirection.LeftToRight;
                pathNewLabelText.Data = App.Current.Resources["newText"] as Geometry;
                pathNewLabelText.Width = brd_main.ActualHeight / 4;
                pathNewLabelText.Height = pathNewLabelText.Width / 3;
                #endregion
                #endregion
                Grid gridNewContainer = new Grid();
                gridNewContainer.VerticalAlignment = VerticalAlignment.Bottom;
                gridNewContainer.HorizontalAlignment = HorizontalAlignment.Right;
                gridNewContainer.Margin = new Thickness(0, 7.5, 0, 7.5);
                gridNewContainer.Children.Add(pathNewLabel);
                gridNewContainer.Children.Add(pathNewLabelText);
                gridContainer.Children.Add(gridNewContainer);

            }
            if (cardViewitem.item.isOffer == 1)
                //if (true)
            {
                #region Path offerLabel
                Path pathOfferLabel = new Path();
                pathOfferLabel.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#D20707"));
                pathOfferLabel.Stretch = Stretch.Fill;
                pathOfferLabel.FlowDirection = FlowDirection.LeftToRight;
                pathOfferLabel.Data = App.Current.Resources["rectangleBlock"] as Geometry;
                pathOfferLabel.Width = brd_main.ActualHeight / 2.1;
                pathOfferLabel.Height = pathOfferLabel.Width / 3;
                #region Text
                Path pathOfferLabelText = new Path();
                pathOfferLabelText.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFD00"));
                pathOfferLabelText.Stretch = Stretch.Fill;
                pathOfferLabelText.FlowDirection = FlowDirection.LeftToRight;
                pathOfferLabelText.Data = App.Current.Resources["offerText"] as Geometry;
                pathOfferLabelText.Width = brd_main.ActualHeight /3;
                pathOfferLabelText.Height = pathOfferLabelText.Width / 3;
                #endregion
                #endregion
                Grid gridOfferContainer = new Grid();
                gridOfferContainer.VerticalAlignment = VerticalAlignment.Top;
                gridOfferContainer.HorizontalAlignment = HorizontalAlignment.Left;
                gridOfferContainer.Margin = new Thickness(0, 7.5, 0, 7.5);
                gridOfferContainer.Children.Add(pathOfferLabel);
                gridOfferContainer.Children.Add(pathOfferLabelText);
                gridContainer.Children.Add(gridOfferContainer);
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
