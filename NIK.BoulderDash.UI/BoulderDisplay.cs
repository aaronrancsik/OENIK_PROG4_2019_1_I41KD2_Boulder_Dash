// <copyright file="BoulderDisplay.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NIK.BoulderDash.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using NIK.BoulderDash.Logic;

    /// <summary>
    /// Class BoulderDisplay.
    /// </summary>
    public class BoulderDisplay
    {
        private int moveTime;
        private GameModel model;
        private double width;
        private double height;
        private Dictionary<string, Brush> assetBrushes = new Dictionary<string, Brush>();
        private Dictionary<string, VisualBrush> animatedVisualBrushes = new Dictionary<string, VisualBrush>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BoulderDisplay"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="w">The width.</param>
        /// <param name="h">The height.</param>
        /// <param name="moveTime">The movetime.</param>
        /// <param name="animatedVisualBrushes">The animated visual brushes that we create from resources.</param>
        public BoulderDisplay(GameModel model, double w, double h, int moveTime, Dictionary<string, VisualBrush> animatedVisualBrushes)
        {
            this.animatedVisualBrushes = animatedVisualBrushes;
            this.moveTime = (int)moveTime;
            this.model = model;
            this.width = w;
            this.height = h;

            this.TileSize = Math.Min(
                w / model.Camera.AngleWidthTile,
                h / model.Camera.AngleHeightTile);

            foreach (DictionaryEntry item in Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true))
            {
                if ((item.Value as System.Drawing.Bitmap) != null)
                {
                    ImageBrush ib = new ImageBrush(this.Bitmap2BitmapImageSource(item.Value as System.Drawing.Bitmap));
                    ib.TileMode = TileMode.Tile;
                    ib.Viewport = new Rect(0, 0, this.TileSize, this.TileSize);
                    ib.ViewportUnits = BrushMappingMode.Absolute;
                    this.assetBrushes[item.Key.ToString()] = ib;
                }
            }

            foreach (var item in animatedVisualBrushes)
            {
                item.Value.Viewport = new Rect(0, 0, this.TileSize, this.TileSize);
            }

            this.titaniums = this.CreateTitaniumDrawing();
        }

        /// <summary>
        /// Gets the size of the tile.
        /// </summary>
        /// <value>The size of the tile.</value>
        public double TileSize { get; private set; }

        private ImageSource Bitmap2BitmapImageSource(System.Drawing.Bitmap bitmap)
        {
            var i =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                           bitmap.GetHbitmap(),
                           IntPtr.Zero,
                           Int32Rect.Empty,
                           System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            bitmap.Dispose();

            return i;
        }

#pragma warning disable SA1201 // Elements should appear in the correct order
        private Dictionary<Logic.Blocks.Boulder, Brush> boulderBrushCache = new Dictionary<Logic.Blocks.Boulder, Brush>();
#pragma warning restore SA1201 // Elements should appear in the correct order

        private Drawing GetBouldersDrawing()
        {
            Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, this.moveTime));

            var ggg = new DrawingGroup();
            foreach (var d in this.model.Boulders)
            {
                if (d != null)
                {
                    TranslateTransform boulderTranslate = new TranslateTransform(d.TilePosition.X * this.TileSize, d.TilePosition.Y * this.TileSize);
                    DoubleAnimation animX = new DoubleAnimation(d.TileOldPosition.X * this.TileSize, d.TilePosition.X * this.TileSize, duration);
                    DoubleAnimation animY = new DoubleAnimation(d.TileOldPosition.Y * this.TileSize, d.TilePosition.Y * this.TileSize, duration);
                    animX.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.2 };
                    animY.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.2 };
                    boulderTranslate.BeginAnimation(TranslateTransform.XProperty, animX);
                    boulderTranslate.BeginAnimation(TranslateTransform.YProperty, animY);

                    Geometry boulder = new RectangleGeometry(new Rect(0, 0, this.TileSize, this.TileSize));
                    boulder.Transform = boulderTranslate;

                    if (this.model.Camera.IsInStage(d.TilePosition))
                    {
                        ImageBrush brush;
                        if (!this.boulderBrushCache.ContainsKey(d))
                        {
                            Brush tmpBrush;
                            switch (d.Variant)
                            {
                                case 1:
                                    tmpBrush = this.assetBrushes[nameof(Properties.Resources.Boulder1)].Clone();
                                    break;
                                case 2:
                                    tmpBrush = this.assetBrushes[nameof(Properties.Resources.Boulder2)].Clone();
                                    break;
                                case 3:
                                    tmpBrush = this.assetBrushes[nameof(Properties.Resources.Boulder3)].Clone();
                                    break;
                                case 4:
                                    tmpBrush = this.assetBrushes[nameof(Properties.Resources.Boulder4)].Clone();
                                    break;
                                default: throw new Exception("Unknown boulder set");
                            }

                            this.boulderBrushCache[d] = tmpBrush;
                            brush = tmpBrush as ImageBrush;
                        }
                        else
                        {
                            brush = this.boulderBrushCache[d] as ImageBrush;
                        }

                        brush.TileMode = TileMode.None;
                        brush.Viewport = new Rect(0, 0, this.TileSize, this.TileSize);
                        brush.ViewportUnits = BrushMappingMode.Absolute;
                        brush.Transform = boulderTranslate;

                        d.TileOldPosition = d.TilePosition;
                        ggg.Children.Add(new GeometryDrawing(brush, null, boulder));
                    }
                }
            }

            return ggg;
        }

