@echo off
setlocal enabledelayedexpansion

echo ========================================================
echo   Lancement de Proman (InterfaceProjet) - Mode Autonome
echo ========================================================
echo.

:: Se positionner dans le dossier du script
cd /d "%~dp0InterfaceProjet"

:: Fermer l'application si elle tourne deja pour eviter les verrous de fichiers
taskkill /f /im InterfaceProjet.exe >nul 2>&1

echo [1/2] Preparation du package autonome (Self-Contained)...
dotnet publish -c Release -r win-x64 --self-contained true --nologo -v q

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERREUR] La compilation ou la publication a echoue.
    pause
    exit /b %ERRORLEVEL%
)

echo [2/2] Demarrage de l'application...
set "EXE_DIR=%~dp0InterfaceProjet\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish"
set "EXE_PATH=%EXE_DIR%\InterfaceProjet.exe"

if exist "%EXE_PATH%" (
    start "" /d "%EXE_DIR%" "InterfaceProjet.exe"
    echo Application lancee avec succes.
) else (
    echo [ERREUR] L'executable est introuvable a l'emplacement : %EXE_PATH%
    pause
)

