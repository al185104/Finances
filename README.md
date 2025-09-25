# Personal Finance App

A tiny, practical personal finance system consisting of a **Python (Flask + SQLite) backend** and a **.NET MAUI** client UI that runs on **Windows, macOS, Android, and iOS**.

> 🎥 A short walkthrough is included at the repo root as **`Demo Recording.mp4`**.

---

## Overview

- **Backend:** **Flask + SQLite (Python)**
  - Minimal REST API for recording transactions (income/expense), organizing by categories, and returning period-based summaries (daily/weekly/monthly/yearly).
  - Swagger UI available at **`/apidocs`**.
  - Local, file-based persistence via **`finance.db`**.

- **UI:** **.NET MAUI**
  - Cross-platform client (Windows, macOS, Android, iOS).
  - Uses typed services and DTOs to call the Flask API.

---

## Features

- CRUD for **Transactions** with **Categories**.
- Period filters via `period` (`daily|weekly|monthly|yearly`) and optional `ref_date`.
- **Summary** endpoint: total income, total spent, and net for a period.
- Categories endpoint includes **amount_spent** roll-up within the selected period.

---

## Tech Stack

- **Backend:** Python, Flask, Flasgger (Swagger UI), SQLite
- **Frontend:** .NET MAUI (C#), HttpClient-based services
- **Docs/Demos:** Swagger UI at `/apidocs`, **Demo Recording.mp4**

---

## Project Structure (Backend)

```
.
├─ app.py                 # Flask app, Swagger setup, blueprint registration
├─ db.py                  # SQLite helpers + schema init
├─ utils.py               # Date parsing + period bounds (daily/weekly/monthly/yearly)
├─ requirements.txt       # Flask, Flasgger, (optional) Flask-Cors
└─ routes/
   ├─ __init__.py         # Blueprint (api) + submodule imports
   ├─ categories.py       # POST /categories, GET /categories (with amount_spent)
   ├─ transactions.py     # CRUD /transactions (supports period/ref_date or explicit dates)
   └─ summary.py          # GET /summary totals for a period
```

---

## Getting Started (Backend)

```bash
python -m venv .venv
# Windows
.venv\Scripts\Activate.ps1
# macOS/Linux
# source .venv/bin/activate

pip install -r requirements.txt
python app.py
# Swagger UI: http://127.0.0.1:8000/apidocs/
```


## Demo

Open **`Demo Recording.mp4`** in the repo root to see a quick end-to-end walkthrough of the API and MAUI UI.

---

## License

Add your preferred license here (e.g., MIT).
