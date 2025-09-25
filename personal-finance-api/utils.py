# Utility functions to be used across applications, especially from the routes.
from datetime import date, datetime, timedelta
import calendar

def parse_date_or_none(s: str):
    try:
        return datetime.fromisoformat(s).date()
    except Exception:
        return None

def period_bounds(ref: date, period: str):
    p = (period or "monthly").lower()
    if p == "daily":
        return ref, ref
    if p == "weekly":
        start = ref - timedelta(days=ref.weekday())   # Mon..Sun
        return start, start + timedelta(days=6)
    if p == "yearly":
        return date(ref.year, 1, 1), date(ref.year, 12, 31)
    # monthly (default)
    last_day = calendar.monthrange(ref.year, ref.month)[1]
    return date(ref.year, ref.month, 1), date(ref.year, ref.month, last_day)
