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
    class BSMainMenu : MenuScreen
    {
        Texture2D backgroundTexture;
        SpriteFont TitleFont;

        Vector2 backgroundPosition;
        Vector2 titlePosition;

        SoundEffect music;
        SoundEffectInstance iMusic;

        ISsystem isolatedStorage = new ISsystem();

        string title;

        float transitionTime;

        double vidCountDown = 15.0d;

        MenuEntry Play, BackStory, Info, HighScore, Quit;

        public BSMainMenu()
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            SelectedColor = Color.White;
            NonSelectedColor = Color.Green;
            isolatedStorage.checkFile();

        }

        public override void Initialize()
        {
            backgroundPosition = new Vector2(60, 0);
            titlePosition = new Vector2(450, 80);

            title = "Beyond Space";

            Play = new MenuEntry(this, "Play Game");
            Play.SetPosition(new Vector2(540, 490), true);
            Play.Selected += PlayGameSelect;
            MenuEntries.Add(Play);

            BackStory = new MenuEntry(this, "Back Story");
            BackStory.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), Play, true);
            BackStory.Selected += BackStorySelect;
            MenuEntries.Add(BackStory);

            Info = new MenuEntry(this, "Controls");
            Info.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), BackStory, true);
            Info.Selected += InfoSelect;
            MenuEntries.Add(Info);

            if (!isolatedStorage.isSaveable)
            {
                HighScore = new MenuEntry(this, "Highscore");
                HighScore.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), Info, true);
                HighScore.Selected += HighscoreSelect;

            }
            else
            {
                HighScore = new MenuEntry(this, "Highscore");
                HighScore.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), Info, true);
                HighScore.Selected += HighscoreSelect;
                MenuEntries.Add(HighScore);
            }

            Quit = new MenuEntry(this, "Quit Game");
            Quit.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), HighScore, true);
            Quit.Selected += QuitSelect;
            MenuEntries.Add(Quit);
            iMusic = music.CreateInstance();
            iMusic.Volume = 0.1f;
            iMusic.IsLooped = true;
            iMusic.Play();
        }


        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            SpriteFont = content.Load<SpriteFont>("Fonts/BeyondSpaceFont/MenuFont");
            TitleFont = content.Load<SpriteFont>("Fonts/BeyondSpaceFont/TitleFont");
            music = content.Load<SoundEffect>("Audio/BSAudio/outer10");
            backgroundTexture = content.Load<Texture2D>("Backgrounds/BSBackgrounds/0001");
        }

        public override void UnloadContent()
        {
            SpriteFont = null;
            TitleFont = null;
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (MediaPlayer.GameHasControl)
                iMusic.Play();

            else
                iMusic.Pause();
            
            this.vidCountDown -= gameTime.ElapsedGameTime.TotalSeconds;

            if (vidCountDown <= 0.0d)
            {
                iMusic.Stop();
                Remove();
                ScreenManager.AddScreen(new DemoVid(1), ControllingPlayer);

            }

            base.Update(gameTime, covered);

        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);

            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.Green);

            if (!isolatedStorage.isSaveable)
                spriteBatch.DrawString(SpriteFont, "High Score", new Vector2(540, (SpriteFont.LineSpacing + 205) + 360), Color.Gray);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void PlayGameSelect(object sender, PlayerIndexEventArgs e)
        {
            iMusic.Stop();
            Remove();
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new BSLevel1(0, 3, false, 0));
            //LoadingScreen.Load(ScreenManager, true, new BSLevel20(0, 3, false, 3, 2000, 3000, 2000, 3000));
        }

        void BackStorySelect(object sender, PlayerIndexEventArgs e)
        {
            iMusic.Stop();
            Remove();
            ScreenManager.AddScreen(new BSBackStory(), e.PlayerIndex);
        }

        void InfoSelect(object sender, PlayerIndexEventArgs e)
        {
            iMusic.Stop();
            Remove();
            ScreenManager.AddScreen(new BSInfo(), e.PlayerIndex);
        }

        void HighscoreSelect(object sender, PlayerIndexEventArgs e)
        {
            iMusic.Stop();
            Remove();
            ScreenManager.AddScreen(new BSHighScore(), e.PlayerIndex);
        }

        void QuitSelect(object sender, PlayerIndexEventArgs e)
        {
            iMusic.Stop();
            Remove();
            ScreenManager.AddScreen(new PlayAsYouGoMainMenu(), e.PlayerIndex);
        }

        void MenuRemove(object sender, PlayerIndexEventArgs e)
        {
            MenuEntries.Clear();
        }
    }
}
