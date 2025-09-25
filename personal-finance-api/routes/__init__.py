from flask import Blueprint

api = Blueprint("api", __name__)

from . import categories, transactions, summary

__all__ = ["api"]