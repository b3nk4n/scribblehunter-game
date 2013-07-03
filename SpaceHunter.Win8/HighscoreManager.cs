using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using ScribbleHunter.Inputs;
using Microsoft.Xna.Framework.Input;
using ScribbleHunter.Windows.Inputs;
using Windows.Storage;

namespace ScribbleHunter
{
    class HighscoreManager
    {
        #region Members

        LeaderboardManager leaderboardManager;

        public enum ScoreState { Local, OnlineAll, OnlineWeek, OnlineMe, OnlineDay, OnlineMonth, MostAddictive };

        private ScoreState scoreState = ScoreState.Local;

        private readonly Rectangle switcherSource = new Rectangle(340, 400, 
                                                                  100, 100);
        private readonly Rectangle switcherRightDestination = new Rectangle(600, 0,
                                                                            100, 100);

        private readonly Rectangle switcherLeftDestination = new Rectangle(100, 0,
                                                                           100, 100);

        private readonly Rectangle browserSource = new Rectangle(340, 500,
                                                                 100, 100);
        private readonly Rectangle browserDestination = new Rectangle(700, 190,
                                                                      100, 100);

        private readonly Rectangle refreshSource = new Rectangle(240, 600,
                                                                 100, 100);
        private readonly Rectangle refreshDestination = new Rectangle(700, 380,
                                                                      100, 100);

        private readonly Rectangle resubmitSource = new Rectangle(340, 600,
                                                                 100, 100);
        private readonly Rectangle resubmitDestination = new Rectangle(700, 380,
                                                                      100, 100);

        private static HighscoreManager highscoreManager;

        private long currentHighScore;

        private List<Highscore> topScores = new List<Highscore>();
        public const int MaxScores = 10;

        public static Texture2D Texture;
        public static SpriteFont Font;
        private readonly Rectangle LocalTitleSource = new Rectangle(0, 1280,
                                                                        240, 80);
        private readonly Rectangle OnlineAllTitleSource = new Rectangle(240, 960,
                                                                        240, 80);
        private readonly Rectangle OnlineMonthTitleSource = new Rectangle(240, 1040,
                                                                        240, 80);
        private readonly Rectangle OnlineWeekTitleSource = new Rectangle(240, 1120,
                                                                        240, 80);
        private readonly Rectangle OnlineDayTitleSource = new Rectangle(240, 1200,
                                                                        240, 80);
        private readonly Rectangle MostAddictiveTitleSource = new Rectangle(240, 1360,
                                                                        240, 80);
        private readonly Rectangle OnlineMeTitleSource = new Rectangle(240, 1280,
                                                                        240, 80);
        private readonly Vector2 TitlePosition = new Vector2(280.0f, 15.0f);

        private string lastName = Highscore.DEFAULT_NAME;

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        private const string BROWSER_URL = "http://bsautermeister.de/scribblehunter/requestscores.php?Method=TOP100WEB";

        private const string TEXT_ME = "Your best personal online achievements:";
        private const string TEXT_RANK = "Best Rank:";
        private const string TEXT_SCORE = "Best Score:";
        private const string TEXT_LEVEL = "Best Level:";
        private const string TEXT_TOTAL_SCORE = "Addiction Score:";
        private const string TEXT_TOTAL_LEVEL = "Addiction Level:";

        public static GameInput GameInput;
        private const string RefreshAction = "Refresh";
        private const string GoLeftAction = "GoLeft";
        private const string GoRightAction = "GoRight";
        private const string BrowserAction = "Browser";
        private const string ResubmitAction = "Resubmit";
        private const string MouseSelectAction = "HighscoresSelect";

        private float switchPageTimer = 0.0f;
        private const float SwitchPageMinTimer = 0.25f;

        private const int RankPositionX = 50;
        private const int NamePositionX = 110;
        private const int ScorePositionX = 570;
        private const int LevelPositionX = 650;

        private const string USERDATA_FILE = "user.txt";

