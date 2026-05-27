#!/usr/bin/env bash
set -euo pipefail

WWWROOT="${1:?wwwroot directory required}"

BASE_DIR=$(python3 -c "
import json
from pathlib import Path

cfg = json.loads(Path('${WWWROOT}/config.json').read_text())
d = (cfg.get('BaseDir') or '/').strip() or '/'
if not d.startswith('/'):
    d = '/' + d
if not d.endswith('/'):
    d += '/'
print(d)
")

for f in "$WWWROOT/index.html" "$WWWROOT/404.html"; do
  sed -i "s|<base href=\"/\" />|<base href=\"${BASE_DIR}\" />|g" "$f"
done

echo "Set base href to ${BASE_DIR} in index.html and 404.html"
