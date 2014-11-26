using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Tetris
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game, Tetris.Tetromino.THandler
    {
        private const int BLOCK_SIZE = 36;
        private const int GRID_WIDTH = 10;
        private const int GRID_HEIGHT = 22;
        private const int GRID_HIDDEN_ROWS = 2;
        private const int SCOREBOARD_SIZE = 200;
        private const int NEXT_OFFSET_X = 11;
        private const int NEXT_OFFSET_Y = 2;

        int dropTimer = 0;
        static int level = 1;
        int dropspeed = 65 - level * 5;

        TimeSpan flashDuration = TimeSpan.FromMilliseconds(500);
        TimeSpan startFlash = TimeSpan.Zero;
        TimeSpan downTimer = TimeSpan.FromMilliseconds(200);

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Tetromino activeT;
        List<Block> deadBlockList = new List<Block>();
        List<Block> activeBlockList = new List<Block>();
        List<int> rowsToClear = new List<int>();
        List<Block> nextBlockList = new List<Block>();

        bool enterPressed = false;
        bool rotateRightPressed = false;
        bool rotateLeftPressed = false;
        bool shiftLeftPressed = false;
        bool shiftRightPressed = false;
        bool shiftDownPressed = false;
        bool flashBegun = false;
        bool doneFlash = false;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = BLOCK_SIZE * GRID_HEIGHT;
            graphics.PreferredBackBufferWidth = BLOCK_SIZE * GRID_WIDTH + SCOREBOARD_SIZE;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            activeT = new Tetromino(this);
            
            

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (!flashBegun)
            {
                
                dropTimer++;
                GamePadState gamepad = GamePad.GetState(PlayerIndex.One);

                KeyboardState keyboard = Keyboard.GetState();
                if ((keyboard.IsKeyDown(Keys.Enter) || (gamepad.IsConnected && gamepad.Buttons.Start == ButtonState.Pressed)) && !enterPressed)
                {
                    activeT.LockT();
                    enterPressed = true;
                }
                else if ((keyboard.IsKeyUp(Keys.Enter) || (gamepad.IsConnected && gamepad.Buttons.Start == ButtonState.Released)) && enterPressed)
                {
                    enterPressed = false;
                }
                if ((keyboard.IsKeyDown(Keys.Right) || (gamepad.IsConnected && gamepad.Buttons.LeftShoulder == ButtonState.Pressed)) && !rotateRightPressed)
                {
                    if(activeT.TryTranslate(Tetromino.TranslationType.RotateRight)) dropTimer = 0;
                    rotateRightPressed = true;
                }
                else if ((keyboard.IsKeyUp(Keys.Right) || (gamepad.IsConnected && gamepad.Buttons.LeftShoulder == ButtonState.Released)) && rotateRightPressed)
                {
                    rotateRightPressed = false;
                }
                if ((keyboard.IsKeyDown(Keys.Left) || (gamepad.IsConnected && gamepad.Buttons.LeftShoulder == ButtonState.Pressed)) && !rotateLeftPressed)
                {
                    if(activeT.TryTranslate(Tetromino.TranslationType.RotateLeft)) dropTimer = 0;
                    rotateLeftPressed = true;
                }
                else if ((keyboard.IsKeyUp(Keys.Left) || (gamepad.IsConnected && gamepad.Buttons.LeftShoulder == ButtonState.Released)) && rotateLeftPressed)
                {
                    rotateLeftPressed = false;
                }

                if ((keyboard.IsKeyDown(Keys.A) || (gamepad.IsConnected && gamepad.DPad.Left == ButtonState.Pressed)) && !shiftLeftPressed)
                {
                    activeT.TryTranslate(Tetromino.TranslationType.MoveLeft);
                    shiftLeftPressed = true;
                }
                else if ((keyboard.IsKeyUp(Keys.A) || (gamepad.IsConnected && gamepad.DPad.Left == ButtonState.Released)) && shiftLeftPressed)
                {
                    shiftLeftPressed = false;
                }

                if ((keyboard.IsKeyDown(Keys.D) || (gamepad.IsConnected && gamepad.DPad.Right == ButtonState.Pressed)) && !shiftRightPressed)
                {
                    activeT.TryTranslate(Tetromino.TranslationType.MoveRight);
                    shiftRightPressed = true;
                }
                else if ((keyboard.IsKeyUp(Keys.D) || (gamepad.IsConnected && gamepad.DPad.Right == ButtonState.Released)) && shiftRightPressed)
                {
                    shiftRightPressed = false;
                }

                if ((keyboard.IsKeyDown(Keys.S) || (gamepad.IsConnected && gamepad.DPad.Down == ButtonState.Pressed)) && !shiftDownPressed)
                {
                    //if (activeT.TryTranslate(Tetromino.TranslationType.MoveDown))
                    dropspeed = 10;
                    shiftDownPressed = true;
                }
                else if ((keyboard.IsKeyUp(Keys.S) || (gamepad.IsConnected && gamepad.DPad.Down == ButtonState.Released)) && shiftDownPressed)
                {
                    shiftDownPressed = false;
                    dropspeed = 65 - level * 5;
                }

                // check to see if it's time to move the piece down automatically
                if (dropTimer >= dropspeed)
                {
                    activeT.TryTranslate(Tetromino.TranslationType.MoveDown);
                    dropTimer = 0;
                }
            }
            // update the list of active blocks (ones in the current tetromino)
            activeBlockList.Clear();
            List<Pair<int>> blocks = activeT.GetBlockLocations();
            foreach (Pair<int> t in blocks)
            {
                Block b = new Block(Content, BLOCK_SIZE);
                b.PositionX = t.First;
                b.PositionY = t.Second;
                b.Colour = activeT.Colour;
                activeBlockList.Add(b);
            }

            // check to see if any rows are complete.  If so, start the flash timer.
            if (!flashBegun)
            {
                rowsToClear = new List<int>();
                for (int i = 2; i < GRID_HEIGHT; i++)
                {
                    if (deadBlockList.Count<Block>(b => b.PositionY == i) == 10)
                    {
                        flashBegun = true;
                        startFlash = gameTime.TotalGameTime;
                        rowsToClear.Add(i);
                    }
                }
            }
            
            // if the flash timer has expired, clear the dead rows, reset flags
            if (doneFlash)
            {
                
                ClearRows(rowsToClear);
                rowsToClear.Clear();
                flashBegun = false;
                doneFlash = false;
            }

            // change colour of blocks in dead rows
            Flash(rowsToClear);

            // check to see if flash has begun.  If it has, check to see if flash
            // duration has expired.  If it has, declare it done.
            if (flashBegun && gameTime.TotalGameTime.Subtract(startFlash) > flashDuration)
            {
                doneFlash = true;
            }
            getNext();
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            foreach (Block b in activeBlockList)
            {
                b.Draw(spriteBatch);
            }
            foreach (Block b in deadBlockList)
            {
                b.Draw(spriteBatch);
            }
            foreach (Block b in nextBlockList)
            {
                b.Draw(spriteBatch);
            }
            spriteBatch.End();
           


            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        /// <summary>
        /// Checks the positions passed in against the positions of dead blocks
        /// and the edges of the board to determine whether all the tested positions
        /// map to empty locations.
        /// </summary>
        /// <param name="list">The list of tile positions to be tested</param>
        /// <returns>
        /// Returns true if all positions tested against the board are currently
        /// empty.
        /// </returns>
        public bool IsLegalTPosition(List<Pair<int>> list)
        {
            // TODO add collision logic
            
            // test if block goes past edge of grid
            foreach (Pair<int> t in list)
            {
                if (t.First < 0 || t.First > 9)
                    return false;
            }
            // test if block goes past bottom of grid
            foreach (Pair<int> t in list)
            {
                if (t.Second > 21)
                    return false;
            }
            // test if collides with other blocks
            foreach (Pair<int> t in list)
            {
                foreach (Block b in deadBlockList)
                {
                    if (t.First == b.PositionX && t.Second == b.PositionY)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void LockTBlocks()
        {
            foreach (Block b in activeBlockList)
            {
                deadBlockList.Add(b);
            }
            activeBlockList.Clear();
            
            
        }

        private void ClearRows(List<int> rows)
        {
            
            List<Block> newDeadBlocks = new List<Block>();
            foreach (Block b in deadBlockList)
            {
                if (!rows.Contains(b.PositionY))
                {
                    newDeadBlocks.Add(b);
                }
            }
            deadBlockList = newDeadBlocks;
            rows.Sort();
            
            foreach (int i in rows){
                foreach (Block b in deadBlockList)
                {
                    if (b.PositionY < i) b.PositionY++;
                }
            }
            
        }

        private void Flash(List<int> rows)
        {
            if (!doneFlash)
            {
                foreach (Block b in deadBlockList)
                {
                    if (rows.Contains(b.PositionY))
                    {
                        b.Flash = true;
                    }
                }
            }
        }

        private void getNext()
        {
            nextBlockList.Clear();
            Color blockColor=Color.White;
            foreach (Pair<int> p in Tetromino.PreviewNext(ref blockColor))
            {
                    Block b = new Block(Content, BLOCK_SIZE);
                    b.PositionX = p.First + NEXT_OFFSET_X;
                    b.PositionY = p.Second + NEXT_OFFSET_Y;
                    b.Colour = blockColor;
                    nextBlockList.Add(b);
                    
            }
            Console.WriteLine(nextBlockList[0].PositionX + " " + nextBlockList[0].PositionY);
        }


    }
}
