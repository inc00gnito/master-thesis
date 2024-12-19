#!/bin/bash

# Define output directory for Docker stats and k6 results
OUTPUT_DIR="docker_stats_output"
K6_OUTPUT_FILE="k6_test_output.json"

# Cleanup old files and directories
rm -rf $OUTPUT_DIR
mkdir -p $OUTPUT_DIR
rm -f $K6_OUTPUT_FILE

# Define Docker container names (adjust as per your setup)
CONTAINER_NAMES=("lms.monolith")

# Function to start Docker stats
start_docker_stats() {
    echo "Starting Docker stats for each container..."

    # Add CSV headers for each container file
    for CONTAINER in "${CONTAINER_NAMES[@]}"; do
        echo "Timestamp,Container,Name,CPU%,MemUsage,NetIO,BlockIO" > "${OUTPUT_DIR}/${CONTAINER}.csv"
    done

    # Start collecting stats in a loop
    while true; do
        timestamp=$(date +"%Y-%m-%d %H:%M:%S")
        for CONTAINER in "${CONTAINER_NAMES[@]}"; do
            docker stats --no-stream --format "$timestamp,{{.Container}},{{.Name}},{{.CPUPerc}},{{.MemUsage}},{{.NetIO}},{{.BlockIO}}" $CONTAINER \
                >> "${OUTPUT_DIR}/${CONTAINER}.csv"
        done
        sleep 2 # Adjust interval as needed
    done
}

# Start Docker stats in the background
start_docker_stats &
DOCKER_STATS_PID=$!

# Run the k6 test
echo "Starting k6 test..."
k6 run --summary-export=final_output.json testing.js

# Stop Docker stats
echo "Stopping Docker stats..."
kill $DOCKER_STATS_PID

# Print completion message
echo "k6 test completed. Outputs:"
echo " - Docker stats saved in: $OUTPUT_DIR"
echo " - k6 results: $K6_OUTPUT_FILE"
