using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;


namespace Play_As_You_Go
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
        MenuEntry back;

        // has to be same thing on all levels
        const string HighScore = "BeyondSpaceScore.sfs";
        ISsystemData Data = ISsystem.LoadHighScores(HighScore, 5);

        public BSHighScore()
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            SelectedColor = Color.White;
            NonSelectedColor = Color.Green;
        }

        public override void Initialize()
        {
            backgroundPosition = new Vector2(60, 0);
            titlePosition = new Vector2(500, 50);

            title = "High Score";

            back = new MenuEntry(this, "Back");
            back.SetPosition(new Vector2(550, 600), true);
            back.Selected += BackSelect;
            MenuEntries.Add(back);

            iMusic = music.CreateInstance();
            iMusic.Volume = 0.1f;
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
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (MediaPlayer.GameHasControl)
                iMusic.Play();

            else
                iMusic.Pause();

            base.Update(gameTime, covered);

        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.Gray);

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

            spriteBatch.DrawString(SpriteFont, " Name", new Vector2(ScreenManager.Viewport.X / 2 + 400, 150), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "---------", new Vector2(ScreenManager.Viewport.X / 2 + 400, 165), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, player[0], new Vector2(ScreenManager.Viewport.X / 2 + 400, 200), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, player[1], new Vector2(ScreenManager.Viewport.X / 2 + 400, 250), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, player[2], new Vector2(ScreenManager.Viewport.X / 2 + 400, 300), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, player[3], new Vector2(ScreenManager.Viewport.X / 2 + 400, 350), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, player[4], new Vector2(ScreenManager.Viewport.X / 2 + 400, 400), Color.DarkSeaGreen);

            spriteBatch.DrawString(SpriteFont, " Score", new Vector2(ScreenManager.Viewport.X / 2 + 600, 150), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "---------", new Vector2(ScreenManager.Viewport.X / 2 + 600, 165), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "  " + scoring[0], new Vector2(ScreenManager.Viewport.X / 2 + 600, 200), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "  " + scoring[1], new Vector2(ScreenManager.Viewport.X / 2 + 600, 250), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "  " + scoring[2], new Vector2(ScreenManager.Viewport.X / 2 + 600, 300), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "  " + scoring[3], new Vector2(ScreenManager.Viewport.X / 2 + 600, 350), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "  " + scoring[4], new Vector2(ScreenManager.Viewport.X / 2 + 600, 400), Color.DarkSeaGreen);

            spriteBatch.DrawString(SpriteFont, " Level", new Vector2(ScreenManager.Viewport.X / 2 + 780, 150), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "---------", new Vector2(ScreenManager.Viewport.X / 2 + 780, 165), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "    " + levels[0], new Vector2(ScreenManager.Viewport.X / 2 + 780, 200), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "    " + levels[1], new Vector2(ScreenManager.Viewport.X / 2 + 780, 250), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "    " + levels[2], new Vector2(ScreenManager.Viewport.X / 2 + 780, 300), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "    " + levels[3], new Vector2(ScreenManager.Viewport.X / 2 + 780, 350), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "    " + levels[4], new Vector2(ScreenManager.Viewport.X / 2 + 780, 400), Color.DarkSeaGreen);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void BackSelect(object sender, PlayerIndexEventArgs e)
        {
            iMusic.Stop();
            Remove();
            ScreenManager.AddScreen(new BSMainMenu(), e.PlayerIndex);
        }

        void MainMenuRemove(object sender, PlayerIndexEventArgs e)
        {
            iMusic.Stop();
            MenuEntries.Clear();
        }
    }
}
