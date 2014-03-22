using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WacMan
{
    abstract class Sprite
    {
        Texture2D textureImage;
        protected Vector2 position;
        protected Point frameSize;
        protected int collisionOffset;
        protected Point currentFrame;
        protected Point sheetSize;
        protected int timeSinceLastFrame = 0;
        protected int millisecondsPerFrame;
        protected int frameChange = 1;
        protected Vector2 speed;
        protected const int defaultMillisecondsPerFrame = 50;
        public Direction moveDirect = Direction.RIGHT;
        protected Point currentTile = new Point(0, 0);

        public Sprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed)
            : this(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, defaultMillisecondsPerFrame) { }
        public Sprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed,
            int millisecondsPerFrame)
        {
            this.textureImage = textureImage;
            this.position = position;
            this.frameSize = frameSize;
            this.collisionOffset = collisionOffset;
            this.currentFrame = currentFrame;
            this.sheetSize = sheetSize;
            this.speed = speed;
            this.millisecondsPerFrame = millisecondsPerFrame;
        }

        public virtual void Update(GameTime gameTime, Rectangle clientBounds)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame -= millisecondsPerFrame;
                currentFrame.Y += frameChange;
                if (currentFrame.Y >= sheetSize.Y || currentFrame.Y < 0)
                {
                    frameChange *= -1;
                    currentFrame.Y += frameChange;
                }
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureImage,position,
                new Rectangle(currentFrame.X * frameSize.X,
                    currentFrame.Y * frameSize.Y,
                    frameSize.X,frameSize.Y),
                    Color.White,0,Vector2.Zero,1f,SpriteEffects.None,0);
        }

        public Vector2 direction()
        {
            Vector2 move = Vector2.Zero;

            if (moveDirect == Direction.LEFT)
                move.X -= 1;
            if (moveDirect == Direction.RIGHT)
                move.X += 1;
            if (moveDirect == Direction.UP)
                move.Y -= 1;
            if (moveDirect == Direction.DOWN)
                move.Y += 1;
            return move * speed;
        }

        public Rectangle collisionRect
        {
            get
            {
                return new Rectangle(
                    (int)position.X + collisionOffset,
                    (int)position.Y + collisionOffset,
                    frameSize.X - (collisionOffset * 2),
                    frameSize.Y - (collisionOffset * 2));
            }
        }
    }
}
