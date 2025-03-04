using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameEngine.Core.Project
{
    public class ProjectSettings
    {
        public string ProjectName { get; set; } = "New Project";
        public string RootDirectory { get; set; } = string.Empty;
        public List<string> AssetPaths { get; set; } = new();
        public Dictionary<string, object> DefaultImportSettings { get; set; } = new()
        {
            ["TextureCompression"] = "BC3",
            ["ModelScale"] = 1.0f,
            ["GenerateColliders"] = true
        };
        
        public void Save(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static ProjectSettings Load(string path)
        {
            return JsonConvert.DeserializeObject<ProjectSettings>(File.ReadAllText(path))
                ?? throw new InvalidDataException("Invalid project settings file");
        }
    }
} 