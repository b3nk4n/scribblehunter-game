
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using System.IO;
using ScribbleHunter.Inputs;
using Microsoft.Phone.Applications.Common;
using System.Collections.Generic;
using ScribbleHunter.Windows.Inputs;

namespace ScribbleHunter
{
    class PlayerManager : ILevel
    {
        #region Members

        public Sprite playerSprite;
        private Rectangle playerAreaLimit;

        private Vector2 startLocation;

        private long playerScore;

        private int comboMulti;
        private int comboScore;
        private float comboElapsedTimer;
        private const float ComboElapsedTimerLimit = 3.33f;

        // Init values
        private float playerSpeed;
        private float shotSpeed;
        private float shotPower;
        private float maxHitPoints;

        private float hitPoints = 1.0f;

        private Vector2 gunOffset = new Vector2(16, 5);
        private float shotTimer = 0.0f;
        public const float ShotTimerMin = 5.0f;
        private bool canReloadPeep = true;
        
        private const int PlayerRadius = 16;
        public ShotManager PlayerShotManager;

        Vector3 currentAccValue = Vector3.Zero;

        SettingsManager settings = SettingsManager.GetInstance();

        GameInput gameInput;
        private const string ActionFire = "Fire";
        private const string ActionFireTouch = "FireTouch";
        private const string ActionMoveLeft = "PlayerLeft";
        private const string ActionMoveRight = "PlayerRight";
        private const string ActionMoveUp = "PlayerUp";
        private const string ActionMoveDown = "PlayerDown";

        // Player ship type/texture
        public enum PlayerType { GreenHornet };
        private PlayerType shipType = PlayerType.GreenHornet;
        Texture2D tex;

        Random rand = new Random();

        private readonly Rectangle screenBounds;

        private DamageExplosionManager damageExplosionManager;

        private float laserTimer;
        public const float LaserTime = 15.0f;
        private float mineTimer;
        public const float MineTime = 15.0f;
        private float fastReloadTimer;
        public const float FastReloadTime = 12.5f;
        public const float FAST_RELOAD_FACTOR = 2.0f;
        private float largeExplosionTimer;
        public const float LargeExplosionTime = 12.5f;
        private float timeFreezeTimer;
        public const float TimeFreezeTime = 10.0f;
        private float starTimer;
        public const float StarTime = 10.0f;

        private float laserShotTimer;
        public const float LaserShotTimerMin = 0.33f;
        private float mineDropTimer;
        public const float MineDropTimerMin = 3.0f;

        // Smoke
        private float smokeTimer;
        private const float SmokeTimerMin = 0.05f;
        private const float SmokeTimerMax = 0.075f;

        // Score timer
        private float scoreTimer;
        private const float  ScoreTimerLimit = 1.0f;
        private int nextTimerScore;
        private const int FirstTimerScore = 10;
        private const int SCORE_INCREMENTOR = 11;

        private readonly Rectangle InitialFrame;
        private readonly int InitialFrameCount;

        // Scales opacity and speed at startup (0 -> 1)
        private float startUpScale;

        private DeviceControlManager deviceControlManager = DeviceControlManager.GetInstance();

        private Vector2 lastVelocityDirection = Vector2.Zero;

        #endregion

        #region Constructors

