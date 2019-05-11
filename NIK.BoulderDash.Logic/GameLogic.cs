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

        private void Move(Direction dir)
        {
            
            int x = (int)model.Rockford.TilePosition.X;
            int y = (int)model.Rockford.TilePosition.Y;
            

            model.Rockford.TileOldPosition = model.Rockford.TilePosition;
            if (tryMove(dir, ref x, ref y))
            {
                model.Rockford.TilePosition.X = x;
                model.Rockford.TilePosition.Y = y;
            }
            
        }

        private bool tryMove(Direction dir, ref int x, ref int y)
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

            if (!model.Exit.IsOpen && model.Exit.TilePosition.Equals(new Point(x, y)))
                return false;


            if ((dir==Direction.Up || dir == Direction.Down) && model.Boulders[x,y]!=null)
            {
                return false;
            }
            bool rollLuck = rnd.Next(100) < 30;
            if (dir==Direction.Left)
            {
                if (model.Boulders[x, y]!=null)
                {
                    if (!nothingHere(x-1,y))
                    {
                        return false;
                    }
                    else
                    {
                        if (rollLuck) //rnd.Next(100) < 20
                        {
                            model.Boulders[x, y].TileOldPosition = model.Boulders[x, y].TilePosition;
                            model.Boulders[x, y].TilePosition.X -= 1;

                            model.Boulders[x - 1, y] = model.Boulders[x, y];
                            model.Boulders[x, y] = null;

                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }


            if(dir == Direction.Right)
            {
                if (model.Boulders[x, y]!=null)
                {
                    if(!nothingHere(x+1, y))
                    {
                        return false;
                    }
                    else
                    {
                        if (rollLuck)
                        {
                            model.Boulders[x, y].TileOldPosition = model.Boulders[x, y].TilePosition;
                            model.Boulders[x, y].TilePosition.X += 1;

                            model.Boulders[x + 1, y] = model.Boulders[x, y];
                            model.Boulders[x, y] = null;

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

        private bool nothingHere(int x, int y)
        {
            if (y >= model.Height)
                return true;
            if (x >= model.Width)
                return true;

            if (model.Rockford!=null && (x == model.Rockford.TilePosition.X && y == model.Rockford.TilePosition.Y))
                return false;

            if (model.WallMatrix[x, y])
                return false;

            if (model.TitaniumMatrix[x, y])
                return false;

            if (null != model.DirtMatrix[x, y])
                return false;

            if ((x == model.Exit.TilePosition.X && y == model.Exit.TilePosition.Y))
                return false;

            if (model.Boulders[x, y] != null)
                return false;

            if (model.Diamonds[x, y] != null)
                return false;

            return true;

        }

        public void OneTick()
        {
            ReduceAllExplodes();

            CheckExit();

            DoFallings();

            DoLeftRolls();

            DoRightRolls();
           

            if (model.GameOver)
            {
                return;
            }

            DeleteDirtUnderRockford();


            //foreach (var f in model.Fireflies)
            //{
            //    for (int i = 0; i < length; i++)
            //    {

            //    }
            //    f.Step()
            //}

            UserInput();

            CollectDiamondUnderRockford();

            CameraFollowRockford();

        }

        private void CameraFollowRockford()
        {
            model.Camera.Follow(model.Rockford.TilePosition);
        }

        private void CollectDiamondUnderRockford()
        {

            if (model.Diamonds[(int)model.Rockford.TilePosition.X, (int)model.Rockford.TilePosition.Y] != null)
            {
                model.CollectedDiamonds++;
                model.Diamonds[(int)model.Rockford.TilePosition.X, (int)model.Rockford.TilePosition.Y] = null;
            }
        }

        private void UserInput()
        {
            model.Rockford.Direaction = State.Stand;
            if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                //if (Keyboard.IsKeyDown(Key.D) || Keyboard.IsKeyDown(Key.Right))
                //{
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
                if (Keyboard.IsKeyDown(Key.D) || Keyboard.IsKeyDown(Key.Right))
                {
                    model.Rockford.Direaction = State.Right;
                    Move(Direction.Right);
                }
                else if (Keyboard.IsKeyDown(Key.A) || Keyboard.IsKeyDown(Key.Left))
                {
                    model.Rockford.Direaction = State.Left;
                    Move(Direction.Left);
                }
                else if (Keyboard.IsKeyDown(Key.W) || Keyboard.IsKeyDown(Key.Up))
                {
                    model.Rockford.Direaction = State.Up;
                    Move(Direction.Up);
                }
                else if (Keyboard.IsKeyDown(Key.S) || Keyboard.IsKeyDown(Key.Down))
                {
                    model.Rockford.Direaction = State.Down;
                    Move(Direction.Down);
                }
            }
        }

        private void DeleteDirtUnderRockford()
        {
            model.DirtMatrix[(int)model.Rockford.TilePosition.X, (int)model.Rockford.TilePosition.Y] = null;
        }

        private void DoRightRolls()
        {
            List<Blocks.DynamicBlock> markedRollingRight = new List<Blocks.DynamicBlock>();
            for (int y = model.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < model.Width; x++)
                {
                    if (model.Diamonds[x, y] != null)
                    {
                        if (isRoundHere(x, y + 1))
                        {

                            if (nothingHere(x + 1, y + 1) && nothingHere(x + 1, y))
                            {
                                markedRollingRight.Add(model.Diamonds[x, y]);
                            }
                        }
                    }
                    else if (model.Boulders[x, y] != null)
                    {
                        if (isRoundHere(x, y + 1))
                        {

                            if (nothingHere(x + 1, y + 1) && nothingHere(x + 1, y))
                            {
                                markedRollingRight.Add(model.Boulders[x, y]);
                            }

                        }
                    }
                }
            }
            foreach (var item in markedRollingRight)
            {
                if (item is Blocks.Diamond)
                {
                    model.Diamonds[(int)item.TilePosition.X, (int)item.TilePosition.Y] = null;
                    model.Diamonds[(int)item.TilePosition.X + 1, (int)item.TilePosition.Y] = item as Blocks.Diamond;

                }
                else if (item is Blocks.Boulder)
                {
                    model.Boulders[(int)item.TilePosition.X, (int)item.TilePosition.Y] = null;
                    model.Boulders[(int)item.TilePosition.X + 1, (int)item.TilePosition.Y] = item as Blocks.Boulder;
                }
                item.TileOldPosition = item.TilePosition;
                item.TilePosition.X += 1;
            }
        }

        private void DoLeftRolls()
        {
            List<Blocks.DynamicBlock> markedRollingLeft = new List<Blocks.DynamicBlock>();

            for (int y = model.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < model.Width; x++)
                {
                    if (model.Diamonds[x, y] != null)
                    {
                        if (isRoundHere(x, y + 1))
                        {
                            if (nothingHere(x - 1, y + 1) && nothingHere(x - 1, y))
                            {
                                markedRollingLeft.Add(model.Diamonds[x, y]);

                            }
                        }
                    }
                    else if (model.Boulders[x, y] != null)
                    {
                        if (isRoundHere(x, y + 1))
                        {
                            if (nothingHere(x - 1, y + 1) && nothingHere(x - 1, y))
                            {
                                markedRollingLeft.Add(model.Boulders[x, y]);
                            }
                        }
                    }
                }
            }
            foreach (var item in markedRollingLeft)
            {
                if (item is Blocks.Diamond)
                {
                    model.Diamonds[(int)item.TilePosition.X, (int)item.TilePosition.Y] = null;
                    model.Diamonds[(int)item.TilePosition.X - 1, (int)item.TilePosition.Y] = item as Blocks.Diamond;

                }
                else if (item is Blocks.Boulder)
                {
                    model.Boulders[(int)item.TilePosition.X, (int)item.TilePosition.Y] = null;
                    model.Boulders[(int)item.TilePosition.X - 1, (int)item.TilePosition.Y] = item as Blocks.Boulder;
                }
                item.TileOldPosition = item.TilePosition;
                item.TilePosition.X -= 1;
            }
        }

        private void DoFallings()
        {

            List<Blocks.DynamicBlock> markedFalling = new List<Blocks.DynamicBlock>();

            for (int y = model.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < model.Width; x++)
                {

                    if (model.Diamonds[x, y] != null)
                    {

                        if (nothingHere(x, y + 1))
                        {
                            markedFalling.Add(model.Diamonds[x, y]);
                        }
                        else
                        {
                            if (model.Diamonds[x, y].Falling)
                            {
                                model.Diamonds[x, y].Falling = false;
                                TryExplode(x, y + 1);
                            }

                        }
                    }
                    else if (model.Boulders[x, y] != null)
                    {

                        if (nothingHere(x, y + 1))
                        {
                            markedFalling.Add(model.Boulders[x, y]);
                        }
                        else
                        {
                            if (model.Boulders[x, y].Falling)
                            {
                                model.Boulders[x, y].Falling = false;
                                TryExplode(x, y + 1);
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

                    model.Diamonds[(int)m.TilePosition.X, (int)m.TilePosition.Y] = null;
                    model.Diamonds[(int)m.TilePosition.X, (int)m.TilePosition.Y + 1] = m as Blocks.Diamond;
                }
                else if (m is Blocks.Boulder)
                {
                    model.Boulders[(int)m.TilePosition.X, (int)m.TilePosition.Y] = null;
                    model.Boulders[(int)m.TilePosition.X, (int)m.TilePosition.Y + 1] = m as Blocks.Boulder;
                }
                m.TileOldPosition = m.TilePosition;
                m.TilePosition.Y++;
            }

        }

        private void CheckExit()
        {
            if (model.RequireDiamonds == model.CollectedDiamonds)
            {
                model.Exit.Open();
            }
        }

        private bool isRoundHere(int x, int y)
        {
            if(model.Diamonds[x,y]!=null || model.Boulders[x, y] != null || model.WallMatrix[x, y])
            {
                return true;
            }
            return false;
        }

        public void ReduceAllExplodes()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (model.Explosion[x, y] > 0)
                    {
                        model.Explosion[x, y]--;
                    }
                }
            }
        }
        private void TryExplode(int centerX, int centerY)
        {
            if(model.Rockford!=null && model.Rockford.TilePosition.X == centerX && model.Rockford.TilePosition.Y == centerY)
            {
                for (int x = centerX - 1; x < centerX + 2; x++)
                {
                    for (int y = centerY - 1; y < centerY + 2; y++)
                    {
                        if (x < model.Width && x >= 0 && y < model.Height && y >= 0)
                        {
                            model.Explosion[x, y] =2;
                            model.WallMatrix[x, y] = false;
                            model.DirtMatrix[x, y] = null;
                            model.Diamonds[x, y] = null;
                            model.Boulders[x, y] = null;
                            model.Fireflies[x, y] = null;
                            if (model.Rockford != null)
                            {
                                if (model.Rockford.TilePosition.X == x && model.Rockford.TilePosition.Y == y)
                                {
                                    model.Rockford = null;
                                    model.GameOver = true;
                                }
                            }

                        }

                    }
                }
            }
           
        }

        public bool CollectedEnought()
        {
            return model.RequireDiamonds <= model.CollectedDiamonds;
        }
    }
}
