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
    class BSBackStory : MenuScreen
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
        MenuEntry next;

        public BSBackStory()
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
            infoPosition = new Vector2(5, 100);

            title = "Back Story";
            info = "The Galactic Alliance has sent an envoy to Zindarie Prime for peace talks \n\n" +
                   "and to renew a one hundred year peace treaty.  As the GDF ship Thantos and \n\n" +
                   "its escort fleet approach Zindarie Prime they are attacked without warning \n\n" +
                   "by the Zindarie fleet.  The order comes out for all defense fighters to \n\n " +
                   "be scrambled and to defend the Thantos.  After a long fight, the order \n\n" +
                   "comes from the crippled Thantos for all remaining fighters to make a run \n\n" +
                   "for it. ";


            /*back = new MenuEntry(this, "Back");
            back.SetPosition(new Vector2(380, 420), true);
            back.Selected += new EventHandler(BackSelect);
            MenuEntries.Add(back);

            next = new MenuEntry(this, "Next");
            next.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), back, true);
            next.Selected += new EventHandler(NextSelect);
            MenuEntries.Add(next);*/

            next = new MenuEntry(this, "Next");
            next.SetPosition(new Vector2(380, 420), true);
            next.Selected += new EventHandler(NextSelect);
            MenuEntries.Add(next);

            back = new MenuEntry(this, "Back");
            back.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 10), next, true);
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
                ScreenManager.AddScreen(new BSMainMenu());
            }
            base.Update(gameTime, covered);
        }



        void BackSelect(object sender, EventArgs e)
        {
            iMusic.Stop();
            Remove();
            ScreenManager.AddScreen(new BSMainMenu());
        }

        void NextSelect(object sender, EventArgs e)
        {
            iMusic.Stop();
            Remove();
            ScreenManager.AddScreen(new BackStory2());
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