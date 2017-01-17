<p align="center">
  <a href="https://ghpreporter.github.io/"><img src="https://github.com/GHPReporter/GHPReporter.github.io/blob/master/img/logo-small.png?raw=true" alt="Project icon"></a>
  <br><br>
  <b>Some Links:</b><br>
  <a href="https://github.com/GHPReporter/Ghpr.Core">Core</a> |
  <a href="https://github.com/GHPReporter/Ghpr.MSTest">MSTest</a> |
  <a href="https://github.com/GHPReporter/Ghpr.NUnit">NUnit</a> |
  <a href="https://github.com/GHPReporter/Ghpr.SpecFlow">SpecFlow</a> |
  <a href="https://github.com/GHPReporter/Ghpr.Console">Console</a> |
  <a href="https://github.com/GHPReporter/GHPReporter.github.io/">Site Repo</a>
</p>

[![Language](http://gh-toprated.info/Badges/LanguageBadge?user=GHPReporter&repo=Ghpr.NUnit&theme=light&fontWeight=bold)](https://github.com/GHPReporter/Ghpr.NUnit)
[![Build status](https://ci.appveyor.com/api/projects/status/edl1eag5luk5v4xs?svg=true)](https://ci.appveyor.com/project/elv1s42/ghpr-nunit)
[![NuGet Version](https://img.shields.io/nuget/v/Ghpr.NUnit.svg)](https://www.nuget.org/packages/Ghpr.NUnit)

# Ghpr.NUnit

##Usage:
Please use this project with targetFramework v4.5.2

 - Install [NUnit 3 console](https://github.com/nunit/nunit-console/releases) latest release
 - Download the latest version of Ghpr.NUnit (using NuGet)
 - Put Ghpr.Core.dll, Ghpr.NUnit.dll and Newtonsoft.Json.dll to the following folder: 
[nunit_console_location]/nunit-console/addins
 - Add path **addins/Ghpr.NUnit.dll** into nunit.engine.addins file
 - Add config data to nunit3-console.exe.config:
 
 ``` 
 <appSettings>
    <add key="OutputPath" value="C:\_GHPReportOutput" />
    <add key="TakeScreenshotAfterFail" value="True" />
    <add key="Sprint" value="" />
    <add key="RunName" value="" />
    <add key="RunGuid" value="" />
    <add key="RealTimeGeneration" value="True" />
  </appSettings> 
 ``` 
 - Run your tests via NUnit Console

## How to publish the report in Jenkins

Please, read [this](https://github.com/GHPReporter/Ghpr.Core#how-to-publish-the-report-in-jenkins) `'How to publish the report in Jenkins'` instruction.

## How to work with screenshots

If you want to add screenshots to your report, you need to implement your own method of taking screenshot as `byte[]`. This is needed because there is no way to take screenshot which will work on any testing framework or CI tool (such as Jenkins or TeamCity). If you are using WebDriver, you can take screenshot using WebDriver.

```csharp
[Test]
public void TestMethod()
{
    var bytes = TakeScreenshot(); //your implementation
    //all you need to do is to pass byte[] to ScreenHelper:
    ScreenHelper.SaveScreenshot(bytes);
}
```
 
## Demo Report

You can view [Demo report](http://ghpreporter.github.io/report/) on our [site](http://ghpreporter.github.io/)

## View report locally

####Firefox

 - Go to `about:config`
 - Find `security.fileuri.strict_origin_policy` parameter
 - Set it to `false`

## Contributing

Anyone contributing is welcome. Write [issues](https://github.com/GHPReporter/Ghpr.NUnit/issues), create [pull requests](https://github.com/GHPReporter/Ghpr.NUnit/pulls).
