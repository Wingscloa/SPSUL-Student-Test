# Fix encoding to UTF-8 with BOM for all JS and Razor files
# This fixes Czech diacritics (ìšèøžýáíéúù) display issues

$files = Get-ChildItem -Path "." -Include *.js,*.cshtml,*.css -Recurse

foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName -Raw
    
    # Save with UTF-8 BOM
    $utf8BOM = New-Object System.Text.UTF8Encoding($true)
    [System.IO.File]::WriteAllText($file.FullName, $content, $utf8BOM)
    
    Write-Host "Fixed: $($file.FullName)" -ForegroundColor Green
}

Write-Host "`nDone! All files converted to UTF-8 with BOM." -ForegroundColor Cyan
Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
