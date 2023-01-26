using System.Text;

namespace steam_shutdxwn.Source.Helpers
{
    public static class AcfToJson
    {
        public static string Convert(string filePath)
        {
            char tabUnicode = '\u0009';
            string[] acfContent = File.ReadAllLines(filePath);

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 1, l = acfContent.Length; i < l; i++)
            {
                string line;
                string nextLine;
                string[] filteredContent = acfContent[i].Split(tabUnicode);

                filteredContent = Array.FindAll(filteredContent, (element) => !element.Contains("{") && !element.Contains("}") && element != "");

                if (filteredContent.Length >= 2)
                {
                    line = $"{filteredContent[0]}: {filteredContent[1]}";
                }
                else
                {
                    line = acfContent[i].Replace(tabUnicode, ' ');
                }

                stringBuilder.Append(line);

                if (i + 1 == l || line.Contains("{"))
                {
                    continue;
                }

                nextLine = acfContent[i + 1];

                if (nextLine.Contains("{"))
                {
                    stringBuilder.Append(":");
                }
                else if (!nextLine.Contains("}"))
                {
                    stringBuilder.Append(",");
                }
                else if (line.EndsWith("}") && i + 1 < l)
                {
                    stringBuilder.Append("");
                }
                else {/** :3 **/}
            }

            return stringBuilder.ToString();
        }
    }
}
