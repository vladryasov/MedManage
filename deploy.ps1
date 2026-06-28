param(
    [switch]$NoBuild
)

try {
    Write-Host "=== Pulling latest changes ===" -ForegroundColor Cyan
    git pull

    if ($NoBuild) {
        Write-Host "=== Skipping build, restarting containers ===" -ForegroundColor Cyan
        docker compose -f docker-compose.base.yml -f docker-compose.frontend.yml up -d
    } else {
        Write-Host "=== Building and starting containers ===" -ForegroundColor Cyan
        docker compose -f docker-compose.base.yml -f docker-compose.frontend.yml up -d --build
    }

    Write-Host "=== Removing unused images ===" -ForegroundColor Cyan
    docker image prune -f

    Write-Host "=== Deploy complete ===" -ForegroundColor Green
} catch {
    Write-Host "=== Deploy failed ===" -ForegroundColor Red
    Write-Error $_
    exit 1
}
