var versionSuffix = Argument("version-suffix", "test");
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

Task("Clean")
    .Does(() =>
{
    CleanDirectories("./artifacts");
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
    var buildSettings = new DotNetCoreBuildSettings
    {
        Configuration = configuration,
        OutputDirectory = "./artifacts/",
        VersionSuffix = versionSuffix
    };

    DotNetCoreBuild("./src/*", buildSettings);
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreTest("./src/*");​ 
});

Task("Pack")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var settings = new DotNetCorePackSettings
    {
        Configurations = new[] { "Debug", "Release" },
        OutputDirectory = "./output/",
        VersionSuffix = versionSuffix
    };

    DotNetCorePack("./src/DotNetAirBrake", settings);
});

Task("Default")
    .IsDependentOn("Test");

RunTarget(target);