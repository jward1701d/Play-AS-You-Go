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
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Input.Touch;

#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif

#endregion

#region Name Space

namespace PAYG
{
    #region Class

    public class PauseScreen : MenuScreen
    {
        // hold the current GameScreen that is active
        GameScreen level;

        // helps with the transition on and off time
        float transitionTime;

        // 2 menu entries
        MenuEntry options, resume, quit;
        Texture2D singlePixel;

        bool optionMenu = false;

        public PauseScreen(GameScreen gameScreen)
        {
            this.level = gameScreen;

            transitionTime = 1.0f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            Selected = Color.White;
            NonSelected = Color.White;

            Removed += new EventHandler(PauseScreenRemove);
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
            resume.SetPosition(new Vector2(300, 175), true);
            resume.Selected += new EventHandler(ResumeSelect);
            MenuEntries.Add(resume);

            options = new MenuEntry(this, "Options");
            options.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), resume, true);
            options.Selected += new EventHandler(OptionsSelect);
            MenuEntries.Add(options);

            quit = new MenuEntry(this, "Quit Game");
            quit.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), options, true);
            quit.Selected += new EventHandler(QuitSelect);
            MenuEntries.Add(quit);
            
        }
        //Added an update function to allow for back button code.
        public override void  Update(GameTime gameTime, bool covered)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && !optionMenu)
            {
                //Remove();
                if (level.ScreenState == ScreenState.Frozen)
                {
                    //Remove();
                    ScreenManager.RemoveScreen(this);
                    MenuEntries.Clear();
                    level.ScreenState = ScreenState.Active;
                }
            }
             
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && optionMenu)
                optionMenu = false;
            
            base.Update(gameTime, covered);
        }


        // Function for when player choices the Resume menu entry
        void ResumeSelect(object sender, EventArgs e)
        {
            Remove();
            ScreenManager.isPressed = false;
            level.ScreenState = ScreenState.Active;           
        }

        // Function for when player choices the Quit menu entry
        void QuitSelect(object sender, EventArgs e)
        {
            ScreenManager.isPressed = false;
            Remove();
            level.Remove();
            TransitionOffTime = TimeSpan.FromSeconds(transitionTime);
            ScreenManager.RemoveAd();
            ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
        }

        void OptionsSelect(object sender, EventArgs e)
        {
            
            TransitionOffTime = TimeSpan.FromSeconds(transitionTime);
            optionMenu = true;
            ScreenManager.AddScreen(new OptionsMenu(4));
        }

        // Function to help remove the pause screen when player choices a menu entry
        void PauseScreenRemove(object sender, EventArgs e)
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