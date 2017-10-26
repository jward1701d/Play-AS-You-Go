using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using Microsoft.Phone.Shell;
using System.Collections;
using System.Runtime.Serialization; //needed for data contracts
using System.Xml;

namespace PAYG
{
    class ColorMimicHighScore : MenuScreen
    {
        const string saveGameFile = "CMHS.sfs";

        Texture2D[] CMBG = new Texture2D[7];
        
        SoundEffect sound;
        SoundEffectInstance music;
        
        SpriteFont TitleFont;
        Texture2D BgTexture;
        Vector2 backgroundPosition;
        Vector2 titlePosition;

        string title;

        int BgNum = 10;

        float transitionTime;

        MenuEntry back;

        public ColorMimicHighScore(Texture2D bgTexture, int bgNum)
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            BgNum = bgNum;

            Selected = Color.White;
            //NonSelected = Color.White;

           // Removed += new EventHandler(MenuRemove);
            BgTexture = bgTexture;
        }

        public override void Initialize()
        {
            base.Initialize();
            PhoneApplicationService.Current.Deactivated += new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(SaveState);
            int screenWidth = ScreenManager.Viewport.Width;

            backgroundPosition = new Vector2(0, 0);
            titlePosition = new Vector2((ScreenManager.Viewport.Width/2) -200, 0);

            back = new MenuEntry(this, "Back");
            back.SetPosition(new Vector2(screenWidth / 2 - 100, 400), true);
            back.Selected += new EventHandler(BackSelect);
            MenuEntries.Add(back);



            music = sound.CreateInstance();
            music.Volume = ScreenManager.MusicVolume;
            music.Play();
                        
        }


        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            SpriteFont = content.Load<SpriteFont>("Fonts/Color Mimic Fonts/CMscoreFont");
            TitleFont = content.Load<SpriteFont>("Fonts/Color Mimic Fonts/CMTitle");
            //backgroundTexture = BgTexture;//content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Colorful");
            sound = content.Load<SoundEffect>("Audio/high3");


            if (BgTexture == null && BgNum >= 6)
            {
                BgNum = 6;
            }

            #region Background Textures
            CMBG[0] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Colorful");
            CMBG[1] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Bubbles");
            CMBG[2] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Magic");
            CMBG[3] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Color");
            CMBG[4] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Night sky");
            CMBG[5] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Stary night");
            CMBG[6] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Sunset");
            #endregion
        }

        public override void UnloadContent()
        {
            SpriteFont = null;
            TitleFont = null;
        }

        //Added an update function to allow for back button code.
        public override void Update(GameTime gameTime, bool covered)
        {
            music.Volume = ScreenManager.MusicVolume;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                music.Stop();
                Remove();
                ScreenManager.AddScreen(new ColorMimicMainMenu());
            }
            base.Update(gameTime, covered);
        }

        public override void Draw(GameTime gameTime)
        {

            int screenWidth = ScreenManager.Viewport.Width;
            int screenHeight = ScreenManager.Viewport.Height;

            ISsystemData data = ISsystem.LoadHighScores(saveGameFile, 5);
            title = "Color Mimic\n"+"High Score ";
            string[] player = new string[5];
            int[] score = new int[5];
            int[] lvl= new int[5];
            int[] timed = new int[5];

            for (int i = 0; i < data.Count; i++)
            {
                player[i] = data.playerName[i];
                score[i] = data.score[i];
                lvl[i] = data.level[i];
                timed[i] = data.playTime[i];
            }

            string[] scoring = new string[5];
            string[] levels = new string[5];
            string[] played = new string[5];

            for (int i = 0; i < data.Count; i++)
            {
                scoring[i] = string.Format("{0}", score[i]);
                levels[i] = string.Format("{0}", lvl[i]);
                played[i] = string.Format("{0}", timed[i]);
            }

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            if (BgNum == 10)
                BgNum = 6;
            spriteBatch.Draw(CMBG[BgNum], backgroundPosition, Color.Gray);

            spriteBatch.DrawString(SpriteFont, "Name", new Vector2(screenWidth / 2 - 350, 150), Color.White);
            spriteBatch.DrawString(SpriteFont, "--------", new Vector2(screenWidth / 2 - 350, 165), Color.White);
            spriteBatch.DrawString(SpriteFont, player[0], new Vector2(screenWidth / 2 - 350, 185), Color.White);
            spriteBatch.DrawString(SpriteFont, player[1], new Vector2(screenWidth / 2 - 350, 205), Color.White);
            spriteBatch.DrawString(SpriteFont, player[2], new Vector2(screenWidth / 2 - 350, 225), Color.White);
            spriteBatch.DrawString(SpriteFont, player[3], new Vector2(screenWidth / 2 - 350, 245), Color.White);
            spriteBatch.DrawString(SpriteFont, player[4], new Vector2(screenWidth / 2 - 350, 265), Color.White);

            spriteBatch.DrawString(SpriteFont, "Score", new Vector2(screenWidth / 2 - 100, 150), Color.White);
            spriteBatch.DrawString(SpriteFont, "--------", new Vector2(screenWidth / 2 - 100, 165), Color.White);
            spriteBatch.DrawString(SpriteFont, scoring[0], new Vector2(screenWidth / 2 - 100, 185), Color.White);
            spriteBatch.DrawString(SpriteFont, scoring[1], new Vector2(screenWidth / 2 - 100, 205), Color.White);
            spriteBatch.DrawString(SpriteFont, scoring[2], new Vector2(screenWidth / 2 - 100, 225), Color.White);
            spriteBatch.DrawString(SpriteFont, scoring[3], new Vector2(screenWidth / 2 - 100, 245), Color.White);
            spriteBatch.DrawString(SpriteFont, scoring[4], new Vector2(screenWidth / 2 - 100, 265), Color.White);

            spriteBatch.DrawString(SpriteFont, "Total Play Time", new Vector2(screenWidth / 2 + 100, 150), Color.White);
            spriteBatch.DrawString(SpriteFont, "----------------------", new Vector2(screenWidth / 2 + 100, 165), Color.White);
            spriteBatch.DrawString(SpriteFont, played[0] + " Seconds", new Vector2(screenWidth / 2 + 125, 185), Color.White);
            spriteBatch.DrawString(SpriteFont, played[1] + " Seconds", new Vector2(screenWidth / 2 + 125, 205), Color.White);
            spriteBatch.DrawString(SpriteFont, played[2] + " Seconds", new Vector2(screenWidth / 2 + 125, 225), Color.White);
            spriteBatch.DrawString(SpriteFont, played[3] + " Seconds", new Vector2(screenWidth / 2 + 125, 245), Color.White);
            spriteBatch.DrawString(SpriteFont, played[4] + " Seconds", new Vector2(screenWidth / 2 + 125, 265), Color.White);


            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.White,0.0f,new Vector2(0,0),0.75f,SpriteEffects.None,1.0f);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void BackSelect(object sender, EventArgs e)
        {
            music.Stop();
            Remove();
            //music.Stop();
            ScreenManager.AddScreen(new ColorMimicMainMenu());
        }

        void MenuRemove(object sender, EventArgs e)
        {
            MenuEntries.Clear();
        }
        protected void SaveState(object sender, DeactivatedEventArgs args)
        {
            PhoneApplicationService.Current.State["ActiveGame"] = GameType.MENU;
        }
    }
}
