﻿using static System.Net.Mime.MediaTypeNames;
using LibVideoExtractFaces.Image.Interfaces;
using LibVideoExtractFaces.Model;
using LibVideoExtractFaces.Video.Interfaces;
using LibVideoExtractFaces;

using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string videoDirectory = "./video";

        // Obter todos os arquivos de vídeo na pasta especificada
        var videoFiles = Directory.GetFiles(videoDirectory, "*.*")
                                  .Where(file => file.EndsWith(".mp4") || file.EndsWith(".avi") || file.EndsWith(".mov")); 
                                    // Adicione outras extensões de vídeo se necessário

        var videoReader = VideoProcessingFactory.CreateVideoReader();
        

        foreach (var videoFile in videoFiles)
        {
            Console.WriteLine($"Processando vídeo: {Path.GetFileName(videoFile)}");

            var frames = videoReader.ReadFrames(videoFile);

            var imageProcessor = VideoProcessingFactory.CreateImageProcessor(Path.GetFileNameWithoutExtension(videoFile));

            var faces = imageProcessor.ExtractFaces(frames, 48);
            //var fullBodies = imageProcessor.ExtractFullBodies(frames);

            // Salvar ou processar as imagens extraídas
            SaveImages(faces, "faces", Path.GetFileNameWithoutExtension(videoFile));
            //SaveImages(fullBodies, "bodies", Path.GetFileNameWithoutExtension(videoFile));

            /// Processamento extra

            var imageProcessorEx = VideoProcessingFactory.CreateImageProcessorExtra(Path.GetFileNameWithoutExtension(videoFile));

            var facesEx = imageProcessor.ExtractFaces(frames, 48);

            // Salvar ou processar as imagens extraídas
            SaveImages(facesEx, "facesEx", Path.GetFileNameWithoutExtension(videoFile));

        }
    }

    static void SaveImages(IEnumerable<LibVideoExtractFaces.Model.Image> images, string directory, string videoName)
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


