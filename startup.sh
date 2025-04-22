#!/bin/bash

# Install dependencies
#pip install -r /home/site/wwwroot/requirements.txt

# Start the application using Gunicorn with Uvicorn workers
gunicorn src.api:app --workers 4 --worker-class uvicorn.workers.UvicornWorker --bind 0.0.0.0:8080