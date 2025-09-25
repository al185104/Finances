# This serves as the backend route for all transaction related operations
from flask import jsonify, request
from db import get_db
from utils import parse_date_or_none, period_bounds
from . import api

@api.post("/transactions")
def create_transaction():
    """
    Create Transaction
    ---
    tags:
      - Transactions
    parameters:
      - in: body
        name: body
        required: true
        schema:
          type: object
          required: [amount, txn_type, txn_date]
          properties:
            amount:        { type: number, example: 3000 }
            txn_type:      { type: string, enum: [income, expense] }
            txn_date:      { type: string, example: "2025-09-01" }
            description:   { type: string }
            category_id:   { type: integer }
            is_recurring:  { type: boolean, example: true }
            recurrence_period:   { type: string, enum: [daily, weekly, monthly, yearly] }
            recurrence_interval: { type: integer, example: 1 }
    responses:
      201: { description: Created }
      400: { description: Validation error }
      404: { description: Category not found }
    """
    data = request.get_json(force=True) or {}
    for f in ["amount", "txn_type", "txn_date"]:
        if f not in data:
            return jsonify({"error": f"Missing field: {f}"}), 400
    try:
        amount = float(data["amount"])
        if amount <= 0: raise ValueError
    except Exception:
        return jsonify({"error": "amount must be positive"}), 400

    txn_type = str(data["txn_type"]).lower()
    if txn_type not in ("income", "expense"):
        return jsonify({"error": "txn_type must be 'income' or 'expense'"}), 400

    d = parse_date_or_none(str(data["txn_date"]))
    if not d:
        return jsonify({"error": "txn_date must be YYYY-MM-DD"}), 400

    description = data.get("description")
    category_id = data.get("category_id")
    is_recurring = 1 if data.get("is_recurring") else 0
    recurrence_period = data.get("recurrence_period")
    recurrence_interval = data.get("recurrence_interval", 1)
    try:
        recurrence_interval = int(recurrence_interval)
        if recurrence_interval < 1: raise ValueError
    except Exception:
        return jsonify({"error": "recurrence_interval must be >= 1"}), 400

    conn = get_db()
    cur = conn.cursor()
    if category_id is not None:
        row = cur.execute("SELECT id FROM categories WHERE id=?", (category_id,)).fetchone()
        if not row:
            conn.close()
            return jsonify({"error": "category_id not found"}), 404

    cur.execute("""
        INSERT INTO transactions(amount, txn_type, txn_date, description, category_id,
                                 is_recurring, recurrence_period, recurrence_interval)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?)
    """, (amount, txn_type, d.isoformat(), description, category_id,
          is_recurring, recurrence_period, recurrence_interval))
    conn.commit()
    new_id = cur.lastrowid
    row = cur.execute("SELECT * FROM transactions WHERE id=?", (new_id,)).fetchone()
    conn.close()
    return jsonify(dict(row)), 201


@api.get("/transactions")
def list_transactions():
    """
    List Transactions (supports period/ref_date or explicit dates)
    ---
    tags:
      - Transactions
    parameters:
      - in: query
        name: period
        type: string
        enum: [daily, weekly, monthly, yearly]
        default: monthly
        description: Used when start_date/end_date are not supplied.
      - in: query
        name: ref_date
        type: string
        description: Reference date YYYY-MM-DD (defaults to today). Used with period.
      - in: query
        name: start_date
        type: string
        description: YYYY-MM-DD. If provided, overrides period/ref_date.
      - in: query
        name: end_date
        type: string
        description: YYYY-MM-DD. If provided, overrides period/ref_date.
      - in: query
        name: category_id
        type: integer
    responses:
      200: { description: OK }
      400: { description: Bad date format }
    """
    period = (request.args.get("period") or "monthly").lower()
    ref_str = request.args.get("ref_date")
    start_q = request.args.get("start_date")
    end_q = request.args.get("end_date")
    category_id = request.args.get("category_id")

    # Determine date bounds
    start_bound = None
    end_bound = None

    # If explicit dates are provided, use them (and validate)
    if start_q:
        d = parse_date_or_none(start_q)
        if not d:
            return jsonify({"error": "start_date must be YYYY-MM-DD"}), 400
        start_bound = d.isoformat()
    if end_q:
        d = parse_date_or_none(end_q)
        if not d:
            return jsonify({"error": "end_date must be YYYY-MM-DD"}), 400
        end_bound = d.isoformat()

    # If neither start nor end given, compute from period/ref_date
    if not start_bound and not end_bound:
        from datetime import date as _date
        ref = _date.today() if not ref_str else parse_date_or_none(ref_str)
        if ref is None:
            return jsonify({"error": "ref_date must be YYYY-MM-DD"}), 400
        start, end = period_bounds(ref, period)
        start_bound, end_bound = start.isoformat(), end.isoformat()

    # Build SQL
    sql = "SELECT * FROM transactions"
    filters = []
    params = []

    if start_bound:
        filters.append("txn_date >= ?")
        params.append(start_bound)
    if end_bound:
        filters.append("txn_date <= ?")
        params.append(end_bound)
    if category_id:
        filters.append("category_id = ?")
        params.append(category_id)

    if filters:
        sql += " WHERE " + " AND ".join(filters)
    sql += " ORDER BY txn_date DESC, id DESC"

    conn = get_db()
    rows = conn.execute(sql, params).fetchall()
    conn.close()
    return jsonify([dict(r) for r in rows])


