﻿#region Info
/***********************************************************
 * Level_02.cs                                             *
 *                                                         *
 *  Written by: Johnathan Witvoet                          *
 ***********************************************************/

#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Shell;

using System.Collections;
using System.Runtime.Serialization; //needed for data contracts
using System.Xml;

#endregion

#region Name Space

namespace PAYG
{
    #region Class

    class H1N1Level_02 : GameScreen
    {
        #region Fields

        bool didRestart = false;

        // Textures to use
        Texture2D Background;
        Texture2D SpriteSheet;
        Texture2D Bars;
        Texture2D HealthBar;
        Texture2D CellsLeft;
        Texture2D CellsRight;

        // Hud Object
        Hud hud;

        // Game Objects
        GameObject bars, background, paddle, paddleColision, block, extraLife;

        GameObject[] rowblocks = new GameObject[2];

        GameObject[] balls = new GameObject[1];

        GameObject[] pigRow1 = new GameObject[2];
        GameObject[] pigRow2 = new GameObject[3];
        GameObject[] pigRow3 = new GameObject[5];
        GameObject[] pigRow4 = new GameObject[3];
        GameObject[] pigRow5 = new GameObject[5];
        GameObject[] pigRow6 = new GameObject[6];
        GameObject[] pigRow7 = new GameObject[6];

        GameObject[] cellsLeft = new GameObject[5];
        GameObject[] cellsRight = new GameObject[5];

        // Random variable
        Random rnd = new Random();

        // Variables to hold paddle and ball speed,radius, each rows space and highscore
        float paddleSpeed;

        float ballSpeed;
        float radius = 8;

        int row1Space = 0;
        int row2Space = 0;
        int row3Space = 0;
        int row4Space = 0;
        int row5Space = 0;
        int row6Space = 0;
        int row7Space = 0;
        int BlockrowSpace = 0;

        int highScore = 920;

        // Variable to hold pigs rectangle 4 corners
        public float[] CornerMarkers;

        // Background Music , volume
        SoundEffect backgroundEffect;
        SoundEffectInstance effectBackground;
        float backgroundVolume;

        // Sounds effects, volume
        SoundEffect pigEffect;
        SoundEffectInstance effectPig;
        SoundEffect wallEffect;
        SoundEffectInstance effectWall;
        float pigVolume;
        float wallVolume;

        // has to be same thing on all levels
        const string HighScore = "H1N1HighScore.sfs";
        const string SaveGameData = "H1N1GameData.sfs";
        const string H1N1GameName = "H1N1 A.K.A Swine Flu";
        ISsystemData Data = ISsystem.LoadHighScores(HighScore, 5);
        ISsystemData H1N1LevelData = ISsystem.LoadInfo(SaveGameData);

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public H1N1Level_02()
        {
            TransitionOnTime = TimeSpan.FromSeconds(2);
            TransitionOffTime = TimeSpan.FromSeconds(1);
        }

        public H1N1Level_02(bool active)
        {
            TransitionOnTime = TimeSpan.FromSeconds(2);
            TransitionOffTime = TimeSpan.FromSeconds(1);
            didRestart = active;
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes all of our Game Objects
        /// </summary>
        public override void Initialize()
        {
            PhoneApplicationService.Current.Deactivated += new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(SaveState);

            // Initializes a backround object and giving it a position
            background = new GameObject(Background, new Rectangle(0, 0, 1280, 720), 1, true, 0);
            background.Position = new Vector2(-20, 0);
            background.Scale = .67f;


            // Initializes bars for the pill to seem to bounce off of
            bars = new GameObject(Bars, new Rectangle(0, 0, 1280, 720), 1, true, 0);
            bars.Position = new Vector2(-28, 0);
            bars.Scale = .67f;


            #region Initialize Paddle

            // Initializes players paddel and give it a positon , origin, scale and speed
            paddle = new GameObject(SpriteSheet, new Rectangle(13, 286, 640, 115), 1, true, 0);
            paddle.Position = new Vector2(400, 465);
            paddle.Origin = new Vector2((paddle.ActualWidth / 2) / paddle.Scale, (paddle.ActualHeight / 2) / paddle.Scale);
            paddle.Scale = 0.35f;
            paddleSpeed = 8;



            paddleColision = new GameObject(SpriteSheet, new Rectangle(15, 417, 640, 3), 1, true, 0);
            paddleColision.Position = new Vector2(400, 464);
            paddleColision.Origin = new Vector2((paddleColision.ActualWidth / 2) / paddleColision.Scale, (paddleColision.ActualHeight / 2) / paddleColision.Scale);
            paddleColision.Scale = 0.35f;

            #endregion

            // Initialize ball and set active to false
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i] = new GameObject(SpriteSheet, new Rectangle(718, 9, 64, 64), 4, true, 0);
                balls[i].Active = false;
            }

            // set ball speed
            ballSpeed = 1;

            #region Initialize Pigs

            // Initialize row 1 of pigs and give it a positon, origin, scale and active to true
            for (int a = 0; a < pigRow1.Length; a++)
            {
                GameObject row1Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow1[a] = row1Pig;
                row1Pig.Position = new Vector2(245 + row1Space, 150);
                row1Pig.Origin = new Vector2((row1Pig.ActualWidth / 2) / row1Pig.Scale, (row1Pig.ActualHeight / 2) / row1Pig.Scale);
                row1Pig.Scale = 0.11f;
                row1Pig.Active = true;
                row1Space += 280;

            }

            // Initialize row 2 of pigs and give it a positon, origin, scale and active to true
            for (int b = 0; b < pigRow2.Length; b++)
            {
                GameObject row2Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow2[b] = row2Pig;
                row2Pig.Position = new Vector2(200 + row2Space, 195);
                row2Pig.Origin = new Vector2((row2Pig.ActualWidth / 2) / row2Pig.Scale, (row2Pig.ActualHeight / 2) / row2Pig.Scale);
                row2Pig.Scale = 0.11f;
                row2Pig.Active = true;
                row2Space += 45;

            }

            // Initialize row 3 of pigs and give it a positon, origin, scale and active to true
            for (int c = 0; c < pigRow3.Length; c++)
            {
                GameObject row3Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow3[c] = row3Pig;              
                row3Pig.Position = new Vector2(155 + row3Space, 240);
                row3Pig.Origin = new Vector2((row3Pig.ActualWidth / 2) / row3Pig.Scale, (row3Pig.ActualHeight / 2) / row3Pig.Scale);
                row3Pig.Scale = 0.11f;
                row3Pig.Active = true;
                row3Space += 45;
            }

            // Initialize row 4 of pigs and give it a positon, origin, scale and active to true
            for (int d = 0; d < pigRow4.Length; d++)
            {
                GameObject row4Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow4[d] = row4Pig;              
                row4Pig.Position = new Vector2(485 + row4Space, 195);
                row4Pig.Origin = new Vector2((row4Pig.ActualWidth / 2) / row4Pig.Scale, (row4Pig.ActualHeight / 2) / row4Pig.Scale);
                row4Pig.Scale = 0.11f;
                row4Pig.Active = true;
                row4Space += 45;
            }

