#region info
/******************************************************************************
 * MenuEntry.cs                                                               *
 *                                                                            *
 * Writen by: Johnathan Witvoet                                               *
 *                                                                            *
 ******************************************************************************/
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

#region Name Space

namespace PAYG
{
    #region Class

    public class MenuEntry
    {
        #region Fields and Properties

        //String to hold the enties
        string entry;
        public string Entry
        {
            get { return entry; }
            set { entry = value; }
        }

        // initialPosition helps with setting the 
        //relative postion of the 2 ,3 , etc. menu entry
        Vector2 initialPosition;
        public Vector2 InitialPosition
        {
            get { return initialPosition; }
            set { initialPosition = value; }
        }

        // helps with seting initialPosition and the relitive position
        Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        // used for moving menu entry 
        Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        // the seep at wich the menu entry moves
        Vector2 acceleration;
        public Vector2 Acceleration
        {
            get { return acceleration; }
            set { acceleration = value; }
        }

        MenuScreen menuScreen;
        public MenuScreen MenuScreen
        {
            get { return menuScreen; }
            set { menuScreen = value; }
        }


        public Rectangle GetEntryHitBounds(SpriteFont font, GraphicsDevice gDevice)
        {
            return new Rectangle((int)Position.X, (int)Position.Y, 175,
                font.LineSpacing);
        }//end GetEntryHitBounds

        #endregion

        #region Events and Methods

        public event EventHandler Highlighted;

        // Highlights the string 
        public virtual void Highlight()
        {
            if (Highlighted != null)
                Highlighted(this, EventArgs.Empty);
        }

        // an event handler for the selected menu entry
        public event EventHandler Selected;
        public virtual void Select()
        {
            if (Selected != null)
                Selected(this, EventArgs.Empty);
        }

        #endregion

        #region Initialize

        //Constructor 
        public MenuEntry(MenuScreen screen, string title)
        {
            this.menuScreen = screen;
            entry = title;
        }

        #endregion

        #region Update and Draw

        public virtual void Update(GameTime gameTime, bool isSelected)
        {
            position = new Vector2(initialPosition.X, initialPosition.Y);

            if (menuScreen.ScreenState == ScreenState.TransitionOn || menuScreen.ScreenState == ScreenState.TransitionOff)
            {
               // acceleration = new Vector2((float)Math.Pow(menuScreen.TransitionPercent - 1, 2), 0);
                //acceleration.X *= menuScreen.TransitionDirection * -150;

                //position += acceleration;
            }
        }

        public virtual void Draw(GameTime gameTime, bool isSelected)
        {
            Color color = isSelected ? menuScreen.Selected : menuScreen.NonSelected;
            color = new Color(color.R, color.G, color.B, menuScreen.ScreenAlpha);

            SpriteBatch spriteBatch = menuScreen.ScreenManager.SpriteBatch;
            SpriteFont spriteFont = menuScreen.SpriteFont;

            Vector2 entryPosition = new Vector2(position.X, position.Y);

            if (spriteFont != null && entry.Length > 0)
                spriteBatch.DrawString(spriteFont, entry, entryPosition, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0.0f);
        }
        #endregion

        #region Methods

        // the next 2 functons help with setting the position of every menu entry
        public void SetRelativePosition(Vector2 relativePosition, MenuEntry entry, bool initialPosition)
        {
            Vector2 position = new Vector2(entry.position.X, entry.position.Y);
            SetPosition(Vector2.Add(position, relativePosition), initialPosition);
        }

        public void SetPosition(Vector2 position, bool initialPosition)
        {
            if (initialPosition)
                this.initialPosition = position;
            this.position = position;
        }
        #endregion
    }

    #endregion
}

#endregion
