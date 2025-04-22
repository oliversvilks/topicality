#!/bin/bash

set -e  # Exit on error

pip install -r /home/site/wwwroot/requirements.txt

gunicorn --bind=0.0.0.0 --timeout 600 --chdir src api:app --workers 4 --worker-class uvicorn.workers.UvicornWorker