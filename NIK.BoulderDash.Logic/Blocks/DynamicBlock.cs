// <copyright file="DynamicBlock.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NIK.BoulderDash.Logic.Blocks
{
    using System.Windows;

    /// <summary>
    /// Class DynamicBlock. Can be move or animate.
    /// Implements the <see cref="NIK.BoulderDash.Logic.Blocks.Block" />.
    /// </summary>
    /// <seealso cref="NIK.BoulderDash.Logic.Blocks.Block" />
    public class DynamicBlock : Block
    {
        private Point tilePosition;
        private Point tileOldPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicBlock"/> class.
        /// </summary>
        /// <param name="explosive">if set to <c>true</c> [explosive].</param>
        /// <param name="isRounded">if set to <c>true</c> [is rounded].</param>
        public DynamicBlock(bool explosive, bool isRounded)
            : base(isRounded)
        {
            this.Explosive = explosive;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="DynamicBlock"/> is explosive.
        /// </summary>
        /// <value><c>true</c> if explosive; otherwise, <c>false</c>.</value>
        public bool Explosive { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DynamicBlock"/> is falling.
        /// </summary>
        /// <value><c>true</c> if falling; otherwise, <c>false</c>.</value>
        public bool Falling { get; set; }

        /// <summary>
        /// Gets the tile old position. Usefull for animations.
        /// </summary>
        /// <value>The tile old position.</value>
        public ref Point TileOldPosition { get => ref this.tileOldPosition; }

        /// <summary>
        /// Gets current the tile position.
        /// </summary>
        /// <value>The tile position.</value>
        public ref Point TilePosition
        {
            get
            {
                // tileOldPosition.X = tilePosition.X;
                // tileOldPosition.Y = tilePosition.Y;
                return ref this.tilePosition;
            }
        }
    }
}
