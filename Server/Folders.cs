using System.IO;

namespace Server
{
    public class Folders
    {
        public static string LogPath = Path.Combine(Directory.GetCurrentDirectory(), "log");
        public static string LOLPath = System.IO.File.ReadAllText("lolpath.txt").TrimEnd('\n');//@"C:\Program Files (x86)\Riot Games\League of Legends";
        public static string LeagueClientExe = Path.Combine(LOLPath, "LeagueClient.exe");
        public static string Lockfile = Path.Combine(LOLPath, "lockfile");
    }
}
