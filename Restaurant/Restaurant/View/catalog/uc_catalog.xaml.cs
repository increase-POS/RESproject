﻿using Restaurant.Classes;
using Restaurant.View.catalog.foods;
using Restaurant.View.catalog.rawMaterials;
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

namespace Restaurant.View.catalog
{
    /// <summary>
    /// Interaction logic for uc_catalog.xaml
    /// </summary>
    public partial class uc_catalog : UserControl
    {
        private static uc_catalog _instance;
        public static uc_catalog Instance
        {
            get
            {
                _instance = new uc_catalog();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        public uc_catalog()
        {
            try
            {
                InitializeComponent();
            }
            catch
            { }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
            GC.Collect();
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                #region translate
                if (AppSettings.lang.Equals("en"))
                    grid_main.FlowDirection = FlowDirection.LeftToRight;
                else
                    grid_main.FlowDirection = FlowDirection.RightToLeft;
                await translate();
                #endregion
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
        private async Task translate()
        {
            if (FillCombo.objectsList is null)
                await FillCombo.RefreshObjects();
            // Title
            if (!string.IsNullOrWhiteSpace(FillCombo.objectsList.Where(x => x.name == this.Tag.ToString()).FirstOrDefault().translate))
                txt_mainTitle.Text = AppSettings.resourcemanager.GetString(
               FillCombo.objectsList.Where(x => x.name == this.Tag.ToString()).FirstOrDefault().translate
               );
            // Info
            List<TextBlock> InfoTextBlocksList = FindControls.FindVisualChildren<TextBlock>(this)
                .Where(x => x.Name.Contains("Info") && x.Tag != null).ToList();
            foreach (var item in InfoTextBlocksList)
            {
                if (!string.IsNullOrWhiteSpace(FillCombo.objectsList.Where(x => x.name == item.Tag.ToString()).FirstOrDefault().translate))
                    item.Text = AppSettings.resourcemanager.GetString(
                   FillCombo.objectsList.Where(x => x.name == item.Tag.ToString()).FirstOrDefault().translate
                   );
            }
            // Hint
            List<TextBlock> HintTextBlocksList = FindControls.FindVisualChildren<TextBlock>(this)
                .Where(x => x.Name.Contains("Hint") && x.Tag != null).ToList();
            foreach (var item in HintTextBlocksList)
            {
                if (!string.IsNullOrWhiteSpace(FillCombo.objectsList.Where(x => x.name == item.Tag.ToString()).FirstOrDefault().translateHint))
                    item.Text = AppSettings.resourcemanager.GetString(
                   FillCombo.objectsList.Where(x => x.name == item.Tag.ToString()).FirstOrDefault().translateHint
                   );
            }
        }

        private void Btn_rawMaterials_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_rawMaterials.Instance);

                Button button = sender as Button;
                MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }

        private void Btn_foods_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                grid_main.Children.Clear();
                grid_main.Children.Add(uc_foods.Instance);

                Button button = sender as Button;
                MainWindow.mainWindow.initializationMainTrack(button.Tag.ToString());
            }
            catch (Exception ex)
            {
                HelpClass.ExceptionMessage(ex, this);
            }
        }
    }
}
