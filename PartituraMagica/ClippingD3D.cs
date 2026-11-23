#region usings

using System;
using System.Runtime.InteropServices;
using PartituraMagica.Lib;
using PartituraMagica.Lib.Domain;
using PartituraMagica.Lib.Playing;
using Vortice.Direct3D9;

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
			public const VertexFormat Format = VertexFormat.Position | VertexFormat.Diffuse | VertexFormat.Texture1;
		}

		private readonly ITextureCache _cache;
		private readonly IApplicationConfiguration _configuration;
		private readonly Rect _coords;
		private readonly IGreyImage _image;

		// 4-vertex quad (triangle strip). Replaces the D3DX9 Mesh used in the net48 build.
		private PositionedColoredTextured[] _vertices;

		public float Width => (float)_coords.Width;

		public double XPos { get; private set; }

		public ClippingD3D(IGreyImage image, Rect coords, IApplicationConfiguration configuration, ITextureCache cache)
		{
			_image = image;
			_configuration = configuration;
			_cache = cache;
			_coords = new Rect(coords);
		}

		public void Dispose() { }

		public void Init(IDirect3DDevice9 dev, float offset)
		{
			XPos = offset;

			float imageWidth  = _image.Width  * 96F / (float)_image.DpiX;
			float imageHeight = _image.Height * 96F / (float)_image.DpiY;

			float uvLeft   = (float)_coords.X      / imageWidth;
			float uvTop    = (float)_coords.Y      / imageHeight;
			float uvRight  = (float)_coords.Right  / imageWidth;
			float uvBottom = (float)_coords.Bottom / imageHeight;

			// Reproduce original mesh vertex layout: box centered at (0,0), shifted by deltaY.
			// Before shift: top Y = +halfH (Tv=top), bottom Y = -halfH (Tv=bottom).
			float halfH  = (float)_coords.Height / 2F;
			float deltaY = (float)(_coords.Height / 2D - _coords.CenterY);
			float topY    = halfH - deltaY;
			float bottomY = -halfH - deltaY;
			float leftX  = offset;
			float rightX = offset + (float)_coords.Width;

			// Triangle strip order: TL, TR, BL, BR (CullMode=None so winding irrelevant)
			_vertices = new PositionedColoredTextured[4];
			_vertices[0] = new PositionedColoredTextured { X = leftX,  Y = topY,    Color = _configuration.Color, Tu = uvLeft,  Tv = uvTop    };
			_vertices[1] = new PositionedColoredTextured { X = rightX, Y = topY,    Color = _configuration.Color, Tu = uvRight, Tv = uvTop    };
			_vertices[2] = new PositionedColoredTextured { X = leftX,  Y = bottomY, Color = _configuration.Color, Tu = uvLeft,  Tv = uvBottom };
			_vertices[3] = new PositionedColoredTextured { X = rightX, Y = bottomY, Color = _configuration.Color, Tu = uvRight, Tv = uvBottom };
		}

		public void Render(IDirect3DDevice9 dev)
		{
			if (_vertices == null) throw new Exception("vertices not initialized");

			IDirect3DTexture9 texture = (IDirect3DTexture9)_cache.GetTexture(_image.Path);
			if (texture == null) throw new Exception("Couldn't load texture");

			dev.SetTexture(0, texture);

			dev.SetTextureStageState(0, TextureStage.ColorOperation,
				_configuration.InvertColors
					? (int)TextureOperation.Subtract
					: (int)TextureOperation.Modulate);
			dev.SetTextureStageState(0, TextureStage.ColorArg1, (int)TextureArgument.Diffuse);
			dev.SetTextureStageState(0, TextureStage.ColorArg2, (int)TextureArgument.Texture);
			dev.SetTextureStageState(0, TextureStage.AlphaOperation, (int)TextureOperation.Disable);

			dev.VertexFormat = PositionedColoredTextured.Format;

			var gch = GCHandle.Alloc(_vertices, GCHandleType.Pinned);
			try
			{
				dev.DrawPrimitiveUP(PrimitiveType.TriangleStrip, 2,
					gch.AddrOfPinnedObject(),
					(uint)Marshal.SizeOf<PositionedColoredTextured>());
			}
			finally
			{
				gch.Free();
			}
		}
	}
}
