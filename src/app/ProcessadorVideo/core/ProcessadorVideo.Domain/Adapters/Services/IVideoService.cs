namespace ProcessadorVideo.Domain.Adapters.Services;

public interface IVideoService
{
    Task GenerateImageFromFrames(byte[] videoBytes, int frameInterval, string outputPath);
}
