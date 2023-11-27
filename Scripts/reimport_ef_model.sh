#!/bin/sh
set -eu

CONTEXT_FILE_NAME="Xdigf001Context.cs"

MODELS_SHARED="MysticLegendsShared/Models"
MODELS_SERVER="MysticLegendsServer/Models"

(cd MysticLegendsServer && dotnet ef dbcontext scaffold "Name=ConnectionStrings:GameDB" Npgsql.EntityFrameworkCore.PostgreSQL -o Models --no-onconfiguring --force)

find "$MODELS_SHARED/" -type f -delete

for file in "$MODELS_SERVER"/*; do
  if [ "$file" != "$MODELS_SERVER/$CONTEXT_FILE_NAME" ]; then
    mv "$file" "$MODELS_SHARED/"
  fi
done

sed -i 's/MysticLegendsServer/MysticLegendsShared/g' "$MODELS_SHARED"/*
sed -i '1 i\using MysticLegendsShared.Models;' "$MODELS_SERVER/$CONTEXT_FILE_NAME"

# convert to CRLF
if [ $(command -v unix2dos | wc -l) -eq 1 ]; then
    echo "Converting to CRLF"
    unix2dos "$MODELS_SHARED"/* 2>/dev/null
    unix2dos "$MODELS_SERVER/$CONTEXT_FILE_NAME" 2>/dev/null
fi
