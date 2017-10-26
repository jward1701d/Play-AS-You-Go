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
    class TopScoresScreen : MenuScreen
    {
        const string colorMimicHS = "CMHS.sfs";
        const string beyondSpaceHS = "BeyondSpaceScore.sfs";
        const string h1n1HS = "H1N1HighScore.sfs";

        SoundEffect sound;
        SoundEffectInstance music;

        SpriteFont TitleFont;
        Texture2D backgroundTexture;
        Vector2 titlePosition;
        GameObject background;

        string title;

        float transitionTime;

        MenuEntry back;

        public TopScoresScreen()
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            Selected = Color.CornflowerBlue;
           // NonSelectedColor = Color.White;

        }
        public override void Initialize()
        {
            base.Initialize();
            PhoneApplicationService.Current.Deactivated += new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(SaveState);
            int screenWidth = ScreenManager.Viewport.Width;


            background = new GameObject(backgroundTexture, new Rectangle(0, 0, 1280, 720), 1, true, 0);
            background.Position = new Vector2(0, 0);
            background.Scale = 0.67f;

            titlePosition = new Vector2((ScreenManager.Viewport.Width / 2) - 300, 32);

            back = new MenuEntry(this, "Back");
            back.SetPosition(new Vector2(screenWidth / 2 - 50, 400), true);
            back.Selected += new EventHandler(BackSelect);
            MenuEntries.Add(back);

            music = sound.CreateInstance();
            music.Volume = ScreenManager.MusicVolume;            

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


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                music.Stop();
                Remove();
                ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
            }

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
            background.Color = Color.Gray;
            background.Draw(gameTime, spriteBatch);
            //spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.Gray);

            spriteBatch.DrawString(SpriteFont, "Name", new Vector2(screenWidth / 2 - 375, 175), Color.White); // X =-450, Y= 230
            spriteBatch.DrawString(SpriteFont, "--------", new Vector2(screenWidth / 2 - 375, 190), Color.White);
            spriteBatch.DrawString(SpriteFont, playerOne[0], new Vector2(screenWidth / 2 - 375, 215), Color.White);
            spriteBatch.DrawString(SpriteFont, playerTwo[0], new Vector2(screenWidth / 2 - 375, 235), Color.White);
            spriteBatch.DrawString(SpriteFont, playerThree[0], new Vector2(screenWidth / 2 - 375, 255), Color.White);
            

            spriteBatch.DrawString(SpriteFont, "Score", new Vector2(screenWidth / 2 - 100, 175), Color.White);
            spriteBatch.DrawString(SpriteFont, "--------", new Vector2(screenWidth / 2 - 100, 190), Color.White);
            spriteBatch.DrawString(SpriteFont, scoringOne[0], new Vector2(screenWidth / 2 - 100, 215), Color.White);
            spriteBatch.DrawString(SpriteFont, scoringTwo[0], new Vector2(screenWidth / 2- 100, 235), Color.White);
            spriteBatch.DrawString(SpriteFont, scoringThree[0], new Vector2(screenWidth / 2 - 100, 255), Color.White);

            spriteBatch.DrawString(SpriteFont, "Game", new Vector2(screenWidth / 2 + 100, 175), Color.White); // +250
            spriteBatch.DrawString(SpriteFont, "---------------------", new Vector2(screenWidth / 2 + 100, 190), Color.White);
            spriteBatch.DrawString(SpriteFont, "Beyond Space", new Vector2(screenWidth / 2 + 100, 215), Color.White);
            spriteBatch.DrawString(SpriteFont, "Color Mimic", new Vector2(screenWidth / 2 + 100, 235), Color.White);
            spriteBatch.DrawString(SpriteFont, "H1N1 a.k.a. Swine Flu", new Vector2(screenWidth / 2 + 100, 255), Color.White);
            

            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
        
        public override void UnloadContent()
        {
            SpriteFont = null;
            TitleFont = null;
        }
        void BackSelect(object sender, EventArgs e)
        {
            Remove();
            music.Stop();
            ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
        }

        void MenuRemove(object sender)
        {
            MenuEntries.Clear();
        }
        protected void SaveState(object sender, DeactivatedEventArgs args)
        {
            PhoneApplicationService.Current.State["ActiveGame"] = GameType.MENU;
        }
    }
}
