Write-Host "========================================================" -ForegroundColor Cyan
Write-Host "  Lancement de Proman (InterfaceProjet) - Mode Autonome" -ForegroundColor Cyan
Write-Host "========================================================" -ForegroundColor Cyan
Write-Host ""

# Fermer l'application existante si elle est en cours
Get-Process -Name "InterfaceProjet" -ErrorAction SilentlyContinue | Stop-Process -Force

Write-Host "[1/2] Préparation et publication autonome (Self-Contained)..." -ForegroundColor Yellow
$projectDir = Join-Path $PSScriptRoot "InterfaceProjet"
Set-Location -Path $projectDir

dotnet publish -c Release -r win-x64 --self-contained true --nologo -v q
if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERREUR] La publication a échoué." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "[2/2] Démarrage de l'application..." -ForegroundColor Green
$exeDir = Join-Path $projectDir "bin\Release\net8.0-windows10.0.19041.0\win-x64\publish"
$exePath = Join-Path $exeDir "InterfaceProjet.exe"

if (Test-Path $exePath) {
    Start-Process -FilePath $exePath -WorkingDirectory $exeDir
    Write-Host "Application lancée avec succès." -ForegroundColor Green
} else {
    Write-Host "[ERREUR] Exécutable introuvable : $exePath" -ForegroundColor Red
}
