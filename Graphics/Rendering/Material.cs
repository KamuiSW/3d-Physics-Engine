using OpenTK.Mathematics;

namespace GameEngine.Graphics.Rendering
{
    public class Material
    {
        public Shader Shader { get; set; }
        public Texture AlbedoMap { get; set; }
        public Texture NormalMap { get; set; }
        public Texture MetallicRoughnessMap { get; set; }
        public Texture AmbientOcclusionMap { get; set; }
        
        public Vector3 Albedo { get; set; } = new Vector3(1.0f);
        public float Metallic { get; set; } = 0.0f;
        public float Roughness { get; set; } = 0.5f;
        public float AmbientOcclusion { get; set; } = 1.0f;

        public void Use()
        {
            Shader.Use();
            
            // Bind textures
            if (AlbedoMap != null)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                AlbedoMap.Bind();
                Shader.SetInt("albedoMap", 0);
            }
            
            // Set material properties
            Shader.SetVector3("material.albedo", Albedo);
            Shader.SetFloat("material.metallic", Metallic);
            Shader.SetFloat("material.roughness", Roughness);
            Shader.SetFloat("material.ao", AmbientOcclusion);
        }
    }
} 