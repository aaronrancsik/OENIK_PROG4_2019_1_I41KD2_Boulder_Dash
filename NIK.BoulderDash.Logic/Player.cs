using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using NIK;

namespace NIK.BoulderDash.Logic
{
    public class Player:GameItem
    {
        //public Vector Delta { get; set; }

        public Player(Point center)
        {
            //Delta = new Vector(2, 0);

            this.Center = center;
            
            GeometryGroup g = new GeometryGroup();
            g.Children.Add(new RectangleGeometry(new Rect(0,0,50,50)));
            //g.Children.Add(new LineGeometry(new Point(10, 0), new Point(-10, 10)));
            area = g.GetWidenedPathGeometry(new Pen(Brushes.Magenta, 2));
        }
    }
}
