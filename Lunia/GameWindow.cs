using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Lunia.Networking;
using Lunia.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lunia
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameWindow : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private List<IScene> IScenes;

        public TCPClient NetworkClient { get; set; }

        private IScene activeIScene;

        #region Statics
        public static GameWindow Instance;

        public static void SwitchIScene(Type type)
        {
            IScene IScene = Instance.IScenes.FirstOrDefault(x => x.GetType() == type);

            if (IScene == null) return;

            Instance.activeIScene?.UnloadContent();
            Instance.activeIScene = IScene;
            Instance.activeIScene.Initialize();
            Instance.activeIScene.LoadContent(Instance.Content);
        }
        #endregion


        public GameWindow()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IScenes = new List<IScene>();
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            NetworkClient = new TCPClient("192.168.244.1", 12340);
            

            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Set window size
            //graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width/2;
            // graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height /2 ;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            //Register IScenes
            IScenes.Add(new LoginScene());

            //Set first IScene
            SwitchIScene(typeof(LoginScene));
        }


        protected override void UnloadContent()
        {
            //Unload content of the IScenes
            IScenes.ForEach(x => x.UnloadContent());
        }


        protected override void Update(GameTime gameTime)
        {
            //Update all IScenes
            activeIScene?.Update(gameTime, Mouse.GetState(), Keyboard.GetState());

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            //Draw
            activeIScene?.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
