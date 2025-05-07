import uvicorn
import os

if __name__ == "__main__":
    port = int(os.environ.get("PORT", 8000))  # Fallback to 8000 for local dev
    uvicorn.run("api:app", host="0.0.0.0", port=port)