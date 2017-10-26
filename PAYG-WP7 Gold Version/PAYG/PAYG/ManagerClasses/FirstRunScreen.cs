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

namespace PAYG
{
    class FirstRunScreen : GameScreen
    {
        Texture2D backgroundTexture;
        
        SpriteFont TitleFont;
        SpriteFont spriteFont;
        Texture2D controllerFont;
        GameObject bButton;
        GameObject aButton;
        GameObject background;

        Vector2 backgroundPosition;
        Vector2 titlePosition;
        

        

        ISsystem saveFeature = new ISsystem();
        
        float transitionTime;

        string title;
        string message;
        string controls;
       
        //bool savingAllowed;

        public FirstRunScreen()
        {
            backgroundPosition = new Vector2(0, 0);
            titlePosition = new Vector2(30, 20);
            

            transitionTime = 1.0f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;
        }
        public override void Initialize()
        {
            int controlStartPosX = 193;
            
            float controlScale = 1.5f;

            background = new GameObject(backgroundTexture, new Rectangle(0, 0, 1280, 720), 1, true, 0);
            background.Position = new Vector2(0, 0);
            background.Scale = 0.67f;
            
            title = "Play As You Go";
           
            message = "Some of this games feature require saving extra \n" +
                       "data to your phone. Some features maybe \n" +
                       "unavailable if saving is not permitted. Will\n" +
                       "you allow the data to be saved to your phone? ";
            controls = "Press      " +"to allow saving\n\n" + "Press      " + "to continue without saving.";

            bButton = new GameObject(controllerFont, new Rectangle(158, 0, 32, 32), 1, true, 0);
            bButton.Position = new Vector2(controlStartPosX, 385);
            bButton.Origin = new Vector2((bButton.ActualWidth / 2) / bButton.Scale, (bButton.ActualHeight / 2) / bButton.Scale);
            bButton.Scale = controlScale;


            aButton = new GameObject(controllerFont, new Rectangle(94, 0, 32, 32), 1, true, 0);
            aButton.Position = new Vector2(controlStartPosX, 335);
            aButton.Origin = new Vector2((aButton.ActualWidth / 2) / aButton.Scale, (aButton.ActualHeight / 2) / aButton.Scale);
            aButton.Scale = controlScale;


        }
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            spriteFont = content.Load<SpriteFont>("Fonts/SmallFont");
            backgroundTexture = content.Load<Texture2D>("Backgrounds/PlayAsYouGoMainMenu");
           
            TitleFont = content.Load<SpriteFont>("Fonts/BigBoldFont");
            controllerFont = content.Load<Texture2D>("Images/HighScoreInterfaceButtonsSpriteSheet");
        }
       
        public override void Update(GameTime gameTime, bool covered)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                //ScreenManager.Game.Exit();
                Remove();
                ScreenManager.AddScreen(new PreMenu());
            }
            
            if (ScreenManager.InputSystem.IsRectanglePressed(this.aButton.BoundingRect))
            {
                //savingAllowed = true;
                ISsystem.LoadHighScores("CMHS.sfs", 5);
                Remove();
                ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
            }

            if (ScreenManager.InputSystem.IsRectanglePressed(this.bButton.BoundingRect))
            {
                //savingAllowed = false;
                Remove();
                ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
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
            background.Color = Color.Gray;
            spriteBatch.Begin();
            //spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.Gray);
            background.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(spriteFont, message, new Vector2(75, 200), Color.White); // X = 310
            spriteBatch.DrawString(spriteFont, controls, new Vector2(100, 325), Color.White); // X = 310
            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.Black);

            bButton.Draw(gameTime, spriteBatch);
            aButton.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }
    }
}

