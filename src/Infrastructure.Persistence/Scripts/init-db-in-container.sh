#!/bin/bash
sleep 20

echo "Waiting for SQL Server to start..."

until /opt/mssql-tools/bin/sqlcmd -S sqlserver -U sa -P "${MSSQL_SA_PASSWORD}" -Q "SELECT 1" > /dev/null 2>&1; do
  echo "SQL Server not ready yet. Waiting..."
  sleep 5
done

echo "SQL Server is up. Running initialization script..."

/opt/mssql-tools/bin/sqlcmd -S sqlserver -U sa -P "${MSSQL_SA_PASSWORD}" -d master -i /opt/mssql-init/init.sql

echo "Database initialization script run successfully"
