#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.PageEditing
{
	/**
	 * Calculates the best position and size for the boxes on the page based on the
	 * systems found by systemFinder
	 */
	public class BoxLayouter
	{
		private readonly IGreyImage _image;

		public double DefaultBoxHeight => _image.Height * 0.1D;

		public double MaxBoxHeight => _image.Height * 0.12D;
		public double MinBoxHeight => _image.Height * 0.08D;

		[UsedImplicitly]
		public BoxLayouter(IDrawingPanel panel)
		{
			_image = panel.CurrentImage;
		}

		public BoxLayouter(IGreyImage image)
		{
			_image = image;
		}

		/** 
     * returns a list of rectangles that enclose each line
     * result is in screen coordinates
     */
		public IList<Rect> Execute(IList<Rect> systems)
		{
			IEnumerable<Rect> boxes = GetBoxes(systems);

			double xFactor = 96D / _image.DpiX;
			double yFactor = 96D / _image.DpiY;

			return boxes.Select(s => s.Scale(xFactor, yFactor)).ToList();
		}

		/* returns boxes in image coordinates */
		public IList<Rect> GetBoxes(IList<Rect> systems)
		{
			if (systems.Count == 0) return new List<Rect>();
			var initialized = systems
				.Select(s => new Rect(s,
					s.Height / 2D,
					new Rect(0, 0, _image.Width, _image.Height)))
				.ToArray();

			var nabourHood = new List<(Rect Previous, Rect Current, Rect Next)>();
			for (int i = 0; i < initialized.Length; i++)
			{
				Rect previous = null;
				Rect next = null;
				if (i > 0) previous = initialized[i - 1];

				if (i < initialized.Length - 1) next = initialized[i + 1];
				nabourHood.Add((Previous: previous, Current: initialized[i], Next: next));
			}

			var result = new List<Rect>();
			foreach (var tuple in nabourHood)
			{
				double spaceAbove = DefaultBoxHeight / 2D;
				if (tuple.Previous != null)
					spaceAbove = Math.Min(
						DefaultBoxHeight / 2D,
						(tuple.Current.Top - tuple.Previous.Bottom) / 2D - 2D * Page.ClickTolerance);

				double spaceBelow = DefaultBoxHeight / 2D;
				if (tuple.Next != null)
					spaceBelow = Math.Min(
						DefaultBoxHeight / 2D,
						(tuple.Next.Top - tuple.Current.Bottom) / 2D - 2D * Page.ClickTolerance);

				//Trace.WriteLine("tuple.Current: " + tuple.Current);
				Rect addedAbove = tuple.Current.AddAbove(spaceAbove);
				//Trace.WriteLine("addedAbove: " + addedAbove);
				Rect addedBelow = addedAbove.AddBelow(spaceBelow);
				//Trace.WriteLine("addedBelow: " + addedBelow);
				result.Add(addedBelow);
			}

			return result;
		}
	}
}