using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NIK;

namespace NIK.BoulderDash.Logic
{
    public class GameLogic : IGameLogic
    {
        static Random rnd = new Random();
        IGameModel model;
        public GameLogic(IGameModel model)
        {
            this.model = model;
        }

        public void MovePlayerDown()
        {
            throw new NotImplementedException();
        }

        public void MovePlayerLeft()
        {
            throw new NotImplementedException();
        }

        public void MovePlayerRight()
        {
            throw new NotImplementedException();
        }

        public void MovePlayerUp()
        {
            throw new NotImplementedException();
        }

        public void OneTick(double w, double h)
        {
            //model.Player.Center = new System.Windows.Point(model.Player.Center.X + 1, model.Player.Center.Y);
            //model.Player.Center.Set(10, 10);
            model.Player.Center.X++;


            //x.PlusX(1);
            //model.Player.Center = x;
            
        }
    }
}
