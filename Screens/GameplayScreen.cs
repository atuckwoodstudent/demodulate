#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace DeModulate
{
    class GameplayScreen : GameScreen
    {
        //Object Declarations
        #region Fields
        clsCamera Cam = new clsCamera();
        bool CamPressed = false;
        bool isTopDown = false;
        AudioEngine audioEngine; WaveBank waveBank; SoundBank soundBank; Cue[] BGM;
        ContentManager content;
        GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;
        MessageBoxScreen GameOverScreen,CongratulationsScreen;
        PauseMenuScreen PauseMenu;
        Matrix WorldMatrix;
        
        Texture2D Expander, Decreaser, BiggerBall, SmallerBall, GoldenBall, AcidBall, AddLife, tMusicPlayer, tMusicPlayerOff;
        Model mArena, mKillZone;
        Model[] mBall, mBlock, mPlayer;
        cls3DObject Player, Arena, KillZone, HealthViewer, Ball;
        cls3DObject[] Blocks; 
        clsDrops MusicPlayer;

        SpriteFont gameFont, HUDfont;
        Rectangle sourceRect;
        TimeSpan tmrTimer, tmrVibrate;
        Random Randomizer;

        bool SetPlayerCarry, GameOverState, Vibrate, SwitchCue, PauseCue;
        int Score, PowerUP, numtoshow, currentcue; byte Level, GameType, LevelSwitch; float timespan;  
        String LevelName;
        #endregion

        //Constructor && Initializers
        #region Initialization
        public GameplayScreen(GraphicsDevice g, byte _gameType)
        {
            audioEngine = new AudioEngine(@"Content\Audio\Sounds.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");
            BGM = new Cue[9];
            graphicsDevice = g;
            graphicsDevice.RenderState.DepthBufferEnable = true;
            spriteBatch = new SpriteBatch(g);
            GameOverScreen = new MessageBoxScreen(" " + Score, false);
            CongratulationsScreen = new MessageBoxScreen(" ", false);
            PauseMenu = new PauseMenuScreen();
            WorldMatrix = Matrix.Identity;
            SwitchCue = false; PauseCue = false; GameOverState = false;

            tmrVibrate = new TimeSpan();
            tmrVibrate = TimeSpan.Zero;
            sourceRect = new Rectangle(0, 0, 64, 16);
            GameType = _gameType;
            currentcue = 0;
            if (GameType > 0)
            {
                timespan = 10;
                Randomizer = new Random();
                tmrTimer = new TimeSpan();
                tmrTimer = TimeSpan.Zero;
            }
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            //Load the game font and HUD font
            gameFont = content.Load<SpriteFont>("gamefont");
            HUDfont = content.Load<SpriteFont>("HUDFont");
            //Load background music
            BGM[0] = soundBank.GetCue("BGM1");
            BGM[1] = soundBank.GetCue("BGM2");
            BGM[2] = soundBank.GetCue("BGM3");
            BGM[3] = soundBank.GetCue("BGM4");
            BGM[4] = soundBank.GetCue("BGM5");
            BGM[5] = soundBank.GetCue("BGM6");
            BGM[6] = soundBank.GetCue("BGM7");
            BGM[7] = soundBank.GetCue("BGM8");
            BGM[8] = soundBank.GetCue("BGM9");
            //Set Powerup Textures and Music Player Textures
            Expander = content.Load<Texture2D>("Expander");
            Decreaser = content.Load<Texture2D>("Decreaser");
            AcidBall = content.Load<Texture2D>("AcidBall");
            GoldenBall = content.Load<Texture2D>("GoldenBall");
            BiggerBall = content.Load<Texture2D>("BigBall");
            SmallerBall = content.Load<Texture2D>("SmallBall");
            AddLife = content.Load<Texture2D>("AddLife");
         
            tMusicPlayer = content.Load<Texture2D>("MusicPlayer");
            tMusicPlayerOff = content.Load<Texture2D>("MusicPlayerOff");
            //Instantiate multiple models for Ball,Block and Player model arrays
            mBall = new Model[5];
            mBlock = new Model[4];
            mPlayer = new Model[3];
            //Load the afformentioned models into content
            mPlayer[0] = content.Load<Model>("Models/Player");
            mPlayer[1] = content.Load<Model>("Models/BigPlayer");
            mPlayer[2] = content.Load<Model>("Models/SmallPlayer");

            mArena = content.Load<Model>("Models/Arena");
            mKillZone = content.Load <Model>("Models/KillZone");

            mBall[0] = content.Load<Model>("Models/NormalBall");
            mBall[1] = content.Load <Model>("Models/AcidBall");
            mBall[2] = content.Load<Model>("Models/GoldBall");
            mBall[3] = content.Load<Model>("Models/BigBall");
            mBall[4] = content.Load<Model>("Models/SmallBall");

            mBlock[0] = content.Load<Model>("Models/BlockLvl1");
            mBlock[1] = content.Load<Model>("Models/BlockLvl2");
            mBlock[2] = content.Load<Model>("Models/BlockLvl3");
            mBlock[3] = content.Load<Model>("Models/BlockUnbreakable");

            //Create 30 blocks to be destroyed
            Blocks = new cls3DObject[30];
            //Create a new arena
            Arena = new cls3DObject(1, new Vector3(20f, 2f, 40f), true, graphicsDevice, WorldMatrix);
            //Create the music player animation
            MusicPlayer = new clsDrops(15, tMusicPlayer, new Vector2(435, 133), new Vector2(155, 24), Vector2.Zero, 4);
            MusicPlayer.Show();
            
            //Add models to various instantiated objects
            Arena.AddModel(ref mArena, 0);

            for (int i = 0; i < Blocks.Length; i++)
            {
                Blocks[i] = new cls3DObject(4, new Vector3(2, 1, 1), true, graphicsDevice, WorldMatrix);
                Blocks[i].AddModel(ref mBlock[0], 0);
                Blocks[i].AddModel(ref mBlock[1], 1);
                Blocks[i].AddModel(ref mBlock[2], 2);
                Blocks[i].AddModel(ref mBlock[3], 3);
            }

            Player = new cls3DObject(3, new Vector3(2.5f, 2, 2), true, graphicsDevice, WorldMatrix);
            Player.AddModel(ref mPlayer[0], 0);
            Player.AddModel(ref mPlayer[1], 1);
            Player.AddModel(ref mPlayer[2], 2);

            Ball = new cls3DObject(5, new Vector3(0.5f, 0.5f, 0.5f), true, graphicsDevice, WorldMatrix);
            Ball.AddModel(ref mBall[0], 0);
            Ball.AddModel(ref mBall[1], 1);
            Ball.AddModel(ref mBall[2], 2);
            Ball.AddModel(ref mBall[3], 3);
            Ball.AddModel(ref mBall[4], 4);

            KillZone = new cls3DObject(1, new Vector3(20f, 1, 3f), true, graphicsDevice, WorldMatrix);
            KillZone.AddModel(ref mKillZone, 0);
            HealthViewer = new cls3DObject(1, new Vector3(3f, 2, 2), true, graphicsDevice, WorldMatrix);
            HealthViewer.AddModel(ref mPlayer[0], 0);


            //Prepare game for first play
            Arena.SetPosition(new Vector3(-10, 0, -20));
            Arena.SetRotation(new Vector3(0, 0, 0));

            if (GameType <= 0)
            { 
                Level = 0; 
                Player.SetHealth(3);
                GetLevelLayout();
            }
            else
            {
                for (int i = 0; i < Blocks.Length; i++)
                Blocks[i].Hide();
                numtoshow = Blocks.Length -1; 
                Player.SetHealth(0);
                GetAssaultLayout();
            }           

            Player.SetPosition(new Vector3(-1,0,16));
            SetPlayerCarry = true; Ball.SetPosition(new Vector3(Player.GetPosition().X + (Player.GetSize().X / 2), Ball.GetPosition().Y, 14.5f));
            KillZone.SetPosition(new Vector3(-10,-1,20));
            HealthViewer.SetScale(0.5f);

           Thread.Sleep(1000);
            ScreenManager.Game.ResetElapsedTime();  
        
        }

        public override void UnloadContent()
        {
            content.Unload();
        }
                #endregion

        //Methods Used for Game Space Processing
        #region Efficiency Methods
        //Method used to determine health of input objects.
        public void HealthGenerator(int i)
        {
            if (Blocks[i].isVisible())
            {
                switch (Blocks[i].GetHealth())
                {
                    case 0: Blocks[i].Hide(); Blocks[i].PowerUp.Show(); break;
                    case 1: Blocks[i].SetFrame(0); break;
                    case 2: Blocks[i].SetFrame(1); break;
                    case 3: Blocks[i].SetFrame(2); break;
                    case 10: Blocks[i].SetFrame(3); break;
                }
            }
        }

        //Used to determine what powerups should be assigned
        public void GetPowerUps()
        {Score +=20;
            switch (PowerUP)
            {
               case 0:
                    if (Player.GetFrame() == 0 || Player.GetFrame() ==1)
                    {
                        Player.SetFrame(1);
                        Player.SetSize(new Vector3(3.9f, 2, 2)); Player.SetPosition(new Vector3(Player.GetPosition().X-1.5f,Player.GetPosition().Y,Player.GetPosition().Z));
                    }
                    else
                    {
                        Player.SetFrame(0);
                        Player.SetSize(new Vector3(2.5f, 2, 2));
                    }
                    break;
                case 1:
                    if (Player.GetFrame() == 0 || Player.GetFrame() == 2)
                    {
                        Player.SetFrame(2);
                        Player.SetSize(new Vector3(1.35f, 2, 2));
                    }
                    else
                    {
                        Player.SetFrame(0);
                        Player.SetSize(new Vector3(2.5f, 2, 2));
                    }
                    break;
                case 2:
                        Ball.SetFrame(1);
                        Ball.SetSize(new Vector3(0.5f, 0.5f, 0.5f));
                    break;
                case 3:
                        Ball.SetFrame(2);
                        Ball.SetSize(new Vector3(0.5f, 0.5f, 0.5f));
                    break;
                case 4:
                    if (Ball.GetFrame()>=0 && Ball.GetFrame()<4)
                    {
                        Ball.SetFrame(3);
                        Ball.SetSize(new Vector3(1.01f, 1.01f, 1.01f));
                    }
                    else
                    {
                        Ball.SetFrame(0);
                        Ball.SetSize(new Vector3(2.5f, 2, 2));
                    }
                    break;
                case 5:
                    if (Ball.GetFrame() >= 0 && Ball.GetFrame() <3 || Ball.GetFrame() == 4)
                    {
                        Ball.SetFrame(4);
                        Ball.SetSize(new Vector3(0.3f, 0.3f, 0.3f));
                    }
                    else
                    {
                        Ball.SetFrame(0);
                        Ball.SetSize(new Vector3(2.5f, 2, 2));
                    }
                    break;
                case 6:
                    Player.SetHealth(Player.GetHealth() + 1);
                    break;
            }
        }

        //Method to load the game over state
        public void GameOver()
        {
            if (!GameOverScreen.IsActive)
            {
                LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen("MainBG"), new MainMenuScreen());
            }
        }

        //Method to determine the layout of the current level for breakout mode, called during level switch event
        public void GetLevelLayout()
        {
            //Reset all blocks
            for (int i = 0; i < Blocks.Length; i++)
            { Blocks[i].Show(); SetPlayerCarry = true; Blocks[i].SetHealth(1); Blocks[i].RemovePowerUp(); Blocks[i].PowerUp.Hide(); }

            switch (Level)
            {
                case 0:
                    LevelName = "Big Pong";
                    for (int i = 15; i < 30; i++)
                        Blocks[i].Hide();

                    Blocks[0].AddPowerUP(0, Expander, Cam, Blocks[0].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[1].AddPowerUP(4, BiggerBall, Cam, Blocks[1].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 2);

                    Blocks[0].SetPosition(new Vector3(-1, 0, -20));
                    Blocks[1].SetPosition(new Vector3(-7, 0, -17));
                    Blocks[2].SetPosition(new Vector3(-4, 0, -17));
                    Blocks[3].SetPosition(new Vector3(2, 0, -17));
                    Blocks[4].SetPosition(new Vector3(5, 0, -17));
                    Blocks[5].SetPosition(new Vector3(-1, 0, -14));
                    Blocks[6].SetPosition(new Vector3(-4, 0, -14));
                    Blocks[7].SetPosition(new Vector3(2, 0, -14));
                    Blocks[8].SetPosition(new Vector3(-3, 0, -11));
                    Blocks[9].SetPosition(new Vector3(1, 0, -11));
                    Blocks[10].SetPosition(new Vector3(-5, 0, 4));
                    Blocks[11].SetPosition(new Vector3(-3, 0, 4));
                    Blocks[12].SetPosition(new Vector3(-1, 0, 4));
                    Blocks[13].SetPosition(new Vector3(1, 0, 4));
                    Blocks[14].SetPosition(new Vector3(3, 0, 4));
                    break;

                case 1:
                    LevelName = "Line Invaders";
                    for (int i = 10; i < 20; i++)
                        Blocks[i].SetHealth(2);

                    for (int i = 20; i < Blocks.Length; i++)
                        Blocks[i].Hide();

                    Blocks[15].AddPowerUP(3, GoldenBall, Cam, Blocks[15].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[19].AddPowerUP(5, SmallerBall, Cam, Blocks[19].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 2);
                    Blocks[12].AddPowerUP(6, AddLife, Cam, Blocks[12].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);

                    Blocks[0].SetPosition(new Vector3(-8, 0, -15));
                    Blocks[1].SetPosition(new Vector3(-6, 0, -15));
                    Blocks[2].SetPosition(new Vector3(-4, 0, -15));
                    Blocks[3].SetPosition(new Vector3(-2, 0, -15));
                    Blocks[4].SetPosition(new Vector3(0, 0, -15));
                    Blocks[5].SetPosition(new Vector3(2, 0, -15));
                    Blocks[6].SetPosition(new Vector3(4, 0, -15));
                    Blocks[7].SetPosition(new Vector3(6, 0, -15));
                    Blocks[8].SetPosition(new Vector3(-5, 0, -14));
                    Blocks[9].SetPosition(new Vector3(3, 0, -14));
                    Blocks[10].SetPosition(new Vector3(-5, 0, -10));
                    Blocks[11].SetPosition(new Vector3(-3, 0, -10));
                    Blocks[12].SetPosition(new Vector3(-1, 0, -10));
                    Blocks[13].SetPosition(new Vector3(1, 0, -10));
                    Blocks[14].SetPosition(new Vector3(3, 0, -10));
                    Blocks[15].SetPosition(new Vector3(-10, 0, -5));
                    Blocks[16].SetPosition(new Vector3(-5, 0, -5));
                    Blocks[17].SetPosition(new Vector3(-1, 0, -5));
                    Blocks[18].SetPosition(new Vector3(3, 0, -5));
                    Blocks[19].SetPosition(new Vector3(8, 0, -5));
                    break;

                case 2:
                    LevelName = "Missile Defence, Upside Down";
                    for (int i = 0; i < 10; i++)
                        Blocks[i].SetHealth(2);

                    for (int i = 10; i < 15; i++)
                        Blocks[i].SetHealth(3);

                    Blocks[9].AddPowerUP(0, Expander, Cam, Blocks[0].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[5].AddPowerUP(1, Decreaser, Cam, Blocks[0].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[10].AddPowerUP(2, AcidBall, Cam, Blocks[10].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[14].AddPowerUP(2, AcidBall, Cam, Blocks[14].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[4].AddPowerUP(4, BiggerBall, Cam, Blocks[0].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 2);
                    Blocks[5].AddPowerUP(6, AddLife, Cam, Blocks[0].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[6].AddPowerUP(6, AddLife, Cam, Blocks[6].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);

                    Blocks[0].SetPosition(new Vector3(-10, 0, -20));
                    Blocks[1].SetPosition(new Vector3(-8, 0, -20));
                    Blocks[2].SetPosition(new Vector3(-6, 0, -20));
                    Blocks[3].SetPosition(new Vector3(-4, 0, -20));
                    Blocks[4].SetPosition(new Vector3(-2, 0, -20));
                    Blocks[5].SetPosition(new Vector3(0, 0, -20));
                    Blocks[6].SetPosition(new Vector3(2, 0, -20));
                    Blocks[7].SetPosition(new Vector3(4, 0, -20));
                    Blocks[8].SetPosition(new Vector3(6, 0, -20));
                    Blocks[9].SetPosition(new Vector3(8, 0, -20));
                    Blocks[10].SetPosition(new Vector3(-10, 0, 0));
                    Blocks[11].SetPosition(new Vector3(-5, 0, 0));
                    Blocks[12].SetPosition(new Vector3(-1, 0, 0));
                    Blocks[13].SetPosition(new Vector3(3, 0, 0));
                    Blocks[14].SetPosition(new Vector3(8, 0, 0));
                    Blocks[15].SetPosition(new Vector3(-9, 0, -19));
                    Blocks[16].SetPosition(new Vector3(-7, 0, -19));
                    Blocks[17].SetPosition(new Vector3(-5, 0, -19));
                    Blocks[18].SetPosition(new Vector3(-3, 0, -19));
                    Blocks[19].SetPosition(new Vector3(-1, 0, -19));
                    Blocks[20].SetPosition(new Vector3(1, 0, -19));
                    Blocks[21].SetPosition(new Vector3(3, 0, -19));
                    Blocks[22].SetPosition(new Vector3(5, 0, -19));
                    Blocks[23].SetPosition(new Vector3(7, 0, -19));
                    Blocks[24].SetPosition(new Vector3(-4, 0, -18));
                    Blocks[25].SetPosition(new Vector3(-2, 0, -18));
                    Blocks[26].SetPosition(new Vector3(1, 0, -18));
                    Blocks[27].SetPosition(new Vector3(3, 0, -18));
                    Blocks[28].SetPosition(new Vector3(-10, 0, -9));
                    Blocks[29].SetPosition(new Vector3(8, 0, -9));
                    break;

                case 3:
                    LevelName = "Fat Snake";
                    for (int i = 0; i < Blocks.Length; i++)
                        Blocks[i].SetHealth(3);

                    Blocks[0].AddPowerUP(0, Expander, Cam, Blocks[0].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[1].AddPowerUP(1, Decreaser, Cam, Blocks[0].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[2].AddPowerUP(2, AcidBall, Cam, Blocks[0].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[3].AddPowerUP(3, GoldenBall, Cam, Blocks[0].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[4].AddPowerUP(4, BiggerBall, Cam, Blocks[0].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 2);
                    Blocks[5].AddPowerUP(5, SmallerBall, Cam, Blocks[0].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 2);
                    Blocks[6].AddPowerUP(6, AddLife, Cam, Blocks[6].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);

                    Blocks[0].SetPosition(new Vector3(-6, 0, -4));
                    Blocks[1].SetPosition(new Vector3(-4, 0, -4));
                    Blocks[2].SetPosition(new Vector3(-2, 0, -4));
                    Blocks[3].SetPosition(new Vector3(0, 0, -4));
                    Blocks[4].SetPosition(new Vector3(2, 0, -4));
                    Blocks[5].SetPosition(new Vector3(4, 0, -4));
                    Blocks[6].SetPosition(new Vector3(-6, 0, -3));
                    Blocks[7].SetPosition(new Vector3(-4, 0, -3));
                    Blocks[8].SetPosition(new Vector3(-2, 0, -3));
                    Blocks[9].SetPosition(new Vector3(0, 0, -3));
                    Blocks[10].SetPosition(new Vector3(2, 0, -3));
                    Blocks[11].SetPosition(new Vector3(4, 0, -3));
                    Blocks[12].SetPosition(new Vector3(-6, 0, -2));
                    Blocks[13].SetPosition(new Vector3(-4, 0, -2));
                    Blocks[14].SetPosition(new Vector3(-2, 0, -2));
                    Blocks[15].SetPosition(new Vector3(0, 0, -2));
                    Blocks[16].SetPosition(new Vector3(2, 0, -2));
                    Blocks[17].SetPosition(new Vector3(4, 0, -2));
                    Blocks[18].SetPosition(new Vector3(-6, 0, -1));
                    Blocks[19].SetPosition(new Vector3(-4, 0, -1));
                    Blocks[20].SetPosition(new Vector3(-2, 0, -1));
                    Blocks[21].SetPosition(new Vector3(0, 0, -1));
                    Blocks[22].SetPosition(new Vector3(2, 0, -1));
                    Blocks[23].SetPosition(new Vector3(4, 0, -1));
                    Blocks[24].SetPosition(new Vector3(-6, 0, 0));
                    Blocks[25].SetPosition(new Vector3(-4, 0, 0));
                    Blocks[26].SetPosition(new Vector3(-2, 0, 0));
                    Blocks[27].SetPosition(new Vector3(0, 0, 0));
                    Blocks[28].SetPosition(new Vector3(2, 0, 0));
                    Blocks[29].SetPosition(new Vector3(4, 0, 0));
                    break;


                case 4:
                    LevelName = "The Great Eye";
                    for (int i = 23; i < Blocks.Length; i++)
                        Blocks[i].Hide();

                    for (int i = 0; i < 3; i++)
                        Blocks[i].SetHealth(3);
                    Blocks[8].SetHealth(3);
                    Blocks[11].SetHealth(3);
                    Blocks[14].SetHealth(3);
                    for (int i = 16; i < 20; i++)
                        Blocks[i].SetHealth(3);
                    for (int i = 20; i < 23; i++)
                        Blocks[i].SetHealth(2);

                    Blocks[0].AddPowerUP(0, Expander, Cam, Blocks[0].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[1].AddPowerUP(1, Decreaser, Cam, Blocks[1].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[2].AddPowerUP(0, Expander, Cam, Blocks[2].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[16].AddPowerUP(3, GoldenBall, Cam, Blocks[16].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[17].AddPowerUP(6, AddLife, Cam, Blocks[17].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[18].AddPowerUP(5, SmallerBall, Cam, Blocks[18].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 2);
                    Blocks[19].AddPowerUP(2, AcidBall, Cam, Blocks[19].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);

                    Blocks[0].SetPosition(new Vector3(-1, 0, -9));
                    Blocks[1].SetPosition(new Vector3(-5, 0, -9));
                    Blocks[2].SetPosition(new Vector3(3, 0, -9));
                    Blocks[3].SetPosition(new Vector3(-4, 0, -4));
                    Blocks[4].SetPosition(new Vector3(-2, 0, -4));
                    Blocks[5].SetPosition(new Vector3(0, 0, -4));
                    Blocks[6].SetPosition(new Vector3(2, 0, -4));
                    Blocks[7].SetPosition(new Vector3(-6, 0, -3));
                    Blocks[8].SetPosition(new Vector3(-1, 0, -3));
                    Blocks[9].SetPosition(new Vector3(4, 0, -3));
                    Blocks[10].SetPosition(new Vector3(-8, 0, -2));
                    Blocks[11].SetPosition(new Vector3(-1, 0, -2));
                    Blocks[12].SetPosition(new Vector3(6, 0, -2));
                    Blocks[13].SetPosition(new Vector3(-6, 0, -1));
                    Blocks[14].SetPosition(new Vector3(-1, 0, -1));
                    Blocks[15].SetPosition(new Vector3(4, 0, -1));
                    Blocks[16].SetPosition(new Vector3(-4, 0, 0));
                    Blocks[17].SetPosition(new Vector3(-2, 0, 0));
                    Blocks[18].SetPosition(new Vector3(0, 0, 0));
                    Blocks[19].SetPosition(new Vector3(2, 0, 0));
                    Blocks[20].SetPosition(new Vector3(-1, 0, 4));
                    Blocks[21].SetPosition(new Vector3(-5, 0, 4));
                    Blocks[22].SetPosition(new Vector3(3, 0, 4));
                    break;

                case 5:
                    LevelName = "Jack O Neil";
                    for (int i = 27; i < Blocks.Length; i++)
                        Blocks[i].Hide();

                    for (int i = 1; i < 6; i++)
                        Blocks[i].SetHealth(3);

                    for (int i = 11; i < 18; i++)
                        Blocks[i].SetHealth(2);

                    for (int i = 18; i < 27; i++)
                        Blocks[i].SetHealth(3);

                    Blocks[11].AddPowerUP(6, AddLife, Cam, Blocks[11].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[14].AddPowerUP(1, Decreaser, Cam, Blocks[14].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[16].AddPowerUP(1, Decreaser, Cam, Blocks[16].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[19].AddPowerUP(5, SmallerBall, Cam, Blocks[19].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 2);
                    Blocks[23].AddPowerUP(5, SmallerBall, Cam, Blocks[23].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 2);
                    Blocks[25].AddPowerUP(1, Decreaser, Cam, Blocks[25].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[26].AddPowerUP(6, AddLife, Cam, Blocks[26].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);

                    Blocks[0].SetPosition(new Vector3(-1, 0, -15));
                    Blocks[1].SetPosition(new Vector3(-2, 0, -13));
                    Blocks[2].SetPosition(new Vector3(-0, 0, -13));
                    Blocks[3].SetPosition(new Vector3(-3, 0, -11));
                    Blocks[4].SetPosition(new Vector3(-1, 0, -11));
                    Blocks[5].SetPosition(new Vector3(1, 0, -11));
                    Blocks[6].SetPosition(new Vector3(-5, 0, -9));
                    Blocks[7].SetPosition(new Vector3(-3, 0, -9));
                    Blocks[8].SetPosition(new Vector3(-1, 0, -9));
                    Blocks[9].SetPosition(new Vector3(1, 0, -9));
                    Blocks[10].SetPosition(new Vector3(3, 0, -9));
                    Blocks[11].SetPosition(new Vector3(-7, 0, -7));
                    Blocks[12].SetPosition(new Vector3(-5, 0, -7));
                    Blocks[13].SetPosition(new Vector3(-3, 0, -7));
                    Blocks[14].SetPosition(new Vector3(-1, 0, -7));
                    Blocks[15].SetPosition(new Vector3(1, 0, -7));
                    Blocks[16].SetPosition(new Vector3(3, 0, -7));
                    Blocks[17].SetPosition(new Vector3(5, 0, -7));
                    Blocks[18].SetPosition(new Vector3(-9, 0, -5));
                    Blocks[19].SetPosition(new Vector3(-7, 0, -5));
                    Blocks[20].SetPosition(new Vector3(-5, 0, -5));
                    Blocks[21].SetPosition(new Vector3(-3, 0, -5));
                    Blocks[22].SetPosition(new Vector3(-1, 0, -5));
                    Blocks[23].SetPosition(new Vector3(1, 0, -5));
                    Blocks[24].SetPosition(new Vector3(3, 0, -5));
                    Blocks[25].SetPosition(new Vector3(5, 0, -5));
                    Blocks[26].SetPosition(new Vector3(7, 0, -5));
                    break;

                case 6:
                    LevelName = "4th July";
                    for (int i = 23; i < Blocks.Length; i++)
                        Blocks[i].Hide();

                    Blocks[5].AddPowerUP(2, AcidBall, Cam, Blocks[5].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[7].AddPowerUP(2, AcidBall, Cam, Blocks[7].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[9].AddPowerUP(2, AcidBall, Cam, Blocks[9].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[11].AddPowerUP(2, AcidBall, Cam, Blocks[11].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[13].AddPowerUP(2, AcidBall, Cam, Blocks[13].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[15].AddPowerUP(2, AcidBall, Cam, Blocks[15].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[0].AddPowerUP(6, AddLife, Cam, Blocks[0].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);

                    Blocks[5].SetHealth(3);
                    Blocks[6].SetHealth(3);
                    Blocks[7].SetHealth(2);
                    Blocks[8].SetHealth(3);
                    Blocks[9].SetHealth(3);
                    Blocks[10].SetHealth(2);
                    Blocks[11].SetHealth(10);
                    Blocks[12].SetHealth(2);
                    Blocks[13].SetHealth(3);
                    Blocks[14].SetHealth(3);
                    Blocks[15].SetHealth(2);

                    for (int i = 16; i < 23; i++)
                        Blocks[i].SetHealth(3);

                    Blocks[23].SetHealth(10);

                    Blocks[0].SetPosition(new Vector3(-1, 0, -15));
                    Blocks[1].SetPosition(new Vector3(-4, 0, -15));
                    Blocks[2].SetPosition(new Vector3(2, 0, -15));
                    Blocks[3].SetPosition(new Vector3(-7, 0, -3));
                    Blocks[4].SetPosition(new Vector3(5, 0, -3));
                    Blocks[5].SetPosition(new Vector3(-1, 0, -9));
                    Blocks[6].SetPosition(new Vector3(-3, 0, -8));
                    Blocks[7].SetPosition(new Vector3(-1, 0, -8));
                    Blocks[8].SetPosition(new Vector3(1, 0, -8));
                    Blocks[9].SetPosition(new Vector3(-5, 0, -7));
                    Blocks[10].SetPosition(new Vector3(-3, 0, -7));
                    Blocks[11].SetPosition(new Vector3(-1, 0, -7));
                    Blocks[12].SetPosition(new Vector3(1, 0, -7));
                    Blocks[13].SetPosition(new Vector3(3, 0, -7));
                    Blocks[14].SetPosition(new Vector3(-3, 0, -6));
                    Blocks[15].SetPosition(new Vector3(-1, 0, -6));
                    Blocks[16].SetPosition(new Vector3(1, 0, -6));
                    Blocks[17].SetPosition(new Vector3(-1, 0, -5));
                    Blocks[18].SetPosition(new Vector3(-6, 0, -1));
                    Blocks[19].SetPosition(new Vector3(-4, 0, -1));
                    Blocks[20].SetPosition(new Vector3(-1, 0, -1));
                    Blocks[21].SetPosition(new Vector3(2, 0, -1));
                    Blocks[22].SetPosition(new Vector3(4, 0, -1));
                    break;

                case 7:
                    LevelName = "Trap for Sam Fisher";

                    Blocks[2].AddPowerUP(3, GoldenBall, Cam, Blocks[2].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[4].AddPowerUP(3, GoldenBall, Cam, Blocks[4].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[6].AddPowerUP(5, SmallerBall, Cam, Blocks[6].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 2);
                    Blocks[9].AddPowerUP(5, SmallerBall, Cam, Blocks[9].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 2);
                    Blocks[13].AddPowerUP(0, Expander, Cam, Blocks[13].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[15].AddPowerUP(1, Decreaser, Cam, Blocks[15].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[0].AddPowerUP(6, AddLife, Cam, Blocks[0].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);

                    for (int i = 24; i < Blocks.Length; i++)
                        Blocks[i].Hide();


                    for (int i = 0; i < 19; i++)
                        Blocks[i].SetHealth(3);
                    Blocks[19].SetHealth(10);
                    Blocks[20].SetHealth(10);
                    Blocks[21].SetHealth(2);
                    Blocks[22].SetHealth(10);
                    Blocks[23].SetHealth(10);

                    Blocks[0].SetPosition(new Vector3(-5, 0, -16));
                    Blocks[1].SetPosition(new Vector3(-2, 0, -16));
                    Blocks[2].SetPosition(new Vector3(0, 0, -16));
                    Blocks[3].SetPosition(new Vector3(3, 0, -16));
                    Blocks[4].SetPosition(new Vector3(-8, 0, -14));
                    Blocks[5].SetPosition(new Vector3(6, 0, -14));
                    Blocks[6].SetPosition(new Vector3(-8, 0, -13));
                    Blocks[7].SetPosition(new Vector3(6, 0, -13));
                    Blocks[8].SetPosition(new Vector3(-8, 0, -12));
                    Blocks[9].SetPosition(new Vector3(6, 0, -12));
                    Blocks[10].SetPosition(new Vector3(-8, 0, -11));
                    Blocks[11].SetPosition(new Vector3(6, 0, -11));
                    Blocks[12].SetPosition(new Vector3(-8, 0, -10));
                    Blocks[13].SetPosition(new Vector3(6, 0, -10));
                    Blocks[14].SetPosition(new Vector3(-8, 0, -9));
                    Blocks[15].SetPosition(new Vector3(6, 0, -9));
                    Blocks[16].SetPosition(new Vector3(-8, 0, -8));
                    Blocks[17].SetPosition(new Vector3(6, 0, -8));
                    Blocks[18].SetPosition(new Vector3(-1, 0, 2));
                    Blocks[19].SetPosition(new Vector3(-5, 0, -13));
                    Blocks[20].SetPosition(new Vector3(-3, 0, -12));
                    Blocks[21].SetPosition(new Vector3(-1, 0, -11));
                    Blocks[22].SetPosition(new Vector3(1, 0, -10));
                    Blocks[23].SetPosition(new Vector3(3, 0, -9));
                    break;

                case 8:
                    LevelName = "House Rocket";
                    for (int i = 0; i < 4; i++)
                        Blocks[i].SetHealth(2);
                    for (int i = 4; i < 8; i++)
                        Blocks[i].SetHealth(10);
                    Blocks[8].SetHealth(3);
                    for (int i = 9; i < 13; i++)
                        Blocks[i].SetHealth(10);
                    for (int i = 13; i < 16; i++)
                        Blocks[i].SetHealth(3);
                    for (int i = 16; i < 25; i++)
                        Blocks[i].SetHealth(2);
                    for (int i = 26; i < 30; i++)
                        Blocks[i].SetHealth(3);
                    Blocks[29].Hide();

                    Blocks[0].AddPowerUP(1, Decreaser, Cam, Blocks[0].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[1].AddPowerUP(1, Decreaser, Cam, Blocks[1].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[16].AddPowerUP(3, GoldenBall, Cam, Blocks[16].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[27].AddPowerUP(3, GoldenBall, Cam, Blocks[27].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[28].AddPowerUP(6, AddLife, Cam, Blocks[28].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[29].AddPowerUP(6, AddLife, Cam, Blocks[29].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);

                    Blocks[0].SetPosition(new Vector3(-4, 0, -8));
                    Blocks[1].SetPosition(new Vector3(-2, 0, -8));
                    Blocks[2].SetPosition(new Vector3(-0, 0, -8));
                    Blocks[3].SetPosition(new Vector3(2, 0, -8));
                    Blocks[4].SetPosition(new Vector3(-4, 0, -7));
                    Blocks[5].SetPosition(new Vector3(2, 0, -7));
                    Blocks[6].SetPosition(new Vector3(-4, 0, -6));
                    Blocks[7].SetPosition(new Vector3(2, 0, -6));
                    Blocks[8].SetPosition(new Vector3(-8, 0, -1));
                    Blocks[9].SetPosition(new Vector3(-4, 0, -5));
                    Blocks[10].SetPosition(new Vector3(-2, 0, -5));
                    Blocks[11].SetPosition(new Vector3(0, 0, -5));
                    Blocks[12].SetPosition(new Vector3(2, 0, -5));
                    Blocks[13].SetPosition(new Vector3(6, 0, -1));
                    Blocks[14].SetPosition(new Vector3(-7, 0, 0));
                    Blocks[15].SetPosition(new Vector3(5, 0, 0));
                    Blocks[16].SetPosition(new Vector3(-9, 0, 1));
                    Blocks[17].SetPosition(new Vector3(-7, 0, 1));
                    Blocks[18].SetPosition(new Vector3(-5, 0, 1));
                    Blocks[19].SetPosition(new Vector3(-3, 0, 1));
                    Blocks[20].SetPosition(new Vector3(-1, 0, 1));
                    Blocks[21].SetPosition(new Vector3(1, 0, 1));
                    Blocks[22].SetPosition(new Vector3(3, 0, 1));
                    Blocks[23].SetPosition(new Vector3(5, 0, 1));
                    Blocks[24].SetPosition(new Vector3(7, 0, 1));
                    Blocks[25].SetPosition(new Vector3(-1, 0, -4));
                    Blocks[26].SetPosition(new Vector3(-3, 0, 4));
                    Blocks[27].SetPosition(new Vector3(-1, 0, 4));
                    Blocks[28].SetPosition(new Vector3(1, 0, 4));
                    break;

                case 9:
                    LevelName = "Balancing...";
                    for (int i = 0; i < 9; i++)
                        Blocks[i].SetHealth(2);
                    for (int i = 9; i < 21; i++)
                        Blocks[i].SetHealth(3);
                    for (int i = 21; i < 30; i++)
                        Blocks[i].SetHealth(10);
                    Blocks[29].SetHealth(10);

                    Blocks[9].AddPowerUP(6, AddLife, Cam, Blocks[0].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);
                    Blocks[20].AddPowerUP(6, AddLife, Cam, Blocks[1].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1);

                    Blocks[0].SetPosition(new Vector3(-5, 0, -18));
                    Blocks[1].SetPosition(new Vector3(-5, 0, -17));
                    Blocks[2].SetPosition(new Vector3(-5, 0, -16));
                    Blocks[3].SetPosition(new Vector3(-5, 0, -15));
                    Blocks[4].SetPosition(new Vector3(3, 0, -18));
                    Blocks[5].SetPosition(new Vector3(3, 0, -17));
                    Blocks[6].SetPosition(new Vector3(3, 0, -16));
                    Blocks[7].SetPosition(new Vector3(3, 0, -15));
                    Blocks[8].SetPosition(new Vector3(-1, 0, -18));
                    Blocks[9].SetPosition(new Vector3(-1, 0, -17));
                    Blocks[10].SetPosition(new Vector3(-1, 0, -16));
                    Blocks[11].SetPosition(new Vector3(-1, 0, -15));
                    Blocks[12].SetPosition(new Vector3(-1, 0, -14));
                    Blocks[13].SetPosition(new Vector3(6, 0, -18));
                    Blocks[14].SetPosition(new Vector3(8, 0, -18));
                    Blocks[15].SetPosition(new Vector3(6, 0, -17));
                    Blocks[16].SetPosition(new Vector3(8, 0, -17));
                    Blocks[17].SetPosition(new Vector3(-10, 0, -18));
                    Blocks[18].SetPosition(new Vector3(-8, 0, -18));
                    Blocks[19].SetPosition(new Vector3(-10, 0, -17));
                    Blocks[20].SetPosition(new Vector3(-8, 0, -17));
                    Blocks[21].SetPosition(new Vector3(-10, 0, -14));
                    Blocks[22].SetPosition(new Vector3(-10, 0, -12));
                    Blocks[23].SetPosition(new Vector3(-8, 0, -14));
                    Blocks[24].SetPosition(new Vector3(-6, 0, -14));
                    Blocks[25].SetPosition(new Vector3(-1, 0, -5));
                    Blocks[26].SetPosition(new Vector3(4, 0, -14));
                    Blocks[27].SetPosition(new Vector3(6, 0, -14));
                    Blocks[28].SetPosition(new Vector3(8, 0, -14));
                    Blocks[29].SetPosition(new Vector3(8, 0, -12));
                    break;

            }
            //Generate the health of rendered blocks
            for (int i = 0; i < Blocks.Length; i++)
            { HealthGenerator(i); }
        }

        //Method used to generate blocks in assault mode.
        public void GetAssaultLayout()
        {
            //Randomize values
            int random_1, random_2, random_3, random_4, random_5;
            bool show_1, show_2;

            show_1 = true; show_2 = true;

            random_1 = Randomizer.Next(18) - 10;
            random_2 = Randomizer.Next(18) - 10;
            random_3 = Randomizer.Next(4);
            random_4 = Randomizer.Next(4);
            random_5 = Randomizer.Next(6);

            //Move blocks down 3 spaces
            for (int i = Blocks.Length - 1; i > -1; i--)
            {
                Blocks[i].SetVelocity(new Vector3(0, 0, 3));
                Blocks[i].Move();
            }
//Process blocks to be drawn
            if (random_1 == random_2 || random_1 == random_2 - 1 || random_1 == random_2 + 1)
                show_1 = false;
            
            if (numtoshow > -1)
            {
                if (show_1)
                {
                    Blocks[numtoshow].Show();
                    Blocks[numtoshow].SetPosition(new Vector3(random_1, 0, -20));
                    Blocks[numtoshow].RemovePowerUp();
                    if (random_3 > 0)
                        Blocks[numtoshow].SetHealth(random_3);
                    else
                        Blocks[numtoshow].Hide();
                }
                else
                    Blocks[numtoshow].Hide();
                numtoshow--;

                if (show_2)
                {
                    Blocks[numtoshow].Show();
                    Blocks[numtoshow].SetPosition(new Vector3(random_2, 0, -20));
                    if (random_4 > 0)
                        Blocks[numtoshow].SetHealth(random_4);
                    else
                        Blocks[numtoshow].Hide();
                    switch (random_5)
                    {
                        case 0:
                            Blocks[numtoshow].AddPowerUP(0, Expander, Cam, Blocks[numtoshow].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1); break;
                        case 1:
                            Blocks[numtoshow].AddPowerUP(1, Decreaser, Cam, Blocks[numtoshow].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1); break;
                        case 2:
                            Blocks[numtoshow].AddPowerUP(2, AcidBall, Cam, Blocks[numtoshow].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1); break;
                        case 3:
                            Blocks[numtoshow].AddPowerUP(3, GoldenBall, Cam, Blocks[numtoshow].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 1); break;
                        case 4:
                            Blocks[numtoshow].AddPowerUP(4, BiggerBall, Cam, Blocks[numtoshow].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 2); break;
                        case 5:
                            Blocks[numtoshow].AddPowerUP(5, SmallerBall, Cam, Blocks[numtoshow].GetScreenPos(), new Vector2(64, 16), graphicsDevice, 2); break;
                    }
                }
                else
                    Blocks[numtoshow].Hide();
                numtoshow--;
            }
            else
                numtoshow = Blocks.Length - 1;
            //Generate health of drawn blocks
            for (int i = 0; i < Blocks.Length; i++)
            { HealthGenerator(i); }

        }
#endregion

        //Update, Input and Drawing methods
        #region Update and Draw
        //Update current states of the game
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            //if the current game screen is active
            if (IsActive)
            {         
                //Change the message of the end state message box
                GameOverScreen.ChangeMessage("GAME OVER.\n\nFinal Score: " + Score);
                CongratulationsScreen.ChangeMessage("CONGRATULATIONS! YOU COMPLETED THE GAME!\n\nFinal Score: " + Score);
                
                //Check if the game is over...                
                if (GameOverState)
                    GameOver();
                //If not then play
                else
                {
                    
                    //Move the player and ball, animate the music graphics.
                    Ball.Move();
                    Player.Move();
                    MusicPlayer.Animate(gameTime);

                    //if music state is set to on then make sure it is playing
                    if(PauseCue)
                    {
                        if (!BGM[currentcue].IsPaused)
                            BGM[currentcue].Pause();
                    }
                    //otherwise make sure it is turned off
                    else
                    {
                    if (!BGM[currentcue].IsPlaying) BGM[currentcue].Play();
                    if (BGM[currentcue].IsPaused) BGM[currentcue].Resume();
                    }

                    //if the controller should be vibrating
                    //check the current timer for the amount is should be vibrating for
                    //turn vibration off if it has gone over that time
                    if (Vibrate)
                        tmrVibrate += gameTime.ElapsedGameTime;
                    if (tmrVibrate.Milliseconds >= 100)
                    {
                        Vibrate = false;
                        tmrVibrate = TimeSpan.Zero;
                    }

                    //for the amount of blocks in the scene
                    //if the powerups from those blocks are showing
                    //then animate them and move them
                    for (int i = 0; i < Blocks.Length; i++)
                        if (Blocks[i].PowerUp.isVisible())
                        {
                            Blocks[i].PowerUp.Animate(gameTime);
                            Blocks[i].PowerUp.Move();
                        }
                    //if breakout mode and all blocks are invisibke
                    //go to next level scene
                    if (GameType <= 0)
                    {
                        if (LevelSwitch == Blocks.Length)
                        {
                            Level++;
                            LevelSwitch = 0;
                            GetLevelLayout();
                        }
                        LevelSwitch = 0;
                    //count the blocks that are invisible
                        for (int i = 0; i < Blocks.Length; i++)
                            if (!Blocks[i].isVisible() || Blocks[i].GetHealth() == 10)
                                LevelSwitch++;
                    }
                    //if assault mode, check timer to make
                    //sure the scene should not be regenerating
                    //if it should then get the next line of blocks from
                    //GetAssaultMethod()
                    else
                    {
                        tmrTimer += gameTime.ElapsedGameTime;
                        if (tmrTimer.Seconds >= timespan)
                        {
                            Score += 10;
                            tmrTimer = TimeSpan.Zero;
                            if (timespan > 1)
                                timespan -= 0.1f;
                            GetAssaultLayout();
                        }
                    }
                    //if all levels are complete, say the game is completed
                    if (Level > 9)
                    {
                        ScreenManager.AddScreen(CongratulationsScreen, ControllingPlayer);
                        GameOverState = true;
                    }
                    //if the player should be carrying the ball, then make it so
                    if (SetPlayerCarry)
                    {
                        Ball.SetPosition(new Vector3(Player.GetPosition().X + (Player.GetSize().X / 2), Ball.GetPosition().Y, 14.5f));
                        Player.SetFrame(0); Player.SetSize(new Vector3(2.5f, 2, 2));
                        Ball.SetFrame(0); Ball.SetSize(new Vector3(0.5f, 0.5f, 0.5f));
                    }

                    //if the ball collides with the kill zone and the game type
                    //is breakout, then set health--. If assault mode
                    //decrement score by 40. Set to player carrying the ball
                    if (Ball.ExternalCollision(KillZone, 5))
                    {
                        SetPlayerCarry = true;
                        if (GameType <= 0)
                        {
                            Ball.SetVelocity(Vector3.Zero);
                            if (Player.GetHealth() > 0)
                                Player.SetHealth(Player.GetHealth() - 1);
                            else
                            {
                                ScreenManager.AddScreen(GameOverScreen, ControllingPlayer);
                                GameOverState = true;
                            }
                        }
                        else
                            Score -= 40;
                    }

                    //for the amount of blocks in the scene
                    for (int i = 0; i < Blocks.Length; i++)
                    {
                        //if assault mode and a block collides with the kill zone
                        //end the game
                        if (GameType > 0)
                            if (Blocks[i].ExternalCollision(KillZone))
                            {
                                if (!GameOverState)
                                {
                                    ScreenManager.AddScreen(GameOverScreen, ControllingPlayer);
                                    GameOverState = true;
                                }
                            }

                        //if the ball collides with a block then play
                        //a sound cue. if the ball is not indestructable
                        //then determine the effect of various ball types
                        //on the blocks health. Increment score by 10 and 
                        //increase the ball speed.
                        if (Ball.ExternalCollision(Blocks[i]))
                        {
                            if (Ball.GetFrame() != 1)
                                Ball.ExtFlatSurfaceBounce(Blocks[i]);
                            soundBank.PlayCue("Glass_Bounce");
                            if (Blocks[i].GetHealth() != 10)
                            {
                                switch (Ball.GetFrame())
                                {
                                    case 0:
                                    case 3:
                                    case 4:
                                        Blocks[i].SetHealth(Blocks[i].GetHealth() - 1);
                                        break;
                                    case 1:
                                        Blocks[i].SetHealth(0);
                                        break;
                                    case 2:
                                        if (Blocks[i].GetHealth() > 1)
                                            Blocks[i].SetHealth(Blocks[i].GetHealth() - 2);
                                        else
                                            Blocks[i].SetHealth(Blocks[i].GetHealth() - 1);
                                        break;
                                }
                            }
                            Score += 10;
                            HealthGenerator(i);
                            if (Ball.GetSpeed() < 0.45f)
                                Ball.SetSpeed(Ball.GetSpeed() + 0.01f);
                        }
                        //if the block powerup collised with the screen position of the player then
                        //vibrate the controller, hide the power up, get the value of the power up
                        //and then process that value to determine what modifier to add. Set the power
                        //value to 0.
                        if (Blocks[i].PowerUp.Collides(Player.GetScreenPos(), Player.GetScreenSize()))
                        {
                            Vibrate = true;
                            Blocks[i].PowerUp.Hide();
                            PowerUP = Blocks[i].PowerUp.GetID();
                            GetPowerUps();
                            PowerUP = 0;
                        }
                        //if the block power up collides with the kill zone then hide it
                        if (Blocks[i].PowerUp.Collides(KillZone.GetScreenPos(), KillZone.GetScreenSize()))
                            Blocks[i].PowerUp.Hide();
                    }

                    //if the ball bounces against the arena wall then
                    //play a bouncing sound file and process the collision
                    if (Ball.InternalCollision(Arena))
                    {
                        soundBank.PlayCue("Normal_Bounce");
                        Ball.InternalBounce(Arena);
                    }

                    //if the ball bounces against the player then
                    //play a bouncing sound file, vibrate the controller,
                    //speed up the ball and process the collision
                    if (Ball.ExternalCollision(Player))
                    {
                        soundBank.PlayCue("Rock_Bounce");
                        Vibrate = true;
                        Ball.ExtCurvedSurfaceBounce(Player);
                        if (Ball.GetSpeed() < 0.45f)
                            Ball.SetSpeed(Ball.GetSpeed() + 0.01f);
                    }
                }
            }
            //if the screen is not active then pause the music
            else
                BGM[currentcue].Pause();
        }


        public override void HandleInput(InputState input)
        {
            //if no input, throw exception
            if (input == null)
                throw new ArgumentNullException("input");
            //playerindex is the controlling player
            int playerIndex = (int)ControllingPlayer.Value;
            //set new keyboard and controller state objects
            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];
            //set a new boolean as the current connection state of the game pad
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];
            //of the gamepad is disconnected or the game is paused then then
            //pause the game
            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
                ScreenManager.AddScreen(PauseMenu, ControllingPlayer);
            //if game is not paused
            else
            {
                //Check if controller should be vibrating, vibrate it
                //Otherwise remove vibration
                if (Vibrate)
                    GamePad.SetVibration(ControllingPlayer.Value, 0.5f, 0.5f);
                else
                    GamePad.SetVibration(ControllingPlayer.Value, 0, 0);

                //if the left trigger or left alt key is held down then switch to top
                //down view
                if (gamePadState.Triggers.Left > 0 || keyboardState.IsKeyDown(Keys.LeftAlt))
                {
                    if (!CamPressed)
                    {
                        if (isTopDown)
                        {
                            Cam.MainCam();
                            isTopDown = false;
                        }
                        else
                        {
                            Cam.TopDown();
                            isTopDown = true;
                        }
                        CamPressed = true;
                    }
                }
                else
                    CamPressed = false;
                //update the camera
                Cam.Update();

                //Set the player velocity to either the X value of the left 
                //thumbstick or the left or right key
                Player.SetVelocity(new Vector3(0.2f * gamePadState.ThumbSticks.Left.X, 0, 0));

                if (keyboardState.IsKeyDown(Keys.Left))
                    Player.SetVelocity(new Vector3(-0.2f, 0, 0));
                if (keyboardState.IsKeyDown(Keys.Right))
                    Player.SetVelocity(new Vector3(0.2f, 0, 0));

                //if the player collides with the arena or the player collides with
                //the ball with a velocity*2 then stop the player
                if (Player.InternalCollision(Arena) || Player.ExternalCollision(Ball, 2))
                    Player.SetVelocity(Vector3.Zero);

                //if the player is carrying the ball and either the A button or
                //space bar is pressed then the ball is released
                if (SetPlayerCarry && gamePadState.IsButtonDown(Buttons.A) || SetPlayerCarry && keyboardState.IsKeyDown(Keys.Space))
                {
                    if (Ball.GetPosition() == new Vector3(Player.GetPosition().X + (Player.GetSize().X / 2), Ball.GetPosition().Y, 14.5f))
                    {
                        Ball.SetVelocity(new Vector3(0.15f, 0, -0.15f));
                        Ball.SetSpeed(Ball.GetVelocity().X + -Ball.GetVelocity().Z);
                        SetPlayerCarry = false;
                    }
                }

                //if the background music is not being changed and the right shoulder
                //button or the W key is pressed then pause the current song, and
                //go to the next one. set the background music to being changed.
                //also check if the song is at the end in which case set to first
                //song again
                if (!SwitchCue)
                {
                    if (gamePadState.IsButtonDown(Buttons.RightShoulder)||keyboardState.IsKeyDown(Keys.W))
                    {
                        if (!PauseCue)
                        {
                            SwitchCue = true;
                            BGM[currentcue].Pause();
                            if (currentcue < BGM.Length - 1)
                                currentcue++;
                            else
                                currentcue = 0;
                        }
                    }
                    //if the background music is not being changed and the left shoulder
                    //button or the Q key is pressed then pause the current song, and
                    //go to the previous one. set the background music to being changed.
                    //also check if the song is at the beginning in which case set to the
                    //last song again
                    if (gamePadState.IsButtonDown(Buttons.LeftShoulder) || keyboardState.IsKeyDown(Keys.Q))
                    {
                        if (!PauseCue)
                        {
                            SwitchCue = true;
                            BGM[currentcue].Pause();
                            if (currentcue != 0)
                                currentcue--;
                            else
                                currentcue = BGM.Length - 1;
                        }
                    }
                }
                    //if the song is being changed and all song modifier buttons are released
                    //then set the song to not being changed
                else
                    if (gamePadState.IsButtonUp(Buttons.RightShoulder) && gamePadState.IsButtonUp(Buttons.LeftShoulder) && keyboardState.IsKeyUp(Keys.Q) && keyboardState.IsKeyUp(Keys.W))
                        SwitchCue = false;

                //if the music is not off and down on the dpad or the A key is pressed
                //then turn off the music and change the music animation to something that looks off
                if (!PauseCue)
                {
                    if (gamePadState.IsButtonDown(Buttons.DPadDown) || keyboardState.IsKeyDown(Keys.A))
                    {
                        PauseCue = true;
                        MusicPlayer = new clsDrops(15, tMusicPlayerOff, new Vector2(435, 133), new Vector2(155, 24), Vector2.Zero, 0);
                        MusicPlayer.Show();
                    }
                }
                //if the music is turned off and up on the dpad or the S key is press
                 //then turn on the music and change the music animation to something that looks on
                else
                {
                    if (gamePadState.IsButtonDown(Buttons.DPadUp) || keyboardState.IsKeyDown(Keys.S))
                    {
                        PauseCue = false;
                        MusicPlayer = new clsDrops(15, tMusicPlayer, new Vector2(435, 133), new Vector2(155, 24), Vector2.Zero, 4);
                        MusicPlayer.Show();
                    }
                }

            }
        }
        
                    public override void Draw(GameTime gameTime)
                    {
                        //Set white background
                        ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                                           Color.White, 0, 0);
                        ScreenManager.GraphicsDevice.RenderState.DepthBufferEnable = true;
                        ScreenManager.GraphicsDevice.RenderState.CullMode = CullMode.None;
                        //Draw the arena model
                        Arena.Draw(Cam, graphicsDevice, spriteBatch, gameFont);

                        //for all of the blocks further away from the camera than ball, render before it
                        for (int i = 0; i < Blocks.Length; i++)
                            if (Blocks[i].GetPosition().Z < Ball.GetPosition().Z)
                                Blocks[i].Draw(Cam, graphicsDevice, spriteBatch, gameFont);
                        //render the ball
                        Ball.Draw(Cam, graphicsDevice, spriteBatch, gameFont);
                        //for all of the blocks closer to the camera than the ball, render after it
                        for (int i = 0; i < Blocks.Length; i++)
                            if (Blocks[i].GetPosition().Z >= Ball.GetPosition().Z)
                                Blocks[i].Draw(Cam, graphicsDevice, spriteBatch, gameFont);
                        //Draw the player model
                        Player.Draw(Cam, graphicsDevice, spriteBatch, gameFont);
                        //Draw the kill zone model
                        KillZone.Draw(Cam, graphicsDevice, spriteBatch, gameFont);
                        //Draw as many health icons as there are lives left for the player
                        for (int i = 0; i < Player.GetHealth(); i++)
                            HealthViewer.Draw(Cam, new Vector3(10.5f, 2, 19 - (1.5f * i)));

                        //begin spritebatech
                        spriteBatch.Begin();
                        //Draw any powerups that should be showing
                        for (int i = 0; i < Blocks.Length; i++)
                                Blocks[i].PowerUp.Draw(spriteBatch, gameTime);
                        //if the player or ball frame are equal to what power up gives them
                        //then render that power up in the HUD as white, if the power up
                        //is not equal to the frame then render it as red
                        if(Player.GetFrame() == 1)
                            spriteBatch.Draw(Expander, new Vector2(300, 682), sourceRect, Color.White);
                        else
                        spriteBatch.Draw(Expander, new Vector2(300, 682), sourceRect, Color.Red);

                        if (Player.GetFrame() == 2)
                            spriteBatch.Draw(Decreaser, new Vector2(370, 682), sourceRect, Color.White);
                        else
                            spriteBatch.Draw(Decreaser, new Vector2(370, 682), sourceRect, Color.Red);

                        if (Ball.GetFrame() == 1)
                            spriteBatch.Draw(AcidBall, new Vector2(440, 682), sourceRect, Color.White);
                        else
                            spriteBatch.Draw(AcidBall, new Vector2(440, 682), sourceRect, Color.Red);

                        if (Ball.GetFrame() == 2)
                            spriteBatch.Draw(GoldenBall, new Vector2(510, 682), sourceRect, Color.White);
                        else
                            spriteBatch.Draw(GoldenBall, new Vector2(510, 682), sourceRect, Color.Red);

                        if (Ball.GetFrame() == 3)
                            spriteBatch.Draw(BiggerBall, new Vector2(580, 682), sourceRect, Color.White);
                        else
                            spriteBatch.Draw(BiggerBall, new Vector2(580, 682), sourceRect, Color.Red);

                        if (Ball.GetFrame() == 4)
                            spriteBatch.Draw(SmallerBall, new Vector2(650, 682), sourceRect, Color.White);
                        else
                            spriteBatch.Draw(SmallerBall, new Vector2(650, 682), sourceRect, Color.Red);

                        //render the current ball speed
                        spriteBatch.DrawString(HUDfont, "Ball Speed: " + (Math.Floor(Ball.GetSpeed() * 100)), new Vector2(361, 90), Color.Black);
                        ///render the current score
                        spriteBatch.DrawString(HUDfont, "Score: " + Score, new Vector2(357, 60), Color.Red);
                        //draw the music player animation
                        MusicPlayer.Draw(spriteBatch, gameTime);
                        //if in breakout mode, show the level number and name in the HUD
                        if (GameType <= 0)
                        {
                            spriteBatch.DrawString(HUDfont, "Level: " + (Level + 1) + "/10", new Vector2(553, 60), Color.Red);
                            spriteBatch.DrawString(HUDfont, LevelName, new Vector2(355, 30), Color.Black);
                        }
                        //if in assault mode, show the time delay between lines in the HUD
                        else
                            spriteBatch.DrawString(HUDfont, "Data Interval: " + timespan.ToString("#.##"), new Vector2(357, 60), Color.Black);
                        //if the music is off render the current track as off
                        //otherwise display the current music track
                        if(PauseCue)
                            spriteBatch.DrawString(HUDfont, "Music Off", new Vector2(460, 114), Color.Red);
                        else
                        spriteBatch.DrawString(HUDfont, "Track: 0" + (currentcue+1), new Vector2(464, 114), Color.Red);
                        spriteBatch.End();
                    }
        #endregion
    }
}
