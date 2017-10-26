#region info
/******************************************************************************
 * ScreenManager.cs                                                           *
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
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;

using System.Collections;
using System.Runtime.Serialization; //needed for data contracts
using System.Xml;

using Microsoft.Advertising.Mobile.Xna;

#endregion

#region Name Space

namespace PAYG
{
    enum GameType { BEYONDSPACE, COLORMIMIC, H1N1, HIGHSCOREINTERFACE, MENU }

    #region Class

    public class ScreenManager : DrawableGameComponent
    {
        #region Fields

        //List of current screens in the manager
        List<GameScreen> screens = new List<GameScreen>();

        //Another list dedicated to the screens that will be 
        //updated in the current game loop.
        List<GameScreen> screensToUpdate = new List<GameScreen>();

        //Spritebatch for 2D drawings
        SpriteBatch spriteBatch;
        
        //An input system so we can have control
        InputControlSystem inputSystem;

        //Is the screen manager initialized?
        bool isInitialized;

        

        #endregion

        #region Properties
        /// <summary>
        /// Return the sprite batch object.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public  float FXVolume = 0.5f;
        public  float prevFXVolume;
        public  bool FXEnabled = true;
        public  float MusicVolume = 0.5f;
        public  float prevMusicVolume;
        public  bool MusicEnabled = true;
        public  bool isPressed = false; // Added to help with back button logic.
        ISsystem checkFile = new ISsystem();
        /// <summary>
        /// The input system property
        /// </summary>
        public InputControlSystem InputSystem
        {
            get { return inputSystem; }
        }

        /// <summary>
        /// Gets the content manager
        /// </summary>
        public ContentManager Content
        {
            get { return Game.Content; }
        }

        /// <summary>
        /// Gets the viewport object
        /// </summary>
        public Viewport Viewport
        {
            get { return GraphicsDevice.Viewport; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor, initializes the manager
        /// </summary>
        public ScreenManager(Game game)
            : base(game)
        {
             IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
             using (storage)
             {
                 if (storage.FileExists("Options.sfs"))
                 {
                     checkFile.checkFile();
                     if (checkFile.isSaveable)
                     {
                         checkFile.LoadOptions(checkFile.isSaveable);
                         FXEnabled = checkFile.fxEnabled;
                         MusicEnabled = checkFile.musicEnabled;
                         FXVolume = checkFile.fxVolume;
                         MusicVolume = checkFile.musicVolume;
                         prevFXVolume = checkFile.prevFxVolume;
                         prevMusicVolume = checkFile.prevMusicVolume;
                     }
                 }
             }
            base.Initialize();
            
            //Creates a new InputSystem
            inputSystem = new InputControlSystem();
            isInitialized = true;
        }

        /// <summary>
        /// Initialize the spriteBatch and screen dedicated content.
        /// </summary>
        protected override void LoadContent()
        {
            ContentManager content = Game.Content;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            //load screen dedicated content
            foreach (GameScreen screen in screens)
                screen.LoadContent();
        }

        /// <summary>
        /// Unload screen dedicated content
        /// </summary>
        protected override void UnloadContent()
        {
            //Tells the screen to unload their content.
            foreach (GameScreen screen in screens)
                screen.UnloadContent();
        }
        #endregion

        #region Update and Draw

        /// <summary>
        /// Update manager and screens
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            //Update the input system
            inputSystem.Update();

            //clear out the screensToUpdate list to copy the screens list
            //this allows us to add or remove screens without complaining.
            screensToUpdate.Clear();

            if (screens.Count == 0)
                this.Game.Exit();

            foreach (GameScreen screen in screens)
                screensToUpdate.Add(screen);

            bool screenIsCovered = false;
            bool firstScreen = true;

            if(Game.IsActive)
            {
                while (screensToUpdate.Count > 0)
                {
                    GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];

                    screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                    //Update the screen unless its frozen or inactive
                    if (screen.ScreenState != ScreenState.Frozen
                        && screen.ScreenState != ScreenState.Inactive)
                    {
                        screen.Update(gameTime, screenIsCovered);
                    }

                    if (screen.IsActive)
                    {
                        if (firstScreen)
                        {
                            screen.HandleInput();
                            firstScreen = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tells each screen to draw
        /// </summary>
        /// <param name="gameTime">Time object to pass to the screens</param>
        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in screens)
            {
                //Tells the current screen to draw if its not hidden
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }
        #endregion

        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }

        #region Methods
        /// <summary>
        /// Adds a screen to the manager
        /// </summary>
        /// <param name="screen">The screen to be added</param>
        public void AddScreen(GameScreen screen)
        {
            //Sets the reference to the screen manager on the screen
            screen.ScreenManager = this;
            
            //If the screen manager is initialized, perform initialize operations 
            //for the screens.
            if (this.isInitialized)
            {
                screen.LoadContent();
                screen.Initialize();
            }

            //Finally, add the screen to the list.
            screens.Add(screen);
        }

        /// <summary>
        /// Removed the desired screen from the system
        /// </summary>
        /// <param name="screen">The screen we wish to remove</param>
        public void RemoveScreen(GameScreen screen)
        {
            //If the screen manager is initialized, unload the screen content.
            if (this.isInitialized)
            {
                screen.UnloadContent();
            }

            //Finally, remove the screen from both lists.
            screens.Remove(screen);
            screensToUpdate.Remove(screen);
        }

        /// <summary>
        /// Informs the screen manager to serialize its state to disk.
        /// </summary>
        public void SerializeState()
        {
            // open up isolated storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // if our screen manager directory already exists, delete the contents
                if (storage.DirectoryExists("PAYG"))
                {
                    DeleteState(storage);
                }

                // otherwise just create the directory
                else
                {
                    storage.CreateDirectory("PAYG");
                }

                // create a file we'll use to store the list of screens in the stack
                using (IsolatedStorageFileStream stream = storage.CreateFile("PAYG\\ScreenList.dat"))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        // write out the full name of all the types in our stack so we can
                        // recreate them if needed.
                        foreach (GameScreen screen in screens)
                        {
                            if (screen.IsSerializable)
                            {
                                writer.Write(screen.GetType().AssemblyQualifiedName);
                            }
                        }
                    }
                }

                // now we create a new file stream for each screen so it can save its state
                // if it needs to. we name each file "ScreenX.dat" where X is the index of
                // the screen in the stack, to ensure the files are uniquely named
                int screenIndex = 0;
                foreach (GameScreen screen in screens)
                {
                    if (screen.IsSerializable)
                    {
                        string fileName = string.Format("PAYG\\Screen{0}.dat", screenIndex);

                        // open up the stream and let the screen serialize whatever state it wants
                        using (IsolatedStorageFileStream stream = storage.CreateFile(fileName))
                        {
                            screen.Serialize(stream);
                        }

                        screenIndex++;
                    }
                }
            }
        }

        public bool DeserializeState()
        {
            // open up isolated storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // see if our saved state directory exists
                if (storage.DirectoryExists("PAYG"))
                {
                    try
                    {
                        // see if we have a screen list
                        if (storage.FileExists("PAYG\\ScreenList.dat"))
                        {
                            // load the list of screen types
                            using (IsolatedStorageFileStream stream = storage.OpenFile("PAYG\\ScreenList.dat", FileMode.Open, FileAccess.Read))
                            {
                                using (BinaryReader reader = new BinaryReader(stream))
                                {
                                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                                    {
                                        // read a line from our file
                                        string line = reader.ReadString();

                                        // if it isn't blank, we can create a screen from it
                                        if (!string.IsNullOrEmpty(line))
                                        {
                                            Type screenType = Type.GetType(line);
                                            GameScreen screen = Activator.CreateInstance(screenType) as GameScreen;
                                            AddScreen(screen);
                                        }
                                    }
                                }
                            }
                        }

                        // next we give each screen a chance to deserialize from the disk
                        for (int i = 0; i < screens.Count; i++)
                        {
                            string filename = string.Format("PAYG\\Screen{0}.dat", i);
                            using (IsolatedStorageFileStream stream = storage.OpenFile(filename, FileMode.Open, FileAccess.Read))
                            {
                                screens[i].Deserialize(stream);
                            }
                        }

                        return true;
                    }
                    catch (Exception)
                    {
                        // if an exception was thrown while reading, odds are we cannot recover
                        // from the saved state, so we will delete it so the game can correctly
                        // launch.
                        DeleteState(storage);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Deletes the saved state files from isolated storage.
        /// </summary>
        private void DeleteState(IsolatedStorageFile storage)
        {
            // get all of the files in the directory and delete them
            string[] files = storage.GetFileNames("PAYG\\*");
            foreach (string file in files)
            {
                storage.DeleteFile(Path.Combine("PAYG", file));
            }
        }

        #endregion

        #region ADs
        
        private static readonly string AdUnitId = "10023417";

        private DrawableAd bannerAd;
        public DrawableAd BannerAd
        {
            get { return bannerAd; }
            set { bannerAd = value; }
        }

        private AdGameComponent adGameComponent;
        public AdGameComponent AdGameComponent
        {
            get { return adGameComponent; }
            set { adGameComponent = value; }
        }

        private int adPositionX;
        public int AdPositionX
        {
            get { return adPositionX; }
            set { adPositionX = value; }
        }

        private int adPositionY;
        public int AdPositionY
        {
            get { return adPositionY; }
            set { adPositionY = value; }
        }

        public void CreateAd()
        {
            int width = 480;
            int height = 80;

            bannerAd = adGameComponent.CreateAd(AdUnitId, new Rectangle(adPositionX, adPositionY, width, height), true);
            // Set some visual properties (optional).
            bannerAd.BorderEnabled = true; // default is true
            bannerAd.BorderColor = Color.White; // default is White
            bannerAd.DropShadowEnabled = true; // default is true

            adGameComponent.Enabled = false;
            AdGameComponent.Current.Enabled = true;
        }

        public void RemoveAd()
        {
            adGameComponent.RemoveAd(bannerAd);
        }

        #endregion

    }

    #endregion
}

#endregion