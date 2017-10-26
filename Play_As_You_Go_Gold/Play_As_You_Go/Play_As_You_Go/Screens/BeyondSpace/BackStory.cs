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

        public BSBackStory()
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            SelectedColor = Color.White;
            NonSelectedColor = Color.Green;
        }

        public override void Initialize()
        {
            backgroundPosition = new Vector2(0, -50);
            titlePosition = new Vector2(525, 30);
            infoPosition = new Vector2(150, 100);

            title = "Back Story";
            info = "The Galactic Alliance has sent an envoy to Zindarie Prime for peace talks and to renew a one  \n\n" +
                   "hundred year peace treaty.  As the GDF ship Thantos and its escort fleet approach Zindarie Prime  \n\n" +
                   "they are attacked without warning by the Zindarie fleet.  The order comes out for all defense   \n\n" +
                   "fighters to be scrambled and to defend the Thantos.  After a long fight, the order comes from the    \n\n" +
                   "crippled Thantos for all remaining fighters to make a run for it.  Your mission is to stay alive, and   \n\n" +
                   "report what has happened here to the Galactic Alliance.  So they may have some warning as to    \n\n" +
                   "what's about to happen.  War has been declared and it's up to you to make sure that the Galactic    \n\n" +
                   "Alliance is ready to defend its self and its allies.  That is if you can make it home alive";


            back = new MenuEntry(this, "Back");
            back.SetPosition(new Vector2(600, 600), true);
            back.Selected += BackSelect;
            MenuEntries.Add(back);

            iMusic = music.CreateInstance();
            iMusic.Volume = 0.1f;
            iMusic.IsLooped = true;
            
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (MediaPlayer.GameHasControl)
                iMusic.Play();

            else
                iMusic.Pause();

            base.Update(gameTime, covered);
            
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
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.Gray);

            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.Green);
            spriteBatch.DrawString(SpriteFont, info, infoPosition, Color.Green);

            spriteBatch.End();

            base.Draw(gameTime);
        }



        void BackSelect(object sender, PlayerIndexEventArgs e)
        {
            iMusic.Stop();
            Remove();
            ScreenManager.AddScreen(new BSMainMenu(),e.PlayerIndex);
        }

        void MainMenuRemove(object sender, PlayerIndexEventArgs e)
        {
            iMusic.Stop();
            MenuEntries.Clear();
        }

    }
}