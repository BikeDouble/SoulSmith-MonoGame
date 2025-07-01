using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using SoulSmith.Drawing;
using SoulSmith.Shapes;
using System.Text.Json;
using SoulSmith.Templates;
using SoulSmith.Collections;
using System.IO;

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
        private Cache<DrawableResource_Texture2D> _textureCache;
        private Cache<ZonedResource> _zonedTextureCache;
        private Cache<UnitTemplate> _unitTemplateCache;

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
            _textureCache = new Cache<DrawableResource_Texture2D>();
            _zonedTextureCache = new Cache<ZonedResource>();
            _unitTemplateCache = new Cache<UnitTemplate>();

        }

        public IReadOnlyTrackedAsset<UnitTemplate> GetUnitTemplate(string key)
        {
            if (_unitTemplateCache.Contains(key)) return _unitTemplateCache.GetAsset(key);

            if (!_manifest.ContainsKey(key)) return null;

            string filePath = FILEPREFIX + _manifest[key];

            if (!File.Exists(filePath)) return null;

            string fileText = File.ReadAllText(filePath);

            UnitTemplate template = JsonSerializer.Deserialize<UnitTemplate>(fileText);

            if (template == null) return null;

            _unitTemplateCache.CacheAsset(key, template);

            return _unitTemplateCache.GetAsset(key);

        }

        public IReadOnlyTrackedAsset<DrawableResource_Texture2D> GetTexture2D(string key)
        {
            if (_textureCache.Contains(key)) return _textureCache.GetAsset(key);

            if (!_manifest.ContainsKey(key)) return null;

            string filepath = FILEPREFIX + _manifest[key];

            if (!File.Exists(filepath)) return null;

            Texture2D texture = Texture2D.FromFile(_graphics, filepath);

            if (texture == null) return null;

            _textureCache.CacheAsset(key, new DrawableResource_Texture2D(texture));

            return _textureCache.GetAsset(key);
        }

        public IReadOnlyTrackedAsset<ZonedResource> GetZonedTexture2D(string key)
        {
            if (_zonedTextureCache.Contains(key)) return _zonedTextureCache.GetAsset(key);

            if (!_manifest.ContainsKey(key)) return null;

            string textureFilepath = FILEPREFIX + _manifest[key];

            if (!File.Exists(textureFilepath)) return null;

            Texture2D texture = Texture2D.FromFile(_graphics, textureFilepath);

            if (texture == null) return null;

            string metaFilepath = GetMetaFilepathFromFilepath(textureFilepath, "json"); //TODO make more elegant

            if (!File.Exists(metaFilepath)) return null;

            IZone zone = JsonSerializer.Deserialize<IZone>(File.ReadAllText(metaFilepath));

            if (zone == null) return null;

            ZonedResource resource = new ZonedResource(zone, new DrawableResource_Texture2D(texture));

            _zonedTextureCache.CacheAsset(key, resource);

            return _zonedTextureCache.GetAsset(key);
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

        public 

        public static AssetManager Instance { get; private set; }
    }
}
