using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using ScribbleHunter.Inputs;
using ScribbleHunter.Extensions;
using Microsoft.Phone.Applications.Common;
using System.Collections.Generic;
using ScribbleHunter.Windows.Inputs;
using Microsoft.Xna.Framework.Input;

namespace ScribbleHunter
{
    class DeviceControlManager
    {
        #region Members

        public enum ControlState { TouchAccelerometer, KeyboardController };

        private ControlState controlState = ControlState.KeyboardController;

        private readonly Rectangle cancelSource = new Rectangle(0, 800,
                                                                240, 80);
        private readonly Rectangle cancelDestination = new Rectangle(480, 370,
                                                                     240, 80);

        private readonly Rectangle goSource = new Rectangle(0, 720,
                                                            240, 80);
        private readonly Rectangle goDestination = new Rectangle(80, 370,
                                                                       240, 80);

        private readonly Rectangle switcherSource = new Rectangle(340, 400,
                                                                  100, 100);
        private readonly Rectangle switcherRightDestination = new Rectangle(650, 0,
                                                                            100, 100);

        private readonly Rectangle switcherLeftDestination = new Rectangle(50, 0,
                                                                           100, 100);

        private readonly Rectangle KeyboardControllerTitleSource = new Rectangle(0, 1540,
                                                                        500, 80);
        private readonly Rectangle TouchAccelerometerTitleSource = new Rectangle(0, 1620,
                                                                        500, 80);
        private readonly Vector2 TitlePosition = new Vector2(150.0f, 15.0f);

        private static DeviceControlManager phonePositionManager;

        public static Texture2D Texture;
        public static Texture2D InputTexture;
        public static SpriteFont Font;
        public static SpriteFont SmallFont;

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        private bool cancelClicked = false;
        private bool startClicked = false;

        private readonly string[] TEXT_PHONEPOSITION = { "Hold your device", "to the", "desired position:" };

        public static GameInput GameInput;
        private const string CancelAction = "CancelStart";
        private const string GoAction = "GoGame";
        private const string GoLeftAction = "GoLeftControl";
        private const string GoRightAction = "GoRightControl";
        private const string MouseSelectAction = "SubmitssionSelect";

        private readonly SettingsManager settingsManager;

        private bool canStart;

        private float switchPageTimer = 0.0f;
        private const float SwitchPageMinTimer = 0.25f;

        private const int ImageFameTime = 90;
        private int imageFrameCounter;

        private const int FramesCount = 2;

        private readonly Rectangle[] KeyboardXboxMoveSource = { new Rectangle(0, 0, 200, 100), new Rectangle(0, 100, 200, 100) };
        private readonly Rectangle[] TouchMoveSource = { new Rectangle(200, 0, 100, 100), new Rectangle(200, 100, 100, 100) };
        private readonly Rectangle[] KeyboardXboxCrosshairSource = { new Rectangle(300, 0, 300, 100), new Rectangle(300, 100, 300, 100) };
        private readonly Rectangle[] KeyboardXboxFireSource = { new Rectangle(600, 0, 300, 100), new Rectangle(600, 100, 300, 100) };
        private readonly Rectangle[] TouchFireSource = { new Rectangle(900, 0, 100, 100), new Rectangle(900, 100, 100, 100) };


        private readonly Rectangle KeyboardXboxMoveDestination = new Rectangle(50, 200, 160, 80);
        private readonly Rectangle TouchMoveDestination = new Rectangle(90, 200, 80, 80);
        private readonly Rectangle KeyboardXboxCrosshaireDestination = new Rectangle(280, 200, 240, 80);
        private readonly Rectangle KeyboardXboxFireDestination = new Rectangle(550, 200, 240, 80);
        private readonly Rectangle TouchFireDestination = new Rectangle(630, 200, 80, 80);

        private readonly Rectangle DevicePositionDesination = new Rectangle(350, 195, 100, 100);

        private const string MoveTitle = "Move:";
        private const string FireTitle = "Fire:";
        private const string CrosshairTitle = "Crosshair:";
        private readonly Vector2 MovePosition = new Vector2(100, 170);
        private readonly Vector2 FirePosition = new Vector2(640, 170);
        private readonly Vector2 CrosshairPosition = new Vector2(350, 170);

        private readonly AccelerometerHelper accelerometerHelper = AccelerometerHelper.Instance;
        private readonly string[] NoAccelerometerText = { "No accelerometer", "sensor detected!" };
        private readonly int NoAccelerometerCenterX = 400;
        private readonly int NoAccelerometerY = 290;

        private const string AngleTitle = "Angle";

        #endregion

        #region Constructors

