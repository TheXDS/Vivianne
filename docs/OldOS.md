# Vivianne notes on older operating systems
This article describes some important caveats when trying to run Vivianne on older/unsupported operating systems. None of the operating systems described here are officially supported, but I took the time to test and verify the requirements to get Vivianne to an usable state.

The earliest operating system that Vivianne is technically capable of running in (and, probably not reliably) is _Windows 7_. Vista and earlier just don't support the required runtimes.

> IMPORTANT: No Pre-release versions of Windows earlier than Windows 10 are or will ever be supported, so don't get any ideas of trying to run Vivianne in Windows Developer Preview 8102. But if you do, you're on your own.

## Windows 10
> WinSDK 19041 is the minimum officially supported target SDK version for Vivianne.

If you keep your Windows 10 installation up to date, Vivianne should run without any issues. However, just like Windows 11 and up, you'll need to install either .NET 8 or .NET 9, depending on which version of Vivianne you got.

The system will prompt you automatically to install the required .NET runtime if it is not already installed.

> Tested with Windows 10 21H1 (build 19041). Any newer builds will work as well.

## Windows 8.1
To enhance your odds at getting Vivianne running in Windows 8.1, please keep your OS up to date. If you don't, it's very likely that nothing will work, not even installing Microsoft's own redistributable packages.

You'll need to install the following:
- [.NET 8.0 Runtime](https://builds.dotnet.microsoft.com/dotnet/Runtime/8.0.17/dotnet-runtime-8.0.17-win-x64.exe)
- [Microsoft Visual C++ 2015 Redistributable 14.0.24215 (also known as KB2999226)](https://www.microsoft.com/en-gb/download/details.aspx?id=48145)

> **IMPORTANT**: If you find issues installing KB2999226, you may need to either troubleshoot the installation yourself, or extract all the required `.dll` files from the `.msu` package and copy them directly to the Vivianne folder. I'm not your personal tech support, so don't expect me to help you with this. I don't know why this happens on a fresh isntall of Windows 8.1, but it does.

> Tested with Windows 8.1 6.3.9600 64-bit.

## Windows 8
As with Windows 8.1, You should keep your system up to date.

You'll need to install the following:
- [.NET 8.0 Runtime](https://builds.dotnet.microsoft.com/dotnet/Runtime/8.0.17/dotnet-runtime-8.0.17-win-x64.exe)
- [Microsoft Visual C++ 2015 Redistributable 14.0.24215 (also known as KB2999226)](https://www.microsoft.com/en-gb/download/details.aspx?id=48145)

> **IMPORTANT**: If you find issues installing KB2999226, you may need to either troubleshoot the installation yourself, or extract all the required `.dll` files from the `.msu` package and copy them directly to the Vivianne folder. I'm not your personal tech support, so don't expect me to help you with this.

> Tested with Windows 8 6.2 64-bit.

## Windows 7
This one is more complicated. While there's reports of people launching Vivianne in it, preparing the system to properly execute is more involved.

- [Follow this guide](https://learn.microsoft.com/en-us/dotnet/core/install/windows#windows-7--81--server-2012) in order to get the system to a state where it can run .NET 8 apps without crashing.
- [Microsoft Visual C++ 2015 Redistributable 14.0.24215 (also known as KB2999226)](https://www.microsoft.com/en-gb/download/details.aspx?id=48145)
- [Windows 7 KB306858](https://www.microsoft.com/download/details.aspx?id=47442)
- [.NET 8.0 Runtime](https://builds.dotnet.microsoft.com/dotnet/Runtime/8.0.17/dotnet-runtime-8.0.17-win-x64.exe)

### Tests without Aero:
Yes, the app works well, and themes seem properly supported.

