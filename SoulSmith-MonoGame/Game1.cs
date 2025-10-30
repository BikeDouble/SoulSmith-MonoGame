using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Collections.Generic;
using SoulSmith.Input;
using SoulSmith.Core;
using SoulSmith.Drawing;
using SoulSmith.Runs;
using SoulSmith.Object;
using Microsoft.Xna.Framework.Content;
using SoulSmith.Asset;
using System.Text.Json;
using System.IO;
using SoulSmith.Templates;
using SoulSmith.Battle.Moves;
using SoulSmith.Drawing.Animation;
using SoulSmith.Drawing.Text;
using SoulSmith.Battle.Emotions;
using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.Battle.Modifiers;
using SoulSmith.Drawing.Textures;
using SoulSmith.Drawing.Zoned;
using SoulSmith.Localization;

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
        private LocalizationManager _localizationManager;

        public static string ASSETMANIFESTPATH = "../../../Assets/assetManifest.json";

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            //_graphics.PreparingDeviceSettings += OnPrepareGraphicsDeviceSettings;
            _graphics.PreferredBackBufferHeight = SoulSmith.Drawing.Window.WINDOWHEIGHT;
            _graphics.PreferredBackBufferWidth = SoulSmith.Drawing.Window.WINDOWWIDTH;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _renderQueue = new RenderQueue();
            _inputQueue = new InputQueue();
        }

        //private void OnPrepareGraphicsDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        //{
        //    //_graphics.PreferMultiSampling = true;
        //    //e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 8;
        //}

        protected override void Initialize()
        {
            SetTrace("debug.log");
            InitializeLocalization();
            InitializeResources(Content, GraphicsDevice);
            _root = new Run();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        private void InitializeResources(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _assetManager = new(content, graphicsDevice, JsonSerializer.Deserialize<KeyPathManifest>(File.ReadAllText(ASSETMANIFESTPATH)));
            _assetManager.RegisterUnitTemplateLoader(new UnitTemplateLoader());
            _assetManager.RegisterSoulSmithTextureLoader(new SoulSmithTextureLoader());
            _assetManager.RegisterZonedTextureLoader(new ZonedResourceLoader());
            _assetManager.RegisterMoveLoader(new MoveLoader());
            _assetManager.RegisterAnimationLoader(new AnimationLoader());
            _assetManager.RegisterFontResourceLoader(new FontResourceLoader());
            _assetManager.RegisterEmotionLoader(new EmotionLoader());
            _assetManager.RegisterEffectVisualizationFactoryLoader(new EffectVisualizationFactoryLoader());
            _assetManager.RegisterModifierFactoryLoader(new ModifierFactoryLoader());
            _assetManager.RegisterTextBoxLoader(new TextBoxLoader());
        }

        private void InitializeLocalization()
        {
            KeyPathManifest localizationManifest = JsonSerializer.Deserialize<KeyPathManifest>(File.ReadAllText("../../../Assets/localizations/english.json"));
            _localizationManager = new LocalizationManager(localizationManifest);
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

            _inputQueue.Process(InputManager.GetCurrentInputs(), new List<InputType>());

            _inputQueue.Clear();
        }



        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            _root.CollectDrawPackets(new Position(0, 0, 1, 1, 0), Color.White, _renderQueue);

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
