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

            RegistryKey registerPath = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam\\");

            string steamPath = $"{registerPath.GetValue("SteamPath")}/steamapps/";
            bool isSteamRunning = Steam.IsSteamRunning();
            bool steamPathExists = Directory.Exists(steamPath);

            Task downloadMonitoring = new Task(() => Steam.FileWatcher(steamPath));

            Banner.ShowBanner();

            if (registerPath == null)
            {
                Console.WriteLine("Steam not installed!");
                Console.ReadKey();
                return;
            }

            if (!steamPathExists)
            {
                Console.WriteLine("Steam not installed! ('steamapps' folder not found)");
                Console.ReadKey();
                return;
            }

            if (!isSteamRunning)
            {
                Console.WriteLine("Steam is not runnnig!. Open Steam and try it again");
                Console.ReadKey();
                return;
            }

            List<GameInfo> downloadsQueued = Steam.GetDownloadQueue(steamPath);

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

                            downloadsQueued = Steam.GetDownloadQueue(steamPath);

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

            downloadMonitoring.Start();

            while (true) { }
        }
    }
}


                /*
                    ls: Download list
                    whoami: Steam user account --> flag '--open' opens the profile in the browser
                    cake: saves portal song txt file on desktop
                    close: closes the application
                    clear: clear screen 
                    help: show all commands avaliable
                 */