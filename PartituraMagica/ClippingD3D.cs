using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D9;

namespace PartituraMagica
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
			public float Z;
			public int Color;
			public float Tu;
			public float Tv;
			public const VertexFormat Format = VertexFormat.Texture1 | VertexFormat.Position | VertexFormat.Diffuse;
		}
		private readonly Rect coords;
		private readonly IGreyImage image;

		private readonly int color = Color.White.ToArgb();
		private const bool invertColors = true;

		private Mesh box;
		private double xPos;

		private class Cache
		{
			private readonly Texture texture;
			public Texture Texture { get { return texture; } }

			private static readonly Dictionary<string, Cache> loadedPages = new Dictionary<string, Cache>();

			private Cache(Texture t)
			{
				texture = t;
			}

			public static void Cleanup()
			{
				foreach (Cache b in loadedPages.Values)
				{
					b.texture.Dispose();
				}
				loadedPages.Clear();
			}

			public static Cache Get(IGreyImage image, Device dev)
			{
				if (!loadedPages.ContainsKey(image.Path))
				{
					//@todo p2 optimize scaling so that it doesn't flicker for low zoom factors
					//for this we can use mip mapping and find the optimal filter for the texture
					//maybe multisampling could help??

					//we use a fixed size of 2048x2048 cause some graphic adapters only allow
					//square textures with a size that is a power of two.

					//@todo p2 if adapter only supports square textures
					//can be checked with caps.Texturecaps.SquareOnly
					//if only powers of two are supported:
					//can be checked with caps.Texturecaps.SupportsPowerTwo

					Trace.TraceInformation("Cache.Get() loading texture from file: " + image.Path);
					loadedPages[image.Path] = new Cache(
						Texture.FromFile(dev, image.Path,
							2048, 2048,
							1, //mip levels
							Usage.None, //
							Format.Unknown, //take from file
							Pool.Default, //we don't need to lock (change) the content
							Filter.Linear, //how to reduce the source image
							Filter.None, //how to handle transition from one mip map to the smaller/bigger one
							unchecked(0x00000000)));
					Trace.TraceInformation("success");
				}
				return loadedPages[image.Path];
			}
		}

		public ClippingD3D(IGreyImage image, Rect coords)
		{
			this.image = image;
			this.coords = new Rect(coords);
		}

		public float Width { get { return (float) coords.Width; } }

		public float Height { get { return (float) coords.Height; } }

		public double XPos { get { return xPos; } }

		public double Right { get { return XPos + Width; } }

		public void Init(Device dev, float offset)
		{
			box = Mesh.CreateBox(dev, (float) coords.Width, (float) coords.Height, 0f);
			
			xPos = offset;

			Mesh tempBox = box.Clone(dev, box.CreationOptions, PositionedColoredTextured.Format);
			box.Dispose();
			box = tempBox;

			try
			{
				//Debug.WriteLine("offset: " + offset.ToString());
				DataStream stream = box.LockVertexBuffer(LockFlags.None);

				float width = image.Width * 96F / (float) image.DpiX;
				float height = image.Height * 96F / (float) image.DpiY;
				
				var vertices = new PositionedColoredTextured[box.VertexCount];
				for (int i = 0; i < box.VertexCount; ++i)
				{
					vertices[i] = stream.Read<PositionedColoredTextured>();
					//var vertice = vertices[i];
					//Debug.WriteLine(String.Format("({0}; {1}; {2}) u:{3} v:{4}", vertice.X, vertice.Y, vertice.Z, vertice.Tu, vertice.Tv));
				}
				box.UnlockVertexBuffer();
				stream = box.LockVertexBuffer(LockFlags.None);
				for (int i = 0; i < box.VertexCount; ++i)
				{
					var vertice = vertices[i];
					float left = (float) coords.X / width;
					float top = (float) coords.Y / height;
					float right = (float) coords.Right / width;
					float bottom = (float) coords.Bottom / height;

					vertice.Color = color;
					vertice.Tu = vertice.X >= 0 ? right : left;
					vertice.Tv = vertice.Y >= 0 ? top : bottom;
					vertice.X += offset + (float) coords.Width / 2F;
					//Debug.WriteLine(String.Format("({0}; {1}; {2}) u:{3} v:{4}", vertice.X, vertice.Y, vertice.Z, vertice.Tu, vertice.Tv));
					
					stream.Write(vertice);
				}
			}
			finally
			{
				box.VertexBuffer.Unlock();
				//				Clock("Created Mesh");
			}
			//			PrintClock();
		}

		public void Render(Device dev)
		{
			if (box == null)
			{
				return;
			}

			dev.SetTexture(0, Cache.Get(image, dev).Texture);

			dev.SetTextureStageState(0, TextureStage.ColorOperation,
				invertColors ?
					TextureOperation.Subtract : TextureOperation.Modulate);

			dev.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Diffuse);
			dev.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Texture); //??
			dev.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Disable);
			box.DrawSubset(0);
		}

		public void Dispose()
		{
			if (box != null)
			{
				box.Dispose();
			}
		}

		public static void Cleanup()
		{
			Cache.Cleanup();
		}
	}
}