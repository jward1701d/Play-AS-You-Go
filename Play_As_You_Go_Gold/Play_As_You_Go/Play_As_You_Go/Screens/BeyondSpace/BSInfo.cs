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
    class BSInfo : MenuScreen
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

        public BSInfo()
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
            infoPosition = new Vector2(300, 100);

            title = "Controls";
            info = "";

            back = new MenuEntry(this, "Back");
            back.SetPosition(new Vector2(600, 600), true);
            back.Selected += BackSelect;
            MenuEntries.Add(back);

            iMusic = music.CreateInstance();
            iMusic.Volume = 0.1f;
            iMusic.IsLooped = true;
            
        }


        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            SpriteFont = content.Load<SpriteFont>("Fonts/BeyondSpaceFont/MenuFont");
            TitleFont = content.Load<SpriteFont>("Fonts/BeyondSpaceFont/TitleFont");
            music = content.Load<SoundEffect>("Audio/BSAudio/outer10");
#if XBOX
            backgroundTexture = content.Load<Texture2D>("Backgrounds/BSBackgrounds/XboxControlsBackground");
#else
            backgroundTexture = content.Load<Texture2D>("Backgrounds/BSBackgrounds/keyboardControlsBackground");
#endif
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

            base.Update(gameTime, covered);

        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);

            spriteBatch.DrawString(TitleFont, title, titlePosition, Color.Green);
            spriteBatch.DrawString(SpriteFont, info, infoPosition, Color.Black);

            spriteBatch.End();

            base.Draw(gameTime);
        }



        void BackSelect(object sender, PlayerIndexEventArgs e)
        {
            iMusic.Stop();
            Remove();
            ScreenManager.AddScreen(new BSMainMenu(), e.PlayerIndex);
        }

        void MainMenuRemove(object sender, PlayerIndexEventArgs e)
        {
            iMusic.Stop();
            MenuEntries.Clear();
        }

    }
}