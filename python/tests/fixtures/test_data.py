import pytest
from datetime import datetime

@pytest.fixture
def sample_document_request():
    return {
        "title": "test.pdf",
        "user": "test_user",
        "category": "test_category",
        "categoryId": "1",
        "description": "test description",
        "documentCreated": "2025-04-26T23:02:28Z",
        "dateCreated": "2025-04-26T23:02:28Z",
        "dateUpdated": "2025-04-26T23:02:28Z",
        "extension": "pdf",
        "source": "https://test.blob.core.windows.net/container/test.pdf"
    }