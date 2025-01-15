using System.Drawing;
using FFMpegCore;
using Microsoft.AspNetCore.Http;
using ProcessadorVideo.CrossCutting.Extensions;
using ProcessadorVideo.Domain.Adapters.Services;

namespace ProcessadorVideo.Infra.Services;

public class VideoService : IVideoService
{
    public async Task GenerateImageFromFrames(byte[] videoBytes, int frameInterval, string outputPath)
    {
        // Criar um arquivo temporário para armazenar o vídeo recebido em byte[]
        string videoPath = Path.GetTempFileName();

        // Gerar nome de pasta sem espaços
        var fileNameWithoutExtension = "video_" + Guid.NewGuid().ToString("N"); // Um nome único para evitar conflitos
        var videoOutputFolder = Path.Combine(outputPath, fileNameWithoutExtension);

        try
        {
            // Criar o diretório de saída
            Directory.CreateDirectory(videoOutputFolder);

            // Salvar o vídeo no arquivo temporário
            await using (var stream = new FileStream(videoPath, FileMode.Create))
            {
                await stream.WriteAsync(videoBytes, 0, videoBytes.Length);
            }

            // Usar FFProbe para analisar o vídeo
            var videoInfo = await FFProbe.AnalyseAsync(videoPath);
            var duration = videoInfo.Duration;

            var interval = TimeSpan.FromSeconds(frameInterval); // Intervalo entre os frames

            // Processar frames do vídeo com base no intervalo
            for (var currentTime = TimeSpan.Zero; currentTime < duration; currentTime += interval)
            {
                Console.WriteLine($"Processando frame: {currentTime}");

                var finalPath = Path.Combine(videoOutputFolder, $"frame_at_{currentTime.TotalSeconds}.jpg");
                FFMpeg.Snapshot(videoPath, finalPath, new Size(1920, 1080), currentTime);
            }
        }
        finally
        {
            // Remover o arquivo temporário de vídeo
            File.Delete(videoPath);
        }
    }
}
