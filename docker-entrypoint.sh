#!/bin/bash
set -e

CONNECTION_STRING="${ConnectionStrings__Default}"

echo "==> Waiting for SQL Server to be ready..."
for i in $(seq 1 30); do
    if dotnet-sql-cache create "$CONNECTION_STRING" dbo Sessions 2>/dev/null; then
        echo "==> Sessions table ensured."
        break
    fi
    echo "    SQL Server not ready yet (attempt $i/30)... waiting 2s"
    sleep 2
done

echo "==> Applying EF Core migrations..."
# Use the EF bundle or just start the app — migrations are applied via the app
# The app will start and connect to the DB

echo "==> Starting SPSUL application..."
exec dotnet SPSUL.dll
