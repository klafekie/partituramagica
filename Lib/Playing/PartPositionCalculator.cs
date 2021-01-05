#region usings

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PartituraMagica.Lib.Domain;

#endregion

namespace PartituraMagica.Lib.Playing
{
	/** 
     * Knows about the position of each line and the relation of each line to its containing part.
     */
	internal class PartPositionCalculator
	{
		private class LinePartInfo
		{
			public readonly int ClippingIndex; //index of the clipping relative to the corresponding part
			public readonly int PartIndex; //index of the parts, 0 corresponds to part "A"
			public readonly int PatternIndex; //the position inside the part pattern, 
			public readonly double XPos; //the absolute xPosition of the line on the view

			public LinePartInfo(int partIndex, int clippingIndex, int patternIndex, double xPos)
			{
				PartIndex = partIndex;
				ClippingIndex = clippingIndex;
				PatternIndex = patternIndex;
				XPos = xPos;
			}
		}

		//Expanded parts. For each letter in the pattern and for each line in a part this list contains an entry.
		private readonly List<LinePartInfo> _lines;
		private readonly string _partPattern;

		//the original parts as configured by the user
		private readonly IList<Part> _parts;

		public PartPositionCalculator(IPlayedPartitura partitura)
		{
			_partPattern = partitura.PartPattern;
			_parts = partitura.AllParts;

			_lines = new List<LinePartInfo>();
			double currentX = 0D;
			int lineIndex = 0;
			for (int patternIndex = 0; patternIndex < partitura.PartPattern.Length; patternIndex++)
			{
				int partIndex = partitura.PartPattern[patternIndex] - 'A';
				Part p = _parts[partIndex];
				for (int clippingIndex = 0; clippingIndex < p.Count; clippingIndex++)
				{
					_lines.Add(
						new LinePartInfo(partIndex,
							clippingIndex, patternIndex, currentX));
					currentX += p[clippingIndex].Coords.Width;
					lineIndex++;
				}
			}
		}

		public IList<Clipping> GetClippings()
		{
			List<Clipping> clips = new List<Clipping>();

			foreach (LinePartInfo line in _lines) clips.Add(new Clipping(_parts[line.PartIndex][line.ClippingIndex]));

			//make the clippings a bit taller, cause the lines might
			//be very near to each other
			foreach (Clipping c in clips)
			{
				//this is safe, cause the outerrect is already set to page coordinates.
				Debug.Assert(c.Coords.OuterRect != null);
				c.AddAboveAndBelow(Page.ClickTolerance);
			}

			return clips;
		}

		public int GetPatternIndexFromXPosition(double viewXPos)
		{
			return _lines.Last(l => l.XPos <= viewXPos).PatternIndex;
		}

		public double GetPatternXPosition(int patternIndex)
		{
			return _lines.First(l => l.PatternIndex == patternIndex).XPos;
		}

		public bool PatternHasPartsAfter(int currentPatternIndex)
		{
			return _partPattern.Length - 1 > currentPatternIndex;
		}

		public int GetLastPatternIndex()
		{
			return _partPattern.Length - 1;
		}

		public double GetPatternEndXPos()
		{
			var last = _lines.Last();
			return last.XPos + _parts[last.PartIndex].Last().Coords.Width;
		}
	}
}