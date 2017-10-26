using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;


namespace Play_As_You_Go
{
    class ColorMimicInfo : MenuScreen
    {
        Texture2D backgroundTexture;

        Vector2 backgroundPosition;

        float transitionTime;

        MenuEntry back;

        public ColorMimicInfo()
        {
            transitionTime = 1.5f;
            TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            TransitionOffTime = TimeSpan.Zero;

            SelectedColor = Color.CornflowerBlue;
            NonSelectedColor = Color.White;

           
        }

        public override void Initialize()
        {
            base.Initialize();

            backgroundPosition = new Vector2(0, 0);

            back = new MenuEntry(this, "Back");
            back.SetPosition(new Vector2(620, 600), true);
            back.Selected += BackSelect;
            MenuEntries.Add(back);

        }


        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            SpriteFont = content.Load<SpriteFont>("Fonts/SmallFont");
#if WINDOWS
            backgroundTexture = content.Load<Texture2D>("Images/ColorMimicImages/Color Mimic Controls PC Controls");
#else
            backgroundTexture = content.Load<Texture2D>("Images/ColorMimicImages/Color Mimic Controls Xbox Controls");
#endif
        }

        public override void UnloadContent()
        {
            SpriteFont = null;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void BackSelect(object sender, PlayerIndexEventArgs e)
        {
            Remove();
            ScreenManager.AddScreen(new ColorMimicMainMenu(), e.PlayerIndex);
        }

        void MenuRemove(object sender, PlayerIndexEventArgs e)
        {
            MenuEntries.Clear();
        }
    }
}
