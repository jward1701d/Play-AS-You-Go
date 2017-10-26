#region info
/******************************************************************************
 * GameObject.cs                                                              *
 *                                                                            *
 * Writen by: Johnathan Witvoet                                               *
 *                                                                            *
 * This class can animate along several frames on a spritesheet               *
 * and can provide other services such as drawing and position updating       *
 *                                                                            *
 ******************************************************************************/
#endregion

#region Using Statements

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

#region Namespace
namespace Play_As_You_Go
{
    #region Class

    public class GameObject
    {
        #region Attributes

        const int AlPHA_THRESHOLD = 48;                   // helps with the alpha threshold of texture

        public Texture2D Texture { get; private set; }    // property from which the sprite is drawn from
        public Vector2 Position;                          // property for postion of sprite   
        public Vector2 Velocity;                          // property for velocity of sprite

        public float Speed;
        public Vector2 Origin;                            // Origin, Scale, Rotation, Layer, Color            
        public float Scale = 1;                           // just mimics the parameters of SpriteBach.Draw
        public float Rotation;                            // that will be used in the Draw Call
        public float Layer;
        public int Health;
        public bool IsDestructable = false;
        public Color Color = Color.White;

        public bool Active = true;                       // is a simple flag that indicates wheather the sprite is "alive"

        public int TotalFrames { get; private set; }     // indicates the number of animation frames
        public TimeSpan AnimationSpeed;                  // speed of the animation

        private Rectangle[] Rectangel;                   // holds the x,y codinates and the width and height of the sprite on spritesheet in a bounding Box

        private int CurrentFrame;                        // holds the current frame in the animation
        private TimeSpan AnimationElapsed;               // holds how many frames have elapsed in the animation

        #endregion

        #region Helper Properties

        /// <summary>
        /// helper property to return the width of each frame
        /// </summary>
        public int FrameWidth { get { return Rectangel == null ? Texture.Width : Rectangel[0].Width; } }

        /// <summary>
        /// helper property to return the Height of each frame
        /// </summary>
        public int FrameHeight { get { return Rectangel == null ? Texture.Height : Rectangel[0].Height; } }

        /// <summary>
        /// helper property to return the ActualWidth just in case the sprite has been scaled in size
        /// </summary>
        public int ActualWidth { get { return (int)(FrameWidth * Scale); } }

        /// <summary>
        /// helper property to return the ActualHeight just in case the sprite has been scaled in size
        /// </summary>
        public int ActualHeight { get { return (int)(FrameHeight * Scale); } }

        #endregion

        #region Constructor

        /// <summary>
        /// only required parameter is the texture 
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="firstRect"> indicates the cordinates and size of the first frame within the image texture</param>
        ///                          if null entire texture is the singal frame
        /// <param name="frames"> how many frames are in the animation</param>
        /// <param name="horizontal">if true the animation is horizontal else if false animation is vertical</param>
        /// <param name="space"> is there any space between the sprites in the animation</param>
        public GameObject(Texture2D texture, Rectangle? firstRect, int frames, bool horizontal, int space)
        {
            // ToDo: error handeling
            Texture = texture;
            TotalFrames = frames;

            if (firstRect != null)
            {
                Rectangel = new Rectangle[frames];

                Rectangle first = (Rectangle)firstRect;
                for (int i = 0; i < frames; i++)
                    Rectangel[i] = new Rectangle(first.Left + (horizontal ? (first.Width + space) * i : 0),
                        first.Top + (horizontal ? 0 : (first.Height + space) * i), first.Width, first.Height);
            }
        }

        #endregion

        #region Update, Draw

        /// <summary>
        /// takes care of animation if any and moves the sprite if Velocity is non-zero
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            if (Active)
            {
                if (TotalFrames > 1 && (AnimationElapsed += gameTime.ElapsedGameTime) > AnimationSpeed)
                {
                    if (++CurrentFrame == TotalFrames)
                        CurrentFrame = 0;
                    AnimationElapsed -= AnimationSpeed;
                }
                Position += Velocity;
            }
        }

        /// <summary>
        /// Draws the animation if sprite is Active
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="batch"></param>
        public virtual void Draw(GameTime gameTime, SpriteBatch batch)
        {
            if (Active)
            {
                batch.Draw(Texture, Position, Rectangel == null ? null : (Rectangle?)Rectangel[CurrentFrame],
                    Color, Rotation, Origin, Scale, SpriteEffects.None, Layer);
            }
        }

        public Rectangle BoundingRect
        {
            get
            {
                return new Rectangle((int)(Position.X - Origin.X * Scale), (int)(Position.Y - Origin.Y * Scale),
                   (int)(Rectangel[0].Width * Scale), (int)(Rectangel[0].Height * Scale));
            }
        }

        // box box collision
        public virtual bool Collide(GameObject other)
        {
            return BoundingRect.Intersects(other.BoundingRect);
        }

        #region per Pixel Collision


        // normilizes the bounding rect
        public Rectangle NormalizeIntersect(Rectangle rect)
        {
            return new Rectangle(rect.X - (int)this.Position.X,
                                 rect.Y - (int)this.Position.Y,
                                 rect.Width, rect.Height);
        }

        // per pixel collision Intersecttion
        public static Rectangle Intersect(Rectangle boundsA, Rectangle boundsB)
        {
            int x1 = Math.Max(boundsA.Left, boundsB.Left);
            int y1 = Math.Max(boundsA.Top, boundsB.Top);
            int x2 = Math.Min(boundsA.Right, boundsB.Right);
            int y2 = Math.Min(boundsA.Bottom, boundsB.Bottom);

            int width = x2 - x1;
            int height = y2 - y1;

            if (width > 0 && height > 0)
                return new Rectangle(x1, y1, width, height);

            else return Rectangle.Empty;
        }

        // check to see if collision has occured
        public static bool CheckCollision(GameObject gameObjectA, GameObject gameObjectB)
        {
            Rectangle collisionRect = Intersect(gameObjectA.BoundingRect, gameObjectB.BoundingRect);

            if (collisionRect == Rectangle.Empty)
                return false;

            int pixelCount = collisionRect.Width * collisionRect.Height;

            Color[] pixelA = new Color[pixelCount];
            Color[] pixelB = new Color[pixelCount];

            gameObjectA.Texture.GetData<Color>(0, gameObjectA.NormalizeIntersect(collisionRect),
                                                pixelA, 0, pixelCount);

            gameObjectB.Texture.GetData<Color>(0, gameObjectB.NormalizeIntersect(collisionRect),
                                                pixelB, 0, pixelCount);

            for (int i = 0; i < pixelCount; i++)
            {
                if (pixelA[i].A >= AlPHA_THRESHOLD && pixelB[i].A >= AlPHA_THRESHOLD)
                    return true;
            }

            return false;
        }

        #endregion

        #endregion
    }

    #endregion
}

#endregion
