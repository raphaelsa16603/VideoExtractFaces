using LibVideoExtractFaces.Image.Interfaces;
using LibVideoExtractFaces.Model;
using LibVideoExtractFaces.Video.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;  // OpenCvSharp namespaces
using DlibDotNet;
using OpenPoseDotNet;
using System.IO;
using Emgu.CV;  // Emgu.CV namespaces
using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace LibVideoExtractFaces.Image
{
    public class ImageProcessorExtra : IImageProcessor
    {
        private readonly IFrameQualityFilter _frameQualityFilter;
        private readonly Net _ageNet;
        private readonly Net _genderNet;
        private readonly string[] _ageList = new string[] { "0-2", "4-6", "8-12", "15-20", "25-32", "38-43", "48-53", "60-" };
        private readonly string[] _genderList = new string[] { "Male", "Female" };

        private string _videoName;

        public ImageProcessorExtra(IFrameQualityFilter frameQualityFilter, string videoName = "Geral")
        {
            _frameQualityFilter = frameQualityFilter;
            _videoName = videoName;

            // Carregar modelos pré-treinados para estimativa de idade e sexo
            string ageProto = "./models/age_deploy.prototxt";
            string ageModel = "./models/age_net.caffemodel";
            _ageNet = DnnInvoke.ReadNetFromCaffe(ageProto, ageModel);

            string genderProto = "./models/gender_deploy.prototxt";
            string genderModel = "./models/gender_net.caffemodel";
            _genderNet = DnnInvoke.ReadNetFromCaffe(genderProto, genderModel);
        }

        public IEnumerable<LibVideoExtractFaces.Model.Image> ExtractFaces(IEnumerable<Frame> frames, int quantidade = 8)
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
                            var (age, gender) = EstimateAgeAndGender(face);
                            yield return new LibVideoExtractFaces.Model.Image(face, $"Face detected - Age: {age}, Gender: {gender}");
                        }
                    }
                }
            }
        }

        public IEnumerable<Model.Image> ExtractFullBodies(IEnumerable<Frame> frames)
        {
            throw new NotImplementedException();
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

        private (string age, string gender) EstimateAgeAndGender(byte[] faceBytes)
        {
            using (var faceMat = OpenCvSharp.Mat.FromImageData(faceBytes))
            {
                // Converta o OpenCvSharp.Mat para Emgu.CV.Image
                using (var img = new Emgu.CV.Image<Bgr, byte>(faceMat.Width, faceMat.Height, (int)faceMat.Step(), faceMat.Data))
                {
                    var blob = DnnInvoke.BlobFromImage(img, 1.0, new System.Drawing.Size(227, 227), new MCvScalar(104, 177, 123), false, false);

                    // Estimar sexo
                    _genderNet.SetInput(blob);
                    var genderPreds = _genderNet.Forward();
                    var gender = _genderList[GetMaxIndex(genderPreds)];

                    // Estimar idade
                    _ageNet.SetInput(blob);
                    var agePreds = _ageNet.Forward();
                    var age = _ageList[GetMaxIndex(agePreds)];

                    return (age, gender);
                }
            }
        }

        private int GetMaxIndex(Emgu.CV.Mat mat)
        {
            System.Drawing.Point maxLoc = new System.Drawing.Point();
            double[] minVal = new double[1];
            double[] maxVal = new double[1];
            CvInvoke.MinMaxLoc(mat, ref minVal[0], ref maxVal[0], ref maxLoc, ref maxLoc);
            return maxLoc.Y;  // Retorna a localização do valor máximo
        }




    }


}
