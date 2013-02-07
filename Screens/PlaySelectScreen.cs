#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace DeModulate
{
    class PlaySelectScreen : MenuScreen
    {
        #region Initialization


        public PlaySelectScreen()
            : base("")
        {
            IsPopup = false;

            MenuEntry BreakoutEntry = new MenuEntry("Breakout Mode");
            MenuEntry AssaultEntry = new MenuEntry("Assault Mode");

            BreakoutEntry.Selected += BreakoutEntrySelected;
            AssaultEntry.Selected += AssaultEntrySelected;

            MenuEntries.Add(BreakoutEntry);
            MenuEntries.Add(AssaultEntry);
        }
        #endregion

        #region Handle Input

        void BreakoutEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Play BreakOut?";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitBreakoutxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }


        void AssaultEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Play Assault?";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitAssaultAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }

        void ConfirmQuitAssaultAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                              new GameplayScreen(ScreenManager.GraphicsDevice, 1));
        }

        void ConfirmQuitBreakoutxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                                new GameplayScreen(ScreenManager.GraphicsDevice, 0));
        }



        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        #endregion
    }
}
