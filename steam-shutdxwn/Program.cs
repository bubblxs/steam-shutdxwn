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
            Console.Title = "steam shutdxwn";

            Banner.ShowBanner();

            Steam steam = new Steam();
            RegistryKey registerPath = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam\\");
            FileSystemWatcher watcher = new FileSystemWatcher();
            
            string steamPath = $"{registerPath.GetValue("SteamPath")}/steamapps/";

            if (!Directory.Exists(steamPath))
            {
                Console.WriteLine("Steam is not installed!.");
                Console.ReadKey();
                return;
            }

            watcher.Filter = "*.acf";
            watcher.Path = steamPath;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite;
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;
            watcher.Deleted += steam.FilesWatcher;

            steam.SetSteamPath(steamPath);

            if (!steam.IsSteamRunning())
            {
                Console.WriteLine("Steam is not runnnig!. Open Steam and then try it again.");
                Console.ReadKey();
                return;
            }

            List<GameInfo> downloadsQueued = steam.GetDownloadQueue(steamPath);

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

                            downloadsQueued = steam.GetDownloadQueue(steamPath);

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

            Console.WriteLine($"\nDownload(s) found.");
            Console.WriteLine($"\nSteam Shutdown is now running.Your computer will shutdown when all games have been downloaded.\n");

            while (true) { Console.ReadKey(); };
        }
    }
}