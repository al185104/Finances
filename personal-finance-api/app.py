from flask import Flask, jsonify
from flasgger import Swagger
from db import init_db
from routes import api

# creating this app.py to be a host for my backend using Flask

def create_app():
    app = Flask(__name__)

    swagger_template = {
        "swagger": "2.0",  # this is just to use swagger 2.0 so that it's eaasier to see the APIs
        "info": {
            "title": "Personal Finance API",
            "version": "1.0.0",
            "description": "Tiny Flask + SQLite API for personal finance (transactions, categories, summary)."
        },
        "tags": [
            {"name": "Categories"},
            {"name": "Transactions"},
            {"name": "Summary"}
        ]
    }

    Swagger(app, template=swagger_template, config={
        "headers": [],
        "specs": [
            {
                "endpoint": "apispec_1",
                "route": "/apispec_1.json",
                "rule_filter": lambda rule: True,
                "model_filter": lambda tag: True,
            }
        ],
        "static_url_path": "/flasgger_static",
        "swagger_ui": True,
        "specs_route": "/apidocs/"
    })

    init_db()
    app.register_blueprint(api, url_prefix="")

    @app.get("/")
    def index():
        return jsonify({"ok": True, "docs": "/apidocs"})

    return app

if __name__ == "__main__":
    app = create_app()
    app.run(host="127.0.0.1", port=8000, debug=True)
