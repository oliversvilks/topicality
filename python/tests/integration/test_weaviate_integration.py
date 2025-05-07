import pytest
from src.processors.weaviate_processor import WeaviateProcessor

@pytest.mark.integration
class TestWeaviateIntegration:
    def test_connection(self):
        processor = WeaviateProcessor()
        assert processor.client.is_ready()

    def test_create_schema(self):
        processor = WeaviateProcessor()
         # Check if collection exists
        
            
        # Delete the collection
        processor.client.collections.delete("test_collection")
        if not processor.client.collections.exists("test_collection"):
            collection = processor.create_schema("test_collection")
            assert processor.client.collections.exists("test_collection")