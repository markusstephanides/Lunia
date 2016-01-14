using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lunia.UI
{
    public class TextBox : UIElement
    {
        private const int caretWidth = 1;
        private const int caretHeightSubstraction = 15;
        private const int textXAddition = 5;

        public string Text { get; set; }
        public Rectangle Rectangle { get; set; }
        public Color Color { get; set; } = Color.Black;
        public Action OnEnter { get; set; }
        public bool PasswordBox { get; set; }

        private Texture2D backgroundTexture;
        private Texture2D caretTexture;
        private SpriteFont font;

        private int caretPosition;
        private bool showCaret;
        private Stopwatch caretStopwatch;

        private KeyboardState oldKeyboardState;
        private MouseState oldMouseState;

        public string RealText { get; set; }

        private bool lockMaxTextLength;


        public TextBox(Texture2D backgroundTexture, SpriteFont font, string text, Rectangle rectangle)
        {
            this.backgroundTexture = backgroundTexture;
            this.font = font;
            this.Text = text;
            this.Rectangle = rectangle;
            this.caretStopwatch = new Stopwatch();
            this.caretStopwatch.Start();

            if (Text == null) Text = "";
            if (RealText == null) RealText = "";

            this.lockMaxTextLength = true;

            //Generate caret
            caretTexture = new Texture2D(GameWindow.Instance.GraphicsDevice, caretWidth, rectangle.Height - caretHeightSubstraction);

            Color[] data = new Color[caretWidth * (rectangle.Height - caretHeightSubstraction)];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
            caretTexture.SetData(data);

            //Register listener
            GameWindow.Instance.Window.TextInput += handleTextInput;
        }

        private void handleTextInput(object sender, TextInputEventArgs textInputEventArgs)
        {
            if (Active)
            {
                char pressedChar = textInputEventArgs.Character;
                // Debug.WriteLine(pressedChar);
                if (pressedChar == '\b')
                {
                    //BACKSPACE
                    if (caretPosition > 0)
                    {
                        RealText = RealText.Remove(caretPosition - 1, 1);
                        UpdateVisualText();
                        caretPosition--;
                    }
                }


                if (!char.IsLetterOrDigit(pressedChar) && !char.IsSymbol(pressedChar)) return;

                processInput(textInputEventArgs.Character);
            }
        }

        public void UpdateVisualText()
        {
            if (PasswordBox)
            {
                Text = "";

                for (int i = 0; i < RealText.Length; i++)
                {
                    Text += "*";
                }
            }
            else
            {
                Text = RealText;
            }
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            //Background
            spriteBatch.Draw(backgroundTexture, Rectangle, Color.White);

            //Draw text
            spriteBatch.DrawString(font, Text, new Vector2(Rectangle.X + textXAddition, (float)(Rectangle.Y + (Rectangle.Height * 0.25))), Color);

            //Draw caret
            if (showCaret && Active) spriteBatch.Draw(caretTexture, new Rectangle(getCaretX(), (int)(Rectangle.Y + (Rectangle.Height - (Rectangle.Height - caretHeightSubstraction)) * 0.5), caretWidth, Rectangle.Height - caretHeightSubstraction), Color.White);
        }

        private int getCaretX()
        {
            return (int)(Rectangle.X + textXAddition + font.MeasureString(Text.Substring(0, caretPosition)).X);
        }

        private void processInput(char charToInsert)
        {
            if (lockMaxTextLength && !(Rectangle.X + textXAddition + font.MeasureString(Text + charToInsert).X + textXAddition >
                                (Rectangle.X + Rectangle.Width)))
            {
                RealText = RealText.Insert(caretPosition, charToInsert.ToString());
                UpdateVisualText();
                caretPosition++;
            }
        }




        public override void Update(GameTime gameTime, MouseState mouseState, KeyboardState keyboardState)
        {
            if(!Enabled) return;

            if (Active)
            {
                if (caretStopwatch.ElapsedMilliseconds >= 500)
                {
                    showCaret = !showCaret;
                    caretStopwatch.Restart();
                }

                if (oldKeyboardState == null) oldKeyboardState = keyboardState;

                if (keyboardState.IsKeyDown(Keys.Right) && !oldKeyboardState.IsKeyDown(Keys.Right) && caretPosition < (Text.Length))
                {
                    caretPosition += 1;
                    caretStopwatch.Restart();
                    showCaret = true;
                }
                if (keyboardState.IsKeyDown(Keys.Left) && !oldKeyboardState.IsKeyDown(Keys.Left) && caretPosition > 0)
                {
                    caretPosition -= 1;
                    caretStopwatch.Restart();
                    showCaret = true;
                }


                foreach (var pressedKey in keyboardState.GetPressedKeys())
                {
                    if (oldKeyboardState.IsKeyUp(pressedKey))
                    {
                        if (pressedKey == Keys.Delete && Text.Length > 0 && caretPosition < Text.Length)
                        {
                            RealText = RealText.Remove(caretPosition, 1);
                            UpdateVisualText();
                        }
                        else if (pressedKey == Keys.Enter)
                        {
                            OnEnter?.Invoke();
                        }
                    }
                }

                oldKeyboardState = keyboardState;
            }

            if (oldMouseState == null) oldMouseState = mouseState;

            if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton != ButtonState.Pressed)
            {
                if (Rectangle.Contains(mouseState.Position))
                {
                    Active = true;

                    if (mouseState.X >= Rectangle.X + textXAddition)
                    {
                        for (int i = 0; i < Text.Length; i++)
                        {
                            var x = Rectangle.X + textXAddition + (int)font.MeasureString(Text.Substring(0, i)).X;
                            var width = (int)font.MeasureString(Text.Substring(i, 1)).X;

                            if (mouseState.X >= x && mouseState.X <= (x + width))
                            {
                                caretPosition = i;
                                caretStopwatch.Restart();
                                showCaret = true;
                                break;
                            }
                            else if (mouseState.X >= (Rectangle.X + textXAddition + font.MeasureString(Text).X))
                            {
                                caretPosition = Text.Length;
                                caretStopwatch.Restart();
                                showCaret = true;
                                break;
                            }
                        }
                    }
                }
                else Active = false;

            }


            oldMouseState = mouseState;

        }
    }
}
