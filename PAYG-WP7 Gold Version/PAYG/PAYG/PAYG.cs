using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

using System.Collections;
using System.Runtime.Serialization; //needed for data contracts
using System.Xml;

using Microsoft.Advertising.Mobile.Xna;

namespace PAYG
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PAYG : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private static readonly string AppID = "7bf8dd7f-c011-4c34-8919-87d79918e6d0";
        ScreenManager screenManager;
        StartupMode mode = PhoneApplicationService.Current.StartupMode;
        GameType currentGame;
        ISsystem checkFile = new ISsystem();
        public static PAYG Current { get; private set; }

       
        
        public PAYG()
        {
            Guide.SimulateTrialMode = false;

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.IsFullScreen = true;

            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            Content.RootDirectory = "Content";

            Current = this;

            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);

            AdGameComponent.Initialize(this, AppID);
            Components.Add(AdGameComponent.Current);
            screenManager.AdGameComponent = AdGameComponent.Current;

            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            PhoneApplicationService.Current.Deactivated += new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(SaveState);
            
            if (mode == StartupMode.Activate)
            {
                if (PhoneApplicationService.Current.State.ContainsKey("ActiveGame"))
                {
                    currentGame = (GameType)PhoneApplicationService.Current.State["ActiveGame"];
                    // Added to geive a general reset to the volume value if tombstone.
                    checkFile.checkFile();
                    if (MediaPlayer.GameHasControl)
                    {
                        if (checkFile.isSaveable)
                        {
                            checkFile.LoadOptions(checkFile.isSaveable);
                        }
                        else
                        {
                            screenManager.FXVolume = 0.5f;
                            screenManager.FXEnabled = true;
                            screenManager.MusicVolume = 0.5f;
                            screenManager.MusicEnabled = true;
                        }
                    }
                    else
                    {
                        screenManager.FXEnabled = false;
                        screenManager.MusicEnabled = false;
                    }
                    //To do:  Restore game data.
                    switch (currentGame)
                    {
                        case GameType.BEYONDSPACE:
                            int activeLevel = (int)PhoneApplicationService.Current.State["level"];
                            switch (activeLevel)
                            {
                                case 1:
                                    screenManager.AddScreen(new BSLevel1(true));
                                    break;
                                case 2:
                                    screenManager.AddScreen(new BSLevel2(true));
                                    break;
                                case 3:
                                    screenManager.AddScreen(new BSLevel3(true));
                                    break;
                                case 4:
                                    screenManager.AddScreen(new BSLevel4(true));
                                    break;
                                case 5:
                                    screenManager.AddScreen(new BSLevel5(true));
                                    break;
                                case 6:
                                    screenManager.AddScreen(new BSLevel6(true));
                                    break;
                                case 7:
                                    screenManager.AddScreen(new BSLevel7(true));
                                    break;
                                case 8:
                                    screenManager.AddScreen(new BSLevel8(true));
                                    break;
                                case 9:
                                    screenManager.AddScreen(new BSLevel9(true));
                                    break;
                                case 10:
                                    screenManager.AddScreen(new BSLevel10(true));
                                    break;
                                case 11:
                                    screenManager.AddScreen(new BSLevel11(true));
                                    break;
                                case 12:
                                    screenManager.AddScreen(new BSLevel12(true));
                                    break;
                                case 13:
                                    screenManager.AddScreen(new BSLevel13(true));
                                    break;
                                case 14:
                                    screenManager.AddScreen(new BSLevel14(true));
                                    break;
                                case 15:
                                    screenManager.AddScreen(new BSLevel15(true));
                                    break;
                                case 16:
                                    screenManager.AddScreen(new BSLevel16(true));
                                    break;
                                case 17:
                                    screenManager.AddScreen(new BSLevel17(true));
                                    break;
                                case 18:
                                    screenManager.AddScreen(new BSLevel18(true));
                                    break;
                                case 19:
                                    screenManager.AddScreen(new BSLevel19(true));
                                    break;
                                case 20:
                                    screenManager.AddScreen(new BSLevel20(true));
                                    break;
                                default:
                                    screenManager.AddScreen(new PlayAsYouGoMainMenu());
                                    break;

                            }//end switch
                            break;

                        case GameType.COLORMIMIC:
                            screenManager.AddScreen(new ColorMimicGame(true));
                            break;
                        case GameType.H1N1:
                            int H1N1activeLevel = (int)PhoneApplicationService.Current.State["hud.Level"];
                            switch (H1N1activeLevel)
                            {
                                case 1:
                                    screenManager.AddScreen(new H1N1Level_01(true));
                                    break;

                                case 2:
                                    screenManager.AddScreen(new H1N1Level_02(true));
                                    break;

                                case 3:
                                    screenManager.AddScreen(new H1N1Level_03(true));
                                    break;

                                case 4:
                                    screenManager.AddScreen(new H1N1Level_04(true));
                                    break;

                                case 5:
                                    screenManager.AddScreen(new H1N1Level_05(true));
                                    break;

                                case 6:
                                    screenManager.AddScreen(new H1N1Level_06(true));
                                    break;

                                case 7:
                                    screenManager.AddScreen(new H1N1Level_07(true));
                                    break;

                                case 8:
                                    screenManager.AddScreen(new H1N1Level_08(true));
                                    break;

                                case 9:
                                    screenManager.AddScreen(new H1N1Level_09(true));
                                    break;

                                case 10:
                                    screenManager.AddScreen(new H1N1Level_10(true));
                                    break;

                                case 11:
                                    screenManager.AddScreen(new H1N1Level_11(true));
                                    break;

                                case 12:
                                    screenManager.AddScreen(new H1N1Level_12(true));
                                    break;

                                case 13:
                                    screenManager.AddScreen(new H1N1Level_13(true));
                                    break;

                                case 14:
                                    screenManager.AddScreen(new H1N1Level_14(true));
                                    break;

                                case 15:
                                    screenManager.AddScreen(new H1N1Level_15(true));
                                    break;

                                case 16:
                                    screenManager.AddScreen(new H1N1Level_16(true));
                                    break;

                                case 17:
                                    screenManager.AddScreen(new H1N1Level_17(true));
                                    break;

                                case 18:
                                    screenManager.AddScreen(new H1N1Level_18(true));
                                    break;

                                case 19:
                                    screenManager.AddScreen(new H1N1Level_19(true));
                                    break;

                                case 20:
                                    screenManager.AddScreen(new H1N1Level_20(true));
                                    break;

                                default:
                                    screenManager.AddScreen(new PlayAsYouGoMainMenu());
                                    break;
                            }
                            break;
                        case GameType.HIGHSCOREINTERFACE:
                            {
                                screenManager.AddScreen(new HighScoreInterface(true));
                                break;
                            }
                        case GameType.MENU:
                            {
                                screenManager.AddScreen(new PlayAsYouGoMainMenu());
                                break;
                            }
                        default:
                            screenManager.AddScreen(new PlayAsYouGoMainMenu());
                            break;
                    }//end switch

                }
                else
                    screenManager.AddScreen(new PlayAsYouGoMainMenu());
            }//end if
            else
            {
                screenManager.AddScreen(new LogoScreen());
            }//end else

            Accelerometer.Initialize();
           
            TouchPanel.EnabledGestures =
               GestureType.Tap |
               GestureType.Hold |
               GestureType.DoubleTap |
               GestureType.FreeDrag |
               GestureType.Flick |
               GestureType.Pinch;

            if (!MediaPlayer.GameHasControl)
            {
                screenManager.MusicVolume = 0.0f;
                screenManager.MusicEnabled = false;
                screenManager.FXVolume = 0.0f;
                screenManager.FXEnabled = false;
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                /* Note : This exit command supercedes all other commands of this type so must be null to be overridden */
                //this.Exit(); 
            }            
                
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        protected void SaveState(object sender, DeactivatedEventArgs args)
        {
            PhoneApplicationService.Current.State["ActiveGame"] = GameType.MENU;
        }
    }
}
