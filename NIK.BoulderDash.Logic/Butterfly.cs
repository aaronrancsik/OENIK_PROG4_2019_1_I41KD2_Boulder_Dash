// <copyright file="Butterfly.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NIK.BoulderDash.Logic
{
    using System.Windows;

    /// <summary>
    /// Class Butterfly.
    /// Implements the <see cref="NIK.BoulderDash.Logic.Enemie" />.
    /// </summary>
    /// <seealso cref="NIK.BoulderDash.Logic.Enemie" />
    public class Butterfly : Enemie
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Butterfly"/> class.
        /// </summary>
        /// <param name="initPosition">The initialize position.</param>
        public Butterfly(Point initPosition)
        {
            this.TilePosition = initPosition;
            this.TileOldPosition = initPosition;
            this.FaceDirection = Direction.Down;
        }

        /// <summary>
        /// Make a step and can check the specified obstacle.
        /// Always try to turm right if possible he turn right, if not go forward, if forward also not possible turn left.
        /// </summary>
        /// <param name="obstacle">The obstacle.</param>
        public override void Step(bool[,] obstacle)
        {
            var primTarget = this.CalcUnit(Direction.Right);
            var secTarget = this.CalcUnit(Direction.Up);
            if (!obstacle[(int)primTarget.X, (int)primTarget.Y])
            {
                this.FaceDirection = this.GetRight();
                this.Move(primTarget);
            }
            else if (!obstacle[(int)secTarget.X, (int)secTarget.Y])
            {
                this.Move(secTarget);
            }
            else
            {
                this.FaceDirection = this.GetLeft();
            }
        }
    }
}
