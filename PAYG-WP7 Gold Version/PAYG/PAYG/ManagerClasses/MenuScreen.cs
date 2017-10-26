#region info
/******************************************************************************
 * MenuScreen.cs                                                              *
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
using Microsoft.Xna.Framework.Input.Touch;

#endregion

#region Name Space

namespace PAYG
{
    #region Class

    public abstract class MenuScreen : GameScreen
    {
        #region Fields and Properties

        //Holds the current GameScreen 
        GameScreen parent;
        public GameScreen Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        //List of menu entries to be displayed
        List<MenuEntry> menuEntries = new List<MenuEntry>();
        public List<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        //The font this menu will use
        SpriteFont spriteFont;
        public SpriteFont SpriteFont
        {
            get { return spriteFont; }
            set { spriteFont = value; }
        }

        //The position of our items
        Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        //The color of the item when it is selected
        Color selected;
        public Color Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        //The color of the text when it is not selected
        Color nonselected;
        public Color NonSelected
        {
            get { return nonselected; }
            set { nonselected = value; }
        }

        //The selected menu item
        int selectedEntry = 0;
        #endregion

        //Constructor to set the transition timing
        public MenuScreen()
        {
        }

        public MenuScreen(GameScreen screen)
        {
            if (screen.ScreenState != ScreenState.Frozen)
                screen.ScreenState = ScreenState.Frozen;
            parent = screen;
        }

        //Unloads the font, if there is one
        public override void UnloadContent()
        {
            if (SpriteFont != null)
                SpriteFont = null;
        }


        //Moves to a different selected menu item and accepts an item
        public override void HandleInput()
        {
            //Grab a reference to the InputSystem object
            InputControlSystem input = ScreenManager.InputSystem;

            //If we move up or down, select a different entry
            if (input.MenuUp)
            {
                selectedEntry--;
                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }
            if (input.MenuDown)
            {
                selectedEntry++;
                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }

            //If we press the select button, call the MenuSelect method
            if (input.MenuSelect)
            {
                menuEntries[selectedEntry].Select();
            }

#if WINDOWS_PHONE

            foreach (GestureSample gesture in input.Gestures)
            {
                if (gesture.GestureType == GestureType.Tap)
                {
                    Point tap = new Point((int)gesture.Position.X, (int)gesture.Position.Y);
                    for (int i = 0; i < menuEntries.Count; i++ )
                    {
                        if (menuEntries[i].GetEntryHitBounds(spriteFont, ScreenManager.GraphicsDevice).Contains(tap))
                        {
                            menuEntries[i].Select();
                        }
                    }//end foreach
                }
            }//end foreach

#endif
        }

        //Updates the menu items position based on transition
        public override void Update(GameTime gameTime, bool covered)
        {
            base.Update(gameTime, covered);
            for (int i = 0; i < menuEntries.Count; i++)
            {
                menuEntries[i].Update(gameTime, i == selectedEntry);
                menuEntries[selectedEntry].Highlight();
            }
        }

        //Draws all the menu items, and increments the positions Y component
        //for every new item
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool selected = (i == selectedEntry);
                menuEntries[i].Draw(gameTime, selected);
            }
            spriteBatch.End();
        }
    }

    #endregion
}

#endregion

