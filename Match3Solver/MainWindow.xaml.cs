using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Documents;
using CrashReporterDotNET;
using System.Text;
using System.Linq;

namespace Match3Solver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window, SolverInterface
    {

        public int width = 9;
        public int length = 7;
        public int boxSize = 30;
        public int sortingMode = 1;

        public int[][] board = new int[7][];
        public Rectangle[][] boardDisplay = new Rectangle[7][];
        List<SolverInterface.Movement> results;
        public SolverUtils solver;
        public GameHook hook;
        public UIFunctions draw;
        public Boolean debugMode = false;

        private int lastScreenHeight = 0;
        private int lastScreenWidth = 0;
        private int selectedIndex = 0;

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
        private const uint VK_PLUS = 0xBB;
        private const uint VK_MINUS = 0xBD;
        private const uint VK_UP = 0x26;
        private const uint VK_DOWN = 0x28;
        private const uint VK_O = 0x4F;

        private IntPtr _windowHandle;
        private HwndSource _source;
        private static ReportCrash _reportCrash;

        public MainWindow()
        {
            InitializeComponent();
            hook = new GameHook(statusText);
            solver = new SolverUtils(length, width, boardDisplay);
            draw = new UIFunctions(this);
            board[0] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            board[1] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            board[2] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            board[3] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            board[4] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            board[5] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            board[6] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            draw.initBoardDisplay();
            results = solver.loopBoard(board);
            initCrashReporter();

        }

        private static void initCrashReporter()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                SendReport((Exception)args.ExceptionObject);
            };
            byte[] data = Convert.FromBase64String("enFhYy56cWh1K3puZ3B1M2ZieWlyZXBlbmZ1ZXJjYmVnQHR6bnZ5LnBieg==");
            string decodedString = Encoding.UTF8.GetString(data);
            byte[] data2 = Convert.FromBase64String("MnI1czM3NjctMG9xMC00NTFxLW4zcXEtcG9wcm44MzkwOHM5");
            string decodedString2 = Encoding.UTF8.GetString(data2);
            _reportCrash = new ReportCrash(String.Join("", decodedString.Select(x => char.IsLetter(x) ? (x >= 65 && x <= 77) || (x >= 97 && x <= 109) ? (char)(x + 13) : (char)(x - 13) : x)))
            {
                Silent = true,
                ShowScreenshotTab = true,
                IncludeScreenshot = false,
                #region Optional Configuration
                AnalyzeWithDoctorDump = true,
                DoctorDumpSettings = new DoctorDumpSettings
                {
                    ApplicationID = new Guid(String.Join("", decodedString2.Select(x => char.IsLetter(x) ? (x >= 65 && x <= 77) || (x >= 97 && x <= 109) ? (char)(x + 13) : (char)(x - 13) : x))),
                    OpenReportInBrowser = true
                }
                #endregion
            };
            _reportCrash.RetryFailedReports();
        }

        public static void SendReport(Exception exception, string developerMessage = "")
        {
            _reportCrash.DeveloperMessage = developerMessage;
            _reportCrash.Silent = false;
            _reportCrash.Send(exception);
        }

        public static void SendReportSilently(Exception exception, string developerMessage = "")
        {
            _reportCrash.DeveloperMessage = developerMessage;
            _reportCrash.Silent = true;
            _reportCrash.Send(exception);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            String errorString = "";

            errorString += !RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_I) ? "CTRL + ALT + I, " : ""; //CTRL + ALT + I
            errorString += !RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_C) ? "CTRL + ALT + C, " : "";  //CTRL + ALT + C
            errorString += !RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_1) ? "CTRL + ALT + 1, " : "";  //CTRL + ALT + 1
            errorString += !RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_2) ? "CTRL + ALT + 2, " : "";  //CTRL + ALT + 2
            errorString += !RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_3) ? "CTRL + ALT + 3, " : "";  //CTRL + ALT + 3
            errorString += !RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_4) ? "CTRL + ALT + 4, " : "";  //CTRL + ALT + 4
            errorString += !RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_5) ? "CTRL + ALT + 5, " : "";  //CTRL + ALT + 5
            errorString += !RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_6) ? "CTRL + ALT + 6, " : "";  //CTRL + ALT + 6
            errorString += !RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_7) ? "CTRL + ALT + 7, " : "";  //CTRL + ALT + 7
            errorString += !RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_8) ? "CTRL + ALT + 8, " : "";  //CTRL + ALT + 8
            errorString += !RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_9) ? "CTRL + ALT + 9, " : "";  //CTRL + ALT + 9
            errorString += !RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_0) ? "CTRL + ALT + 0, " : "";  //CTRL + ALT + 0
            errorString += !RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_MINUS) ? "CTRL + ALT + -, " : ""; //CTRL + ALT + -
            errorString += !RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_PLUS) ? "CTRL + ALT + +, " : ""; //CTRL + ALT + +
            errorString += !RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_UP) ? "CTRL + ALT + UP_ARROW, " : ""; //CTRL + ALT + UP_ARROW
            errorString += !RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_DOWN) ? "CTRL + ALT + DOWN_ARROW, " : ""; //CTRL + ALT + DOWN_ARROW
            errorString += !RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_O) ? "CTRL + ALT + O, " : ""; //CTRL + ALT + O

            if (!errorString.Equals(""))
            {
                MessageBox.Show("Cannot Bind Key Combinations: " + errorString.Remove(errorString.Length - 2), "BIND ERROR");
            }

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
                                    sortingMode = 10;
                                    draw.highLightMode("0 - Red First", rightTextBox, leftTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_1:
                                    sortingMode = 1;
                                    draw.highLightMode("1 - Chain First", leftTextBox, rightTextBox);
                                    updateResultView(results);
                                    resultListView.SelectedIndex = 0;
                                    break;
                                case VK_2:
                                    sortingMode = 2;
                                    draw.highLightMode("2 - TotalWB First", leftTextBox, rightTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_3:
                                    sortingMode = 3;
                                    draw.highLightMode("3 - 4/5 Match First", leftTextBox, rightTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_4:
                                    sortingMode = 4;
                                    draw.highLightMode("4 - Heart First", leftTextBox, rightTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_5:
                                    sortingMode = 5;
                                    draw.highLightMode("5 - Joy First", leftTextBox, rightTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_6:
                                    sortingMode = 6;
                                    draw.highLightMode("6 - Sentiment First", rightTextBox, leftTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_7:
                                    sortingMode = 7;
                                    draw.highLightMode("7 - Blue First", rightTextBox, leftTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_8:
                                    sortingMode = 8;
                                    draw.highLightMode("8 - Green First", rightTextBox, leftTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_9:
                                    sortingMode = 9;
                                    draw.highLightMode("9 - Orange First", rightTextBox, leftTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_MINUS:
                                    sortingMode = 11;
                                    draw.highLightMode("- - Stamina First", rightTextBox, leftTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_PLUS:
                                    sortingMode = 12;
                                    draw.highLightMode("+ - Broken Heart First", rightTextBox, leftTextBox);
                                    updateResultView(results);
                                    break;
                                case VK_UP:
                                    if(this.selectedIndex > 0)
                                    {
                                        this.selectedIndex--;
                                        resultListView.SelectedIndex = this.selectedIndex;
                                        resultListView.ScrollIntoView(resultListView.Items.GetItemAt(this.selectedIndex));
                                        hook.drawOverlay(draw.parseMovementAndDraw(results[this.selectedIndex], board[results[this.selectedIndex].yPos][results[this.selectedIndex].xPos], lastScreenHeight, lastScreenWidth));
                                    }
                                    break;
                                case VK_DOWN:
                                    if(this.selectedIndex < results.Count - 1)
                                    {
                                        this.selectedIndex++;
                                        resultListView.SelectedIndex = this.selectedIndex;
                                        resultListView.ScrollIntoView(resultListView.Items.GetItemAt(this.selectedIndex));
                                        hook.drawOverlay(draw.parseMovementAndDraw(results[this.selectedIndex], board[results[this.selectedIndex].yPos][results[this.selectedIndex].xPos], lastScreenHeight, lastScreenWidth));
                                    }
                                    break;
                                case VK_O:
                                    break;
                                case VK_I:
                                    hook.AttachProcess();
                                    break;
                                case VK_C:
                                    statusText.Text = "Capturing Screenshot";
                                    new Thread(() =>
                                    {
                                        System.Drawing.Bitmap screenshot = debugMode ? new System.Drawing.Bitmap(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\debug.png") : captureBoard();
                                        lastScreenHeight = screenshot.Height;
                                        lastScreenWidth = screenshot.Width;
                                        board = solver.parseImage(screenshot);
                                        screenshot.Dispose();
                                        GC.Collect();

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
                                                    updateResultView(results, lastScreenHeight, lastScreenWidth);
                                                    draw.drawBoard(board);

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

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, HOTKEY_ID);
            base.OnClosed(e);
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
            updateResultView(results, lastScreenHeight, lastScreenWidth);
        }

        private void updateResultView(List<SolverInterface.Movement> incomingList, int height, int width)
        {
            resultListView.Items.Clear();
            
            results = solver.sortList(incomingList, sortingMode);
            hook.drawOverlay(draw.parseMovementAndDraw(results[0], board[results[0].yPos][results[0].xPos], height, width));
            results.ForEach(result =>
            {
                resultListView.Items.Add(new resultItem(result));
            });
            selectedIndex = 0;
            resultListView.SelectedIndex = 0;

        }

        private void resultListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
