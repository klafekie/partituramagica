using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Runtime.InteropServices;
using System.Text;

namespace PartituraMagica
{
	/** 
	 * The Direct3D visual representation of a Clipping for use in the 
	 * PlayerViewD3D impl.
	 */
	internal class ClippingD3D : IDisposable
	{
		private Rect coords;
		private IGreyImage image;

		private int color = Color.White.ToArgb();
		private bool invertColors = true;

		private Mesh box = null;
		private double xPos = 0d;

		private class Cache
		{
			Texture texture;
			float width;
			float height;
			public Texture Texture { get { return texture; } }
			public float Width { get { return width; } }
			public float Height { get { return height; } }

			private static Dictionary<string, Cache> loadedPages = new Dictionary<string, Cache>();

			private Cache(Texture t, float width, float height) {
				this.texture = t;
				this.width = width;
				this.height = height;
			}

			public static void Cleanup() {
				foreach (Cache b in loadedPages.Values) {
					b.texture.Dispose();
				}
				loadedPages.Clear();
			}

			public static Cache Get(IGreyImage image, Device dev) {
				if (!loadedPages.ContainsKey(image.Path)) {					
					
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
						TextureLoader.FromFile(dev, image.Path, 
							(int)2048, (int)2048, 
							1, //mip levels
							Usage.None, //
							Format.Unknown, //take from file
							Pool.Default, //we don't need to lock (change) the content
							Filter.Linear, //how to reduce the source image
							Filter.None, //how to handle transition from one mip map to the smaller/bigger one
							unchecked((int)0x00000000)),//transparent color 0 = disable
						2048, 2048);
					Trace.TraceInformation("success");
				}
				return loadedPages[image.Path];
			}
		}

		public ClippingD3D(IGreyImage image, Rect coords) {
			this.image = image;
			this.coords = new Rect(coords);
		}

		public float Width {
			get {
                return (float)this.coords.Width;
			}
		}

		public float Height {
			get {
				return (float)this.coords.Height;
			}
		}

		public double XPos {
			get {
				return this.xPos;
			}
		}

		public double Right {
			get {
				return this.XPos + this.Width;
			}
		}		

		public void Init(Device dev, float offset) {
			Cache cache = Cache.Get(this.image, dev);
			

			this.box = Mesh.Box(dev, (float)this.coords.Width, (float)this.coords.Height, 0f);
			this.xPos = offset;
			//VertexFormats format =
				//VertexFormats.PositionNormal | VertexFormats.Texture1;
			VertexFormats format =
							CustomVertex.PositionColoredTextured.Format;

			Mesh tempBox = box.Clone(box.Options.Value, format, dev);
			box.Dispose();
			box = tempBox;
			try {
				//Debug.WriteLine("offset: " + offset.ToString());
				CustomVertex.PositionColoredTextured[] verts =
					(CustomVertex.PositionColoredTextured[])box.VertexBuffer.Lock(
						0, typeof(CustomVertex.PositionColoredTextured),
						LockFlags.None, box.NumberVertices);

				//CustomVertex.PositionNormalTextured[] verts =
				//    (CustomVertex.PositionNormalTextured[])box.VertexBuffer.Lock(
				//        0, typeof(CustomVertex.PositionNormalTextured),
				//        LockFlags.None, box.NumberVertices);
                float width = (float)image.Width * 96F / (float)image.DpiX;
                float height = (float)image.Height * 96F / (float)image.DpiY;					

				for (int i = 0; i < verts.Length; ++i) {

					float left = (float)this.coords.X / width;
					float top = (float)this.coords.Y / height;
					float right = (float)this.coords.Right / width;
					float bottom = (float)this.coords.Bottom / height;

					verts[i].Color = this.color;
					verts[i].Tu = verts[i].X >= 0 ? right : left;
					verts[i].Tv = verts[i].Y >= 0 ? top : bottom;
					verts[i].X += offset + (float)this.coords.Width / 2F;
					//	Debug.WriteLine(String.Format("({0}; {1}; {2}) u:{3} v:{4}", verts[i].X, verts[i].Y, verts[i].Z, verts[i].Tu, verts[i].Tv));
				}
			} finally {
				box.VertexBuffer.Unlock();
				//				Clock("Created Mesh");
			}
			//			PrintClock();
		}


		public void Render(Device dev) {
			if (this.box == null) return;
			
			dev.SetTexture(0, Cache.Get(this.image, dev).Texture);
						
			if (this.invertColors) {
				dev.TextureState[0].ColorOperation = TextureOperation.Subtract;
			} else {
				dev.TextureState[0].ColorOperation = TextureOperation.Modulate;
			}
			dev.TextureState[0].ColorArgument1 = TextureArgument.Diffuse;
			dev.TextureState[0].ColorArgument2 = TextureArgument.TextureColor;
			dev.TextureState[0].AlphaOperation = TextureOperation.Disable;
			this.box.DrawSubset(0);
		}

		public void Dispose() {
			this.box.Dispose();
		}

		public static void Cleanup() {
			Cache.Cleanup();
		}
	}
}
