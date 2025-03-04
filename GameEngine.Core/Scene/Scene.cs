using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace GameEngine.Core.Scene
{
    public class Scene
    {
        private string _name = "New Scene";
        private readonly ObservableCollection<GameObject> _rootObjects = new();

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public ObservableCollection<GameObject> RootObjects => _rootObjects;

        public GameObject CreateGameObject(string name = "GameObject", GameObject? parent = null)
        {
            var gameObject = new GameObject(name);
            gameObject.AddComponent<Components.Transform>();

            if (parent != null)
            {
                parent.AddChild(gameObject);
            }
            else
            {
                _rootObjects.Add(gameObject);
            }

            return gameObject;
        }

        public void DestroyGameObject(GameObject gameObject)
        {
            if (gameObject.Parent != null)
            {
                gameObject.Parent.RemoveChild(gameObject);
            }
            else
            {
                _rootObjects.Remove(gameObject);
            }

            foreach (var component in gameObject.Components)
            {
                component.OnDestroy();
            }

            // Recursively destroy children
            var children = gameObject.Children.ToList();
            foreach (var child in children)
            {
                DestroyGameObject(child);
            }
        }

        public async Task SaveAsync(string path)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };

            var json = JsonConvert.SerializeObject(this, settings);
            await File.WriteAllTextAsync(path, json);
        }

        public static async Task<Scene> LoadAsync(string path)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            var json = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<Scene>(json, settings) ?? new Scene();
        }
    }
} 