        public PlayerManager(Texture2D texture, Rectangle initialFrame,
                             int frameCount, Rectangle screenBounds, GameInput input)
        {
            this.InitialFrame = initialFrame;
            this.InitialFrameCount = frameCount;

            tex = texture;

            this.playerSprite = new Sprite(new Vector2(500, 500),
                                           texture,
                                           initialFrame,
                                           Vector2.Zero);

            initPlayer(this.shipType);
            
            this.PlayerShotManager = new ShotManager(texture,
                                                     new Rectangle(650, 160, 20, 20),
                                                     4,
                                                     2,
                                                     shotSpeed,
                                                     screenBounds);
            this.playerAreaLimit = new Rectangle(0,
                                                 0,
                                                 screenBounds.Width,
                                                 screenBounds.Height - 15);

            playerSprite.CollisionRadius = PlayerRadius;

            AccelerometerHelper.Instance.ReadingChanged += new EventHandler<AccelerometerHelperReadingEventArgs>(OnAccelerometerHelperReadingChanged);
            AccelerometerHelper.Instance.Active = true;

            this.startLocation = new Vector2(screenBounds.Width / 2 - playerSprite.FrameWidth / 2,
                                             screenBounds.Height / 2 - playerSprite.FrameHeight / 2);

            gameInput = input;

            this.screenBounds = screenBounds;

            this.damageExplosionManager = DamageExplosionManager.Instance;
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            // Touch
            gameInput.AddTouchTapInput(ActionFireTouch,
                                       screenBounds,
                                       false);

            // Controller
            gameInput.AddGamepadInput(
                ActionFire,
                Buttons.A,
                false);
            gameInput.AddGamepadInput(
                ActionFire,
                Buttons.LeftTrigger,
                false);
            gameInput.AddGamepadInput(
                ActionFire,
                Buttons.RightTrigger,
                false);
            gameInput.AddGamepadInput(
                ActionMoveLeft,
                Buttons.DPadLeft,
                false);
            gameInput.AddGamepadInput(
                ActionMoveRight,
                Buttons.DPadRight,
                false);
            gameInput.AddGamepadInput(
                ActionMoveUp,
                Buttons.DPadUp,
                false);
            gameInput.AddGamepadInput(
                ActionMoveDown,
                Buttons.DPadDown,
                false);

            // Keyboard
            gameInput.AddKeyboardInput(
                ActionFire,
                Keys.Space,
                false);
            gameInput.AddKeyboardInput(
                ActionMoveLeft,
                Keys.Left,
                false);
            gameInput.AddKeyboardInput(
                ActionMoveRight,
                Keys.Right,
                false);
            gameInput.AddKeyboardInput(
                ActionMoveUp,
                Keys.Up,
                false);
            gameInput.AddKeyboardInput(
                ActionMoveDown,
                Keys.Down,
                false);
        }

        public void Reset()
        {
            this.PlayerShotManager.Reset();

            this.playerSprite.Location = startLocation;

            this.hitPoints = maxHitPoints;

            this.shotTimer = ShotTimerMin;
            this.canReloadPeep = true;

            initPlayer(this.shipType);

            this.laserTimer = 0.0f;
            this.mineTimer = 0.0f;
            this.fastReloadTimer = 0.0f;
            this.largeExplosionTimer = 0.0f;
            this.timeFreezeTimer = 0.0f;
            this.starTimer = 0.0f;

            this.mineDropTimer = 0.0f;
            this.laserShotTimer = 0.0f;

            this.smokeTimer = SmokeTimerMin;

            this.scoreTimer = ScoreTimerLimit;
            this.nextTimerScore = FirstTimerScore;

            this.comboMulti = 0;
            this.comboScore = 0;
            this.comboElapsedTimer = 0;

            this.startUpScale = 0;
        }

        private void selectPlayerType(PlayerType type)
        {
            this.shipType = type;

            switch (type)
            {
                case PlayerType.GreenHornet:
                    playerSprite = new Sprite(new Vector2(500, 500),
                                           tex,
                                           InitialFrame,
                                           Vector2.Zero);

                    for (int x = 1; x < InitialFrameCount; x++)
                    {
                        this.playerSprite.AddFrame(new Rectangle(InitialFrame.X + (x * InitialFrame.Width),
                                                                 InitialFrame.Y,
                                                                 InitialFrame.Width,
                                                                 InitialFrame.Height));
                    }
                    break;
                default:
                    break;
            }

            playerSprite.Location = startLocation;
            playerSprite.CollisionRadius = PlayerRadius;
            playerSprite.Rotation = MathHelper.PiOver2 * 3;
        }

        private void initPlayer(PlayerType type)
        {
            selectPlayerType(type);

            switch (type)
            {
                case PlayerType.GreenHornet:
                    playerSpeed = 125.0f;
                    shotSpeed = 333.0f;
                    shotPower = 50f;
                    maxHitPoints = 300.0f;
                    break;
            }
        }

        private void fireWeapon(Vector2 location)
        {
            if (shotTimer <= 0.0f)
            {
                if (IsLargeExplosionActive())
                    damageExplosionManager.AddLargePlayerExplosion(location);
                else
                    damageExplosionManager.AddPlayerExplosion(location);

                this.canReloadPeep = true;
                shotTimer = ShotTimerMin;
            }
        }

