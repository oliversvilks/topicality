from pydantic import BaseModel, Field
from typing import List, Optional
from datetime import datetime
from datetime import timezone

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

def to_rfc3339(dt: datetime) -> str:
    # Ensure UTC and RFC3339 format with 'Z'
    if dt.tzinfo is None:
        dt = dt.replace(tzinfo=timezone.utc)
    return dt.isoformat().replace("+00:00", "Z")

class TextMetadata(BaseModel):
    user: str
    text: str  # Adding text field to store the content
    category: str = "notes"  # Default category
    description: str
    datecreated: datetime =  to_rfc3339(datetime.utcnow())
    dateupdated: datetime = to_rfc3339(datetime.utcnow())


    class Config:
        schema_extra = {
            "example": {
                "user": "admin",
                "text": "Today's meeting notes about authentication system",
                "description": "Meeting notes about auth system",
                "datecreated": "2024-04-11T10:00:00",
                "dateupdated": "2024-04-11T10:00:00"
            }
        }

class MultiContextFlowRequest(BaseModel):
    question: str
    entrance_category: str
    contexts: List[ContextFilter]

class TreeNode(BaseModel):
    entrance_category: str
    contexts: List[ContextFilter]

class FlowStep(BaseModel):
    entrance_category: str
    contexts: List[ContextFilter]

class MultiTreeFlowRequest(BaseModel):
    question: str
    flow: List[FlowStep]

# --- Response tracking ---
class FlowNodeResponse(BaseModel):
    entrance_category: str
    chosen_category: Optional[str]
    reason: Optional[str]
    refined_question: Optional[str]
    response: Optional[str]

class FlowStep(BaseModel):
    entrance_category: str
    response: str  # <--- NEW: answer at this node
    next_category: Optional[str]
    reason: Optional[str]
    next_question: Optional[str]
    finished: bool

# ---- Models ----
class Context(BaseModel):
    collections: List[str]
    categories: List[str]

class FlowStep(BaseModel):
    entrance_category: str
    contexts: List[Context]

class FlowRequest(BaseModel):
    question: str
    flow: List[FlowStep]

class JourneyStep(BaseModel):
    entrance_category: str
    chosen_category: str
    reason: str
    refined_question: str
    response: str

class FlowResponse(BaseModel):
    status: str
    initial_question: str
    journey: List[JourneyStep]

class BlobDocumentRequest(BaseModel):
    title: str
    user: str
    category: str
    categoryId: str
    description: str
    documentCreated: datetime = Field(..., alias="documentCreated")
    dateCreated: datetime = Field(..., alias="dateCreated")
    dateUpdated: datetime = Field(..., alias="dateUpdated")
    extension: str
    source: str  # Azure Blob URL

    class Config:
        allow_population_by_field_name = True
        schema_extra = {
            "example": {
                "title": "tunesol-thermal-diffuse-en-pdf.pdf",
                "user": "olvio@inbox.lv",
                "category": "test2",
                "categoryId": "4",
                "description": "tunesol-thermal-diffuse-en-pdf.pdf",
                "documentCreated": "2025-04-26T23:02:28Z",
                "dateCreated": "2025-04-26T23:02:28Z",
                "dateUpdated": "2025-04-26T23:02:28Z",
                "extension": "pdf",
                "source": "https://topicalityhub6820989656.blob.core.windows.net/topicality/tunesol-thermal-diffuse-en-pdf.pdf"
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