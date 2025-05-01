from .processors.weaviate_processor import WeaviateProcessor
from docling.document_converter import DocumentConverter
from langchain.text_splitter import RecursiveCharacterTextSplitter
from docling.datamodel.document import ConversionResult
from docling_core.transforms.chunker import HierarchicalChunker
import weaviate.classes.config as wc
from weaviate.classes.config import DataType, Property
from weaviate.classes.generate import GenerativeConfig
from weaviate.classes.query import MetadataQuery
from weaviate.classes.init import Auth

def setup_weaviate():
    processor = WeaviateProcessor()
    #processor.create_schema()
    print("Weaviate is running")

def process_documents(source_urls: list[str], source_titles: list[str]):

    converter = DocumentConverter()
    conv_results_iter = converter.convert_all(source_urls)  # previously `convert`

    # Iterate over the generator to get a list of Docling documents
    docs = [result.document for result in conv_results_iter]
    for doc in docs:
        print(doc.export_to_markdown())

    
    texts, titles = [], []
    chunker = HierarchicalChunker()
    # Process each document in the list
    for doc, title in zip(docs, source_titles):  # Pair each document with its title
        chunks = list(
            chunker.chunk(doc)
        )  # Perform hierarchical chunking and get text from chunks
        for chunk in chunks:
            texts.append(chunk.text)
            titles.append(title)
            # Concatenate title and text
    for i in range(len(texts)):
        texts[i] = f"{titles[i]} {texts[i]}"

    # Initialize the data object
    data = []

#    Create a dictionary for each row by iterating through the corresponding lists
    for text, title in zip(texts, titles):
        data_point = {
        "text": text,
        "title": title,
    }
    data.append(data_point)

    processor = WeaviateProcessor()
    collection_name = "docling"
    if processor.client.collections.exists(collection_name):
        processor.client.collections.delete(collection_name)
    # Create the collection
    collection = processor.client.collections.create(
        name=collection_name,
        vectorizer_config=wc.Configure.Vectorizer.text2vec_openai(vectorize_collection_name=False),
        generative_config=wc.Configure.Generative.openai(),  # Fixed typo here
        #api_key=os.getenv("AZURE_API_KEY")  # Add this line
    
        # Define properties of metadata
        properties=[
            wc.Property(name="text", data_type=wc.DataType.TEXT),
            wc.Property(name="title", data_type=wc.DataType.TEXT, skip_vectorization=True),
        ],
    )
    response = collection.data.insert_many(data)
    if response.has_errors:
        print(response.errors)
    else:
        print("Insert complete.")
    # Create a prompt where context from the Weaviate collection will be injected
    prompt = "WHich is the test server for AIS."
    query = "bert"
    
    response = collection.generate.near_text(
                query=query,
                limit=3,
                grouped_task=prompt,
                return_properties=["text", "title"],
                generative_provider=GenerativeConfig.openai(
        temperature=0.1,
    ),
               
            )
    print(response)
    
    response1 = processor.query(
        query=query,
        prompt=prompt,
        collection=collection
    )
    processor.client.close()
    print("Weaviate client closed.")


if __name__ == "__main__":
    setup_weaviate()
     # Influential machine learning papers
    source_urls = [
        "",

    ]

    # And their corresponding titles (because Docling doesn't have title extraction yet!)
    source_titles = [
        "",
    ]
    #process_documents(source_titles=source_titles, source_urls=source_urls)
