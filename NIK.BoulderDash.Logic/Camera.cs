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
        public int Width { get; } = 20;
        public int Height { get;  } = 12;
        private Point center;
        private Point centerOld;
        public ref Point Center { get => ref center; }
        public ref Point CenterOld { get => ref centerOld; }

        public void Follow(Point target)
        {
            CenterOld = center;

            //if (target.X > (Width - centerOld.X)%6)
            //{
            //    center.X += 4;
            //}


            if (Math.Sqrt(Math.Pow(center.X - target.X, 2) + Math.Pow(center.Y - target.Y, 2)) > 5 || (target.X < 7 || target.Y < 7))
            {
                target.X *= target.X / 15;

                if (target.X >= Width * 2)
                    target.X = Width + Width - 1;

                target.Y *= target.Y / 7;

                if (target.Y >= Height * 2)
                    target.Y = Height + Height - 4;

                center = target;
            }

        }
    }

    
}
