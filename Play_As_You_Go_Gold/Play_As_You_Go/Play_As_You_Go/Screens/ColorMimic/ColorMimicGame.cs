/****************************************************
 *                  Color Mimic Code                *
 *                      By                          *
 *                  James Ward                      *
 *   Property of Scrubby Fresh Studios 2011 all     *
 *   rights reserved.                               *
 * **************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Play_As_You_Go
{
    enum GameState
    {
        OutputClean,
        InputClean,
        Ready,
        Input,
        Output,
        Succeed,
        Fail,
        GameOver
    }

    public class ColorMimicGame : GameScreen
    {
        #region Fields
        const string saveGameFile = "CMHS.sfs";
        const string gametitle = "Color Mimic";
        const string CMother = "CMOtherData.sfs";
        const int highScoreScreen = 2;

        


        SoundEffect[] soundEffect = new SoundEffect[4];
        SoundEffectInstance[] effectSound = new SoundEffectInstance[4];

        SoundEffect[] musicEffect = new SoundEffect[3];
        SoundEffectInstance[] musicSound = new SoundEffectInstance[3];
        int audioTrackIndex = 0;

        Texture2D Texture;
        Texture2D[] backgroundTexture = new Texture2D[7];
        Vector2 backgroundPosition;
        int bgTexture = 0;
        
        GameObject yellowRing;
        GameObject blueRing;
        GameObject redRing;
        GameObject greenRing;
        GameObject playerRing;
        GameObject aiRing;

        GameObject[] Circles = new GameObject[4];

        GameState gameState;

        SpriteFont largeFont;
        SpriteFont scoreFont;
        

        Random random;
       
        List<byte> sequence;
        List<byte> playerSequence;

        int sequenceIndex;
        int score;
        int timerDisplay;
        int lives = 3;
        int targetLevel = 5;
        int level = 1;
        int timerScore;
        int played;
        int prevLvl = 10;

        bool prevState = false;
        
        double pulseDuration;
        double duration = 0.3d;
        double timeInterval = 0.5d;
        double timeRemaining;
        double timer = 15.0d;
        double timerCounter;
        double lvlCountDownTimer = 1.5d;
        double playerCountDown = 5.0d;
        double totalPlayTime;
        double totalPlayTimeCounter = 0.0d;

        ISsystem checkFile = new ISsystem();
        
        ISsystemData data; 
        
        
        #endregion
        
        #region AI
        private void SequenceGenerator(int count)
        {
            for (int i = 0; i < count; i++)
            {
                sequence.Add((byte)random.Next(Circles.Length));
            }
        }
        private void Pulse(byte index, double dur)
        {
            this.pulseDuration = dur;
            Circles[index].Active = true;
            effectSound[index].Play();
        }
        private void PulseOff(GameTime gameTime, int index)
        {
            if (this.pulseDuration > 0d)
            {
                this.pulseDuration -= gameTime.ElapsedGameTime.TotalSeconds;
                if (this.pulseDuration <= 0)
                {
                    Circles[index].Active = false;
                    if (Circles[index].Active == false)
                    {
                        effectSound[index].Stop();
                    }
                }
            }

        }
        private void PressButton(byte buttonIndex)
        {
            if (playerSequence.Count < sequence.Count)
            {
                playerSequence.Add(buttonIndex);
                this.Circles[buttonIndex].Active = true;
                effectSound[buttonIndex].Play();
                CheckSequence();
            }
        }
        private void CheckSequence()
        {
            for (int i = 0; i < playerSequence.Count; i++)
            {
                if (playerSequence[i] != sequence[i])
                {
                    this.gameState = GameState.Fail;
                    return;
                }
            }
            if (playerSequence.Count == sequence.Count)
            {
                /* The following code handles the scoring 
                   for the correct sequence. */
                this.gameState = GameState.Succeed;
                for (int i = 0; i < playerSequence.Count; i++)
                {
                    score = score + 50;
                }
            }
        }
        // Used to add time to the timer as the game progresses.
        // Also added the extra lives code here.
        public void TimerControl() 
        {
            if (level == 5)
            {
                timer = timer + 2.0d;
                lives++;
                targetLevel = level;
                bgTexture++;
            }
            
            if (level == 10)
            {
                timer = timer + 3.0d;
                prevLvl = level;
                lives++;
                bgTexture++;
            }
            else if (level == (prevLvl + 5))
            {
                timer = timer + 3.0d;
                prevLvl = level;
                lives++;
                bgTexture++;
                
                // Used to reset the index variable 
                // to 0 if it become greater than the 
                // items in the array.
                if (bgTexture > 6)
                {
                    bgTexture = 0;
                }
            }
        }
        //Created a new method to check audio state.
        public void AudioTrackUpdate()
        {
                audioTrackIndex++;
                if (audioTrackIndex == 3)
                {
                    audioTrackIndex = 0;
                }
                musicSound[audioTrackIndex].Play();
        }
        #endregion

        public ColorMimicGame()
        {
            TransitionOnTime = TimeSpan.FromSeconds(2);
            TransitionOffTime = TimeSpan.FromSeconds(1);
        }

        public override void Initialize()
        {
            checkFile.checkFile();

            if (checkFile.isSaveable)
            {
                data = ISsystem.LoadHighScores(saveGameFile, 5);
            }
            
            
            backgroundPosition = new Vector2(0, 0);
            
            random = new Random();
            sequence = new List<byte>();
            playerSequence = new List<byte>();
            SequenceGenerator(3);
            timeRemaining = timeInterval;

            gameState = GameState.Ready;
          
            #region Sound Effects
            effectSound[0] = soundEffect[0].CreateInstance();
            effectSound[1] = soundEffect[1].CreateInstance();
            effectSound[2] = soundEffect[2].CreateInstance();
            effectSound[3] = soundEffect[3].CreateInstance();

            effectSound[0].Volume = 0.2f;
            effectSound[1].Volume = 0.2f;
            effectSound[2].Volume = 0.2f;
            effectSound[3].Volume = 0.2f;

            musicSound[0] = musicEffect[0].CreateInstance();
            musicSound[1] = musicEffect[1].CreateInstance();
            musicSound[2] = musicEffect[2].CreateInstance();

            musicSound[0].Volume = 0.25f;
            musicSound[1].Volume = 0.25f;
            musicSound[2].Volume = 0.25f;
            
            #endregion

            timerCounter = timer;

            #region graphics
            redRing = new GameObject(Texture, new Rectangle(0, 130, 64, 64), 1, true, 0);
            redRing.Position = new Vector2(((ScreenManager.Viewport.Width / 2) + 185), ScreenManager.Viewport.Height / 2);
            redRing.Origin = new Vector2((redRing.ActualWidth / 2) / redRing.Scale, (redRing.ActualHeight / 2) / redRing.Scale);
            redRing.Scale = 1.5f;

            blueRing = new GameObject(Texture, new Rectangle(0, 195, 64, 64), 1, true, 0);
            blueRing.Position = new Vector2(((ScreenManager.Viewport.Width / 2) - 175), ScreenManager.Viewport.Height / 2);
            blueRing.Origin = new Vector2((blueRing.ActualWidth / 2) / blueRing.Scale, (blueRing.ActualHeight / 2) / blueRing.Scale);
            blueRing.Scale = 1.5f;

            yellowRing = new GameObject(Texture, new Rectangle(0, 0, 64, 64), 1, true, 0);
            yellowRing.Position = new Vector2(((ScreenManager.Viewport.Width / 2)), ScreenManager.Viewport.Height / 2 - 175);
            yellowRing.Origin = new Vector2((yellowRing.ActualWidth / 2) / yellowRing.Scale, (yellowRing.ActualHeight / 2) / yellowRing.Scale);
            yellowRing.Scale = 1.5f;

            greenRing = new GameObject(Texture, new Rectangle(0, 65, 64, 64), 1, true, 0);
            greenRing.Position = new Vector2(((ScreenManager.Viewport.Width / 2)), ((ScreenManager.Viewport.Height / 2) + 175));
            greenRing.Origin = new Vector2((greenRing.ActualWidth / 2) / greenRing.Scale, (greenRing.ActualHeight / 2) / greenRing.Scale);
            greenRing.Scale = 1.5f;

            aiRing = new GameObject(Texture, new Rectangle(66, 130, 64, 64), 1, true, 0);
            aiRing.Position = new Vector2(((ScreenManager.Viewport.Width / 2) - 275), 75);
            aiRing.Origin = new Vector2((redRing.ActualWidth / 2) / redRing.Scale, (redRing.ActualHeight / 2) / redRing.Scale);
            aiRing.Scale = 0.5f;

            playerRing = new GameObject(Texture, new Rectangle(66, 195, 64, 64), 1, true, 0);
            playerRing.Position = new Vector2(((ScreenManager.Viewport.Width / 2) - 275), 75);
            playerRing.Origin = new Vector2((blueRing.ActualWidth / 2) / blueRing.Scale, (blueRing.ActualHeight / 2) / blueRing.Scale);
            playerRing.Scale = 0.5f;
            
            //Blue circle
            Circles[0] = new GameObject(Texture, new Rectangle(66, 195, 64, 64), 1, true, 0);
            Circles[0].Position = new Vector2(((ScreenManager.Viewport.Width / 2) - 175), ScreenManager.Viewport.Height / 2);
            Circles[0].Origin = new Vector2((Circles[0].ActualWidth / 2) / Circles[0].Scale, (Circles[0].ActualHeight / 2) / Circles[0].Scale);
            Circles[0].Scale = 1.5f;
            Circles[0].Active = false;

            //Red Circle
            Circles[1] = new GameObject(Texture, new Rectangle(66, 130, 64, 64), 1, true, 0);
            Circles[1].Position = new Vector2(((ScreenManager.Viewport.Width / 2) + 185), ScreenManager.Viewport.Height / 2);
            Circles[1].Origin = new Vector2((Circles[1].ActualWidth / 2) / Circles[1].Scale, (Circles[1].ActualHeight / 2) / Circles[1].Scale);
            Circles[1].Scale = 1.5f;
            Circles[1].Active = false;
            
            //Yellow cirlce
            Circles[2] = new GameObject(Texture, new Rectangle(66, 0, 64, 64), 1, true, 0);
            Circles[2].Position = new Vector2(((ScreenManager.Viewport.Width / 2)), ((ScreenManager.Viewport.Height / 2) - 175));
            Circles[2].Origin = new Vector2((Circles[2].ActualWidth / 2) / Circles[2].Scale, (Circles[2].ActualHeight / 2) / Circles[2].Scale);
            Circles[2].Scale = 1.5f;
            Circles[2].Active = false;
            
            //Green circle
            Circles[3] = new GameObject(Texture, new Rectangle(66, 65, 64, 64), 1, true, 0);
            Circles[3].Position = new Vector2(((ScreenManager.Viewport.Width / 2)), ((ScreenManager.Viewport.Height / 2) + 175));
            Circles[3].Origin = new Vector2((Circles[3].ActualWidth / 2) / Circles[3].Scale, (Circles[3].ActualHeight / 2) / Circles[3].Scale);
            Circles[3].Scale = 1.5f;
            Circles[3].Active = false;

            #endregion

           
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            #region Sprite Sheets
            Texture = content.Load<Texture2D>("Images/ColorMimicImages/CMSpriteSheet");
            #endregion

            #region Fonts
            largeFont = content.Load<SpriteFont>("Fonts/Color Mimic Fonts/largeFont");
            scoreFont = content.Load<SpriteFont>("Fonts/Color Mimic Fonts/CMscoreFont");

            backgroundTexture[0] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Colorful");
            backgroundTexture[1] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Bubbles");
            backgroundTexture[2] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Magic");
            backgroundTexture[3] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Color");
            backgroundTexture[4] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Night sky");
            backgroundTexture[5] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Stary night");
            backgroundTexture[6] = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Sunset");

            #endregion

            #region Sound Effects
            soundEffect[0] = content.Load<SoundEffect>("Audio/Color Mimic SoundFX/BlueMid");
            soundEffect[1] = content.Load<SoundEffect>("Audio/Color Mimic SoundFX/YellowMid");
            soundEffect[2] = content.Load<SoundEffect>("Audio/Color Mimic SoundFX/GreenMid");
            soundEffect[3] = content.Load<SoundEffect>("Audio/Color Mimic SoundFX/RedMid");

            musicEffect[0] = content.Load<SoundEffect>("Audio/Color Mimic SoundFX/Audio tracks/Crank");
            musicEffect[1] = content.Load<SoundEffect>("Audio/Color Mimic SoundFX/Audio tracks/Gape");
            musicEffect[2] = content.Load<SoundEffect>("Audio/Color Mimic SoundFX/Audio tracks/RizyBiz");

            #endregion
        }
        
        public override void Update(GameTime gameTime, bool covered)
        {
            if (musicSound[audioTrackIndex].State == SoundState.Stopped)
            {
                AudioTrackUpdate();
            }

            if (MediaPlayer.GameHasControl)
            {
                musicSound[audioTrackIndex].Play();
            }
            else
                musicSound[audioTrackIndex].Pause();

            int playerIndex = (int)ControllingPlayer.Value;


            KeyboardState keyboardState = ScreenManager.InputSystem.currentKeyboardState[playerIndex];
            GamePadState gamepadState = ScreenManager.InputSystem.currentGamePadState[playerIndex];
            PlayerIndex pIndex = new PlayerIndex();
            pIndex = ControllingPlayer.Value;
            bool gamePadDisconnected = !gamepadState.IsConnected && ScreenManager.InputSystem.gamePadWasConnected[playerIndex];

            // Pause
            if (ScreenManager.InputSystem.PauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                musicSound[audioTrackIndex].Pause();
                ScreenState = ScreenState.Frozen;
                prevState = true;
                ScreenManager.AddScreen(new PauseScreen(this), ControllingPlayer);
            }
            else
            {
               ScreenState = ScreenState.Active;
                 if (prevState == true)
                {
                    InputHelper.Update(prevState, pIndex);
                    musicSound[audioTrackIndex].Resume();
                    prevState = false;
                }
            }


            #region GamePadDisconnect
#if XBOX
           
            
            
            
            
            /* if(ScreenManager.InputSystem.PauseGame || ScreenManager.InputSystem.gamePadDisconnected == true)
            {
                musicSound[audioTrackIndex].Pause();
                prevState = true;
                ScreenState = ScreenState.Frozen;
                ScreenManager.AddScreen(new PauseScreen(this));
            }
            else
            {
                ScreenState = ScreenState.Active;
                
                if (prevState == true)
                {
                    InputHelper.Update(prevState, , playerIndex);
                    musicSound[audioTrackIndex].Resume();
                    prevState = false;
                }

            }*/
#endif
            #endregion


            #region Exit Commands
#if WINDOWS
            /*if (ScreenManager.InputSystem.PauseGame)
            {
                musicSound[audioTrackIndex].Pause();
                prevState = true;
                ScreenState = ScreenState.Frozen;
                ScreenManager.AddScreen(new PauseScreen(this));
            }
            else
            {
                ScreenState = ScreenState.Active;
                
                if (prevState == true)
                {
                    InputHelper.Update(prevState, playerIndex);
                    musicSound[audioTrackIndex].Resume();
                    prevState = false;
                }

            }*/
#endif
            #endregion

            for (int i = 0; i < Circles.Length; i++)
            {
                if (Circles[i].Active)
                {
                    PulseOff(gameTime, i);
                }
            }
            InputHelper.Update(prevState,pIndex);

            switch (gameState)
            {
                #region Switch Statments

                case GameState.Ready:
                    this.playerCountDown -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (playerCountDown <= 0.995)
                    {
                        gameState = GameState.Output;
                    }
                    break;
                case GameState.OutputClean:
                    for (int i = 0; i < Circles.Length; i++)
                    {
                        if (Circles[i].Active)
                        {
                            PulseOff(gameTime, i);
                        }
                    }
                    gameState = GameState.Input;
                    break;
                case GameState.InputClean:
                    for (int i = 0; i < Circles.Length; i++)
                    {
                        if (Circles[i].Active)
                        {
                            Circles[i].Active = false;
                        }
                    }
                    
                    gameState = GameState.Output;
                    break;
                case GameState.Input:
                    this.timerCounter -= gameTime.ElapsedGameTime.TotalSeconds;

#if WINDOWS
                    if (InputHelper.gameKeys[0].Pressed)
                    {
                        PressButton(0);
                        effectSound[0].Play();
                    }

                    if (InputHelper.gameKeys[0].Released)
                    {
                        this.Circles[0].Active = false;
                        if (Circles[0].Active == false)
                        {
                            effectSound[0].Stop();
                        }
                    }

                    if (InputHelper.gameKeys[1].Pressed)
                    {
                        PressButton(2);
                        effectSound[2].Play();
                    }
                    if (InputHelper.gameKeys[1].Released)
                    {
                        this.Circles[2].Active = false;
                        if (Circles[2].Active == false)
                        {
                            effectSound[2].Stop();
                        }
                    }
                    if (InputHelper.gameKeys[2].Pressed)
                    {
                        PressButton(3);
                        effectSound[3].Play();
                    }
                    if (InputHelper.gameKeys[2].Released)
                    {
                        this.Circles[3].Active = false;
                        if (Circles[3].Active == false)
                        {
                            effectSound[3].Stop();
                        }
                    }
                    if (InputHelper.gameKeys[3].Pressed)
                    {
                        PressButton(1);
                        effectSound[1].Play();
                    }
                    if (InputHelper.gameKeys[3].Released)
                    {
                        this.Circles[1].Active = false;
                        if (Circles[1].Active == false)
                        {
                            effectSound[1].Stop();
                        }
                    }
#else
                    if (InputHelper.gameButtons[0].Pressed)
                    {
                        PressButton(0);
                        effectSound[0].Play();
                    }

                    if (InputHelper.gameButtons[0].Released)
                    {
                        this.Circles[0].Active = false;
                        if (Circles[0].Active == false)
                        {
                            effectSound[0].Stop();
                        }
                    }

                    if (InputHelper.gameButtons[1].Pressed)
                    {
                        PressButton(2);
                        effectSound[2].Play();
                    }
                    if (InputHelper.gameButtons[1].Released)
                    {
                        this.Circles[2].Active = false;
                        if (Circles[2].Active == false)
                        {
                            effectSound[2].Stop();
                        }
                    }
                    if (InputHelper.gameButtons[2].Pressed)
                    {
                        PressButton(3);
                        effectSound[3].Play();
                    }
                    if (InputHelper.gameButtons[2].Released)
                    {
                        this.Circles[3].Active = false;
                        if (Circles[3].Active == false)
                        {
                            effectSound[3].Stop();
                        }
                    }
                    if (InputHelper.gameButtons[3].Pressed)
                    {
                        PressButton(1);
                        effectSound[1].Play();
                    }
                    if (InputHelper.gameButtons[3].Released)
                    {
                        this.Circles[1].Active = false;
                        if (Circles[1].Active == false)
                        {
                            effectSound[1].Stop();
                        }
                    }

#endif

                    if (this.timerCounter <= 0d)
                    {
                        gameState = GameState.Fail;
                    }
                    break;
                case GameState.Output:
                    
                    this.timeRemaining -= gameTime.ElapsedGameTime.TotalSeconds;

                    if (this.timeRemaining <= 0d)
                    {
                            Pulse(sequence[sequenceIndex], duration);
                            sequenceIndex++;

                        if (sequenceIndex >= sequence.Count)
                        {
                            playerSequence.Clear();

                            gameState = GameState.OutputClean;
                        }
                        this.timeRemaining = this.timeInterval;
                    }
                    break;


                case GameState.Succeed:
                    this.lvlCountDownTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                    if(lvlCountDownTimer <= 0.0d)
                    {
                        level++;
                        if (gameState == GameState.Succeed)
                        {
                            totalPlayTime = (int)timer - (int)timerCounter;
                            totalPlayTimeCounter = totalPlayTimeCounter + totalPlayTime;
                            timerScore = (int)timerCounter * 10;
                            score = score + timerScore;
                            timerCounter = timer;
                            TimerControl();
                            SequenceGenerator(1);
                            lvlCountDownTimer = 1.5d;
                        }
                        sequenceIndex = 0;
                        gameState = GameState.InputClean;
                    }
                    break;
                case GameState.Fail:
                    this.lvlCountDownTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (lvlCountDownTimer <= 0.0d)
                    {
                        totalPlayTime = (int)timer - (int)timerCounter;
                        totalPlayTimeCounter = totalPlayTimeCounter + totalPlayTime;
                        lives--;
                        timerCounter = timer;
                        lvlCountDownTimer = 1.5d;
                        if (gameState == GameState.Succeed)
                        {
                            SequenceGenerator(1);
                        }
                        sequenceIndex = 0;
                        gameState = GameState.InputClean;
                    }
                    break;
                case GameState.GameOver:
                    for (int i = 0; i < Circles.Length; i++)
                    {
                        if (Circles[i].Active)
                        {
                            Circles[i].Active = false;
                        }
                    }
                    this.lvlCountDownTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (lvlCountDownTimer <= 0.0d)
                    {
                        
                        played = (int)totalPlayTimeCounter;
                        musicSound[audioTrackIndex].Stop();
                        base.Remove();
                        if (checkFile.isSaveable)
                        {
                            data.gameTitle[0] = gametitle;
                            ScreenManager.AddScreen(new HighScoreInterface(data, score, level, played, saveGameFile, highScoreScreen, backgroundTexture[bgTexture]), ControllingPlayer);
                        }
                        else
                        {
                            ScreenManager.AddScreen(new ColorMimicMainMenu(), ControllingPlayer);
                        }
                    }
                    break;
                #endregion
            }

            if (lives == 0)
            {
                gameState = GameState.GameOver;
            }
            base.Update(gameTime, IsExiting);
        }

        public override void Draw(GameTime gameTime)
        {
            #region Local variables
            string text = "";
            Vector2 pos = largeFont.MeasureString(text);

            int screenWidth = ScreenManager.Viewport.Width;
            int screenHeight = ScreenManager.Viewport.Height;

            string start = " Get Ready";
            string set = "Set";
            string go = "Go!";

            string scoreLabal = "Score";
            timerDisplay = (int)timerCounter;
            string scoring = string.Format("{0}", score);
            string timer = string.Format("{0}", timerDisplay);
            string lifeCounter = string.Format("{0}", lives);
            string checker = string.Format("{0}", level);

            Color textColor = Color.BlanchedAlmond;
            #endregion

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture[bgTexture], backgroundPosition, Color.Gray);

            #region Graphics
            redRing.Draw(gameTime, spriteBatch);
            blueRing.Draw(gameTime, spriteBatch);
            yellowRing.Draw(gameTime, spriteBatch);
            greenRing.Draw(gameTime, spriteBatch);

            Circles[0].Draw(gameTime, spriteBatch);
            Circles[1].Draw(gameTime, spriteBatch);
            Circles[2].Draw(gameTime, spriteBatch);
            Circles[3].Draw(gameTime, spriteBatch);
            #endregion

            #region Screen Messages
            if (this.gameState == GameState.Fail)
            {
                pos.X = screenWidth / 2 - 90;
                pos.Y = screenHeight / 2 - 25;
                text = "   FAIL";
                textColor = Color.Red;
            }
            else if (this.gameState == GameState.Succeed)
            {
                pos.X = screenWidth / 2 - 135;
                pos.Y = screenHeight / 2 - 25;
                text = "   SUCCESS";
                textColor = Color.CornflowerBlue;
            }
            if (gameState == GameState.Ready)
            {
                spriteBatch.DrawString(largeFont, text, pos, textColor);
            }
            spriteBatch.DrawString(largeFont, text, pos, textColor);
            #endregion

            #region Timer Display
            spriteBatch.DrawString(scoreFont, "Time", new Vector2(screenWidth / 2, 30), Color.White);
            spriteBatch.DrawString(scoreFont, timer, new Vector2(screenWidth / 2 + 5, 50), Color.White);
            #endregion

            #region Lives
            spriteBatch.DrawString(scoreFont, "Lives", new Vector2(screenWidth / 2 + 335, 30), Color.White);
            spriteBatch.DrawString(scoreFont, lifeCounter, new Vector2(screenWidth / 2 + 350, 50), Color.White);
            #endregion

            #region Scroe drawing area
            spriteBatch.DrawString(scoreFont, scoreLabal, new Vector2(100, 30), Color.White);
            if (score == 0)
            {
                spriteBatch.DrawString(scoreFont, scoring, new Vector2(115, 50), Color.White);
            }
            if (score > 0 && score < 1000)
            {
                spriteBatch.DrawString(scoreFont, scoring, new Vector2(100, 50), Color.White);
            }
            else if (score >= 1000)
            {
                spriteBatch.DrawString(scoreFont, scoring, new Vector2(95, 50), Color.White);
            }
            #endregion

            #region Player promts
            if (gameState == GameState.Ready)
            {
                if (playerCountDown >= 3.0)
                {
                    spriteBatch.DrawString(largeFont, start, new Vector2(screenWidth / 2 -140, screenHeight / 2 - 25), Color.White);
                }
                if (playerCountDown >= 2.0 && playerCountDown <= 3.0)
                {
                    spriteBatch.DrawString(largeFont, set, new Vector2(screenWidth / 2 - 75, screenHeight / 2 - 25), Color.White);
                }
                if (playerCountDown >= 1.0 && playerCountDown <= 2.0)
                {
                    spriteBatch.DrawString(largeFont, set, new Vector2(screenWidth / 2 - 75, screenHeight / 2 - 25), Color.White);
                    spriteBatch.DrawString(largeFont, go, new Vector2(screenWidth / 2 + 25, screenHeight / 2 - 25), Color.White);
                }
            }

            if (gameState == GameState.GameOver)
            {
                spriteBatch.DrawString(largeFont, "Game Over", new Vector2(screenWidth / 2 - 129, screenHeight / 2 - 25), Color.White);
            }
            #endregion

            spriteBatch.DrawString(scoreFont, "Turn", new Vector2(screenWidth / 2-275 , 30), Color.White);
            if (gameState == GameState.Input || gameState == GameState.Succeed || gameState == GameState.Fail)
            {
                playerRing.Draw(gameTime, spriteBatch);
                spriteBatch.DrawString(scoreFont, "Player", new Vector2(screenWidth / 2 - 250, 59), Color.White);
            }
            else if (gameState == GameState.Output || gameState == GameState.Ready)
            {
                aiRing.Draw(gameTime, spriteBatch);
                spriteBatch.DrawString(scoreFont, "AI", new Vector2(screenWidth / 2 - 250, 59), Color.White);
            }
            spriteBatch.End();
        }
    }
}
