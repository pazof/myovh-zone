// // Setup.cs
// /*
// Paul Schneider paul@pschneider.fr 01/12/2018 22:52 20182018 12 1
// */
using System;
using System.Net;
using System.Dynamic;
namespace myovh_zone
{
    /// <summary>
    /// Setup.
    /// </summary>
    public class Setup
    {
        /// <summary>
        /// Gets or sets the ovh neworking interface name.
        /// </summary>
        /// <value>The ovh interface.</value>
        public string OvhInterface { get; set; }

        /// <summary>
        /// Gets or sets the ovh end point.
        /// </summary>
        /// <value>The end point.</value>
        public string EndPoint { get; set; }

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
        public string ZoneTemplateFileName { get; set; }

        public string IpKeyWord { get; set; } = "{ip}";

        public string IpCacheFileName { get; set; } = "latest-ip";

        public string ZoneName { get; set; }

    }
}
