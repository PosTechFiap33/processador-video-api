namespace ProcessadorVideo.Domain.Adapters.Services;

public interface IFileStorageService
{
    public Task Salvar(string path, string fileName, byte[] fileBytes, string contentType);
    public Task<byte[]> Ler(string path, string fileName);
    public Task Remover(string path, string fileName);
}
