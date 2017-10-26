using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif

namespace PAYG
{
    public class InputControlSystem
    {

        #region Fields and Properties

        public KeyboardState currentKeyboardState;
        public KeyboardState previousKeyboardState;

        public GamePadState currentGamePadState;
        public GamePadState previousGamePadState;

        public bool gamePadDisconnected;

#if WINDOWS_PHONE
        public TouchCollection TouchState;

        public readonly List<GestureSample> Gestures = new List<GestureSample>();
#endif

        //Keyboard and GamePad states
        #endregion

        #region Properties
        public bool MenuUp
        {
            get { return IsKeyboardPress(Keys.Up) || IsKeyboardPress(Keys.W) || IsGamePadPress(Buttons.LeftThumbstickUp); }
        }

        public bool MenuDown
        {
            get { return IsKeyboardPress(Keys.Down) || IsKeyboardPress(Keys.S) || IsGamePadPress(Buttons.LeftThumbstickDown); }
        }

        public bool MenuSelect
        {
            get { return IsKeyboardPress(Keys.Enter) || IsGamePadPress(Buttons.A); }
        }

        public bool Cancel
        {
            get { return IsKeyboardPress(Keys.Escape) || IsGamePadPress(Buttons.Back); }
        }

        public bool PauseGame
        {
            get { return IsKeyboardPress(Keys.Escape) || IsGamePadPress(Buttons.Start); }
        }

        public bool MoveUp
        {
            get { return KeypressedCurrently(Keys.Up); }
        }

        public bool MoveDown
        {
            get { return KeypressedCurrently(Keys.Down); }
        }

        public bool MoveLeft
        {
            get { return KeypressedCurrently(Keys.Left); }
        }

        public bool MoveRight
        {
            get { return KeypressedCurrently(Keys.Right); }
        }

        public Vector2 LeftThumbstick
        {
            get { return currentGamePadState.ThumbSticks.Left; }
        }

        public Vector2 RightThumbstick
        {
            get { return currentGamePadState.ThumbSticks.Right; }
        }

        public bool GamePadDissconected
        {
            get { return gamePadDisconnected; }
        }


        #endregion

        #region Input System Methods

        public InputControlSystem()
        {

            currentKeyboardState = new KeyboardState();
            currentGamePadState = new GamePadState();

            previousKeyboardState = new KeyboardState();
            previousGamePadState = new GamePadState();

        }
        public void Update()
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            previousGamePadState = currentGamePadState;
           
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            if (currentGamePadState.IsConnected == true)
            {
                gamePadDisconnected = false;
            }
            else if (currentGamePadState.IsConnected == false)
            {
                gamePadDisconnected = true;
            }


#if WINDOWS_PHONE

            TouchState = TouchPanel.GetState();

            Gestures.Clear();

            TouchPanel.EnabledGestures = 
                GestureType.Tap |
                GestureType.Hold |
                GestureType.DoubleTap |
                GestureType.FreeDrag |
                GestureType.Flick |
                GestureType.Pinch;

            while (TouchPanel.IsGestureAvailable)
            {
                Gestures.Add(TouchPanel.ReadGesture());
            }
#endif

        }

        public bool IsKeyboardPress(Keys key)
        {
            return previousKeyboardState.IsKeyUp(key) && currentKeyboardState.IsKeyDown(key);
        }

        public bool KeypressedCurrently(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        public bool IsGamePadPress(Buttons b)
        {
            return previousGamePadState.IsButtonUp(b) && currentGamePadState.IsButtonDown(b);
        }

        public bool IsnewGamePadPress(Buttons b)
        {
            return currentGamePadState.IsButtonDown(b);
        }

#if WINDOWS_PHONE

        public bool IsRectanglePressed(Rectangle rectangle)
        {
            foreach (GestureSample gesture in Gestures)
            {
                if (gesture.GestureType == GestureType.Tap)
                {
                    Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);
                    if (rectangle.Contains(tapLocation))
                    {
                        return true;
                    }//end if
                    else
                    {
                        return false;
                    }//end else
                }//end if
            }//end foreach

            return false;
        }//end IsRectanglePressed

#endif

        #endregion

    }
}
