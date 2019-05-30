// <copyright file="MainMenu.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NIK.BoulderDash.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using NIK.BoulderDash.Logic;

    /// <summary>
    /// Interaction logic for MianMenu.xaml.
    /// </summary>
    public partial class MainMenu : Window
    {
        private MyMenuItem selectedMenu;
        private GameLogic logic = new GameLogic(() => { });

        private GameModel model;
        private BoulderDisplay display;
        private Dictionary<string, byte[]> levels = new Dictionary<string, byte[]>();
        private List<Bitmap> rs = new List<Bitmap>();
        private Random r = new Random();
        private Grid mapp = null;
        private Dictionary<string, VisualBrush> animatedVisualBrushes = new Dictionary<string, VisualBrush>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenu"/>  class.
        /// This is the main menu.
        /// </summary>
        public MainMenu()
        {
            this.WindowState = WindowState.Maximized;
            foreach (DictionaryEntry item in Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true))
            {
                if (item.Value is byte[])
                {
                    this.levels[item.Key.ToString()] = item.Value as byte[];
                }
            }

            foreach (DictionaryEntry item in Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true))
            {
                if (item.Value is Bitmap)
                {
                    this.rs.Add(item.Value as Bitmap);
                }
            }

            this.InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.PreviewKeyDown += this.Window_KeyDown;
            foreach (DictionaryEntry item in Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true))
            {
                if (item.Value is System.Drawing.Bitmap)
                {
                    var brush = this.TryFindResource(item.Key.ToString());
                    if (brush is VisualBrush)
                    {
                        this.animatedVisualBrushes[item.Key.ToString()] = brush as VisualBrush;
                    }
                }
            }

            this.mapp = this.FindName("map") as Grid;
            ListBox lb = this.FindName("lbMain") as ListBox;
            lb.Focus();
            var rss = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            foreach (DictionaryEntry item in rss)
            {
                if (item.Value is byte[])
                {
                    (lb as ListBox).Items.Add(new MyMenuItem() { Name = item.Key.ToString(), Map = item.Value as byte[] });
                }
            }

            lb.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription(string.Empty, System.ComponentModel.ListSortDirection.Descending));
            lb.SelectedIndex = 0;
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void LbMain_Selected(object sender, RoutedEventArgs e)
        {
            this.selectedMenu = (sender as ListBox).SelectedItem as MyMenuItem;
            this.model = this.logic.LoadLevel(this.levels[this.selectedMenu.Name]);
            this.model.Camera.AngleWidthTile = 400;
            this.model.Camera.AngleHeightTile = 220;
            this.display = new BoulderDisplay(this.model, this.ActualWidth, this.ActualHeight, 200, this.animatedVisualBrushes);

            this.map.Background = new DrawingBrush(this.display.BuildDrawing());
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                MainWindow win = new MainWindow();
                win.LoadMap(this.levels, this.selectedMenu.Name);
                win.Show();
                this.Close();
            }
        }

        private void LbMain_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
    }
}
