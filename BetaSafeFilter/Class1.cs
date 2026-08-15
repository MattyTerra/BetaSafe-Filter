using FFMpegCore;
//using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using NsfwSharp;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using SkiaSharp;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace BetaSafeFilter
{
    public enum CensorType
    {
        GaussianBlur,
        Pixelate,
        CensoredLogo,
        SolidColor,
        Test
    }
    public class NSFWCensorService
    {
        private readonly NsfwAnalyzer _analyzer;
        private readonly double _DefaultConfidenceThreshold;


        //Build the constructor
        public NSFWCensorService(string Model, double DefaultConfidenceThreshold = .20)
        {
            _analyzer = new NsfwAnalyzer(Model);
            _DefaultConfidenceThreshold = DefaultConfidenceThreshold;
        }

        private static Rect ClampRectToFrame(Rect rect, int frameWidth, int frameHeight)
        {
            int x = Math.Max(0, rect.X);
            int y = Math.Max(0, rect.Y);

            int right = Math.Min(frameWidth, rect.X + rect.Width);
            int bottom = Math.Min(frameHeight, rect.Y + rect.Height);

            int width = Math.Max(0, right - x);
            int height = Math.Max(0, bottom - y);

            return new Rect(x, y, width, height);
        }

        public void CensorMatInPlace(Mat Frame, double k = 1.25, CensorType censorType = CensorType.GaussianBlur, double AnalysisScale=0.5,Scalar? color=null)
        {
            if (Frame == null || Frame.Empty()) { return; }

            using Bitmap AnalysisFrame = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(Frame);

            NsfwAnalysis analysis = _analyzer.GetNsfwAnalysis(AnalysisFrame);
            Mat analysisMat;
            
            if (AnalysisScale < 1.0)
            {
                int analysisWidth = Math.Max(1, (int)(Frame.Width * AnalysisScale));
                int analysisHeight = Math.Max(1, (int)(Frame.Height * AnalysisScale));

                analysisMat = new Mat();

                Cv2.Resize(
                    Frame,
                    analysisMat,
                    new OpenCvSharp.Size(analysisWidth, analysisHeight),
                    0,
                    0,
                    InterpolationFlags.Linear);
            }
            else
            {
                analysisMat = Frame;
            }

            foreach (SKRectI blur in analysis.BoundingBoxes)
            {
                Rect censorZone = new Rect
                {
                    X = (int)(blur.MidX - (blur.Width * k / 2.0)),
                    Y = (int)(blur.MidY - (blur.Height * k / 2.0)),
                    Width = (int)(blur.Width * k),
                    Height = (int)(blur.Height * k)
                };

                censorZone = ClampRectToFrame(censorZone, Frame.Width, Frame.Height);

                if (censorZone.Width <= 0 || censorZone.Height <= 0)
                    continue;

                ApplyCensor(Frame, censorZone, censorType,color);
            }
        }


        public Bitmap CensorImage(Bitmap ImagePath, double k = 1.25, CensorType censorType = CensorType.GaussianBlur,Scalar? color=null)
        {
            NsfwAnalysis Analysis = _analyzer.GetNsfwAnalysis(ImagePath);  //set up analyizer
            using (Mat boop2 = OpenCvSharp.Extensions.BitmapConverter.ToMat(ImagePath))
            {
                CensorMatInPlace(boop2,k,censorType,color: color);
                
                return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(boop2);
            }


        }

        public void CensorVideo(String VideoPath, String Framesource, String OutputSource, String Video, double k = 1.25, CensorType censorType = CensorType.GaussianBlur,Scalar? color=null)
        {
            /*Summary
             * Video Path: Location of the OG Video
             * Framesource: Location of the Frames that have been Made from the Video
             * OutputSource: Location of the frames that have been censored 
             * Video Final Video
             */

            //Start with a bunch of Error handling. If Paths arent right, throw errors
            if (string.IsNullOrWhiteSpace(VideoPath))
                throw new ArgumentException("Video path cannot be empty.");

            if (!File.Exists(VideoPath))
                throw new FileNotFoundException("Input video not found.", VideoPath);

            if (string.IsNullOrWhiteSpace(Video))
                throw new ArgumentException("Output video path cannot be empty.");

            // Make sure output has a usable extension
            if (string.IsNullOrWhiteSpace(Path.GetExtension(Video)))
                Video = Path.ChangeExtension(Video, ".mp4");
            //Make the output Path


            string outputVideoPath = Path.Combine(Path.GetDirectoryName(OutputSource), @"Frameholder\Censored Video");


            ExtractFrames(VideoPath, Framesource); //Takes all the frames and dumps them into the folder

            var frameFiles = Directory.EnumerateFiles(Framesource).ToList();

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = 8
            };


            Parallel.ForEach(frameFiles, options, FilePath =>
            {
                using (Bitmap sourceBitmap = new Bitmap(FilePath))
                using (Bitmap CensoredFrame = CensorImage(sourceBitmap, k, censorType))
                {
                    String CensoredFileName = "CENSORED_" + Path.GetFileName(FilePath);
                    String SavePath = Path.Combine(OutputSource, CensoredFileName);
                    CensoredFrame.Save(SavePath, System.Drawing.Imaging.ImageFormat.Png);
                }


            });

            String FramePattern = Path.Combine(OutputSource, @"CENSORED_frame_%04d.png");
            Debug.WriteLine(FramePattern);
            FFMpegArguments
            .FromFileInput(FramePattern, false, input => input
            .WithCustomArgument("-start_number 0001")
            .WithFramerate(30)
            )
            //.AddInput(originalVideoPath)                    // Add original video for audio TODO
            .OutputToFile(Video, true, options => options
                .WithVideoCodec("h264_nvenc")
                .WithCustomArgument("-preset p5")
                .WithFramerate(25)
                .WithConstantRateFactor(18)
                .WithCustomArgument("-pix_fmt yuv420p")
                .WithCustomArgument("-movflags +faststart")
            )
            .ProcessSynchronously();
        }

        public void CensorVideoFast(String VideoPath, String Video, double k = 1.25, CensorType censorType = CensorType.GaussianBlur, Scalar? color = null)
        {
            /*Summary
             * Video Path: Location of the OG Video
             * Framesource: Location of the Frames that have been Made from the Video
             * OutputSource: Location of the frames that have been censored 
             * Video Final Video
             */


            //Start with Error Handling, If files arent there, throw errors
            if (string.IsNullOrWhiteSpace(VideoPath))
                throw new ArgumentException("Video path cannot be empty.");

            if (!File.Exists(VideoPath))
                throw new FileNotFoundException("Input video not found.", VideoPath);

            if (string.IsNullOrWhiteSpace(Video))
                throw new ArgumentException("Output video path cannot be empty.");

            // Make sure output has a usable extension.
            if (string.IsNullOrWhiteSpace(Path.GetExtension(Video)))
                Video = Path.ChangeExtension(Video, ".mp4");

            Directory.CreateDirectory(Path.GetDirectoryName(Video)!);



            using var capture = new VideoCapture(VideoPath);

            if (!capture.IsOpened())
                throw new InvalidOperationException("Could not open input video.");

            int width = (int)capture.FrameWidth;
            int height = (int)capture.FrameHeight;
            double fps = capture.Fps;

            if (fps <= 0 || double.IsNaN(fps)) { fps = 30; }

            string ffmpegArgs =
                $"-y " +

                // Raw censored video comes from stdin
                $"-f rawvideo " +
                $"-pix_fmt bgr24 " +
                $"-s {width}x{height} " +
                $"-r {fps.ToString(System.Globalization.CultureInfo.InvariantCulture)} " +
                $"-i pipe:0 " +

                // Original video is added only to copy audio
                $"-i \"{VideoPath}\" " +

                // Use censored video stream + optional original audio
                $"-map 0:v:0 " +
                $"-map 1:a? " +

                // NVENC encoding
                $"-c:v h264_nvenc " +
                $"-preset p5 " +
                $"-cq 23 " +
                $"-pix_fmt yuv420p " +

                // Copy audio without re-encoding
                $"-c:a copy " +

                // Stop when video stream ends
                $"-shortest " +

                // Better MP4 compatibility
                $"-movflags +faststart " +

                $"\"{Video}\"";
            using var ffmpeg = new Process();

            ffmpeg.StartInfo.FileName = "ffmpeg";
            ffmpeg.StartInfo.Arguments = ffmpegArgs;
            ffmpeg.StartInfo.UseShellExecute = false;
            ffmpeg.StartInfo.RedirectStandardInput = true;
            ffmpeg.StartInfo.RedirectStandardError = true;
            ffmpeg.StartInfo.CreateNoWindow = true;

            ffmpeg.ErrorDataReceived += (_, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                    Debug.WriteLine(e.Data);
            };

            ffmpeg.Start();
            ffmpeg.BeginErrorReadLine();
            using Stream ffmpegInput = ffmpeg.StandardInput.BaseStream;
            using var mat = new Mat();

            byte[] frameBuffer = new byte[width * height * 3];

            int frameNumber = 0;

            while (capture.Read(mat))
            {
                if (mat.Empty())
                    break;

                CensorMatInPlace(mat, k, censorType,0.5,color);

                WriteBitmapAsBgr24ToStream(mat, ffmpegInput,frameBuffer);

                frameNumber++;

                if (frameNumber % 30 == 0)
                {
                    Debug.WriteLine($"Processed {frameNumber} frames...");
                }
            }

            ffmpegInput.Flush();
            ffmpegInput.Close();

            ffmpeg.WaitForExit();

            if (ffmpeg.ExitCode != 0)
            {
                throw new Exception($"FFmpeg failed with exit code {ffmpeg.ExitCode}.");
            }

        }

        private void ApplyCensor(Mat Image, Rect Zone, CensorType censorType, Scalar? color = null)
        {
            /* Summary:
             * 
             * Contains the list of all the differnt censor types that I plan to add later.
             * To be populated later when I get some more time
             * Currently contins Gaussian Blur, Pixelate, Censored Logo, and a solid color
             * 
             */
            try
            {
                using Mat Region = new Mat(Image, Zone);


                switch (censorType)
                {
                    case CensorType.GaussianBlur:
                        Cv2.Rectangle(Image, Zone, Scalar.Lime, 2);
                        Cv2.GaussianBlur(Region, Region, new OpenCvSharp.Size(65, 65), 0);
                        break;
                    case CensorType.Pixelate:
                        // Define the size of the pixel blocks (larger number = more pixelated)
                        int pixelSize = 30;

                        // Calculate temporary tiny dimensions (ensure they are at least 1 pixel)
                        int smallW = Math.Max(1, Region.Width / pixelSize);
                        int smallH = Math.Max(1, Region.Height / pixelSize);
                        OpenCvSharp.Size smallSize = new OpenCvSharp.Size(smallW, smallH);
                        OpenCvSharp.Size originalSize = new OpenCvSharp.Size(Region.Width, Region.Height);

                        // Create a temporary matrix to hold the tiny image
                        using (Mat smallRegion = new Mat())
                        {
                            // 1. Shrink the region down to a tiny size
                            Cv2.Resize(Region, smallRegion, smallSize, 0, 0, InterpolationFlags.Linear);

                            // 2. Blow it back up using Nearest Neighbor to keep hard pixel edges
                            // We write the result right back into 'Region', which updates the parent 'Image'
                            Cv2.Resize(smallRegion, Region, originalSize, 0, 0, InterpolationFlags.Nearest);
                        }
                        break;
                    case CensorType.CensoredLogo:
                        //Make This Later
                        break;
                    case CensorType.SolidColor:

                        Scalar chosenColor = color ?? Scalar.Black;
                        Cv2.Rectangle(Image, Zone, chosenColor, Cv2.FILLED);
                        break;
                    default:
                        Cv2.Rectangle(Image, Zone, Scalar.HotPink);
                        break;
                }

            }
            catch (Exception ex) { return; }
        }

        public void ExtractFrames(string videoPath, string outputFolder = null)
        {
            if (String.IsNullOrEmpty(videoPath)) { throw new ArgumentException("Video path cannot be empty."); ; }

            if (outputFolder == null)
            {
                //If you dont give a file location, the frames are saved into a folder names "frames_videoname"
                //at the same location as the origional video
                string videoName = Path.GetFileNameWithoutExtension(videoPath);
                outputFolder = Path.Combine(Path.GetDirectoryName(videoPath), $"frames_{videoName}");
            }

            Directory.CreateDirectory(outputFolder);

            Debug.WriteLine(outputFolder);

            string outputPattern = Path.Combine(outputFolder, "frame_%04d.png");

            FFMpegArguments
            .FromFileInput(videoPath, true, inputOptions => inputOptions
                .WithCustomArgument("-hwaccel auto")      // Hardware acceleration
             )
            .OutputToFile(outputPattern, true, outputOptions => outputOptions
                    .WithFramerate(30)
                    .WithCustomArgument("-vf fps=30")
                    )
            .ProcessSynchronously();

            var frames = Directory.GetFiles(outputFolder, "frame_*.png")
                          .OrderBy(f => f)
                          .ToList();

            //return frames;
        }

        private static void WriteBitmapAsBgr24ToStream(Mat mat, Stream output, byte[] frameBuffer)
        {

            //like always, start with error handling
            if (mat == null || mat.Empty())
                return;

            if (mat.Type() != MatType.CV_8UC3)
                throw new ArgumentException("Mat must be CV_8UC3, which means 8-bit 3-channel BGR.");

            int width = mat.Width;
            int height = mat.Height;
            int rowBytes = width * 3;
            int totalBytes = rowBytes * height;

            if (frameBuffer.Length < totalBytes)
                throw new ArgumentException("Frame buffer is too small for this Mat.");

            for (int y = 0; y < height; y++)
            {
                IntPtr rowPtr = mat.Ptr(y);
                Marshal.Copy(rowPtr, frameBuffer, y * rowBytes, rowBytes);
            }

            output.Write(frameBuffer, 0, totalBytes);
        }
    }
}
