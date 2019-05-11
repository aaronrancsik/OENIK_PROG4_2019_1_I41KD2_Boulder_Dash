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
    public class GameLogic
    {
        
        static Random rnd = new Random();
        GameModel model;
        int width;
        int height;
        
        public GameLogic()
        {
        }

        public GameModel LoadLevel(byte[] levelResource)
        {
            string[] lines = LoadFileLinesFromResource(levelResource);
            width = int.Parse(lines[0]); //cella szeleseeg, magasseg
            height = int.Parse(lines[1]);
            GameModel model = new GameModel(width, height);
            this.model = model;
            model.RequireDiamonds = int.Parse(lines[2]);
            model.TextureSet = int.Parse(lines[3]);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    char current = lines[y+4][x];
                    var point = new Point(x, y);
                    var initPrev = new Point(x, y);
                    switch (current)
                    {
                        case 'w':
                            model.WallMatrix[x, y] = true;
                            break;
                        case 'X':
                            model.Rockford = new Rockford();
                            model.Rockford.TilePosition = point;
                            model.Rockford.TileOldPosition = initPrev;
                            break;
                        case 'P':
                            
                            model.Exit.TilePosition = new Point(x, y);
                            break;
                        case 'r':
                            var boulder = new Blocks.Boulder();
                            boulder.TilePosition = point;
                            boulder.TileOldPosition = initPrev;
                            boulder.Variant= rnd.Next(1, 5);
                            model.Boulders[x,y]=boulder;
                            break;
                        case 'd':
                            var diamond = new Blocks.Diamond();
                            diamond.TilePosition = point;
                            diamond.TileOldPosition = initPrev;
                            model.Diamonds[x,y] = diamond;
                            break;
                        case '.':
                            var dirt = new Blocks.Dirt();
                            dirt.Variant = rnd.Next(1, 5);
                            model.DirtMatrix[x, y] = dirt;
                            break;
                        case 'W':
                            model.TitaniumMatrix[x, y] = true;
                            break;
                        case 'q':
                            model.Fireflies[x, y] = new Firefly(point);
                            break;
                    }
                }
            }
            CameraFollowRockford();
            model.Rockford.Direaction = State.Birth;
            return model;
        }

        private string[] LoadFileLinesFromResource(byte[] levelResource)
        {
            return Encoding.ASCII.GetString(levelResource).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

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
