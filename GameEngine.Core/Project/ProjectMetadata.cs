using System;
using Newtonsoft.Json;

namespace GameEngine.Core.Project
{
    public class ProjectMetadata
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("lastModified")]
        public DateTime LastModified { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; } = "1.0.0";

        [JsonProperty("engineVersion")]
        public string EngineVersion { get; set; } = "1.0.0";

        [JsonProperty("projectPath")]
        public string ProjectPath { get; set; } = string.Empty;

        public ProjectMetadata()
        {
            Created = DateTime.UtcNow;
            LastModified = Created;
        }
    }
} 