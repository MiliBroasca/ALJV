#!/usr/bin/env python3
from __future__ import annotations

"""
Evaluate Unity GA JSON logs.

Usage:
    python evaluate_ga_logs.py --logs-dir "Assets/GA_Logs"
    python evaluate_ga_logs.py --logs-dir "Assets/GA_Logs" --export-csv
    python evaluate_ga_logs.py --logs-dir "Assets/GA_Logs" --plot
"""

import argparse
import json
from pathlib import Path
from typing import Any, Dict

import pandas as pd


def safe_get(d: Dict[str, Any], *keys: str, default=None):
    for key in keys:
        if key in d:
            return d[key]
    return default


def load_log_file(path: Path) -> Dict[str, Any]:
    with path.open("r", encoding="utf-8") as f:
        data = json.load(f)

    row: Dict[str, Any] = {
        "file": path.name,
        "algorithm": safe_get(data, "algorithm", default="Unknown"),
        "timestamp": safe_get(data, "timestamp"),
        "mapVariant": safe_get(data, "mapVariant", default="Unknown"),
        "startRoom": safe_get(data, "startRoom", default="Unknown"),
        "populationSize": safe_get(data, "populationSize"),
        "genomeLength": safe_get(data, "genomeLength"),
        "generations": safe_get(data, "generations"),
        "mutationRate": safe_get(data, "mutationRate"),
        "eliteCount": safe_get(data, "eliteCount"),
        "bestFitness": safe_get(data, "bestFitness"),
        "finalScore": safe_get(data, "finalScore"),
        "remainingHealth": safe_get(data, "remainingHealth"),
        "stepsUsed": safe_get(data, "stepsUsed"),
        "died": safe_get(data, "died"),
        "reachedGoal": safe_get(data, "reachedGoal"),
        "trainMap": safe_get(data, "trainMap", "sourceMap", default=None),
        "evalMap": safe_get(data, "evalMap", "targetMap", default=None),
        "runType": safe_get(data, "runType", default="train"),
        "bestGenomeLength": len(safe_get(data, "bestGenome", default=[])),
        "generationStats": safe_get(data, "generationStats", default=[]),
    }

    if row["trainMap"] is None:
        row["trainMap"] = f'{row["mapVariant"]}:{row["startRoom"]}'
    if row["evalMap"] is None:
        row["evalMap"] = row["trainMap"]

    return row


def load_logs(logs_dir: Path) -> pd.DataFrame:
    files = sorted(logs_dir.glob("*.json"))
    if not files:
        raise FileNotFoundError(f"No JSON files found in: {logs_dir}")

    rows = [load_log_file(path) for path in files]
    df = pd.DataFrame(rows)

    for col in ("died", "reachedGoal"):
        if col in df.columns:
            df[col] = df[col].fillna(False).astype(bool)

    numeric_cols = [
        "populationSize", "genomeLength", "generations", "mutationRate",
        "eliteCount", "bestFitness", "finalScore", "remainingHealth",
        "stepsUsed", "bestGenomeLength"
    ]
    for col in numeric_cols:
        if col in df.columns:
            df[col] = pd.to_numeric(df[col], errors="coerce")

    return df


def print_overall_summary(df: pd.DataFrame) -> None:
    print("\n=== OVERALL SUMMARY ===")
    summary = {
        "runs": len(df),
        "avg_bestFitness": df["bestFitness"].mean(),
        "max_bestFitness": df["bestFitness"].max(),
        "avg_finalScore": df["finalScore"].mean(),
        "avg_remainingHealth": df["remainingHealth"].mean(),
        "avg_stepsUsed": df["stepsUsed"].mean(),
        "success_rate": df["reachedGoal"].mean() if len(df) else 0.0,
    }
    for k, v in summary.items():
        if isinstance(v, float):
            print(f"{k}: {v:.3f}")
        else:
            print(f"{k}: {v}")


def print_grouped_summary(df: pd.DataFrame) -> None:
    print("\n=== SUMMARY BY MAP / START ROOM / RUN TYPE ===")
    grouped = (
        df.groupby(["algorithm", "mapVariant", "startRoom", "runType", "trainMap", "evalMap"], dropna=False)
          .agg(
              runs=("file", "count"),
              avg_bestFitness=("bestFitness", "mean"),
              max_bestFitness=("bestFitness", "max"),
              avg_finalScore=("finalScore", "mean"),
              avg_remainingHealth=("remainingHealth", "mean"),
              avg_stepsUsed=("stepsUsed", "mean"),
              success_rate=("reachedGoal", "mean"),
          )
          .reset_index()
          .sort_values(["algorithm", "mapVariant", "startRoom", "runType", "trainMap", "evalMap"])
    )
    with pd.option_context("display.max_rows", None, "display.max_columns", None, "display.width", 160):
        print(grouped.to_string(index=False, float_format=lambda x: f"{x:.3f}"))


