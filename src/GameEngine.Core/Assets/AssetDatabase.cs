using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using GameEngine.Core.IO;

namespace GameEngine.Core.Assets
{
    public class AssetDatabase
    {
        private readonly FileSystemService _fileSystem;
        private readonly Dictionary<Guid, string> _assetGuidToPath = new();
        private readonly Dictionary<string, Guid> _pathToAssetGuid = new();
        private readonly Dictionary<Guid, List<Guid>> _dependencies = new();

        public event Action<Guid>? AssetImported;
        public event Action<Guid>? AssetDeleted;

        public AssetDatabase(FileSystemService fileSystem)
        {
            _fileSystem = fileSystem;
            _fileSystem.FileChanged += HandleFileChange;
        }

        public Guid ImportAsset(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Asset file not found", path);

            var guid = Guid.NewGuid();
            var metaPath = $"{path}.meta";
            
            var metadata = new AssetMetadata
            {
                Guid = guid,
                OriginalPath = path,
                LastModified = DateTime.UtcNow,
                Dependencies = new List<Guid>()
            };

            _fileSystem.CreateDirectory(Path.GetDirectoryName(metaPath)!);
            File.WriteAllText(metaPath, JsonConvert.SerializeObject(metadata));
            
            _assetGuidToPath[guid] = path;
            _pathToAssetGuid[path] = guid;
            
            AssetImported?.Invoke(guid);
            return guid;
        }

        public string GetAssetPath(Guid guid)
        {
            if (!_assetGuidToPath.TryGetValue(guid, out string? path))
                throw new KeyNotFoundException($"Asset not found: {guid}");
            
            return path;
        }

        private void HandleFileChange(string path)
        {
            if (path.EndsWith(".meta")) return;
            
            if (_pathToAssetGuid.TryGetValue(path, out Guid guid))
            {
                // Handle asset modification
                AssetImported?.Invoke(guid);
            }
            else
            {
                // New asset detected
                ImportAsset(path);
            }
        }

        // Additional asset database methods...
    }

    public class AssetMetadata
    {
        public Guid Guid { get; set; }
        public string OriginalPath { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
        public List<Guid> Dependencies { get; set; } = new();
        public Dictionary<string, object> ImportSettings { get; set; } = new();
        public int Version { get; set; } = 1;
    }
} 