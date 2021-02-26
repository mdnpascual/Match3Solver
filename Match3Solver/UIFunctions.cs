using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        Dictionary<int, byte[]> arrowHeadV = new Dictionary<int, byte[]>();
        Dictionary<int, byte[]> arrowHeadH = new Dictionary<int, byte[]>();
        Dictionary<int, byte[]> arrowTailV = new Dictionary<int, byte[]>();
        Dictionary<int, byte[]> arrowTailH = new Dictionary<int, byte[]>();

        private MainWindow parent;

        public UIFunctions(MainWindow parent)
        {
            this.parent = parent;
            loadAssets();
        }

        private void loadAssets()
        {
            System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();
            arrowHeadV.Add(0, (byte[])converter.ConvertTo(System.Drawing.Image.FromStream(URIImage2Stream(new Uri(@"pack://application:,,,/Resources/bheartheadv.png"))), typeof(byte[])));
            arrowHeadH.Add(0, (byte[])converter.ConvertTo(System.Drawing.Image.FromStream(URIImage2Stream(new Uri(@"pack://application:,,,/Resources/bheartheadh.png"))), typeof(byte[])));
            arrowTailV.Add(0, (byte[])converter.ConvertTo(System.Drawing.Image.FromStream(URIImage2Stream(new Uri(@"pack://application:,,,/Resources/bhearttailv.png"))), typeof(byte[])));
            arrowTailH.Add(0, (byte[])converter.ConvertTo(System.Drawing.Image.FromStream(URIImage2Stream(new Uri(@"pack://application:,,,/Resources/bhearttailh.png"))), typeof(byte[])));
        }

        private MemoryStream URIImage2Stream(Uri path)
        {
            var bitmapImage = new BitmapImage(path);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapImage)bitmapImage));
            var stream = new MemoryStream();
            encoder.Save(stream);
            stream.Flush();

            return stream;
        }

        public Capture.Hook.Common.Overlay parseMovementAndDraw(SolverInterface.Movement command, int height, int width)
        {
            // CENTER = 1200X35 ON 3840X2160
            // PIXEL TO CHECK 1227X364 ON 3840X2160
            int startX = (int)(width * 0.2961);
            int startY = (int)(height * 0.12686);
            int offset = (int)(0.04688 * width); // 180 FOR 4K

            // -30, 0, 60 FOR 4K
            int directionOffsetX = command.isVertical ? (int)(-1 * (width * 0.0078125)) : (int)(width * 0.015625);
            // 50, 60, -20 FOR 4K
            int directionOffsetY = command.isVertical ? ((command.amount > 0) ? (int)(width * 0.01302083) : (int)(width * 0.015625)) : (int)(-1 * (width * 0.0052083));
            
            List <Capture.Hook.Common.IOverlayElement> elem = new List<Capture.Hook.Common.IOverlayElement>();

            // TAILS
            int i = command.amount > 0 ? command.amount - 1 : command.amount + 1;
            while(i != 0) //!= 0
            {
                int xOffset = command.isVertical ? 0 : (command.amount > 0) ? i-1 : i;
                int yOffset = command.isVertical ? (command.amount > 0) ? i - 1 : i : 0;

                elem.Add(new Capture.Hook.Common.ImageElement() { Location = new System.Drawing.Point((startX + (offset * command.xPos)) + (offset * xOffset) + directionOffsetX, (startY + (offset * command.yPos)) + (offset * yOffset) + directionOffsetY), Image = command.isVertical ? arrowTailV[0] : arrowTailH[0] });

                if (i >= 0) i--;
                else i++;
            }

            int finalX = command.isVertical ? command.xPos : command.xPos + command.amount;
            int finalY = command.isVertical ? command.yPos + command.amount : command.yPos;
            // -190, 7 FOR 4K
            int headOffsetX = command.isVertical ? 0 : ((command.amount > 0) ? (int)(-1 * (width * 0.049479167)) : (int)(width * 0.0018229167));
            // -188, 10, 3 FOR 4K
            int headOffsetY = command.isVertical ? ((command.amount > 0) ? (int)(-1 * (width * 0.0489584)) : (int)(width * 0.002604167)) : ((command.amount > 0) ? (int)(width * 0.00078125) : 1);

            // HEAD
            elem.Add(new Capture.Hook.Common.ImageElement() { Location = new System.Drawing.Point(startX + (offset * finalX) + directionOffsetX + headOffsetX, startY + (offset * finalY) + directionOffsetY + headOffsetY), Image = command.isVertical ? arrowHeadV[0] : arrowHeadH[0], Angle = (command.amount > 0) ? 3.14159f : 0.0f});

            return new Capture.Hook.Common.Overlay
            {
                Elements = elem,
                Hidden = false
            };
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
