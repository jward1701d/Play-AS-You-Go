/******************************************
 *     High Score Interface Code          *  
 *              By                        *  
 *          James Ward                    *  
 *                                        *  
 * Property of Scrubby Fresh Studios      *  
 * 2011 all rights reserved.              *  
 *                                        *
 * -Note modified to W7P scale 10/3/2011  *                                          
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
using System.Xml.Serialization;
using System.Diagnostics;
using Microsoft.Phone.Shell;


using System.Collections;
using System.Runtime.Serialization; //needed for data contracts
using System.Xml;

namespace PAYG
{
    class HighScoreInterface : GameScreen
    {
        #region Fields
        // variables used in this class for this screen.
        Texture2D ButtonsTexture;
        GameObject upArrow;
        GameObject downArrow;
        GameObject bButton;
        GameObject xButton;
        GameObject aButton;

        Texture2D underScoreTexture; // Added for the underscore game object.

        Texture2D[] CMBG = new Texture2D[8];

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
        int BgNum = 10;

        int posTracker;

        bool isInHS = true;//false;
        bool HSTSeffect = false;

        SpriteFont TitleFont;
        Vector2 backgroundPosition;
        Vector2 titlePosition;
        Vector2 creditPosition;

        SpriteFont spriteFont;

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
        string title;
        //This variable holds the position of what letter the player is changing 
        int letterPosition = 1;

        string pName;

        int controlStartPosX = 200;
        int controlStartPosY = 325;
        float controlScale = 2.0f;
        int pixelSpacing = 100;

        #endregion

        public HighScoreInterface(bool hsTsEffect)
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;
            
            HSTSeffect = hsTsEffect;
            
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
        /// <param name="fileName">Accepts the name of the file to save the changes to.</param>
        /// <param name="highScoreScreen">Takes a number to represent which highscore screen to call (1) Beyond Space, (2) Color Mimic, (3) H1N1</param>
        public HighScoreInterface(ISsystemData data, int score, int level, string fileName, int highScoreScreen)
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            Score = score;
            Level = level;
            FileName = fileName;
            HighScoreScreen = highScoreScreen;
            BgTexture = null;
            posTracker = 1;

            gameData = data;
            title = gameData.gameTitle[0];
            /*
            if (highScoreScreen == 1)
            {
                titlePosition = new Vector2(165, 20);
            }
            if (highScoreScreen == 2)
            {
                titlePosition = new Vector2(175, 20);
            }
            if (highScoreScreen == 3)
            {
                titlePosition = new Vector2(1, 20);
            }*/

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
        public HighScoreInterface(ISsystemData data, int score, int level, int playtime, string fileName, int highScoreScreen, Texture2D bgTexture, int bgNum)
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            Score = score;
            Level = level;
            FileName = fileName;
            HighScoreScreen = highScoreScreen;
            BgTexture = bgTexture;
            posTracker = 1;
            PlayTime = playtime;
            BgNum = bgNum;

            gameData = data;
            title = gameData.gameTitle[0];
            

            base.Initialize();
        }

        public override void Initialize()
        {
            PhoneApplicationService.Current.Deactivated += new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(SaveState);

            backgroundPosition = new Vector2(0, 0);

            ScreenManager.AdPositionX = 160;
            ScreenManager.AdPositionY = 395;
            ScreenManager.CreateAd();

            plyrNamePos1 = "a";
            plyrNamePos2 = plyrNamePos1;
            plyrNamePos3 = plyrNamePos2;
            plyrNamePos4 = plyrNamePos3;
            plyrNamePos5 = plyrNamePos4;

            if (HSTSeffect)
            {
                LoadLevel();
            }
            
            if (HighScoreScreen == 1)
            {
                titlePosition = new Vector2(165, 20);
            }
            if (HighScoreScreen == 2)
            {
                titlePosition = new Vector2(175, 20);
            }
            if (HighScoreScreen == 3)
            {
                titlePosition = new Vector2(1, 20);
            }

           

            #region Phone Buttons
            upArrow = new GameObject(ButtonsTexture, new Rectangle(0, 0, 32, 32), 1, true, 0);
            upArrow.Position = new Vector2(controlStartPosX, controlStartPosY);
            upArrow.Origin = new Vector2((upArrow.ActualWidth / 2) / upArrow.Scale, (upArrow.ActualHeight / 2) / upArrow.Scale);
            upArrow.Scale = controlScale;
            
            downArrow = new GameObject(ButtonsTexture, new Rectangle(32, 0, 32, 32), 1, true, 0);
            downArrow.Position = new Vector2(controlStartPosX + pixelSpacing, controlStartPosY);
            downArrow.Origin = new Vector2((downArrow.ActualWidth / 2) / downArrow.Scale, (downArrow.ActualHeight / 2) / downArrow.Scale);
            downArrow.Scale = controlScale;

            bButton = new GameObject(ButtonsTexture, new Rectangle(158, 0, 32, 32), 1, true, 0);
            bButton.Position = new Vector2(controlStartPosX + (pixelSpacing * 2), controlStartPosY);
            bButton.Origin = new Vector2((bButton.ActualWidth / 2) / bButton.Scale, (bButton.ActualHeight / 2) / bButton.Scale);
            bButton.Scale = controlScale;

            xButton = new GameObject(ButtonsTexture, new Rectangle(64, 0, 32, 32), 1, true, 0);
            xButton.Position = new Vector2(controlStartPosX + (pixelSpacing * 3), controlStartPosY);
            xButton.Origin = new Vector2((xButton.ActualWidth / 2) / xButton.Scale, (xButton.ActualHeight / 2) / xButton.Scale);
            xButton.Scale = controlScale;

            aButton = new GameObject(ButtonsTexture, new Rectangle(94, 0, 32, 32), 1, true, 0);
            aButton.Position = new Vector2(controlStartPosX + (pixelSpacing * 4), controlStartPosY);
            aButton.Origin = new Vector2((aButton.ActualWidth / 2) / aButton.Scale, (aButton.ActualHeight / 2) / aButton.Scale);
            aButton.Scale = controlScale;
            #endregion

            base.Initialize();
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            spriteFont = content.Load<SpriteFont>("Fonts/SmallFont");
            TitleFont = content.Load<SpriteFont>("Fonts/BigBoldFont");

            ButtonsTexture = content.Load<Texture2D>("Images/HighScoreInterfaceButtonsSpriteSheet");

            if (BgTexture == null && BgNum >=7)
            {
                BgNum = 7;
            }

            backgroundTexture = BgTexture; //content.Load<Texture2D>(BgTexture);
            underScoreTexture = content.Load<Texture2D>("Images/underscore");

            

            #region Background Textures
            CMBG[0] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Colorful");
            CMBG[1] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Bubbles");
            CMBG[2] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Magic");
            CMBG[3] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Color");
            CMBG[4] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Night sky");
            CMBG[5] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Stary night");
            CMBG[6] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Sunset");
            backupBackground = content.Load<Texture2D>("Backgrounds/PlayAsYouGoMainMenuGreyScale");
            CMBG[7] = backupBackground;
            
            #endregion
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
            upArrow.Update(gameTime);
            downArrow.Update(gameTime);
            bButton.Update(gameTime);
            xButton.Update(gameTime);
            aButton.Update(gameTime);

            if (HSTSeffect)
            {
                HSTSeffect = false;
            }
            //Added to fix the failure from not being able to hit back on the highscore interface screen.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && !ScreenManager.isPressed)
            {
                ScreenManager.isPressed = true;
                ScreenState = ScreenState.Frozen;
                ScreenManager.AddScreen(new PauseScreen(this));
            }
            else if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && ScreenManager.isPressed)
            {
                ScreenManager.isPressed = false;
                ScreenState = ScreenState.Active;
            }

            if (!isInHS)
            {
                if (HighScoreScreen == 1)
                {
                    ScreenManager.RemoveAd();
                    base.Remove();
                    ScreenManager.AddScreen(new BSHighScore());
                }
                if (HighScoreScreen == 2)
                {
                    ScreenManager.RemoveAd();
                    base.Remove();
                    ScreenManager.AddScreen(new ColorMimicHighScore(CMBG[BgNum],BgNum));
                }
                if (HighScoreScreen == 3)
                {
                    ScreenManager.RemoveAd();
                    base.Remove();
                    ScreenManager.AddScreen(new H1N1HighScoreScreen());
                }
            }

            //UP AND DOWN KEYS FOR HIGH SCORE METHOD 
            if(ScreenManager.InputSystem.IsRectanglePressed(this.upArrow.BoundingRect))
            {
                hiScoreUp();
            }
            else if (ScreenManager.InputSystem.IsRectanglePressed(this.downArrow.BoundingRect)) 
            {
                hiScoreDown();
            }

            //Side to side for each letter - not allowing to go higher than 5 letters or less than 1 
            if (ScreenManager.InputSystem.IsRectanglePressed(this.xButton.BoundingRect))
            {
                letterPosition++;
                posTracker++;
                if (letterPosition > 5)
                {
                    letterPosition = 5;
                    posTracker = 5;
                }
            }
            else if (ScreenManager.InputSystem.IsRectanglePressed(this.bButton.BoundingRect))
            {
                letterPosition--;
                posTracker--;
                if (letterPosition < 1)
                {
                    letterPosition = 1;
                    posTracker = 1;
                }
            }

            if (ScreenManager.InputSystem.IsRectanglePressed(this.aButton.BoundingRect))
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
                    ScreenManager.RemoveAd();
                    Remove();
                    ScreenManager.AddScreen(new BSHighScore());
                }
                if (HighScoreScreen == 2)
                {
                    ScreenManager.RemoveAd();
                    Remove();
                    ScreenManager.AddScreen(new ColorMimicHighScore(CMBG[BgNum],BgNum));
                }
                if (HighScoreScreen == 3)
                {
                    ScreenManager.RemoveAd();
                    Remove();
                    ScreenManager.AddScreen(new H1N1HighScoreScreen());
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
            gameData = data; // Added in for temp purposes.
            for (int i = 0; i < gameData.Count; i++)
            {
                if (score > gameData.score[i])
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
            gameData = data; // Added in for temp purposes.

            for (int i = 0; i < gameData.Count; i++)
            {
                if (score > gameData.score[i])
                {
                    scoreIndex = i;
                    break;
                }
            }

            if (scoreIndex > -1)
            {
                //New high score found ... do swaps
                for (int i = gameData.Count - 1; i > scoreIndex; i--)
                {
                    gameData.playerName[i] = gameData.playerName[i - 1];
                    gameData.score[i] = gameData.score[i - 1];
                    gameData.level[i] = gameData.level[i - 1];
                    gameData.playTime[i] = gameData.playTime[i - 1];
                }

                gameData.playerName[scoreIndex] = name; //Retrieve User Name Here
                gameData.score[scoreIndex] = score;
                gameData.level[scoreIndex] = level;
                gameData.playTime[scoreIndex] = PlayTime;

                ISsystem.SaveHighScores(gameData, fileName);
            }
        }

        private void CheckScore(ISsystemData data, int score, int level, int played, string fileName, string plyrName)
        {
            int scoreIndex = -1;
            string name = plyrName;
            gameData = data; // Added in for temp purposes.

            for (int i = 0; i < gameData.Count; i++)
            {
                if (score > gameData.score[i])
                {
                    scoreIndex = i;
                    break;
                }
            }

            if (scoreIndex > -1)
            {
                //New high score found ... do swaps
                for (int i = gameData.Count - 1; i > scoreIndex; i--)
                {
                    gameData.playerName[i] = gameData.playerName[i - 1];
                    gameData.score[i] = gameData.score[i - 1];
                    gameData.level[i] = gameData.level[i - 1];
                    gameData.playTime[i] = gameData.playTime[i - 1];
                }

                gameData.playerName[scoreIndex] = name;//name; //Retrieve User Name Here
                gameData.score[scoreIndex] = score;
                gameData.level[scoreIndex] = level;
                gameData.playTime[scoreIndex] = PlayTime;

                ISsystem.SaveHighScores(gameData, fileName);
            }
        }

        #endregion

        public override void Draw(GameTime gameTime)
        {
            int screenWidth = ScreenManager.Viewport.Width;
            int screenHeight = ScreenManager.Viewport.Height;
            creditPosition = new Vector2(175, 60);
            string message;
            
            string scoring = string.Format("{0}", Score);
            string played = string.Format("{0}", PlayTime);

            //string controls;

            float textScale = 0.75f;

            //controls = "TODO : Controls To Be Added";


            message = "High Score!";

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

           

           // if (backgroundTexture == null)
           // {
           //     spriteBatch.Draw(backupBackground, backgroundPosition, Color.Gray);
           // }
           // else
           // {

            if (BgNum == 10)
                BgNum = 7;
            spriteBatch.Draw(CMBG[BgNum], backgroundPosition, Color.Gray);
           // }

            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.White, 0.0f, new Vector2(0, 0), textScale, SpriteEffects.None, 1.0f);
            
            // Add screen messages and draws here.


            spriteBatch.DrawString(TitleFont, message, creditPosition, Color.White, 0.0f, new Vector2(0, 0), textScale, SpriteEffects.None, 1.0f);
            //spriteBatch.DrawString(spriteFont, controls, new Vector2((screenWidth / 2) - 200, (screenHeight / 2) + 200), Color.White);
            #region Under Score Graphics
            spriteBatch.Draw(underScoreTexture, new Vector2((screenWidth / 2) - 101, 242), Color.White); // prev Y = 292
            spriteBatch.Draw(underScoreTexture, new Vector2((screenWidth / 2) - 51, 242), Color.White);
            spriteBatch.Draw(underScoreTexture, new Vector2((screenWidth / 2) + 1, 242), Color.White);
            spriteBatch.Draw(underScoreTexture, new Vector2((screenWidth / 2) + 51, 242), Color.White);
            spriteBatch.Draw(underScoreTexture, new Vector2((screenWidth / 2) + 101, 242), Color.White);
            #endregion

            spriteBatch.DrawString(spriteFont, "SCORE", new Vector2((screenWidth / 2) - 40, 125), Color.White); // prev Y = 175
            spriteBatch.DrawString(spriteFont, scoring, new Vector2((screenWidth / 2) - 25, 150), Color.White); // prev Y = 200

            spriteBatch.DrawString(spriteFont, "INTIALS", new Vector2((screenWidth / 2) - 50, 200), Color.White); // prev Y = 250
            spriteBatch.DrawString(spriteFont, plyrNamePos1, new Vector2((screenWidth / 2) - 100, 230), Color.White); // prev Y = 280

            if (letterPosition > 1)
            {
                spriteBatch.DrawString(spriteFont, plyrNamePos2, new Vector2((screenWidth / 2) - 50, 230), Color.White);

                if (letterPosition > 2)
                {
                    spriteBatch.DrawString(spriteFont, plyrNamePos3, new Vector2((screenWidth / 2), 230), Color.White);

                    if (letterPosition > 3)
                    {
                        spriteBatch.DrawString(spriteFont, plyrNamePos4, new Vector2((screenWidth / 2) + 50, 230), Color.White);

                        if (letterPosition > 4)
                        {
                            spriteBatch.DrawString(spriteFont, plyrNamePos5, new Vector2((screenWidth / 2) + 100, 230), Color.White);
                        }
                    }
                }
            }

            #region Interface Graphics
            upArrow.Draw(gameTime, spriteBatch);
            downArrow.Draw(gameTime, spriteBatch);
            bButton.Draw(gameTime, spriteBatch);
            xButton.Draw(gameTime, spriteBatch);
            aButton.Draw(gameTime, spriteBatch);
            #region Button info
            spriteBatch.DrawString(spriteFont, "Scroll", new Vector2(controlStartPosX -45, controlStartPosY - 55), Color.White);
            spriteBatch.DrawString(spriteFont, "Scroll", new Vector2(controlStartPosX +60, controlStartPosY - 55), Color.White);
            spriteBatch.DrawString(spriteFont, "Back", new Vector2(controlStartPosX +170, controlStartPosY - 55), Color.White);
            spriteBatch.DrawString(spriteFont, "Next", new Vector2(controlStartPosX + 265, controlStartPosY - 55), Color.White);
            spriteBatch.DrawString(spriteFont, "Done", new Vector2(controlStartPosX + 365, controlStartPosY - 55), Color.White);
            #endregion
            #endregion
            spriteBatch.End();

        }

        protected void SaveState(object sender, DeactivatedEventArgs args)
        {
            // To do add tombstone code
            PhoneApplicationService.Current.State["Score"] = Score;
            PhoneApplicationService.Current.State["Level"] = Level;
            PhoneApplicationService.Current.State["FileName"] = FileName;
            PhoneApplicationService.Current.State["HighScoreScreen"] = HighScoreScreen;
            
            PhoneApplicationService.Current.State["PosTracker"] = posTracker;
            PhoneApplicationService.Current.State["PlayTime"] = PlayTime;
            PhoneApplicationService.Current.State["LtrCounter1"] = ltrCounter1;
            PhoneApplicationService.Current.State["LtrCounter2"] = ltrCounter2;
            PhoneApplicationService.Current.State["LtrCounter3"] = ltrCounter3;
            PhoneApplicationService.Current.State["LtrCounter4"] = ltrCounter4;
            PhoneApplicationService.Current.State["LtrCounter5"] = ltrCounter5;
            PhoneApplicationService.Current.State["PlyrNamePos1"] = plyrNamePos1;
            PhoneApplicationService.Current.State["PlyrNamePos2"] = plyrNamePos2;
            PhoneApplicationService.Current.State["PlyrNamePos3"] = plyrNamePos3;
            PhoneApplicationService.Current.State["PlyrNamePos4"] = plyrNamePos4;
            PhoneApplicationService.Current.State["PlyrNamePos5"] = plyrNamePos5;
            PhoneApplicationService.Current.State["LetterPostion"] = letterPosition;
            PhoneApplicationService.Current.State["CMBGnum"] = BgNum;
            PhoneApplicationService.Current.State["Title"] = title;
            PhoneApplicationService.Current.State["Name"] = pName;
            PhoneApplicationService.Current.State["ISdata"] = gameData;
            PhoneApplicationService.Current.State["ActiveGame"] = GameType.HIGHSCOREINTERFACE;
        }

        public void LoadLevel()
        {
            // to do add the recovery code.
            Score = (int)PhoneApplicationService.Current.State["Score"];
            Level = (int)PhoneApplicationService.Current.State["Level"];
            FileName = (string)PhoneApplicationService.Current.State["FileName"];
            HighScoreScreen = (int)PhoneApplicationService.Current.State["HighScoreScreen"];
            posTracker = (int)PhoneApplicationService.Current.State["PosTracker"];
            PlayTime = (int)PhoneApplicationService.Current.State["PlayTime"];
            ltrCounter1 = (int)PhoneApplicationService.Current.State["LtrCounter1"];
            ltrCounter2 = (int)PhoneApplicationService.Current.State["LtrCounter2"];
            ltrCounter3 = (int)PhoneApplicationService.Current.State["LtrCounter3"];
            ltrCounter4 = (int)PhoneApplicationService.Current.State["LtrCounter4"];
            ltrCounter5 = (int)PhoneApplicationService.Current.State["LtrCounter5"];
            plyrNamePos1 = (string)PhoneApplicationService.Current.State["PlyrNamePos1"];
            plyrNamePos2 = (string)PhoneApplicationService.Current.State["PlyrNamePos2"];
            plyrNamePos3 = (string)PhoneApplicationService.Current.State["PlyrNamePos3"];
            plyrNamePos4 = (string)PhoneApplicationService.Current.State["PlyrNamePos4"];
            plyrNamePos5 = (string)PhoneApplicationService.Current.State["PlyrNamePos5"];
            letterPosition = (int)PhoneApplicationService.Current.State["LetterPostion"];
            pName = (string)PhoneApplicationService.Current.State["Name"];
            BgNum = (int)PhoneApplicationService.Current.State["CMBGnum"];
            title = (string)PhoneApplicationService.Current.State["Title"];

            gameData = (ISsystemData)PhoneApplicationService.Current.State["ISdata"];
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


