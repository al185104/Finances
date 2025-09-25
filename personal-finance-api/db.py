# using SQLite for simplicity. 
import sqlite3

DB_PATH = "finance.db"

def get_db():
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    return conn

# just an initialization of the db -- create tables if they don't exist
def init_db():
    conn = get_db()
    cur = conn.cursor()
    cur.execute("""
        CREATE TABLE IF NOT EXISTS categories (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL UNIQUE
        )
    """)
    cur.execute("""
        CREATE TABLE IF NOT EXISTS transactions (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            amount REAL NOT NULL,
            txn_type TEXT NOT NULL CHECK (txn_type IN ('income','expense')),
            txn_date TEXT NOT NULL,   -- YYYY-MM-DD
            description TEXT,
            category_id INTEGER,
            is_recurring INTEGER DEFAULT 0,
            recurrence_period TEXT,
            recurrence_interval INTEGER DEFAULT 1,
            FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE SET NULL
        )
    """)
    conn.commit()
    conn.close()
