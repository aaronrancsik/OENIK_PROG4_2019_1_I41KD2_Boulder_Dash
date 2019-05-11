using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NIK.BoulderDash.Logic.Blocks;
namespace NIK.BoulderDash.Logic
{
    public class GameModel
    {
        public GameModel(int width, int height)
        {
            Width = width;
            Height = height;
            Boulders = new Boulder[width, height];
            Diamonds = new Diamond[width, height];
            Camera = new Camera();
        }
        public int WhiteBgCount { get; set; } = 2;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int TextureSet { get; set; }
        public Camera Camera { get; set; }
        public int CollectedDiamonds { get; set; }
        public int RequireDiamonds { get; set; }
        public Exit Exit { get; set; }
        public Diamond[,] Diamonds { get; set; }
        public Boulder[,] Boulders { get; private set; }
        public Dirt[,] DirtMatrix { get; set; }
        public bool[,] TitaniumMatrix { get; set; }
        public bool[,] WallMatrix { get; set; }
        public Rockford Rockford { get; set; }
    }
}
