// <copyright file="BoulderControl.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NIK.BoulderDash.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;
    using NIK.BoulderDash.Logic;

    /// <summary>
    /// Class BoulderControl. Can be rendered.
    /// Implements the <see cref="System.Windows.FrameworkElement" />.
    /// </summary>
    /// <seealso cref="System.Windows.FrameworkElement" />
    public class BoulderControl : FrameworkElement
    {
        private DispatcherTimer logicCalcTimer;
        private GameLogic logic;
        private BoulderDisplay display;
        private GameModel model;
        private Dictionary<string, VisualBrush> animatedVisualBrushes = new Dictionary<string, VisualBrush>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BoulderControl"/> class.
        /// </summary>
        public BoulderControl()
        {
            this.Loaded += this.BoulderControl_Loaded;
        }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.display != null)
            {
                drawingContext.DrawDrawing(this.display.BuildDrawing());
            }
        }

        private void BoulderControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window win = Window.GetWindow(this);
            if (true)
            {
                this.logicCalcTimer = new DispatcherTimer();
                this.logicCalcTimer.Interval = TimeSpan.FromMilliseconds(GameModel.MOVETIME);
                this.logicCalcTimer.Tick += this.TimerTick;
                Task.Delay(GameModel.MOVETIME * 4).ContinueWith(_ =>
                {
                    this.logicCalcTimer.Start();
                });
            }

            foreach (DictionaryEntry item in Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true))
            {
                if ((item.Value as System.Drawing.Bitmap) != null)
                {
                    var brush = this.TryFindResource(item.Key.ToString());
                    if (brush is VisualBrush)
                    {
                        this.animatedVisualBrushes[item.Key.ToString()] = brush as VisualBrush;
                    }
                }
            }

            this.logic = new GameLogic();
            this.model = this.logic.LoadLevel(Properties.Resources.AL10);
            this.display = new BoulderDisplay(this.model, this.ActualWidth, this.ActualHeight, GameModel.MOVETIME, this.animatedVisualBrushes);
            this.InvalidateVisual();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            this.logic.OneTick();
            this.InvalidateVisual();
        }
    }
}
