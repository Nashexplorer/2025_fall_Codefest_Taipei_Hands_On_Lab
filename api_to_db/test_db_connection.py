import mysql.connector
import os
from dotenv import load_dotenv

# load environment variables from .env file
load_dotenv()

DB_CONFIG = {
    "host": os.getenv("DB_HOST"),
    "port": int(os.getenv("DB_PORT")),
    "user": os.getenv("DB_USER"),
    "password": os.getenv("DB_PASSWORD")
}

DB_NAME = os.getenv("DB_NAME")

try:
    conn = mysql.connector.connect(**DB_CONFIG)


    if conn.is_connected():
        print("Connection successful.")
    else:
        print("Connection failed.")
except mysql.connector.Error as err:
    print("Connection error", err)
finally:
    if 'conn' in locals() and conn.is_connected():
        conn.close()
