#!/bin/bash

# Usage: ./increment_version.sh <VERSION> <OUTPUT>

set -euo pipefail

# Check if the correct number of arguments is provided
if [ "$#" -ne 2 ]; then
  echo "Usage: $0 <VERSION> <OUTPUT>"
  exit 1
fi

# Set the version and output from arguments
VERSION="$1"
OUTPUT="$2"

# Extract imageTag values from the output
TAGS=$(echo $OUTPUT | jq -r '.imageIds[].imageTag')

# Find the latest version matching the given pattern
LATEST_VERSION=""
for TAG in $TAGS; do
  if [[ $TAG =~ ^$VERSION\.([0-9]+)$ ]]; then
    CURRENT_VERSION=${BASH_REMATCH[1]}
    if [ -z $LATEST_VERSION ] || [ $CURRENT_VERSION -gt $LATEST_VERSION ]; then
      LATEST_VERSION=$CURRENT_VERSION
    fi
  fi
done

# Increment the version
if [ -z $LATEST_VERSION ]; then
  INCREMENTED_VERSION="${VERSION}.1"
else
  INCREMENTED_VERSION="${VERSION}.$((LATEST_VERSION + 1))"
fi

# Print the result
echo "$INCREMENTED_VERSION"
