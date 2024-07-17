using Microsoft.Win32;
using System.Diagnostics;

namespace steam_shutdxwn.Source
{
    public class Steam
    {
        private bool _isDevEnv = false;
        private bool _isShuttingDown = false;
        private List<Game>? _downloads = new();
        private List<string> _steamFoldersPath = new();
        private string _steamMainPath = string.Empty;
        private List<FileSystemWatcher> watchlist = new();

        public Steam(bool isDevEnv)
        {
            _isDevEnv = isDevEnv;
        }

        public void Init()
        {
            Console.Title = "steam shutdxwn";

            Banner.Show();

            if (!IsSteamRunning())
            {
                Quit("Steam is not running. Close the app and try again.");
            }

            _steamMainPath = FetchSteamMainPath();
            _steamFoldersPath = FetchAllSteamPaths();
            _downloads = FetchDownloads(_steamFoldersPath);

            SetupFileWatchers();

            if (_downloads == null)
            {
                Console.WriteLine("Downloads not found");
                Console.Write("Would you like to try again? (y): ");

                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    RetryDownloadCheck();
                }

                if (_downloads == null)
                {
                    Quit("\nExiting...");
                }
            }

            Console.Clear();
            Banner.Show();
            Console.WriteLine("steam shutdxwn is now running");

            while (true) Console.ReadKey();
        }

        private static string FetchSteamMainPath()
        {
            RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam\\");

            if (registryKey == null)
            {
                Quit("Steam is not installed");
            }

            string? steamPath = $"{registryKey.GetValue("SteamPath")}/steamapps/";

            if (!Directory.Exists(steamPath) || steamPath == null)
            {
                Quit("\"steamapps\" folder was not found");
            }

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
                {
                    if (Directory.Exists(path))
                    {
                        paths.Add(path);
                    }
                }
            }

            string? match = paths.FirstOrDefault(stringToCheck => stringToCheck.Contains(_steamMainPath));

            if (match == null)
            {
                paths.Add(Path.GetFullPath(_steamMainPath));
            }

            return paths;
        }

        private List<Game>? FetchDownloads(List<string> steamPaths)
        {
            List<Game> games = new();

            foreach (string path in steamPaths)
            {
                List<Game>? game = FetchDownloads(path);

                if (game != null)
                {
                    games.AddRange(game);
                }
            }

            return games.Count > 0 ? games : null;
        }

        private List<Game>? FetchDownloads(string steamPath)
        {
            List<Game> games = new();
            string[] files = Directory.GetFiles(steamPath, "*.acf");
            string[] downloadState = { "4", "68", "1090", "514", "518" };
            /**
             * complete = 4
             * ignore   = 68, 1096
             * not scheduled = 518
            **/

            if (files == null)
            {
                return null;
            }

            foreach (string file in files)
            {
                Acf? acf = Acf.Parse(file);

                if (acf == null)
                {
                    Console.WriteLine($"Something went wrong parsing the file '{file}'. We're skiping it for now.");
                    continue;
                }

                int.TryParse(acf.StateFlags, out int stateFlag);
                if (downloadState.Contains(acf.StateFlags) || stateFlag > 1500)
                {
                    continue;
                }

                Game game = new()
                {
                    Name = acf.name,
                    AppId = acf.appid,
                    StateFlag = acf.StateFlags,
                    FileName = file
                };

                games.Add(game);

                Console.WriteLine(game.ToString());
            }

            return games.Count > 0 ? games : null;
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

                    if (_downloads != null)
                    {
                        break;
                    }

                    Console.WriteLine("Downloads not found. Trying again in 2s.");

                    Thread.Sleep(2000);
                }

                break;

            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }

        private void SetupFileWatchers()
        {
            foreach (string path in _steamFoldersPath)
            {
                FileSystemWatcher watcher = new()
                {
                    Filter = "*.acf",
                    Path = path,
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite,
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = true
                };
                watcher.Deleted += FilesMonitoring;
                watchlist.Add(watcher);
            }
        }

        private static void Quit(string msg = "\nPress any key to exit...")
        {
            Console.WriteLine(msg);
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
