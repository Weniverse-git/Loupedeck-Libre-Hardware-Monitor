# LLHMPlugin .lplug4 패키지 빌드 스크립트
# 사용법: powershell -ExecutionPolicy Bypass -File build-lplug4.ps1

$ErrorActionPreference = "Stop"

$projectDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$srcDir = Join-Path $projectDir "src"
$metadataDir = Join-Path $projectDir "metadata"
$outputDir = Join-Path $projectDir "release"

Write-Host "=== LLHMPlugin .lplug4 Builder ===" -ForegroundColor Green

# 1. Release 빌드
Write-Host "`n[1/3] Building Release..." -ForegroundColor Cyan
Push-Location $srcDir
dotnet build -c Release 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    Pop-Location
    exit 1
}
Pop-Location

# 2. 패키지 준비 (.NET 8 플러그인 구조: win/ 하위 폴더)
Write-Host "`n[2/3] Preparing package..." -ForegroundColor Cyan
$stagingDir = Join-Path $outputDir "staging"
if (Test-Path $stagingDir) { Remove-Item -Recurse -Force $stagingDir }
New-Item -ItemType Directory -Path $stagingDir -Force | Out-Null
New-Item -ItemType Directory -Path (Join-Path $stagingDir "metadata") -Force | Out-Null
New-Item -ItemType Directory -Path (Join-Path $stagingDir "win") -Force | Out-Null

# DLL을 win/ 하위 폴더에 복사
$dllPath = Join-Path $srcDir "bin\Release\LLHMPlugin.dll"
Copy-Item $dllPath (Join-Path $stagingDir "win")

# deps.json을 win/ 하위 폴더에 복사
$depsPath = Join-Path $srcDir "bin\Release\LLHMPlugin.deps.json"
if (Test-Path $depsPath) {
    Copy-Item $depsPath (Join-Path $stagingDir "win")
}

# metadata 복사 (YAML은 UTF-8 no BOM + LF 인코딩으로 변환)
$yamlSrc = Join-Path $metadataDir "LoupedeckPackage.yaml"
$yamlDst = Join-Path $stagingDir "metadata\LoupedeckPackage.yaml"
$yamlContent = [System.IO.File]::ReadAllText($yamlSrc)
$yamlContent = $yamlContent -replace "`r`n", "`n"
$yamlContent = $yamlContent -replace "`r", "`n"
$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
[System.IO.File]::WriteAllText($yamlDst, $yamlContent, $utf8NoBom)

$iconPath = Join-Path $metadataDir "Icon256x256.png"
if (Test-Path $iconPath) {
    Copy-Item $iconPath (Join-Path $stagingDir "metadata")
}

# 3. ZIP → .lplug4
Write-Host "`n[3/3] Creating .lplug4 package..." -ForegroundColor Cyan
$zipPath = Join-Path $outputDir "LLHMPlugin.zip"
$lplug4Path = Join-Path $outputDir "LLHMPlugin.lplug4"
if (Test-Path $zipPath) { Remove-Item $zipPath }
if (Test-Path $lplug4Path) { Remove-Item $lplug4Path }
Compress-Archive -Path (Join-Path $stagingDir "*") -DestinationPath $zipPath -Force
Rename-Item $zipPath $lplug4Path

# 정리
Remove-Item -Recurse -Force $stagingDir

$fileSize = (Get-Item $lplug4Path).Length / 1KB
Write-Host "`n=== Build Complete ===" -ForegroundColor Green
Write-Host "Output: $lplug4Path ($([math]::Round($fileSize, 1)) KB)" -ForegroundColor Yellow
Write-Host "Install: Double-click the .lplug4 file to install" -ForegroundColor Yellow
