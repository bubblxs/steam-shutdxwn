using System;
using System.IO;
using System.Text;

namespace steam_shutdxwn.Source.Banner
{
    public static class Banner
    {
        public static void ShowBanner()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("                                                                                            ");
            stringBuilder.AppendLine("     _                             _           _      _                                     ");
            stringBuilder.AppendLine(" ___| |_ ___  __ _ _ __ ___    ___| |__  _   _| |_ __| | _____      ___ __                  ");
            stringBuilder.AppendLine("/ __| __/ _ \\/ _` | '_ ` _ \\  / __| '_ \\| | | | __/ _` |/ _ \\ \\ /\\ / / '_ \\          ");
            stringBuilder.AppendLine("\\__ \\ ||  __/ (_| | | | | | | \\__ \\ | | | |_| | || (_| | (_) \\ V  V /| | | |           ");
            stringBuilder.AppendLine("|___/\\__\\___|\\__,_|_| |_| |_| |___/_| |_|\\__,_|\\__\\__,_|\\___/ \\_/\\_/ |_| |_|       ");
            stringBuilder.AppendLine("                                                                                            ");
            
            /*
            string filePath = $"{AppDomain.CurrentDomain.BaseDirectory}/Source/Banner/banner.txt";
            string[] file = File.ReadAllLines(filePath);

            for (int i = 0, l = file.Length; i < l; i++)
            {
                stringBuilder.Append($"{file[i]}\n");
            }
            */
            
            Console.WriteLine(stringBuilder.ToString());
        }
    }
}
