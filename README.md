<h2 align="center"> steam shutdxwn </h2>

> shutdown your computer once all your Steam downloads have finished.

### requirements
  - .NET 6 Runtime;
  - Steam MUST be installed on your system;
  - Windows 10 or superior.

### installation
You can either build the solution or download the executable.

Download
  - [Windows x64](https://github.com/bubblxs/steam-shutdxwn/releases/latest)

### usage
  - Make sure Steam is installed and running;
  - Run the executable;
  - Once all downloads are finished, the application will trigger a system shutdown, so make sure to save and close any work before running the application.

### configuration (optional)
You can also define a custom banner for the application by creating the **shutdxwn** folder with a **banner.txt** in the **Documents** folder.
```
  C:\Users\%username%\Documents\shutdxwn\banner.txt
```
### when working on it

If you decide to work on it make sure to pass **--dev** as first argument.

e.g.: `` dotnet run --dev ``