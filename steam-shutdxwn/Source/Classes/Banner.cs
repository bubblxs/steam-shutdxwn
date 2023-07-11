using System.Text;

namespace steam_shutdxwn.Source.Classes
{
    public class Banner
    {
        private static StringBuilder _banner = new StringBuilder();

        public static void Show()
        {
            if (_banner.Length > 0)
            {
                Console.WriteLine(_banner);
                return;
            }

            string bannerPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folderName = "shutdxwn";
            string fileName = "banner.txt";
            string fullPath = Path.Combine(bannerPath, folderName, fileName);

            if (File.Exists(fullPath))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(fullPath))
                    {
                        string? line = string.Empty;
                        for (int i = 0, maxBannerSize = 20; (line = reader.ReadLine()) != null && i < maxBannerSize; i++)
                        {
                            _banner.AppendLine(line);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _banner.Clear();
                    _banner.Append("steam shutdxwn");
                }
            }
            else
            {
                _banner.AppendLine("                                                                                            ");
                _banner.AppendLine("     _                             _           _      _                                     ");
                _banner.AppendLine(" ___| |_ ___  __ _ _ __ ___    ___| |__  _   _| |_ __| | _____      ___ __                  ");
                _banner.AppendLine("/ __| __/ _ \\/ _` | '_ ` _ \\  / __| '_ \\| | | | __/ _` |/ _ \\ \\ /\\ / / '_ \\          ");
                _banner.AppendLine("\\__ \\ ||  __/ (_| | | | | | | \\__ \\ | | | |_| | || (_| | (_) \\ V  V /| | | |           ");
                _banner.AppendLine("|___/\\__\\___|\\__,_|_| |_| |_| |___/_| |_|\\__,_|\\__\\__,_|\\___/ \\_/\\_/ |_| |_|       ");
                _banner.AppendLine("                                                                                            ");
            }

            Console.WriteLine(_banner);
        }
    }
}
