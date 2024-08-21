# LibVideoExtractFaces

**LibVideoExtractFaces** é uma biblioteca em C# para extração de faces e corpos completos de frames de vídeo utilizando várias técnicas de visão computacional, incluindo **Dlib** e **Yolo**.

## Funcionalidades

- **Extração de Faces:** Utiliza a biblioteca **Dlib** para detectar e extrair faces em imagens provenientes de frames de vídeo.
- **Detecção de Corpos Completos:** Implementada com o **Alturos.Yolo** para detecção de pessoas (full-body) em imagens de vídeo.
- **Filtragem de Qualidade de Frames:** Através de uma interface personalizada, a biblioteca permite filtrar frames com base na qualidade antes da extração.

## Dependências

- [OpenCvSharp](https://github.com/shimat/opencvsharp) - Biblioteca .NET para manipulação de imagens e vídeos.
- [DlibDotNet](https://github.com/takuya-takeuchi/DlibDotNet) - Porta C# de Dlib, usada para detecção de faces.
- [Alturos.Yolo](https://github.com/AlturosDestinations/Alturos.Yolo) - Wrapper para o YOLO (You Only Look Once), utilizado para detecção de corpos completos.

## Como Usar

1. Clone este repositório:

   ```bash
   git clone https://github.com/seu-usuario/LibVideoExtractFaces.git
