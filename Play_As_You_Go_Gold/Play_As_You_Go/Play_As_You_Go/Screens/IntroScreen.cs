#region Info

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

#endregion

#region Namespace

namespace Play_As_You_Go
{
    #region Class

    public class IntroScreen : GameScreen
    {
        #region Methods and Properties

        VideoPlayer videoPlayer;
        Video video;
        Texture2D videoTexture;
        Vector2 videoPosition = new Vector2(125, 0);
        TimeSpan screenTime;

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

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            video = content.Load<Video>("Images/test1");
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
                screenTime = screenTime.Subtract(gameTime.ElapsedGameTime);
                if (screenTime.TotalSeconds <= 0)
                {
                    videoPlayer.Stop();
                    ExitScreen();
                }

            }

            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
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