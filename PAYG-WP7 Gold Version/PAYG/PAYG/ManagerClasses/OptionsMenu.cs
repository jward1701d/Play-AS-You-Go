#region info
/******************************************************************************
 * Options Menu.cs                                                            *
 *                                                                            *
 * Writen by: Daniel McFee & James Ward                                       *
 *                                                                            *
 ******************************************************************************/
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.IsolatedStorage;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif

#endregion

#region Namespace
namespace PAYG
{
    #region Option Menu Class
    public class OptionsMenu : MenuScreen
    {
        #region Variable list
        // helps with the transition on and off time
        float transitionTime;

        // 7 menu entries
        MenuEntry enableMusic, enableFX, plus, minus, FXplus, FXminus, save, back;

        ISsystem checkFile = new ISsystem();

        int screenFlag;
        #endregion

        #region Constructor Method
        /// <summary>
        /// Preforms the actual saving operation of the ISsystem.
        /// </summary>
        /// <param name="flag">Uses a numeric digit to navigate back to a prevouis menu: 0) PAYG Main menu 1) Beyond Space 2) Color Mimic 3) H1N1 4) Pause Menu.</param>
        public OptionsMenu(int flag)
        {
            screenFlag = flag;
            transitionTime = 1.0f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            Selected = Color.White;
            NonSelected = Color.White;

            Removed += new EventHandler(MenuRemove);
        }
        #endregion

        #region Load Content
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            SpriteFont = content.Load<SpriteFont>("Fonts/SmallFont");
        }
        #endregion

        #region Initialize Method
        public override void Initialize()
        {
            checkFile.checkFile();

            

            enableMusic = new MenuEntry(this, "Music");
            enableMusic.SetPosition(new Vector2(300, 150), true);
            enableMusic.Selected += new EventHandler(MusicSelect);
            MenuEntries.Add(enableMusic);

            plus = new MenuEntry(this, "+");
            plus.SetRelativePosition(new Vector2(285, SpriteFont.LineSpacing + 10), enableMusic, true);//SetPosition(new Vector2(450, 150), true);
            plus.Selected += new EventHandler(PlusSelect);
            MenuEntries.Add(plus);

            minus = new MenuEntry(this, "-");
            minus.SetRelativePosition(new Vector2(-50, SpriteFont.LineSpacing + 10), enableMusic, true);//SetPosition(new Vector2(250, 150), true);
            minus.Selected += new EventHandler(MinusSelect);
            MenuEntries.Add(minus);

            enableFX = new MenuEntry(this, "Sound Effects");
            enableFX.SetRelativePosition(new Vector2(0, (SpriteFont.LineSpacing * 2) + 20), enableMusic, true);
            enableFX.Selected += new EventHandler(FXSelect);
            MenuEntries.Add(enableFX);

            FXplus = new MenuEntry(this, "+");
            FXplus.SetRelativePosition(new Vector2(285, SpriteFont.LineSpacing + 10), enableFX, true);//SetPosition(new Vector2(450, 150), true);
            FXplus.Selected += new EventHandler(FXPlusSelect);
            MenuEntries.Add(FXplus);

            FXminus = new MenuEntry(this, "-");
            FXminus.SetRelativePosition(new Vector2(-50, SpriteFont.LineSpacing + 10), enableFX, true);//SetPosition(new Vector2(450, 150), true);
            FXminus.Selected += new EventHandler(FXMinusSelect);
            MenuEntries.Add(FXminus);

            back = new MenuEntry(this, "Back");
            back.SetRelativePosition(new Vector2(0, (SpriteFont.LineSpacing * 2) + 20), enableFX, true);//SetPosition(new Vector2(450, 150), true);
            back.Selected += new EventHandler(BackSelect);
            MenuEntries.Add(back);

            if(checkFile.isSaveable)
            {
                save = new MenuEntry(this, "Save");
                save.SetRelativePosition(new Vector2(0, (SpriteFont.LineSpacing * 2) + 20), back, true);//SetPosition(new Vector2(450, 150), true);
                save.Selected += new EventHandler(SaveSelect);
                MenuEntries.Add(save);
            }
            else
            {
                // DO NOTHING
            }

            ScreenManager.prevMusicVolume = ScreenManager.MusicVolume;
            ScreenManager.prevFXVolume = ScreenManager.FXVolume;
        }
        #endregion

