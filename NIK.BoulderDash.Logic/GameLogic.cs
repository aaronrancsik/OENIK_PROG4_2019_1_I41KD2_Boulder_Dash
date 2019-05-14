// <copyright file="GameLogic.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NIK.BoulderDash.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Enum Direction.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Up
        /// </summary>
        Up,

        /// <summary>
        /// Down
        /// </summary>
        Down,

        /// <summary>
        /// Left
        /// </summary>
        Left,

        /// <summary>
        /// Right
        /// </summary>
        Right,
    }

    /// <summary>
    /// Class GameLogic.
    /// </summary>
    public class GameLogic
    {
        private static Random rnd = new Random();
        private GameModel model;
        private int width;
        private int height;
        private byte[] originalMap;
        private Action finishMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameLogic"/> class.
        /// </summary>
        public GameLogic(Action finsihMap)
        {

            this.finishMap = finsihMap;
        }

        /// <summary>
        /// Loads the level.
        /// </summary>
        /// <param name="levelResource">The level resource from resorces.</param>
        /// <returns>GameModel.</returns>
        public GameModel LoadLevel(byte[] levelResource)
        {
            this.originalMap = levelResource;
            string[] lines = this.LoadFileLinesFromResource(levelResource);
            this.width = int.Parse(lines[0]); // cella szeleseeg, magasseg
            this.height = int.Parse(lines[1]);
            this.model = new GameModel(this.width, this.height);
            this.model.RequireDiamonds = int.Parse(lines[2]);
            this.model.TextureSet = int.Parse(lines[3]);
            for (int x = 0; x < this.width; x++)
            {
                for (int y = 0; y < this.height; y++)
                {
                    char current = lines[y + 4][x];
                    var point = new Point(x, y);
                    var initPrev = new Point(x, y);
                    switch (current)
                    {
                        case 'w':
                            this.model.WallMatrix[x, y] = true;
                            break;
                        case 'X':
                            this.model.Rockford = new Rockford();
                            this.model.Rockford.TilePosition = point;
                            this.model.Rockford.TileOldPosition = initPrev;

                            // model.Camera.Follow(point);
                            break;
                        case 'P':

                            this.model.Exit.TilePosition = new Point(x, y);
                            break;
                        case 'r':
                            var boulder = new Blocks.Boulder();
                            boulder.TilePosition = point;
                            boulder.TileOldPosition = initPrev;
                            boulder.Variant = rnd.Next(1, 5);
                            this.model.Boulders[x, y] = boulder;
                            break;
                        case 'd':
                            var diamond = new Blocks.Diamond();
                            diamond.TilePosition = point;
                            diamond.TileOldPosition = initPrev;
                            this.model.Diamonds[x, y] = diamond;
                            break;
                        case '.':
                            var dirt = new Blocks.Dirt();
                            dirt.Variant = rnd.Next(1, 5);
                            this.model.DirtMatrix[x, y] = dirt;
                            break;
                        case 'W':
                            this.model.TitaniumMatrix[x, y] = true;
                            break;
                        case 'q':
                            this.model.Fireflies[x, y] = new Firefly(point);
                            break;
                        case 'B':
                            this.model.Butterflies[x, y] = new Butterfly(point);
                            break;
                    }
                }
            }

            this.CameraFollowRockford();
            this.model.Rockford.Direaction = State.Birth;
            return this.model;
        }

        /// <summary>
        /// Called when [tick]. The logic calculate everything that need for the next gameloop.
        /// </summary>
        public void OneTick()
        {
            this.ReduceAllExplodes();

            this.CheckExit();

            this.DoFallings();

            this.DoLeftRolls();

            this.DoRightRolls();

            this.MoveFireflies();

            this.MoveButterflies();

            this.FirefliesScan();

            this.ButterfliesScan();

            if (this.model.GameOver)
            {
                return;
            }

            if (this.CheckFinish())
            {
                return;
            }

            this.DeleteDirtUnderRockford();

            this.UserInput();

            this.CollectDiamondUnderRockford();

            this.CameraFollowRockford();
        }

        private bool CheckFinish()
        {
            if (model.Exit.IsOpen && model.Exit.TilePosition.Equals(model.Rockford.TilePosition))
            {
                finishMap.Invoke();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Resource binary to string array converter.
        /// </summary>
        /// <param name="levelResource">The level resource.</param>
        /// <returns>System.String[].</returns>
        private string[] LoadFileLinesFromResource(byte[] levelResource)
        {
            return Encoding.ASCII.GetString(levelResource).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void Move(Direction dir)
        {
            int x = (int)this.model.Rockford.TilePosition.X;
            int y = (int)this.model.Rockford.TilePosition.Y;

            this.model.Rockford.TileOldPosition = this.model.Rockford.TilePosition;
            if (this.TryMove(dir, ref x, ref y))
            {
                this.model.Rockford.TilePosition.X = x;
                this.model.Rockford.TilePosition.Y = y;
            }
        }

        private bool TryMove(Direction dir, ref int x, ref int y)
        {
            switch (dir)
            {
                case Direction.Up:
                    x += 0;
                    y += -1;
                    break;
                case Direction.Down:
                    x += 0;
                    y += 1;
                    break;
                case Direction.Left:
                    x += -1;
                    y += 0;
                    break;
                case Direction.Right:
                    x += 1;
                    y += 0;
                    break;
            }

            if (x < 0 || x >= this.model.WallMatrix.GetLength(0) || y < 0 || y >= this.model.WallMatrix.GetLength(1))
            {
                return false;
            }

            if (this.model.TitaniumMatrix[x, y])
            {
                return false;
            }

            if (this.model.WallMatrix[x, y])
            {
                return false;
            }

            if (!this.model.Exit.IsOpen && this.model.Exit.TilePosition.Equals(new Point(x, y)))
            {
                return false;
            }

            if ((dir == Direction.Up || dir == Direction.Down) && this.model.Boulders[x, y] != null)
            {
                return false;
            }

            bool rollLuck = rnd.Next(100) < 30;
            if (dir == Direction.Left)
            {
                if (this.model.Boulders[x, y] != null)
                {
                    if (!this.NothingHere(x - 1, y))
                    {
                        return false;
                    }
                    else
                    {
                        if (rollLuck)
                        {
                            this.model.Boulders[x, y].TileOldPosition = this.model.Boulders[x, y].TilePosition;
                            this.model.Boulders[x, y].TilePosition.X -= 1;

                            this.model.Boulders[x - 1, y] = this.model.Boulders[x, y];
                            this.model.Boulders[x, y] = null;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            if (dir == Direction.Right)
            {
                if (this.model.Boulders[x, y] != null)
                {
                    if (!this.NothingHere(x + 1, y))
                    {
                        return false;
                    }
                    else
                    {
                        if (rollLuck)
                        {
                            this.model.Boulders[x, y].TileOldPosition = this.model.Boulders[x, y].TilePosition;
                            this.model.Boulders[x, y].TilePosition.X += 1;

                            this.model.Boulders[x + 1, y] = this.model.Boulders[x, y];
                            this.model.Boulders[x, y] = null;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool NothingHere(int x, int y)
        {
            if (y >= this.model.Height)
            {
                return true;
            }

            if (x >= this.model.Width)
            {
                return true;
            }

            if (this.model.Rockford != null && (x == this.model.Rockford.TilePosition.X && y == this.model.Rockford.TilePosition.Y))
            {
                return false;
            }

            if (this.model.Fireflies[x, y] != null)
            {
                return false;
            }

            if (this.model.Butterflies[x, y] != null)
            {
                return false;
            }

            if (this.model.WallMatrix[x, y])
            {
                return false;
            }

            if (this.model.TitaniumMatrix[x, y])
            {
                return false;
            }

            if (this.model.DirtMatrix[x, y] != null)
            {
                return false;
            }

            if (x == this.model.Exit.TilePosition.X && y == this.model.Exit.TilePosition.Y)
            {
                return false;
            }

            if (this.model.Boulders[x, y] != null)
            {
                return false;
            }

            if (this.model.Diamonds[x, y] != null)
            {
                return false;
            }

            return true;
        }

        private void FirefliesScan()
        {
            foreach (var item in this.model.Fireflies)
            {
                bool found = false;
                if (item != null)
                {
                    int[,] m = new int[,]
                    {
                        { 0, 1, 0 },
                        { 1, 1, 1 },
                        { 0, 1, 0 },
                    };
                    for (double x = item.TilePosition.X - 1; x < item.TilePosition.X + 2 && !found; x++)
                    {
                        for (double y = item.TilePosition.Y - 1; y < item.TilePosition.Y + 2 && !found; y++)
                        {
                            int xx = (int)(x - (item.TilePosition.X - 1));
                            int yy = (int)(y - (item.TilePosition.Y - 1));
                            if (m[xx, yy] == 1)
                            {
                                if (this.model.Rockford != null && (int)this.model.Rockford.TilePosition.X == x && (int)this.model.Rockford.TilePosition.Y == y)
                                {
                                    found = true;
                                    this.Explode((int)x, (int)y);
                                }
                            }
                        }
                    }

                    if (found)
                    {
                        return;
                    }
                }
            }
        }

        private void ButterfliesScan()
        {
            foreach (var item in this.model.Butterflies)
            {
                bool found = false;
                if (item != null)
                {
                    int[,] m = new int[,]
                    {
                        { 0, 1, 0 },
                        { 1, 1, 1 },
                        { 0, 1, 0 },
                    };
                    for (double x = item.TilePosition.X - 1; x < item.TilePosition.X + 2 && !found; x++)
                    {
                        for (double y = item.TilePosition.Y - 1; y < item.TilePosition.Y + 2 && !found; y++)
                        {
                            int xx = (int)(x - (item.TilePosition.X - 1));
                            int yy = (int)(y - (item.TilePosition.Y - 1));
                            if (m[xx, yy] == 1)
                            {
                                if (this.model.Rockford != null && (int)this.model.Rockford.TilePosition.X == x && (int)this.model.Rockford.TilePosition.Y == y)
                                {
                                    found = true;
                                    this.DiamondExpode((int)x, (int)y);
                                }
                            }
                        }
                    }

                    if (found)
                    {
                        return;
                    }
                }
            }
        }

        private void MoveFireflies()
        {
            List<Logic.Firefly> moved = new List<Firefly>();
            foreach (var f in this.model.Fireflies)
            {
                if (f != null && !moved.Contains(f))
                {
                    bool[,] obs = new bool[this.width, this.height];
                    for (int x = 0; x < this.width; x++)
                    {
                        for (int y = 0; y < this.height; y++)
                        {
                            obs[x, y] = !this.NothingHere(x, y);
                        }
                    }

                    int xx = (int)f.TilePosition.X;
                    int yy = (int)f.TilePosition.Y;
                    f.Step(obs);
                    moved.Add(f);
                    this.model.Fireflies[xx, yy] = null;
                    this.model.Fireflies[(int)f.TilePosition.X, (int)f.TilePosition.Y] = f;
                }
            }
        }

        private void MoveButterflies()
        {
            List<Butterfly> moved = new List<Butterfly>();
            foreach (var f in this.model.Butterflies)
            {
                if (f != null && !moved.Contains(f))
                {
                    bool[,] obs = new bool[this.width, this.height];
                    for (int x = 0; x < this.width; x++)
                    {
                        for (int y = 0; y < this.height; y++)
                        {
                            obs[x, y] = !this.NothingHere(x, y);
                        }
                    }

                    int xx = (int)f.TilePosition.X;
                    int yy = (int)f.TilePosition.Y;
                    f.Step(obs);
                    moved.Add(f);
                    this.model.Butterflies[xx, yy] = null;
                    this.model.Butterflies[(int)f.TilePosition.X, (int)f.TilePosition.Y] = f;
                }
            }
        }

        private void CameraFollowRockford()
        {
            this.model.Camera.Follow(this.model.Rockford.TilePosition);
        }

        private void CollectDiamondUnderRockford()
        {
            if (this.model.Diamonds[(int)this.model.Rockford.TilePosition.X, (int)this.model.Rockford.TilePosition.Y] != null)
            {
                this.model.CollectedDiamonds++;
                this.model.Diamonds[(int)this.model.Rockford.TilePosition.X, (int)this.model.Rockford.TilePosition.Y] = null;
            }
        }

        private void UserInput()
        {
            this.model.Rockford.Direaction = State.Stand;
            if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                // if (Keyboard.IsKeyDown(Key.D) || Keyboard.IsKeyDown(Key.Right))
                // {
                // }
                // else if (Keyboard.IsKeyDown(Key.Left))
                // {
                //    Move(Direction.Left);
                // }
                // else if (Keyboard.IsKeyDown(Key.Up))
                // {
                //    Move(Direction.Up);
                // }
                // else if (Keyboard.IsKeyDown(Key.Down))
                // {
                //    Move(Direction.Down);
                // }
            }
            else
            {
                if (Keyboard.IsKeyDown(Key.D) || Keyboard.IsKeyDown(Key.Right))
                {
                    this.model.Rockford.Direaction = State.Right;
                    this.Move(Direction.Right);
                }
                else if (Keyboard.IsKeyDown(Key.A) || Keyboard.IsKeyDown(Key.Left))
                {
                    this.model.Rockford.Direaction = State.Left;
                    this.Move(Direction.Left);
                }
                else if (Keyboard.IsKeyDown(Key.W) || Keyboard.IsKeyDown(Key.Up))
                {
                    this.model.Rockford.Direaction = State.Up;
                    this.Move(Direction.Up);
                }
                else if (Keyboard.IsKeyDown(Key.S) || Keyboard.IsKeyDown(Key.Down))
                {
                    this.model.Rockford.Direaction = State.Down;
                    this.Move(Direction.Down);
                }
            }
        }

        private void DeleteDirtUnderRockford()
        {
            this.model.DirtMatrix[(int)this.model.Rockford.TilePosition.X, (int)this.model.Rockford.TilePosition.Y] = null;
        }

        private void DoRightRolls()
        {
            List<Blocks.DynamicBlock> markedRollingRight = new List<Blocks.DynamicBlock>();
            for (int y = this.model.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < this.model.Width; x++)
                {
                    if (this.model.Diamonds[x, y] != null)
                    {
                        if (this.IsRoundHere(x, y + 1))
                        {
                            if (this.NothingHere(x + 1, y + 1) && this.NothingHere(x + 1, y))
                            {
                                markedRollingRight.Add(this.model.Diamonds[x, y]);
                            }
                        }
                    }
                    else if (this.model.Boulders[x, y] != null)
                    {
                        if (this.IsRoundHere(x, y + 1))
                        {
                            if (this.NothingHere(x + 1, y + 1) && this.NothingHere(x + 1, y))
                            {
                                markedRollingRight.Add(this.model.Boulders[x, y]);
                            }
                        }
                    }
                }
            }

            foreach (var item in markedRollingRight)
            {
                if (item is Blocks.Diamond)
                {
                    this.model.Diamonds[(int)item.TilePosition.X, (int)item.TilePosition.Y] = null;
                    this.model.Diamonds[(int)item.TilePosition.X + 1, (int)item.TilePosition.Y] = item as Blocks.Diamond;
                }
                else if (item is Blocks.Boulder)
                {
                    this.model.Boulders[(int)item.TilePosition.X, (int)item.TilePosition.Y] = null;
                    this.model.Boulders[(int)item.TilePosition.X + 1, (int)item.TilePosition.Y] = item as Blocks.Boulder;
                }

                item.TileOldPosition = item.TilePosition;
                item.TilePosition.X += 1;
            }
        }

        private void DoLeftRolls()
        {
            List<Blocks.DynamicBlock> markedRollingLeft = new List<Blocks.DynamicBlock>();

            for (int y = this.model.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < this.model.Width; x++)
                {
                    if (this.model.Diamonds[x, y] != null)
                    {
                        if (this.IsRoundHere(x, y + 1))
                        {
                            if (this.NothingHere(x - 1, y + 1) && this.NothingHere(x - 1, y))
                            {
                                markedRollingLeft.Add(this.model.Diamonds[x, y]);
                            }
                        }
                    }
                    else if (this.model.Boulders[x, y] != null)
                    {
                        if (this.IsRoundHere(x, y + 1))
                        {
                            if (this.NothingHere(x - 1, y + 1) && this.NothingHere(x - 1, y))
                            {
                                markedRollingLeft.Add(this.model.Boulders[x, y]);
                            }
                        }
                    }
                }
            }

            foreach (var item in markedRollingLeft)
            {
                if (item is Blocks.Diamond)
                {
                    this.model.Diamonds[(int)item.TilePosition.X, (int)item.TilePosition.Y] = null;
                    this.model.Diamonds[(int)item.TilePosition.X - 1, (int)item.TilePosition.Y] = item as Blocks.Diamond;
                }
                else if (item is Blocks.Boulder)
                {
                    this.model.Boulders[(int)item.TilePosition.X, (int)item.TilePosition.Y] = null;
                    this.model.Boulders[(int)item.TilePosition.X - 1, (int)item.TilePosition.Y] = item as Blocks.Boulder;
                }

                item.TileOldPosition = item.TilePosition;
                item.TilePosition.X -= 1;
            }
        }

        private void DoFallings()
        {
            List<Blocks.DynamicBlock> markedFalling = new List<Blocks.DynamicBlock>();

            for (int y = this.model.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < this.model.Width; x++)
                {
                    if (this.model.Diamonds[x, y] != null)
                    {
                        if (this.NothingHere(x, y + 1))
                        {
                            markedFalling.Add(this.model.Diamonds[x, y]);
                        }
                        else
                        {
                            if (this.model.Diamonds[x, y].Falling)
                            {
                                this.model.Diamonds[x, y].Falling = false;
                                this.TryExplode(x, y + 1);
                            }
                        }
                    }
                    else if (this.model.Boulders[x, y] != null)
                    {
                        if (this.NothingHere(x, y + 1))
                        {
                            markedFalling.Add(this.model.Boulders[x, y]);
                        }
                        else
                        {
                            if (this.model.Boulders[x, y].Falling)
                            {
                                this.model.Boulders[x, y].Falling = false;
                                this.TryExplode(x, y + 1);
                            }
                        }
                    }
                }
            }

            foreach (var m in markedFalling)
            {
                m.Falling = true;
                if (m is Blocks.Diamond)
                {
                    this.model.Diamonds[(int)m.TilePosition.X, (int)m.TilePosition.Y] = null;
                    this.model.Diamonds[(int)m.TilePosition.X, (int)m.TilePosition.Y + 1] = m as Blocks.Diamond;
                }
                else if (m is Blocks.Boulder)
                {
                    this.model.Boulders[(int)m.TilePosition.X, (int)m.TilePosition.Y] = null;
                    this.model.Boulders[(int)m.TilePosition.X, (int)m.TilePosition.Y + 1] = m as Blocks.Boulder;
                }

                m.TileOldPosition = m.TilePosition;
                m.TilePosition.Y++;
            }
        }

        private void CheckExit()
        {
            if (this.model.RequireDiamonds == this.model.CollectedDiamonds)
            {
                this.model.Exit.Open();
            }
        }

        private bool IsRoundHere(int x, int y)
        {
            if (this.model.Diamonds[x, y] != null || this.model.Boulders[x, y] != null || this.model.WallMatrix[x, y])
            {
                return true;
            }

            return false;
        }

        private void ReduceAllExplodes()
        {
            for (int x = 0; x < this.width; x++)
            {
                for (int y = 0; y < this.height; y++)
                {
                    if (this.model.Explosion[x, y] > 0)
                    {
                        this.model.Explosion[x, y]--;
                    }
                }
            }
        }

        private void TryExplode(int centerX, int centerY)
        {
            var rockford = this.model.Rockford != null && this.model.Rockford.TilePosition.X == centerX && this.model.Rockford.TilePosition.Y == centerY;
            bool fire = false;
            bool butter = false;
            foreach (var item in this.model.Fireflies)
            {
                if (item != null)
                {
                    if (item.TilePosition.X == centerX && item.TilePosition.Y == centerY)
                    {
                        fire = true;
                        break;
                    }
                }
            }

            foreach (var item in this.model.Butterflies)
            {
                if (item != null)
                {
                    if (item.TilePosition.X == centerX && item.TilePosition.Y == centerY)
                    {
                        butter = true;
                        break;
                    }
                }
            }

            if (rockford || fire)
            {
                this.Explode(centerX, centerY);
            }
            else if (butter)
            {
                this.DiamondExpode(centerX, centerY);
            }
        }

        private void DiamondExpode(int centerX, int centerY)
        {
            this.Explode(centerX, centerY);
            for (int x = centerX - 1; x < centerX + 2; x++)
            {
                for (int y = centerY - 1; y < centerY + 2; y++)
                {
                    if (x < this.model.Width && x >= 0 && y < this.model.Height && y >= 0)
                    {
                        if (!this.model.TitaniumMatrix[x, y])
                        {
                            var d = new Blocks.Diamond();
                            d.TilePosition = new Point(x, y);
                            d.TileOldPosition = new Point(x, y);
                            this.model.Diamonds[x, y] = d;
                        }
                    }
                }
            }
        }

        private void Explode(int centerX, int centerY)
        {
            for (int x = centerX - 1; x < centerX + 2; x++)
            {
                for (int y = centerY - 1; y < centerY + 2; y++)
                {
                    if (x < this.model.Width && x >= 0 && y < this.model.Height && y >= 0)
                    {
                        if (this.model.Rockford != null)
                        {
                            if (this.model.Rockford.TilePosition.X == x && this.model.Rockford.TilePosition.Y == y)
                            {
                                this.model.Rockford = null;
                                this.model.GameOver = true;
                            }
                        }

                        this.model.Explosion[x, y] = 2;
                        this.model.WallMatrix[x, y] = false;
                        this.model.DirtMatrix[x, y] = null;
                        this.model.Diamonds[x, y] = null;
                        this.model.Boulders[x, y] = null;
                        this.model.Fireflies[x, y] = null;
                        this.model.Butterflies[x, y] = null;
                    }
                }
            }
        }

        private bool CollectedEnought()
        {
            return this.model.RequireDiamonds <= this.model.CollectedDiamonds;
        }
    }
}
