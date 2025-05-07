from setuptools import setup, find_packages

setup(
    name="topicality",
    version="1.0.0",
    packages=find_packages(),
    install_requires=[
        "fastapi",
        "python-multipart",
        "pydantic",
        "weaviate-client>=4.0.0",  # Updated to latest major version
        "python-dotenv",
        "azure-storage-blob",
        "docling",
        "docling-core",
        "openai",
        "uvicorn",
        "rich",  # Added for console output
    ],
    extras_require={
        'test': [
            'pytest',
            'pytest-asyncio',
            'pytest-mock',
            'httpx',
            'pytest-cov',
        ],
    },
)