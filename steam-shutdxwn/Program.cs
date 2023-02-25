using Microsoft.Win32;
using steam_shutdxwn.Source;
using steam_shutdxwn.Source.Banner;
using steam_shutdxwn.Source.Classes;
using steam_shutdxwn.Source.Helpers;

namespace steam_shutdxwn
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "steam shutdxwn";

            Banner.ShowBanner();

            Steam steam = new Steam();
            List<FileSystemWatcher> watcherList = new List<FileSystemWatcher>();
            PathSearcher pathSearcher = new PathSearcher();
            List<string> steamappPaths = new List<string>();

            var steamPath = pathSearcher.getMainSteamPath();
            if (!Directory.Exists(steamPath))
            {
                Console.WriteLine("Steam is not installed!.");
                Console.ReadKey();
                return;
            }

            steam.SetSteamPath(steamappPaths);
            steamappPaths.AddRange(pathSearcher.getAllSteamappPaths());

            foreach (var path in steamappPaths)
            {
                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Filter = "*.acf";
                watcher.Path = path;
                watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite;
                watcher.EnableRaisingEvents = true;
                watcher.IncludeSubdirectories = true;
                watcher.Deleted += steam.FilesWatcher;
                watcherList.Add(watcher);
            }


            if (!steam.IsSteamRunning())
            {
                Console.WriteLine("Steam is not runnnig!. Open Steam and then try again.");
                Console.ReadKey();
                return;
            }

            List<GameInfo> downloadsQueued = steam.GetDownloadQueue(steamappPaths);

            if (downloadsQueued == null)
            {
                Console.WriteLine("Downloads not found.");
                Console.Write("Would you like to try again? (y): ");

                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine("\n\nPress 'ESC' to cancel.\n");

                    do
                    {
                        while (!Console.KeyAvailable)
                        {
                            Thread.Sleep(2000);

                            downloadsQueued = steam.GetDownloadQueue(steamappPaths);

                            if (downloadsQueued != null)
                            {
                                break;
                            }

                            Console.WriteLine("Downloads in queue were not found. Trying again in 2s.");
                        }

                        break;

                    } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
                }
                else
                {
                    return;
                }
            }
            
            if (downloadsQueued == null)
            {
                Console.WriteLine("\n...");
                return;
            }

            Console.WriteLine($"\nDownload(s) found.");
            Console.WriteLine($"\nSteam Shutdown is now running. Your computer will shutdown when all games have been downloaded.\n");

            while (true) { Console.ReadKey(); };
        }
    }
}