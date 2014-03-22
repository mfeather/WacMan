using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WacMan
{
    class Blinky : AutomatedSprite
    {
        public Blinky(Texture2D textureImage, Vector2 position, Point frameSize, 
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed)
            : base(textureImage,position,frameSize,collisionOffset,currentFrame,sheetSize,speed,GhostName.BLINKY,50)
        {
            cornerTile = new Point(16, 2);
        }
        
        public override void Update(GameTime gameTime, Rectangle clientBounds,
            LevelManager levelManager, SpriteManager spriteManager)
        {
            targetTile = spriteManager.getPlayerTile();
            base.Update(gameTime,clientBounds,levelManager,spriteManager);
        }
    }
}
