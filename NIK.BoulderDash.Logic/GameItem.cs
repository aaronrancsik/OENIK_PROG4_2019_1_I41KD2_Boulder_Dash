using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NIK.BoulderDash.Logic
{
    public abstract class GameItem
    {

        protected Geometry area;

        Point center;
        public ref Point Center { get => ref center;  }


        double degree;

        public double Rad
        {
            get { return degree / 180 * Math.PI; }
            set { degree = value * 180 / Math.PI; }
        }

        public Geometry RealArea
        {
            get
            {
                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new TranslateTransform(Center.X, Center.Y));
                tg.Children.Add(new RotateTransform(degree, Center.X, Center.Y));
                area.Transform = tg;
                return area.GetFlattenedPathGeometry();
            }
        }

        public bool isCollision(GameItem other)
        {
            
            return Geometry.Combine(this.RealArea, other.RealArea, GeometryCombineMode.Intersect, null).GetArea() > 0;
        }

    }
}
