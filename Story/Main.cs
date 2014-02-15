using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;

namespace Story
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        public const bool GodMode = false;
        public const bool DebugMode = true;
        public const bool Muted = true;
        // Resources for drawing.
        private GraphicsDeviceManager Graphics;
        private SpriteBatch SpriteBatch;

        private SpriteFont HudFont;

        private Texture2D WinOverlay;
        private Texture2D LoseOverlay;
        private Texture2D DiedOverlay;

        private Level Level;
        private bool WasContinuePressed;

        private const int TargetFrameRate = 60;
        public const int BackBufferWidth = 1280;
        public const int BackBufferHeight = 720;
        private const Buttons ContinueButton = Buttons.A;

        public Main()
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreferredBackBufferWidth = BackBufferWidth;
            Graphics.PreferredBackBufferHeight = BackBufferHeight;

            Content.RootDirectory = "Content";

            // Framerate differs between platforms.
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);

        }
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            Level = new Level(Services);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Load fonts
            HudFont = Content.Load<SpriteFont>("Fonts/font");

            // Load overlay textures
            WinOverlay = Content.Load<Texture2D>("Overlays/you_win");
            LoseOverlay = Content.Load<Texture2D>("Overlays/you_lose");
            DiedOverlay = Content.Load<Texture2D>("Overlays/you_died");

            MediaPlayer.IsRepeating = true;

            if (!Muted)
                MediaPlayer.Play(Content.Load<Song>("Sounds/Music"));

            Level.LoadLevel();

        }

        TimeSpan ElapsedTime = TimeSpan.Zero;
        int FPS = 0;
        int FrameCounter = 0;
        protected override void Update(GameTime gameTime)
        {
            HandleInput();
            Level.Update(gameTime);

            if (Main.DebugMode)
            {
                ElapsedTime += gameTime.ElapsedGameTime;

                if (ElapsedTime > TimeSpan.FromSeconds(1))
                {
                    ElapsedTime -= TimeSpan.FromSeconds(1);
                    FPS = FrameCounter;
                    FrameCounter = 0;
                }
            }

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);

            //Exit game if back/esc pressed for now
            if (gamepadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            bool continuePressed =
                keyboardState.IsKeyDown(Keys.Enter) ||
                gamepadState.IsButtonDown(ContinueButton);

            //Go to next level
            if (!WasContinuePressed && continuePressed)
            {
                if (Level.ReachedExit)
                {
                    Level.LevelID++;
                    Level.LoadLevel();
                }
            }

            WasContinuePressed = continuePressed;
        }

        protected override void Draw(GameTime GameTime)
        {
            FrameCounter++;

            Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin();

            Level.Draw(GameTime, SpriteBatch);

            DrawUI(GameTime);

            SpriteBatch.End();

            base.Draw(GameTime);
        }

        private void DrawUI(GameTime GameTime)
        {
            Rectangle TitleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 HudLocation = new Vector2(TitleSafeArea.X, TitleSafeArea.Y);
            Vector2 Center = new Vector2(TitleSafeArea.X + TitleSafeArea.Width / 2.0f,
                                         TitleSafeArea.Y + TitleSafeArea.Height / 2.0f);


            if (Main.DebugMode)
                SpriteBatch.DrawString(HudFont, "FPS " + FPS.ToString(), HudLocation + new Vector2(Main.BackBufferWidth / 2 - 24, Main.BackBufferHeight / 2 + 256), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            // Draw UI egg count
            DrawShadowedString(HudFont, "x" + Level.Eggs.ToString(), HudLocation + new Vector2(Level.HeartTexture[0].Width + Level.EggUITexture.Width, 64), Color.Yellow);

            SpriteBatch.Draw(Level.HeartTexture[Level.Player.Health], Vector2.Zero, Color.White);
            SpriteBatch.Draw(Level.EggUITexture, new Vector2(Level.HeartTexture[0].Width, 64 + Level.EggUITexture.Height / 2), Color.White);

            //Mind power
            SpriteBatch.Draw(Level.MindPowerBackTexture, new Vector2(Main.BackBufferWidth - Level.MindPowerBackTexture.Width - 8, 8), Color.White);
            Level.MindPowerEmitter.Draw(GameTime, SpriteBatch, Vector2.Zero);
            SpriteBatch.Draw(Level.MindPowerCoverTexture, new Vector2(Main.BackBufferWidth - Level.MindPowerBackTexture.Width - 8, 8), Color.White);

            // Determine the status overlay message to show.
            Texture2D Status = null;
            if (Level.ReachedExit)
                Status = WinOverlay;

            if (Status != null)
            {
                // Draw status message.
                Vector2 statusSize = new Vector2(Status.Width, Status.Height);
                SpriteBatch.Draw(Status, Center - statusSize / 2, Color.White);
            }
        }

        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            SpriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            SpriteBatch.DrawString(font, value, position, color);
        }
    }
}