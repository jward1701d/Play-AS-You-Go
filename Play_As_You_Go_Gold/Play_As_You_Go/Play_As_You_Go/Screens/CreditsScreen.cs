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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

#endregion

#region Namespace

namespace Play_As_You_Go
{
    #region Class

    public class CreditsScreen : GameScreen
    {
        #region Methods and Properties

        VideoPlayer videoPlayer;
        Video video;
        Texture2D videoTexture;
        Vector2 videoPosition = new Vector2(0, 0);
        TimeSpan creditsTime;

        public TimeSpan CreditsTime
        {
            get { return creditsTime; }
            set { creditsTime = value; }
        }

        #endregion

        public CreditsScreen()
        {
            TransitionOnTime = TimeSpan.FromMilliseconds(100);
            TransitionOffTime = TimeSpan.FromMilliseconds(100);

        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            video = content.Load<Video>("Images/Xbox Credits w Audio New a");
            videoPlayer = new VideoPlayer();
        }

        public override void UnloadContent()
        {
            videoTexture = null;
        }


        public override void Update(GameTime gameTime, bool covered)
        {
            if (ScreenState == ScreenState.Active)
            {
                if (videoPlayer.State == MediaState.Stopped)//added new
                {
                    videoPlayer.IsLooped = false;
                    videoPlayer.Play(video);
                }
                creditsTime = creditsTime.Subtract(gameTime.ElapsedGameTime);
                if (creditsTime.TotalSeconds <= 0)
                {
                    videoPlayer.Stop();
                    ExitScreen();
                }

            }

            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            int playerIndex = (int)ControllingPlayer.Value;
            
            KeyboardState keyboardState = ScreenManager.InputSystem.currentKeyboardState[playerIndex];
            GamePadState gamepadState = ScreenManager.InputSystem.currentGamePadState[playerIndex];

            if(ScreenManager.InputSystem.Cancel(ControllingPlayer))
            {
                videoPlayer.Stop();
                ExitScreen();
            }            

            base.Update(gameTime, covered);
        }


        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.Black);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            if (videoPlayer.State != MediaState.Stopped)
            {
                videoTexture = videoPlayer.GetTexture();
            }
            if (videoTexture != null)
            {
                spriteBatch.Begin();

                spriteBatch.Draw(videoTexture, videoPosition, Color.White);

                spriteBatch.End();
            }

        }
    }

    #endregion
}

#endregion