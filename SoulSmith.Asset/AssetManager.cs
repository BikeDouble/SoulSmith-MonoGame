using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Asset
{ 
    /// <summary>
    /// Class to load and retrieve assets.
    /// </summary>
    public class AssetManager
    {
        private Dictionary<string, string> _manifest;
        private Cache<Texture2D> _textureCache;
        private ContentManager _content;

        public static void Initialize(ContentManager content, Dictionary<string, string> manifest)
        {
            Instance = new AssetManager(content, manifest);
        }

        public AssetManager(ContentManager content, Dictionary<string, string> manifest)
        {
            if (Instance == null) { Instance = this; }

            _content = content;
            _manifest = manifest;
            _textureCache = new Cache<Texture2D>();
        }

        public TrackedAsset<Texture2D> GetTexture2D(string key)
        {
            if (_textureCache.Contains(key)) return _textureCache.GetAsset(key);

            Texture2D texture = _content.Load<Texture2D>(_manifest[key]);

            if (texture == null) return null;

            _textureCache.CacheAsset(key, texture);

            return _textureCache.GetAsset(key);
        }

        public static AssetManager Instance { get; private set; }
    }
}
