using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topicality.Client.Application.Dto;

namespace Topicality.Client.Application.Services
{
    public interface IWeaviateApiService
    {
        Task<string> ProcessDocumentAsync(DocumentMetadataDto documentMetadata, Stream fileStream);
        Task<string> QueryDocsAsync(QueryRequest queryRequest);
        Task<string> CreateCollectionAsync(SchemaDefinitionDto schemaDefinition);
        Task<string> ListCollectionsAsync();
        Task<string> ListCategoriesAsync(string collectionName);
        Task<string> ListCategoryDocumentsAsync(string collectionName, string category);
        Task<string> DeleteCollectionAsync(string collectionName);
        Task<string> DeleteDocumentAsync(string collectionName, string source);
        Task<string> DeleteCategoryDocumentsAsync(string collectionName, string category);
        Task<string> CopyDocumentsAsync(string sourceCollection, CopyDocumentRequestDto request);
        Task<string> CompareContextsAsync(ComparisonQueryRequestDto request);
        Task<string> VectorizeTextAsync(TextMetadataDto textMetadata);
        Task<string> CompareMultipleContextsAsync(MultiContextComparisonRequestDto request);
        Task<HttpResponseMessage> ProcessDocumentBlobAsync(BlobDocumentRequestDto request);
        
        Task<string> SubmitFLow(FlowSubmissionDto flowData);
    }
}
