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
    class BackStory2 : MenuScreen
    {
        Texture2D backgroundTexture;
        SpriteFont TitleFont;

        SoundEffect music;
        SoundEffectInstance iMusic;

        Vector2 backgroundPosition;
        Vector2 titlePosition;
        Vector2 infoPosition;

        string title;
        string info;

        float transitionTime;

        MenuEntry back;
   

        public BackStory2()
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
            infoPosition = new Vector2(10, 100);

            title = "Back Story";
            info = "Your mission is to stay alive, and report what has happened here \n\n" +
                   "to the Galactic Alliance.  So they may have some warning as to what's \n\n" +
                   "about to happen.  War has been declared and it's up to you to make sure \n\n" +
                   "that the GalacticAlliance is ready to defend its self and its allies.  That \n\n" +
                   "is if you can make it home alive";


            back = new MenuEntry(this, "Back");
            back.SetPosition(new Vector2(380, 460), true);
            back.Selected += new EventHandler(BackSelect);
            MenuEntries.Add(back);

            iMusic = music.CreateInstance();
            iMusic.Volume = ScreenManager.MusicVolume;
            iMusic.IsLooped = true;
            iMusic.Play();
        }


        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            SpriteFont = content.Load<SpriteFont>("Fonts/BeyondSpaceFont/MenuFont");
            TitleFont = content.Load<SpriteFont>("Fonts/BeyondSpaceFont/TitleFont");
            backgroundTexture = content.Load<Texture2D>("Backgrounds/BSBackgrounds/0001");
            music = content.Load<SoundEffect>("Audio/BSAudio/outer10");
        }

        public override void UnloadContent()
        {
            SpriteFont = null;
            TitleFont = null;
            backgroundTexture = null;
            music = null;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.GraphicsDevice.Clear(Color.Black);

            //spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.Gray);
            spriteBatch.Draw(backgroundTexture, backgroundPosition, null, Color.Gray, 0f, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0f);

            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.Green);
            spriteBatch.DrawString(SpriteFont, info, infoPosition, Color.Green);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                iMusic.Stop();
                Remove();
                ScreenManager.AddScreen(new BSBackStory());
            }
            base.Update(gameTime, covered);
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
   
