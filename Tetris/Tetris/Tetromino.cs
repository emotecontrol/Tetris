using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetris
{
    class Tetromino
    {
        private List<Pair<int>> positions;
        private Color colour;
        private Type type;
        private static Random random = new Random();
        private THandler tHandler;

        public Color Colour
        {
            get { return colour; }
        }

        public enum Type
        {
            L,
            J,
            I,
            T,
            O,
            S,
            Z
        }

        public enum TranslationType
        {
            RotateLeft,
            RotateRight,
            MoveLeft,
            MoveRight,
            MoveDown,
            FailTranslate
        }

        private static Queue<Type> nextT = new Queue<Type>();

        // 
        /// <summary>
        /// Interface to make sure game class can handle collision detection
        /// and add dead tetrominos to the block pile.
        /// </summary>
        public interface THandler
        {
            bool IsLegalTPosition(List<Pair<int>> l);
            void LockTBlocks();
        }

        public Tetromino (THandler tHandler)
        {
            this.tHandler = tHandler;
            if (nextT.Count < 1)
            {
                InitQueue();
            }
            this.type = GetNext();
            positions = AssignStartPosition();
        }

        /// <summary>
        /// Adds an L, J, I, or T tetromino as the starting block.
        /// </summary>
        private static void InitQueue()
        {   
                Type firstT = (Type) random.Next(4);
                nextT.Enqueue(firstT);
        }

        /// <summary>
        /// Gets the type of the next tetromino in the queue, removing it from the queue.
        /// If the queue is running low on tetrominos, adds one of each type to the queue,
        /// in random order.
        /// </summary>
        /// <returns>The type of the new current tetromino</returns>
        private static Type GetNext()
        {
            if (nextT.Count < 2)
            {
                List<Type> tList = new List<Type>() { Type.L, Type.J, Type.I, Type.T, Type.O, Type.S, Type.Z };
                tList.Shuffle();
                foreach (Type t in tList) nextT.Enqueue(t);
            }
            return nextT.Dequeue();
        }

        /// <summary>
        /// Previews the type of the upcoming tetromino
        /// </summary>
        /// <returns>The type of the next tetromino in the queue</returns>
        public static Type PeekNext()
        {
            return nextT.Peek();
        }

        public Type TetrominoType
        {
            get { return type; }
        }

        public int XPosition
        {
            get;
            set;
        }

        public int YPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Provides a list of the grid locations of each tile, including X and Y offset
        /// </summary>
        /// <param name="list">The list of tile locations in the tetromino</param>
        /// <param name="xPos">The X offset of the tetromino</param>
        /// <param name="yPos">The Y offset of the tetromino</param>
        /// <returns>A list of (x,y) values for each tile.</returns>
        public List<Pair<int>> ProvideBlockLocations(List<Pair<int>> list, int xPos, int yPos)
        {
            List<Pair<int>> blockLocations = new List<Pair<int>>();
            foreach (Pair<int> p in list)
            {
                blockLocations.Add(new Pair<int>(p.First + xPos, p.Second + yPos));
            }
            return blockLocations;
        }

        public List<Pair<int>> GetBlockLocations()
        {
            return ProvideBlockLocations(positions, XPosition, YPosition);
        }
        /// <summary>
        /// Sets the starting position of the active tetromino, based on its type.
        /// </summary>
        /// <returns>The locations of each tile in the tetromino.</returns>
        private List<Pair<int>> AssignStartPosition()
        {
            switch (type)
            {
                case Type.L:
                    colour = Color.Orange;
                    XPosition = 2;
                    YPosition = 0;
                    return new List<Pair<int>>() { new Pair<int>(0, 1), new Pair<int>(1, 1), new Pair<int>(2, 1), new Pair<int>(2, 0) };
                    
                case Type.J:
                    colour = Color.Blue;
                    XPosition = 2;
                    YPosition = 0;
                    return new List<Pair<int>>() { new Pair<int>(0, 1), new Pair<int>(1, 1), new Pair<int>(2, 1), new Pair<int>(0, 0) };
                   
                case Type.I:
                    colour = Color.Cyan;
                    XPosition = 3;
                    YPosition = 0;
                    return new List<Pair<int>>() { new Pair<int> (0, 1), new Pair<int>(1, 1), new Pair<int>(2, 1), new Pair<int>(3, 1) };
                    
                case Type.T:
                    colour = Color.Purple;
                    XPosition = 2;
                    YPosition = 0;
                    return new List<Pair<int>>() { new Pair<int>(0, 1), new Pair<int>(1, 1), new Pair<int>(2, 1), new Pair<int>(1, 0) };
                    
                case Type.O:
                    colour = Color.Yellow;
                    XPosition = 4;
                    YPosition = 0;
                    return new List<Pair<int>>() { new Pair<int>(0, 1), new Pair<int>(1, 1), new Pair<int>(0, 0), new Pair<int>(1, 0) };
                    
                case Type.S:
                    colour = Color.Green;
                    XPosition = 2;
                    YPosition = 0;
                    return new List<Pair<int>>() { new Pair<int>(0, 1), new Pair<int>(1, 1), new Pair<int>(1, 0), new Pair<int>(2, 0) };
                    
                case Type.Z:
                    colour = Color.Red;
                    XPosition = 2;
                    YPosition = 0;
                    return new List<Pair<int>>() { new Pair<int>(1, 1), new Pair<int>(2, 1), new Pair<int>(0, 0), new Pair<int>(1, 0) };
                    
                default:
                    throw new ArgumentOutOfRangeException("Tetromino.Type", "No tetromino type specified.");                    
            }
               
        }
        /// <summary>
        /// Attempts to either move or rotate the active tetromino. If successful, the tetromino takes the
        /// new position.  If it can't move down, the blocks are locked in place and the next tetromino
        /// in the queue is activated.
        /// </summary>
        /// <param name="tType">The type of translation: move or rotate right or left, or move down</param>
        /// <returns>Whether the translation was successful.</returns>
        public bool TryTranslate(TranslationType tType)
        {
            List<Pair<int>> testLocations;
            switch (tType)
            {
                case TranslationType.RotateRight:
                    testLocations = Rotate(true);
                    //List<Pair<int>> tryTranslateLocations = (List<Pair<int>>)testLocations.Clone();
                    if (CheckRotateIsLegal(testLocations))
                    {
                        positions = testLocations;
                        return true;
                    }
                    return false;
                case TranslationType.RotateLeft:
                    testLocations = Rotate(false);
                    if (CheckRotateIsLegal(testLocations))
                    {
                        positions = testLocations;
                        return true;
                    }
                    return false;
                case TranslationType.MoveRight:
                    if (tHandler.IsLegalTPosition(ProvideBlockLocations(positions, XPosition + 1, YPosition))) 
                    {
                        XPosition++;
                        return true;
                    }
                    return false;
                case TranslationType.MoveLeft:
                    if (tHandler.IsLegalTPosition(ProvideBlockLocations(positions, XPosition - 1, YPosition)))
                    {
                        XPosition--;
                        return true;
                    }
                    return false;
                case TranslationType.MoveDown:
                    if (tHandler.IsLegalTPosition(ProvideBlockLocations(positions, XPosition, YPosition + 1)))
                    {
                        YPosition++;
                        return true;
                    }
                    else
                    {
                        LockT();
                        return false;
                    }
                default:
                    return false;
            }
            
        }

        /// <summary>
        /// Tests whether a rotation is successful.  If not, attempts to wall kick the tetromino
        /// before determining success or failure.
        /// </summary>
        /// <param name="tLocations">Locations of active tiles in the tetromino.</param>
        /// <returns>Returns true if the rotation is ultimately successful.</returns>
        private bool CheckRotateIsLegal(List<Pair<int>> tLocations){
            bool isLegal = tHandler.IsLegalTPosition(ProvideBlockLocations(tLocations, XPosition, YPosition));
                    TranslationType testShift = TranslationType.FailTranslate;
                    if (!isLegal)
                    {
                        testShift = Wallkick(tLocations);
                        if (testShift == TranslationType.MoveLeft)
                        {
                            // left wall kick successful
                            XPosition--;
                            return true;
                        }
                        else if (testShift == TranslationType.MoveRight)
                        {
                            // right wall kick successful
                            XPosition++;
                            return true;
                        }
                        else
                        {
                            // block can't find a legal position to left or right, so don't shift it
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
        }
        /// <summary>
        /// Tests whether a block can move to the left or right by one space, which happens when a
        /// block tries to rotate against a wall.  If the block can't rotate, it should try to move
        /// one space either way before failing, thereby "kicking" off the wall.
        /// </summary>
        /// <param name="tLocations">The</param>
        /// <returns></returns>
        private TranslationType Wallkick(List<Pair<int>> tLocations)
        {
            int tempX = XPosition;
            // test left shift
            if (tHandler.IsLegalTPosition(ProvideBlockLocations(tLocations, XPosition-1, YPosition)))
            {
                return TranslationType.MoveLeft;
            }
            if (tHandler.IsLegalTPosition(ProvideBlockLocations(tLocations, XPosition+1, YPosition)))
            {
                return TranslationType.MoveRight;
            }
            return TranslationType.FailTranslate;
            
        }
        /// <summary>
        /// Finds the position of each tile in the active tetromino following a rotation.
        /// </summary>
        /// <param name="direction">The direction of rotation, either right (true) or left (false).</param>
        /// <returns>The list of new X,Y positions of each tile in the tetromino.</returns>
        private List<Pair<int>> Rotate(bool direction)
        {
            List<Pair<int>> newList = new List<Pair<int>>();
            
            switch (type)
            {
                case Type.I:
                    // test to see whether horizontal or vertical (all ys same or all xs same)
                    Boolean vertical = false;
                    int line = 0;
                    if (positions.TrueForAll(p => p.First == 1 || p.First == 2))
                    {
                        vertical = true;
                        line = positions[0].First;
                    }
                    else
                    {
                        vertical = false;
                        line = positions[0].Second;
                    }
                    int newLine;
                    if (direction)
                    {
                        if (vertical)
                        {
                            newLine = line == 1 ? 1 : 2;
                            return new List<Pair<int>>() { new Pair<int>(0, newLine), new Pair<int>(1, newLine), new Pair<int>(2, newLine), new Pair<int>(3, newLine) };
                        }
                        else
                        {
                            newLine = line == 1 ? 2 : 1;
                            return new List<Pair<int>>() { new Pair<int>(newLine, 0), new Pair<int>(newLine, 1), new Pair<int>(newLine, 2), new Pair<int>(newLine, 3) };
                        }
                    }
                    else
                    {
                        if (vertical)
                        {
                            newLine = line == 1 ? 2 : 1;
                            return new List<Pair<int>>() { new Pair<int>(0, newLine), new Pair<int>(1, newLine), new Pair<int>(2, newLine), new Pair<int>(3, newLine) };
                        }
                        else
                        {
                            newLine = line == 1 ? 1 : 2;
                            return new List<Pair<int>>() { new Pair<int>(newLine, 0), new Pair<int>(newLine, 1), new Pair<int>(newLine, 2), new Pair<int>(newLine, 3) };
                        }
                    }
     
                case Type.O:
                    // square blocks don't rotate
                    return positions;
                    
                default:
                    foreach (Pair<int> p in positions)
                    {
                        
                        int x = p.First;
                        int y = p.Second;
                        // rotation algorithm for L, J, S, Z pieces
                        // (n,m) -> (Abs(m-2), n)) if right; (n,m) -> (m, Abs(n-2)) if left
                        if (direction) newList.Add(new Pair<int>(Math.Abs(y - 2), x));
                        else newList.Add(new Pair<int>(y, Math.Abs(x-2)));
                    }
                    return newList;
                    

            }
        }

        /// <summary>
        /// Method used to lock the position of the blocks on the grid and activate the
        /// next tetromino. Returns the type of the next tetromino and sets its position 
        /// to the start position for that block type.
        /// </summary>
        /// <returns>
        /// The type of tetromino that has been activated.
        /// </returns>
        public Type LockT()
        {
            tHandler.LockTBlocks();
            type = GetNext();
            positions = AssignStartPosition();
            return type;
        }

        /**
         * rotation sequence:
         * 1. Rotate();
         * 2. Locate new set of positions on grid by adding X and Y offsets
         * 3. Determine if this puts the tetromino off the left or right edge
         * 4. If so, test it one space to the right, then one space to the left,
         *    and fail to rotate if neither position works
         * 5. Replace stored positions with new positions
         */
         
    }
    
}
