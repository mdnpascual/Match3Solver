using Capture.Hook;
using Capture;
using System;
using System.Diagnostics;
using Capture.Interface;
using System.Threading;
using System.Windows.Controls;

using Bitmap = System.Drawing.Bitmap;
using Rectangle = System.Drawing.Rectangle;

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

        public GameHook(TextBlock statusMessage)
        {
            this.message = statusMessage;
        }

        public void AttachProcess()
        {

            Process[] processes = Process.GetProcessesByName("HuniePop 2 - Double Date");

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
                    ShowOverlay = true
                };

                processId = process.Id;
                _process = process;

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

        }

        public Bitmap getScreenshot()
        {
            _captureProcess.BringProcessWindowToFront();
            return _captureProcess.CaptureInterface.GetScreenshot(Rectangle.Empty, new TimeSpan(0,0,3), null, ImageFormat.Png).ToBitmap();
        }

        public void drawOverlay(Capture.Hook.Common.Overlay items)
        {
            if (hooked)
            {
                _captureProcess.CaptureInterface.DrawOverlayInGame(items);
            }
        }

        /// <summary>
        /// Display messages from the target process
        /// </summary>
        /// <param name="message"></param>
        void CaptureInterface_RemoteMessage(MessageReceivedEventArgs message)
        {
            Console.WriteLine(message.Message);
        }

    }
}
