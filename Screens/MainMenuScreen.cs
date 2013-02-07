#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace DeModulate
{
    class MainMenuScreen : MenuScreen
    {
        #region Initialization

        public MainMenuScreen()
            : base("")
        {
            MenuEntry playGameMenuEntry = new MenuEntry("Play");
            MenuEntry prologueMenuEntry = new MenuEntry("Mission Briefing");
            MenuEntry instructionsMenuEntry = new MenuEntry("Instructions");
            MenuEntry controlsMenuEntry = new MenuEntry("Controls");
            MenuEntry exitMenuEntry = new MenuEntry("Quit Game");

            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            prologueMenuEntry.Selected += PrologueMenuEntrySelected;
            instructionsMenuEntry.Selected += InstructionsMenuEntrySelected;
            controlsMenuEntry.Selected += controlsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(prologueMenuEntry);
            MenuEntries.Add(instructionsMenuEntry);
            MenuEntries.Add(controlsMenuEntry);
            MenuEntries.Add(exitMenuEntry);

        }


        #endregion

        #region Handle Input

        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new PlaySelectScreen(), e.PlayerIndex);
        }

           void PrologueMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new MessageBoxScreen("This old TV aerial wire has terrible signal. It's our job to\nmake sure none of that pesky extra data can clog up our\nnice reception. Luckily we have our trusty cleanup\nsystem at hand.\n\nWe can go about this two different ways. To clean the\ncables when theres no incoming signal, or we can do it\non the fly, up to you.",false), ControllingPlayer);

        }

           void InstructionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
           {
               ScreenManager.AddScreen(new MessageBoxScreen("Bounce the ball against the blocks.\n\n-Red blocks are the weakest and require a single hit.\n-Green blocks are a little stronger, needing 2 hits.\n-Blue blocks need the most with 3 hits.\n-Brown blocks cannot be destroyed.\n\nUse Modifiers\n-Acid Ball: Melts everything in it's path.\n-Size Changers: Slim down or fatten up in pesky spots.\n-Gold Ball: Twice the damage!\n-Padle Size: Get Big or Small with these.\n\nGame Modes\n-BreakOut: Destroy levels of increasing difficulty.\n-Assault: Hold back a horde of blocks for as long as possible.", false), ControllingPlayer);

           }
        void controlsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
             ScreenManager.AddScreen(new ControlsScreen(), ControllingPlayer);
        }
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Really Quit?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
