using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Phone.Shell;
using System.Collections;
using System.Runtime.Serialization; //needed for data contracts
using System.Xml;

namespace PAYG
{
    class BSMainMenu : MenuScreen
    {
        Texture2D backgroundTexture;
        SpriteFont TitleFont;

        Vector2 backgroundPosition;
        Vector2 titlePosition;

        SoundEffect music;
        SoundEffectInstance iMusic;

        string title;

        float transitionTime;

        MenuEntry Play, BackStory, Info, HighScore, Options, Quit;
        ISsystem checkFile = new ISsystem();

        bool optionMenu = false;

        public BSMainMenu()
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            Selected = Color.Green;
            NonSelected = Color.Green;

            Removed += new EventHandler(MenuRemove);
        }

        public override void Initialize()
        {
            PhoneApplicationService.Current.Deactivated += new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(SaveState);
            checkFile.checkFile();
            backgroundPosition = new Vector2(60, 40);
            titlePosition = new Vector2(220, 80);

            title = "Beyond Space";

            Play = new MenuEntry(this, "Play Game");
            Play.SetPosition(new Vector2(320, 220), true);
            Play.Selected += new EventHandler(PlayGameSelect);
            MenuEntries.Add(Play);

            BackStory = new MenuEntry(this, "Back Story");
            BackStory.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), Play, true);
            BackStory.Selected += new EventHandler(BackStorySelect);
            MenuEntries.Add(BackStory);

            Info = new MenuEntry(this, "Controls");
            Info.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), BackStory, true);
            Info.Selected += new EventHandler(InfoSelect);
            MenuEntries.Add(Info);

            /*
            HighScore = new MenuEntry(this, "Highscore");
            HighScore.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), Info, true);
            HighScore.Selected += new EventHandler(HighscoreSelect);
            MenuEntries.Add(HighScore);
             * */
            if (!checkFile.isSaveable)
            {
                HighScore = new MenuEntry(this, "High Score");
                HighScore.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), Info, true);
                HighScore.Selected += new EventHandler(HighscoreSelect);
            }
            else
            {

                HighScore = new MenuEntry(this, "High Score");
                HighScore.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), Info, true);
                HighScore.Selected += new EventHandler(HighscoreSelect);
                MenuEntries.Add(HighScore);
            }

            Options = new MenuEntry(this, "Options");
            Options.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), HighScore, true);
            Options.Selected += new EventHandler(OptionsSelect);
            MenuEntries.Add(Options);

            Quit = new MenuEntry(this, "Quit Game");
            Quit.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), Options, true);
            Quit.Selected += new EventHandler(QuitSelect);
            MenuEntries.Add(Quit);

            iMusic = music.CreateInstance();
            iMusic.Volume = ScreenManager.MusicVolume;
            iMusic.IsLooped = true;
            iMusic.Play();
            TouchPanel.EnabledGestures = GestureType.Tap;

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
            backgroundTexture = null;
            music = null;
        }
        public override void Update(GameTime gameTime, bool covered)
        {
            iMusic.Volume = ScreenManager.MusicVolume;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && !optionMenu)
            {
                iMusic.Stop();
                Remove();
                ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
            }
            else if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && optionMenu)
                optionMenu = false;

            base.Update(gameTime, covered);
        }
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.GraphicsDevice.Clear(Color.Black);

            //spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);
            spriteBatch.Draw(backgroundTexture, backgroundPosition, null, Color.White, 0f, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0f);

            if(!checkFile.isSaveable)
                spriteBatch.DrawString(SpriteFont, "High Score", new Vector2(320, (250 + (SpriteFont.LineSpacing + 45))), Color.Gray);

            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.Green);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void PlayGameSelect(object sender, EventArgs e)
        {
            iMusic.Stop();
            Remove();
            LoadingScreen.Load(ScreenManager, true, new BSLevel1(0, 3, false, 0));
            //LoadingScreen.Load(ScreenManager, true, new BSLevel14(0, 3, false, 3, 2000, 3000, 2000, 3000));
        }

        void BackStorySelect(object sender, EventArgs e)
        {
            iMusic.Stop();
            Remove();
            ScreenManager.AddScreen(new BSBackStory());
        }

        void InfoSelect(object sender, EventArgs e)
        {
            iMusic.Stop();
            Remove();
            ScreenManager.AddScreen(new BSInfo());
        }

        void HighscoreSelect(object sender, EventArgs e)
        {
            iMusic.Stop();
            Remove();
            ScreenManager.AddScreen(new BSHighScore());
        }

        void OptionsSelect(object sender, EventArgs e)
        {
            iMusic.Stop();
            Remove();
            ScreenManager.AddScreen(new OptionsMenu(1));
        }

        void QuitSelect(object sender, EventArgs e)
        {
            iMusic.Stop();
            Remove();
            ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
        }

        void MenuRemove(object sender, EventArgs e)
        {
            MenuEntries.Clear();
        }
        protected void SaveState(object sender, DeactivatedEventArgs args)
        {
            PhoneApplicationService.Current.State["ActiveGame"] = GameType.MENU;
        }
    }
}
