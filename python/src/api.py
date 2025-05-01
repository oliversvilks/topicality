from fastapi import FastAPI, File, UploadFile, Form, HTTPException, Path, Query, Body
from fastapi.params import Path as FastAPIPath  # Change this import
from fastapi.responses import JSONResponse
from docling.document_converter import DocumentConverter
from pydantic import BaseModel
from typing import List
import shutil
import os
import json
from pathlib import Path
from .models import FlowNodeResponse,MultiTreeFlowRequest, MultiContextFlowRequest, FlowStep,  MultiContextComparisonRequest, BlobDocumentRequest, ProcessRequest, DocumentMetadata, SchemaDefinition, ComparisonQueryRequest, ContextFilter, CopyDocumentRequest, TextMetadata, TextQueryFilter
from .main import process_documents, setup_weaviate
from .processors.weaviate_processor import WeaviateProcessor
import weaviate.classes.config as wc
from datetime import datetime
from docling.datamodel.document import ConversionResult
from docling.document_converter import DocumentConverter
from docling_core.transforms.chunker import HierarchicalChunker
from docling_core.transforms.chunker import BaseChunker
from docling_core.transforms.chunker.hybrid_chunker import HybridChunker
from weaviate.classes.query import Filter
import logging
import openai
from azure.storage.blob import BlobServiceClient
import os
from urllib.parse import urlparse

# Configure logging
logging.basicConfig(level=logging.DEBUG)
logger = logging.getLogger(__name__)

app = FastAPI(
    title="Topicality Weaviate API",
    description="API for processing informtion contexts and documents",
    version="1.0.0"
)

DATA_DIR = Path("data/field_data")
DATA_DIR.mkdir(parents=True, exist_ok=True)

class QueryRequest(BaseModel):
    query: str
    prompt: str
    users: List[str] = []  # Optional list of users to filter by
    categories: List[str] = []  # Optional list of categories to filter by

    class Config:
        schema_extra = {
            "example": {
                "query": "bert",
                "prompt": "Explain how this works",
                "users": ["user1", "user2"],
                "categories": ["documentation", "technical"]
            }
        }

@app.on_event("startup")
async def startup_event():
    logger.debug("Starting up FastAPI application...")
    setup_weaviate()
    logger.debug("Startup complete")

@app.get("/")
def read_root():
    return {"message": "Hello from Azure!"}

@app.post("/process/blob/", tags=["Documents"])
async def process_document_blob(request: BlobDocumentRequest = Body(...)):
    try:
        # Download the file from Azure Blob Storage
        file_name = request.title
        if not file_name.endswith(f".{request.extension}"):
            file_name = f"{file_name}.{request.extension}"
        file_path = DATA_DIR / file_name


        # In your endpoint:
        container, blob = parse_blob_url(request.source)
        download_blob_to_file(container, blob, file_path)

        # Create document metadata using camelCase keys
        metadata = {
            "title": request.title,
            "filename": file_name,
            "user": request.user,
            "category": request.category,
            "categoryId": request.categoryId,
            "description": request.description,
            "document_created": request.documentCreated.isoformat(),
            "datecreated": request.dateCreated.isoformat(),
            "dateupdated": request.dateUpdated.isoformat(),
            "extension": request.extension,
            "source": request.source,
            "file_path": str(file_path)
        }

        # Process the document and save to Weaviate
        processor = WeaviateProcessor()
        if not processor.client.collections.exists(metadata["user"]):
            collection = processor.create_schema(metadata["user"])
            print(f"Created new collection for user: {metadata['user']}")
        else:
            collection = processor.client.collections.get(metadata["user"])
            print(f"Using existing collection for user: {metadata['user']}")

        # Convert document to text chunks
        converter = DocumentConverter()
        doc_result = converter.convert(str(file_path))
        conv_results_iter = converter.convert_all([str(file_path)])

        # Iterate over the generator to get a list of Docling documents
        docs = [doc_result.document for result in conv_results_iter]
        chunker = HybridChunker(max_tokens=8000)
        chunks = chunker.chunk(doc_result.document)

        for chunk in chunks:
            chunk_data = {
                **metadata,
                "text": chunk.text,
            }
            collection.data.insert(chunk_data)

        return JSONResponse(
            content={
                "status": "success",
                "message": "Blob document processed and stored successfully",
                "document": {
                    "title": request.title,
                    "filename": file_name,
                    "category": request.category,
                }
            }
        )

    except Exception as e:
        return JSONResponse(
            status_code=500,
            content={
                "status": "error",
                "message": str(e),
                "document": request.source
            }
        )
    finally:
        if 'processor' in locals():
            processor.client.close()

