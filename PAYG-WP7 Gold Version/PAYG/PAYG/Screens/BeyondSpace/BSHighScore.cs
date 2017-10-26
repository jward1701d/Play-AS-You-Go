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
    class BSHighScore : MenuScreen
    {
        Texture2D backgroundTexture;
        SpriteFont TitleFont;

        SoundEffect music;
        SoundEffectInstance iMusic;

        Vector2 backgroundPosition;
        Vector2 titlePosition;

        string title;

        float transitionTime;
        MenuEntry back, quit;

        // has to be same thing on all levels
        const string HighScore = "BeyondSpaceScore.sfs";
        ISsystemData Data = ISsystem.LoadHighScores(HighScore, 5);

        public BSHighScore()
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            Selected = Color.Green;
            NonSelected = Color.Green;

            Removed += new EventHandler(MainMenuRemove);
        }

        public override void Initialize()
        {
            PhoneApplicationService.Current.Deactivated += new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(SaveState);
            backgroundPosition = new Vector2(60, 40);
            titlePosition = new Vector2(500, 50);

            title = "High Score";

            back = new MenuEntry(this, "Back");
            back.SetPosition(new Vector2(340, 420), true);
            back.Selected += new EventHandler(BackSelect);
            MenuEntries.Add(back);

            quit = new MenuEntry(this, "Quit Game");
            quit.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), back, true);
            quit.Selected += new EventHandler(QuitSelect);
            MenuEntries.Add(quit);

            iMusic = music.CreateInstance();
            iMusic.Volume = ScreenManager.MusicVolume;
            iMusic.IsLooped = true;
            iMusic.Play();
        }


        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            SpriteFont = content.Load<SpriteFont>("Fonts/BeyondSpaceFont/MenuFont");
            TitleFont = content.Load<SpriteFont>("Fonts/BeyondSpaceFont/TitleFont");

            backgroundTexture = content.Load<Texture2D>("Backgrounds/BSBackgrounds/0001");
            music = content.Load<SoundEffect>("Audio/BSAudio/outer10");
        }

        public override void UnloadContent()
        {
            SpriteFont = null;
            TitleFont = null;
            backgroundTexture = null;
            music = null;
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            iMusic.Volume = ScreenManager.MusicVolume;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                iMusic.Stop();
                Remove();
                ScreenManager.AddScreen(new BSMainMenu());
            }

            base.Update(gameTime, covered);
        }
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.GraphicsDevice.Clear(Color.Black);

            //spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.Gray);
            spriteBatch.Draw(backgroundTexture, backgroundPosition, null, Color.White, 0f, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0f);

            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.Black);

            string[] player = new string[5];
            int[] score = new int[5];
            int[] lvl = new int[5];

            for (int i = 0; i < Data.Count; i++)
            {
                player[i] = Data.playerName[i];
                score[i] = Data.score[i];
                lvl[i] = Data.level[i];
            }

            string[] scoring = new string[5];
            string[] levels = new string[5];

            for (int i = 0; i < Data.Count; i++)
            {
                scoring[i] = string.Format("{0}", score[i]);
                levels[i] = string.Format("{0}", lvl[i]);
            }

            //Game1.Current.GraphicsDevice.Clear(Color.Black);

            spriteBatch.DrawString(SpriteFont, " Name", new Vector2(ScreenManager.Viewport.X / 2 + 150, 50), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "---------", new Vector2(ScreenManager.Viewport.X / 2 + 150, 65), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, player[0], new Vector2(ScreenManager.Viewport.X / 2 + 150, 100), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, player[1], new Vector2(ScreenManager.Viewport.X / 2 + 150, 150), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, player[2], new Vector2(ScreenManager.Viewport.X / 2 + 150, 200), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, player[3], new Vector2(ScreenManager.Viewport.X / 2 + 150, 250), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, player[4], new Vector2(ScreenManager.Viewport.X / 2 + 150, 300), Color.DarkSeaGreen);

            spriteBatch.DrawString(SpriteFont, " Score", new Vector2(ScreenManager.Viewport.X / 2 + 350, 50), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "---------", new Vector2(ScreenManager.Viewport.X / 2 + 350, 65), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "  " + scoring[0], new Vector2(ScreenManager.Viewport.X / 2 + 350, 100), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "  " + scoring[1], new Vector2(ScreenManager.Viewport.X / 2 + 350, 150), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "  " + scoring[2], new Vector2(ScreenManager.Viewport.X / 2 + 350, 200), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "  " + scoring[3], new Vector2(ScreenManager.Viewport.X / 2 + 350, 250), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "  " + scoring[4], new Vector2(ScreenManager.Viewport.X / 2 + 350, 300), Color.DarkSeaGreen);

            spriteBatch.DrawString(SpriteFont, " Level", new Vector2(ScreenManager.Viewport.X / 2 + 530, 50), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "---------", new Vector2(ScreenManager.Viewport.X / 2 + 530, 65), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "    " + levels[0], new Vector2(ScreenManager.Viewport.X / 2 + 530, 100), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "    " + levels[1], new Vector2(ScreenManager.Viewport.X / 2 + 530, 150), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "    " + levels[2], new Vector2(ScreenManager.Viewport.X / 2 + 530, 200), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "    " + levels[3], new Vector2(ScreenManager.Viewport.X / 2 + 530, 250), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "    " + levels[4], new Vector2(ScreenManager.Viewport.X / 2 + 530, 300), Color.DarkSeaGreen);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void BackSelect(object sender, EventArgs e)
        {
            iMusic.Stop();
            Remove();
            ScreenManager.AddScreen(new BSMainMenu());
        }

        void QuitSelect(object sender, EventArgs e)
        {
            iMusic.Stop();
            Remove();
            ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
        }

        void MainMenuRemove(object sender, EventArgs e)
        {
            iMusic.Stop();
            MenuEntries.Clear();
        }
        protected void SaveState(object sender, DeactivatedEventArgs args)
        {
            PhoneApplicationService.Current.State["ActiveGame"] = GameType.MENU;
        }
    }
}
