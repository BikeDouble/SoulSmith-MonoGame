using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Shapes;
using MonoGame.Extended;
using System.Diagnostics;
using System;
using System.Collections.Generic;

namespace SoulSmith_MonoGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private FormsObject _root;

        public static int WINDOWHEIGHT = 900;
        public static int WINDOWLENGTH = 1600;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = WINDOWHEIGHT;
            _graphics.PreferredBackBufferWidth = WINDOWLENGTH;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            SetTrace("debug.log");
            _root = new GameManager(Content);

            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _root.Process(gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            _spriteBatch.Begin();
            _root.Draw(new Position(0, 0, 1, 1, 0), _spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        static void SetTrace(string fileName)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(fileName));
            Trace.AutoFlush = true;
        }
    }
}

/*
TODO DELETE OLD REFERENCE CODE

using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

static class Program
{
    static void Main(string[] args)
    {

        while (!stop)
        {
            stopwatch.Start();
            
            stopwatch.Stop();
            deltaTime = stopwatch.Elapsed.TotalSeconds;
        }

    }

    static void PerformGameLoop(FormsObject root, double deltaTime)
    {
        root.Process(deltaTime);
    }

    
}

*/