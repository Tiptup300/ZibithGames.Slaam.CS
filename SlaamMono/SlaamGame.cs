using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SlaamMono.Helpers;
using SlaamMono.Library.Drawing.Text;
using SlaamMono.Library.Input;
using SlaamMono.Library.Logging;
using SlaamMono.Resources;
using SlaamMono.Resources.Loaders;
using SlaamMono.Screens;
using System;
using ZBlade;

namespace SlaamMono
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SlaamGame : Game, ISlaamGame
    {
        private GraphicsDeviceManager graphics;
        new public static ContentManager Content;
        private SpriteBatch gamebatch;

        public static ZuneBlade mainBlade;
        public static SlaamGame Instance { get { return instance; } }

        public static GraphicsDeviceManager Graphics { get { return instance.graphics; } }

        public static SlaamGame instance;

        public static bool ShowFPS = false;

        private XnaContentManager _contentManager;

        private readonly ILogger _logger;
        private readonly IScreenDirector _screenDirector;
        private readonly IFirstScreenResolver _firstScreenResolver;

        public SlaamGame(ILogger logger, IScreenDirector screenDirector, IFirstScreenResolver firstScreenResolver)
        {
            _logger = logger;
            _screenDirector = screenDirector;
            _firstScreenResolver = firstScreenResolver;

            graphics = new GraphicsDeviceManager(this);
            Content = new ContentManager(Services);
            this.Exiting += Game1_Exiting;

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = GameGlobals.DRAWING_GAME_WIDTH;
            graphics.PreferredBackBufferHeight = GameGlobals.DRAWING_GAME_HEIGHT;
            this.IsFixedTimeStep = false;

            graphics.PreferMultiSampling = false;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            Components.Insert(0, new FrameRateDirector(this));
            Components.Add(new InputComponent(this));


            SetupZuneBlade();
            _logger.Log("Creating SpriteBatch...");
            gamebatch = new SpriteBatch(graphics.GraphicsDevice);
            _logger.Log("Created SpriteBatch;");
            base.Initialize();

            _logger.Log("Set Graphics Settings (1280x1024 No MultiSampling);");
            instance = this;
            ResourceManager.Initiailze(
                DiImplementer.Instance.Get<ILogger>(), 
                DiImplementer.Instance.Get<IImageLoader>(),
                DiImplementer.Instance.Get<IPixelFactory>(),
                DiImplementer.Instance.Get<ITextLineLoader>(),
                DiImplementer.Instance.Get<IFontLoader>());
            ResourceManager.LoadAll();
            Qwerty.CurrentPlayer = InputComponent.Players[0];
            _contentManager = new XnaContentManager(DiImplementer.Instance.Get<ILogger>());

            GameGlobals.SetupGame();
            _screenDirector.ChangeTo(_firstScreenResolver.Resolve());
        }

        public void SetupZuneBlade()
        {
            mainBlade = new ZuneBlade(this);

            mainBlade.Opacity = 1f;
            mainBlade.CurrentGameInfo = new GameInfo("Slaam! Mobile", "Zibith Games", null);
            mainBlade.UserCanNavigateMenu = true;
            mainBlade.UserCanCloseMenu = false;
            mainBlade.Status = BladeStatus.Hidden;
            mainBlade.ScreenWidth = GameGlobals.DRAWING_GAME_WIDTH;
            mainBlade.ScreenHeight = GameGlobals.DRAWING_GAME_HEIGHT;

            Components.Add(mainBlade);
        }


        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Console.WriteLine("dog");

            if (_contentManager.NeedsDevice)
            {
                _contentManager.Update();
            }
            else
            {
                BackgroundManager.Update();
                FeedManager.Update();

                if (Qwerty.Active)
                {
                    Qwerty.Update();
                }
                else
                {
                    _screenDirector.Update();
                }
            }

        }


        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Black);

            gamebatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Matrix.Identity);

            BackgroundManager.Draw(gamebatch);

            _screenDirector.Draw(gamebatch);

            FeedManager.Draw(gamebatch);

            if (Qwerty.Active)
                Qwerty.Draw(gamebatch);

            if (ShowFPS)
            {
                string temp = "" + FrameRateDirector.FUPS;
                Vector2 fpsBack = ResourceManager.SegoeUIx32pt.MeasureString(temp);
                gamebatch.Draw(ResourceManager.Dot, new Rectangle(0, 0, (int)fpsBack.X + 10, (int)fpsBack.Y), new Color(0, 0, 0, 100));
                ResourceManager.DrawText(temp, new Vector2(5, fpsBack.Y / 2f), ResourceManager.SegoeUIx32pt, TextAlignment.Default, Color.White, true);
            }

            gamebatch.End();

            base.Draw(gameTime);
        }



        void Game1_Exiting(object sender, EventArgs e)
        {
            GC.Collect();
        }

    }
}