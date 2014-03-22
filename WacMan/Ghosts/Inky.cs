using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WacMan
{
    class Inky : AutomatedSprite
    {
        public Inky(Texture2D textureImage, Vector2 position, Point frameSize, 
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed)
            : base(textureImage,position,frameSize,collisionOffset,currentFrame,sheetSize,speed,GhostName.INKY,50)
        {
            cornerTile = new Point(2, 2);
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds,
            LevelManager levelManager, SpriteManager spriteManager)
        {
            Vector2 tempTile = new Vector2(0, 0);
            Vector2 tileDiff = new Vector2(0,0);
            switch (spriteManager.getPlayerDirection())
            {
                case Direction.UP:
                    tempTile.X = spriteManager.getPlayerTile().X;
                    tempTile.Y = spriteManager.getPlayerTile().Y - 2;
                    break;
                case Direction.DOWN:
                    tempTile.X = spriteManager.getPlayerTile().X;
                    tempTile.Y = spriteManager.getPlayerTile().Y + 2;
                    break;
                case Direction.RIGHT:
                    tempTile.X = spriteManager.getPlayerTile().X + 2;
                    tempTile.Y = spriteManager.getPlayerTile().Y;
                    break;
                case Direction.LEFT:
                    tempTile.X = spriteManager.getPlayerTile().X - 2;
                    tempTile.Y = spriteManager.getPlayerTile().Y;
                    break;
            }

            tileDiff.X = tempTile.X - spriteManager.getBlinkyTile().X;
            tileDiff.Y = tempTile.Y - spriteManager.getBlinkyTile().Y;

            targetTile.X = (int)(tempTile.X + tileDiff.X);
            targetTile.Y = (int)(tempTile.Y + tileDiff.Y);

            base.Update(gameTime, clientBounds, levelManager, spriteManager);
        }
    }
}
