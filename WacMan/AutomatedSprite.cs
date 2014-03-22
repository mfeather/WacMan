using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WacMan
{
    class AutomatedSprite : Sprite
    {
        const int Num_Rows = 22;
        const int Num_Columns = 19;
        const int Tile_Size = 32;
        public const int Level_Width = 608;
        public const int Level_Height = 704;
        GhostName name;
        GhostMode mode = GhostMode.ATTACK;
        protected Point nextTile,targetTile,cornerTile;
        protected Vector2 normalSpeed;
        protected int retreatTimer = 0;
        public AutomatedSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed,GhostName gName)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed) 
        {
            name = gName;
        }

        public AutomatedSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, GhostName gName, int millisecondsPerFrame)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame, 
            sheetSize, speed, millisecondsPerFrame) 
        {
            name = gName;
            normalSpeed = speed;
        }

        public Vector2 direction(LevelManager levelManager)
        {
            if (mode == GhostMode.SCATTER)
            {
                targetTile = cornerTile;
            }

            //If it's time to change direction
            if (Math.Abs(position.X - currentTile.X * 32) <= 2 
                && Math.Abs(position.Y - currentTile.Y * 32) <= 2
                && (levelManager.tileIsIntersect(currentTile) 
                || !levelManager.tileIsMovable(nextTile)))
            {
                bool right, down, up, left;
                right = down = up = left = false;
                
                //Set move direction options
                if (levelManager.tileIsMovable(new Point(currentTile.X + 1, currentTile.Y))
                    && moveDirect != Direction.LEFT)
                    right = true;
                if (levelManager.tileIsMovable(new Point(currentTile.X - 1, currentTile.Y))
                    && moveDirect != Direction.RIGHT)
                    left = true;
                if (levelManager.tileIsMovable(new Point(currentTile.X, currentTile.Y + 1))
                    && moveDirect != Direction.UP)
                    down = true;
                if (levelManager.tileIsMovable(new Point(currentTile.X, currentTile.Y - 1))
                    && moveDirect != Direction.DOWN)
                    up = true;

                if (mode == GhostMode.RETREAT)
                {
                    Direction rand = (Direction)RandomNumber(0, 3);
                    if (rand == Direction.UP && up)
                        moveDirect = Direction.UP;
                    else if (rand == Direction.DOWN && down)
                        moveDirect = Direction.DOWN;
                    else if (rand == Direction.RIGHT && right)
                        moveDirect = Direction.RIGHT;
                    else if (rand == Direction.LEFT && left)
                        moveDirect = Direction.LEFT;
                }

                if (mode == GhostMode.ATTACK || mode == GhostMode.SCATTER || mode == GhostMode.EATEN)
                {

                    //First, try to move toward target

                    //If horizontal distance is larger, check those first
                    if (Math.Abs(targetTile.X - currentTile.X) > Math.Abs(targetTile.Y - currentTile.Y))
                    {
                        if (targetTile.X > currentTile.X && right)
                        {
                            moveDirect = Direction.RIGHT;
                            return base.direction();
                        }
                        if (targetTile.X < currentTile.X && left)
                        {
                            moveDirect = Direction.LEFT;
                            return base.direction();
                        }
                        if (targetTile.Y < currentTile.Y && up)
                        {
                            moveDirect = Direction.UP;
                            return base.direction();
                        }
                        if (targetTile.Y > currentTile.Y && down)
                        {
                            moveDirect = Direction.DOWN;
                            return base.direction();
                        }
                    }
                    //Otherwise, check vertical directions
                    else
                    {
                        if (targetTile.Y < currentTile.Y && up)
                        {
                            moveDirect = Direction.UP;
                            return base.direction();
                        }
                        if (targetTile.Y > currentTile.Y && down)
                        {
                            moveDirect = Direction.DOWN;
                            return base.direction();
                        }
                        if (targetTile.X > currentTile.X && right)
                        {
                            moveDirect = Direction.RIGHT;
                            return base.direction();
                        }
                        if (targetTile.X < currentTile.X && left)
                        {
                            moveDirect = Direction.LEFT;
                            return base.direction();
                        }
                    }
                }

                //If that fails, pick any available direction
                if (up)
                {
                    moveDirect = Direction.UP;
                    return base.direction();
                }
                if (down)
                {
                    moveDirect = Direction.DOWN;
                    return base.direction();
                }
                if (right)
                {
                    moveDirect = Direction.RIGHT;
                    return base.direction();
                }
                if (left)
                {
                    moveDirect = Direction.LEFT;
                    return base.direction();
                }
            }
            return base.direction();
        }

        public virtual void Update(GameTime gameTime, Rectangle clientBounds, 
            LevelManager levelManager, SpriteManager spriteManager)
        {
            //Wrap around player position
            if (position.X < frameSize.X)
                position.X = Level_Width - frameSize.X*2;
            if (position.Y < frameSize.Y)
                position.Y = Level_Height - frameSize.Y*2;
            if (position.X > Level_Width - frameSize.X*2 + 1)
                position.X = frameSize.X;
            if (position.Y > Level_Height - frameSize.Y*2 + 1)
                position.Y = frameSize.Y;
            
            //Set current tile
            currentTile.X = (int)(position.X / 32);
            currentTile.Y = (int)(position.Y / 32);
            
            setNextTile();

            if (mode == GhostMode.RETREAT)
            {
                speed = new Vector2((float)(normalSpeed.X/2.0), (float)(normalSpeed.Y/2.0));
                currentFrame.X = 4; 
                retreatTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (7000 - retreatTimer <= 2000)
                    currentFrame.X = 4 + (int)(retreatTimer / 333) % 2;
            }
            else if (mode == GhostMode.EATEN)
            {
                speed = normalSpeed;
                currentFrame.X = 6;
                targetTile = new Point(9, 8);
                if (currentTile == targetTile)
                    mode = GhostMode.ATTACK;
            }
            else
            {
                speed = normalSpeed;
                currentFrame.X = (int)name;
            }

            position += direction(levelManager);
            
            
            base.Update(gameTime, clientBounds);
        }

        private void setNextTile()
        {
            if (moveDirect == Direction.RIGHT)
            {
                nextTile.X = currentTile.X + 1;
                nextTile.Y = currentTile.Y;
            }
            if (moveDirect == Direction.LEFT)
            {
                nextTile.X = currentTile.X - 1;
                nextTile.Y = currentTile.Y;
            }
            if (moveDirect == Direction.DOWN)
            {
                nextTile.X = currentTile.X;
                nextTile.Y = currentTile.Y + 1;
            }
            if (moveDirect == Direction.UP)
            {
                nextTile.X = currentTile.X;
                nextTile.Y = currentTile.Y - 1;
            }

            //Wrap around tile if out of bounds
            if (nextTile.X > Num_Columns - 1)
                nextTile.X = 0;
            if (nextTile.Y > Num_Rows - 1)
                nextTile.Y = 0;
            if (nextTile.X < 0)
                nextTile.X = Num_Columns - 1;
            if (nextTile.Y < 0)
                nextTile.Y = Num_Rows - 1;
        }

        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public void setGhostMode(GhostMode newMode) 
        {
            if (newMode == GhostMode.RETREAT)
            {
                retreatTimer = 0;
            }
            mode = newMode;
        }

        public GhostMode getGhostMode() { return mode; }

        public Point getCurrentTile() { return currentTile; }

        public Point getCurrentTarget() { return targetTile; }

    }
}
