using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScribbleHunter.Inputs;
using Microsoft.Xna.Framework.Input.Touch;
using ScribbleHunter.Extensions;
using Microsoft.Xna.Framework.Input;
using ScribbleHunter.Windows.Inputs;

namespace ScribbleHunter
{
    class HelpManager
    {
        #region Members

        private Texture2D texture;
        private SpriteFont font;
        private readonly Rectangle HelpTitleSource = new Rectangle(0, 1200,
                                                                   240, 80);
        private readonly Vector2 TitlePosition = new Vector2(280.0f, 15.0f);

        private static readonly string[] Content = {"If you have any further questions,",
                                            "ideas or problems with ScribbleHunter,",
                                            "please do not hesitate to contact us."};

        private const string Email = "apps@bsautermeister.de";
        private const string Blog = "bsautermeister.de";
        private const string MusicTitle = "Music sponsor:";
        private const string Music = "PLSQMPRFKT";

        private readonly Rectangle screenBounds;

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        private const string BSAUTERMEISTER_URL = "http://bsautermeister.de";
        private const string SOUND_URL = "https://soundcloud.com/plsqmprfkt";

        private readonly Rectangle EmailDestination = new Rectangle(250,230,
                                                                    300,50);
        private readonly Rectangle BlogDestination = new Rectangle(250, 290,
                                                                    300, 50);

        private const int MusicLocationY = 405;

        public static GameInput GameInput;
        private const string EmailAction = "Email";
        private const string BlogAction = "Blog";
        private const string MusicAction = "Music";
        private const string MouseSelectAction = "HelpSelect";

        private readonly Rectangle SoundCloudSource = new Rectangle(500, 900,
                                                                   240, 115);

        private readonly Rectangle SoundCloudDestination = new Rectangle(280, 350,
                                                                         240, 115);

        #endregion

        #region Constructors

        public HelpManager(Texture2D tex, SpriteFont font, Rectangle screenBounds)
        {
            this.texture = tex;
            this.font = font;
            this.screenBounds = screenBounds;
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            GameInput.AddTouchGestureInput(EmailAction,
                                           GestureType.Tap,
                                           EmailDestination);
            GameInput.AddTouchGestureInput(BlogAction,
                                           GestureType.Tap,
                                           BlogDestination);
            GameInput.AddTouchGestureInput(MusicAction,
                                           GestureType.Tap,
                                           SoundCloudDestination);

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

        private async void handleTouchInputs()
        {
            // Blog
            if (GameInput.IsPressed(BlogAction) || MouseHelper.IsLeftMouseDownDelay(BlogDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, BlogDestination))
            {
                //await global::Windows.System.Launcher.LaunchUriAsync(new Uri(BLOG_URL));
                // strange behaviour: links are sometimes opened in an infinite loop!
            }
            // Music
            if (GameInput.IsPressed(MusicAction) || MouseHelper.IsLeftMouseDownDelay(SoundCloudDestination) || MouseHelper.IsActionPressedInRegion(GameInput, MouseSelectAction, SoundCloudDestination))
            {
                await global::Windows.System.Launcher.LaunchUriAsync(new Uri(SOUND_URL));
            }
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
                             HelpTitleSource,
                             Color.White * opacity);

            for (int i = 0; i < Content.Length; ++i)
            {
                spriteBatch.DrawString(font,
                       Content[i],
                       new Vector2((screenBounds.Width - font.MeasureString(Content[i]).X) / 2,
                                   110 + (i * 35)),
                       Color.Black * opacity);
            }

            spriteBatch.DrawString(font,
                       Email,
                       new Vector2((screenBounds.Width - font.MeasureString(Email).X) / 2,
                                   EmailDestination.Y + 15),
                       Color.Black * opacity);

            spriteBatch.DrawString(font,
                       Blog,
                       new Vector2((screenBounds.Width - font.MeasureString(Blog).X) / 2,
                                   BlogDestination.Y + 15),
                       Color.Black * opacity);

            spriteBatch.Draw(texture,
                             SoundCloudDestination,
                             SoundCloudSource,
                             Color.White * opacity * 0.66f);

            spriteBatch.DrawStringBordered(
                       font,
                       MusicTitle,
                       new Vector2((screenBounds.Width - font.MeasureString(MusicTitle).X) / 2,
                                   MusicLocationY - 10),
                       Color.Black * opacity,
                       Color.White * opacity);

            spriteBatch.DrawStringBordered(
                       font,
                       Music,
                       new Vector2((screenBounds.Width - font.MeasureString(Music).X) / 2,
                                   MusicLocationY + 15),
                       Color.Black * opacity,
                       Color.White * opacity);
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
                }
            }
        }

        #endregion
    }
}
