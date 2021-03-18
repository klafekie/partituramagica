#region usings

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LearnWPF.Effects;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.App
{
	internal class DrawingPanel : IDrawingPanel
	{
		private class Clipping
		{
			private static readonly Brush SelectedColor = Brushes.LightGreen;
			private static readonly Brush NotSelectedColor = Brushes.SlateBlue;

			private readonly Line _line;
			private readonly Rectangle _rectangle;
			private readonly TextBlock _text;

			public Clipping()
			{
				_rectangle = new Rectangle {StrokeThickness = Page.ClickTolerance, Stroke = NotSelectedColor};
				_text = new TextBlock {FontSize = 24, Foreground = NotSelectedColor};
				_line = new Line
				{
					StrokeThickness = Page.ClickTolerance / 2D,
					Stroke = NotSelectedColor,
					StrokeDashArray =
						new DoubleCollection {2, 1, 3, 1}
				};
			}

			public void Select()
			{
				Panel.SetZIndex(_rectangle, 1);
				Panel.SetZIndex(_line, 1);
				Panel.SetZIndex(_text, 3);

				_rectangle.Stroke = SelectedColor;
				_line.Stroke = SelectedColor;
				_text.Foreground = SelectedColor;
			}

			public void Deselect()
			{
				Panel.SetZIndex(_rectangle, 0);
				Panel.SetZIndex(_line, 0);
				Panel.SetZIndex(_text, 2);
				_rectangle.Stroke = NotSelectedColor;
				_line.Stroke = NotSelectedColor;
				_text.Foreground = NotSelectedColor;
			}

			public void RemoveFrom(Canvas pageCanvas)
			{
				pageCanvas.Children.Remove(_rectangle);
				pageCanvas.Children.Remove(_line);
				pageCanvas.Children.Remove(_text);
			}

			public void SetCoordinates(Rect coordinates)
			{
				//Trace.WriteLine("DrawingPanel.Clipping:" + coordinates);
				double thickness = _rectangle.StrokeThickness;
				double left = coordinates.X - thickness / 2.0;
				double top = coordinates.Y - thickness / 2.0;
				double centerY = coordinates.CenterY;

				_rectangle.Width = coordinates.Width + thickness;
				_rectangle.Height = coordinates.Height + thickness;

				Canvas.SetLeft(_rectangle, left);
				Canvas.SetTop(_rectangle, top);
				Canvas.SetLeft(_line, left);
				Canvas.SetTop(_line, top);
				_line.X1 = 0;
				_line.Y1 = centerY;
				_line.X2 = coordinates.Width + thickness;
				_line.Y2 = centerY;
				Canvas.SetLeft(_text, left + Page.ClickTolerance);
				Canvas.SetTop(_text, top + Page.ClickTolerance);
			}

			public void Label(string label)
			{
				if (string.IsNullOrEmpty(label))
				{
					_text.Visibility = Visibility.Collapsed;
				}
				else
				{
					_text.Text = label;
					_text.Visibility = Visibility.Visible;
				}
			}

			public void AddTo(Canvas pageCanvas)
			{
				pageCanvas.Children.Add(_rectangle);
				pageCanvas.Children.Add(_line);
				pageCanvas.Children.Add(_text);
			}
		}

		private readonly Dictionary<string, GreyImage> _imageCache = new Dictionary<string, GreyImage>();
		private readonly Canvas _pageCanvas;

		private string _currentImagePath;
		private Dictionary<int, Clipping> _id2Clipping;

		public DrawingPanel(Canvas canvas)
		{
			_pageCanvas = canvas;
			Clear();
		}

		public void AddOrUpdateRectangle(int id, Rect coordinates)
		{
			if (_id2Clipping.TryGetValue(id, out Clipping c))
				c.SetCoordinates(coordinates);
			else
				AddRectangle(id, coordinates);
		}

		public void RemoveRectangle(int id)
		{
			_id2Clipping[id].RemoveFrom(_pageCanvas);
			_id2Clipping.Remove(id);
		}

		public void SelectRectangle(int id)
		{
			foreach (Clipping c in _id2Clipping.Values) c.Deselect();

			_id2Clipping[id].Select();
		}

		public void DeselectRectangle(int id)
		{
			_id2Clipping[id].Deselect();
		}

		public void Clear()
		{
			_pageCanvas.Children.Clear();
			_id2Clipping = new Dictionary<int, Clipping>();
		}

		public Point DrawImage(string path)
		{
			var bm = LoadImage(path);

			Image image = new Image
			{
				Stretch = Stretch.None,
				Height = bm.RawImage.Height,
				Width = bm.RawImage.Width,
				Source = bm.RawImage,
				Effect = new NegativeEffect()
			};

			_pageCanvas.Width = bm.RawImage.Width;
			_pageCanvas.Height = bm.RawImage.Height;
			_pageCanvas.Children.Add(image);
			_currentImagePath = path;
			return new Point(image.Width, image.Height);
		}

		public void LabelRectangle(int id, string label)
		{
			_id2Clipping[id].Label(label);
		}

		public IGreyImage CurrentImage => LoadImage(_currentImagePath);

		IGreyImage IImageLoader.LoadImage(string path)
		{
			return LoadImage(path);
		}

		public void AddRectangle(int id, Rect coordinates)
		{
			//Trace.WriteLine("DrawingPanel.AddRectangle:" + coordinates);

			Clipping clipping = new Clipping();
			clipping.SetCoordinates(coordinates);
			clipping.AddTo(_pageCanvas);
			_id2Clipping[id] = clipping;
		}

		public GreyImage LoadImage(string path)
		{
			if (path == null) return null;
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