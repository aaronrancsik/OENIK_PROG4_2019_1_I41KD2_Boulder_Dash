using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using NIK.BoulderDash.Logic;

namespace NIK.BoulderDash.VM
{
    public class BoulderDisplay
    {
        IGameModel model;
        double width;
        double height;

        public BoulderDisplay(IGameModel model, double w, double h)
        {
            this.model = model;
            this.width = w;
            this.height = h;
        }

        Pen black = new Pen(Brushes.Black, 2);
        Pen red = new Pen(Brushes.Red, 2);
        Typeface arial = new Typeface("Arial");
        Point textLocation = new Point(10, 10);
        Rect rect;
        CultureInfo ci = CultureInfo.CurrentCulture;
        Brush b = Brushes.Black;
        Brush r = Brushes.Red;
        Brush lg = Brushes.LightGray;

        public void BuildDisplay(DrawingContext ctx)
        {
            // TODO !!!
            // Ordo notation, optimalization is important
            DrawBackground(ctx);
            DrawPlayer(ctx);
            //DrawBird(ctx);
            //DrawErrors(ctx);
        }

        private void DrawBackground(DrawingContext ctx)
        {
            ctx.DrawRectangle(lg, null, rect);
        }
        private void DrawPlayer(DrawingContext ctx)
        {
            ctx.DrawGeometry(r, red, model.Player.RealArea);
        }


    }
}
