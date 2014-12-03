using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Tetris
{
    class MenuButton
    {
        Texture2D texture;
        Texture2D mainTexture;
        Texture2D focusTexture;
        Vector2 position, textWidth;
        Rectangle rectangle;
        Vector2 textPosition;
        string text;
        SpriteFont font;
        bool hasText = false;
        bool colourSpin = false;
        Color colour = Color.White;
        Color textColour = Color.Black;
        

        public Vector2 size;
        public bool isActive = false;

        public int Height
        {
            get { return (int)size.Y; }
            
        }

        public Color Colour
        {
            get { return colour; }
            set { colour = value; }
        }

        public Color TextColour
        {
            get { return textColour; }
            set { textColour = value; }
        }

        public int Width
        {
            get { return (int)size.X; }
        }

        public bool ColourSpin
        {
            get { return colourSpin; }
            set { colourSpin = value; }
        }
        public MenuButton(Texture2D texture, Texture2D focusTexture, GraphicsDevice g)
        {
            this.texture = texture;
            this.mainTexture = texture;
            this.focusTexture = focusTexture;
            size = new Vector2(100, 100);
            
        }

        
        public bool isClicked;
        public void Update(MouseState mouse)
        {
            isClicked = false;
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);

            Rectangle trackingRectangle = new Rectangle(mouse.X, mouse.Y, 1,1);

            if (trackingRectangle.Intersects(rectangle) && isActive)
            {
                
                texture = focusTexture;
                if (mouse.LeftButton == ButtonState.Pressed) isClicked = true;
                else isClicked = false;
            }
            else
            {
                texture = mainTexture;
                isClicked = false;
            }

        }

        public void SetText(string text, SpriteFont font, ContentManager c)
        {
            this.text = text;
            this.font = font;
            this.hasText = true;
            SetTextPosition(text, font);
           
        }

        public void SetTextPosition(string text, SpriteFont font)
        {
            textWidth = font.MeasureString(text);
            textPosition = new Vector2((position.X + size.X / 2 - textWidth.X / 2), (position.Y + size.Y / 2 - textWidth.Y / 2));

        }

        public void setPosition(Vector2 newPosition){
            position = newPosition;
            if (hasText) SetTextPosition(text, font);
            
        }

        public void setSize(Vector2 newSize)
        {
            size = newSize;
        }

        public void activate()
        {
            this.isActive = true;
            
        }

        public void deactivate()
        {
            this.isActive = false;
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isActive) spriteBatch.Draw(texture, rectangle, Colour);
            if (hasText)
            {
                spriteBatch.DrawString(font, text, textPosition, TextColour);
            }
        }
    }
    
}
