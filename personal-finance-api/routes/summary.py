# this servres as the backend route for summary related operations. This will be later used for cards in .NET Maui front end
from flask import jsonify, request
from datetime import date
from db import get_db
from utils import parse_date_or_none, period_bounds
from . import api

@api.get("/summary")
def summary():
    """
    Summary (Totals for a period)
    ---
    tags: [Summary]
    parameters:
      - in: query
        name: period
        schema: { type: string, enum: [daily, weekly, monthly, yearly], default: monthly }
      - in: query
        name: ref_date
        schema: { type: string, example: "2025-09-15" }
    responses:
      200:
        description: OK
    """
    period = (request.args.get("period") or "monthly").lower()
    ref_str = request.args.get("ref_date")
    ref = date.today() if not ref_str else parse_date_or_none(ref_str)
    if ref is None:
        return jsonify({"error":"ref_date must be YYYY-MM-DD"}), 400

    start, end = period_bounds(ref, period)
    conn = get_db()
    rows = conn.execute(
        "SELECT amount, txn_type FROM transactions WHERE txn_date BETWEEN ? AND ?",
        (start.isoformat(), end.isoformat())
    ).fetchall()
    conn.close()

    income = sum(r["amount"] for r in rows if r["txn_type"] == "income")
    spent  = sum(r["amount"] for r in rows if r["txn_type"] == "expense")

    return jsonify({
        "period": period,
        "start_date": start.isoformat(),
        "end_date": end.isoformat(),
        "total_income": round(income, 2),
        "total_spent": round(spent, 2),
        "net": round(income - spent, 2)
    })