@api.get("/transactions/<int:txn_id>")
def get_transaction(txn_id: int):
    """
    Get Transaction by ID
    ---
    tags:
      - Transactions
    parameters:
      - in: path
        name: txn_id
        required: true
        type: integer
    responses:
      200: { description: OK }
      404: { description: Not found }
    """
    conn = get_db()
    row = conn.execute("SELECT * FROM transactions WHERE id=?", (txn_id,)).fetchone()
    conn.close()
    if not row:
        return jsonify({"error": "Transaction not found"}), 404
    return jsonify(dict(row))


@api.put("/transactions/<int:txn_id>")
def update_transaction(txn_id: int):
    """
    Update Transaction (partial)
    ---
    tags:
      - Transactions
    parameters:
      - in: path
        name: txn_id
        required: true
        type: integer
      - in: body
        name: body
        schema:
          type: object
          properties:
            amount: { type: number }
            txn_type: { type: string, enum: [income, expense] }
            txn_date: { type: string, description: "YYYY-MM-DD" }
            description: { type: string }
            category_id: { type: integer }
            is_recurring: { type: boolean }
            recurrence_period: { type: string, enum: [daily, weekly, monthly, yearly] }
            recurrence_interval: { type: integer }
    responses:
      200: { description: Updated }
      400: { description: Validation error }
      404: { description: Not found }
    """
    data = request.get_json(force=True) or {}
    allowed = {"amount","txn_type","txn_date","description","category_id",
               "is_recurring","recurrence_period","recurrence_interval"}
    updates = {k: v for k, v in data.items() if k in allowed}
    if not updates:
        return jsonify({"error": "No valid fields to update"}), 400

    if "amount" in updates:
        try:
            if float(updates["amount"]) <= 0: raise ValueError
        except Exception:
            return jsonify({"error":"amount must be positive"}), 400
    if "txn_type" in updates and str(updates["txn_type"]).lower() not in ("income","expense"):
        return jsonify({"error":"txn_type must be 'income' or 'expense'"}), 400
    if "txn_date" in updates and not parse_date_or_none(str(updates["txn_date"])):
        return jsonify({"error":"txn_date must be YYYY-MM-DD"}), 400
    if "recurrence_interval" in updates:
        try:
            if int(updates["recurrence_interval"]) < 1: raise ValueError
        except Exception:
            return jsonify({"error":"recurrence_interval must be >= 1"}), 400

    conn = get_db()
    cur = conn.cursor()

    if "category_id" in updates and updates["category_id"] is not None:
        row = cur.execute("SELECT id FROM categories WHERE id=?", (updates["category_id"],)).fetchone()
        if not row:
            conn.close()
            return jsonify({"error":"category_id not found"}), 404

    fields = []
    params = []
    for k, v in updates.items():
        fields.append(f"{k}=?")
        if k == "txn_type": v = str(v).lower()
        if k == "txn_date": v = str(v)
        if k == "is_recurring": v = 1 if v else 0
        params.append(v)
    params.append(txn_id)

    cur.execute(f"UPDATE transactions SET {', '.join(fields)} WHERE id=?", params)
    if cur.rowcount == 0:
        conn.close()
        return jsonify({"error": "Transaction not found"}), 404
    conn.commit()
    row = cur.execute("SELECT * FROM transactions WHERE id=?", (txn_id,)).fetchone()
    conn.close()
    return jsonify(dict(row))


@api.delete("/transactions/<int:txn_id>")
def delete_transaction(txn_id: int):
    """
    Delete Transaction
    ---
    tags:
      - Transactions
    parameters:
      - in: path
        name: txn_id
        required: true
        type: integer
    responses:
      204: { description: Deleted }
      404: { description: Not found }
    """
    conn = get_db()
    cur = conn.cursor()
    cur.execute("DELETE FROM transactions WHERE id=?", (txn_id,))
    changes = cur.rowcount
    conn.commit()
    conn.close()
    if changes == 0:
        return jsonify({"error":"Transaction not found"}), 404
    return "", 204
