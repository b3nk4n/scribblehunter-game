using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using ScribbleHunter.Inputs;
using ScribbleHunter.Extensions;
using Microsoft.Xna.Framework.Input;
using ScribbleHunter.Windows.Inputs;
using System.Collections.Generic;

namespace ScribbleHunter
{
    class SubmissionManager
    {
        #region Members

        LeaderboardManager leaderboardManager;

        private readonly Rectangle submitSource = new Rectangle(0, 1120,
                                                                240, 80);
        private readonly Rectangle submitDestination = new Rectangle(80, 370,
                                                                       240, 80);

        private readonly Rectangle cancelSource = new Rectangle(0, 800,
                                                                240, 80);
        private readonly Rectangle cancelDestination = new Rectangle(480, 370,
                                                                     240, 80);

        private readonly Rectangle retrySource = new Rectangle(0, 1040,
                                                                  240, 80);
        private readonly Rectangle retryDestination = new Rectangle(80, 370,
                                                                       240, 80);

        private static SubmissionManager submissionManager;

        public const int MaxScores = 10;

        public static Texture2D Texture;
        public static Texture2D KeyboardTexture;
        public static SpriteFont Font;
        private readonly Rectangle TitleSource = new Rectangle(0, 300,
                                                               480, 100);
        private readonly Vector2 TitlePosition = new Vector2(160.0f, 15.0f);

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        private string name = string.Empty;
        private long score;
        private int level;

        private bool cancelClicked = false;
        private bool retryClicked = false;


        private const string TEXT_SUBMIT = "You have now the ability to submit your score!";
        private const string TEXT_NAME = "Name:";
        private const string TEXT_SCORE = "Score:";
        private const string TEXT_LEVEL = "Level:";

        private enum SubmitState { Submit, Submitted };
        private SubmitState submitState = SubmitState.Submit;

        public static GameInput GameInput;
        private const string SubmitAction = "Submit";
        private const string CancelAction = "Cancel";
        private const string RetryAction = "Retry";
        private const string ChangeNameAction = "ChangeName";
        private const string MouseSelectAction = "SubmitssionSelect";

        private bool isInputActive = false;

        private readonly Rectangle formLeftSource = new Rectangle(480, 1350,
                                                                  20, 60);
        private readonly Rectangle formContentSource = new Rectangle(500, 1350,
                                                                     160, 60);
        private readonly Rectangle formRightSource = new Rectangle(670, 1350,
                                                                   80, 60);
        private readonly Rectangle formClickDestination = new Rectangle(220, 215, 360, 60);

        private readonly Rectangle textCursorSource = new Rectangle(480, 1360,
                                                                   20, 40);
        private int textCursorBlinkCounter;

        // Counter for the text form to ensure that the form is not selected and unselected in the next frame
        private int formActiveCounter;

        private readonly Rectangle[] KeyboardNumbersSource = {
                                                                 new Rectangle(360, 0, 40, 40),
                                                                 new Rectangle(0, 0, 40, 40),
                                                                 new Rectangle(40, 0, 40, 40),
                                                                 new Rectangle(80, 0, 40, 40),
                                                                 new Rectangle(120, 0, 40, 40),
                                                                 new Rectangle(160, 0, 40, 40),
                                                                 new Rectangle(200, 0, 40, 40),
                                                                 new Rectangle(240, 0, 40, 40),
                                                                 new Rectangle(280, 0, 40, 40),
                                                                 new Rectangle(320, 0, 40, 40)
                                                             };
        private readonly Rectangle[] KeyboardRow1Source = {
                                                                 new Rectangle(0, 40, 40, 40),
                                                                 new Rectangle(40, 40, 40, 40),
                                                                 new Rectangle(80, 40, 40, 40),
                                                                 new Rectangle(120, 40, 40, 40),
                                                                 new Rectangle(160, 40, 40, 40),
                                                                 new Rectangle(200, 40, 40, 40),
                                                                 new Rectangle(240, 40, 40, 40),
                                                                 new Rectangle(280, 40, 40, 40),
                                                                 new Rectangle(320, 40, 40, 40),
                                                                 new Rectangle(360, 40, 40, 40)
                                                             };
        private readonly Rectangle[] KeyboardRow2Source = {
                                                                 new Rectangle(0, 80, 40, 40),
                                                                 new Rectangle(40, 80, 40, 40),
                                                                 new Rectangle(80, 80, 40, 40),
                                                                 new Rectangle(120, 80, 40, 40),
                                                                 new Rectangle(160, 80, 40, 40),
                                                                 new Rectangle(200, 80, 40, 40),
                                                                 new Rectangle(240, 80, 40, 40),
                                                                 new Rectangle(280, 80, 40, 40),
                                                                 new Rectangle(320, 80, 40, 40)
                                                             };