def print_transfer_matrix(df: pd.DataFrame) -> None:
    if "trainMap" not in df.columns or "evalMap" not in df.columns:
        return

    print("\n=== TRANSFER MATRIX (avg bestFitness) ===")
    pivot = pd.pivot_table(
        df,
        values="bestFitness",
        index="trainMap",
        columns="evalMap",
        aggfunc="mean"
    )
    with pd.option_context("display.max_rows", None, "display.max_columns", None, "display.width", 160):
        print(pivot.to_string(float_format=lambda x: f"{x:.3f}"))

    print("\n=== TRANSFER MATRIX (success rate) ===")
    pivot_success = pd.pivot_table(
        df,
        values="reachedGoal",
        index="trainMap",
        columns="evalMap",
        aggfunc="mean"
    )
    with pd.option_context("display.max_rows", None, "display.max_columns", None, "display.width", 160):
        print(pivot_success.to_string(float_format=lambda x: f"{x:.3f}"))


def export_csvs(df: pd.DataFrame, out_dir: Path) -> None:
    out_dir.mkdir(parents=True, exist_ok=True)

    raw_path = out_dir / "ga_logs_raw.csv"
    df.drop(columns=["generationStats"], errors="ignore").to_csv(raw_path, index=False)

    grouped = (
        df.groupby(["algorithm", "mapVariant", "startRoom", "runType", "trainMap", "evalMap"], dropna=False)
          .agg(
              runs=("file", "count"),
              avg_bestFitness=("bestFitness", "mean"),
              max_bestFitness=("bestFitness", "max"),
              avg_finalScore=("finalScore", "mean"),
              avg_remainingHealth=("remainingHealth", "mean"),
              avg_stepsUsed=("stepsUsed", "mean"),
              success_rate=("reachedGoal", "mean"),
          )
          .reset_index()
    )
    grouped.to_csv(out_dir / "ga_logs_summary.csv", index=False)
    print(f"\nExported CSV files to: {out_dir}")


def plot_generation_curves(df: pd.DataFrame, out_dir: Path) -> None:
    try:
        import matplotlib.pyplot as plt
    except ImportError:
        print("\nmatplotlib is not installed. Skipping plots.")
        return

    out_dir.mkdir(parents=True, exist_ok=True)

    for _, row in df.iterrows():
        stats = row.get("generationStats", [])
        if not stats:
            continue

        gen_df = pd.DataFrame(stats)
        if "generation" not in gen_df or "bestFitness" not in gen_df:
            continue

        plt.figure(figsize=(8, 4.5))
        plt.plot(gen_df["generation"], gen_df["bestFitness"])
        plt.xlabel("Generation")
        plt.ylabel("Best fitness")
        plt.title(f'{row["file"]} | {row["mapVariant"]} | {row["startRoom"]}')
        plt.tight_layout()
        plt.savefig(out_dir / f'{Path(row["file"]).stem}_fitness.png', dpi=150)
        plt.close()

    print(f"\nSaved plots to: {out_dir}")


def main() -> None:
    parser = argparse.ArgumentParser(description="Evaluate Unity GA JSON logs.")
    parser.add_argument("--logs-dir", type=str, required=True, help="Directory containing GA JSON logs.")
    parser.add_argument("--export-csv", action="store_true", help="Export raw and summary CSV files.")
    parser.add_argument("--plot", action="store_true", help="Save best-fitness-per-generation plots.")
    parser.add_argument("--out-dir", type=str, default=None, help="Output directory for CSVs/plots (default: <logs-dir>/analysis)")
    args = parser.parse_args()

    logs_dir = Path(args.logs_dir)
    if not logs_dir.exists():
        raise FileNotFoundError(f"Logs directory does not exist: {logs_dir}")

    df = load_logs(logs_dir)

    print_overall_summary(df)
    print_grouped_summary(df)
    print_transfer_matrix(df)

    out_dir = Path(args.out_dir) if args.out_dir else (logs_dir / "analysis")

    if args.export_csv:
        export_csvs(df, out_dir)

    if args.plot:
        plot_generation_curves(df, out_dir)


if __name__ == "__main__":
    main()
