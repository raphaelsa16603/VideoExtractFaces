using LibVideoExtractFaces.Model;
using LibVideoExtractFaces.Video.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Drawing;

namespace LibVideoExtractFaces.Video
{


    using OpenCvSharp;

    public class FrameQualityFilter : IFrameQualityFilter
    {
        public bool IsFrameQualityGood(Frame frame)
        {
            // Converter a imagem para uma matriz OpenCV
            Mat imageMat = OpenCvSharp.Mat.FromImageData(frame.ImageData);

            // Verificar nitidez usando a Transformada de Laplace
            if (!IsSharp(imageMat))
            {
                return false;
            }

            // Verificar a qualidade de iluminação
            if (!IsWellLit(imageMat))
            {
                return false;
            }

            return true;
        }

        private bool IsSharp(Mat imageMat)
        {
            // Aplicar a Transformada de Laplace
            Mat laplacianMat = new Mat();
            Cv2.Laplacian(imageMat, laplacianMat, MatType.CV_64F);

            // Calcular a variação da Laplace
            Mat squareLaplacianMat = laplacianMat.Mul(laplacianMat);
            double variance = Cv2.Mean(squareLaplacianMat).Val0;

            // Um limiar típico para nitidez, pode ser ajustado conforme necessário
            return variance > 100; // Valor ajustável dependendo do caso
        }

        private bool IsWellLit(Mat imageMat)
        {
            // Converter para escala de cinza
            Mat grayMat = new Mat();
            Cv2.CvtColor(imageMat, grayMat, ColorConversionCodes.BGR2GRAY);

            // Calcular o histograma
            int histSize = 256;
            Rangef range = new Rangef(0, 256);
            Mat hist = new Mat();
            Cv2.CalcHist(new[] { grayMat }, new[] { 0 }, null, hist, 1, new[] { histSize }, new[] { range });

            // Normalizar o histograma
            Cv2.Normalize(hist, hist, 0, 1, NormTypes.MinMax);

            // Verificar distribuição do histograma
            double meanIntensity = Cv2.Mean(grayMat).Val0;
            double lowIntensityThreshold = 50;
            double highIntensityThreshold = 200;

            return meanIntensity > lowIntensityThreshold && meanIntensity < highIntensityThreshold;
        }
    }



}
