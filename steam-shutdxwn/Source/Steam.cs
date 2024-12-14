using Microsoft.Win32;
using System.Diagnostics;

namespace steam_shutdxwn.Source
{
    public class Steam
    {
        private readonly List<FileSystemWatcher> _watchlist = new();
        private readonly List<string> _steamFoldersPath = new();
        private readonly string _steamMainPath = string.Empty;
        private readonly bool _isDevEnv = false;
        private bool _isShuttingDown = false;
        private List<App>? _downloads = new();

        public Steam(bool isDevEnv)
        {
            _isDevEnv = isDevEnv;
            _steamMainPath = FetchSteamMainPath();
            _steamFoldersPath = FetchAllSteamPaths();
            _downloads = FetchDownloads(_steamFoldersPath);
        }

        public void Init()
        {
            Console.Title = "steam shutdxwn";

            Banner.Show();

            if (!IsSteamRunning())
                Quit("Steam is not running. Close the app and try again.");

            if (_downloads == null)
            {
                Console.WriteLine("Downloads not found");
                Console.Write("Would you like to try again? (y): ");

                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    RetryDownloadCheck();
                }
                else
                {
                    Quit();
                }
            }

            foreach (string path in _steamFoldersPath)
            {
                FileSystemWatcher watcher = new()
                {
                    Filter = "*.acf",
                    Path = path,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.Attributes | NotifyFilters.LastAccess | NotifyFilters.Size,
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = true
                };
                watcher.Deleted += FilesMonitoring;
                watcher.Renamed += FilesMonitoring;
                watcher.Created += FilesMonitoring;
                watcher.Changed += FilesMonitoring;

                _watchlist.Add(watcher);
            }

            Console.Clear();
            Banner.Show();
            Console.WriteLine("steam shutdxwn is now running");

            while (true) Console.ReadKey();
        }

        private static string FetchSteamMainPath()
        {
            RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam\\");

            if (registryKey == null) Quit("Steam is not installed");

            string? steamPath = $"{registryKey.GetValue("SteamPath")}/steamapps/";

            if (!Directory.Exists(steamPath) || steamPath == null) Quit("\"steamapps\" folder was not found");

            return steamPath!;
        }

        //Thx to @Alves24
        private List<string> FetchAllSteamPaths()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            List<string> paths = new();

            foreach (DriveInfo drive in allDrives)
            {
                List<string> possiblePaths = new()
                {
                    // Steam would create the first path below in a drive that isnt the main one
                    Path.Combine(drive.RootDirectory.FullName, "SteamLibrary", "Steamapps"),
                    // but i think that this one is possible too..
                    Path.Combine(drive.RootDirectory.FullName, "Steamapps")
                    // maybe there is others possibilities, but those would be the common ones
                };

                foreach (string path in possiblePaths)
                    if (Directory.Exists(path)) paths.Add(path);
            }

            string? match = paths.FirstOrDefault(stringToCheck => stringToCheck.Contains(_steamMainPath));

            if (match == null)
                paths.Add(Path.GetFullPath(_steamMainPath));

            return paths;
        }

        private static List<App>? FetchDownloads(List<string> steamPaths)
        {
            List<App> appList = new();

            foreach (string path in steamPaths)
            {
                List<App>? app = FetchDownloads(path);

                if (app != null) appList.AddRange(app);
            }

            return appList.Count > 0 ? appList : null;
        }

        private static List<App>? FetchDownloads(string steamPath)
        {
            List<App> appList = new();
            string[] files = Directory.GetFiles(steamPath, "*.acf");
            string[] downloadState = { "0", "4", "68", "38", "1090", "514", "518" };

            if (files == null) return null;

            foreach (string file in files)
            {
                Acf? acf = Acf.Parse(file);

                if (acf == null)
                {
                    Console.WriteLine($"Something went wrong parsing the file '{file}'. We're skiping it for now.");
                    continue;
                }

                int.TryParse(acf.StateFlags, out int stateFlag);
                if (downloadState.Contains(acf.StateFlags) || stateFlag > 1500) continue;

                App app = new()
                {
                    Name = acf.name,
                    AppId = acf.appid,
                    StateFlag = acf.StateFlags,
                    FileName = file
                };

                appList.Add(app);

                Console.WriteLine(app.ToString());
            }

            return appList.Count > 0 ? appList : null;
        }

        private static bool IsSteamRunning()
        {
            return Process.GetProcessesByName("Steam").Length > 0;
        }

        private void FilesMonitoring(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(2000);

            _downloads = FetchDownloads(_steamFoldersPath);

            if (_downloads == null && !_isShuttingDown)
            {
                _isShuttingDown = true;
                ShutdownComputer();
            }
        }

        private void ShutdownComputer()
        {
            Console.Title = "bye bye";

            if (!_isDevEnv)
            {
                Process.Start("cmd.exe", "/C shutdown /s");
                Console.Clear();
                Banner.Show();
            }

            Console.WriteLine("Shutting down...");
        }

        private void RetryDownloadCheck()
        {
            Console.WriteLine("\n\nPress 'ESC' to cancel.\n");
            do
            {
                while (!Console.KeyAvailable)
                {
                    _downloads = FetchDownloads(_steamFoldersPath);

                    if (_downloads != null) break;

                    Console.WriteLine("Downloads not found. Trying again in 2s.");
                    Thread.Sleep(2000);
                }

                if (_downloads != null) break;

            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            if (_downloads == null) Quit();
        }

        private static void Quit(string msg = "\nPress any key to exit...")
        {
            Console.WriteLine(msg);
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
