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
        private readonly Rect _coords;

        private readonly Color _foregroundColor;
        private readonly IGreyImage _image;
        private readonly bool _invertColors;

        /// <summary>
        /// The D3D representation of the contents of this clipping, which can be 'scrolled' by the D3D rendering.
        /// Is a mesh of type 'box', depth 0 so it's like a rectangle of paper.
        /// The contents to be displayed gets set as the texture.
        /// </summary>
        private Mesh _mesh; // [renamed from 'box' jed 20150118]

        public ClippingD3D(IGreyImage image, Rect coords, bool invertColors, Color foregroundColor)
        {
            _image = image;
            _invertColors = invertColors;
            _foregroundColor = foregroundColor;
            _coords = new Rect(coords);
        }

        public float Width
        {
            get { return (float) _coords.Width; }
        }

        public float Height
        {
            get { return (float) _coords.Height; }
        }

        public double XPos { get; private set; }

        public double Right
        {
            get { return XPos + Width; }
        }

        public void Dispose()
        {
            if (_mesh != null)
            {
                _mesh.Dispose();
            }
        }

        public void Init(Device dev, float offset)
        {
            _mesh = Mesh.CreateBox(dev, (float) _coords.Width, (float) _coords.Height, 0f);

            XPos = offset;

            // Replace _mesh by a clone of itself (why?) [jed]
            Mesh tempBox = _mesh.Clone(dev, _mesh.CreationOptions, PositionedColoredTextured.Format);
            _mesh.Dispose();
            _mesh = tempBox;

            try
            {
                //Debug.WriteLine("offset: " + offset.ToString());
                DataStream stream = _mesh.LockVertexBuffer(LockFlags.None);

                float width = _image.Width*96F/(float) _image.DpiX;
                float height = _image.Height*96F/(float) _image.DpiY;

                var vertices = new PositionedColoredTextured[_mesh.VertexCount];
                for (int i = 0; i < _mesh.VertexCount; ++i)
                {
                    vertices[i] = stream.Read<PositionedColoredTextured>();
                    //var vertice = vertices[i];
                    //Debug.WriteLine(String.Format("{0}: ({1}; {2}; {3}) u:{4} v:{5}", i, vertice.X, vertice.Y, vertice.Z, vertice.Tu, vertice.Tv));
                }
                _mesh.UnlockVertexBuffer();
                stream = _mesh.LockVertexBuffer(LockFlags.None);
                for (int i = 0; i < _mesh.VertexCount; ++i)
                {
                    var vertice = vertices[i];
                    float left = (float) _coords.X/width;
                    float top = (float) _coords.Y/height;
                    float right = (float) _coords.Right/width;
                    float bottom = (float) _coords.Bottom/height;

                    vertice.Color = _foregroundColor.ToArgb();
                    vertice.Tu = vertice.X >= 0 ? right : left;
                    vertice.Tv = vertice.Y >= 0 ? top : bottom;
                    vertice.X += offset + (float) _coords.Width/2F;
                    //Debug.WriteLine(String.Format("{0}: ({1}; {2}; {3}) u:{4} v:{5}", i, vertice.X, vertice.Y, vertice.Z, vertice.Tu, vertice.Tv));

                    stream.Write(vertice);
                }
            }
            finally
            {
                _mesh.VertexBuffer.Unlock();
                //				Clock("Created Mesh");
            }
            //			PrintClock();
        }

        public void Render(Device dev)
        {
            if (_mesh == null)
            {
                return;
            }

            dev.SetTexture(0, Cache.Get(_image, dev).Texture);

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
                _invertColors
                    ? TextureOperation.Subtract
                    : TextureOperation.Modulate);
            dev.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Diffuse);
            dev.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Texture); //??
            dev.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Disable);
            _mesh.DrawSubset(0);
        }

        public static void Cleanup()
        {
            Cache.Cleanup();
        }

        //caution: do not change the order of struct members
        [StructLayout(LayoutKind.Sequential)]
        private struct PositionedColoredTextured
        {
            public float X;
            public readonly float Y;
            public readonly float Z;
            public int Color;
            public float Tu;
            public float Tv;
            public const VertexFormat Format = VertexFormat.Texture1 | VertexFormat.Position | VertexFormat.Diffuse;
        }

        private class Cache
        {
            private static readonly Dictionary<string, Cache> LoadedPages = new Dictionary<string, Cache>();

            private Cache(Texture t)
            {
                Texture = t;
            }

            public Texture Texture { get; }

            public static void Cleanup()
            {
                foreach (Cache b in LoadedPages.Values)
                {
                    b.Texture.Dispose();
                }
                LoadedPages.Clear();
            }

            public static Cache Get(IGreyImage image, Device dev)
            {
                if (!LoadedPages.ContainsKey(image.Path))
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
                    int width = 0; // 2048; // 0 means 'use the resolution from file' [todo check]
                    int height = 0; // 2048;
                    int levelCount = 1;
                    Filter filter = Filter.Point; // options a.o.: Triangle / Linear / Point / None;
                    Texture texture = Texture.FromFile(dev, image.Path,
                        width, height,
                        levelCount, //mip levels
                        Usage.None, //
                        Format.Unknown, //take from file
                        Pool.Default, //we don't need to lock (change) the content
                        filter, //how to reduce the source image
                        Filter.None, //how to handle transition from one mip map to the smaller/bigger one
                        0x00000000);
                    LoadedPages[image.Path] = new Cache(texture);
                    Trace.TraceInformation("success");
                }
                return LoadedPages[image.Path];
            }
        }
    }
}