using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using ScribbleHunter.Inputs;
using ScribbleHunter.Windows.Inputs;
using Windows.Storage;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace ScribbleHunter
{
    class SettingsManager
    {
        #region Members

        private const string SETTINGS_FILE = "settings.txt";

        private static SettingsManager settingsManager;

        private static Texture2D texture;
        private static SpriteFont font;
        private readonly Rectangle SettingsTitleSource = new Rectangle(0,1360,
                                                                       240, 80);
        private readonly Vector2 TitlePosition = new Vector2(280.0f, 15.0f);

        public enum SoundValues {Off, VeryLow, Low, Med, High, VeryHigh};
        public enum ToggleValues { On, Off };
        public enum NeutralPositionValues { Angle0, Angle10, Angle20, Angle30, Angle40, Angle50, Angle60, Unsupported };

        private const string MUSIC_TITLE = "Music: ";
        private SoundValues musicValue = SoundValues.Med;
        private readonly int musicPositionY = 200;
        private readonly Rectangle musicDestination = new Rectangle(250, 195,
                                                                    300, 50);

        private const string SFX_TITLE = "SFX: ";
        private SoundValues sfxValue = SoundValues.High;
        private readonly int sfxPositionY = 270;
        private readonly Rectangle sfxDestination = new Rectangle(250, 265,
                                                                  300, 50);

        private NeutralPositionValues neutralPositionValue = NeutralPositionValues.Angle20;

        private static Rectangle screenBounds;

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        public static GameInput GameInput;
        private const string MusicAction = "Music";
        private const string SfxAction = "SFX";
        private const string MouseSelectAction = "SetingsSelect";

        private const string ON = "ON";
        private const string OFF = "OFF";
        private const string VERY_LOW = "VERY LOW";
        private const string LOW = "LOW";
        private const string MEDIUM = "MEDIUM";
        private const string HIGH = "HIGH";
        private const string VERY_HIGH = "VERY HIGH";

        private const int TextPositonX = 250;
        private const int ValuePositionX = 550;

        private bool isInvalidated = false;

        #endregion

        #region Constructors

        private SettingsManager()
        {
            this.Load();
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            GameInput.AddTouchGestureInput(MusicAction,
                                           GestureType.Tap,
                                           musicDestination);
            GameInput.AddTouchGestureInput(SfxAction,
                                           GestureType.Tap,
                                           sfxDestination);

            // Controller:
            GameInput.AddGamepadInput(MouseSelectAction,
                                      Buttons.A,
                                      true);

            // Keyboard
            GameInput.AddKeyboardInput(
                MouseSelectAction,
                Keys.Enter,
                true);
            GameInput.AddKeyboardInput(
                MouseSelectAction,
                Keys.Space,
                true);
        }

        public void Initialize(Texture2D tex, SpriteFont f, Rectangle screen)
        {
            texture = tex;
            font = f;
            screenBounds = screen;
        }

        public static SettingsManager GetInstance()
        {
            if (settingsManager == null)
            {
                settingsManager = new SettingsManager();
            }

            return settingsManager;
        }

        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;
            }

            handleTouchInputs();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                             TitlePosition,
                             SettingsTitleSource,
                             Color.White * opacity);

            drawMusic(spriteBatch);
            drawSfx(spriteBatch);
        }

        private void handleTouchInputs()
        {
            // Music
            if (GameInput.IsPressed(MusicAction) || MouseHelper.IsLeftMouseDownDelay(musicDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, musicDestination))
            {
                toggleMusic();
            }
            // Sfx
            else if (GameInput.IsPressed(SfxAction) || MouseHelper.IsLeftMouseDownDelay(sfxDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, sfxDestination))
            {
                toggleSfx();
            }
        }

        private void toggleMusic()
        {
            switch (musicValue)
            {
                case SoundValues.Off:
                    musicValue = SoundValues.VeryLow;
                    break;
                case SoundValues.VeryLow:
                    musicValue = SoundValues.Low;
                    break;
                case SoundValues.Low:
                    musicValue = SoundValues.Med;
                    break;
                case SoundValues.Med:
                    musicValue = SoundValues.High;
                    break;
                case SoundValues.High:
                    musicValue = SoundValues.VeryHigh;
                    break;
                case SoundValues.VeryHigh:
                    musicValue = SoundValues.Off;
                    break;
            }
            isInvalidated = true;
            SoundManager.RefreshMusicVolume();
        }

        private void toggleSfx()
        {
            switch (sfxValue)
            {
                case SoundValues.Off:
                    sfxValue = SoundValues.VeryLow;
                    break;
                case SoundValues.VeryLow:
                    sfxValue = SoundValues.Low;
                    break;
                case SoundValues.Low:
                    sfxValue = SoundValues.Med;
                    break;
                case SoundValues.Med:
                    sfxValue = SoundValues.High;
                    break;
                case SoundValues.High:
                    sfxValue = SoundValues.VeryHigh;
                    break;
                case SoundValues.VeryHigh:
                    sfxValue = SoundValues.Off;
                    break;
            }
            isInvalidated = true;
            if (sfxValue != SoundValues.Off)
                SoundManager.PlayPlayerShot();
        }

        public void SetNeutralPosition(NeutralPositionValues value)
        {
            this.neutralPositionValue = value;
        }

        private void drawMusic(SpriteBatch spriteBatch)
        {
            string text;

            switch (musicValue)
            {
                case SoundValues.VeryLow:
                    text = VERY_LOW;
                    break;
                case SoundValues.Low:
                    text = LOW;
                    break;
                case SoundValues.Med:
                    text = MEDIUM;
                    break;
                case SoundValues.High:
                    text = HIGH;
                    break;
                case SoundValues.VeryHigh:
                    text = VERY_HIGH;
                    break;
                default:
                    text = OFF;
                    break;
            }

            spriteBatch.DrawString(font,
                                   MUSIC_TITLE,
                                   new Vector2(TextPositonX,
                                               musicPositionY),
                                   Color.Black * opacity);

            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2((ValuePositionX - font.MeasureString(text).X),
                                               musicPositionY),
                                   Color.Black * opacity);
        }

        private void drawSfx(SpriteBatch spriteBatch)
        {
            string text;

            switch (sfxValue)
            {
                case SoundValues.VeryLow:
                    text = VERY_LOW;
                    break;
                case SoundValues.Low:
                    text = LOW;
                    break;
                case SoundValues.Med:
                    text = MEDIUM;
                    break;
                case SoundValues.High:
                    text = HIGH;
                    break;
                case SoundValues.VeryHigh:
                    text = VERY_HIGH;
                    break;
                default:
                    text = OFF;
                    break;
            }

            spriteBatch.DrawString(font,
                                   SFX_TITLE,
                                   new Vector2(TextPositonX,
                                               sfxPositionY),
                                   Color.Black * opacity);

            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2((ValuePositionX - font.MeasureString(text).X),
                                               sfxPositionY),
                                   Color.Black * opacity);
        }

        #endregion

        #region Load/Save

        public async void Save()
        {
            if (!isInvalidated)
                return;

            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(SETTINGS_FILE, CreationCollisionOption.OpenIfExists);
            IList<string> settingsToWrite = new List<string>();

            settingsToWrite.Add(this.musicValue.ToString());
            settingsToWrite.Add(this.sfxValue.ToString());

            await FileIO.WriteLinesAsync(file, settingsToWrite);
        }

        public async void Load()
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(SETTINGS_FILE, CreationCollisionOption.OpenIfExists);
            isInvalidated = false;
            IList<string> readStrings = await FileIO.ReadLinesAsync(file);

            if (readStrings.Count > 0)
            {
                this.musicValue = (SoundValues)Enum.Parse(musicValue.GetType(), readStrings[0], true);
                this.sfxValue = (SoundValues)Enum.Parse(sfxValue.GetType(), readStrings[1], true);
            }
            else
            {
                IList<string> settingsToWrite = new List<string>();

                settingsToWrite.Add(this.musicValue.ToString());
                settingsToWrite.Add(this.sfxValue.ToString());

                await FileIO.WriteLinesAsync(file, settingsToWrite);
            }
        }

        public float GetMusicValue()
        {
            switch (settingsManager.musicValue)
            {
                case SoundValues.Off:
                    return 0.0f;

                case SoundValues.VeryLow:
                    return 0.1f;

                case SoundValues.Low:
                    return 0.2f;

                case SoundValues.Med:
                    return 0.3f;

                case SoundValues.High:
                    return 0.4f;

                case SoundValues.VeryHigh:
                    return 0.5f;

                default:
                    return 0.3f;
            }
        }

        public float GetSfxValue()
        {
            switch (settingsManager.sfxValue)
            {
                case SoundValues.Off:
                    return 0.0f;

                case SoundValues.VeryLow:
                    return 0.2f;

                case SoundValues.Low:
                    return 0.4f;

                case SoundValues.Med:
                    return 0.6f;

                case SoundValues.High:
                    return 0.8f;

                case SoundValues.VeryHigh:
                    return 1.0f;

                default:
                    return 0.6f;
            }
        }

        public float GetNeutralPosition()
        {
            return GetNeutralPositionValue(settingsManager.neutralPositionValue);
        }

        private float GetNeutralPositionValue(NeutralPositionValues value)
        {
            switch (value)
            {
                case NeutralPositionValues.Angle0:
                    return 0.0f;

                case NeutralPositionValues.Angle10:
                    return (float)Math.PI * 10.0f / 180.0f;

                case NeutralPositionValues.Angle20:
                    return (float)Math.PI * 20.0f / 180.0f;

                case NeutralPositionValues.Angle30:
                    return (float)Math.PI * 30.0f / 180.0f;

                case NeutralPositionValues.Angle40:
                    return (float)Math.PI * 40.0f / 180.0f;

                case NeutralPositionValues.Angle50:
                    return (float)Math.PI * 50.0f / 180.0f;

                case NeutralPositionValues.Angle60:
                    return (float)Math.PI * 60.0f / 180.0f;

                default:
                    return 0.0f;
            }
        }

        public float GetNeutralPositionRadianValue(float angle)
        {
            return (float)Math.PI * angle / 180.0f;
        }

        public int GetNeutralPositionIndex()
        {
            switch (settingsManager.neutralPositionValue)
            {
                case NeutralPositionValues.Angle0:
                    return 0;

                case NeutralPositionValues.Angle10:
                    return 1;

                case NeutralPositionValues.Angle20:
                    return 2;

                case NeutralPositionValues.Angle30:
                    return 3;

                case NeutralPositionValues.Angle40:
                    return 4;

                case NeutralPositionValues.Angle50:
                    return 5;

                case NeutralPositionValues.Angle60:
                    return 6;

                default:
                    return -1;
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(Queue<string> data)
        {
            opacity = Single.Parse(data.Dequeue());
            isInvalidated = Boolean.Parse(data.Dequeue());
            neutralPositionValue = (NeutralPositionValues)Enum.Parse(neutralPositionValue.GetType(), data.Dequeue(), false);

        }

        public void Deactivated(Queue<string> data)
        {
            data.Enqueue(opacity.ToString());
            data.Enqueue(isInvalidated.ToString());
            data.Enqueue(neutralPositionValue.ToString());
        }

        #endregion

        #region Properties

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
                    Save();
                }
            }
        }

        #endregion
    }
}
