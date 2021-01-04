namespace PartituraMagica.Lib.Playing
{
	internal interface IZoomManager
	{
		float XZoom { get; set; }
		float YZoom { get; set; }
		void ZoomOut(bool xOnly);
		void ZoomIn(bool xOnly);

		/** 
        * This method is called each time the current view is rendered.
        * When the user has increased the zoom, the new value is "smoothly"
        * reached. That is, on each call to this method the returned value is 
        * a bit bigger until the actual target zoom value is reached.
        * This results in a nice "animated" zoom in/out behaviour
        */
		void GetNextZoom(out float zoomXRes, out float zoomYRes);
	}
}