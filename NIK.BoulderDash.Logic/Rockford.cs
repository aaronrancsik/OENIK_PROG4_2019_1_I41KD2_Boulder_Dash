// <copyright file="Rockford.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NIK.BoulderDash.Logic
{
    using NIK.BoulderDash.Logic.Blocks;

    /// <summary>
    /// Represent rockford`s state.
    /// </summary>
    public enum State
    {
        /// <summary>
        /// up
        /// </summary>
        Up,

        /// <summary>
        /// down
        /// </summary>
        Down,

        /// <summary>
        /// left
        /// </summary>
        Left,

        /// <summary>
        /// right
        /// </summary>
        Right,

        /// <summary>
        /// stand, when not moving
        /// </summary>
        Stand,

        /// <summary>
        /// birth
        /// </summary>
        Birth,
    }

    /// <summary>
    /// Class Rockford. The player.
    /// Implements the <see cref="NIK.BoulderDash.Logic.Blocks.DynamicBlock" />.
    /// </summary>
    /// <seealso cref="NIK.BoulderDash.Logic.Blocks.DynamicBlock" />
    public class Rockford : DynamicBlock
    {
        /// <summary>
        /// The direction.
        /// </summary>
        private State direction = State.Right;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rockford"/> class.
        /// </summary>
        public Rockford()
            : base(true, false)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance last move was right or not.
        /// </summary>
        /// <value><c>true</c> if this instance is last move was right; otherwise, <c>false</c>.</value>
        public bool IsLastMoveWasRight { get; private set; } = true;

        /// <summary>
        /// Gets or sets the moving state of player and change isLastMoveWasRight if needed.
        /// </summary>
        public State Direaction
        {
            get
            {
                return this.direction;
            }

            set
            {
                if (value == State.Right)
                {
                    this.IsLastMoveWasRight = true;
                }
                else if (value == State.Left)
                {
                    this.IsLastMoveWasRight = false;
                }

                this.direction = value;
            }
        }
    }
}
