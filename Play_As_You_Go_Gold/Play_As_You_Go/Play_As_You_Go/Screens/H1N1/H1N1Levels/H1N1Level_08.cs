#region Info
/***********************************************************
 * Level_06.cs                                             *
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

#endregion

#region Name Space

namespace Play_As_You_Go
{
    #region Class

    class H1N1Level_08 : GameScreen
    {
        #region Fields

        // Textures to use
        Texture2D Background;
        Texture2D SpriteSheet;
        Texture2D Bars;
        Texture2D HealthBar;
        Texture2D CellsLeft;
        Texture2D CellsRight;

        // Hud Object
        Hud hud;

        // FPS object
        FramesPerSecond framesPerSecond;

        // Game Objects
        GameObject bars, background, paddle, paddleColision;

        GameObject[] balls = new GameObject[1];

        GameObject[] blocks1 = new GameObject[2];
        GameObject[] blocks3 = new GameObject[2];

        GameObject[] pigRow1 = new GameObject[3];
        GameObject[] pigRow2 = new GameObject[3];
        GameObject[] pigRow3 = new GameObject[10];
        GameObject[] pigRow4 = new GameObject[12];
        GameObject[] pigRow5 = new GameObject[2];
        GameObject[] pigRow6 = new GameObject[2];
        GameObject[] pigRow7 = new GameObject[4];
        GameObject[] pigRow8 = new GameObject[4];
        GameObject[] pigRow9 = new GameObject[2];

        GameObject[] cellsLeft = new GameObject[5];
        GameObject[] cellsRight = new GameObject[5];

        GameObject[] extraLifes = new GameObject[1];

        // Random variable
        Random rnd = new Random();

        // Variables to hold paddle and ball speed,radius, each rows space and highscore
        float paddleSpeed;

        float ballSpeed;
        float radius = 8;

        int row1space = 0;
        int row2Space = 0;
        int row3Space = 0;
        int row5Space = 0;
        int row4Space = 0;
        int row6Space = 0;
        int row7Space = 0;
        int row8Space = 0;
        int row9Space = 0;

        int block1RowSpace = 0;
        int block3RowSpace = 0;

        int highScore = 3310;

        // Variable to hold pigs rectangle 4 corners
        public float[] CornerMarkers;

        // Background Music , volume
        SoundEffect backgroundEffect;
        SoundEffectInstance effectBackground;
        float backgroundVolume = 0.10f;

        // Sounds effects, volume
        SoundEffect pigEffect;
        SoundEffectInstance effectPig;
        SoundEffect wallEffect;
        SoundEffectInstance effectWall;
        float pigVolume = 0.20f;
        float wallVolume = 0.10f;

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
        public H1N1Level_08()
        {
            TransitionOnTime = TimeSpan.FromSeconds(2);
            TransitionOffTime = TimeSpan.FromSeconds(1);
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes all of our Game Objects
        /// </summary>
        public override void Initialize()
        {
            // Initializes a backround object and giving it a position
            background = new GameObject(Background, new Rectangle(0, 0, 1280, 720), 1, true, 0);
            background.Position = new Vector2(0, 0);

            // Initializes bars for the pill to seem to bounce off of
            bars = new GameObject(Bars, new Rectangle(0, 0, 1280, 720), 1, true, 0);
            bars.Position = new Vector2(0, 0); ;

            #region Initialize Paddle

            // Initializes players paddel and give it a positon , origin, scale and speed
            paddle = new GameObject(SpriteSheet, new Rectangle(13, 286, 640, 115), 1, true, 0);
            paddle.Position = new Vector2(675, 650);
            paddle.Origin = new Vector2((paddle.ActualWidth / 2) / paddle.Scale, (paddle.ActualHeight / 2) / paddle.Scale);
            paddle.Scale = 0.35f;
            paddleSpeed = 18;

            paddleColision = new GameObject(SpriteSheet, new Rectangle(15, 417, 640, 3), 1, true, 0);
            paddleColision.Position = new Vector2(675, 650);
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
                row1Pig.Position = new Vector2(159 + row1space, 162);
                row1Pig.Origin = new Vector2((row1Pig.ActualWidth / 2) / row1Pig.Scale, (row1Pig.ActualHeight / 2) / row1Pig.Scale);
                row1Pig.Scale = 0.125f;
                row1Pig.Active = true;
                row1space += 64;
            }

            // Initialize row 2 of pigs and give it a positon, origin, scale and active to true
            for (int b = 0; b < pigRow2.Length; b++)
            {
                GameObject row2Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow2[b] = row2Pig;
                row2Pig.Position = new Vector2(991 + row2Space, 162);
                row2Pig.Origin = new Vector2((row2Pig.ActualWidth / 2) / row2Pig.Scale, (row2Pig.ActualHeight / 2) / row2Pig.Scale);
                row2Pig.Scale = 0.125f;
                row2Pig.Active = true;
                row2Space += 64;
            }

            // Initialize row 3 of pigs and give it a positon, origin, scale and active to true
            for (int c = 0; c < pigRow3.Length; c++)
            {
                GameObject row3Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow3[c] = row3Pig;
                row3Pig.Position = new Vector2(351 + row3Space, 226);
                row3Pig.Origin = new Vector2((row3Pig.ActualWidth / 2) / row3Pig.Scale, (row3Pig.ActualHeight / 2) / row3Pig.Scale);
                row3Pig.Scale = 0.125f;
                row3Pig.Active = true;
                row3Space += 64;
            }

            // Initialize row 4 of pigs and give it a positon, origin, scale and active to true
            for (int d = 0; d < pigRow4.Length; d++)
            {
                GameObject row4Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow4[d] = row4Pig;
                row4Pig.Position = new Vector2(286 + row4Space, 290);
                row4Pig.Origin = new Vector2((row4Pig.ActualWidth / 2) / row4Pig.Scale, (row4Pig.ActualHeight / 2) / row4Pig.Scale);
                row4Pig.Scale = 0.125f;
                row4Pig.Active = true;
                row4Space += 64;
            }

            // Initialize row 5 of pigs and give it a positon, origin, scale and active to true
            for (int e = 0; e < pigRow5.Length; e++)
            {
                GameObject row5Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow5[e] = row5Pig;
                row5Pig.Position = new Vector2(127, 386 + row5Space);
                row5Pig.Origin = new Vector2((row5Pig.ActualWidth / 2) / row5Pig.Scale, (row5Pig.ActualHeight / 2) / row5Pig.Scale);
                row5Pig.Scale = 0.125f;
                row5Pig.Active = true;
                row5Space += 64;
            }

            // Initialize row 6 of pigs and give it a positon, origin, scale and active to true
            for (int f = 0; f < pigRow6.Length; f++)
            {
                GameObject row6Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow6[f] = row6Pig;
                row6Pig.Position = new Vector2(1156 , 386 + row6Space);
                row6Pig.Origin = new Vector2((row6Pig.ActualWidth / 2) / row6Pig.Scale, (row6Pig.ActualHeight / 2) / row6Pig.Scale);
                row6Pig.Scale = 0.125f;
                row6Pig.Active = true;
                row6Space += 64;
            }

            // Initialize row 7 of pigs and give it a positon, origin, scale and active to true
            for (int f = 0; f < pigRow7.Length; f++)
            {
                GameObject row7Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow7[f] = row7Pig;
                row7Pig.Position = new Vector2(287  + row7Space, 386);
                row7Pig.Origin = new Vector2((row7Pig.ActualWidth / 2) / row7Pig.Scale, (row7Pig.ActualHeight / 2) / row7Pig.Scale);
                row7Pig.Scale = 0.125f;
                row7Pig.Active = true;
                row7Space += 64;
            }

            // Initialize row 8 of pigs and give it a positon, origin, scale and active to true
            for (int f = 0; f < pigRow8.Length; f++)
            {
                GameObject row8Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow8[f] = row8Pig;
                row8Pig.Position = new Vector2(799  + row8Space, 386);
                row8Pig.Origin = new Vector2((row8Pig.ActualWidth / 2) / row8Pig.Scale, (row8Pig.ActualHeight / 2) / row8Pig.Scale);
                row8Pig.Scale = 0.125f;
                row8Pig.Active = true;
                row8Space += 64;
            }

            // Initialize row 9 of pigs and give it a positon, origin, scale and active to true
            for (int f = 0; f < pigRow9.Length; f++)
            {
                GameObject row9Pig = new GameObject(SpriteSheet, new Rectangle(760, 188, 256, 256), 1, true, 0);
                pigRow9[f] = row9Pig;
                row9Pig.Position = new Vector2(383 + row9Space, 450);
                row9Pig.Origin = new Vector2((row9Pig.ActualWidth / 2) / row9Pig.Scale, (row9Pig.ActualHeight / 2) / row9Pig.Scale);
                row9Pig.Scale = 0.125f;
                row9Pig.Active = true;
                row9Space += 512;
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

            for (int x = 0; x < blocks1.Length; x++)
            {
                GameObject block1 = new GameObject(SpriteSheet, new Rectangle(14, 26, 160, 32), 1, true, 0);
                blocks1[x] = block1;
                block1.Position = new Vector2(223 + block1RowSpace, 226);
                block1.Origin = new Vector2((block1.ActualWidth / 2) / block1.Scale, (block1.ActualHeight / 2) / block1.Scale);
                block1.Scale = 1.0f;
                block1RowSpace += 832;
            }

            for (int x = 0; x < blocks3.Length; x++)
            {
                GameObject block3 = new GameObject(SpriteSheet, new Rectangle(14, 26, 160, 32), 1, true, 0);
                blocks3[x] = block3;
                block3.Position = new Vector2(543 + block3RowSpace, 450);
                block3.Origin = new Vector2((block3.ActualWidth / 2) / block3.Scale, (block3.ActualHeight / 2) / block3.Scale);
                block3.Scale = 1.0f;
                block3RowSpace += 192;
            }

            #endregion

            for (int i = 0; i < extraLifes.Length; i++)
            {
                GameObject extraLife = new GameObject(SpriteSheet, new Rectangle(192, 11, 256, 256), 2, true, 0);
                extraLifes[i] = extraLife;
                extraLife.AnimationSpeed = TimeSpan.FromMilliseconds(600);
                extraLife.Position = new Vector2(644, 370);
                extraLife.Origin = new Vector2((extraLife.ActualWidth / 2) / extraLife.Scale, (extraLife.ActualHeight / 2) / extraLife.Scale);
                extraLife.Scale = 0.15f;
            }

            // Initalizes all sounds in game
            effectBackground = backgroundEffect.CreateInstance();
            effectWall = wallEffect.CreateInstance();
            effectWall.Volume = wallVolume;
            effectPig = pigEffect.CreateInstance();

            hud.Score = H1N1LevelData.score[0];
            hud.HealthLife = H1N1LevelData.lives[0];
            hud.Level = 8;

            framesPerSecond.FPSColor = Color.Orange;
            framesPerSecond.FPSPosition = new Vector2(800, 47);
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

            framesPerSecond = new FramesPerSecond();
            framesPerSecond.Font = content.Load<SpriteFont>("Fonts/H1N1Fonts/H1N1ScoreFont");

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

            int playerIndex = (int)ControllingPlayer.Value;


            KeyboardState keyboardState = ScreenManager.InputSystem.currentKeyboardState[playerIndex];
            GamePadState gamepadState = ScreenManager.InputSystem.currentGamePadState[playerIndex];

            bool gamePadDisconnected = !gamepadState.IsConnected && ScreenManager.InputSystem.gamePadWasConnected[playerIndex];

            // Pause
            if (ScreenManager.InputSystem.PauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                effectBackground.Stop();
                ScreenState = ScreenState.Frozen;
                ScreenManager.AddScreen(new PauseScreen(this), ControllingPlayer);
            }
            else
            {
                effectBackground.Volume = backgroundVolume;
                if (MediaPlayer.GameHasControl)
                {
                    effectBackground.Play();
                }
                else
                    effectBackground.Pause();
                ScreenState = ScreenState.Active;
            }


            #endregion
            #region paddle update

            // Updates paddle position and stops it from going off screen
            Vector2 Lthumbstick = gamepadState.ThumbSticks.Left;
            Vector2 Rthumbstick = gamepadState.ThumbSticks.Right;

            Vector2 movement = Vector2.Zero;
            // Keyboard
            if (keyboardState.IsKeyDown(Keys.Left))
                movement.X--;
            if (keyboardState.IsKeyDown(Keys.Right))
                movement.X++;

            // Game Pad DPad left, right
            if (gamepadState.IsButtonDown(Buttons.DPadLeft))
                movement.X--;
            if (gamepadState.IsButtonDown(Buttons.DPadRight))
                movement.X++;

            // Thumbsticks left right 
            movement.X += Lthumbstick.X;
            movement.X += Rthumbstick.X;

            if (movement.Length() > 1)
                movement.Normalize();

            paddle.Position.X += movement.X * paddleSpeed;
            paddleColision.Position.X += movement.X * paddleSpeed;


            // left bar wall
            if (paddle.Position.X < paddle.ActualWidth / 2 + 77 && paddleColision.Position.X < paddleColision.ActualWidth / 2 + 77)
            {
                paddle.Position.X = paddle.ActualWidth / 2 + 77;
                paddleColision.Position.X = paddleColision.ActualWidth / 2 + 77;
            }
            // right bar wall
            else if (paddle.Position.X > ScreenManager.Viewport.Width - (paddle.ActualWidth / 2) - 80 && paddleColision.Position.X > ScreenManager.Viewport.Width - (paddleColision.ActualWidth / 2) - 80)
            {
                paddle.Position.X = ScreenManager.Viewport.Width - (paddle.ActualWidth / 2) - 80;
                paddleColision.Position.X = ScreenManager.Viewport.Width - (paddleColision.ActualWidth / 2) - 80;
            }

            paddle.Update(gameTime);
            paddleColision.Update(gameTime);

            #endregion          

            #region Ball Update

            if (ScreenManager.InputSystem.Shoot(ControllingPlayer))
            {
                ShootBall();
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
                    if (--hud.HealthLife == 0)
                    {
                        Data.gameTitle[0] = H1N1GameName;
                        effectBackground.Stop();
                        ScreenManager.AddScreen(new CheckScoreScreen(Data, hud.Score, hud.Level, HighScore, 3),ControllingPlayer);
                        base.Remove();

                    }

                    else
                    {
                        if (ScreenManager.InputSystem.Shoot(ControllingPlayer))
                        {
                            ShootBall();
                        }
                    }
                }

                CheckBarCollision();
                CheckPaddleCollision();
                CheckBlock1Collision();
                CheckBlock3Collision();
                extraLifeCollision();
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

            foreach (var row8Pigs in pigRow8)
            {
                if (row8Pigs.Active)
                    row8PigCollision();
                row8Pigs.Update(gameTime);
            }

            foreach (var row9Pigs in pigRow9)
            {
                if (row9Pigs.Active)
                    row9PigCollision();
                row9Pigs.Update(gameTime);
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
                ISsystem.SaveScore(hud.Score, hud.HealthLife, SaveGameData);
                effectBackground.Stop();
                base.Remove();
                LoadingScreen.Load(ScreenManager, true, ControllingPlayer, new H1N1Level_09());                
            }

            #endregion

            #region Block, extraLife Update

            foreach (var block1 in blocks1)
            {
                block1.Update(gameTime);
            }

            foreach (var block3 in blocks3)
            {
                block3.Update(gameTime);
            }

            foreach (var extraLife in extraLifes)
            {
                extraLife.Update(gameTime);
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

            foreach (var row8Pig in pigRow8)
            {
                if (row8Pig.Active)
                    row8Pig.Draw(gameTime, spriteBatch);
            }

            foreach (var row9Pig in pigRow9)
            {
                if (row9Pig.Active)
                    row9Pig.Draw(gameTime, spriteBatch);
            }

            #endregion

            #region Draw hud and life bar

            hud.Draw(spriteBatch, gameTime);

            //Draw the box around the health bar
            spriteBatch.Draw(SpriteSheet, new Rectangle(150, 51, 223, 24), new Rectangle(725, 115, 223, 24), Color.White);

            //Draw the health for the health bar
            if (hud.HealthLife >= 3)
            {
                //Draw full healthbar
                spriteBatch.Draw(HealthBar, new Rectangle(153, 55, 218, 17), new Rectangle(0, 0, HealthBar.Width, 0), Color.Red);
            }
            else if (hud.HealthLife == 2)
            {
                //Draw -59 healthbar
                spriteBatch.Draw(HealthBar, new Rectangle(153, 55, 147, 17), new Rectangle(0, 0, HealthBar.Width, 0), Color.Red);
            }
            else if (hud.HealthLife == 1)
            {
                //Draw -162 healthbar
                spriteBatch.Draw(HealthBar, new Rectangle(153, 55, 72, 17), new Rectangle(0, 0, HealthBar.Width, 0), Color.Red);
            }
            else if (hud.HealthLife == 0)
            {
                //Draw -242 healthbar
                spriteBatch.Draw(HealthBar, new Rectangle(153, 55, 0, 17), new Rectangle(0, 0, HealthBar.Width, 0), Color.Red);
            }

            //framesPerSecond.Draw(gameTime, spriteBatch);

            #endregion

            #region Draw Blocks, extraLife

            foreach (var block1 in blocks1)
            {
                block1.Draw(gameTime, spriteBatch);
            }

            foreach (var block3 in blocks3)
            {
                block3.Draw(gameTime, spriteBatch);
            }

            foreach (var extraLife in extraLifes)
            {
                extraLife.Draw(gameTime, spriteBatch);
            }

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
            float[] BlockCornerMarkers;

            foreach (var ball in balls)
            {
                foreach (var block1 in blocks1)
                {
                    if (ball.BoundingRect.Intersects(block1.BoundingRect))
                    {
                        effectWall.Play();

                        Vector2[] blockCorners = new Vector2[4];
                        blockCorners[0] = new Vector2(block1.Position.X - block1.Origin.X * block1.Scale, block1.Position.Y - block1.Origin.Y * block1.Scale);
                        blockCorners[1] = new Vector2(block1.Position.X + block1.Origin.X * block1.Scale, block1.Position.Y - block1.Origin.Y * block1.Scale);
                        blockCorners[2] = new Vector2(block1.Position.X + block1.Origin.X * block1.Scale, block1.Position.Y + block1.Origin.Y * block1.Scale);
                        blockCorners[3] = new Vector2(block1.Position.X - block1.Origin.X * block1.Scale, block1.Position.Y + block1.Origin.Y * block1.Scale);

                        BlockCornerMarkers = new float[4];

                        for (int i = 1; i < BlockCornerMarkers.Length; i++)
                        {
                            Vector2 direction = Vector2.Normalize(blockCorners[i] - block1.Position);
                            BlockCornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                        }

                        Vector2 direction2 = Vector2.Normalize(ball.Position - block1.Position);

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
        }

        private void CheckBlock3Collision()
        {
            // Variable to hold block rectangle 4 corners
            float[] BlockCornerMarkers;

            foreach (var ball in balls)
            {
                foreach (var block3 in blocks3)
                {
                    if (ball.BoundingRect.Intersects(block3.BoundingRect))
                    {
                        effectWall.Play();

                        Vector2[] blockCorners = new Vector2[4];
                        blockCorners[0] = new Vector2(block3.Position.X - block3.Origin.X * block3.Scale, block3.Position.Y - block3.Origin.Y * block3.Scale);
                        blockCorners[1] = new Vector2(block3.Position.X + block3.Origin.X * block3.Scale, block3.Position.Y - block3.Origin.Y * block3.Scale);
                        blockCorners[2] = new Vector2(block3.Position.X + block3.Origin.X * block3.Scale, block3.Position.Y + block3.Origin.Y * block3.Scale);
                        blockCorners[3] = new Vector2(block3.Position.X - block3.Origin.X * block3.Scale, block3.Position.Y + block3.Origin.Y * block3.Scale);

                        BlockCornerMarkers = new float[4];

                        for (int i = 1; i < BlockCornerMarkers.Length; i++)
                        {
                            Vector2 direction = Vector2.Normalize(blockCorners[i] - block3.Position);
                            BlockCornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                        }

                        Vector2 direction2 = Vector2.Normalize(ball.Position - block3.Position);

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
        }

        private void CheckBarCollision()
        {
            for (int i = 0; i < balls.Length; i++)
            {
                if (balls[i].Active)
                {
                    // check left bar
                    if (balls[i].Position.X < this.radius + 77)
                    {
                        balls[i].Position.X = this.radius + 77;
                        balls[i].Velocity.X *= -1;
                        effectWall.Play();
                        effectWall.Volume = wallVolume;
                    }

                    // check right bar
                    else if (balls[i].Position.X > ScreenManager.Viewport.Width - this.radius - 81)
                    {
                        balls[i].Position.X = ScreenManager.Viewport.Width - this.radius - 81;
                        balls[i].Velocity.X *= -1;
                        effectWall.Play();
                    }

                    // check top bar
                    if (balls[i].Position.Y < this.radius + 114)
                    {
                        balls[i].Position.Y = this.radius + 114;
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

        #region row8Pig Collision

        private void row8PigCollision()
        {
            foreach (var row8Pig in pigRow8)
            {
                if (row8Pig == null)
                    continue;

                foreach (var ball in balls)
                {
                    if (ball.BoundingRect.Intersects(row8Pig.BoundingRect))
                    {
                        hud.Score = hud.Score + 10;

                        effectPig.Play();
                        effectPig.Volume = pigVolume;

                        Vector2[] corners = new Vector2[4];
                        corners[0] = new Vector2(row8Pig.Position.X - row8Pig.Origin.X * row8Pig.Scale, row8Pig.Position.Y - row8Pig.Origin.Y * row8Pig.Scale);
                        corners[1] = new Vector2(row8Pig.Position.X + row8Pig.Origin.X * row8Pig.Scale, row8Pig.Position.Y - row8Pig.Origin.Y * row8Pig.Scale);
                        corners[2] = new Vector2(row8Pig.Position.X + row8Pig.Origin.X * row8Pig.Scale, row8Pig.Position.Y + row8Pig.Origin.Y * row8Pig.Scale);
                        corners[3] = new Vector2(row8Pig.Position.X - row8Pig.Origin.X * row8Pig.Scale, row8Pig.Position.Y + row8Pig.Origin.Y * row8Pig.Scale);

                        this.CornerMarkers = new float[4];

                        for (int i = 1; i < CornerMarkers.Length; i++)
                        {
                            Vector2 direction = Vector2.Normalize(corners[i] - row8Pig.Position);
                            CornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                        }

                        Vector2 direction2 = Vector2.Normalize(ball.Position - row8Pig.Position);

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

                        row8Pig.Active = false;
                        row8Pig.Position = new Vector2(1300, 800);
                    }
                }
            }
        }

        #endregion

        #region row9Pig Collision

        private void row9PigCollision()
        {
            foreach (var row9Pig in pigRow9)
            {
                if (row9Pig == null)
                    continue;

                foreach (var ball in balls)
                {
                    if (ball.BoundingRect.Intersects(row9Pig.BoundingRect))
                    {
                        hud.Score = hud.Score + 10;

                        effectPig.Play();
                        effectPig.Volume = pigVolume;

                        Vector2[] corners = new Vector2[4];
                        corners[0] = new Vector2(row9Pig.Position.X - row9Pig.Origin.X * row9Pig.Scale, row9Pig.Position.Y - row9Pig.Origin.Y * row9Pig.Scale);
                        corners[1] = new Vector2(row9Pig.Position.X + row9Pig.Origin.X * row9Pig.Scale, row9Pig.Position.Y - row9Pig.Origin.Y * row9Pig.Scale);
                        corners[2] = new Vector2(row9Pig.Position.X + row9Pig.Origin.X * row9Pig.Scale, row9Pig.Position.Y + row9Pig.Origin.Y * row9Pig.Scale);
                        corners[3] = new Vector2(row9Pig.Position.X - row9Pig.Origin.X * row9Pig.Scale, row9Pig.Position.Y + row9Pig.Origin.Y * row9Pig.Scale);

                        this.CornerMarkers = new float[4];

                        for (int i = 1; i < CornerMarkers.Length; i++)
                        {
                            Vector2 direction = Vector2.Normalize(corners[i] - row9Pig.Position);
                            CornerMarkers[i] = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.Pi;
                        }

                        Vector2 direction2 = Vector2.Normalize(ball.Position - row9Pig.Position);

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

                        row9Pig.Active = false;
                        row9Pig.Position = new Vector2(1300, 800);
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
                foreach(var exrtaLife in extraLifes)
                {
                if (ball.BoundingRect.Intersects(exrtaLife.BoundingRect))
                {
                    exrtaLife.Active = false;
                    exrtaLife.Position = new Vector2(1300, 750);

                    if (hud.HealthLife < 3)
                    {
                        hud.HealthLife = hud.HealthLife + 1;
                    }
                }

                }
            }
        }

        #endregion
    }

    #endregion
}

#endregion