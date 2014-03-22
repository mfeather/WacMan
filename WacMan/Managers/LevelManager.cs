using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WacMan
{
    public class LevelManager
    {
        const int Num_Rows = 22;
        const int Num_Columns = 19;
        const int Tile_Size = 32;
        int[,] tiles = new int[Num_Rows,Num_Columns];
        int[,] pellets = new int[Num_Rows * 2, Num_Columns * 2];
        bool[,] movable = new bool[Num_Rows, Num_Columns];
        bool[,] intersect = new bool[Num_Rows, Num_Columns];

        Texture2D mapSheet;
        string levelName;
        string pelletsName;
        public Vector2 playerStartPos;

        public LevelManager(Game game)
        {
            mapSheet = game.Content.Load<Texture2D>(@"Images/MapSheet");
            loadLevel(1);
            findIntersect();
        }

        public void loadLevel(int levelNum)
        {
            levelName = "Level" + levelNum;
            pelletsName = levelName + "Pellets.txt";
            levelName = levelName + ".txt";
            using(StreamReader sr = File.OpenText(levelName))
            {
                string s = sr.ReadToEnd();
                string[] inputs = s.Split(null);
                for (int column = 0; column < Num_Columns; column++)
                    {
                    for(int row = 0; row < Num_Rows; row++)
                    {
                    
                        s = inputs[row * Num_Columns + column];
                        tiles[row, column] = Int32.Parse(s);
                        if (tiles[row, column] == 00)
                            movable[row, column] = true;
                        else
                            movable[row, column] = false;
                    }
                }
                s = inputs[Num_Rows * Num_Columns];
                playerStartPos.X = Int32.Parse(s);
                s = inputs[Num_Rows * Num_Columns + 1];
                playerStartPos.Y = Int32.Parse(s);
                sr.Close();
            }

            using (StreamReader sr = File.OpenText(pelletsName))
            {
                string s = sr.ReadToEnd();
                string[] inputs = s.Split(null);
                for (int column = 0; column < Num_Columns*2; column++)
                {
                    for (int row = 0; row < Num_Rows*2; row++)
                    {

                        s = inputs[row * Num_Columns*2 + column];
                        pellets[row, column] = Int32.Parse(s);
                    }
                }
                sr.Close();
            }
        }

        public void drawLevel(SpriteBatch spriteBatch)
        {
            for (int column = 0; column < Num_Columns; column++)
                {
                for (int row = 0; row < Num_Rows; row++)
                {
                
                    spriteBatch.Draw(mapSheet, new Rectangle(column * Tile_Size, row * Tile_Size,
                        Tile_Size,Tile_Size),
                        new Rectangle((tiles[row, column] / 4) * Tile_Size,
                        (tiles[row, column] % 4) * Tile_Size, Tile_Size, Tile_Size),
                        Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
                }
            }
        }

        private void findIntersect()
        {
            
            for (int column = 0; column < Num_Columns; column++)
            {
                for (int row = 0; row < Num_Rows; row++)
                {
                    intersect[row, column] = false;
                }
            }
            for (int column = 1; column < Num_Columns - 1; column++)
            {
                for (int row = 1; row < Num_Rows - 1; row++)
                {
                    if (tileIsMovable(new Point(column, row)))
                    {
                        int directions = 0;
                        if (tileIsMovable(new Point(column - 1, row)))
                            directions++;
                        if (tileIsMovable(new Point(column + 1, row)))
                            directions++;
                        if (tileIsMovable(new Point(column, row - 1)))
                            directions++;
                        if (tileIsMovable(new Point(column, row + 1)))
                            directions++;

                        if (directions > 2)
                            intersect[row, column] = true;
                    }
                }
            }
        }

        public bool tileIsMovable(Point point) { return movable[point.Y, point.X]; }

        public bool tileIsIntersect(Point point) { return intersect[point.Y, point.X]; }

        public bool tileIsGate(Point point)
        {
            if (tiles[point.Y,point.X] == 40)
                return true;
            else
                return false;
        }

        public bool isLevelOver()
        {
            if (pellets.Length == 0)
                return true;
            else
                return false;
        }

        public int getPellet(Point point) { return pellets[point.Y, point.X]; }
    }
}
