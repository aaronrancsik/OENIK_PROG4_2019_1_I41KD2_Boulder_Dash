using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NIK.BoulderDash.Logic
{
    public class GameModel : IGameModel
    {
        public GameModel()
        {
            Player = new Player(new Point(10,10));
        }
        public int Time { get; private set; }

        public Player Player { get; private set; }

        public Map Map { get; private set; }

        public Camera Camera { get; private set; }

        public List<Enemie> Enemies { get; private set; }
    }
}
