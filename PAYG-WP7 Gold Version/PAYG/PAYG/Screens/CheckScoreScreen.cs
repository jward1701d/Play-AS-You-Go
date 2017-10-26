/******************************************
 *       Check Score Screen Code          *  
 *              By                        *  
 *          James Ward                    *  
 *                                        *  
 * Property of Scrubby Fresh Studios      *  
 * 2011 all rights reserved.              *  
 *                                        *  
 ******************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PAYG
{
    class CheckScoreScreen : GameScreen
    {
        #region Fields
        // variables used in this class for this screen.

        Texture2D backgroundTexture;
        Vector2 backgroundPosition;

        ISsystemData gameData;
        int Score;
        int Level;
        int PlayTime;
        string FileName;
        Texture2D BgTexture;
        int HighScoreScreen;


        bool isInHS = false;

        int BgNum = 10;

        #endregion


        /// <summary>
        ///  Check Score System componet for ISsystem.
        /// HighScoreScreen Value (1) = Beyond Space
        /// HighScoreScreen Value (2) = Color Mimic
        /// HighScoreScreen Value (3) = H1N1
        /// </summary>
        /// <param name="data">Accepts the data struct to be modified.</param>
        /// <param name="score">Accepts the score to be checked against the score stored in the data structure.</param>
        /// <param name="level">Accepts the level number achieved to be added to the new spot in data struct</param>
        /// <param name="fileName">Accepts the name of the file to save the changes to.</param>
        /// <param name="highScoreScreen">Takes a number to represent which highscore screen to call (1) Beyond Space, (2) Color Mimic, (3) H1N1</param>
        public CheckScoreScreen(ISsystemData data, int score, int level, string fileName, int highScoreScreen)
        {
            Initialize(data, score, level, fileName, highScoreScreen);
            base.Initialize();
        }

        /// <summary>
        /// Check Score System componet for ISsystem.
        /// HighScoreScreen Value (1) = Beyond Space
        /// HighScoreScreen Value (2) = Color Mimic
        /// HighScoreScreen Value (3) = H1N1
        /// </summary>
        /// <param name="data">Accepts the data struct to be modified.</param>
        /// <param name="score">Accepts the score to be checked against the score stored in the data structure.</param>
        /// <param name="level">Accepts the level number achieved to be added to the new spot in data struct</param>
        /// <param name="playtime">Accepts the total play time achieved to be added to the new spot in data struct</param>
        /// <param name="fileName">Accepts the name of the file to save the changes to.</param>
        /// <param name="highScoreScreen">Takes a number to represent which highscore screen to call (1) Beyond Space, (2) Color Mimic, (3) H1N1</param>
        public CheckScoreScreen(ISsystemData data, int score, int level, int playtime, string fileName, int highScoreScreen, Texture2D bgTexture, int bgNum)
        {
            BgNum = bgNum;
            Initialize(data, score, level, playtime, fileName, highScoreScreen, bgTexture);
            base.Initialize();
        }


        public void Initialize(ISsystemData data, int score, int level, string fileName, int highScoreScreen)
        {
            // Sets all the variables perimeters 
            backgroundPosition = new Vector2(0, 0);
            gameData = data;
            Score = score;
            Level = level;
            FileName = fileName;
            HighScoreScreen = highScoreScreen;
            BgTexture = null;
        }


        public void Initialize(ISsystemData data, int score, int level, int playtime, string fileName, int highScoreScreen, Texture2D bgTexture)
        {
            // Sets all the variables perimeters 
            backgroundPosition = new Vector2(0, 0);
            gameData = data;
            Score = score;
            Level = level;
            PlayTime = playtime;
            FileName = fileName;
            HighScoreScreen = highScoreScreen;
            BgTexture = bgTexture;

        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            if (BgTexture == null)
            {
                backgroundTexture = content.Load<Texture2D>("Backgrounds/PlayAsYouGoMainMenuGreyScale");
            }
            else
            {
                backgroundTexture = BgTexture;
            }
        }

       
        public override void Update(GameTime gameTime, bool covered)
        {
            CheckScore(gameData, Score);

            if (!isInHS)
            {
                if (HighScoreScreen == 1)
                {
                    base.Remove();
                    //LoadingScreen.Load(ScreenManager, true, new BSHighScore());
                    ScreenManager.AddScreen(new BSHighScore());
                }
                if (HighScoreScreen == 2)
                {
                    base.Remove();
                    //LoadingScreen.Load(ScreenManager, true, new ColorMimicHighScore(BgTexture));
                    ScreenManager.AddScreen(new ColorMimicHighScore(BgTexture,BgNum));
                }
                if (HighScoreScreen == 3)
                {
                    base.Remove();
                    //LoadingScreen.Load(ScreenManager, true, new H1N1HighScoreScreen());
                    ScreenManager.AddScreen(new H1N1HighScoreScreen());
                }
            }
            else
            {
                if (HighScoreScreen == 1)
                {
                    base.Remove();
                    //LoadingScreen.Load(ScreenManager, true, new HighScoreInterface(gameData, Score, Level, FileName, HighScoreScreen));
                    ScreenManager.AddScreen(new HighScoreInterface(gameData, Score, Level, FileName, HighScoreScreen));
                }
                if (HighScoreScreen == 2)
                {
                    base.Remove();
                    //LoadingScreen.Load(ScreenManager, true, new HighScoreInterface(gameData, Score, Level, PlayTime, FileName, HighScoreScreen, backgroundTexture));
                    ScreenManager.AddScreen(new HighScoreInterface(gameData, Score, Level, PlayTime, FileName, HighScoreScreen, backgroundTexture,BgNum));
                }
                if (HighScoreScreen == 3)
                {
                    base.Remove();
                    //LoadingScreen.Load(ScreenManager, true, new HighScoreInterface(gameData, Score, Level, FileName, HighScoreScreen));
                    ScreenManager.AddScreen(new HighScoreInterface(gameData, Score, Level, FileName, HighScoreScreen));
                }
            }

        }


        #region CheckScore
        private void CheckScore(ISsystemData data, int score)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (score > data.score[i])
                {
                    isInHS = true;
                    break;
                }
            }
        }
        #endregion
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            ScreenManager.GraphicsDevice.Clear(Color.Black);
        }
    }
}

   

   



