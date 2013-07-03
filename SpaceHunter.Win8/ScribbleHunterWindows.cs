/****************
 * REMARK: - Remember to set the flags in ALL files
 *         - Set up the correct app name: "ScribbleHunter" / "ScribbleHunter Free"
 * */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Text;
using System.IO;
using ScribbleHunter.Inputs;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScribbleHunter.Windows.Inputs;

namespace ScribbleHunter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ScribbleHunterWindows : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private const string HighscoreText = "Personal Highscore!";
        private const string GameOverText = "GAME OVER!";

        private const string ContinueText = "Push to continue...";
        private string VersionText;
        private const string MusicByText = "Music by";
        private const string MusicCreatorText = "PLSQMPRFKT";
        private const string CreatorText = "by B. Sautermeister";

        enum GameStates
        {
            TitleScreen, MainMenu, Highscores, Help, Settings, Playing, Paused, GameOver,
            Leaderboards, Submittion, PhonePosition
        };

        GameStates gameState = GameStates.TitleScreen;
        GameStates stateBeforePaused;
        Texture2D spriteSheet;
        Texture2D menuSheet;
        Texture2D planetSheet;
        Texture2D paperSheet;
        Texture2D handSheet;
        Texture2D keyboardSheet;
        Texture2D inputsSheet;

        StarFieldManager starFieldManager1;

        PlayerManager playerManager;

        EnemyManager enemyManager;
        BossManager bossManager;

        CollisionManager collisionManager;

        SpriteFont pericles20;
        SpriteFont pericles22;
        SpriteFont pericles32;

        ZoomTextManager zoomTextManager;
        
        private float playerDeathTimer = 0.0f;
        private const float playerDeathDelayTime = 6.0f;
        private const float playerGameOverDelayTime = 5.0f;
        
        private float titleScreenTimer = 0.0f;
        private const float titleScreenDelayTime = 1.0f;

        private Vector2 highscoreLocation = new Vector2(10, 10);

        Hud hud;

        HighscoreManager highscoreManager;
        private bool highscoreMessageShown = false;

        LeaderboardManager leaderboardManager;

        SubmissionManager submissionManager;

        MainMenuManager mainMenuManager;

        private float backButtonTimer = 0.0f;
        private const float backButtonDelayTime = 0.25f;

        LevelManager levelManager;

        HelpManager helpManager;

        PowerUpManager powerUpManager;
        Texture2D powerUpSheet;

        SettingsManager settingsManager;

        GameInput gameInput = new GameInput();
        private const string TitleAction = "Title";
        private const string BackToGameAction = "BackToGame";
        private const string BackToMainAction = "BackToMain";
        private const string WindowsBackAction = "WindowsBack";
        private const string WindowsPauseBackAction = "WindowsPauseBack";
        private const string ActionCursorLeft = "CursorLeft";
        private const string ActionCursorRight = "CursorRight";
        private const string ActionCursorUp = "CursorUp";
        private const string ActionCursorDown = "CursorDown";
        private const string MouseSelectAction = "HunterSelect";

        private readonly Rectangle exitGameSource = new Rectangle(0, 960,
                                                                240, 80);
        private readonly Rectangle exitGameDestination = new Rectangle(480, 370,
                                                                     240, 80);

        private readonly Rectangle continueSource = new Rectangle(0, 880,
                                                                  240, 80);
        private readonly Rectangle continueDestination = new Rectangle(80, 370,
                                                                       240, 80);
        HandManager handManager;

        DamageExplosionManager damageExplosionManager;

        DeviceControlManager phonePositionManager;

        private const int SCORE_SUBMIT_LIMIT = 100;

        private readonly Rectangle screenBounds = new Rectangle(0, 0,
                                                                800, 480);

        private readonly Rectangle windowsBackButtonSource = new Rectangle(750, 1350, 60, 60);
        private readonly Rectangle windowsBackButtonDestination = new Rectangle(25, 25, 60, 60);
        private readonly Rectangle windowsBackPauseButtonDestination = new Rectangle(2, 2, 26, 26);

        private const string STATE_FILE = "state.dat";

        private const string PRIVACY_URL = "http://bsautermeister.de/privacy.php";

        public ScribbleHunterWindows()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);

            Content.RootDirectory = "Content";

            // Frame rate is 60 fps
            TargetElapsedTime = TimeSpan.FromTicks(166667);

            Guide.IsScreenSaverEnabled = false;

            global::Windows.ApplicationModel.Core.CoreApplication.Suspending += (o, args) =>
            {
                // nothing...
            };

            global::Windows.ApplicationModel.Core.CoreApplication.Resuming += (o, args) =>
            {
                // nothing...
            };

            SettingsPane.GetForCurrentView().CommandsRequested += SpacepixxWindows_CommandsRequested;
        }

        void SpacepixxWindows_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var appCommands = args.Request.ApplicationCommands;

            if (appCommands.Count == 0)
            {
                args.Request.ApplicationCommands.Add(new SettingsCommand("privacy", "Privacy Statement", OpenPrivacyInBrowser));
            }
        }

        private async void OpenPrivacyInBrowser(IUICommand command)
        {
            await global::Windows.System.Launcher.LaunchUriAsync(new Uri(PRIVACY_URL));
        }

        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.One;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferHeight = 480;
            graphics.PreferredBackBufferWidth = 800;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;

            // Aplly the gfx changes
            graphics.ApplyChanges();

            TouchPanel.EnabledGestures = GestureType.Tap;

            loadVersion();

            ApplicationViewChanged += Game_ApplicationViewChanged;
            this.Window.ClientSizeChanged += Window_ClientSizeChanged;

            handleScreenViewState();

            base.Initialize();
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            this.GraphicsDevice.Viewport = new Viewport(0, 0, 800, 480);
            graphics.PreferredBackBufferHeight = 480;
            graphics.PreferredBackBufferWidth = 800;
            // Aplly the gfx changes
            graphics.ApplyChanges();
        }

        void Game_ApplicationViewChanged(object sender, ViewStateChangedEventArgs e)
        {
            handleScreenViewState();
        }

        private bool isSnapped = false;

        private void handleScreenViewState()
        {
            isSnapped = (global::Windows.UI.ViewManagement.ApplicationView.Value == global::Windows.UI.ViewManagement.ApplicationViewState.Snapped);

            if (isSnapped)
            {
                SoundManager.MusicOff();
            }
            else
            {
                SoundManager.MusicOn();
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteSheet = Content.Load<Texture2D>(@"Textures\SpriteSheet");
            menuSheet = Content.Load<Texture2D>(@"Textures\MenuSheet");
            powerUpSheet = Content.Load<Texture2D>(@"Textures\PowerUpSheet");
            planetSheet = Content.Load<Texture2D>(@"Textures\PlanetSheet");
            paperSheet = Content.Load<Texture2D>(@"Textures\PaperSheet");
            handSheet = Content.Load<Texture2D>(@"Textures\HandSheet");
            keyboardSheet = Content.Load<Texture2D>(@"Textures\Keyboard");
            inputsSheet = Content.Load<Texture2D>(@"Textures\Inputs");

            starFieldManager1 = new StarFieldManager(this.GraphicsDevice.Viewport.Width,
                                                    this.GraphicsDevice.Viewport.Height,
                                                    100,
                                                    50,
                                                    new Vector2(0, 25.0f),
                                                    new Vector2(0, 50.0f),
                                                    spriteSheet,
                                                    new Rectangle(0, 550, 2, 2),
                                                    new Rectangle(0, 550, 3, 3),
                                                    planetSheet,
                                                    new Rectangle(0, 0, 400, 314),
                                                    2,
                                                    3);

            playerManager = new PlayerManager(spriteSheet,
                                              new Rectangle(0, 100, 45, 45),
                                              6,
                                              new Rectangle(0, 0,
                                                            this.GraphicsDevice.Viewport.Width,
                                                            this.GraphicsDevice.Viewport.Height),
                                              gameInput);

            enemyManager = new EnemyManager(spriteSheet,
                                            playerManager,
                                            new Rectangle(0, 0,
                                                          this.GraphicsDevice.Viewport.Width,
                                                          this.GraphicsDevice.Viewport.Height));

            bossManager = new BossManager(spriteSheet,
                                          playerManager,
                                          new Rectangle(0, 0,
                                                        this.GraphicsDevice.Viewport.Width,
                                                        this.GraphicsDevice.Viewport.Height));

            EffectManager.Initialize(spriteSheet,
                                     new Rectangle(0, 550, 3, 3),
                                     new Rectangle(0, 50, 50, 50),
                                     5);

            powerUpManager = new PowerUpManager(powerUpSheet, playerManager);

            collisionManager = new CollisionManager(playerManager,
                                                    enemyManager,
                                                    bossManager,
                                                    powerUpManager);

            SoundManager.Initialize(Content);

            pericles20 = Content.Load<SpriteFont>(@"Fonts\Pericles20");
            pericles22 = Content.Load<SpriteFont>(@"Fonts\Pericles22");
            pericles32 = Content.Load<SpriteFont>(@"Fonts\Pericles32");

            zoomTextManager = new ZoomTextManager(new Vector2(this.GraphicsDevice.Viewport.Width / 2,
                                                              this.GraphicsDevice.Viewport.Height / 2),
                                                              pericles20,
                                                              pericles32);

            hud = Hud.GetInstance(GraphicsDevice.Viewport.Bounds,
                                  spriteSheet,
                                  pericles22,
                                  0,
                                  bossManager,
                                  playerManager);

            highscoreManager = HighscoreManager.GetInstance();
            HighscoreManager.Font = pericles20;
            HighscoreManager.Texture = menuSheet;

            leaderboardManager = LeaderboardManager.GetInstance();
            LeaderboardManager.Font = pericles22;
            LeaderboardManager.Texture = menuSheet;
            HighscoreManager.GameInput = gameInput;

            submissionManager = SubmissionManager.GetInstance();
            SubmissionManager.Font = pericles22;
            SubmissionManager.Texture = menuSheet;
            SubmissionManager.KeyboardTexture = keyboardSheet;
            SubmissionManager.GameInput = gameInput;

            mainMenuManager = new MainMenuManager(menuSheet, gameInput, pericles20);

            levelManager = new LevelManager();
            levelManager.Register(enemyManager);
            levelManager.Register(bossManager);
            levelManager.Register(playerManager);
            levelManager.Register(powerUpManager);

            helpManager = new HelpManager(menuSheet, pericles22, new Rectangle(0, 0,
                                                                               GraphicsDevice.Viewport.Width,
                                                                               GraphicsDevice.Viewport.Height));
            HelpManager.GameInput = gameInput;
            SoundManager.PlayBackgroundSound();


            settingsManager = SettingsManager.GetInstance();
            settingsManager.Initialize(menuSheet, pericles22, new Rectangle(0, 0,
                                                                            GraphicsDevice.Viewport.Width,
                                                                            GraphicsDevice.Viewport.Height));
            SettingsManager.GameInput = gameInput;

            handManager = new HandManager(handSheet);

            damageExplosionManager = DamageExplosionManager.Instance;
            DamageExplosionManager.Initialize(spriteSheet);

            phonePositionManager = DeviceControlManager.GetInstance();
            DeviceControlManager.Font = pericles22;
            DeviceControlManager.SmallFont = pericles20;
            DeviceControlManager.Texture = menuSheet;
            DeviceControlManager.InputTexture = inputsSheet;
            DeviceControlManager.GameInput = gameInput;

            MouseHelper.Initialize(this, menuSheet, new Rectangle(820, 1350, 30, 30), 800, 480);

            setupInputs();
        }

        private void setupInputs()
        {
            gameInput.AddTouchGestureInput(TitleAction, GestureType.Tap, screenBounds);
            gameInput.AddTouchGestureInput(BackToGameAction, GestureType.Tap, continueDestination);
            gameInput.AddTouchGestureInput(BackToMainAction, GestureType.Tap, exitGameDestination);
            gameInput.AddTouchGestureInput(WindowsBackAction, GestureType.Tap, windowsBackButtonDestination);
            gameInput.AddTouchGestureInput(WindowsPauseBackAction, GestureType.Tap, windowsBackPauseButtonDestination);
            mainMenuManager.SetupInputs();
            playerManager.SetupInputs();
            submissionManager.SetupInputs();
            highscoreManager.SetupInputs();
            settingsManager.SetupInputs();
            helpManager.SetupInputs();
            phonePositionManager.SetupInputs();

            // Controller - Game
            gameInput.AddGamepadInput(
                ActionCursorLeft,
                Buttons.DPadLeft,
                false);
            gameInput.AddGamepadInput(
                ActionCursorRight,
                Buttons.DPadRight,
                false);
            gameInput.AddGamepadInput(
                ActionCursorUp,
                Buttons.DPadUp,
                false);
            gameInput.AddGamepadInput(
                ActionCursorDown,
                Buttons.DPadDown,
                false);

            // Controller - Misc
            gameInput.AddGamepadInput(
                MouseSelectAction,
                Buttons.A,
                true);
            gameInput.AddGamepadInput(
                TitleAction,
                Buttons.A,
                true);
            gameInput.AddGamepadInput(
                TitleAction,
                Buttons.Start,
                true);

            // Keyboard
            gameInput.AddKeyboardInput(
                TitleAction,
                Keys.Enter,
                true);

            gameInput.AddKeyboardInput(
                TitleAction,
                Keys.Space,
                true);

            gameInput.AddKeyboardInput(
                MouseSelectAction,
                Keys.Enter,
                true);
            gameInput.AddKeyboardInput(
                MouseSelectAction,
                Keys.Space,
                true);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            if (PreviousExecutionState == global::Windows.ApplicationModel.Activation.ApplicationExecutionState.Running ||
                PreviousExecutionState == global::Windows.ApplicationModel.Activation.ApplicationExecutionState.Suspended)
            {
                LoadGameState();
            }
        }

        /// <summary>
        /// Pauses the game when a call is incoming.
        /// Attention: Also called for GUID !!!
        /// </summary>
        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);

            if ((gameState == GameStates.Playing))
            {
                stateBeforePaused = gameState;
                gameState = GameStates.Paused;
            }

            SaveGameState();
        }

        /// <summary>
        /// Occurs when the game class (and application) deactivated and tombstoned.
        /// Saves state of: Spacepixx, AsteroidManager
        /// Does not save: Starfield-Manager (not necessary)
        /// </summary>
        private async void SaveGameState()
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(STATE_FILE, CreationCollisionOption.OpenIfExists);
            IList<string> dataToWrite = new List<string>();
            Queue<string> dataQueue = new Queue<string>(dataToWrite);

            // Write date to the file
            dataQueue.Enqueue(this.gameState.ToString());
            dataQueue.Enqueue(this.stateBeforePaused.ToString());
            dataQueue.Enqueue(this.playerDeathTimer.ToString());
            dataQueue.Enqueue(this.titleScreenTimer.ToString());
            dataQueue.Enqueue(this.highscoreMessageShown.ToString());
            dataQueue.Enqueue(this.backButtonTimer.ToString());

            playerManager.Deactivated(dataQueue);

            enemyManager.Deactivated(dataQueue);

            bossManager.Deactivated(dataQueue);

            EffectManager.Deactivated(dataQueue);

            powerUpManager.Deactivated(dataQueue);

            zoomTextManager.Deactivated(dataQueue);

            levelManager.Deactivated(dataQueue);

            mainMenuManager.Deactivated(dataQueue);

            highscoreManager.Deactivated(dataQueue);

            submissionManager.Deactivated(dataQueue);

            starFieldManager1.Deactivated(dataQueue);

            handManager.Deactivated(dataQueue);

            damageExplosionManager.Deactivated(dataQueue);

            phonePositionManager.Deactivated(dataQueue);

            settingsManager.Deactivated(dataQueue);

            await FileIO.WriteLinesAsync(file, dataQueue);
        }

        private async void LoadGameState()
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.CreateFileAsync(STATE_FILE, CreationCollisionOption.OpenIfExists);
                IList<string> readData = await FileIO.ReadLinesAsync(file);
                Queue<string> dataQueue = new Queue<string>(readData);

                if (readData.Count > 0)
                {
                    this.gameState = (GameStates)Enum.Parse(gameState.GetType(), dataQueue.Dequeue(), true);
                    this.stateBeforePaused = (GameStates)Enum.Parse(stateBeforePaused.GetType(), dataQueue.Dequeue(), true);

                    if (gameState == GameStates.Playing)
                    {
                        gameState = GameStates.Paused;
                        stateBeforePaused = GameStates.Playing;
                    }

                    this.playerDeathTimer = (float)Single.Parse(dataQueue.Dequeue());
                    this.titleScreenTimer = (float)Single.Parse(dataQueue.Dequeue());
                    this.highscoreMessageShown = (bool)Boolean.Parse(dataQueue.Dequeue());
                    this.backButtonTimer = (float)Single.Parse(dataQueue.Dequeue());

                    playerManager.Activated(dataQueue);

                    enemyManager.Activated(dataQueue);

                    bossManager.Activated(dataQueue);

                    EffectManager.Activated(dataQueue);

                    powerUpManager.Activated(dataQueue);

                    zoomTextManager.Activated(dataQueue);

                    levelManager.Activated(dataQueue);

                    mainMenuManager.Activated(dataQueue);

                    highscoreManager.Activated(dataQueue);

                    submissionManager.Activated(dataQueue);

                    starFieldManager1.Activated(dataQueue);

                    handManager.Activated(dataQueue);

                    damageExplosionManager.Activated(dataQueue);

                    phonePositionManager.Activated(dataQueue);

                    settingsManager.Activated(dataQueue);
                }

                await DeleteStatFile();
            }
            catch (Exception)
            {
                // catch end restore in case of incompatible active/deactivate dat-files
                this.resetGame();
                this.gameState = GameStates.TitleScreen;
            }
        }

        private async Task DeleteStatFile()
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync(STATE_FILE);

                if (file != null)
                {
                    await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (isSnapped)
            {
                updateBackground(elapsed);
            }
            else
            {
                gameInput.BeginUpdate();

                if (gameState == GameStates.Playing || gameState == GameStates.GameOver)
                {
                    MouseHelper.IsCrosshair = true;
                }
                else
                {
                    MouseHelper.IsCrosshair = false;
                }
                MouseHelper.Update(elapsed);

                KeyboardHelper.Update(elapsed);

                if (gameState != GameStates.Playing && gameState != GameStates.GameOver)
                {
                    int x = 0;
                    int y = 0;

                    if (KeyboardHelper.IsKeyDown(Keys.Left) || GamepadHelper.IsLeftThumbstickLeft() || GamepadHelper.IsRightThumbstickLeft() || gameInput.IsPressed(ActionCursorLeft))
                    {
                        x -= 1;
                    }
                    if (KeyboardHelper.IsKeyDown(Keys.Right) || GamepadHelper.IsLeftThumbstickRight() || GamepadHelper.IsRightThumbstickRight() || gameInput.IsPressed(ActionCursorRight))
                    {
                        x += 1;
                    }
                    if (KeyboardHelper.IsKeyDown(Keys.Up) || GamepadHelper.IsLeftThumbstickUp() || GamepadHelper.IsRightThumbstickUp() || gameInput.IsPressed(ActionCursorUp))
                    {
                        y -= 1;
                    }
                    if (KeyboardHelper.IsKeyDown(Keys.Down) || GamepadHelper.IsLeftThumbstickDown() || GamepadHelper.IsRightThumbstickDown() || gameInput.IsPressed(ActionCursorDown))
                    {
                        y += 1;
                    }

                    if (x != 0 || y != 0)
                    {
                        MouseHelper.Move(new Vector2(x, y));
                    }
                }
                else
                {
                    int x = 0;
                    int y = 0;

                    if (KeyboardHelper.IsKeyDown(Keys.A) || GamepadHelper.IsRightThumbstickLeft())
                    {
                        x -= 1;
                    }
                    if (KeyboardHelper.IsKeyDown(Keys.D) || GamepadHelper.IsRightThumbstickRight())
                    {
                        x += 1;
                    }
                    if (KeyboardHelper.IsKeyDown(Keys.W) || GamepadHelper.IsRightThumbstickUp())
                    {
                        y -= 1;
                    }
                    if (KeyboardHelper.IsKeyDown(Keys.S) || GamepadHelper.IsRightThumbstickDown())
                    {
                        y += 1;
                    }

                    if (x != 0 || y != 0)
                    {
                        MouseHelper.Move(new Vector2(x, y));
                    }
                }

                bool backButtonPressed = false;

                backButtonTimer += elapsed;

                if (backButtonTimer >= backButtonDelayTime)
                {
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || KeyboardHelper.IsKeyDownDelay(Keys.Escape))
                    {
                        backButtonPressed = true;
                        backButtonTimer = 0.0f;
                    }

                    if (gameState == GameStates.Help || gameState == GameStates.Settings || gameState == GameStates.Highscores)
                    {
                        if (gameInput.IsPressed(WindowsBackAction) || MouseHelper.IsLeftMouseDownDelay(windowsBackButtonDestination) ||
                            (MouseHelper.IsActionPressedInRegion(gameInput, MouseSelectAction, windowsBackButtonDestination)))
                        {
                            backButtonPressed = true;
                            backButtonTimer = 0.0f;
                        }
                    }

                    if (gameState == GameStates.Playing || gameState == GameStates.GameOver)
                    {
                        if (gameInput.IsPressed(WindowsPauseBackAction))
                        {
                            backButtonPressed = true;
                            backButtonTimer = 0.0f;
                        }
                    }
                }

                switch (gameState)
                {
                    case GameStates.TitleScreen:

                        titleScreenTimer += elapsed;

                        updateBackground(elapsed);

                        EffectManager.Update(elapsed);
                        damageExplosionManager.Update(gameTime);

                        if (titleScreenTimer >= titleScreenDelayTime)
                        {
                            if (gameInput.IsPressed(TitleAction) || KeyboardHelper.IsKeyDownDelay(Keys.Space) || KeyboardHelper.IsKeyDownDelay(Keys.Enter) || MouseHelper.IsLeftMouseDownDelay(screenBounds))
                            {
                                gameState = GameStates.MainMenu;
                                SoundManager.PlayPaperSound();
                            }
                        }

                        if (backButtonPressed)
                            this.Exit();

                        break;

                    case GameStates.MainMenu:

                        updateBackground(elapsed);

                        EffectManager.Update(elapsed);
                        damageExplosionManager.Update(gameTime);

                        mainMenuManager.IsActive = true;
                        mainMenuManager.Update(gameTime);

                        handManager.ShowHands();

                        switch (mainMenuManager.LastPressedMenuItem)
                        {
                            case MainMenuManager.MenuItems.Start:
                                gameState = GameStates.PhonePosition;
                                break;

                            case MainMenuManager.MenuItems.Highscores:
                                leaderboardManager.Receive();
                                gameState = GameStates.Highscores;
                                break;

                            case MainMenuManager.MenuItems.Help:
                                gameState = GameStates.Help;
                                break;

                            case MainMenuManager.MenuItems.Settings:
                                gameState = GameStates.Settings;
                                break;

                            case MainMenuManager.MenuItems.None:
                                // do nothing
                                break;
                        }

                        if (gameState != GameStates.MainMenu)
                        {
                            mainMenuManager.IsActive = false;
                            handManager.HideHands();
                        }

                        if (backButtonPressed)
                            this.Exit();

                        break;

                    case GameStates.PhonePosition:

                        updateBackground(elapsed);

                        EffectManager.Update(elapsed);
                        damageExplosionManager.Update(gameTime);

                        phonePositionManager.IsActive = true;
                        phonePositionManager.Update(gameTime);

                        if (phonePositionManager.CancelClicked || backButtonPressed)
                        {
                            phonePositionManager.IsActive = false;
                            gameState = GameStates.MainMenu;
                            SoundManager.PlayPaperSound();
                        }
                        else if (phonePositionManager.StartClicked)
                        {
                            phonePositionManager.IsActive = false;
                            resetGame();
                            updateHud(elapsed);
                            handManager.HideHandsAndScribble();
                            gameState = GameStates.Playing;
                            SoundManager.PlayPaperSound();
                        }

                        break;

                    case GameStates.Highscores:

                        updateBackground(elapsed);

                        EffectManager.Update(elapsed);
                        damageExplosionManager.Update(gameTime);

                        highscoreManager.IsActive = true;
                        highscoreManager.Update(gameTime);

                        if (backButtonPressed)
                        {
                            highscoreManager.IsActive = false;
                            gameState = GameStates.MainMenu;
                            SoundManager.PlayPaperSound();
                        }

                        break;

                    case GameStates.Submittion:

                        updateBackground(elapsed);

                        EffectManager.Update(elapsed);
                        damageExplosionManager.Update(gameTime);

                        submissionManager.IsActive = true;
                        submissionManager.Update(gameTime);

                        highscoreMessageShown = false;

                        zoomTextManager.Reset();

                        if (submissionManager.CancelClicked || backButtonPressed)
                        {
                            submissionManager.IsActive = false;

                            string name = submissionManager.Name;

                            if (name.Length == 0)
                            {
                                name = Highscore.DEFAULT_NAME;
                            }

                            highscoreManager.SaveHighScore(name,
                                                           playerManager.TotalScore,
                                                           levelManager.CurrentLevel);

                            gameState = GameStates.MainMenu;
                            SoundManager.PlayPaperSound();
                        }
                        else if (submissionManager.RetryClicked)
                        {
                            submissionManager.IsActive = false;
                            resetGame();
                            updateHud(elapsed);

                            string name = submissionManager.Name;

                            if (name.Length == 0)
                            {
                                name = Highscore.DEFAULT_NAME;
                            }

                            highscoreManager.SaveHighScore(name,
                                                           playerManager.TotalScore,
                                                           levelManager.CurrentLevel);
                            handManager.HideHandsAndScribble();
                            gameState = GameStates.Playing;
                        }

                        break;

                    case GameStates.Help:

                        updateBackground(elapsed);

                        EffectManager.Update(elapsed);
                        damageExplosionManager.Update(gameTime);

                        helpManager.IsActive = true;
                        helpManager.Update(gameTime);

                        if (backButtonPressed)
                        {
                            helpManager.IsActive = false;
                            gameState = GameStates.MainMenu;
                            SoundManager.PlayPaperSound();
                        }

                        break;

                    case GameStates.Settings:

                        updateBackground(elapsed);

                        EffectManager.Update(elapsed);
                        damageExplosionManager.Update(gameTime);

                        settingsManager.IsActive = true;
                        settingsManager.Update(gameTime);

                        if (backButtonPressed)
                        {
                            settingsManager.IsActive = false;
                            gameState = GameStates.MainMenu;
                            SoundManager.PlayPaperSound();
                        }

                        break;

                    case GameStates.Playing:

                        updateBackground(elapsed * playerManager.TimeFreezeSpeedFactor);

                        levelManager.Update(gameTime);

                        playerManager.Update(gameTime);

                        enemyManager.Update(elapsed * playerManager.TimeFreezeSpeedFactor);
                        enemyManager.IsActive = true;

                        bossManager.Update(elapsed * playerManager.TimeFreezeSpeedFactor);
                        bossManager.IsActive = true;

                        EffectManager.Update(elapsed * playerManager.TimeFreezeSpeedFactor);
                        damageExplosionManager.Update(gameTime);

                        collisionManager.Update();

                        powerUpManager.Update(gameTime);

                        zoomTextManager.Update();

                        updateHud(elapsed);

                        if (levelManager.HasChanged)
                        {
                            levelManager.GoToNextState();
                        }

                        if (playerManager.TotalScore > highscoreManager.CurrentHighscore &&
                            highscoreManager.CurrentHighscore != 0 &&
                           !highscoreMessageShown)
                        {
                            zoomTextManager.ShowText(HighscoreText);
                            highscoreMessageShown = true;
                        }

                        if (playerManager.IsDestroyed)
                        {
                            playerDeathTimer = 0.0f;
                            enemyManager.IsActive = false;
                            bossManager.IsActive = false;

                            gameState = GameStates.GameOver;
                            zoomTextManager.ShowText(GameOverText);
                        }

                        if (backButtonPressed)
                        {
                            stateBeforePaused = GameStates.Playing;
                            gameState = GameStates.Paused;
                            SoundManager.PlayPaperSound();
                        }

                        break;

                    case GameStates.Paused:

                        if (gameInput.IsPressed(BackToGameAction) || backButtonPressed || MouseHelper.IsLeftMouseDownDelay(continueDestination) ||
                            MouseHelper.IsActionPressedInRegion(gameInput, MouseSelectAction, continueDestination))
                        {
                            gameState = stateBeforePaused;
                            SoundManager.PlayPaperSound();
                        }

                        if (gameInput.IsPressed(BackToMainAction) || MouseHelper.IsLeftMouseDownDelay(exitGameDestination) ||
                            MouseHelper.IsActionPressedInRegion(gameInput, MouseSelectAction, exitGameDestination))
                        {
                            SoundManager.PlayPaperSound();

                            if (playerManager.TotalScore > SCORE_SUBMIT_LIMIT)
                            {
                                gameState = GameStates.Submittion;
                                submissionManager.SetUp(highscoreManager.LastName, playerManager.TotalScore, levelManager.CurrentLevel);
                            }
                            else
                            {
                                gameState = GameStates.MainMenu;
                            }
                        }

                        break;

                    case GameStates.GameOver:

                        playerDeathTimer += elapsed;

                        updateBackground(elapsed * playerManager.TimeFreezeSpeedFactor);

                        playerManager.Update(gameTime);
                        powerUpManager.Update(gameTime);
                        enemyManager.Update(elapsed * playerManager.TimeFreezeSpeedFactor);
                        bossManager.Update(elapsed * playerManager.TimeFreezeSpeedFactor);
                        EffectManager.Update(elapsed * playerManager.TimeFreezeSpeedFactor);
                        damageExplosionManager.Update(gameTime);
                        collisionManager.Update();
                        zoomTextManager.Update();

                        updateHud(elapsed);

                        if (playerDeathTimer >= playerGameOverDelayTime)
                        {
                            playerDeathTimer = 0.0f;
                            titleScreenTimer = 0.0f;

                            if (playerManager.TotalScore > SCORE_SUBMIT_LIMIT)
                            {
                                gameState = GameStates.Submittion;
                                submissionManager.SetUp(highscoreManager.LastName, playerManager.TotalScore, levelManager.CurrentLevel);
                            }
                            else
                            {
                                gameState = GameStates.MainMenu;
                            }

                            EffectManager.Reset();
                        }

                        if (backButtonPressed)
                        {
                            stateBeforePaused = GameStates.GameOver;
                            gameState = GameStates.Paused;
                            SoundManager.PlayPaperSound();
                        }

                        break;
                }

                // Reset Back-Button flag
                backButtonPressed = false;

                gameInput.EndUpdate();
            }

            SoundManager.Update(gameTime);

            handManager.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (isSnapped)
            {
                drawSnappedPaper(spriteBatch);

                spriteBatch.Draw(menuSheet,
                                     new Rectangle(
                                         100,
                                         this.GraphicsDevice.Viewport.Height / 2 - 35,
                                         600,
                                         70),
                                     new Rectangle(0, 0,
                                                   480,
                                                   200),
                                     Color.White);
            }
            else
            {
                if (gameState == GameStates.TitleScreen)
                {
                    drawBackground(spriteBatch);

                    // title
                    spriteBatch.Draw(menuSheet,
                                     new Vector2(160.0f, 50.0f),
                                     new Rectangle(0, 0,
                                                   480,
                                                   200),
                                     Color.White);

                    spriteBatch.DrawString(pericles22,
                                           ContinueText,
                                           new Vector2(this.GraphicsDevice.Viewport.Width / 2 - pericles22.MeasureString(ContinueText).X / 2,
                                                       285),
                                           Color.Black * (0.25f + (float)(Math.Pow(Math.Sin(gameTime.TotalGameTime.TotalSeconds), 2.0f)) * 0.75f));

                    spriteBatch.DrawString(pericles20,
                                           MusicByText,
                                           new Vector2(this.GraphicsDevice.Viewport.Width / 2 - pericles22.MeasureString(MusicByText).X / 2,
                                                       405),
                                           Color.Black);
                    spriteBatch.DrawString(pericles20,
                                           MusicCreatorText,
                                           new Vector2(this.GraphicsDevice.Viewport.Width / 2 - pericles22.MeasureString(MusicCreatorText).X / 2,
                                                       435),
                                           Color.Black);

                    spriteBatch.DrawString(pericles20,
                                           VersionText,
                                           new Vector2(this.GraphicsDevice.Viewport.Width - (pericles20.MeasureString(VersionText).X + 15),
                                                       this.GraphicsDevice.Viewport.Height - (pericles20.MeasureString(VersionText).Y + 10)),
                                           Color.Black);

                    spriteBatch.DrawString(pericles20,
                                           CreatorText,
                                           new Vector2(15,
                                                       this.GraphicsDevice.Viewport.Height - (pericles20.MeasureString(CreatorText).Y + 10)),
                                           Color.Black);
                }

                if (gameState == GameStates.MainMenu)
                {
                    drawBackground(spriteBatch);

                    mainMenuManager.Draw(spriteBatch);

                    handManager.Draw(spriteBatch);
                }

                if (gameState == GameStates.Highscores)
                {
                    drawBackground(spriteBatch);

                    highscoreManager.Draw(spriteBatch);

                    handManager.Draw(spriteBatch);
                }

                if (gameState == GameStates.Submittion)
                {
                    drawBackground(spriteBatch);

                    submissionManager.Draw(spriteBatch);

                    handManager.Draw(spriteBatch);
                }

                if (gameState == GameStates.PhonePosition)
                {
                    drawBackground(spriteBatch);

                    phonePositionManager.Draw(spriteBatch);

                    handManager.Draw(spriteBatch);
                }

                if (gameState == GameStates.Help)
                {
                    drawBackground(spriteBatch);

                    helpManager.Draw(spriteBatch);

                    handManager.Draw(spriteBatch);
                }

                if (gameState == GameStates.Settings)
                {
                    drawBackground(spriteBatch);

                    settingsManager.Draw(spriteBatch);

                    handManager.Draw(spriteBatch);
                }

                if (gameState == GameStates.Paused)
                {
                    drawBackground(spriteBatch);

                    powerUpManager.Draw(spriteBatch);

                    playerManager.Draw(spriteBatch);

                    enemyManager.Draw(spriteBatch);

                    bossManager.Draw(spriteBatch);

                    EffectManager.Draw(spriteBatch);
                    damageExplosionManager.Draw(spriteBatch);

                    handManager.Draw(spriteBatch);

                    // Pause title

                    spriteBatch.Draw(spriteSheet,
                                     new Rectangle(0, 0, 800, 480),
                                     new Rectangle(0, 550, 1, 1),
                                     Color.White * 0.5f);

                    spriteBatch.Draw(menuSheet,
                                     new Vector2(160.0f, 150.0f),
                                     new Rectangle(0, 200,
                                                   480,
                                                   100),
                                     Color.White * (0.25f + (float)(Math.Pow(Math.Sin(gameTime.TotalGameTime.TotalSeconds), 2.0f)) * 0.75f));

                    spriteBatch.Draw(menuSheet,
                                     exitGameDestination,
                                     exitGameSource,
                                     Color.White);

                    spriteBatch.Draw(menuSheet,
                                     continueDestination,
                                     continueSource,
                                     Color.White);
                }

                if (gameState == GameStates.Playing ||
                    gameState == GameStates.GameOver)
                {
                    drawBackground(spriteBatch);

                    powerUpManager.Draw(spriteBatch);

                    playerManager.Draw(spriteBatch);

                    enemyManager.Draw(spriteBatch);

                    bossManager.Draw(spriteBatch);

                    EffectManager.Draw(spriteBatch);
                    damageExplosionManager.Draw(spriteBatch);

                    zoomTextManager.Draw(spriteBatch);

                    handManager.Draw(spriteBatch);

                    hud.Draw(spriteBatch);

                    spriteBatch.Draw(
                        menuSheet,
                        windowsBackPauseButtonDestination,
                        windowsBackButtonSource,
                        Color.White);
                }

                if (gameState == GameStates.Help || gameState == GameStates.Settings || gameState == GameStates.Highscores)
                {
                    spriteBatch.Draw(
                        menuSheet,
                        windowsBackButtonDestination,
                        windowsBackButtonSource,
                        Color.White);
                }

                if (gameState != GameStates.TitleScreen)
                {
                    if (!((gameState == GameStates.Playing || gameState == GameStates.GameOver) && phonePositionManager.CurrentControlState == DeviceControlManager.ControlState.TouchAccelerometer))
                    {
                        MouseHelper.Draw(spriteBatch);
                    }
                    
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Helper method to reduce update redundace.
        /// </summary>
        private void updateBackground(float elapsed)
        {
            starFieldManager1.Update(elapsed);
        }

        /// <summary>
        /// Helper method to reduce draw redundace.
        /// </summary>
        private void drawBackground(SpriteBatch spriteBatch)
        {
            drawPaper(spriteBatch);

            starFieldManager1.Draw(spriteBatch);
        }

        private void drawPaper(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(paperSheet,
                                 new Rectangle(0, 0,
                                               800, 480),
                                 new Rectangle(0, 0,
                                               800, 480),
                                 Color.White);
        }

        private void drawSnappedPaper(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(paperSheet,
                                 new Rectangle(0, 0,
                                               800, 480),
                                 new Rectangle(0, 0,
                                               320, 480),
                                 Color.White);
        }

        private void resetRound()
        {
            enemyManager.Reset();
            bossManager.Reset();
            playerManager.Reset();
            EffectManager.Reset();
            powerUpManager.Reset();

            zoomTextManager.Reset();
        }

        private void resetGame()
        {
            resetRound();

            levelManager.Reset();

            playerManager.ResetPlayerScore();

            GC.Collect();
        }

        /// <summary>
        /// Loads the current version from assembly.
        /// </summary>
        private void loadVersion()
        {
            this.VersionText = "1.3";
        }

        private void updateHud(float elapsed)
        {
            hud.Update(elapsed, levelManager.CurrentLevel);
        }
    }
}