        private readonly Rectangle[] KeyboardRow3Source = {
                                                                 new Rectangle(0, 120, 40, 40),
                                                                 new Rectangle(40, 120, 40, 40),
                                                                 new Rectangle(80, 120, 40, 40),
                                                                 new Rectangle(120, 120, 40, 40),
                                                                 new Rectangle(160, 120, 40, 40),
                                                                 new Rectangle(200, 120, 40, 40),
                                                                 new Rectangle(240, 120, 40, 40)
                                                             };

        private readonly Rectangle[] KeyboardNumbersDestination = {
                                                                 new Rectangle(605, 285, 40, 40),
                                                                 new Rectangle(155, 285, 40, 40),
                                                                 new Rectangle(205, 285, 40, 40),
                                                                 new Rectangle(255, 285, 40, 40),
                                                                 new Rectangle(305, 285, 40, 40),
                                                                 new Rectangle(355, 285, 40, 40),
                                                                 new Rectangle(405, 285, 40, 40),
                                                                 new Rectangle(455, 285, 40, 40),
                                                                 new Rectangle(505, 285, 40, 40),
                                                                 new Rectangle(555, 285, 40, 40)
                                                             };
        private readonly Rectangle[] KeyboardRow1Destination = {
                                                                 new Rectangle(155, 335, 40, 40),
                                                                 new Rectangle(205, 335, 40, 40),
                                                                 new Rectangle(255, 335, 40, 40),
                                                                 new Rectangle(305, 335, 40, 40),
                                                                 new Rectangle(355, 335, 40, 40),
                                                                 new Rectangle(405, 335, 40, 40),
                                                                 new Rectangle(455, 335, 40, 40),
                                                                 new Rectangle(505, 335, 40, 40),
                                                                 new Rectangle(555, 335, 40, 40),
                                                                 new Rectangle(605, 335, 40, 40)
                                                             };
        private readonly Rectangle[] KeyboardRow2Destination = {
                                                                 new Rectangle(155, 385, 40, 40),
                                                                 new Rectangle(205, 385, 40, 40),
                                                                 new Rectangle(255, 385, 40, 40),
                                                                 new Rectangle(305, 385, 40, 40),
                                                                 new Rectangle(355, 385, 40, 40),
                                                                 new Rectangle(405, 385, 40, 40),
                                                                 new Rectangle(455, 385, 40, 40),
                                                                 new Rectangle(505, 385, 40, 40),
                                                                 new Rectangle(555, 385, 40, 40)
                                                             };

        private readonly Rectangle[] KeyboardRow3Destination = {
                                                                 new Rectangle(155, 435, 40, 40),
                                                                 new Rectangle(205, 435, 40, 40),
                                                                 new Rectangle(255, 435, 40, 40),
                                                                 new Rectangle(305, 435, 40, 40),
                                                                 new Rectangle(355, 435, 40, 40),
                                                                 new Rectangle(405, 435, 40, 40),
                                                                 new Rectangle(455, 435, 40, 40)
                                                             };

        private readonly Rectangle KeyboardDelSource = new Rectangle(280, 120, 80, 40);
        private readonly Rectangle KeyboardDelDestination = new Rectangle(505, 435, 90, 40);

