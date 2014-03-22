using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WacMan
{
    class Clyde : AutomatedSprite
    {
        public Clyde(Texture2D textureImage, Vector2 position, Point frameSize, 
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed)
            : base(textureImage,position,frameSize,collisionOffset,currentFrame,sheetSize,speed,GhostName.CLYDE,50)
        {
            cornerTile = new Point(3, 15);
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds,
            LevelManager levelManager, SpriteManager spriteManager)
        {
            double distFromPlayer = Math.Sqrt(Math.Pow(spriteManager.getPlayerTile().X - currentTile.X,2.0)
                + Math.Pow(spriteManager.getPlayerTile().Y - currentTile.Y,2.0));
            if (distFromPlayer >= 8.0)
            {
                targetTile.X = spriteManager.getPlayerTile().X;
                targetTile.Y = spriteManager.getPlayerTile().Y;
            }
            else
            {
                targetTile = cornerTile;
            }

            base.Update(gameTime, clientBounds, levelManager, spriteManager);
        }
    }
}