@app.post("/process/", tags=["Documents"])
async def process_document(
    file: UploadFile = File(...),
    title: str = Form(...),  
    user: str = Form(...),
    category: str = Form(...),
    categoryId: str = Form(...),
    description: str = Form(...),
    document_created: datetime = Form(...),
    datecreated: datetime = Form(...),
    dateupdated: datetime = Form(...),
    extension: str = Form(...),
    source: str = Form(...)
):
    try:
        # Save file
        file_path = DATA_DIR / file.filename
        with file_path.open("wb") as buffer:
            shutil.copyfileobj(file.file, buffer)

        # Create document metadata
        metadata = {
            "title": title,
            "filename": file.filename,
            "user": user,
            "category": category,
            "categoryId": categoryId,
            "description": description,
            "document_created": document_created.isoformat(),
            "datecreated": datecreated.isoformat(),
            "dateupdated": dateupdated.isoformat(),
            "extension": extension,
            "source": source,
            "file_path": str(file_path)
        }

        # Process the document and save to Weaviate
        processor = WeaviateProcessor()
        
        # Check if collection exists, if not create it
        if not processor.client.collections.exists(metadata["user"]):
            collection = processor.create_schema(metadata["user"])
            print(f"Created new collection for user: {metadata['user']}")
        else:
            collection = processor.client.collections.get(metadata["user"])
            print(f"Using existing collection for user: {metadata['user']}")

        # Convert document to text chunks
        converter = DocumentConverter()
        doc_result = converter.convert(str(file_path))
        # Directly pass list of files or streams to `convert_all`
        conv_results_iter = converter.convert_all([str(file_path)])

        # Iterate over the generator to get a list of Docling documents
        docs = [doc_result.document for result in conv_results_iter]
        # Create chunks with metadata
        chunker = HybridChunker(max_tokens=8000)  
        
        
        chunks = chunker.chunk(doc_result.document)
        # Initialize lists for text, and titles
        texts, titles = [], []
        # Save each chunk with metadata
        for chunk in chunks:
            chunk_data = {
                **metadata,  # Include all metadata
                "text": chunk.text,  # Add chunk text
            }
            collection.data.insert(chunk_data)

        return JSONResponse(
            content={
                "status": "success",
                "message": "Document processed and stored successfully",
                "document": {
                    "title": title,
                    "filename": file.filename,
                    "category": category,
                    #"chunks_processed": len(chunks)
                }
            }
        )

    except Exception as e:
        return JSONResponse(
            status_code=500,
            content={
                "status": "error",
                "message": str(e),
                "document": file.filename
            }
        )
    finally:
        file.file.close()
        if 'processor' in locals():
            processor.client.close()

def build_category_filter(categories):
    """Build an OR filter for multiple categories."""
    if not categories:
        return None
    # Start with the first category
    filter_query = Filter.by_property("category").equal(categories[0])
    # OR the rest
    for cat in categories[1:]:
        filter_query = filter_query | Filter.by_property("category").equal(cat)
    return filter_query

