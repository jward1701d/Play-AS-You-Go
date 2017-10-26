using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;

#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif

#region namespace
namespace Play_As_You_Go
{
    #region class
    public class InputControlSystem
    {

        #region Fields and Properties

        public const int MaxControllers = 4;

        public readonly KeyboardState[] currentKeyboardState;
        public readonly KeyboardState[] previousKeyboardState;

        public readonly GamePadState[] currentGamePadState;
        public readonly GamePadState[] previousGamePadState;

        public readonly GamePadState[] currentThumbStickState;
        public readonly GamePadState[] previousThumbStickState;

        public readonly bool[] gamePadWasConnected;

#if WINDOWS_PHONE
        public TouchCollection TouchState;

        public readonly List<GestureSample> Gestures = new List<GestureSample>();
#endif


        //Keyboard and GamePad states
        #endregion

        #region Initialize

        public InputControlSystem()
        {

            currentKeyboardState = new KeyboardState[MaxControllers];
            currentGamePadState = new GamePadState[MaxControllers];

            previousKeyboardState = new KeyboardState[MaxControllers];
            previousGamePadState = new GamePadState[MaxControllers];            

            gamePadWasConnected = new bool[MaxControllers];

        }
        public void Update()
        {
            for (int i = 0; i < MaxControllers; i++)
            {
                previousKeyboardState[i] = currentKeyboardState[i];
                currentKeyboardState[i] = Keyboard.GetState((PlayerIndex)i);

                previousGamePadState[i] = currentGamePadState[i];
                currentGamePadState[i] = GamePad.GetState((PlayerIndex)i);


                if (currentGamePadState[i].IsConnected)
                {
                    gamePadWasConnected[i] = true;
                }
            }

#if WINDOWS_PHONE

            TouchState = TouchPanel.GetState();

            Gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                Gestures.Add(TouchPanel.ReadGesture());
            }
#endif

        }

        #endregion

        #region Input System Methods

        public bool IsNewKeyboardPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (previousKeyboardState[i].IsKeyUp(key) && currentKeyboardState[i].IsKeyDown(key));
            }
            else
            {
                // Accept input from any player.
                return (IsNewKeyboardPress(key, PlayerIndex.One, out playerIndex) ||
                        IsNewKeyboardPress(key, PlayerIndex.Two, out playerIndex) ||
                        IsNewKeyboardPress(key, PlayerIndex.Three, out playerIndex) ||
                        IsNewKeyboardPress(key, PlayerIndex.Four, out playerIndex));
            }
        }


        public bool IsnewGamePadPress(Buttons b, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (currentGamePadState[i].IsButtonDown(b) && previousGamePadState[i].IsButtonUp(b));
            }
            else
            {
                // Accept input from any player.
                return (IsnewGamePadPress(b, PlayerIndex.One, out playerIndex) ||
                        IsnewGamePadPress(b, PlayerIndex.Two, out playerIndex) ||
                        IsnewGamePadPress(b, PlayerIndex.Three, out playerIndex) ||
                        IsnewGamePadPress(b, PlayerIndex.Four, out playerIndex));
            }
        }


        #region Properties
        public bool MenuUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyboardPress(Keys.Up, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.DPadUp, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.RightThumbstickUp, controllingPlayer, out playerIndex);
        }

        public bool MenuDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyboardPress(Keys.Down, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.DPadDown, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.RightThumbstickDown, controllingPlayer, out playerIndex);
        }

        public bool MenuSelect(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            
            return IsNewKeyboardPress(Keys.Enter, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.A, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.Start, controllingPlayer, out playerIndex);
        }

        public bool Cancel(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyboardPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.B, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.Back, controllingPlayer, out playerIndex);
        }

        public bool PauseGame(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyboardPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                 IsnewGamePadPress(Buttons.Start, controllingPlayer, out playerIndex);
        }

        public bool MoveUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
           return IsNewKeyboardPress(Keys.Up, controllingPlayer, out playerIndex) ||
                  IsnewGamePadPress(Buttons.LeftThumbstickLeft, controllingPlayer, out playerIndex) ||
                  IsnewGamePadPress(Buttons.RightThumbstickLeft, controllingPlayer, out playerIndex) ||
                  IsnewGamePadPress(Buttons.DPadUp, controllingPlayer, out playerIndex); 
        }

        public bool MoveDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyboardPress(Keys.Down, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.LeftThumbstickLeft, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.RightThumbstickLeft, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.DPadDown, controllingPlayer, out playerIndex); 
        }

        public bool MoveLeft(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyboardPress(Keys.Left, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.LeftThumbstickLeft, controllingPlayer, out playerIndex)||
                   IsnewGamePadPress(Buttons.RightThumbstickLeft, controllingPlayer, out playerIndex)||
                   IsnewGamePadPress(Buttons.DPadLeft, controllingPlayer, out playerIndex); 
        }

        public bool MoveRight(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyboardPress(Keys.Right, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.LeftThumbstickRight, controllingPlayer, out playerIndex)||
                   IsnewGamePadPress(Buttons.RightThumbstickRight, controllingPlayer, out playerIndex)||
                   IsnewGamePadPress(Buttons.DPadRight, controllingPlayer, out playerIndex); 
        }

        public bool Shoot(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyboardPress(Keys.Space, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.A, controllingPlayer, out playerIndex); 

        }

        #endregion

        #region HighscoreInterface controlls
        // if (ScreenManager.InputSystem.IsNewKeyboardPress(Keys.Space) || ScreenManager.InputSystem.IsnewGamePadPress(Buttons.X))
        public bool highscoreInterfacespace(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyboardPress(Keys.Space, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.X, controllingPlayer, out playerIndex); 
        }

        //(ScreenManager.InputSystem.IsKeyboardPress(Keys.Back) || ScreenManager.InputSystem.IsGamePadPress(Buttons.B))
        public bool highscoreInterfaceBack(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyboardPress(Keys.Back, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.B, controllingPlayer, out playerIndex); 
        }

        //(ScreenManager.InputSystem.IsKeyboardPress(Keys.Enter) || ScreenManager.InputSystem.IsGamePadPress(Buttons.A))
        public bool highscoreInterfaceEnter(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyboardPress(Keys.Enter, controllingPlayer, out playerIndex) ||
                   IsnewGamePadPress(Buttons.A, controllingPlayer, out playerIndex); 
        }
        #endregion

        #endregion

    }
    #endregion
}
#endregion