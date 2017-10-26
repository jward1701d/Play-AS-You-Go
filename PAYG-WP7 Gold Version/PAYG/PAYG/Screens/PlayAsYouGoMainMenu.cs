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

using Microsoft.Phone.Shell;
using System.Collections;
using System.Runtime.Serialization; //needed for data contracts
using System.Xml;

namespace PAYG
{
    public class PlayAsYouGoMainMenu : MenuScreen
    {
        Texture2D backgroundTexture;
        SpriteFont TitleFont;

        /*Texture2D test; Transparency example */

        GameObject background;
        Vector2 titlePosition;

        string title;

        float transitionTime;
        ISsystem isSystem = new ISsystem();

        MenuEntry beyondSpace, colorMimic, H1N1, topScore, options, credits, quit;

        SoundEffect sound;
        SoundEffectInstance music;

        bool isSAveEnabled;
        ISsystem checkFile = new ISsystem();

        public PlayAsYouGoMainMenu()
        {
            checkFile.checkFile();
            isSAveEnabled = checkFile.isSaveable;
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            Selected = Color.White;
            NonSelected = Color.White;

            Removed += new EventHandler(MenuRemove);

        }
      
        public override void Initialize()
        {
            PhoneApplicationService.Current.Deactivated += new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(SaveState);

            background = new GameObject(backgroundTexture, new Rectangle(0, 0, 1280, 720), 1, true, 0);
            background.Position = new Vector2(0, 0);
            background.Scale = 0.67f;
    
            titlePosition = new Vector2(30,20);

            title = "Play As You Go";

            beyondSpace = new MenuEntry(this, "BeyondSpace");
            beyondSpace.SetPosition(new Vector2(350, 200), true);
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

            options = new MenuEntry(this, "Options");
            options.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), topScore, true);
            options.Selected += OptionsSelect;
            MenuEntries.Add(options);

            credits = new MenuEntry(this, "Credits");
            credits.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), options, true);
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
            /*test = content.Load<Texture2D>("test"); transparency example */
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (MediaPlayer.GameHasControl)
            {
                music.Volume = ScreenManager.MusicVolume;
                music.Play();
            }
            else
                music.Pause();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                // TO DO: Add something here to help handle the back button.
                ScreenManager.Game.Exit();
                // Remove(); // Going to fix a way to detect if last screen was option menu.
            }
            base.Update(gameTime, covered);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;            

            spriteBatch.Begin();

            background.Draw(gameTime, spriteBatch);

            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.Black);

            if (!isSAveEnabled)
            {
                spriteBatch.DrawString(SpriteFont, "H1N1", new Vector2(350, ((SpriteFont.LineSpacing + 45) + 200)), Color.Gray);
                spriteBatch.DrawString(SpriteFont, "Top Scores", new Vector2(350, ((SpriteFont.LineSpacing + 80) + 200)), Color.Gray);
            }

            /*spriteBatch.Draw(test, new Rectangle(240, 400, 99, 99), Color.White * 0.5f); Transparency example */

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void BeyondSpaceSelect(object sender, EventArgs e)
        {
            music.Stop();
            Remove();
            ScreenManager.AddScreen(new BSMainMenu());
        }

        void ColorMimicSelected(object sender, EventArgs e)
        {
            music.Stop();
            Remove();
            ScreenManager.AddScreen(new ColorMimicMainMenu());
        }

        void H1N1Selected(object sender, EventArgs e)
        {
            music.Stop();
            Remove();
            ScreenManager.AddScreen(new H1N1MenuScreen());
        }

        void TopScoreSelected(object sender, EventArgs e)
        {
            music.Stop();
            Remove();
            ScreenManager.AddScreen(new TopScoresScreen());
        }

        void OptionsSelect(object sender, EventArgs e)
        {
            music.Stop();
            Remove();
            ScreenManager.AddScreen(new OptionsMenu(0));
            
        }

        void creditsSelect(object sender, EventArgs e)
        {
            music.Stop();
            Remove();
            ScreenManager.AddScreen(new creditScreen());
        }

        void QuitSelect(object sender, EventArgs e)
        {
            TransitionOffTime = TimeSpan.FromSeconds(transitionTime);
            Remove();
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
