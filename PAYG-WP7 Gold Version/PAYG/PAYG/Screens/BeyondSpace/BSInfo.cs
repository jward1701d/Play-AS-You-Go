using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Phone.Shell;
using System.Collections;
using System.Runtime.Serialization; //needed for data contracts
using System.Xml;

namespace PAYG
{
    class BSInfo : MenuScreen
    {
        SpriteFont TitleFont;

        string controls;

        SoundEffect music;
        SoundEffectInstance iMusic;

        Vector2 backgroundPosition;
        Vector2 titlePosition;
        Vector2 infoPosition;

        string title;
        string info;

        float transitionTime;

        MenuEntry back;

        public BSInfo()
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            Selected = Color.Green;
            NonSelected = Color.Green;

            Removed += new EventHandler(MainMenuRemove);
        }

        public override void Initialize()
        {
            PhoneApplicationService.Current.Deactivated += new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(SaveState);
            backgroundPosition = new Vector2(60, 40);
            titlePosition = new Vector2(270, 20);
            infoPosition = new Vector2(300, 100);

            title = "Controls";
            info = "";

            back = new MenuEntry(this, "Back");
            back.SetPosition(new Vector2(340, 420), true);
            back.Selected += new EventHandler(BackSelect);
            MenuEntries.Add(back);

            iMusic = music.CreateInstance();
            iMusic.Volume = ScreenManager.MusicVolume;
            iMusic.IsLooped = true;
            iMusic.Play();

            controls = "Tilt forward\\backward:     move forward\\backward\n" +
                       "Double Tap:                                launch\\detonate missle\n" +
                       "Hold:                                             Pause";
        }


        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            SpriteFont = content.Load<SpriteFont>("Fonts/BeyondSpaceFont/MenuFont");
            TitleFont = content.Load<SpriteFont>("Fonts/BeyondSpaceFont/TitleFont");
            music = content.Load<SoundEffect>("Audio/BSAudio/outer10");

        }

        public override void UnloadContent()
        {
            SpriteFont = null;
            TitleFont = null;
            music = null;
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            iMusic.Volume = ScreenManager.MusicVolume;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                iMusic.Stop();
                Remove();
                ScreenManager.AddScreen(new BSMainMenu());
            }

            base.Update(gameTime, covered);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.GraphicsDevice.Clear(Color.Black);
            
            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.Green);
            spriteBatch.DrawString(SpriteFont, info, infoPosition, Color.Black);
            spriteBatch.DrawString(SpriteFont, controls, new Vector2(150, 200), Color.Green);

            spriteBatch.End();

            base.Draw(gameTime);
        }



        void BackSelect(object sender, EventArgs e)
        {
            iMusic.Stop();
            Remove();
            ScreenManager.AddScreen(new BSMainMenu());
        }

        void MainMenuRemove(object sender, EventArgs e)
        {
            iMusic.Stop();
            MenuEntries.Clear();
        }
        protected void SaveState(object sender, DeactivatedEventArgs args)
        {
            PhoneApplicationService.Current.State["ActiveGame"] = GameType.MENU;
        }

    }
}