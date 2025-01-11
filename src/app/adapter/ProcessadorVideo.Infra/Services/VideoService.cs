using System.Drawing;
using FFMpegCore;
using Microsoft.AspNetCore.Http;
using ProcessadorVideo.CrossCutting.Extensions;
using ProcessadorVideo.Domain.Services;

namespace ProcessadorVideo.Infra.Services;

public class VideoService : IVideoService
{
    public async Task GenerateImageFromFrames(IFormFile video, int frameInterval, string outputPath)
    {
        string videoPath = Path.GetTempFileName();

        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(video.FileName)
                                   .Replace(" ", "_");

        var videoOutputFolder = Path.Combine(outputPath, fileNameWithoutExtension);

        try
        {

            Directory.CreateDirectory(videoOutputFolder);

            await using (var stream = new FileStream(videoPath, FileMode.Create))
            {
                await video.CopyToAsync(stream);

                var videoInfo = await FFProbe.AnalyseAsync(videoPath);
                var duration = videoInfo.Duration;

                var interval = TimeSpan.FromSeconds(20); // Intervalo de 20 segundos entre frames

                for (var currentTime = TimeSpan.Zero; currentTime < duration; currentTime += interval)
                {
                    Console.WriteLine($"Processando frame: {currentTime}");

                    var finalPath = Path.Combine(videoOutputFolder, $"frame_at_{currentTime.TotalSeconds}.jpg");
                    FFMpeg.Snapshot(videoPath, finalPath, new Size(1920, 1080), currentTime);
                }

            }
        }
        finally
        {
            DirectoryExtensions.RemoveDirectory(videoPath);
        }
    }
}
