using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lunia.Scenes
{
    public interface IScene : IDisposable
    {
        void Initialize();
        void LoadContent(ContentManager contentManager);
        void UnloadContent();
        void Update(GameTime gameTime, MouseState mouseState, KeyboardState keyboardState);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}