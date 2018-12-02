using System;
using Ovh.Api;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;

namespace myovh_zone
{
    // 
    class Program
    {
        public const string OvhConfigFile = "ovh.json";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello OVH World!");

            switch (args.Length)
            {
                case (0):
                    Console.WriteLine("Usage? : dotnet " +
                                      Environment.CommandLine
                                      + " [init|validate|getip|zoneupload|showconfig]");

                    break;
                case 1:
                    switch (args[0])
                    {
                        case "init":
                            Console.WriteLine("Enter application key (8bTEPYMKP8m4rKI2):");
                            var apkey = Console.ReadLine();
                            Console.WriteLine("Enter secret:");
                            var secret = Console.ReadLine();
                            Console.WriteLine("Enter consumer key:");
                            var conskey = Console.ReadLine();
                            Console.WriteLine("Enter you network interface to ovh (eth0):");
                            var netdev = Console.ReadLine();

                            Setup newSetup = new Setup
                            {
                                EndPoint = "ovh-eu",
                                ApplicationKey = apkey,
                                ApplicationSecret = secret,
                                ConsumerKey = conskey,
                                OvhInterface = netdev
                            };

                            File.WriteAllText
                                (OvhConfigFile,
                                 JsonConvert.SerializeObject(newSetup, Formatting.Indented));

                            break;
                        case "validate":
                            SetAndValidateConsumerKey();
                            break;
                        case "showconfig":
                            var sconfig = GetSetup();
                            var c = GetClient(sconfig);
                            Console.WriteLine($"EP: {c.Endpoint}");
                            Console.WriteLine($"ApiKey: {c.ApplicationKey}");
                            Console.WriteLine($"Secret: {c.ApplicationSecret}");
                            Console.WriteLine($"ConsKey: {c.ConsumerKey}");
                            Console.WriteLine($"Interface: {sconfig.OvhInterface}");
                            Console.WriteLine($"Zone template: {sconfig.ZoneTemplateFileName}");
                            break;

                        case "getip":
                            var config = GetSetup();
                            Console.WriteLine($"Interface: {config.OvhInterface}");
                            var saddr = GetIp(config.OvhInterface);
                            Console.WriteLine("Ip:" + saddr);
                            break;

                        case "zoneupload":
                            UploadNewConfig();
                            break;

                        default:
                            Console.WriteLine($"Not a command : {args[0]}");
                            break;
                    }
                    break;

            }


        }

        static Setup GetSetup() => JsonConvert.DeserializeObject<Setup>(
                                File.ReadAllText(OvhConfigFile));

        static Client GetClient(Setup setup)
        {
            return new Client(setup.EndPoint,
                              setup.ApplicationKey,
                              setup.ApplicationSecret,
                              setup.ConsumerKey,
                              180);
        }

        static string GetIp(string ifname)
        {
            var startInfo = new ProcessStartInfo
            {
                RedirectStandardOutput = true,
                FileName = "ifaddr",
                Arguments = ifname
            };
            var info = Process.Start(startInfo);
            return info.StandardOutput.ReadToEnd();
        }

        static void UploadNewConfig()
        {
            var config = GetSetup();
            var saddr = GetIp(config.OvhInterface);
            var ipci = new FileInfo(config.IpCacheFileName);
            if (ipci.Exists)
            {
                var cachedinfo = File.ReadAllText(config.IpCacheFileName);
                if (cachedinfo == saddr) {
                    Console.WriteLine($"Cached info is correct (read from {config.IpCacheFileName}). aborting.");
                    return;
                }
            }
            var zone = File.ReadAllLines(config.ZoneTemplateFileName);
            StringBuilder sb = new StringBuilder();
            foreach (string line in zone) {
                sb.AppendLine(line.Replace(config.IpKeyWord, saddr));
            }

            var cli = GetClient(config);
            var result = cli.Post($"/domain/zone/{config.ZoneName}/import",
                     new { 
                zoneFile = sb.ToString()
            });
            Console.WriteLine(result);
            File.WriteAllText(config.IpCacheFileName, saddr);
        }

        static void SetAndValidateConsumerKey()
        {
            Setup setup = GetSetup();
            CredentialRequest requestPayload = new CredentialRequest(
                new List<AccessRight>(){
                new AccessRight("POST", $"/domain/zone/{setup.ZoneName}/import")
                },
                $"https://{setup.ZoneName}"
            );
            var client = GetClient(setup);
            CredentialRequestResult credentialRequestResult =
                client.RequestConsumerKey(requestPayload);

            setup.ConsumerKey = credentialRequestResult.ConsumerKey;
            File.WriteAllText(OvhConfigFile,
                              JsonConvert.SerializeObject(setup, Formatting.Indented));
            Console.WriteLine("Setup was updated, for you consumer key.");
            Console.Write( 
                String.Format("Please visit {0} to authenticate ",
                    credentialRequestResult.ValidationUrl));


            Console.WriteLine("and press enter to continue");
            Console.ReadLine();

        }
    }
}