        private const string DOTS3 = ". . . ";
        private const string DOTS6 = ". . . . . . ";
        private const string DOTS12 = ". . . . . . . . . . ";
        private const string DOTS21 = ". . . . . . . . . . . . . . . . . . . ";

        private const int RankPositionStartY = 100;
        private const int RankOffsetY = 33;

        private const string HIGHSCORES_FILE = "highscores.txt";

        #endregion

        #region Constructors

        private HighscoreManager()
        {
            leaderboardManager = LeaderboardManager.GetInstance();

            this.LoadHighScore();

            this.loadUserData();
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            GameInput.AddTouchGestureInput(RefreshAction,
                                          GestureType.Tap,
                                          refreshDestination);
            GameInput.AddTouchGestureInput(GoLeftAction,
                                           GestureType.Tap,
                                           switcherLeftDestination);
            GameInput.AddTouchGestureInput(GoRightAction,
                                           GestureType.Tap,
                                           switcherRightDestination);
            GameInput.AddTouchGestureInput(BrowserAction,
                                           GestureType.Tap,
                                           browserDestination);
            GameInput.AddTouchGestureInput(ResubmitAction,
                                           GestureType.Tap,
                                           resubmitDestination);

            GameInput.AddTouchSlideInput(GoLeftAction,
                                         Input.Direction.Right,
                                         40.0f);
            GameInput.AddTouchSlideInput(GoRightAction,
                                         Input.Direction.Left,
                                         40.0f);

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

        public static HighscoreManager GetInstance()
        {
            if (highscoreManager == null)
            {
                highscoreManager = new HighscoreManager();
            }

            return highscoreManager;
        }

        private async void handleTouchInputs()
        {
            // Switcher right
            if ((GameInput.IsPressed(GoRightAction) || MouseHelper.IsLeftMouseDownDelay(switcherRightDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, switcherRightDestination)) &&
                switchPageTimer > SwitchPageMinTimer)
            {
                switchPageTimer = 0.0f;

                if (scoreState == ScoreState.Local)
                    scoreState = ScoreState.OnlineAll;
                else if (scoreState == ScoreState.OnlineAll)
                    scoreState = ScoreState.OnlineMonth;
                else if (scoreState == ScoreState.OnlineMonth)
                    scoreState = ScoreState.OnlineWeek;
                else if (scoreState == ScoreState.OnlineWeek)
                    scoreState = ScoreState.OnlineDay;
                else if (scoreState == ScoreState.OnlineDay)
                    scoreState = ScoreState.OnlineMe;
                else if (scoreState == ScoreState.OnlineMe)
                    scoreState = ScoreState.MostAddictive;
                else
                    scoreState = ScoreState.Local;

                SoundManager.PlayPaperSound();
            }
            // Switcher left
            if ((GameInput.IsPressed(GoLeftAction) || MouseHelper.IsLeftMouseDownDelay(switcherLeftDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, switcherLeftDestination)) &&
                switchPageTimer > SwitchPageMinTimer)
            {
                switchPageTimer = 0.0f;

                if (scoreState == ScoreState.Local)
                    scoreState = ScoreState.MostAddictive;
                else if (scoreState == ScoreState.MostAddictive)
                    scoreState = ScoreState.OnlineMe;
                else if (scoreState == ScoreState.OnlineMe)
                    scoreState = ScoreState.OnlineDay;
                else if (scoreState == ScoreState.OnlineDay)
                    scoreState = ScoreState.OnlineWeek;
                else if (scoreState == ScoreState.OnlineWeek)
                    scoreState = ScoreState.OnlineMonth;
                else if (scoreState == ScoreState.OnlineMonth)
                    scoreState = ScoreState.OnlineAll;
                else
                    scoreState = ScoreState.Local;

                SoundManager.PlayPaperSound();
            }
            // Resubmit
            if (GameInput.IsPressed(ResubmitAction) || MouseHelper.IsLeftMouseDownDelay(resubmitDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, resubmitDestination))
            {
                if (scoreState == ScoreState.Local && topScores.Count > 0 && topScores[0].Score > 0)
                {
                    leaderboardManager.Submit(LeaderboardManager.RESUBMIT,
                                              topScores[0].Name,
                                              topScores[0].Score,
                                              topScores[0].Level);

                    SoundManager.PlayPaperSound();
                }
            }
            // Browser - Top100
            if (GameInput.IsPressed(BrowserAction) || MouseHelper.IsLeftMouseDownDelay(browserDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, browserDestination))
            {
                if (scoreState != ScoreState.Local)
                {
                    SoundManager.PlayPaperSound();
                    await global::Windows.System.Launcher.LaunchUriAsync(new Uri(BROWSER_URL));
                }
            }
            // Refresh
            if (GameInput.IsPressed(RefreshAction) || MouseHelper.IsLeftMouseDownDelay(refreshDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, refreshDestination))
            {
                if (scoreState != ScoreState.Local)
                {
                    SoundManager.PlayPaperSound();
                    leaderboardManager.Receive();
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (isActive)
            {
                switchPageTimer += elapsed;

                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;
            }

            handleTouchInputs();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
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

            spriteBatch.DrawString(Font,
                                       leaderboardManager.StatusText,
                                       new Vector2(400 - Font.MeasureString(leaderboardManager.StatusText).X / 2,
                                                   440),
                                       Color.Black * opacity);

            if (scoreState == ScoreState.Local)
            {
                spriteBatch.Draw(Texture,
                                 TitlePosition,
                                 LocalTitleSource,
                                 Color.White * opacity);

                if (topScores.Count > 0 && topScores[0].Score > 0)
                {
                    spriteBatch.Draw(Texture,
                                     resubmitDestination,
                                     resubmitSource,
                                     Color.White * opacity);
                }

                for (int i = 0; i < MaxScores; i++)
                {
                    string scoreText;
                    string nameText;
                    string levelText;

                    if (topScores[i].Score > 0)
                    {
                        Highscore h = new Highscore(topScores[i].Name, topScores[i].Score, topScores[i].Level);

                        scoreText = h.ScoreText;
                        nameText = h.Name;
                        levelText = h.LevelText;
                    }
                    else
                    {
                        scoreText = DOTS12;
                        nameText = DOTS21;
                        levelText = DOTS3;
                    }

                    spriteBatch.DrawString(Font,
                           string.Format("{0:d}.", i + 1),
                           new Vector2(RankPositionX, RankPositionStartY + (i * RankOffsetY)),
                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           nameText,
                                           new Vector2(NamePositionX, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           scoreText,
                                           new Vector2(ScorePositionX - Font.MeasureString(scoreText).X, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           levelText,
                                           new Vector2(LevelPositionX - Font.MeasureString(levelText).X, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);
                }
            }

            if (scoreState == ScoreState.OnlineAll)
            {
                spriteBatch.Draw(Texture,
                                 TitlePosition,
                                 OnlineAllTitleSource,
                                 Color.White * opacity);

                spriteBatch.Draw(Texture,
                                 browserDestination,
                                 browserSource,
                                 Color.White * opacity);

                spriteBatch.Draw(Texture,
                                 refreshDestination,
                                 refreshSource,
                                 Color.White * opacity);

                for (int i = 0; i < MaxScores; i++)
                {
                    string scoreText;
                    string nameText;
                    string levelText;

                    if (leaderboardManager.TopScoresAll.Count > i)
                    {
                        Highscore h = new Highscore(leaderboardManager.TopScoresAll[i].Name, leaderboardManager.TopScoresAll[i].Score, leaderboardManager.TopScoresAll[i].Level);

                        scoreText = h.ScoreText;
                        nameText = h.Name;
                        levelText = h.LevelText;
                    }
                    else
                    {
                        scoreText = DOTS12;
                        nameText = DOTS21;
                        levelText = DOTS3;
                    }

                    spriteBatch.DrawString(Font,
                           string.Format("{0:d}.", i + 1),
                           new Vector2(RankPositionX, RankPositionStartY + (i * RankOffsetY)),
                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           nameText,
                                           new Vector2(NamePositionX, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           scoreText,
                                           new Vector2(ScorePositionX - Font.MeasureString(scoreText).X, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           levelText,
                                           new Vector2(LevelPositionX - Font.MeasureString(levelText).X, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);
                }
            }

            if (scoreState == ScoreState.OnlineMonth)
            {
                spriteBatch.Draw(Texture,
                                 TitlePosition,
                                 OnlineMonthTitleSource,
                                 Color.White * opacity);

                spriteBatch.Draw(Texture,
                                 browserDestination,
                                 browserSource,
                                 Color.White * opacity);

                spriteBatch.Draw(Texture,
                                 refreshDestination,
                                 refreshSource,
                                 Color.White * opacity);

                for (int i = 0; i < MaxScores; i++)
                {
                    string scoreText;
                    string nameText;
                    string levelText;

                    if (leaderboardManager.TopScoresMonth.Count > i)
                    {
                        Highscore h = new Highscore(leaderboardManager.TopScoresMonth[i].Name, leaderboardManager.TopScoresMonth[i].Score, leaderboardManager.TopScoresMonth[i].Level);

                        scoreText = h.ScoreText;
                        nameText = h.Name;
                        levelText = h.LevelText;
                    }
                    else
                    {
                        scoreText = DOTS12;
                        nameText = DOTS21;
                        levelText = DOTS3;
                    }

                    spriteBatch.DrawString(Font,
                           string.Format("{0:d}.", i + 1),
                           new Vector2(RankPositionX, RankPositionStartY + (i * RankOffsetY)),
                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           nameText,
                                           new Vector2(NamePositionX, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           scoreText,
                                           new Vector2(ScorePositionX - Font.MeasureString(scoreText).X, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           levelText,
                                           new Vector2(LevelPositionX - Font.MeasureString(levelText).X, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);
                }
            }

            if (scoreState == ScoreState.OnlineWeek)
            {
                spriteBatch.Draw(Texture,
                                 TitlePosition,
                                 OnlineWeekTitleSource,
                                 Color.White * opacity);

                spriteBatch.Draw(Texture,
                                 browserDestination,
                                 browserSource,
                                 Color.White * opacity);

                spriteBatch.Draw(Texture,
                                 refreshDestination,
                                 refreshSource,
                                 Color.White * opacity);

                for (int i = 0; i < MaxScores; i++)
                {
                    string scoreText;
                    string nameText;
                    string levelText;

                    if (leaderboardManager.TopScoresWeek.Count > i)
                    {
                        Highscore h = new Highscore(leaderboardManager.TopScoresWeek[i].Name, leaderboardManager.TopScoresWeek[i].Score, leaderboardManager.TopScoresWeek[i].Level);

                        scoreText = h.ScoreText;
                        nameText = h.Name;
                        levelText = h.LevelText;
                    }
                    else
                    {
                        scoreText = DOTS12;
                        nameText = DOTS21;
                        levelText = DOTS3;
                    }

                    spriteBatch.DrawString(Font,
                           string.Format("{0:d}.", i + 1),
                           new Vector2(RankPositionX, RankPositionStartY + (i * RankOffsetY)),
                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           nameText,
                                           new Vector2(NamePositionX, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           scoreText,
                                           new Vector2(ScorePositionX - Font.MeasureString(scoreText).X, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           levelText,
                                           new Vector2(LevelPositionX - Font.MeasureString(levelText).X, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);
                }
            }

            if (scoreState == ScoreState.OnlineDay)
            {
                spriteBatch.Draw(Texture,
                                 TitlePosition,
                                 OnlineDayTitleSource,
                                 Color.White * opacity);

                spriteBatch.Draw(Texture,
                                 browserDestination,
                                 browserSource,
                                 Color.White * opacity);

                spriteBatch.Draw(Texture,
                                 refreshDestination,
                                 refreshSource,
                                 Color.White * opacity);

                for (int i = 0; i < MaxScores; i++)
                {
                    string scoreText;
                    string nameText;
                    string levelText;

                    if (leaderboardManager.TopScoresDay.Count > i)
                    {
                        Highscore h = new Highscore(leaderboardManager.TopScoresDay[i].Name, leaderboardManager.TopScoresDay[i].Score, leaderboardManager.TopScoresDay[i].Level);

                        scoreText = h.ScoreText;
                        nameText = h.Name;
                        levelText = h.LevelText;
                    }
                    else
                    {
                        scoreText = DOTS12;
                        nameText = DOTS21;
                        levelText = DOTS3;
                    }

                    spriteBatch.DrawString(Font,
                           string.Format("{0:d}.", i + 1),
                           new Vector2(RankPositionX, RankPositionStartY + (i * RankOffsetY)),
                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           nameText,
                                           new Vector2(NamePositionX, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           scoreText,
                                           new Vector2(ScorePositionX - Font.MeasureString(scoreText).X, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           levelText,
                                           new Vector2(LevelPositionX - Font.MeasureString(levelText).X, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);
                }
            }

            if (scoreState == ScoreState.MostAddictive)
            {
                spriteBatch.Draw(Texture,
                                 TitlePosition,
                                 MostAddictiveTitleSource,
                                 Color.White * opacity);

                spriteBatch.Draw(Texture,
                                 browserDestination,
                                 browserSource,
                                 Color.White * opacity);

                spriteBatch.Draw(Texture,
                                 refreshDestination,
                                 refreshSource,
                                 Color.White * opacity);

                for (int i = 0; i < MaxScores; i++)
                {
                    string scoreText;
                    string nameText;
                    string levelText;

                    if (leaderboardManager.TopScoresMostAddictive.Count > i)
                    {
                        Highscore h = new Highscore(leaderboardManager.TopScoresMostAddictive[i].Name, leaderboardManager.TopScoresMostAddictive[i].Score, leaderboardManager.TopScoresMostAddictive[i].Level);

                        scoreText = h.ScoreText;
                        nameText = h.Name;
                        levelText = string.Format("{0:d}",h.Level);
                    }
                    else
                    {
                        scoreText = DOTS12;
                        nameText = DOTS21;
                        levelText = DOTS3;
                    }

                    spriteBatch.DrawString(Font,
                           string.Format("{0:d}.", i + 1),
                           new Vector2(RankPositionX, RankPositionStartY + (i * RankOffsetY)),
                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           nameText,
                                           new Vector2(NamePositionX, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           scoreText,
                                           new Vector2(ScorePositionX - Font.MeasureString(scoreText).X, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);

                    spriteBatch.DrawString(Font,
                                           levelText,
                                           new Vector2(LevelPositionX - Font.MeasureString(levelText).X, RankPositionStartY + (i * RankOffsetY)),
                                           Color.Black * opacity);
                }
            }

            if (scoreState == ScoreState.OnlineMe)
            {
                spriteBatch.Draw(Texture,
                                 TitlePosition,
                                 OnlineMeTitleSource,
                                 Color.White * opacity);

                spriteBatch.Draw(Texture,
                                 browserDestination,
                                 browserSource,
                                 Color.White * opacity);

                spriteBatch.Draw(Texture,
                                 refreshDestination,
                                 refreshSource,
                                 Color.White * opacity);

                spriteBatch.DrawString(Font,
                                   TEXT_ME,
                                   new Vector2(400 - Font.MeasureString(TEXT_ME).X / 2,
                                               140),
                                   Color.Black * opacity);

                // Title:
                spriteBatch.DrawString(Font,
                                       TEXT_RANK,
                                       new Vector2(200,
                                                   210),
                                       Color.Black * opacity);

                spriteBatch.DrawString(Font,
                                       TEXT_SCORE,
                                       new Vector2(200,
                                                   250),
                                       Color.Black * opacity);

                spriteBatch.DrawString(Font,
                                       TEXT_LEVEL,
                                       new Vector2(200,
                                                   290),
                                       Color.Black * opacity);

                spriteBatch.DrawString(Font,
                                       TEXT_TOTAL_SCORE,
                                       new Vector2(200,
                                                   330),
                                       Color.Black * opacity);

                spriteBatch.DrawString(Font,
                                       TEXT_TOTAL_LEVEL,
                                       new Vector2(200,
                                                   370),
                                       Color.Black * opacity);


                // Content:
                int topRank = leaderboardManager.TopRankMe;
                long topScore = leaderboardManager.TopScoreMe;
                int topLevel = leaderboardManager.TopLevelMe;
                long totalScore = leaderboardManager.TotalScoreMe;
                int totalLevel = leaderboardManager.TotalLevelMe;
                string topRankText;
                string topScoreText;
                string topLevelText;
                string totalScoreText;
                string totalLevelText;

                if (topRank == 0)
                    topRankText = DOTS6;
                else
                    topRankText = string.Format("{0:d}", leaderboardManager.TopRankMe);

                if (topScore == 0)
                    topScoreText = DOTS12;
                else
                    topScoreText = string.Format("{0:d}", leaderboardManager.TopScoreMe);

                if (topLevel == 0)
                    topLevelText = DOTS3;
                else
                    topLevelText = string.Format("{0:d}", leaderboardManager.TopLevelMe);

                if (totalScore == 0)
                    totalScoreText = DOTS12;
                else
                    totalScoreText = string.Format("{0:d}", leaderboardManager.TotalScoreMe);

                if (totalLevel == 0)
                    totalLevelText = DOTS3;
                else
                    totalLevelText = string.Format("{0:d}", leaderboardManager.TotalLevelMe);

                spriteBatch.DrawString(Font,
                                       topRankText,
                                       new Vector2(600 - Font.MeasureString(topRankText).X,
                                                   210),
                                       Color.Black * opacity);

                spriteBatch.DrawString(Font,
                                       topScoreText,
                                       new Vector2(600 - Font.MeasureString(topScoreText).X,
                                                   250),
                                       Color.Black * opacity);

                spriteBatch.DrawString(Font,
                                       topLevelText,
                                       new Vector2(600 - Font.MeasureString(topLevelText).X,
                                                   290),
                                       Color.Black * opacity);

                spriteBatch.DrawString(Font,
                                       totalScoreText,
                                       new Vector2(600 - Font.MeasureString(totalScoreText).X,
                                                   330),
                                       Color.Black * opacity);

                spriteBatch.DrawString(Font,
                                       totalLevelText,
                                       new Vector2(600 - Font.MeasureString(totalLevelText).X,
                                                   370),
                                       Color.Black * opacity);
            }
        }


        /// <summary>
        /// Saves the current highscore to a text file.
        /// </summary>
        public async void SaveHighScore(string name, long score, int level)
        {
            this.lastName = name;

            if (this.IsInScoreboard(score))
            {
                Highscore newScore = new Highscore(name, score, level);

                topScores.Add(newScore);
                this.sortScoreList();
                this.trimScoreList();

                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.CreateFileAsync(HIGHSCORES_FILE, CreationCollisionOption.OpenIfExists);

                IList<string> scoresToWrite = new List<string>();

                for (int i = 0; i < MaxScores; i++)
                {
                    scoresToWrite.Add(topScores[i].ToString());
                }

                await FileIO.WriteLinesAsync(file, scoresToWrite);

                this.currentHighScore = maxScore();
            }

            this.saveUserData();
        }

        /// <summary>
        /// Loads the high score from a text file.
        /// </summary>
        private async void LoadHighScore()
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(HIGHSCORES_FILE, CreationCollisionOption.OpenIfExists);

            IList<string> readStrings = await FileIO.ReadLinesAsync(file);

            if (readStrings.Count > 0)
            {
                for (int i = 0; i < MaxScores; i++)
                {
                    if (i < readStrings.Count)
                    {
                        topScores.Add(new Highscore(readStrings[i]));
                    }
                }

                this.sortScoreList();
                this.currentHighScore = this.maxScore();
            }
            else
            {
                IList<string> scoresToWrite = new List<string>();

                for (int i = 0; i < MaxScores; i++)
                {
                    Highscore newScore = new Highscore();
                    topScores.Add(newScore);
                    scoresToWrite.Add(newScore.ToString());
                }

                await FileIO.WriteLinesAsync(file, scoresToWrite);
                this.currentHighScore = 0;
            }
        }

        private void sortScoreList()
        {
            for (int i = 0; i < topScores.Count; i++)
            {
                for (int j = 0; j < topScores.Count - 1; j++)
                {
                    if (topScores[j].Score < topScores[j + 1].Score)
                    {
                        Highscore tmp = topScores[j];
                        topScores[j] = topScores[j + 1];
                        topScores[j + 1] = tmp;
                    }
                }
            }
        }

        private void trimScoreList()
        {
            while (topScores.Count > MaxScores)
            {
                topScores.RemoveAt(topScores.Count - 1);
            }
        }

        private long maxScore()
        {
            long max = 0;

            for (int i = 0; i < topScores.Count; i++)
            {
                max = Math.Max(max, topScores[i].Score);
            }

            return max;
        }

        /// <summary>
        /// Checks wheather the score reaches top 10.
        /// </summary>
        /// <param name="score">The score to check</param>
        /// <returns>True if the player is under the top 1.</returns>
        public bool IsInScoreboard(long score)
        {
            return score > topScores[MaxScores - 1].Score;
        }

        /// <summary>
        /// Calculates the rank of the new score.
        /// </summary>
        /// <param name="score">The new score</param>
        /// <returns>Returns the calculated rank (-1, if the score is not top 10).</returns>
        public int GetRank(long score)
        {
            if (topScores.Count < 0)
                return 1;

            for (int i = 0; i < topScores.Count; i++)
            {
                if (topScores[i].Score < score)
                    return i + 1;
            }

            return -1;
        }

        public void Save()
        {
            this.saveUserData();
        }

        private async void saveUserData()
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(USERDATA_FILE, CreationCollisionOption.OpenIfExists);
            IList<string> settingsToWrite = new List<string>();

            settingsToWrite.Add(this.LastName);

            await FileIO.WriteLinesAsync(file, settingsToWrite);
        }

        private async void loadUserData()
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(USERDATA_FILE, CreationCollisionOption.OpenIfExists);
            IList<string> readStrings = await FileIO.ReadLinesAsync(file);

            if (readStrings.Count > 0)
            {
                this.lastName = readStrings[0];
            }
            else
            {
                IList<string> settingsToWrite = new List<string>();

                settingsToWrite.Add(this.LastName);

                await FileIO.WriteLinesAsync(file, settingsToWrite);
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(Queue<string> data)
        {
            this.currentHighScore = Int64.Parse(data.Dequeue());
            this.lastName = data.Dequeue();
            this.opacity = Single.Parse(data.Dequeue());
            this.isActive = Boolean.Parse(data.Dequeue());
            this.scoreState = (ScoreState)Enum.Parse(scoreState.GetType(), data.Dequeue(), false);
        }

        public void Deactivated(Queue<string> data)
        {
            data.Enqueue(currentHighScore.ToString());
            data.Enqueue(lastName);
            data.Enqueue(opacity.ToString());
            data.Enqueue(isActive.ToString());
            data.Enqueue(scoreState.ToString());
        }

        #endregion

        #region Properties

        public long CurrentHighscore
        {
            get
            {
                return this.currentHighScore;
            }
        }

        public string LastName
        {
            set
            {
                this.lastName = value;
            }
            get
            {
                return this.lastName;
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
