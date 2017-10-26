using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Play_As_You_Go
{
    class H1N1MenuScreen : MenuScreen
    {
        Texture2D backgroundTexture;
        Texture2D CellsLeftTexture;
        Texture2D CellsRightTexture;
        SpriteFont TitleFont;

        Vector2 backgroundPosition;
        Vector2 titlePosition;

        GameObject[] cellsLeft = new GameObject[5];
        GameObject[] cellsRight = new GameObject[5];

        string title;

        float transitionTime;
        MenuEntry play, controls, highscore, quit;

        SoundEffect sound;
        SoundEffectInstance music;

        float volume = 0.15f;
        Random rnd = new Random();

        double vidCountDown = 15.0d;

        public H1N1MenuScreen()
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            SelectedColor = Color.White;
            NonSelectedColor = new Color(62, 227, 66);
        }

        public override void Initialize()
        {
            backgroundPosition = new Vector2(0, 0);
            titlePosition = new Vector2(400, 100);

            title = "H1N1 A.K.A SwineFlu";

            play = new MenuEntry(this, "Play Game");
            play.SetPosition(new Vector2(550, 360), true);
            play.Selected += PlayGameSelect;
            MenuEntries.Add(play);

            controls = new MenuEntry(this, "Controls");
            controls.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), play, true);
            controls.Selected += ControlSelect;
            MenuEntries.Add(controls);

            highscore = new MenuEntry(this, "Highscore");
            highscore.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), controls, true);
            highscore.Selected += HighscoreSelect;
            MenuEntries.Add(highscore);

            quit = new MenuEntry(this, "Quit Game");
            quit.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), highscore, true);
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

            SpriteFont = content.Load<SpriteFont>("Fonts/H1N1Fonts/H1N1MenuEntryFont");
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
                music.Volume = volume;
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
            this.vidCountDown -= gameTime.ElapsedGameTime.TotalSeconds;

            if (vidCountDown <= 0.0d)
            {
                music.Stop();
                Remove();
                ScreenManager.AddScreen(new DemoVid(3), ControllingPlayer);

            }
            base.Update(gameTime, covered);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);

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

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void PlayGameSelect(object sender, PlayerIndexEventArgs e)
        {
            music.Stop();
            Remove();
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new H1N1BackStory());
            //ScreenManager.AddScreen(new H1N1Level_02(), e.PlayerIndex);
        }

        void ControlSelect(object sender, PlayerIndexEventArgs e)
        {
            music.Stop();
            Remove();
#if WINDOWS
            ScreenManager.AddScreen(new H1N1KeybordCotrols(), e.PlayerIndex);
#else
            ScreenManager.AddScreen(new H1N1Xbo360PadddleControlsScreen(),  e.PlayerIndex);
#endif


        }

        void HighscoreSelect(object sender, PlayerIndexEventArgs e)
        {
            music.Stop();
            Remove();
            ScreenManager.AddScreen(new H1N1HighScoreScreen(), e.PlayerIndex);

        }

        void QuitSelect(object sender, PlayerIndexEventArgs e)
        {
            music.Stop();
            Remove();
            ScreenManager.AddScreen(new PlayAsYouGoMainMenu(),e.PlayerIndex);            
           
        }

        void MenuRemove(object sender, PlayerIndexEventArgs e)
        {
            MenuEntries.Clear();
        }
    }
}