        private void fireLaser()
        {
            Vector2 fireLocation = this.playerSprite.Center;

            Vector2 shotDirection = playerSprite.Velocity;

            if (shotDirection == Vector2.Zero)
            {
                shotDirection = lastVelocityDirection;
            }

            if (shotDirection != Vector2.Zero)
            {
                shotDirection.Normalize();

                PlayerShotManager.FireShot(
                    fireLocation + shotDirection * 20,
                    shotDirection,
                    true,
                    Color.Purple,
                    true);
            }
        }

        private void dropMine()
        {
            Vector2 direction = playerSprite.Velocity;

            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }

            Vector2 location = playerSprite.Center - (direction * 10);

            PlayerShotManager.DropMine(location, Color.White, true);
        }

        private void HandleInput(TouchCollection touches)
        {
            float moveX = 0.0f;
            float moveY = 0.0f;
            bool fireTouch = false;
            bool fire = false;

            // Resove actions
            if (gameInput.IsPressed(ActionMoveLeft))
            {
                moveX -= 1.0f;
            }
            if (gameInput.IsPressed(ActionMoveRight))
            {
                moveX += 1.0f;
            }
            if (gameInput.IsPressed(ActionMoveUp))
            {
                moveY -= 1.0f;
            }
            if (gameInput.IsPressed(ActionMoveDown))
            {
                moveY += 1.0f;
            }

            if (gameInput.IsPressed(ActionFireTouch))
            {
                fireTouch = true;
            }
            if (gameInput.IsPressed(ActionFire))
            {
                MouseHelper.WakuUpMouse();
                fire = true;
            }
            if (MouseHelper.IsLeftMouseDown())
            {
                fire = true;
            }

            // Execute action
            if (deviceControlManager.CurrentControlState == DeviceControlManager.ControlState.KeyboardController)
            {
                Vector2 gamepadStick = GamepadHelper.LeftThumbstickDirection;

                if (gamepadStick != Vector2.Zero)
                {
                    playerSprite.Velocity = new Vector2(gamepadStick.X,
                                                        -gamepadStick.Y);
                }
                else
                {
                    playerSprite.Velocity = new Vector2(moveX,
                                                        moveY);

                    if (playerSprite.Velocity != Vector2.Zero)
                        playerSprite.Velocity = Vector2.Normalize(playerSprite.Velocity);
                }
            }
            else
            {
                Vector3 current = currentAccValue;

                current.Y = current.Y + (float)Math.Sin(settings.GetNeutralPosition());

                current.Y = MathHelper.Clamp(current.Y, -0.4f, 0.4f);
                current.X = MathHelper.Clamp(current.X, -0.4f, 0.4f);

                playerSprite.Velocity = new Vector2(current.X * 5,
                                                    -current.Y * 5);
            }

            if (fireTouch)
            {
                Vector2 pos = gameInput.CurrentTouchPosition(ActionFire);

                if (pos.X > 30 && pos.Y > 30)
                {
                    fireWeapon(pos);
                }
            }
            else if (fire)
            {
                fireWeapon(MouseHelper.Position);
            }
        }

