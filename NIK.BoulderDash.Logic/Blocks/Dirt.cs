// <copyright file="Dirt.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NIK.BoulderDash.Logic.Blocks
{
    /// <summary>
    /// Class Dirt.
    /// Implements the <see cref="NIK.BoulderDash.Logic.Blocks.Block" />
    /// Implements the <see cref="NIK.BoulderDash.Logic.Blocks.IVariable" />.
    /// </summary>
    /// <seealso cref="NIK.BoulderDash.Logic.Blocks.Block" />
    /// <seealso cref="NIK.BoulderDash.Logic.Blocks.IVariable" />
    public class Dirt : Block, IVariable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dirt"/> class.
        /// </summary>
        public Dirt()
            : base(false)
        {
        }

        /// <summary>
        /// Gets or sets the variant.
        /// </summary>
        /// <value>The variant.</value>
        public int Variant { get; set; }
    }
}
