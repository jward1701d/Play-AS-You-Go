#region info
/******************************************************************************
 * GameScreen.cs                                                              *
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
using System.IO;

#endregion

#region NameSpace

namespace PAYG
{
    #region Enumerations
    /// <summary>
    /// What is the screen doing currently?
    /// </summary>
    public enum ScreenState
    {
        TransitionOn,
        TransitionOff,
        Active,
        Hidden,
        Frozen,
        Inactive,
    }

    #endregion

    #region Class

    public abstract class GameScreen
    {
        #region Fields and Properties

        /// <summary>
        /// The amount of time it takes for the screen to transition on
        /// </summary>
        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }
        TimeSpan transitionOnTime = TimeSpan.Zero;

        /// <summary>
        /// The amount of time it takes for the screen to transition off
        /// </summary>
        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }
        TimeSpan transitionOffTime = TimeSpan.Zero;

        /// <summary>
        /// How far along is the transition?
        /// </summary>
        public float TransitionPercent
        {
            get { return transitionPercent; }
        }
        float transitionPercent = 0.00f;

        /// <summary>
        /// Controls how fast the screen transitions
        /// </summary>
        public float TransitionSpeed
        {
            get { return transitionSpeed; }
        }
        float transitionSpeed = 1.5f;

        /// <summary>
        /// Controls the direction the screen transitions
        /// </summary>
        public int TransitionDirection
        {
            get { return transitionDirection; }
        }
        int transitionDirection = 1;

        /// <summary>
        /// Holds the alpha value of the screen
        /// </summary>
        public byte ScreenAlpha
        {
            get { return (byte)(transitionPercent * 255); }
        }

        /// <summary>
        /// Gets the current position of the screen transition, ranging
        /// from zero (fully active, no transition) to one (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        float transitionPosition = 1;


        /// <summary>
        /// Gets the current alpha of the screen transition, ranging
        /// from 1 (fully active, no transition) to 0 (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }

        /// <summary>
        /// What is the screen doing currently?
        /// </summary>
        public ScreenState ScreenState
        {
            get { return screenState; }
            set { screenState = value; }
        }
        ScreenState screenState = ScreenState.TransitionOn;

        /// <summary>
        /// The screen manager that controls the screen.
        /// </summary>
        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            internal set { screenManager = value; }
        }
        ScreenManager screenManager;

        /// <summary>
        /// Is the screen currently exiting?
        /// </summary>
        public bool IsExiting
        {
            get { return isExiting; }
            protected set
            {
                isExiting = value;
                if (isExiting && (Exiting != null))
                {
                    Exiting(this, EventArgs.Empty);
                }
            }
        }
        bool isExiting = false;

        /// <summary>
        /// Gets whether or not this screen is serializable. If this is true,
        /// the screen will be recorded into the screen manager's state and
        /// its Serialize and Deserialize methods will be called as appropriate.
        /// If this is false, the screen will be ignored during serialization.
        /// By default, all screens are assumed to be serializable.
        /// </summary>
        public bool IsSerializable
        {
            get { return isSerializable; }
            protected set { isSerializable = value; }
        }

        bool isSerializable = true;

        public bool IsActive
        {
            get
            {
                return (screenState == ScreenState.TransitionOn
                    || screenState == ScreenState.Active);
            }
        }

        /// <summary>
        /// Event Handlers for the screen exiting and being removed
        /// </summary>
        public event EventHandler Exiting;
        public event EventHandler Removed;

        /// <summary>
        /// Is the screen currently being covered by another?
        /// </summary>
        #endregion

        #region Initialization
        //Allows each screen to load dedicated content
        public virtual void LoadContent() { }

        //Allows each screen to unload dedicated content
        public virtual void UnloadContent() { }
        #endregion

        #region Update and Draw
        /// <summary>
        /// Method to initialize objects and variables (not content!)
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// Updates the screen by checking its state and performing
        /// actions based on what the screen state is.
        /// 
        /// Override this method to perform screen-independent update logic.
        /// </summary>
        /// <param name="gameTime">GameTime object to allow us to perform time basic logic</param>
        /// <param name="covered">Is the screen covered?</param>
        public virtual void Update(GameTime gameTime, bool covered)
        {
            //If the screen state is either frozen or inactive, do not do any updating.
            //This is needed in case a screen sets the status before base.Update();
            if (screenState == ScreenState.Frozen || screenState == ScreenState.Inactive)
            {
                return;
            }

            if (IsExiting)
            {
                //if we are exiting, set the screen to transition off
                screenState = ScreenState.TransitionOff;

                if (!ScreenTransition(gameTime, transitionOffTime, -1))
                {
                    //Remove the screen if the time is up.
                    this.Remove();
                }
            }
            else if (covered)
            {
                if (ScreenTransition(gameTime, transitionOffTime, 1))
                {
                    //if the screen is covered and transitioning, set it
                    //to transition off
                    screenState = ScreenState.TransitionOff;
                }
                else
                {
                    //otherwise its hidden
                    screenState = ScreenState.Hidden;
                }
            }
            else if (screenState == ScreenState.TransitionOn)
            {
                //Check the transition
                if (ScreenTransition(gameTime, transitionOnTime, 1))
                {
                    screenState = ScreenState.TransitionOn;
                }
                //If we are done with the transition, set the state to active
                else
                {
                    screenState = ScreenState.Active;
                }
            }
            else if (screenState == ScreenState.TransitionOn)
            {
                //Check the transition.
                if (ScreenTransition(gameTime, transitionOffTime, -1))
                {
                    screenState = ScreenState.TransitionOff;
                }
                //If we are done with the transition, set the state to inactive.
                else
                {
                    screenState = ScreenState.Inactive;
                }
            }
        }

        /// <summary>
        /// Removes the screen from the manager
        /// Either Override this if you want to perform
        /// special actions (new screen added), or
        /// use the event.
        /// </summary>
        public virtual void Remove()
        {
            screenManager.RemoveScreen(this);
            if (Removed != null)
                Removed(this, EventArgs.Empty);
        }

        /// <summary>
        /// Transition the screen
        /// </summary>
        /// <param name="gameTime">GameTime object for calculating the difference in time</param>
        /// <param name="transitionTime">The time we set our screen to transition</param>
        /// <param name="direction">Direction of the transition (1 for on and -1 for off)</param>
        /// <returns></returns>
        private bool ScreenTransition(GameTime gameTime, TimeSpan transitionTime, int direction)
        {
            //Check the direction to make sure it is either 1 or -1
            if (direction > 0 && direction != 1)
                direction = 1;
            else if (direction < 0 && direction != -1)
                direction = -1;

            float transitionDelta;

            //If the time is 0, there is no transition
            if (transitionTime == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / transitionTime.TotalMilliseconds);

            //Update the percent value of the transition
            transitionPercent += transitionDelta * direction * transitionSpeed;

            if ((transitionPercent <= 0) || (transitionPercent >= 1))
            {
                //We are done transitioning
                transitionPercent = MathHelper.Clamp(transitionPercent, 0, 1);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Allows the screen to handle input
        /// </summary>
        public virtual void HandleInput()
        {
            //if the screen is not active, do nothing
            if (screenState != ScreenState.Active)
                return;
        }

        /// <summary>
        /// Draws stuff to the game window.
        /// </summary>
        /// <param name="gameTime">GameTime object to perform timed elements</param>
        public abstract void Draw(GameTime gameTime);

        /// <summary>
        /// This method will fade the screen behind it by
        /// creating a large texture over the current screen.
        /// spriteBatch HAS to be in between begin and end calls.
        /// </summary>
        /// <param name="spriteBatch">The active sprite batch object</param>
        /// <param name="fade">The texture to use for the fading.</param>
        /// <param name="color">The color you want the screen to fade</param>
        /// <param name="opaticy">Amount to fade by (between 0 and 1)</param>
        public void FadeScreen(SpriteBatch spriteBatch, Texture2D fade, Color color, float opacity)
        {
            opacity = MathHelper.Clamp(opacity, 0, 1);
            spriteBatch.Draw(fade, new Rectangle(0, 0, ScreenManager.Viewport.Width, ScreenManager.Viewport.Height),
                new Color(color.R, color.G, color.B, (byte)(opacity * 255)));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Preforms logic for when we want to exit the screen.
        /// Override this to perform special actions (new screen).
        /// If you add a new screen here, you will not get a transition off.
        /// </summary>
        public virtual void ExitScreen()
        {
            IsExiting = true;
            if (transitionOffTime == TimeSpan.Zero)
                this.Remove();
        }
        public void FreezeScreen()
        {
            //Screen will be drawn but not updated
            screenState = ScreenState.Frozen;
        }

        /// <summary>
        /// Tells the screen to serialize its state into the given stream.
        /// </summary>
        public virtual void Serialize(Stream stream) { }

        /// <summary>
        /// Tells the screen to deserialize its state from the given stream.
        /// </summary>
        public virtual void Deserialize(Stream stream) { }

        #endregion
    }

    #endregion
}

#endregion
