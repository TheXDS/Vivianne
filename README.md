# Ganymede

[![CodeFactor](https://www.codefactor.io/repository/github/thexds/ganymede/badge)](https://www.codefactor.io/repository/github/thexds/ganymede)
[![Build Ganymede](https://github.com/TheXDS/Ganymede/actions/workflows/build.yml/badge.svg)](https://github.com/TheXDS/Ganymede/actions/workflows/build.yml)
[![Publish Ganymede](https://github.com/TheXDS/Ganymede/actions/workflows/publish.yml/badge.svg)](https://github.com/TheXDS/Ganymede/actions/workflows/publish.yml)
[![Issues](https://img.shields.io/github/issues/TheXDS/Ganymede)](https://github.com/TheXDS/Ganymede/issues)
[![MIT](https://img.shields.io/github/license/TheXDS/Ganymede)](https://mit-license.org/)

Ganymede is an app-template using common MVVM-frameworks. It includes helpers for navigation, base ViewModel classes, custom dialog services and common entry points. You just need to provide your actual ViewModels and views, not having to worry about implementing auxiliary UI services for them.

## Releases
Release | Link
--- | ---
Latest public release: | [![Latest stable NuGet package](https://buildstats.info/nuget/TheXDS.Ganymede)](https://www.nuget.org/packages/TheXDS.Ganymede/)  
Latest development release: | [![Latest development NuGet package](https://buildstats.info/nuget/TheXDS.Ganymede?includePreReleases=true)](https://www.nuget.org/packages/TheXDS.Ganymede/)

**Package Manager**  
```sh
Install-Package TheXDS.Ganymede
```

**.NET CLI**  
```sh
dotnet add package TheXDS.Ganymede
```

**Paket CLI**  
```sh
paket add TheXDS.Ganymede
```

**Package reference**  
```xml
<PackageReference Include="TheXDS.Ganymede" Version="1.0.0" />
```

**C# interactive window (CSI)**  
```
#r "nuget: TheXDS.Ganymede, 1.0.0"
```

## Building
Ganymede can be built on any platform or CI environment supported by dotnet.

### Prerequisites
- [.Net SDK 6.0](https://dotnet.microsoft.com/).

### Build Ganymede
```sh
dotnet build ./src/Ganymede.sln
```
The resulting binaries will be in the `./Build/bin` directory.

## Contribute
[![Buy Me A Coffee](https://cdn.buymeacoffee.com/buttons/default-orange.png)](https://www.buymeacoffee.com/xdsxpsivx)

If `Ganymede` is useful to you, or if you're interested in donating to sponsor the project, feel free to to a donation via [PayPal](https://paypal.me/thexds), [BuyMeACoffee](https://www.buymeacoffee.com/xdsxpsivx) or just contact me directly.

Sadly, I cannot offer other means of sending donations as of right now due to my country (Honduras) not being supported by almost any platform.