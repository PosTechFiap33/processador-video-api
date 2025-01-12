namespace ProcessadorVideo.Domain.Adapters.Services;

public interface IFileStorageService
{
    public Task Salvar(string path, string fileName, Stream stream);
    public Task<byte[]> Ler(string path, string fileName);
}
