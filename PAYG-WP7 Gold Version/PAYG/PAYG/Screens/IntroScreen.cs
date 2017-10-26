#region Info

#endregion


#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Phone.Tasks;

#endregion

#region Namespace

namespace PAYG
{
    #region Class

    public class IntroScreen : GameScreen
    {
        #region Methods and Properties  

        TimeSpan screenTime;
        Texture2D logotexture;

        GameObject logo;

        SoundEffect logosound;
        SoundEffectInstance effectLogo;

        public TimeSpan ScreenTime
        {
            get { return screenTime; }
            set { screenTime = value; }
        }

        #endregion

        public IntroScreen()
        {
            TransitionOnTime = TimeSpan.FromMilliseconds(100);
            TransitionOffTime = TimeSpan.FromMilliseconds(100);
        }

        public override void Initialize()
        {
            logo = new GameObject(logotexture, new Rectangle(0, 0, 256, 154), 8, true, 0);
            logo.Origin = new Vector2((logo.ActualWidth / 2) / logo.Scale, (logo.ActualHeight / 2) / logo.Scale);
            logo.Position = new Vector2(ScreenManager.Viewport.Width / 2, ScreenManager.Viewport.Height / 2);
            logo.AnimationSpeed = TimeSpan.FromMilliseconds(400);
            logo.Scale = 3.0f;
            logo.Active = false;

            
            effectLogo = logosound.CreateInstance();
            effectLogo.IsLooped = false;            
            effectLogo.Volume = ScreenManager.FXVolume;
            
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            logotexture = content.Load<Texture2D>("Backgrounds/LogoSpritesheet");
            logosound = content.Load<SoundEffect>("Audio/Logosound01");

            base.LoadContent();            
        }

        public override void UnloadContent()
        {
            logotexture = null;
            effectLogo = null;
        }


        public override void Update(GameTime gameTime, bool covered)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                ScreenManager.Game.Exit();
            } 
            
            logo.Update(gameTime);           

            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (ScreenState == ScreenState.Active)
            {   
                screenTime = screenTime.Subtract(gameTime.ElapsedGameTime);

                if (screenTime.TotalSeconds <= 0)
                {
                    
                    effectLogo.Stop();
                    Remove();                    
                }
                else if (screenTime.TotalSeconds <= 5)
                {                   
                    logo.Active = true;
                    if (MediaPlayer.GameHasControl)
                    {
                        effectLogo.Play();
                    }
                    else
                    {
                        effectLogo.Pause();
                    }                   
                }

                if (logo.Active)
                {
                    if (screenTime.TotalSeconds <= 0.5)
                    {
                        logo.Active = false;
                    }
                }
            }                       

            base.Update(gameTime, covered);
        }


        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.Black);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            
            spriteBatch.Begin();

            logo.Draw(gameTime, spriteBatch);            

            spriteBatch.End();
        }
    }
    #endregion
}
#endregion