        private void adaptMovementLimits()
        {
            Vector2 location = playerSprite.Location;

            if (location.X < playerAreaLimit.X)
            {
                location.X = playerAreaLimit.X;
                playerSprite.Velocity = Vector2.Zero;
            }

            if (location.X > (playerAreaLimit.Right - playerSprite.Source.Width))
            {
                location.X = (playerAreaLimit.Right - playerSprite.Source.Width);
                playerSprite.Velocity = Vector2.Zero;
            }

            if (location.Y < playerAreaLimit.Y)
            {
                location.Y = playerAreaLimit.Y;
                playerSprite.Velocity = Vector2.Zero;
            }

            if (location.Y > (playerAreaLimit.Bottom - playerSprite.Source.Height))
            {
                location.Y = (playerAreaLimit.Bottom - playerSprite.Source.Height);
                playerSprite.Velocity = Vector2.Zero;
            }

            playerSprite.Location = location;
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            PlayerShotManager.Update(elapsed);

            if (!IsDestroyed)
            {
                laserTimer -= elapsed;
                mineTimer -= elapsed;
                fastReloadTimer -= elapsed;
                largeExplosionTimer -= elapsed;
                timeFreezeTimer -= elapsed;
                starTimer -= elapsed;

                if (IsLaserActive())
                {
                    laserShotTimer -= elapsed;

                    if (laserShotTimer <= 0)
                    {
                        fireLaser();
                        laserShotTimer = LaserShotTimerMin;
                    }
                }

                if (IsMineActive())
                {
                    mineDropTimer -= elapsed;

                    if (mineDropTimer <= 0)
                    {
                        dropMine();
                        mineDropTimer = MineDropTimerMin;
                    }
                }

                if (IsFastReloadActive())
                {
                    shotTimer -= elapsed * FAST_RELOAD_FACTOR;
                }
                else
                {
                    shotTimer -= elapsed;
                }

                if (canReloadPeep && CanFire)
                {
                    SoundManager.PlayPiepSound();
                    canReloadPeep = false;
                }

                startUpScale += elapsed;

                if (startUpScale > 1)
                    startUpScale = 1;

                HandleInput(TouchPanel.GetState());

                if (playerSprite.Velocity.Length() != 0.0f)
                {
                    playerSprite.Velocity.Normalize();
                }

                float startFactor = (float)Math.Pow(startUpScale, 2.0);

                playerSprite.Velocity *= ((playerSpeed) * startFactor);

                if (playerSprite.Velocity != Vector2.Zero)
                {
                    playerSprite.RotateTo(playerSprite.Velocity);
                    lastVelocityDirection = playerSprite.Velocity;
                }
                else
                {
                    playerSprite.RotateTo(lastVelocityDirection);
                }



                playerSprite.Update(elapsed);
                adaptMovementLimits();

                updateScoreTimer(elapsed);

                this.playerSprite.TintColor = Color.Purple * startFactor * StarTimeOpacityFactor;
            }

            updateComboScore(elapsed);
            updateSmoke(elapsed);
        }

        private void updateComboScore(float elapsed)
        {
            if (comboElapsedTimer > 0)
            {
                this.comboElapsedTimer -= elapsed;

                if (comboElapsedTimer <= 0)
                {
                    this.playerScore += (comboMulti * comboScore);
                    this.comboMulti = 0;
                    this.comboScore = 0;
                } 
            }
        }

        private void updateScoreTimer(float elapsed)
        {
            this.scoreTimer -= elapsed;

            if (scoreTimer <= 0)
            {
                this.playerScore += nextTimerScore;
                nextTimerScore += SCORE_INCREMENTOR;

                scoreTimer = ScoreTimerLimit;
            }
        }

        private void updateSmoke(float elapsed)
        {
            if (startUpScale > 0.33f)
            {
                smokeTimer -= elapsed;

                if (!IsDestroyed && smokeTimer <= 0.0f)
                {
                    Vector2 direction = this.playerSprite.Velocity;

                    if (direction == Vector2.Zero)
                        direction = lastVelocityDirection;

                    if (direction != Vector2.Zero)
                        direction.Normalize();
                    Vector2 location = this.playerSprite.Center - (direction * 15);

                    smokeTimer = MathHelper.Clamp(SmokeTimerMax - this.playerSprite.Velocity.Length() * 0.00075f, SmokeTimerMin, SmokeTimerMax);

                    EffectManager.AddPlayerSmoke(location, this.playerSprite.Velocity / 25);
                }
            }
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerShotManager.Draw(spriteBatch);

            if (!IsDestroyed)
            {
                playerSprite.Draw(spriteBatch);
            }
        }

        public void SetLevel(int lvl)
        {
            // nothing happens for player when a level up occures
        }

        public void IncreaseScoreWithCombo(int score)
        {
            this.comboElapsedTimer = ComboElapsedTimerLimit;
            this.comboMulti += 1;
            this.comboScore += score;
        }

        public void IncreaseScore(int score)
        {
            this.playerScore += score;
        }

        public void SetHitPoints(float hp)
        {
            this.hitPoints = MathHelper.Clamp(hp, 0.0f, maxHitPoints);
        }

