// <copyright file="Enemie.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NIK.BoulderDash.Logic
{
    using System;
    using System.Windows;
    using NIK.BoulderDash.Logic.Blocks;

    /// <summary>
    /// Class Enemie. The reuseble baseclass for enemies.
    /// Implements the <see cref="NIK.BoulderDash.Logic.Blocks.DynamicBlock" />.
    /// </summary>
    /// <seealso cref="NIK.BoulderDash.Logic.Blocks.DynamicBlock" />
    public abstract class Enemie : DynamicBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Enemie"/> class.
        /// No instance can create frow this class because its abstarct, but that shit stylecop forced me to write the standard text.
        /// Just set the explosive to true and rounded to false.
        /// </summary>
        public Enemie()
            : base(true, false)
        {
        }

        /// <summary>
        /// Gets or sets the face direction. Where is it facing in global view.
        /// </summary>
        /// <value>The face direction.</value>
        protected Direction FaceDirection { get; set; }

        /// <summary>
        /// Make a step and can check the specified obstacle.
        /// </summary>
        /// <param name="obstacle">The obstacle.</param>
        public virtual void Step(bool[,] obstacle)
        {
        }

        /// <summary>
        /// Move to the specified point.
        /// </summary>
        /// <param name="p">The point.</param>
        protected void Move(Point p)
        {
            this.TileOldPosition = this.TilePosition;
            this.TilePosition.X = p.X;
            this.TilePosition.Y = p.Y;
        }

        /// <summary>
        /// Turn the face direction left.
        /// </summary>
        /// <returns>Direction.</returns>
        /// <exception cref="Exception">Bad direction.</exception>
        protected Direction GetLeft()
        {
            if (this.FaceDirection == Direction.Left)
            {
                return Direction.Down;
            }
            else if (this.FaceDirection == Direction.Down)
            {
                return Direction.Right;
            }
            else if (this.FaceDirection == Direction.Right)
            {
                return Direction.Up;
            }
            else if (this.FaceDirection == Direction.Up)
            {
                return Direction.Left;
            }

            throw new Exception("Bad direction");
        }

        /// <summary>
        /// Turn the face direction right.
        /// </summary>
        /// <returns>Direction.</returns>
        /// <exception cref="Exception">Bad direction.</exception>
        protected Direction GetRight()
        {
            if (this.FaceDirection == Direction.Left)
            {
                return Direction.Up;
            }
            else if (this.FaceDirection == Direction.Down)
            {
                return Direction.Left;
            }
            else if (this.FaceDirection == Direction.Right)
            {
                return Direction.Down;
            }
            else if (this.FaceDirection == Direction.Up)
            {
                return Direction.Right;
            }

            throw new Exception("Bad direction");
        }

        /// <summary>
        /// Calculates the coordinates from the face direction and the specifien direction. Ex. If face is up and the dir is left the unit will be the coordinates globally left.
        /// Ex2. If the face is left and dir is left, the unit will be the coordinates globally down. And so on.
        /// </summary>
        /// <param name="dir">The relative direction.</param>
        /// <returns>Golobal Point.</returns>
        protected Point CalcUnit(Direction dir)
        {
            if (this.FaceDirection == Direction.Up)
            {
                if (dir == Direction.Left)
                {
                    return new Point(this.TilePosition.X - 1, this.TilePosition.Y);
                }
                else if (dir == Direction.Right)
                {
                    return new Point(this.TilePosition.X + 1, this.TilePosition.Y);
                }
                else if (dir == Direction.Up)
                {
                    return new Point(this.TilePosition.X, this.TilePosition.Y - 1);
                }
                else if (dir == Direction.Down)
                {
                    return new Point(this.TilePosition.X, this.TilePosition.Y + 1);
                }
            }
            else if (this.FaceDirection == Direction.Down)
            {
                if (dir == Direction.Left)
                {
                    return new Point(this.TilePosition.X + 1, this.TilePosition.Y);
                }
                else if (dir == Direction.Right)
                {
                    return new Point(this.TilePosition.X - 1, this.TilePosition.Y);
                }
                else if (dir == Direction.Up)
                {
                    return new Point(this.TilePosition.X, this.TilePosition.Y + 1);
                }
                else if (dir == Direction.Down)
                {
                    return new Point(this.TilePosition.X, this.TilePosition.Y - 1);
                }
            }
            else if (this.FaceDirection == Direction.Left)
            {
                if (dir == Direction.Left)
                {
                    return new Point(this.TilePosition.X, this.TilePosition.Y + 1);
                }
                else if (dir == Direction.Right)
                {
                    return new Point(this.TilePosition.X, this.TilePosition.Y - 1);
                }
                else if (dir == Direction.Up)
                {
                    return new Point(this.TilePosition.X - 1, this.TilePosition.Y);
                }
                else if (dir == Direction.Down)
                {
                    return new Point(this.TilePosition.X + 1, this.TilePosition.Y);
                }
            }
            else if (this.FaceDirection == Direction.Right)
            {
                if (dir == Direction.Left)
                {
                    return new Point(this.TilePosition.X, this.TilePosition.Y - 1);
                }
                else if (dir == Direction.Right)
                {
                    return new Point(this.TilePosition.X, this.TilePosition.Y + 1);
                }
                else if (dir == Direction.Up)
                {
                    return new Point(this.TilePosition.X + 1, this.TilePosition.Y);
                }
                else if (dir == Direction.Down)
                {
                    return new Point(this.TilePosition.X - 1, this.TilePosition.Y);
                }
            }

            return new Point(0, 0);
        }
    }
}
