import pytest
from fastapi.testclient import TestClient
import os
import sys

# Add the project root directory to Python path
project_root = os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))
sys.path.insert(0, project_root)

from src.api import app

@pytest.fixture
def client():
    return TestClient(app)

@pytest.fixture
def mock_weaviate_processor(mocker):
    return mocker.patch('src.processors.weaviate_processor.WeaviateProcessor')