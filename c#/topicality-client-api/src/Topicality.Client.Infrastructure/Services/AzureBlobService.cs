using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Topicality.Domain.Interfaces;

namespace Topicality.Client.Infrastructure.Services;

public class AzureBlobService :  IAzureBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<AzureBlobService> _logger;
    private readonly string _containerName;
    
    public AzureBlobService(BlobServiceClient blobServiceClient, ILogger<AzureBlobService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
        _containerName = "topicality";
    }
    
    public async Task<string> UploadFileAsync(string fileName, Stream fileStream,   string contentType)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });

            _logger.LogInformation($"File {fileName} uploaded successfully to container {_containerName}");
            
            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error uploading file {fileName} to container {_containerName}");
            throw;
        }
    }

    public async Task<Stream> DownloadFileAsync(string fileName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            var downloadInfo = await blobClient.DownloadAsync();
            return downloadInfo.Value.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error downloading file {fileName} from container {_containerName}");
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string fileName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            return await blobClient.DeleteIfExistsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting file {fileName} from container {_containerName}");
            throw;
        }
    }

    public async Task<bool> FileExistsAsync(string fileName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            return await blobClient.ExistsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking existence of file {fileName} in container {_containerName}");
            throw;
        }
    }
}