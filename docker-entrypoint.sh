#!/bin/bash
set -e

# ============================================
# 1. Wait for SQL Server
# ============================================
echo "==> Waiting for SQL Server..."
for i in $(seq 1 60); do
    if timeout 1 bash -c '</dev/tcp/sqlserver/1433' >/dev/null 2>&1; then
        echo "==> SQL Server is ready."
        break
    fi
    echo "    SQL Server not ready yet (attempt $i/60)... waiting 2s"
    sleep 2
done

# ============================================
# 2. Start the application
# ============================================
echo "==> Starting SPSUL application..."
exec dotnet SPSUL.dll