@app.post("/query")
async def query_docs(request: QueryRequest):
    """
    Query documents by categories within a collection
    
    Parameters:
    - **query**: Search query
    - **prompt**: Prompt for generative search
    - **categories**: Optional list of categories to filter by
    """
    try:
        processor = WeaviateProcessor()
        collection = processor.client.collections.get(request.users[0])  # Get the first user collection

        # Build where filter if categories are specified
        where_filter = build_category_filter(request.categories) if request.categories else None

        response = processor.query(
            query=request.query,
            prompt=request.prompt,
            collection=collection,
            where_filter=where_filter
        )
        
        return {
            "status": "success",
            "categories_filtered": request.categories if request.categories else "all",
            "response": response
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        processor.client.close()

@app.post("/collections/", tags=["Schema"], summary="Create a new collection")
async def create_collection(schema: SchemaDefinition):
    """
    Create a new Weaviate collection with the given schema
    
    - **title**: Collection name
    - **filename**: Source filename
    - **user**: User creating the collection
    - **description**: Collection description
    - **category**: Collection category
    - **datecreated**: Creation date
    - **dateupdated**: Last update date
    """
    try:
        processor = WeaviateProcessor()
        
        processor.create_schema(schema.title)
        
        return {
            "status": "success",
            "message": f"Collection {schema.title} created successfully",
            "collection": schema.dict()
        }
        
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.get("/collections", 
    tags=["Schema"],
    summary="List all collections",
    response_description="List of collections and their details")
async def list_collections():
    """
    Get a list of all available Weaviate collections
    """
    try:
        processor = WeaviateProcessor()
        # Get all collections using get_all() instead of list()
        collections = processor.client.collections.list_all()
        
        result = []
        for collection in collections:
            result.append({
                "name": collection,
               
            })
            
        return {
            "status": "success",
            "collections": result
        }
        
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        processor.client.close()

@app.get("/collections/{collection_name}/categories", 
    tags=["Schema"],
    summary="List categories in collection",
    response_description="List of unique categories")
async def list_categories(
    collection_name: str = FastAPIPath(
        default=...,
        title="Collection name",
        description="The name of the collection to get categories from"
    )
):
    """
    Get a list of unique categories defined in a collection
    
    Parameters:
    - **collection_name**: Name of the collection
    
    Returns:
    - List of unique category values
    """
    try:
        processor = WeaviateProcessor()
        
        if not processor.client.collections.exists(collection_name):
            raise HTTPException(
                status_code=404,
                detail=f"Collection '{collection_name}' not found"
            )
            
        collection = processor.client.collections.get(collection_name)
        
        result = collection.query.fetch_objects(
            return_properties=["category"]
        )
        
        # Extract unique categories using set comprehension
        categories = {obj.properties.get("category") for obj in result.objects if obj.properties.get("category")}
           
        return {
            "status": "success",
            "collection": collection_name,
            "categories": categories
        }
        
    except Exception as e:
        if isinstance(e, HTTPException):
            raise e
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        processor.client.close()

@app.get("/collections/{collection_name}/categories/{category}/documents", 
    tags=["Documents"],
    summary="List documents in category",
    response_description="List of documents and their sources")
async def list_category_documents(
    collection_name: str = FastAPIPath(
        default=..., 
        title="Collection name",
        description="The name of the collection to query"
    ),
    category: str = FastAPIPath(
        default=...,
        title="Category",
        description="Category to list documents from"
    )
):
    """
    Get a list of documents and their sources for a specific category
    
    Parameters:
    - **collection_name**: Name of the collection
    - **category**: Category to filter documents by
    
    Returns:
    - List of documents with their metadata including sources
    """
    try:
        processor = WeaviateProcessor()
        
        # Check if collection exists
        if not processor.client.collections.exists(collection_name):
            raise HTTPException(
                status_code=404,
                detail=f"Collection '{collection_name}' not found"
            )
        
        collection = processor.client.collections.get(collection_name)
        
        # Query documents with the specific category
        result = collection.query.fetch_objects(
            return_properties=[
                "title",
                "filename",
                "source",
                "description",
                "category",
                "extension",
                "datecreated",
                "dateupdated"
            ],
            where={
                "path": ["category"],
                "operator": "Equal",
                "valueText": category
            }
        )
        
        if not result.objects:
            return {
                "status": "success",
                "category": category,
                "documents": [],
                "count": 0
            }
            
        documents = []
        for obj in result.objects:
            documents.append({
                "title": obj.properties["title"],
                "filename": obj.properties["filename"],
                "source": obj.properties["source"],
                "description": obj.properties["description"],
                "extension": obj.properties["extension"],
                "datecreated": obj.properties["datecreated"],
                "dateupdated": obj.properties["dateupdated"]
            })
            
        return {
            "status": "success",
            "category": category,
            "documents": documents,
            "count": len(documents)
        }
        
    except Exception as e:
        if isinstance(e, HTTPException):
            raise e
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        processor.client.close()

@app.delete("/collections/{collection_name}", 
    tags=["Schema"],
    summary="Delete a collection",
    response_description="Delete operation status")
async def delete_collection(
    collection_name: str = FastAPIPath(
        default=...,  # Use default instead of ...
        title="Collection name",
        description="The name of the collection to delete"
    )
):
    """
    Delete a Weaviate collection by name
    
    Parameters:
    - **collection_name**: Name of the collection to delete
    
    Returns:
    - Success message if deleted
    - Error if collection doesn't exist or deletion fails
    """
    try:
        processor = WeaviateProcessor()
        
        # Check if collection exists
        if not processor.client.collections.exists(collection_name):
            raise HTTPException(
                status_code=404,
                detail=f"Collection '{collection_name}' not found"
            )
            
        # Delete the collection
        processor.client.collections.delete(collection_name)
        
        return {
            "status": "success",
            "message": f"Collection '{collection_name}' deleted successfully"
        }
        
    except Exception as e:
        if isinstance(e, HTTPException):
            raise e
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        processor.client.close()

@app.delete("/collections/{collection_name}/documents", 
    tags=["Documents"],
    summary="Delete a document from collection",
    response_description="Delete document operation status")
async def delete_document(
    collection_name: str =  FastAPIPath(
        default=..., 
        title="Collection name",
        description="The name of the collection containing the document"
    ),
    source: str = Query(
        ..., 
        title="Document source",
        description="URL or filename of the document to delete"
    )
):
    """
    Delete a document from a collection by its source (URL or filename)
    
    Parameters:
    - **collection_name**: Name of the collection containing the document
    - **source**: URL or filename of the document to delete
    
    Returns:
    - Success message if document deleted
    - Error if collection or document doesn't exist
    """
    try:
        processor = WeaviateProcessor()
        
        # Check if collection exists
        if not processor.client.collections.exists(collection_name):
            raise HTTPException(
                status_code=404,
                detail=f"Collection '{collection_name}' not found"
            )
        
        collection = processor.client.collections.get(collection_name)
        
        # Delete objects where filename matches source
        result = collection.data.delete_many(
            where={
                "path": ["filename"],
                "operator": "Equal",
                "valueText": source
            }
        )
        
        if result.matches == 0:
            raise HTTPException(
                status_code=404,
                detail=f"No documents found with source: {source}"
            )
            
        return {
            "status": "success",
            "message": f"Deleted {result.matches} documents with source: {source}",
            "deleted_count": result.matches
        }
        
    except Exception as e:
        if isinstance(e, HTTPException):
            raise e
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        processor.client.close()

@app.delete("/collections/{collection_name}/categories/{category}", 
    tags=["Documents"],
    summary="Delete all documents in a category",
    response_description="Delete category operation status")
async def delete_category_documents(
    collection_name: str = FastAPIPath(
        default=..., 
        title="Collection name",
        description="The name of the collection containing the documents"
    ),
    category: str = FastAPIPath(
        default=...,
        title="Category",
        description="Category to delete all documents from"
    )
):
    """
    Delete all documents with a specific category from a collection
    
    Parameters:
    - **collection_name**: Name of the collection
    - **category**: Category value to match for deletion
    
    Returns:
    - Success message with count of deleted documents
    - Error if collection doesn't exist or no documents found
    """
    try:
        processor = WeaviateProcessor()
        
        # Check if collection exists
        if not processor.client.collections.exists(collection_name):
            raise HTTPException(
                status_code=404,
                detail=f"Collection '{collection_name}' not found"
            )
        
        collection = processor.client.collections.get(collection_name)
        
        # Delete all objects with matching category
        result = collection.data.delete_many(
            where={
                "path": ["category"],
                "operator": "Equal",
                "valueText": category
            }
        )
        
        if result.matches == 0:
            raise HTTPException(
                status_code=404,
                detail=f"No documents found with category: {category}"
            )
            
        return {
            "status": "success",
            "message": f"Deleted {result.matches} documents with category: {category}",
            "deleted_count": result.matches,
            "category": category
        }
        
    except Exception as e:
        if isinstance(e, HTTPException):
            raise e
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        processor.client.close()

@app.post("/collections/{source_collection}/copy", 
    tags=["Documents"],
    summary="Copy documents between collections",
    response_description="Copy operation status")
async def copy_documents(
    source_collection: str = FastAPIPath(
        default=..., 
        title="Source collection name",
        description="The collection to copy from"
    ),
    request: CopyDocumentRequest = Body(...)
):
    """
    Copy documents between collections with category filtering
    
    Parameters:
    - **source_collection**: Name of the source collection
    - **request.target_collection**: Name of target collection
    - **request.source_category**: Optional category to filter source documents
    - **request.target_category**: Category to assign to copied documents
    - **request.documents**: Optional list of specific documents to copy
    - **request.description**: Optional description for copied documents
    """
    try:
        processor = WeaviateProcessor()
        
        # Check source collection exists
        if not processor.client.collections.exists(source_collection):
            raise HTTPException(
                status_code=404,
                detail=f"Source collection '{source_collection}' not found"
            )
        
        # Create target collection if it doesn't exist
        if not processor.client.collections.exists(request.target_collection):
            target_coll = processor.create_schema(request.target_collection)
            print(f"Created new collection: {request.target_collection}")
        else:
            target_coll = processor.client.collections.get(request.target_collection)
        
        source_coll = processor.client.collections.get(source_collection)
        
        # Build query filters
        filters = []
        
        # Add category filter if specified
        if request.source_category:
            filters.append({
                "path": ["category"],
                "operator": "Equal",
                "valueText": request.source_category
            })
        
        # Add document filter if specified
        if request.documents:
            filters.append({
                "path": ["filename"],
                "operator": "ContainsAny",
                "valueText": request.documents
            })
        
        # Combine filters with AND if multiple exist
        where_filter = None
        if filters:
            if len(filters) > 1:
                where_filter = {
                    "operator": "And",
                    "operands": filters
                }
            else:
                where_filter = filters[0]
        
        # Query source documents
        results = source_coll.query.fetch_objects(
            return_properties=[
                "text", "title", "filename", "description",
                "extension", "source", "chunk_index",
                "datecreated", "dateupdated"
            ],
            where=where_filter
        )
        
        if not results.objects:
            return {
                "status": "warning",
                "message": "No matching documents found to copy",
                "source": {
                    "collection": source_collection,
                    "category": request.source_category
                },
                "target": {
                    "collection": request.target_collection,
                    "category": request.target_category
                },
                "copied_count": 0
            }
        
        # Copy documents to target collection
        copied_count = 0
        for obj in results.objects:
            properties = obj.properties
            # Update metadata for copied document
            properties.update({
                "category": request.target_category,
                "description": request.description or properties.get("description", ""),
                "dateupdated": datetime.now().isoformat()
            })
            
            target_coll.data.insert(properties)
            copied_count += 1
        
        return {
            "status": "success",
            "message": f"Copied {copied_count} documents successfully",
            "source": {
                "collection": source_collection,
                "category": request.source_category
            },
            "target": {
                "collection": request.target_collection,
                "category": request.target_category
            },
            "copied_count": copied_count
        }
        
    except Exception as e:
        if isinstance(e, HTTPException):
            raise e
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        processor.client.close()

@app.post("/compare", 
    tags=["Analysis"],
    summary="Compare contexts from different collections/categories")
async def compare_contexts(request: ComparisonQueryRequest):
    """Compare two different contexts and analyze their compatibility"""
    try:
        processor = WeaviateProcessor()
        
        async def get_context(context_filter: ContextFilter):
            results = []
            for collection_name in context_filter.collections:
                if not processor.client.collections.exists(collection_name):
                    raise HTTPException(
                        status_code=404,
                        detail=f"Collection '{collection_name}' not found"
                    )
                
                collection = processor.client.collections.get(collection_name)
                
                # Build filters using filter builder pattern
                filter_query = None
                if context_filter.categories:
                    print(context_filter.categories[0])
                    # Create base filter for first category
                    filter_query = Filter.by_property("category").equal(context_filter.categories[0])
                    
                    # Add additional categories with OR operator
                    for category in context_filter.categories[1:]:
                        print(category)
                        filter_query = filter_query | Filter.by_property("category").equal(category)
                

                # Execute query with properties and filter
                print(filter_query)
                response = collection.query.fetch_objects(
                    return_properties=["text", "category", "description"],
                    filters=(filter_query),  # Now filter_query is always defined
                    limit=10
                )
                
                # Extract and format results
                if response and hasattr(response, 'objects'):
                    context_data = [
                        {
                            "text": obj.properties.get("text", ""),
                            "category": obj.properties.get("category", ""),
                            "description": obj.properties.get("description", "")
                        } for obj in response.objects
                    ]
                    results.extend(context_data)
           
            return results

        # Get both contexts
        context1_results = await get_context(request.context1)
        print("results1") 
        context2_results = await get_context(request.context2)
        print("results2") 

        # Format contexts for comparison
        context1_text = "\n".join([f"- {r['text']}" for r in context1_results])
        context2_text = "\n".join([f"- {r['text']}" for r in context2_results])
        
        # Perform comparison analysis
        comparison_prompt = f"""
        Compare the following two contexts and answer: {request.question}
        
        Context 1 (from {', '.join(request.context1.collections)}, categories: {', '.join(request.context1.categories)}):
        {context1_text}
        
        Context 2 (from {', '.join(request.context2.collections)}, categories: {', '.join(request.context2.categories)}):
        {context2_text}
        
        Provide a detailed analysis of compatibility or relationships between these contexts.
        """
        
        # Get comparison analysis using the first collection
        print("before collection")
        collection = processor.client.collections.get(request.context1.collections[0])
        comparison_analysis = processor.query(
            query=request.question,
            prompt=comparison_prompt,
            collection=collection
        )
        
        return {
            "status": "success",
            "question": request.question,
            "context1": {
                "collections": request.context1.collections,
                "categories": request.context1.categories,
                "results": context1_results
            },
            "context2": {
                "collections": request.context2.collections,
                "categories": request.context2.categories,
                "results": context2_results
            },
            "analysis": comparison_analysis
        }
        
    except Exception as e:
        if isinstance(e, HTTPException):
            raise e
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        processor.client.close()

@app.post("/vectorize/text", 
    tags=["Vectorization"],
    summary="Vectorize text and store in user's collection",
    response_description="Vectorization status")
async def vectorize_text(text_metadata: TextMetadata):
    """
    Vectorize text and store in user's collection with metadata
    
    Parameters:
    - **user**: User identifier
    - **text**: Text content to vectorize
    - **annotation**: Note or description about the text
    - **datecreated**: Creation timestamp
    - **dateupdated**: Update timestamp
    """
    try:
        processor = WeaviateProcessor()
        
        # Check if user collection exists, if not create it
        if not processor.client.collections.exists(text_metadata.user):
            collection = processor.create_schema(text_metadata.user)
            print(f"Created new collection for user: {text_metadata.user}")
        else:
            collection = processor.client.collections.get(text_metadata.user)
            print(f"Using existing collection for user: {text_metadata.user}")

        # Prepare metadata for storage
        metadata_dict = {
            "text": text_metadata.text,
            "category": text_metadata.category,
            "description": text_metadata.description,
            "datecreated": text_metadata.datecreated,
            "dateupdated": text_metadata.dateupdated
        }

        # Store text with metadata
        result = collection.data.insert(metadata_dict)

        return JSONResponse(
            content={
                "status": "success",
                "message": "Text vectorized and stored successfully",
                "metadata": {
                    "user": text_metadata.user,
                    "category": text_metadata.category,
                    "description": text_metadata.description,
                    "vector_id": str(result)
                }
            }
        )

    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail=f"Error vectorizing text: {str(e)}"
        )
    finally:
        processor.client.close()

