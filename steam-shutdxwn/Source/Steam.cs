using Newtonsoft.Json;
using steam_shutdxwn.Source.Classes;
using System.Diagnostics;
using steam_shutdxwn.Source.Helpers;

namespace steam_shutdxwn.Source
{
    public class Steam
    {
        public static bool isAlreadyCalled = false;
        public static string steamPath = String.Empty;

        public bool IsSteamRunning()
        {
           return Process.GetProcessesByName("Steam").Length > 0;
        }

        public void SetSteamPath(string sp)
        {
            steamPath = sp;
        }

        public void FilesWatcher(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(2000);
            List<GameInfo> downloadQueue = GetDownloadQueue(steamPath);

            if (downloadQueue == null && !isAlreadyCalled)
            {
                isAlreadyCalled = true;
                Shutdown();
            }
        }

        public List<GameInfo> GetDownloadQueue(string steamPath)
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

                if (stateFlag == (int)DownloadState.complete || 
                    stateFlag == (int)DownloadState.ignore2  ||
                    stateFlag == (int)DownloadState.ignore   ||
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

        public void Shutdown()
        {
            Process.Start("cmd.exe", "/C shutdown /s");
            Console.Clear();
            Banner.Banner.ShowBanner();
            Console.WriteLine("\nbye bye\n");
        }
    }
}
