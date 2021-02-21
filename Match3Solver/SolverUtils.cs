﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Match3Solver
{
    public class SolverUtils : SolverInterface
    {
        
        System.Windows.Media.Color[] rawColor = new System.Windows.Media.Color[] {
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF5F1282"), //0 - Broken heart
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFF553B4"), //1 - Heart
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFA3ABC3"), //2 - Stamina
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF51C1C6"), //3 - Sentiment
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF115CA8"), //4 - Blue
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFE55949"), //5 - Red
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF8EB94A"), //6 - Green
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFBF6D16"), //7 - Gold
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFDDB842")  //8 - Bell
        };

        int length;
        int width;
        System.Windows.Shapes.Rectangle[][] boardDisplay;
        public SolverUtils(int length, int width, System.Windows.Shapes.Rectangle[][] boardDisplay)
        {
            this.length = length;
            this.width = width;
            this.boardDisplay = boardDisplay;
        }
        public List<SolverInterface.Movement> loopBoard(int[][] board2Test)
        {
            List<SolverInterface.Movement> returnThis = new List<SolverInterface.Movement>();
            int y = 0;
            int x = 0;
            while (y < length)
            {
                x = 0;
                while (x < width)
                {
                    /////////////////////
                    // HORIZONTAL
                    /////////////////////
                    ///
                    // MOVE LEFT
                    int offset = 1;
                    while (x - offset > -1)
                    {
                        // DEEP COPY
                        int[][] board2TestCopy = Array.ConvertAll(board2Test, a => (int[])a.Clone());

                        // MOVE
                        board2TestCopy = moveHorizontal(y, x, -1 * offset, board2TestCopy);
                        int boardHash = getBoardHash(board2TestCopy);

                        // ONLY SAVE IF THERE WAS A MATCH/SCORE
                        SolverInterface.Score result = evalBoard(new SolverInterface.Score(0), board2TestCopy, true);
                        if (result.hasScore())
                        {
                            // CHECK IF BOARD PATTERN ALREADY EXIST. REDUCE DUPLICATE
                            SolverInterface.Movement testExist = returnThis.SingleOrDefault(s => s.boardHash == boardHash);
                            if (!testExist.score.hasScore())
                            {
                                returnThis.Add(new SolverInterface.Movement(x, y, false, -1 * offset, result, boardHash));
                            }
                        }
                        offset++;
                    }
                    // MOVE RIGHT
                    offset = 1;
                    while (x + offset < width)
                    {
                        // DEEP COPY
                        int[][] board2TestCopy = Array.ConvertAll(board2Test, a => (int[])a.Clone());

                        // MOVE
                        board2TestCopy = moveHorizontal(y, x, offset, board2TestCopy);
                        int boardHash = getBoardHash(board2TestCopy);

                        // ONLY SAVE IF THERE WAS A MATCH/SCORE
                        SolverInterface.Score result = evalBoard(new SolverInterface.Score(0), board2TestCopy, true);
                        if (result.hasScore())
                        {
                            // CHECK IF BOARD PATTERN ALREADY EXIST. REDUCE DUPLICATE
                            SolverInterface.Movement testExist = returnThis.SingleOrDefault(s => s.boardHash == boardHash);
                            if (!testExist.score.hasScore())
                            {
                                returnThis.Add(new SolverInterface.Movement(x, y, false, offset, result, boardHash));
                            }
                        }
                        offset++;
                    }

                    /////////////////////
                    // VERTICAL
                    /////////////////////

                    // MOVE UP
                    offset = 1;
                    while (y - offset > -1)
                    {
                        // DEEP COPY
                        int[][] board2TestCopy = Array.ConvertAll(board2Test, a => (int[])a.Clone());

                        // MOVE
                        board2TestCopy = moveVertical(y, x, -1 * offset, board2TestCopy);
                        int boardHash = getBoardHash(board2TestCopy);

                        // ONLY SAVE IF THERE WAS A MATCH/SCORE
                        SolverInterface.Score result = evalBoard(new SolverInterface.Score(0), board2TestCopy, true);
                        if (result.hasScore())
                        {
                            // CHECK IF BOARD PATTERN ALREADY EXIST. REDUCE DUPLICATE
                            SolverInterface.Movement testExist = returnThis.SingleOrDefault(s => s.boardHash == boardHash);
                            if (!testExist.score.hasScore())
                            {
                                returnThis.Add(new SolverInterface.Movement(x, y, true, -1 * offset, result, boardHash));
                            }
                        }
                        offset++;
                    }

                    // MOVE DOWN
                    offset = 1;
                    while (y + offset < length)
                    {
                        // DEEP COPY
                        int[][] board2TestCopy = Array.ConvertAll(board2Test, a => (int[])a.Clone());

                        // MOVE
                        board2TestCopy = moveVertical(y, x, offset, board2TestCopy);
                        int boardHash = getBoardHash(board2TestCopy);

                        // ONLY SAVE IF THERE WAS A MATCH/SCORE
                        SolverInterface.Score result = evalBoard(new SolverInterface.Score(0), board2TestCopy, true);
                        if (result.hasScore())
                        {
                            // CHECK IF BOARD PATTERN ALREADY EXIST. REDUCE DUPLICATE
                            SolverInterface.Movement testExist = returnThis.SingleOrDefault(s => s.boardHash == boardHash);
                            if (!testExist.score.hasScore())
                            {
                                returnThis.Add(new SolverInterface.Movement(x, y, true, offset, result, boardHash));
                            }
                        }
                        offset++;
                    }

                    x++;
                }
                y++;
            }

            return returnThis;
        }

        /// <summary>
        /// Gets HashCode by Flattening to 1D Array and Joining to a String of Ints.
        /// </summary>
        /// <param name="board2TestCopy"></param>
        /// <returns></returns>
        private int getBoardHash(int[][] board2TestCopy)
        {
            int[] flattenedBoard = board2TestCopy.SelectMany(elem => elem).ToArray();
            String result = string.Join(string.Empty, flattenedBoard);
            return result.GetHashCode();
        }

        public int[][] moveHorizontal(int yPos, int xPos, int amount, int[][] board2Test)
        {
            // DEEP COPY
            int[][] board2TestCopy = Array.ConvertAll(board2Test, a => (int[])a.Clone());
            int newX = xPos + amount;
            
            // JUST RETURN IF NO MOVEMENT
            if (amount == 0)
            {
                return board2TestCopy;
            }
            else if (newX > width || newX < 0)
            {
                Console.WriteLine("Out of Bounds moving entity Horizontally: [" + yPos + "," + xPos + " " + amount + " positions");
                return board2TestCopy;
            }
            else
            {
                int[] line = board2TestCopy[yPos];
                int value = line[xPos];

                // CHECK DIRECTION, POSITIVE MEANS GOING RIGHT
                if (amount > 0)
                {
                    int count = 0;
                    while (count < amount)
                    {
                        // WHEN MOVING TARGET TO RIGHT, TILES THAT GET VISITED MOVES TO THE LEFT
                        line[xPos + count] = line[xPos + count + 1];
                        count++;
                    }
                }
                else
                {
                    int count = 0;
                    while (count < amount * -1)
                    {
                        // WHEN MOVING TARGET TO LEFT, TILES THAT GET VISITED MOVES TO THE RIGHT
                        line[xPos - count] = line[xPos - count - 1];
                        count++;
                    }
                }
                line[xPos + amount] = value;
                board2TestCopy[yPos] = line;
            }
            return board2TestCopy;
        }

        public int[][] moveVertical(int yPos, int xPos, int amount, int[][] board2Test)
        {
            // DEEP COPY
            int[][] board2TestCopy = Array.ConvertAll(board2Test, a => (int[])a.Clone());
            int newY = yPos + amount;

            // JUST RETURN IF NO MOVEMENT
            if (amount == 0)
            {
                return board2TestCopy;
            }
            else if (newY > length || newY < 0)
            {
                Console.WriteLine("Out of Bounds moving entity Vertically: [" + yPos + "," + xPos + " " + amount + " positions");
                return board2TestCopy;
            }
            else
            {
                int[][] boardCopy = board2TestCopy;
                int value = board2TestCopy[yPos][xPos];

                // CHECK DIRECTION, POSITIVE MEANS GOING DOWN
                if (amount > 0)
                {
                    int count = 0;
                    while (count < amount)
                    {
                        // WHEN MOVING TARGET DOWN, TILES THAT GET VISITED MOVES UP
                        boardCopy[yPos + count][xPos] = boardCopy[yPos + count + 1][xPos];
                        count++;
                    }
                }
                else
                {
                    int count = 0;
                    while (count < amount * -1)
                    {
                        // WHEN MOVING TARGET UP, TILES THAT GET VISITED MOVES DOWN
                        boardCopy[yPos - count][xPos] = boardCopy[yPos - count - 1][xPos];
                        count++;
                    }
                }

                boardCopy[yPos + amount][xPos] = value;
                board2TestCopy = boardCopy;
            }
            return board2TestCopy;
        }

        /// <summary>
        /// Recursive function to evaluate score of the board.
        /// First checks matches, then get the score to evaluate.
        /// Trigger gravity for tiles to drop down then recurse
        /// </summary>
        /// <param name="score"></param>
        /// <param name="board2Test"></param>
        /// <param name="initial"></param>
        /// <returns></returns>
        public SolverInterface.Score evalBoard(SolverInterface.Score score, int[][] board2Test, Boolean initial)
        {
            // DEEP COPY
            int[][] board2TestCopy = Array.ConvertAll(board2Test, a => (int[])a.Clone());
            int sum = 0;
            int index = 0;

            while (index < 63)
            {
                board2TestCopy = checkMatch(index / width, index % width, board2TestCopy[index / width][index % width] % 10, board2TestCopy);
                index++;
            }
            score = extractScore(score, board2TestCopy, initial);
            if (score.wasChanged)
            {
                if (!initial)
                {
                    score.chains++;
                }
                score.resetWasChanged();
                board2TestCopy = gravityFall(board2TestCopy);
                score = evalBoard(score, board2TestCopy, false);
            }
            return score;
        }

        /// <summary>
        /// Triggers tiles to fall down. Loops starts at the end of board.
        /// </summary>
        /// <param name="board2Test"></param>
        /// <returns></returns>
        private int[][] gravityFall(int[][] board2Test)
        {
            int x = width - 1;
            int y = length - 1;

            while (y > -1)
            {
                x = width - 1;
                while (x > -1)
                {
                    if (board2Test[y][x] == 9)
                    {
                        int countBlank = 1;
                        while ((y - countBlank > -1) && board2Test[y - countBlank][x] == 9)
                        {
                            countBlank++;
                        }
                        int countMove = y + 1 - countBlank;
                        while (countMove > 0)
                        {
                            board2Test[y][x] = board2Test[y - countBlank][x];
                            board2Test[y - countBlank][x] = 9;
                            countMove--;
                            y--;
                        }
                    }
                    x--;
                }
                y--;
            }
            return board2Test;
        }

        /// <summary>
        /// Checks if the 2 neighboring tiles to the left or down are the same.
        /// Marks them by adding 10 to the value.
        /// </summary>
        /// <param name="yPos"></param>
        /// <param name="xPos"></param>
        /// <param name="target"></param>
        /// <param name="board2Test"></param>
        /// <returns></returns>
        public int[][] checkMatch(int yPos, int xPos, int target, int[][] board2Test)
        {
            // IGNORE UNKNOWN TILES
            if (target == 9)
            {
                return board2Test;
            }
            if (xPos < width - 2)
            {
                if ((board2Test[yPos][xPos] % 10 == target) && (board2Test[yPos][xPos + 1] % 10 == target) && (board2Test[yPos][xPos + 2] % 10 == target))
                {
                    board2Test[yPos][xPos] += 10;
                    board2Test[yPos][xPos + 1] += 10;
                    board2Test[yPos][xPos + 2] += 10;
                }
            }
            if (yPos < length - 2)
            {
                if ((board2Test[yPos][xPos] % 10 == target) && (board2Test[yPos + 1][xPos] % 10 == target) && (board2Test[yPos + 2][xPos] % 10 == target))
                {
                    board2Test[yPos][xPos] += 10;
                    board2Test[yPos + 1][xPos] += 10;
                    board2Test[yPos + 2][xPos] += 10;
                }
            }

            return board2Test;

        }

        /// <summary>
        /// Adds score by checking values which are more than 10. Only tiles that got matched will have it's value > 10
        /// </summary>
        /// <param name="score"></param>
        /// <param name="board2Test"></param>
        /// <param name="initial"></param>
        /// <returns></returns>
        public SolverInterface.Score extractScore(SolverInterface.Score score, int[][] board2Test, Boolean initial)
        {
            int x = 0;
            int y = 0;

            while (y < length)
            {
                x = 0;
                while (x < width)
                {
                    int value = board2Test[y][x];
                    if (value > 10)
                    {
                        score.addScoreFromValue(board2Test[y][x]);
                        board2Test[y][x] = 9;
                        if (initial)
                        {
                            score.staminaCost++;
                        }
                    }
                    x++;
                }
                y++;
            }
            return score;
        }

        public int[][] parseImage(Image img)
        {
            int[][] board = new int[length][];
            List<System.Windows.Media.Color> extractedColor = new List<System.Windows.Media.Color>();

            using (Bitmap bmp = new Bitmap(img))
            {
                try
                {
                    int sizeWidth = bmp.Width;
                    int sizeLength = bmp.Height;
                    BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, sizeWidth, sizeLength), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                    int startX = (int)(sizeWidth * 0.3125);
                    int startY = (int)(sizeLength * 0.1553);
                    int offset = 180; // 180 FOR 4K

                    int x = 0;
                    int y = 0;
                    while (y < length)
                    {
                        x = 0;
                        board[y] = new int[width];
                        startX = (int)(sizeWidth * 0.3125);
                        while (x < width)
                        {
                            byte[] rgb = getPixel(startX, startY, sizeWidth, bitmapData);
                            board[y][x] = Array.IndexOf(rawColor, GetClosestColor(rawColor, System.Windows.Media.Color.FromArgb((byte)255, rgb[0], rgb[1], rgb[2])));
                            x++;
                            startX += offset;
                        }
                        y++;
                        startY += offset;
                    }
                    
                }
                catch (Exception e)
                {

                }
            }

            return board;
        }

        private static System.Windows.Media.Color GetClosestColor(System.Windows.Media.Color[] colorArray, System.Windows.Media.Color baseColor)
        {
            var colors = colorArray.Select(x => new { Value = x, Diff = GetDiffColor(x, baseColor) }).ToList();
            var min = colors.Min(x => x.Diff);
            return colors.Find(x => x.Diff == min).Value;
        }

        private static int GetDiffColor(System.Windows.Media.Color color, System.Windows.Media.Color baseColor)
        {
            int a = color.A - baseColor.A,
                r = color.R - baseColor.R,
                g = color.G - baseColor.G,
                b = color.B - baseColor.B;
            return a * a + r * r + g * g + b * b;
        }

        public byte[] getPixel(int x, int y, int maxWidth, BitmapData img)
        {
            int stride = maxWidth * 3;
            int pixel = 3 * x + stride * y;
            unsafe
            {
                byte* scan0 = (byte*)img.Scan0.ToPointer();
                byte B = scan0[pixel + 0];
                byte G = scan0[pixel + 1];
                byte R = scan0[pixel + 2];
                return new byte[] { R, G, B };
            }
        }

    }
}
