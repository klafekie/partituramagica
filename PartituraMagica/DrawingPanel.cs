#region usings

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LearnWPF.Effects;
using PartituraMagica.Domain;

#endregion

namespace PartituraMagica
{
	internal class DrawingPanel : IDrawingPanel
	{
		private class Clipping
		{
			public readonly Rectangle R;
			public readonly TextBlock T;

			public Clipping()
			{
				R = new Rectangle();
				T = new TextBlock();
				T.FontSize = 24;
			}
		}

		private static readonly Brush SelectedColor = Brushes.LightGreen;
		private static readonly Brush NotSelectedColor = Brushes.DarkSlateBlue;
		private readonly Dictionary<string, GreyImage> _imageCache = new Dictionary<string, GreyImage>();
		private readonly Canvas _pageCanvas;

		private string _currentImagePath;
		private Dictionary<int, Clipping> _id2Clipping;

		public DrawingPanel(Canvas pageCanvas)
		{
			_pageCanvas = pageCanvas;
			Clear();
		}

		public void AddRectangle(int id, Rect coordinates)
		{
			Clipping c = new Clipping();

			c.R.StrokeThickness = Page.ClickTolerance;
			c.R.Stroke = NotSelectedColor;

			SetCoords(c, coordinates);

			_pageCanvas.Children.Add(c.R);
			_pageCanvas.Children.Add(c.T);
			_id2Clipping[id] = c;
		}

		public void AddOrUpdateRectangle(int id, Rect coordinates)
		{
			Clipping c;
			if (_id2Clipping.TryGetValue(id, out c))
				SetCoords(c, coordinates);
			else
				AddRectangle(id, coordinates);
		}

		public void RemoveRectangle(int id)
		{
			Clipping i = _id2Clipping[id];
			_pageCanvas.Children.Remove(i.R);
			_pageCanvas.Children.Remove(i.T);
			_id2Clipping.Remove(id);
		}

		public void SelectRectangle(int id)
		{
			//we could optimize by just changing zindex of last selected rect
			//but this requires to store last selected id.
			foreach (Clipping c in _id2Clipping.Values)
			{
				Panel.SetZIndex(c.R, 0);
				Panel.SetZIndex(c.T, 2);
			}

			Clipping selectedClipping = _id2Clipping[id];
			selectedClipping.R.Stroke = SelectedColor;
			selectedClipping.T.Foreground = SelectedColor;
			Panel.SetZIndex(selectedClipping.R, 1); //bring to top			
			Panel.SetZIndex(selectedClipping.T, 3); //text on top of rectangle
		}

		public void DeselectRectangle(int id)
		{
			Clipping selectedClipping = _id2Clipping[id];
			selectedClipping.R.Stroke = NotSelectedColor;
			selectedClipping.T.Foreground = NotSelectedColor;
		}

		public void Clear()
		{
			_pageCanvas.Children.Clear();
			_id2Clipping = new Dictionary<int, Clipping>();
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

			_pageCanvas.Width = bm.RawImage.Width;
			_pageCanvas.Height = bm.RawImage.Height;
			_pageCanvas.Children.Add(image);
			_currentImagePath = path;
			return new Point(image.Width, image.Height);
		}

		public void LabelRectangle(int id, string label)
		{
			Clipping i = _id2Clipping[id];
			if (label == null || label.Length == 0)
			{
				//hide i.t
				i.T.Visibility = Visibility.Collapsed;
			}
			else
			{
				i.T.Text = label;
				i.T.Visibility = Visibility.Visible;
			}
		}

		public IGreyImage CurrentImage => GetImage(_currentImagePath);

		public IGreyImage GetImage(string path)
		{
			return LoadImage(path);
		}

		private static void SetCoords(Clipping c, Rect coordinates)
		{
			double thickness = c.R.StrokeThickness;
			double left = coordinates.X - thickness / 2.0;
			double top = coordinates.Y - thickness / 2.0;

			c.R.Width = coordinates.Width + thickness;
			c.R.Height = coordinates.Height + thickness;
			Canvas.SetLeft(c.R, left);
			Canvas.SetTop(c.R, top);
			Canvas.SetLeft(c.T, left + Page.ClickTolerance);
			Canvas.SetTop(c.T, top + Page.ClickTolerance);
		}

		private GreyImage LoadImage(string path)
		{
			if (!_imageCache.ContainsKey(path))
			{
				BitmapImage bm = new BitmapImage();
				// BitmapImage.UriSource must be in a BeginInit/EndInit block
				bm.BeginInit();
				bm.UriSource = new Uri(path);
				bm.EndInit();
				_imageCache[path] = new GreyImage(path, bm);
			}

			return _imageCache[path];
		}
	}
}