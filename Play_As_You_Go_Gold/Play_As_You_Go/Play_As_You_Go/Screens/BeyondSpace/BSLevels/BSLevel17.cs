using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;

namespace Play_As_You_Go
{
    public class BSLevel17 : GameScreen
    {
        #region Fields

        SpriteFont TitleFont;
        Texture2D spStar;
        Texture2D newSheet;
        Texture2D explosionSheet;

        SoundEffect music;
        SoundEffectInstance backgroundMusic;

        SoundEffect warning;
        SoundEffectInstance warningInst;

        SoundEffect powerUp1;
        SoundEffectInstance powerUp1Inst;

        SoundEffect powerUp2;
        SoundEffectInstance powerUp2Inst;

        SoundEffect shot;
        SoundEffectInstance gunShot;

        SoundEffect hit;
        SoundEffectInstance hitExplosion;

        GameObject[] EnemyType1 = new GameObject[10];
        GameObject[] EnemyType2 = new GameObject[7];
        GameObject[] EnemyType3 = new GameObject[5];
        GameObject[] EnemyType4 = new GameObject[5];
        GameObject[] EnemyType5 = new GameObject[75];
        GameObject[] enemyBullets = new GameObject[30];

        List<GameObject> shootingEnemies;

        GameObject[] explosion = new GameObject[77];
        double[] explosionTime = new double[77];

        //Bonus Objects
        GameObject missle;
        GameObject missleExplosion;
        double missleExplosionTime = 1116;
        int numMissles = 0;
        int missleBonus = 3000;
        int toMissleBonus = 3000;

        int blasterBonus = 2000;
        int toBlasterBonus = 2000;
        bool doubleBlaster = false;

        GameObject[] stars = new GameObject[50];

        //Lists to determine where enemies should move to.
        //Only used for EnemyType5.  The array
        //size determines the number of enemies that can be
        //present on screen of that type using path finding.
        int numPaths = 15;
        List<Vector2>[] path = new List<Vector2>[15];

        //variables used to help spawn enemies
        int e1 = 0;
        int e2 = 0;
        int e3 = 0;
        int e4 = 0;
        int e5 = 0;

        //timer variables used for spawning
        int span1 = 4;
        double toNextSpawn1 = 4;

        double span2 = 5.2;
        double toNextSpawn2 = 5.2;

        double span3 = 7.4;
        double toNextSpawn3 = 7.4;

        double span4 = 8.6;
        double toNextSpawn4 = 8.6;

        double span5 = 3.7;
        int num5Count = 0;  //control variable for non-swarm enemies
        double toNextSpawn5 = 3.7;

        int spanSwarm = 20;
        double toNextSwarm = 20;
        int swarmCount = 3; //control variable for swarm enemies
        int swarmTimes = 0;
        int swarmStart; //starting variable holding the index of the first enemy
        List<GameObject> swarm;
        //used to fire a bullet during enemy swarm
        double toRandomFire = 5;
        int spanRandomFire = 5;
        int fireIndex = 0; //swarm enemy that will fire

        //timing variables used to have enemies retarget the player
        double seekSpan = 0.5d;
        double toNextSeek = 0.5d;

        //timing variable for level end notification
        double end = 3;
        bool bEnd = false;

        //determines the number of times the swirl
        //formation is spawned
        Random spawn = new Random();

        float playerMoveX;
        float playerMoveY;
        GameObject player;
        GameObject[] bullets = new GameObject[8];

        int score;
        int lives = 3;

        int level = 17;

        const string HighScore = "BeyondSpaceScore.sfs";
        const string GameName = "Beyond Space";
        ISsystemData Data;

        ISsystem isolatedStorage = new ISsystem();
        
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public BSLevel17(int sc, int life, bool blaster, int numMissles, int bBonus, int mBonus,
            int toBBonus, int toMBonus)
        {
            score = sc;
            lives = life;
            TransitionOnTime = TimeSpan.FromSeconds(2);
            TransitionOffTime = TimeSpan.FromSeconds(1);
            toBlasterBonus = toBBonus;
            toMissleBonus = toMBonus;
            blasterBonus = bBonus;
            missleBonus = mBonus;
            doubleBlaster = blaster;
            this.numMissles = numMissles;
        }

        public override void Initialize()
        {
            //To do: Set Spawn times to the number of formations
            //required.  These are level dependent.  EnemyType3 and
            //EnemyType4 must have at least 10 elements.  If more than
            //10 elements are used, you must spawn the additional
            //enemies manually.

            isolatedStorage.checkFile();
            if (isolatedStorage.isSaveable)
            {
                Data = ISsystem.LoadHighScores(HighScore, 5);
                Data.gameTitle[0] = GameName;
            }

            missle = new GameObject(newSheet, new Rectangle(1162, 54, 126, 64), 1, true, 0);
            missle.Active = false;
            missle.Scale = 0.25f;

            missleExplosion = new GameObject(explosionSheet, new Rectangle(0, 83, 64, 64), 32, true, 0);
            missleExplosion.AnimationSpeed = TimeSpan.FromMilliseconds(36);
            missleExplosion.Active = false;
            missleExplosion.Scale = 2f;

            for (int i = 0; i < explosion.Length; i++)
            {
                explosion[i] = new GameObject(explosionSheet, new Rectangle(0, 83, 64, 64), 32, true, 0);
                explosion[i].AnimationSpeed = TimeSpan.FromMilliseconds(36);
                explosion[i].Active = false;
                explosionTime[i] = 1116;
            }
            player = new GameObject(newSheet, new Rectangle(12, 13, 499, 512), 1, true, 0);
            player.Position = new Vector2(ScreenManager.Viewport.Width / 2,
                            (ScreenManager.Viewport.Height / 2) + 200);
            player.Scale = 0.1f;
            playerMoveX = 5f;
            playerMoveY = 5f;
            for (int i = 0; i < bullets.Length; i++)
            {
                bullets[i] = new GameObject(newSheet, new Rectangle(1078, 56, 10, 128), 1, true, 0);
                bullets[i].Active = false;
                bullets[i].Scale = 0.15f;
            }//end for

            for (int i = 0; i < EnemyType1.Length; i++)
            {
                EnemyType1[i] = new GameObject(newSheet, new Rectangle(13, 551, 499, 512), 1, true, 1);
                EnemyType1[i].Scale = 0.1f;
                EnemyType1[i].Active = false;
                EnemyType1[i].Color = Color.DarkOliveGreen;
            }//end for
            for (int i = 0; i < EnemyType2.Length; i++)
            {
                EnemyType2[i] = new GameObject(newSheet, new Rectangle(13, 551, 499, 512), 1, true, 1);
                EnemyType2[i].Scale = 0.1f;
                EnemyType2[i].Active = false;
                EnemyType2[i].Color = Color.DarkKhaki;
            }//end for
            for (int i = 0; i < EnemyType3.Length; i++)
            {
                EnemyType3[i] = new GameObject(newSheet, new Rectangle(13, 551, 499, 512), 1, true, 1);
                EnemyType3[i].Scale = 0.1f;
                EnemyType3[i].Active = false;
                EnemyType3[i].Color = Color.BlanchedAlmond;
            }//end for
            for (int i = 0; i < EnemyType4.Length; i++)
            {
                EnemyType4[i] = new GameObject(newSheet, new Rectangle(13, 551, 499, 512), 1, true, 1);
                EnemyType4[i].Scale = 0.1f;
                EnemyType4[i].Active = false;
                EnemyType4[i].Color = Color.DarkSlateGray;
            }//end for
            for (int i = 0; i < EnemyType5.Length; i++)
            {
                EnemyType5[i] = new GameObject(newSheet, new Rectangle(13, 551, 499, 512), 1, true, 1);
                EnemyType5[i].Scale = 0.1f;
                EnemyType5[i].Active = false;
            }//end for

            shootingEnemies = new List<GameObject>(EnemyType5.Length % 20);
            for (int i = 0; i < EnemyType5.Length % 20; i++)
            {
                shootingEnemies.Add(EnemyType5[i]);
            }//end for

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i] = new GameObject(spStar, new Rectangle(0, 0, 1, 1), 1, true, 0);
                stars[i].Active = true;
                stars[i].Position = new Vector2(spawn.Next(ScreenManager.Viewport.Width), spawn.Next(ScreenManager.Viewport.Height));
                stars[i].Velocity = new Vector2(0, spawn.Next(2));
                if (stars[i].Velocity.Y < 1)
                {
                    stars[i].Velocity = new Vector2(0, 0.5f);
                }//end if
            }//end for
            for (int i = 0; i < enemyBullets.Length; i++)
            {
                enemyBullets[i] = new GameObject(newSheet, new Rectangle(1078, 56, 10, 128), 1, true, 0);
                enemyBullets[i].Active = false;
                enemyBullets[i].Scale = 0.15f;
            }//end for
            for (int i = 0; i < numPaths; i++)
            {
                path[i] = new List<Vector2>(5);
            }//end for

            
            swarmStart = shootingEnemies.Count;
            swarm = new List<GameObject>(20);

