using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using System.IO;

namespace GameEngine
{
    public class ResourceManager
    {
        private static Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();
        private static Dictionary<string, int> textures = new Dictionary<string, int>();

        public static Shader LoadShader(string vertexPath, string fragmentPath, string name)
        {
            var shader = new Shader(vertexPath, fragmentPath);
            shaders[name] = shader;
            return shader;
        }

        public static Shader GetShader(string name)
        {
            return shaders[name];
        }

        public static int LoadTexture(string path)
        {
            if (textures.ContainsKey(path))
                return textures[path];

            int textureHandle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);

            // Load texture using StbImageSharp or similar library
            // For now, this is a placeholder
            
            textures[path] = textureHandle;
            return textureHandle;
        }

        public static void Cleanup()
        {
            foreach (var shader in shaders.Values)
            {
                shader.Dispose();
            }
            
            foreach (var texture in textures.Values)
            {
                GL.DeleteTexture(texture);
            }
        }
    }
} 