        public void IncreaseHitPoints(float hp)
        {
            if (hp < 0)
                throw new ArgumentException("Negative values are not allowed.");

            this.hitPoints += hp;
            this.hitPoints = MathHelper.Clamp(hitPoints, 0.0f, maxHitPoints);
        }

        public void Kill()
        {
            this.hitPoints = 0;
        }

        public void ResetPlayerScore()
        {
            this.playerScore = 0;
        }

        public void ActivateLaser()
        {
            this.laserTimer = LaserTime;
            this.laserShotTimer = LaserShotTimerMin;
        }

        public void ActivateMines()
        {
            this.mineTimer = MineTime;
            this.mineDropTimer = MineDropTimerMin;
        }

        public void ReleaseNuke()
        {
            this.damageExplosionManager.AddSuperExplosion(playerSprite.Center);
        }

        public void ReleaseBomb()
        {
            this.damageExplosionManager.AddBombExplosion(playerSprite.Center);
        }

        public void DirectReload()
        {
            this.shotTimer = 0.0f;
        }

        public void ActivateLargeExplosion()
        {
            this.largeExplosionTimer = LargeExplosionTime;
        }

        public void ActivateFastReload()
        {
            this.fastReloadTimer = FastReloadTime;
        }

        public void ActivateTimeFreeze()
        {
            this.timeFreezeTimer = TimeFreezeTime;
        }

        public void ActivateStar()
        {
            this.starTimer = StarTime;
        }

        public bool IsLaserActive()
        {
            return this.laserTimer > 0;
        }

        public bool IsMineActive()
        {
            return this.mineTimer > 0;
        }

        public bool IsLargeExplosionActive()
        {
            return this.largeExplosionTimer > 0;
        }

        public bool IsFastReloadActive()
        {
            return this.fastReloadTimer > 0;
        }

        public bool IsTimeFreezeActive()
        {
            return this.timeFreezeTimer > 0;
        }
        public bool IsStarActive()
        {
            return this.starTimer > 0;
        }

        public float LaserValue()
        {
            return MathHelper.Clamp(laserTimer / LaserTime, 0.0f, 1.0f);
        }

        public float MineValue()
        {
            return MathHelper.Clamp(mineTimer / MineTime, 0.0f, 1.0f);
        }

        public float FastReloadValue()
        {
            return MathHelper.Clamp(fastReloadTimer / FastReloadTime, 0.0f, 1.0f);
        }

        public float LargeExplosionValue()
        {
            return MathHelper.Clamp(largeExplosionTimer / LargeExplosionTime, 0.0f, 1.0f);
        }

        public float TimeFreezeValue()
        {
            return MathHelper.Clamp(timeFreezeTimer / TimeFreezeTime, 0.0f, 1.0f);
        }

