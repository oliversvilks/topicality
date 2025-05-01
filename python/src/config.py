from pydantic_settings import BaseSettings

class Settings(BaseSettings):
    debug: bool = True
    reload: bool = True
    workers: int = 1
    host: str = "127.0.0.1"
    port: int = 8000
    
    class Config:
        env_file = ".env"

settings = Settings()