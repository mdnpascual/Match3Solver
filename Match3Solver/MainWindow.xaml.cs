using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Threading;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Documents;
//using System.Drawing;

namespace Match3Solver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window, SolverInterface
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

        int width = 9;
        int length = 7;
        int boxSize = 30;
        int sortingMode = 1;

        public int[][] board = new int[7][];
        public Rectangle[][] boardDisplay = new Rectangle[7][];
        List<SolverInterface.Movement> results;
        public SolverUtils solver;
        public GameHook hook;

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 9000;

        //Modifiers:
        private const uint MOD_NONE = 0x0000; //(none)
        private const uint MOD_ALT = 0x0001; //ALT
        private const uint MOD_CONTROL = 0x0002; //CTRL
        private const uint MOD_SHIFT = 0x0004; //SHIFT
        private const uint MOD_WIN = 0x0008; //WINDOWS

        private const uint VK_I = 0x49;
        private const uint VK_C = 0x43;
        private const uint VK_0 = 0x30;
        private const uint VK_1 = 0x31;
        private const uint VK_2 = 0x32;
        private const uint VK_3 = 0x33;
        private const uint VK_4 = 0x34;
        private const uint VK_5 = 0x35;
        private const uint VK_6 = 0x36;
        private const uint VK_7 = 0x37;
        private const uint VK_8 = 0x38;
        private const uint VK_9 = 0x39;

        private IntPtr _windowHandle;
        private HwndSource _source;

        public MainWindow()
        {
            InitializeComponent();
            hook = new GameHook(statusText);
            solver = new SolverUtils(length, width, boardDisplay);
            board[0] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            board[1] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            board[2] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            board[3] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            board[4] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            board[5] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            board[6] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            initBoardDisplay();
            results = solver.loopBoard(board);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            Console.WriteLine(RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_I)); //CTRL + ALT + I
            Console.WriteLine(RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_C)); //CTRL + ALT + C
            Console.WriteLine(RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_1)); //CTRL + ALT + 1
            Console.WriteLine(RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_2)); //CTRL + ALT + 2
            Console.WriteLine(RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_3)); //CTRL + ALT + 3
            Console.WriteLine(RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_4)); //CTRL + ALT + 4
            Console.WriteLine(RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_5)); //CTRL + ALT + 5
            Console.WriteLine(RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_6)); //CTRL + ALT + 6
            Console.WriteLine(RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_7)); //CTRL + ALT + 7
            Console.WriteLine(RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_8)); //CTRL + ALT + 8
            Console.WriteLine(RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_9)); //CTRL + ALT + 9
            Console.WriteLine(RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_0)); //CTRL + ALT + 0
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            switch ((uint)vkey)
                            {
                                case VK_0:
                                    sortingMode = 0;
                                    highLightMode("0 - Broken Heart First", rightTextBox, leftTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_1:
                                    sortingMode = 1;
                                    highLightMode("1 - Chain First", leftTextBox, rightTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_2:
                                    sortingMode = 2;
                                    highLightMode("2 - TotalWB First", leftTextBox, rightTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_3:
                                    sortingMode = 3;
                                    highLightMode("3 - Heart First", leftTextBox, rightTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_4:
                                    sortingMode = 4;
                                    highLightMode("4 - Joy First", leftTextBox, rightTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_5:
                                    sortingMode = 5;
                                    highLightMode("5 - Sentiment First", rightTextBox, leftTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_6:
                                    sortingMode = 6;
                                    highLightMode("6 - Blue First", rightTextBox, leftTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_7:
                                    sortingMode = 7;
                                    highLightMode("7 - Green First", rightTextBox, leftTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_8:
                                    sortingMode = 8;
                                    highLightMode("8 - Orange First", rightTextBox, leftTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_9:
                                    sortingMode = 9;
                                    highLightMode("9 - Red First", rightTextBox, leftTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_I:
                                    hook.AttachProcess();
                                    break;
                                case VK_C:
                                    statusText.Text = "Capturing Screenshot";

                                    new Thread(() =>
                                    {
                                        board = solver.parseImage(captureBoard());

                                        Dispatcher.BeginInvoke((Action)(() =>
                                        {
                                           statusText.Foreground = new SolidColorBrush(Colors.IndianRed);
                                           statusText.Text = "Screenshot Parsed. Solving Board...";
                                        }));

                                        new Thread(() =>
                                        {
                                            results.Clear();
                                            results = solver.loopBoard(board);

                                            Dispatcher.BeginInvoke((Action)(() =>
                                            {
                                                statusText.Foreground = new SolidColorBrush(Colors.Goldenrod);
                                                statusText.Text = "Board Solved. Updating Legal Moves...";
                                            }));

                                            new Thread(() =>
                                            {
                                                Dispatcher.BeginInvoke((Action)(() =>
                                                {
                                                    updateResultView(results);
                                                    drawBoard(board);

                                                    statusText.Foreground = new SolidColorBrush(Colors.LimeGreen);
                                                    statusText.Text = "Done!";
                                                }));
                                                
                                            }).Start();

                                        }).Start();

                                    }).Start();                                    

                                    break;
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void highLightMode(String toSearch, RichTextBox rtb, RichTextBox other)
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

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, HOTKEY_ID);
            base.OnClosed(e);
        }

        private IntPtr findHuniePopWindow()
        {
            IntPtr targetWindow = FindWindow(null, "HuniePop 2 - Double Date");
            if (targetWindow == IntPtr.Zero)
            {
                statusText.Foreground = new SolidColorBrush(Colors.Red);
                statusText.Text = "Game not Found!";
            }
            else
            {
                statusText.Foreground = new SolidColorBrush(Colors.Green);
                statusText.Text = "Injected to Game";
            }
            return targetWindow;
        }

        private System.Drawing.Bitmap captureBoard()
        {
            if(hook.hooked)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    statusText.Foreground = new SolidColorBrush(Colors.Black);
                }));
                
                return hook.getScreenshot();
            }
            else
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    statusText.Foreground = new SolidColorBrush(Colors.Red);
                    statusText.Text = "Game not Injected!";
                }));
                
                return null;
            }
            
        }

        private void updateResultView(List<SolverInterface.Movement> results)
        {
            resultListView.Items.Clear();
            
            results = solver.sortList(results, sortingMode);
            results.ForEach(result =>
            {
                resultListView.Items.Add(new resultItem(result));
            });

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

        private void resultListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // DEEP COPY
            int[][] board2Test = Array.ConvertAll(board, a => (int[])a.Clone());
            resultItem selectedItem = (resultItem)e.AddedItems[0];
            if (selectedItem.isVertical)
            {
                board2Test = solver.moveVertical(selectedItem.yPos, selectedItem.xPos, selectedItem.Amount, board2Test);
            }
            else
            {
                board2Test = solver.moveHorizontal(selectedItem.yPos, selectedItem.xPos, selectedItem.Amount, board2Test);
            }
            drawBoard(board2Test);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            hook.AttachProcess();
        }
    }
}
