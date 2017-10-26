using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Play_As_You_Go
{
    public class Hud
    {
        private Vector2 scorePosition = new Vector2(450, 47);
        private Vector2 livesPosition = new Vector2(100, 47);
        private Vector2 levelPositon = new Vector2(100, 25);

        public SpriteFont Font { get; set; }

        public int Score = 0;
        public int HealthLife;
        public int Level = 0;

        public Hud()
        {

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(Font, "Score: " + Score.ToString(), scorePosition, Color.Yellow);
            spriteBatch.DrawString(Font, "Life: ", livesPosition, Color.Yellow);
            spriteBatch.DrawString(Font, "Level: " + Level.ToString(), levelPositon, Color.Yellow);
        }
    }
}
