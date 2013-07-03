using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using ScribbleHunter.Inputs;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using ScribbleHunter.Windows.Inputs;
using System.Collections.Generic;

namespace ScribbleHunter
{
    class MainMenuManager
    {
        #region Members

        public enum MenuItems { None, Start, Highscores, Help, Settings };

        private MenuItems lastPressedMenuItem = MenuItems.None;

        private Texture2D texture;

        private Rectangle ScribbleHunterSource = new Rectangle(0, 0,
                                                          480, 200);
        private Rectangle ScribbleHunterDestination = new Rectangle(160, 0,
                                                               480, 200);

        private Rectangle startSource = new Rectangle(0, 400,
                                                      240, 80);
        private Rectangle startDestination = new Rectangle(280, 200,
                                                           240, 80);
        
        private Rectangle highscoresSource = new Rectangle(0, 560,
                                                           240, 80);
        private Rectangle highscoresDestination = new Rectangle(280, 290,
                                                                240, 80);

        private Rectangle settingsSource = new Rectangle(0, 640,
                                                     240, 80);
        private Rectangle settingsDestination = new Rectangle(280, 380,
                                                          240, 80);

        private Rectangle moreGamesSource = new Rectangle(240, 500,
                                                       100, 100);
        private Rectangle moreGamesDestination = new Rectangle(10, 380,
                                                            100, 100);

        private Rectangle helpSource = new Rectangle(240, 400,
                                                     100, 100);
        private Rectangle helpDestination = new Rectangle(690, 380,
                                                          100, 100);

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        private float time = 0.0f;

        private GameInput gameInput;
        private const string StartAction = "Start";
        private const string InstructionsAction = "Instructions";
        private const string HighscoresAction = "Highscores";
        private const string SettingsAction = "Settings";
        private const string HelpAction = "Help";
        private const string MoreGamesAction = "MoreGames";
        private const string MouseSelectAction = "MenuSelect";

        private const string SEARCH_TERM = "Benjamin Sautermeister";

        private SpriteFont font;

        private const string WP_MARKET_WEBSITE = "http://www.windowsphone.com/en-US/search?q=benjamin%20sautermeister";

        #endregion

        #region Constructors

        public MainMenuManager(Texture2D spriteSheet, GameInput input, SpriteFont font)
        {
            this.texture = spriteSheet;
            this.gameInput = input;
            this.font = font;
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            gameInput.AddTouchGestureInput(StartAction,
                                           GestureType.Tap,
                                           startDestination);
            gameInput.AddTouchGestureInput(HighscoresAction,
                                           GestureType.Tap,
                                           highscoresDestination);
            gameInput.AddTouchGestureInput(SettingsAction,
                                           GestureType.Tap,
                                           settingsDestination);
            gameInput.AddTouchGestureInput(HelpAction,
                                           GestureType.Tap,
                                           helpDestination);

            gameInput.AddTouchGestureInput(MoreGamesAction,
                                           GestureType.Tap,
                                           moreGamesDestination);

            // Controller:
            gameInput.AddGamepadInput(StartAction,
                                      Buttons.Start,
                                      true);

            gameInput.AddGamepadInput(MouseSelectAction,
                                      Buttons.A,
                                      true);

            // Keyboard
            gameInput.AddKeyboardInput(
                MouseSelectAction,
                Keys.Enter,
                true);
            gameInput.AddKeyboardInput(
                MouseSelectAction,
                Keys.Space,
                true);
        }

        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;
            }

            time = (float)gameTime.TotalGameTime.TotalSeconds;

            this.handleTouchInputs();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                             ScribbleHunterDestination,
                             ScribbleHunterSource,
                             Color.White * opacity);

            spriteBatch.Draw(texture,
                             startDestination,
                             startSource,
                             Color.White * opacity);

            spriteBatch.Draw(texture,
                             highscoresDestination,
                             highscoresSource,
                             Color.White * opacity);

            spriteBatch.Draw(texture,
                             helpDestination,
                             helpSource,
                             Color.White * opacity);

            spriteBatch.Draw(texture,
                             settingsDestination,
                             settingsSource,
                             Color.White * opacity);

            spriteBatch.Draw(texture,
                            moreGamesDestination,
                            moreGamesSource,
                            Color.White * opacity);
        }

        private async void handleTouchInputs()
        {
            // Start
            if (gameInput.IsPressed(StartAction) || MouseHelper.IsLeftMouseDownDelay(startDestination) ||
                MouseHelper.IsActionPressedInRegion(gameInput, MouseSelectAction, startDestination))
            {
                this.lastPressedMenuItem = MenuItems.Start;
                SoundManager.PlayPaperSound();
            }
            // Highscores
            else if (gameInput.IsPressed(HighscoresAction) || MouseHelper.IsLeftMouseDownDelay(highscoresDestination) ||
                MouseHelper.IsActionPressedInRegion(gameInput, MouseSelectAction, highscoresDestination))
            {
                this.lastPressedMenuItem = MenuItems.Highscores;
                SoundManager.PlayPaperSound();
            }
            // Help
            else if (gameInput.IsPressed(HelpAction) || MouseHelper.IsLeftMouseDownDelay(helpDestination) ||
                MouseHelper.IsActionPressedInRegion(gameInput, MouseSelectAction, helpDestination))
            {
                this.lastPressedMenuItem = MenuItems.Help;
                SoundManager.PlayPaperSound();
            }
            // Settings
            else if (gameInput.IsPressed(SettingsAction) || MouseHelper.IsLeftMouseDownDelay(settingsDestination) ||
                MouseHelper.IsActionPressedInRegion(gameInput, MouseSelectAction, settingsDestination))
            {
                this.lastPressedMenuItem = MenuItems.Settings;
                SoundManager.PlayPaperSound();
            }
            // More games
            else if (gameInput.IsPressed(MoreGamesAction) || MouseHelper.IsLeftMouseDownDelay(moreGamesDestination) ||
                MouseHelper.IsActionPressedInRegion(gameInput, MouseSelectAction, moreGamesDestination))
            {
                SoundManager.PlayPaperSound();
                await global::Windows.System.Launcher.LaunchUriAsync(new Uri(WP_MARKET_WEBSITE));
            }
            else
            {
                this.lastPressedMenuItem = MenuItems.None;
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(Queue<string> data)
        {
            this.lastPressedMenuItem = (MenuItems)Enum.Parse(lastPressedMenuItem.GetType(), data.Dequeue(), false);
            this.opacity = Single.Parse(data.Dequeue());
            this.isActive = Boolean.Parse(data.Dequeue());
            this.time = Single.Parse(data.Dequeue());
        }

        public void Deactivated(Queue<string> data)
        {
            data.Enqueue(lastPressedMenuItem.ToString());
            data.Enqueue(opacity.ToString());
            data.Enqueue(isActive.ToString());
            data.Enqueue(time.ToString());
        }

        #endregion

        #region Properties

        public MenuItems LastPressedMenuItem
        {
            get
            {
                return this.lastPressedMenuItem;
            }
        }

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            set
            {
                this.isActive = value;

                if (isActive == false)
                {
                    this.opacity = OpacityMin;
                }
            }
        }

        #endregion
    }
}
