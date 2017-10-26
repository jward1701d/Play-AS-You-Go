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

#endregion

#region Name Space

namespace Play_As_You_Go
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
        protected IList<MenuEntry> MenuEntries
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
        Color selectedColor;
        public Color SelectedColor
        {
            get { return selectedColor; }
            set { selectedColor = value; }
        }

        //The color of the text when it is not selected
        Color nonselectedColor;
        public Color NonSelectedColor
        {
            get { return nonselectedColor; }
            set { nonselectedColor = value; }
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
        public override void HandleInput(InputControlSystem input)
        {
            
            //If we move up or down, select a different entry
            if (input.MenuUp(ControllingPlayer))
            {
                selectedEntry--;
                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }
            if (input.MenuDown(ControllingPlayer))
            {
                selectedEntry++;
                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }

            PlayerIndex playerIndex;
            //If we press the select button, call the MenuSelect method
            if (input.MenuSelect(ControllingPlayer, out playerIndex))
            {
                OnSelectEntry(selectedEntry, playerIndex);
            }
                       

        }

        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[entryIndex].OnSelectEntry(playerIndex);
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