        #region Updatre Method
        //Added an update function to allow for back button code.
        public override void Update(GameTime gameTime, bool covered)
        {
            // Handles the back button on the phone.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                ReturnMenu();
            } // End IF

            base.Update(gameTime, covered);
        }
        #endregion

        #region Draw Method
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            #region Local Vairables
            string soundOn = "ON";
            string soundOff = "OFF";
            string musicValue;
            string fxValue;

            int mv = (int)(ScreenManager.MusicVolume * 10);
            int fxv = (int)(ScreenManager.FXVolume * 10);
            musicValue = string.Format("{0}", mv);
            fxValue = string.Format("{0}", fxv);
            #endregion

            ScreenManager.GraphicsDevice.Clear(Color.Black);
            
            spriteBatch.Begin();
            if (!ScreenManager.MusicEnabled)
            {
                // Handle special drawn strings for not enabled music
                spriteBatch.DrawString(SpriteFont, soundOff, new Vector2(650, 150), Color.White); // X = 580
                spriteBatch.DrawString(SpriteFont, "Music Volume", new Vector2(300, 182), Color.Gray);
                spriteBatch.DrawString(SpriteFont, musicValue, new Vector2(650, 182), Color.Gray); // X = 625
            }// End IF
            else
            {
                spriteBatch.DrawString(SpriteFont, soundOn, new Vector2(650, 150), Color.White);
                spriteBatch.DrawString(SpriteFont, "Music Volume", new Vector2(300, 182), Color.White);
                spriteBatch.DrawString(SpriteFont, musicValue, new Vector2(650, 182), Color.White);
            }// End ELSE

            if (!ScreenManager.FXEnabled)
            {
                // Handle special drawn strings for not enabled fx
                spriteBatch.DrawString(SpriteFont, soundOff, new Vector2(650, 218), Color.White);
                spriteBatch.DrawString(SpriteFont, "Sound FX Volume", new Vector2(300, 252), Color.Gray);
                spriteBatch.DrawString(SpriteFont, fxValue, new Vector2(650, 252), Color.Gray);
            }// End IF
            else
            {
                spriteBatch.DrawString(SpriteFont, soundOn, new Vector2(650, 218), Color.White);
                spriteBatch.DrawString(SpriteFont, "Sound FX Volume", new Vector2(300, 252), Color.White);
                spriteBatch.DrawString(SpriteFont, fxValue, new Vector2(650, 252), Color.White);
            }// End ELSE

            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion

        #region Menu Entry Remove Method
        void MenuRemove(object sender, EventArgs e)
        {
            MenuEntries.Clear();
        }
        #endregion

        #region Music ON/OFF Method
        // Function for when player choices the Resume menu entry
        void MusicSelect(object sender, EventArgs e)
        {
            ScreenManager.MusicEnabled = !ScreenManager.MusicEnabled;
            if (!ScreenManager.MusicEnabled)
                ScreenManager.MusicVolume = 0.0f;
            else
                ScreenManager.MusicVolume = ScreenManager.prevMusicVolume;
        }
        #endregion

        #region FX ON/OFF Method
        // Function for when player choices the Quit menu entry
        void FXSelect(object sender, EventArgs e)
        {
            ScreenManager.FXEnabled = !ScreenManager.FXEnabled;
            if (!ScreenManager.FXEnabled)
                ScreenManager.FXVolume = 0.0f;
            else
                ScreenManager.FXVolume = ScreenManager.prevFXVolume;
        }
        #endregion

        #region Music Plus Method
        void PlusSelect(object sender, EventArgs e)
        {
            if (ScreenManager.MusicEnabled)
            {
                ScreenManager.MusicVolume += 0.1f;
                ScreenManager.prevMusicVolume = ScreenManager.MusicVolume;
                if (ScreenManager.MusicVolume > 1.0f)
                {
                    ScreenManager.MusicVolume = 1.0f;
                    ScreenManager.prevMusicVolume = ScreenManager.MusicVolume;
                }// End IF
            }// End IF
            else
            {
                // DO NOTHING.
            }// End ELSE
        }
        #endregion

        #region FX Plus Method
        void FXPlusSelect(object sender, EventArgs e)
        {
            if (ScreenManager.MusicEnabled)
            {
                ScreenManager.FXVolume += 0.1f;
                ScreenManager.prevFXVolume = ScreenManager.FXVolume;
                if (ScreenManager.FXVolume > 1.0f)
                {
                    ScreenManager.FXVolume = 1.0f;
                    ScreenManager.prevFXVolume = ScreenManager.FXVolume;
                }// End IF
            }// End IF
            else
            {
                // DO NOTHING.
            }// End ELSE
        }
        #endregion

        #region Music Minus Method
        void MinusSelect(object sender, EventArgs e)
        {
            if (ScreenManager.MusicEnabled)
            {
                ScreenManager.MusicVolume -= 0.1f;
                ScreenManager.prevMusicVolume = ScreenManager.MusicVolume;
                if (ScreenManager.MusicVolume < 0.0f)
                {
                    ScreenManager.MusicVolume = 0.0f;
                    ScreenManager.prevMusicVolume = ScreenManager.MusicVolume;
                }// End IF
            }// End IF
            else
            {
                // DO NOTHING.
            }// End ELSE
        }
        #endregion

        #region FX Minus Method
        void FXMinusSelect(object sender, EventArgs e)
        {
            if (ScreenManager.MusicEnabled)
            {
                ScreenManager.FXVolume -= 0.1f;
                ScreenManager.prevFXVolume = ScreenManager.FXVolume;
                if (ScreenManager.FXVolume < 0.0f)
                {
                    ScreenManager.FXVolume = 0.0f;
                    ScreenManager.prevFXVolume = ScreenManager.FXVolume;
                }// End IF
            }// End IF
            else
            {
                // DO NOTHING.
            }// End ELSE
        }
        #endregion

        #region Back Button Method
        void BackSelect(object sender, EventArgs e)
        {
            //Remove();
            ReturnMenu();
        }
        #endregion

        void SaveSelect(object sender, EventArgs e)
        {
            if (checkFile.isSaveable)
            {
                SaveOptions();
                ReturnMenu();
            }
            else
            {
                // DO NOTHING
            }
        }

        #region Return Menu Method
        void ReturnMenu()
        {
            switch (screenFlag)
            {
                case 0:
                    Remove();
                    ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
                    break;
                case 1:
                    Remove();
                    ScreenManager.AddScreen(new BSMainMenu());
                    break;
                case 2:
                    Remove();
                    ScreenManager.AddScreen(new ColorMimicMainMenu());
                    break;
                case 3:
                    Remove();
                    ScreenManager.AddScreen(new H1N1MenuScreen());
                    break;
                case 4:
                    Remove();
                    break;
                default:
                    Remove();
                    ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
                    break;
            }// End SWITCH
        }
        #endregion

        void SaveOptions()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

            using (storage)
            {
                using (IsolatedStorageFileStream isfs =
                    storage.CreateFile("Options.sfs"))
                {
                    using (StreamWriter writer = new StreamWriter(isfs))
                    {
                        writer.WriteLine(ScreenManager.MusicEnabled.ToString());
                        writer.WriteLine(ScreenManager.FXEnabled.ToString());
                        writer.WriteLine(ScreenManager.MusicVolume.ToString());
                        writer.WriteLine(ScreenManager.prevMusicVolume.ToString());
                        writer.WriteLine(ScreenManager.FXVolume.ToString());
                        writer.WriteLine(ScreenManager.prevFXVolume.ToString());
                    }
                }
            }
        }
        
    }
    #endregion
}
#endregion