#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Windows.Forms;
#endregion

namespace DeModulate
{
    public class DeModulateGame : Microsoft.Xna.Framework.Game
    {
        #region Fields
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        #endregion

        #region Initialization
        public DeModulateGame()
        {
            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            screenManager.AddScreen(new BackgroundScreen("FacelessBG"), null);
            screenManager.AddScreen(new SplashScreen(screenManager),null);
        }
        #endregion

        #region Draw
        
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }
        #endregion
    }

    #region Entry Point

    static class Program
    {
        static void Main()
        {
            try
            {
                using (DeModulateGame game = new DeModulateGame())
                {
                    game.Run();
                }
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
        }
    }
    #endregion
}
