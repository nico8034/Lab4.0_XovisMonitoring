import csv
from datetime import datetime


def calculate_timegaps(data):
    gaps = []
    prev_timestamp = None

    for row in data:
        current_timestamp = datetime.strptime(
            row["Date"] + " " + row["Timestamp"], "%Y-%m-%d %H:%M:%S.%f")

        if prev_timestamp:
            gap = (current_timestamp - prev_timestamp).total_seconds()
            gaps.append(gap)

        prev_timestamp = current_timestamp

    return gaps


def compute_statistics(gaps):
    if not gaps:
        return {
            "longest_gap": 0,
            "average_gap": 0
        }
    return {
        "longest_gap": max(gaps),
        "average_gap": sum(gaps) / len(gaps)
    }


# Read CSV data
data = []
with open("./TimeGap/data.csv", "r") as file:
    reader = csv.DictReader(file)
    for row in reader:
        if int(row["PersonCount"]) == 1:
            data.append(row)

# Calculate timegaps across all zones
gaps = calculate_timegaps(data)

# Compute statistics
statistics = compute_statistics(gaps)

# Write results to a txt file
with open("results_total.txt", "w") as out_file:
    out_file.write(
        f"Largest timegap across all zones: {statistics['longest_gap']} seconds\n")
    out_file.write(
        f"Average timegap across all zones: {statistics['average_gap']} seconds\n")

print("Results written to results_total.txt")
