using LibVideoExtractFaces.Image.Interfaces;
using LibVideoExtractFaces.Model;
using LibVideoExtractFaces.Video.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using DlibDotNet;
using OpenPoseDotNet;
using System.Collections.Generic;
using System.Linq;
using Alturos.Yolo;
using Alturos.Yolo.Model;
using System.Collections.Generic;
using System.Linq;

namespace LibVideoExtractFaces.Image
{


    public class ImageProcessor : IImageProcessor
    {
        private readonly IFrameQualityFilter _frameQualityFilter;
        private readonly YoloWrapper _yoloWrapper;

        public ImageProcessor(IFrameQualityFilter frameQualityFilter)
        {
            _frameQualityFilter = frameQualityFilter;

            /*var dirConfig = "./models/yolov3";

            if(!Directory.Exists(dirConfig))
            {
                Directory.CreateDirectory(dirConfig);
            }

            // Configurar o YoloWrapper com os arquivos de configuração
            var configurationDetector = new YoloConfigurationDetector();
            var config = configurationDetector.Detect(dirConfig);  // Detecta automaticamente a configuração apropriada
            
            string configFile = "path/to/yolov3.cfg";
            string weightsFile = "path/to/yolov3.weights";
            string namesFile = "path/to/coco.names";

            // Inicializando YoloWrapper com os arquivos de configuração
            _yoloWrapper = new YoloWrapper(configFile, weightsFile, namesFile);*/
        }

        public IEnumerable<LibVideoExtractFaces.Model.Image> ExtractFaces(IEnumerable<Frame> frames)
        {
            foreach (var frame in frames)
            {
                if (_frameQualityFilter.IsFrameQualityGood(frame))
                {
                    using (var mat = OpenCvSharp.Mat.FromImageData(frame.ImageData))
                    {
                        var faces = DetectFaces(mat);
                        foreach (var face in faces)
                        {
                            yield return new LibVideoExtractFaces.Model.Image(face, "Face detected");
                        }
                    }
                }
            }
        }

        public IEnumerable<LibVideoExtractFaces.Model.Image> ExtractFullBodies(IEnumerable<Frame> frames)
        {
            /*foreach (var frame in frames)
            {
                if (_frameQualityFilter.IsFrameQualityGood(frame))
                {
                    using (var mat = OpenCvSharp.Mat.FromImageData(frame.ImageData))
                    {
                        var bodies = DetectFullBodies(mat);
                        foreach (var body in bodies)
                        {
                            yield return new LibVideoExtractFaces.Model.Image(body, "Full body detected");
                        }
                    }
                }
            }*/

            return new List<LibVideoExtractFaces.Model.Image>();
        }

        private IEnumerable<byte[]> DetectFaces(OpenCvSharp.Mat mat)
        {
            var facesList = new List<byte[]>();

            using (var faceDetector = DlibDotNet.Dlib.GetFrontalFaceDetector())
            {
                using (var img = DlibDotNet.Dlib.LoadImageData<DlibDotNet.RgbPixel>(
                    mat.ToBytes(".bmp"),
                    (uint)mat.Height,
                    (uint)mat.Width,
                    (uint)(mat.Width * 3)))
                {
                    var faces = faceDetector.Operator(img).ToArray();
                    foreach (var face in faces)
                    {
                        var rect = new OpenCvSharp.Rect(face.Left, face.Top, (int)face.Width, (int)face.Height);
                        if (rect.X >= 0 && rect.Y >= 0 && rect.X + rect.Width <= mat.Cols && rect.Y + rect.Height <= mat.Rows)
                        {
                            using (var faceMat = new OpenCvSharp.Mat(mat, rect))
                            {
                                facesList.Add(faceMat.ToBytes(".jpg"));
                            }
                        }

                    }
                }
            }

            return facesList;
        }

        private IEnumerable<byte[]> DetectFullBodies(OpenCvSharp.Mat mat)
        {
            var bodiesList = new List<byte[]>();

            // Converte a imagem para um formato reconhecível pelo Yolo
            var imageBytes = mat.ToBytes(".bmp");
            var items = _yoloWrapper.Detect(imageBytes);  // Detecta objetos na imagem

            foreach (var item in items.Where(i => i.Type == "Person"))  // Filtra para detectar apenas pessoas
            {
                var rect = new OpenCvSharp.Rect(item.X, item.Y, item.Width, item.Height);
                using (var bodyMat = new OpenCvSharp.Mat(mat, rect))
                {
                    bodiesList.Add(bodyMat.ToBytes(".jpg"));  // Adiciona a detecção à lista
                }
            }

            return bodiesList;
        }
    }



}
