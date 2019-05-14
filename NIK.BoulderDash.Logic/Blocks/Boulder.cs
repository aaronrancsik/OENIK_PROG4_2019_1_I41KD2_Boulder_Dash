// <copyright file="Boulder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NIK.BoulderDash.Logic.Blocks
{
    /// <summary>
    /// Class Boulder.
    /// Implements the <see cref="NIK.BoulderDash.Logic.Blocks.DynamicBlock" />
    /// Implements the <see cref="NIK.BoulderDash.Logic.Blocks.IVariable" />.
    /// </summary>
    /// <seealso cref="NIK.BoulderDash.Logic.Blocks.DynamicBlock" />
    /// <seealso cref="NIK.BoulderDash.Logic.Blocks.IVariable" />
    public class Boulder : DynamicBlock, IVariable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Boulder"/> class. Set explosive false and round true.
        /// </summary>
        public Boulder()
            : base(false, true)
        {
        }

        /// <summary>
        /// Gets or sets the variant each variant only change the texture on display.
        /// </summary>
        /// <value>The variant.</value>
        public int Variant { get; set; }
    }
}
