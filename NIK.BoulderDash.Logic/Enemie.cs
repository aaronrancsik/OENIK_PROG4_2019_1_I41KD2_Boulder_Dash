using NIK.BoulderDash.Logic.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NIK.BoulderDash.Logic
{
    public abstract class Enemie : DynamicBlock
    {
        public Enemie() : base(true, false)
        {
        }
        protected Direction FaceDirection { get; set; }
        virtual public void Step(bool[,] obstacle) { }

        protected void move(Point p)
        {
            TileOldPosition = TilePosition;
            TilePosition.X = p.X;
            TilePosition.Y = p.Y;
        }

        protected Direction GetLeft()
        {
            if (FaceDirection == Direction.Left)
                return Direction.Down;
            else if (FaceDirection == Direction.Down)
                return Direction.Right;
            else if (FaceDirection == Direction.Right)
                return Direction.Up;
            else if (FaceDirection == Direction.Up)
                return Direction.Left;
            throw new Exception("Bad direction");
        }
        protected Direction GetRight()
        {
            if (FaceDirection == Direction.Left)
                return Direction.Up;
            else if (FaceDirection == Direction.Down)
                return Direction.Left;
            else if (FaceDirection == Direction.Right)
                return Direction.Down;
            else if (FaceDirection == Direction.Up)
                return Direction.Right;
            throw new Exception("Bad direction");
        }
        protected Point calcUnit(Direction dir)
        {
            if (FaceDirection == Direction.Up)
            {
                if (dir == Direction.Left)
                {
                    return new Point(TilePosition.X - 1, TilePosition.Y);
                }
                else if (dir == Direction.Right)
                {
                    return new Point(TilePosition.X + 1, TilePosition.Y);
                }
                else if (dir == Direction.Up)
                {
                    return new Point(TilePosition.X, TilePosition.Y - 1);
                }
                else if (dir == Direction.Down)
                {
                    return new Point(TilePosition.X, TilePosition.Y + 1);
                }
            }
            else if (FaceDirection == Direction.Down)
            {
                if (dir == Direction.Left)
                {
                    return new Point(TilePosition.X + 1, TilePosition.Y);

                }
                else if (dir == Direction.Right)
                {
                    return new Point(TilePosition.X - 1, TilePosition.Y);
                }
                else if (dir == Direction.Up)
                {
                    return new Point(TilePosition.X, TilePosition.Y + 1);
                }
                else if (dir == Direction.Down)
                {
                    return new Point(TilePosition.X, TilePosition.Y - 1);
                }
            }
            else if (FaceDirection == Direction.Left)
            {
                if (dir == Direction.Left)
                {
                    return new Point(TilePosition.X, TilePosition.Y + 1);
                }
                else if (dir == Direction.Right)
                {
                    return new Point(TilePosition.X, TilePosition.Y - 1);

                }
                else if (dir == Direction.Up)
                {
                    return new Point(TilePosition.X - 1, TilePosition.Y);

                }
                else if (dir == Direction.Down)
                {
                    return new Point(TilePosition.X + 1, TilePosition.Y);
                }
            }
            else if (FaceDirection == Direction.Right)
            {
                if (dir == Direction.Left)
                {
                    return new Point(TilePosition.X, TilePosition.Y - 1);
                }
                else if (dir == Direction.Right)
                {
                    return new Point(TilePosition.X, TilePosition.Y + 1);
                }
                else if (dir == Direction.Up)
                {
                    return new Point(TilePosition.X + 1, TilePosition.Y);
                }
                else if (dir == Direction.Down)
                {
                    return new Point(TilePosition.X - 1, TilePosition.Y);
                }
            }
            return new Point(0, 0);

        }
    }
}
