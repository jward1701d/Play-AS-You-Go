using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Play_As_You_Go
{
#if WINDOWS
    struct GameKey
    {
        public Keys Key;
        public bool Down;
        public bool Pressed;
        public bool Released;

        public GameKey(Keys key)
        {
            this.Key = key;
            this.Down = false;
            this.Pressed = false;
            this.Released = false;
        }
    }

    static class InputHelper
    {
        public static GameKey[] gameKeys;
        private static KeyboardState keyboardState;
        private static KeyboardState lastKeyboardState;

        static InputHelper()
        {
            gameKeys = new GameKey[] { new GameKey(Keys.Left),
                                       new GameKey(Keys.Up),
                                       new GameKey(Keys.Down),
                                       new GameKey(Keys.Right),
                                       new GameKey(Keys.Enter)};


        }
        public static void Update(bool prevState, PlayerIndex pIndex)
        {
            keyboardState = Keyboard.GetState();

            for (int i = 0; i < gameKeys.Length; i++)
            {
                if (keyboardState.IsKeyDown(gameKeys[i].Key))
                {
                    gameKeys[i].Down = true;

                    if (lastKeyboardState.IsKeyUp(gameKeys[i].Key))
                    {
                        gameKeys[i].Pressed = true;

                    }
                    else
                    {
                        gameKeys[i].Pressed = false;

                    }
                }
                else
                {
                    gameKeys[i].Down = false;

                    if (lastKeyboardState.IsKeyDown(gameKeys[i].Key))
                    {
                        gameKeys[i].Released = true;

                    }
                    else
                    {
                        gameKeys[i].Released = false;

                    }
                }
            }
            lastKeyboardState = keyboardState;
        }
    }
}
#else
    struct GameButtons
    {
        public Buttons Button;
        public bool Down;
        public bool Pressed;
        public bool Released;
    
        public GameButtons(Buttons button)
        {
            this.Button = button;
            this.Down = false;
            this.Pressed = false;
            this.Released = false;
        }
    }
    static class InputHelper
    {
     
        public static GameButtons[] gameButtons;
        private static GamePadState gamePadState;
        private static GamePadState lastGamePadState;
       

        static InputHelper()
        {
            gameButtons = new GameButtons[] { new GameButtons(Buttons.X),
                                              new GameButtons(Buttons.Y),
                                              new GameButtons(Buttons.A),
                                              new GameButtons(Buttons.B),
                                              new GameButtons(Buttons.Start)};
            
        }
        public static void Update(bool prevState, PlayerIndex pIndex)
        {
           
            gamePadState = GamePad.GetState(pIndex);
            for (int i = 0; i < gameButtons.Length; i++)
            {   
                 if(prevState == true)
                 {
                    gameButtons[i].Pressed = false;
                 }
    
                if (gamePadState.IsButtonDown(gameButtons[i].Button))
                {
                    gameButtons[i].Down = true;
                    if (lastGamePadState.IsButtonUp(gameButtons[i].Button))
                    {
                        gameButtons[i].Pressed = true;
                    }
                    else
                    {
                        gameButtons[i].Pressed = false;
                    }
                }
                else
                {
                    gameButtons[i].Down = false;
                    if (lastGamePadState.IsButtonDown(gameButtons[i].Button))
                    {
                        gameButtons[i].Released = true;
                    }
                    else
                    {
                        gameButtons[i].Released = false;
                    }
                }
            }
            lastGamePadState = gamePadState;
        }
    }
}
#endif
