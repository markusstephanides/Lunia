using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Lunia.Networking;
using Lunia.Scenes;
using LuniaAssembly.Packet;
using LuniaAssembly.States;
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
        private List<IScene> Scenes;

        public TCPClient NetworkClient { get; set; }

        private IScene activeIScene;

        #region Statics
        public static GameWindow Instance;

        public static void SwitchScene(Type type)
        {
            IScene IScene = Instance.Scenes.FirstOrDefault(x => x.GetType() == type);

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
            Scenes = new List<IScene>();
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            NetworkClient = new TCPClient("192.168.0.109", 12340);
            TCPClient.PacketReceived += TCPClient_PacketReceived;

            base.Initialize();
        }

        private void TCPClient_PacketReceived(object sender, PacketReceivedArgs e)
        {
            IPacket packet = e.Packet;

            if (packet is LCStateSwitch)
            {
                LCStateSwitch state = (LCStateSwitch)packet;

                switch (state.GameState)
                {
                    case GameState.LOGIN:
                        SwitchScene(typeof(LoginScene));
                        break;
                    case GameState.CHARACTER_SELECTION:
                        SwitchScene(typeof(CharacterSelectionScene));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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

            //Register Scenes
            Scenes.Add(new LoginScene());
            Scenes.Add(new CharacterSelectionScene());

            //Set first IScene
            SwitchScene(typeof (LoginScene));
        }


        protected override void UnloadContent()
        {
            //Unload content of the Scenes
            Scenes.ForEach(x => x.UnloadContent());
        }


        protected override void Update(GameTime gameTime)
        {
            //Update all Scenes
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
