using System.Text;
using System.Text.Json;

namespace steam_shutdxwn.Source.Classes
{
    public class Acf
    {
        public string name { get; set; } = string.Empty;
        public string appid { get; set; } = string.Empty;
        public string buildid { get; set; } = string.Empty;
        public string universe { get; set; } = string.Empty;
        public string LastOwner { get; set; } = string.Empty;
        public string StateFlags { get; set; } = string.Empty;
        public string installdir { get; set; } = string.Empty;
        public string SizeOnDisk { get; set; } = string.Empty;
        public string LastUpdated { get; set; } = string.Empty;
        public string StagingSize { get; set; } = string.Empty;
        public string BytesStaged { get; set; } = string.Empty;
        public string LauncherPath { get; set; } = string.Empty;
        public string BytesToStage { get; set; } = string.Empty;
        public string BytesToDownload { get; set; } = string.Empty;
        public string BytesDownloaded { get; set; } = string.Empty;

        public static string? ToJson(string filePath)
        {
            try
            {
                char tab = '\u0009';
                string[] acfContent = File.ReadAllLines(filePath);

                StringBuilder content = new StringBuilder();

                if (acfContent.Length == 0 || string.IsNullOrEmpty(acfContent.ToString()))
                {
                    return null;
                }

                for (int i = 1, l = acfContent.Length; i < l; i++)
                {
                    string line = string.Empty;
                    string nextLine = string.Empty;
                    string[] filteredContent = acfContent[i].Split(tab);

                    filteredContent = Array.FindAll(filteredContent, (el) => !el.Contains("{") && !el.Contains("}") && el != "");

                    if (filteredContent.Length >= 2)
                    {
                        line = $"{filteredContent[0]}: {filteredContent[1]}";
                    }
                    else
                    {
                        line = acfContent[i].Replace(tab, ' ');
                    }

                    content.Append(line);

                    if (i + 1 == l) break;

                    nextLine = acfContent[i + 1];

                    if (nextLine.Contains('{'))
                    {
                        content.Append(':');
                        continue;
                    }

                    if (!nextLine.Contains('}') && !line.Contains('{'))
                    {
                        content.Append(',');
                        continue;
                    }

                    if (line.EndsWith('}') && i + 1 < l)
                    {
                        content.Append("");
                        continue;
                    }
                }

                // this will check if the file has been parsed correctly.
                // btw, i cant remember the reason why we dont return an ACF instead of a json.
                JsonSerializer.Deserialize<Acf>(content.ToString());

                return content.ToString();
            }
            catch (Exception ex)
            {
                // maybe this should return an error message or something instead of null.
                return null;
            }
        }
    }
}
