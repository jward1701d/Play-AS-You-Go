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
    class ColorMimicHighScore : MenuScreen
    {
        const string saveGameFile = "CMHS.sfs";

        SoundEffect sound;
        SoundEffectInstance music;
        
        SpriteFont TitleFont;
        Texture2D backgroundTexture;
        Texture2D BgTexture;
        Vector2 backgroundPosition;
        Vector2 titlePosition;

        string title;

        float transitionTime;

        MenuEntry back;

        public ColorMimicHighScore(Texture2D bgTexture)
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            SelectedColor= Color.CornflowerBlue;
            NonSelectedColor = Color.White;

           
            BgTexture = bgTexture;
        }

        public override void Initialize()
        {
            base.Initialize();

            backgroundPosition = new Vector2(0, 0);
            titlePosition = new Vector2((ScreenManager.Viewport.Width/2) -200, 32);

            back = new MenuEntry(this, "Back");
            back.SetPosition(new Vector2(620, 600), true);
            back.Selected += BackSelect;
            MenuEntries.Add(back);

            music = sound.CreateInstance();
            music.Volume = 0.25f;
           
                        
        }


        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            SpriteFont = content.Load<SpriteFont>("Fonts/Color Mimic Fonts/CMscoreFont");
            TitleFont = content.Load<SpriteFont>("Fonts/Color Mimic Fonts/CMTitle");
            backgroundTexture = BgTexture;//content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Colorful");
            sound = content.Load<SoundEffect>("Audio/high3");
        }

        public override void UnloadContent()
        {
            SpriteFont = null;
            TitleFont = null;
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (MediaPlayer.GameHasControl)
            {
                music.Play();
            }
            else
                music.Pause();
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
            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.Gray);

            spriteBatch.DrawString(SpriteFont, "Name", new Vector2(screenWidth / 2 - 250, 230), Color.White);
            spriteBatch.DrawString(SpriteFont, "--------", new Vector2(screenWidth / 2 - 250, 245), Color.White);
            spriteBatch.DrawString(SpriteFont, player[0], new Vector2(screenWidth / 2 - 250, 265), Color.White);
            spriteBatch.DrawString(SpriteFont, player[1], new Vector2(screenWidth / 2 - 250, 285), Color.White);
            spriteBatch.DrawString(SpriteFont, player[2], new Vector2(screenWidth / 2 - 250, 305), Color.White);
            spriteBatch.DrawString(SpriteFont, player[3], new Vector2(screenWidth / 2 - 250, 325), Color.White);
            spriteBatch.DrawString(SpriteFont, player[4], new Vector2(screenWidth / 2 - 250, 345), Color.White);

            spriteBatch.DrawString(SpriteFont, "Score", new Vector2(screenWidth / 2 , 230), Color.White);
            spriteBatch.DrawString(SpriteFont, "--------", new Vector2(screenWidth / 2 , 245), Color.White);
            spriteBatch.DrawString(SpriteFont, scoring[0], new Vector2(screenWidth / 2, 265), Color.White);
            spriteBatch.DrawString(SpriteFont, scoring[1], new Vector2(screenWidth / 2, 285), Color.White);
            spriteBatch.DrawString(SpriteFont, scoring[2], new Vector2(screenWidth / 2, 305), Color.White);
            spriteBatch.DrawString(SpriteFont, scoring[3], new Vector2(screenWidth / 2, 325), Color.White);
            spriteBatch.DrawString(SpriteFont, scoring[4], new Vector2(screenWidth / 2, 345), Color.White);

            spriteBatch.DrawString(SpriteFont, "Total Play Time", new Vector2(screenWidth / 2 + 200, 230), Color.White);
            spriteBatch.DrawString(SpriteFont, "----------------------", new Vector2(screenWidth / 2 + 200, 245), Color.White);
            spriteBatch.DrawString(SpriteFont, played[0] + " Seconds", new Vector2(screenWidth / 2 + 225, 265), Color.White);
            spriteBatch.DrawString(SpriteFont, played[1] + " Seconds", new Vector2(screenWidth / 2 + 225, 285), Color.White);
            spriteBatch.DrawString(SpriteFont, played[2] + " Seconds", new Vector2(screenWidth / 2 + 225, 305), Color.White);
            spriteBatch.DrawString(SpriteFont, played[3] + " Seconds", new Vector2(screenWidth / 2 + 225, 325), Color.White);
            spriteBatch.DrawString(SpriteFont, played[4] + " Seconds", new Vector2(screenWidth / 2 + 225, 345), Color.White);


            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void BackSelect(object sender, PlayerIndexEventArgs e)
        {
            Remove();
            music.Stop();
            ScreenManager.AddScreen(new ColorMimicMainMenu(), e.PlayerIndex);
        }

        void MenuRemove(object sender, PlayerIndexEventArgs e)
        {
            MenuEntries.Clear();
        }
    }
}
