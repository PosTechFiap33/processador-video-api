using System.Drawing;
using FFMpegCore;
using ProcessadorVideo.Domain.Adapters.Services;

namespace ProcessadorVideo.Infra.Services;

public class VideoService : IVideoService
{
    public async Task GenerateImageFromFrames(byte[] videoBytes, string videoName, int frameInterval, string outputPath)
    {
        string videoPath = Path.GetTempFileName();

        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(videoName);

        var videoOutputFolder = Path.Combine(outputPath, fileNameWithoutExtension);

        try
        {
            Directory.CreateDirectory(videoOutputFolder);

            await using (var stream = new FileStream(videoPath, FileMode.Create))
            {
                await stream.WriteAsync(videoBytes, 0, videoBytes.Length);
            }

            var videoInfo = await FFProbe.AnalyseAsync(videoPath);
            var duration = videoInfo.Duration;

            var interval = TimeSpan.FromSeconds(frameInterval); 

            for (var currentTime = TimeSpan.Zero; currentTime < duration; currentTime += interval)
            {
                Console.WriteLine($"Processando frame: {currentTime}");

                var finalPath = Path.Combine(videoOutputFolder, $"frame_at_{currentTime.TotalSeconds}.jpg");
                FFMpeg.Snapshot(videoPath, finalPath, new Size(1920, 1080), currentTime);
            }
        }
        finally
        {
            File.Delete(videoPath);
        }
    }
}