        private readonly Rectangle KeyboardEnterSource = new Rectangle(360, 80, 40, 80);
        private readonly Rectangle KeyboardEnterDestination = new Rectangle(605, 385, 40, 90);

        private readonly string[] KeyNumberAction = { "Nr0", "Nr1", "Nr2", "Nr3", "Nr4", "Nr5", "Nr6", "Nr7", "Nr8", "Nr9" };
        private readonly string[] KeyRow1Action = { "KeyQ", "KeyW", "KeyE", "KeyR", "KeyT", "KeyY", "KeyU", "KeyI", "KeyO", "KeyP" };
        private readonly string[] KeyRow2Action = { "KeyA", "KeyS", "KeyD", "KeyF", "KeyG", "KeyH", "KeyJ", "KeyK", "KeyL" };
        private readonly string[] KeyRow3Action = { "KeyZ", "KeyX", "KeyC", "KeyV", "KeyB", "KeyN", "KeyM" };
        private const string KeyEnterAction = "KeyEnter";
        private const string KeyDelAction = "KeyDel";

        #endregion

        #region Constructors

        private SubmissionManager()
        {
            leaderboardManager = LeaderboardManager.GetInstance();
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            GameInput.AddTouchGestureInput(SubmitAction,
                                           GestureType.Tap,
                                           submitDestination);
            GameInput.AddTouchGestureInput(CancelAction,
                                           GestureType.Tap,
                                           cancelDestination);
            GameInput.AddTouchGestureInput(RetryAction,
                                           GestureType.Tap,
                                           retryDestination);
            GameInput.AddTouchGestureInput(ChangeNameAction,
                                           GestureType.Tap,
                                           formClickDestination);

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

            // Virtual Keyboard Actions:
            for (int i = 0; i < KeyNumberAction.Length; ++i)
            {
                GameInput.AddTouchGestureInput(KeyNumberAction[i],
                                               GestureType.Tap,
                                               KeyboardNumbersDestination[i]);
            }

            for (int i = 0; i < KeyRow1Action.Length; ++i)
            {
                GameInput.AddTouchGestureInput(KeyRow1Action[i],
                                               GestureType.Tap,
                                               KeyboardRow1Destination[i]);
            }

            for (int i = 0; i < KeyRow2Action.Length; ++i)
            {
                GameInput.AddTouchGestureInput(KeyRow2Action[i],
                                               GestureType.Tap,
                                               KeyboardRow2Destination[i]);
            }

            for (int i = 0; i < KeyRow3Action.Length; ++i)
            {
                GameInput.AddTouchGestureInput(KeyRow3Action[i],
                                               GestureType.Tap,
                                               KeyboardRow3Destination[i]);
            }

            GameInput.AddTouchGestureInput(KeyEnterAction,
                                            GestureType.Tap,
                                            KeyboardEnterDestination);
            GameInput.AddTouchGestureInput(KeyDelAction,
                                            GestureType.Tap,
                                            KeyboardDelDestination);
        }

        public static SubmissionManager GetInstance()
        {
            if (submissionManager == null)
            {
                submissionManager = new SubmissionManager();
            }

            return submissionManager;
        }

