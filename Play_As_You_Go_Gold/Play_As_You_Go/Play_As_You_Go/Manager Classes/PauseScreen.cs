#region info
/******************************************************************************
 * PauseScreen.cs                                                             *
 *                                                                            *
 * Writen by: Johnathan Witvoet                                               *
 *                                                                            *
 ******************************************************************************/
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

#region Name Space

namespace Play_As_You_Go
{
    #region Class

    public class PauseScreen : MenuScreen
    {
        // hold the current GameScreen that is active
        GameScreen level;

        // helps with the transition on and off time
        float transitionTime;

        // 2 menu entries
        MenuEntry resume, quit;
        Texture2D singlePixel;

        public PauseScreen(GameScreen gameScreen)
        {
            this.level = gameScreen;

            transitionTime = 1.0f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            SelectedColor = Color.Yellow;
            NonSelectedColor = Color.White;
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            SpriteFont = content.Load<SpriteFont>("Fonts/SmallFont");
            singlePixel = content.Load<Texture2D>("Images/singlePixel");
        }
        public override void Initialize()
        {
            resume = new MenuEntry(this, "Resume Game");
            resume.SetPosition(new Vector2(550, 340), true);
            resume.Selected += ResumeSelect;
            MenuEntries.Add(resume);

            quit = new MenuEntry(this, "Quit Game");
            quit.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), resume, true);
            quit.Selected += QuitSelect;
            MenuEntries.Add(quit);
        }

        // Function for when player choices the Resume menu entry
        void ResumeSelect(object sender, PlayerIndexEventArgs e)
        {
            Remove();
            level.ScreenState = ScreenState.Active;
            
        }

        // Function for when player choices the Quit menu entry
        void QuitSelect(object sender, PlayerIndexEventArgs e)
        {
            Remove();
            level.Remove();
            ScreenManager.AddScreen(new PlayAsYouGoMainMenu(), ControllingPlayer);
        }

        // Function to help remove the pause screen when player choices a menu entry
        void PauseScreenRemove(object sender, PlayerIndexEventArgs e)
        {
            MenuEntries.Clear();
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            this.level.FadeScreen(spriteBatch, singlePixel, Color.Black, 0.7f);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }

    #endregion
}

#endregion