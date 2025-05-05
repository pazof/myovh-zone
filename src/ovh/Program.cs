using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using Ovh.Api;
using Newtonsoft.Json;
using Ovh.Api.Models;
using Mono.Options;


/// <summary>
/// The OVH command line
/// </summary>
class Program
{
    public static string OvhConfigFile = "ovh.json";
    public static string? networkInterface = null;
    public static string? zoneName = null;

    public static bool showHelp = false;

    
    static Mono.Options.OptionSet optionSet = new Mono.Options.OptionSet()
    {
        { "c|config=", "Configuration file", o => OvhConfigFile = o },
        { "i|interface=", "The network interface to get an ip setup from", o=> networkInterface = o },
        { "z|zone-name=", "The zone name used against OVH database (the managed hostname)", o=> zoneName = o },
        { "h|help", "Show help", o=> showHelp=true },
    };
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello OVH World!");

        List<string> commands;
        try
        {
            commands = optionSet.Parse(args);
        }
        catch (OptionException e)
        {
            Console.Write("ovh command line: ");
            Console.WriteLine(e.Message);
            Console.WriteLine("Try `greet --help' for more information.");
            return;
        }

        if (showHelp)
        {
            return;
        }
        Setup? config = GetSetup();
        if (config == null && (commands.Count == 0 || commands[0] != "init"))
        {
            Console.Error.WriteLine("No config found. Use init command");
            return;
        }
        else if (config == null)
        {
            config = new Setup().InitSetup();
        }
        else
        {
            // command option are
            // only on setup from config file
            // and not a brand new the user just filled in
            if (networkInterface != null)
                config.NetworkInterface = networkInterface;
            if (zoneName != null)
                config.ZoneName = zoneName;
        }

        switch (commands.Count)
        {
            case 1:
                switch (args[0])
                {
                    case "init":
                       
                        break;
                    case "validate":
                        await SetAndValidateConsumerKeyAsync();
                        break;

                    case "show-config":
                        Console.WriteLine($"EP: {config.EndPoint}");
                        Console.WriteLine($"ApiKey: {config.ApplicationKey}");
                        Console.WriteLine($"Secret: {config.ApplicationSecret}");
                        Console.WriteLine($"ConsKey: {config.ConsumerKey}");
                        Console.WriteLine($"Interface: {config.NetworkInterface}");
                        Console.WriteLine($"Zone template: {config.ZoneTemplateFileName}");
                        break;

                    case "get-ip":
                        Console.WriteLine($"Interface: {config.NetworkInterface}");
                        var myIp = Tasks.GetIp(config.NetworkInterface);
                        Console.WriteLine("Ip:" + myIp);
                        break;

                    case "zone-upload":
                        await config.UploadNewConfigAsync();
                        break;

                    case "update":
                        await config.FixMyIp();
                        break;

                    default:
                        Console.WriteLine($"Not a command : {args[0]}");
                        break;
                }
                break;
            default:
                ShowHelp();
                break;
        }


    }


    static void ShowHelp()
    {
        Console.WriteLine("Usage: ovh [OPTIONS]+ [init|show-config|validate|update|help]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        optionSet.WriteOptionDescriptions(Console.Out);
    }

    static Setup? GetSetup()
    {
        var fileSetupInfo = new FileInfo(OvhConfigFile);
        if (!fileSetupInfo.Exists)
        {
            Console.Error.WriteLine($"Setup was not found in {fileSetupInfo.FullName} Please, create a new one by using the 'init' command.");
            throw new InvalidOperationException("reading config");
        }
        var fileSetup = File.ReadAllText(OvhConfigFile);
        return JsonConvert.DeserializeObject<Setup>(fileSetup);
    }

    
    static async Task SetAndValidateConsumerKeyAsync()
    {
        Setup? setup = GetSetup();
        if (setup==null) throw new InvalidOperationException("Set And Validate Consumer Key : no config");
        CredentialRequest requestPayload = new CredentialRequest(
            new List<AccessRight>(){
                new AccessRight("POST", setup.GetImportEndpoint()),
                new AccessRight("GET", setup.GetRecordSearchEndpoint()),
                new AccessRight("GET", setup.GetRecordEndpoint("*")),
                new AccessRight("PUT", setup.GetRecordEndpoint("*")),
            },
            $"https://{setup.ZoneName}"
        );
        var client = setup.GetClient();
        CredentialRequestResult credentialRequestResult =
           await client.RequestConsumerKeyAsync(requestPayload);

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
