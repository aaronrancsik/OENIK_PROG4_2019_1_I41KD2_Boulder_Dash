using System;
using System.Collections.Generic;
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
        const int MOVETIME = 125;
        DispatcherTimer timer;
        DispatcherTimer timer2;
        VisualBrush diamonvb;
        VisualBrush rockfordvb;
        GameLogic logic;
        BoulderDisplay display;
        GameModel model;
        
        public BoulderControl()
        {
            Loaded += BoulderControl_Loaded;
        }

        private void BoulderControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window win = Window.GetWindow(this);
            if (win != null)
            {
                timer = new DispatcherTimer();
                timer2 = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(MOVETIME);
                timer2.Interval = TimeSpan.FromMilliseconds(MOVETIME);
                timer.Tick += timerTick;
                timer2.Tick += timerTick2;

               
                Task.Delay(MOVETIME*2).ContinueWith(_ =>
                {
                    timer.Start();
                });
                Task.Delay(MOVETIME*3).ContinueWith(_ =>
                {
                    timer2.Start();
                });
            }
            model = new GameModel();
            logic = new GameLogic(model,Properties.Resources.AL01);
            display = new BoulderDisplay(model, ActualWidth, ActualHeight, MOVETIME);
            diamonvb = (FindResource("diamonvb") as VisualBrush);
            rockfordvb = (FindResource("rockfordvb") as VisualBrush);
            InvalidateVisual();
        }

        private void timerTick2(object sender, EventArgs e)
        {
            
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
                drawingContext.DrawDrawing(display.BuildDrawing(diamonvb, rockfordvb));
            }
        }
    }
}
