using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Topicality.Client.Application.Dto;
using Topicality.Client.Application.Helpers;
using Topicality.Client.Application.Services;
using Topicality.Domain.Entities;
using Topicality.Domain.Interfaces;

namespace Topicality.Web.Controllers
{
    public class CategoryDocumentsController : Controller
    {
        private readonly ICategoryDocumentService _categoryDocumentService;
        private readonly ICategoryService _categoryService;
        private readonly IAzureBlobService _azureBlobService;
        private readonly IWeaviateApiService _weaviateApiService;
        public CategoryDocumentsController(ICategoryDocumentService categoryDocumentService,
             IAzureBlobService azureBlobService,
             IWeaviateApiService weaviateApiService,
             ICategoryService categoryService)
        {
            _categoryDocumentService = categoryDocumentService;
            _azureBlobService = azureBlobService;
            _weaviateApiService = weaviateApiService;
            _categoryService = categoryService;
        }

        // GET: CategoryDocuments
        public async Task<ActionResult> Index()
        {
            var categoryDocuments = await _categoryDocumentService.GetAllCategoryDocumentsAsync();
            return View(categoryDocuments);
        }

        // GET: CategoryDocuments/Details/5
        public async Task<ActionResult> Details(long id)
        {
            var categoryDocument = await _categoryDocumentService.GetCategoryDocumentByIdAsync(id);
            if (categoryDocument == null)
            {
                return NotFound();
            }
            return View(categoryDocument);
        }

        // GET: CategoryDocuments/Create
        // GET: CategoryDocuments/Create
        public IActionResult Create(long categoryId)
        {
            // Pass the categoryId to the view using ViewBag or ViewData
            ViewBag.CategoryId = categoryId;
            return View();
        }

        // POST: CategoryDocuments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDocument categoryDocument,  IFormFileCollection files)
        {
            var category = await _categoryService.GetCategoryByIdAsync((long)categoryDocument.CategoryId);
          //  if (category != null)
          //      categoryDocument.Category = category;
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Normalize the filename
                    string normalizedFileName = FileNameHelper.NormalizeFileName(file.FileName);

                    // Save the file temporarily to get metadata
                    var tempFilePath = Path.Combine(Path.GetTempPath(), normalizedFileName);
                    using (var stream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Get file metadata
                    FileInfo fileInfo = new FileInfo(tempFilePath);
                    DateTime creationDate = fileInfo.CreationTime;
                    DateTime lastModifiedDate = fileInfo.LastWriteTime;

                    // Set Title and Filename from the uploaded file
                    categoryDocument.DocumentMetadata = new DocumentMetadata
                    {
                        Title = file.FileName,
                        Filename = normalizedFileName,
                        DocumentCreated = creationDate,
                        DateCreated = creationDate,
                        DateUpdated = lastModifiedDate,
                        Description = categoryDocument.DocumentMetadata?.Description
                    };

                    using (var stream = file.OpenReadStream())
                    {
                        var blobName = $"{categoryDocument.DocumentMetadata.Filename}";
                       var blobUrl = await _azureBlobService.UploadFileAsync(blobName, stream, file.ContentType);
                       if (string.IsNullOrEmpty(categoryDocument.DocumentMetadata.Description))
                       {
                           categoryDocument.DocumentMetadata.Description = categoryDocument.DocumentMetadata.Title;
                       }
                       var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                       if (userId == null)
                       {
                           return NotFound();
                       }
                        var blobRequest = new BlobDocumentRequestDto
                        {
                            Title = categoryDocument.DocumentMetadata.Title,
                            User = User.Identity.Name.Replace('@','_').Replace('.','_'), // Assuming you have user identity set up
                            Category =  category.Name,
                            CategoryId = categoryDocument.CategoryId.ToString(),
                            Description = categoryDocument.DocumentMetadata.Description,
                            DocumentCreated = categoryDocument.DocumentMetadata.DocumentCreated,
                            DateCreated = categoryDocument.DocumentMetadata.DateCreated,
                            DateUpdated = categoryDocument.DocumentMetadata.DateUpdated,
                            Extension = Path.GetExtension(file.FileName).TrimStart('.'),
                            Source = blobUrl // Azure Blob URL
                        };

                        // Call the WeaviateService to process the document blob
                        var response = await _weaviateApiService.ProcessDocumentBlobAsync(blobRequest);
                    }

                    // Save the document metadata to the database
                    await _categoryDocumentService.CreateCategoryDocumentAsync(categoryDocument);
                }
            }

            ViewBag.CategoryId = categoryDocument.CategoryId;
            return RedirectToAction(nameof(ByCategory), new { id = categoryDocument.CategoryId });
            
                // Save the document to the database
                // Ensure that the CategoryId is set correctly in the categoryDocument object
                await _categoryDocumentService.CreateCategoryDocumentAsync(categoryDocument);
                return RedirectToAction(nameof(Index));
            
            return View(categoryDocument);
        }

        // GET: CategoryDocuments/Edit/5
        public async Task<ActionResult> Edit(long id)
        {
            var categoryDocument = await _categoryDocumentService.GetCategoryDocumentByIdAsync(id);
            if (categoryDocument == null)
            {
                return NotFound();
            }
            return View(categoryDocument);
        }

        // POST: CategoryDocuments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, CategoryDocument categoryDocument)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoryDocumentService.UpdateCategoryDocumentAsync(id, categoryDocument);
                if (result == null)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(categoryDocument);
        }

        // GET: CategoryDocuments/Delete/5
        public async Task<ActionResult> Delete(long id)
        {
            var categoryDocument = await _categoryDocumentService.GetCategoryDocumentByIdAsync(id);
            if (categoryDocument == null)
            {
                return NotFound();
            }
            return View(categoryDocument);
        }

        // POST: CategoryDocuments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            var result = await _categoryDocumentService.DeleteCategoryDocumentAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> ByCategory(long id)
        {
            var documents = await _categoryDocumentService.GetDocumentsByCategoryIdAsync(id);
            if (documents == null)
            {
                return NotFound();
            }
            ViewBag.CategoryId = id;
            return View(documents);
        }
    }
}
