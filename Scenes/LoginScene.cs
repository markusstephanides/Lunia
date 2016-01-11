using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Lunia.Networking;
using Lunia.UI;
using LuniaAssembly;
using LuniaAssembly.Packet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lunia.Scenes
{
    public class LoginScene : IScene
    {
       
        private Texture2D background;

      
        private TextBox usernameTextBox;
        private TextBox passwordTextBox;
        private MessageBox loginStatusBox;

        private Button button;

        private bool authenticating;

        private Texture2D testTexture;


        #region Fonts
        private SpriteFont arial30;
        private SpriteFont arial16Bold;
        private SpriteFont arial15;
        private SpriteFont segoeUI18;
        #endregion


        public void Dispose()
        {

        }

        public void Initialize()
        {

        }

        public void LoadContent(ContentManager contentManager)
        {
            //Load fonts
            arial30 = contentManager.Load<SpriteFont>("Fonts/Arial_30");
            arial15 = contentManager.Load<SpriteFont>("Fonts/Arial_15");
            arial16Bold = contentManager.Load<SpriteFont>("Fonts/Arial_16_Bold");
            segoeUI18 = contentManager.Load<SpriteFont>("Fonts/SegoeUI_18");

            testTexture = contentManager.Load<Texture2D>("Textures/UI/MessageBox");

            background = contentManager.Load<Texture2D>("Textures/Login/Background");

            //Load controls
            usernameTextBox = new TextBox(contentManager.Load<Texture2D>("Textures/UI/TextBox"),arial15 , "", new Rectangle(
                (int)((GameWindow.Instance.GraphicsDevice.Viewport.Width - 180) * 0.5), (int)(GameWindow.Instance.GraphicsDevice.Viewport.Height * 0.5) - 15, 180, 34)
                );

            passwordTextBox = new TextBox(contentManager.Load<Texture2D>("Textures/UI/TextBox"), arial15, "", new Rectangle(
                (int)((GameWindow.Instance.GraphicsDevice.Viewport.Width - 180) * 0.5), (int)(GameWindow.Instance.GraphicsDevice.Viewport.Height * 0.5) + 55, 180, 34)
                );

            loginStatusBox = new MessageBox(contentManager.Load<Texture2D>("Textures/UI/MessageBox"), new Vector2((
                GameWindow.Instance.GraphicsDevice.Viewport.Width - 320) * 0.5f, (GameWindow.Instance.GraphicsDevice.Viewport.Height - 80) * 0.5f), new Vector2(320, 80), "Hallo", segoeUI18);
            loginStatusBox.TextColor = Color.White;

            button = new Button("Cancel", new Vector2(10, 10), new Vector2(120, 30), arial16Bold,
                contentManager.Load<Texture2D>("Textures/UI/BlueButtonIdle"),
                contentManager.Load<Texture2D>("Textures/UI/BlueButtonHover"),
                contentManager.Load<Texture2D>("Textures/UI/BlueButtonClicked"));

            button.Boundries = new Rectangle(22, 5, 80, (int) button.Size.Y - 10);

            loginStatusBox.AddButton(button);

            button.OnClick += (sender, args) => Debug.WriteLine("Hallo!");

            usernameTextBox.Color = Color.White;
            usernameTextBox.OnEnter = ProcessLogin;

            passwordTextBox.Color = Color.White;
            passwordTextBox.OnEnter = ProcessLogin;
            passwordTextBox.PasswordBox = true;
        }

        private void ProcessLogin()
        {
            if (authenticating) return;

            authenticating = true;

            //Disable all textboxes
            usernameTextBox.Enabled = false;
            passwordTextBox.Enabled = false;

            loginStatusBox.Text = "Connecting";
            loginStatusBox.Active = true;

            Task.Factory.StartNew(() =>
            {
                GameWindow.Instance.NetworkClient.StartClient(() =>
                {
                    loginStatusBox.Text = "Authenticating";
                    GameWindow.Instance.NetworkClient.Send(new LCAuthentication(GameWindow.Instance.NetworkClient.NetworkSalt, usernameTextBox.Text, Encrypter.GetHashed(passwordTextBox.RealText)));

                    TCPClient.PacketReceived += onPacketReceived;
                });
            });
        }

        private void onPacketReceived(object sender, PacketReceivedArgs packetReceivedArgs)
        {
            IPacket packet = packetReceivedArgs.Packet;

            if (packet is LCAuthenticationResponse)
            {
                LCAuthenticationResponse response = (LCAuthenticationResponse)packet;
                switch (response.Result)
                {
                    case AuthenticationResult.BAD_USERNAME:
                        loginStatusBox.Text = "Username not found!";
                        authenticating = false;
                        break;
                    case AuthenticationResult.BAD_PASSWORD:
                        loginStatusBox.Text = "Password not correct!";
                        authenticating = false;
                        break;
                    case AuthenticationResult.SUCCESS:
                        loginStatusBox.Text = "Success!";
                        break;
                }
            }
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime, MouseState mouseState, KeyboardState keyboardState)
        {
            usernameTextBox.Update(gameTime, mouseState, keyboardState);
            passwordTextBox.Update(gameTime, mouseState, keyboardState);
            loginStatusBox.Update(gameTime, mouseState, keyboardState);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Background
            spriteBatch.Draw(background, new Rectangle(0, 0, GameWindow.Instance.GraphicsDevice.Viewport.Width, GameWindow.Instance.GraphicsDevice.Viewport.Width), Color.White);

            //Username
            spriteBatch.DrawString(segoeUI18, "Username", new Vector2((GameWindow.Instance.GraphicsDevice.Viewport.Width - segoeUI18.MeasureString("Username").X) * 0.5f, (int)(GameWindow.Instance.GraphicsDevice.Viewport.Height * 0.5) - 50), Color.Black);
            usernameTextBox.Draw(spriteBatch);

            //Password
            spriteBatch.DrawString(segoeUI18, "Password", new Vector2((GameWindow.Instance.GraphicsDevice.Viewport.Width - segoeUI18.MeasureString("Password").X) * 0.5f, (int)(GameWindow.Instance.GraphicsDevice.Viewport.Height * 0.5) + 20), Color.Black);
            passwordTextBox.Draw(spriteBatch);

            //MessageBox
            loginStatusBox.Draw(spriteBatch);

        }
    }
}