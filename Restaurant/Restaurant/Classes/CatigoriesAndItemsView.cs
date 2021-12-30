using Restaurant.Classes;
using Restaurant.controlTemplate;
using Restaurant.View;
using Restaurant.View.catalog;
using Restaurant.View.catalog.foods;
using Restaurant.View.catalog.rawMaterials;
using Restaurant.View.sales;
using Restaurant.View.storage;
using Restaurant.View.windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Restaurant.Classes
{
    public class CatigoriesAndItemsView 
    {



        public uc_diningHall ucdiningHall;
        public uc_itemsRawMaterials ucitemsRawMaterials;
        public uc_itemsFoods ucitemsFoods;

        public Grid gridCatigories;
        public Grid gridCatigorieItems;
        private int _idItem;
        //private int _iddiningHall;
        //private int _iditemsRawMaterials;
        //private int _iditemsFoods;

        public int idItem
        {
            get => _idItem; set
            {
                _idItem = value;
                INotifyPropertyChangedIdCatigorieItems();
            }
        }
        //public int idwdItems
        //{
        //    get => _iddiningHall; set
        //    {
        //        _iddiningHall = value;
        //        INotifyPropertyChangedIdCatigorieItems();
        //    }
        //}

        private  void INotifyPropertyChangedIdCatigorieItems()
        {
            if (ucdiningHall != null)
            {
                ucdiningHall.ChangeItemIdEvent(idItem);
            }
            
            if (ucitemsRawMaterials != null)
            {
                ucitemsRawMaterials.ChangeItemIdEvent(idItem);
            }
            if (ucitemsFoods != null)
            {
                ucitemsFoods.ChangeItemIdEvent(idItem);
            }
        }
      
        #region Catalog Items


        private int pastCatalogItem = -1;
        double gridCatigorieItems_ActualHeight = 0;
        double gridCatigorieItems_ActualWidth = 0;
        public void  FN_refrishCatalogItem(List<Item> items)
        {
            gridCatigorieItems.Children.Clear();
            gridCatigorieItems_ActualHeight = gridCatigorieItems.ActualHeight/3;
            gridCatigorieItems_ActualWidth = gridCatigorieItems.ActualWidth/5;
            int row = 0;
            int column = 0;
            foreach (var item in items)
            {

                CardViewItems itemCardView = new CardViewItems();
                itemCardView.item = item;
                itemCardView.row = row;
                itemCardView.column = column;
                FN_createRectangelCard(itemCardView);
               

                column++;
                if (column == 5)
                {
                    column = 0;
                    row++;
                }
            }
        }

        uc_squareCard FN_createRectangelCard(CardViewItems itemCardView, string BorderBrush = "#DFDFDF")
        {
            uc_squareCard uc = new uc_squareCard(itemCardView, gridCatigorieItems_ActualHeight , gridCatigorieItems_ActualWidth);
            uc.squareCardBorderBrush = BorderBrush;
            uc.Name = "CardName" + itemCardView.item.itemId;
            Grid.SetRow(uc, itemCardView.row);
            Grid.SetColumn(uc, itemCardView.column);
            gridCatigorieItems.Children.Add(uc);
            uc.MouseDown += this.ucItemMouseDown;
            return uc;
        }
        private void ucItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 0)
                doubleClickItem(sender);
        }
        private void doubleClickItem(object sender)
        {
            try
            {
                uc_squareCard uc = (uc_squareCard)sender;
                uc = gridCatigorieItems.Children.OfType<uc_squareCard>().Where(x => x.Name.ToString() == "CardName" + uc.cardViewitem.item.itemId).FirstOrDefault();

                uc.squareCardBorderBrush = "#D35400";

                if (pastCatalogItem != -1 && pastCatalogItem != uc.cardViewitem.item.itemId)
                {
                    var pastUc = new uc_squareCard() { contentId = pastCatalogItem };
                    pastUc = gridCatigorieItems.Children.OfType<uc_squareCard>().Where(x => x.Name.ToString() == "CardName" + pastUc.contentId).FirstOrDefault();
                    if (pastUc != null)
                    {
                        pastUc.squareCardBorderBrush = "#DFDFDF";
                    }

                }
                pastCatalogItem = uc.cardViewitem.item.itemId;
                idItem = uc.cardViewitem.item.itemId;
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        #endregion



    }
}
