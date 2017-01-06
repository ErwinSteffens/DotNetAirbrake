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
    DotNetCoreRestore("./src/DotNetAirbrake.AspNetCore");
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings
    {
        Configuration = configuration,
        VersionSuffix = versionSuffix
    };

    DotNetCoreBuild("./src/DotNetAirbrake", settings);
    DotNetCoreBuild("./src/DotNetAirbrake.AspNetCore", settings);
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
        args.Append("projectFile", "./src/DotNetAirbrake/project.json").Append("version", version);
    });
    StartPowershellFile("./update-version.ps1", args =>
    {
        args.Append("projectFile", "./src/DotNetAirbrake.AspNetCore/project.json").Append("version", version);
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
        OutputDirectory = "./output/"
    };

    DotNetCorePack("./src/DotNetAirbrake", settings);
    DotNetCorePack("./src/DotNetAirbrake.AspNetCore", settings);
});

Task("Default")
    .IsDependentOn("Test");

RunTarget(target);