﻿using LibVideoExtractFaces.Image.Interfaces;
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
using System.Collections;

namespace LibVideoExtractFaces.Image
{


    public class ImageProcessor : IImageProcessor
    {
        private readonly IFrameQualityFilter _frameQualityFilter;
        private readonly YoloWrapper _yoloWrapper;

        private string _videoName;

        public ImageProcessor(IFrameQualityFilter frameQualityFilter, string videoName = "Geral")
        {
            _frameQualityFilter = frameQualityFilter;
            _videoName = videoName;

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

        public IEnumerable<LibVideoExtractFaces.Model.Image> ExtractFaces(IEnumerable<Frame> frames, int quantidade = 8)
        {
            ExtrairFramesPricipais(frames, quantidade);

            foreach (var frame in frames)
            {
                if (_frameQualityFilter.IsFrameQualityGood(frame))
                {
                    using (var mat = OpenCvSharp.Mat.FromImageData(frame.ImageData))
                    {
                        var faces = DetectFaces(mat);
                        if(faces != null && faces.Count() > 0)
                        {
                            foreach (var face in faces)
                            {
                                yield return new LibVideoExtractFaces.Model.Image(face, "Face detected");
                            }
                        }
                        else
                        {
                            // Não funcionou muito bem com imagens de baixa resolução....
                            /*faces = DetectFaces(mat, "./models/haarcascade_frontalface_default.xml");
                            foreach (var face in faces)
                            {
                                yield return new LibVideoExtractFaces.Model.Image(face, "Face detected");
                            }*/


                            // Modelo não deteca rosto também com baixa qualidade. Pegou só linhas no chão.
                            /*faces = DetectFaces(mat, "./models/haarcascade_frontalface_alt.xml");
                            foreach (var face in faces)
                            {
                                yield return new LibVideoExtractFaces.Model.Image(face, "Face detected");
                            }*/


                            // Modelo não deteca corpo inteiro com baixa qualidade. Pegou nada
                            /*faces = DetectFaces(mat, "./models/haarcascade_fullbody.xml");
                            foreach (var face in faces)
                            {
                                yield return new LibVideoExtractFaces.Model.Image(face, "Face detected");
                            }*/

                            // Detecta parte superior do rosto com baixa qualidade. Para um modelo de rosto de perfil.
                            /*faces = DetectFaces(mat, "./models/haarcascade_profileface.xml");
                            foreach (var face in faces)
                            {
                                yield return new LibVideoExtractFaces.Model.Image(face, "Face detected");
                            }*/
                            // Aplicando rotação da imagem para pegar a face.
                            faces = DetectFacesWithRotations(mat, "./models/haarcascade_profileface.xml");
                            foreach (var face in faces)
                            {
                                yield return new LibVideoExtractFaces.Model.Image(face, "Face detected");
                            }

                            // haarcascade_frontalface_default_cuda.xml
                            // Não funcionou bem
                            /*faces = DetectFaces(mat, "./models/haarcascade_frontalface_default_cuda.xml");
                            foreach (var face in faces)
                            {
                                yield return new LibVideoExtractFaces.Model.Image(face, "Face detected");
                            }*/
                        }
                    }
                }
            }
        }

        private void ExtrairFramesPricipais(IEnumerable<Frame> frames, int quantidade = 8)
        {
            // Dividir a quantidade de frames em 8 partes
            int numParts = quantidade;
            int framesPerPart = frames.Count() / numParts;

            var lista = new List<LibVideoExtractFaces.Model.Image>();

            for (int i = 0; i < numParts; i++)
            {
                // Obter a parte atual de frames
                var currentFrames = frames.Skip(i * framesPerPart).Take(framesPerPart);
                var iFrame = currentFrames.FirstOrDefault();
                var imagem = OpenCvSharp.Mat.FromImageData(iFrame.ImageData);
                var imagemBytes = new LibVideoExtractFaces.Model.Image(imagem.ToBytes(".jpg"), $"Frame_{i}_");
                
                lista.Add(imagemBytes);

            }

            // Obter a data e hora atual
            var now = DateTime.Now;
            var folderName = now.ToString("yyyy_MM_dd_HH");

            // Salvar as imagens dos frames na pasta frames na subpasta ano_mes_dia_hora
            if (_videoName != null)
            {
                SaveImages(lista, folderName, _videoName.Trim());
            }
            else
            {
                SaveImages(lista, folderName, "Geral");
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

        
        private IEnumerable<byte[]> DetectFaces(OpenCvSharp.Mat mat, string fileXml = "./models/haarcascade_frontalface_default.xml")
        {
            var facesList = new List<byte[]>();

            // Carregar o classificador pré-treinado do OpenCV para detecção de faces
            var faceCascade = new OpenCvSharp.CascadeClassifier(fileXml);

            // Converter a imagem para escala de cinza
            using (var grayMat = new OpenCvSharp.Mat())
            {
                Cv2.CvtColor(mat, grayMat, OpenCvSharp.ColorConversionCodes.BGR2GRAY);

                // Detectar faces na imagem
                var faces = faceCascade.DetectMultiScale(grayMat, 1.1, 3, HaarDetectionTypes.ScaleImage, new OpenCvSharp.Size(30, 30));

                foreach (var face in faces)
                {
                    if (face.X >= 0 && face.Y >= 0 && face.X + face.Width <= mat.Cols && face.Y + face.Height <= mat.Rows)
                    {
                        using (var faceMat = new OpenCvSharp.Mat(mat, face))
                        {
                            facesList.Add(faceMat.ToBytes(".jpg"));
                        }
                    }
                }
            }

            return facesList;
        }


        private IEnumerable<byte[]> DetectFacesWithRotations(OpenCvSharp.Mat mat, string fileXml = "./models/haarcascade_frontalface_default.xml")
        {
            var facesList = new List<byte[]>();

            // Carregar o classificador pré-treinado do OpenCV para detecção de faces
            var faceCascade = new OpenCvSharp.CascadeClassifier(fileXml);

            // Tentar detectar faces em diferentes rotações
            for (int angle = 0; angle < 360; angle += 90)
            {
                using (var rotatedMat = RotateImage(mat, angle))
                {
                    // Converter a imagem para escala de cinza
                    using (var grayMat = new OpenCvSharp.Mat())
                    {
                        Cv2.CvtColor(rotatedMat, grayMat, OpenCvSharp.ColorConversionCodes.BGR2GRAY);

                        // Detectar faces na imagem rotacionada
                        var faces = faceCascade.DetectMultiScale(grayMat, 1.1, 3, HaarDetectionTypes.ScaleImage, new OpenCvSharp.Size(30, 30));

                        foreach (var face in faces)
                        {
                            var rect = new OpenCvSharp.Rect(face.X, face.Y, face.Width, face.Height);

                            if (rect.X >= 0 && rect.Y >= 0 && rect.X + rect.Width <= rotatedMat.Cols && rect.Y + rect.Height <= rotatedMat.Rows)
                            {
                                // Rotacionar a face de volta à posição original
                                using (var faceMat = new OpenCvSharp.Mat(rotatedMat, rect))
                                {
                                    if (angle != 0)
                                    {
                                        using (var correctedFaceMat = RotateImage(faceMat, 360 - angle))
                                        {
                                            facesList.Add(correctedFaceMat.ToBytes(".jpg"));
                                        }
                                    }
                                    else
                                    {
                                        facesList.Add(faceMat.ToBytes(".jpg"));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return facesList;
        }

        private OpenCvSharp.Mat RotateImage(OpenCvSharp.Mat mat, double angle)
        {
            var center = new OpenCvSharp.Point2f(mat.Width / 2, mat.Height / 2);
            var rotationMatrix = Cv2.GetRotationMatrix2D(center, angle, 1.0);
            var rotatedMat = new OpenCvSharp.Mat();
            Cv2.WarpAffine(mat, rotatedMat, rotationMatrix, new OpenCvSharp.Size(mat.Width, mat.Height));
            return rotatedMat;
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

        private static void SaveImages(IEnumerable<LibVideoExtractFaces.Model.Image> images, string directory, string videoName)
        {
            string outputDirectory = Path.Combine("output", directory, videoName);

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            int imageIndex = 1;

            foreach (var image in images)
            {
                string filePath = Path.Combine(outputDirectory, $"image_{imageIndex}.jpg");
                File.WriteAllBytes(filePath, image.Data);
                imageIndex++;
            }
        }
    }



}
