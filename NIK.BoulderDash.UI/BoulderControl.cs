using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using NIK.BoulderDash.Logic;

namespace NIK.BoulderDash.UI
{
    public class BoulderControl : FrameworkElement
    {
        
        DispatcherTimer logicCalcTimer;
        GameLogic logic;
        BoulderDisplay display;
        GameModel model;
        Dictionary<string, VisualBrush> animatedVisualBrushes = new Dictionary<string, VisualBrush>();
        
        public BoulderControl()
        {
            Loaded += BoulderControl_Loaded;
        }

        private void BoulderControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window win = Window.GetWindow(this);
            if (true)
            {
                logicCalcTimer = new DispatcherTimer();      
                logicCalcTimer.Interval = TimeSpan.FromMilliseconds(GameModel.MOVETIME);
                logicCalcTimer.Tick += timerTick;
                Task.Delay(GameModel.MOVETIME*4).ContinueWith(_ =>
                {
                    logicCalcTimer.Start();
                });
            }

            foreach (DictionaryEntry item in Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true))
            {
                if ((item.Value as System.Drawing.Bitmap) != null)
                {
                    var brush = TryFindResource(item.Key.ToString());
                    if (brush is VisualBrush)
                    {
                        animatedVisualBrushes[item.Key.ToString()] = brush as VisualBrush;
                    }
                }
            }

            logic = new GameLogic();
            model = logic.LoadLevel(Properties.Resources.AL03);
            display = new BoulderDisplay(model, ActualWidth, ActualHeight, GameModel.MOVETIME, animatedVisualBrushes);
            InvalidateVisual();
        }

        private void timerTick(object sender, EventArgs e)
        {
            logic.OneTick();
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (display != null)
            {
                drawingContext.DrawDrawing(display.BuildDrawing());
            }
        }
    }
}
