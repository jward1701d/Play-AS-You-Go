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
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO.IsolatedStorage;

using System.Collections;
using System.Runtime.Serialization; //needed for data contracts
using System.Xml;

#endregion

#region Namespace
namespace PAYG
{
    #region Class
    
    [DataContract]
    public class GameObject
    {
        #region Attributes
        [DataMember]
        const int AlPHA_THRESHOLD = 48;                   // helps with the alpha threshold of texture
        [IgnoreDataMember]
        public Texture2D Texture { get; private set; }    // property from which the sprite is drawn from
        [DataMember]
        public Vector2 Position;                          // property for postion of sprite   
        [DataMember]
        public Vector2 Velocity;                          // property for velocity of sprite
        [IgnoreDataMember]
        public float Speed;
        [DataMember]
        public Vector2 Origin;                            // Origin, Scale, Rotation, Layer, Color   
        [DataMember]
        public float Scale = 1;                           // just mimics the parameters of SpriteBach.Draw
        [DataMember]
        public float Rotation;                            // that will be used in the Draw Call
        [IgnoreDataMember]
        public float Layer;
        [IgnoreDataMember]
        public int Health;
        [IgnoreDataMember]
        public bool IsDestructable = false;
        [IgnoreDataMember]
        public Color Color = Color.White;
        [DataMember]
        public bool Active = true;                       // is a simple flag that indicates wheather the sprite is "alive"
        [IgnoreDataMember]
        public int TotalFrames { get; private set; }     // indicates the number of animation frames
        [IgnoreDataMember]
        public TimeSpan AnimationSpeed;                  // speed of the animation
        [IgnoreDataMember]
        private Rectangle[] Rectangel;                   // holds the x,y codinates and the width and height of the sprite on spritesheet in a bounding Box
        [IgnoreDataMember]
        private int CurrentFrame;                        // holds the current frame in the animation
        [IgnoreDataMember]
        private TimeSpan AnimationElapsed;               // holds how many frames have elapsed in the animation

        #endregion

        #region Helper Properties

        /// <summary>
        /// helper property to return the width of each frame
        /// </summary>
        [IgnoreDataMember]
        public int FrameWidth { get { return Rectangel == null ? Texture.Width : Rectangel[0].Width; } }

        /// <summary>
        /// helper property to return the Height of each frame
        /// </summary>
        [IgnoreDataMember]
        public int FrameHeight { get { return Rectangel == null ? Texture.Height : Rectangel[0].Height; } }

        /// <summary>
        /// helper property to return the ActualWidth just in case the sprite has been scaled in size
        /// </summary>
        [IgnoreDataMember]
        public int ActualWidth { get { return (int)(FrameWidth * Scale); } }

        /// <summary>
        /// helper property to return the ActualHeight just in case the sprite has been scaled in size
        /// </summary>
        [IgnoreDataMember]
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
        public GameObject()
        {
         Position = new Vector2(); 
         Velocity = new Vector2();
         Speed = 0;
         Origin = new Vector2();   
         Scale = 1;
         Rotation = 0;
         Layer = 0;
         Health = 0;
         IsDestructable = false;
         Active = true;            
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
        [IgnoreDataMember]
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
