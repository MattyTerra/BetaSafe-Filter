using SkiaSharp;
using YoloDotNet.Models;

namespace NsfwSharp
{
    public class NsfwAnalysis
    {
        public bool IsNsfw { get; private set; }
        public NsfwDetection[] Detections { get; private set; }
        public SKImage OriginalImage { get; private set; }
        public SKImage DetectionsImage { get; private set; }

        public List<SKRectI> BoundingBoxes { get; private set; }

        
        public NsfwAnalysis(bool isNsfw, NsfwDetection[] detections, SKImage originalImage, SKImage detectionsImage, List<SKRectI> boundingbox)
        {
            IsNsfw = isNsfw; //boolean that tells whether or not the  image is NSFW
            Detections = detections; //I dont know
            OriginalImage = originalImage; //The origional Image
            DetectionsImage = detectionsImage; //The image with boxes around the detections (Not censored)
            BoundingBoxes = boundingbox; //Useful Boxes for censorship
        }
    }
}