@app.post("/compare/multi", 
    tags=["Analysis"],
    summary="Compare multiple contexts")
async def compare_multiple_contexts(request: MultiContextComparisonRequest):
    """
    Compare multiple contexts and analyze their relationships
    
    Parameters:
    - **question**: The analysis question to answer
    - **contexts**: List of context filters (collections and categories)
    """
    try:
        processor = WeaviateProcessor()
        
        async def get_context(context_filter: ContextFilter):
            results = []
            for collection_name in context_filter.collections:
                if not processor.client.collections.exists(collection_name):
                    raise HTTPException(
                        status_code=404,
                        detail=f"Collection '{collection_name}' not found"
                    )
                
                collection = processor.client.collections.get(collection_name)
                
                # Build filters using filter builder pattern
                filter_query = None
                if context_filter.categories:
                    # Create base filter for first category
                    filter_query = Filter.by_property("category").equal(context_filter.categories[0])
                    
                    # Add additional categories with OR operator
                    for category in context_filter.categories[1:]:
                        filter_query = filter_query | Filter.by_property("category").equal(category)

                # Execute query with properties and filter
                response = collection.query.fetch_objects(
                    return_properties=["text", "category", "description"],
                    filters=filter_query,
                    limit=10
                )
                
                # Extract and format results
                if response and hasattr(response, 'objects'):
                    context_data = [
                        {
                            "text": obj.properties.get("text", ""),
                            "category": obj.properties.get("category", ""),
                            "description": obj.properties.get("description", "")
                        } for obj in response.objects
                    ]
                    results.extend(context_data)
           
            return results

        # Get all contexts
        contexts_results = []
        for context in request.contexts:
            results = await get_context(context)
            formatted_text = "\n".join([f"- {r['text']}" for r in results])
            contexts_results.append({
                "collections": context.collections,
                "categories": context.categories,
                "results": results,
                "formatted_text": formatted_text
            })

        # Build multi-context comparison prompt
        comparison_prompt = f"""
        Compare the following {len(request.contexts)} contexts and answer: {request.question}
        
        {'\n\n'.join([
            f"Context {i+1} (from {', '.join(ctx['collections'])}, "
            f"categories: {', '.join(ctx['categories'])}):\n{ctx['formatted_text']}"
            for i, ctx in enumerate(contexts_results)
        ])}
        
        Provide a detailed analysis addressing:
        1. Key characteristics of each context
        2. Relationships and patterns between contexts
        3. Notable similarities or differences
        4. Potential conflicts or inconsistencies
        5. Overall compatibility assessment
        """
        
        # Use the first collection for analysis
        collection = processor.client.collections.get(request.contexts[0].collections[0])
        analysis = processor.query(
            query=request.question,
            prompt=comparison_prompt,
            collection=collection
        )
        
        return {
            "status": "success",
            "question": request.question,
            "context_count": len(request.contexts),
            "contexts": [
                {
                    "collections": ctx["collections"],
                    "categories": ctx["categories"],
                    "results": ctx["results"]
                } for ctx in contexts_results
            ],
            "analysis": analysis
        }
        
    except Exception as e:
        if isinstance(e, HTTPException):
            raise e
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        processor.client.close()

