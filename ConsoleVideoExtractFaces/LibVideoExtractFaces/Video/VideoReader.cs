using LibVideoExtractFaces.Model;
using LibVideoExtractFaces.Video.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System;
using System.Collections.Generic;

namespace LibVideoExtractFaces.Video
{
    

    public class VideoReader : IVideoReader
    {
        public IEnumerable<Frame> ReadFrames(string videoPath)
        {
            // Lista para armazenar os frames extraídos
            var frames = new List<Frame>();

            // Inicializa o capturador de vídeo usando OpenCV
            using (var capture = new VideoCapture(videoPath))
            {
                if (!capture.IsOpened())
                {
                    throw new ArgumentException($"Não foi possível abrir o vídeo: {videoPath}");
                }

                int frameNumber = 0;
                var mat = new Mat();

                // Ler cada frame do vídeo
                while (capture.Read(mat))
                {
                    // Se o frame não for válido (fim do vídeo, por exemplo)
                    if (mat.Empty())
                        break;

                    // Converte o frame para um array de bytes
                    byte[] imageData = mat.ToBytes(".jpg");

                    // Cria um novo objeto Frame e adiciona à lista
                    var frame = new Frame(frameNumber, imageData);
                    frames.Add(frame);

                    frameNumber++;
                }
            }

            return frames;
        }
    }


}