#pragma warning disable SA1201 // Elements should appear in the correct order
        private DrawingGroup firtGeoDrawGroup = new DrawingGroup();
#pragma warning restore SA1201 // Elements should appear in the correct order
        private Dictionary<Coord, GeometryDrawing> dirtGeoDraw = new Dictionary<Coord, GeometryDrawing>();

        private Drawing GetDirtsDrawing()
        {
            this.firtGeoDrawGroup = new DrawingGroup();
            for (int x = 0; x < this.model.Width; x++)
            {
                for (int y = 0; y < this.model.Height; y++)
                {
                    if (this.model.DirtMatrix[x, y] != null && this.model.Camera.IsInStage(new Point(x, y)))
                    {
                        Brush brush;
                        brush = this.assetBrushes["_" + (93 + (this.model.TextureSet * 4) + (this.model.DirtMatrix[x, y].Variant - 1)).ToString("000")];
                        var c = new Coord() { X = x * this.TileSize, Y = y * this.TileSize };
                        if (!this.dirtGeoDraw.ContainsKey(c))
                        {
                            this.dirtGeoDraw[c] = new GeometryDrawing(brush, null, new RectangleGeometry(new Rect(c.X, c.Y, this.TileSize, this.TileSize)));
                        }

                        this.firtGeoDrawGroup.Children.Add(this.dirtGeoDraw[c]);
                    }
                }
            }

            return this.firtGeoDrawGroup;
        }

        private Drawing GetWallsDrawing()
        {
            GeometryGroup wallgeo = new GeometryGroup();
            for (int x = 0; x < this.model.WallMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < this.model.WallMatrix.GetLength(1); y++)
                {
                    if (this.model.WallMatrix[x, y])
                    {
                        Geometry box = new RectangleGeometry(new Rect(x * this.TileSize, y * this.TileSize, this.TileSize, this.TileSize));
                        wallgeo.Children.Add(box);
                    }
                }
            }

            var ret = new GeometryDrawing(this.assetBrushes[nameof(Properties.Resources.Wall)], null, wallgeo);
            return ret;
        }

#pragma warning disable SA1201 // Elements should appear in the correct order
        private Drawing titaniums;
#pragma warning restore SA1201 // Elements should appear in the correct order

        private Drawing CreateTitaniumDrawing()
        {
            GeometryGroup tianiumGeo = new GeometryGroup();
            for (int x = 0; x < this.model.TitaniumMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < this.model.TitaniumMatrix.GetLength(1); y++)
                {
                    if (this.model.TitaniumMatrix[x, y])
                    {
                        Geometry box = new RectangleGeometry(new Rect(x * this.TileSize, y * this.TileSize, this.TileSize, this.TileSize));
                        tianiumGeo.Children.Add(box);
                    }
                }
            }

            return new GeometryDrawing(this.assetBrushes["TitanWall" + this.model.TextureSet], null, tianiumGeo);
        }

#pragma warning disable SA1201 // Elements should appear in the correct order
        private TranslateTransform rockfordTranslate;
