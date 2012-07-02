using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace VaderpiXX
{
    /// <summary>
    /// Manages the enemies.
    /// </summary>
    class EnemyManager : ILevel
    {
        #region Members

        private Texture2D texture;

        private List<List<Enemy>> enemies = new List<List<Enemy>>(COLUMNS_COUNT);

        public ShotManager EnemyShotManager;
        private PlayerManager playerManager;

        public const int ROWS_COUNT = 5;
        public const int COLUMNS_COUNT = 10;

        private const float WAIT_AT_START = 1.5f;
        private float nextMoveTimer = InitialNextMoveMinTimer;
        public const float InitialNextMoveMinTimer = 0.01f; // 0.1f
        public const float InitialNextMoveDelayMinTimer = 0.05f;

        private int currentMoveRow = 0;
        private enum MoveDirection { Right, RightDown, Left, LeftDown };
        private MoveDirection currentMoveDirection = MoveDirection.Right;

        public bool IsActive = false;

        private Random rand = new Random();

        private int currentLevel;

        private readonly Rectangle screen = new Rectangle(0, 0, 800, 480);

        private const int TOP_PADDING = 80;
        private const int HORIZONTAL_PADDING = 30;
        private const int MOVE_PADDING = 15;
        private const int SPACE_X = 65;
        private const int SPACE_Y = 50;
        private readonly Vector2 MOVE_X = new Vector2(20, 0);
        private readonly Vector2 MOVE_Y = new Vector2(0, 20);

        Enemy ufo;

        private float nextUfoTimer;
        public const float InitialNextUfoMinTimer = 15.0f;
        public const float InitialNextUfoMaxTimer = 25.0f;

        private float nextUfoSoundTimer;
        private const float nextUfoSoundMinTimer = 0.25f;

        private readonly Vector2 UFO_START_POSITION = new Vector2(900, 40);
        private readonly Vector2 UFO_TARGET_POSITION = new Vector2(-100, 40);

        public const int BOTTOM_LIMIT_Y = 440;

        private bool shotManagerActive;

        private readonly Rectangle flagSource = new Rectangle(0, 550,
                                                              40, 40);

        private float elapsedLastMoveSound;
        private const float elapsedLastMoveSoundMin = 0.3f;

        #endregion

        #region Constructors

        public EnemyManager(Texture2D texture, PlayerManager playerManager,
                            Rectangle screenBounds)
        {
            this.texture = texture;
            this.playerManager = playerManager;

            EnemyShotManager = new ShotManager(texture,
                                               new Rectangle(100, 430, 18, 6),
                                               4,
                                               1,
                                               175.0f,
                                               screenBounds);

            setUpEnemies();
            setUpUfo();

            this.currentLevel = 1;

            ResetUfoTimer();
        }

        #endregion

        #region Methods

        private void setUpEnemies()
        {
            for (int i = 0; i < COLUMNS_COUNT; ++i)
            {
                enemies.Add(new List<Enemy>(ROWS_COUNT));

                for (int j = 0; j < ROWS_COUNT; ++j)
                {
                    if (j < 2)
                        enemies[i].Add(Enemy.CreateEasyEnemy(texture, new Vector2()));
                    else if (j < 4)
                        enemies[i].Add(Enemy.CreateSpeederEnemy(texture, new Vector2()));
                    else
                        enemies[i].Add(Enemy.CreateMediumEnemy(texture, new Vector2()));
                }
            }
        }

        /// <summary>
        /// Initializes the default ship position.
        /// </summary>
        private void resetEnemies()
        {
            for (int i = 0; i < COLUMNS_COUNT; ++i)
            {
                for (int j = 0; j < ROWS_COUNT; ++j)
                {
                    enemies[i][j].Reset();
                    enemies[i][j].SetCenter(new Vector2(HORIZONTAL_PADDING + i * SPACE_X + MOVE_PADDING,
                                                        TOP_PADDING + (ROWS_COUNT - (j + 1)) * SPACE_Y));
                    enemies[i][j].HitPoints = Enemy.INIT_HITPOINTS;

                    enemies[i][j].Opacity = 0.0f - (0.066f * i + 0.66f * (ROWS_COUNT - j - 1));
                    enemies[i][j].SetLevel(currentLevel);
                }
            }
            EnemyShotManager.Shots.Clear();
        }

        private void setUpUfo()
        {
            ufo = Enemy.CreateUfoEnemy(texture, UFO_START_POSITION);
            ufo.Reset();
            ufo.SetCenter(UFO_START_POSITION);
        }

        private void startUfo()
        {
            setUpUfo();
            ufo.Speed = Enemy.DEFAULT_UFO_SPEED_MIN + (float)rand.Next((int)Enemy.DEFAULT_UFO_SPEED_MAX - Enemy.DEFAULT_UFO_SPEED_MIN);
            ufo.GoToTarget(UFO_TARGET_POSITION);
        }

        private void moveRow(int rowIndex)
        {
            for (int i = 0; i < COLUMNS_COUNT; ++i)
            {
                Vector2 enemyCenter = enemies[i][rowIndex].EnemySprite.Center;

                switch (currentMoveDirection)
                {
                    case MoveDirection.Right:
                        enemies[i][rowIndex].GoToTarget(enemyCenter + MOVE_X);
                        break;
                    
                    case MoveDirection.Left:
                        enemies[i][rowIndex].GoToTarget(enemyCenter - MOVE_X);
                        break;

                    case MoveDirection.RightDown:
                    case MoveDirection.LeftDown:
                        enemies[i][rowIndex].GoToTarget(enemyCenter + MOVE_Y);
                        break;
                }
            }
        }

        private void adjustCurrentMoveDirection()
        {
            int mostBottomRow = getMostBottom();
            
            switch (currentMoveDirection)
            {
                case MoveDirection.Right:
                    if (getMostRight() + MOVE_X.X > screen.Width - MOVE_PADDING)
                    {
                        currentMoveDirection = MoveDirection.RightDown;
                    }
                    break;
                case MoveDirection.RightDown:
                        currentMoveDirection = MoveDirection.Left;
                    break;
                case MoveDirection.Left:
                    if (getMostLeft() - MOVE_X.X < MOVE_PADDING)
                    {
                        currentMoveDirection = MoveDirection.LeftDown;
                    }
                    break;
                case MoveDirection.LeftDown:
                    currentMoveDirection = MoveDirection.Right;
                    break;
            }
        }

        private bool nextRun = false;

        private void updateMovement(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            nextMoveTimer -= elapsed;
            elapsedLastMoveSound += elapsed;

            if (nextMoveTimer <= 0.0f)
            {
                shotManagerActive = true;

                if (nextRun)
                {
                    nextRun = false;
                    adjustCurrentMoveDirection();

                    if (elapsedLastMoveSound >= elapsedLastMoveSoundMin)
                    {
                        SoundManager.PlayMoveSound();

                        elapsedLastMoveSound -= elapsedLastMoveSoundMin;
                    }
                }

                if (currentMoveRow == getMostTop() || getActiveRowsCount() < 2)
                {
                    nextRun = true;
                    nextMoveTimer = InitialNextMoveDelayMinTimer - ((currentLevel - 1) * 0.00075f) + (0.01f * getActiveEnemiesCount() + 0.01f * (ROWS_COUNT - getActiveRowsCount()));
                }
                else
                {
                    nextMoveTimer = InitialNextMoveMinTimer + (0.002f * getActiveEnemiesCount()) - ((currentLevel - 1) * 0.001f);
                }

                int startIndexOfLoop = currentMoveRow;

                while (!isRowActive(currentMoveRow))
                {
                    currentMoveRow = (++currentMoveRow) % ROWS_COUNT;

                    if (startIndexOfLoop == currentMoveRow)
                        break;
                }

                moveRow(currentMoveRow);

                currentMoveRow = (++currentMoveRow) % ROWS_COUNT;
            }
        }

        public void Update(GameTime gameTime)
        {
            EnemyShotManager.Update(gameTime);

            updateEnemies(gameTime);

            updateUfo(gameTime);

            updateShots();

            if (this.IsActive && getActiveEnemiesCount() > 0)
            {
                updateMovement(gameTime);
            }
        }

        private void updateEnemies(GameTime gameTime)
        {
            for (int i = 0; i < COLUMNS_COUNT; ++i)
            {
                for (int j = 0; j < ROWS_COUNT; ++j)
                {
                    enemies[i][j].Update(gameTime);
                }
            }
        }

        private void updateUfo(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (getActiveEnemiesCount() > 0 && !playerManager.IsDestroyed)
                nextUfoTimer -= elapsed;

            if (nextUfoTimer <= 0.0f)
            {
                ResetUfoTimer();
                startUfo();
                nextUfoSoundTimer = 0.0f;
            }

            if (ufo.EnemySprite.isBoxColliding(screen) && !ufo.IsDestroyed)
            {
                nextUfoSoundTimer += elapsed;

                nextUfoSoundTimer -= nextUfoSoundMinTimer;
                SoundManager.PlayUfoHighpitchSound();
            }
            else
            {
                SoundManager.StopUfoHighpitchSound();
            }

            ufo.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            EnemyShotManager.Draw(spriteBatch);

            drawEnemies(spriteBatch);
            drawUfo(spriteBatch);

            drawFlags(spriteBatch);
        }

        private void drawFlags(SpriteBatch spriteBatch)
        {
            if (ReachedBottom())
            {
                int bottomRow = getMostBottom();

                for (int i = 0; i < COLUMNS_COUNT; ++i)
                {
                    if (!enemies[i][bottomRow].IsDestroyed)
                    {
                        spriteBatch.Draw(texture,
                                         new Vector2(enemies[i][bottomRow].EnemySprite.Center.X - 20, 445),
                                         flagSource,
                                         Color.Lime);
                    }
                }
            }
        }

        private void drawEnemies(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < COLUMNS_COUNT; ++i)
            {
                for (int j = 0; j < ROWS_COUNT; ++j)
                {
                    enemies[i][j].Draw(spriteBatch);
                }
            }
        }

        private void drawUfo(SpriteBatch spriteBatch)
        {
            if (ufo.EnemySprite.isBoxColliding(screen))
                ufo.Draw(spriteBatch);
        }

        public void Reset()
        {
            resetEnemies();
            ResetUfoTimer();

            ufo.EnemySprite.Location = UFO_START_POSITION;
            ufo.GoToTarget(UFO_START_POSITION);

            this.EnemyShotManager.Shots.Clear();

            this.nextMoveTimer = WAIT_AT_START;

            this.currentMoveDirection = MoveDirection.Right;
            this.currentMoveRow = 0;

            shotManagerActive = false;

            this.IsActive = true;

            elapsedLastMoveSound = 0.0f;
        }

        public void ResetUfoTimer()
        {
            this.nextUfoTimer = InitialNextUfoMinTimer + (float)rand.Next((int)(InitialNextUfoMaxTimer - InitialNextUfoMinTimer) + 1);
        }

        private void updateShots()
        {
            for (int i = 0; i < COLUMNS_COUNT; ++i)
            {
                for (int j = 0; j < ROWS_COUNT; ++j)
                {
                    if (!enemies[i][j].IsDestroyed)
                    {
                        probablyShoot(enemies[i][j]);
                        break;
                    }
                }
            }
        }

        private void probablyShoot(Enemy enemy)
        {
            if (shotManagerActive)
            {
                float chance = enemy.ShotChance;

                if (getActiveEnemiesCount() <= 5)
                    chance *= 1.2f;

                if ((float)rand.Next(0, 20000) / 100 < chance &&
                        !playerManager.IsDestroyed &&
                         screen.Contains((int)enemy.EnemySprite.Center.X,
                                                 (int)enemy.EnemySprite.Center.Y))
                {
                    Vector2 fireLocation = enemy.EnemySprite.Location;
                    fireLocation += enemy.GunOffset;

                    EnemyShotManager.FireShot(fireLocation,
                                              Vector2.UnitY,
                                              false,
                                              Color.White,
                                              true);
                }
            }
        }

        private int getMostRight()
        {
            int resX = 0;

            for (int i = COLUMNS_COUNT - 1; i >= 0; --i)
            {
                for (int j = 0; j < ROWS_COUNT; ++j)
                {
                    if (enemies[i][j].IsActive())
                    {
                        resX = (int)Math.Max(resX, enemies[i][j].EnemySprite.Center.X);
                    }
                }

                if (resX > 0)
                    return resX;
            }

            return resX;
        }

        private int getMostLeft()
        {
            int resX = screen.Width;

            for (int i = 0; i < COLUMNS_COUNT; ++i)
            {
                for (int j = 0; j < ROWS_COUNT; ++j)
                {
                    if (enemies[i][j].IsActive())
                    {
                        resX = (int)Math.Min(resX, enemies[i][j].EnemySprite.Center.X);
                    }
                }

                if (resX <  screen.Width)
                    return resX;
            }

            return resX;
        }

        private int getMostTop()
        {
            for (int i = ROWS_COUNT - 1; i >= 0; --i)
            {
                if (isRowActive(i))
                    return i;
            }
            return -1;
        }

        private int getMostBottom()
        {
            for (int i = 0; i < ROWS_COUNT; ++i)
            {
                if (isRowActive(i))
                    return i;
            }
            return -1;
        }

        private int getActiveRowsCount()
        {
            int activeCount = 0;

            for (int i = ROWS_COUNT - 1; i >= 0; --i)
            {
                if (isRowActive(i))
                    ++activeCount;
            }
            return activeCount;
        }

        private bool isRowActive(int rowIndex)
        {
            for (int i = 0; i < COLUMNS_COUNT; ++i)
            {
                if (enemies[i][rowIndex].IsActive())
                    return true;
            }
            return false;
        }

        private int getActiveEnemiesCount()
        {
            int activeCount = 0;

            for (int i = 0; i < COLUMNS_COUNT; ++i)
            {
                for (int j = 0; j < ROWS_COUNT; ++j)
                {
                    if (enemies[i][j].IsActive())
                    {
                        ++activeCount;
                    }
                }
            }

            return activeCount;
        }

        public bool ReachedBottom()
        {
            int mostBottomIndex = getMostBottom();

            if (getMostBottom() == -1)
                return false;


            for (int i = 0; i < COLUMNS_COUNT; ++i)
            {
                Enemy e = enemies[i][mostBottomIndex];

                if (!e.IsDestroyed)
                {
                    if (e.EnemySprite.Location.Y + e.EnemySprite.Source.Height > BOTTOM_LIMIT_Y)
                        return true;
                }
            }
            return false; 
        }

        public bool HasEnemies()
        {
            for (int i = 0; i < ROWS_COUNT; ++i)
            {
                if (isRowActive(i))
                    return true;
            }
            return false;
        }

        public void SetLevel(int lvl)
        {
            this.currentLevel = lvl;
        }

        public long GetUfoScore()
        {
            int rnd = rand.Next(currentLevel + 8);

            switch (rnd)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    return 50;

                case 5:
                case 6:
                case 7:
                case 8:
                    return 100;

                case 9:
                case 10:
                case 11:
                    return 150;

                default:
                    return 300;
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Enemies
            enemies.Clear();

            for (int c = 0; c < COLUMNS_COUNT; ++c)
            {
                enemies.Add(new List<Enemy>(ROWS_COUNT));

                for (int r = 0; r < ROWS_COUNT; ++r)
                {
                    EnemyType type = EnemyType.Easy;
                    Enemy e;

                    type = (EnemyType)Enum.Parse(type.GetType(), reader.ReadLine(), false);

                    switch (type)
                    {
                        case EnemyType.Easy:
                            e = Enemy.CreateEasyEnemy(texture, Vector2.Zero);
                            break;
                        case EnemyType.Medium:
                            e = Enemy.CreateMediumEnemy(texture, Vector2.Zero);
                            break;
                        case EnemyType.Speeder:
                            e = Enemy.CreateSpeederEnemy(texture, Vector2.Zero);
                            break;
                        default:
                            e = Enemy.CreateEasyEnemy(texture, Vector2.Zero);
                            break;
                    }
                    e.Activated(reader);

                    enemies[c].Add(e);
                }
            }

            EnemyShotManager.Activated(reader);

            this.nextMoveTimer = Single.Parse(reader.ReadLine());

            this.IsActive = Boolean.Parse(reader.ReadLine());

            this.currentLevel = Int32.Parse(reader.ReadLine());

            this.currentMoveRow = Int32.Parse(reader.ReadLine());
            this.currentMoveDirection = (MoveDirection)Enum.Parse(currentMoveDirection.GetType(), reader.ReadLine(), false);
            this.ufo.Activated(reader);
            this.nextUfoTimer = Single.Parse(reader.ReadLine());
            this.nextUfoSoundTimer = Single.Parse(reader.ReadLine());
            this.shotManagerActive = Boolean.Parse(reader.ReadLine());
            this.elapsedLastMoveSound = Single.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            //Enemies
            for (int c = 0; c < COLUMNS_COUNT; ++c)
            {
                for (int r = 0; r < ROWS_COUNT; ++r)
                {
                    writer.WriteLine(enemies[c][r].Type);
                    enemies[c][r].Deactivated(writer);
                }
            }

            EnemyShotManager.Deactivated(writer);

            writer.WriteLine(nextMoveTimer);

            writer.WriteLine(IsActive);

            writer.WriteLine(currentLevel);

            writer.WriteLine(currentMoveRow);
            writer.WriteLine(currentMoveDirection);
            ufo.Deactivated(writer);
            writer.WriteLine(nextUfoTimer);
            writer.WriteLine(nextUfoSoundTimer);
            writer.WriteLine(shotManagerActive);
            writer.WriteLine(elapsedLastMoveSound);
        }

        #endregion

        #region Properties

        public IEnumerable<Enemy> Enemies
        {
            get
            {
                for (int i = 0; i < COLUMNS_COUNT; ++i)
                {
                    for (int j = 0; j < ROWS_COUNT; ++j)
                    {
                        yield return enemies[i][j];
                    }
                }
            }
        }

        public Enemy Ufo
        {
            get
            {
                return ufo;
            }
        }

        #endregion
    }
}
