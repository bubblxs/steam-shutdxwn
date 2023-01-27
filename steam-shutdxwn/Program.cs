using Microsoft.Win32;
using steam_shutdxwn.Source;
using steam_shutdxwn.Source.Banner;
using steam_shutdxwn.Source.Classes;

namespace steam_shutdxwn
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "steam-shutdxwn";

            Steam steam = new Steam();
            RegistryKey registerPath = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam\\");
            FileSystemWatcher watcher = new FileSystemWatcher();
            
            string steamPath = $"{registerPath.GetValue("SteamPath")}/steamapps/";

            watcher.Filter = "*.acf";
            watcher.Path = steamPath;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite;
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;
            watcher.Deleted += steam.FilesWatcher;

            steam.SetSteamPath(steamPath);

            Banner.ShowBanner();

            if (registerPath == null)
            {
                Console.WriteLine("Steam not installed!");
                Console.ReadKey();
                return;
            }

            if (!Directory.Exists(steamPath))
            {
                Console.WriteLine("Steam not installed! ('steamapps' folder not found)");
                Console.ReadKey();
                return;
            }

            if (!steam.IsSteamRunning())
            {
                Console.WriteLine("Steam is not runnnig!. Open Steam and try it again");
                Console.ReadKey();
                return;
            }

            List<GameInfo> downloadsQueued = steam.GetDownloadQueue(steamPath);

            if (downloadsQueued == null)
            {
                Console.WriteLine("No downloads were found");
                Console.Write("Would you like to try again? [y/n]: ");

                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine("\n\nPress 'ESC' to cancel.\n");

                    do
                    {
                        while (!Console.KeyAvailable)
                        {
                            Thread.Sleep(2000);

                            downloadsQueued = steam.GetDownloadQueue(steamPath);

                            if (downloadsQueued == null)
                            {
                                Console.WriteLine("Downloads in queue were not found. Trying again in 2s.");
                            }
                            else
                            {
                                break;
                            }
                        }

                        break;

                    } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
                }
                else
                {
                    return;
                }
            }

            Console.WriteLine($"\nDownload(s) found.");
            Console.WriteLine($"\nSteam Shutdown is now running.Your computer will shutdown when all games have been downloaded.\n");

            while (true) { Console.ReadKey(); };
        }
    }
}