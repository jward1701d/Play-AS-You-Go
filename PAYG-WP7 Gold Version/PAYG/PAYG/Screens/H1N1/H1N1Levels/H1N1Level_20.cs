#region Info
/***********************************************************
 * Level_20.cs                                             *
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
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input.Touch;

using Microsoft.Phone.Shell;
using System.Collections;
using System.Runtime.Serialization; //needed for data contracts
using System.Xml;

#endregion

#region Name Space

namespace PAYG
{
    #region Class

    class H1N1Level_20 : GameScreen
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
        GameObject bars, background, paddle, paddleColision, block1, block2, block3, block4, block5, block6;

        GameObject[] balls = new GameObject[1];

        GameObject[] pigRow1 = new GameObject[8];
        GameObject[] pigRow2 = new GameObject[8];
        GameObject[] pigRow3 = new GameObject[8];
        GameObject[] pigRow4 = new GameObject[8];
        GameObject[] pigRow5 = new GameObject[4];
        GameObject[] pigRow6 = new GameObject[4];

        GameObject[] cellsLeft = new GameObject[5];
        GameObject[] cellsRight = new GameObject[5];

        //  A Random variable
        Random rnd = new Random();

        // Variables to hold paddle and ball speed, ball radius, each rows space and highscore
        float paddleSpeed;

        float ballSpeed;
        float radius = 8;

        int row1Space = 0;
        int row2Space = 0;
        int row3Space = 0;
        int row4Space = 0;
        int row5Space = 0;
        int row6Space = 0;

        int highScore = 8770;

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
        public H1N1Level_20()
        {
            TransitionOnTime = TimeSpan.FromSeconds(2);
            TransitionOffTime = TimeSpan.FromSeconds(1);

        }

        public H1N1Level_20(bool active)
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

            // Initialize top row of pigs and give it a positon, origin, scale and active to true
            for (int a = 0; a < pigRow1.Length; a++)
            {
                GameObject row1Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow1[a] = row1Pig;
                row1Pig.Position = new Vector2(250 + row1Space,175);
                row1Pig.Origin = new Vector2((row1Pig.ActualWidth / 2) / row1Pig.Scale, (row1Pig.ActualHeight / 2) / row1Pig.Scale);
                row1Pig.Velocity = new Vector2(0, -2);
                row1Pig.Scale = 0.11f;
                row1Pig.Active = true;
                row1Space += 45;
            }

            // Initialize middle row of pigs and give it a positon, origin, scale and active to true
            for (int b = 0; b < pigRow2.Length; b++)
            {
                GameObject row2Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow2[b] = row2Pig;
                row2Pig.Position = new Vector2(250 + row2Space, 220);
                row2Pig.Origin = new Vector2((row2Pig.ActualWidth / 2) / row2Pig.Scale, (row2Pig.ActualHeight / 2) / row2Pig.Scale);
                row2Pig.Velocity = new Vector2(0, -2);
                row2Pig.Scale = 0.11f;
                row2Pig.Active = true;
                row2Space += 45;
            }

            // Initialize bottom row of pigs and give it a positon, origin, scale and active to true
            for (int c = 0; c < pigRow3.Length; c++)
            {
                GameObject row3Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow3[c] = row3Pig;
                row3Pig.Position = new Vector2(250 + row3Space, 265);
                row3Pig.Origin = new Vector2((row3Pig.ActualWidth / 2) / row3Pig.Scale, (row3Pig.ActualHeight / 2) / row3Pig.Scale);
                row3Pig.Velocity = new Vector2(0, -2);
                row3Pig.Scale = 0.11f;
                row3Pig.Active = true;
                row3Space += 45;
            }

            // Initialize bottom row of pigs and give it a positon, origin, scale and active to true
            for (int d = 0; d < pigRow4.Length; d++)
            {
                GameObject row4Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow4[d] = row4Pig;
                row4Pig.Position = new Vector2(250 + row4Space,310);
                row4Pig.Origin = new Vector2((row4Pig.ActualWidth / 2) / row4Pig.Scale, (row4Pig.ActualHeight / 2) / row4Pig.Scale);
                row4Pig.Velocity = new Vector2(0, -2);
                row4Pig.Scale = 0.11f;
                row4Pig.Active = true;
                row4Space += 45;
            }

            // Initialize bottom row of pigs and give it a positon, origin, scale and active to true
            for (int d = 0; d < pigRow5.Length; d++)
            {
                GameObject row5Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow5[d] = row5Pig;
                row5Pig.Position = new Vector2(70 , 175 + row5Space);
                row5Pig.Origin = new Vector2((row5Pig.ActualWidth / 2) / row5Pig.Scale, (row5Pig.ActualHeight / 2) / row5Pig.Scale);
                row5Pig.Velocity = new Vector2(0, -2);
                row5Pig.Scale = 0.11f;
                row5Pig.Active = true;
                row5Space += 45;
            }

            // Initialize bottom row of pigs and give it a positon, origin, scale and active to true
            for (int d = 0; d < pigRow6.Length; d++)
            {
                GameObject row6Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow6[d] = row6Pig;
                row6Pig.Position = new Vector2(735 , 175 + row6Space);
                row6Pig.Origin = new Vector2((row6Pig.ActualWidth / 2) / row6Pig.Scale, (row6Pig.ActualHeight / 2) / row6Pig.Scale);
                row6Pig.Velocity = new Vector2(0, -2);
                row6Pig.Scale = 0.11f;
                row6Pig.Active = true;
                row6Space += 45;
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
                cellLeft.Scale = 0.5f;
            }

            // Initialize cell facing right background flow
            for (int i = 0; i < cellsRight.Length; i++)
            {
                GameObject cellRight = new GameObject(CellsRight, new Rectangle(0, 0, 256, 256), 1, true, 0);
                cellsRight[i] = cellRight;
                cellRight.Position = new Vector2(rnd.Next(ScreenManager.Viewport.Width), rnd.Next(ScreenManager.Viewport.Height));
                cellRight.Speed = (float)rnd.NextDouble() * 20 + 2;
                cellRight.Scale = 0.5f;
            }

            #endregion

            #region Initialize block

            // Initializes players paddel and give it a positon , origin, scale and speed
            block1 = new GameObject(SpriteSheet, new Rectangle(14, 26, 160, 32), 1, true, 0);
            block1.Position = new Vector2(85, 150);
            block1.Origin = new Vector2((block1.ActualWidth / 2) / block1.Scale, (block1.ActualHeight / 2) / block1.Scale);
            block1.Velocity = new Vector2(-2, 0);
            block1.Scale = .75f;

            block2 = new GameObject(SpriteSheet, new Rectangle(14, 26, 160, 32), 1, true, 0);
            block2.Position = new Vector2(500, 150);
            block2.Origin = new Vector2((block2.ActualWidth / 2) / block2.Scale, (block2.ActualHeight / 2) / block2.Scale);
            block2.Scale = .75f;

            block3 = new GameObject(SpriteSheet, new Rectangle(14, 26, 160, 32), 1, true, 0);
            block3.Position = new Vector2(155, 255);
            block3.Origin = new Vector2((block3.ActualWidth / 2) / block3.Scale, (block2.ActualHeight / 2) / block2.Scale);
            block3.Scale = .75f;

            block4 = new GameObject(SpriteSheet, new Rectangle(14, 26, 160, 32), 1, true, 0);
            block4.Position = new Vector2(650, 255);
            block4.Origin = new Vector2((block4.ActualWidth / 2) / block4.Scale, (block4.ActualHeight / 2) / block4.Scale);
            block4.Scale = .75f;

            block5 = new GameObject(SpriteSheet, new Rectangle(14, 26, 160, 32), 1, true, 0);
            block5.Position = new Vector2(465, 365);
            block5.Origin = new Vector2((block5.ActualWidth / 2) / block5.Scale, (block5.ActualHeight / 2) / block5.Scale);
            block5.Velocity = new Vector2(2, 0);
            block5.Scale = .75f;

            block6 = new GameObject(SpriteSheet, new Rectangle(14, 26, 160, 32), 1, true, 0);
            block6.Position = new Vector2(70, 365);
            block6.Origin = new Vector2((block6.ActualWidth / 2) / block6.Scale, (block6.ActualHeight / 2) / block6.Scale);
            block6.Velocity = new Vector2(2, 0);
            block6.Scale = .75f;

            #endregion

            hud.Score = H1N1LevelData.score[0];
            hud.Life = H1N1LevelData.lives[0];
            hud.Level = 20;

            backgroundVolume = ScreenManager.MusicVolume;
            wallVolume = ScreenManager.FXVolume;
            pigVolume = ScreenManager.FXVolume;
            // Initalizes all sounds in game
            effectBackground = backgroundEffect.CreateInstance();
            effectWall = wallEffect.CreateInstance();
            effectWall.Volume = wallVolume;
            effectPig = pigEffect.CreateInstance();

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
            #region Pause

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
                CheckBlock1Collision();
                CheckBlock2Collision();
                CheckBlock3Collision();
                CheckBlock4Collision();
                CheckBlock5Collision();
                CheckBlock6Collision();
         }

            #endregion

            #region Piggy Update

            // Update pigs 
            foreach (var row1Pigs in pigRow1)
            {
                if (row1Pigs.Active)
                    row1PigCollision();
                row1Pigs.Update(gameTime);

                if (row1Pigs.Position.Y <= 175)
                {
                    row1Pigs.Velocity = new Vector2(0, 2);
                }
                else if (row1Pigs.Position.Y >= 310)
                {
                    row1Pigs.Velocity = new Vector2(0, -2);
                }
            }

            foreach (var row2Pigs in pigRow2)
            {
                if (row2Pigs.Active)
                    row2PigCollision();
                row2Pigs.Update(gameTime);

                if (row2Pigs.Position.Y <= 175)
                {
                    row2Pigs.Velocity = new Vector2(0, 2);
                }
                else if (row2Pigs.Position.Y >= 310)
                {
                    row2Pigs.Velocity = new Vector2(0, -2);
                }
            }

            foreach (var row3Pigs in pigRow3)
            {
                if (row3Pigs.Active)
                    row3PigCollision();
                row3Pigs.Update(gameTime);

                if (row3Pigs.Position.Y <= 175)
                {
                    row3Pigs.Velocity = new Vector2(0, 2);
                }
                else if (row3Pigs.Position.Y >= 310)
                {
                    row3Pigs.Velocity = new Vector2(0, -2);
                }

            }

            foreach (var row4Pigs in pigRow4)
            {
                if (row4Pigs.Active)
                    row4PigCollision();
                row4Pigs.Update(gameTime);

                if (row4Pigs.Position.Y <= 175)
                {
                    row4Pigs.Velocity = new Vector2(0, 2);
                }
                else if (row4Pigs.Position.Y >= 310)
                {
                    row4Pigs.Velocity = new Vector2(0, -2);
                }
            }


            foreach (var row5Pigs in pigRow5)
            {
                if (row5Pigs.Active)
                    row5PigCollision();
                row5Pigs.Update(gameTime);

                if (row5Pigs.Position.Y <= 175)
                {
                    row5Pigs.Velocity = new Vector2(0, 2);
                }
                else if (row5Pigs.Position.Y >= 310)
                {
                    row5Pigs.Velocity = new Vector2(0, -2);
                }
            }

            foreach (var row6Pigs in pigRow6)
            {
                if (row6Pigs.Active)
                    row6PigCollision();
                row6Pigs.Update(gameTime);

                if (row6Pigs.Position.Y <= 175)
                {
                    row6Pigs.Velocity = new Vector2(0, 2);
                }
                else if (row6Pigs.Position.Y >= 310)
                {
                    row6Pigs.Velocity = new Vector2(0, -2);
                }
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
                Data.gameTitle[0] = H1N1GameName;                
                effectBackground.Stop();
                ScreenManager.RemoveAd();
                base.Remove();
                ScreenManager.AddScreen(new CheckScoreScreen(Data, hud.Score, hud.Level, HighScore, 3));
            }

            #endregion

            #region Block Update

            block1.Update(gameTime);
            if (block1.Position.X <= 85)
            {
                block1.Velocity = new Vector2(2, 0);
            }
            else if (block1.Position.X >= 300)
            {
                block1.Velocity = new Vector2(-2, 0);
            }         
          
            block2.Update(gameTime);
            if (block2.Position.X >= 710)
            {
                block2.Velocity = new Vector2(-2, 0);
            }
            else if (block2.Position.X <= 500)
            {
                block2.Velocity = new Vector2(2, 0);
            }

            block3.Update(gameTime);
            block4.Update(gameTime);

            block5.Update(gameTime);
            if (block5.Position.X <= 465)
            {
                block5.Velocity = new Vector2(2, 0);
            }
            else if (block5.Position.X >= 710)
            {
                block5.Velocity = new Vector2(-2, 0);
            }

            block6.Update(gameTime);
            if (block6.Position.X <= 70)
            {
                block6.Velocity = new Vector2(2, 0);
            }
            
            else if (block6.Position.X >= 300)
            {
                block6.Velocity = new Vector2(-2, 0);
            }

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

            #region Draw Blocks

            block1.Draw(gameTime, spriteBatch);
            block2.Draw(gameTime, spriteBatch);
            block3.Draw(gameTime, spriteBatch);
            block4.Draw(gameTime, spriteBatch);
            block5.Draw(gameTime, spriteBatch);
            block6.Draw(gameTime, spriteBatch);


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

        private void CheckBlock1Collision()
        {
            // Variable to hold block rectangle 4 corners
            float[] Block1CornerMarkers;

            foreach (var ball in balls)
            {
                if (ball.BoundingRect.Intersects(block1.BoundingRect))
                {
                    effectWall.Play();

                    Vector2[] block1Corners = new Vector2[4];
                    block1Corners[0] = new Vector2(block1.Position.X - block1.Origin.X * block1.Scale, block1.Position.Y - block1.Origin.Y * block1.Scale);
                    block1Corners[1] = new Vector2(block1.Position.X + block1.Origin.X * block1.Scale, block1.Position.Y - block1.Origin.Y * block1.Scale);
                    block1Corners[2] = new Vector2(block1.Position.X + block1.Origin.X * block1.Scale, block1.Position.Y + block1.Origin.Y * block1.Scale);
                    block1Corners[3] = new Vector2(block1.Position.X - block1.Origin.X * block1.Scale, block1.Position.Y + block1.Origin.Y * block1.Scale);

                    Block1CornerMarkers = new float[4];

                    for (int i = 1; i < Block1CornerMarkers.Length; i++)
                    {
                        Vector2 direction = Vector2.Normalize(block1Corners[i] - block1.Position);
                        Block1CornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                    }

                    Vector2 direction2 = Vector2.Normalize(ball.Position - block1.Position);

                    float angle = (float)Math.Atan2(direction2.Y, direction2.X) + MathHelper.Pi;

                    if (angle >= Block1CornerMarkers[0] && angle < Block1CornerMarkers[1])
                    {
                        ball.Velocity.Y = -Math.Abs(ball.Velocity.Y);
                    }

                    else if (angle >= Block1CornerMarkers[1] && angle < Block1CornerMarkers[2])
                    {
                        ball.Velocity.X = Math.Abs(ball.Velocity.X);
                    }

                    else if (angle >= Block1CornerMarkers[2] && angle < Block1CornerMarkers[3])
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

        private void CheckBlock2Collision()
        {
            // Variable to hold block rectangle 4 corners
            float[] Block2CornerMarkers;

            foreach (var ball in balls)
            {
                if (ball.BoundingRect.Intersects(block2.BoundingRect))
                {
                    effectWall.Play();

                    Vector2[] block2Corners = new Vector2[4];
                    block2Corners[0] = new Vector2(block2.Position.X - block2.Origin.X * block2.Scale, block2.Position.Y - block2.Origin.Y * block2.Scale);
                    block2Corners[1] = new Vector2(block2.Position.X + block2.Origin.X * block2.Scale, block2.Position.Y - block2.Origin.Y * block2.Scale);
                    block2Corners[2] = new Vector2(block2.Position.X + block2.Origin.X * block2.Scale, block2.Position.Y + block2.Origin.Y * block2.Scale);
                    block2Corners[3] = new Vector2(block2.Position.X - block2.Origin.X * block2.Scale, block2.Position.Y + block2.Origin.Y * block2.Scale);

                    Block2CornerMarkers = new float[4];

                    for (int i = 1; i < Block2CornerMarkers.Length; i++)
                    {
                        Vector2 direction = Vector2.Normalize(block2Corners[i] - block2.Position);
                        Block2CornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                    }

                    Vector2 direction2 = Vector2.Normalize(ball.Position - block2.Position);

                    float angle = (float)Math.Atan2(direction2.Y, direction2.X) + MathHelper.Pi;

                    if (angle >= Block2CornerMarkers[0] && angle < Block2CornerMarkers[1])
                    {
                        ball.Velocity.Y = -Math.Abs(ball.Velocity.Y);
                    }

                    else if (angle >= Block2CornerMarkers[1] && angle < Block2CornerMarkers[2])
                    {
                        ball.Velocity.X = Math.Abs(ball.Velocity.X);
                    }

                    else if (angle >= Block2CornerMarkers[2] && angle < Block2CornerMarkers[3])
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


        private void CheckBlock3Collision()
        {
            // Variable to hold block rectangle 4 corners
            float[] Block3CornerMarkers;

            foreach (var ball in balls)
            {
                if (ball.BoundingRect.Intersects(block3.BoundingRect))
                {
                    effectWall.Play();

                    Vector2[] block3Corners = new Vector2[4];
                    block3Corners[0] = new Vector2(block3.Position.X - block3.Origin.X * block3.Scale, block3.Position.Y - block3.Origin.Y * block3.Scale);
                    block3Corners[1] = new Vector2(block3.Position.X + block3.Origin.X * block3.Scale, block3.Position.Y - block3.Origin.Y * block3.Scale);
                    block3Corners[2] = new Vector2(block3.Position.X + block3.Origin.X * block3.Scale, block3.Position.Y + block3.Origin.Y * block3.Scale);
                    block3Corners[3] = new Vector2(block3.Position.X - block3.Origin.X * block3.Scale, block3.Position.Y + block3.Origin.Y * block3.Scale);

                    Block3CornerMarkers = new float[4];

                    for (int i = 1; i < Block3CornerMarkers.Length; i++)
                    {
                        Vector2 direction = Vector2.Normalize(block3Corners[i] - block3.Position);
                        Block3CornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                    }

                    Vector2 direction2 = Vector2.Normalize(ball.Position - block3.Position);

                    float angle = (float)Math.Atan2(direction2.Y, direction2.X) + MathHelper.Pi;

                    if (angle >= Block3CornerMarkers[0] && angle < Block3CornerMarkers[1])
                    {
                        ball.Velocity.Y = -Math.Abs(ball.Velocity.Y);
                    }

                    else if (angle >= Block3CornerMarkers[1] && angle < Block3CornerMarkers[2])
                    {
                        ball.Velocity.X = Math.Abs(ball.Velocity.X);
                    }

                    else if (angle >= Block3CornerMarkers[2] && angle < Block3CornerMarkers[3])
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


        private void CheckBlock4Collision()
        {
            // Variable to hold block rectangle 4 corners
            float[] Block4CornerMarkers;

            foreach (var ball in balls)
            {
                if (ball.BoundingRect.Intersects(block4.BoundingRect))
                {
                    effectWall.Play();

                    Vector2[] block4Corners = new Vector2[4];
                    block4Corners[0] = new Vector2(block4.Position.X - block4.Origin.X * block4.Scale, block4.Position.Y - block4.Origin.Y * block4.Scale);
                    block4Corners[1] = new Vector2(block4.Position.X + block4.Origin.X * block4.Scale, block4.Position.Y - block4.Origin.Y * block4.Scale);
                    block4Corners[2] = new Vector2(block4.Position.X + block4.Origin.X * block4.Scale, block4.Position.Y + block4.Origin.Y * block4.Scale);
                    block4Corners[3] = new Vector2(block4.Position.X - block4.Origin.X * block4.Scale, block4.Position.Y + block4.Origin.Y * block4.Scale);

                    Block4CornerMarkers = new float[4];

                    for (int i = 1; i < Block4CornerMarkers.Length; i++)
                    {
                        Vector2 direction = Vector2.Normalize(block4Corners[i] - block4.Position);
                        Block4CornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                    }

                    Vector2 direction2 = Vector2.Normalize(ball.Position - block4.Position);

                    float angle = (float)Math.Atan2(direction2.Y, direction2.X) + MathHelper.Pi;

                    if (angle >= Block4CornerMarkers[0] && angle < Block4CornerMarkers[1])
                    {
                        ball.Velocity.Y = -Math.Abs(ball.Velocity.Y);
                    }

                    else if (angle >= Block4CornerMarkers[1] && angle < Block4CornerMarkers[2])
                    {
                        ball.Velocity.X = Math.Abs(ball.Velocity.X);
                    }

                    else if (angle >= Block4CornerMarkers[2] && angle < Block4CornerMarkers[3])
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

        private void CheckBlock5Collision()
        {
            // Variable to hold block rectangle 4 corners
            float[] Block5CornerMarkers;

            foreach (var ball in balls)
            {
                if (ball.BoundingRect.Intersects(block5.BoundingRect))
                {
                    effectWall.Play();

                    Vector2[] block5Corners = new Vector2[4];
                    block5Corners[0] = new Vector2(block5.Position.X - block5.Origin.X * block5.Scale, block5.Position.Y - block5.Origin.Y * block5.Scale);
                    block5Corners[1] = new Vector2(block5.Position.X + block5.Origin.X * block5.Scale, block5.Position.Y - block5.Origin.Y * block5.Scale);
                    block5Corners[2] = new Vector2(block5.Position.X + block5.Origin.X * block5.Scale, block5.Position.Y + block5.Origin.Y * block5.Scale);
                    block5Corners[3] = new Vector2(block5.Position.X - block5.Origin.X * block5.Scale, block5.Position.Y + block5.Origin.Y * block5.Scale);

                    Block5CornerMarkers = new float[4];

                    for (int i = 1; i < Block5CornerMarkers.Length; i++)
                    {
                        Vector2 direction = Vector2.Normalize(block5Corners[i] - block5.Position);
                        Block5CornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                    }

                    Vector2 direction2 = Vector2.Normalize(ball.Position - block5.Position);

                    float angle = (float)Math.Atan2(direction2.Y, direction2.X) + MathHelper.Pi;

                    if (angle >= Block5CornerMarkers[0] && angle < Block5CornerMarkers[1])
                    {
                        ball.Velocity.Y = -Math.Abs(ball.Velocity.Y);
                    }

                    else if (angle >= Block5CornerMarkers[1] && angle < Block5CornerMarkers[2])
                    {
                        ball.Velocity.X = Math.Abs(ball.Velocity.X);
                    }

                    else if (angle >= Block5CornerMarkers[2] && angle < Block5CornerMarkers[3])
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

        private void CheckBlock6Collision()
        {
            // Variable to hold block rectangle 4 corners
            float[] Block6CornerMarkers;

            foreach (var ball in balls)
            {
                if (ball.BoundingRect.Intersects(block6.BoundingRect))
                {
                    effectWall.Play();

                    Vector2[] block6Corners = new Vector2[4];
                    block6Corners[0] = new Vector2(block6.Position.X - block6.Origin.X * block6.Scale, block6.Position.Y - block6.Origin.Y * block6.Scale);
                    block6Corners[1] = new Vector2(block6.Position.X + block6.Origin.X * block6.Scale, block6.Position.Y - block6.Origin.Y * block6.Scale);
                    block6Corners[2] = new Vector2(block6.Position.X + block6.Origin.X * block6.Scale, block6.Position.Y + block6.Origin.Y * block6.Scale);
                    block6Corners[3] = new Vector2(block6.Position.X - block6.Origin.X * block6.Scale, block6.Position.Y + block6.Origin.Y * block6.Scale);

                    Block6CornerMarkers = new float[4];

                    for (int i = 1; i < Block6CornerMarkers.Length; i++)
                    {
                        Vector2 direction = Vector2.Normalize(block6Corners[i] - block6.Position);
                        Block6CornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                    }

                    Vector2 direction2 = Vector2.Normalize(ball.Position - block6.Position);

                    float angle = (float)Math.Atan2(direction2.Y, direction2.X) + MathHelper.Pi;

                    if (angle >= Block6CornerMarkers[0] && angle < Block6CornerMarkers[1])
                    {
                        ball.Velocity.Y = -Math.Abs(ball.Velocity.Y);
                    }

                    else if (angle >= Block6CornerMarkers[1] && angle < Block6CornerMarkers[2])
                    {
                        ball.Velocity.X = Math.Abs(ball.Velocity.X);
                    }

                    else if (angle >= Block6CornerMarkers[2] && angle < Block6CornerMarkers[3])
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

        /*
              // Game Objects
        GameObject bars, background, paddle, paddleColision, block1, block2, block3, block4, block5, block6;

        GameObject[] balls = new GameObject[1];

        GameObject[] pigRow1 = new GameObject[8];
        GameObject[] pigRow2 = new GameObject[8];
        GameObject[] pigRow3 = new GameObject[8];
        GameObject[] pigRow4 = new GameObject[8];
        GameObject[] pigRow5 = new GameObject[4];
        GameObject[] pigRow6 = new GameObject[4];

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


            PhoneApplicationService.Current.State["block1"] = block1;
            PhoneApplicationService.Current.State["block2"] = block2;
            PhoneApplicationService.Current.State["block3"] = block3;
            PhoneApplicationService.Current.State["block4"] = block4;
            PhoneApplicationService.Current.State["block5"] = block5;
            PhoneApplicationService.Current.State["block6"] = block6;

            PhoneApplicationService.Current.State["balls"] = balls;

            PhoneApplicationService.Current.State["pigRow1"] = pigRow1;
            PhoneApplicationService.Current.State["pigRow2"] = pigRow2;
            PhoneApplicationService.Current.State["pigRow3"] = pigRow3;
            PhoneApplicationService.Current.State["pigRow4"] = pigRow4;
            PhoneApplicationService.Current.State["pigRow5"] = pigRow5;
            PhoneApplicationService.Current.State["pigRow6"] = pigRow6;



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

            GameObject blocks1Load = (GameObject)PhoneApplicationService.Current.State["block1"];
            block1.Position = blocks1Load.Position;
            block1.Velocity = blocks1Load.Velocity;

            GameObject blocks2Load = (GameObject)PhoneApplicationService.Current.State["block2"];
            block2.Position = blocks2Load.Position;
            block2.Velocity = blocks2Load.Velocity;

            GameObject blocks3Load = (GameObject)PhoneApplicationService.Current.State["block3"];
            block3.Position = blocks3Load.Position;
            block3.Velocity = blocks3Load.Velocity;

            GameObject blocks4Load = (GameObject)PhoneApplicationService.Current.State["block4"];
            block4.Position = blocks4Load.Position;
            block4.Velocity = blocks4Load.Velocity;

            GameObject blocks5Load = (GameObject)PhoneApplicationService.Current.State["block5"];
            block5.Position = blocks5Load.Position;
            block5.Velocity = blocks5Load.Velocity;

            GameObject blocks6Load = (GameObject)PhoneApplicationService.Current.State["block6"];
            block6.Position = blocks6Load.Position;
            block6.Velocity = blocks6Load.Velocity;

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
                pigRow1[i].Velocity = row1pigs[i].Velocity;
                pigRow1[i].Scale = row1pigs[i].Scale;
            }
            GameObject[] row2pigs = (GameObject[])PhoneApplicationService.Current.State["pigRow2"];
            for (int i = 0; i < pigRow2.Length; i++)
            {
                pigRow2[i].Position = row2pigs[i].Position;
                pigRow2[i].Velocity = row2pigs[i].Velocity;
                pigRow2[i].Scale = row2pigs[i].Scale;
            }
            GameObject[] row3pigs = (GameObject[])PhoneApplicationService.Current.State["pigRow3"];
            for (int i = 0; i < pigRow3.Length; i++)
            {
                pigRow3[i].Position = row3pigs[i].Position;
                pigRow3[i].Velocity = row3pigs[i].Velocity;
                pigRow3[i].Scale = row3pigs[i].Scale;
            }
            GameObject[] row4pigs = (GameObject[])PhoneApplicationService.Current.State["pigRow4"];
            for (int i = 0; i < pigRow4.Length; i++)
            {
                pigRow4[i].Position = row4pigs[i].Position;
                pigRow4[i].Velocity = row4pigs[i].Velocity;
                pigRow4[i].Scale = row4pigs[i].Scale;
            }
            GameObject[] row5pigs = (GameObject[])PhoneApplicationService.Current.State["pigRow5"];
            for (int i = 0; i < pigRow5.Length; i++)
            {
                pigRow5[i].Position = row5pigs[i].Position;
                pigRow5[i].Velocity = row5pigs[i].Velocity;
                pigRow5[i].Scale = row5pigs[i].Scale;
            }
            GameObject[] row6pigs = (GameObject[])PhoneApplicationService.Current.State["pigRow6"];
            for (int i = 0; i < pigRow6.Length; i++)
            {
                pigRow6[i].Position = row6pigs[i].Position;
                pigRow6[i].Velocity = row6pigs[i].Velocity;
                pigRow6[i].Scale = row6pigs[i].Scale;
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