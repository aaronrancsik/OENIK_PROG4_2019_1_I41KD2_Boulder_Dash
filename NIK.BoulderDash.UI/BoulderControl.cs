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
        VisualBrush diamonvb;
        VisualBrush rockfordvb;
        IGameLogic logic;
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
                timer.Interval = TimeSpan.FromMilliseconds(MOVETIME);
                timer.Tick += timerTick;
                timer.Start();
            }
            model = new GameModel();
            logic = new GameLogic(model,Properties.Resources.AL01);
            display = new BoulderDisplay(model, ActualWidth, ActualHeight);
            diamonvb = (FindResource("diamonvb") as VisualBrush);
            rockfordvb = (FindResource("rockfordvb") as VisualBrush);
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
                display.BuildDisplay(drawingContext);
            }
        }
    }
}
