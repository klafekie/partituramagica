namespace PartituraMagica.Lib.Playing
{
	internal class ZoomManager : IZoomManager
	{
		private float _xZoom;

		//with the help of xZoomdelta and yzoomdelta we implement a 
		//smooth change of the zoom. See GetNextZoom()
		private int _xZoomDelta;

		private float _yPosition;
		private int _yPositionDelta;
		private float _yZoom;
		private int _yZoomDelta;

		public float XZoom
		{
			get => _xZoom;
			set
			{
				_xZoom = value == 0F ? 0.000001F : value;
				_xZoomDelta = 0;
			}
		}

		public float YZoom
		{
			get => _yZoom;
			set
			{
				_yZoom = value == 0F ? 0.000001F : value;
				_yZoomDelta = 0;
			}
		}

		public float YPosition
		{
			get => _yPosition;
			set
			{
				_yPosition = value;
				_yPositionDelta = 0;
			}
		}

		public void ZoomOut(bool xOnly)
		{
			_xZoomDelta = -15;
			if (!xOnly) _yZoomDelta = -15;
		}

		public void ZoomIn(bool xOnly)
		{
			_xZoomDelta = 15;
			if (!xOnly) _yZoomDelta = 15;
		}

		public void Init(PlayerMemento memento)
		{
			XZoom = memento.XZoom;
			YZoom = memento.YZoom;
			YPosition = memento.YPosition;
		}

		public void ToMemento(PlayerMemento memento)
		{
			memento.XZoom = XZoom;
			memento.YZoom = YZoom;
			memento.YPosition = YPosition;
		}

		/** 
        * This method is called each time the current view is rendered.
        * When the user has increased the zoom, the new value is "smoothly"
        * reached. That is, on each call to this method the returned value is 
        * a bit bigger until the actual target zoom value is reached.
        * This results in a nice "animated" zoom it behaviour
        */
		public void GetNextZoom(out float zoomXRes, out float zoomYRes, out float yPositionRes)
		{
			if (_xZoomDelta > 0)
			{
				_xZoom *= 1.01F;
				_xZoomDelta--;
			}

			if (_xZoomDelta < 0)
			{
				_xZoom *= 0.99F;
				_xZoomDelta++;
			}

			if (_yZoomDelta > 0)
			{
				_yZoom *= 1.01F;
				_yZoomDelta--;
			}

			if (_yZoomDelta < 0)
			{
				_yZoom *= 0.99F;
				_yZoomDelta++;
			}

			if (_yPositionDelta > 0)
			{
				_yPosition -= 0.01F;
				_yPositionDelta--;
			}

			if (_yPositionDelta < 0)
			{
				_yPosition += 0.01F;
				_yPositionDelta++;
			}

			zoomXRes = _xZoom;
			zoomYRes = _yZoom;
			yPositionRes = _yPosition;
		}

		public void MoveUp()
		{
			_yPositionDelta = -15;
		}

		public void MoveDown()
		{
			_yPositionDelta = 15;
		}
	}
}