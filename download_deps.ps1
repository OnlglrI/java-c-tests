# =============================================================================
# download_deps.ps1 - скачивает ВСЕ зависимости для всех модулей (S3-S6)
#
# Результат:
#   libs\maven\  - jar'ы для S3_back, S5_back, S6_back (Java/Spring Boot)
#   libs\nuget\  - пакеты для S4_Client, S5_desk, S6_desk (C# WinForms)
#
# После запуска этого скрипта интернет больше не нужен.
#
# Использование при офлайн-сборке:
#   Maven : mvn package -o -Dmaven.repo.local=..\..\libs\maven
#   NuGet : dotnet restore   (nuget.config уже прописан в каждом проекте)
#           dotnet build --no-restore
#
# Запуск: .\download_deps.ps1
# =============================================================================

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$LibsMaven = "$ScriptDir\libs\maven"
$LibsNuget = "$ScriptDir\libs\nuget"

function Write-Info ($msg) { Write-Host "[INFO] $msg" -ForegroundColor Green  }
function Write-Warn ($msg) { Write-Host "[WARN] $msg" -ForegroundColor Yellow }
function Write-Err  ($msg) { Write-Host "[ERROR] $msg" -ForegroundColor Red; exit 1 }

# --------------------------------------------------------------------------- #
# Проверка инструментов
# --------------------------------------------------------------------------- #
if (-not (Get-Command mvn    -ErrorAction SilentlyContinue)) { Write-Err  "Maven не найден. Установи с https://maven.apache.org/" }
if (-not (Get-Command java   -ErrorAction SilentlyContinue)) { Write-Err  "Java не найдена. Установи JDK 17+" }
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { Write-Warn "dotnet не найден - NuGet-зависимости будут пропущены" }

# --------------------------------------------------------------------------- #
# Создаём папки
# --------------------------------------------------------------------------- #
Write-Info "Создание папок libs\maven и libs\nuget..."
New-Item -ItemType Directory -Force -Path $LibsMaven | Out-Null
New-Item -ItemType Directory -Force -Path $LibsNuget  | Out-Null

# =========================================================================== #
#  MAVEN - Java бэкенды (S3, S5, S6)
# =========================================================================== #

function Invoke-MavenDownload {
    param (
        [string]$Label,
        [string]$PomDir
    )

    if (-not (Test-Path "$PomDir\pom.xml")) {
        Write-Warn "[$Label] pom.xml не найден в $PomDir - пропускаю"
        return
    }

    Write-Info "[$Label] Скачиваю Maven-зависимости..."
    mvn -f "$PomDir\pom.xml" `
        dependency:go-offline `
        "-Dmaven.repo.local=$LibsMaven" `
        --no-transfer-progress `
        -q

    if ($LASTEXITCODE -ne 0) {
        Write-Warn "[$Label] Maven завершился с ошибкой (код $LASTEXITCODE)"
    } else {
        Write-Info "[$Label] Maven-зависимости скачаны"
    }
}

# --- S3 back ---
Invoke-MavenDownload -Label "S3_back" -PomDir "$ScriptDir\S3_plan\S3_SpringServer"

# --- S5 back ---
Invoke-MavenDownload -Label "S5_back" -PomDir "$ScriptDir\S5_plan\S5_back"

# --- S6 back ---
Invoke-MavenDownload -Label "S6_back" -PomDir "$ScriptDir\S6_plan\S6_back"

# =========================================================================== #
#  NUGET - C# клиенты (S4, S5_desk, S6_desk)
# =========================================================================== #

function Invoke-NugetDownload {
    param (
        [string]$Label,
        [string]$ProjDir,
        [string]$Csproj
    )

    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        Write-Warn "[$Label] dotnet не найден - пропускаю"
        return
    }

    if (-not (Test-Path "$ProjDir\$Csproj")) {
        Write-Warn "[$Label] $Csproj не найден в $ProjDir - пропускаю"
        return
    }

    Write-Info "[$Label] Скачиваю NuGet-зависимости..."

    dotnet restore "$ProjDir\$Csproj" `
        --packages $LibsNuget `
        --no-http-cache | Out-Null

    if ($LASTEXITCODE -ne 0) {
        Write-Warn "[$Label] dotnet restore завершился с ошибкой (код $LASTEXITCODE)"
        return
    }

    # Относительный путь от папки проекта до libs\nuget
    $RelPath = [System.IO.Path]::GetRelativePath($ProjDir, $LibsNuget)

    # Пишем nuget.config в папку проекта для офлайн-сборки
    $NugetConfig = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <config>
    <add key="globalPackagesFolder" value="$RelPath" />
  </config>
  <packageSources>
    <clear />
    <add key="local" value="$RelPath" />
  </packageSources>
</configuration>
"@
    Set-Content -Encoding UTF8 -Path "$ProjDir\nuget.config" -Value $NugetConfig
    Write-Info "[$Label] NuGet-зависимости скачаны, nuget.config записан"
}

# --- S4 client ---
Invoke-NugetDownload -Label "S4_Client" -ProjDir "$ScriptDir\S4_plan\S4_Client" -Csproj "S4_Client.csproj"

# --- S5 desk ---
Invoke-NugetDownload -Label "S5_desk"   -ProjDir "$ScriptDir\S5_plan\S5_desk"   -Csproj "S5_desk.csproj"

# --- S6 desk ---
Invoke-NugetDownload -Label "S6_desk"   -ProjDir "$ScriptDir\S6_plan\S6_desk"   -Csproj "S6_desk.csproj"

# =========================================================================== #
#  Итог
# =========================================================================== #
$MavenSize = if (Test-Path $LibsMaven) { "{0:N0} MB" -f ((Get-ChildItem $LibsMaven -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB) } else { "0 MB" }
$NugetSize  = if (Test-Path $LibsNuget)  { "{0:N0} MB" -f ((Get-ChildItem $LibsNuget  -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB) } else { "0 MB" }

Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host "  Все зависимости скачаны!"                  -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Green
Write-Host ""
Write-Host "  Папки:"
Write-Host "    libs\maven\  - $MavenSize - Maven jar'ы"
Write-Host "    libs\nuget\  - $NugetSize  - NuGet пакеты"
Write-Host ""
Write-Host "  Офлайн-сборка Maven:"
Write-Host "    cd S3_plan\S3_SpringServer"
Write-Host "    mvn package -o -Dmaven.repo.local=..\..\libs\maven"
Write-Host ""
Write-Host "  Офлайн-сборка NuGet:"
Write-Host "    cd S4_plan\S4_Client"
Write-Host "    dotnet restore"
Write-Host "    dotnet build --no-restore"
Write-Host ""
