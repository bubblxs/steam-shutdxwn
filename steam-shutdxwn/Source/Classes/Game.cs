namespace steam_shutdxwn.Source.Classes
{
    public class Game
    {
        public string AppId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string StateFlag { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{AppId} - {Name} - {StateFlag} - {FileName}";
        }
    }
}
