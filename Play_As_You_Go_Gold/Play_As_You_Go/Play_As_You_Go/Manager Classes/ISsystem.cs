/****************************************
 *     Isolated Storage code            *
 *              By                      *
 *          James Ward                  *
 *                                      *
 * Property of Scrubby Fresh Studios    *    
 * 2011 all rights reserved.            *
 *                                      *    
 ****************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

namespace Play_As_You_Go
{
    public struct ISsystemData
    {
        public string[] playerName;
        public int[] score;
        public int[] level;
        public int[] lives;
        public int[] playTime;
        public string[] gameTitle;
        public int Count;

        public ISsystemData(int count)
        {
            playerName = new string[count];
            score = new int[count];
            level = new int[count];
            lives = new int[count];
            playTime = new int[count];
            gameTitle = new string[count];

            Count = count;
        }
    }
    public class ISsystem 
    {
        /// <summary>
        /// Constructor for the ISsystem.
        /// </summary>
        public void ISsytem()
        {
        }
        
        bool saveable;
        
        public bool isSaveable
        {
            get { return saveable; }
            set { saveable = value; }
        }

        public void checkFile()
        {
#if WINDOWS
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
#else
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
#endif
            using (storage)
            {
                if (storage.FileExists("CMHS.sfs") || storage.FileExists("BeyondSpaceScore.sfs") || storage.FileExists("H1N1HighScore.sfs"))
                {
                    isSaveable = true;
                }
                else if (!storage.FileExists("CMHS.sfs") || !storage.FileExists("BeyondSpaceScore.sfs") || !storage.FileExists("H1N1HighScore.sfs"))
                {
                    isSaveable = false;
                }
            }

        }

        /// <summary>
        /// Preforms the actual saving operation of the ISsystem.
        /// </summary>
        /// <param name="data">takes the data struct to be saved.</param>
        /// <param name="fileName">Name of the file to be opened.</param>
        public static void SaveHighScores(ISsystemData data, string fileName)
        {
#if WINDOWS
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
#else
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
#endif
            
            
            using (storage)
            {
                using (IsolatedStorageFileStream isfs =
                    storage.CreateFile(fileName))
                {
                    using (StreamWriter writer = new StreamWriter(isfs))
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            // Write the scores
                            writer.WriteLine(data.playerName[i]);
                            writer.WriteLine(data.score[i].ToString());
                            writer.WriteLine(data.level[i].ToString());
                            writer.WriteLine(data.playTime[i].ToString());
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Preforms the actual saving operation for H1N1.
        /// </summary>
        /// <param name="score">takes the score to be saved.</param>
        /// <param name="lives">takes the lives to be saved.</param>
        /// <param name="fileName">Name of the file to be opened.</param>
        public static void SaveHighScores(int score, int lives, string fileName)
        {
#if WINDOWS
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
#else
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
#endif
            using (storage)
            {
                using (IsolatedStorageFileStream isfs =
                    storage.CreateFile(fileName))
                {
                    using (StreamWriter writer = new StreamWriter(isfs))
                    {
                        writer.WriteLine(score.ToString());
                        writer.WriteLine(lives.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Opens and loads the highscore data into the system.
        /// </summary>
        /// <param name="fileName">Name of the file to be opened.</param>
        /// <param name="size">Creates the size of the data structure to accept the loaded information.</param>
        public static ISsystemData LoadHighScores(string fileName, int size)
        {

#if WINDOWS
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
#else
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
#endif
            ISsystemData data = new ISsystemData(size);
            using (storage)
            {
                if (storage.FileExists(fileName))
                {

                    using (IsolatedStorageFileStream isfs =
                        storage.OpenFile(fileName, FileMode.Open))
                    {

                        using (StreamReader reader = new StreamReader(isfs))
                        {

                            // Read the highscores
                            int i = 0;

                            while (!reader.EndOfStream)
                            {
                                string name = reader.ReadLine();
                                int score = int.Parse(reader.ReadLine());
                                int level = int.Parse(reader.ReadLine());
                                int played = int.Parse(reader.ReadLine());
                                data.playerName[i] = name;
                                data.score[i] = score;
                                data.level[i] = level;
                                data.playTime[i] = played;
                                i++;
                            }
                            //reader.Close();
                        }
                    }
                }
                else if (!storage.FileExists(fileName))
                {
                    data = new ISsystemData(5);
                    data.playerName[0] = "Dan";
                    data.level[0] = 1;
                    data.score[0] = 0;
                    data.playTime[0] = 0;

                    data.playerName[1] = "James";
                    data.level[1] = 1;
                    data.score[1] = 0;
                    data.playTime[1] = 0;

                    data.playerName[2] = "J W";
                    data.level[2] = 1;
                    data.score[2] = 0;
                    data.playTime[2] = 1;

                    data.playerName[3] = "Zach";
                    data.level[3] = 1;
                    data.score[3] = 0;
                    data.playTime[3] = 0;


                    data.playerName[4] = "Joe";
                    data.level[4] = 1;
                    data.score[4] = 0;
                    data.playTime[4] = 0;

                    ISsystem.SaveHighScores(data, fileName);
                }
                return (data);
            }
        }

        /// <summary>
        /// Handles the saving function for H1N1.
        /// </summary>
        /// <param name="score">Takes the score to save for the next level of H1N1.</param>
        /// <param name="level">Takes the level data to be saved for the next level of H1N1.</param>
        /// <param name="fileName">Takes the name of the file to be saved to.</param>
        public static void SaveScore(int score, int lives, string fileName)
        {
            SaveHighScores(score, lives, fileName);
        }

        /// <summary>
        /// Handles the loading function for H1N1.
        /// </summary>
        /// <param name="fileName">Takes the name of the file to be loaded.</param>
        public static ISsystemData LoadInfo(string fileName)
        {
#if WINDOWS
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
#else
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
#endif
            ISsystemData saveData = new ISsystemData(1);
            using (storage)
            {
                if (storage.FileExists(fileName))
                {

                    using (IsolatedStorageFileStream isfs =
                        storage.OpenFile(fileName, FileMode.Open))
                    {

                        using (StreamReader reader = new StreamReader(isfs))
                        {

                            // Read the highscores
                            int i = 0;

                            while (!reader.EndOfStream)
                            {
                                saveData.score[i] = int.Parse(reader.ReadLine());
                                saveData.lives[i] = int.Parse(reader.ReadLine());
                            }
                        }
                    }
                }
                return (saveData);
            }
        }
    }
}
