import mysql.connector
import os
from dotenv import load_dotenv
import requests
import re

TABLE_NAME = "taipei_senior_meal"
URL = "https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer/findAddressCandidates"
ADDRESS_PATTERN = r'(?:臺|台)北市.*?\d+號'

load_dotenv()

DB_CONFIG = {
    "host": os.getenv("DB_HOST"),
    "port": int(os.getenv("DB_PORT")),
    "user": os.getenv("DB_USER"),
    "password": os.getenv("DB_PASSWORD"),
    "database": os.getenv("DB_NAME")
}

# Connect to DB
try:
    conn = mysql.connector.connect(**DB_CONFIG)
    cursor = conn.cursor()

    # Add lon & lat columns if not exist
    cursor.execute(f"SHOW COLUMNS FROM {TABLE_NAME}")
    existing_columns = [col[0] for col in cursor.fetchall()]
    if "lat" not in existing_columns:
        cursor.execute(f"ALTER TABLE {TABLE_NAME} ADD COLUMN lat DECIMAL(10,6)")
    if "lon" not in existing_columns:
        cursor.execute(f"ALTER TABLE {TABLE_NAME} ADD COLUMN lon DECIMAL(10,6)")
    conn.commit()

    # Fetch addresses
    cursor.execute(f"""
        SELECT _id, address
        FROM {TABLE_NAME}
        WHERE lon IS NULL OR lat IS NULL
    """)
    rows = cursor.fetchall()

    for row in rows:
        record_id, address = row

        matches = re.findall(ADDRESS_PATTERN, address)

        for idx, match in enumerate(matches):
        
            params = {
                "SingleLine": match,
                "f": "json",
                "outSR": '{"wkid":4326}',
                "outFields": "Addr_type, Match_addr, StAddr, City",
                "maxLocations": 1
            }

            response = requests.get(URL, params=params)
            if response.status_code == 200:
                data = response.json()
                if data.get("candidates"):
                    location = data["candidates"][0]["location"]
                    lon = round(location["x"], 6)
                    lat = round(location["y"], 6)
                    print(lon, lat)
                    if idx == 0:
                        cursor.execute(
                            f"UPDATE {TABLE_NAME} SET lon=%s, lat=%s WHERE _id=%s",
                            (lon, lat, record_id)
                        )
                        print(f"Updated for id {record_id} with address: {match}, lon: {lon}, lat: {lat}")
                    else:
                        # Add new record for additional address match
                        cursor.execute(
                            f"SELECT org_group_name, org_name, division, admin_code, city, phone FROM {TABLE_NAME} WHERE _id=%s",
                            (record_id,)
                        )
                        original = cursor.fetchone()
                        if original:
                            org_group_name, org_name, division, admin_code, city, phone = original
                            cursor.execute(
                                f"""INSERT INTO {TABLE_NAME} 
                                (_importdate, org_group_name, org_name, division, admin_code, city, address, phone, lon, lat)
                                VALUES (NOW(), %s, %s, %s, %s, %s, %s, %s, %s, %s)""",
                                (org_group_name, org_name, division, admin_code, city, match, phone, lon, lat)
                            )
                            print(f"Inserted new record for id {record_id} with address: {match}, lon: {lon}, lat: {lat}")
                        
                else:
                    print(f"No candidates found for id {record_id}, address: {match}")
            else:
                print(f"Error fetching geocode for id {record_id}, status: {response.status_code}")

    conn.commit()

except mysql.connector.Error as err:
    print("Connection error: ", err)
    if conn.is_connected():
        conn.commit()
        cursor.close()
        conn.close()
finally:
    if conn.is_connected():
        cursor.close()
        conn.close()

