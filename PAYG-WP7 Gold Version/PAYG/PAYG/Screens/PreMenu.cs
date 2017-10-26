using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace PAYG
{
    class PreMenu : MenuScreen
    {
        Texture2D backgroundTexture;
        SpriteFont TitleFont;
        Texture2D controllerFont;
       
        GameObject aButton;
        GameObject background;


        Vector2 titlePosition;

        string title;
       
        float transitionTime;

        double timer = 0.035d;
       
        int colorValue = 149;
        int fadeCount = 3;
        
        SoundEffect sound;
        SoundEffectInstance music;

        SpriteFont spriteFont;


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
            titlePosition = new Vector2(30, 20);

            background = new GameObject(backgroundTexture, new Rectangle(0, 0, 1280, 720), 1, true, 0);
            background.Position = new Vector2(0, 0);
            background.Scale = 0.67f;

            int controlStartPosX = 382;

            float controlScale = 1.5f;
            
            title = "Play As You Go";

            music = sound.CreateInstance();

            aButton = new GameObject(controllerFont, new Rectangle(95, 1, 32, 32), 1, true, 0);
            aButton.Position = new Vector2(controlStartPosX, 280); // Y = 335
            aButton.Origin = new Vector2((aButton.ActualWidth / 2) / aButton.Scale, (aButton.ActualHeight / 2) / aButton.Scale);
           
            aButton.Scale = controlScale;
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            spriteFont = content.Load<SpriteFont>("Fonts/SmallFont");
            TitleFont = content.Load<SpriteFont>("Fonts/BigBoldFont");
            controllerFont = content.Load<Texture2D>("Images/HighScoreInterfaceButtonsSpriteSheet");

            backgroundTexture = content.Load<Texture2D>("Backgrounds/PlayAsYouGoMainMenu");
            
            sound = content.Load<SoundEffect>("Audio/cold2");

            
        }

        public override void UnloadContent()
        {
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                ScreenManager.Game.Exit();
            } 
            if (MediaPlayer.GameHasControl)
            {
                music.Volume = ScreenManager.MusicVolume;
                music.Play();
            }
            else
                music.Pause();
                
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
            
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;


            if (ScreenManager.InputSystem.IsRectanglePressed(this.aButton.BoundingRect))
            {
                checking.checkFile();
                if (checking.isSaveable)
                {
                    music.Stop();
                    Remove();
                    ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
                }
                else
                {
                    music.Stop();
                    Remove();
                    ScreenManager.AddScreen(new FirstRunScreen());
                }
            }
            base.Update(gameTime, covered);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            aButton.Color = new Color(colorValue, colorValue, colorValue);
            string pressStartMessage = "Tap      " + "to start";
            
            spriteBatch.Begin();
            
            background.Draw(gameTime, spriteBatch);
                
            spriteBatch.DrawString(spriteFont, pressStartMessage, new Vector2((ScreenManager.Viewport.Width / 2) - 90, (ScreenManager.Viewport.Height / 2 )+25), new Color(colorValue, colorValue, colorValue)); // X=-150
            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.Black);
            
            aButton.Draw(gameTime, spriteBatch);
            
            spriteBatch.End();
        }
    }
}

