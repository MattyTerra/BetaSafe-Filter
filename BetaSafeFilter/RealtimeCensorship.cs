using NsfwSharp;
using OpenCvSharp;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Graphics.Imaging;
using WinRT;
using WinRT.Interop;


namespace BetaSafeFilter
{


    internal class RealtimeCensorship
    {
        private static GraphicsCaptureItem? _captureItem;
        private static Direct3D11CaptureFramePool? _framePool;
        private static GraphicsCaptureSession? _session;
        private static CaptureOverlay? _overlay;
        private static IDirect3DDevice? _device;
        private static int _frameCount = 0;
        private static NsfwAnalyzer _Analysizer;
        private static int _busy= 0;

        public static bool WGCworks()
        {
            //makes sure that WGC is an option for the user
            bool supported = GraphicsCaptureSession.IsSupported();
            Debug.WriteLine($"WGC supported: {supported}");

            return supported;
        }


        //This makes the code select which window or monitor the user wants to track for censorship purposes
        public static Screen? StartCaptureOnScreen(Screen screen,CaptureOverlay overlay,NsfwAnalyzer Analyzer)
        {
            _Analysizer= Analyzer;
            //start by making sure WGC is supported
            if (WGCworks() == false)
            {
                Debug.WriteLine("WGC not supported on this machine");
                return null;
            }

            //if any capture is currently running; stop it
            Stop();
            _overlay = overlay;
            try
            {
                IntPtr hWmonitor = MonitorInterop.GetHMonitor(screen);
                _captureItem = CaptureItemFactory.CreateForMonitor(hWmonitor);


                Debug.WriteLine($"Capturing monitor: {screen.DeviceName}  {screen.Bounds}");

                _device = CreateDirect3DDevice();

                _framePool = Direct3D11CaptureFramePool.CreateFreeThreaded(
                    _device,
                    DirectXPixelFormat.B8G8R8A8UIntNormalized,
                    2,
                    _captureItem.Size);

                _framePool.FrameArrived += OnFrameArrived;

                _session = _framePool.CreateCaptureSession(_captureItem);
                _session.IsCursorCaptureEnabled = false;
                _session.StartCapture();

                return screen;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to start capture: {ex.Message}");
                Stop();
                return null;
            }


        }
        private static void OnFrameArrived(Direct3D11CaptureFramePool sender, object args)
        {
            using var frame = sender.TryGetNextFrame();
            if (frame == null) return;

            if (Interlocked.CompareExchange(ref _busy, 1, 0) != 0)
                return;

            try
            {
                _frameCount++;
                if (_frameCount % 9 != 0) return;
                //softwareBMP = SoftwareBitmap.CreateCopyFromSurfaceAsync(frame.Surface);

                using var softwareBMP = SoftwareBitmap
                    .CreateCopyFromSurfaceAsync(frame.Surface)
                    .AsTask().GetAwaiter().GetResult();

                using var skImage = ConvertToSKImage(softwareBMP);

                var filler = _Analysizer.GetNsfwAnalysis(skImage, null);
                _overlay?.SetBoxes(filler.BoundingBoxes);
                
                //softwareBMP?.Dispose();
                //skImage?.Dispose();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Conversion failed: {ex.Message}");
            }
            finally
            {
                Interlocked.Exchange(ref _busy, 0);
                
            }

        }


        private static byte[]? _pixelBuffer;

        private static SKImage ConvertToSKImage(SoftwareBitmap softwareBitmap)
        {
            SoftwareBitmap? converted = null;
            var source = softwareBitmap;

            if (source.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                source.BitmapAlphaMode != BitmapAlphaMode.Premultiplied)
            {
                converted = SoftwareBitmap.Convert(
                    source,
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied);
                source = converted;
            }

            try
            {
                int width = source.PixelWidth;
                int height = source.PixelHeight;
                int needed = width * height * 4;

                if (_pixelBuffer == null || _pixelBuffer.Length < needed)
                    _pixelBuffer = new byte[needed];

                var winrtBuffer = _pixelBuffer.AsBuffer();
                try
                {
                    source.CopyToBuffer(winrtBuffer);
                }
                finally
                {
                    if (winrtBuffer is IDisposable disposable)
                        disposable.Dispose();
                }

                var info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
                return SKImage.FromPixelCopy(info, _pixelBuffer, width * 4);
            }
            finally
            {
                converted?.Dispose();
            }
        }
        private static async Task<SKImage?> ConvertToSKImageAsync(SoftwareBitmap softwareBitmap)
        {
            // Ensure we have BGRA8 + Premultiplied (Skia works well with this)
            if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                softwareBitmap.BitmapAlphaMode != BitmapAlphaMode.Premultiplied)
            {
                softwareBitmap = SoftwareBitmap.Convert(
                    softwareBitmap,
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied);
            }

