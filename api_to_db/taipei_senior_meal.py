import requests
import mysql.connector
import os
from dotenv import load_dotenv
from datetime import datetime
import time

API_URL = "https://data.taipei/api/v1/dataset/86d839db-85c7-48b0-991b-dc86922eb093?scope=resourceAquire"
TABLE_NAME = "taipei_senior_meal"

load_dotenv()

DB_CONFIG = {
    "host": os.getenv("DB_HOST"),
    "port": int(os.getenv("DB_PORT")),
    "user": os.getenv("DB_USER"),
    "password": os.getenv("DB_PASSWORD"),
    "database": os.getenv("DB_NAME")
}

def fetch_and_insert(offset=0, limit=1000):
    params = {
        "offset": offset,
        "limit": limit
    }

    # API request
    max_retries = 3
    for attempt in range(max_retries):
        try:
            response = requests.get(API_URL, params=params, timeout=10)
            response.raise_for_status()
            data = response.json()
            break
        except requests.RequestException as e:
            print(f"API request failed (attempt {attempt+1}): {e}")
            if attempt < max_retries - 1:
                time.sleep(3)  # wait before retry
            else:
                return 0

    records = data.get("result", {}).get("results", [])
    print(f"Fetched {len(records)} records from API.")

    if not records:
        return 0

    # Connect to DB
    try:
        conn = mysql.connector.connect(**DB_CONFIG)
        cursor = conn.cursor()

        # Create table if not exists
        cursor.execute(f"""
            CREATE TABLE IF NOT EXISTS {TABLE_NAME} (
                _id INT PRIMARY KEY AUTO_INCREMENT,
                _importdate DATETIME,
                org_group_name VARCHAR(255),
                org_name VARCHAR(255),
                division VARCHAR(50),
                admin_code VARCHAR(50),
                city VARCHAR(50),
                address VARCHAR(255),
                phone VARCHAR(50)
            )
        """)

        # Insert data into table
        insert_sql = f"""
            INSERT INTO {TABLE_NAME} (
                _id, _importdate, org_group_name, org_name, division,
                admin_code, city, address, phone
            ) VALUES (
                %s, %s, %s, %s, %s, %s, %s, %s, %s
            )
            ON DUPLICATE KEY UPDATE
                _importdate=VALUES(_importdate),
                org_group_name=VALUES(org_group_name),
                org_name=VALUES(org_name),
                division=VALUES(division),
                admin_code=VALUES(admin_code),
                city=VALUES(city),
                address=VALUES(address),
                phone=VALUES(phone)
        """

        # Prepare batch data
        batch_record = []
        seen_keys = set()

        for record in records:
            # Check duplicates based on org_name and address
            key = (record.get("據點名稱"), record.get("據點地址"))
            if key in seen_keys:
                continue
            seen_keys.add(key)

            # Parse _importdate
            _importdate = record.get("_importdate", {}).get("date")
            if _importdate:
                try:
                    _importdate = datetime.strptime(_importdate.split(".")[0], "%Y-%m-%d %H:%M:%S")
                except:
                    _importdate = None

            # Parse 據點地址
            address = record.get("據點地址", "")
            if address:
                address.replace(" ", "").replace("\n", "")

            batch_record.append((
                record.get("_id"),
                _importdate,
                "老人共餐單位",
                record.get("據點名稱"),
                record.get("行政區"),
                record.get("行政區代碼"),
                "臺北市",
                address,
                record.get("聯絡電話")
            ))

        # Execute batch insert
        for i in range(0, len(batch_record), 200):
            cursor.executemany(insert_sql, batch_record[i:i+200])
            conn.commit()
        conn.commit()
        print(f"{len(seen_keys)} records inserted successfully.")

    except mysql.connector.Error as err:
        print("Connection error: ", err)
    finally:
        if conn.is_connected():
            cursor.close()
            conn.close()

    return len(records)

def main():
    limit = 1000
    offset = 0
    
    while True:
        print(f"Fetching records offset={offset} limit={limit}")
        inserted_count = fetch_and_insert(offset, limit)
        if inserted_count < limit:
            print("All data fetched.")
            break
        offset += limit

if __name__ == "__main__":
    main()


