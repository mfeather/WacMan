using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WacMan
{
    class UserControlledSprite : Sprite
    {
        const int Num_Rows = 22;
        const int Num_Columns = 19;
        const int Tile_Size = 32;
        public const int Level_Width = 608;
        public const int Level_Height = 704;
        Vector2 movement = new Vector2(0, 0);
        private int numLives, dyingTimer;
        public bool isDying = false;
        public bool isDead = false;

        public UserControlledSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed) { }

        public UserControlledSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, int lives, int millisecondsPerFrame)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame, 
            sheetSize, speed, millisecondsPerFrame) 
        {
            numLives = lives;
            dyingTimer = 0;
        }

        public Vector2 direction(LevelManager levelManager)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left)
                && position.Y == (currentTile.Y * frameSize.Y)
                && levelManager.tileIsMovable(new Point(currentTile.X - 1,currentTile.Y)))
                moveDirect = Direction.LEFT;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)
                && position.Y == (currentTile.Y * frameSize.Y)
                && levelManager.tileIsMovable(new Point(currentTile.X + 1,currentTile.Y)))
                moveDirect = Direction.RIGHT;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)
                && position.X == (currentTile.X * frameSize.X)
                && levelManager.tileIsMovable(new Point(currentTile.X,currentTile.Y - 1)))
                moveDirect = Direction.UP;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)
                && position.X == (currentTile.X * frameSize.X)
                && levelManager.tileIsMovable(new Point(currentTile.X, currentTile.Y + 1)))
                moveDirect = Direction.DOWN;
            
            return base.direction();
        }

        public void Update(GameTime gameTime, Rectangle clientBounds,LevelManager levelManager)
        {
            if (isDying)
            {
                timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                dyingTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastFrame > millisecondsPerFrame)
                {
                    timeSinceLastFrame -= millisecondsPerFrame;
                    currentFrame.Y++;
                    if (currentFrame.Y >= sheetSize.Y)
                    {
                        currentFrame.Y = 0;
                        currentFrame.X++;
                    }
                }
                if (dyingTimer / 1000 >= 3)
                {
                    isDying = false;
                    isDead = true;
                }
            }
            else
            {
                //Wrap around player position
                if (position.X < frameSize.X)
                    position.X = Level_Width - frameSize.X*2;
                if (position.Y < frameSize.Y)
                    position.Y = Level_Height - frameSize.Y*2;
                if (position.X > Level_Width - frameSize.X * 2)
                    position.X = frameSize.X;
                if (position.Y > Level_Height - frameSize.Y * 2)
                    position.Y = frameSize.Y;
                //Set current tile
                currentTile.X = (int)(position.X / frameSize.X);
                currentTile.Y = (int)(position.Y / frameSize.Y);

                //Set movement and direction
                movement = direction(levelManager);
                currentFrame.X = (int)moveDirect;

                //Move player while it will not collide with a non-movable tile
                if (notBlocked(levelManager))
                    position += movement;

                base.Update(gameTime, clientBounds);
            }
            
        }

        private bool notBlocked(LevelManager levelManager)
        {
            Point tile;
            for (int row = 0; row < Num_Rows; row++)
            {
                for (int column = 0; column < Num_Columns; column++)
                {
                    tile.X = column;
                    tile.Y = row;
                    if (!levelManager.tileIsMovable(tile)
                    && isCollision(
                    new Rectangle((int)(position.X + movement.X), (int)(position.Y + movement.Y), 32, 32),
                    new Rectangle((int)(tile.X * frameSize.X), (int)(tile.Y * frameSize.Y), 32, 32)))
                        return false;
                }
            }
            return true;
        }

        public Point getCoords(){
            return new Point((int)position.X,(int)position.Y);
        }

        public Point getCurrentTile() { return currentTile; }

        public int getLives() { return numLives; }

        public Direction getDirection() { return moveDirect; }

        public void startDeath()
        {
            isDying = true;
            currentFrame.X = 4;
            dyingTimer = 0;
            millisecondsPerFrame = 200;
        }

        public bool isCollision( Rectangle A, Rectangle B )
        {
             if ( A.X < B.X + B.Width &&
             A.X + A.Width > B.X &&
             A.Y < B.Y + B.Height &&
             A.Y + A.Height > B.Y )
             {
                  return true;
             }
             return false;
        }
    }
}
