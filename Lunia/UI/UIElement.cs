using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lunia.UI
{
    public abstract class UIElement
    {
        public event EventHandler PositionChanged;
        public event EventHandler SizeChanged;

        private Vector2 position;
        private Vector2 size;

        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                PositionChanged?.Invoke(this, null);
            }
        }

        public Vector2 Size
        {
            get { return size; }
            set
            {
                size = value;
                SizeChanged?.Invoke(this, null);
            }
        }

  
        public bool Active { get; set; }
        public bool Enabled { get; set; } = true;

        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Update(GameTime gameTime, MouseState mouseState, KeyboardState keyboardState);
    }
}