        public float StarValue()
        {
            return MathHelper.Clamp(starTimer / StarTime, 0.0f, 1.0f);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(Queue<string> data)
        {
            //Player sprite
            playerSprite.Activated(data);

            this.playerScore = Int64.Parse(data.Dequeue());

            this.hitPoints = Single.Parse(data.Dequeue());

            this.shotTimer = Single.Parse(data.Dequeue());

            this.PlayerShotManager.Activated(data);

            this.laserTimer = Single.Parse(data.Dequeue());
            this.mineTimer = Single.Parse(data.Dequeue());
            this.largeExplosionTimer = Single.Parse(data.Dequeue());
            this.fastReloadTimer = Single.Parse(data.Dequeue());
            this.timeFreezeTimer = Single.Parse(data.Dequeue());
            this.starTimer = Single.Parse(data.Dequeue());

            this.laserShotTimer = Single.Parse(data.Dequeue());
            this.mineDropTimer = Single.Parse(data.Dequeue());

            this.smokeTimer = Single.Parse(data.Dequeue());

            this.scoreTimer = Single.Parse(data.Dequeue());
            this.nextTimerScore = Int32.Parse(data.Dequeue());

            this.comboMulti = Int32.Parse(data.Dequeue());
            this.comboScore = Int32.Parse(data.Dequeue());
            this.comboElapsedTimer = Single.Parse(data.Dequeue());

            this.canReloadPeep = Boolean.Parse(data.Dequeue());

            this.startUpScale = Single.Parse(data.Dequeue());
            this.lastVelocityDirection.X = Single.Parse(data.Dequeue());
            this.lastVelocityDirection.Y = Single.Parse(data.Dequeue());
        }

        public void Deactivated(Queue<string> data)
        {
            // Player sprite
            playerSprite.Deactivated(data);

            data.Enqueue(this.playerScore.ToString());

            data.Enqueue(this.hitPoints.ToString());

            data.Enqueue(this.shotTimer.ToString());

            PlayerShotManager.Deactivated(data);

            data.Enqueue(this.laserTimer.ToString());
            data.Enqueue(this.mineTimer.ToString());
            data.Enqueue(this.largeExplosionTimer.ToString());
            data.Enqueue(this.fastReloadTimer.ToString());
            data.Enqueue(this.timeFreezeTimer.ToString());
            data.Enqueue(this.starTimer.ToString());

            data.Enqueue(this.laserShotTimer.ToString());
            data.Enqueue(this.mineDropTimer.ToString());

            data.Enqueue(smokeTimer.ToString());

            data.Enqueue(scoreTimer.ToString());
            data.Enqueue(nextTimerScore.ToString());

            data.Enqueue(comboMulti.ToString());
            data.Enqueue(comboScore.ToString());
            data.Enqueue(comboElapsedTimer.ToString());

            data.Enqueue(canReloadPeep.ToString());

            data.Enqueue(startUpScale.ToString());

            data.Enqueue(lastVelocityDirection.X.ToString());
            data.Enqueue(lastVelocityDirection.Y.ToString());
        }

        #endregion

        #region Properties

        public bool IsDestroyed
        {
            get
            {
                return this.hitPoints <= 0.0f;
            }
        }

        public float ShotPower
        {
            get
            {
                return this.shotPower;
            }
        }

        public long TotalScore
        {
            get
            {
                return this.playerScore + this.comboMulti * this.comboScore;
            }
        }

        public long PlayerScore
        {
            get
            {
                return this.playerScore;
            }
        }

        public int ComboScore
        {
            get
            {
                return this.comboScore;
            }
        }

        public int ComboMulti
        {
            get
            {
                return this.comboMulti;
            }
        }

        public bool HasCombo
        {
            get
            {
                return this.comboElapsedTimer > 0;
            }
        }

        public float ComboProgress
        {
            get
            {
                if (HasCombo)
                {
                    return this.comboElapsedTimer / ComboElapsedTimerLimit;
                }

                return 0;
            }
        }

        public PlayerType ShipType
        {
            get
            {
                return shipType;
            }
        }

        public float PlayerSpeed
        {
            get
            {
                return this.playerSpeed;
            }
        }

        public float ShotSpeed
        {
            get 
            { 
                return this.shotSpeed; 
            }
        }

        public float ShotTimer
        {
            get
            {
                return this.shotTimer;
            }
        }

        public bool CanFire
        {
            get
            {
                return this.shotTimer < 0;
            }
        }

        public float TimeFreezeSpeedFactor
        {
            get
            {
                float val = 1.0f - MathHelper.Clamp(timeFreezeTimer / TimeFreezeTime, 0.0f, 1.0f);

                if (val <= 0.1f)
                {
                    return 1.0f - (val * 8.5f);
                }
                else
                {
                    return 0.15f + 0.85f * (float)Math.Pow(val, 3);
                }
            }
        }

        private float StarTimeOpacityFactor
        {
            get
            {
                float val = 1.0f - MathHelper.Clamp(starTimer / StarTime, 0.0f, 1.0f);

                return 0.2f + 0.8f * (float)Math.Pow(val, 2);
            }
        }

        public bool IsHitable
        {
            get { return StarTimeOpacityFactor > 0.95f; }
        }

        #endregion

        #region Events

        private void OnAccelerometerHelperReadingChanged(object sender, AccelerometerHelperReadingEventArgs e)
        {
            currentAccValue = new Vector3((float)e.LowPassFilteredAcceleration.X,
                                          (float)e.LowPassFilteredAcceleration.Y,
                                          (float)e.LowPassFilteredAcceleration.Z);
        }

        #endregion
    }
}
