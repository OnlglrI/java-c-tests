# =============================================================================
# download_deps.ps1 - скачивает ВСЕ зависимости для всех модулей (S3-S6)
#
# Результат:
#   libs\maven\  - jar'ы для Java/Spring Boot бэкендов
#   libs\nuget\  - пакеты для C# WinForms клиентов
#
# Скрипт НЕЗАВИСИМЫЙ - не требует наличия проектных папок S3/S5/S6.
# Все зависимости прописаны явно.
#
# После запуска этого скрипта интернет больше не нужен.
#
# Использование при офлайн-сборке:
#   Maven : mvn package -o -Dmaven.repo.local=..\..\libs\maven
#   NuGet : dotnet restore --packages ..\..\libs\nuget
#           dotnet build --no-restore
#
# Запуск: .\download_deps.ps1
# =============================================================================

$ScriptDir  = Split-Path -Parent $MyInvocation.MyCommand.Path
$LibsMaven  = "$ScriptDir\libs\maven"
$LibsNuget  = "$ScriptDir\libs\nuget"
$TempDir    = "$ScriptDir\_tmp_deps"

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
New-Item -ItemType Directory -Force -Path $TempDir    | Out-Null

# =========================================================================== #
#  MAVEN - все Java-зависимости проектов S3-basic, S5-auth, S6-tests
#
#  Spring Boot:   3.2.0
#  Java:          17
#  JJWT:          0.12.3
#  mysql-connector-j: управляется Spring Boot BOM (3.2.0)
#  lombok:        управляется Spring Boot BOM
#  h2:            управляется Spring Boot BOM
# =========================================================================== #

Write-Info "Создание временного pom.xml со всеми зависимостями..."

$PomContent = @"
<?xml version="1.0" encoding="UTF-8"?>
<project xmlns="http://maven.apache.org/POM/4.0.0"
         xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
         xsi:schemaLocation="http://maven.apache.org/POM/4.0.0
         https://maven.apache.org/xsd/maven-4.0.0.xsd">
    <modelVersion>4.0.0</modelVersion>

    <parent>
        <groupId>org.springframework.boot</groupId>
        <artifactId>spring-boot-starter-parent</artifactId>
        <version>3.2.0</version>
    </parent>

    <groupId>com.example</groupId>
    <artifactId>all-deps-downloader</artifactId>
    <version>1.0.0</version>
    <name>All Deps Downloader</name>

    <properties>
        <java.version>17</java.version>
        <jjwt.version>0.12.3</jjwt.version>
    </properties>

    <dependencies>

        <!-- ===== S3-basic, S5-auth, S6-tests: общие зависимости ===== -->

        <!-- Spring Boot Web (REST API, Tomcat) -->
        <dependency>
            <groupId>org.springframework.boot</groupId>
            <artifactId>spring-boot-starter-web</artifactId>
        </dependency>

        <!-- Spring Data JPA + Hibernate -->
        <dependency>
            <groupId>org.springframework.boot</groupId>
            <artifactId>spring-boot-starter-data-jpa</artifactId>
        </dependency>

        <!-- Bean Validation (javax/jakarta) -->
        <dependency>
            <groupId>org.springframework.boot</groupId>
            <artifactId>spring-boot-starter-validation</artifactId>
        </dependency>

        <!-- MySQL Connector (версия управляется BOM Spring Boot 3.2.0 -> 8.x) -->
        <dependency>
            <groupId>com.mysql</groupId>
            <artifactId>mysql-connector-j</artifactId>
            <scope>runtime</scope>
        </dependency>

        <!-- Lombok (кодогенерация, опциональный) -->
        <dependency>
            <groupId>org.projectlombok</groupId>
            <artifactId>lombok</artifactId>
            <optional>true</optional>
        </dependency>

        <!-- ===== S5-auth, S6-tests: Security + JWT ===== -->

        <!-- Spring Security -->
        <dependency>
            <groupId>org.springframework.boot</groupId>
            <artifactId>spring-boot-starter-security</artifactId>
        </dependency>

        <!-- JJWT API 0.12.3 -->
        <dependency>
            <groupId>io.jsonwebtoken</groupId>
            <artifactId>jjwt-api</artifactId>
            <version>${jjwt.version}</version>
        </dependency>

        <!-- JJWT Impl 0.12.3 (runtime) -->
        <dependency>
            <groupId>io.jsonwebtoken</groupId>
            <artifactId>jjwt-impl</artifactId>
            <version>${jjwt.version}</version>
            <scope>runtime</scope>
        </dependency>

        <!-- JJWT Jackson 0.12.3 (runtime) -->
        <dependency>
            <groupId>io.jsonwebtoken</groupId>
            <artifactId>jjwt-jackson</artifactId>
            <version>${jjwt.version}</version>
            <scope>runtime</scope>
        </dependency>

        <!-- ===== S6-tests: тестовые зависимости ===== -->

        <!-- Spring Boot Test (JUnit 5, Mockito) -->
        <dependency>
            <groupId>org.springframework.boot</groupId>
            <artifactId>spring-boot-starter-test</artifactId>
            <scope>test</scope>
        </dependency>

        <!-- Spring Security Test -->
        <dependency>
            <groupId>org.springframework.security</groupId>
            <artifactId>spring-security-test</artifactId>
            <scope>test</scope>
        </dependency>

        <!-- H2 in-memory БД для тестов -->
        <dependency>
            <groupId>com.h2database</groupId>
            <artifactId>h2</artifactId>
            <scope>test</scope>
        </dependency>

    </dependencies>

    <build>
        <plugins>
            <plugin>
                <groupId>org.springframework.boot</groupId>
                <artifactId>spring-boot-maven-plugin</artifactId>
            </plugin>
        </plugins>
    </build>
