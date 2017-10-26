#region Using

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

#endregion

namespace Play_As_You_Go
{
    class FirstRunScreen : GameScreen
    {
        Texture2D backgroundTexture;
        Texture2D messageBox;
        SpriteFont TitleFont;
        SpriteFont spriteFont;
        SpriteFont controllerFont;

        Vector2 backgroundPosition;
        Vector2 titlePosition;
        Vector2 msgBoxPosition;

        Vector2 aButtonPos = new Vector2(383, 435);
        Vector2 bButtonPos = new Vector2(383, 483);

        ISsystem saveFeature = new ISsystem();
        
        float transitionTime;

        string title;
        string message;
        string controls;
        string aButton;
        string bButton;
        //bool savingAllowed;

        public FirstRunScreen()
        {
            backgroundPosition = new Vector2(0, 0);
            titlePosition = new Vector2(275, 80);
            msgBoxPosition = new Vector2(100, 50);

            transitionTime = 1.0f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;
        }
        public override void Initialize()
        {
            title = "Play As You Go";
            aButton = "'";
            bButton = ")";
            message = "Some of this games feature require saving extra \n" +
                       "data to your system. Some features maybe \n" +
                       "unavailable if saving is not permitted. Will\n" +
                       "you allow the data to be saved to your system? ";
            controls = "Press      " +"to allow saving\n\n" + "Press      " + "to continue without saving.";

        }
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            spriteFont = content.Load<SpriteFont>("Fonts/SmallFont");
            backgroundTexture = content.Load<Texture2D>("Backgrounds/PlayAsYouGoMainMenu");
            messageBox = content.Load<Texture2D>("Images/MessageBox");
            TitleFont = content.Load<SpriteFont>("Fonts/BigBoldFont");
            controllerFont = content.Load<SpriteFont>("Fonts/xboxControllerSpriteFont");
        }
       
        public override void Update(GameTime gameTime, bool covered)
        {
            int playerIndex = (int)ControllingPlayer.Value;
            PlayerIndex pIndex = new PlayerIndex();
            pIndex = ControllingPlayer.Value;

            KeyboardState keyboardState = ScreenManager.InputSystem.currentKeyboardState[playerIndex];
            GamePadState gamepadState = ScreenManager.InputSystem.currentGamePadState[playerIndex];
            KeyboardState previousKeyboardState = ScreenManager.InputSystem.previousKeyboardState[playerIndex]; 
            GamePadState previousGamepadState = ScreenManager.InputSystem.previousGamePadState[playerIndex];

            bool gamePadDisconnected = !gamepadState.IsConnected && ScreenManager.InputSystem.gamePadWasConnected[playerIndex];

            
            if (previousGamepadState.IsButtonUp(Buttons.A) && gamepadState.IsButtonDown(Buttons.A) || previousKeyboardState.IsKeyUp(Keys.Enter) && keyboardState.IsKeyDown(Keys.Enter))
            {
                //savingAllowed = true;
                ISsystem.LoadHighScores("CMHS.sfs", 5);
                Remove();
                ScreenManager.AddScreen(new PlayAsYouGoMainMenu(), ControllingPlayer);
            }

            if (previousGamepadState.IsButtonUp(Buttons.B) && gamepadState.IsButtonDown(Buttons.B) || previousKeyboardState.IsKeyUp(Keys.Escape) && keyboardState.IsKeyDown(Keys.Escape))
            {
                //savingAllowed = false;
                Remove();
                ScreenManager.AddScreen(new PlayAsYouGoMainMenu() , ControllingPlayer);
            }

            
            
            
            if (MediaPlayer.GameHasControl)
            {
                //music.Volume = volume;
                //music.Play();
            }
            else
            {
                //music.Pause();
            }
            

            base.Update(gameTime, covered);
        }
        
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.Gray);
            spriteBatch.Draw(messageBox, msgBoxPosition, Color.White);
            spriteBatch.DrawString(spriteFont, message, new Vector2(310, 200), Color.White);
            spriteBatch.DrawString(spriteFont, controls, new Vector2(310, 470), Color.White);
            spriteBatch.DrawString(controllerFont, aButton, aButtonPos, Color.White, 0f, new Vector2( 0,0), 0.50f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(controllerFont, bButton, bButtonPos, Color.White, 0f, new Vector2(0, 0), 0.50f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(spriteFont, "Play As You Go Version 1.1.0 Copyright 2011 Scrubby Fresh Studios LLC", new Vector2(45, 670), Color.White, 0f, new Vector2(0, 0), 0.70f, SpriteEffects.None, 1.0f); 
            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.Black);
            spriteBatch.End();
        }
    }
}

