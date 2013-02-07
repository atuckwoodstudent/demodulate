#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DeModulate
{
    class SplashScreen : GameScreen
    {
        #region Fields
        TimeSpan timer = new TimeSpan();

         public SplashScreen(ScreenManager screenManager)
         {
             timer = TimeSpan.Zero;
         }


        #endregion
        #region Update and Draw

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            timer += gameTime.ElapsedGameTime;
            if (timer.Seconds >= 4)
            {
                ScreenManager.RemoveScreen(this);

                ScreenManager.AddScreen(new BackgroundScreen("MainBG"), ControllingPlayer);
                ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
                ScreenManager.Game.ResetElapsedTime();
            }
       }
        


        public override void Draw(GameTime gameTime)
        {

        }


        #endregion
    }
}
