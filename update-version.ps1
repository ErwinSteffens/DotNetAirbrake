param(
    [Parameter(Mandatory=$true)]
    [string]
    $projectFile,

    [Parameter(Mandatory=$true)]
    [string]
    $version    
)

trap 
{ 
    write-output $_ 
    exit 1 
} 

if (-Not (Test-Path $projectFile)) {
    throw "Could not file project file '$projectFile'"
}

$object = Get-Content $projectFile | ConvertFrom-Json
$object.version = $version
$object | ConvertTo-Json -Depth 20 | Set-Content $projectFile