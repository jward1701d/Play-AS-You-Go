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

using Microsoft.Phone.Shell;
using System.Collections;
using System.Runtime.Serialization; //needed for data contracts
using System.Xml;

namespace PAYG
{
    class H1N1MenuScreen : MenuScreen
    {
        Texture2D backgroundTexture;
        Texture2D CellsLeftTexture;
        Texture2D CellsRightTexture;
        SpriteFont TitleFont;

        Vector2 titlePosition;
        Vector2 position;

        GameObject background;
        GameObject[] cellsLeft = new GameObject[5];
        GameObject[] cellsRight = new GameObject[5];

        string title;

        float transitionTime;
        MenuEntry play, controls, highscore, options, quit;

        SoundEffect sound;
        SoundEffectInstance music;

       
        Random rnd = new Random();

        bool optionMenu = false;

        public H1N1MenuScreen()
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            Selected = Color.LightGreen;
            NonSelected = Color.LightGreen;

            Removed += new EventHandler(MenuRemove);
        }

        public override void Initialize()
        {
            PhoneApplicationService.Current.Deactivated += new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(SaveState);

            background = new GameObject(backgroundTexture, new Rectangle(0, 0, 1280, 720), 1, true, 0);
            background.Position = new Vector2(0, 0);
            background.Scale = 0.67f;

            position = new Vector2(400, 150);

            titlePosition = new Vector2(175, 50);

            title = "H1N1 A.K.A SwineFlu";

            play = new MenuEntry(this, "Play Game");
            play.SetPosition(new Vector2(350, 300), true);

            play.Selected += new EventHandler(PlayGameSelect);
            MenuEntries.Add(play);

            controls = new MenuEntry(this, "Controls");
            controls.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), play, true);
            controls.Selected += new EventHandler(ControlSelect);
            MenuEntries.Add(controls);

            highscore = new MenuEntry(this, "Highscore");
            highscore.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), controls, true);
            highscore.Selected += new EventHandler(HighscoreSelect);
            MenuEntries.Add(highscore);

            options = new MenuEntry(this, "Options");
            options.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), highscore, true);
            options.Selected += new EventHandler(OptionsSelect);
            MenuEntries.Add(options);

            quit = new MenuEntry(this, "Quit Game");
            quit.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), options, true);
            quit.Selected += new EventHandler(QuitSelect);
            MenuEntries.Add(quit);

            // Initialize cell facing left background flow
            for (int i = 0; i < cellsLeft.Length; i++)
            {
                GameObject cellLeft = new GameObject(CellsLeftTexture, new Rectangle(0, 0, 256, 256), 1, true, 0);
                cellsLeft[i] = cellLeft;
                cellLeft.Position = new Vector2(rnd.Next(ScreenManager.Viewport.Width), rnd.Next(ScreenManager.Viewport.Height));
                cellLeft.Speed = (float)rnd.NextDouble() * 5 + 2;

                cellLeft.Scale = .35f;
            }

            // Initialize cell facing right background flow
            for (int i = 0; i < cellsRight.Length; i++)
            {
                GameObject cellRight = new GameObject(CellsRightTexture, new Rectangle(0, 0, 256, 256), 1, true, 0);
                cellsRight[i] = cellRight;
                cellRight.Position = new Vector2(rnd.Next(ScreenManager.Viewport.Width), rnd.Next(ScreenManager.Viewport.Height));
                cellRight.Speed = (float)rnd.NextDouble() * 5 + 2;
                cellRight.Scale = .35f;

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
            music.Volume = ScreenManager.MusicVolume;

            if (MediaPlayer.GameHasControl)
            {
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && !optionMenu)
            {
                music.Stop();
                TransitionOffTime = TimeSpan.FromSeconds(transitionTime);
                Remove();
                ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && optionMenu)
                optionMenu = false;
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

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void PlayGameSelect(object sender, EventArgs e)
        {
            music.Stop();
            TransitionOffTime = TimeSpan.FromSeconds(transitionTime);
            Remove();
            LoadingScreen.Load(ScreenManager, true, new H1N1BackStory());            
        }

        void ControlSelect(object sender, EventArgs e)
        {
            music.Stop();
            TransitionOffTime = TimeSpan.FromSeconds(transitionTime);
            Remove();
            ScreenManager.AddScreen(new H1N1WindowsPhoneControlsScreen()); 

        }

        void HighscoreSelect(object sender, EventArgs e)
        {
            music.Stop();
            TransitionOffTime = TimeSpan.FromSeconds(transitionTime);
            Remove();
            ScreenManager.AddScreen(new H1N1HighScoreScreen());

        }
        void OptionsSelect(object sender, EventArgs e)
        {
            music.Stop();
            Remove();
            optionMenu = true;
            ScreenManager.AddScreen(new OptionsMenu(3));
        }
        void QuitSelect(object sender, EventArgs e)
        {
            music.Stop();
            TransitionOffTime = TimeSpan.FromSeconds(transitionTime);
            Remove();
            ScreenManager.AddScreen(new PlayAsYouGoMainMenu());  
           
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