# Dummy OpenAI call
async def ask_openai(prompt: str) -> str:
    # Replace with your real OpenAI call
    response = await openai.ChatCompletion.acreate(
        model="gpt-4-turbo",
        messages=[
            {"role": "system", "content": "You are a strategic AI assistant."},
            {"role": "user", "content": prompt}
        ],
        temperature=0.3
    )
    return response.choices[0].message.content.strip()

openai_client = openai.OpenAI()

def get_category_texts(collection: str, category: str) -> List[str]:
    # Replace this with actual Weaviate search
    mock_data = {
        "second": ["its larger than 10, but is it odd or even"],
        "third": ["it's smaller than 10 but is it odd or even?"],
        "fourth": ["its even"],
        "fifth": ["It's odd"]
    }
    return mock_data.get(category, [f"No info found for {category}"])

# ---- The upgraded route ----
@app.post("/flow/multi_tree", tags=["Flow"])
async def flow_multi_tree(request: MultiTreeFlowRequest):
    try:
        processor = WeaviateProcessor()

        async def get_context(context_filter: ContextFilter):
            results = []
            for collection_name in context_filter.collections:
                if not processor.client.collections.exists(collection_name):
                    raise HTTPException(
                        status_code=404,
                        detail=f"Collection '{collection_name}' not found"
                    )
                
                collection = processor.client.collections.get(collection_name)

                # Build category OR filter
                filter_query = None
                if context_filter.categories:
                    filter_query = Filter.by_property("category").equal(context_filter.categories[0])
                    for cat in context_filter.categories[1:]:
                        filter_query = filter_query | Filter.by_property("category").equal(cat)

                response = collection.query.fetch_objects(
                    return_properties=["text", "category", "description"],
                    filters=filter_query,
                    limit=10
                )

                if response and hasattr(response, 'objects'):
                    context_data = [
                        {
                            "text": obj.properties.get("text", ""),
                            "category": obj.properties.get("category", ""),
                            "description": obj.properties.get("description", "")
                        } for obj in response.objects
                    ]
                    results.extend(context_data)

            return results

        # --- STEP 1: Build flow lookup ---
        flow_map = {step.entrance_category: step for step in request.flow}
        journey: List[FlowNodeResponse] = []
        current_category = request.flow[0].entrance_category
        current_question = request.question

        # --- STEP 2: Iteratively process each flow step ---
        while current_category:
            current_step = flow_map.get(current_category)
            if not current_step:
                break

            # --- STEP 2.1: Fetch and format context for current step ---
            formatted_context = []
            for context in current_step.contexts:
                results = await get_context(context)
                context_text = "\n".join([f"- {r['text']}" for r in results])
                formatted_context.append(f"Collections: {', '.join(context.collections)} | Categories: {', '.join(context.categories)}\n{context_text}")
            
            context_text_block = "\n\n".join(formatted_context)

            # --- STEP 2.2: Construct prompt ---
            available_categories = list(set(sum([ctx.categories for ctx in current_step.contexts], [])))
            system_prompt = "You are a decision-making assistant navigating through multiple topics. Always respond only in JSON format."

            user_prompt = f"""
You are answering the question: "{current_question}"

You are currently at the entrance category: "{current_category}"

Here is the information available:
{context_text_block}

But also take reasoning forces into account to make decisions.

Respond strictly in JSON format like this:
{{
  "response": "Your answer to the question",
  "next_category": "The chosen next category (must be one of {', '.join(available_categories)} or empty if end)",
  "reason": "Why you chose this category",
  "next_question": "Refined question to ask at next step"
}}
"""

            # --- STEP 2.3: Ask OpenAI to make decision ---
            response = openai_client.chat.completions.create(
                model="gpt-4o",
                messages=[
                    {"role": "system", "content": system_prompt},
                    {"role": "user", "content": user_prompt}
                ],
                functions=[
                    {
                        "name": "flow_step",
                        "description": "Make a decision about the next flow step",
                        "parameters": {
                            "type": "object",
                            "properties": {
                                "response": {"type": "string"},
                                "next_category": {"type": "string"},
                                "reason": {"type": "string"},
                                "next_question": {"type": "string"}
                            },
                            "required": ["response", "next_category", "reason", "next_question"]
                        }
                    }
                ],
                function_call={"name": "flow_step"}
            )

            result_args = json.loads(response.choices[0].message.function_call.arguments)

            # --- STEP 2.4: Save step result ---
            journey.append(FlowNodeResponse(
                entrance_category=current_category,
                chosen_category=result_args["next_category"],
                reason=result_args["reason"],
                refined_question=result_args["next_question"],
                response=result_args["response"]
            ))

            # --- STEP 2.5: Prepare for next step ---
            current_category = result_args["next_category"] or None
            current_question = result_args["next_question"] or current_question

        # --- STEP 3: Return the journey ---
        return {
            "status": "success",
            "initial_question": request.question,
            "journey": [node.dict() for node in journey]
        }

    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))