            // Initialize row 5 of pigs and give it a positon, origin, scale and active to true
            for (int e = 0; e < pigRow5.Length; e++)
            {
                GameObject row5Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow5[e] = row5Pig;              
                row5Pig.Position = new Vector2(440 + row5Space, 240);
                row5Pig.Origin = new Vector2((row5Pig.ActualWidth / 2) / row5Pig.Scale, (row5Pig.ActualHeight / 2) / row5Pig.Scale);
                row5Pig.Scale = 0.11f;
                row5Pig.Active = true;
                row5Space += 45;
            }

            // Initialize row 6 of pigs and give it a positon, origin, scale and active to true
            for (int f = 0; f < pigRow6.Length; f++)
            {
                GameObject row6Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow6[f] = row6Pig;               
                row6Pig.Position = new Vector2(135 + row6Space, 285);
                row6Pig.Origin = new Vector2((row6Pig.ActualWidth / 2) / row6Pig.Scale, (row6Pig.ActualHeight / 2) / row6Pig.Scale);
                row6Pig.Scale = 0.11f;
                row6Pig.Active = true;
                row6Space += 45;

            }

            // Initialize row 7 of pigs and give it a positon, origin, scale and active to true
            for (int f = 0; f < pigRow7.Length; f++)
            {
                GameObject row7Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow7[f] = row7Pig;             
                row7Pig.Position = new Vector2(425 + row7Space, 285);
                row7Pig.Origin = new Vector2((row7Pig.ActualWidth / 2) / row7Pig.Scale, (row7Pig.ActualHeight / 2) / row7Pig.Scale);
                row7Pig.Scale = 0.11f;
                row7Pig.Active = true;
                row7Space += 45;
            }

            #endregion

            #region Initialize background blood cells right and left

            // Initialize cell facing left background flow
            for (int i = 0; i < cellsLeft.Length; i++)
            {
                GameObject cellLeft = new GameObject(CellsLeft, new Rectangle(0, 0, 256, 256), 1, true, 0);
                cellsLeft[i] = cellLeft;
                cellLeft.Position = new Vector2(rnd.Next(ScreenManager.Viewport.Width), rnd.Next(ScreenManager.Viewport.Height));
                cellLeft.Speed = (float)rnd.NextDouble() * 5 + 2;
                cellLeft.Scale = .35f;

            }

            // Initialize cell facing right background flow
            for (int i = 0; i < cellsRight.Length; i++)
            {
                GameObject cellRight = new GameObject(CellsRight, new Rectangle(0, 0, 256, 256), 1, true, 0);
                cellsRight[i] = cellRight;
                cellRight.Position = new Vector2(rnd.Next(ScreenManager.Viewport.Width), rnd.Next(ScreenManager.Viewport.Height));
                cellRight.Speed = (float)rnd.NextDouble() * 20 + 2;
                cellRight.Scale = 0.35f;

            }

            #endregion

            #region Initialize block

            // Initializes players paddel and give it a positon , origin, scale and speed
            block = new GameObject(SpriteSheet, new Rectangle(14, 26, 160, 32), 1, true, 0);
            block.Position = new Vector2(387, 160);
            block.Origin = new Vector2((block.ActualWidth / 2) / block.Scale, (block.ActualHeight / 2) / block.Scale);
            block.Scale = 1.0f;


            // Initialize row 7 of pigs and give it a positon, origin, scale and active to true
            for (int a = 0; a < rowblocks.Length; a++)
            {
                GameObject Blockrow = new GameObject(SpriteSheet, new Rectangle(14, 26, 160, 32), 1, true, 0);
                rowblocks[a] = Blockrow;
                Blockrow.Position = new Vector2(250 + BlockrowSpace, 350);
                Blockrow.Origin = new Vector2((Blockrow.ActualWidth / 2) / Blockrow.Scale, (Blockrow.ActualHeight / 2) / Blockrow.Scale);
                Blockrow.Scale = 1.0f;
                Blockrow.Active = true;
                BlockrowSpace += 300;
            }

            #endregion

            extraLife = new GameObject(SpriteSheet, new Rectangle(192, 11, 256, 256), 2, true, 0);
            extraLife.AnimationSpeed = TimeSpan.FromMilliseconds(600);
            extraLife.Position = new Vector2(385, 125);
            extraLife.Origin = new Vector2((extraLife.ActualWidth / 2) / extraLife.Scale, (extraLife.ActualHeight / 2) / extraLife.Scale);
            extraLife.Scale = 0.12f;

            backgroundVolume = ScreenManager.MusicVolume;
            wallVolume = ScreenManager.FXVolume;
            pigVolume = ScreenManager.FXVolume;
            // Initalizes all sounds in game
            effectBackground = backgroundEffect.CreateInstance();
            effectBackground.Volume = backgroundVolume;
            effectWall = wallEffect.CreateInstance();
            effectWall.Volume = wallVolume;
            effectPig = pigEffect.CreateInstance();

            hud.Score = H1N1LevelData.score[0];
            hud.Life = H1N1LevelData.lives[0];
            hud.Level = 2;

            if (didRestart)
                LoadLevel();

            ScreenManager.AdPositionX = 320;
            ScreenManager.AdPositionY = 0;
            ScreenManager.CreateAd();
            
        }

        #endregion

        #region Load and UnLoad Content

        /// <summary>
        /// Loads all of our content we need for our game
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            // loads all of our textures "content"
            Background = content.Load<Texture2D>("Backgrounds/H1N1Backgrounds/H1N1Background");

            SpriteSheet = content.Load<Texture2D>("Images/H1N1Images/H1N1SpriteSheet");

            Bars = content.Load<Texture2D>("Backgrounds/H1N1Backgrounds/bars");

            HealthBar = content.Load<Texture2D>("Images/H1N1Images/HealthBar");

            hud = new Hud();
            hud.Font = content.Load<SpriteFont>("Fonts/H1N1Fonts/H1N1ScoreFont");

            CellsLeft = content.Load<Texture2D>("Backgrounds/H1N1Backgrounds/CellLeft");
            CellsRight = content.Load<Texture2D>("Backgrounds/H1N1Backgrounds/CellRight");

