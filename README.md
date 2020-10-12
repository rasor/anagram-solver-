# AnagramSolver

## Problem:

* Find 3 secret prases from an anagram

## Important hints

* An anagram of the phrase is: "poultry outwits ants"
* There are three levels of difficulty to try your skills with
* The MD5 hash of the easiest secret phrase is "e4820b45d2277f3844eac66c903e84be"
* The MD5 hash of the more difficult secret phrase is "23170acc097c24edb98fc5488ab033fe"
* The MD5 hash of the hard secret phrase is "665e5bcb0c20062fe8abaaf4628bb154"
* A list of english words is supplied

# Build and Run

Install 
* VS Code
* [C# for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
* [.NET Core 2.1 SDK (v2.1.810) - Windows x64 Installer](https://dotnet.microsoft.com/download/dotnet-core/thank-you/sdk-2.1.810-windows-x64-installer)

To build and run the sample, change to the `./anagramsolver/src/anagramsolver` directory and
type the following two commands:

`dotnet restore`
`dotnet run`

`dotnet restore` restores the dependencies for this sample.
`dotnet run` builds the sample and runs the output assembly.

To run the tests, change to the `tests/anagramsolverTests` directory and
type the following three commands:

`dotnet restore`
`dotnet build`
`dotnet test`

`dotnet test` runs all the configure tests 

Note that you must run `dotnet restore` in the `src/anagramsolver` directory before you can run
the tests. `dotnet build` will follow the dependency and build both the library and unit
tests projects, but it will not restore NuGet packages.

## Structure

Organized according to <https://docs.microsoft.com/en-us/dotnet/core/tutorials/testing-with-cli>
