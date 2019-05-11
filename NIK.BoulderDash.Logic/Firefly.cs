using NIK.BoulderDash.Logic.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NIK.BoulderDash.Logic
{
    public class Firefly : Enemie
    {
        public Firefly(Point initPosition)
        {
            TilePosition = initPosition;
            TileOldPosition = initPosition;
            FaceDirection = Direction.Left;
        }


        public override void Step(bool[,] obstacle)
        {
            var primTarget = calcUnit(Direction.Left);
            var secTarget = calcUnit(Direction.Up);
            if (!obstacle[(int)primTarget.X, (int)primTarget.Y])
            {
                FaceDirection = GetLeft();
                move(primTarget);
            }
            else if (!obstacle[(int)secTarget.X, (int)secTarget.Y])
            {
                move(secTarget);
            }
            else
            {
                FaceDirection = GetRight();
            }

        }
    }
}
