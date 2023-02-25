using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace steam_shutdxwn.Source.Helpers
{
    public class PathSearcher
    { 
        public string getMainSteamPath()
        {
            RegistryKey registryPath = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam\\");
            string steamPath = $"{registryPath.GetValue("SteamPath")}/steamapps/";
            return steamPath;
        }

        public List<string> getAllSteamappPaths()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            List<string> paths = new List<string>();

            foreach (var drive in allDrives)
            {
                var possiblePaths = new List<string>
                {
                    // Steam would create the first path below in a drive that isnt the main one
                    Path.Combine(drive.RootDirectory.FullName, "SteamLibrary", "Steamapps"),
                    // but i think that this one is possible too..
                    Path.Combine(drive.RootDirectory.FullName, "Steamapps")
                    // maybe there is others possibilities, but those would be the common ones
                };

                foreach (var path in possiblePaths)
                {
                    if (Directory.Exists(path))
                    {
                        paths.Add(path);
                    }
                }
            }

            var mainPath = getMainSteamPath();
            var match = paths.FirstOrDefault(stringToCheck => stringToCheck.Contains(mainPath));
            if (match == null)
            {
                paths.Add(Path.GetFullPath(mainPath));
            }    

            return paths;
        }
    }
}
