// <copyright file="Exit.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NIK.BoulderDash.Logic.Blocks
{
    /// <summary>
    /// Class Exit. Can be open or close and player can finsih the map is go in here.
    /// Implements the <see cref="NIK.BoulderDash.Logic.Blocks.DynamicBlock" />.
    /// </summary>
    /// <seealso cref="NIK.BoulderDash.Logic.Blocks.DynamicBlock" />
    public class Exit : DynamicBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Exit"/> class.
        /// </summary>
        public Exit()
            : base(false, false)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public bool IsOpen { get; private set; }

        /// <summary>
        /// Opens this instance.
        /// </summary>
        public void Open()
        {
            this.IsOpen = true;
        }
    }
}
