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

using Microsoft.Phone.Shell;
using System.Collections;
using System.Runtime.Serialization; //needed for data contracts
using System.Xml;

namespace PAYG
{
    class ColorMimicMainMenu : MenuScreen
    {

        //SoundEffect sound;
        //SoundEffectInstance music;
        //Song song;
        SpriteFont TitleFont;
        Texture2D backgroundTexture;
        Vector2 backgroundPosition;
        Vector2 titlePosition;

        string title;

        int BgNum = 1;

        float transitionTime;

        MenuEntry play, options, highScore, quit;

        ISsystem checkFile = new ISsystem();

        bool optionMenu = false;
        

        public ColorMimicMainMenu()
        {
           
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            Selected = Color.White;
            NonSelected = Color.White;

            Removed += new EventHandler(MenuRemove);
        }

        public override void Initialize()
        {
            base.Initialize();
            checkFile.checkFile();
            PhoneApplicationService.Current.Deactivated += new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(SaveState);

            backgroundPosition = new Vector2(0, 0);
            titlePosition = new Vector2(200, 50);

            title = "Color Mimic";
            play = new MenuEntry(this, "Play");
            play.SetPosition(new Vector2(325, 240), true);
            play.Selected += new EventHandler(PlaySelect);
            MenuEntries.Add(play);

            options = new MenuEntry(this, "Options");
            options.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), play, true);
            options.Selected += new EventHandler(OptionsSelected);
            MenuEntries.Add(options);

            if (!checkFile.isSaveable)
            {
                highScore = new MenuEntry(this, "High Score");
                highScore.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), options, true);
                highScore.Selected += new EventHandler(HighScoreSelected);
                //MenuEntries.Add(highScore);
            }
            else
            {

                highScore = new MenuEntry(this, "High Score");
                highScore.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), options, true);
                highScore.Selected += new EventHandler(HighScoreSelected);
                MenuEntries.Add(highScore);
            }
            
            
            
            quit = new MenuEntry(this, "Quit Game");
            quit.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), highScore, true);
            quit.Selected += new EventHandler(QuitSelect);
            MenuEntries.Add(quit);

            
           // music = sound.CreateInstance();
           /* MediaPlayer.Play(song);
            MediaPlayer.Volume = 0.25f;
            MediaPlayer.IsRepeating = true;*/
        }


        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            SpriteFont = content.Load<SpriteFont>("Fonts/Color Mimic Fonts/CMsmallFont");
            TitleFont = content.Load<SpriteFont>("Fonts/Color Mimic Fonts/CMTitle");

            //song = content.Load<Song>("Audio/ZachJulinFlyAway");
            backgroundTexture = content.Load<Texture2D>("Backgrounds/Color Mimic Backgrounds/Colorful");
        }

        public override void UnloadContent()
        {
            //backgroundTexture = null;
            SpriteFont = null;
            TitleFont = null;
        }

        // Added to handle back button code needs.
        public override void Update(GameTime gameTime, bool covered)
        {
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && !optionMenu)
            {
                Remove();
                ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && optionMenu)
                optionMenu = false;
            base.Update(gameTime, covered);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);
            //spriteBatch.Draw(backgroundTexture, backgroundPosition, new Rectangle(0, 0, 1280, 720), Color.White, 0, 
               // new Vector2(backgroundTexture.Width / 2, backgroundTexture.Height / 2), 1.0f, SpriteEffects.None,  1); 
            spriteBatch.DrawString(TitleFont,title,titlePosition,Color.White,0,new Vector2(0,0),0.75f,SpriteEffects.None,1);

            if (!checkFile.isSaveable)
            {
                spriteBatch.DrawString(SpriteFont, "High Score", new Vector2(325, (240 + (SpriteFont.LineSpacing + 45))), Color.Gray);
            }

            spriteBatch.End();

           
            base.Draw(gameTime);
        }

        public override void Remove()
        {
            base.Remove();

            MenuEntries.Clear();

        }

        void PlaySelect(object sender, EventArgs e)
        {
            Remove();
            //music.Stop();
            //MediaPlayer.Stop();
            //LoadingScreen.Load(ScreenManager, true, new ColorMimicGame());
            //ScreenManager.Game.Exit();
            ScreenManager.AddScreen(new ColorMimicGame());
        }
        void HighScoreSelected(object sender, EventArgs e)
        {
            Remove();
            //music.Stop();
            //MediaPlayer.Stop();
            ScreenManager.AddScreen(new ColorMimicHighScore(backgroundTexture,BgNum));
            //ScreenManager.Game.Exit();
        }
        void OptionsSelected(object sender, EventArgs e)
        {
            //music.Stop();
            Remove();
            optionMenu = true;
            ScreenManager.AddScreen(new OptionsMenu(2));
        }
        void QuitSelect(object sender, EventArgs e)
        {
            Remove();
            //music.Stop();
            //MediaPlayer.Stop();
            ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
            //ScreenManager.Game.Exit();
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