        private DeviceControlManager()
        {
            settingsManager = SettingsManager.GetInstance();

            accelerometerHelper.ReadingChanged += new EventHandler<AccelerometerHelperReadingEventArgs>(OnAccelerometerHelperReadingChanged);
            accelerometerHelper.Active = true;
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            GameInput.AddTouchGestureInput(CancelAction,
                                           GestureType.Tap,
                                           cancelDestination);

            GameInput.AddTouchGestureInput(GoAction,
                                           GestureType.Tap,
                                           goDestination);

            GameInput.AddTouchGestureInput(GoLeftAction,
                                           GestureType.Tap,
                                           switcherLeftDestination);
            GameInput.AddTouchGestureInput(GoRightAction,
                                           GestureType.Tap,
                                           switcherRightDestination);

            GameInput.AddTouchSlideInput(GoLeftAction,
                                         Input.Direction.Right,
                                         50.0f);
            GameInput.AddTouchSlideInput(GoRightAction,
                                         Input.Direction.Left,
                                         50.0f);

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

        public static DeviceControlManager GetInstance()
        {
            if (phonePositionManager == null)
            {
                phonePositionManager = new DeviceControlManager();
            }

            return phonePositionManager;
        }

        private void handleInputs()
        {
            // Cancel
            if (GameInput.IsPressed(CancelAction) || MouseHelper.IsLeftMouseDownDelay(cancelDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, cancelDestination))
            {
                cancelClicked = true;
            }

            // Start
            if (canStart && (GameInput.IsPressed(GoAction) || MouseHelper.IsLeftMouseDownDelay(goDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, goDestination)))
            {
                startClicked = true;
            }
            // Switcher right
            if ((GameInput.IsPressed(GoRightAction) || MouseHelper.IsLeftMouseDownDelay(switcherRightDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, switcherRightDestination)) &&
                switchPageTimer > SwitchPageMinTimer)
            {
                switchPageTimer = 0.0f;

                if (controlState == ControlState.KeyboardController)
                    controlState = ControlState.TouchAccelerometer;
                else
                    controlState = ControlState.KeyboardController;
            }
            // Switcher left
            if ((GameInput.IsPressed(GoLeftAction) || MouseHelper.IsLeftMouseDownDelay(switcherLeftDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, switcherLeftDestination)) &&
                switchPageTimer > SwitchPageMinTimer)
            {
                switchPageTimer = 0.0f;

                if (controlState == ControlState.KeyboardController)
                    controlState = ControlState.TouchAccelerometer;
                else
                    controlState = ControlState.KeyboardController;
            }
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (isActive)
            {
                switchPageTimer += elapsed;
                imageFrameCounter++;

                if (settingsManager.GetNeutralPositionIndex() >= 0 && !accelerometerHelper.NoAccelerometer || controlState == ControlState.KeyboardController)
                {
                    canStart = true;
                }
                else
                {
                    canStart = false;
                }

                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;

                handleInputs();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int frameIndex = (imageFrameCounter / ImageFameTime) % FramesCount;

            spriteBatch.Draw(Texture,
                                 cancelDestination,
                                 cancelSource,
                                 Color.White * opacity);

            Color startColor;

            if (canStart)
            {
                startColor = Color.White * opacity;
            }
            else
            {
                startColor = Color.White * opacity * 0.5f;
            }

            spriteBatch.Draw(Texture,
                                 goDestination,
                                 goSource,
                                 startColor);

            spriteBatch.DrawString(
                    Font,
                    MoveTitle,
                    MovePosition,
                    Color.Black * opacity);

            spriteBatch.DrawString(
                Font,
                FireTitle,
                FirePosition,
                Color.Black * opacity);

            if (controlState == ControlState.KeyboardController)
            {
                spriteBatch.DrawString(
                    Font,
                    CrosshairTitle,
                    CrosshairPosition,
                    Color.Black * opacity);

                spriteBatch.Draw(Texture,
                             TitlePosition,
                             KeyboardControllerTitleSource,
                             Color.White * opacity);

                spriteBatch.Draw(
                    InputTexture,
                    KeyboardXboxMoveDestination,
                    KeyboardXboxMoveSource[frameIndex],
                    Color.White * opacity);

                spriteBatch.Draw(
                    InputTexture,
                    KeyboardXboxCrosshaireDestination,
                    KeyboardXboxCrosshairSource[frameIndex],
                    Color.White * opacity);

                spriteBatch.Draw(
                    InputTexture,
                    KeyboardXboxFireDestination,
                    KeyboardXboxFireSource[frameIndex],
                    Color.White * opacity);
            }
            else
            {
                spriteBatch.Draw(Texture,
                             TitlePosition,
                             TouchAccelerometerTitleSource,
                             Color.White * opacity);

                spriteBatch.Draw(
                    InputTexture,
                    TouchMoveDestination,
                    TouchMoveSource[frameIndex],
                    Color.White * opacity);

                spriteBatch.Draw(
                    InputTexture,
                    TouchFireDestination,
                    TouchFireSource[frameIndex],
                    Color.White * opacity);

                for (int i = 0; i < TEXT_PHONEPOSITION.Length; ++i)
                {
                    spriteBatch.DrawString(SmallFont,
                                        TEXT_PHONEPOSITION[i],
                                        new Vector2(400 - SmallFont.MeasureString(TEXT_PHONEPOSITION[i]).X / 2,
                                                    120 + i * 24),
                                        Color.Black * opacity);
                }

                spriteBatch.Draw(
                    InputTexture,
                    DevicePositionDesination,
                    TouchFireSource[0],
                    Color.White);

                string naturalPositionText;

                if (canStart)
                {
                    naturalPositionText = ((int)(settingsManager.GetNeutralPositionIndex() * 10)).ToString();
                }
                else
                {
                    naturalPositionText = "?";
                }

                if (accelerometerHelper.NoAccelerometer)
                {
                    for (int i = 0; i < NoAccelerometerText.Length; ++i)
                    {
                        spriteBatch.DrawString(SmallFont,
                                            NoAccelerometerText[i],
                                            new Vector2(NoAccelerometerCenterX - SmallFont.MeasureString(NoAccelerometerText[i]).X / 2,
                                                        NoAccelerometerY + i * 24),
                                            Color.Black * opacity);
                    }

                    spriteBatch.DrawString(
                        Font,
                        naturalPositionText,
                        new Vector2((DevicePositionDesination.X + DevicePositionDesination.Width / 2) - Font.MeasureString(naturalPositionText).X / 2,
                                    (DevicePositionDesination.Y + DevicePositionDesination.Height / 2) - 18),
                        Color.Black * opacity);
                }
                else
                {
                    spriteBatch.DrawString(
                        SmallFont,
                        AngleTitle,
                        new Vector2((DevicePositionDesination.X + DevicePositionDesination.Width / 2) - SmallFont.MeasureString(AngleTitle).X / 2,
                                    (DevicePositionDesination.Y + DevicePositionDesination.Height / 2) - 28),
                        Color.Black * opacity);

                    spriteBatch.DrawString(
                        SmallFont,
                        naturalPositionText,
                        new Vector2((DevicePositionDesination.X + DevicePositionDesination.Width / 2) - SmallFont.MeasureString(naturalPositionText).X / 2,
                                    (DevicePositionDesination.Y + DevicePositionDesination.Height / 2) - 5),
                        Color.Black * opacity);
                }
            }

            spriteBatch.Draw(Texture,
                             switcherRightDestination,
                             switcherSource,
                             Color.White * opacity);

            spriteBatch.Draw(Texture,
                             switcherLeftDestination,
                             switcherSource,
                             Color.White * opacity,
                             0.0f,
                             Vector2.Zero,
                             SpriteEffects.FlipHorizontally,
                             0.0f);
        }

        private void OnAccelerometerHelperReadingChanged(object sender, AccelerometerHelperReadingEventArgs e)
        {
            if (isActive)
            {
                Vector3 currentAccValue = new Vector3((float)e.AverageAcceleration.X,
                                          (float)e.AverageAcceleration.Y,
                                          (float)e.AverageAcceleration.Z);

                if (currentAccValue.Z > 0.001f || Math.Abs(currentAccValue.X) > 0.5f)
                {
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Unsupported);
                    return;
                }

                float val = -(float)Math.Asin(currentAccValue.Y);

                if (val >= settingsManager.GetNeutralPositionRadianValue(-10.0f) && val < settingsManager.GetNeutralPositionRadianValue(5.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle0);
                else if (val >= settingsManager.GetNeutralPositionRadianValue(5.0f) && val < settingsManager.GetNeutralPositionRadianValue(15.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle10);
                else if (val >= settingsManager.GetNeutralPositionRadianValue(15.0f) && val < settingsManager.GetNeutralPositionRadianValue(25.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle20);
                else if (val >= settingsManager.GetNeutralPositionRadianValue(25.0f) && val < settingsManager.GetNeutralPositionRadianValue(35.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle30);
                else if (val >= settingsManager.GetNeutralPositionRadianValue(35.0f) && val < settingsManager.GetNeutralPositionRadianValue(45.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle40);
                else if (val >= settingsManager.GetNeutralPositionRadianValue(45.0f) && val < settingsManager.GetNeutralPositionRadianValue(55.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle50);
                else if (val >= settingsManager.GetNeutralPositionRadianValue(55.0f) && val < settingsManager.GetNeutralPositionRadianValue(70.0f))
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Angle60);
                else
                    settingsManager.SetNeutralPosition(SettingsManager.NeutralPositionValues.Unsupported);
            }

        }

        #endregion

        #region Activate/Deactivate

        public void Activated(Queue<string> data)
        {
            this.opacity = Single.Parse(data.Dequeue());
            this.isActive = Boolean.Parse(data.Dequeue());
            this.controlState = (ControlState)Enum.Parse(controlState.GetType(), data.Dequeue(), true);
            this.imageFrameCounter = Int32.Parse(data.Dequeue());
        }

        public void Deactivated(Queue<string> data)
        {
            data.Enqueue(opacity.ToString());
            data.Enqueue(isActive.ToString());
            data.Enqueue(controlState.ToString());
            data.Enqueue(imageFrameCounter.ToString());
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
                    this.startClicked = false;
                    this.cancelClicked = false;
                }
            }
        }

        public bool CancelClicked
        {
            get
            {
                return this.cancelClicked;
            }
        }

        public bool StartClicked
        {
            get
            {
                return this.startClicked;
            }
        }

        public ControlState CurrentControlState
        {
            get
            {
                return this.controlState;
            }
        }

        #endregion
    }
}
