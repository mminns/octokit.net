#r @"tools/FAKE.Core/tools/FakeLib.dll"
#load "tools/SourceLink.Fake/tools/SourceLink.fsx"
open Fake 
open System
open SourceLink

let authors = ["GitHub"]

// project name and description
let projectName = "Octokit"
let projectDescription = "An async-based GitHub API client library for .NET"
let projectSummary = projectDescription // TODO: write a summary

let projectSpiName = "Octokit.Spi"
let projectSpiDescription = "An SPI defining an async-based REST API client library for .NET"
let projectSpiSummary = projectSpiDescription // TODO: write a summary

let projectApiName = "Octokit.Api"
let projectApiDescription = "An API providing an async-based REST API client library for .NET"
let projectApiSummary = projectApiDescription // TODO: write a summary

// directories
let buildDir = "./Octokit/bin"
let buildSpiDir = "./Octokit.Spi/bin"
let buildApiDir = "./Octokit.Api/bin"
let testResultsDir = "./testresults"
let packagingRoot = "./packaging/"
let packagingDir = packagingRoot @@ "octokit"
let packagingSpiDir = packagingRoot @@ "octokit.spi"
let packagingApiDir = packagingRoot @@ "octokit.api"

let releaseNotes = 
    ReadFile "ReleaseNotes.md"
    |> ReleaseNotesHelper.parseReleaseNotes

let buildMode = getBuildParamOrDefault "buildMode" "Release"

MSBuildDefaults <- { 
    MSBuildDefaults with 
        ToolsVersion = Some "14.0"
        Verbosity = Some MSBuildVerbosity.Minimal }

Target "Clean" (fun _ ->
    CleanDirs [buildSpiDir; buildApiDir; buildDir; testResultsDir; packagingRoot; packagingDir]
)

open Fake.AssemblyInfoFile
open Fake.Testing

Target "AssemblyInfo" (fun _ ->
    CreateCSharpAssemblyInfo "./SolutionInfo.cs"
      [ Attribute.Product projectName
        Attribute.Version releaseNotes.AssemblyVersion
        Attribute.FileVersion releaseNotes.AssemblyVersion
        Attribute.ComVisible false ]
)

Target "CheckProjects" (fun _ ->
    !! "./Octokit/Octokit*.csproj"
    |> Fake.MSBuild.ProjectSystem.CompareProjectsTo "./Octokit/Octokit.csproj"
)

Target "FixProjects" (fun _ ->
    !! "./Octokit/Octokit*.csproj"
    |> Fake.MSBuild.ProjectSystem.FixProjectFiles "./Octokit/Octokit.csproj"
)

let setParams defaults = {
    defaults with
        ToolsVersion = Some("14.0")
        Targets = ["Build"]
        Properties =
            [
                "Configuration", buildMode
            ]
    }

Target "BuildApp" (fun _ ->
    build setParams "./Octokit.SpiApi.sln"
        |> DoNothing
)

Target "ConventionTests" (fun _ ->
    !! (sprintf "./Octokit.Tests.Conventions/bin/%s/**/Octokit.Tests.Conventions.dll" buildMode)
    |> xUnit2 (fun p -> 
            {p with
                HtmlOutputPath = Some (testResultsDir @@ "xunit.html") })
)

Target "UnitTests" (fun _ ->
    !! (sprintf "./Octokit*Tests/bin/%s/**/Octokit*Tests*.dll" buildMode)
    |> xUnit2 (fun p -> 
            {p with
                HtmlOutputPath = Some (testResultsDir @@ "xunit.html") })
)

Target "SourceLink" (fun _ ->
    [   "Octokit/Octokit.csproj"; "Octokit.Api/Octokit.Api.csproj"; "Octokit.Spi/Octokit.Spi.csproj"]
    |> Seq.iter (fun pf ->
        let proj = VsProj.LoadRelease pf
        let url = "https://raw.githubusercontent.com/mminns/octokit.net/{0}/%var2%"
        SourceLink.Index proj.Compiles proj.OutputFilePdb __SOURCE_DIRECTORY__ url
    )
)

