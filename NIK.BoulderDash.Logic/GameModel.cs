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
            DirtMatrix = new Dirt[width, height];
            TitaniumMatrix = new bool[width, height];
            WallMatrix = new bool[width, height];
            Boulders = new Boulder[width, height];
            Diamonds = new Diamond[width, height];
            Fireflies = new Firefly[width, height];
            Butterflies = new Butterfly[width, height];
            Explosion = new int[width, height];
            Camera = new Camera();
            Exit = new Exit();
        }
        public int WhiteBgCount { get; set; } = 2;
        public bool GameOver { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public const int MOVETIME = 130;
        public int TextureSet { get; set; }
        public Camera Camera { get; set; }
        public int CollectedDiamonds { get; set; }
        public int RequireDiamonds { get; set; }
        public Exit Exit { get; set; }
        public Diamond[,] Diamonds { get; set; }
        public Boulder[,] Boulders { get; private set; }
        public Dirt[,] DirtMatrix { get; set; }
        public Firefly[,] Fireflies { get; set; }
        public Butterfly[,] Butterflies { get; set; }
        public bool[,] TitaniumMatrix { get; set; }
        public bool[,] WallMatrix { get; set; }
        public int[,] Explosion { get; set; }
        public Rockford Rockford { get; set; }
    }
}
