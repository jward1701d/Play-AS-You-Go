#region Code Credits
/*
 * ******************************************
 *          Color Mimic Mobile Version      *
 *                  James Ward              *
 *              August 24, 2011             *
 * Property of Scrubby Fresh Studios L.L.C  *
 * ******************************************
 */
#endregion

#region Using Statements
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

#region Namespace
namespace PAYG
{
    #region enum area
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
    }//End enum
    #endregion

    #region Color Mimic Game Class
    class ColorMimicGame : GameScreen
    {
        #region Fields
        #region Fields - Const Variables
        const string saveGameFile = "CMHS.sfs";
        const string gametitle = "Color Mimic";
        const string CMother = "CMotherData.sfs";
        const int highScoreScreen = 2;
        #endregion

        #region Fields - Audio Variables
        SoundEffect[] soundEffect = new SoundEffect[4];
        SoundEffect[] musicEffect = new SoundEffect[3];
        SoundEffectInstance[] effectSound = new SoundEffectInstance[4];
        SoundEffectInstance[] musicSound = new SoundEffectInstance[3];
        int audioTrackIndex = 0;
        #endregion

        #region Fields - Texture2Ds
        Texture2D Texture;
        Texture2D[] backgroundTexture = new Texture2D[7];
        Vector2 backgroundPosition;
        int bgTexture = 0;
        #endregion

        #region Fields - GameObjects
        GameObject yellowRing;
        GameObject blueRing;
        GameObject redRing;
        GameObject greenRing;
        GameObject playerRing;
        GameObject aiRing;
        GameObject[] Circles = new GameObject[4];
        #endregion

        #region Fields - GameState
        GameState gameState;
        #endregion

        #region Fields - Sprite Fonts
        SpriteFont largeFont;
        SpriteFont scoreFont;
        #endregion

        #region Fields - Random Variable
        Random random;
        #endregion

        #region Fields - List Variables
        List<byte> sequence;
        List<byte> playerSequence;
        #endregion

        #region Fields - Int Variables
        int sequenceIndex;
        int score;
        int timerDisplay;
        int lives = 3;
        //int targetLevel = 5;
        int level; 
        int timerScore;
        int played;
        int prevLvl = 10;
        #endregion

        #region Fields - Double Variables
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
        double phonePulseDur = 0.30d;
        #endregion

        #region Fields - ISSystem Objects
        ISsystem checkFile = new ISsystem();
        ISsystemData data; 
        #endregion

        #region Tombstone variable
        bool TSeffect = false;
        #endregion

        #endregion

        #region AI
        #region AI - Sequence Code
        private void SequenceGenerator(int count)
        {
            for (int i = 0; i < count; i++)
            {
                sequence.Add((byte)random.Next(Circles.Length));
            }//End for
        }
        #endregion
        #region AI - Pulse Code
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
                    }//End if
                }//End if
            }//End if

        }
        private void PulseOff(GameTime gameTime, int index, double dur)
        {
            this.pulseDuration = dur;
            if (this.pulseDuration > 0d)
            {
                this.pulseDuration -= gameTime.ElapsedGameTime.TotalSeconds;
                if (this.pulseDuration <= 0)
                {
                    Circles[index].Active = false;
                    if (Circles[index].Active == false)
                    {
                        effectSound[index].Stop();
                    }//End if
                }//End if
            }//End if

        }
        #endregion
        #region AI - Player Checking Code
        private void PressButton(byte buttonIndex)
        {
            if (playerSequence.Count < sequence.Count)
            {
                playerSequence.Add(buttonIndex);
                this.Circles[buttonIndex].Active = true;
                effectSound[buttonIndex].Play();
                CheckSequence();
            }//End if
        }
        private void CheckSequence()
        {
            for (int i = 0; i < playerSequence.Count; i++)
            {
                if (playerSequence[i] != sequence[i])
                {
                    this.gameState = GameState.Fail;
                    return;
                }//End if
            }//End for
            if (playerSequence.Count == sequence.Count)
            {
                /* The following code handles the scoring 
                   for the correct sequence. */
                this.gameState = GameState.Succeed;
                for (int i = 0; i < playerSequence.Count; i++)
                {
                    score = score + 50;
                }//End for
            }//End if
        }
        #endregion
        #region AI - Timer Control Code
        public void TimerControl()
        {
            if (level == 5)
            {
                timer = timer + 2.0d;
                lives++;
                //targetLevel = level;
                bgTexture++;
            }//End if

            if (level == 10)
            {
                timer = timer + 3.0d;
                prevLvl = level;
                lives++;
                bgTexture++;
            }//End if
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
                }//End if
            }//End if
        }
        #endregion
        #region AI - Audio Handler Code
        public void AudioTrackUpdate()
        {
            audioTrackIndex++;
            if (audioTrackIndex == 3)
            {
                audioTrackIndex = 0;
            }//End if
            musicSound[audioTrackIndex].Play();
        }
        #endregion


        #endregion

        #region Constructor
        public ColorMimicGame()
        {
            TransitionOnTime = TimeSpan.FromSeconds(2);
            TransitionOffTime = TimeSpan.FromSeconds(1);
        }
        public ColorMimicGame(bool active)
        {
            TransitionOnTime = TimeSpan.FromSeconds(2);
            TransitionOffTime = TimeSpan.FromSeconds(1);
            TSeffect = active;
        }
        #endregion

        #region Initialize Method
        public override void Initialize()
        {
            PhoneApplicationService.Current.Deactivated += new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(SaveState);
            #region Cecck Saveable Code
            checkFile.checkFile();

            if (checkFile.isSaveable)
            {
                data = ISsystem.LoadHighScores(saveGameFile, 5);
                data.gameTitle[0] = gametitle;
                
            }
            #endregion
            #region Ingame Ads
            ScreenManager.AdPositionX = 160;
            ScreenManager.AdPositionY = 390;
            ScreenManager.CreateAd();
            #endregion
            #region Variable Settings
            backgroundPosition = new Vector2(0, 0);

            random = new Random();
            sequence = new List<byte>();
            playerSequence = new List<byte>();
            timeRemaining = timeInterval;
            level = 1;

            if (TSeffect == true)
            {
                LoadLevel();
            }
            SequenceGenerator(level + 2);
            timerCounter = timer;

            gameState = GameState.Ready;

            
            #endregion
            #region Sound Effects
            effectSound[0] = soundEffect[0].CreateInstance();
            effectSound[1] = soundEffect[1].CreateInstance();
            effectSound[2] = soundEffect[2].CreateInstance();
            effectSound[3] = soundEffect[3].CreateInstance();

            effectSound[0].Volume = ScreenManager.FXVolume;
            effectSound[1].Volume = ScreenManager.FXVolume;
            effectSound[2].Volume = ScreenManager.FXVolume;
            effectSound[3].Volume = ScreenManager.FXVolume;

            musicSound[0] = musicEffect[0].CreateInstance();
            musicSound[1] = musicEffect[1].CreateInstance();
            musicSound[2] = musicEffect[2].CreateInstance();

            musicSound[0].Volume = ScreenManager.MusicVolume;
            musicSound[1].Volume = ScreenManager.MusicVolume;
            musicSound[2].Volume = ScreenManager.MusicVolume;

            #endregion
            #region graphics
            redRing = new GameObject(Texture, new Rectangle(0, 130, 64, 64), 1, true, 0);
            redRing.Position = new Vector2(((ScreenManager.Viewport.Width / 2) + 135), (ScreenManager.Viewport.Height / 2) - 40);
            redRing.Origin = new Vector2((redRing.ActualWidth / 2) / redRing.Scale, (redRing.ActualHeight / 2) / redRing.Scale);
            redRing.Scale = 1.0f;

            blueRing = new GameObject(Texture, new Rectangle(0, 195, 64, 64), 1, true, 0);
            blueRing.Position = new Vector2(((ScreenManager.Viewport.Width / 2) - 135), (ScreenManager.Viewport.Height / 2)-40);
            blueRing.Origin = new Vector2((blueRing.ActualWidth / 2) / blueRing.Scale, (blueRing.ActualHeight / 2) / blueRing.Scale);
            blueRing.Scale = 1.0f;

            yellowRing = new GameObject(Texture, new Rectangle(0, 0, 64, 64), 1, true, 0);
            yellowRing.Position = new Vector2(((ScreenManager.Viewport.Width / 2)), ScreenManager.Viewport.Height / 2 - 145); // Y=-125
            yellowRing.Origin = new Vector2((yellowRing.ActualWidth / 2) / yellowRing.Scale, (yellowRing.ActualHeight / 2) / yellowRing.Scale);
            yellowRing.Scale = 1.0f;

            greenRing = new GameObject(Texture, new Rectangle(0, 65, 64, 64), 1, true, 0);
            greenRing.Position = new Vector2(((ScreenManager.Viewport.Width / 2)), ((ScreenManager.Viewport.Height / 2) + 65)); // Y = +125
            greenRing.Origin = new Vector2((greenRing.ActualWidth / 2) / greenRing.Scale, (greenRing.ActualHeight / 2) / greenRing.Scale);
            greenRing.Scale = 1.0f;

            aiRing = new GameObject(Texture, new Rectangle(66, 130, 64, 64), 1, true, 0);
            aiRing.Position = new Vector2(((ScreenManager.Viewport.Width / 2) - 200), 45);
            aiRing.Origin = new Vector2((redRing.ActualWidth / 2) / redRing.Scale, (redRing.ActualHeight / 2) / redRing.Scale);
            aiRing.Scale = 0.5f;

            playerRing = new GameObject(Texture, new Rectangle(66, 195, 64, 64), 1, true, 0);
            playerRing.Position = new Vector2(((ScreenManager.Viewport.Width / 2) - 200), 45);
            playerRing.Origin = new Vector2((blueRing.ActualWidth / 2) / blueRing.Scale, (blueRing.ActualHeight / 2) / blueRing.Scale);
            playerRing.Scale = 0.5f;

            //Blue circle
            Circles[0] = new GameObject(Texture, new Rectangle(66, 195, 64, 64), 1, true, 0);
            Circles[0].Position = new Vector2(((ScreenManager.Viewport.Width / 2) - 135), (ScreenManager.Viewport.Height / 2)-40);
            Circles[0].Origin = new Vector2((Circles[0].ActualWidth / 2) / Circles[0].Scale, (Circles[0].ActualHeight / 2) / Circles[0].Scale);
            Circles[0].Scale = 1.0f;
            Circles[0].Active = false;

            //Red Circle
            Circles[1] = new GameObject(Texture, new Rectangle(66, 130, 64, 64), 1, true, 0);
            Circles[1].Position = new Vector2(((ScreenManager.Viewport.Width / 2) + 135), (ScreenManager.Viewport.Height / 2 )-40);
            Circles[1].Origin = new Vector2((Circles[1].ActualWidth / 2) / Circles[1].Scale, (Circles[1].ActualHeight / 2) / Circles[1].Scale);
            Circles[1].Scale = 1.0f;
            Circles[1].Active = false;

            //Yellow cirlce
            Circles[2] = new GameObject(Texture, new Rectangle(66, 0, 64, 64), 1, true, 0);
            Circles[2].Position = new Vector2(((ScreenManager.Viewport.Width / 2)), ((ScreenManager.Viewport.Height / 2) - 145)); // Y = -125
            Circles[2].Origin = new Vector2((Circles[2].ActualWidth / 2) / Circles[2].Scale, (Circles[2].ActualHeight / 2) / Circles[2].Scale);
            Circles[2].Scale = 1.0f;
            Circles[2].Active = false;

            //Green circle
            Circles[3] = new GameObject(Texture, new Rectangle(66, 65, 64, 64), 1, true, 0);
            Circles[3].Position = new Vector2(((ScreenManager.Viewport.Width / 2)), ((ScreenManager.Viewport.Height / 2) + 65)); // Y = +125
            Circles[3].Origin = new Vector2((Circles[3].ActualWidth / 2) / Circles[3].Scale, (Circles[3].ActualHeight / 2) / Circles[3].Scale);
            Circles[3].Scale = 1.0f;
            Circles[3].Active = false;

            #endregion
        }
        #endregion

        #region LoadContent Method
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            #region Sprite Sheets
            Texture = content.Load<Texture2D>("Images/ColorMimicImages/CMSpriteSheet");
            #endregion
            #region Fonts
            largeFont = content.Load<SpriteFont>("Fonts/Color Mimic Fonts/largeFont");
            scoreFont = content.Load<SpriteFont>("Fonts/Color Mimic Fonts/CMscoreFont");
            #endregion
            #region Background Textures
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
        #endregion

        #region UnloadContent Method
        public override void UnloadContent()
        {
            #region Background unloading
            for (int i = 0; i < 7; i++)
            {
                backgroundTexture[i] = null;
            }//End for
            #endregion

            #region Sound Effects unloading
            for (int j = 0; j < 4; j++)
            {
                soundEffect[j] = null;
            }//End for
            #endregion

            #region Music unloading
            musicSound[audioTrackIndex].Stop();
            for (int k = 0; k < 3; k++)
            {
                musicEffect[k] = null;
            }//End for
            
            #endregion

            #region Remaining Unloadind
            largeFont = null;
            scoreFont = null;
            Texture = null;
            #endregion
        }
        #endregion

        #region Update Method
        public override void Update(GameTime gameTime, bool covered)
        {
            musicSound[0].Volume = ScreenManager.MusicVolume;
            musicSound[1].Volume = ScreenManager.MusicVolume;
            musicSound[2].Volume = ScreenManager.MusicVolume;

            effectSound[0].Volume = ScreenManager.FXVolume;
            effectSound[1].Volume = ScreenManager.FXVolume;
            effectSound[2].Volume = ScreenManager.FXVolume;
            effectSound[3].Volume = ScreenManager.FXVolume;

            foreach (GestureSample gesture in ScreenManager.InputSystem.Gestures)
            {
                if (gesture.GestureType == GestureType.Hold)
                {
                    ScreenManager.isPressed = true;
                    musicSound[audioTrackIndex].Pause();
                    ScreenState = ScreenState.Frozen;
                    ScreenManager.AddScreen(new PauseScreen(this));
                }
                    
                else
                {
                    ScreenManager.isPressed = false;
                    ScreenState = ScreenState.Active;
                    if (musicSound[audioTrackIndex].State == SoundState.Paused)
                    {
                        musicSound[audioTrackIndex].Resume();

                    }

                }
            }
             

            if (TSeffect == true)
            {
                ScreenManager.isPressed = true;
                ScreenState = ScreenState.Frozen;
                ScreenManager.AddScreen(new PauseScreen(this));
                TSeffect = false;
            }
            
            #region Audio Track update management

            if (musicSound[audioTrackIndex].State == SoundState.Stopped)
            {
                AudioTrackUpdate();
            }
            if (MediaPlayer.GameHasControl && musicSound[audioTrackIndex].State != SoundState.Paused)
            {
                musicSound[audioTrackIndex].Play();
            }
            else
                musicSound[audioTrackIndex].Pause();
            #endregion
            #region Circle Clean up
            for (int i = 0; i < Circles.Length; i++)
            {
                if (Circles[i].Active)
                {
                    PulseOff(gameTime, i);
                }
            }
            #endregion
            #region Switch Statments
            switch (gameState)
            {
                #region Switch Ready
                case GameState.Ready:
                    this.playerCountDown -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (playerCountDown <= 0.995)
                    {
                        gameState = GameState.Output;
                    }//End if
                    break;
                #endregion
                #region Switch outputClean
                case GameState.OutputClean:
                    for (int i = 0; i < Circles.Length; i++)
                    {
                        if (Circles[i].Active)
                        {
                            PulseOff(gameTime, i);
                        }//End if
                    }//End for
                    gameState = GameState.Input;
                    break;
                #endregion
                #region Switch inputClean
                case GameState.InputClean:
                    for (int i = 0; i < Circles.Length; i++)
                    {
                        if (Circles[i].Active)
                        {
                            Circles[i].Active = false;
                        }//End if
                    }// End for

                    gameState = GameState.Output;
                    break;
                #endregion
                #region Switch Input
                case GameState.Input:
                    this.timerCounter -= gameTime.ElapsedGameTime.TotalSeconds;

                    #region Player phone controls
                    //Handles for the phone input.

                    if (ScreenManager.InputSystem.IsRectanglePressed(this.Circles[0].BoundingRect))
                    {
                        PressButton(0);
                        PulseOff(gameTime, 0, phonePulseDur);
                    }//End if

                    if (ScreenManager.InputSystem.IsRectanglePressed(this.Circles[1].BoundingRect))
                    {
                        PressButton(1);
                        PulseOff(gameTime, 1, phonePulseDur);
                    }//End if

                    if (ScreenManager.InputSystem.IsRectanglePressed(this.Circles[2].BoundingRect))
                    {
                        PressButton(2);
                        PulseOff(gameTime, 2, phonePulseDur);
                    }//End if
                    if (ScreenManager.InputSystem.IsRectanglePressed(this.Circles[3].BoundingRect))
                    {
                        PressButton(3);
                        PulseOff(gameTime, 3, phonePulseDur);
                    }//End if
                    #endregion
                    if (this.timerCounter <= 0d)
                    {
                        gameState = GameState.Fail;
                    }//End if
                    break;
                #endregion
                #region Switch Output
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
                        }//End if
                        this.timeRemaining = this.timeInterval;
                    }//End if
                    break;
                #endregion
                #region Switch Succeed
                case GameState.Succeed:
                    this.lvlCountDownTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (lvlCountDownTimer <= 0.0d)
                    {
                        level++;
                        if (gameState == GameState.Succeed)
                        {
                            totalPlayTime = (int)timer - (int)timerCounter;
                            totalPlayTimeCounter = totalPlayTimeCounter + totalPlayTime;
                            timerScore = (int)timerCounter * 10;
                            score = score + timerScore;
                            TimerControl();
                            timerCounter = timer;
                            SequenceGenerator(1);
                            lvlCountDownTimer = 1.5d;
                        }//End if
                        sequenceIndex = 0;
                        gameState = GameState.InputClean;
                    }//End if
                    break;
                #endregion
                #region Switch Fail
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
                        }//End if
                        sequenceIndex = 0;
                        gameState = GameState.InputClean;
                    }//End if
                    break;
                #endregion
                #region Switch GameOver
                case GameState.GameOver:
                    for (int i = 0; i < Circles.Length; i++)
                    {
                        if (Circles[i].Active)
                        {
                            Circles[i].Active = false;
                        }//End if
                    }//End for
                    this.lvlCountDownTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (lvlCountDownTimer <= 0.0d)
                    {

                        played = (int)totalPlayTimeCounter;
                        musicSound[audioTrackIndex].Stop();
                        ScreenManager.RemoveAd();
                        base.Remove();
                        //checkFile.isSaveable = true; // Temp to check out the HighScore system  
                        if (checkFile.isSaveable)
                          {
                              
                              ScreenManager.AddScreen(new CheckScoreScreen(data, score, level, played, saveGameFile, highScoreScreen, backgroundTexture[bgTexture],bgTexture));
                          }
                          else
                          {
                              ScreenManager.AddScreen(new ColorMimicMainMenu());
                          }//End if 
                        //ScreenManager.AddScreen(new ColorMimicMainMenu()); // Added temporarly to return to main menu at game over will be removed when Isystem is active.
                    }//End if

                    break;
                #endregion

            }
            #endregion

            #region Life loss control
            if (lives == 0)
            {
                gameState = GameState.GameOver;
            }//End if
            #endregion

            #region Back Button Code
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && !ScreenManager.isPressed)
            {
                ScreenManager.isPressed = true;
                musicSound[audioTrackIndex].Pause();
                ScreenState = ScreenState.Frozen;
                ScreenManager.AddScreen(new PauseScreen(this));
            }
            else if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && ScreenManager.isPressed)
            {
                ScreenManager.isPressed = false;
                ScreenState = ScreenState.Active;
                if (musicSound[audioTrackIndex].State == SoundState.Paused)
                {
                    musicSound[audioTrackIndex].Resume();
                }
            }
            #endregion
            base.Update(gameTime, IsExiting);

        }
        #endregion

        #region Draw Method
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
            string timerText = string.Format("{0}", timerDisplay);
            string lifeCounter = string.Format("{0}", lives);
            string checker = string.Format("{0}", level);

            Color textColor = Color.BlanchedAlmond;
            #endregion
            #region ScreenManager stuff
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            #endregion
            #region SpriteBatch
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
                pos.X = screenWidth / 2 - 75;
                pos.Y = screenHeight / 2 - 65; // Y = -25
                text = "   FAIL";
                textColor = Color.Red;
            }//End if
            else if (this.gameState == GameState.Succeed)
            {
                pos.X = screenWidth / 2 - 105;
                pos.Y = screenHeight / 2 - 65; // Y = -25
                text = "   SUCCESS";
                textColor = Color.CornflowerBlue;
            }//End if
            if (gameState == GameState.Ready)
            {
                spriteBatch.DrawString(largeFont, text, pos, textColor, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 1);
            }//End if
            spriteBatch.DrawString(largeFont, text, pos, textColor, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 1);
            #endregion

            #region Timer Display
            spriteBatch.DrawString(scoreFont, "Time", new Vector2(screenWidth / 2, 0), Color.White);
            spriteBatch.DrawString(scoreFont, timerText, new Vector2(screenWidth / 2 + 5, 20), Color.White);
            #endregion

            #region Lives
            spriteBatch.DrawString(scoreFont, "Lives", new Vector2(screenWidth / 2 + 335, 0), Color.White);
            spriteBatch.DrawString(scoreFont, lifeCounter, new Vector2(screenWidth / 2 + 350, 20), Color.White);
            #endregion

            #region Scroe drawing area
            spriteBatch.DrawString(scoreFont, scoreLabal, new Vector2(25, 0), Color.White);
            if (score == 0)
            {
                spriteBatch.DrawString(scoreFont, scoring, new Vector2(45, 20), Color.White);
            }//End if
            if (score > 0 && score < 1000)
            {
                spriteBatch.DrawString(scoreFont, scoring, new Vector2(30, 20), Color.White);
            }//End if
            else if (score >= 1000)
            {
                spriteBatch.DrawString(scoreFont, scoring, new Vector2(25, 20), Color.White);
            }//End if
            #endregion

            #region Player promts
            if (gameState == GameState.Ready)
            {
                if (playerCountDown >= 3.0)
                {
                    spriteBatch.DrawString(largeFont, start, new Vector2(screenWidth / 2 - 105, screenHeight / 2 - 65), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 1); // Y= -25
                }//End if
                if (playerCountDown >= 2.0 && playerCountDown <= 3.0)
                {
                    spriteBatch.DrawString(largeFont, set, new Vector2(screenWidth / 2 - 75, screenHeight / 2 - 65), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 1); // Y = -25
                }//End if
                if (playerCountDown >= 1.0 && playerCountDown <= 2.0)
                {
                    spriteBatch.DrawString(largeFont, set, new Vector2(screenWidth / 2 - 75, screenHeight / 2 - 65), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 1);
                    spriteBatch.DrawString(largeFont, go, new Vector2(screenWidth / 2 + 25, screenHeight / 2 - 65), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 1);
                }//End if
            }//End if

            if (gameState == GameState.GameOver)
            {
                spriteBatch.DrawString(largeFont, "Game Over", new Vector2(screenWidth / 2 - 99, screenHeight / 2 - 65), Color.White, 0, new Vector2(0, 0), 0.75f, SpriteEffects.None, 1);
            }//End if
            #endregion

            #region Turn Graphics
            spriteBatch.DrawString(scoreFont, "Turn", new Vector2(screenWidth / 2 - 200, 0), Color.White);
            if (gameState == GameState.Input || gameState == GameState.Succeed || gameState == GameState.Fail)
            {
                playerRing.Draw(gameTime, spriteBatch);
                spriteBatch.DrawString(scoreFont, "Player", new Vector2(screenWidth / 2 - 175, 29), Color.White);
            }//End if
            else if (gameState == GameState.Output || gameState == GameState.Ready)
            {
                aiRing.Draw(gameTime, spriteBatch);
                spriteBatch.DrawString(scoreFont, "AI", new Vector2(screenWidth / 2 - 175, 29), Color.White);
            }//End if
            #endregion
            spriteBatch.End();
            #endregion
        }
        #endregion

        #region SaveState Method
        protected void SaveState(object sender, DeactivatedEventArgs args)
        {
            // Primary varaibles to be saved.
            PhoneApplicationService.Current.State["Score"] = score;
            PhoneApplicationService.Current.State["Lives"] = lives;
            PhoneApplicationService.Current.State["Time"] = timer;
            PhoneApplicationService.Current.State["Level"] = level;
            PhoneApplicationService.Current.State["Prevlevel"] = prevLvl;
            PhoneApplicationService.Current.State["BgTexture"] = bgTexture;
            PhoneApplicationService.Current.State["TotalPlayTimeCounter"] = totalPlayTimeCounter;
            PhoneApplicationService.Current.State["TotalPlayTime"] = totalPlayTime;
            //PhoneApplicationService.Current.State["AudioTrack"] = audioTrackIndex;

            PhoneApplicationService.Current.State["ActiveGame"] = GameType.COLORMIMIC;
            // Include the music value in TSeffect
        }
        #endregion

        #region LoadLevel Method
        public void LoadLevel()
        {
            score = (int)PhoneApplicationService.Current.State["Score"];
            lives = (int)PhoneApplicationService.Current.State["Lives"];
            timer = (double)PhoneApplicationService.Current.State["Time"];
            level = (int)PhoneApplicationService.Current.State["Level"];
            prevLvl = (int)PhoneApplicationService.Current.State["Prevlevel"];
            bgTexture = (int)PhoneApplicationService.Current.State["BgTexture"];
            totalPlayTimeCounter = (double)PhoneApplicationService.Current.State["TotalPlayTimeCounter"];
            totalPlayTime = (double)PhoneApplicationService.Current.State["TotalPlayTime"];
            //audioTrackIndex = (int)PhoneApplicationService.Current.State["AudioTrack"];
        }
        #endregion
    }
    #endregion
}
#endregion