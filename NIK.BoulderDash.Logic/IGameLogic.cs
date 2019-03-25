using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIK.BoulderDash.Logic
{
    interface IGameLogic
    {
        void OneTick();

        void MovePlayerLeft();
        void MovePlayerRight();
        void MovePlayerUp();
        void MovePlayerDown();
    }
}
