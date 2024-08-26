using LibVideoExtractFaces.Image.Interfaces;
using LibVideoExtractFaces.Image;
using LibVideoExtractFaces.Video.Interfaces;
using LibVideoExtractFaces.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibVideoExtractFaces
{
    public static class VideoProcessingFactory
    {
        public static IVideoReader CreateVideoReader()
        {
            return new VideoReader();
        }

        public static IImageProcessor CreateImageProcessor(string videoName = "Geral")
        {
            return new ImageProcessor(new FrameQualityFilter(), videoName);
        }

        public static IImageProcessor CreateImageProcessorExtra(string videoName = "Geral")
        {
            return new ImageProcessorExtra(new FrameQualityFilter(), videoName);
        }
    }
}
