namespace ProcessadorVideo.Domain.Adapters.Services;

public interface IVideoService
{
    Task GenerateImageFromFrames(byte[] videoBytes, string videoName, int frameInterval, string outputPath);
}
