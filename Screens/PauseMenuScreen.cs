#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace DeModulate
{
    class PauseMenuScreen : MenuScreen
    {
        #region Initialization


        public PauseMenuScreen()
            : base("")
        {
            IsPopup = true;

            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume");
            MenuEntry controlsGameMenuEntry = new MenuEntry("Controls");
            MenuEntry quitGameMenuEntry = new MenuEntry("End Game");
            
            resumeGameMenuEntry.Selected += OnCancel;
            controlsGameMenuEntry.Selected += controlsMenuEntrySelected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(controlsGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }
        #endregion

        #region Handle Input

        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Quit Current Game?";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }

        void controlsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new ControlsScreen(), ControllingPlayer);
        }

        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen("MainBG"),
                                                           new MainMenuScreen());
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
