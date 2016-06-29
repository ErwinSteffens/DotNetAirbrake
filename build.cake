#addin "Cake.Powershell"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var versionSuffix = "nuget-test";

var isRunningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;

Task("Clean")
    .Does(() =>
{
    CleanDirectories("./src/**/bin");
    CleanDirectories("./src/**/obj");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore("./src/DotNetAirbrake");    
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetCoreBuild("./src/DotNetAirbrake", new DotNetCoreBuildSettings
    {
        Configuration = configuration,
        VersionSuffix = versionSuffix
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    // TODO
});

Task("Version")
    .Does(() =>
{
    if (!isRunningOnAppVeyor)
    {
        throw new InvalidOperationException("Can only set version when running on AppVeyor");
    }

    var version = AppVeyor.Environment.Build.Version;

    StartPowershellFile("./update-version.ps1", args =>
    {
        args.Append("projectFile", "./src/DotNetAirbrake/project.json")
            .Append("version", version);
    });
});

Task("Pack")
    .IsDependentOn("Version")
    .IsDependentOn("Build")
    .Does(() =>
{
    var settings = new DotNetCorePackSettings
    {
        Configuration = "Release",
        OutputDirectory = "./output/",
        VersionSuffix = versionSuffix
    };

    DotNetCorePack("./src/DotNetAirbrake", settings);
});

Task("Default")
    .IsDependentOn("Test");

RunTarget(target);