using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using Ovh.Api;

public static class ConfigurationExtensions
{
    public static Client GetClient(this Setup setup)
    {
        return new Client(setup.EndPoint,
                          setup.ApplicationKey,
                          setup.ApplicationSecret,
                          setup.ConsumerKey,
                          new TimeSpan(0, 0, 180));
    }

    public static async Task UploadNewConfigAsync(this Setup config)
    {
        string saddr;
        if (!IpAddressNeedsUpdate(config, out saddr))
        {
            return;
        }
        var zone = File.ReadAllLines(config.ZoneTemplateFileName);
        StringBuilder sb = new StringBuilder();
        foreach (string line in zone)
        {
            var localzed = line.Replace(config.ZoneNameKeyWord, config.ZoneName);
            var finalline = localzed.Replace(config.IpKeyWord, saddr);
            sb.AppendLine(finalline);
            Console.WriteLine(finalline);
        }

        var cli = GetClient(config);
        var data = new
        {
            zoneFile = sb.ToString()
        };
        var result = await cli.PostAsync($"/domain/zone/{config.ZoneName}/import",
        JsonConvert.SerializeObject(data)
                 );
        Console.WriteLine(result);
        File.WriteAllText(config.IpCacheFileName, saddr);
    }

    public static string? GetLastSentIp(this Setup config)
    {
        var ipci = new FileInfo(config.IpCacheFileName);
        if (ipci.Exists)
        {
            return File.ReadAllText(config.IpCacheFileName);
        }
        return null;
    }

    public static bool IpAddressNeedsUpdate(this Setup config, out string saddr)
    {
        saddr = Tasks.GetIp(config.NetworkInterface);
        var cachedinfo = GetLastSentIp(config);
        if (cachedinfo == saddr)
        {
            Console.WriteLine($"Cached info is correct (read from {config.IpCacheFileName}). aborting.");
            return false;
        }
        return true;
    }
    
    public static Setup InitSetup(this Setup newSetup)
    {
        InputMethod(newSetup,
         $"Enter application key ({newSetup.ApplicationKey}):",
         (s, v) => s.ApplicationKey = v);
        InputMethod(newSetup,
         $"Enter secret: ({newSetup.ApplicationSecret}):",
         (s, v) => s.ApplicationSecret = v);
        InputMethod(newSetup,
         $"Enter consumer key: ({newSetup.ConsumerKey}):",
         (s, v) => s.ConsumerKey = v);
        InputMethod(newSetup,
         $"Enter you network interface: ({newSetup.NetworkInterface}):",
         (s, v) => s.NetworkInterface = v);

        File.WriteAllText
            (Program.OvhConfigFile,
             JsonConvert.SerializeObject(newSetup, Formatting.Indented));
        return newSetup;
    }

    private static void InputMethod(this Setup setup, string description, Action<Setup, String> setupaction)
    { // $"Enter application key ({newSetup.ApplicationKey}):"
        Console.WriteLine(description);
        var applicationKey = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(applicationKey))
        {
            setupaction(setup, applicationKey);
        }
    }

    public static async Task FixMyIp(this Setup config)
    {
        string ipAddress;
        if (!IpAddressNeedsUpdate(config, out ipAddress))
        {
            return;
        }
        // get the A record id
        // GET /domain/zone/{zoneName}/record ? zoneName= &  fieldType=A
        var client = GetClient(config);

        long[] response = await client.GetAsync<long[]>(config.GetRecordSearchEndpoint(),
        new QueryStringParams()
        {
            {"fieldType", "A"}
        });
        if (response.Length != 1)
        {
            Console.Error.WriteLine("Unexpected number of A record type");
            Console.Error.WriteLine("Waitting for one, and got " + response.Length);
            throw new InvalidOperationException("Expecting a single A record");
        }
        long recordId = response[0];
        // GET the record from id
        string resourceId = config.GetRecordEndpoint(recordId.ToString());
        Console.WriteLine("Resource Id : " + resourceId);
        Record r = await client.GetAsync<Record>(resourceId);
        // Update the record
        r.target = ipAddress;
        string putResponse = await client.PutAsync(resourceId, r);
        Console.WriteLine(putResponse);
    }
}