Target "CreateOctokitPackage" (fun _ ->
    let net45Dir = packagingDir @@ "lib/net45/"
    CleanDirs [net45Dir; ]

    CopyFile net45Dir (buildDir @@ "Release/Net45/Octokit.dll")
    CopyFile net45Dir (buildDir @@ "Release/Net45/Octokit.XML")
    CopyFile net45Dir (buildDir @@ "Release/Net45/Octokit.pdb")
    CopyFiles packagingDir ["LICENSE.txt"; "README.md"; "ReleaseNotes.md"]

    NuGet (fun p -> 
        {p with
            Authors = authors
            Project = projectName
            Description = projectDescription
            OutputPath = packagingRoot
            Summary = projectSummary
            WorkingDir = packagingDir
            Version = releaseNotes.AssemblyVersion + "-alpha"
            ReleaseNotes = toLines releaseNotes.Notes
            AccessKey = getBuildParamOrDefault "nugetkey" ""
            Publish = hasBuildParam "nugetkey" }) "octokit.nuspec"
)

Target "CreateOctokitSpiPackage" (fun _ ->
    let net45Dir = packagingSpiDir @@ "lib/net45/"
    CleanDirs [net45Dir; ]

    CopyFile net45Dir (buildSpiDir @@ "Release/Net45/Octokit.Spi.dll")
    CopyFile net45Dir (buildSpiDir @@ "Release/Net45/Octokit.Spi.XML")
    CopyFile net45Dir (buildSpiDir @@ "Release/Net45/Octokit.Spi.pdb")
    CopyFiles packagingDir ["LICENSE.txt"; "README.md"; "ReleaseNotes.md"]

    NuGet (fun p -> 
        {p with
            Authors = authors
            Project = projectSpiName
            Description = projectSpiDescription
            OutputPath = packagingRoot
            Summary = projectSpiSummary
            WorkingDir = packagingSpiDir
            Version = releaseNotes.AssemblyVersion + "-alpha"
            ReleaseNotes = toLines releaseNotes.Notes
            AccessKey = getBuildParamOrDefault "nugetkey" ""
            Publish = hasBuildParam "nugetkey" }) "octokit.spi.nuspec"
)

Target "CreateOctokitApiPackage" (fun _ ->
    let net45Dir = packagingApiDir @@ "lib/net45/"
    CleanDirs [net45Dir; ]

    CopyFile net45Dir (buildApiDir @@ "Release/Net45/Octokit.Api.dll")
    CopyFile net45Dir (buildApiDir @@ "Release/Net45/Octokit.Api.XML")
    CopyFile net45Dir (buildApiDir @@ "Release/Net45/Octokit.Api.pdb")
    CopyFiles packagingDir ["LICENSE.txt"; "README.md"; "ReleaseNotes.md"]

    NuGet (fun p -> 
        {p with
            Authors = authors
            Project = projectApiName
            Description = projectApiDescription
            OutputPath = packagingRoot
            Summary = projectApiSummary
            WorkingDir = packagingApiDir
            Version = releaseNotes.AssemblyVersion + "-alpha"
            ReleaseNotes = toLines releaseNotes.Notes
            AccessKey = getBuildParamOrDefault "nugetkey" ""
            Publish = hasBuildParam "nugetkey" }) "octokit.api.nuspec"
)

Target "Default" DoNothing

Target "CreatePackages" DoNothing

"Clean"
   ==> "AssemblyInfo"
   ==> "CheckProjects"
   ==> "BuildApp"

"Clean"
   ==> "AssemblyInfo"
   ==> "CheckProjects"
   ==> "BuildXSApp"

"UnitTests"
   ==> "Default"

"ConventionTests"
   ==> "Default"

"SourceLink"
   ==> "CreatePackages"
   
"CreateOctokitPackage"
   ==> "CreatePackages"

"CreateOctokitSpiPackage"
   ==> "CreatePackages"

"CreateOctokitApiPackage"
   ==> "CreatePackages"


RunTargetOrDefault "Default"
