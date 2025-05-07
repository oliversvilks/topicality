import pytest
from fastapi.testclient import TestClient
from src.api import app
from src.models import BlobDocumentRequest
from datetime import datetime

def test_read_root(client):
    response = client.get("/")
    assert response.status_code == 200
    assert response.json() == {"message": "Hello from Azure!"}

def test_process_document_blob(client, mock_weaviate_processor):
    test_data = {
        "title": "test.pdf",
        "user": "test_user",
        "category": "test_category",
        "categoryId": "1",
        "description": "test description",
        "documentCreated": "2025-04-26T23:02:28Z",
        "dateCreated": "2025-04-26T23:02:28Z",
        "dateUpdated": "2025-04-26T23:02:28Z",
        "extension": "pdf",
        "source": "https://topicalityhub6820989656.blob.core.windows.net/topicality/Here.docx"
    }
    
    response = client.post("/process/blob/", json=test_data)
    assert response.status_code == 200
    assert response.json()["status"] == "success"