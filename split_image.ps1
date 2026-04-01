Add-Type -AssemblyName System.Drawing

$inputPath = "c:\Users\hanys\.gemini\antigravity\scratch\CarWashStation\wwwroot\images\services.png"
$outputDir = "c:\Users\hanys\.gemini\antigravity\scratch\CarWashStation\wwwroot\images"

$img = [System.Drawing.Image]::FromFile($inputPath)

Write-Host "Original Image: $($img.Width)x$($img.Height)"

# Determine if the image is a vertical or horizontal list
if ($img.Height -gt $img.Width) {
    # Vertical list
    $sliceHeight = [math]::Floor($img.Height / 11)
    $sliceWidth = $img.Width
    Write-Host "Vertical layout detected. Slice size: $($sliceWidth)x$($sliceHeight)"
    
    for ($i = 0; $i -lt 11; $i++) {
        $rect = New-Object System.Drawing.Rectangle(0, ($i * $sliceHeight), $sliceWidth, $sliceHeight)
        $bmp = New-Object System.Drawing.Bitmap($rect.Width, $rect.Height)
        $g = [System.Drawing.Graphics]::FromImage($bmp)
        $g.DrawImage($img, (New-Object System.Drawing.Rectangle(0, 0, $bmp.Width, $bmp.Height)), $rect, [System.Drawing.GraphicsUnit]::Pixel)
        $g.Dispose()
        $bmp.Save("$outputDir\service-$($i+1).png", [System.Drawing.Imaging.ImageFormat]::Png)
        $bmp.Dispose()
    }
} else {
    # Horizontal list
    $sliceWidth = [math]::Floor($img.Width / 11)
    $sliceHeight = $img.Height
    Write-Host "Horizontal layout detected. Slice size: $($sliceWidth)x$($sliceHeight)"
    
    for ($i = 0; $i -lt 11; $i++) {
        $rect = New-Object System.Drawing.Rectangle(($i * $sliceWidth), 0, $sliceWidth, $sliceHeight)
        $bmp = New-Object System.Drawing.Bitmap($rect.Width, $rect.Height)
        $g = [System.Drawing.Graphics]::FromImage($bmp)
        $g.DrawImage($img, (New-Object System.Drawing.Rectangle(0, 0, $bmp.Width, $bmp.Height)), $rect, [System.Drawing.GraphicsUnit]::Pixel)
        $g.Dispose()
        $bmp.Save("$outputDir\service-$($i+1).png", [System.Drawing.Imaging.ImageFormat]::Png)
        $bmp.Dispose()
    }
}

$img.Dispose()
Write-Host "Successfully sliced into 11 images!"
