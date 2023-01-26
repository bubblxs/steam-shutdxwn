using Newtonsoft.Json;
using steam_shutdxwn.Source.Classes;
using System.Diagnostics;
using steam_shutdxwn.Source.Helpers;

namespace steam_shutdxwn.Source
{
    public static class Steam
    {
        public static bool isAlreadyCalled = false;
        public static string steamPath;
        public static List<GameInfo> downloadQueue;

        public static void FileWatcher(string sp)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(sp);

            steamPath = sp;
            downloadQueue = GetDownloadQueue(steamPath);

            watcher.Filter = "*.acf";
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite;
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;
            watcher.Deleted += FileMonitoring;

            while (true) { }
        }

        public static bool IsSteamRunning()
        {
           return Process.GetProcessesByName("Steam").Length > 0;
        }

        private static void FileMonitoring(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(2000);

            downloadQueue = GetDownloadQueue(steamPath);

            if (downloadQueue == null && !isAlreadyCalled)
            {
                isAlreadyCalled = true;
                Shutdown();
            }
        }

        public static List<GameInfo> GetDownloadQueue(string steamPath)
        {
            List<GameInfo> downloadQueued = new List<GameInfo>();

            string[] files = Directory.GetFiles(steamPath, "*.acf");

            if (files == null)
            {
                return null;
            }

            for (int i = 0, l = files.Length; i < l; i++)
            {
                string content  = AcfToJson.Convert(files[i]);
                Acf contentJson = JsonConvert.DeserializeObject<Acf>(content);
                int stateFlag   = int.Parse(contentJson.StateFlags);

                if (stateFlag == (int)DownloadState.complete || stateFlag == (int)DownloadState.canceled ||
                                                                stateFlag == (int)DownloadState.ignore2 ||
                                                                stateFlag == (int)DownloadState.canceled2 ||
                                                                stateFlag > 1500)
                {
                    continue;
                }

                GameInfo game = new GameInfo()
                {
                    name = contentJson.name,
                    appid = contentJson.appid,
                    stateFlag = contentJson.StateFlags,
                    fileName = files[i]
                };

                downloadQueued.Add(game);
            }
            
            return downloadQueued.Count > 0 ? downloadQueued : null;
        }

        public static void Shutdown()
        {
            Process.Start("cmd.exe", "shutdown");
            Console.Clear();

            Banner.Banner.ShowBanner();

            Console.WriteLine("\nbye bye\n");
        }
    }
}
