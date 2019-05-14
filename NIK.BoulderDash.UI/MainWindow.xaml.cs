// <copyright file="MainWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NIK.BoulderDash.UI
{
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private byte[] map;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.WindowState = WindowState.Maximized;
            this.InitializeComponent();
        }

        /// <summary>
        /// Loads the selected map.
        /// </summary>
        /// <param name="map">The map.</param>
        public void LoadMap(byte[] map)
        {
            this.map = map;
            (this.FindName("control") as BoulderControl).LoadMap(map);
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as Window).WindowState = WindowState.Normal;
            (sender as Window).Width = ((sender as Window).ActualHeight / 12) * 20;
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                MainMenu menu = new MainMenu();
                menu.Show();
                this.Close();
            }

            if (e.Key == Key.Space)
            {
                (this.FindName("control") as BoulderControl).Restart();
            }
        }
    }
}
