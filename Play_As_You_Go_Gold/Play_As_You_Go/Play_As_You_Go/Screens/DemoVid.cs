using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Play_As_You_Go
{
    class DemoVid : MenuScreen
    {
        #region Variable Feild

        #region Video based variables
        VideoPlayer vidPlayer;
        Video demoVid;
        Texture2D vidTexture;
        Vector2 vidPosition = new Vector2(0, 0);
        #endregion

        #region Timer Based varaibles
        double vidTimer = 86.80d;
        #endregion

        #region Audio track varibles
        SoundEffect vidSound; 
        SoundEffectInstance vidMusic;
        
        #endregion

        #region Other variables
        int menuNum;
        float transitionTime;
        SpriteFont controllerFont;
        SpriteFont spriteFont;
        string startButton = "%";
        string aButton = "'";
        int colorValue = 149;
        double timer = 0.035d;
        int fadeCount = 3;
        MenuEntry start;
        #endregion

        #endregion

        public DemoVid(int menu)
        {
            //transitionTime = 1.5f;
            //TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            // TransitionOffTime = TimeSpan.Zero;
            menuNum = menu;
            
            //base.Initialize(menuNumber);
        }

        public override void Initialize()
        {
          // menuNum = menu;
            start = new MenuEntry(this, "");
            start.SetPosition(new Vector2(500, 360), true);
            start.Selected += StartSelect;
            MenuEntries.Add(start);

            vidMusic = vidSound.CreateInstance();
            vidMusic.Volume = 0.15f;
            vidMusic.IsLooped = false;
           base.Initialize();
        }
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            vidSound = content.Load<SoundEffect>("Audio/Whatever");
            
            spriteFont = content.Load<SpriteFont>("Fonts/SmallFont");
            controllerFont = content.Load<SpriteFont>("Fonts/xboxControllerSpriteFont");
            demoVid = content.Load<Video>("Images/PromoReel");
            vidPlayer = new VideoPlayer();
        }
        public override void UnloadContent()
        {
            vidTexture = null;
        }
        public override void Update(GameTime gameTime, bool covered)
        {
            
            if (MediaPlayer.GameHasControl)
            {
                vidMusic.Play();
            }
            else
                vidMusic.Pause();
            

            vidPlayer.IsLooped = false;
            vidPlayer.Play(demoVid);
            

            this.timer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (timer <= 0)
            {
                timer = .035d;
                colorValue += fadeCount;
                if (colorValue >= 255 || colorValue <= 149)
                {
                    fadeCount *= -1;
                }

            }
            this.vidTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (vidTimer <= 0.0f)
            {
                vidPlayer.Stop();
                vidMusic.Stop();
                if (menuNum == 0)
                {
                    Remove();
                    ScreenManager.AddScreen(new PlayAsYouGoMainMenu(), ControllingPlayer);
                }
                if (menuNum == 1)
                {
                    Remove();
                    ScreenManager.AddScreen(new BSMainMenu(), ControllingPlayer);
                }
                if (menuNum == 2)
                {
                    Remove();
                    ScreenManager.AddScreen(new ColorMimicMainMenu(), ControllingPlayer);
                }
                if (menuNum == 3)
                {
                    Remove();
                    ScreenManager.AddScreen(new H1N1MenuScreen(), ControllingPlayer);
                }
                if (menuNum == 9)
                {
                    Remove();
                    ScreenManager.AddScreen(new PreMenu(), ControllingPlayer);
                }


            }
            base.Update(gameTime, covered);
        }
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            string pressStartMessage = "Press      " + "or      " + "to start";
            spriteBatch.Begin();
            if (vidPlayer.State != MediaState.Stopped)
            {
                vidTexture = vidPlayer.GetTexture();
            }
            if (vidTexture != null)
            {
                spriteBatch.Draw(vidTexture, vidPosition, Color.White);
                spriteBatch.DrawString(controllerFont, startButton, new Vector2((ScreenManager.Viewport.Width / 2) - 24, (ScreenManager.Viewport.Height / 2) + 265), new Color(colorValue, colorValue, colorValue), 0f, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(controllerFont, aButton, new Vector2((ScreenManager.Viewport.Width / 2) + 58, (ScreenManager.Viewport.Height / 2) + 265), new Color(colorValue, colorValue, colorValue), 0f, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(spriteFont, pressStartMessage, new Vector2((ScreenManager.Viewport.Width / 2) - 100, (ScreenManager.Viewport.Height / 2) + 300), new Color(colorValue, colorValue, colorValue));
            }
            spriteBatch.End();
        }
        void StartSelect(object sender, PlayerIndexEventArgs e)
        {
            vidPlayer.Stop();
            vidMusic.Stop();
            if (menuNum == 0)
            {
                Remove();
                ScreenManager.AddScreen(new PlayAsYouGoMainMenu(), ControllingPlayer);
            }
            if (menuNum == 1)
            {
                Remove();
                ScreenManager.AddScreen(new BSMainMenu(), ControllingPlayer);
            }
            if (menuNum == 2)
            {
                Remove();
                ScreenManager.AddScreen(new ColorMimicMainMenu(), ControllingPlayer);
            }
            if (menuNum == 3)
            {
                Remove();
                ScreenManager.AddScreen(new H1N1MenuScreen(), ControllingPlayer);
            }
            if (menuNum == 9)
            {
                Remove();
                ScreenManager.AddScreen(new PreMenu(), ControllingPlayer);
            }
        }
    }
}
