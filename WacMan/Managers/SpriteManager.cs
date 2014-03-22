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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        public GameMode currentGameMode = GameMode.PLAY;
        public LevelManager levelManager;
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue ghostSndCue;
        UserControlledSprite player;
        List<Pellet> pellets = new List<Pellet>();
        List<AutomatedSprite> spriteList = new List<AutomatedSprite>();
        List<GhostTarget> ghostTargets = new List<GhostTarget>();
        GhostMode ghostTeamMode = GhostMode.SCATTER;
        GhostMode lastMode;
        int levelNum = 1;
        int[] attackTimes = new int[4]{7, 34, 59, 84};
        int[] scatterTimes = new int[3]{27, 54, 79};
        int attackTimer = 0;
        int retreatTimer = 0;
        int score = 0;
        int ghostsEaten = 0;

        public SpriteManager(Game game, LevelManager levelMan)
            : base(game)
        {
            levelManager = levelMan;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");
            ghostSndCue = soundBank.GetCue("GhostSnd");
            ghostSndCue.Play();
            ghostSndCue.Pause();
            loadPellets();
            resetSpritePositions(2);
            for (int i = 0; i < 4; i++)
                ghostTargets.Add(new GhostTarget(Game.Content.Load<Texture2D>(@"Images/GhostTargets"),
                    new Vector2(spriteList[i].getCurrentTarget().X, spriteList[i].getCurrentTarget().Y),
                    new Point(32,32), new Point(i, 0)));
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (!player.isDying && !player.isDead)
            {
                if (ghostTeamMode == GhostMode.RETREAT)
                {
                    retreatTimer += gameTime.ElapsedGameTime.Milliseconds;
                    if (retreatTimer / 1000 >= 7)
                    {
                        ghostTeamMode = lastMode;
                        setGhostModes();
                        ghostSndCue.Pause();
                    }
                }
                else
                {
                    attackTimer += gameTime.ElapsedGameTime.Milliseconds;
                    foreach (int i in attackTimes)
                    {
                        if ((int)(attackTimer / 1000) == i)
                        {
                            ghostTeamMode = GhostMode.ATTACK;
                            setGhostModes();
                        }
                    }
                    foreach (int i in scatterTimes)
                    {
                        if ((int)(attackTimer / 1000) == i)
                        {
                            ghostTeamMode = GhostMode.SCATTER;
                            setGhostModes();
                        }
                    }
                }
                foreach (AutomatedSprite s in spriteList)
                    s.Update(gameTime, Game.Window.ClientBounds, levelManager, this);
                for (int i = 0; i < 4; i++)
                    ghostTargets[i].update(new Vector2(spriteList[i].getCurrentTarget().X, 
                            spriteList[i].getCurrentTarget().Y));
                checkCollisions();
            }
            if(player.isDead)
            {
                if (player.getLives() == 0)
                    currentGameMode = GameMode.LOSE;
                else
                    resetSpritePositions(player.getLives() - 1);
            }

            player.Update(gameTime, Game.Window.ClientBounds,levelManager);

            audioEngine.Update();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            levelManager.drawLevel(spriteBatch);
            foreach(Pellet p in pellets)
                p.draw(spriteBatch);
            foreach (Sprite s in spriteList)
                s.Draw(gameTime, spriteBatch);
            player.Draw(gameTime, spriteBatch);
            if(Keyboard.GetState().IsKeyDown(Keys.T))
            {
                foreach (GhostTarget g in ghostTargets)
                    g.draw(spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void resetGame()
        {
            levelNum = 1;
            attackTimer = 0;
            retreatTimer = 0;
            score = 0;
            ghostsEaten = 0;
            levelManager.loadLevel(levelNum);
            loadPellets();
            resetSpritePositions(2);
            ghostTeamMode = GhostMode.SCATTER;
            setGhostModes();
            currentGameMode = GameMode.PAUSE;
        }

        public void resetSpritePositions(int lives)
        {
            player = new UserControlledSprite(Game.Content.Load<Texture2D>(@"Images/WacMan"),
                levelManager.playerStartPos, new Point(32, 32), 10, new Point(0, 1), new Point(7, 3),
                new Vector2(2, 2), lives, 50);
            spriteList.Clear();
            spriteList.Add(new Blinky(Game.Content.Load<Texture2D>(@"Images/Ghosts"),
                new Vector2(224, 256), new Point(32, 32), 10, new Point(0, 0), new Point(5, 2),
                new Vector2(2, 2)));
            spriteList.Add(new Pinky(Game.Content.Load<Texture2D>(@"Images/Ghosts"),
                new Vector2(256, 256), new Point(32, 32), 10, new Point(1, 0), new Point(5, 2),
                new Vector2((float)1.5, (float)1.5)));
            spriteList.Add(new Inky(Game.Content.Load<Texture2D>(@"Images/Ghosts"),
                new Vector2(288, 256), new Point(32, 32), 10, new Point(2, 0), new Point(5, 2),
                new Vector2((float)1.5, (float)1.5)));
            spriteList.Add(new Clyde(Game.Content.Load<Texture2D>(@"Images/Ghosts"),
                new Vector2(320, 256), new Point(32, 32), 10, new Point(3, 0), new Point(5, 2),
                new Vector2((float)1.5, (float)1.5)));
            ghostTeamMode = GhostMode.SCATTER;
            attackTimer = 0;
        }

        public void checkCollisions()
        {
            for (int i = 0; i < spriteList.Count; i++)
            {
                if (player.isCollision(player.collisionRect, spriteList[i].collisionRect))
                {
                    if (spriteList[i].getGhostMode() == GhostMode.RETREAT)
                    {
                        ghostsEaten++;
                        score += (int)(100 * Math.Pow(2, ghostsEaten));
                        spriteList[i].setGhostMode(GhostMode.EATEN);
                        soundBank.PlayCue("EatenSnd");
                    }
                    else if(spriteList[i].getGhostMode() != GhostMode.EATEN)
                    {
                        player.startDeath();
                        if(ghostSndCue.IsPlaying)
                            ghostSndCue.Pause();
                        soundBank.PlayCue("DeathSnd");
                        break;
                    }
                }
            }

            for (int i = pellets.Count - 1; i >= 0; i--)
            {
                if (player.isCollision(player.collisionRect, pellets[i].collisionRect()))
                {
                    if (pellets[i].isSuper())
                    {
                        if(ghostTeamMode != GhostMode.RETREAT)
                            lastMode = ghostTeamMode;
                        ghostTeamMode = GhostMode.RETREAT;
                        ghostSndCue.Resume();
                        setGhostModes();
                        retreatTimer = 0;
                        ghostsEaten = 0;
                    }
                    score += pellets[i].getNumPoints();
                    pellets.RemoveAt(i);
                    if(pellets.Count % 2 == 0)
                        soundBank.PlayCue("WakaSnd");
                }
            }
            //If the player has beaten the level, load the next.
            if (pellets.Count == 0)
            {
                if (levelNum < 3)
                {
                    levelNum++;
                    levelManager.loadLevel(levelNum);
                    loadPellets();
                    resetSpritePositions(player.getLives());
                    attackTimer = 0;
                    ghostTeamMode = GhostMode.SCATTER;
                    setGhostModes();
                    currentGameMode = GameMode.PAUSE;
                }
                //If this level is the last, quit.
                else
                    currentGameMode = GameMode.WIN;
            }
        }

        private void loadPellets()
        {
            pellets.Clear();
            for (int column = 0; column < 38; column++)
            {
                for (int row = 0; row < 44; row++)
                {
                    if (levelManager.getPellet(new Point(column, row)) == 1)
                        pellets.Add(new Pellet(Game.Content.Load<Texture2D>(@"Images/Pellet"),
                            new Vector2(column, row), new Point(4, 4), 10));
                    else if (levelManager.getPellet(new Point(column, row)) == 2)
                        pellets.Add(new Pellet(Game.Content.Load<Texture2D>(@"Images/SuperPellet"),
                            new Vector2(column, row), new Point(16, 16), 50));
                }
            }
        }

        private void setGhostModes()
        {
            foreach (AutomatedSprite s in spriteList)
            {
                if (s.getGhostMode() != GhostMode.EATEN)
                    s.setGhostMode(ghostTeamMode);
            }
        }

        public Point getPlayerTile() { return player.getCurrentTile(); }

        public Point getBlinkyTile() { return spriteList[0].getCurrentTile(); }

        public Point getInkyTile() { return spriteList[2].getCurrentTile(); }

        public Point getPlayerCoords() { return player.getCoords(); }

        public int getPlayerLives() { return player.getLives(); }

        public Direction getPlayerDirection() { return player.getDirection(); }

        public int getScore() { return score; }

        public int getNumPellets() { return pellets.Count; }
    }
}