#pragma warning restore SA1201 // Elements should appear in the correct order
        private VisualBrush rockfordVisualBrush;
        private Geometry rockfordGeo;

        private Drawing GetRockfordDrawing()
        {
            if (this.rockfordGeo == null)
            {
                this.rockfordGeo = new RectangleGeometry(new Rect(0, 0, this.TileSize, this.TileSize));
            }

            if (this.rockfordTranslate == null)
            {
                this.rockfordTranslate = new TranslateTransform(this.model.Rockford.TilePosition.X * this.TileSize, this.model.Rockford.TilePosition.Y * this.TileSize);
            }

            if (!this.model.Rockford.TileOldPosition.Equals(this.model.Rockford.TilePosition))
            {
                Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, this.moveTime));
                DoubleAnimation animX = new DoubleAnimation(this.model.Rockford.TileOldPosition.X * this.TileSize, this.model.Rockford.TilePosition.X * this.TileSize, duration);
                DoubleAnimation animY = new DoubleAnimation(this.model.Rockford.TileOldPosition.Y * this.TileSize, this.model.Rockford.TilePosition.Y * this.TileSize, duration);
                animX.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.2 };
                animY.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.2 };

                this.rockfordTranslate.BeginAnimation(TranslateTransform.XProperty, animX);
                this.rockfordTranslate.BeginAnimation(TranslateTransform.YProperty, animY);
            }

            this.rockfordGeo.Transform = this.rockfordTranslate;

            switch (this.model.Rockford.Direaction)
            {
                case State.Left:
                    this.rockfordVisualBrush = this.animatedVisualBrushes[nameof(Properties.Resources.RockfordLeft)];
                    break;
                case State.Right:
                    this.rockfordVisualBrush = this.animatedVisualBrushes[nameof(Properties.Resources.RockfordRight)];
                    break;
                case State.Stand:
                    this.rockfordVisualBrush = this.animatedVisualBrushes[nameof(Properties.Resources.RockfordTap)];
                    break;
                case State.Birth:
                    this.rockfordVisualBrush = this.animatedVisualBrushes[nameof(Properties.Resources.RockfordBirth)];
                    break;
                default:
                    this.rockfordVisualBrush = this.animatedVisualBrushes[this.model.Rockford.IsLastMoveWasRight ? nameof(Properties.Resources.RockfordRight) : nameof(Properties.Resources.RockfordLeft)];
                    break;
            }

            var brush = this.rockfordVisualBrush;
            brush.TileMode = TileMode.None;
            brush.Viewport = new Rect(0, 0, this.TileSize, this.TileSize);
            brush.ViewportUnits = BrushMappingMode.Absolute;
            brush.Transform = this.rockfordTranslate;
            this.model.Rockford.TileOldPosition = this.model.Rockford.TilePosition;
            return new GeometryDrawing(brush, null, this.rockfordGeo);
        }

#pragma warning disable SA1201 // Elements should appear in the correct order
        private Dictionary<Logic.Blocks.Diamond, VisualBrush> visualBrushCache = new Dictionary<Logic.Blocks.Diamond, VisualBrush>();
#pragma warning restore SA1201 // Elements should appear in the correct order

        private Drawing GetDiamondsDrawing()
        {
            Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, this.moveTime));
            var ggg = new DrawingGroup();
            foreach (var d in this.model.Diamonds)
            {
                if (d != null)
                {
                    TranslateTransform diamondTranslate = new TranslateTransform(d.TilePosition.X * this.TileSize, d.TilePosition.Y * this.TileSize);
                    if (!d.TilePosition.Equals(d.TileOldPosition) && this.model.Camera.IsInStage(d.TilePosition))
                    {
                        DoubleAnimation animX = new DoubleAnimation(d.TileOldPosition.X * this.TileSize, d.TilePosition.X * this.TileSize, duration);
                        DoubleAnimation animY = new DoubleAnimation(d.TileOldPosition.Y * this.TileSize, d.TilePosition.Y * this.TileSize, duration);
                        animX.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.2 };
                        animY.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.2 };

                        diamondTranslate.BeginAnimation(TranslateTransform.XProperty, animX);
                        diamondTranslate.BeginAnimation(TranslateTransform.YProperty, animY);
                    }

                    Geometry dia = new RectangleGeometry(new Rect(0, 0, this.TileSize, this.TileSize));
                    dia.Transform = diamondTranslate;
                    d.TileOldPosition = d.TilePosition;
                    if (this.model.Camera.IsInStage(d.TilePosition))
                    {
                        if (this.visualBrushCache.ContainsKey(d))
                        {
                            var brush = this.visualBrushCache[d];

                            // brush.TileMode = TileMode.None;
                            // brush.Viewport = new Rect(0, 0, TileSize, TileSize);
                            // brush.ViewportUnits = BrushMappingMode.Absolute;
                            brush.Transform = diamondTranslate;
                            if (brush.CanFreeze)
                            {
                                brush.Freeze();
                            }

                            ggg.Children.Add(new GeometryDrawing(brush, null, dia));
                        }
                        else
                        {
                            var brush = this.animatedVisualBrushes["Diamond" + this.model.TextureSet].Clone();
                            RenderOptions.SetCachingHint(brush, CachingHint.Cache);
                            this.visualBrushCache[d] = brush;

                            brush.TileMode = TileMode.None;
                            brush.Viewport = new Rect(0, 0, this.TileSize, this.TileSize);
                            brush.ViewportUnits = BrushMappingMode.Absolute;
                            brush.Transform = diamondTranslate;
                            ggg.Children.Add(new GeometryDrawing(brush, null, dia));
                        }
                    }
                }
            }

            return ggg;
        }

