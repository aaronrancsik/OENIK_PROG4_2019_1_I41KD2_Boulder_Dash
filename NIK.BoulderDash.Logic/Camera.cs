using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NIK.BoulderDash.Logic
{
    public class Camera
    {
        public bool isInStage(Point p)
        {
            if(p.X+6 > (center.X-AngleWidthTile/2) && p.X-5 < (center.X + AngleWidthTile / 2) )
            {
                if(p.Y+2 > (center.Y - AngleHeightTile-2 / 2)  && p.Y-2 < (center.Y + AngleHeightTile / 2) )
                {
                    return true;
                }
            }
            return false;
        }
        public Camera()
        {
            //center = new Point(-AngleWidthTile, -AngleHeightTile);
        }
        public int AngleWidthTile { get; } = 20;
        public int AngleHeightTile { get;  } = 12;
        private Point center;
        private Point centerOld;
        public ref Point Center { get => ref center; }
        public ref Point CenterOld { get => ref centerOld; }

        bool first = true;
        public void Follow(Point target)
        {

            centerOld = center;

            if (target.X < AngleWidthTile / 2)
            {
                target.X = AngleWidthTile / 2;
            }
            if (target.X > AngleWidthTile * 1.5)
            {
                target.X = AngleWidthTile * 1.5;
            }
            if (target.Y < AngleHeightTile / 2)
            {
                target.Y = AngleHeightTile / 2;
            }
            if (target.Y > AngleHeightTile * 1.5 - 2)
            {
                target.Y = AngleHeightTile * 1.5 - 2;
            }


            if (first)
            {
                Center = target;
                CenterOld = target;
                first = false;
            }

           
            if (Math.Abs(Center.X - target.X) >= 5)
            {
                Center.X = target.X;
            }

            if (Math.Abs(Center.Y - target.Y) >= 2)
            {
                Center.Y = target.Y;
            }

        }
    }   
}