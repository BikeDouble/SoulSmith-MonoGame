using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json;
using SoulSmith.Collections;

namespace SoulSmith.Asset
{ 
    /// <summary>
    /// Class to load and retrieve assets.
    /// </summary>
    public class AssetManager
    {
        public const string FILEPREFIX = "../../../Assets/";
        public const int PERIODSINPREFIX = 6;

        private Dictionary<string, string> _manifest;
        private ContentManager _content;
        private GraphicsDevice _graphics;

        // Cached data
        private Cache _textureCache;
        private Cache _zonedTextureCache;
        private Cache _unitTemplateCache;

        // Loaders
        private IGraphicsAssetLoader _textureLoader;
        private IGraphicsAssetLoader _zonedTextureLoader;
        private IBasicAssetLoader _unitTemplateLoader;
        private IBasicAssetLoader _moveLoader;

        public static void Initialize(ContentManager content, GraphicsDevice graphics, AssetManifest manifest)
        {
            if (Instance == null) Instance = new AssetManager(content, graphics, manifest);
        }

        public AssetManager(ContentManager content, GraphicsDevice graphics, AssetManifest manifest)
        {
            if (Instance == null) { Instance = this; }

            _content = content;
            _manifest = manifest.Manifest;
            _graphics = graphics;
            InitializeCaches();
        }

        private void InitializeCaches()
        {
            _textureCache = new Cache();
            _zonedTextureCache = new Cache();
            _unitTemplateCache = new Cache();

        }

        public void RegisterUnitTemplateLoader(IBasicAssetLoader loader)
        {
            _unitTemplateLoader = loader;
        }

        public IReadOnlyTrackedAsset<T> GetUnitTemplate<T>(string key) where T : IAsset //TODO eliminate UnitTemplate
        {
            if (_unitTemplateCache.Contains(key)) return _unitTemplateCache.GetAsset<T>(key);

            if (!_manifest.ContainsKey(key)) return null;

            string filePath = FILEPREFIX + _manifest[key];

            if (_unitTemplateLoader == null) throw new Exception("UnitTemplate loader not registered");

            IAsset template = _unitTemplateLoader.Load(filePath);

            if (template == null) return null;

            _unitTemplateCache.CacheAsset(key, template);

            return _unitTemplateCache.GetAsset<T>(key);
        }

        public void RegisterTextureLoader(IGraphicsAssetLoader loader)
        {
            _textureLoader = loader;
        }

        public IReadOnlyTrackedAsset<T> GetTexture2D<T>(string key) where T : IAsset
        {
            if (_textureCache.Contains(key)) return _textureCache.GetAsset<T>(key);

            if (!_manifest.ContainsKey(key)) return null;

            string filepath = FILEPREFIX + _manifest[key];

            IAsset resource = _textureLoader.Load(filepath, _graphics);

            if (resource == null) return null;

            _textureCache.CacheAsset(key, resource);

            return _textureCache.GetAsset<T>(key);
        }

        public void RegisterZonedTextureLoader(IGraphicsAssetLoader loader)
        {
            _zonedTextureLoader = loader;
        }

        public IReadOnlyTrackedAsset<T> GetZonedTexture2D<T>(string key) where T : IAsset
        {
            if (_zonedTextureCache.Contains(key)) return _zonedTextureCache.GetAsset<T>(key);

            if (!_manifest.ContainsKey(key)) return null;

            string textureFilepath = FILEPREFIX + _manifest[key];

            IAsset resource = _zonedTextureLoader.Load(textureFilepath, _graphics);

            if (resource == null) return null;

            _zonedTextureCache.CacheAsset(key, resource);

            return _zonedTextureCache.GetAsset<T>(key);
        }

        public void RegisterMoveLoader(IBasicAssetLoader loader) { _moveLoader = loader; }

        public T GetMove<T>(string key) where T : IAsset
        {
            if (!_manifest.ContainsKey(key)) return default(T);

            string filePath = FILEPREFIX + _manifest[key];

            if (_moveLoader == null) throw new Exception("Move loader not registered");
            
            T move = (T)_moveLoader.Load(filePath);

            return move;
        }

        public static string GetMetaFilepathFromFilepath(string filepath, string metafileExtension = "json")
        {
            string filepathWithoutExtension = Path.Combine(Path.GetDirectoryName(filepath), Path.GetFileNameWithoutExtension(filepath));
            string metafilepath = filepathWithoutExtension + ".meta." + metafileExtension;
            return metafilepath;
        }

        public SoulSmithWeightedList<string> GetSpawnList(string key)
        {
            return GetWeightedList<string>(key);
        }

        public SoulSmithWeightedList<T> GetWeightedList<T>(string key)
        {
            if (!_manifest.ContainsKey(key)) return null;

            string filepath = FILEPREFIX + _manifest[key];

            if (!File.Exists(filepath)) return null;

            string fileString = File.ReadAllText(filepath);

            JsonSerializerOptions optionsWithWeightedListConverter = new JsonSerializerOptions();
            optionsWithWeightedListConverter.Converters.Add(new SoulSmithWeightedListJsonConverter<T>());
            SoulSmithWeightedList<T> list = JsonSerializer.Deserialize<SoulSmithWeightedList<T>>(filepath, optionsWithWeightedListConverter);

            return list;
        }

        public static AssetManager Instance { get; private set; }
    }
}
