using System;
using System.Collections.Generic;
using System.Diagnostics;
using LuniaAssembly;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lunia.UI
{
    public class MessageBox : UIElement
    {
        public Texture2D Background { get; set; }
        public string Text { get; set; }
        public Color TextColor { get; set; } = Color.White;
        public SpriteFont Font { get; set; }

        private Rectangle drawingArea;

        private List<Button> buttons;

        public MessageBox(Texture2D background, Vector2 position, Vector2 size, string text, SpriteFont font)
        {
            Background = background;
            Text = text;
            Position = position;
            Size = size;
            Font = font;

            setDrawingArea();

            buttons = new List<Button>();

            PositionChanged += (sender, args) => setDrawingArea();
            SizeChanged += (sender, args) => setDrawingArea();

        }
       

        private void setDrawingArea()
        {
            drawingArea = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active) return;

            //Background
            spriteBatch.Draw(Background, drawingArea, Color.White);

            Vector2 textPos = new Vector2(Position.X + (float)((Size.X - Font.MeasureString(Text).X) * 0.5), Position.Y +
                (float)((Size.Y - Font.MeasureString(Text).Y) * 0.5));

            //Text
            spriteBatch.DrawString(Font, Text, textPos, TextColor);

            //Buttons
            for (int i = 0; i < buttons.Count; i++)
            {
                Button button = buttons[i];

                float buttonX = Position.X + (Size.X / buttons.Count) * i;
                Vector2 pos = new Vector2(buttonX, Position.Y + Size.Y - button.Size.Y);
                button.Position = pos;

                //TODO ReEnable
               // button.Draw(spriteBatch);
            }
        }

        public void AddButton(Button button)
        {
            if (buttons.Count == Constants.MaxButtonsInMessageBox)
                throw new Exception(
                    $"Too many buttons in the messagebox! (Maximum is {Constants.MaxButtonsInMessageBox})");

            buttons.Add(button);
        }

        public override void Update(GameTime gameTime, MouseState mouseState, KeyboardState keyboardState)
        {
            foreach (var button in buttons)
            {
                button.Update(gameTime, mouseState, keyboardState);
            }
        }
    }
}