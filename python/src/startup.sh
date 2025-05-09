#!/bin/bash
set -e

gunicorn src.api:app \
  --workers 4 \
  --worker-class uvicorn.workers.UvicornWorker \
  --bind 0.0.0.0:${PORT}
