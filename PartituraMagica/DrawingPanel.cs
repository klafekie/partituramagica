using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace PartituraMagica
{
    class DrawingPanel : IDrawingPanel
	{

		class Clipping
		{
			public Clipping()
			{
				this.r = new Rectangle();
				this.t = new TextBlock();
				this.t.FontSize = 24;
			}
			public Rectangle r;
			public TextBlock t;
		}

		private string currentImagePath;
		private Dictionary<string, GreyImage> imageCache = new Dictionary<string, GreyImage>();
        private static Brush selectedColor = Brushes.LightGreen;
		private static Brush notSelectedColor = Brushes.DarkSlateBlue;
		private Canvas pageCanvas;
		private Dictionary<int, Clipping> id2clipping;

		public DrawingPanel(Canvas pageCanvas)
		{
			this.pageCanvas = pageCanvas;
			Clear();
		}

		public void AddRectangle(int id, PartituraMagica.Rect coordinates)
		{
			Clipping c = new Clipping();

			c.r.StrokeThickness = Page.ClickTolerance;
			c.r.Stroke = notSelectedColor;

			SetCoords(c, coordinates);

			pageCanvas.Children.Add(c.r);
			pageCanvas.Children.Add(c.t);
			id2clipping[id] = c;
		}

		private static void SetCoords(Clipping c, PartituraMagica.Rect coordinates)
		{
			double thickness = c.r.StrokeThickness;
			double left = coordinates.X - thickness / 2.0;
			double top = coordinates.Y - thickness / 2.0;

			c.r.Width = coordinates.Width + thickness;
			c.r.Height = coordinates.Height + thickness;
			Canvas.SetLeft(c.r, left);
			Canvas.SetTop(c.r, top);
			Canvas.SetLeft(c.t, left + Page.ClickTolerance);
			Canvas.SetTop(c.t, top + Page.ClickTolerance);
		}

		public void AddOrUpdateRectangle(int id, PartituraMagica.Rect coordinates)
		{
			Clipping c;			
			if (this.id2clipping.TryGetValue(id, out c)) {
				SetCoords(c, coordinates);
			} else {
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
			foreach (Clipping c in this.id2clipping.Values) {
				Canvas.SetZIndex(c.r, 0);
				Canvas.SetZIndex(c.t, 2);
			}
			Clipping selectedClipping = this.id2clipping[id];
			selectedClipping.r.Stroke = selectedColor;
			selectedClipping.t.Foreground = selectedColor;
			Canvas.SetZIndex(selectedClipping.r, 1);//bring to top			
			Canvas.SetZIndex(selectedClipping.t, 3);//text on top of rectangle
		}

		public void DeselectRectangle(int id)
		{
			Clipping selectedClipping = this.id2clipping[id];
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
			image.Effect = new LearnWPF.Effects.NegativeEffect();

			pageCanvas.Width = bm.RawImage.Width;
			pageCanvas.Height = bm.RawImage.Height;
			pageCanvas.Children.Add(image);
			this.currentImagePath = path;			
			return new Point(image.Width, image.Height);
		}

		public void LabelRectangle(int id, string label)
		{
			Clipping i = id2clipping[id];
			if (label == null || label.Length == 0) {
				//hide i.t
				i.t.Visibility = Visibility.Collapsed;
			} else {
				i.t.Text = label;				
				i.t.Visibility = Visibility.Visible;
			}
		}

		public IGreyImage CurrentImage
		{
			get
			{
				return GetImage(this.currentImagePath);
			}
		}

		private GreyImage LoadImage(string path) {
			if (!this.imageCache.ContainsKey(path)) {				
				BitmapImage bm = new BitmapImage();
				// BitmapImage.UriSource must be in a BeginInit/EndInit block
				bm.BeginInit();
				bm.UriSource = new Uri(path);
				bm.EndInit();
				this.imageCache[path] = new GreyImage(path, bm);
			}
			return this.imageCache[path];			
		}

		public IGreyImage GetImage(string path) {
			return LoadImage(path);
		}
	}
}
