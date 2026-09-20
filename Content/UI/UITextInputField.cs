using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace BossLoadouts.UI
{
    public class UITextInputField : UIPanel
    {
        public string Text = "";
        public bool Focused = false;
        private int _cursorTimer;
        private bool _cursorVisible;
        public string PlaceholderText = "";
        private Texture2D _pixelTexture;
        private float _textScale = 1f;
        private bool _justFocused = false;
        private bool[] _keyProcessed = new bool[256];
        private float _lastTextWidth = 0f;
        private DynamicSpriteFont _font;

        public UITextInputField()
        {
            SetPadding(6f);
            BackgroundColor = new Color(30, 30, 60);
            BorderColor = new Color(80, 80, 120);

            _font = FontAssets.MouseText.Value;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Focused)
            {
                if (_justFocused)
                {
                    Main.clrInput();
                    Main.blockInput = true;
                    _justFocused = false;
                    ResetKeyStates();
                }

                _cursorTimer++;
                if (_cursorTimer > 20)
                {
                    _cursorVisible = !_cursorVisible;
                    _cursorTimer = 0;
                }

                HandleTextInput();

                if (Main.keyState.IsKeyDown(Keys.Escape))
                {
                    Unfocus();
                }
            }
            else
            {
                ResetKeyStates();
                Main.drawingPlayerChat = false;
            }
        }

        private void ResetKeyStates()
        {
            Array.Fill(_keyProcessed, false);
        }

        private void HandleTextInput()
        {
            if (!Main.hasFocus || !Focused)
                return;

            Main.drawingPlayerChat = true;

            Text = Main.GetInputText(Text);

            _lastTextWidth = CalculateTextWidth(Text);

            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Enter) && !_keyProcessed[(int)Keys.Enter])
            {
                Unfocus();
                _keyProcessed[(int)Keys.Enter] = true;
            }
        }


        private float CalculateTextWidth(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0f;

            return _font.MeasureString(text).X * _textScale;
        }

        private void Unfocus()
        {
            Focused = false;
            Main.blockInput = false;
            Main.drawingPlayerChat = false;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            var innerDimensions = GetInnerDimensions();
            var pos = innerDimensions.Position() + new Vector2(4f, 4f);

            if (_pixelTexture == null)
            {
                _pixelTexture = new Texture2D(Main.graphics.GraphicsDevice, 1, 1);
                _pixelTexture.SetData(new[] { Color.White });
            }

            if (string.IsNullOrEmpty(Text) && !Focused && !string.IsNullOrEmpty(PlaceholderText))
            {
                Utils.DrawBorderString(spriteBatch, PlaceholderText, pos, Color.Gray, _textScale);
                return;
            }

            Utils.DrawBorderString(spriteBatch, Text, pos, Color.White, _textScale);

            if (Focused && _cursorVisible)
            {
                float textHeight = _font.MeasureString("A").Y * _textScale;
                spriteBatch.Draw(
                    _pixelTexture,
                    new Rectangle((int)(pos.X + _lastTextWidth + 2), (int)pos.Y, 2, (int)textHeight),
                    Color.White
                );
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            if (!Focused)
            {
                Focused = true;
                _justFocused = true;
                _cursorVisible = true;
                _cursorTimer = 0;
                _lastTextWidth = CalculateTextWidth(Text);
            }
        }

        public override void OnDeactivate()
        {
            Unfocus();
            Main.drawingPlayerChat = false;
            base.OnDeactivate();
            _pixelTexture?.Dispose();
        }
    }
}