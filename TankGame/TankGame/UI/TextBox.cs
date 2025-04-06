using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Input;

namespace TankGame.UI
{
    public class TextBox : CanvasObject
    {
        public string Content { get; set; }
        public int MaxLength { get; set; }
        public bool Selected { get; set; }
        public override Vector2 Dimensions => font.MeasureString(Content);

        public Action<string> OnConfirm { get; set; }

        #region CURSOR

        private bool HasBackground => backgroundTexture!= null;

        private Texture2D backgroundTexture;
        private int animationTime;
        private Vector2 cursorPos;
        private Texture2D cursorTexture;
        Vector2 cursorDims;
        Vector2 buttonDims;
        public Color ButtonColor { get; set; } = Color.DimGray;
        public Rectangle ButtonRect { get => new Rectangle((int)Position.X, (int)Position.Y, (int)(buttonDims.X), (int)(buttonDims.Y)); }
        private Vector2 ButtonOrigin => GetCenter(Allignment, ButtonRect.Size.ToVector2());

        public float HoverSize { get; set; } = 1;
        #endregion

        public bool numericsOnly { get; set; } = false;
        private SpriteFont font;

        public TextBox(int _maxLength, Vector2 _pos, Vector2 _size, Color _color, float _scale = 1)
        {
            MaxLength = _maxLength;
            Position = _pos;
            Visible = true;
            Color = _color;
            Scale = _scale;
            font = Global.basicFont;
            animationTime = 0;
            cursorDims = new Vector2(6, 24) * Scale;
            buttonDims = _size * Scale;
            Content = string.Empty;

            goalSize = Scale + 0.2f;

            cursorTexture = ContentLoader.LoadTexture("Content/Textures/UI/FlashingCursor.png");
            backgroundTexture = ContentLoader.LoadTexture("Content/Textures/UI/Square.png");

            Main.instance.Window.TextInput += GetKeyboardInput;
        }

        public override void OnDestroyed()
        {
            Main.instance.Window.TextInput -= GetKeyboardInput;
        }

        void GetKeyboardInput(object sender, TextInputEventArgs e)
        {
            AppendText(e.Character);
        }

        private bool IsFlashingCursorVisible()
        {
            int time = animationTime % 60;

            if (time >= 0 && time < 31) return true;
            return false;
        }

        public void AppendText(char _text)
        {
            if (!Selected) return;
            Vector2 spacing = new Vector2();
            var keyboardState = MyKeyboard.CurrentState;
            bool lowerThisCharacter = true;
            if (keyboardState.CapsLock || keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
            {
                lowerThisCharacter = false;
            }

            if (numericsOnly)
            {
                if (char.IsLetter(_text)) return;
            }
            //if (numericsOnly && (int)Char.GetNumericValue(_text) < 0 || (int)Char.GetNumericValue(_text) > 9)
            //{
            //    if (_text != '\b') return;
            //}

            if (_text != '\b')
            {
                if (_text == '\r') ConfirmText();
                if (Content.Length < MaxLength)
                {
                    if (lowerThisCharacter) _text = Char.ToLower(_text);

                    if (!font.Characters.Contains(_text) | _text == '\r' | _text == '\n') return;
                    Content += (_text);
                    spacing = font.MeasureString(Content) * Scale;
                    cursorPos = new Vector2(spacing.X, 0);

                }
            }
            else
            {
                if (Content.Length > 0)
                {
                    Content = Content.Remove((Content.Length - 1), 1);
                    spacing = font.MeasureString(Content) * Scale;
                    cursorPos = new Vector2(spacing.X, 0);
                }
            }
        }

        private void ConfirmText()
        {
            Selected = false;
            OnConfirm?.Invoke(Content);
        }

        public void Reset()
        {
            Content = string.Empty;
            cursorPos = Vector2.Zero;
        }

        public virtual bool Click()
        {

            if (MyMouse.LeftButtonDown())
            {
                if (Hover())
                {
                    Vector2 spacing = font.MeasureString(Content)*Scale;
                    cursorPos = new Vector2(spacing.X, 0);
                    Selected = true;
                    return true;
                }
                Selected = false;
                return false;
            }
            return false;
        }

        float goalSize = 0;
        public override bool Hover()
        {
            if (ButtonRect.Contains(MyMouse.MouseWorldPosition))
            {
                return true;
            }
            return false;
        }

        public override void Update()
        {
            animationTime++;
            Click();
        }

        public override void Draw()
        {
            if (Visible)
            {
                if (HasBackground) Global.SpriteBatch.Draw(backgroundTexture, ButtonRect, new Rectangle(0, 0, 1, 1), ButtonColor, Rotation, ButtonOrigin, SpriteEffects.None, LayerDepth);
                Global.SpriteBatch.DrawString(font, Content, Position, Color, Rotation, ButtonOrigin, Scale, SpriteEffects.None, LayerDepth + 0.05f);
                if (IsFlashingCursorVisible() & Selected)
                {
                    Rectangle sourceRect = new Rectangle(0, 0, (int)cursorDims.X, (int)cursorDims.Y);
                    Rectangle destRect = new Rectangle(((int)Position.X + (int)cursorPos.X), (int)Position.Y + (int)((cursorDims.Y / 2)), (int)cursorDims.X, (int)cursorDims.Y);

                    Global.SpriteBatch.Draw(cursorTexture, destRect, sourceRect, Color.White, Rotation, ButtonOrigin, SpriteEffects.None, LayerDepth+0.05f);
                }
            }
        }
    }
}
