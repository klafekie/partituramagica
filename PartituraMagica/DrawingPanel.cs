using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LearnWPF.Effects;

namespace PartituraMagica
{
    internal class DrawingPanel : IDrawingPanel
    {
        private static readonly Brush selectedColor = Brushes.LightGreen;
        private static readonly Brush notSelectedColor = Brushes.DarkSlateBlue;

        private string currentImagePath;
        private Dictionary<int, Clipping> id2clipping;
        private readonly Dictionary<string, GreyImage> imageCache = new Dictionary<string, GreyImage>();
        private readonly Canvas pageCanvas;

        public DrawingPanel(Canvas pageCanvas)
        {
            this.pageCanvas = pageCanvas;
            Clear();
        }

        public void AddRectangle(int id, Rect coordinates)
        {
            Clipping c = new Clipping();

            c.r.StrokeThickness = Page.ClickTolerance;
            c.r.Stroke = notSelectedColor;

            SetCoords(c, coordinates);

            pageCanvas.Children.Add(c.r);
            pageCanvas.Children.Add(c.t);
            id2clipping[id] = c;
        }

        public void AddOrUpdateRectangle(int id, Rect coordinates)
        {
            Clipping c;
            if (id2clipping.TryGetValue(id, out c))
            {
                SetCoords(c, coordinates);
            }
            else
            {
                AddRectangle(id, coordinates);
            }
        }

        public void RemoveRectangle(int id)
        {
            Clipping i = id2clipping[id];
            pageCanvas.Children.Remove(i.r);
            pageCanvas.Children.Remove(i.t);
            id2clipping.Remove(id);
        }

        public void SelectRectangle(int id)
        {
            //we could optimize by just changing zindex of last selected rect
            //but this requires to store last selected id.
            foreach (Clipping c in id2clipping.Values)
            {
                Panel.SetZIndex(c.r, 0);
                Panel.SetZIndex(c.t, 2);
            }
            Clipping selectedClipping = id2clipping[id];
            selectedClipping.r.Stroke = selectedColor;
            selectedClipping.t.Foreground = selectedColor;
            Panel.SetZIndex(selectedClipping.r, 1); //bring to top			
            Panel.SetZIndex(selectedClipping.t, 3); //text on top of rectangle
        }

        public void DeselectRectangle(int id)
        {
            Clipping selectedClipping = id2clipping[id];
            selectedClipping.r.Stroke = notSelectedColor;
            selectedClipping.t.Foreground = notSelectedColor;
        }

        public void Clear()
        {
            pageCanvas.Children.Clear();
            id2clipping = new Dictionary<int, Clipping>();
        }

        public Point DrawImage(string path)
        {
            GreyImage bm = LoadImage(path);

            Image image = new Image();
            image.Stretch = Stretch.None;
            image.Height = bm.RawImage.Height;
            image.Width = bm.RawImage.Width;
            image.Source = bm.RawImage;
            image.Effect = new NegativeEffect();

            pageCanvas.Width = bm.RawImage.Width;
            pageCanvas.Height = bm.RawImage.Height;
            pageCanvas.Children.Add(image);
            currentImagePath = path;
            return new Point(image.Width, image.Height);
        }

        public void LabelRectangle(int id, string label)
        {
            Clipping i = id2clipping[id];
            if (label == null || label.Length == 0)
            {
                //hide i.t
                i.t.Visibility = Visibility.Collapsed;
            }
            else
            {
                i.t.Text = label;
                i.t.Visibility = Visibility.Visible;
            }
        }

        public IGreyImage CurrentImage
        {
            get { return GetImage(currentImagePath); }
        }

        public IGreyImage GetImage(string path)
        {
            return LoadImage(path);
        }

        private static void SetCoords(Clipping c, Rect coordinates)
        {
            double thickness = c.r.StrokeThickness;
            double left = coordinates.X - thickness/2.0;
            double top = coordinates.Y - thickness/2.0;

            c.r.Width = coordinates.Width + thickness;
            c.r.Height = coordinates.Height + thickness;
            Canvas.SetLeft(c.r, left);
            Canvas.SetTop(c.r, top);
            Canvas.SetLeft(c.t, left + Page.ClickTolerance);
            Canvas.SetTop(c.t, top + Page.ClickTolerance);
        }

        private GreyImage LoadImage(string path)
        {
            if (!imageCache.ContainsKey(path))
            {
                BitmapImage bm = new BitmapImage();
                // BitmapImage.UriSource must be in a BeginInit/EndInit block
                bm.BeginInit();
                bm.UriSource = new Uri(path);
                bm.EndInit();
                imageCache[path] = new GreyImage(path, bm);
            }
            return imageCache[path];
        }

        private class Clipping
        {
            public readonly Rectangle r;
            public readonly TextBlock t;

            public Clipping()
            {
                r = new Rectangle();
                t = new TextBlock();
                t.FontSize = 24;
            }
        }
    }
}