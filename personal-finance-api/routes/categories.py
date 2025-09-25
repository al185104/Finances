# this servres as the backend route for all category related operations 
from flask import jsonify, request
from datetime import date
from db import get_db
from utils import parse_date_or_none, period_bounds
from . import api

@api.post("/categories")
def create_category():
    """
    Create Category
    ---
    tags:
      - Categories
    parameters:
      - in: body
        name: body
        required: true
        schema:
          type: object
          required: [name]
          properties:
            name:
              type: string
              example: Grocery
    responses:
      201:
        description: Created
      400:
        description: Duplicate or invalid
    """
    data = request.get_json(force=True) or {}
    name = data.get("name")
    if not name:
        return jsonify({"error": "name is required"}), 400
    try:
        conn = get_db()
        cur = conn.cursor()
        cur.execute("INSERT INTO categories(name) VALUES (?)", (name,))
        conn.commit()
        new_id = cur.lastrowid
        row = cur.execute("SELECT id, name FROM categories WHERE id=?", (new_id,)).fetchone()
        return jsonify(dict(row)), 201
    except Exception as e:
        if "UNIQUE" in str(e):
            return jsonify({"error": "Category name already exists"}), 400
        raise
    finally:
        conn.close()


@api.get("/categories")
def list_categories():
    """
    List Categories (with amount spent for a period)
    ---
    tags:
      - Categories
    parameters:
      - in: query
        name: period
        type: string
        enum: [daily, weekly, monthly, yearly]
        default: monthly
        description: Period to summarize expenses.
      - in: query
        name: ref_date
        type: string
        description: Reference date YYYY-MM-DD (defaults to today).
    responses:
      200:
        description: OK
    """
    # Parse period + reference date
    period = (request.args.get("period") or "monthly").lower()
    ref_str = request.args.get("ref_date")
    ref = date.today() if not ref_str else parse_date_or_none(ref_str)
    if ref is None:
        return jsonify({"error": "ref_date must be YYYY-MM-DD"}), 400

    start, end = period_bounds(ref, period)

    # Sum ONLY expenses per category within [start, end]
    # Using LEFT JOIN so categories with no spending appear with 0.
    sql = """
        SELECT
            c.id,
            c.name,
            COALESCE(
              SUM(
                CASE
                  WHEN t.txn_type = 'expense'
                   AND t.txn_date BETWEEN ? AND ?
                  THEN t.amount
                  ELSE 0
                END
              ), 0
            ) AS amount_spent
        FROM categories AS c
        LEFT JOIN transactions AS t
          ON t.category_id = c.id
        GROUP BY c.id, c.name
        ORDER BY amount_spent DESC, c.name ASC
    """

    conn = get_db()
    rows = conn.execute(sql, (start.isoformat(), end.isoformat())).fetchall()
    conn.close()

    result = [
        {
            "id": r["id"],
            "name": r["name"],
            "amount_spent": round(float(r["amount_spent"]), 2)
        }
        for r in rows
    ]
    return jsonify(result)
