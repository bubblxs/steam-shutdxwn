using Microsoft.Win32;
using System.Text.Json;
using System.Diagnostics;

namespace steam_shutdxwn.Source.Classes
{
    public class Steam
    {
        private bool _isShuttingDown = false;
        private List<Game>? _downloadList = new List<Game>();
        private List<string> _steamFoldersPath = new List<string>();
        private string _steamMainPath = string.Empty;

        public void Init()
        {
            Console.Title = "steam shutdxwn";

            Banner.Show();

            if (!IsRunning())
            {
                Console.WriteLine("Steam is not running. Close the app and try again.");
                Console.ReadKey();
                return;
            }

            _steamMainPath = GetSteamMainPath();
            _steamFoldersPath = GetAllSteamPaths();
            _downloadList = GetDownloads(_steamFoldersPath);

            List<FileSystemWatcher> watchlist = new List<FileSystemWatcher>();

            foreach (string path in _steamFoldersPath)
            {
                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Filter = "*.acf";
                watcher.Path = path;
                watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite;
                watcher.EnableRaisingEvents = true;
                watcher.IncludeSubdirectories = true;
                watcher.Deleted += MonitoringAcfFiles;
                watchlist.Add(watcher);
            }

            if (_downloadList is null)
            {
                Console.WriteLine("Downloads not found");
                Console.Write("Would you like to try again? (y): ");

                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine("\n\nPress 'ESC' to cancel.\n");

                    do
                    {
                        while (!Console.KeyAvailable)
                        {
                            _downloadList = GetDownloads(_steamFoldersPath);

                            if (_downloadList is not null) break;

                            Console.WriteLine("Downloads not found. Trying again in 2s.");

                            Thread.Sleep(2000);
                        }

                        break;

                    } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
                }

                if (_downloadList is null)
                {
                    Console.WriteLine("\nExiting...");
                    Environment.Exit(0);
                }
            }

            Console.Clear();
            Banner.Show();
            Console.WriteLine("steam shutdxwn is now running");

            while (true) Console.ReadKey();
        }

        private static string GetSteamMainPath()
        {
            RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam\\");

            if (registryKey is null)
            {
                Console.WriteLine("Steam is not installed");
                Console.ReadKey();
                Environment.Exit(0);
            }

            string? steamPath = $"{registryKey.GetValue("SteamPath")}/steamapps/";

            if (!Directory.Exists(steamPath) || steamPath is null)
            {
                Console.WriteLine("'steamapps' folder was not found");
                Console.ReadKey();
                Environment.Exit(0);
            }

            return steamPath;
        }

        //Thx to @Alves24
        private static List<string> GetAllSteamPaths()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            List<string> paths = new List<string>();

            foreach (DriveInfo drive in allDrives)
            {
                List<string> possiblePaths = new List<string>
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

            string mainPath = GetSteamMainPath();
            string? match = paths.FirstOrDefault(stringToCheck => stringToCheck.Contains(mainPath));

            if (match is null)
            {
                paths.Add(Path.GetFullPath(mainPath));
            }

            return paths;
        }

        private List<Game>? GetDownloads(List<string> steamPaths)
        {
            List<Game> games = new List<Game>();

            foreach (string path in steamPaths)
            {
                List<Game>? game = GetDownloads(path);

                if (game is not null)
                {
                    games.AddRange(game);
                }
            }

            return games.Count > 0 ? games : null;
        }

        private List<Game>? GetDownloads(string steamPath)
        {
            List<Game> games = new List<Game>();
            string[] files = Directory.GetFiles(steamPath, "*.acf");
            string[] downloadState = { "4", "68", "1090", "514", "518" };
            /**
             * complete = 4
             * ignore   = 68, 1096
             * not scheduled = 518
            **/

            if (files is null) return null;

            foreach (string file in files)
            {
                string? content = Acf.ToJson(file);

                if (content is null)
                {
                    Console.WriteLine($"Something went wrong converting the file '{file}'. We're skiping it for now.");
                    continue;
                }

                Acf? acf = JsonSerializer.Deserialize<Acf>(content);
                Game game = new Game()
                {
                    Name = acf.name,
                    AppId = acf.appid,
                    StateFlag = acf.StateFlags,
                    FileName = file
                };

                int stateFlag;
                int.TryParse(acf.StateFlags, out stateFlag);

                if (downloadState.Contains(acf.StateFlags) || stateFlag > 1500)
                {
                    continue;
                }

                games.Add(game);

                Console.WriteLine(game.ToString());
            }

            return games.Count > 0 ? games : null;
        }

        private bool IsRunning()
        {
            return Process.GetProcessesByName("Steam").Count() > 0;
        }

        private void MonitoringAcfFiles(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(2000);

            _downloadList = GetDownloads(_steamFoldersPath);

            if (_downloadList is null && !_isShuttingDown)
            {
                _isShuttingDown = true;
                Shutdown();
            }
        }

        private void Shutdown()
        {
            Console.Title = "bye bye";
            Process.Start("cmd.exe", "/C shutdown /s");
            Console.Clear();
            Banner.Show();
            Console.WriteLine("shutting down...");
        }
    }
}
