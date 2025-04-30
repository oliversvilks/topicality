using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topicality.Client.Infrastructure.Configuration
{
    /// <summary>
    /// CCN2 service Options
    /// </summary>
    public class CCN2ServiceOptions
    {
        /// <summary>
        /// Configuration key
        /// </summary>
        public const string CCN2Service = "CCN2Service";

        /// <summary>
        /// CCN2Service base URL
        /// </summary>
        public string? BaseUri { get; set; }

        /// <summary>
        /// Endpoint username
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Endpoint password
        /// </summary>
        public string? Password { get; set; }
    }
}
