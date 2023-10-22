import pandas as pd
import matplotlib.pyplot as plt
import matplotlib.dates as mdates
from matplotlib.ticker import FuncFormatter

# Example data reading
try:
    data = pd.read_csv('data.csv')
except FileNotFoundError:
    print("File not found. Please check your file path and name.")
    exit()  # Exiting the script if file not found

# Convert 'Timestamp' to datetime type for proper plotting
data['Timestamp'] = pd.to_datetime(data['Timestamp'], format='%H:%M:%S.%f')

# Filter data where Person Count is > 0
data = data[data['PersonCount'] > 0]

# Setting up the plot
plt.figure(figsize=(15, 8))

# Custom function to format the x-axis labels


def format_milli_second(x, _):
    dt = mdates.num2date(x)
    # display microseconds as milliseconds
    return dt.strftime('%H:%M:%S')


# Apply custom function to the X-axis
plt.gca().xaxis.set_major_formatter(FuncFormatter(format_milli_second))
plt.gca().xaxis.set_major_locator(mdates.SecondLocator())

# Plot data for each zone
for zone in data['Zone'].unique():
    subset = data[data['Zone'] == zone]
    plt.plot(subset['Timestamp'], subset['Zone'], 'o')

# Format plot
plt.xticks(rotation='vertical')  # Rotate x-axis labels vertically
plt.xlabel('Time')
plt.ylabel('Zone')
plt.title('Zone Markings Over Time')
plt.grid(True)
plt.tight_layout()

# Save or display the plot
plt.savefig('zone_plot.png')
plt.show()

unique_zone_count = data['Zone'].nunique()
print(f'There are {unique_zone_count} unique zones in the data.')
unique_zones = sorted(data['Zone'].unique())
print("Zones in ascending order:")
for zone in unique_zones:
    print(zone)
