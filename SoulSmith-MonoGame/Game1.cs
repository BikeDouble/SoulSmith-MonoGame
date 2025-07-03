using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Collections.Generic;
using SoulSmith.Input;
using SoulSmith.Core;
using SoulSmith.Drawing;
using SoulSmith.Game;
using SoulSmith.Object;
using Microsoft.Xna.Framework.Content;
using SoulSmith.Asset;
using System.Text.Json;
using System.IO;
using SoulSmith.Templates;
using SoulSmith.Battle.Move;

namespace SoulSmith_MonoGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SoulSmithObject _root;
        private RenderQueue _renderQueue;
        private InputQueue _inputQueue;
        private AssetManager _assetManager;

        public static string ASSETMANIFESTPATH = "../../../Assets/assetManifest.json";

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = SoulSmith.Drawing.Window.WINDOWHEIGHT;
            _graphics.PreferredBackBufferWidth = SoulSmith.Drawing.Window.WINDOWLENGTH;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _renderQueue = new RenderQueue();
            _inputQueue = new InputQueue();
        }

        protected override void Initialize()
        {
            SetTrace("debug.log");
            InitializeResources(Content, GraphicsDevice);
            _root = new GameManager();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        private void InitializeResources(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _assetManager = new(content, graphicsDevice, JsonSerializer.Deserialize<AssetManifest>(File.ReadAllText(ASSETMANIFESTPATH)));
            _assetManager.RegisterUnitTemplateLoader(new UnitTemplateLoader());
            _assetManager.RegisterTextureLoader(new DrawableResource_Texture2DLoader());
            _assetManager.RegisterZonedTextureLoader(new ZonedResourceLoader());
            _assetManager.RegisterMoveLoader(new MoveLoader());
        }

        protected override void Update(GameTime gameTime)
        {
            ProcessInputs();

            _root.Process(gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        private void ProcessInputs()
        {
            _root.CollectInputPackets(new Position(), _inputQueue);

            _inputQueue.Process(GetCurrentInputs());

            _inputQueue.Clear();
        }

        private List<InputType> GetCurrentInputs()
        {
            List<InputType> inputs = new List<InputType> { InputType.MouseHover };
            
            if (MouseFunctions.IsMouseLeftPressed()) inputs.Add(InputType.MouseLeft);

            if (MouseFunctions.IsMouseRightPressed()) inputs.Add(InputType.MouseRight);

            return inputs;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            _root.CollectDrawPackets(new Position(0, 0, 1, 1, 0), new Vector4(255, 255, 255, 255), _renderQueue);

            _renderQueue.Draw(_spriteBatch, _graphics.GraphicsDevice);

            _renderQueue.Clear();

            base.Draw(gameTime);
        }

        static void SetTrace(string fileName)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(fileName));
            Trace.AutoFlush = true;
        }
    }
}
