import pandas as pd
import matplotlib.pyplot as plt
import os

# Input files (replace paths with your file paths)
input_files = [
    # fr"C:\Users\patry\Desktop\Mono,liths data\2VU\lms.monolith.csv",
    # fr"C:\Users\patry\Desktop\Mono,liths data\100VU\lms.monolith.csv",
    fr"C:\Users\patry\Desktop\Mono,liths data\500VU\lms.monolith.csv"
]

# Output folder
output_folder = "results"
os.makedirs(output_folder, exist_ok=True)  # Create the folder if it doesn't exist

# Function to process each file
def process_file(file_path):
    # Extract the filename without extension for output naming
    filename = os.path.splitext(os.path.basename(file_path))[0]
    output_table = os.path.join(output_folder, f"{filename}_metrics_summary.csv")
    output_image = os.path.join(output_folder, f"{filename}_visualization.png")

    # Load the file
    data = pd.read_csv(file_path, sep=None, engine='python')
    data.columns = data.columns.str.strip()

    # Convert Timestamp to datetime
    data['Timestamp'] = pd.to_datetime(data['Timestamp'], errors='coerce')

    # Convert CPU% to numeric
    data['CPU%'] = data['CPU%'].str.replace('%', '').astype(float)

    # Convert Memory Usage to numeric
    def convert_memory(mem):
        if 'MiB' in mem:
            return float(mem.replace('MiB', ''))
        elif 'GiB' in mem:
            return float(mem.replace('GiB', '')) * 1024  # Convert GiB to MiB
        return 0

    data['MemUsage'] = data['MemUsage'].apply(lambda x: x.split('/')[0].strip())
    data['MemUsage'] = data['MemUsage'].apply(convert_memory)

    # Convert NetIO to numeric
    def convert_netio(net):
        net_in, net_out = net.split('/')
        def to_bytes(val):
            val = val.strip()
            if 'kB' in val:
                return float(val.replace('kB', '')) * 1024
            elif 'MB' in val:
                return float(val.replace('MB', '')) * 1024 * 1024
            elif 'GB' in val:
                return float(val.replace('GB', '')) * 1024 * 1024 * 1024
            return float(val)
        return to_bytes(net_in) + to_bytes(net_out)

    data['NetIO'] = data['NetIO'].apply(convert_netio)

    # Calculate statistics
    stats = pd.DataFrame({
        "Metric": ["CPU%", "Memory Usage (MiB)", "Network I/O (MB)"],
        "Average": [data['CPU%'].mean(), data['MemUsage'].mean(), data['NetIO'].mean() / (1024 * 1024)],
        "Min": [data['CPU%'].min(), data['MemUsage'].min(), data['NetIO'].min() / (1024 * 1024)],
        "Max": [data['CPU%'].max(), data['MemUsage'].max(), data['NetIO'].max() / (1024 * 1024)],
    })

    # Save the table to a CSV file
    stats.to_csv(output_table, index=False)
    print(f"Metrics summary table saved to: {output_table}")

    # Plot visualizations
    plt.figure(figsize=(12, 8))

    # CPU Usage
    plt.subplot(3, 1, 1)
    plt.plot(data['Timestamp'], data['CPU%'], label='CPU%', color='blue')
    plt.title('CPU Usage Over Time')
    plt.ylabel('CPU%')
    plt.grid()

    # Memory Usage
    plt.subplot(3, 1, 2)
    plt.plot(data['Timestamp'], data['MemUsage'], label='Memory Usage (MiB)', color='green')
    plt.title('Memory Usage Over Time')
    plt.ylabel('Memory (MiB)')
    plt.grid()

    # Network I/O
    plt.subplot(3, 1, 3)
    plt.plot(data['Timestamp'], data['NetIO'] / (1024 * 1024), label='Network I/O (MB)', color='orange')
    plt.title('Network I/O Over Time')
    plt.ylabel('Network I/O (MB)')
    plt.xlabel('Timestamp')
    plt.grid()

    # Save the plot as a PNG file
    plt.tight_layout()
    plt.savefig(output_image)
    print(f"Visualization saved to: {output_image}")
    plt.close()

# Process all files
for file in input_files:
    process_file(file)

print(f"All outputs have been saved to the '{output_folder}' folder.")
