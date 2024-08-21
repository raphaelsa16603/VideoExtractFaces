using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibVideoExtractFaces.Model
{
    public class Frame
    {
        public int FrameNumber { get; set; }
        public byte[] ImageData { get; set; } // Supondo que ImageData é um byte[] com os dados da imagem

        public Frame(int frameNumber, byte[] imageData)
        {
            FrameNumber = frameNumber;
            ImageData = imageData;
        }
    }


}
