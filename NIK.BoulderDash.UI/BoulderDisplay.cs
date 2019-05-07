using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using NIK.BoulderDash.Logic;

namespace NIK.BoulderDash.UI
{
    public class BoulderDisplay
    {
        int MOVETIME;
        GameModel model;
        double width;
        double height;
        public double TileSize { get; private set; }

        Brush dirtBrush;
        Brush rockBrush;
        Brush wallBrush;
        Brush blackBrush = Brushes.Black;

        Drawing bg;
        Drawing walls;
        Drawing titaniums;
        Drawing exit;

        Geometry playerGeo;

        public BoulderDisplay(GameModel model, double w, double h, int MOVETIME)
        {
            this.MOVETIME = MOVETIME;
            this.model = model;
            this.width = w;
            this.height = h;

            TileSize = Math.Min(
                w / model.Camera.Width,
                h / model.Camera.Height
            );


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

        private Drawing GetBoulders()
        {

            GeometryGroup gg = new GeometryGroup();
            foreach (var d in model.Boulders)
            {
                Geometry dia = new RectangleGeometry(new Rect(d.TilePosition.X * TileSize, d.TilePosition.Y * TileSize, TileSize, TileSize));
                gg.Children.Add(dia);
            }
            return new GeometryDrawing(rockBrush, null, gg);
        }
        private Drawing GetDirts()
        {
            GeometryGroup gg = new GeometryGroup();
            for (int x = 0; x < model.DirtMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < model.DirtMatrix.GetLength(1); y++)
                {
                    if (model.DirtMatrix[x, y]!=null)
                    {
                        Geometry box = new RectangleGeometry(new Rect(x * TileSize, y * TileSize, TileSize, TileSize));
                        gg.Children.Add(box);
                    }
                }
            }
            return new GeometryDrawing(dirtBrush, null, gg);
        }
        private Drawing GetWalls()
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
