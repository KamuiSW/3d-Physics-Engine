using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;
using SDPixelFormat = System.Drawing.Imaging.PixelFormat;
using GLPixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using System.Drawing.Imaging;
using System.IO;

namespace GameEngine.Core.Graphics
{
    public class Texture : IDisposable
    {
        private readonly int _handle;
        private bool _disposed;

        public int Width { get; }
        public int Height { get; }
        public TextureFormat Format { get; }
        public bool HasMipmaps { get; private set; }
        public TextureWrapMode WrapMode { get; set; }
        public TextureFilterMode FilterMode { get; set; }

        public Texture(int width, int height, TextureFormat format = TextureFormat.RGBA8)
        {
            Width = width;
            Height = height;
            Format = format;
            WrapMode = TextureWrapMode.Repeat;
            FilterMode = TextureFilterMode.Linear;

            _handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _handle);

            // Allocate storage
            switch (format)
            {
                case TextureFormat.R8:
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, width, height, 0,
                        GLPixelFormat.Red, PixelType.UnsignedByte, IntPtr.Zero);
                    break;
                case TextureFormat.RGB8:
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, width, height, 0,
                        GLPixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
                    break;
                case TextureFormat.RGBA8:
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0,
                        GLPixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
                    break;
                case TextureFormat.R16F:
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R16f, width, height, 0,
                        GLPixelFormat.Red, PixelType.Float, IntPtr.Zero);
                    break;
                case TextureFormat.RGB16F:
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, width, height, 0,
                        GLPixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
                    break;
                case TextureFormat.RGBA16F:
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, width, height, 0,
                        GLPixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
                    break;
            }

            UpdateParameters();
        }

        public static Texture FromFile(string path)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            using var image = new Bitmap(path);
            var texture = new Texture(image.Width, image.Height);
            texture.SetData(image);
            return texture;
#pragma warning restore CA1416
        }

        public void SetData<T>(T[] data) where T : unmanaged
        {
            GL.BindTexture(TextureTarget.Texture2D, _handle);

            var format = Format switch
            {
                TextureFormat.R8 => GLPixelFormat.Red,
                TextureFormat.RGB8 => GLPixelFormat.Rgb,
                TextureFormat.RGBA8 => GLPixelFormat.Rgba,
                TextureFormat.R16F => GLPixelFormat.Red,
                TextureFormat.RGB16F => GLPixelFormat.Rgb,
                TextureFormat.RGBA16F => GLPixelFormat.Rgba,
                _ => throw new ArgumentException("Unsupported texture format")
            };

            var type = Format switch
            {
                TextureFormat.R8 or TextureFormat.RGB8 or TextureFormat.RGBA8 => PixelType.UnsignedByte,
                TextureFormat.R16F or TextureFormat.RGB16F or TextureFormat.RGBA16F => PixelType.Float,
                _ => throw new ArgumentException("Unsupported texture format")
            };

            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, Width, Height, format, type, data);

            if (FilterMode == TextureFilterMode.Linear)
            {
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                HasMipmaps = true;
            }
        }

        public void SetData(Bitmap bitmap)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            GL.BindTexture(TextureTarget.Texture2D, _handle);

            var data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                SDPixelFormat.Format32bppArgb
            );

            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, bitmap.Width, bitmap.Height,
                GLPixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);

            if (FilterMode == TextureFilterMode.Linear)
            {
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                HasMipmaps = true;
            }
#pragma warning restore CA1416
        }

        public void Use(int unit = 0)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + unit);
            GL.BindTexture(TextureTarget.Texture2D, _handle);
        }

        private void UpdateParameters()
        {
            GL.BindTexture(TextureTarget.Texture2D, _handle);

            // Set wrap mode
            var wrap = WrapMode switch
            {
                TextureWrapMode.Repeat => OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat,
                TextureWrapMode.Clamp => OpenTK.Graphics.OpenGL4.TextureWrapMode.ClampToBorder,
                TextureWrapMode.Mirror => OpenTK.Graphics.OpenGL4.TextureWrapMode.MirroredRepeat,
                _ => OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat
            };

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrap);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrap);

            // Set filter mode
            var min = FilterMode == TextureFilterMode.Linear
                ? (HasMipmaps ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear)
                : TextureMinFilter.Nearest;
            var mag = FilterMode == TextureFilterMode.Linear
                ? TextureMagFilter.Linear
                : TextureMagFilter.Nearest;

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)min);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)mag);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                GL.DeleteTexture(_handle);
                _disposed = true;
            }
        }

        ~Texture()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public enum TextureFormat
    {
        R8,
        RGB8,
        RGBA8,
        R16F,
        RGB16F,
        RGBA16F
    }

    public enum TextureWrapMode
    {
        Repeat,
        Clamp,
        Mirror
    }

    public enum TextureFilterMode
    {
        Point,
        Linear
    }
} 