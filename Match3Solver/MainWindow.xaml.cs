using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Match3Solver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //0 - Broken heart
        //1 - Heart
        //2 - Stamina
        //3 - Sentiment
        //4 - Blue
        //5 - Red
        //6 - Green
        //7 - Gold
        //8 - Bell
        //9 - Unknown
        Dictionary<int, Color> myColors = new Dictionary<int, Color>
        {
            { 0, Colors.DarkViolet },
            { 1, Colors.HotPink },
            { 2, Colors.LightSlateGray },
            { 3, Colors.Cyan },
            { 4, Colors.Blue },
            { 5, Colors.Red },
            { 6, Colors.Lime },
            { 7, Colors.Goldenrod },
            { 8, Colors.Yellow },
            { 9, Colors.White }
        };

        public struct Score
        {
            public int BrokenHeart;
            public int Heart;
            public int Stamina;
            public int Sentiment;
            public int Blue;
            public int Red;
            public int Green;
            public int Gold;
            public int Bell;

            public Boolean wasChanged;

            public Score(int whyDoINeedThis)
            {
                this.BrokenHeart = 0;
                this.Heart = 0;
                this.Stamina = 0;
                this.Sentiment = 0;
                this.Blue = 0;
                this.Red = 0;
                this.Green = 0;
                this.Gold = 0;
                this.Bell = 0;
                wasChanged = false;
            }

            public void addScoreFromValue(int value)
            {
                wasChanged = true;
                switch (value % 10)
                {
                    case 0:
                        this.BrokenHeart++;
                        break;
                    case 1:
                        this.Heart++;
                        break;
                    case 2:
                        this.Stamina++;
                        break;
                    case 3:
                        this.Sentiment++;
                        break;
                    case 4:
                        this.Blue++;
                        break;
                    case 5:
                        this.Red++;
                        break;
                    case 6:
                        this.Green++;
                        break;
                    case 7:
                        this.Gold++;
                        break;
                    case 8:
                        this.Bell++;
                        break;
                }
            }

            public void resetWasChanged()
            {
                this.wasChanged = false;
            }
        }
        
        int width = 9;
        int length = 7;
        int boxSize = 30;

        public int[][] board = new int[7][];
        public Rectangle[][] boardDisplay = new Rectangle[7][];

        public MainWindow()
        {
            InitializeComponent();
            board[0] = new int[] { 6, 7, 4, 1, 7, 7, 6, 4, 1 };
            board[1] = new int[] { 2, 4, 3, 1, 4, 4, 5, 2, 3 };
            board[2] = new int[] { 2, 2, 3, 7, 4, 7, 1, 0, 5 };
            board[3] = new int[] { 5, 0, 5, 6, 1, 6, 1, 0, 6 };
            board[4] = new int[] { 6, 0, 0, 2, 3, 6, 5, 6, 5 };
            board[5] = new int[] { 3, 5, 3, 1, 1, 3, 1, 3, 5 };
            board[6] = new int[] { 3, 6, 2, 3, 8, 4, 5, 1, 4 };
            initBoardDisplay();
            //board = moveHorizontal(3, 2, 6);
            //board = moveVertical(2, 3, -2);
            board = moveVertical(4, 6, 1);
            Score score = new Score(0);
            evalBoard(score, board);
            drawBoard(board);
        }

        private void drawBoard(int[][] board)
        {
            int x = 0;
            int y = 0;
            while (y < length)
            {
                x = 0;
                while (x < width)
                {
                    boardDisplay[y][x].Fill = new SolidColorBrush(myColors[board[y][x] % 10]);
                    x++;
                }
                y++;
            }
        }

        private void initBoardDisplay()
        {
            int xPos = 10;
            int yPos = 10;
            int x = 0;
            int y = 0;

            while(y < length)
            {
                boardDisplay[y] = new Rectangle[width];
                xPos = 10;
                x = 0;
                while(x < width)
                {
                    boardDisplay[y][x] = createRectangle(xPos, yPos, myColors[board[y][x] % 10]);
                    mainGrid.Children.Add(boardDisplay[y][x]);
                    xPos += boxSize;
                    x++;
                }
                yPos += boxSize;
                y++;
            }
        }

        private Rectangle createRectangle(int xPos, int yPos, Color color)
        {
            Rectangle rect = new Rectangle();
            rect.Fill = new SolidColorBrush(color);
            
            rect.VerticalAlignment = VerticalAlignment.Top;
            rect.HorizontalAlignment = HorizontalAlignment.Left;

            rect.Margin = new Thickness(xPos, yPos, 0, 0);
            rect.Stroke = new SolidColorBrush(Colors.Black);

            rect.Height = boxSize;
            rect.Width = boxSize;

            return rect;
        }

        public int[][] moveHorizontal(int yPos, int xPos, int amount)
        {
            int newX = xPos + amount;
            if (amount == 0)
            {
                return board;
            }
            else if (newX > width || newX < 0)
            {
                Console.WriteLine("Out of Bounds moving entity Horizontally: [" + yPos + "," + xPos + " " + amount + " positions");
                return board;
            }
            else
            {
                int[] line = board[yPos];
                int value = line[xPos];

                // CHECK DIRECTION, POSITIVE MEANS GOING RIGHT
                if(amount > 0)
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
                board[yPos] = line;
            }
            return board;
        }

        public int[][] moveVertical(int yPos, int xPos, int amount)
        {
            int newY = yPos + amount;
            if(amount == 0)
            {
                return board;
            }
            else if(newY > length || newY < 0)
            {
                Console.WriteLine("Out of Bounds moving entity Vertically: [" + yPos + "," + xPos + " " + amount + " positions");
                return board;
            }
            else
            {
                int[][] boardCopy = board;
                int value = board[yPos][xPos];

                // CHECK DIRECTION, POSITIVE MEANS GOING DOWN
                if (amount > 0)
                {
                    int count = 0;
                    while(count < amount)
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
                board = boardCopy;
            }
            return board;
        }

        public void evalBoard(Score score, int[][] board2Test)
        {
            int[][] board2TestCopy = board2Test;
            int sum = 0;
            int index = 0;

            while(index < 63)
            {
                board2TestCopy = checkMatch(index / width, index % width, board2Test[index / width][index % width] % 10, board2Test);
                index++;
            }
            score = extractScore(score, board2TestCopy);
            if (score.wasChanged)
            {
                gravityFall(board2TestCopy);
                // RECURSE?
            }
            Console.WriteLine("halt");
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
                    if(board2Test[y][x] == 9)
                    {
                        int countBlank = 1;
                        while((y - countBlank > -1) && board2Test[y - countBlank][x] == 9)
                        {
                            countBlank++;
                        }
                        int countMove = y + 1 - countBlank;
                        while(countMove > 0)
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
            if(target == 9)
            {
                return board2Test;
            }
            if(xPos < width - 2)
            {
                if((board2Test[yPos][xPos] % 10 == target)&&(board2Test[yPos][xPos+1] % 10 == target) &&(board2Test[yPos][xPos+2] % 10 == target))
                {
                    board2Test[yPos][xPos] += 10;
                    board2Test[yPos][xPos+1] += 10;
                    board2Test[yPos][xPos+2] += 10;
                }
            }
            if(yPos < length - 2)
            {
                if((board2Test[yPos][xPos] % 10 == target) &&(board2Test[yPos+1][xPos] % 10 == target) &&(board2Test[yPos+2][xPos] % 10 == target))
                {
                    board2Test[yPos][xPos] += 10;
                    board2Test[yPos+1][xPos] += 10;
                    board2Test[yPos+2][xPos] += 10;
                }
            }

            return board2Test;

        }

        public Score extractScore(Score score, int[][] board2Test)
        {
            int x = 0;
            int y = 0;

            while (y < length)
            {
                x = 0;
                while (x < width)
                {
                    int value = board2Test[y][x];
                    if(value > 10)
                    {
                        score.addScoreFromValue(board2Test[y][x]);
                        board2Test[y][x] = 9;
                    }
                    x++;
                }
                y++;
            }
            return score;
        }
    }
}
