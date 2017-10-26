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

using Microsoft.Phone.Shell;
using System.Collections;
using System.Runtime.Serialization; //needed for data contracts
using System.Xml;


namespace PAYG
{
    class H1N1HighScoreScreen : MenuScreen
    {
        Texture2D backgroundTexture;
        Texture2D CellsLeftTexture;
        Texture2D CellsRightTexture;
        SpriteFont TitleFont;

        Vector2 titlePosition;

        GameObject background;
        GameObject[] cellsLeft = new GameObject[5];
        GameObject[] cellsRight = new GameObject[5];

        string title;

        float transitionTime;
        MenuEntry back, quit;

        SoundEffect sound;
        SoundEffectInstance music;


        Random rnd = new Random();

        // has to be same thing on all levels
        const string HighScore = "H1N1HighScore.sfs";
        ISsystemData Data = ISsystem.LoadHighScores(HighScore, 5);

        public H1N1HighScoreScreen()
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            Selected = Color.LightGreen;
            NonSelected = Color.LightGreen;

            Removed += new EventHandler(MainMenuRemove);
        }

        public override void Initialize()
        {
            PhoneApplicationService.Current.Deactivated += new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(SaveState);

            background = new GameObject(backgroundTexture, new Rectangle(0, 0, 1280, 720), 1, true, 0);
            background.Position = new Vector2(0, 0);
            background.Scale = 0.67f; 

            titlePosition = new Vector2(275, 20);

            title = "High Score";

            back = new MenuEntry(this, "Back");
            back.SetPosition(new Vector2(360, 400), true);
            back.Selected += BackSelect;
            MenuEntries.Add(back);

            quit = new MenuEntry(this, "Quit Game");
            quit.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), back, true);
            quit.Selected += QuitSelect;
            MenuEntries.Add(quit);

            // Initialize cell facing left background flow
            for (int i = 0; i < cellsLeft.Length; i++)
            {
                GameObject cellLeft = new GameObject(CellsLeftTexture, new Rectangle(0, 0, 256, 256), 1, true, 0);
                cellsLeft[i] = cellLeft;
                cellLeft.Position = new Vector2(rnd.Next(ScreenManager.Viewport.Width), rnd.Next(ScreenManager.Viewport.Height));
                cellLeft.Speed = (float)rnd.NextDouble() * 5 + 2;
                cellLeft.Scale = 0.5f;
            }

            // Initialize cell facing right background flow
            for (int i = 0; i < cellsRight.Length; i++)
            {
                GameObject cellRight = new GameObject(CellsRightTexture, new Rectangle(0, 0, 256, 256), 1, true, 0);
                cellsRight[i] = cellRight;
                cellRight.Position = new Vector2(rnd.Next(ScreenManager.Viewport.Width), rnd.Next(ScreenManager.Viewport.Height));
                cellRight.Speed = (float)rnd.NextDouble() * 5 + 2;
                cellRight.Scale = 0.5f;
            }

            music = sound.CreateInstance();
        }


        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            SpriteFont = content.Load<SpriteFont>("Fonts/H1N1Fonts/H1N1HighScoreFont");
            TitleFont = content.Load<SpriteFont>("Fonts/H1N1Fonts/H1N1PageTitleFont");

            backgroundTexture = content.Load<Texture2D>("Backgrounds/H1N1Backgrounds/H1N1Background");

            CellsLeftTexture = content.Load<Texture2D>("Backgrounds/H1N1Backgrounds/CellLeft");
            CellsRightTexture = content.Load<Texture2D>("Backgrounds/H1N1Backgrounds/CellRight");
            sound = content.Load<SoundEffect>("Audio/H1N1Audio/puzz3");
        }

        public override void UnloadContent()
        {
            SpriteFont = null;
            TitleFont = null;
            backgroundTexture = null;
            CellsLeftTexture = null;
            CellsRightTexture = null;
            sound = null;
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (MediaPlayer.GameHasControl)
            {
                music.Volume = ScreenManager.MusicVolume;
                music.Play();
            }
            else
                music.Pause();

            int height = ScreenManager.Viewport.Height;

            // Update left facing Cell
            for (int i = 0; i < cellsLeft.Length; i++)
            {
                var cellleft = cellsLeft[i];

                if ((cellleft.Position.Y += cellleft.Speed) > height - 20)
                {
                    // "generate" a new blood cell
                    cellleft.Position = new Vector2(rnd.Next(ScreenManager.Viewport.Width), -rnd.Next(20));
                    cellleft.Speed = (float)rnd.NextDouble() * 8;
                }
            }

            //Update right facing Cell
            for (int i = 0; i < cellsRight.Length; i++)
            {
                var cellRight = cellsRight[i];

                if ((cellRight.Position.Y += cellRight.Speed) > height - 20)
                {
                    // "generate" a new blood cell
                    cellRight.Position = new Vector2(rnd.Next(ScreenManager.Viewport.Width), -rnd.Next(20));
                    cellRight.Speed = (float)rnd.NextDouble() * 8;
                }
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                music.Stop();
                Remove();
                ScreenManager.AddScreen(new H1N1MenuScreen());
            }
            base.Update(gameTime, covered);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            background.Draw(gameTime, spriteBatch);

            // Draws left facing Cell
            foreach (var cellLeft in cellsLeft)
            {
                cellLeft.Draw(gameTime, spriteBatch);
            }

            // Draw right facing Cell
            foreach (var cellRight in cellsRight)
            {
                cellRight.Draw(gameTime, spriteBatch);
            }

            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.LightGreen);

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

            spriteBatch.DrawString(SpriteFont, " Name", new Vector2(ScreenManager.Viewport.X / 2 + 100, 100), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "---------", new Vector2(ScreenManager.Viewport.X / 2 + 100, 120), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, player[0], new Vector2(ScreenManager.Viewport.X / 2 + 100, 150), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, player[1], new Vector2(ScreenManager.Viewport.X / 2 + 100, 200), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, player[2], new Vector2(ScreenManager.Viewport.X / 2 + 100, 250), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, player[3], new Vector2(ScreenManager.Viewport.X / 2 + 100, 300), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, player[4], new Vector2(ScreenManager.Viewport.X / 2 + 100, 350), Color.DarkSeaGreen);

            spriteBatch.DrawString(SpriteFont, " Score", new Vector2(ScreenManager.Viewport.X / 2 + 375, 100), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "---------", new Vector2(ScreenManager.Viewport.X / 2 + 375, 120), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "  " + scoring[0], new Vector2(ScreenManager.Viewport.X / 2 + 375, 150), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "  " + scoring[1], new Vector2(ScreenManager.Viewport.X / 2 + 375, 200), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "  " + scoring[2], new Vector2(ScreenManager.Viewport.X / 2 + 375, 250), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "  " + scoring[3], new Vector2(ScreenManager.Viewport.X / 2 + 375, 300), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "  " + scoring[4], new Vector2(ScreenManager.Viewport.X / 2 + 375, 350), Color.DarkSeaGreen);

            spriteBatch.DrawString(SpriteFont, " Level", new Vector2(ScreenManager.Viewport.X / 2 + 600, 100), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "---------", new Vector2(ScreenManager.Viewport.X / 2 + 600, 120), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "    " + levels[0], new Vector2(ScreenManager.Viewport.X / 2 + 600, 150), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "    " + levels[1], new Vector2(ScreenManager.Viewport.X / 2 + 600, 200), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "    " + levels[2], new Vector2(ScreenManager.Viewport.X / 2 + 600, 250), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "    " + levels[3], new Vector2(ScreenManager.Viewport.X / 2 + 600, 300), Color.DarkSeaGreen);
            spriteBatch.DrawString(SpriteFont, "    " + levels[4], new Vector2(ScreenManager.Viewport.X / 2 + 600, 350), Color.DarkSeaGreen);


            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.DarkSeaGreen);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void BackSelect(object sender, EventArgs e)
        {
            music.Stop();
            Remove();
            ScreenManager.AddScreen(new H1N1MenuScreen());
        }

        void QuitSelect(object sender, EventArgs e)
        {
            music.Stop();
            Remove();
            ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
        }

        void MainMenuRemove(object sender, EventArgs e)
        {
            MenuEntries.Clear();
        }

        protected void SaveState(object sender, DeactivatedEventArgs args)
        {
            PhoneApplicationService.Current.State["ActiveGame"] = GameType.MENU;
        }
    }
}
