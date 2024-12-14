using System.Diagnostics;
using System.Text;

namespace steam_shutdxwn.Source
{
    public class Banner
    {
        private static StringBuilder _banner = new();

        public static void Show()
        {
            if (_banner.Length > 0)
            {
                Console.WriteLine(_banner);
                return;
            }

            _banner.AppendLine("                                                                                            ");
            _banner.AppendLine("     _                             _           _      _                                     ");
            _banner.AppendLine(" ___| |_ ___  __ _ _ __ ___    ___| |__  _   _| |_ __| | _____      ___ __                  ");
            _banner.AppendLine("/ __| __/ _ \\/ _` | '_ ` _ \\  / __| '_ \\| | | | __/ _` |/ _ \\ \\ /\\ / / '_ \\          ");
            _banner.AppendLine("\\__ \\ ||  __/ (_| | | | | | | \\__ \\ | | | |_| | || (_| | (_) \\ V  V /| | | |           ");
            _banner.AppendLine("|___/\\__\\___|\\__,_|_| |_| |_| |___/_| |_|\\__,_|\\__\\__,_|\\___/ \\_/\\_/ |_| |_|       ");
            _banner.AppendLine("                                                                                            ");

            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folderName = "shutdxwn";
            string filename = "banner.txt";
            string fullPath = Path.Combine(documentsPath, folderName, filename);

            if (File.Exists(fullPath))
            {
                StringBuilder customBaner = new();

                try
                {
                    using StreamReader reader = new(fullPath);
                    string? line = string.Empty;
                    for (int i = 0, maxBannerSize = 20; (line = reader.ReadLine()) != null && i < maxBannerSize; i++)
                    {
                        customBaner.AppendLine(line);
                    }
                }
                catch (Exception ex)
                {
                    customBaner.Clear();
                    Debug.WriteLine(ex.Message);
                }

                if (customBaner.Length > 0) _banner = customBaner;
            }

            Console.WriteLine(_banner);
        }
    }
}
