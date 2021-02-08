#!/usr/bin/env bash

# script inspired by https://andrewlock.net/simplifying-the-cake-global-tool-bootstrapper-scripts-in-netcore3-with-local-tools/

# Define default arguments.
SCRIPT="build.cake"
CAKE_ARGUMENTS=()

# Parse arguments.
for i in "$@"; do
    if [ -n "$2" ]; then
        case $1 in
            -s|--script) SCRIPT="$2"; shift ;;
            --) shift; CAKE_ARGUMENTS+=("$@"); break ;;
            *) CAKE_ARGUMENTS+=("$1") ;;
        esac
    fi
    shift
done

# Restore Cake tool
dotnet tool restore

if [ $? -ne 0 ]; then
    echo "An error occured while installing Cake."
    exit 1
fi

# Start Cake
dotnet tool run dotnet-cake "$SCRIPT" "${CAKE_ARGUMENTS[@]}"