using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIK.BoulderDash.Logic
{
    public interface IGameLogic
    {
        void OneTick(double w, double h);

        void MovePlayerLeft();
        void MovePlayerRight();
        void MovePlayerUp();
        void MovePlayerDown();
    }
}
