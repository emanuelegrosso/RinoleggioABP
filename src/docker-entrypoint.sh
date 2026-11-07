#!/bin/sh
set -e

# /data is a persistent Fly volume. On first start, seed it with the database
# bundled in the image; afterwards the volume copy is never overwritten.
mkdir -p /data/home
if [ ! -f /data/Rinoleggio.db ]; then
  echo "Seeding /data/Rinoleggio.db from the image copy"
  cp /app/seed/Rinoleggio.db /data/Rinoleggio.db
fi

# ASP.NET Data Protection keys live in $HOME/.aspnet: keep them on the volume
# so cookies/sessions survive restarts and redeploys.
export HOME=/data/home

exec dotnet Rinoleggio.Web.dll
