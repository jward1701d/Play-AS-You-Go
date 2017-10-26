#region Info
/**********************************************
 *                 Credit Screen              *  
 *                      By:                   *
 *        James Ward & Johnathan Witvoet      *
 *                                            *  
 *  Property of Scrubby Fresh Studios L.L.C.  *
 *  2011.                                     *  
 **********************************************/
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
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Phone.Tasks;

#endregion

#region Namespace

namespace PAYG
{
    #region Class

    public class CreditsScreen : GameScreen
    {
        #region Methods and Properties

        TimeSpan screenTime;
        Texture2D creditTexture;

        GameObject creditsObject;

        SoundEffect creditsound;
        SoundEffectInstance effectCredit;
        //float creditvolume = 1.0f;

        Texture2D backTexture;
        GameObject backObject;

        SpriteFont backfont;
        String back;
        Vector2 backposition;

        public TimeSpan CreditsTime
        {
            get { return screenTime; }
            set { screenTime = value; }
        }

        #endregion

        public CreditsScreen()
        {
            TransitionOnTime = TimeSpan.FromMilliseconds(100);
            TransitionOffTime = TimeSpan.FromMilliseconds(100);

        }

        public override void Initialize()
        {
            creditsObject = new GameObject(creditTexture, new Rectangle(0, 0, 630, 2048), 1, true, 0);
            creditsObject.Position = new Vector2(170, ScreenManager.Viewport.Height + 50);
            creditsObject.Velocity = new Vector2(0, -2.10f);
            creditsObject.Scale = .75f;
            creditsObject.Active = true;

            effectCredit = creditsound.CreateInstance();
            effectCredit.IsLooped = false;
            effectCredit.Volume = ScreenManager.MusicVolume; //creditvolume;

            backObject = new GameObject(backTexture, new Rectangle(158, 0, 32, 32), 1, true, 0);
            backObject.Position = new Vector2(50, 340);
            backObject.Scale = 2.0f;

            back = "Back";
            backposition = new Vector2(50, 400);
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            creditTexture = content.Load<Texture2D>("Images/CreditscreenSpritesheet01");
            creditsound = content.Load<SoundEffect>("Audio/PAYGCreditscreen");
            backTexture = content.Load<Texture2D>("Images/HighScoreInterfaceButtonsSpriteSheet");
            backfont = content.Load<SpriteFont>("Fonts/SmallFont");

        }

        public override void UnloadContent()
        {
            creditTexture = null;
        }


        public override void Update(GameTime gameTime, bool covered)
        {
            if (ScreenState == ScreenState.Active)
            {
                effectCredit.Play();

                creditsObject.Update(gameTime);

                screenTime = screenTime.Subtract(gameTime.ElapsedGameTime);

                if (screenTime.TotalSeconds <= 0)
                {
                    effectCredit.Stop();
                    Remove();
                }

            }

            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            backObject.Update(gameTime);

            if (ScreenManager.InputSystem.IsRectanglePressed(this.backObject.BoundingRect))
            {
                effectCredit.Stop();
                Remove();
                ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
            }
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                effectCredit.Stop();
                Remove();
                ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
            }

            base.Update(gameTime, covered);
        }


        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.Black);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            creditsObject.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(backfont, back, backposition, Color.White);
            backObject.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }
    }

    #endregion
}

#endregion