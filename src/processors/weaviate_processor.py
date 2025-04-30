import weaviate
import os
from dotenv import load_dotenv
from weaviate.auth import Auth
import weaviate.classes.config as wc
from rich.console import Console
from rich.panel import Panel
from weaviate.classes.generate import GenerativeConfig
from weaviate.classes.query import MetadataQuery
from weaviate.classes.query import Filter

class WeaviateProcessor:
    def __init__(self):
        load_dotenv()
        self.client = weaviate.connect_to_weaviate_cloud(
            cluster_url=os.getenv("WEAVIATE_URL"),
            auth_credentials=Auth.api_key(os.getenv("WEAVIATE_API_KEY")),
            headers={
                "X-OpenAI-Api-Key": os.getenv("OPENAI_APIKEY")
               # "X-Azure-Api-Key": os.getenv("AZURE_API_KEY"),
               # "X-Azure-Client-Resource": os.getenv("AZURE_RESOURCE_NAME")
            }
        )
        print(self.client.is_ready())
        
    def create_schema(self, schema_name: str):
        """
        Create a Weaviate schema with specified name
        
        Args:
            schema_name: Name for the collection/schema
        """
        collection = self.client.collections.create(
            name=schema_name,
            description=f"Document collection for {schema_name}",
            vectorizer_config=wc.Configure.Vectorizer.text2vec_openai(vectorize_collection_name=False),
            generative_config=wc.Configure.Generative.openai(),
       
            properties=[
                wc.Property(name="text", data_type=wc.DataType.TEXT),
                wc.Property(name="title", data_type=wc.DataType.TEXT),
                wc.Property(name="filename", data_type=wc.DataType.TEXT),
                wc.Property(name="user", data_type=wc.DataType.TEXT),
                wc.Property(name="category", data_type=wc.DataType.TEXT),
                wc.Property(name="categoryId", data_type=wc.DataType.TEXT),
                wc.Property(name="description", data_type=wc.DataType.TEXT),
                wc.Property(name="document_created", data_type=wc.DataType.DATE),
                wc.Property(name="datecreated", data_type=wc.DataType.DATE),
                wc.Property(name="dateupdated", data_type=wc.DataType.DATE),
                wc.Property(name="extension", data_type=wc.DataType.TEXT),
                wc.Property(name="source", data_type=wc.DataType.TEXT),
                wc.Property(name="file_path", data_type=wc.DataType.TEXT),
                wc.Property(name="chunk_index", data_type=wc.DataType.INT),
                wc.Property(name="chunk_type", data_type=wc.DataType.TEXT)
            ]
        )
        return collection
        
    def query(self, query: str, prompt: str, collection: object, where_filter: dict = None):
        """Execute a generative query against a Weaviate collection."""
        filter_query = None

        
        response = collection.generate.near_text(
            query=query,
            filters=where_filter,
            grouped_task=prompt,
            generative_provider=GenerativeConfig.openai(
                    model ="gpt-4.1",
                    temperature=0.1,
                ),
            limit=3
        )           
               
        
        

        print(response)
            # Prettify the output using Rich
        console = Console()
        console.print(
                Panel(f"{prompt}".replace("{text}", query), 
                      title="Prompt", 
                      border_style="bold red")
            )
        console.print(
                Panel(response.generated, 
                      title="Generated Content", 
                      border_style="bold green")
            )
            
        return response
        
if __name__ == "__main__":
    processor = WeaviateProcessor()
    processor.create_schema()
    print("Schema created successfully!")