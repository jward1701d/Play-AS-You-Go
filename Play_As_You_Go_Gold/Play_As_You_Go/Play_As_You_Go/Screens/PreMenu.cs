using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Play_As_You_Go
{
    class PreMenu : MenuScreen
    {
        Texture2D backgroundTexture;
        SpriteFont TitleFont;
        SpriteFont controllerFont;
        //Texture2D pressStart;

        Vector2 backgroundPosition;
        Vector2 titlePosition;

        string title;
        string startButton = "%";
        string aButton = "'";

        float transitionTime;

        double timer = 0.035d;
        double bigTimer = 10.0d;
        int colorValue = 149;
        int fadeCount = 3;
        double timerCount = 86.80d; 

        MenuEntry start;

        SoundEffect sound;
        SoundEffectInstance music;

        SpriteFont spriteFont;

        float volume = 0.15f;

        #region Methods and Properties

       
        ISsystem checking = new ISsystem();
        
       

        #endregion

        

        public PreMenu()
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

        }

        public override void Initialize()
        {
            backgroundPosition = new Vector2(0, 0);
            titlePosition = new Vector2(275, 80);

            title = "Play As You Go";

            start = new MenuEntry(this, "");
            start.SetPosition(new Vector2(500, 360), true);
            start.Selected += StartSelect;
            MenuEntries.Add(start);

            music = sound.CreateInstance();
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            spriteFont = content.Load<SpriteFont>("Fonts/SmallFont");
            TitleFont = content.Load<SpriteFont>("Fonts/BigBoldFont");
            controllerFont = content.Load<SpriteFont>("Fonts/xboxControllerSpriteFont");

            backgroundTexture = content.Load<Texture2D>("Backgrounds/PlayAsYouGoMainMenu");
            //pressStart = content.Load<Texture2D>("Images/PressStart");
            sound = content.Load<SoundEffect>("Audio/cold2");

            
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (MediaPlayer.GameHasControl)
            {
                music.Volume = volume;
                music.Play();
            }
            else
                music.Pause();
                
            


            this.timer -= gameTime.ElapsedGameTime.TotalSeconds;
            this.bigTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (timer <= 0)
            {
                timer = .035d;
                colorValue += fadeCount;
                if (colorValue >= 255 || colorValue <= 149)
                {
                    fadeCount *= -1;
                }

            }
            if (bigTimer <= 0)
            {
                music.Stop();
                Remove();
                ScreenManager.AddScreen(new DemoVid(9), ControllingPlayer);
            }

            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime, covered);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            string pressStartMessage = "Press      " + "or      " + "to start";
            spriteBatch.Begin();
            
           
                spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);

                //spriteBatch.Draw(pressStart, new Vector2(525, 360), new Color(colorValue, colorValue, colorValue));
                spriteBatch.DrawString(controllerFont, startButton, new Vector2((ScreenManager.Viewport.Width / 2) - 24, (ScreenManager.Viewport.Height / 2) - 33), new Color(colorValue, colorValue, colorValue), 0f, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(controllerFont, aButton, new Vector2((ScreenManager.Viewport.Width / 2) + 58, (ScreenManager.Viewport.Height / 2) - 33), new Color(colorValue, colorValue, colorValue), 0f, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(spriteFont, pressStartMessage, new Vector2((ScreenManager.Viewport.Width / 2) - 100, (ScreenManager.Viewport.Height / 2)), new Color(colorValue, colorValue, colorValue));
                spriteBatch.DrawString(spriteFont, "Play As You Go Version 1.1.0 Copyright 2011 Scrubby Fresh Studios LLC", new Vector2(45, 670), Color.White, 0f, new Vector2(0, 0), 0.70f, SpriteEffects.None, 1.0f); 
                spriteBatch.DrawString(TitleFont, title, titlePosition, Color.Black);
            
            spriteBatch.End();
        }

        void StartSelect(object sender, PlayerIndexEventArgs e)
        {
            checking.checkFile();
            if (checking.isSaveable)
            {
                music.Stop();
                Remove();
                ScreenManager.AddScreen(new PlayAsYouGoMainMenu(), e.PlayerIndex);
            }
            else
            {
                music.Stop();
                Remove();
                ScreenManager.AddScreen(new FirstRunScreen(), e.PlayerIndex);
            }
        }
        
        void MenuRemove(object sender, PlayerIndexEventArgs e)
        {
           MenuEntries.Clear();
       }
    }
}

