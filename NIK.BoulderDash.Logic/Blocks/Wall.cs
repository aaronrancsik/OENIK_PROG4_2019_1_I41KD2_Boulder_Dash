// <copyright file="Wall.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NIK.BoulderDash.Logic.Blocks
{
    /// <summary>
    /// Class Wall.
    /// Implements the <see cref="NIK.BoulderDash.Logic.Blocks.Block" />
    /// Implements the <see cref="NIK.BoulderDash.Logic.Blocks.IVariable" />.
    /// </summary>
    /// <seealso cref="NIK.BoulderDash.Logic.Blocks.Block" />
    /// <seealso cref="NIK.BoulderDash.Logic.Blocks.IVariable" />
    public class Wall : Block, IVariable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Wall"/> class.
        /// </summary>
        public Wall()
            : base(true)
        {
        }

        /// <summary>
        /// Gets or sets the variant.
        /// </summary>
        /// <value>The variant.</value>
        public int Variant { get; set; }
    }
}
