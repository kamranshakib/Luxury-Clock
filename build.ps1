Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "G:\win - software\Clock"
Write-Host "Building Luxury Clock installer..." -ForegroundColor Cyan
cmd.exe /c "npm run build:win"
Write-Host ""
Write-Host "Build Complete! You can find your Setup.exe in the 'release' folder." -ForegroundColor Green
Read-Host "Press Enter to exit"
