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

$xml = [xml](Get-Content $projectFile)
$xml.Project.PropertyGroup.Version = "$version"
$xml.Save($projectFile)