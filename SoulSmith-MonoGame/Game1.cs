using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Shapes;
using MonoGame.Extended;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using SoulSmithObjects;
using SoulSmithInput;
using SoulSmith.Core;
using SoulSmith.Drawing;
using Microsoft.Xna.Framework.Content;

namespace SoulSmith_MonoGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SoulSmithObject _root;
        private RenderQueue _renderQueue;
        private InputQueue _inputQueue;
        private MasterAssetLoader _assetLoader;

        public static int WINDOWHEIGHT = 900;
        public static int WINDOWLENGTH = 1600;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = WINDOWHEIGHT;
            _graphics.PreferredBackBufferWidth = WINDOWLENGTH;
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
            _assetLoader = new(content, graphicsDevice);
            //_assetManager = new(content, manifest);
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

            _root.CollectDrawPackets(new Position(0, 0, 1, 1, 0), Vector4.Zero, _renderQueue);

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