def parse_blob_url(url):
    # Example: https://account.blob.core.windows.net/container/blob
    parts = urlparse(url)
    path_parts = parts.path.lstrip('/').split('/', 1)
    return path_parts[0], path_parts[1]  # container, blob


def download_blob_to_file(container_name: str, blob_name: str, file_path: str):
    # Get connection string from environment variable or Azure App Service settings
    conn_str = os.environ["AZURE_STORAGE_CONNECTION_STRING"]
    blob_service_client = BlobServiceClient.from_connection_string(conn_str)
    container_client = blob_service_client.get_container_client(container_name)
    blob_client = container_client.get_blob_client(blob_name)
    with open(file_path, "wb") as f:
        download_stream = blob_client.download_blob()
        f.write(download_stream.readall())

def build_comparison_prompt(contexts_results, question, entrance_category, available_categories):
    prompt = f"""
You are guiding a user through an analysis flow.

The entrance category is: **{entrance_category}**.

Available categories to move to next: {', '.join(available_categories)}.

Main Question: {question}

Here are the current contexts:

{'\n\n'.join([
    f"Context {i+1} (collections: {', '.join(ctx['collections'])}, categories: {', '.join(ctx['categories'])}):\n{ctx['formatted_text']}"
    for i, ctx in enumerate(contexts_results)
])}

Instructions:
- Based on the entrance, decide the best next category.
- If no good next category remains, set finished=true.
- Otherwise, pick next_category, explain your reasoning, and create a next_question.
"""
    return prompt

flow_tool = {
    "type": "function",
    "function": {
        "name": "decide_next_category",
        "description": "Decide the next category to focus on based on the entrance analysis. If no good next step, mark finished=true.",
        "parameters": {
            "type": "object",
            "properties": {
                "next_category": {
                    "type": "string",
                    "description": "The category to explore next."
                },
                "reason": {
                    "type": "string",
                    "description": "Why this category was chosen or why the flow should stop."
                },
                "next_question": {
                    "type": "string",
                    "description": "The follow-up question for the next category."
                },
                "finished": {
                    "type": "boolean",
                    "description": "Whether the flow is finished."
                }
            },
            "required": ["reason", "finished"]
        }
    }
}