#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DeModulate
{
    class ControlsScreen : GameScreen
    {
        #region Fields

        string message;
        Texture2D background;

        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Accepted;

        #endregion

        #region Initialization

        public ControlsScreen()
        {
            const string usageText = "Go Back";
            this.message = message + usageText;
            IsPopup = true;
            TransitionOnTime = TimeSpan.FromSeconds(0.4);
            TransitionOffTime = TimeSpan.FromSeconds(0.4);
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            background = content.Load<Texture2D>("instructionsBG");
        }


        #endregion

        #region Handle Input

        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;

            if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                if (Accepted != null)
                    Accepted(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
        }


        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
            Color color = Color.Red;
            Vector2 position = new Vector2(80, 70);
            position.X -= transitionOffset * 256;
            Vector2 titleOrigin = font.MeasureString(message) / 2;
            color = new Color(color.R, color.G, color.B, TransitionAlpha);
            float scale = 1;

            position.Y -= transitionOffset * 100;



            spriteBatch.Begin();
            spriteBatch.Draw(background,new Vector2(0,0),Color.White);
            spriteBatch.DrawString(font, message, position, color, 0,
                               titleOrigin, scale, SpriteEffects.None, 0);
            spriteBatch.End();
        }


        #endregion
    }
}
