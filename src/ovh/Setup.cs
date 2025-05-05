// // Setup.cs
// 
// Paul Schneider paul@pschneider.fr 01/12/2018 22:52 20182018 12 1
// 
using System;
using System.Net;
using System.Dynamic;

/// <summary>
/// Setup.
/// </summary>
public record Setup
{
    public Setup()
    {}

    /// <summary>
    /// Builds a new Setup
    /// </summary>
    /// <param name="name"></param>
    /// <param name="netInterface"></param>
    /// <param name="endpoint"></param>
    /// <param name="applicationKey"></param>
    /// <param name="applicationSecret"></param>
    /// <param name="consumerKey"></param>
    public Setup(
        string name,
        string netInterface,
        string endpoint,
        string applicationKey,
        string applicationSecret,
        string consumerKey
    )
    {
        ZoneName = name;
        NetworkInterface = netInterface;
        EndPoint = endpoint;
        ApplicationKey = applicationKey;
        ApplicationSecret = applicationSecret;
        ConsumerKey = consumerKey;
    }

    /// <summary>
    /// Gets or sets the ovh neworking interface name.
    /// </summary>
    /// <value>The ovh interface.</value>
    public string NetworkInterface { get; set; }

    /// <summary>
    /// Gets or sets the ovh end point.
    /// </summary>
    /// <value>The end point.</value>
    public string EndPoint { get; set; } = "ovh-eu";

    /// <summary>
    /// Gets or sets the application key.
    /// </summary>
    /// <value>The application key.</value>
    public string ApplicationKey { get; set; }

    /// <summary>
    /// Gets or sets the application secret.
    /// </summary>
    /// <value>The application secret.</value>
    public string ApplicationSecret { get; set; }

    /// <summary>
    /// Gets or sets the consumer key.
    /// </summary>
    /// <value>The consumer key.</value>
    public string ConsumerKey { get; set; }

    /// <summary>
    /// Gets or sets the name of the zone template file.
    /// </summary>
    /// <value>The name of the zone template file.</value>
    public string ZoneTemplateFileName { get; set; } = "zone.template";

    /// <summary>
    /// Get or set the keywork used to be replaced with interface ip
    /// </summary>
    public string IpKeyWord { get; set; } = "{ip}";

    /// <summary>
    /// Get or set the keywork used to be replaced with zone name
    /// </summary>
    public string ZoneNameKeyWord { get; set; } = "{zn}";

    public string IpCacheFileName { get; set; } = "latest-ip";

    /// <summary>
    /// OVH Zone Name (like in <c>example.com</c>)
    /// </summary>
    /// <value></value>
    public string ZoneName { get; set; }

    public string GetImportEndpoint()
    {
        return $"/domain/zone/{ZoneName}/import";
    }
    public string GetRecordSearchEndpoint()
    {
        return $"/domain/zone/{ZoneName}/record";
    }
    public string GetRecordEndpoint(string recordId)
    {
        return $"/domain/zone/{ZoneName}/record/{recordId}";
    }

}