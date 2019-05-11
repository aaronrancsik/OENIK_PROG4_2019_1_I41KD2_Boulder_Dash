﻿using System;
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

            dirtBrush = GetDirtBrush();
            rockBrush = GetRockBrush();
            wallBrush = GetWallBrush();

            exit = GetExit();
            bg = GetBackground();
            walls = GetWalls();
            titaniums = GetTitaniums();
            playerGeo = new RectangleGeometry(new Rect(0, 0, TileSize, TileSize));
        }

        Brush GetBrush(ImageSource image)
        {

            ImageBrush i = new ImageBrush();
            ImageBrush ib = new ImageBrush(image);

            ib.TileMode = TileMode.Tile;
            //ib.Viewbox  //Only if multiple textures, atlasnak kell
            ib.Viewport = new Rect(0, 0, TileSize, TileSize);
            ib.ViewportUnits = BrushMappingMode.Absolute; //view box;
            return ib;
        }
        private Brush GetRockBrush()
        {
            var p = Properties.Resources.Boulder1;
            return GetBrush(Bitmap2BitmapImageSource(p));
        }
        private Brush GetDirtBrush()
        {
            var p = Properties.Resources._105;
            return GetBrush(Bitmap2BitmapImageSource(p));
        }
        private Brush GetWallBrush()
        {
            var p = Properties.Resources.Wall;
            return GetBrush(Bitmap2BitmapImageSource(p));
        }
        private Brush GetExitBrush()
        {
            var p = Properties.Resources.ExitClose1;
            return GetBrush(Bitmap2BitmapImageSource(p));
        }
        private Brush getTitanimBrush()
        {
            var p = Properties.Resources.TitanWall1;
            return GetBrush(Bitmap2BitmapImageSource(p));
        }

        private ImageSource Bitmap2BitmapImageSource(System.Drawing.Bitmap bitmap)
        {
            System.Windows.Media.Imaging.BitmapSource i =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                           bitmap.GetHbitmap(),
                           IntPtr.Zero,
                           Int32Rect.Empty,
                           System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions()
                );
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

        DrawingGroup dirtGeoDraw = new DrawingGroup();
        private Drawing getDirtsDrawing()
        {
            dirtGeoDraw = new DrawingGroup();
            for (int x = 0; x < model.DirtMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < model.DirtMatrix.GetLength(1); y++)
                {
                    if (model.DirtMatrix[x, y]!=null)
                    {
                        Brush a;
                        a = assetBrushes["_" + (93 + (model.TextureSet * 4) + (model.DirtMatrix[x, y].Variant - 1)).ToString("000")];
                        dirtGeoDraw.Children.Add(new GeometryDrawing(a, null, new RectangleGeometry(new Rect(x * TileSize, y * TileSize, TileSize, TileSize))));
                    }
                }
            }

            
            
           
            return dirtGeoDraw;
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
            var ret = new GeometryDrawing(wallBrush, null, wallgeo);
            
            return ret;
        }
        private Drawing GetExit()
        {
            Geometry g = new RectangleGeometry(new Rect(model.ExitPistition.X * TileSize, model.ExitPistition.Y * TileSize, TileSize, TileSize));
            return new GeometryDrawing(GetExitBrush(), null, g);
        }
        private Drawing GetTitaniums()
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
            return new GeometryDrawing(getTitanimBrush(), null, tianiumGeo);
        }
        private Drawing GetBackground()
        {
            Geometry g = new RectangleGeometry(new Rect(0, 0, model.WallMatrix.GetLength(0)*TileSize, model.WallMatrix.GetLength(1) * TileSize));

            return new GeometryDrawing(blackBrush, null, g);
        }

        TranslateTransform myTranslate = new TranslateTransform();
        private Drawing GetPlayer(VisualBrush rockfordvb)
        {

            if (!model.Player.TileOldPosition.Equals(model.Player.TilePosition))
            {
                Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, MOVETIME));
                DoubleAnimation animX = new DoubleAnimation(model.Player.TileOldPosition.X * TileSize, model.Player.TilePosition.X * TileSize, duration);
                DoubleAnimation animY = new DoubleAnimation(model.Player.TileOldPosition.Y * TileSize, model.Player.TilePosition.Y * TileSize, duration);
                animX.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseInOut };
                animY.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseInOut };

                myTranslate.BeginAnimation(TranslateTransform.XProperty, animX);
                myTranslate.BeginAnimation(TranslateTransform.YProperty, animY);

            }
            playerGeo.Transform = myTranslate;
            var brush = rockfordvb;
            brush.TileMode = TileMode.None;
            brush.Viewport = new Rect(0, 0, TileSize, TileSize);
            brush.ViewportUnits = BrushMappingMode.Absolute;
            brush.Transform = myTranslate;
            model.Player.TileOldPosition = model.Player.TilePosition;
            return new GeometryDrawing(brush, null, playerGeo);
        }
        private Drawing GetDiamonds(VisualBrush diamondvv)
        {
            GeometryGroup gg = new GeometryGroup();
            diamondvv.TileMode = TileMode.Tile;
            diamondvv.Viewport = new Rect(0, 0, TileSize, TileSize);
            diamondvv.ViewportUnits = BrushMappingMode.Absolute;

            foreach (var d in model.Diamonds)
            {
                Geometry dia = new RectangleGeometry(new Rect(d.TilePosition.X * TileSize, d.TilePosition.Y * TileSize, TileSize, TileSize));
                gg.Children.Add(dia);
            }
            return new GeometryDrawing(diamondvv, null, gg);
        }
        public Drawing BuildDrawing(VisualBrush diamondvb, VisualBrush rockfordvb)
        {
            DrawingGroup dg = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(dg, BitmapScalingMode.NearestNeighbor);

            Duration dur = new Duration(TimeSpan.FromMilliseconds(MOVETIME));
            DoubleAnimation animX = new DoubleAnimation(-(int)model.Camera.CenterOld.X / 2 * TileSize, -(int)model.Camera.Center.X / 2 * TileSize, dur);
            DoubleAnimation animY = new DoubleAnimation(-(int)model.Camera.CenterOld.Y / 2 * TileSize, -(int)model.Camera.Center.Y / 2 * TileSize, dur);
            var trans = new TranslateTransform();
            trans.BeginAnimation(TranslateTransform.XProperty, animX);
            trans.BeginAnimation(TranslateTransform.YProperty, animY);

            dg.Children.Add(bg);
            dg.Children.Add(titaniums);
            dg.Children.Add(walls);
            dg.Children.Add(GetDirts());
            dg.Children.Add(GetDiamonds(diamondvb));
            dg.Children.Add(GetExit());
            dg.Children.Add(GetBoulders());
            dg.Children.Add(GetPlayer(rockfordvb));
            //dg.ClipGeometry =new RectangleGeometry(new Rect((int)model.Camera.Center.X / 2*TileSize, (int)model.Camera.Center.Y / 2*TileSize, (model.Camera.Width+1)*(TileSize), model.Camera.Height*TileSize));
            dg.Transform = trans;

            return dg; //TODO minimalize new calls
        }


    }
}
