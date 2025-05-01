using FluentResults;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Topicality.Client.Application.Services;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using System.Reflection.PortableExecutable;
using Topicality.Client.Application;
using Topicality.Client.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Xml.Linq;
using Topicality.Client.Application.Dto;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net.Http.Json;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Topicality.Client.Infrastructure.Services
{
    public class WeaviateApiService : IWeaviateApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseAddress;


        public WeaviateApiService()
        {
            _httpClient = new HttpClient();
            _baseAddress =
                "https://topicality-weaviate-dzefhwf9fzamfccu.westus2-01.azurewebsites.net/"; // configuration["WeaviateApiUrl"]; // Get the URL from configuration
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.BaseAddress = new Uri(_baseAddress);

        }
      //  public WeaviateApiService(HttpClient httpClient, string baseAddress)
     //   {
     //       _httpClient = httpClient;
      //      _baseAddress = baseAddress;
      //      _httpClient.BaseAddress = new Uri(baseAddress);
      //             }

        public async Task<string> ProcessDocumentAsync(DocumentMetadataDto documentMetadata, Stream fileStream)
    {
        var content = new MultipartFormDataContent();
        content.Add(new StreamContent(fileStream), "file", "file.txt");
        content.Add(new StringContent(documentMetadata.Title), "title");
        content.Add(new StringContent(documentMetadata.UserId), "user");
        content.Add(new StringContent(documentMetadata.Category), "category");
        content.Add(new StringContent(documentMetadata.CategoryId), "categoryId");
        content.Add(new StringContent(documentMetadata.Description), "description");
        content.Add(new StringContent(documentMetadata.DocumentCreated.ToString()), "document_created");
        content.Add(new StringContent(documentMetadata.DateCreated.ToString()), "datecreated");
        content.Add(new StringContent(documentMetadata.DateUpdated.ToString()), "dateupdated");
        content.Add(new StringContent(documentMetadata.Extension), "extension");
        content.Add(new StringContent(documentMetadata.Source), "source");

        var response = await _httpClient.PostAsync("/process/", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
        public async Task<HttpResponseMessage> ProcessDocumentBlobAsync(BlobDocumentRequestDto request)
        {
           
            // Ensure DateTime properties are in ISO 8601 format
            var jsonSettings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

// Add the custom DateTime converter
            jsonSettings.Converters.Add(new IsoDateTimeConverter());

            var jsonContent = JsonSerializer.Serialize(request, jsonSettings);
            var response = await _httpClient.PostAsync("/process/blob/", new StringContent(jsonContent, Encoding.UTF8, "application/json"));
            
            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<string> SubmitFLow(FlowSubmissionDto flowData)
        {
            var jsonSettings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            var textstring = JsonConvert.SerializeObject(flowData);
            var jsonContent = new StringContent(JsonSerializer.Serialize(flowData, jsonSettings), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/flow/multi_tree", jsonContent);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> QueryDocsAsync(QueryRequest queryRequest)
    {
        var jsonSettings = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        var textcongtent = JsonConvert.SerializeObject(queryRequest);
        var jsonContent = new StringContent(JsonSerializer.Serialize(queryRequest, jsonSettings), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/query", jsonContent);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();;
    }

    public async Task<string> CreateCollectionAsync(SchemaDefinitionDto schemaDefinition)
    {
        var jsonContent = new StringContent(JsonSerializer.Serialize(schemaDefinition), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/collections/", jsonContent);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> ListCollectionsAsync()
    {
        var response = await _httpClient.GetAsync("/collections");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> ListCategoriesAsync(string collectionName)
    {
        var response = await _httpClient.GetAsync($"/collections/{collectionName}/categories");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> ListCategoryDocumentsAsync(string collectionName, string category)
    {
        var response = await _httpClient.GetAsync($"/collections/{collectionName}/categories/{category}/documents");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> DeleteCollectionAsync(string collectionName)
    {
        var response = await _httpClient.DeleteAsync($"/collections/{collectionName}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> DeleteDocumentAsync(string collectionName, string source)
    {
        var response = await _httpClient.DeleteAsync($"/collections/{collectionName}/documents?source={source}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> DeleteCategoryDocumentsAsync(string collectionName, string category)
    {
        var response = await _httpClient.DeleteAsync($"/collections/{collectionName}/categories/{category}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> CopyDocumentsAsync(string sourceCollection, CopyDocumentRequestDto request)
    {
        var jsonContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"/collections/{sourceCollection}/copy", jsonContent);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> CompareContextsAsync(ComparisonQueryRequestDto request)
    {
        var jsonContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/compare", jsonContent);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> VectorizeTextAsync(TextMetadataDto textMetadata)
    {
        var jsonSettings = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

// Add the custom DateTime converter
        jsonSettings.Converters.Add(new IsoDateTimeConverter());
        var textContent = JsonConvert.SerializeObject(textMetadata);
        var jsonContent = new StringContent(JsonSerializer.Serialize(textMetadata, jsonSettings), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/vectorize/text", jsonContent);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> CompareMultipleContextsAsync(MultiContextComparisonRequestDto request)
    {
        var jsonSettings = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        var textstring = JsonConvert.SerializeObject(request);
        var jsonContent = new StringContent(JsonSerializer.Serialize(request, jsonSettings), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/compare/multi", jsonContent);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
        
    }
    public class IsoDateTimeConverter : System.Text.Json.Serialization.JsonConverter<DateTime>
    {
        private readonly string _format = "yyyy-MM-ddTHH:mm:ssZ"; // ISO 8601 format

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format));
        }
    }
}
