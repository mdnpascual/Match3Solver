using Capture.Hook;
using Capture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Capture.Interface;
using System.Threading;
using System.Windows.Controls;
using System.Drawing;

using SharpDX.Direct2D1;
using Factory = SharpDX.Direct2D1.Factory;
using FontFactory = SharpDX.DirectWrite.Factory;
using Format = SharpDX.DXGI.Format;
using SharpDX;
using SharpDX.DirectWrite;
using Bitmap = System.Drawing.Bitmap;
using Rectangle = System.Drawing.Rectangle;
using Color = SharpDX.Color;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;
using FontStyle = SharpDX.DirectWrite.FontStyle;
using RectangleF = SharpDX.RectangleF;

using SharpDX.Direct3D11;

namespace Match3Solver
{
    public class GameHook
    {
        int processId = 0;
        Process _process;
        CaptureProcess _captureProcess;
        public Boolean hooked = false;

        TextBlock message;

        private Thread sDX = null;
        private WindowRenderTarget device;
        private HwndRenderTargetProperties renderProperties;
        private Factory factory;
        private FontFactory fontFactory;
        private IntPtr targetWindow;
        private TextFormat textFormat;

        public GameHook(TextBlock statusMessage)
        {
            this.message = statusMessage;
        }

        public void AttachProcess()
        {

            //Process[] processes = Process.GetProcessesByName("HuniePop 2 - Double Date");
            Process[] processes = Process.GetProcessesByName("vlc");
            foreach (Process process in processes)
            {
                // Simply attach to the first one found.

                // If the process doesn't have a mainwindowhandle yet, skip it (we need to be able to get the hwnd to set foreground etc)
                if (process.MainWindowHandle == IntPtr.Zero)
                {
                    continue;
                }

                // Skip if the process is already hooked (and we want to hook multiple applications)
                if (HookManager.IsHooked(process.Id))
                {
                    continue;
                }

                Direct3DVersion direct3DVersion = Direct3DVersion.AutoDetect;

                CaptureConfig cc = new CaptureConfig()
                {
                    Direct3DVersion = direct3DVersion,
                    ShowOverlay = false
                };

                processId = process.Id;
                _process = process;
                targetWindow = process.MainWindowHandle;

                var captureInterface = new CaptureInterface();
                captureInterface.RemoteMessage += new MessageReceivedEvent(CaptureInterface_RemoteMessage);
                _captureProcess = new CaptureProcess(process, cc, captureInterface);
                
                break;
            }
            Thread.Sleep(10);

            if (_captureProcess == null)
            {
                this.message.Text = "No executable found matching: 'HuniePop 2 - Double Date'";
                hooked = false;
            }
            else
            {
                this.message.Text = "Attached to game";
                hooked = true;
            }

            //////////////
            factory = new Factory();
            fontFactory = new FontFactory();
            textFormat = new TextFormat(fontFactory, "Arial",
                FontWeight.Normal, FontStyle.Normal, 32.0f)
            {
                TextAlignment = TextAlignment.Center,
                ParagraphAlignment = ParagraphAlignment.Center
            };
            renderProperties = new HwndRenderTargetProperties()
            {
                Hwnd = this.targetWindow,
                PixelSize = new Size2(800, 600),
                PresentOptions = PresentOptions.None,
            };

            //Init DirectX
            device = new WindowRenderTarget(factory, new RenderTargetProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)), renderProperties);
            sDX = new Thread(new ParameterizedThreadStart(sDXThread));
            sDX.Priority = ThreadPriority.Highest;
            sDX.IsBackground = true;
            sDX.Start();
            //////////////

            //SharpDX.Direct3D11.Device device11 = new SharpDX.Direct3D11.Device(this.targetWindow);
        }

        public Bitmap getScreenshot()
        {
            _captureProcess.BringProcessWindowToFront();
            return _captureProcess.CaptureInterface.GetScreenshot(Rectangle.Empty, new TimeSpan(0,0,3), null, ImageFormat.Png).ToBitmap();
        }

        /// <summary>
        /// Display messages from the target process
        /// </summary>
        /// <param name="message"></param>
        void CaptureInterface_RemoteMessage(MessageReceivedEventArgs message)
        {
            Console.WriteLine(message.Message);
        }

        private void sDXThread(object sender)
        {
            while (true)
            {
                device.BeginDraw();
                device.Clear(Color.Transparent);
                device.TextAntialiasMode = TextAntialiasMode.Aliased;// you can set another text mode

                //device.DrawRectangle(new RectangleF(0, 0, 400, 400), new SolidColorBrush(device, Color.Pink));
                device.DrawText("LKJDSAGHFKLJASGDFKAGSDKFASDF", textFormat, new RectangleF(0, 0, 400, 400), new SolidColorBrush(device, Color.Pink));

                //place your rendering things here

                device.EndDraw();
            }

            //whatever you want
        }
    }
}
