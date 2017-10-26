using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;

namespace PAYG
{
    public class Hud
    {
        private float FPS = 0f, TotalTime = 0f, DisplayFPS = 0f;


        private Vector2 scorePosition = new Vector2(150, 03);       
        private Vector2 livesPosition = new Vector2(25,27);
        private Vector2 levelPositon = new Vector2(25, 03);
        Vector2 FPSPos = new Vector2(600, 27);

        public SpriteFont Font { get; set; }

        public int Score;
        public int Life;
        public int Level;

        public Hud()
        {
            
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Calculate the Frames Per Second       
            float ElapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TotalTime += ElapsedTime;
            if (TotalTime >= 1)
            {
                DisplayFPS = FPS;
                FPS = 0;
                TotalTime = 0;
            }

            // Format the string appropriately  
            string FpsText = DisplayFPS.ToString() + " FPS";

           // spriteBatch.DrawString(Font, FpsText, FPSPos, Color.Orange);
            FPS += 1;
            spriteBatch.DrawString(Font, "Score: " + Score.ToString(),scorePosition,Color.Yellow);         
            spriteBatch.DrawString(Font, "Life: " , livesPosition, Color.Yellow);
            spriteBatch.DrawString(Font, "Level: " + Level.ToString() ,  levelPositon, Color.Yellow);
        }
    }
}
