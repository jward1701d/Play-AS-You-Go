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
    class H1N1WindowsPhoneControlsScreen : MenuScreen
    {
        Texture2D backgroundTexture;
        Texture2D WindowsPhoneControlBackground;
        Texture2D CellsLeftTexture;
        Texture2D CellsRightTexture;
        SpriteFont TitleFont;

        Vector2 titlePosition;

        GameObject background;
        GameObject WPBackground;
        GameObject[] cellsLeft = new GameObject[5];
        GameObject[] cellsRight = new GameObject[5];

        string title;

        float transitionTime;
        MenuEntry back, quit;

        SoundEffect sound;
        SoundEffectInstance music;

        Random rnd = new Random();

        public H1N1WindowsPhoneControlsScreen()
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

            background = new GameObject(backgroundTexture, new Rectangle(0, 0,1280, 720), 1, true, 0);
            background.Position = new Vector2(0, 0);
            background.Scale = .67f;

            WPBackground = new GameObject(WindowsPhoneControlBackground, new Rectangle(0, 0, 830, 482), 1, true, 0);
            WPBackground.Position = new Vector2(0, 0);
            WPBackground.Scale = 1.0f;

            title = "H1N1 Phone Controls";
            titlePosition = new Vector2(200, 50);            

            back = new MenuEntry(this, "Back");
            back.SetPosition(new Vector2(350, 400), true);
            back.Selected += new EventHandler(BackGameSelect);
            MenuEntries.Add(back);

            quit = new MenuEntry(this, "Quit Game");
            quit.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), back, true);
            quit.Selected += new EventHandler(QuitSelect);
            MenuEntries.Add(quit);

            // Initialize cell facing left background flow
            for (int i = 0; i < cellsLeft.Length; i++)
            {
                GameObject cellLeft = new GameObject(CellsLeftTexture, new Rectangle(0, 0, 256, 256), 1, true, 0);
                cellsLeft[i] = cellLeft;
                cellLeft.Position = new Vector2(rnd.Next(ScreenManager.Viewport.Width), rnd.Next(ScreenManager.Viewport.Height));
                cellLeft.Speed = (float)rnd.NextDouble() * 5 + 2;
                cellLeft.Scale = 0.35f;
            }

            // Initialize cell facing right background flow
            for (int i = 0; i < cellsRight.Length; i++)
            {
                GameObject cellRight = new GameObject(CellsRightTexture, new Rectangle(0, 0, 256, 256), 1, true, 0);
                cellsRight[i] = cellRight;
                cellRight.Position = new Vector2(rnd.Next(ScreenManager.Viewport.Width), rnd.Next(ScreenManager.Viewport.Height));
                cellRight.Speed = (float)rnd.NextDouble() * 5 + 2;
                cellRight.Scale = 0.35f;
            }

            music = sound.CreateInstance();
        }


        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            SpriteFont = content.Load<SpriteFont>("Fonts/H1N1Fonts/H1N1MenuEntryFont");
            TitleFont = content.Load<SpriteFont>("Fonts/H1N1Fonts/H1N1PageTitleFont");

            backgroundTexture = content.Load<Texture2D>("Backgrounds/H1N1Backgrounds/H1N1Background");
            WindowsPhoneControlBackground = content.Load<Texture2D>("Backgrounds/H1N1Backgrounds/H1N1WindowPhoneControls");

            CellsLeftTexture = content.Load<Texture2D>("Backgrounds/H1N1Backgrounds/CellLeft");
            CellsRightTexture = content.Load<Texture2D>("Backgrounds/H1N1Backgrounds/CellRight");

            sound = content.Load<SoundEffect>("Audio/H1N1Audio/puzz3");
        }

        public override void UnloadContent()
        {
            SpriteFont = null;
            TitleFont = null;
            backgroundTexture = null;
            WindowsPhoneControlBackground = null;
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

            WPBackground.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void BackGameSelect(object sender, EventArgs e)
        {
            music.Stop();
            Remove();
            ScreenManager.AddScreen(new H1N1MenuScreen());
        }

        void QuitSelect(object sender, EventArgs e)
        {
            music.Stop();
            TransitionOffTime = TimeSpan.FromSeconds(transitionTime);
            Remove();           
           
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

    
