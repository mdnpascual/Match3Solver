using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Match3Solver
{
    public class UIFunctions
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

        private MainWindow parent;

        public UIFunctions(MainWindow parent)
        {
            this.parent = parent;
        }

        public void initBoardDisplay()
        {
            int xPos = 10;
            int yPos = 10;
            int x = 0;
            int y = 0;

            while (y < parent.length)
            {
                parent.boardDisplay[y] = new Rectangle[parent.width];
                xPos = 10;
                x = 0;
                while (x < parent.width)
                {
                    parent.boardDisplay[y][x] = createRectangle(xPos, yPos, myColors[parent.board[y][x] % 10]);
                    parent.mainGrid.Children.Add(parent.boardDisplay[y][x]);
                    xPos += parent.boxSize;
                    x++;
                }
                yPos += parent.boxSize;
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

            rect.Height = parent.boxSize;
            rect.Width = parent.boxSize;

            return rect;
        }

        public void drawBoard(int[][] board)
        {
            int x = 0;
            int y = 0;
            while (y < parent.length)
            {
                x = 0;
                while (x < parent.width)
                {
                    parent.boardDisplay[y][x].Fill = new SolidColorBrush(myColors[board[y][x] % 10]);
                    x++;
                }
                y++;
            }
        }

        public void highLightMode(String toSearch, RichTextBox rtb, RichTextBox other)
        {
            // RESET OTHER RICH TEXT BOX
            TextRange text2 = new TextRange(other.Document.ContentStart, other.Document.ContentEnd);
            text2.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
            text2.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

            // RESET CURRENT RICH TEXT BOX
            TextRange text = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            TextPointer current = text.Start.GetInsertionPosition(LogicalDirection.Forward);
            text.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
            text.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

            while (current != null)
            {
                string textInRun = current.GetTextInRun(LogicalDirection.Forward);
                if (!string.IsNullOrWhiteSpace(textInRun))
                {
                    int index = textInRun.IndexOf(toSearch);
                    if (index != -1)
                    {
                        TextPointer selectionStart = current.GetPositionAtOffset(index, LogicalDirection.Forward);
                        TextPointer selectionEnd = selectionStart.GetPositionAtOffset(toSearch.Length, LogicalDirection.Forward);
                        TextRange selection = new TextRange(selectionStart, selectionEnd);
                        selection.Text = toSearch;
                        selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                        selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                        rtb.Selection.Select(selection.Start, selection.End);
                        rtb.Focus();
                    }
                }
                current = current.GetNextContextPosition(LogicalDirection.Forward);
            }
        }
    }
}
