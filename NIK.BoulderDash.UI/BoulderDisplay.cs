using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using NIK.BoulderDash.Logic;
using NIK.BoulderDash.Logic.Blocks;

namespace NIK.BoulderDash.UI
{
    struct Coord
    {
        public double x;
        public double y;
    }
    public class BoulderDisplay
    {
        int MOVETIME;
        GameModel model;
        double width;
        double height;
        public double TileSize { get; private set; }
        Dictionary<string, Brush> assetBrushes = new Dictionary<string, Brush>();
        Dictionary<string, VisualBrush> animatedVisualBrushes = new Dictionary<string, VisualBrush>();

        public BoulderDisplay(GameModel model, double w, double h, int MOVETIME, Dictionary<string, VisualBrush> animatedVisualBrushes)
        {
            this.animatedVisualBrushes = animatedVisualBrushes;
            this.MOVETIME = (int)MOVETIME;
            this.model = model;
            this.width = w;
            this.height = h;

            TileSize = Math.Min(
                w / model.Camera.AngleWidthTile,
                h / model.Camera.AngleHeightTile
            );

            foreach (DictionaryEntry item in Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true))
            {
                if ((item.Value as System.Drawing.Bitmap) != null)
                {
                    ImageBrush ib = new ImageBrush(Bitmap2BitmapImageSource(item.Value as System.Drawing.Bitmap));
                    ib.TileMode = TileMode.Tile;
                    ib.Viewport = new Rect(0, 0, TileSize, TileSize);
                    ib.ViewportUnits = BrushMappingMode.Absolute;
                    assetBrushes[item.Key.ToString()] = ib;
                }
            }
            foreach (var item in animatedVisualBrushes)
            {
                item.Value.Viewport = new Rect(0, 0, TileSize, TileSize);
            }

            
            titaniums = createTitaniumDrawing();
        }
        private ImageSource Bitmap2BitmapImageSource(System.Drawing.Bitmap bitmap)
        {
            var i =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                           bitmap.GetHbitmap(),
                           IntPtr.Zero,
                           Int32Rect.Empty,
                           System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions()
                );
            bitmap.Dispose();
            