#pragma warning disable SA1201 // Elements should appear in the correct order
        private DrawingGroup mainDrGr = new DrawingGroup();
#pragma warning restore SA1201 // Elements should appear in the correct order
        private TranslateTransform cropTrans;
        private AnimationClock clockX;
        private AnimationClock clockY;

        /// <summary>
        /// The main drawing builder.
        /// </summary>
        /// <returns>Drawing.</returns>
        public Drawing BuildDrawing()
        {
            double toOffsetX = (1 - (this.model.Camera.Center.X * this.TileSize)) + (this.TileSize * this.model.Camera.AngleWidthTile / 2);
            double toOffsetY = (1 - (this.model.Camera.Center.Y * this.TileSize)) + (this.TileSize * this.model.Camera.AngleHeightTile / 2);
            double fromOffsetX = (1 - (this.model.Camera.CenterOld.X * this.TileSize)) + (this.TileSize * this.model.Camera.AngleWidthTile / 2);
            double fromOffsetY = (1 - (this.model.Camera.CenterOld.Y * this.TileSize)) + (this.TileSize * this.model.Camera.AngleHeightTile / 2);
            if (this.cropTrans == null)
            {
                this.cropTrans = new TranslateTransform(toOffsetX, toOffsetY);
            }

            RenderOptions.SetBitmapScalingMode(this.mainDrGr, BitmapScalingMode.NearestNeighbor);

            if (!this.model.Camera.Center.Equals(this.model.Camera.CenterOld))
            {
                Duration durX = new Duration(TimeSpan.FromMilliseconds(this.moveTime * 5));
                Duration durY = new Duration(TimeSpan.FromMilliseconds(this.moveTime * 2));
                DoubleAnimation cropAnimX = new DoubleAnimation(fromOffsetX, toOffsetX, durX);
                DoubleAnimation cropAnimY = new DoubleAnimation(fromOffsetY, toOffsetY, durY);
                cropAnimX.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.3 };

                // cropAnimY.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.2 };
                this.clockX = cropAnimX.CreateClock();
                this.clockY = cropAnimY.CreateClock();
                this.cropTrans.ApplyAnimationClock(TranslateTransform.XProperty, this.clockX);
                this.cropTrans.ApplyAnimationClock(TranslateTransform.YProperty, this.clockY);
            }

            this.mainDrGr.Transform = this.cropTrans;
            this.mainDrGr.Children = new DrawingCollection();
            this.mainDrGr.Children.Add(this.GetBackGroundDrawing());
            this.mainDrGr.Children.Add(this.titaniums);
            this.mainDrGr.Children.Add(this.GetWallsDrawing());
            this.mainDrGr.Children.Add(this.GetDirtsDrawing());
            this.mainDrGr.Children.Add(this.GetFirefliesDrawing());
            this.mainDrGr.Children.Add(this.GetButterfliesDrawing());
            this.mainDrGr.Children.Add(this.GetDiamondsDrawing());

            this.mainDrGr.Children.Add(this.GetExitDrawing());
            this.mainDrGr.Children.Add(this.GetBouldersDrawing());
            if (!this.model.GameOver)
            {
                this.mainDrGr.Children.Add(this.GetRockfordDrawing());
            }

            this.mainDrGr.Children.Add(this.GetExplodeDrawing(this.animatedVisualBrushes[nameof(Properties.Resources.Explode)]));

            return this.mainDrGr; // TODO minimalize new calls
        }

#pragma warning disable SA1201 // Elements should appear in the correct order
        private bool exitCache = false;
