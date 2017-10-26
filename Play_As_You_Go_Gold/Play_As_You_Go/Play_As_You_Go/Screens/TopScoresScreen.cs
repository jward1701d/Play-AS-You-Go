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
    class TopScoresScreen : MenuScreen
    {
        const string colorMimicHS = "CMHS.sfs";
        const string beyondSpaceHS = "BeyondSpaceScore.sfs";
        const string h1n1HS = "H1N1HighScore.sfs";

        SoundEffect sound;
        SoundEffectInstance music;

        SpriteFont TitleFont;
        Texture2D backgroundTexture;
        Vector2 backgroundPosition;
        Vector2 titlePosition;

        string title;

        float transitionTime;

        MenuEntry back;

        public TopScoresScreen()
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            SelectedColor = Color.CornflowerBlue;
            NonSelectedColor = Color.White;

        }
        public override void Initialize()
        {
            base.Initialize();

            backgroundPosition = new Vector2(0, 0);
            titlePosition = new Vector2((ScreenManager.Viewport.Width / 2) - 300, 32);

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
            TitleFont = content.Load<SpriteFont>("Fonts/BigBoldFont");
            backgroundTexture = content.Load<Texture2D>("Backgrounds/PlayAsYouGoMainMenuGreyScale");
            sound = content.Load<SoundEffect>("Audio/high3");
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (MediaPlayer.GameHasControl)
            {
                music.Play();
            }
            else
                music.Pause();

            base.Update(gameTime, covered);
        }

        public override void Draw(GameTime gameTime)
        {
            int screenWidth = ScreenManager.Viewport.Width;
            int screenHeight = ScreenManager.Viewport.Height;

            ISsystemData gdataOne = ISsystem.LoadHighScores(beyondSpaceHS, 5);
            ISsystemData gdataTwo = ISsystem.LoadHighScores(colorMimicHS, 5);
            ISsystemData gdataThree = ISsystem.LoadHighScores(h1n1HS, 5);
            
            title = "Top Scores";
            string[] playerOne = new string[5];
            int[] scoreOne = new int[5];
            int[] lvlOne = new int[5];
            int[] timedOne = new int[5];

            string[] playerTwo = new string[5];
            int[] scoreTwo = new int[5];
            int[] lvlTwo = new int[5];
            int[] timedTwo = new int[5];

            string[] playerThree = new string[5];
            int[] scoreThree = new int[5];
            int[] lvlThree = new int[5];
            int[] timedThree = new int[5];

            for (int i = 0; i < gdataOne.Count; i++)
            {
                playerOne[i] = gdataOne.playerName[i];
                scoreOne[i] = gdataOne.score[i];
                lvlOne[i] = gdataOne.level[i];
                timedOne[i] = gdataOne.playTime[i];

                playerTwo[i] = gdataTwo.playerName[i];
                scoreTwo[i] = gdataTwo.score[i];
                lvlTwo[i] = gdataTwo.level[i];
                timedTwo[i] = gdataTwo.playTime[i];

                playerThree[i] = gdataThree.playerName[i];
                scoreThree[i] = gdataThree.score[i];
                lvlThree[i] = gdataThree.level[i];
                timedThree[i] = gdataThree.playTime[i];
            }

            string[] scoringOne = new string[5];
            string[] levelsOne = new string[5];
            string[] playedOne = new string[5];

            string[] scoringTwo = new string[5];
            string[] levelsTwo = new string[5];
            string[] playedTwo = new string[5];

            string[] scoringThree = new string[5];
            string[] levelsThree = new string[5];
            string[] playedThree = new string[5];

            for (int i = 0; i < gdataOne.Count; i++)
            {
                scoringOne[i] = string.Format("{0}", scoreOne[i]);
                levelsOne[i] = string.Format("{0}", lvlOne[i]);
                playedOne[i] = string.Format("{0}", timedOne[i]);

                scoringTwo[i] = string.Format("{0}", scoreTwo[i]);
                levelsTwo[i] = string.Format("{0}", lvlTwo[i]);
                playedTwo[i] = string.Format("{0}", timedTwo[i]);

                scoringThree[i] = string.Format("{0}", scoreThree[i]);
                levelsThree[i] = string.Format("{0}", lvlThree[i]);
                playedThree[i] = string.Format("{0}", timedThree[i]);
            }

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.Gray);

            spriteBatch.DrawString(SpriteFont, "Name", new Vector2(screenWidth / 2 - 450, 230), Color.White);
            spriteBatch.DrawString(SpriteFont, "--------", new Vector2(screenWidth / 2 - 450, 245), Color.White);
            spriteBatch.DrawString(SpriteFont, playerOne[0], new Vector2(screenWidth / 2 - 450, 265), Color.White);
            spriteBatch.DrawString(SpriteFont, playerTwo[0], new Vector2(screenWidth / 2 - 450, 285), Color.White);
            spriteBatch.DrawString(SpriteFont, playerThree[0], new Vector2(screenWidth / 2 - 450, 305), Color.White);
            

            spriteBatch.DrawString(SpriteFont, "Score", new Vector2(screenWidth / 2 - 100, 230), Color.White);
            spriteBatch.DrawString(SpriteFont, "--------", new Vector2(screenWidth / 2 - 100, 245), Color.White);
            spriteBatch.DrawString(SpriteFont, scoringOne[0], new Vector2(screenWidth / 2 - 100, 265), Color.White);
            spriteBatch.DrawString(SpriteFont, scoringTwo[0], new Vector2(screenWidth / 2- 100, 285), Color.White);
            spriteBatch.DrawString(SpriteFont, scoringThree[0], new Vector2(screenWidth / 2 - 100, 305), Color.White);

            spriteBatch.DrawString(SpriteFont, "Game", new Vector2(screenWidth / 2 + 250, 230), Color.White);
            spriteBatch.DrawString(SpriteFont, "---------------------", new Vector2(screenWidth / 2 + 250, 245), Color.White);
            spriteBatch.DrawString(SpriteFont, "Beyond Space", new Vector2(screenWidth / 2 + 250, 265), Color.White);
            spriteBatch.DrawString(SpriteFont, "Color Mimic", new Vector2(screenWidth / 2 + 250, 285), Color.White);
            spriteBatch.DrawString(SpriteFont, "H1N1 a.k.a. Swine Flu", new Vector2(screenWidth / 2 + 250, 305), Color.White);
            

            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
        
        public override void UnloadContent()
        {
            SpriteFont = null;
            TitleFont = null;
        }
        void BackSelect(object sender, PlayerIndexEventArgs e)
        {
            Remove();
            music.Stop();
            ScreenManager.AddScreen(new PlayAsYouGoMainMenu(), e.PlayerIndex);
        }

        void MenuRemove(object sender, PlayerIndexEventArgs e)
        {
            MenuEntries.Clear();
        }
    }
}
