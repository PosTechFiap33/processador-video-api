using Microsoft.AspNetCore.Http;

namespace ProcessadorVideo.Domain.Adapters.Services;

public interface IVideoService
{
    Task GenerateImageFromFrames(IFormFile video, int frameInterval, string outputPath);
}
