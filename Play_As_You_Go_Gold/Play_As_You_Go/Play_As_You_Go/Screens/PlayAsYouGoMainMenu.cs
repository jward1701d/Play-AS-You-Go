using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;



namespace Play_As_You_Go
{
    public class PlayAsYouGoMainMenu : MenuScreen
    {
        Texture2D backgroundTexture;
        SpriteFont TitleFont;

        Vector2 backgroundPosition;
        Vector2 titlePosition;

        string title;

        float transitionTime;
        ISsystem isSystem = new ISsystem();

        MenuEntry beyondSpace, colorMimic, H1N1, topScore, credits, quit;

        SoundEffect sound;
        SoundEffectInstance music;

        float volume = 0.15f;
        double vidCountDown = 15.0d;
        bool isSAveEnabled;
        ISsystem checkFile = new ISsystem();

        public PlayAsYouGoMainMenu()
        {
            checkFile.checkFile();
            isSAveEnabled = checkFile.isSaveable;
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            SelectedColor = Color.Firebrick;
            NonSelectedColor = Color.White;

        }
      
        public override void Initialize()
        {
            backgroundPosition = new Vector2(0, 0);
            titlePosition = new Vector2(275, 80);

            title = "Play As You Go";

            beyondSpace = new MenuEntry(this, "BeyondSpace");
            beyondSpace.SetPosition(new Vector2(550, 340), true);
            beyondSpace.Selected += BeyondSpaceSelect;
            MenuEntries.Add(beyondSpace);

            colorMimic = new MenuEntry(this, "ColorMimic");
            colorMimic.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), beyondSpace, true);
            colorMimic.Selected += ColorMimicSelected;
            MenuEntries.Add(colorMimic);

            if (!isSAveEnabled)
            {
                H1N1 = new MenuEntry(this, "H1N1");
                H1N1.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), colorMimic, true);
                H1N1.Selected += H1N1Selected;
                //MenuEntries.Add(H1N1);


                topScore = new MenuEntry(this, "Top Scores");
                topScore.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), H1N1, true);
                topScore.Selected += TopScoreSelected;
                // MenuEntries.Add(topScore);
            }
            else
            {
                H1N1 = new MenuEntry(this, "H1N1");
                H1N1.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), colorMimic, true);
                H1N1.Selected += H1N1Selected;
                MenuEntries.Add(H1N1);


                topScore = new MenuEntry(this, "Top Scores");
                topScore.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), H1N1, true);
                topScore.Selected += TopScoreSelected;
                MenuEntries.Add(topScore);
            }

            credits = new MenuEntry(this, "Credits");
            credits.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), topScore, true);
            credits.Selected += creditsSelect;
            MenuEntries.Add(credits);

            quit = new MenuEntry(this, "Quit Game");
            quit.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), credits, true);
            quit.Selected += QuitSelect;
            MenuEntries.Add(quit);

            music = sound.CreateInstance();
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            SpriteFont = content.Load<SpriteFont>("Fonts/SmallFont");
            TitleFont = content.Load<SpriteFont>("Fonts/BigBoldFont");

            backgroundTexture = content.Load<Texture2D>("Backgrounds/PlayAsYouGoMainMenu");
            sound = content.Load<SoundEffect>("Audio/cold2");
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (MediaPlayer.GameHasControl)
            {
                music.Volume = volume;
                music.Play();
            }
            else
                music.Pause();

            this.vidCountDown -= gameTime.ElapsedGameTime.TotalSeconds;

            if (vidCountDown <= 0.0d)
            {
                music.Stop();
                Remove();
                ScreenManager.AddScreen(new DemoVid(0), ControllingPlayer);
                
            }


            base.Update(gameTime, covered);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;            

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);

            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.Black);
            if (!isSAveEnabled)
            {
                spriteBatch.DrawString(SpriteFont, "H1N1", new Vector2(550, ((SpriteFont.LineSpacing + 45) + 340)), Color.Gray);
                spriteBatch.DrawString(SpriteFont, "Top Scores", new Vector2(550, ((SpriteFont.LineSpacing + 80) + 340)), Color.Gray);
            }
            spriteBatch.DrawString(SpriteFont, "Play As You Go Version 1.1.0 Copyright 2011 Scrubby Fresh Studios LLC", new Vector2(45, 670), Color.White, 0f, new Vector2(0, 0), 0.70f, SpriteEffects.None, 1.0f); 
            spriteBatch.End();

            base.Draw(gameTime);
        }

        void BeyondSpaceSelect(object sender, PlayerIndexEventArgs e)
        {
            music.Stop();
            Remove();
            ScreenManager.AddScreen(new BSMainMenu(), e.PlayerIndex);
        }

        void ColorMimicSelected(object sender, PlayerIndexEventArgs e)
        {
            music.Stop();
            Remove();
            ScreenManager.AddScreen(new ColorMimicMainMenu(), e.PlayerIndex);
        }

        void H1N1Selected(object sender, PlayerIndexEventArgs e)
        {
            music.Stop();
            Remove();
            ScreenManager.AddScreen(new H1N1MenuScreen(), e.PlayerIndex);
        }

        void TopScoreSelected(object sender, PlayerIndexEventArgs e)
        {
            music.Stop();
            Remove();
            ScreenManager.AddScreen(new TopScoresScreen(), e.PlayerIndex);
        }

        void creditsSelect(object sender, PlayerIndexEventArgs e)
        {
            music.Stop();
            Remove();
            ScreenManager.AddScreen(new creditScreen(), e.PlayerIndex);
        }

        void QuitSelect(object sender, PlayerIndexEventArgs e)
        {
            TransitionOffTime = TimeSpan.FromSeconds(transitionTime);
            Remove();
        }

       /* void DemoVidScreen(object sender, PlayerIndexEventArgs e)
        {
            music.Stop();
            Remove();
            //ScreenManager.AddScreen(new DemoVid(0), e.PlayerIndex);
        }*/
        void MenuRemove(object sender, PlayerIndexEventArgs e)
        {
            MenuEntries.Clear();
        }
    }
}
