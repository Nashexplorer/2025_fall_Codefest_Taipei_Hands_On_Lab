import requests
import mysql.connector
import os
from dotenv import load_dotenv
from datetime import datetime
import time

API_URL = "https://data.taipei/api/v1/dataset/bdc841eb-e8c8-41ee-abfc-1e198a96e905?scope=resourceAquire"
TABLE_NAME = "taipei_meal_aid"

# load environment variables from .env file
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
            time.sleep(0.5)
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
                address_code VARCHAR(50),
                l1_code VARCHAR(50),
                l1_name VARCHAR(100),
                org_type VARCHAR(50),
                org_group_name VARCHAR(100),
                org_name VARCHAR(255),
                division VARCHAR(50),
                person_in_charge VARCHAR(255),
                contact_name VARCHAR(100),
                zone_code VARCHAR(50),
                city VARCHAR(50),
                address VARCHAR(255),
                phone VARCHAR(50),
                fax VARCHAR(50),
                web_homepage VARCHAR(255),
                org_introduction TEXT,
                registered VARCHAR(100),
                create_date_title VARCHAR(100),
                create_date VARCHAR(100),
                prop_title VARCHAR(100),
                prop VARCHAR(100),
                bedno_title VARCHAR(100),
                bedno VARCHAR(50),
                lat DECIMAL(10,6),
                lon DECIMAL(10,6),
                dept_name VARCHAR(100),
                post_date VARCHAR(100)
            )
        """)

        # Insert data into table
        insert_sql = f"""
            INSERT INTO {TABLE_NAME} (
                _id, _importdate, address_code, l1_code, l1_name, org_type, org_group_name, org_name, division,
                person_in_charge, contact_name, zone_code, city, address, phone, fax, web_homepage,
                org_introduction, registered, create_date_title, create_date, prop_title, prop, bedno_title,
                bedno, lat, lon, dept_name, post_date
            ) VALUES (
                %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s
            )
            ON DUPLICATE KEY UPDATE
                _importdate=VALUES(_importdate),
                address_code=VALUES(address_code),
                l1_code=VALUES(l1_code),
                l1_name=VALUES(l1_name),
                org_type=VALUES(org_type),
                org_group_name=VALUES(org_group_name),
                org_name=VALUES(org_name),
                division=VALUES(division),
                person_in_charge=VALUES(person_in_charge),
                contact_name=VALUES(contact_name),
                zone_code=VALUES(zone_code),
                city=VALUES(city),
                address=VALUES(address),
                phone=VALUES(phone),
                fax=VALUES(fax),
                web_homepage=VALUES(web_homepage),
                org_introduction=VALUES(org_introduction),
                registered=VALUES(registered),
                create_date_title=VALUES(create_date_title),
                create_date=VALUES(create_date),
                prop_title=VALUES(prop_title),
                prop=VALUES(prop),
                bedno_title=VALUES(bedno_title),
                bedno=VALUES(bedno),
                lat=VALUES(lat),
                lon=VALUES(lon),
                dept_name=VALUES(dept_name),
                post_date=VALUES(post_date)
        """

        # Prepare batch data
        batch_record = []
        seen_keys = set()

        for record in records:
            # Check duplicates based on org_name and address
            key = (record.get("org_name"), record.get("address"))
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

            # Parse lat/lon
            lat = record.get("lat")
            lon = record.get("lon")
            try:
                lat = float(lat) if lat else None
                lon = float(lon) if lon else None
            except ValueError:
                print(f"Invalid lat/lon for record {record.get('_id')}: {lat}, {lon}")
                lat, lon = None, None

            batch_record.append((
                record.get("_id"),
                _importdate,
                record.get("address_code"),
                record.get("l1_code"),
                record.get("l1_name"),
                record.get("org_type"),
                record.get("org_group_name"),
                record.get("org_name"),
                record.get("division"),
                record.get("person_in_charge"),
                record.get("contact_name"),
                record.get("zone_code"),
                record.get("city"),
                record.get("address"),
                record.get("phone"),
                record.get("fax"),
                record.get("web_homepage"),
                record.get("org_introduction"),
                record.get("registered"),
                record.get("create_date_title"),
                record.get("create_date"),
                record.get("prop_title"),
                record.get("prop"),
                record.get("bedno_title"),
                record.get("bedno"),
                lat,
                lon,
                record.get("dept_name"),
                record.get("post_date")
            ))

        # Execute batch insert
        for i in range(0, len(batch_record), 200):
            cursor.executemany(insert_sql, batch_record[i:i+200])
            conn.commit()
        conn.commit()
        print(f"{len(seen_keys)} records inserted successfully.")
        print(seen_keys)

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
