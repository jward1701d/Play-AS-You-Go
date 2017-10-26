#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
#endregion

namespace Play_As_You_Go
{
    /// <summary>
    /// The loading screen coordinates transitions between the menu system and the
    /// game itself. Normally one screen will transition off at the same time as
    /// the next screen is transitioning on, but for larger transitions that can
    /// take a longer time to load their data, we want the menu system to be entirely
    /// gone before we start loading the game. This is done as follows:
    /// 
    /// - Tell all the existing screens to transition off.
    /// - Activate a loading screen, which will transition on at the same time.
    /// - The loading screen watches the state of the previous screens.
    /// - When it sees they have finished transitioning off, it activates the real
    ///   next screen, which may take a long time to load its data. The loading
    ///   screen will be the only thing displayed while this load is taking place.
    /// </summary>
    class LoadingScreen : GameScreen
    {
        #region Fields

        bool loadingIsSlow;
        bool otherScreensAreGone;

        GameScreen[] screensToLoad;

        GameObject _object;
        Texture2D texture;

        SpriteFont font;
        Vector2 fontPosition;
        string loading;

        VideoPlayer videoPlayer;
        Video video;
        Texture2D videoTexture;
        Vector2 videoPosition;

        #endregion

        #region Initialization


        /// <summary>
        /// The constructor is private: loading screens should
        /// be activated via the static Load method instead.
        /// </summary>
        private LoadingScreen(ScreenManager screenManager, bool loadingIsSlow,
                              GameScreen[] screensToLoad)
        {
            this.loadingIsSlow = loadingIsSlow;
            this.screensToLoad = screensToLoad;

            TransitionOnTime = TimeSpan.FromSeconds(6.0);
        }

        public override void Initialize()
        {
            _object = new GameObject(texture, new Rectangle(0, 17, 638, 87), 3, true, 1);
            _object.Position = new Vector2(640, 590);
            _object.Origin = new Vector2((_object.ActualWidth / 2) / _object.Scale, (_object.ActualHeight / 2) / _object.Scale);
            _object.Scale = 0.5f;
            _object.AnimationSpeed = TimeSpan.FromSeconds(1.5);

            loading = "Loading . . .";
            fontPosition = new Vector2(600, 615);

            videoPosition = new Vector2(0, -20);
            videoPlayer = new VideoPlayer();
            videoPlayer.IsLooped = true;
        }
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            texture = content.Load<Texture2D>("Images/LoadingBar");
            font = content.Load<SpriteFont>("Fonts/SmallFont");

            video = content.Load<Video>("Images/ShortLogo");           

        }

        /// <summary>
        /// Activates the loading screen.
        /// </summary>
        public static void Load(ScreenManager screenManager, bool loadingIsSlow,                                
                               PlayerIndex? controllingPlayer, params GameScreen[] screensToLoad)
        {
            // Tell all the current screens to transition off.
            foreach (GameScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            LoadingScreen loadingScreen = new LoadingScreen(screenManager,
                                                            loadingIsSlow,
                                                            screensToLoad);

            screenManager.AddScreen(loadingScreen, controllingPlayer);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the loading screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool covered)
        {
            if (videoPlayer.State == MediaState.Stopped)//added new
            {
                videoPlayer.IsLooped = false;
                videoPlayer.Play(video);
            }

            base.Update(gameTime,covered);

            _object.Update(gameTime);

            // If all the previous screens have finished transitioning
            // off, it is time to actually perform the load.
            if (otherScreensAreGone)
            {
                ScreenManager.RemoveScreen(this);

                foreach (GameScreen screen in screensToLoad)
                {
                    if (screen != null)
                    {
                        ScreenManager.AddScreen(screen, ControllingPlayer);

                       
                    }                    
                }

                
                // Once the load has finished, we use ResetElapsedTime to tell
                // the  game timing mechanism that we have just finished a very
                // long frame, and that it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
        }


        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            // If we are the only active screen, that means all the previous screens
            // must have finished transitioning off. We check for this in the Draw
            // method, rather than in Update, because it isn't enough just for the
            // screens to be gone: in order for the transition to look good we must
            // have actually drawn a frame without them before we perform the load.
            if ((ScreenState == ScreenState.Active) &&
                (ScreenManager.GetScreens().Length == 1))
            {
                otherScreensAreGone = true;
                               
            }

            if (videoPlayer.State != MediaState.Stopped)
            {
                videoTexture = videoPlayer.GetTexture();
            }                
                // The gameplay screen takes a while to load, so we display a loading
                // message while that is going on, but the menus load very quickly, and
                // it would look silly if we flashed this up for just a fraction of a
                // second while returning from the game to the menus. This parameter
                // tells us how long the loading is going to take, so we know whether
                // to bother drawing the message.
            if (loadingIsSlow)
            {
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;


                Color color = Color.White * TransitionAlpha;


                // Draw the text.
                spriteBatch.Begin();

                if (videoTexture != null)
                {
                    spriteBatch.Draw(videoTexture, videoPosition, Color.White);
                }
                _object.Draw(gameTime, spriteBatch);
                spriteBatch.DrawString(font, loading, fontPosition, Color.Green);


                spriteBatch.End();

            }
        }


        #endregion
    }
}
