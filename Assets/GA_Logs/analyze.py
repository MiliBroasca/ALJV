#!/usr/bin/env python3
from __future__ import annotations

"""
Evaluate Unity GA JSON logs and generate aggregate plots.

Usage:
    python evaluate_ga_logs_v2.py --logs-dir "Assets/GA_Logs"
    python evaluate_ga_logs_v2.py --logs-dir "Assets/GA_Logs" --export-csv
    python evaluate_ga_logs_v2.py --logs-dir "Assets/GA_Logs" --plot
"""

"""
    NOTES:
    - json44 last with not modified step
    - +0.5 penalty for steps
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
    pivot = pd.pivot_table(df, values="bestFitness", index="trainMap", columns="evalMap", aggfunc="mean")
    with pd.option_context("display.max_rows", None, "display.max_columns", None, "display.width", 160):
        print(pivot.to_string(float_format=lambda x: f"{x:.3f}"))

    print("\n=== TRANSFER MATRIX (success rate) ===")
    pivot_success = pd.pivot_table(df, values="reachedGoal", index="trainMap", columns="evalMap", aggfunc="mean")
    with pd.option_context("display.max_rows", None, "display.max_columns", None, "display.width", 160):
        print(pivot_success.to_string(float_format=lambda x: f"{x:.3f}"))


def export_csvs(df: pd.DataFrame, out_dir: Path) -> None:
    out_dir.mkdir(parents=True, exist_ok=True)

    df.drop(columns=["generationStats"], errors="ignore").to_csv(out_dir / "ga_logs_raw.csv", index=False)

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
    import matplotlib.pyplot as plt

    out_dir.mkdir(parents=True, exist_ok=True)
    curves_dir = out_dir / "per_run_curves"
    curves_dir.mkdir(parents=True, exist_ok=True)

    # Per-run fitness curves
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
        plt.savefig(curves_dir / f'{Path(row["file"]).stem}_fitness.png', dpi=150)
        plt.close()

    # Distribution of best fitness across all files
    best_fit = df["bestFitness"].dropna()
    if not best_fit.empty:
        plt.figure(figsize=(8, 4.5))
        plt.hist(best_fit, bins=min(15, max(5, len(best_fit))))
        plt.xlabel("Best fitness")
        plt.ylabel("Number of runs")
        plt.title("Distribution of best fitness across all runs")
        plt.tight_layout()
        plt.savefig(out_dir / "best_fitness_distribution.png", dpi=150)
        plt.close()

    # Boxplot by map variant
    grouped_variant = [grp["bestFitness"].dropna().values for _, grp in df.groupby("mapVariant")]
    labels_variant = [str(name) for name, _ in df.groupby("mapVariant")]
    if grouped_variant and any(len(g) > 0 for g in grouped_variant):
        plt.figure(figsize=(8, 4.5))
        plt.boxplot(grouped_variant, tick_labels=labels_variant)
        plt.xlabel("Map variant")
        plt.ylabel("Best fitness")
        plt.title("Best fitness by map variant")
        plt.tight_layout()
        plt.savefig(out_dir / "best_fitness_by_map_variant_boxplot.png", dpi=150)
        plt.close()

    # Final score distribution
    final_score = df["finalScore"].dropna()
    if not final_score.empty:
        plt.figure(figsize=(8, 4.5))
        plt.hist(final_score, bins=min(15, max(5, len(final_score))))
        plt.xlabel("Final score")
        plt.ylabel("Number of runs")
        plt.title("Distribution of final score across all runs")
        plt.tight_layout()
        plt.savefig(out_dir / "final_score_distribution.png", dpi=150)
        plt.close()

    # Best fitness vs steps
    scatter_df = df[["bestFitness", "stepsUsed"]].dropna()
    if not scatter_df.empty:
        plt.figure(figsize=(8, 4.5))
        plt.scatter(scatter_df["stepsUsed"], scatter_df["bestFitness"])
        plt.xlabel("Steps used")
        plt.ylabel("Best fitness")
        plt.title("Best fitness vs steps used")
        plt.tight_layout()
        plt.savefig(out_dir / "best_fitness_vs_steps.png", dpi=150)
        plt.close()

    # Success rate by map variant
    success_by_variant = df.groupby("mapVariant", dropna=False)["reachedGoal"].mean()
    if not success_by_variant.empty:
        plt.figure(figsize=(8, 4.5))
        plt.bar(success_by_variant.index.astype(str), success_by_variant.values)
        plt.xlabel("Map variant")
        plt.ylabel("Success rate")
        plt.title("Success rate by map variant")
        plt.ylim(0, 1)
        plt.tight_layout()
        plt.savefig(out_dir / "success_rate_by_map_variant.png", dpi=150)
        plt.close()

    # Overlay all fitness curves
    any_curves = False
    plt.figure(figsize=(8, 4.5))
    for _, row in df.iterrows():
        stats = row.get("generationStats", [])
        if not stats:
            continue
        gen_df = pd.DataFrame(stats)
        if "generation" not in gen_df or "bestFitness" not in gen_df:
            continue
        plt.plot(gen_df["generation"], gen_df["bestFitness"], alpha=0.5)
        any_curves = True

    if any_curves:
        plt.xlabel("Generation")
        plt.ylabel("Best fitness")
        plt.title("Best fitness curves for all runs")
        plt.tight_layout()
        plt.savefig(out_dir / "all_runs_fitness_curves.png", dpi=150)
    plt.close()

    print(f"\nSaved plots to: {out_dir}")


def main() -> None:
    parser = argparse.ArgumentParser(description="Evaluate Unity GA JSON logs.")
    parser.add_argument("--logs-dir", type=str, required=True, help="Directory containing GA JSON logs.")
    parser.add_argument("--export-csv", action="store_true", help="Export raw and summary CSV files.")
    parser.add_argument("--plot", action="store_true", help="Save aggregate and per-run plots.")
    parser.add_argument("--out-dir", type=str, default=None, help="Output directory for CSVs/plots (default: <logs-dir>/analysis)")
    args = parser.parse_args()

    logs_dir = Path(args.logs_dir)
    if not logs_dir.exists():
        raise FileNotFoundError(f"Logs directory does not exist: {logs_dir}")

    df = load_logs(logs_dir)

    print_overall_summary(df)
    print_grouped_summary(df)
    # print_transfer_matrix(df)

    out_dir = Path(args.out_dir) if args.out_dir else (logs_dir / "analysis")

    if args.export_csv:
        export_csvs(df, out_dir)

    if args.plot:
        plot_generation_curves(df, out_dir)


if __name__ == "__main__":
    main()
