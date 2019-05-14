// <copyright file="GameModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NIK.BoulderDash.Logic
{
    using NIK.BoulderDash.Logic.Blocks;

    /// <summary>
    /// Class GameModel.
    /// </summary>
    public class GameModel
    {
        /// <summary>
        /// The movetime reptresent in milisec the time when that used when need new draws, and start new cicle in gameloop.
        /// </summary>
        public const int MOVETIME = 130;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameModel"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public GameModel(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.DirtMatrix = new Dirt[width, height];
            this.TitaniumMatrix = new bool[width, height];
            this.WallMatrix = new bool[width, height];
            this.Boulders = new Boulder[width, height];
            this.Diamonds = new Diamond[width, height];
            this.Fireflies = new Firefly[width, height];
            this.Butterflies = new Butterfly[width, height];
            this.Explosion = new int[width, height];
            this.Camera = new Camera();
            this.Exit = new Exit();
        }

        /// <summary>
        /// Gets or sets the number of loop cicle we want to see white back ground, when collect enought diamond on a map.
        /// </summary>
        /// <value>The white bg count.</value>
        public int WhiteBgCount { get; set; } = 2;

        /// <summary>
        /// Gets or sets a value indicating whether [game over].
        /// </summary>
        /// <value><c>true</c> if [game over]; otherwise, <c>false</c>.</value>
        public bool GameOver { get; set; }

        /// <summary>
        /// Gets the game width in tile.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the game height in tile.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get; private set; }

        /// <summary>
        /// Gets or sets texture set. We have 4 of them, each use diffrent colored block element in drawing.
        /// </summary>
        /// <value>The texture set.</value>
        public int TextureSet { get; set; }

        /// <summary>
        /// Gets or sets the camera.
        /// </summary>
        /// <value>The camera.</value>
        public Camera Camera { get; set; }

        /// <summary>
        /// Gets or sets the collected diamonds.
        /// </summary>
        /// <value>The collected diamonds.</value>
        public int CollectedDiamonds { get; set; }

        /// <summary>
        /// Gets or sets the require diamonds.
        /// </summary>
        /// <value>The require diamonds.</value>
        public int RequireDiamonds { get; set; }

        /// <summary>
        /// Gets or sets the exit.
        /// </summary>
        /// <value>The exit.</value>
        public Exit Exit { get; set; }

        /// <summary>
        /// Gets or sets the diamonds.
        /// </summary>
        /// <value>The diamonds.</value>
        public Diamond[,] Diamonds { get; set; }

        /// <summary>
        /// Gets the boulders.
        /// </summary>
        /// <value>The boulders.</value>
        public Boulder[,] Boulders { get; private set; }

        /// <summary>
        /// Gets or sets the dirt matrix.
        /// </summary>
        /// <value>The dirt matrix.</value>
        public Dirt[,] DirtMatrix { get; set; }

        /// <summary>
        /// Gets or sets the fireflies.
        /// </summary>
        /// <value>The fireflies.</value>
        public Firefly[,] Fireflies { get; set; }

        /// <summary>
        /// Gets or sets the butterflies.
        /// </summary>
        /// <value>The butterflies.</value>
        public Butterfly[,] Butterflies { get; set; }

        /// <summary>
        /// Gets or sets the titanium matrix.
        /// </summary>
        /// <value>The titanium matrix.</value>
        public bool[,] TitaniumMatrix { get; set; }

        /// <summary>
        /// Gets or sets the wall matrix.
        /// </summary>
        /// <value>The wall matrix.</value>
        public bool[,] WallMatrix { get; set; }

        /// <summary>
        /// Gets or sets the explosion.
        /// </summary>
        /// <value>The explosion.</value>
        public int[,] Explosion { get; set; }

        /// <summary>
        /// Gets or sets the rockford.
        /// </summary>
        /// <value>The rockford.</value>
        public Rockford Rockford { get; set; }
    }
}
