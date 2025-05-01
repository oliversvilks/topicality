using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Topicality.Client.Application.Dto;
using Topicality.Client.Application.Services;
using Markdig;
using System.Text.Json;
namespace Topicality.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueryController : ControllerBase
{
    private readonly ICategoryService _categoryDocumentService;
    private readonly IWeaviateApiService _weaviateApiService;

    public QueryController(ICategoryService categoryDocumentService, IWeaviateApiService weaviateApiService)
    {
        _categoryDocumentService = categoryDocumentService;
        _weaviateApiService = weaviateApiService;
    }
    
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitSelection([FromBody] CategorySelectionDto model)
    {
        var userEmail = User.Identity.Name; // Assuming the user email is available here

        // Retrieve categories based on IDs
        //var categoryIds = model.Select(m => m.SelectedCategoryId).ToList();
        //var categories = await _categoryDocumentService.GetCategoriesByIdsAsync(categoryIds, userEmail);
        var catcount = model.Categories.Count;
        List<string> users = new List<string>();
        var user = User.Identity.Name.Replace("@", "_").Replace(".","_");
        for (int i = 0; i < catcount; i++)
        {
            
            users.Add(user);
        }
        // Construct the request object
        var request = new QueryRequest
        {
            Query = model.Question, // Replace with actual query
            Prompt = "answer from context", // Replace with actual prompt
            Users = users,
            Categories = model.Categories,
            
        };
        
        var result = await _weaviateApiService.QueryDocsAsync(request);
        var resp = JsonConvert.DeserializeObject<WeaviateQueryResponse>(result);
        return Ok(new { message = Markdown.ToHtml(resp.response._GenerativeReturn__generated) });
    }
    
    [HttpPost("vectorize/text")]
    public async Task<IActionResult> VectorizeText([FromBody] TextMetadataDto textMetadata)
    {

        var user = User.Identity.Name.Replace('@', '_').Replace('.', '_'); // Assuming you have user identity set up
        textMetadata.User = user;
        if (string.IsNullOrEmpty(textMetadata.Category))
            textMetadata.Category = "notes";
           // textMetadata.DateUpdated  = DateTime.Now;
           // textMetadata.DateCreated = DateTime.Now;
        var result = await _weaviateApiService.VectorizeTextAsync(textMetadata);
        
        return Ok(result);
    }
    
    [HttpPost("multicomparison")]
    public async Task<IActionResult> MultiComparison([FromBody] MultiContextComparisonRequestDto comparison)
    {

        var user = User.Identity.Name.Replace('@', '_').Replace('.', '_'); // Assuming you have user identity set up

        foreach (var context in comparison.Contexts)
        {
            var topicCount = context.Categories.Count();
            List<string> catalogs = new List<string>();
            for (int i = 0; i < topicCount; i++)
            {
                catalogs.Add(user);
            }
            context.Collections = catalogs;
        }
        string generatedContent = "no content";
        var result = await _weaviateApiService.CompareMultipleContextsAsync(comparison);
        using var doc = JsonDocument.Parse(result);
        if (doc.RootElement.TryGetProperty("analysis", out JsonElement generatedElement))
        {
            generatedContent  =  generatedElement.GetProperty("_GenerativeReturn__generated").ToString();
            
        }

        return Ok(Markdown.ToHtml(generatedContent));
    }
    
    [HttpPost("submitFlow")]
    public async Task<IActionResult> SubmitFlow([FromBody] FlowSubmissionDto flowData)
    {
        var user = User.Identity.Name.Replace('@', '_').Replace('.', '_');
        foreach (var f in flowData.flow)
        {
            foreach (var c in f.contexts)
            {
                var catcount = c.Categories.Count();
                List<string> catalogs = new List<string>();
                for (int i = 0; i < catcount; i++)
                {
                    catalogs.Add(user);
                }
                c.Collections = catalogs;
            }
        }
        try
        {
            var result = await _weaviateApiService.SubmitFLow(flowData);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error submitting flow: {ex.Message}");
        }
    }
}