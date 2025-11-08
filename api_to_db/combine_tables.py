import mysql.connector
import os
from dotenv import load_dotenv

load_dotenv()

DB_CONFIG = {
    "host": os.getenv("DB_HOST"),
    "port": int(os.getenv("DB_PORT")),
    "user": os.getenv("DB_USER"),
    "password": os.getenv("DB_PASSWORD"),
    "database": os.getenv("DB_NAME")
}

def merge_tables():
    conn = mysql.connector.connect(**DB_CONFIG)
    cursor = conn.cursor()

    # Create new combined table if not exists
    cursor.execute("""
        CREATE TABLE IF NOT EXISTS support_points (
            _id INT PRIMARY KEY AUTO_INCREMENT,
            _importdate DATETIME,
            org_group_name VARCHAR(255),
            org_name VARCHAR(255),
            division VARCHAR(50),
            address VARCHAR(255),
            phone VARCHAR(50),
            lat DECIMAL(10,6),
            lon DECIMAL(10,6),
            person_in_charge VARCHAR(255),
            fax VARCHAR(50)
        )
    """)

   # Insert data from taipei_meal_aid
    cursor.execute("""
        INSERT INTO support_points (_importdate, org_group_name, org_name, division, address, phone, lat, lon, person_in_charge, fax)
        SELECT _importdate, org_group_name, org_name, division, address, phone, lat, lon, person_in_charge, fax
        FROM taipei_meal_aid
        WHERE org_name IS NOT NULL
    """)

    # Insert data from taipei_senior_meal
    cursor.execute("""
        INSERT INTO support_points (_importdate, org_group_name, org_name, division, address, phone, lat, lon)
        SELECT _importdate, org_group_name, org_name, division, address, phone, lat, lon
        FROM taipei_senior_meal
        WHERE org_name IS NOT NULL
    """)

    conn.commit()
    cursor.close()
    conn.close()
    print("Merging completed successfully.")

if __name__ == "__main__":
    merge_tables()
