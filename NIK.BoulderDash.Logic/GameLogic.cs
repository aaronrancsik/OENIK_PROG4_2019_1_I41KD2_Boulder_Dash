using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using NIK;

namespace NIK.BoulderDash.Logic
{
    public enum Direction { Up, Down, Left, Right };
    public class GameLogic : IGameLogic
    {
        
        static Random rnd = new Random();
        GameModel model;
        public GameLogic(GameModel model, byte[] levelResource)
        {
            this.model = model;
            LoadLevel(levelResource);
        }

        public void LoadLevel(byte[] levelResource)
        {
            string[] lines = Encoding.ASCII.GetString(levelResource).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int width = int.Parse(lines[0]); //cella szeleseeg, magasseg
            int height = int.Parse(lines[1]);
            model.RequireDiamonds = int.Parse(lines[2]);
            int textureSet = int.Parse(lines[3]);

            model.DirtMatrix = new Dirt[width, height];
            model.TitaniumMatrix = new bool[width, height];
            model.WallMatrix = new bool[width, height];

            model.Blocks = new DynamicBlock[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    char current = lines[y + 4][x];
                    var point = new Point(x, y);
                    var initPrev = new Point(-1, -1);
                    switch (current)
                    {
                        case 'w':
                            model.WallMatrix[x, y] = true;
                            break;
                        case 'X':
                            model.Player = new Player();
                            model.Player.TilePosition = point;
                            model.Player.TileOldPosition = initPrev;
                            model.Blocks[x, y] = model.Player;
                            break;
                        case 'P':
                            model.ExitPistition = new Point(x, y);
                            break;
                        case 'r':
                            var boulder = new Blocks.Boulder();
                            boulder.TilePosition = point;
                            boulder.TileOldPosition = initPrev;
                            model.Boulders.Add(boulder);
                            model.Blocks[x, y] = boulder;
                            break;
                        case 'd':
                            var diamond = new Blocks.Diamond();
                            diamond.TilePosition = point;
                            diamond.TileOldPosition = initPrev;
                            model.Diamonds.Add(diamond);
                            model.Blocks[x, y] = diamond;
                            break;
                        case '.':
                            var dirt = new Dirt();
                            dirt.Variant = rnd.Next(1, 5);
                            model.DirtMatrix[x, y] = dirt;
                            break;
                        case 'W':
                            model.TitaniumMatrix[x, y] = true;
                            break;
                    }
                }
            }
        }
        
        private void Move(Direction dir) //mozgas validalas ha tobbet si tud mozogni akkor bonyi
        {
            model.Player.TileOldPosition = model.Player.TilePosition;

            int x = (int)model.Player.TilePosition.X;
            int y = (int)model.Player.TilePosition.Y;

            if (validatePlayerMove(dir, ref x, ref y))
            {
                model.Blocks[(int)model.Player.TileOldPosition.X, (int)model.Player.TileOldPosition.Y] = null;
                model.Player.TilePosition.X = x;
                model.Player.TilePosition.Y = y;
                model.Blocks[(int)model.Player.TilePosition.X, (int)model.Player.TilePosition.Y] = model.Player;
            }
        }

        private bool validatePlayerMove(Direction dir, ref int x, ref int y)
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

            if (x < 0 || x > model.WallMatrix.GetLength(0) || y < 0 || y > model.WallMatrix.GetLength(1))
                return false;

            if (model.TitaniumMatrix[x, y])
                return false;
            
            if (model.WallMatrix[x, y])
                return false;

            bool[,] boulders = new bool[model.WallMatrix.GetLength(0), model.WallMatrix.GetLength(1)];
            foreach (var bo in model.Boulders)
            {
                boulders[(int)bo.TilePosition.X, (int)bo.TilePosition.Y] = true;
            }

            if ((dir==Direction.Up || dir == Direction.Down) && boulders[x,y])
            {
                return false;
            }

            if(dir==Direction.Left)
            {
                if (boulders[x, y]
                     &&
                     (boulders[x - 1, y]|| model.WallMatrix[x - 1, y] || model.TitaniumMatrix[x - 1, y]))
                {
                    return false;
                }
            }

            if(dir == Direction.Right)
            {
                if (boulders[x, y]
                   &&
                   boulders[x + 1, y])
                {
                    return false;
                }
            }

            if(dir == Direction.Right)
            {
                if (boulders[x, y] && !checkAny(x+1, y))
                {
                    return false;
                }
            }

            if (dir == Direction.Left)
            {
                if (boulders[x, y] && !checkAny(x-1, y))
                {
                    return false;
                }
            }




            return true;
        }

        private bool checkAny(int x, int y)
        {
            return y  < model.WallMatrix.GetLength(1) && x<model.WallMatrix.GetLength(0) && !model.WallMatrix[x, y] && !model.TitaniumMatrix[x, y] && null == model.DirtMatrix[x, y] && model.Blocks[x, y] == null;
        }
        public void OneTick()
        {
            for (int y = model.WallMatrix.GetLength(1)-1; y >= 0 ; y--)
            {
                bool row = false;
                for (int x = 0; x < model.WallMatrix.GetLength(0); x++)
                {
                    if(model.Blocks[x,y] is Player)
                    {
                        
                    }
                    else if(model.Blocks[x, y] != null && checkAny(x,y+1))
                    {
                        var tmp = model.Blocks[x, y];
                        model.Blocks[x, y] = null;
                        model.Blocks[x, y+1] = tmp;
                        tmp.TileOldPosition = tmp.TilePosition;
                        tmp.TilePosition.Y++;
                        row = true;
                    }
                }

            }

            model.DirtMatrix[(int)model.Player.TilePosition.X, (int)model.Player.TilePosition.Y] = null;

            if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                //if (Keyboard.IsKeyDown(Key.Right))
                //{
                //    Move(Direction.Right);
                //}
                //else if (Keyboard.IsKeyDown(Key.Left))
                //{
                //    Move(Direction.Left);
                //}
                //else if (Keyboard.IsKeyDown(Key.Up))
                //{
                //    Move(Direction.Up);
                //}
                //else if (Keyboard.IsKeyDown(Key.Down))
                //{
                //    Move(Direction.Down);
                //}
            }
            else
            {
                if (Keyboard.IsKeyDown(Key.Right))
                {
                    Move(Direction.Right);
                }
                else if (Keyboard.IsKeyDown(Key.Left))
                {
                    Move(Direction.Left);
                }
                else if (Keyboard.IsKeyDown(Key.Up))
                {
                    Move(Direction.Up);
                }
                else if (Keyboard.IsKeyDown(Key.Down))
                {
                    Move(Direction.Down);
                }
            }

            model.Camera.Follow(model.Player.TilePosition);
           
        }
    }
}
