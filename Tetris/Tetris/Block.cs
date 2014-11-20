using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetris
{
    class Block
    {
        Texture2D image;
        int gridSize;
        Color colour;
        public Block(ContentManager c, int gridSize)
        {
            image = c.Load<Texture2D>("block");
            this.gridSize = gridSize;
            this.Flash = false;
        }

        public int PositionX
        {
            get;
            set;
        }

        public int PositionY
        {
            get;
            set;
        }

        public Color Colour
        {
            get
            {
                if (Flash) return Color.White;
                else return colour;
            }
            set
            {
                colour = value;
            }
        }

        public bool Flash
        {
            get;
            set;
        }

        public void Draw(SpriteBatch s)
        {
            Vector2 pos = new Vector2(PositionX * gridSize, PositionY * gridSize);
            s.Draw(image, pos, Colour);
        }
    }
}
