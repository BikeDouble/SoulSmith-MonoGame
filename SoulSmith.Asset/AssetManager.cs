using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json;
using SoulSmith.Collections;
using SoulSmith.Core;

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
        private Cache _unitTemplateCache;
        private Cache _animationDataCache;
        private Cache _fontResourceCache;
        private Cache _textBoxCache;

        // Loaders
        private IGraphicsAssetLoader _soulSmithTextureLoader;
        private IBasicAssetLoader _zonedTextureLoader;
        private IBasicAssetLoader _unitTemplateLoader;
        private IBasicAssetLoader _moveLoader;
        private IBasicAssetLoader _emotionLoader;
        private IBasicAssetLoader _animationLoader;
        private IBasicAssetLoader _fontResourceLoader;
        private IBasicAssetLoader _textBoxLoader;
        private IBasicAssetLoader _effectVisualizationFactoryLoader;
        private IBasicAssetLoader _modifierFactoryLoader;

        public static void Initialize(ContentManager content, GraphicsDevice graphics, KeyPathManifest manifest)
        {
            if (Instance == null) Instance = new AssetManager(content, graphics, manifest);
        }

        public AssetManager(ContentManager content, GraphicsDevice graphics, KeyPathManifest manifest)
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
            _unitTemplateCache = new Cache();
            _animationDataCache = new Cache();
            _fontResourceCache = new Cache();
            _textBoxCache = new Cache();
        }

        public void RegisterUnitTemplateLoader(IBasicAssetLoader loader)
        {
            _unitTemplateLoader = loader;
        }

        public IAssetWrapper<T> GetUnitTemplate<T>(string key) where T : IDisposable //TODO eliminate UnitTemplate
        {
            if (_unitTemplateCache.Contains(key)) return _unitTemplateCache.GetAsset<T>(key);

            if (!_manifest.ContainsKey(key)) return null;

            string filePath = FILEPREFIX + _manifest[key];

            if (_unitTemplateLoader == null) throw new Exception("UnitTemplate loader not registered");

            IDisposable template = _unitTemplateLoader.Load(filePath);

            if (template == null) return null;

            _unitTemplateCache.CacheAsset(key, template);

            return _unitTemplateCache.GetAsset<T>(key);
        }

        public void RegisterSoulSmithTextureLoader(IGraphicsAssetLoader loader)
        {
            _soulSmithTextureLoader = loader;
        }

        public IAssetWrapper<T> GetSoulSmithTexture<T>(string key) where T : IDisposable
        {
            if (_textureCache.Contains(key)) return _textureCache.GetAsset<T>(key);

            if (!_manifest.ContainsKey(key)) return null;

            string filepath = FILEPREFIX + _manifest[key];

            IDisposable resource = _soulSmithTextureLoader.Load(filepath, _graphics);

            if (resource == null) return null;

            _textureCache.CacheAsset(key, resource);

            return _textureCache.GetAsset<T>(key);
        }

        public void RegisterZonedTextureLoader(IBasicAssetLoader loader)
        {
            _zonedTextureLoader = loader;
        }

        public T GetZonedResource<T>(string key) where T : IDisposable
        {
            if (!_manifest.ContainsKey(key)) return default(T);

            string textureFilepath = FILEPREFIX + _manifest[key];

            T resource = (T)_zonedTextureLoader.Load(textureFilepath);

            return resource;
        }

        public void RegisterFontResourceLoader(IBasicAssetLoader loader) { _fontResourceLoader = loader; }

        public IAssetWrapper<T> GetFontResource<T>(string key) where T : IDisposable
        {
            if (_fontResourceCache.Contains(key)) return _fontResourceCache.GetAsset<T>(key);

            if (!_manifest.ContainsKey(key)) return null;

            string fontFilepath = FILEPREFIX + _manifest[key];

            IDisposable resource = _fontResourceLoader.Load(fontFilepath);

            if (resource == null) return null;

            _fontResourceCache.CacheAsset(key, resource);

            return _fontResourceCache.GetAsset<T>(key);
        }

        public void RegisterTextBoxLoader(IBasicAssetLoader loader) { _textBoxLoader = loader; }

        public IAssetWrapper<T> GetTextBox<T>(string key) where T : IDisposable
        {
            if (_textBoxCache.Contains(key)) return _textBoxCache.GetAsset<T>(key);

            if (!_manifest.ContainsKey(key)) return null;

            string textBoxFilepath = FILEPREFIX + _manifest[key];

            IDisposable resource = _textBoxLoader.Load(textBoxFilepath);

            if (resource == null) return null;

            _textBoxCache.CacheAsset(key, resource);

            return _textBoxCache.GetAsset<T>(key);
        }

        public void RegisterMoveLoader(IBasicAssetLoader loader) { _moveLoader = loader; }

        public T GetMove<T>(string key) where T : IDisposable
        {
            if (!_manifest.ContainsKey(key)) return default(T);

            string filePath = FILEPREFIX + _manifest[key];

            if (_moveLoader == null) throw new Exception("Move loader not registered");
            
            T move = (T)_moveLoader.Load(filePath);

            return move;
        }

        public void RegisterEmotionLoader(IBasicAssetLoader loader) { _emotionLoader = loader; }

        public T GetEmotion<T>(string key) where T : IDisposable
        {
            if (!_manifest.ContainsKey(key)) return default(T);

            string filePath = FILEPREFIX + _manifest[key];

            if (_emotionLoader == null) throw new Exception("Emotion loader not registered");

            T emotion = (T)_emotionLoader.Load(filePath);

            return emotion;
        }

        public void RegisterEffectVisualizationFactoryLoader(IBasicAssetLoader loader) { _effectVisualizationFactoryLoader = loader; }

        public T GetEffectVisualizationFactory<T>(string key) where T : IDisposable
        {
            if (!_manifest.ContainsKey(key)) return default(T);

            string filePath = FILEPREFIX + _manifest[key];

            if (_effectVisualizationFactoryLoader == null) throw new Exception("EffectVisualizationFactory loader not registered");

            T effectVisualizationFactory = (T)_effectVisualizationFactoryLoader.Load(filePath);

            return effectVisualizationFactory;
        }

        public void RegisterModifierFactoryLoader(IBasicAssetLoader loader) { _modifierFactoryLoader = loader; }

        public T GetModifierFactory<T>(string key) where T : IDisposable
        {
            if (!_manifest.ContainsKey(key)) return default(T);

            string filePath = FILEPREFIX + _manifest[key];

            if (_modifierFactoryLoader == null) throw new Exception("ModifierFactory loader not registered");

            T modifierFactory = (T)_modifierFactoryLoader.Load(filePath);

            return modifierFactory;
        }

        public void RegisterAnimationLoader(IBasicAssetLoader loader) { _animationLoader = loader; }

        public IAssetWrapper<T> GetAnimation<T>(string key) where T : IDisposable
        {
            if (!_manifest.ContainsKey(key)) return null;

            string filePath = FILEPREFIX + _manifest[key];

            if (_animationLoader == null) throw new Exception("Animation data loader not registered");

            T animationData = (T)_animationLoader.Load(filePath);

            _animationDataCache.CacheAsset(key, animationData);

            return _animationDataCache.GetAsset<T>(key);
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
            SoulSmithWeightedList<T> list = JsonSerializer.Deserialize<SoulSmithWeightedList<T>>(fileString, optionsWithWeightedListConverter);

            return list;
        }

        public static AssetManager Instance { get; private set; }
    }
}
