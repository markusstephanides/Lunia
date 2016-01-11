using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lunia.UI
{
    public class Button : UIElement
    {
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public Texture2D IdleTexture { get; set; }
        public Texture2D HoverTexture { get; set; }
        public Texture2D ClickedTexture { get; set; }
        public Rectangle Boundries { get; set; }
        public Color Color { get; set; } = Color.White;

        private UIButtonState state;
        private Rectangle drawArea;

        private MouseState oldState;

        public event EventHandler OnClick;

        public Button(string text, Vector2 position, Vector2 size, SpriteFont font, Texture2D idleTexture, Texture2D hoverTexture, Texture2D clickedTexture)
        {
            Text = text;
            Font = font;
            IdleTexture = idleTexture;
            HoverTexture = hoverTexture;
            ClickedTexture = clickedTexture;
            Position = position;
            Size = size;

            PositionChanged += (sender, args) => setDrawingArea();
            SizeChanged += (sender, args) => setDrawingArea();
        }


        private void setDrawingArea()
        {
            drawArea = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            //TODO Disabled state and texture

            //Background
            switch (state)
            {
                case UIButtonState.IDLE:
                    spriteBatch.Draw(IdleTexture, drawArea, Color.White);
                    break;
                case UIButtonState.HOVER:
                    spriteBatch.Draw(HoverTexture, drawArea, Color.White);
                    break;
                case UIButtonState.CLICKED:
                    spriteBatch.Draw(ClickedTexture, drawArea, Color.White);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Text
            Vector2 pos = new Vector2(Position.X + (float) ((Size.X - Font.MeasureString(Text).X)*0.5),
                Position.Y + (float)((Size.Y - Font.MeasureString(Text).Y) * 0.5));
            if(Boundries == null) spriteBatch.DrawString(Font, Text, pos , Color);
            else Utility.DrawString(spriteBatch, Font, Text, new Rectangle(drawArea.X + Boundries.X, drawArea.Y + Boundries.Y, Boundries.Width, Boundries.Height));
        }

        public override void Update(GameTime gameTime, MouseState mouseState, KeyboardState keyboardState)
        {
            if(!Enabled) return;

            if (drawArea.Contains(mouseState.Position))
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                    state = UIButtonState.CLICKED;

                if (mouseState.LeftButton == ButtonState.Pressed && oldState.LeftButton != ButtonState.Pressed)
                    OnClick?.Invoke(this, null);
                else if(mouseState.LeftButton != ButtonState.Pressed)
                    state = UIButtonState.HOVER;
              
            }
            else state = UIButtonState.IDLE;

            oldState = mouseState;
        }
    }
}