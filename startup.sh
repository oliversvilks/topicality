#!/bin/bash

set -e  # Exit on error

pip install -r /home/site/wwwroot/requirements.txt

gunicorn src.api:app --workers 4 --worker-class uvicorn.workers.UvicornWorker --bind 0.0.0.0:8080