#pragma warning restore SA1201 // Elements should appear in the correct order
        private Drawing exit;

        private Drawing GetExitDrawing()
        {
            if (this.model.Exit.IsOpen == this.exitCache && this.exit != null)
            {
                return this.exit;
            }

            if (!this.model.Exit.IsOpen)
            {
                this.exitCache = this.model.Exit.IsOpen;

                this.exit = new GeometryDrawing(this.assetBrushes["ExitClose" + this.model.TextureSet], null, new RectangleGeometry(new Rect(this.model.Exit.TilePosition.X * this.TileSize, this.model.Exit.TilePosition.Y * this.TileSize, this.TileSize, this.TileSize)));
            }
            else
            {
                this.exitCache = this.model.Exit.IsOpen;
                this.exit = new GeometryDrawing(this.assetBrushes["ExitOpen" + this.model.TextureSet], null, new RectangleGeometry(new Rect(this.model.Exit.TilePosition.X * this.TileSize, this.model.Exit.TilePosition.Y * this.TileSize, this.TileSize, this.TileSize)));
            }

            return this.exit;
        }

#pragma warning disable SA1201 // Elements should appear in the correct order
        private Drawing bg;
#pragma warning restore SA1201 // Elements should appear in the correct order

        private Drawing GetBackGroundDrawing()
        {
            if (this.model.RequireDiamonds <= this.model.CollectedDiamonds && this.model.WhiteBgCount > 0)
            {
                this.bg = new GeometryDrawing(Brushes.White, null, new RectangleGeometry(new Rect(0, 0, this.model.WallMatrix.GetLength(0) * this.TileSize, this.model.WallMatrix.GetLength(1) * this.TileSize)));
                this.model.WhiteBgCount--;
                if (this.model.WhiteBgCount == 0)
                {
                    this.bg = null;
                }
            }

            if (this.bg == null)
            {
                this.bg = new GeometryDrawing(Brushes.Black, null, new RectangleGeometry(new Rect(0, 0, this.model.WallMatrix.GetLength(0) * this.TileSize, this.model.WallMatrix.GetLength(1) * this.TileSize)));
            }

            return this.bg;
        }

#pragma warning disable SA1201 // Elements should appear in the correct order
        private GeometryGroup explodeGG = new GeometryGroup();
#pragma warning restore SA1201 // Elements should appear in the correct order

        private Drawing GetExplodeDrawing(VisualBrush visualBrush)
        {
            visualBrush.TileMode = TileMode.Tile;
            visualBrush.Viewport = new Rect(0, 0, this.TileSize, this.TileSize);
            visualBrush.ViewportUnits = BrushMappingMode.Absolute;

            List<Geometry> marked = new List<Geometry>();
            foreach (var item in this.explodeGG.Children)
            {
                int y = (int)((item as RectangleGeometry).Rect.Y / this.TileSize);
                int x = (int)((item as RectangleGeometry).Rect.X / this.TileSize);
                if (this.model.Explosion[x, y] == 0)
                {
                    marked.Add(item);
                }
            }

            foreach (var item in marked)
            {
                this.explodeGG.Children.Remove(item);
            }

            for (int x = 0; x < this.model.Width; x++)
            {
                for (int y = 0; y < this.model.Height; y++)
                {
                    if (this.model.Explosion[x, y] > 0)
                    {
                        this.explodeGG.Children.Add(new RectangleGeometry(new Rect(x * this.TileSize, y * this.TileSize, this.TileSize, this.TileSize)));
                    }
                    else if (this.model.Explosion[x, y] == 0)
                    {
                    }
                }
            }

            return new GeometryDrawing(visualBrush, null, this.explodeGG);
        }

#pragma warning disable SA1201 // Elements should appear in the correct order
        private Dictionary<Firefly, VisualBrush> visualBrushFirefliesCache = new Dictionary<Firefly, VisualBrush>();
