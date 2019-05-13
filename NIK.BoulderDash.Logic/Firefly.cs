// <copyright file="Firefly.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NIK.BoulderDash.Logic
{
    using System.Windows;

    /// <summary>
    /// Class Firefly. This enemi can explode if a somethng fall on it or meet with rockford in the scan area.
    /// Implements the <see cref="NIK.BoulderDash.Logic.Enemie" />.
    /// </summary>
    /// <seealso cref="NIK.BoulderDash.Logic.Enemie" />.
    public class Firefly : Enemie
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Firefly"/> class.
        /// </summary>
        /// <param name="initPosition">The initialize position.</param>
        public Firefly(Point initPosition)
        {
            this.TilePosition = initPosition;
            this.TileOldPosition = initPosition;
            this.FaceDirection = Direction.Left;
        }

        /// <summary>
        /// check the specified obstacle and Make a step.
        /// Always try to turm left if possible he turn left, if not go forward, if forward also not possible turn right.
        /// </summary>
        /// <param name="obstacle">The obstacle.</param>
        public override void Step(bool[,] obstacle)
        {
            var primTarget = this.CalcUnit(Direction.Left);
            var secTarget = this.CalcUnit(Direction.Up);
            if (!obstacle[(int)primTarget.X, (int)primTarget.Y])
            {
                this.FaceDirection = this.GetLeft();
                this.Move(primTarget);
            }
            else if (!obstacle[(int)secTarget.X, (int)secTarget.Y])
            {
                this.Move(secTarget);
            }
            else
            {
                this.FaceDirection = this.GetRight();
            }
        }
    }
}
