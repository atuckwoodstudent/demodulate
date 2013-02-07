#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DeModulate
{
    class MenuEntry
    {
        #region Fields

       
        string text;

        float selectionFade;

        #endregion

        #region Properties

        public string Text
        {
            get { return text; }
            set { text = value; }
        }


        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Selected;

        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }


        #endregion

        #region Initialization

        public MenuEntry(string text)
        {
            this.text = text;
        }


        #endregion

        #region Update and Draw

        public virtual void Update(MenuScreen screen, bool isSelected,
                                                      GameTime gameTime)
        {
  
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }

        public virtual void Draw(MenuScreen screen, Vector2 position,
                                 bool isSelected, GameTime gameTime)
        {
            Color color = isSelected ? Color.Red : Color.Black;
                      
            float scale = 1;

            color = new Color(color.R, color.G, color.B, screen.TransitionAlpha);
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.DrawString(font, text, position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
        }

        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.ScreenManager.Font.LineSpacing;
        }


        #endregion
    }
}