            backgroundMusic = music.CreateInstance();
            gunShot = shot.CreateInstance();
            gunShot.Volume = 0.5f;
            hitExplosion = hit.CreateInstance();
            powerUp1Inst = powerUp1.CreateInstance();
            powerUp2Inst = powerUp2.CreateInstance();
            warningInst = warning.CreateInstance();
            warningInst.Play();

            powerUp1Inst.Volume = 0.4f;
            powerUp2Inst.Volume = 0.4f;
            warningInst.Volume = 0.4f;
            hitExplosion.Volume = 0.3f;
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            newSheet = content.Load<Texture2D>("Images/BSImages/BeyondSpaceSpriteSheet");
            TitleFont = content.Load<SpriteFont>("Fonts/BeyondSpaceFont/HUDFont");
            spStar = content.Load<Texture2D>("Images/singlePixel");
            explosionSheet = content.Load<Texture2D>("Images/BSImages/BSExplosionSpriteSheet");

            music = content.Load<SoundEffect>("Audio/BSAudio/outer7");
            shot = content.Load<SoundEffect>("Audio/BSAudio/Shot4");
            hit = content.Load<SoundEffect>("Audio/BSAudio/Explosion");
            warning = content.Load<SoundEffect>("Audio/BSAudio/AlienWarning");
            powerUp1 = content.Load<SoundEffect>("Audio/BSAudio/PowerUp3");
            powerUp2 = content.Load<SoundEffect>("Audio/BSAudio/PowerUp1");
        }

        public override void UnloadContent()
        {
            backgroundMusic = null;
        }

        public override void Update(GameTime gameTime, bool covered)
        {
            if (missle.Active)
                missle.Update(gameTime);

            if (missleExplosion.Active)
                missleExplosion.Update(gameTime);

            for (int i = 0; i < explosion.Length; i++)
            {
                explosion[i].Update(gameTime);
            }//end if

            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = ScreenManager.InputSystem.currentKeyboardState[playerIndex];
            GamePadState gamepadState = ScreenManager.InputSystem.currentGamePadState[playerIndex];

            bool gamePadDisconnected = !gamepadState.IsConnected && ScreenManager.InputSystem.gamePadWasConnected[playerIndex];

            if (MediaPlayer.GameHasControl)
            {
                backgroundMusic.Volume = 0.2f;
                backgroundMusic.Play();
            }
            else
                backgroundMusic.Pause();

#if XBOX

            if (ScreenManager.InputSystem.PauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                backgroundMusic.Stop();
                warningInst.Stop();
                ScreenState = ScreenState.Frozen;
                ScreenManager.AddScreen(new PauseScreen(this), ControllingPlayer);
            }//end if
            else
            {
                ScreenState = ScreenState.Active;
            }//end else

#endif

#if WINDOWS

            if (ScreenManager.InputSystem.PauseGame(ControllingPlayer))
            {
                backgroundMusic.Stop();
                warningInst.Stop();
                ScreenState = ScreenState.Frozen;
                ScreenManager.AddScreen(new PauseScreen(this), ControllingPlayer);
            }//end if
            else
            {
                ScreenState = ScreenState.Active;
            }//end else

#endif

            /************************
             * Spawing logic
             * *********************/

            if (missleExplosion.Active)
            {
                missleExplosionTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
                if (missleExplosionTime < 0)
                {
                    missleExplosion.Active = false;
                    missleExplosionTime = 1116;
                }
            }//end if

            for (int i = 0; i < explosionTime.Length; i++)
            {
                if (explosion[i].Active)
                {
                    explosionTime[i] -= gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (explosionTime[i] < 0)
                    {
                        explosion[i].Active = false;
                        explosionTime[i] = 1116;
                    }//end if
                }//end if
            }//end for

            toNextSpawn1 -= gameTime.ElapsedGameTime.TotalSeconds;
            if (toNextSpawn1 < 0)
            {
                SpawnEnemy1();
                toNextSpawn1 = span1;
            }//end if

            toNextSpawn2 -= gameTime.ElapsedGameTime.TotalSeconds;
            if (toNextSpawn2 < 0)
            {
                SpawnEnemy2();
                toNextSpawn2 = span2;
            }//end if

            toNextSpawn3 -= gameTime.ElapsedGameTime.TotalSeconds;
            if (toNextSpawn3 < 0 && e3 < EnemyType3.Length)
            {
                SpawnEnemy3(new Vector2((float)spawn.Next(ScreenManager.Viewport.Width - EnemyType3[e3].ActualWidth), 0));
                toNextSpawn3 = span3;
            }//end if

            toNextSpawn4 -= gameTime.ElapsedGameTime.TotalSeconds;
            if (toNextSpawn4 < 0 && e4 < EnemyType4.Length)
            {
                SpawnEnemy4(new Vector2((float)spawn.Next(ScreenManager.Viewport.Width - EnemyType4[e4].ActualWidth), 0));
                toNextSpawn4 = span4;
            }//end if

            toNextSpawn5 -= gameTime.ElapsedGameTime.TotalSeconds;

            if (toNextSpawn5 < 0 && num5Count < shootingEnemies.Count)
            {
                shootingEnemies[num5Count].Position = new Vector2(ScreenManager.Viewport.Width / 2, -32f);
                shootingEnemies[num5Count].Active = true;
                toNextSpawn5 = span5;
                populateNodes(path[num5Count], 5);
                moveToNode(shootingEnemies[num5Count], path[num5Count], 3);
                e5++;
                num5Count++;
            }//end if
            toNextSwarm -= gameTime.ElapsedGameTime.TotalSeconds;
            if (toNextSwarm < 0)
            {
                SpawnSwarm();
                toNextSwarm = spanSwarm;
            }//end if


            /************************
             * Update Enemy Movement
             * **********************/

            for (int i = 0; i < bullets.Length; i++)
            {
                if (bullets[i].Active)
                    bullets[i].Update(gameTime);
            }//end for

            for (int i = 0; i < EnemyType1.Length; i++)
            {
                if (EnemyType1[i].Active)
                    EnemyType1[i].Update(gameTime);
            }//end for

            for (int i = 0; i < EnemyType2.Length; i++)
            {
                if (EnemyType2[i].Active)
                    EnemyType2[i].Update(gameTime);
            }//end for

            /*******************************************
             * Makes the two enemy types seek the player
             * ****************************************/
            toNextSeek -= gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = 0; i < EnemyType3.Length; i++)
            {
                if (EnemyType3[i].Active)
                    EnemyType3[i].Update(gameTime);
                if (toNextSeek < 0)
                {
                    createVelocity(EnemyType3[i], 2);
                    if (EnemyType3[i].Velocity.Y < 1f)
                    {
                        EnemyType3[i].Velocity.Y = 1f;
                    }//end if
                }//end if
            }//end for

            for (int i = 0; i < EnemyType4.Length; i++)
            {
                if (EnemyType4[i].Active)
                    EnemyType4[i].Update(gameTime);
                if (toNextSeek < 0)
                {
                    createVelocity(EnemyType4[i], 2);
                    if (EnemyType4[i].Velocity.Y < 1f)
                    {
                        EnemyType4[i].Velocity.Y = 1f;
                    }//end if
                }//end if
            }//end for
            //reset seeking behavior counter
            if (toNextSeek < 0)
                toNextSeek = seekSpan;

            /****************************************
             * EnemyType5 moving behavior
             * *************************************/
            for (int i = 0; i < shootingEnemies.Count; i++)
            {
                if (shootingEnemies[i].Active)
                {
                    shootingEnemies[i].Update(gameTime);
                    checkDestination(shootingEnemies[i], path[i]);
                }//end if
            }//end for

            /*****************************
             * Update Star position
             * **************************/
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].Update(gameTime);
            }//end for

            /****************************************
             * Check Swarming enemies
             * *************************************/
            for (int i = 0; i < swarm.Count; i++)
            {
                if (swarm[i].Active)
                    swarm[i].Update(gameTime);
            }//end for

            toRandomFire -= gameTime.ElapsedGameTime.TotalSeconds;
            if (toRandomFire < 0)
            {
                fireIndex = spawn.Next(20);
                if (swarm.Count > 0 && swarm.Count <= 20)
                {
                    if (swarm[fireIndex].Active)
                        enemyFire(swarm[fireIndex]);
                }//end if
                toRandomFire = spanRandomFire;
            }//end if

            /********************************************
             * Check enemy bullets
             * *****************************************/
            for (int i = 0; i < enemyBullets.Length; i++)
            {
                if (enemyBullets[i].Active)
                    enemyBullets[i].Update(gameTime);
            }//end for

            /********************************************
             * Bonus Check
             * *****************************************/
            if (toMissleBonus - score <= 0)
            {
                numMissles++;

                missleBonus += missleBonus;
                if (numMissles > 3)
                {
                    numMissles = 3;
                }
                else
                {
                    powerUp1Inst.Play();
                }
            }//end if
            toMissleBonus = missleBonus;

            if (toBlasterBonus - score <= 0)
            {
                if (!doubleBlaster)
                    powerUp2Inst.Play();

                blasterBonus += blasterBonus;
                doubleBlaster = true;
            }//end if
            toBlasterBonus = blasterBonus;

            /******************
             * collision checks
             * ****************/
            checkBulletBounds();
            checkBulletCollision();
            checkEnemyBounds();
            checkEnemyBullets();
            checkPlayerEnemy();
            checkSwarmBounds();
            checkStars();
            UpdateInput();
            checkMissleBounds();
            checkMissleCollsion();
            checkMissleExplosion();
            checkBlasterUpgrade();
            /******************
             * Leveling Check
             * ***************/
            levelWon();
            if (lives <= 0)
                GameOver();

            base.Update(gameTime, covered);

        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].Draw(gameTime, spriteBatch);
            }//end for
            

            if (bEnd)
            {
                spriteBatch.DrawString(TitleFont, "Level " + level + " complete!", new Vector2(550, 360), Color.Firebrick);
                end -= gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (missle.Active)
                missle.Draw(gameTime, spriteBatch);

            player.Draw(gameTime, spriteBatch);

            for (int i = 0; i < bullets.Length; i++)
            {
                if (bullets[i].Active)
                    bullets[i].Draw(gameTime, spriteBatch);
            }//end for

            for (int i = 0; i < EnemyType1.Length; i++)
            {
                if (EnemyType1[i].Active)
                    EnemyType1[i].Draw(gameTime, spriteBatch);
            }//end for
            for (int i = 0; i < EnemyType2.Length; i++)
            {
                if (EnemyType2[i].Active)
                    EnemyType2[i].Draw(gameTime, spriteBatch);
            }//end for
            for (int i = 0; i < EnemyType3.Length; i++)
            {
                if (EnemyType3[i].Active)
                    EnemyType3[i].Draw(gameTime, spriteBatch);
            }//end for
            for (int i = 0; i < EnemyType4.Length; i++)
            {
                if (EnemyType4[i].Active)
                    EnemyType4[i].Draw(gameTime, spriteBatch);
            }//end for
            for (int i = 0; i < EnemyType5.Length; i++)
            {
                if (EnemyType5[i].Active)
                    EnemyType5[i].Draw(gameTime, spriteBatch);
            }//end for
            for (int i = 0; i < enemyBullets.Length; i++)
            {
                if (enemyBullets[i].Active)
                    enemyBullets[i].Draw(gameTime, spriteBatch);
            }//end for
            for (int i = 0; i < explosion.Length; i++)
            {
                if (explosion[i].Active)
                    explosion[i].Draw(gameTime, spriteBatch);
            }//end for

            if (missleExplosion.Active)
                missleExplosion.Draw(gameTime, spriteBatch);

            //HUD
            spriteBatch.DrawString(TitleFont, "Score", new Vector2(64, 32), Color.Firebrick);
            spriteBatch.DrawString(TitleFont, score.ToString(), new Vector2(150, 32), Color.Firebrick);
            spriteBatch.DrawString(TitleFont, "Lives", new Vector2(300, 32), Color.Firebrick);
            spriteBatch.DrawString(TitleFont, lives.ToString(), new Vector2(375, 32), Color.Firebrick);
            spriteBatch.DrawString(TitleFont, "Missles", new Vector2(400, 32), Color.Firebrick);
            spriteBatch.DrawString(TitleFont, numMissles.ToString(), new Vector2(500, 32), Color.Firebrick);

            spriteBatch.End();
        }

        #region Collision

        #region checkBulletCollsion
        /// <summary>
        /// Determines if any bullet collided with any enemy.
        /// </summary>
        private void checkBulletCollision()
        {
            for (int i = 0; i < bullets.Length; i++)
            {
                if (bullets[i].Active)
                {
                    //check bullet collision against each object
                    for (int j = 0; j < EnemyType1.Length; j++)
                    {
                        if (bullets[i].BoundingRect.Intersects(EnemyType1[j].BoundingRect))
                        {
                            CreateExplosion(EnemyType1[j]);
                            EnemyType1[j].Active = false;
                            bullets[i].Active = false;
                            EnemyType1[j].Position = new Vector2(-32f, -32f);
                            hitExplosion.Play();
                            score += 10;
                        }//end for
                    }

                    for (int j = 0; j < EnemyType2.Length; j++)
                    {
                        if (bullets[i].BoundingRect.Intersects(EnemyType2[j].BoundingRect) && bullets[i].Active)
                        {
                            CreateExplosion(EnemyType2[j]);
                            EnemyType2[j].Active = false;
                            bullets[i].Active = false;
                            EnemyType2[j].Position = new Vector2(-32f, -32f);
                            hitExplosion.Play();
                            score += 25;
                        }
                    }//end for

                    for (int j = 0; j < EnemyType3.Length; j++)
                    {
                        if (bullets[i].BoundingRect.Intersects(EnemyType3[j].BoundingRect) && bullets[i].Active)
                        {
                            CreateExplosion(EnemyType3[j]);
                            EnemyType3[j].Active = false;
                            bullets[i].Active = false;
                            EnemyType3[j].Position = new Vector2(-32f, -32f);
                            hitExplosion.Play();
                            score += 30;
                        }
                    }//end for

                    for (int j = 0; j < EnemyType4.Length; j++)
                    {
                        if (bullets[i].BoundingRect.Intersects(EnemyType4[j].BoundingRect) && bullets[i].Active)
                        {
                            CreateExplosion(EnemyType4[j]);
                            EnemyType4[j].Active = false;
                            bullets[i].Active = false;
                            EnemyType4[j].Position = new Vector2(-32f, -32f);
                            hitExplosion.Play();
                            score += 30;
                        }
                    }//end for

                    for (int j = 0; j < EnemyType5.Length; j++)
                    {
                        if (bullets[i].BoundingRect.Intersects(EnemyType5[j].BoundingRect) && bullets[i].Active)
                        {
                            CreateExplosion(EnemyType5[j]);
                            EnemyType5[j].Active = false;
                            bullets[i].Active = false;
                            EnemyType5[j].Position = new Vector2(-32f, -32f);
                            hitExplosion.Play();
                            score += 35;
                        }
                    }//end for
                }//end if
            }//end for
        }//end checkBulletCollision

        #endregion

        #region checkBulletsBounds
        /// <summary>
        /// Determines if a bullet is off screen
        /// </summary>
        private void checkBulletBounds()
        {
            for (int i = 0; i < bullets.Length; i++)
            {
                if (bullets[i].Active)
                {
                    if (bullets[i].Position.Y < 0)
                    {
                        bullets[i].Position = new Vector2(-32f, -32f);
                        bullets[i].Active = false;
                    }//end if
                }//end if
            }//end for
        }//end checkBulletBounds

        #endregion

        #region checkEnemyBullets

        /// <summary>
        /// Determines if enemy bullets hit the player.
        /// </summary>
        private void checkEnemyBullets()
        {
            for (int i = 0; i < enemyBullets.Length; i++)
            {
                if (enemyBullets[i].Active)
                {
                    if (enemyBullets[i].BoundingRect.Intersects(player.BoundingRect))
                    {
                        enemyBullets[i].Active = false;
                        enemyBullets[i].Position = new Vector2(-32f, -32f);
                        CreateExplosion(player);
                        lives -= 1;
                        player.Position = new Vector2(ScreenManager.Viewport.Width / 2,
                            ScreenManager.Viewport.Height - player.ActualHeight - 30);
                        hitExplosion.Play();
                        doubleBlaster = false;
                    }//end if
                    if (enemyBullets[i].Position.Y > ScreenManager.Viewport.Height)
                    {
                        enemyBullets[i].Active = false;
                    }//end if
                    if (enemyBullets[i].Velocity.Y < 1)
                        enemyBullets[i].Active = false;
                }//end if
            }//end for
        }//end checkEnemyBullets

        #endregion

        #region checkPlayerEnemy

        /// <summary>
        /// Checks if an enemy hit the player.
        /// </summary>
        private void checkPlayerEnemy()
        {
            for (int i = 0; i < EnemyType1.Length; i++)
            {
                if (EnemyType1[i].Active)
                {
                    if (EnemyType1[i].BoundingRect.Intersects(player.BoundingRect))
                    {
                        EnemyType1[i].Active = false;
                        CreateExplosion(EnemyType1[i]);
                        CreateExplosion(player);
                        EnemyType1[i].Position = new Vector2(-32f, -32f);
                        lives -= 1;
                        player.Position = new Vector2(ScreenManager.Viewport.Width / 2,
                            ScreenManager.Viewport.Height - player.ActualHeight - 30);
                        doubleBlaster = false;
                        hitExplosion.Play();
                    }
                }//end if
            }//end for

            for (int i = 0; i < EnemyType2.Length; i++)
            {
                if (EnemyType2[i].Active)
                {
                    if (EnemyType2[i].BoundingRect.Intersects(player.BoundingRect))
                    {
                        EnemyType2[i].Active = false;
                        CreateExplosion(EnemyType2[i]);
                        CreateExplosion(player);
                        EnemyType2[i].Position = new Vector2(-32f, -32f);
                        lives -= 1;
                        player.Position = new Vector2(ScreenManager.Viewport.Width / 2,
                            ScreenManager.Viewport.Height - player.ActualHeight - 30);
                        doubleBlaster = false;
                        hitExplosion.Play();
                    }
                }//end if
            }//end for

            for (int i = 0; i < EnemyType3.Length; i++)
            {
                if (EnemyType3[i].Active)
                {
                    if (EnemyType3[i].BoundingRect.Intersects(player.BoundingRect))
                    {
                        EnemyType3[i].Active = false;
                        CreateExplosion(EnemyType3[i]);
                        CreateExplosion(player);
                        EnemyType3[i].Position = new Vector2(-32f, -32f);
                        lives -= 1;
                        player.Position = new Vector2(ScreenManager.Viewport.Width / 2,
                            ScreenManager.Viewport.Height - player.ActualHeight - 30);
                        doubleBlaster = false;
                        hitExplosion.Play();
                    }
                }//end if
            }//end for

            for (int i = 0; i < EnemyType4.Length; i++)
            {
                if (EnemyType4[i].Active)
                {
                    if (EnemyType4[i].BoundingRect.Intersects(player.BoundingRect))
                    {
                        EnemyType4[i].Active = false;
                        CreateExplosion(EnemyType4[i]);
                        CreateExplosion(player);
                        EnemyType4[i].Position = new Vector2(-32f, -32f);
                        lives -= 1;
                        player.Position = new Vector2(ScreenManager.Viewport.Width / 2,
                            ScreenManager.Viewport.Height - player.ActualHeight - 30);
                        doubleBlaster = false;
                        hitExplosion.Play();
                    }
                }//end if
            }//end for

            for (int i = 0; i < EnemyType5.Length; i++)
            {
                if (EnemyType5[i].BoundingRect.Intersects(player.BoundingRect))
                {
                    EnemyType5[i].Active = false;
                    CreateExplosion(EnemyType5[i]);
                    CreateExplosion(player);
                    EnemyType5[i].Position = new Vector2(-32f, -32f);
                    lives -= 1;
                    player.Position = new Vector2(ScreenManager.Viewport.Width / 2,
                        ScreenManager.Viewport.Height - player.ActualHeight - 30);
                    doubleBlaster = false;
                    hitExplosion.Play();
                }
            }//end for

        }//end checkPlayerEnemy

        #endregion

        #region checkEnemyBounds

        /// <summary>
        /// Determines if an enemy goes off screen and removes it.
        /// If the enemy swirl formation falls out of bounds, it
        /// reflects them.
        /// </summary>
        private void checkEnemyBounds()
        {
            for (int i = 0; i < EnemyType1.Length; i++)
            {
                if (EnemyType1[i].Position.Y > ScreenManager.Viewport.Height ||
                    EnemyType1[i].Position.Y < 0f)
                {
                    EnemyType1[i].Active = false;
                    EnemyType1[i].Position = new Vector2(-32f, -32f);
                }//end if
                if (EnemyType1[i].Position.X > ScreenManager.Viewport.Width - EnemyType1[i].ActualWidth ||
                    EnemyType1[i].Position.X < 0f)
                {
                    EnemyType1[i].Velocity.X = -EnemyType1[i].Velocity.X;
                }//end if
            }//end for

            for (int i = 0; i < EnemyType2.Length; i++)
            {
                if (EnemyType2[i].Position.Y > ScreenManager.Viewport.Height ||
                    EnemyType2[i].Position.Y < 0)
                {
                    EnemyType2[i].Active = false;
                    EnemyType2[i].Position = new Vector2(-32f, -32f);
                }//end if
                if (EnemyType2[i].Position.X > ScreenManager.Viewport.Width - EnemyType2[i].ActualWidth ||
                    EnemyType2[i].Position.X < 0f)
                {
                    EnemyType2[i].Velocity.X = -EnemyType2[i].Velocity.X;
                }//end if
            }//end for

            for (int i = 0; i < EnemyType3.Length; i++)
            {
                if (EnemyType3[i].Position.Y > ScreenManager.Viewport.Height ||
                    EnemyType3[i].Position.Y < 0)
                {
                    EnemyType3[i].Active = false;
                    EnemyType3[i].Position = new Vector2(-32f, -32f);
                }//end if

                if (EnemyType3[i].Position.X > (int)(ScreenManager.Viewport.Width - EnemyType3[i].ActualWidth))
                {
                    EnemyType3[i].Velocity.X = -EnemyType3[i].Velocity.X;
                }//end if

                if (EnemyType3[i].Position.X <= 0)
                {
                    EnemyType3[i].Velocity.X = -EnemyType3[i].Velocity.X;
                }//end if
            }//end for

            for (int i = 0; i < EnemyType4.Length; i++)
            {
                if (EnemyType4[i].Position.Y > ScreenManager.Viewport.Height ||
                    EnemyType4[i].Position.Y < 0)
                {
                    EnemyType4[i].Active = false;
                    EnemyType4[i].Position = new Vector2(-32f, -32f);
                }//end if

                if (EnemyType4[i].Position.X > (int)(ScreenManager.Viewport.Width - EnemyType4[i].ActualWidth))
                {
                    EnemyType4[i].Velocity.X = -EnemyType4[i].Velocity.X;
                }//end if

                if (EnemyType4[i].Position.X < 0)
                {
                    EnemyType4[i].Velocity.X = -EnemyType4[i].Velocity.X;
                }//end if
            }//end for

            for (int i = 0; i < EnemyType5.Length; i++)
            {
                if (EnemyType5[i].Position.Y > ScreenManager.Viewport.Height)
                {
                    EnemyType5[i].Active = false;
                    EnemyType5[i].Position = new Vector2(-32f, -32f);
                }//end if
                if (EnemyType5[i].Position.Y < -360f)
                {
                    EnemyType5[i].Active = false;
                    EnemyType5[i].Position = new Vector2(-32f, -32f);
                }//end if

                if (EnemyType5[i].Position.X > (int)(ScreenManager.Viewport.Width - EnemyType5[i].ActualWidth))
                {
                    EnemyType5[i].Velocity.X = -EnemyType5[i].Velocity.X;
                    if (i < path.Length)
                        if (path[i].Count > 0)
                        {
                            path[i].RemoveAt(0);
                            moveToNode(EnemyType5[i], path[i], 5);
                        }//end if
                }//end if

                if (EnemyType5[i].Position.X < 0)
                {
                    EnemyType5[i].Velocity.X = -EnemyType5[i].Velocity.X;
                    if (i < path.Length)
                        if (path[i].Count > 0)
                        {
                            path[i].RemoveAt(0);
                            moveToNode(EnemyType5[i], path[i], 5);
                        }//end if
                }//end if
            }//end for
        }//end checkEnemyBounds

        #endregion

        #region checkSwarmBounds

        private void checkSwarmBounds()
        {
            for (int i = 0; i < swarm.Count; i++)
            {
                if (swarm[i].Active)
                    if (swarm[i].Position.X >= (ScreenManager.Viewport.Width / 2) + swarm[i].ActualWidth * 10 ||
                        swarm[i].Position.X <= (ScreenManager.Viewport.Width / 2) - swarm[i].ActualWidth * 10)
                    {
                        swarm[i].Velocity.X *= -1f;
                    }//end if
            }//end for
        }//end checkSwarmBounds

        #endregion

        #region checkStars

        private void checkStars()
        {
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i].Position.Y > ScreenManager.Viewport.Height)
                    stars[i].Position.Y = -10f;
            }//end for
        }//end checkStars

        #endregion

        #region checkMissleCollision

        private void checkMissleCollsion()
        {
            for (int i = 0; i < EnemyType1.Length; i++)
            {
                if (missle.Active && missle.BoundingRect.Intersects(EnemyType1[i].BoundingRect))
                {
                    CreateExplosion(EnemyType1[i]);
                    CreateMissleExplosion(missle);
                    EnemyType1[i].Active = false;
                    EnemyType1[i].Position = new Vector2(-32f, -32f);
                    missle.Position = new Vector2(-32f, -32f);
                    missle.Active = false;
                    hitExplosion.Play();
                }
            }//end for

            for (int i = 0; i < EnemyType2.Length; i++)
            {
                if (missle.Active && missle.BoundingRect.Intersects(EnemyType2[i].BoundingRect))
                {
                    CreateExplosion(EnemyType2[i]);
                    CreateMissleExplosion(missle);
                    EnemyType2[i].Active = false;
                    EnemyType2[i].Position = new Vector2(-32f, -32f);
                    missle.Position = new Vector2(-32f, -32f);
                    missle.Active = false;
                    hitExplosion.Play();
                }
            }//end for

            for (int i = 0; i < EnemyType3.Length; i++)
            {
                if (missle.Active && missle.BoundingRect.Intersects(EnemyType3[i].BoundingRect))
                {
                    CreateExplosion(EnemyType3[i]);
                    CreateMissleExplosion(missle);
                    EnemyType3[i].Active = false;
                    EnemyType3[i].Position = new Vector2(-32f, -32f);
                    missle.Position = new Vector2(-32f, -32f);
                    missle.Active = false;
                    hitExplosion.Play();
                }
            }//end for

            for (int i = 0; i < EnemyType4.Length; i++)
            {
                if (missle.Active && missle.BoundingRect.Intersects(EnemyType4[i].BoundingRect))
                {
                    CreateExplosion(EnemyType4[i]);
                    CreateMissleExplosion(missle);
                    EnemyType4[i].Active = false;
                    EnemyType4[i].Position = new Vector2(-32f, -32f);
                    missle.Position = new Vector2(-32f, -32f);
                    missle.Active = false;
                    hitExplosion.Play();
                }
            }//end for

            for (int i = 0; i < EnemyType5.Length; i++)
            {
                if (missle.Active && missle.BoundingRect.Intersects(EnemyType5[i].BoundingRect))
                {
                    CreateMissleExplosion(missle);
                    CreateExplosion(EnemyType5[i]);
                    EnemyType5[i].Active = false;
                    EnemyType5[i].Position = new Vector2(-32f, -32f);
                    missle.Position = new Vector2(-32f, -32f);
                    missle.Active = false;
                    hitExplosion.Play();
                }
            }//end for
        }//end checkMissleCollsion

        #endregion

        #region checkMissleBounds

        private void checkMissleBounds()
        {
            if (missle.Position.Y < 0)
                missle.Active = false;
        }//end checkMissleBounds

        #endregion

        #region checkMissleExplosion

        private void checkMissleExplosion()
        {
            if (missleExplosion.Active)
            {
                for (int i = 0; i < EnemyType1.Count(); i++)
                {
                    if (missleExplosion.BoundingRect.Intersects(EnemyType1[i].BoundingRect))
                    {
                        EnemyType1[i].Active = false;
                        CreateExplosion(EnemyType1[i]);
                        EnemyType1[i].Position = new Vector2(-32f, -32f);
                        hitExplosion.Play();
                    }//end if
                }//end for

                for (int i = 0; i < EnemyType2.Count(); i++)
                {
                    if (missleExplosion.BoundingRect.Intersects(EnemyType2[i].BoundingRect))
                    {
                        EnemyType2[i].Active = false;
                        CreateExplosion(EnemyType2[i]);
                        EnemyType2[i].Position = new Vector2(-32f, -32f);
                        hitExplosion.Play();
                    }//end if
                }//end for

                for (int i = 0; i < EnemyType3.Count(); i++)
                {
                    if (missleExplosion.BoundingRect.Intersects(EnemyType3[i].BoundingRect))
                    {
                        EnemyType3[i].Active = false;
                        CreateExplosion(EnemyType3[i]);
                        EnemyType3[i].Position = new Vector2(-32f, -32f);
                        hitExplosion.Play();
                    }//end if
                }//end for

                for (int i = 0; i < EnemyType4.Count(); i++)
                {
                    if (missleExplosion.BoundingRect.Intersects(EnemyType4[i].BoundingRect))
                    {
                        EnemyType4[i].Active = false;
                        CreateExplosion(EnemyType4[i]);
                        EnemyType4[i].Position = new Vector2(-32f, -32f);
                        hitExplosion.Play();
                    }//end if
                }//end for

                for (int i = 0; i < EnemyType5.Count(); i++)
                {
                    if (missleExplosion.BoundingRect.Intersects(EnemyType5[i].BoundingRect))
                    {
                        EnemyType5[i].Active = false;
                        CreateExplosion(EnemyType5[i]);
                        EnemyType5[i].Position = new Vector2(-32f, -32f);
                        hitExplosion.Play();
                    }//end if
                }//end for
            }//end if
        }//end CheckMissleExplosion

        #endregion

        #endregion

        #region Spawning Logic

        #region Spawn player bullets
        /// <summary>
        /// Spawns player bullets
        /// </summary>
        private void playerFire()
        {
            for (int i = 0; i < bullets.Length; i++)
            {
                bool found = false;
                if (!bullets[i].Active && !found)
                {
                    bullets[i].Active = true;
                    bullets[i].Position = new Vector2(player.Position.X + (player.ActualWidth / 2), player.Position.Y);
                    gunShot.Play();
                }//end if
            }//end for
        }//end playerFire

        #endregion

        #region Spawn enemy bullets

        /// <summary>
        /// Pass the enemy shooting.
        /// </summary>
        /// <param name="enemy"></param>
        private void enemyFire(GameObject enemy)
        {
            bool found = false;
            for (int i = 0; i < enemyBullets.Length; i++)
            {
                if (!enemyBullets[i].Active && !found)
                {
                    enemyBullets[i].Active = true;
                    //gunShot.Play();
                    enemyBullets[i].Position = new Vector2(enemy.Position.X + (enemy.ActualWidth / 2), enemy.Position.Y + enemy.ActualHeight);
                    found = true;
                }//end if
            }//end for
        }//end enemyFire

        #endregion

        #region SpawnEnemy1

        /// <summary>
        /// Spawns EnemyType1 in a random location.
        /// </summary>
        private void SpawnEnemy1()
        {
            if (e1 < EnemyType1.Length)
            {
                Random position = new Random();
                EnemyType1[e1].Position = new Vector2((float)position.Next(ScreenManager.Viewport.Width - EnemyType1[e1].ActualWidth), 0f);
                EnemyType1[e1].Active = true;
                createVelocity(EnemyType1[e1], 2);
                e1++;
            }//end if
        }//end SpawnEnemy1

        #endregion

        #region SpawnEnemy2

        /// <summary>
        /// Spawns EnemyType2 in a random location.
        /// </summary>
        private void SpawnEnemy2()
        {
            if (e2 < EnemyType2.Length)
            {
                Random position = new Random();
                EnemyType2[e2].Position = new Vector2((float)position.Next(ScreenManager.Viewport.Width - EnemyType2[e2].ActualWidth), 0f);
                EnemyType2[e2].Active = true;
                createVelocity(EnemyType2[e2], spawn.Next(3));
                while (EnemyType2[e2].Velocity.Length() == 0)
                {
                    createVelocity(EnemyType2[e2], spawn.Next(3));
                }//end while
                e2++;
            }//end if
        }//end SpawnEnemy2

        #endregion

        #region SpawnEnemy3

        /// <summary>
        /// Spawns EnemyType3 in a specified location.
        /// </summary>
        /// <param name="location"></param>
        private void SpawnEnemy3(Vector2 location)
        {
            if (e3 < EnemyType3.Length)
            {
                EnemyType3[e3].Position = location;
                EnemyType3[e3].Active = true;
                createVelocity(EnemyType3[e3], spawn.Next(5));
                while (EnemyType3[e3].Velocity.Length() == 0)
                {
                    createVelocity(EnemyType3[e3], spawn.Next(3));
                }//end while
                e3++;
            }//end if
        }//end SpawnEnemy3

        #endregion

        #region SpawnEnemy4

        /// <summary>
        /// Spawns EnemyType4 in a specified location.
        /// </summary>
        /// <param name="location"></param>
        private void SpawnEnemy4(Vector2 location)
        {
            if (e4 < EnemyType4.Length)
            {
                EnemyType4[e4].Position = location;
                EnemyType4[e4].Active = true;
                EnemyType4[e4].Velocity = new Vector2(0f, 3f);
            }//end if

            e4++;
        }//end SpawnEnemy4

        #endregion

        #region SpawnSwarm

        /// <summary>
        /// Create a swirling wave of enemies.
        /// </summary>
        private void SpawnSwarm()
        {
            if (swarmTimes < swarmCount)
            {
                swarm.Clear();
                //populate the list
                for (int i = 0; i < 20; i++)
                {
                    swarm.Add(EnemyType5[i + swarmStart]);
                    e5++;
                }//end for
                swarmTimes++;

                //set position and velocity
                for (int i = 0; i < 10; i++)
                {
                    swarm[i].Position = new Vector2((ScreenManager.Viewport.Width / 2) + (i * swarm[i].ActualWidth),
                        i * -swarm[i].ActualHeight);
                    swarm[i].Velocity = new Vector2(3f, 1f);
                    swarm[i].Active = true;
                }//end for
                for (int k = 10, i = 0; k < swarm.Count; k++, i++)
                {
                    swarm[k].Position = new Vector2((ScreenManager.Viewport.Width / 2) - (i * swarm[k].ActualWidth),
                        i * -swarm[k].ActualHeight);
                    swarm[k].Velocity = new Vector2(-3f, 1f);
                    swarm[k].Active = true;
                }//end for
            }//end if
        }//end SpawnSwarm

        #endregion

        #region CreateExplosion

        private void CreateExplosion(GameObject source)
        {
            bool found = false;

            for (int i = 0; i < explosion.Length; i++)
            {
                if (!found)
                {
                    if (explosion[i].Active == false)
                    {
                        explosion[i] = new GameObject(explosionSheet, new Rectangle(0, 83, 64, 64), 32, true, 0);
                        explosion[i].AnimationSpeed = TimeSpan.FromMilliseconds(35);
                        explosion[i].Position = source.Position;
                        explosion[i].Active = true;
                        found = true;
                    }//end if
                }
            }//end for
        }//end CreateExplosion

        #endregion

        #region SpawnMissle

        private void SpawnMissle()
        {
            if (numMissles > 0)
            {
                missle.Position = player.Position;
                missle.Active = true;
                missle.Velocity = new Vector2(0, -5);
                numMissles--;
            }//end if
        }//end SpawnMissle

        #endregion

        #region CreateMissleExplosion

        private void CreateMissleExplosion(GameObject missle)
        {
            missleExplosion.Position = new Vector2(missle.Position.X - (missleExplosion.ActualWidth / 2), missle.Position.Y - (missleExplosion.ActualHeight / 2));
            missleExplosion.Active = true;
            hitExplosion.Play();
        }//end CreateMissleExplosion

        #endregion

        #endregion

        #region Player Control functions

        #region MoveX
        /// <summary>
        /// Moves the player along the x axis as long
        /// as it won't go off screen.
        /// </summary>
        /// <param name="value"></param>
        private void PlayerMoveX(float value)
        {
            if (player.Position.X > 50 &&
                value < 0)
            {
                player.Position.X += value;
            }//end if

            if (player.Position.X < (ScreenManager.Viewport.Width - player.ActualWidth - 50)
                && value > 0)
            {
                player.Position.X += value;
            }//end if
        }//end PlayerMoveX

        #endregion

        #region MoveY

        /// <summary>
        /// Moves the player along the y axis as long
        /// as it is within the bounds.
        /// </summary>
        /// <param name="value"></param>
        private void PlayerMoveY(float value)
        {
            if (player.Position.Y > (int)(80f)
                && value < 0)
            {
                player.Position.Y += value;
            }//end if

            if (player.Position.Y < (int)(ScreenManager.Viewport.Height - 80f) && value > 0)
            {
                player.Position.Y += value;
            }//end if
        }//end PlayerMoveY

        #endregion

        #region singleShot

        private void singleShot()
        {
            int k = 0;
            int l = 0;
            bool found = false;
            for (int i = 0; i < bullets.Length; i++)
            {
                if (found == false && !bullets[i].Active && l < 3)
                {
                    found = true;
                    bullets[i].Active = true;
                    k = i;
                }//end if
                l++;
            }//end for

            if (found)
            {
                bullets[k].Position.X = player.Position.X + (player.ActualWidth / 2);
                bullets[k].Position.Y = player.Position.Y;
                bullets[k].Velocity = new Vector2(0, -10f);
                gunShot.Play();
            }//end if
        }//end shoot

        #endregion

        #region doubleShot

        private void doubleShot()
        {
            int j = 0;
            int k = 0;
            int l = 0;
            bool jFound = false;
            bool kFound = false;
            for (int i = 0; i < bullets.Length; i++)
            {
                if (jFound == false && !bullets[i].Active && l < 12)
                {
                    jFound = true;
                    bullets[i].Active = true;
                    k = i;
                }//end if
                l++;
            }//end for

            l = 0;
            for (int i = 0; i < bullets.Length; i++)
            {
                if (kFound == false && !bullets[i].Active && l < 12)
                {
                    kFound = true;
                    bullets[i].Active = true;
                    j = i;
                }//end if
                l++;
            }//end for

            if (kFound && jFound)
            {
                bullets[j].Position.X = player.Position.X + (player.ActualWidth / 4);
                bullets[j].Position.Y = player.Position.Y;
                bullets[j].Velocity = new Vector2(0, -10f);

                bullets[k].Position.X = player.Position.X + (player.ActualWidth * 0.75f);
                bullets[k].Position.Y = player.Position.Y;
                bullets[k].Velocity = new Vector2(0, -10f);
                gunShot.Play();
            }//end if
        }//end shoot

        #endregion

        #region checkBlasterUpgrade

        private void checkBlasterUpgrade()
        {
            if (doubleBlaster)
            {
                Vector2 temp = new Vector2(player.Position.X, player.Position.Y);
                player = new GameObject(newSheet, new Rectangle(534, 13, 499, 512), 1, true, 0);
                player.Scale = 0.1f;
                player.Position = temp;
            }//end if
            else
            {
                Vector2 temp = new Vector2(player.Position.X, player.Position.Y);
                player = new GameObject(newSheet, new Rectangle(12, 13, 499, 512), 1, true, 0);
                player.Scale = 0.1f;
                player.Position = temp;
            }//end if

        }//end checkBlasterUpgrade

        #endregion

        #region UpdateInput

        private void UpdateInput()
        {
            int playerIndex = (int)ControllingPlayer.Value;
            PlayerIndex pIndex = ControllingPlayer.Value;

            KeyboardState keyboardState = ScreenManager.InputSystem.currentKeyboardState[playerIndex];
            GamePadState gamepadState = ScreenManager.InputSystem.currentGamePadState[playerIndex];

            if (ScreenManager.InputSystem.IsnewGamePadPress(Buttons.A, ControllingPlayer, out pIndex) || ScreenManager.InputSystem.IsnewGamePadPress(Buttons.RightTrigger, ControllingPlayer, out pIndex) ||
                ScreenManager.InputSystem.IsNewKeyboardPress(Keys.Space, ControllingPlayer, out pIndex))
            {
                if (doubleBlaster)
                    doubleShot();
                else
                    singleShot();

            }//end if

            if (ScreenManager.InputSystem.IsnewGamePadPress(Buttons.B, ControllingPlayer, out pIndex) || ScreenManager.InputSystem.IsnewGamePadPress(Buttons.LeftTrigger, ControllingPlayer, out pIndex)
                || ScreenManager.InputSystem.IsNewKeyboardPress(Keys.LeftAlt, ControllingPlayer, out pIndex) || ScreenManager.InputSystem.IsNewKeyboardPress(Keys.RightAlt, ControllingPlayer, out pIndex))
            {
                if (missle.Active)
                {
                    CreateMissleExplosion(missle);
                    missle.Active = false;
                }//end if
                else
                {
                    SpawnMissle();
                }
            }//end if

            PlayerMoveX((playerMoveX * gamepadState.ThumbSticks.Left.X));
            PlayerMoveY((playerMoveY * -gamepadState.ThumbSticks.Left.Y));

            PlayerMoveX((playerMoveX * gamepadState.ThumbSticks.Right.X));
            PlayerMoveY((playerMoveY * -gamepadState.ThumbSticks.Right.Y));

            if (keyboardState.IsKeyDown(Keys.Right))
                PlayerMoveX(playerMoveX);

            if (keyboardState.IsKeyDown(Keys.Left))
                PlayerMoveX(-playerMoveX);

            if (gamepadState.IsButtonDown(Buttons.DPadRight))
            {
                PlayerMoveX(playerMoveX);
            }//end if

            else if (gamepadState.IsButtonDown(Buttons.DPadLeft))
            {
                PlayerMoveX(-playerMoveX);
            }//end if

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                PlayerMoveY(-playerMoveY);
            }//end if

            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                PlayerMoveY(playerMoveY);
            }//end if

            if (gamepadState.IsButtonDown(Buttons.DPadUp))
            {
                PlayerMoveY(-playerMoveY);
            }//end if

            else if (gamepadState.IsButtonDown(Buttons.DPadDown))
            {
                PlayerMoveY(playerMoveY);
            }//end if
        }

        #endregion

        #endregion

        #region EnemyMovement

        #region moveToNode

        /// <summary>
        /// Move an enemy to the next node in a list.  The velocity's
        /// magnitude is the scale passed.
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="nodes"></param>
        /// <param name="velScale"></param>
        private void moveToNode(GameObject enemy, List<Vector2> nodes, int velScale)
        {
            //make sure the list isn't empty
            if (nodes.Count > 0)
            {
                Vector2 velocity = new Vector2();
                double angle;
                velocity = nodes[0] - enemy.Position;
                angle = Math.Atan2(velocity.Y, velocity.X);
                velocity.X = (float)(velScale * Math.Cos(angle));
                velocity.Y = (float)(velScale * Math.Sin(angle));
                enemy.Velocity = velocity;
            }//end if

            //if it is empty move down the screen
            else
            {
                createVelocity(enemy, velScale);
            }//end else
        }//end moveToNode

        #endregion

        #region Check destination

        /// <summary>
        /// Determines if the enemy is within 5 pixels of the node.
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="nodes"></param>
        private void checkDestination(GameObject enemy, List<Vector2> nodes)
        {
            //determine if the list is empty
            if (nodes.Count > 0)
            {
                //determine if the enemy is close
                if (enemy.Position.X < nodes[0].X + 5f && enemy.Position.X > nodes[0].Y - 5f
                    && enemy.Position.Y < nodes[0].Y + 5f && enemy.Position.Y > nodes[0].Y - 5f)
                {
                    nodes.RemoveAt(0);
                    moveToNode(enemy, nodes, 2);
                    enemyShoot(enemy);
                }//end if
            }//end if
        }//end checkDestination

        #endregion

        #region populateNodes

        /// <summary>
        /// Populates the list of random vector2 objects..
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="size"></param>
        private void populateNodes(List<Vector2> nodes, int size)
        {
            Random next = new Random();
            for (int i = 0; i < size; i++)
            {
                nodes.Add(new Vector2(next.Next(ScreenManager.Viewport.Width - 32),
                    next.Next(ScreenManager.Viewport.Height / 2)));
            }
            //ensures the nodes never go offscreen
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].X < 5f)
                {
                    Vector2 temp = new Vector2(10f, nodes[i].Y);
                    nodes[i] = temp;
                }//end if
            }//end for
        }//end populateNodes

        #endregion

        #region createVelocity

        /// <summary>
        /// Assings an enemy's velocity magnitude equal to scale.
        /// The direction is towards the players current position.
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="scale"></param>
        private void createVelocity(GameObject enemy, int scale)
        {
            Vector2 velocity = new Vector2();
            double angle;
            velocity = player.Position - enemy.Position;
            angle = Math.Atan2(velocity.Y, velocity.X);
            velocity.X = (float)(scale * Math.Cos(angle));
            velocity.Y = (float)(scale * Math.Sin(angle));
            enemy.Velocity = velocity;
        }//end createVelocity

        #endregion

        #region EnemyShoot

        private void enemyShoot(GameObject enemy)
        {
            bool found = false;
            for (int i = 0; i < enemyBullets.Length; i++)
            {
                if (!found && !enemyBullets[i].Active)
                {
                    enemyBullets[i].Active = true;
                    enemyBullets[i].Velocity = new Vector2(0, 10f);
                    enemyBullets[i].Position = new Vector2(enemy.Position.X + (enemy.ActualWidth / 2), enemy.Position.Y + enemy.ActualHeight);
                    found = true;
                    gunShot.Play();
                }//end if
            }//end for
        }//end enemyShoot

        #endregion

        #endregion

        #region Level and Game Cycling

        #region gameOver

        /// <summary>
        /// Determines if the player has lost all his lives.
        /// </summary>
        private void GameOver()
        {
            if (isolatedStorage.isSaveable)
                LoadingScreen.Load(ScreenManager, true, ControllingPlayer, new CheckScoreScreen(Data, score, level, HighScore, 1));

            else
                LoadingScreen.Load(ScreenManager, true, ControllingPlayer, new BSMainMenu());

            backgroundMusic.Stop();
            base.Remove();
        }//end gameOver

        #endregion

        #region levelWon
        /// <summary>
        /// Determines if all enemies have dissappeared.
        /// </summary>
        private void levelWon()
        {
            //determines if no more enemies can be spawned, then loads the next level.
            if (e1 >= EnemyType1.Length && e2 >= EnemyType2.Length &&
                e3 >= EnemyType3.Length && e4 >= EnemyType4.Length &&
                e5 >= EnemyType5.Length)
            {
                bool active = false;
                for (int i = 0; i < EnemyType1.Length; i++)
                {
                    if (EnemyType1[i].Active)
                        active = true;
                }//end for
                for (int i = 0; i < EnemyType2.Length; i++)
                {
                    if (EnemyType2[i].Active)
                        active = true;
                }//end for
                for (int i = 0; i < EnemyType3.Length; i++)
                {
                    if (EnemyType3[i].Active)
                        active = true;
                }//end for
                for (int i = 0; i < EnemyType4.Length; i++)
                {
                    if (EnemyType4[i].Active)
                        active = true;
                }//end for
                for (int i = 0; i < EnemyType5.Length; i++)
                {
                    if (EnemyType5[i].Active)
                        active = true;
                }//end for
                if (!active)
                {
                    bEnd = true;
                    if (end < 0)
                    {
                        LoadLevel();

                    }//end if
                }
            }//end if
        }//end levelWon

        void LoadLevel()
        {
            backgroundMusic.Stop();
            base.Remove();
            LoadingScreen.Load(ScreenManager, true, ControllingPlayer, new BSLevel18(score, lives, doubleBlaster, numMissles, blasterBonus, missleBonus, toBlasterBonus, toMissleBonus));
        }//end load level

        #endregion

        #endregion
    }
}