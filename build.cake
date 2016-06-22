var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var versionSuffix = "nuget-test";

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
    DotNetCoreRestore();    
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

Task("Pack")
    .IsDependentOn("Restore")
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