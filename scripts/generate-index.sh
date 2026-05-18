#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

POSTS="$REPO_ROOT/src/BoneLog.Blazor/wwwroot/data/posts"
INDEX="$REPO_ROOT/src/BoneLog.Blazor/wwwroot/data/index.json"

dotnet run "$SCRIPT_DIR/GenerateIndex.cs" -- "$POSTS" "$INDEX"