            return i;

        }

        Dictionary<Logic.Blocks.Boulder, Brush> boulderBrushCache = new Dictionary<Logic.Blocks.Boulder, Brush>();
        private Drawing getBouldersDrawing()
        {
            Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, MOVETIME));
            
            var ggg = new DrawingGroup();
            foreach (var d in model.Boulders)
            {
                if (d != null)
                {
                    
                    TranslateTransform boulderTranslate = new TranslateTransform(d.TilePosition.X * TileSize, d.TilePosition.Y * TileSize);
                    DoubleAnimation animX = new DoubleAnimation(d.TileOldPosition.X * TileSize, d.TilePosition.X * TileSize, duration);
                    DoubleAnimation animY = new DoubleAnimation(d.TileOldPosition.Y * TileSize, d.TilePosition.Y * TileSize, duration);
                    animX.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.2 };
                    animY.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.2 };
                    boulderTranslate.BeginAnimation(TranslateTransform.XProperty, animX);
                    boulderTranslate.BeginAnimation(TranslateTransform.YProperty, animY);

                    Geometry boulder = new RectangleGeometry(new Rect(0,0, TileSize, TileSize));
                    boulder.Transform = boulderTranslate;
                    
                    if (model.Camera.isInStage(d.TilePosition))
                    {
                        ImageBrush brush;
                        if (!boulderBrushCache.ContainsKey(d))
                        {
                            Brush tmpBrush;
                            switch (d.Variant)
                            {
                                case 1:
                                    tmpBrush = assetBrushes[nameof(Properties.Resources.Boulder1)].Clone();
                                    break;
                                case 2:
                                    tmpBrush = assetBrushes[nameof(Properties.Resources.Boulder2)].Clone();
                                    break;
                                case 3:
                                    tmpBrush = assetBrushes[nameof(Properties.Resources.Boulder3)].Clone();
                                    break;
                                case 4:
                                    tmpBrush = assetBrushes[nameof(Properties.Resources.Boulder4)].Clone();
                                    break;
                                default: throw new Exception("Unknown boulder set");
                            }
                            boulderBrushCache[d] = tmpBrush;
                            brush = tmpBrush as ImageBrush;
                        }
                        else
                        {
                            brush = boulderBrushCache[d] as ImageBrush;
                        }
                        brush.TileMode = TileMode.None;
                        brush.Viewport = new Rect(0, 0, TileSize, TileSize);
                        brush.ViewportUnits = BrushMappingMode.Absolute;
                        brush.Transform = boulderTranslate;

                        d.TileOldPosition = d.TilePosition;
                        ggg.Children.Add(new GeometryDrawing(brush, null, boulder));

                    }
                   
                }

            }
            return ggg;
        }

        DrawingGroup firtGeoDrawGroup = new DrawingGroup();
        Dictionary<Coord, GeometryDrawing> dirtGeoDraw = new Dictionary<Coord, GeometryDrawing>();
        private Drawing getDirtsDrawing()
        {
            firtGeoDrawGroup = new DrawingGroup();
            for (int x = 0; x < model.Width; x++)
            {
                for (int y = 0; y < model.Height; y++)
                {
                    if (model.DirtMatrix[x, y]!=null && model.Camera.isInStage(new Point(x,y)))
                    {
                        Brush brush;
                        brush = assetBrushes["_" + (93 + (model.TextureSet * 4) + (model.DirtMatrix[x, y].Variant - 1)).ToString("000")];
                        var c = new Coord() { x = x * TileSize, y = y * TileSize };
                        if (!dirtGeoDraw.ContainsKey(c))
                        {
                            dirtGeoDraw[c] = new GeometryDrawing(brush, null, new RectangleGeometry(new Rect(c.x, c.y, TileSize, TileSize)));
                        }
 
                        firtGeoDrawGroup.Children.Add(dirtGeoDraw[c]);
                        
                        
                    }
                }
            }

            
            
           
            return firtGeoDrawGroup;
        }
        private Drawing getWallsDrawing()
        {
            GeometryGroup wallgeo = new GeometryGroup();
            for (int x = 0; x < model.WallMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < model.WallMatrix.GetLength(1); y++)
                {
                    if (model.WallMatrix[x, y])
                    {
                        Geometry box = new RectangleGeometry(new Rect(x * TileSize, y * TileSize, TileSize, TileSize));
                        wallgeo.Children.Add(box);
                    }
                }
            }
            var ret = new GeometryDrawing(assetBrushes[nameof(Properties.Resources.Wall)], null, wallgeo);
            return ret;
        }

        Drawing titaniums;
        private Drawing createTitaniumDrawing()
        {
            GeometryGroup tianiumGeo = new GeometryGroup();
            for (int x = 0; x < model.TitaniumMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < model.TitaniumMatrix.GetLength(1); y++)
                {
                    if (model.TitaniumMatrix[x, y])
                    {
                        Geometry box = new RectangleGeometry(new Rect(x * TileSize, y * TileSize, TileSize, TileSize));
                        tianiumGeo.Children.Add(box);
                    }
                }
            }
            return new GeometryDrawing(assetBrushes["TitanWall"+model.TextureSet], null, tianiumGeo);
        }

        TranslateTransform rockfordTranslate;
        VisualBrush rockfordVisualBrush;
        Geometry rockfordGeo;
        private Drawing getRockfordDrawing()
        {
            if(rockfordGeo==null)
                rockfordGeo = new RectangleGeometry(new Rect(0, 0, TileSize, TileSize));
            if(rockfordTranslate==null)
                rockfordTranslate = new TranslateTransform(model.Rockford.TilePosition.X * TileSize, model.Rockford.TilePosition.Y*TileSize);

            if (!model.Rockford.TileOldPosition.Equals(model.Rockford.TilePosition))
            {
                Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, MOVETIME));
                DoubleAnimation animX = new DoubleAnimation(model.Rockford.TileOldPosition.X * TileSize, model.Rockford.TilePosition.X * TileSize, duration);
                DoubleAnimation animY = new DoubleAnimation(model.Rockford.TileOldPosition.Y * TileSize, model.Rockford.TilePosition.Y * TileSize, duration);
                animX.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power=1.2 };
                animY.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power=1.2 };

                rockfordTranslate.BeginAnimation(TranslateTransform.XProperty, animX);
                rockfordTranslate.BeginAnimation(TranslateTransform.YProperty, animY);

            }

            rockfordGeo.Transform = rockfordTranslate;

            switch (model.Rockford.Direaction)
            {
                case State.Left:
                    rockfordVisualBrush = animatedVisualBrushes[nameof(Properties.Resources.RockfordLeft)];
                    break;
                case State.Right:
                    rockfordVisualBrush = animatedVisualBrushes[nameof(Properties.Resources.RockfordRight)];
                    break;
                case State.Stand:
                    rockfordVisualBrush = animatedVisualBrushes[nameof(Properties.Resources.RockfordTap)];
                    break;
                case State.Birth:
                    rockfordVisualBrush = animatedVisualBrushes[nameof(Properties.Resources.RockfordBirth)];
                    break;
                default :
                    rockfordVisualBrush = animatedVisualBrushes[model.Rockford.isLastMoveWasRight ? nameof(Properties.Resources.RockfordRight) : nameof(Properties.Resources.RockfordLeft)];
                    break;
            }
            var brush = rockfordVisualBrush;
            brush.TileMode = TileMode.None;
            brush.Viewport = new Rect(0, 0, TileSize, TileSize);
            brush.ViewportUnits = BrushMappingMode.Absolute;
            brush.Transform = rockfordTranslate;
            model.Rockford.TileOldPosition = model.Rockford.TilePosition;
            return new GeometryDrawing(brush, null, rockfordGeo);
        }

        Dictionary<Logic.Blocks.Diamond, VisualBrush> visualBrushCache = new Dictionary<Logic.Blocks.Diamond, VisualBrush>();
        private Drawing getDiamondsDrawing()
        {
            Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, MOVETIME));
            var ggg = new DrawingGroup();
            foreach (var d in model.Diamonds)
            {
                if (d != null)
                {
                    TranslateTransform diamondTranslate = new TranslateTransform(d.TilePosition.X*TileSize,d.TilePosition.Y*TileSize);
                    if (!d.TilePosition.Equals(d.TileOldPosition) &&  model.Camera.isInStage(d.TilePosition))
                    {
                       
                        DoubleAnimation animX = new DoubleAnimation(d.TileOldPosition.X * TileSize, d.TilePosition.X * TileSize, duration);
                        DoubleAnimation animY = new DoubleAnimation(d.TileOldPosition.Y * TileSize, d.TilePosition.Y * TileSize, duration);
                        animX.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.2 };
                        animY.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.2 };

                        diamondTranslate.BeginAnimation(TranslateTransform.XProperty, animX);
                        diamondTranslate.BeginAnimation(TranslateTransform.YProperty, animY);
                        
                    }
                    

                    Geometry dia = new RectangleGeometry(new Rect(0,0, TileSize, TileSize));
                    dia.Transform = diamondTranslate;
                    d.TileOldPosition = d.TilePosition;
                    if (model.Camera.isInStage(d.TilePosition))
                    {
                        if (visualBrushCache.ContainsKey(d))
                        {
                            var brush = visualBrushCache[d];
                            brush.TileMode = TileMode.None;
                            brush.Viewport = new Rect(0, 0, TileSize, TileSize);
                            brush.ViewportUnits = BrushMappingMode.Absolute;
                            brush.Transform = diamondTranslate;
                            ggg.Children.Add(new GeometryDrawing(brush, null, dia));
                        }
                        else
                        {
                            
                            var brush = animatedVisualBrushes["Diamond" + model.TextureSet].Clone();
                            RenderOptions.SetCachingHint(brush, CachingHint.Cache);
                            visualBrushCache[d] = brush;
                            brush.TileMode = TileMode.None;
                            brush.Viewport = new Rect(0, 0, TileSize, TileSize);
                            brush.ViewportUnits = BrushMappingMode.Absolute;
                            brush.Transform = diamondTranslate;
                            ggg.Children.Add(new GeometryDrawing(brush, null, dia));
                        }
                    }
                }
            }
           
            return ggg;
            
        }

        DrawingGroup mainDrGr = new DrawingGroup();
        TranslateTransform cropTrans;
        AnimationClock clockX;
        AnimationClock clockY;
        public Drawing BuildDrawing()
        {
            double toOffsetX = (1 - model.Camera.Center.X * TileSize) + TileSize * model.Camera.AngleWidthTile / 2;
            double toOffsetY = (1 - model.Camera.Center.Y * TileSize) + TileSize * model.Camera.AngleHeightTile / 2;
            double fromOffsetX = (1 - model.Camera.CenterOld.X * TileSize) + TileSize * model.Camera.AngleWidthTile / 2;
            double fromOffsetY = (1 - model.Camera.CenterOld.Y * TileSize) + TileSize * model.Camera.AngleHeightTile / 2;
            if (cropTrans == null)
                cropTrans = new TranslateTransform(toOffsetX, toOffsetY);
            RenderOptions.SetBitmapScalingMode(mainDrGr, BitmapScalingMode.NearestNeighbor);

            
            if (!model.Camera.Center.Equals(model.Camera.CenterOld))
            {
                Duration durX = new Duration(TimeSpan.FromMilliseconds(MOVETIME*5));
                Duration durY = new Duration(TimeSpan.FromMilliseconds(MOVETIME*2));
                DoubleAnimation cropAnimX = new DoubleAnimation(fromOffsetX, toOffsetX , durX);
                DoubleAnimation cropAnimY = new DoubleAnimation(fromOffsetY, toOffsetY , durY);
                cropAnimX.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.3 };
                //cropAnimY.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.2 };
                clockX = cropAnimX.CreateClock();
                clockY = cropAnimY.CreateClock();
                cropTrans.ApplyAnimationClock(TranslateTransform.XProperty, clockX);
                cropTrans.ApplyAnimationClock(TranslateTransform.YProperty, clockY);
                
            }
            mainDrGr.Transform = cropTrans;
            mainDrGr.Children = new DrawingCollection();
            mainDrGr.Children.Add(getBackGroundDrawing());
            mainDrGr.Children.Add(titaniums);
            mainDrGr.Children.Add(getWallsDrawing());
            mainDrGr.Children.Add(getDirtsDrawing());
            mainDrGr.Children.Add(getFirefliesDrawing());
            mainDrGr.Children.Add(getDiamondsDrawing());
           
            
            mainDrGr.Children.Add(getExitDrawing());
            mainDrGr.Children.Add(getBouldersDrawing());
            if (!model.GameOver)
            {
                mainDrGr.Children.Add(getRockfordDrawing());
            }

            mainDrGr.Children.Add(getExplodeDrawing(animatedVisualBrushes[nameof(Properties.Resources.Explode)]));

            return mainDrGr; //TODO minimalize new calls
        }

        bool exitCache = false;
        Drawing exit;
        private Drawing getExitDrawing()
        {
            if (model.Exit.IsOpen == exitCache && exit!=null)
            {
                return exit;
            }
            if (!model.Exit.IsOpen)
            {
                exitCache = model.Exit.IsOpen;

                
                exit = new GeometryDrawing(assetBrushes["ExitClose" + model.TextureSet], null, new RectangleGeometry(new Rect(model.Exit.TilePosition.X * TileSize, model.Exit.TilePosition.Y * TileSize, TileSize, TileSize)));
            }
            else 
            {
                exitCache = model.Exit.IsOpen;
                exit = new GeometryDrawing(assetBrushes["ExitOpen" + model.TextureSet], null, new RectangleGeometry(new Rect(model.Exit.TilePosition.X * TileSize, model.Exit.TilePosition.Y * TileSize, TileSize, TileSize)));
            }
            return exit;
           
        }

        Drawing bg;
        private Drawing getBackGroundDrawing()
        {
            if (model.RequireDiamonds <= model.CollectedDiamonds && model.WhiteBgCount > 0)
            {
                bg = new GeometryDrawing(Brushes.White, null, new RectangleGeometry(new Rect(0, 0, model.WallMatrix.GetLength(0) * TileSize, model.WallMatrix.GetLength(1) * TileSize)));
                model.WhiteBgCount--;
                if (model.WhiteBgCount == 0)
                {
                    bg = null;
                }  
            }
            if (bg == null)
            {
                bg = new GeometryDrawing(Brushes.Black, null, new RectangleGeometry(new Rect(0, 0, model.WallMatrix.GetLength(0) * TileSize, model.WallMatrix.GetLength(1) * TileSize)));
            }


            return bg;
        }

        GeometryGroup explodeGG = new GeometryGroup();
        private Drawing getExplodeDrawing(VisualBrush visualBrush)
        {
            visualBrush.TileMode = TileMode.Tile;
            visualBrush.Viewport = new Rect(0, 0, TileSize, TileSize);
            visualBrush.ViewportUnits = BrushMappingMode.Absolute;

            List<Geometry> marked = new List<Geometry>();
            foreach (var item in explodeGG.Children)
            {
                int y = (int)((item as RectangleGeometry).Rect.Y/TileSize);
                int x = (int)((item as RectangleGeometry).Rect.X/TileSize);
                if (model.Explosion[x,y] == 0)
                {
                    marked.Add(item);
                } 
            }
            foreach (var item in marked)
            {
                explodeGG.Children.Remove(item);
            }

            for (int x = 0; x < model.Width; x++)
            {
                for (int y = 0; y < model.Height; y++)
                {
                    if(model.Explosion[x,y]>0)
                        explodeGG.Children.Add(new RectangleGeometry(new Rect(x * TileSize, y * TileSize, TileSize, TileSize)));
                    else if (model.Explosion[x, y] == 0)
                    {
                    
                    }
                }
            }
            return new GeometryDrawing(visualBrush, null, explodeGG);
        }

        Dictionary<Logic.Firefly, VisualBrush> visualBrushFirefliesCache = new Dictionary<Logic.Firefly, VisualBrush>();
        private Drawing getFirefliesDrawing()
        {
            Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, MOVETIME));
            var ggg = new DrawingGroup();
            foreach (var d in model.Fireflies)
            {
                if (d != null)
                {
                    TranslateTransform fireFlyTrans = new TranslateTransform(d.TilePosition.X * TileSize, d.TilePosition.Y * TileSize);
                    if (!d.TilePosition.Equals(d.TileOldPosition) && model.Camera.isInStage(d.TilePosition))
                    {

                        DoubleAnimation animX = new DoubleAnimation(d.TileOldPosition.X * TileSize, d.TilePosition.X * TileSize, duration);
                        DoubleAnimation animY = new DoubleAnimation(d.TileOldPosition.Y * TileSize, d.TilePosition.Y * TileSize, duration);
                        animX.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.2 };
                        animY.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 1.2 };

                        fireFlyTrans.BeginAnimation(TranslateTransform.XProperty, animX);
                        fireFlyTrans.BeginAnimation(TranslateTransform.YProperty, animY);

                    }


                    Geometry fireflyGeo = new RectangleGeometry(new Rect(0, 0, TileSize, TileSize));
                    fireflyGeo.Transform = fireFlyTrans;
                    d.TileOldPosition = d.TilePosition;
                    if (model.Camera.isInStage(d.TilePosition))
                    {
                        VisualBrush brush;
                        if (visualBrushFirefliesCache.ContainsKey(d))
                        {
                            brush = visualBrushFirefliesCache[d];
                            
                            ggg.Children.Add(new GeometryDrawing(brush, null, fireflyGeo));
                        }
                        else
                        {

                            brush = animatedVisualBrushes["Firefly" + model.TextureSet].Clone();
                            RenderOptions.SetCachingHint(brush, CachingHint.Cache);
                            visualBrushFirefliesCache[d] = brush;
                            
                            ggg.Children.Add(new GeometryDrawing(brush, null, fireflyGeo));
                        }
                        brush.TileMode = TileMode.None;
                        brush.Viewport = new Rect(0, 0, TileSize, TileSize);
                        brush.ViewportUnits = BrushMappingMode.Absolute;
                        brush.Transform = fireFlyTrans;
                    }
                }
            }
            return ggg;
        }
    }
}