        private void handleTouchInputs()
        {
            if (!isInputActive && formActiveCounter > 10 && (GameInput.IsPressed(CancelAction) || MouseHelper.IsLeftMouseDownDelay(cancelDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, cancelDestination)))
            {
                leaderboardManager.StatusText = LeaderboardManager.TEXT_NONE;
                cancelClicked = true;
            }

            if (submitState == SubmitState.Submit)
            {
                // Submit
                if (!isInputActive && (this.name.Length > 0 && (GameInput.IsPressed(SubmitAction) || MouseHelper.IsLeftMouseDownDelay(submitDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, submitDestination))))
                {
                    leaderboardManager.Submit(LeaderboardManager.SUBMIT,
                                              name,
                                              score,
                                              level);
                    submitState = SubmitState.Submitted;
                }

                formActiveCounter++;

                // keyboard name input
                if (isInputActive)
                {
                    Keys key = KeyboardHelper.IsAnyTextOrAcceptKeyDownDelay();

                    if (key == Keys.None)
                    {
                        if (GameInput.IsPressed(KeyNumberAction[0]) || MouseHelper.IsLeftMouseDownDelay(KeyboardNumbersDestination[0]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardNumbersDestination[0]))
                        {
                            key = Keys.D0;
                        }
                        else if (GameInput.IsPressed(KeyNumberAction[1]) || MouseHelper.IsLeftMouseDownDelay(KeyboardNumbersDestination[1]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardNumbersDestination[1]))
                        {
                            key = Keys.D1;
                        }
                        else if (GameInput.IsPressed(KeyNumberAction[2]) || MouseHelper.IsLeftMouseDownDelay(KeyboardNumbersDestination[2]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardNumbersDestination[2]))
                        {
                            key = Keys.D2;
                        }
                        else if (GameInput.IsPressed(KeyNumberAction[3]) || MouseHelper.IsLeftMouseDownDelay(KeyboardNumbersDestination[3]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardNumbersDestination[3]))
                        {
                            key = Keys.D3;
                        }
                        else if (GameInput.IsPressed(KeyNumberAction[4]) || MouseHelper.IsLeftMouseDownDelay(KeyboardNumbersDestination[4]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardNumbersDestination[4]))
                        {
                            key = Keys.D4;
                        }
                        else if (GameInput.IsPressed(KeyNumberAction[5]) || MouseHelper.IsLeftMouseDownDelay(KeyboardNumbersDestination[5]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardNumbersDestination[5]))
                        {
                            key = Keys.D5;
                        }
                        else if (GameInput.IsPressed(KeyNumberAction[6]) || MouseHelper.IsLeftMouseDownDelay(KeyboardNumbersDestination[6]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardNumbersDestination[6]))
                        {
                            key = Keys.D6;
                        }
                        else if (GameInput.IsPressed(KeyNumberAction[7]) || MouseHelper.IsLeftMouseDownDelay(KeyboardNumbersDestination[7]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardNumbersDestination[7]))
                        {
                            key = Keys.D7;
                        }
                        else if (GameInput.IsPressed(KeyNumberAction[8]) || MouseHelper.IsLeftMouseDownDelay(KeyboardNumbersDestination[8]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardNumbersDestination[8]))
                        {
                            key = Keys.D8;
                        }
                        else if (GameInput.IsPressed(KeyNumberAction[9]) || MouseHelper.IsLeftMouseDownDelay(KeyboardNumbersDestination[9]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardNumbersDestination[9]))
                        {
                            key = Keys.D9;
                        }
                        else if (GameInput.IsPressed(KeyRow1Action[0]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow1Destination[0]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow1Destination[0]))
                        {
                            key = Keys.Q;
                        }
                        else if (GameInput.IsPressed(KeyRow1Action[1]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow1Destination[1]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow1Destination[1]))
                        {
                            key = Keys.W;
                        }
                        else if (GameInput.IsPressed(KeyRow1Action[2]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow1Destination[2]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow1Destination[2]))
                        {
                            key = Keys.E;
                        }
                        else if (GameInput.IsPressed(KeyRow1Action[3]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow1Destination[3]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow1Destination[3]))
                        {
                            key = Keys.R;
                        }
                        else if (GameInput.IsPressed(KeyRow1Action[4]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow1Destination[4]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow1Destination[4]))
                        {
                            key = Keys.T;
                        }
                        else if (GameInput.IsPressed(KeyRow1Action[5]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow1Destination[5]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow1Destination[5]))
                        {
                            key = Keys.Y;
                        }
                        else if (GameInput.IsPressed(KeyRow1Action[6]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow1Destination[6]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow1Destination[6]))
                        {
                            key = Keys.U;
                        }
                        else if (GameInput.IsPressed(KeyRow1Action[7]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow1Destination[7]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow1Destination[7]))
                        {
                            key = Keys.I;
                        }
                        else if (GameInput.IsPressed(KeyRow1Action[8]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow1Destination[8]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow1Destination[8]))
                        {
                            key = Keys.O;
                        }
                        else if (GameInput.IsPressed(KeyRow1Action[9]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow1Destination[9]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow1Destination[9]))
                        {
                            key = Keys.P;
                        }
                        else if (GameInput.IsPressed(KeyRow2Action[0]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow2Destination[0]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow2Destination[0]))
                        {
                            key = Keys.A;
                        }
                        else if (GameInput.IsPressed(KeyRow2Action[1]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow2Destination[1]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow2Destination[1]))
                        {
                            key = Keys.S;
                        }
                        else if (GameInput.IsPressed(KeyRow2Action[2]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow2Destination[2]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow2Destination[2]))
                        {
                            key = Keys.D;
                        }
                        else if (GameInput.IsPressed(KeyRow2Action[3]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow2Destination[3]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow2Destination[3]))
                        {
                            key = Keys.F;
                        }
                        else if (GameInput.IsPressed(KeyRow2Action[4]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow2Destination[4]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow2Destination[4]))
                        {
                            key = Keys.G;
                        }
                        else if (GameInput.IsPressed(KeyRow2Action[5]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow2Destination[5]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow2Destination[5]))
                        {
                            key = Keys.H;
                        }
                        else if (GameInput.IsPressed(KeyRow2Action[6]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow2Destination[6]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow2Destination[6]))
                        {
                            key = Keys.J;
                        }
                        else if (GameInput.IsPressed(KeyRow2Action[7]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow2Destination[7]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow2Destination[7]))
                        {
                            key = Keys.K;
                        }
                        else if (GameInput.IsPressed(KeyRow2Action[8]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow2Destination[8]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow2Destination[8]))
                        {
                            key = Keys.L;
                        }
                        else if (GameInput.IsPressed(KeyRow3Action[0]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow3Destination[0]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow3Destination[0]))
                        {
                            key = Keys.Z;
                        }
                        else if (GameInput.IsPressed(KeyRow3Action[1]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow3Destination[1]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow3Destination[1]))
                        {
                            key = Keys.X;
                        }
                        else if (GameInput.IsPressed(KeyRow3Action[2]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow3Destination[2]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow3Destination[2]))
                        {
                            key = Keys.C;
                        }
                        else if (GameInput.IsPressed(KeyRow3Action[3]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow3Destination[3]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow3Destination[3]))
                        {
                            key = Keys.V;
                        }
                        else if (GameInput.IsPressed(KeyRow3Action[4]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow3Destination[4]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow3Destination[4]))
                        {
                            key = Keys.B;
                        }
                        else if (GameInput.IsPressed(KeyRow3Action[5]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow3Destination[5]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow3Destination[5]))
                        {
                            key = Keys.N;
                        }
                        else if (GameInput.IsPressed(KeyRow3Action[6]) || MouseHelper.IsLeftMouseDownDelay(KeyboardRow3Destination[6]) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardRow3Destination[6]))
                        {
                            key = Keys.M;
                        }
                        else if (GameInput.IsPressed(KeyEnterAction) || MouseHelper.IsLeftMouseDownDelay(KeyboardEnterDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardEnterDestination))
                        {
                            key = Keys.Enter;
                        }
                        else if (GameInput.IsPressed(KeyDelAction) || MouseHelper.IsLeftMouseDownDelay(KeyboardDelDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, KeyboardDelDestination))
                        {
                            key = Keys.Back;
                        }
                    }

                    if (key != Keys.None)
                    {
                        if (formActiveCounter > 10 && KeyboardHelper.IsAcceptKey(key))
                        {
                            unfocusInputForm();
                        }
                        else
                        {
                            if (KeyboardHelper.IsDeleteKey(key) && name.Length > 0)
                            {
                                this.name = name.Substring(0, name.Length - 1);
                            }
                            else
                            {
                                if (this.name.Length < Highscore.MaxNameLength && key != Keys.Back && key != Keys.Enter && key != Keys.Escape)
                                {
                                    string keyString = key.ToString();

                                    if (keyString.Length == 1)
                                        this.name += keyString;
                                    else if (keyString.Length > 1)
                                        this.name += keyString[1];
                                }
                            }
                        }
                    }

                    if (formActiveCounter > 10 && (GameInput.IsPressed(ChangeNameAction) || MouseHelper.IsLeftMouseDownDelay(formClickDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, formClickDestination)))
                    {
                        unfocusInputForm();
                    }
                }
                else if (GameInput.IsPressed(ChangeNameAction) || MouseHelper.IsLeftMouseDownDelay(formClickDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, formClickDestination))
                {
                    isInputActive = true;
                    formActiveCounter = 0;

                    this.name = string.Empty;
                }
            }
            else
            {
                // Retry
                if (GameInput.IsPressed(RetryAction) || MouseHelper.IsLeftMouseDownDelay(retryDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, retryDestination))
                {
                    if (submitState == SubmitState.Submitted)
                    {
                        retryClicked = true;
                    }
                }
            }
        }

