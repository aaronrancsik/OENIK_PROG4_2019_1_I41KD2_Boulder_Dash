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
        IGameLogic logic;
        IGameModel model;
        BoulderDisplay display;
        DispatcherTimer timer;

        public BoulderControl()
        {
            Loaded += BoulderControl_Loaded;
        }

        private void BoulderControl_Loaded(object sender, RoutedEventArgs e)
        {

            model = new GameModel();
            logic = new GameLogic(model);
            display = new BoulderDisplay(model, ActualWidth, ActualHeight);

            Window win = Window.GetWindow(this);
            if (win != null)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(2);
                timer.Tick += timerTick;
                timer.Start();

                //win.KeyDown += Win_KeyDown;
            }
            InvalidateVisual(); //kerek egy ujrarajzolast
        }

        private void timerTick(object sender, EventArgs e)
        {
            logic.OneTick(ActualWidth, ActualHeight);
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
