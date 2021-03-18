#region usings

using System;
using System.Runtime.InteropServices;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.Playing;
using SlimDX;
using SlimDX.Direct3D9;

#endregion

namespace PartituraMagica.App
{
	/** 
     * The Direct3D visual representation of a Clipping for use in the 
     * PlayerViewD3D impl.
     */
	internal class ClippingD3D : IDisposable
	{
		//caution: do not change the order of struct members
		[StructLayout(LayoutKind.Sequential)]
		private struct PositionedColoredTextured
		{
			public float X;
			public float Y;
			public readonly float Z;
			public int Color;
			public float Tu;
			public float Tv;
			public const VertexFormat Format = VertexFormat.Texture1 | VertexFormat.Position | VertexFormat.Diffuse;
		}

		private readonly ITextureCache _cache;

		private readonly ApplicationConfiguration _configuration;
		private readonly Rect _coords;
		private readonly IGreyImage _image;

		/// <summary>
		///   The D3D representation of the contents of this clipping, which can be 'scrolled' by the D3D rendering.
		///   Is a mesh of type 'box', depth 0 so it's like a rectangle of paper.
		///   The contents to be displayed gets set as the texture.
		/// </summary>
		private Mesh _mesh; // [renamed from 'box' jed 20150118]

		public float Width => (float) _coords.Width;

		public double XPos { get; private set; }

		public ClippingD3D(IGreyImage image, Rect coords, ApplicationConfiguration configuration, ITextureCache cache)
		{
			_image = image;
			_configuration = configuration;
			_cache = cache;
			_coords = new Rect(coords);
		}

		public void Dispose()
		{
			_mesh?.Dispose();
		}

		public void Init(Device dev, float offset)
		{
			_mesh = Mesh.CreateBox(dev, (float) _coords.Width, (float) _coords.Height, 0f);

			XPos = offset;

			// Replace _mesh by a clone of itself to set format
			Mesh tempBox = _mesh.Clone(dev, _mesh.CreationOptions, PositionedColoredTextured.Format);
			_mesh.Dispose();
			_mesh = tempBox;

			try
			{
				//Debug.WriteLine("offset: " + offset.ToString());
				DataStream stream = _mesh.LockVertexBuffer(LockFlags.None);

				float imageWidth = _image.Width * 96F / (float) _image.DpiX;
				float imageHeight = _image.Height * 96F / (float) _image.DpiY;

				var vertices = new PositionedColoredTextured[_mesh.VertexCount];
				for (int i = 0; i < _mesh.VertexCount; ++i)
				{
					vertices[i] = stream.Read<PositionedColoredTextured>();
					var vertice = vertices[i];
					//Trace.WriteLine($"{i}: ({vertice.X}; {vertice.Y}; {vertice.Z}) u:{vertice.Tu} v:{vertice.Tv}");
				}

				_mesh.UnlockVertexBuffer();
				stream = _mesh.LockVertexBuffer(LockFlags.None);
				float deltaY = (float) (_coords.Height / 2D - _coords.CenterY);
				for (int i = 0; i < _mesh.VertexCount; ++i)
				{
					var vertice = vertices[i];
					float left = (float) _coords.X / imageWidth;
					float top = (float) _coords.Y / imageHeight;
					float right = (float) _coords.Right / imageWidth;
					float bottom = (float) _coords.Bottom / imageHeight;

					vertice.Color = _configuration.Color.ToArgb();
					vertice.Tu = vertice.X >= 0 ? right : left;
					vertice.Tv = vertice.Y >= 0 ? top : bottom;
					vertice.X += offset + (float) _coords.Width / 2F;
					vertice.Y -= deltaY;

					//Trace.WriteLine($"{i}: ({vertice.X}; {vertice.Y}; {vertice.Z}) u:{vertice.Tu} v:{vertice.Tv}");

					stream.Write(vertice);
				}
			}
			finally
			{
				_mesh.VertexBuffer.Unlock();
			}
		}

		public void Render(Device dev)
		{
			if (_mesh == null) throw new Exception("mesh not initialized");

			Texture texture = (Texture) _cache.GetTexture(_image.Path);

			if (texture == null) throw new Exception("Couldn't load texture");
			dev.SetTexture(0, texture);

			// Set the 'blending mode': how colors are mixed.
			// Note: the following three calls (ColorOperation, ColorArg1, ColorArg2 belong together [according to
			// http://msdn.microsoft.com/en-us/library/windows/desktop/bb204870%28v=vs.85%29.aspx]
			/* Experiment with ColorArgs [jed 201412]:
			 * - Doc found:
			 *   - http://msdn.microsoft.com/en-us/library/windows/desktop/bb204870%28v=vs.85%29.aspx
			 *   - http://msdn.microsoft.com/en-us/library/windows/desktop/bb172617%28v=vs.85%29.aspx
			 * - ColorArg1 'Texture' gives no visible texture.
			 * - Disabling the ColorArg2 line of setting the default (= .Current) explicitly gives no visible texture.
			 */
			dev.SetTextureStageState(0, TextureStage.ColorOperation,
				_configuration.InvertColors
					? TextureOperation.Subtract
					: TextureOperation.Modulate);
			dev.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Diffuse);
			dev.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Texture); //??
			dev.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Disable);
			_mesh.DrawSubset(0);
		}
	}
}