        private void unfocusInputForm()
        {
            isInputActive = false;

            if (name.Length == 0)
            {
                name = Highscore.DEFAULT_NAME;
            }
        }

        public void SetUp(string name, long score, int level)
        {
            this.name = name;
            this.score = score;
            this.level = level;
        }

        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;

                textCursorBlinkCounter++;
            }

            handleTouchInputs();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isInputActive)
            {
                spriteBatch.Draw(Texture,
                                 cancelDestination,
                                 cancelSource,
                                 Color.White * opacity);
            }


            if (submitState == SubmitState.Submit)
            {


                if (!isInputActive)
                {
                    float factor = 1.0f;

                    if (this.name.Length == 0)
                    {
                        factor = 0.5f;
                    }

                    spriteBatch.Draw(Texture,
                                     submitDestination,
                                     submitSource,
                                     Color.White * opacity * factor);
                }
            }
            else if (submitState == SubmitState.Submitted)
            {
                spriteBatch.Draw(Texture,
                                    retryDestination,
                                    retrySource,
                                    Color.White * opacity);

                spriteBatch.DrawString(Font,
                                   leaderboardManager.StatusText,
                                   new Vector2(800 / 2 - Font.MeasureString(leaderboardManager.StatusText).X / 2,
                                               450),
                                   Color.Black * opacity);

            }

            spriteBatch.DrawString(Font,
                                   TEXT_SUBMIT,
                                   new Vector2(800 / 2 - Font.MeasureString(TEXT_SUBMIT).X / 2,
                                               130),
                                   Color.Black * opacity);

            spriteBatch.DrawString(Font,
                                   TEXT_NAME,
                                   new Vector2(370,
                                               180),
                                   Color.Black * opacity);

            // Form:
            int realNameWidth = (int)Font.MeasureString(name).X;
            int nameWidth = Math.Max(realNameWidth, 100);

            Color formColor;

            if (submitState == SubmitState.Submit)
            {
                formColor = Color.White;
            }
            else
            {
                formColor = Color.White * 0.5f;
            }

            spriteBatch.Draw(
                Texture,
                new Rectangle(400 - nameWidth / 2 - formLeftSource.Width - 10, formClickDestination.Y,
                              formLeftSource.Width,
                              formLeftSource.Height),
                formLeftSource,
                formColor);

            spriteBatch.Draw(
                Texture,
                new Rectangle(400 - nameWidth / 2 - 10, formClickDestination.Y,
                              nameWidth + 10,
                              formContentSource.Height),
                formContentSource,
                formColor);

            spriteBatch.Draw(
                Texture,
                new Rectangle(400 + nameWidth / 2, formClickDestination.Y,
                              formRightSource.Width,
                              formRightSource.Height),
                formRightSource,
                formColor);


            spriteBatch.DrawString(Font,
                                   name,
                                   new Vector2(400 - nameWidth / 2 - 10,
                                               232),
                                   Color.Black * opacity);

            if (submitState == SubmitState.Submit && isInputActive)
            {
                DrawKeyboard(spriteBatch);

                if ((textCursorBlinkCounter / 30) % 2 == 0)
                {
                    spriteBatch.Draw(
                    Texture,
                    new Vector2(400 - nameWidth / 2 - 10 + realNameWidth,
                                225),
                    textCursorSource,
                    Color.White);
                }
            }

            if (submitState == SubmitState.Submit && !isInputActive || submitState == SubmitState.Submitted)
            {
                spriteBatch.DrawString(Font,
                                       TEXT_SCORE,
                                       new Vector2(350,
                                                   285),
                                       Color.Black * opacity);

                spriteBatch.DrawString(Font,
                                       TEXT_LEVEL,
                                       new Vector2(350,
                                                   325),
                                       Color.Black * opacity);

                String scoreString = score.ToString();

                spriteBatch.DrawString(Font,
                                      scoreString,
                                      new Vector2(450,
                                                  285),
                                      Color.Black * opacity);

                String levelString = level.ToString();

                spriteBatch.DrawString(Font,
                                      levelString,
                                      new Vector2(450,
                                                  325),
                                      Color.Black * opacity);
            }

            spriteBatch.Draw(Texture,
                             TitlePosition,
                             TitleSource,
                             Color.White * opacity);
        }

        private void DrawKeyboard(SpriteBatch batch)
        {
            for (int i = 0; i < KeyboardNumbersSource.Length; ++i)
            {
                batch.Draw(KeyboardTexture, KeyboardNumbersDestination[i], KeyboardNumbersSource[i], Color.White * 0.9f);
            }

            for (int i = 0; i < KeyboardRow1Source.Length; ++i)
            {
                batch.Draw(KeyboardTexture, KeyboardRow1Destination[i], KeyboardRow1Source[i], Color.White * 0.9f);
            }

            for (int i = 0; i < KeyboardRow2Source.Length; ++i)
            {
                batch.Draw(KeyboardTexture, KeyboardRow2Destination[i], KeyboardRow2Source[i], Color.White * 0.9f);
            }

            for (int i = 0; i < KeyboardRow3Source.Length; ++i)
            {
                batch.Draw(KeyboardTexture, KeyboardRow3Destination[i], KeyboardRow3Source[i], Color.White * 0.9f);
            }

            batch.Draw(KeyboardTexture, KeyboardEnterDestination, KeyboardEnterSource, Color.White * 0.9f);
            batch.Draw(KeyboardTexture, KeyboardDelDestination, KeyboardDelSource, Color.White * 0.9f);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(Queue<string> data)
        {
            this.opacity = Single.Parse(data.Dequeue());
            this.isActive = Boolean.Parse(data.Dequeue());
            this.name = data.Dequeue();
            this.score = Int64.Parse(data.Dequeue());
            this.level = Int32.Parse(data.Dequeue());
            this.submitState = (SubmitState)Enum.Parse(submitState.GetType(), data.Dequeue(), false);
        }

        public void Deactivated(Queue<string> data)
        {
            data.Enqueue(opacity.ToString());
            data.Enqueue(isActive.ToString());
            data.Enqueue(name);
            data.Enqueue(score.ToString());
            data.Enqueue(level.ToString());
            data.Enqueue(submitState.ToString());
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
                    this.isInputActive = false;
                    this.retryClicked = false;
                    this.cancelClicked = false;
                    this.submitState = SubmitState.Submit;
                }
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public bool CancelClicked
        {
            get
            {
                return this.cancelClicked;
            }
        }

        public bool RetryClicked
        {
            get
            {
                return this.retryClicked;
            }
        }

        #endregion
    }
}
