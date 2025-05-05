using System.Diagnostics;
using Newtonsoft.Json;

public static class Tasks
{

    public static string GetIp(string ifname)
    {
        var startInfo = new ProcessStartInfo
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            FileName = "ifaddr",
            Arguments = ifname
        };
        var info = Process.Start(startInfo);
        if (info == null) throw new InvalidProgramException("ifaddr");
        return info.StandardOutput.ReadToEnd();
    }

}