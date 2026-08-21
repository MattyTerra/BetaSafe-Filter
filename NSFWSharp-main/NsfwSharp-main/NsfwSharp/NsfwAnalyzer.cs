using SkiaSharp;
using YoloDotNet;
using YoloDotNet.ExecutionProvider.DirectML;
using YoloDotNet.Extensions;
using YoloDotNet.Models;
using YoloDotNet.Enums;
using System.Drawing;
using System.Diagnostics;

namespace NsfwSharp
{
    public class NsfwAnalyzer
    {
        private Yolo _yolo;

        public NsfwAnalyzer(string modelPath)
        {
            //runs on GPU for multiprocessing
            _yolo = new Yolo(new YoloOptions
            {
                ExecutionProvider = new DirectMLExecutionProvider(
                    model: modelPath,
                    gpuId:0
                )

            });
        }

        public NsfwAnalysis GetNsfwAnalysis(Bitmap BitmapImage, List<string> Categories)
        {
            using (var ms = new MemoryStream())
            {
                BitmapImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                using (SKImage skImage = SKImage.FromEncodedData(ms))
                {
                    return GetNsfwAnalysis(skImage, Categories);
                }
            }
        }

        public NsfwAnalysis GetNsfwAnalysis(string imagePath, List<string> Categories)
        {
            return GetNsfwAnalysis(SKImage.FromEncodedData(imagePath), Categories);
        }

        public NsfwAnalysis GetNsfwAnalysis(byte[] imageData, List<string> Categories)
        {
            return GetNsfwAnalysis(SKImage.FromEncodedData(imageData), Categories);
        }

        public NsfwAnalysis GetNsfwAnalysis(SKImage image, List<string> Categories)
        {
            Categories ??= ["TEST"];  //todo make this not so janky
            List<ObjectDetection>? results = _yolo.RunObjectDetection(image, confidence: 0.25, iou: 0.7);
            List<NsfwDetection> detections = new List<NsfwDetection>();
            List<SKRectI> boxes = new List<SKRectI>();

          

            foreach (ObjectDetection objectDetection in results)
            {
                //if the list is empty OR the list contains the name of the object detected
                //Debug.WriteLine(objectDetection.Label.Name.ToUpper());
                //Debug.WriteLine(string.Join(",", Categories));

                if (Categories[0] == "TEST" || Categories.Contains(objectDetection.Label.Name.ToUpper())){
                    //Debug.WriteLine($"Censoring {objectDetection.Label.Name.ToUpper()}");
                    
                    detections.Add(new NsfwDetection(objectDetection.Label.Name.Substring(0, 1).ToUpper() + objectDetection.Label.Name.Substring(1).ToLower(), objectDetection.Confidence));
                    boxes.Add(objectDetection.BoundingBox);
                }
            }

            SKImage detectionsImage = SKImage.FromBitmap(image.Draw(results));

            //SKBitmap detectionsImage = SKBitmap.FromImage(image.Draw(results));
            return new NsfwAnalysis(detections.Count > 0, detections.ToArray(), image, detectionsImage,boxes);
        }
    }
}