#pragma warning restore SA1201 // Elements should appear in the correct order

        private Drawing GetFirefliesDrawing()
        {
            Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, this.moveTime));
            var ggg = new DrawingGroup();
            foreach (var d in this.model.Fireflies)
            {
                if (d != null)
                {
                    TranslateTransform fireFlyTrans = new TranslateTransform(d.TilePosition.X * this.TileSize, d.TilePosition.Y * this.TileSize);
                    if (!d.TilePosition.Equals(d.TileOldPosition) && this.model.Camera.IsInStage(d.TilePosition))
                    {
                        DoubleAnimation animX = new DoubleAnimation(d.TileOldPosition.X * this.TileSize, d.TilePosition.X * this.TileSize, duration);
                        DoubleAnimation animY = new DoubleAnimation(d.TileOldPosition.Y * this.TileSize, d.TilePosition.Y * this.TileSize, duration);
                        animX.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.2 };
                        animY.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.2 };

                        fireFlyTrans.BeginAnimation(TranslateTransform.XProperty, animX);
                        fireFlyTrans.BeginAnimation(TranslateTransform.YProperty, animY);
                    }

                    Geometry fireflyGeo = new RectangleGeometry(new Rect(0, 0, this.TileSize, this.TileSize));
                    fireflyGeo.Transform = fireFlyTrans;
                    d.TileOldPosition = d.TilePosition;
                    if (this.model.Camera.IsInStage(d.TilePosition))
                    {
                        VisualBrush brush;
                        if (this.visualBrushFirefliesCache.ContainsKey(d))
                        {
                            brush = this.visualBrushFirefliesCache[d];

                            ggg.Children.Add(new GeometryDrawing(brush, null, fireflyGeo));
                        }
                        else
                        {
                            brush = this.animatedVisualBrushes["Firefly" + this.model.TextureSet].Clone();
                            RenderOptions.SetCachingHint(brush, CachingHint.Cache);
                            this.visualBrushFirefliesCache[d] = brush;

                            ggg.Children.Add(new GeometryDrawing(brush, null, fireflyGeo));
                        }

                        brush.TileMode = TileMode.None;
                        brush.Viewport = new Rect(0, 0, this.TileSize, this.TileSize);
                        brush.ViewportUnits = BrushMappingMode.Absolute;
                        brush.Transform = fireFlyTrans;
                    }
                }
            }

            return ggg;
        }

#pragma warning disable SA1201 // Elements should appear in the correct order
        private Dictionary<Butterfly, VisualBrush> visualBrushButterfliesCache = new Dictionary<Butterfly, VisualBrush>();
#pragma warning restore SA1201 // Elements should appear in the correct order

        private Drawing GetButterfliesDrawing()
        {
            Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, this.moveTime));
            var ggg = new DrawingGroup();
            foreach (var d in this.model.Butterflies)
            {
                if (d != null)
                {
                    TranslateTransform butterflyTrans = new TranslateTransform(d.TilePosition.X * this.TileSize, d.TilePosition.Y * this.TileSize);
                    if (!d.TilePosition.Equals(d.TileOldPosition) && this.model.Camera.IsInStage(d.TilePosition))
                    {
                        DoubleAnimation animX = new DoubleAnimation(d.TileOldPosition.X * this.TileSize, d.TilePosition.X * this.TileSize, duration);
                        DoubleAnimation animY = new DoubleAnimation(d.TileOldPosition.Y * this.TileSize, d.TilePosition.Y * this.TileSize, duration);
                        animX.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.2 };
                        animY.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.2 };

                        butterflyTrans.BeginAnimation(TranslateTransform.XProperty, animX);
                        butterflyTrans.BeginAnimation(TranslateTransform.YProperty, animY);
                    }

                    Geometry butterflyGeo = new RectangleGeometry(new Rect(0, 0, this.TileSize, this.TileSize));
                    butterflyGeo.Transform = butterflyTrans;
                    d.TileOldPosition = d.TilePosition;
                    if (this.model.Camera.IsInStage(d.TilePosition))
                    {
                        VisualBrush brush;
                        if (this.visualBrushButterfliesCache.ContainsKey(d))
                        {
                            brush = this.visualBrushButterfliesCache[d];

                            ggg.Children.Add(new GeometryDrawing(brush, null, butterflyGeo));
                        }
                        else
                        {
                            brush = this.animatedVisualBrushes["Butterfly" + this.model.TextureSet].Clone();
                            RenderOptions.SetCachingHint(brush, CachingHint.Cache);
                            this.visualBrushButterfliesCache[d] = brush;

                            ggg.Children.Add(new GeometryDrawing(brush, null, butterflyGeo));
                        }

                        brush.TileMode = TileMode.None;
                        brush.Viewport = new Rect(0, 0, this.TileSize, this.TileSize);
                        brush.ViewportUnits = BrushMappingMode.Absolute;
                        brush.Transform = butterflyTrans;
                    }
                }
            }

            return ggg;
        }

        /// <summary>
        /// Simple Struct Coord require for performance porpouse.
        /// </summary>
        internal struct Coord
        {
            /// <summary>
            /// The x.
            /// </summary>
            public double X;

            /// <summary>
            /// The y.
            /// </summary>
            public double Y;
        }
    }
}
