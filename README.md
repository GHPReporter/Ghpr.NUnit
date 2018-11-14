<p align="center">
  <a href="https://ghpreporter.github.io/"><img src="https://github.com/GHPReporter/GHPReporter.github.io/blob/master/img/logo-small.png?raw=true" alt="Project icon"></a>
  <br><br>
  <b>Some Links:</b><br>
  <a href="https://github.com/GHPReporter/Ghpr.Core">Core</a> |
  <a href="https://github.com/GHPReporter/Ghpr.MSTest">MSTest</a> |
  <a href="https://github.com/GHPReporter/Ghpr.MSTestV2">MSTestV2</a> |
  <a href="https://github.com/GHPReporter/Ghpr.NUnit">NUnit</a> |
  <a href="https://github.com/GHPReporter/Ghpr.SpecFlow">SpecFlow</a> |
  <a href="https://github.com/GHPReporter/Ghpr.Console">Console</a> |
  <a href="https://github.com/GHPReporter/GHPReporter.github.io/">Site Repo</a>
</p>

[![Build status](https://ci.appveyor.com/api/projects/status/edl1eag5luk5v4xs?svg=true)](https://ci.appveyor.com/project/elv1s42/ghpr-nunit)
[![NuGet Version](https://img.shields.io/nuget/v/Ghpr.NUnit.svg)](https://www.nuget.org/packages/Ghpr.NUnit)
[![CodeFactor](https://www.codefactor.io/repository/github/ghpreporter/ghpr.nunit/badge)](https://www.codefactor.io/repository/github/ghpreporter/ghpr.nunit)

# Ghpr.NUnit

## Usage:
Please use this project with targetFramework v4.5.2

 - Install [NUnit 3 console](https://github.com/nunit/nunit-console/releases) latest release
 - Download the latest version of Ghpr.NUnit (using NuGet)
 - Add relative path to `Ghpr.NUnit.dll` from build folder of your tests (example: _addins/../../../Users/Evgeniy_Kosjakov/Documents/GitHub/Ghpr.NUnit.Examples/build/Ghpr.NUnit.dll_) into `nunit.engine.addins` file (file located in *[nunit_console_location]/nunit-console/addins*). Also make sure that your build contains such libraries and files  as `Ghpr.Core.dll`, `Ghpr.NUnit.dll`, `Ghpr.LocalFileSystem.dll`, `Ghpr.NUnit.Settings.json` and `Newtonsoft.Json.dll`:
 ![image](https://user-images.githubusercontent.com/6689884/42174465-21b053a6-7e2b-11e8-8bae-a5cced08812f.png)
 - Run your tests via NUnit Console
 
## How to publish the report in Jenkins

Please, read [this](https://github.com/GHPReporter/Ghpr.Core#how-to-publish-the-report-in-jenkins) `'How to publish the report in Jenkins'` instruction.

## How to work with screenshots

If you want to add screenshots to your report, you need to implement your own method of taking screenshot as `byte[]`. This is needed because there is no way to take screenshot which will work on any testing framework or CI tool (such as Jenkins or TeamCity). If you are using WebDriver, you can take screenshot using WebDriver. Also NUnit attachments are supported. 

```csharp
[Test]
public void TestMethod()
{
    var bytes = TakeScreenshot(); //your implementation
    //all you need to do is to pass byte[] to ScreenHelper:
    ScreenHelper.SaveScreenshot(bytes);
}
```
If you want to be able to take screenshots for failed tests, you can take a look at this approach:

```csharp
[TearDown]
public void TakeScreenIfFailed()
{
    var res = TestContext.CurrentContext.Result.Outcome;
    if (res.Equals(ResultState.Failure) || res.Equals(ResultState.Error))
    {
        ScreenHelper.SaveScreenshot(TakeScreenshot());
    }
}
```

## Demo Report

You can view [Demo report](http://ghpreporter.github.io/report/) on our [site](http://ghpreporter.github.io/)

## View report locally

Please read [Core instructions](https://github.com/GHPReporter/Ghpr.Core#view-report-locally) or [this comment](https://github.com/GHPReporter/Ghpr.NUnit/issues/16#issuecomment-291445978) about opening report in Chrome

## Contributing

Anyone contributing is welcome. Write [issues](https://github.com/GHPReporter/Ghpr.NUnit/issues), create [pull requests](https://github.com/GHPReporter/Ghpr.NUnit/pulls).

# Release notes

You can find it [here](https://github.com/GHPReporter/Ghpr.Core/blob/master/RELEASE_NOTES.md) for all packages.
