namespace Topicality.Domain.Interfaces;

public interface IAzureBlobService
{
    Task<string> UploadFileAsync(string fileName, Stream fileStream,  string contentType);
    Task<Stream> DownloadFileAsync(string fileName);
    Task<bool> DeleteFileAsync(string fileName);
    Task<bool> FileExistsAsync(string fileName);
}