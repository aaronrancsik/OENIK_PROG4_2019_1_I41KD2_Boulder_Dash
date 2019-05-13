// <copyright file="Camera.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NIK.BoulderDash.Logic
{
    using System;
    using System.Windows;

    /// <summary>
    /// Camera class can follow points and can calculate the visible coordinates around followed point on map.
    /// </summary>
    public class Camera
    {
        private bool first = true;
        private Point center;
        private Point centerOld;

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        public Camera()
        {
            // center = new Point(-AngleWidthTile, -AngleHeightTile);
        }

        /// <summary>
        /// Gets the camera`s viewport width in tile.
        /// </summary>
        public int AngleWidthTile { get; } = 20;

        /// <summary>
        ///  Gets the camera`s viewport hieght in tile.
        /// </summary>
        public int AngleHeightTile { get;  } = 12;

        /// <summary>
        /// Gets the center of camera.
        /// </summary>
        /// <value>The center.</value>
        public ref Point Center { get => ref this.center; }

        /// <summary>
        /// Gets the old center usefull for animations.
        /// </summary>
        /// <value>The center old.</value>
        public ref Point CenterOld { get => ref this.centerOld; }

        /// <summary>
        /// Follows the specified target. Set the camera center. Whatch the corne of map.
        /// </summary>
        /// <param name="target">The target.</param>
        public void Follow(Point target)
        {
            this.centerOld = this.center;

            if (target.X < this.AngleWidthTile / 2)
            {
                target.X = this.AngleWidthTile / 2;
            }

            if (target.X > this.AngleWidthTile * 1.5)
            {
                target.X = this.AngleWidthTile * 1.5;
            }

            if (target.Y < this.AngleHeightTile / 2)
            {
                target.Y = this.AngleHeightTile / 2;
            }

            if (target.Y > (this.AngleHeightTile * 1.5) - 2)
            {
                target.Y = (this.AngleHeightTile * 1.5) - 2;
            }

            if (this.first)
            {
                this.Center = target;
                this.CenterOld = target;
                this.first = false;
            }

            if (Math.Abs(this.Center.X - target.X) >= 5)
            {
                this.Center.X = target.X;
            }

            if (Math.Abs(this.Center.Y - target.Y) >= 2)
            {
                this.Center.Y = target.Y;
            }
        }

        /// <summary>
        /// IsInStage determine is a point on visible by the camera or not.
        /// </summary>
        /// <param name="p">the point we want to check.</param>
        /// <returns>true if the point is visible by the current camera.</returns>
        public bool IsInStage(Point p)
        {
            if ((p.X + 6) > (this.center.X - (this.AngleWidthTile / 2)) && p.X - 5 < (this.center.X + (this.AngleWidthTile / 2)))
            {
                if (p.Y + 2 > (this.center.Y - this.AngleHeightTile - (2 / 2)) && p.Y - 2 < (this.center.Y + (this.AngleHeightTile / 2)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}