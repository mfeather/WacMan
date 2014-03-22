using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WacMan
{
    class GhostTarget
    {
        Texture2D textureImage;
        protected Vector2 position;
        protected Point frameSize;
        protected Point currentFrame;

        public GhostTarget(Texture2D texture, Vector2 pos, Point size, Point frame)
        {
            textureImage = texture;
            position.X = pos.X * 32;
            position.Y = pos.Y * 32;
            frameSize = size;
            currentFrame = frame;
        }

        public void update(Vector2 pos)
        {
            position.X = pos.X * 32;
            position.Y = pos.Y * 32;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureImage, position,
                new Rectangle(currentFrame.X * frameSize.X,
                    currentFrame.Y * frameSize.Y,
                    frameSize.X, frameSize.Y),
                    Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
    }
}