            int width = softwareBitmap.PixelWidth;
            int height = softwareBitmap.PixelHeight;

            // Copy pixels into a byte array
            byte[] pixels = new byte[width * height * 4];
            softwareBitmap.CopyToBuffer(pixels.AsBuffer());

            // Create SKImage directly from the pixel data
            var info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);

            // FromPixelCopy makes its own copy of the data, which is safe
            SKImage image = SKImage.FromPixelCopy(info, pixels);

            return image;
        }

        // ---------------------------------------------------------------------
        // Helper: Create an IDirect3DDevice from a Win32 D3D11 device
        // ---------------------------------------------------------------------
        private static IDirect3DDevice CreateDirect3DDevice()
        {
            // This uses the official interop path.
            // We create a regular D3D11 device and then wrap it as IDirect3DDevice.

            // Note: For production you may want to use SharpDX / Vortice / CsWin32
            // for more control. This minimal version works for testing.

            var d3dDevice = Direct3D11Helper.CreateDevice();
            return Direct3D11Helper.CreateDirect3DDeviceFromD3D11Device(d3dDevice);
        }

        public static void Stop()
        {
            _session?.Dispose();
            _session = null;

            _framePool?.Dispose();
            _framePool = null;

            _captureItem = null;
            _frameCount = 0;
        }
    }

    internal static class Direct3D11Helper
    {
        //I have absolutely no idea what any of this is You an thank GROK for this monstronsity
        [System.Runtime.InteropServices.DllImport("d3d11.dll", EntryPoint = "D3D11CreateDevice", CharSet = System.Runtime.InteropServices.CharSet.Unicode, ExactSpelling = true, PreserveSig = true)]
        private static extern int D3D11CreateDevice(
            IntPtr pAdapter,
            int DriverType,
            IntPtr Software,
            uint Flags,
            int[] pFeatureLevels,
            int FeatureLevels,
            uint SDKVersion,
            out IntPtr ppDevice,
            out int pFeatureLevel,
            out IntPtr ppImmediateContext);

        private const int D3D_DRIVER_TYPE_HARDWARE = 1;
        private const uint D3D11_CREATE_DEVICE_BGRA_SUPPORT = 0x20;
        private const uint D3D11_SDK_VERSION = 7;

        public static IntPtr CreateDevice()
        {
            int hr = D3D11CreateDevice(
                IntPtr.Zero,
                D3D_DRIVER_TYPE_HARDWARE,
                IntPtr.Zero,
                D3D11_CREATE_DEVICE_BGRA_SUPPORT,
                null, 0,
                D3D11_SDK_VERSION,
                out IntPtr device,
                out _,
                out _);

            if (hr < 0)
                throw new Exception($"D3D11CreateDevice failed (HRESULT: 0x{hr:X8})");

            return device;
        }

        public static IDirect3DDevice CreateDirect3DDeviceFromD3D11Device(IntPtr d3dDevice)
        {
            // This is the standard WinRT interop call
            var inspectable = CreateDirect3D11DeviceFromDXGIDevice(d3dDevice);
            return (IDirect3DDevice)inspectable;
        }

        [System.Runtime.InteropServices.DllImport(
            "d3d11.dll",
            EntryPoint = "CreateDirect3D11DeviceFromDXGIDevice",
            SetLastError = true,
            CharSet = System.Runtime.InteropServices.CharSet.Unicode,
            ExactSpelling = true,
            PreserveSig = true)]
        private static extern int CreateDirect3D11DeviceFromDXGIDevice(
            IntPtr dxgiDevice,
            out IntPtr graphicsDevice);

        private static object CreateDirect3D11DeviceFromDXGIDevice(IntPtr dxgiDevice)
        {
            int hr = CreateDirect3D11DeviceFromDXGIDevice(dxgiDevice, out IntPtr pUnknown);
            if (hr < 0)
                throw new Exception($"CreateDirect3D11DeviceFromDXGIDevice failed (HRESULT: 0x{hr:X8})");

            var obj = WinRT.MarshalInspectable<object>.FromAbi(pUnknown);
            WinRT.MarshalInspectable<object>.DisposeAbi(pUnknown); // we already have a managed reference
            return obj;
        }
    }

    public class CaptureOverlay : Form
    {
        //chatGPT gave me this shit. dont ask me how it works rn.
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;



        [DllImport("user32.dll")]

        private static extern bool SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

        private readonly List<Rectangle> _boxes = new();
        private readonly object _lock = new();
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                cp.ExStyle |= WS_EX_TRANSPARENT;
                cp.ExStyle |= WS_EX_LAYERED;
                cp.ExStyle |= WS_EX_NOACTIVATE;

                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTTRANSPARENT = -1;

            if (m.Msg == WM_NCHITTEST)
            {
                m.Result = (IntPtr)HTTRANSPARENT;
                return;
            }

            base.WndProc(ref m);
        }

        public CaptureOverlay(Screen screen)
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            ShowInTaskbar = false;
            DoubleBuffered = true;

            Bounds = screen.Bounds;
            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;


            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            bool ok = SetWindowDisplayAffinity(Handle, WDA_EXCLUDEFROMCAPTURE);
            Debug.WriteLine($"ExcludeFromCapture: {ok}");
        }
        public void SetBoxes(List <SKRectI> BoundBoxes)
        {

            if (IsDisposed) return;

            if (InvokeRequired)
            {
                BeginInvoke(() => SetBoxes(BoundBoxes));
                return;
            }

            var CensorBoxes = new List<Rectangle>(BoundBoxes.Count);
            foreach (SKRectI box in BoundBoxes)
            {
                CensorBoxes.Add(new Rectangle((int)box.Left, (int)box.Top, (int)box.Width, (int)box.Height));
            }

            lock (_lock)
            {
                _boxes.Clear();
                _boxes.AddRange(CensorBoxes);
            }
            Invalidate();
        }

        public void ClearBoxes()
        {
            SetBoxes(new List <SKRectI>());
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Magenta);

            lock (_lock)
            {
                using var brush = new SolidBrush(Color.Black);
                foreach (var box in _boxes)
                    e.Graphics.FillRectangle(brush, box);
            }
        }

    }

    

    internal static class MonitorInterop
    {
        private const uint MONITOR_DEFAULTTONEAREST = 2;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromRect(ref RECT lprc, uint dwFlags);

        public static IntPtr GetHMonitor(Screen screen)
        {
            var rect = new RECT
            {
                Left = screen.Bounds.Left,
                Top = screen.Bounds.Top,
                Right = screen.Bounds.Right,
                Bottom = screen.Bounds.Bottom
            };

            return MonitorFromRect(ref rect, MONITOR_DEFAULTTONEAREST);
        }
    }
    internal static class CaptureItemFactory
    {
        private static readonly Guid GraphicsCaptureItemIid =
            new("79C3F95B-31F7-4EC2-A464-632EF5D30760");

        [ComImport]
        [Guid("3628E81B-3CAC-4C60-B7F4-23CE0E0C3356")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IGraphicsCaptureItemInterop
        {
            IntPtr CreateForWindow([In] IntPtr window, [In] ref Guid iid);
            IntPtr CreateForMonitor([In] IntPtr monitor, [In] ref Guid iid);
        }

        public static GraphicsCaptureItem CreateForMonitor(IntPtr hMonitor)
        {
            var factory = ActivationFactory.Get(typeof(GraphicsCaptureItem).FullName);
            var interop = factory.AsInterface<IGraphicsCaptureItemInterop>();

            var iid = GraphicsCaptureItemIid;
            var itemPtr = interop.CreateForMonitor(hMonitor, ref iid);

            return MarshalInterface<GraphicsCaptureItem>.FromAbi(itemPtr);
        }
    }

}
