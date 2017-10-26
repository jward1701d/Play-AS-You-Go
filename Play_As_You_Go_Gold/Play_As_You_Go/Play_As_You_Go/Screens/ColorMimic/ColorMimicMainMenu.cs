using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Play_As_You_Go
{
    class ColorMimicMainMenu : MenuScreen
    {

        //SoundEffect sound;
        //SoundEffectInstance music;
        Song song;
        SpriteFont TitleFont;
        Texture2D backgroundTexture;
        Vector2 backgroundPosition;
        Vector2 titlePosition;
        ISsystem checkFile = new ISsystem();

        string title;

        float transitionTime;
        double vidCountDown = 15.0d;

        MenuEntry play, controls, highScore, quit;

        public ColorMimicMainMenu()
        {
           
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            SelectedColor = Color.CornflowerBlue;
            NonSelectedColor = Color.White;
            checkFile.checkFile();
            
        }

        public override void Initialize()
        {
            base.Initialize();

            backgroundPosition = new Vector2(0, 0);
            titlePosition = new Vector2((ScreenManager.Viewport.Width / 2)-250, 100);

            title = "Color Mimic";
            play = new MenuEntry(this, "Play");
            play.SetPosition(new Vector2(550, 500), true);
            play.Selected += PlaySelect;
            MenuEntries.Add(play);

            controls = new MenuEntry(this, "Controls");
            controls.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), play, true);
            controls.Selected += ControlsSelected;
            MenuEntries.Add(controls);

            if (!checkFile.isSaveable)
            {
                highScore = new MenuEntry(this, "High Score");
                highScore.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), controls, true);
                highScore.Selected += HighScoreSelected;
                //MenuEntries.Add(highScore);
            }
            else
            {

                highScore = new MenuEntry(this, "High Score");
                highScore.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), controls, true);
                highScore.Selected += HighScoreSelected;
                MenuEntries.Add(highScore);
            }
            quit = new MenuEntry(this, "Quit Game");
            quit.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), highScore, true);
            quit.Selected += QuitSelect;
            MenuEntries.Add(quit);

            
           // music = sound.CreateInstance();
            MediaPlayer.Play(song);
            MediaPlayer.Volume = 0.25f;
            MediaPlayer.IsRepeating = true;
        }


        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            SpriteFont = content.Load<SpriteFont>("Fonts/Color Mimic Fonts/CMsmallFont");
            TitleFont = content.Load<SpriteFont>("Fonts/Color Mimic Fonts/CMTitle");

            song = content.Load<Song>("Audio/ZachJulinFlyAway");
            backgroundTexture = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Colorful");
        }

        public override void UnloadContent()
        {
            SpriteFont = null;
            TitleFont = null;
        }


        public override void Update(GameTime gameTime, bool covered)
        {
            this.vidCountDown -= gameTime.ElapsedGameTime.TotalSeconds;

            if (vidCountDown <= 0.0d)
            {
                MediaPlayer.Stop();
                Remove();
                ScreenManager.AddScreen(new DemoVid(2), ControllingPlayer);

            }
            base.Update(gameTime, covered);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);
            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.White);
            if (!checkFile.isSaveable)
            {
                spriteBatch.DrawString(SpriteFont, "High Score", new Vector2(550, (500 + (SpriteFont.LineSpacing + 45))), Color.Gray);
            }

            spriteBatch.End();

           
            base.Draw(gameTime);
        }

        public override void Remove()
        {
            base.Remove();

            MenuEntries.Clear();

        }

        void PlaySelect(object sender, PlayerIndexEventArgs e)
          {
              Remove();
              //music.Stop();
              MediaPlayer.Stop();
              LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new ColorMimicGame());
          }

        void ControlsSelected(object sender, PlayerIndexEventArgs e)
        {
            Remove();
            //music.Stop();
            MediaPlayer.Stop();
            ScreenManager.AddScreen(new ColorMimicInfo(), e.PlayerIndex);
        }

        void HighScoreSelected(object sender, PlayerIndexEventArgs e)
        {
            Remove();
            //music.Stop();
            MediaPlayer.Stop();
            ScreenManager.AddScreen(new ColorMimicHighScore(backgroundTexture), e.PlayerIndex);
        }

        void QuitSelect(object sender, PlayerIndexEventArgs e)
        {
            Remove();
            //music.Stop();
            MediaPlayer.Stop();
            ScreenManager.AddScreen(new PlayAsYouGoMainMenu(), e.PlayerIndex);
        }


        void MenuRemove(object sender, PlayerIndexEventArgs e)
        {
            MenuEntries.Clear();
        }
    }
}
