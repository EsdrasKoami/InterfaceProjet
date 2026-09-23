#!/bin/bash
set -e

echo "========================================================"
echo "  Lancement de Proman (InterfaceProjet) - Mode Autonome"
echo "========================================================"
echo ""

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR/InterfaceProjet"

echo "[1/2] Préparation et publication autonome (Self-Contained)..."
dotnet publish -c Release -r win-x64 --self-contained true --nologo -v q

echo "[2/2] Démarrage de l'application..."
EXE_PATH="./bin/Release/net8.0-windows10.0.19041.0/win-x64/publish/InterfaceProjet.exe"

if [ -f "$EXE_PATH" ]; then
    cmd.exe /c start "" "$EXE_PATH"
    echo "Application lancée avec succès."
else
    echo "[ERREUR] Exécutable introuvable : $EXE_PATH"
    exit 1
fi

