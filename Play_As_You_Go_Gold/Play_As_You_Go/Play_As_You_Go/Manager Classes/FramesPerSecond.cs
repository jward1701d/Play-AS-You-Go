using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Play_As_You_Go
{
    public class FramesPerSecond 
    {
        private float FPS = 0f, TotalTime = 0f, DisplayFPS = 0f;

        public SpriteFont Font { get; set; }

        public Vector2 FPSPosition { get; set; }

        public Color FPSColor { get; set; }

        public FramesPerSecond()
            
        {
           
        }

        public void Draw(GameTime gameTime, SpriteBatch spritebatch)
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
            FPS += 1;

            // Format the string appropriately  
            string FpsText = DisplayFPS.ToString() + " FPS";

            spritebatch.DrawString(Font, FpsText, FPSPosition, FPSColor);
        }
    }
}
