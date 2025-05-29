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

namespace SoulSmith.Asset
{ 
    /// <summary>
    /// Class to load and retrieve assets.
    /// </summary>
    public class AssetManager
    {
        private Dictionary<string, string> _manifest;
        private ContentManager _content;

        // Cached data
        private Cache<DrawableResource_Texture2D> _textureCache;
        private Cache<ZonedResource> _zonedTextureCache;
        private Cache<UnitTemplate> _unitTemplateCache;

        public static void Initialize(ContentManager content, AssetManifest manifest)
        {
            if (Instance == null) Instance = new AssetManager(content, manifest);
        }

        public AssetManager(ContentManager content, AssetManifest manifest)
        {
            if (Instance == null) { Instance = this; }

            _content = content;
            _manifest = manifest.Manifest;
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

            string filePath = _manifest[key];

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

            string filePath = _manifest[key];

            if (!File.Exists(filePath)) return null;

            Texture2D texture = _content.Load<Texture2D>(filePath);

            if (texture == null) return null;

            _textureCache.CacheAsset(key, new DrawableResource_Texture2D(texture));

            return _textureCache.GetAsset(key);
        }

        public IReadOnlyTrackedAsset<ZonedResource> GetZonedTexture2D(string key)
        {
            if (_zonedTextureCache.Contains(key)) return _zonedTextureCache.GetAsset(key);

            if (!_manifest.ContainsKey(key)) return null;

            string textureFilepath = _manifest[key];

            if (!File.Exists(textureFilepath)) return null;

            Texture2D texture = _content.Load<Texture2D>(textureFilepath);

            if (texture == null) return null;

            string metaFilepath = textureFilepath.Split('.')[0] + ".meta.json";

            if (!File.Exists(metaFilepath)) return null;

            IZone zone = JsonSerializer.Deserialize<IZone>(File.ReadAllText(metaFilepath));

            if (zone == null) return null;

            ZonedResource resource = new ZonedResource(zone, new DrawableResource_Texture2D(texture));

            _zonedTextureCache.CacheAsset(key, resource);

            return _zonedTextureCache.GetAsset(key);
        }

        public SoulSmithWeightedList<string> GetSpawnList(string key)
        {
            return GetWeightedList<string>(key);
        }

        public SoulSmithWeightedList<T> GetWeightedList<T>(string key)
        {
            if (!_manifest.ContainsKey(key)) return null;

            string filepath = _manifest[key];

            if (!File.Exists(filepath)) return null;

            string fileString = File.ReadAllText(filepath);

            SoulSmithWeightedList<T> list = JsonSerializer.Deserialize<SoulSmithWeightedList<T>>(filepath);

            return list;
        }

        public static AssetManager Instance { get; private set; }
    }
}
