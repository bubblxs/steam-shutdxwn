using steam_shutdxwn.Source;

namespace steam_shutdxwn
{
    internal class Program
    {
        static void Main(string[] args)
        {   
            bool isDevEnv = args.Length > 0 && args[0] == "--dev";
            
            new Steam(isDevEnv).Init();
        }
    }
}