// <copyright file="Titanium.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NIK.BoulderDash.Logic.Blocks
{
    /// <summary>
    /// Class Titanium. The unterminateable block. Usually around the map.
    /// Implements the <see cref="NIK.BoulderDash.Logic.Blocks.Block" />
    /// Implements the <see cref="NIK.BoulderDash.Logic.Blocks.IVariable" />.
    /// </summary>
    /// <seealso cref="NIK.BoulderDash.Logic.Blocks.Block" />
    /// <seealso cref="NIK.BoulderDash.Logic.Blocks.IVariable" />
    internal class Titanium : Block, IVariable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Titanium"/> class.
        /// </summary>
        public Titanium()
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
