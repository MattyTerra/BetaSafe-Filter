using System;
using System.Collections.Generic;
using System.Text;
using OpenCvSharp;

//located here will be all the censorhship options and constructors and mutators for them.

namespace BetaSafeFilter
{
    public enum CensorType   //censortypes for reference later
    {
        GaussianBlur,
        Pixelate,
        CensoredLogo,
        SolidColor,
        TVStatic,
        Test
    }

    public class Options
    {
        public Color CensorColor;
        public int GaussianBlurFactor;
        public int PixelateFactor;
        public int TVstatic;

        public Options(Color CensorColor = default, int GaussianBlurFactor = 65, int PixelateFactor = 30, int TVstatic = 30)
        {

            this.CensorColor = CensorColor;
            this.GaussianBlurFactor = GaussianBlurFactor;
            this.PixelateFactor = PixelateFactor;
            this.TVstatic = TVstatic;
        }

        public void ChangeCensorColor(Color CensorColor)
        {
            this.CensorColor = CensorColor;
        }

        public void ChangeGaussianBlurFactor(int GaussianBlurFactor)
        {
            this.GaussianBlurFactor = GaussianBlurFactor;
        }

        public void ChangePixelateFactor(int PixelateFactor)
        {
            this.PixelateFactor = PixelateFactor;
        }
        public void ChangeTVStatic(int TVstatic)
        {
            this.TVstatic = TVstatic;
        }
    }
}
