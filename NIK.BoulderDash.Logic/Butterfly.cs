using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NIK.BoulderDash.Logic
{
    public class Butterfly : Enemie
    {
        public Butterfly(Point initPosition)
        {
            TilePosition = initPosition;
            TileOldPosition = initPosition;
            FaceDirection = Direction.Down;
        }


        public override void Step(bool[,] obstacle)
        {
            var primTarget = calcUnit(Direction.Right);
            var secTarget = calcUnit(Direction.Up);
            if (!obstacle[(int)primTarget.X, (int)primTarget.Y])
            {
                FaceDirection = GetRight();
                move(primTarget);
            }
            else if (!obstacle[(int)secTarget.X, (int)secTarget.Y])
            {
                move(secTarget);
            }
            else
            {
                FaceDirection = GetLeft();
            }

        }
    }
}