</project>
"@

Set-Content -Encoding UTF8 -Path "$TempDir\pom.xml" -Value $PomContent

Write-Info "Скачиваю все Maven-зависимости (Spring Boot 3.2.0 + JJWT 0.12.3)..."
mvn -f "$TempDir\pom.xml" `
    dependency:go-offline `
    "-Dmaven.repo.local=$LibsMaven" `
    --no-transfer-progress

if ($LASTEXITCODE -ne 0) {
    Write-Warn "Maven завершился с ошибкой (код $LASTEXITCODE)"
} else {
    Write-Info "Maven-зависимости успешно скачаны"
}

# =========================================================================== #
#  NUGET - C# WinForms клиенты (D3App, D5App)
#
#  Target framework: net8.0-windows
#  Внешних NuGet-пакетов нет - используется только встроенный SDK
#  (.NET WinForms компоненты поставляются вместе с .NET 8 SDK)
#
#  Если позже добавятся пакеты - добавить <PackageReference> ниже.
# =========================================================================== #

if (Get-Command dotnet -ErrorAction SilentlyContinue) {

    Write-Info "Создание временного .csproj для net8.0-windows..."

    $CsprojContent = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <!-- PackageReference-зависимости отсутствуют: WinForms входит в .NET 8 SDK -->
</Project>
"@

    $TempCsprojDir = "$TempDir\winforms_temp"
    New-Item -ItemType Directory -Force -Path $TempCsprojDir | Out-Null
    Set-Content -Encoding UTF8 -Path "$TempCsprojDir\TempApp.csproj" -Value $CsprojContent

    Write-Info "Скачиваю NuGet-зависимости (net8.0-windows WinForms)..."
    dotnet restore "$TempCsprojDir\TempApp.csproj" `
        --packages $LibsNuget

    if ($LASTEXITCODE -ne 0) {
        Write-Warn "dotnet restore завершился с ошибкой (код $LASTEXITCODE)"
    } else {
        Write-Info "NuGet-зависимости успешно скачаны"
    }

} else {
    Write-Warn "dotnet не найден - NuGet-зависимости пропущены"
}

# --------------------------------------------------------------------------- #
# Удаляем временную папку
# --------------------------------------------------------------------------- #
Write-Info "Удаление временных файлов..."
Remove-Item -Recurse -Force -Path $TempDir

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
Write-Host "  Скачанные Maven-зависимости:"
Write-Host "    spring-boot-starter-parent   3.2.0"
Write-Host "    spring-boot-starter-web      (управляется BOM)"
Write-Host "    spring-boot-starter-data-jpa (управляется BOM)"
Write-Host "    spring-boot-starter-validation (управляется BOM)"
Write-Host "    spring-boot-starter-security (управляется BOM)"
Write-Host "    spring-boot-starter-test     (управляется BOM)"
Write-Host "    spring-security-test         (управляется BOM)"
Write-Host "    mysql-connector-j            (управляется BOM)"
Write-Host "    h2                           (управляется BOM)"
Write-Host "    lombok                       (управляется BOM)"
Write-Host "    jjwt-api                     0.12.3"
Write-Host "    jjwt-impl                    0.12.3"
Write-Host "    jjwt-jackson                 0.12.3"
Write-Host ""
Write-Host "  Скачанные NuGet-зависимости:"
Write-Host "    .NET 8 WinForms SDK пакеты   (net8.0-windows)"
Write-Host ""
Write-Host "  Офлайн-сборка Maven:"
Write-Host "    cd back\s3-basic"
Write-Host "    mvn package -o -Dmaven.repo.local=..\..\libs\maven"
Write-Host ""
Write-Host "  Офлайн-сборка NuGet:"
Write-Host "    cd back\s3-basic\desktop"
Write-Host "    dotnet restore --packages ..\..\..\libs\nuget"
Write-Host "    dotnet build --no-restore"
Write-Host ""
