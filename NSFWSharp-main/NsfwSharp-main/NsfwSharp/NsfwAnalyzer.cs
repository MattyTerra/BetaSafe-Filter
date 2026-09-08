using SkiaSharp;
using YoloDotNet;
using YoloDotNet.ExecutionProvider.DirectML;
//using YoloDotNet.ExecutionProvider.Cuda;
using YoloDotNet.Extensions;
using YoloDotNet.Models;
using YoloDotNet.Enums;
using System.Drawing;
using System.Diagnostics;
using YoloDotNet.Models.Interfaces;

namespace NsfwSharp
{
    public class NsfwAnalyzer
    {
        private Yolo _yolo;
        private double ConfidenceThreshold;

        public NsfwAnalyzer(string modelPath, double ConfidenceThreshold=.2)
        {
            
            this.ConfidenceThreshold= ConfidenceThreshold;

            //TODO Look into adding CUDA in later. I am convined something is wrong this CUDA integration at the moment. 
            IExecutionProvider provider;
            //try
            //{
            //    provider = new CudaExecutionProvider(model: modelPath,gpuId: 0);
            //}
            //catch
            //{
            provider = new DirectMLExecutionProvider(model: modelPath,gpuId: 0);

            //}
            _yolo = new Yolo(new YoloOptions
            {
                ExecutionProvider = provider

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
            List<ObjectDetection>? results = _yolo.RunObjectDetection(image, confidence: this.ConfidenceThreshold, iou: 0.7);
            
            var  detections = new List<NsfwDetection>();
            var boxes = new List<SKRectI>();

          

            foreach (ObjectDetection objectDetection in results)
            {
                if (Categories[0] == "TEST" || Categories.Contains(objectDetection.Label.Name.ToUpper())){
                    detections.Add(new NsfwDetection(
                        objectDetection.Label.Name.Substring(0, 1).ToUpper() + 
                        objectDetection.Label.Name.Substring(1).ToLower(), 
                        objectDetection.Confidence));
                    boxes.Add(objectDetection.BoundingBox);
                }
            }
            image.Dispose();
            return new NsfwAnalysis(detections.Count > 0, detections.ToArray(), boxes);
            //SKImage detectionsImage = SKImage.FromBitmap(image.Draw(results));

            //SKBitmap detectionsImage = SKBitmap.FromImage(image.Draw(results));
            //return new NsfwAnalysis(detections.Count > 0, detections.ToArray(), image, detectionsImage,boxes);
        }
    }
}