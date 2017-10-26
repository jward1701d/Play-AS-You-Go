/******************************************
 *     High Score Interface Code          *  
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

namespace Play_As_You_Go
{
    class HighScoreInterface : GameScreen
    {
        #region Fields
        // variables used in this class for this screen.

        Texture2D Texture; // Added for the underscore game object.


        //string playerName;
        Texture2D backgroundTexture;
        Texture2D backupBackground;

        ISsystemData gameData;
        int Score;
        int Level;
        int PlayTime;
        string FileName;
        Texture2D BgTexture;
        int HighScoreScreen;

        int posTracker;

        bool isInHS = false;

        SpriteFont TitleFont;
        Vector2 backgroundPosition;
        Vector2 titlePosition;
        Vector2 creditPosition;

        SpriteFont spriteFont;
        SpriteFont controllerFont;

        //SoundEffect sound;
        //SoundEffectInstance music;

        float transitionTime;
        //MenuEntry back, quit;

        //Here is our alphabet array that we will use to fill the letter objects. 
        string[] alphabet = new string[] { " ", "a", "b", "c", "d", "e", "f", "g", 
                                            "h", "i", "j", "k", "l", "m", "n", "o", 
                                            "p", "q", "r", "s", "t", "u", "v", "w", 
                                            "x", "y", "z","1","2","3","4","5","6",
                                            "7","8","9","0"};

        //Index counters for the letters 
        int ltrCounter1, ltrCounter2, ltrCounter3, ltrCounter4, ltrCounter5;

        //Here are the letters of the players name 
        string plyrNamePos1, plyrNamePos2, plyrNamePos3, plyrNamePos4, plyrNamePos5;

        //This variable holds the position of what letter the player is changing 
        int letterPosition = 1;

        string pName;

        #endregion


        /// <summary>
        /// Graphic interface for ISsystem.
        /// HighScoreScreen Value (1) = Beyond Space
        /// HighScoreScreen Value (2) = Color Mimic
        /// HighScoreScreen Value (3) = H1N1
        /// </summary>
        /// <param name="data">Accepts the data struct to be modified.</param>
        /// <param name="score">Accepts the score to be checked against the score stored in the data structure.</param>
        /// <param name="level">Accepts the level number achieved to be added to the new spot in data struct</param>
        /// <param name="fileName">Accepts the name of the file to save the changes to.</param>
        /// <param name="highScoreScreen">Takes a number to represent which highscore screen to call (1) Beyond Space, (2) Color Mimic, (3) H1N1</param>
        public HighScoreInterface(ISsystemData data, int score, int level, string fileName, int highScoreScreen)
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;



            Initialize(data, score, level, fileName, highScoreScreen);

            base.Initialize();
        }
        
        /// <summary>
        /// Graphic interface for ISsystem.
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
        public HighScoreInterface(ISsystemData data, int score, int level, int playtime, string fileName, int highScoreScreen, Texture2D bgTexture)
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;



            Initialize(data, score, level, playtime, fileName, highScoreScreen, bgTexture);

            base.Initialize();
        }


        public void Initialize(ISsystemData data, int score, int level, string fileName, int highScoreScreen)
        {
            // Sets all the variables perimeters 

            backgroundPosition = new Vector2(0, 0);
            if (highScoreScreen == 1)
            {
                titlePosition = new Vector2(350, 40);
            }
            if (highScoreScreen == 2)
            {
                titlePosition = new Vector2(400, 40);
            }
            if (highScoreScreen == 3)
            {
                titlePosition = new Vector2(100, 40);
            }



            //Credits = "" ;

            gameData = data;

            plyrNamePos1 = " ";
            plyrNamePos2 = plyrNamePos1;
            plyrNamePos3 = plyrNamePos2;
            plyrNamePos4 = plyrNamePos3;
            plyrNamePos5 = plyrNamePos4;

            Score = score;
            Level = level;
            FileName = fileName;
            HighScoreScreen = highScoreScreen;
            BgTexture = null;
            posTracker = 1;


        }


        public void Initialize(ISsystemData data, int score, int level, int playtime, string fileName, int highScoreScreen, Texture2D bgTexture)
        {
            // Sets all the variables perimeters 

            backgroundPosition = new Vector2(0, 0);

            if (highScoreScreen == 1)
            {
                titlePosition = new Vector2(350, 40);
            }
            if (highScoreScreen == 2)
            {
                titlePosition = new Vector2(400, 40);
            }
            if (highScoreScreen == 3)
            {
                titlePosition = new Vector2(100, 40);
            }

            //Credits = "" ;

            gameData = data;

            plyrNamePos1 = " ";
            plyrNamePos2 = plyrNamePos1;
            plyrNamePos3 = plyrNamePos2;
            plyrNamePos4 = plyrNamePos3;
            plyrNamePos5 = plyrNamePos4;

            Score = score;
            Level = level;
            PlayTime = playtime;
            FileName = fileName;
            HighScoreScreen = highScoreScreen;
            BgTexture = bgTexture;

            posTracker = 1;


        }
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            spriteFont = content.Load<SpriteFont>("Fonts/SmallFont");
            TitleFont = content.Load<SpriteFont>("Fonts/BigBoldFont");
            controllerFont = content.Load<SpriteFont>("Fonts/xboxControllerSpriteFont");

            backupBackground = content.Load<Texture2D>("Backgrounds/PlayAsYouGoMainMenuGreyScale");

            backgroundTexture = BgTexture; //content.Load<Texture2D>(BgTexture);
            Texture = content.Load<Texture2D>("Images/underscore");
        }

        // Clears varables as this screen exits so they may be used later.
        public override void UnloadContent()
        {
            spriteFont = null;
            TitleFont = null;
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            CheckScore(gameData, Score);

            if (!isInHS)
            {
                if (HighScoreScreen == 1)
                {
                    base.Remove();
                    ScreenManager.AddScreen(new BSHighScore(), ControllingPlayer);
                }
                if (HighScoreScreen == 2)
                {
                    base.Remove();
                    ScreenManager.AddScreen(new ColorMimicHighScore(backgroundTexture), ControllingPlayer);
                }
                if (HighScoreScreen == 3)
                {
                    base.Remove();
                    ScreenManager.AddScreen(new H1N1HighScoreScreen(), ControllingPlayer);
                }
            }

            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = ScreenManager.InputSystem.currentKeyboardState[playerIndex];
            GamePadState gamepadState = ScreenManager.InputSystem.currentGamePadState[playerIndex];

            //UP AND DOWN KEYS FOR HIGH SCORE METHOD 
            if (ScreenManager.InputSystem.MenuUp(ControllingPlayer))
            {
                hiScoreUp();
            }
            else if (ScreenManager.InputSystem.MenuDown(ControllingPlayer))
            {
                hiScoreDown();
            }

            //Side to side for each letter - not allowing to go higher than 5 letters or less than 1 
            if (ScreenManager.InputSystem.highscoreInterfacespace(ControllingPlayer))
            {
                letterPosition++;
                posTracker++;
                if (letterPosition > 5)
                {
                    letterPosition = 5;
                    posTracker = 5;
                }
            }
            else if (ScreenManager.InputSystem.highscoreInterfaceBack(ControllingPlayer))
            {
                letterPosition--;
                posTracker--;
                if (letterPosition < 1)
                {
                    letterPosition = 1;
                    posTracker = 1;
                }
            }

            if (ScreenManager.InputSystem.highscoreInterfaceEnter(ControllingPlayer))
            {

                if (posTracker == 1)
                {
                    pName = PlayerName(plyrNamePos1);
                }
                if (posTracker == 2)
                {
                    pName = PlayerName(plyrNamePos1, plyrNamePos2);
                }
                if (posTracker == 3)
                {
                    pName = PlayerName(plyrNamePos1, plyrNamePos2, plyrNamePos3);
                }
                if (posTracker == 4)
                {
                    pName = PlayerName(plyrNamePos1, plyrNamePos2, plyrNamePos3, plyrNamePos4);
                }
                if (posTracker == 5)
                {
                    pName = PlayerName(plyrNamePos1, plyrNamePos2, plyrNamePos3, plyrNamePos4, plyrNamePos5);
                }

                if (HighScoreScreen == 2)
                {
                    CheckScore(gameData, Score, Level, PlayTime, FileName, pName);
                }
                else
                {
                    CheckScore(gameData, Score, Level, FileName, pName);
                }
                if (HighScoreScreen == 1)
                {
                    Remove();
                    ScreenManager.AddScreen(new BSHighScore(), ControllingPlayer);
                }
                if (HighScoreScreen == 2)
                {
                    Remove();
                    ScreenManager.AddScreen(new ColorMimicHighScore(backgroundTexture), ControllingPlayer);
                }
                if (HighScoreScreen == 3)
                {
                    Remove();
                    ScreenManager.AddScreen(new H1N1HighScoreScreen(), ControllingPlayer);
                }
            }
            base.Update(gameTime, IsExiting);
        }

        #region PlayerName
        private static string PlayerName(string playerNamePos1)
        {
            string playerName;
            playerName = playerNamePos1;
            return playerName;
        }

        private static string PlayerName(string playerNamePos1, string playerNamePos2)
        {
            string playerName;
            playerName = playerNamePos1 + playerNamePos2;
            return playerName;
        }

        private static string PlayerName(string playerNamePos1, string playerNamePos2, string playerNamePos3)
        {
            string playerName;
            playerName = playerNamePos1 + playerNamePos2 + playerNamePos3;
            return playerName;
        }

        private static string PlayerName(string playerNamePos1, string playerNamePos2, string playerNamePos3, string playerNamePos4)
        {
            string playerName;
            playerName = playerNamePos1 + playerNamePos2 + playerNamePos3 + playerNamePos4;
            return playerName;
        }

        private static string PlayerName(string playerNamePos1, string playerNamePos2, string playerNamePos3, string playerNamePos4, string playerNamePos5)
        {
            string playerName;
            playerName = playerNamePos1 + playerNamePos2 + playerNamePos3 + playerNamePos4 + playerNamePos5;
            return playerName;
        }
        #endregion

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

        private void CheckScore(ISsystemData data, int score, int level, string fileName, string plyrName)
        {
            int scoreIndex = -1;
            string name = plyrName;

            for (int i = 0; i < data.Count; i++)
            {
                if (score > data.score[i])
                {
                    scoreIndex = i;
                    break;
                }
            }

            if (scoreIndex > -1)
            {
                //New high score found ... do swaps
                for (int i = data.Count - 1; i > scoreIndex; i--)
                {
                    data.playerName[i] = data.playerName[i - 1];
                    data.score[i] = data.score[i - 1];
                    data.level[i] = data.level[i - 1];
                    data.playTime[i] = data.playTime[i - 1];
                }

                data.playerName[scoreIndex] = name; //Retrieve User Name Here
                data.score[scoreIndex] = score;
                data.level[scoreIndex] = level;
                data.playTime[scoreIndex] = PlayTime;

                ISsystem.SaveHighScores(data, fileName);
            }
        }

        private void CheckScore(ISsystemData data, int score, int level, int played, string fileName, string plyrName)
        {
            int scoreIndex = -1;
            string name = plyrName;

            for (int i = 0; i < data.Count; i++)
            {
                if (score > data.score[i])
                {
                    scoreIndex = i;
                    break;
                }
            }

            if (scoreIndex > -1)
            {
                //New high score found ... do swaps
                for (int i = data.Count - 1; i > scoreIndex; i--)
                {
                    data.playerName[i] = data.playerName[i - 1];
                    data.score[i] = data.score[i - 1];
                    data.level[i] = data.level[i - 1];
                    data.playTime[i] = data.playTime[i - 1];
                }

                data.playerName[scoreIndex] = name; //Retrieve User Name Here
                data.score[scoreIndex] = score;
                data.level[scoreIndex] = level;
                data.playTime[scoreIndex] = PlayTime;

                ISsystem.SaveHighScores(data, fileName);
            }
        }

        #endregion

        public override void Draw(GameTime gameTime)
        {
            int screenWidth = ScreenManager.Viewport.Width;
            int screenHeight = ScreenManager.Viewport.Height;
            creditPosition = new Vector2(375, 150);
            
            string message;
            string title = gameData.gameTitle[0];
            string scoring = string.Format("{0}", Score);
            string played = string.Format("{0}", PlayTime);

            string controls;

#if WINDOWS
            controls = "[Arrow Up/Arrow Down] to scroll\n" + "[Space Bar] for next letter\n" + "[Backspace] to delete letter\n" + "[Enter] when finished";
#else
            string aButton = "'";
            string bButton = ")";
            string thumb = " ";
            string xButton= "&";
            controls = "to scroll\n"+"for next letter\n"+"to delete letter\n"+"when finished";
#endif

            message = "High Score!";

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            if (backgroundTexture == null)
            {
                spriteBatch.Draw(backupBackground, backgroundPosition, Color.White);
            }
            else
            {
                spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.Gray);
            }

            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.White);

            // Add screen messages and draws here.


            spriteBatch.DrawString(TitleFont, message, creditPosition, Color.White);
#if WINDOWS            
            spriteBatch.DrawString(spriteFont, controls, new Vector2((screenWidth / 2) - 200, (screenHeight / 2) + 200), Color.White);
#else
            spriteBatch.DrawString(spriteFont, controls, new Vector2((screenWidth / 2) - 50, (screenHeight / 2) + 200), Color.White);
            spriteBatch.DrawString(controllerFont, thumb, new Vector2(550,553), Color.White, 0f, new Vector2(0, 0), 0.15f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(controllerFont, xButton, new Vector2(553,577), Color.White, 0f, new Vector2(0, 0), 0.20f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(controllerFont, bButton, new Vector2(553,603), Color.White, 0f, new Vector2(0, 0), 0.20f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(controllerFont, aButton, new Vector2(553,628), Color.White, 0f, new Vector2(0, 0), 0.20f, SpriteEffects.None, 0f);
#endif
            #region Under Score Graphics
            spriteBatch.Draw(Texture, new Vector2((screenWidth / 2) - 101, 430), Color.White);
            spriteBatch.Draw(Texture, new Vector2((screenWidth / 2) - 51, 430), Color.White);
            spriteBatch.Draw(Texture, new Vector2((screenWidth / 2) + 1, 430), Color.White);
            spriteBatch.Draw(Texture, new Vector2((screenWidth / 2) + 51, 430), Color.White);
            spriteBatch.Draw(Texture, new Vector2((screenWidth / 2) + 101, 430), Color.White);
            #endregion

            spriteBatch.DrawString(spriteFont, "SCORE", new Vector2((screenWidth / 2) - 40, 300), Color.White);
            spriteBatch.DrawString(spriteFont, scoring, new Vector2((screenWidth / 2) - 25, 325), Color.White);

            spriteBatch.DrawString(spriteFont, "INTIALS", new Vector2((screenWidth / 2) - 50, 390), Color.White);
            spriteBatch.DrawString(spriteFont, plyrNamePos1, new Vector2((screenWidth / 2) - 100, 420), Color.White);

            if (letterPosition > 1)
            {
                spriteBatch.DrawString(spriteFont, plyrNamePos2, new Vector2((screenWidth / 2) - 50, 420), Color.White);

                if (letterPosition > 2)
                {
                    spriteBatch.DrawString(spriteFont, plyrNamePos3, new Vector2((screenWidth / 2), 420), Color.White);

                    if (letterPosition > 3)
                    {
                        spriteBatch.DrawString(spriteFont, plyrNamePos4, new Vector2((screenWidth / 2) + 50, 420), Color.White);

                        if (letterPosition > 4)
                        {
                            spriteBatch.DrawString(spriteFont, plyrNamePos5, new Vector2((screenWidth / 2) + 100, 420), Color.White);
                        }
                    }
                }
            }

            spriteBatch.End();

        }


        ///////////////////////NOW HERE ARE THE METHODS TO CHANGE YOUR LETTERS 
        //Case statement looks at where we are in the 5 letters and changes that one - IT also loops us through the alphabet and starts again on the other side so we do not go out of the array index.     
        private void hiScoreUp()
        {
            switch (letterPosition)
            {
                case 1:
                    ltrCounter1++;
                    if (ltrCounter1 > 36)
                    {
                        ltrCounter1 = 0;
                    }
                    plyrNamePos1 = alphabet[ltrCounter1];
                    break;
                case 2:
                    ltrCounter2++;
                    if (ltrCounter2 > 36)
                    {
                        ltrCounter2 = 0;
                    }
                    plyrNamePos2 = alphabet[ltrCounter2];
                    break;
                case 3:
                    ltrCounter3++;
                    if (ltrCounter3 > 36)
                    {
                        ltrCounter3 = 0;
                    }
                    plyrNamePos3 = alphabet[ltrCounter3];
                    break;
                case 4:
                    ltrCounter4++;
                    if (ltrCounter4 > 36)
                    {
                        ltrCounter4 = 0;
                    }
                    plyrNamePos4 = alphabet[ltrCounter4];
                    break;
                case 5:
                    ltrCounter5++;
                    if (ltrCounter5 > 36)
                    {
                        ltrCounter5 = 0;
                    }
                    plyrNamePos5 = alphabet[ltrCounter5];
                    break;
            }
        }
        //Same thing only backwards. 
        private void hiScoreDown()
        {
            switch (letterPosition)
            {
                case 1:
                    ltrCounter1--;
                    if (ltrCounter1 < 0)
                    {
                        ltrCounter1 = 36;
                    }
                    plyrNamePos1 = alphabet[ltrCounter1];
                    break;
                case 2:
                    ltrCounter2--;
                    if (ltrCounter2 < 0)
                    {
                        ltrCounter2 = 36;
                    }
                    plyrNamePos2 = alphabet[ltrCounter2];
                    break;
                case 3:
                    ltrCounter3--;
                    if (ltrCounter3 < 0)
                    {
                        ltrCounter3 = 36;
                    }
                    plyrNamePos3 = alphabet[ltrCounter3];
                    break;
                case 4:
                    ltrCounter4--;
                    if (ltrCounter4 < 0)
                    {
                        ltrCounter4 = 36;
                    }
                    plyrNamePos4 = alphabet[ltrCounter4];
                    break;
                case 5:
                    ltrCounter5--;
                    if (ltrCounter5 < 0)
                    {
                        ltrCounter5 = 36;
                    }
                    plyrNamePos5 = alphabet[ltrCounter5];
                    break;
            }
        }
    }
}


