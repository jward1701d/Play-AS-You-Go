﻿#region Using Statements
using System;
using System.Xml.Serialization;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Tasks;
#endregion

namespace PAYG
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

        Texture2D texture;
        Texture2D loadingBarTexture;

        bool loadingIsSlow;
        bool otherScreensAreGone;

        GameScreen[] screensToLoad;

        GameObject logo;
        GameObject loadingBar;

        SpriteFont font;
        Vector2 fontPosition;
        string loading;

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
            loading = "Loading . . .";

            logo = new GameObject(texture, new Rectangle(0, 0, 256, 154), 8, true, 0);
            logo.Origin = new Vector2((logo.ActualWidth / 2) / logo.Scale, (logo.ActualHeight / 2) / logo.Scale);
            logo.Position = new Vector2(ScreenManager.Viewport.Width / 2, ScreenManager.Viewport.Height / 2);
            logo.AnimationSpeed = TimeSpan.FromMilliseconds(400);
            logo.Scale = 3.0f;
            logo.Active = true;

            fontPosition = new Vector2(300, 450);

            loadingBar = new GameObject(loadingBarTexture, new Rectangle(0, 17, 638, 87), 3, true, 1);
            loadingBar.Position = new Vector2(400, 425);
            loadingBar.Origin = new Vector2((loadingBar.ActualWidth / 2) / loadingBar.Scale, (loadingBar.ActualHeight / 2) / loadingBar.Scale);
            loadingBar.Scale = 0.35f;
            loadingBar.AnimationSpeed = TimeSpan.FromSeconds(1.5);

        }
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            texture = content.Load<Texture2D>("Backgrounds/LogoSpritesheet");
            font = content.Load<SpriteFont>("Fonts/SmallFont");
            loadingBarTexture = content.Load<Texture2D>("Images/LoadingBar");
            
        }

        /// <summary>
        /// Activates the loading screen.
        /// </summary>
        public static void Load(ScreenManager screenManager, bool loadingIsSlow,                                
                                params GameScreen[] screensToLoad)
        {
            // Tell all the current screens to transition off.
            foreach (GameScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            LoadingScreen loadingScreen = new LoadingScreen(screenManager,
                                                            loadingIsSlow,
                                                            screensToLoad);

            screenManager.AddScreen(loadingScreen);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the loading screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool covered)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && !ScreenManager.isPressed)
            {
                //ScreenManager.isPressed = true;
                //ScreenManager.AddScreen(new PauseScreen(this));
                Remove();
                ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
            }
            /*else if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && ScreenManager.isPressed)
            {
                ScreenManager.isPressed = false;
                
            }*/
            
            base.Update(gameTime,covered);

            logo.Update(gameTime);
            loadingBar.Update(gameTime);

            // If all the previous screens have finished transitioning
            // off, it is time to actually perform the load.
            if (otherScreensAreGone)
            {
                ScreenManager.RemoveScreen(this);

                foreach (GameScreen screen in screensToLoad)
                {
                    if (screen != null)
                    {
                        ScreenManager.AddScreen(screen);
                       
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

                logo.Draw(gameTime, spriteBatch);
                loadingBar.Draw(gameTime, spriteBatch);
                spriteBatch.DrawString(font, loading, fontPosition, Color.Green);

                spriteBatch.End();
            }
        }

        #endregion
    }
}
