﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3Solver
{
    public class SolverUtils : SolverInterface
    {
        int length;
        int width;
        public SolverUtils(int length, int width)
        {
            this.length = length;
            this.width = width;
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

        private int getBoardHash(int[][] board2TestCopy)
        {
            int[] flattenedBoard = board2TestCopy.SelectMany(elem => elem).ToArray();
            String result = string.Join(string.Empty, flattenedBoard);
            return result.GetHashCode();
            //int boardHash = ((System.Collections.IStructuralEquatable)flattenedBoard).GetHashCode(EqualityComparer<int>.Default);
            //throw new NotImplementedException();
        }

        public int[][] moveHorizontal(int yPos, int xPos, int amount, int[][] board2Test)
        {
            // DEEP COPY
            int[][] board2TestCopy = Array.ConvertAll(board2Test, a => (int[])a.Clone());
            int newX = xPos + amount;
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
                        line[xPos + count] = line[xPos + count + 1];
                        count++;
                    }
                }
                else
                {
                    int count = 0;
                    while (count < amount * -1)
                    {
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
                        boardCopy[yPos + count][xPos] = boardCopy[yPos + count + 1][xPos];
                        count++;
                    }
                }
                else
                {
                    int count = 0;
                    while (count < amount * -1)
                    {
                        boardCopy[yPos - count][xPos] = boardCopy[yPos - count - 1][xPos];
                        count++;
                    }
                }

                boardCopy[yPos + amount][xPos] = value;
                board2TestCopy = boardCopy;
            }
            return board2TestCopy;
        }

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

    }
}