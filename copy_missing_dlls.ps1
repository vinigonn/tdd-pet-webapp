# Define paths
$nugetPath = "C:\Users\vinicius.goncalves\.nuget\packages"
$binPath = "C:\Dev\Backend\src\Application\bin\Debug\net9.0"

# Ensure binPath exists
if (!(Test-Path $binPath)) {
    Write-Host "❌ ERROR: Bin directory does not exist. Check build path." -ForegroundColor Red
    exit
}

# Get all installed packages
$packages = Get-ChildItem -Path $nugetPath | Select-Object -ExpandProperty Name

# Loop through each package and copy .dll files
foreach ($package in $packages) {
    $dllPath = "$nugetPath\$package\*\lib\**\*.dll"

    # Find all DLLs inside the package
    $dllFiles = Get-ChildItem -Path $dllPath -Recurse -ErrorAction SilentlyContinue

    # Copy each DLL to the bin folder
    foreach ($dll in $dllFiles) {
        Copy-Item -Path $dll.FullName -Destination $binPath -Force
        Write-Host "✅ Copied: $($dll.Name) -> bin directory" -ForegroundColor Green
    }
}

Write-Host "🎉 All missing DLLs have been copied successfully!" -ForegroundColor Cyan
