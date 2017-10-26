﻿using System;
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
    class H1N1BackStory : MenuScreen
    {
        Texture2D backgroundTexture;
        Texture2D CellsLeftTexture;
        Texture2D CellsRightTexture;
        SpriteFont TitleFont;

        Vector2 backgroundPosition;
        Vector2 titlePosition;
        Vector2 infoPosition;

        GameObject[] cellsLeft = new GameObject[5];
        GameObject[] cellsRight = new GameObject[5];

        string title;
        string info;

        float transitionTime;
        MenuEntry play, quit;

        SoundEffect sound;
        SoundEffectInstance music;

        float volume = 0.15f;

        Random rnd = new Random();

        public H1N1BackStory()
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            SelectedColor = Color.White;
            NonSelectedColor = new Color(62, 227, 66); ;

        }

        public override void Initialize()
        {
            backgroundPosition = new Vector2(0, 0);
            titlePosition = new Vector2(525, 50);
            infoPosition = new Vector2(245, 150);

            title = "BackStory";
            info = "The H1N1 virus has made them selves at home in your friend's body. \n\n\n" +
                   "Since there is no vaccine to be found anywhere, you decide to take your \n\n\n" +
                   "latest invention and shrink yourself down to be injected into his blood stream.  \n\n\n" +
                   "Make your way through the dirty little piggy virus's one wave at a time. Good luck!!";

            play = new MenuEntry(this, "Play");
            play.SetPosition(new Vector2(550, 600), true);
            play.Selected += PlaySelect;
            MenuEntries.Add(play);

            quit = new MenuEntry(this, "Quit Game");
            quit.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), play, true);
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

            spriteBatch.DrawString(TitleFont, title, titlePosition, new Color(62, 227, 66));
            spriteBatch.DrawString(SpriteFont, info, infoPosition, new Color(62, 227, 66));

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void PlaySelect(object sender, PlayerIndexEventArgs e)
        {
            music.Stop();
            Remove();
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new H1N1Level_01());
        }

        void QuitSelect(object sender, PlayerIndexEventArgs e)
        {
            music.Stop();
            Remove();
            ScreenManager.AddScreen(new H1N1MenuScreen(), e.PlayerIndex);
        }

        void MainMenuRemove(object sender, PlayerIndexEventArgs e)
        {
            MenuEntries.Clear();
        }
    }

}