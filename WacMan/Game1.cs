using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace WacMan
{
    public enum Direction {RIGHT,UP,LEFT,DOWN};
    enum GhostName { BLINKY, PINKY, INKY, CLYDE };
    enum GhostMode { CAGED, SCATTER, ATTACK, RETREAT, EATEN };
    public enum GameMode { MMENU, PAUSE, PLAY, WIN, LOSE };
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont debugFont;
        Texture2D livesTexture, titleTexture, loseTexture, winTexture;

        SpriteManager spriteManager;
        LevelManager levelManager;


        GameMode currentGameMode = GameMode.MMENU;
        int countdownTimer = 1000;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            levelManager = new LevelManager(this);
            spriteManager = new SpriteManager(this,levelManager);
            Components.Add(spriteManager);
            spriteManager.Enabled = false;
            spriteManager.Visible = false;
            InitGraphicsMode(1024, 768, false);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            debugFont = Content.Load<SpriteFont>(@"fonts/Arial");
            livesTexture = Content.Load<Texture2D>(@"images/WacMan");
            titleTexture = Content.Load<Texture2D>(@"images/TitleScreen");
            loseTexture = Content.Load<Texture2D>(@"images/LoseScreen");
            winTexture = Content.Load<Texture2D>(@"images/WinScreen");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            switch (currentGameMode)
            {
                case GameMode.MMENU:
                    countdownTimer -= gameTime.ElapsedGameTime.Milliseconds;
                    if (countdownTimer <= 0 && (
                        Keyboard.GetState().IsKeyDown(Keys.Space)
                        ||Mouse.GetState().LeftButton==ButtonState.Pressed))
                        SetCurrentGameMode(GameMode.PAUSE);
                    break;
                case GameMode.PAUSE:
                    countdownTimer -= gameTime.ElapsedGameTime.Milliseconds;
                    if(countdownTimer <= 0)
                        SetCurrentGameMode(GameMode.PLAY);
                    break;
                case GameMode.PLAY:
                    if (spriteManager.currentGameMode == GameMode.WIN
                        || spriteManager.currentGameMode == GameMode.LOSE
                        || spriteManager.currentGameMode == GameMode.PAUSE)
                        SetCurrentGameMode(spriteManager.currentGameMode);
                    break;
                case GameMode.WIN:
                    if (Keyboard.GetState().IsKeyDown(Keys.Space)
                        || Mouse.GetState().LeftButton == ButtonState.Pressed)
                        SetCurrentGameMode(GameMode.MMENU);
                    break;
                case GameMode.LOSE:
                    if (Keyboard.GetState().IsKeyDown(Keys.Space)
                        || Mouse.GetState().LeftButton == ButtonState.Pressed)
                        SetCurrentGameMode(GameMode.MMENU);
                    break;
            }

            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

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
            switch (currentGameMode)
            {
                case GameMode.MMENU:
                    spriteBatch.Draw(titleTexture, Vector2.Zero,
                        new Rectangle(0, 0, 1024, 768), Color.White);
                    spriteBatch.DrawString(debugFont, "Left-Click or press SPACE to play Wac-Man!",
                        new Vector2(450, 600), Color.Yellow);
                    spriteBatch.DrawString(debugFont, "Press ESC to QUIT",
                        new Vector2(450, 620), Color.Yellow);
                    break;
                case GameMode.LOSE:
                    spriteBatch.Draw(loseTexture, Vector2.Zero,
                        new Rectangle(0, 0, 1024, 768), Color.White);
                    spriteBatch.DrawString(debugFont, " " + spriteManager.getScore(),
                        new Vector2(650, 610), Color.Yellow);
                    spriteBatch.DrawString(debugFont, "Left-Click or press SPACE to return to the MENU",
                        new Vector2(517, 665), Color.Yellow);
                    spriteBatch.DrawString(debugFont, "Press ESC to QUIT",
                        new Vector2(517, 685), Color.Yellow);
                    break;
                case GameMode.WIN:
                    spriteBatch.Draw(winTexture, Vector2.Zero,
                        new Rectangle(0, 0, 1024, 768), Color.White);
                    spriteBatch.DrawString(debugFont, " " + spriteManager.getScore(),
                        new Vector2(650, 610), Color.Yellow);
                    spriteBatch.DrawString(debugFont, "Left-Click or press SPACE to return to the MENU",
                        new Vector2(517, 665), Color.Yellow);
                    spriteBatch.DrawString(debugFont, "Press ESC to QUIT",
                        new Vector2(517, 685), Color.Yellow);
                    break;
                case GameMode.PAUSE:
                case GameMode.PLAY:
                    spriteBatch.DrawString(debugFont, "SCORE: " + spriteManager.getScore(),
                        new Vector2(450, 710), Color.Yellow);
                    for (int i = 0; i < spriteManager.getPlayerLives(); i++)
                        spriteBatch.Draw(livesTexture, new Vector2(20 + i * 40, 710),
                            new Rectangle(0, 32, 32, 32), Color.White);
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void SetCurrentGameMode(GameMode newGameMode)
        {
            switch (newGameMode)
            {
                case GameMode.LOSE:
                    spriteManager.Visible = false;
                    spriteManager.Enabled = false;
                    break;
                case GameMode.WIN:
                    spriteManager.Visible = false;
                    spriteManager.Enabled = false;
                    break;
                case GameMode.MMENU:
                    spriteManager.Visible = false;
                    spriteManager.Enabled = false;
                    spriteManager.resetGame();
                    countdownTimer = 1000;
                    break;
                case GameMode.PAUSE:
                    spriteManager.Visible = true;
                    spriteManager.Enabled = false;
                    countdownTimer = 3000;
                    break;
                case GameMode.PLAY:
                    spriteManager.Visible = true;
                    spriteManager.Enabled = true;
                    break;
            }
            currentGameMode = newGameMode;
            spriteManager.currentGameMode = currentGameMode;
        }



        private bool InitGraphicsMode(int iWidth, int iHeight, bool bFullScreen)
        {
            // If we aren't using a full screen mode, the height and width of the window can
            // be set to anything equal to or smaller than the actual screen size.
            if (bFullScreen == false)
            {
                if ((iWidth <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    && (iHeight <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    graphics.PreferredBackBufferWidth = iWidth;
                    graphics.PreferredBackBufferHeight = iHeight;
                    graphics.IsFullScreen = bFullScreen;
                    graphics.ApplyChanges();
                    return true;
                }
            }
            else
            {
                // If we are using full screen mode, we should check to make sure that the display
                // adapter can handle the video mode we are trying to set.  To do this, we will
                // iterate thorugh the display modes supported by the adapter and check them against
                // the mode we want to set.
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    // Check the width and height of each mode against the passed values
                    if ((dm.Width == iWidth) && (dm.Height == iHeight))
                    {
                        // The mode is supported, so set the buffer formats, apply changes and return
                        graphics.PreferredBackBufferWidth = iWidth;
                        graphics.PreferredBackBufferHeight = iHeight;
                        graphics.IsFullScreen = bFullScreen;
                        graphics.ApplyChanges();
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
