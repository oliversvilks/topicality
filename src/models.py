from pydantic import BaseModel
from typing import List, Optional
from datetime import datetime

class DocumentMetadata(BaseModel):
    title: str
    filename: str
    user: str
    DocumentCreated: datetime
    description: str
    source: str
    category: str
    extension: str
    datecreated: datetime
    dateupdated: datetime

class ProcessRequest(BaseModel):
    documents: List[DocumentMetadata]

class SchemaDefinition(BaseModel):
    title: str
    filename: str
    user: str
    description: str
    category: str
    datecreated: datetime
    dateupdated: datetime

class CopyDocumentRequest(BaseModel):
    source_category: str | None = None
    target_collection: str  # Added target collection
    target_category: str
    documents: List[str] | None = None  # Optional list of specific documents
    description: str | None = None

    class Config:
        schema_extra = {
            "example": {
                "source_category": "documentation",
                "target_collection": "new_collection",  # Example target collection
                "target_category": "approved_docs",
                "documents": ["doc1.pdf", "doc2.docx"],
                "description": "Copied from source collection"
            }
        }

class ContextFilter(BaseModel):
    collections: List[str]
    categories: List[str] = []

class ComparisonQueryRequest(BaseModel):
    question: str
    context1: ContextFilter
    context2: ContextFilter

    class Config:
        schema_extra = {
            "example": {
                "question": "Is the authentication mechanism in dev compatible with prod?",
                "context1": {
                    "collections": ["dev_docs"],
                    "categories": ["authentication", "security"]
                },
                "context2": {
                    "collections": ["prod_docs"],
                    "categories": ["authentication", "security"]
                }
            }
        }

class TextMetadata(BaseModel):
    user: str
    text: str  # Adding text field to store the content
    category: str = "dailies"  # Default category
    description: str
    datecreated: datetime
    dateupdated: datetime

    class Config:
        schema_extra = {
            "example": {
                "user": "admin",
                "text": "Today's meeting notes about authentication system",
                "des cription": "Meeting notes about auth system",
                "datecreated": "2024-04-11T10:00:00",
                "dateupdated": "2024-04-11T10:00:00"
            }
        }

class TextQueryFilter(BaseModel):
    user: str
    start_date: Optional[datetime] = None
    end_date: Optional[datetime] = None
    annotation_contains: Optional[str] = None

    class Config:
        schema_extra = {
            "example": {
                "user": "admin",
                "start_date": "2024-04-01T00:00:00",
                "end_date": "2024-04-11T23:59:59",
                "text_contains": "meeting"
            }
        }
        
class MultiContextComparisonRequest(BaseModel):
    question: str
    contexts: List[ContextFilter]

    class Config:
        schema_extra = {
            "example": {
                "question": "Compare implementation approaches across these contexts",
                "contexts": [
                    {
                        "collections": ["collection1"],
                        "categories": ["category1"]
                    },
                    {
                        "collections": ["collection2"],
                        "categories": ["category2"]
                    },
                    {
                        "collections": ["collection3"],
                        "categories": ["category3"]
                    }
                ]
            }
        }