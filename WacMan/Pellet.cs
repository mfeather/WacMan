using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WacMan
{
    class Pellet
    {
        Texture2D textureImage;
        protected Vector2 position;
        protected Point frameSize;
        protected int numPoints;

        public Pellet(Texture2D texture, Vector2 pos, Point size, int points)
        {
            textureImage = texture;
            frameSize = size;
            position.X = pos.X * 16 - frameSize.X/2;
            position.Y = pos.Y * 16 - frameSize.Y/2;
            numPoints = points;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureImage, position, new Rectangle(0,0,frameSize.X, frameSize.Y),
                    Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        public Rectangle collisionRect()
        {
            return new Rectangle((int)position.X, (int)position.Y, frameSize.X, frameSize.Y);
        }

        public int getNumPoints() { return numPoints; }

        public bool isSuper()
        {
            if (numPoints == 50)
                return true;
            else
                return false;
        }
    }
}
