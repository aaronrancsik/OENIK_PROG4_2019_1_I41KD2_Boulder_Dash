// <copyright file="Block.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NIK.BoulderDash.Logic.Blocks
{
    /// <summary>
    /// Class Block. Base of lots of blocks.
    /// </summary>
    public class Block
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Block"/> class.
        /// </summary>
        /// <param name="isRounded">if set to <c>true</c> [is rounded].</param>
        public Block(bool isRounded)
        {
            this.IsRounded = isRounded;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is rounded. If rouneded can roll of from edges.
        /// </summary>
        /// <value><c>true</c> if this instance is rounded; otherwise, <c>false</c>.</value>
        public bool IsRounded { get; private set; }
    }
}
