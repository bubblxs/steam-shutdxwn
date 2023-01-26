using Newtonsoft.Json;

namespace steam_shutdxwn.Source.Classes
{
    public class Acf
    {
        public string name { get; set; } = String.Empty;
        public string appid { get; set; } = String.Empty;
        public string buildid { get; set; } = String.Empty;
        public string universe { get; set; } = String.Empty;
        public string LastOwner { get; set; } = String.Empty;
        public string StateFlags { get; set; } = String.Empty;
        public string installdir { get; set; } = String.Empty;
        public string SizeOnDisk { get; set; } = String.Empty;
        public string LastUpdated { get; set; } = String.Empty;
        public string StagingSize { get; set; } = String.Empty;
        public string BytesStaged { get; set; } = String.Empty;
        public string LauncherPath { get; set; } = String.Empty;
        public string BytesToStage { get; set; } = String.Empty;
        public string BytesToDownload { get; set; } = String.Empty;
        public string BytesDownloaded { get; set; } = String.Empty;

        [JsonIgnore]
        public string UpdateResult { get; set; } = String.Empty;
        [JsonIgnore]
        public string TargetBuildID { get; set; } = String.Empty;
        [JsonIgnore]
        public string AutoUpdateBehavior { get; set; } = String.Empty;
        [JsonIgnore]
        public string AllowOtherDownloadsWhileRunning { get; set; } = String.Empty;
        [JsonIgnore]
        public string ScheduledAutoUpdate { get; set; } = String.Empty;
        [JsonIgnore]
        public Dictionary<string, Dictionary<string, string>> InstalledDepots { get; set; }
        [JsonIgnore]
        public Dictionary<string, string> sharedSharedDepots { get; set; }
        [JsonIgnore]
        public Dictionary<string, string> checkguid { get; set; }
        [JsonIgnore]
        public Dictionary<string, string> UserConfig { get; set; }
        [JsonIgnore]
        public Dictionary<string, string> MountedConfig { get; set; }
    }
}
