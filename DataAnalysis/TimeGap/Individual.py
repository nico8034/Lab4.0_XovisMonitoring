import csv
from datetime import datetime


def calculate_timegaps(data):
    gaps = {}  # Dictionary to store timegaps per zone
    prev_timestamps = {}  # Dictionary to store previous timestamp per zone
    prev_zone = None  # Track the previous zone

    for row in data:
        zone = row["Zone"]
        current_timestamp = datetime.strptime(
            row["Date"] + " " + row["Timestamp"], "%Y-%m-%d %H:%M:%S.%f")

        if zone == prev_zone and zone in prev_timestamps:
            gap = (current_timestamp - prev_timestamps[zone]).total_seconds()

            if zone not in gaps:
                gaps[zone] = []

            gaps[zone].append(gap)

        prev_timestamps[zone] = current_timestamp
        prev_zone = zone

    return gaps


def compute_statistics(gaps):
    stats = {}

    for zone, zone_gaps in gaps.items():
        if zone_gaps:
            stats[zone] = {
                "longest_gap": max(zone_gaps),
                "average_gap": sum(zone_gaps) / len(zone_gaps)
            }

    return stats


# Read CSV data
data = []
with open("./TimeGap/data.csv", "r") as file:
    reader = csv.DictReader(file)
    for row in reader:
        if int(row["PersonCount"]) == 1:
            data.append(row)

# Calculate timegaps
gaps = calculate_timegaps(data)

# Compute statistics
statistics = compute_statistics(gaps)

# Write results to a txt file
with open("results_individual.txt", "w") as out_file:
    for zone, stats in statistics.items():
        out_file.write(f"Zone: {zone}\n")
        out_file.write(f"  Longest timegap: {stats['longest_gap']} seconds\n")
        out_file.write(
            f"  Average timegap: {stats['average_gap']} seconds\n\n")

print("Results written to results_individual.txt")
