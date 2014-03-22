using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WacMan
{
    class Pinky : AutomatedSprite
    {
        public Pinky(Texture2D textureImage, Vector2 position, Point frameSize, 
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed)
            : base(textureImage,position,frameSize,collisionOffset,currentFrame,sheetSize,speed,GhostName.PINKY,50)
        {
            cornerTile = new Point(15, 15);
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds,
            LevelManager levelManager, SpriteManager spriteManager)
        {
            switch (spriteManager.getPlayerDirection())
            {
                case Direction.UP:
                    targetTile.X = spriteManager.getPlayerTile().X;
                    targetTile.Y = spriteManager.getPlayerTile().Y - 4;
                    break;
                case Direction.DOWN:
                    targetTile.X = spriteManager.getPlayerTile().X;
                    targetTile.Y = spriteManager.getPlayerTile().Y + 4;
                    break;
                case Direction.RIGHT:
                    targetTile.X = spriteManager.getPlayerTile().X + 4;
                    targetTile.Y = spriteManager.getPlayerTile().Y;
                    break;
                case Direction.LEFT:
                    targetTile.X = spriteManager.getPlayerTile().X - 4;
                    targetTile.Y = spriteManager.getPlayerTile().Y;
                    break;
            }

            base.Update(gameTime, clientBounds, levelManager, spriteManager);
        }
    }
}