            // Loads our sounds
            backgroundEffect = content.Load<SoundEffect>("Audio/H1N1Audio/birth5");
            pigEffect = content.Load<SoundEffect>("Audio/H1N1Audio/p3");
            wallEffect = content.Load<SoundEffect>("Audio/H1N1Audio/RedShort");

        }

        /// <summary>
        /// UnLoads all of our Content
        /// </summary>
        public override void UnloadContent()
        {
            Background = null;
            SpriteSheet = null;
            Bars = null;
            HealthBar = null;            
            CellsLeft = null;
            CellsRight = null;
            backgroundEffect = null;
            pigEffect = null;
            wallEffect = null;
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates all of our Game Objects
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="covered"></param>
        public override void Update(GameTime gameTime, bool covered)
        {
            #region pause update
            TouchPanel.EnabledGestures = GestureType.Hold;
            TouchPanel.EnabledGestures = GestureType.Tap;
            TouchPanel.EnabledGestures = GestureType.DoubleTap;

            foreach (GestureSample gesture in ScreenManager.InputSystem.Gestures)
            {
                if (gesture.GestureType == GestureType.Hold)
                {
                    ScreenManager.isPressed = true;
                    effectBackground.Stop();
                    ScreenState = ScreenState.Frozen;
                    ScreenManager.AddScreen(new PauseScreen(this));
                }
                else
                {
                    ScreenState = ScreenState.Active;
                }
            }
            #endregion

            effectBackground.Volume = ScreenManager.MusicVolume; //backgroundVolume;
            effectWall.Volume = ScreenManager.FXVolume;//wallVolume;
            effectPig.Volume = ScreenManager.FXVolume;

            if (MediaPlayer.GameHasControl && ScreenState != ScreenState.Frozen)
            {
                effectBackground.Play();
            }
            else
                effectBackground.Pause();

            #region paddle update

            Vector3 acceleration = new Vector3(Accelerometer.GetState().Acceleration.X,
            Accelerometer.GetState().Acceleration.Y, Accelerometer.GetState().Acceleration.Z);

            if (acceleration.Y > 0)
            {
                acceleration.Y = -paddleSpeed;
            }
            else if (acceleration.Y < 0)
            {
                acceleration.Y = paddleSpeed;
            }

            paddle.Velocity.X = acceleration.Y;
            paddleColision.Velocity.X = acceleration.Y;

            paddle.Position += paddle.Velocity;
            paddleColision.Position += paddleColision.Velocity;

            // left bar wall
            if (paddle.Position.X < 125)
            {
                paddle.Position.X = 125;
                paddle.Velocity.X = 0;
            }

            // right bar wall
            else if (paddle.Position.X > 665)
            {
                paddle.Position.X = 665;
                paddle.Velocity.X = 0;
            }

            // left bar wall
            if (paddleColision.Position.X < 125)
            {
                paddleColision.Position.X = 125;
                paddleColision.Velocity.X = 0;
            }

            // right bar wall
            else if (paddleColision.Position.X > 670)
            {
                paddleColision.Position.X = 670;
                paddleColision.Velocity.X = 0;
            }


            paddle.Update(gameTime);
            paddleColision.Update(gameTime);
            #endregion          

            #region Ball Update

            foreach (GestureSample gesture in ScreenManager.InputSystem.Gestures)
            {
                if (gesture.GestureType == GestureType.DoubleTap)
                {
                    Point doubleTap = new Point((int)gesture.Position.X, (int)gesture.Position.Y);
                    ShootBall();
                }
            }


            for (int i = 0; i < balls.Length; i++)
            {
                if (balls[i].Active)
                {
                    balls[i].Position += balls[i].Velocity * ballSpeed;
                    balls[i].Update(gameTime);
                }

                if (OffBottom())
                {
                    if (--hud.Life == 0)
                    {                        
                        Data.gameTitle[0] = H1N1GameName;
                        effectBackground.Stop();
                        ScreenManager.RemoveAd();
                        base.Remove();
                        ScreenManager.AddScreen(new CheckScoreScreen(Data, hud.Score, hud.Level, HighScore, 3));                      

                    }

                    else
                    {

                        foreach (GestureSample gesture in ScreenManager.InputSystem.Gestures)
                        {
                            if (gesture.GestureType == GestureType.DoubleTap)
                            {
                                Point doubleTap = new Point((int)gesture.Position.X, (int)gesture.Position.Y);
                                ShootBall();
                            }
                        }

                    }
                }

                CheckBarCollision();
                CheckPaddleCollision();
                CheckBlockCollision();
                extraLifeCollision();
                /*
                if (didRestart)
                {
                    ScreenManager.isPressed = true;
                    effectBackground.Stop();
                    ScreenState = ScreenState.Frozen;
                    ScreenManager.AddScreen(new PauseScreen(this));
                    didRestart = false;
                }*/
            }

            #endregion

            #region Piggy Update

            // Update pigs 
            foreach (var row1Pigs in pigRow1)
            {
                if (row1Pigs.Active)
                    row1PigCollision();
                row1Pigs.Update(gameTime);
            }

            foreach (var row2Pigs in pigRow2)
            {
                if (row2Pigs.Active)
                    row2PigCollision();
                row2Pigs.Update(gameTime);
            }

            foreach (var row3Pigs in pigRow3)
            {
                if (row3Pigs.Active)
                    row3PigCollision();
                row3Pigs.Update(gameTime);
            }

            foreach (var row4Pigs in pigRow4)
            {
                if (row4Pigs.Active)
                    row4PigCollision();
                row4Pigs.Update(gameTime);
            }

            foreach (var row5Pigs in pigRow5)
            {
                if (row5Pigs.Active)
                    row5PigCollision();
                row5Pigs.Update(gameTime);
            }

            foreach (var row6Pigs in pigRow6)
            {
                if (row6Pigs.Active)
                    row6PigCollision();
                row6Pigs.Update(gameTime);
            }

            foreach (var row7Pigs in pigRow7)
            {
                if (row7Pigs.Active)
                    row7PigCollision();
                row7Pigs.Update(gameTime);
            }
            #endregion

            #region Cell Background Update

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

            #endregion

            #region Highscore Update

            // if score = highscore save level data stop background music
            // remove the level and go to loading screen then next level
            if (hud.Score >= highScore)
            {
                ISsystem.SaveScore(hud.Score, hud.Life, SaveGameData);
                effectBackground.Stop();
                ScreenManager.RemoveAd();
                base.Remove();
                LoadingScreen.Load(ScreenManager, true, new H1N1Level_03());
            }


            #endregion

            #region Block, extraLife Update

            block.Update(gameTime); 

            foreach (var rowBlock in rowblocks)
            {
                if (rowBlock.Active)
                    blockRowCollision();
                rowBlock.Update(gameTime);
            }

            extraLife.Update(gameTime);

            #endregion

            #region Back Button
            // Handles the pausing of the game if it was tombstoned.
            if (didRestart)
            {
                ScreenManager.isPressed = true;
                effectBackground.Stop();
                ScreenState = ScreenState.Frozen;
                ScreenManager.AddScreen(new PauseScreen(this));
                didRestart = false;
            }

            // Following code handles the back button logic.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && !ScreenManager.isPressed)
            {
                ScreenManager.isPressed = true;
                effectBackground.Stop();
                ScreenState = ScreenState.Frozen;
                ScreenManager.AddScreen(new PauseScreen(this));
            }
            else if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && ScreenManager.isPressed)
            {
                ScreenManager.isPressed = false;
                ScreenState = ScreenState.Active;
            }
            #endregion
            base.Update(gameTime, covered);
        }

        #endregion

        #region Draw

        /// <summary>
        /// Draws all of the games Ojects
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            background.Draw(gameTime, spriteBatch);

            #region Draw the background blood cells right and left

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

            #endregion

            bars.Draw(gameTime, spriteBatch);

            paddleColision.Draw(gameTime, spriteBatch);
            paddle.Draw(gameTime, spriteBatch);

            for (int i = 0; i < balls.Length; i++)
            {
                if (balls[i].Active)
                    balls[i].Draw(gameTime, spriteBatch);
            }

            #region Draw the rows of pigs

            foreach (var row1Pig in pigRow1)
            {
                if (row1Pig.Active)
                    row1Pig.Draw(gameTime, spriteBatch);
            }

            foreach (var row2Pig in pigRow2)
            {
                if (row2Pig.Active)
                    row2Pig.Draw(gameTime, spriteBatch);
            }

            foreach (var row3Pig in pigRow3)
            {
                if (row3Pig.Active)
                    row3Pig.Draw(gameTime, spriteBatch);
            }

            foreach (var row4Pig in pigRow4)
            {
                if (row4Pig.Active)
                    row4Pig.Draw(gameTime, spriteBatch);
            }

            foreach (var row5Pig in pigRow5)
            {
                if (row5Pig.Active)
                    row5Pig.Draw(gameTime, spriteBatch);
            }

            foreach (var row6Pig in pigRow6)
            {
                if (row6Pig.Active)
                    row6Pig.Draw(gameTime, spriteBatch);
            }

            foreach (var row7Pig in pigRow7)
            {
                if (row7Pig.Active)
                    row7Pig.Draw(gameTime, spriteBatch);
            }


            #endregion

            #region Draw hud and life bar

            hud.Draw(spriteBatch, gameTime);

            //Draw the box around the health bar
            spriteBatch.Draw(SpriteSheet, new Rectangle(77, 28, 223, 24), new Rectangle(725, 115, 223, 24), Color.White);

            //Draw the health for the health bar
            if (hud.Life >= 3)
            {
                //Draw full healthbar
                spriteBatch.Draw(HealthBar, new Rectangle(80, 32, 218, 17), new Rectangle(0, 0, HealthBar.Width, 0), Color.Red);
            }
            else if (hud.Life == 2)
            {
                //Draw -59 healthbar
                spriteBatch.Draw(HealthBar, new Rectangle(80, 32, 147, 17), new Rectangle(0, 0, HealthBar.Width, 0), Color.Red);
            }
            else if (hud.Life == 1)
            {
                //Draw -162 healthbar
                spriteBatch.Draw(HealthBar, new Rectangle(80, 32, 72, 17), new Rectangle(0, 0, HealthBar.Width, 0), Color.Red);
            }
            else if (hud.Life == 0)
            {
                //Draw -242 healthbar
                spriteBatch.Draw(HealthBar, new Rectangle(80, 32, 0, 17), new Rectangle(0, 0, HealthBar.Width, 0), Color.Red);

            }


            #endregion

            #region Draw Blocks, extraLife

            block.Draw(gameTime, spriteBatch);

            foreach (var rowBlock in rowblocks)
            {
                if (rowBlock.Active)
                    rowBlock.Draw(gameTime, spriteBatch);
            }

            extraLife.Draw(gameTime, spriteBatch);

            #endregion

            spriteBatch.End();
        }

        #endregion

        #region Ball Colision, OffBottom, Shoot functions

        public bool OffBottom()
        {
            for (int i = 0; i < balls.Length; i++)
            {
                if (balls[i].Position.Y > ScreenManager.Viewport.Height + 10)
                {
                    balls[i].Active = false;
                    balls[i].Position = new Vector2(paddleColision.Position.X, paddleColision.Position.Y - 15);
                    return true;
                }
            }
            return false;
        }

        private void ShootBall()
        {
            int j = 0;
            bool active = false;

            for (int i = 0; i < balls.Length; i++)
            {
                if (!balls[i].Active)
                {
                    active = true;
                    balls[i].Active = true;
                    j = i;
                }
            }

            if (active)
            {
                balls[j].Position = new Vector2(paddleColision.Position.X, paddleColision.Position.Y - 15);
                balls[j].Velocity = new Vector2(-3, -3);
                balls[j].AnimationSpeed = TimeSpan.FromMilliseconds(400);
                balls[j].Origin = new Vector2((balls[j].ActualWidth / 2) / balls[j].Scale, (balls[j].ActualHeight / 2) / balls[j].Scale);
                balls[j].Scale = 0.5f;
            }
        }

        private void CheckPaddleCollision()
        {
            for (int i = 0; i < balls.Length; i++)
            {
                if (balls[i].Active)
                {
                    if (balls[i].BoundingRect.Intersects(paddleColision.BoundingRect))
                    {
                        balls[i].Position.Y = paddleColision.Position.Y - balls[i].ActualHeight;
                        balls[i].Velocity.Y *= -1;

                        effectWall.Play();
                        effectWall.Volume = wallVolume;
                    }
                }
            }
        }

        private void CheckBlockCollision()
        {
            // Variable to hold block rectangle 4 corners
            float[] BlockCornerMarkers;

            foreach (var ball in balls)
            {
                if (ball.BoundingRect.Intersects(block.BoundingRect))
                {
                    effectWall.Play();

                    Vector2[] blockCorners = new Vector2[4];
                    blockCorners[0] = new Vector2(block.Position.X - block.Origin.X * block.Scale, block.Position.Y - block.Origin.Y * block.Scale);
                    blockCorners[1] = new Vector2(block.Position.X + block.Origin.X * block.Scale, block.Position.Y - block.Origin.Y * block.Scale);
                    blockCorners[2] = new Vector2(block.Position.X + block.Origin.X * block.Scale, block.Position.Y + block.Origin.Y * block.Scale);
                    blockCorners[3] = new Vector2(block.Position.X - block.Origin.X * block.Scale, block.Position.Y + block.Origin.Y * block.Scale);

                    BlockCornerMarkers = new float[4];

                    for (int i = 1; i < BlockCornerMarkers.Length; i++)
                    {
                        Vector2 direction = Vector2.Normalize(blockCorners[i] - block.Position);
                        BlockCornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                    }

                    Vector2 direction2 = Vector2.Normalize(ball.Position - block.Position);

                    float angle = (float)Math.Atan2(direction2.Y, direction2.X) + MathHelper.Pi;

                    if (angle >= BlockCornerMarkers[0] && angle < BlockCornerMarkers[1])
                    {
                        ball.Velocity.Y = -Math.Abs(ball.Velocity.Y);
                    }

                    else if (angle >= BlockCornerMarkers[1] && angle < BlockCornerMarkers[2])
                    {
                        ball.Velocity.X = Math.Abs(ball.Velocity.X);
                    }

                    else if (angle >= BlockCornerMarkers[2] && angle < BlockCornerMarkers[3])
                    {
                        ball.Velocity.Y = Math.Abs(ball.Velocity.Y);
                    }

                    else 
                    {
                        ball.Velocity.X = -Math.Abs(ball.Velocity.X);
                    }
                }
            }
        }

        #region block row Collision

        private void blockRowCollision()
        {
            foreach (var rowBlock in rowblocks)
            {
                foreach (var ball in balls)
                {
                    if (ball.BoundingRect.Intersects(rowBlock.BoundingRect))
                    {
                        effectWall.Play();                      

                        Vector2[] corners = new Vector2[4];
                        corners[0] = new Vector2(rowBlock.Position.X - rowBlock.Origin.X * rowBlock.Scale, rowBlock.Position.Y - rowBlock.Origin.Y * rowBlock.Scale);
                        corners[1] = new Vector2(rowBlock.Position.X + rowBlock.Origin.X * rowBlock.Scale, rowBlock.Position.Y - rowBlock.Origin.Y * rowBlock.Scale);
                        corners[2] = new Vector2(rowBlock.Position.X + rowBlock.Origin.X * rowBlock.Scale, rowBlock.Position.Y + rowBlock.Origin.Y * rowBlock.Scale);
                        corners[3] = new Vector2(rowBlock.Position.X - rowBlock.Origin.X * rowBlock.Scale, rowBlock.Position.Y + rowBlock.Origin.Y * rowBlock.Scale);

                        this.CornerMarkers = new float[4];

                        for (int i = 1; i < CornerMarkers.Length; i++)
                        {
                            Vector2 direction = Vector2.Normalize(corners[i] - rowBlock.Position);
                            CornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                        }

                        Vector2 direction2 = Vector2.Normalize(ball.Position - rowBlock.Position);

                        float angle = (float)Math.Atan2(direction2.Y, direction2.X) + MathHelper.Pi;

                        if (angle >= CornerMarkers[0] && angle < CornerMarkers[1])
                        {
                            ball.Velocity.Y = -Math.Abs(ball.Velocity.Y);
                        }

                        else if (angle >= CornerMarkers[1] && angle < CornerMarkers[2])
                        {
                            ball.Velocity.X = Math.Abs(ball.Velocity.X);
                        }

                        else if (angle >= CornerMarkers[2] && angle < CornerMarkers[3])
                        {
                            ball.Velocity.Y = Math.Abs(ball.Velocity.Y);
                        }

                        else 
                        {
                            ball.Velocity.X = -Math.Abs(ball.Velocity.X);
                        }
                    }
                }
            }
        }

        #endregion

        private void CheckBarCollision()
        {
            for (int i = 0; i < balls.Length; i++)
            {
                if (balls[i].Active)
                {
                    // check left bar
                    if (balls[i].Position.X < this.radius + 25)
                    {
                        balls[i].Position.X = this.radius + 25;
                        balls[i].Velocity.X *= -1;
                        effectWall.Play();
                        effectWall.Volume = wallVolume;
                    }

                    // check right bar
                    else if (balls[i].Position.X > 760)
                    {
                        balls[i].Position.X = 760;
                        balls[i].Velocity.X *= -1;
                        effectWall.Play();
                    }

                    // check top bar
                    if (balls[i].Position.Y < this.radius + 80)
                    {
                        balls[i].Position.Y = this.radius + 80;
                        balls[i].Velocity.Y *= -1;
                        effectWall.Play();
                        effectWall.Volume = wallVolume;
                    }

                    // check bottom
                    else if (balls[i].Position.Y > ScreenManager.Viewport.Height - this.radius)
                    {
                        OffBottom();
                    }
                }
            }
        }

        #endregion

        #region row1Pig Collision

        private void row1PigCollision()
        {
            foreach (var row1Pig in pigRow1)
            {
                if (row1Pig == null)
                    continue;

                foreach (var ball in balls)
                {
                    if (ball.BoundingRect.Intersects(row1Pig.BoundingRect))
                    {
                        hud.Score = hud.Score + 10;

                        effectPig.Play();
                        effectPig.Volume = pigVolume;

                        Vector2[] corners = new Vector2[4];
                        corners[0] = new Vector2(row1Pig.Position.X - row1Pig.Origin.X * row1Pig.Scale, row1Pig.Position.Y - row1Pig.Origin.Y * row1Pig.Scale);
                        corners[1] = new Vector2(row1Pig.Position.X + row1Pig.Origin.X * row1Pig.Scale, row1Pig.Position.Y - row1Pig.Origin.Y * row1Pig.Scale);
                        corners[2] = new Vector2(row1Pig.Position.X + row1Pig.Origin.X * row1Pig.Scale, row1Pig.Position.Y + row1Pig.Origin.Y * row1Pig.Scale);
                        corners[3] = new Vector2(row1Pig.Position.X - row1Pig.Origin.X * row1Pig.Scale, row1Pig.Position.Y + row1Pig.Origin.Y * row1Pig.Scale);

                        this.CornerMarkers = new float[4];

                        for (int i = 1; i < CornerMarkers.Length; i++)
                        {
                            Vector2 direction = Vector2.Normalize(corners[i] - row1Pig.Position);
                            CornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                        }

                        Vector2 direction2 = Vector2.Normalize(ball.Position - row1Pig.Position);

                        float angle = (float)Math.Atan2(direction2.Y, direction2.X) + MathHelper.Pi;

                        if (angle >= CornerMarkers[0] && angle < CornerMarkers[1])
                        {
                            ball.Velocity.Y = -Math.Abs(ball.Velocity.Y);
                        }

                        else if (angle >= CornerMarkers[1] && angle < CornerMarkers[2])
                        {
                            ball.Velocity.X = Math.Abs(ball.Velocity.X);
                        }

                        else if (angle >= CornerMarkers[2] && angle < CornerMarkers[3])
                        {
                            ball.Velocity.Y = Math.Abs(ball.Velocity.Y);
                        }

                        else 
                        {
                            ball.Velocity.X = -Math.Abs(ball.Velocity.X);
                        }

                        row1Pig.Active = false;
                        row1Pig.Position = new Vector2(1300, 800);
                    }
                }
            }
        }

        #endregion

        #region row2Pig Collision

        private void row2PigCollision()
        {
            foreach (var row2Pig in pigRow2)
            {
                if (row2Pig == null)
                    continue;

                foreach (var ball in balls)
                {
                    if (ball.BoundingRect.Intersects(row2Pig.BoundingRect))
                    {
                        hud.Score = hud.Score + 10;

                        effectPig.Play();
                        effectPig.Volume = pigVolume;

                        Vector2[] corners = new Vector2[4];
                        corners[0] = new Vector2(row2Pig.Position.X - row2Pig.Origin.X * row2Pig.Scale, row2Pig.Position.Y - row2Pig.Origin.Y * row2Pig.Scale);
                        corners[1] = new Vector2(row2Pig.Position.X + row2Pig.Origin.X * row2Pig.Scale, row2Pig.Position.Y - row2Pig.Origin.Y * row2Pig.Scale);
                        corners[2] = new Vector2(row2Pig.Position.X + row2Pig.Origin.X * row2Pig.Scale, row2Pig.Position.Y + row2Pig.Origin.Y * row2Pig.Scale);
                        corners[3] = new Vector2(row2Pig.Position.X - row2Pig.Origin.X * row2Pig.Scale, row2Pig.Position.Y + row2Pig.Origin.Y * row2Pig.Scale);

                        this.CornerMarkers = new float[4];

                        for (int i = 1; i < CornerMarkers.Length; i++)
                        {
                            Vector2 direction = Vector2.Normalize(corners[i] - row2Pig.Position);
                            CornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                        }

                        Vector2 direction2 = Vector2.Normalize(ball.Position - row2Pig.Position);

                        float angle = (float)Math.Atan2(direction2.Y, direction2.X) + MathHelper.Pi;

                        if (angle >= CornerMarkers[0] && angle < CornerMarkers[1])
                        {
                            ball.Velocity.Y = -Math.Abs(ball.Velocity.Y);
                        }

                        else if (angle >= CornerMarkers[1] && angle < CornerMarkers[2])
                        {
                            ball.Velocity.X = Math.Abs(ball.Velocity.X);
                        }

                        else if (angle >= CornerMarkers[2] && angle < CornerMarkers[3])
                        {
                            ball.Velocity.Y = Math.Abs(ball.Velocity.Y);
                        }

                        else 
                        {
                            ball.Velocity.X = -Math.Abs(ball.Velocity.X);
                        }

                        row2Pig.Active = false;
                        row2Pig.Position = new Vector2(1300, 800);
                    }
                }
            }
        }

        #endregion

        #region row3Pig Collision

        private void row3PigCollision()
        {
            foreach (var row3Pig in pigRow3)
            {
                if (row3Pig == null)
                    continue;

                foreach (var ball in balls)
                {
                    if (ball.BoundingRect.Intersects(row3Pig.BoundingRect))
                    {
                        hud.Score = hud.Score + 10;

                        effectPig.Play();
                        effectPig.Volume = pigVolume;

                        Vector2[] corners = new Vector2[4];
                        corners[0] = new Vector2(row3Pig.Position.X - row3Pig.Origin.X * row3Pig.Scale, row3Pig.Position.Y - row3Pig.Origin.Y * row3Pig.Scale);
                        corners[1] = new Vector2(row3Pig.Position.X + row3Pig.Origin.X * row3Pig.Scale, row3Pig.Position.Y - row3Pig.Origin.Y * row3Pig.Scale);
                        corners[2] = new Vector2(row3Pig.Position.X + row3Pig.Origin.X * row3Pig.Scale, row3Pig.Position.Y + row3Pig.Origin.Y * row3Pig.Scale);
                        corners[3] = new Vector2(row3Pig.Position.X - row3Pig.Origin.X * row3Pig.Scale, row3Pig.Position.Y + row3Pig.Origin.Y * row3Pig.Scale);

                        this.CornerMarkers = new float[4];

                        for (int i = 1; i < CornerMarkers.Length; i++)
                        {
                            Vector2 direction = Vector2.Normalize(corners[i] - row3Pig.Position);
                            CornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                        }

                        Vector2 direction2 = Vector2.Normalize(ball.Position - row3Pig.Position);

                        float angle = (float)Math.Atan2(direction2.Y, direction2.X) + MathHelper.Pi;

                        if (angle >= CornerMarkers[0] && angle < CornerMarkers[1])
                        {
                            ball.Velocity.Y = -Math.Abs(ball.Velocity.Y);
                        }

                        else if (angle >= CornerMarkers[1] && angle < CornerMarkers[2])
                        {
                            ball.Velocity.X = Math.Abs(ball.Velocity.X);
                        }

                        else if (angle >= CornerMarkers[2] && angle < CornerMarkers[3])
                        {
                            ball.Velocity.Y = Math.Abs(ball.Velocity.Y);
                        }

                        else 
                        {
                            ball.Velocity.X = -Math.Abs(ball.Velocity.X);
                        }

                        row3Pig.Active = false;
                        row3Pig.Position = new Vector2(1300, 800);
                    }
                }
            }
        }

        #endregion

        #region row4Pig Collision

        private void row4PigCollision()
        {
            foreach (var row4Pig in pigRow4)
            {
                if (row4Pig == null)
                    continue;

                foreach (var ball in balls)
                {
                    if (ball.BoundingRect.Intersects(row4Pig.BoundingRect))
                    {
                        hud.Score = hud.Score + 10;

                        effectPig.Play();
                        effectPig.Volume = pigVolume;

                        Vector2[] corners = new Vector2[4];
                        corners[0] = new Vector2(row4Pig.Position.X - row4Pig.Origin.X * row4Pig.Scale, row4Pig.Position.Y - row4Pig.Origin.Y * row4Pig.Scale);
                        corners[1] = new Vector2(row4Pig.Position.X + row4Pig.Origin.X * row4Pig.Scale, row4Pig.Position.Y - row4Pig.Origin.Y * row4Pig.Scale);
                        corners[2] = new Vector2(row4Pig.Position.X + row4Pig.Origin.X * row4Pig.Scale, row4Pig.Position.Y + row4Pig.Origin.Y * row4Pig.Scale);
                        corners[3] = new Vector2(row4Pig.Position.X - row4Pig.Origin.X * row4Pig.Scale, row4Pig.Position.Y + row4Pig.Origin.Y * row4Pig.Scale);

                        this.CornerMarkers = new float[4];

                        for (int i = 1; i < CornerMarkers.Length; i++)
                        {
                            Vector2 direction = Vector2.Normalize(corners[i] - row4Pig.Position);
                            CornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                        }

                        Vector2 direction2 = Vector2.Normalize(ball.Position - row4Pig.Position);

                        float angle = (float)Math.Atan2(direction2.Y, direction2.X) + MathHelper.Pi;

                        if (angle >= CornerMarkers[0] && angle < CornerMarkers[1])
                        {
                            ball.Velocity.Y = -Math.Abs(ball.Velocity.Y);
                        }

                        else if (angle >= CornerMarkers[1] && angle < CornerMarkers[2])
                        {
                            ball.Velocity.X = Math.Abs(ball.Velocity.X);
                        }

                        else if (angle >= CornerMarkers[2] && angle < CornerMarkers[3])
                        {
                            ball.Velocity.Y = Math.Abs(ball.Velocity.Y);
                        }

                        else 
                        {
                            ball.Velocity.X = -Math.Abs(ball.Velocity.X);
                        }

                        row4Pig.Active = false;
                        row4Pig.Position = new Vector2(1300, 800);
                    }
                }
            }
        }

        #endregion

        #region row5Pig Collision

        private void row5PigCollision()
        {
            foreach (var row5Pig in pigRow5)
            {
                if (row5Pig == null)
                    continue;

                foreach (var ball in balls)
                {
                    if (ball.BoundingRect.Intersects(row5Pig.BoundingRect))
                    {
                        hud.Score = hud.Score + 10;

                        effectPig.Play();
                        effectPig.Volume = pigVolume;

                        Vector2[] corners = new Vector2[4];
                        corners[0] = new Vector2(row5Pig.Position.X - row5Pig.Origin.X * row5Pig.Scale, row5Pig.Position.Y - row5Pig.Origin.Y * row5Pig.Scale);
                        corners[1] = new Vector2(row5Pig.Position.X + row5Pig.Origin.X * row5Pig.Scale, row5Pig.Position.Y - row5Pig.Origin.Y * row5Pig.Scale);
                        corners[2] = new Vector2(row5Pig.Position.X + row5Pig.Origin.X * row5Pig.Scale, row5Pig.Position.Y + row5Pig.Origin.Y * row5Pig.Scale);
                        corners[3] = new Vector2(row5Pig.Position.X - row5Pig.Origin.X * row5Pig.Scale, row5Pig.Position.Y + row5Pig.Origin.Y * row5Pig.Scale);

                        this.CornerMarkers = new float[4];

                        for (int i = 1; i < CornerMarkers.Length; i++)
                        {
                            Vector2 direction = Vector2.Normalize(corners[i] - row5Pig.Position);
                            CornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                        }

                        Vector2 direction2 = Vector2.Normalize(ball.Position - row5Pig.Position);

                        float angle = (float)Math.Atan2(direction2.Y, direction2.X) + MathHelper.Pi;

                        if (angle >= CornerMarkers[0] && angle < CornerMarkers[1])
                        {
                            ball.Velocity.Y = -Math.Abs(ball.Velocity.Y);
                        }

                        else if (angle >= CornerMarkers[1] && angle < CornerMarkers[2])
                        {
                            ball.Velocity.X = Math.Abs(ball.Velocity.X);
                        }

                        else if (angle >= CornerMarkers[2] && angle < CornerMarkers[3])
                        {
                            ball.Velocity.Y = Math.Abs(ball.Velocity.Y);
                        }

                        else 
                        {
                            ball.Velocity.X = -Math.Abs(ball.Velocity.X);
                        }

                        row5Pig.Active = false;
                        row5Pig.Position = new Vector2(1300, 800);
                    }
                }
            }
        }

        #endregion

        #region row6Pig Collision

        private void row6PigCollision()
        {
            foreach (var row6Pig in pigRow6)
            {
                if (row6Pig == null)
                    continue;

                foreach (var ball in balls)
                {
                    if (ball.BoundingRect.Intersects(row6Pig.BoundingRect))
                    {
                        hud.Score = hud.Score + 10;

                        effectPig.Play();
                        effectPig.Volume = pigVolume;

                        Vector2[] corners = new Vector2[4];
                        corners[0] = new Vector2(row6Pig.Position.X - row6Pig.Origin.X * row6Pig.Scale, row6Pig.Position.Y - row6Pig.Origin.Y * row6Pig.Scale);
                        corners[1] = new Vector2(row6Pig.Position.X + row6Pig.Origin.X * row6Pig.Scale, row6Pig.Position.Y - row6Pig.Origin.Y * row6Pig.Scale);
                        corners[2] = new Vector2(row6Pig.Position.X + row6Pig.Origin.X * row6Pig.Scale, row6Pig.Position.Y + row6Pig.Origin.Y * row6Pig.Scale);
                        corners[3] = new Vector2(row6Pig.Position.X - row6Pig.Origin.X * row6Pig.Scale, row6Pig.Position.Y + row6Pig.Origin.Y * row6Pig.Scale);

                        this.CornerMarkers = new float[4];

                        for (int i = 1; i < CornerMarkers.Length; i++)
                        {
                            Vector2 direction = Vector2.Normalize(corners[i] - row6Pig.Position);
                            CornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                        }

                        Vector2 direction2 = Vector2.Normalize(ball.Position - row6Pig.Position);

                        float angle = (float)Math.Atan2(direction2.Y, direction2.X) + MathHelper.Pi;

                        if (angle >= CornerMarkers[0] && angle < CornerMarkers[1])
                        {
                            ball.Velocity.Y = -Math.Abs(ball.Velocity.Y);
                        }

                        else if (angle >= CornerMarkers[1] && angle < CornerMarkers[2])
                        {
                            ball.Velocity.X = Math.Abs(ball.Velocity.X);
                        }

                        else if (angle >= CornerMarkers[2] && angle < CornerMarkers[3])
                        {
                            ball.Velocity.Y = Math.Abs(ball.Velocity.Y);
                        }

                        else 
                        {
                            ball.Velocity.X = -Math.Abs(ball.Velocity.X);
                        }

                        row6Pig.Active = false;
                        row6Pig.Position = new Vector2(1300, 800);
                    }
                }
            }
        }

        #endregion

        #region row7Pig Collision

        private void row7PigCollision()
        {
            foreach (var row7Pig in pigRow7)
            {
                if (row7Pig == null)
                    continue;

                foreach (var ball in balls)
                {
                    if (ball.BoundingRect.Intersects(row7Pig.BoundingRect))
                    {
                        hud.Score = hud.Score + 10;

                        effectPig.Play();
                        effectPig.Volume = pigVolume;

                        Vector2[] corners = new Vector2[4];
                        corners[0] = new Vector2(row7Pig.Position.X - row7Pig.Origin.X * row7Pig.Scale, row7Pig.Position.Y - row7Pig.Origin.Y * row7Pig.Scale);
                        corners[1] = new Vector2(row7Pig.Position.X + row7Pig.Origin.X * row7Pig.Scale, row7Pig.Position.Y - row7Pig.Origin.Y * row7Pig.Scale);
                        corners[2] = new Vector2(row7Pig.Position.X + row7Pig.Origin.X * row7Pig.Scale, row7Pig.Position.Y + row7Pig.Origin.Y * row7Pig.Scale);
                        corners[3] = new Vector2(row7Pig.Position.X - row7Pig.Origin.X * row7Pig.Scale, row7Pig.Position.Y + row7Pig.Origin.Y * row7Pig.Scale);

                        this.CornerMarkers = new float[4];

                        for (int i = 1; i < CornerMarkers.Length; i++)
                        {
                            Vector2 direction = Vector2.Normalize(corners[i] - row7Pig.Position);
                            CornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                        }

                        Vector2 direction2 = Vector2.Normalize(ball.Position - row7Pig.Position);

                        float angle = (float)Math.Atan2(direction2.Y, direction2.X) + MathHelper.Pi;

                        if (angle >= CornerMarkers[0] && angle < CornerMarkers[1])
                        {
                            ball.Velocity.Y = -Math.Abs(ball.Velocity.Y);
                        }

                        else if (angle >= CornerMarkers[1] && angle < CornerMarkers[2])
                        {
                            ball.Velocity.X = Math.Abs(ball.Velocity.X);
                        }

                        else if (angle >= CornerMarkers[2] && angle < CornerMarkers[3])
                        {
                            ball.Velocity.Y = Math.Abs(ball.Velocity.Y);
                        }

                        else 
                        {
                            ball.Velocity.X = -Math.Abs(ball.Velocity.X);
                        }

                        row7Pig.Active = false;
                        row7Pig.Position = new Vector2(1300, 800);
                    }
                }
            }
        }

        #endregion

        #region ExtraLife Collision

        private void extraLifeCollision()
        {
            foreach (var ball in balls)
            {
                if (ball.BoundingRect.Intersects(extraLife.BoundingRect))
                {
                    extraLife.Active = false;
                    extraLife.Position = new Vector2(1300, 750);

                    if (hud.Life < 3)
                    {
                        hud.Life = hud.Life + 1;
                    }                     
                }
            }
        }

        #endregion

        /*
         *  // Game Objects
        GameObject bars, background, paddle, paddleColision, block, extraLife;

        GameObject[] rowblocks = new GameObject[2];

        GameObject[] balls = new GameObject[1];

        GameObject[] pigRow1 = new GameObject[2];
        GameObject[] pigRow2 = new GameObject[3];
        GameObject[] pigRow3 = new GameObject[5];
        GameObject[] pigRow4 = new GameObject[3];
        GameObject[] pigRow5 = new GameObject[5];
        GameObject[] pigRow6 = new GameObject[6];
        GameObject[] pigRow7 = new GameObject[6];

        GameObject[] cellsLeft = new GameObject[5];
        GameObject[] cellsRight = new GameObject[5];*/

        protected void SaveState(object sender, DeactivatedEventArgs args)
        {
            PhoneApplicationService.Current.State["hud.Score"] = hud.Score;
            PhoneApplicationService.Current.State["hud.Life"] = hud.Life;
            PhoneApplicationService.Current.State["hud.Level"] = hud.Level;


            PhoneApplicationService.Current.State["bars"] = bars;
            PhoneApplicationService.Current.State["background"] = background;
            PhoneApplicationService.Current.State["paddle"] = paddle;
            PhoneApplicationService.Current.State["paddleColision"] = paddleColision;
            PhoneApplicationService.Current.State["block"] = block;
            PhoneApplicationService.Current.State["extraLife"] = extraLife;

            PhoneApplicationService.Current.State["rowblocks"] = rowblocks;

            PhoneApplicationService.Current.State["balls"] = balls;

            PhoneApplicationService.Current.State["pigRow1"] = pigRow1;
            PhoneApplicationService.Current.State["pigRow2"] = pigRow2;
            PhoneApplicationService.Current.State["pigRow3"] = pigRow3;
            PhoneApplicationService.Current.State["pigRow4"] = pigRow4;
            PhoneApplicationService.Current.State["pigRow5"] = pigRow5;
            PhoneApplicationService.Current.State["pigRow6"] = pigRow6;
            PhoneApplicationService.Current.State["pigRow7"] = pigRow7;

            PhoneApplicationService.Current.State["cellsLeft"] = cellsLeft;
            PhoneApplicationService.Current.State["cellsRight"] = cellsRight;

            PhoneApplicationService.Current.State["ActiveGame"] = GameType.H1N1;

        }

        public void LoadLevel()
        {
            hud.Score = (int)PhoneApplicationService.Current.State["hud.Score"];
            hud.Life = (int)PhoneApplicationService.Current.State["hud.Life"];
            hud.Level = (int)PhoneApplicationService.Current.State["hud.Level"];

            GameObject barsLoad = (GameObject)PhoneApplicationService.Current.State["bars"];
            bars.Position = barsLoad.Position;
            GameObject backgroundLoad = (GameObject)PhoneApplicationService.Current.State["background"];
            background.Position = backgroundLoad.Position;
            GameObject paddleLoad = (GameObject)PhoneApplicationService.Current.State["paddle"];
            paddle.Position = paddleLoad.Position;
            GameObject paddleColisionLoad = (GameObject)PhoneApplicationService.Current.State["paddleColision"];
            paddleColision.Position = paddleColisionLoad.Position;
            GameObject block1Load = (GameObject)PhoneApplicationService.Current.State["block"];
            block.Position = block1Load.Position;
            GameObject extraLifeLoad = (GameObject)PhoneApplicationService.Current.State["extraLife"];
            extraLife.Position = extraLifeLoad.Position;
            extraLife.AnimationSpeed = TimeSpan.FromMilliseconds(600);

            GameObject[] rowblocksLoad = (GameObject[])PhoneApplicationService.Current.State["rowblocks"];
            for (int i = 0; i < balls.Length; i++)
            {
                rowblocks[i].Position = rowblocksLoad[i].Position;
            }

            GameObject[] ballsLoad = (GameObject[])PhoneApplicationService.Current.State["balls"];
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Position = ballsLoad[i].Position;
                balls[i].Velocity = ballsLoad[i].Velocity;
                balls[i].Active = ballsLoad[i].Active;
                balls[i].Scale = ballsLoad[i].Scale;
                balls[i].AnimationSpeed = TimeSpan.FromMilliseconds(400);
            }

            GameObject[] row1pigs = (GameObject[])PhoneApplicationService.Current.State["pigRow1"];
            for (int i = 0; i < pigRow1.Length; i++)
            {
                pigRow1[i].Position = row1pigs[i].Position;
            }
            GameObject[] row2pigs = (GameObject[])PhoneApplicationService.Current.State["pigRow2"];
            for (int i = 0; i < pigRow2.Length; i++)
            {
                pigRow2[i].Position = row2pigs[i].Position;
            }
            GameObject[] row3pigs = (GameObject[])PhoneApplicationService.Current.State["pigRow3"];
            for (int i = 0; i < pigRow3.Length; i++)
            {
                pigRow3[i].Position = row3pigs[i].Position;
            }
            GameObject[] row4pigs = (GameObject[])PhoneApplicationService.Current.State["pigRow4"];
            for (int i = 0; i < pigRow4.Length; i++)
            {
                pigRow4[i].Position = row4pigs[i].Position;
            }
            GameObject[] row5pigs = (GameObject[])PhoneApplicationService.Current.State["pigRow5"];
            for (int i = 0; i < pigRow5.Length; i++)
            {
                pigRow5[i].Position = row5pigs[i].Position;
            }
            GameObject[] row6pigs = (GameObject[])PhoneApplicationService.Current.State["pigRow6"];
            for (int i = 0; i < pigRow6.Length; i++)
            {
                pigRow6[i].Position = row6pigs[i].Position;
            }
            GameObject[] row7pigs = (GameObject[])PhoneApplicationService.Current.State["pigRow7"];
            for (int i = 0; i < pigRow7.Length; i++)
            {
                pigRow7[i].Position = row7pigs[i].Position;
            }

            GameObject[] cellsLeftLoad = (GameObject[])PhoneApplicationService.Current.State["cellsLeft"];
            for (int i = 0; i < cellsLeft.Length; i++)
            {
                cellsLeft[i].Position = cellsLeftLoad[i].Position;
                cellsLeft[i].Velocity = cellsLeftLoad[i].Velocity;
            }
            GameObject[] cellsRightLoad = (GameObject[])PhoneApplicationService.Current.State["cellsRight"];
            for (int i = 0; i < cellsRight.Length; i++)
            {
                cellsRight[i].Position = cellsRightLoad[i].Position;
                cellsRight[i].Velocity = cellsRightLoad[i].Velocity;
            }
        }
    }

    #endregion
}

#endregion