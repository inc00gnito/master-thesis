#!/bin/bash

# Output file
DOCKER_STATS_FILE="microservices-docker_stats_over_time_bash.csv"

# Clear existing file
echo "Timestamp,Container,Name,CPU%,MemUsage,NetIO,BlockIO" > $DOCKER_STATS_FILE

# Loop to capture stats every 2 seconds
while true; do
  echo "Saving to csv file..."
  timestamp=$(date +"%Y-%m-%d %H:%M:%S")
  docker stats --no-stream --format "$timestamp,{{.Container}},{{.Name}},{{.CPUPerc}},{{.MemUsage}},{{.NetIO}},{{.BlockIO}}" >> $DOCKER_STATS_FILE
  sleep 2
done