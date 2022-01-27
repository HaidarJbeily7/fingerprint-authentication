import os
from dotenv import load_dotenv
from pathlib import Path


class Constants:
    def __init__(self) -> None:
        env_path = Path('./app') / '.env'
        load_dotenv(dotenv_path = env_path)
        self.STORAGE_PATH = os.environ['STORAGE_PATH']

