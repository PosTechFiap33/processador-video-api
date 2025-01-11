using Microsoft.AspNetCore.Http;

namespace ProcessadorVideo.Domain.Services;

public interface IVideoService
{
    Task GenerateImageFromFrames(IFormFile video, int frameInterval, string outputPath);
}
