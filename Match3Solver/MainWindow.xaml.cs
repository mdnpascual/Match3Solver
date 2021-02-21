﻿using System;
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

        public int[][] board = new int[7][];
        public Rectangle[][] boardDisplay = new Rectangle[7][];
        public SolverUtils solver;
        public IntPtr huniePopWindow;
        public GameCapture capture;

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
            capture = new GameCapture();
            solver = new SolverUtils(length, width, boardDisplay);

            //board[0] = new int[] { 6, 7, 4, 1, 7, 7, 6, 4, 1 };
            //board[1] = new int[] { 2, 4, 3, 1, 4, 4, 5, 2, 3 };
            //board[2] = new int[] { 2, 2, 3, 7, 4, 7, 1, 0, 5 };
            //board[3] = new int[] { 5, 0, 5, 6, 1, 6, 1, 0, 6 };
            //board[4] = new int[] { 6, 0, 0, 2, 3, 6, 5, 6, 5 };
            //board[5] = new int[] { 3, 5, 3, 1, 1, 3, 1, 3, 5 };
            //board[6] = new int[] { 3, 6, 2, 3, 8, 4, 5, 1, 4 };
            board[0] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            board[1] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            board[2] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            board[3] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            board[4] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            board[5] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            board[6] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            initBoardDisplay();
            //List<SolverInterface.Movement> results = solver.loopBoard(board);
            //updateResultView(results);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            Console.WriteLine(RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_I)); //CTRL + ALT + I
            RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_C); //CTRL + ALT + C
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
                                case VK_I:
                                    huniePopWindow = findHuniePopWindow();
                                    break;
                                case VK_C:
                                    //System.Drawing.Image newImage = System.Drawing.Image.FromFile("C:\\Users\\mdnpm\\Desktop\\4K.png");
                                    //drawBoard(solver.parseImage(newImage));
                                    board = solver.parseImage(captureBoard(huniePopWindow));
                                    updateResultView(solver.loopBoard(board));
                                    drawBoard(board);

                                    break;
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
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

        private System.Drawing.Image captureBoard(IntPtr targetWindow)
        {
            if(targetWindow != IntPtr.Zero)
            {
                statusText.Foreground = new SolidColorBrush(Colors.Black);
                statusText.Text = "Screenshot taken";
                capture.CaptureWindowToFile(targetWindow, "C:\\Users\\mdnpm\\Desktop\\breh.png", ImageFormat.Png);
                return capture.CaptureWindow(targetWindow);
            }
            else
            {
                statusText.Foreground = new SolidColorBrush(Colors.Red);
                statusText.Text = "Game not Injected!";
                return null;
            }
            
        }

        private void updateResultView(List<SolverInterface.Movement> results)
        {
            resultListView.Items.Clear();
            results.Clear();

            GridViewColumn Position = new GridViewColumn();
            Position.Header = "Pos(Y,X)";
            Position.DisplayMemberBinding = new Binding("Position");
            GridViewColumn Amount = new GridViewColumn();
            Amount.Header = "Amt";
            Amount.DisplayMemberBinding = new Binding("Amount");
            GridViewColumn Chain = new GridViewColumn();
            Chain.Header = "Chain";
            Chain.DisplayMemberBinding = new Binding("Chain");
            GridViewColumn StaminaCost = new GridViewColumn();
            StaminaCost.Header = "Cost";
            StaminaCost.DisplayMemberBinding = new Binding("StaminaCost");
            GridViewColumn TotalScore = new GridViewColumn();
            TotalScore.Header = "Total";
            TotalScore.DisplayMemberBinding = new Binding("Total");
            GridViewColumn TotalWBroken = new GridViewColumn();
            TotalWBroken.Header = "TotWBroken";
            TotalWBroken.DisplayMemberBinding = new Binding("TotalWBroken");
            GridViewColumn sHeart = new GridViewColumn();
            sHeart.Header = "SHeart";
            sHeart.DisplayMemberBinding = new Binding("sHeart");
            GridViewColumn sStam = new GridViewColumn();
            sStam.Header = "SStam";
            sStam.DisplayMemberBinding = new Binding("sStam");
            GridViewColumn sSent = new GridViewColumn();
            sSent.Header = "SSent";
            sSent.DisplayMemberBinding = new Binding("sSent");
            GridViewColumn sBlue = new GridViewColumn();
            sBlue.Header = "SBlue";
            sBlue.DisplayMemberBinding = new Binding("sBlue");
            GridViewColumn sRed = new GridViewColumn();
            sRed.Header = "SRed";
            sRed.DisplayMemberBinding = new Binding("sRed");
            GridViewColumn sGreen = new GridViewColumn();
            sGreen.Header = "SGreen";
            sGreen.DisplayMemberBinding = new Binding("sGreen");
            GridViewColumn sGold = new GridViewColumn();
            sGold.Header = "SGold";
            sGold.DisplayMemberBinding = new Binding("sGold");
            GridViewColumn sBell = new GridViewColumn();
            sBell.Header = "SBell";
            sBell.DisplayMemberBinding = new Binding("sBell");
            GridViewColumn sBHeart = new GridViewColumn();
            sBHeart.Header = "SBHeart";
            sBHeart.DisplayMemberBinding = new Binding("sBHeart");
            
            resultGridView.Columns.Add(Position);
            resultGridView.Columns.Add(Amount);
            resultGridView.Columns.Add(Chain);
            resultGridView.Columns.Add(StaminaCost);
            resultGridView.Columns.Add(TotalScore);
            resultGridView.Columns.Add(TotalWBroken);
            resultGridView.Columns.Add(sHeart);
            resultGridView.Columns.Add(sStam);
            resultGridView.Columns.Add(sSent);
            resultGridView.Columns.Add(sBlue);
            resultGridView.Columns.Add(sRed);
            resultGridView.Columns.Add(sGreen);
            resultGridView.Columns.Add(sGold);
            resultGridView.Columns.Add(sBell);
            resultGridView.Columns.Add(sBHeart);

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
            huniePopWindow = findHuniePopWindow();
        }
    }
}
