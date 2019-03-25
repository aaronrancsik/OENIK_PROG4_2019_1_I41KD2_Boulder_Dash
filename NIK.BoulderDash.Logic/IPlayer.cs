using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIK.BoulderDash.Logic
{
    interface IPlayer
    {
        int PosX { get; }
        int PosY { get; }

        void MoveLeft();
        void MoveRight();
        void MoveUp();
        void MoveDown();


    }
}
