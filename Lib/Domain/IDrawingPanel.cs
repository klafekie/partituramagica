using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PartituraMagica
{
	public interface IImageLoader
	{
		IGreyImage GetImage(string path);
	}

    /** 
	 * represents a user interface control where the user can see the 
	 * current page and draw boxes on it 
	 */
    public interface IDrawingPanel : IImageLoader
	{
		/** new rectangles are initially painted as deselected. */
		void AddRectangle(int id, Rect coordinates);
		void AddOrUpdateRectangle(int id, Rect coordinates);
		void SelectRectangle(int id);
		void DeselectRectangle(int id);
		void RemoveRectangle(int id);        
		/** labels the rectangle with id with the given string.
		 * if label is null, any labels are removed!
		 */
		void LabelRectangle(int id, string label);
		void Clear();
		Point DrawImage(string path);

		IGreyImage CurrentImage
		{
			get;
		}			
	}
}
