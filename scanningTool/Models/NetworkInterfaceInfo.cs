using System;
using System.Collections.Generic;

namespace scanningTool.Models
{
    /// <summary>
    /// Class representing network interface information.
    /// </summary>
    public class NetworkInterfaceInfo
    {
        /// <summary>
        /// Gets or sets the name of the network interface.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the network interface.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the type of the network interface.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the status of the network interface.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the speed of the network interface in bits per second.
        /// </summary>
        public long Speed { get; set; }

        /// <summary>
        /// Gets or sets the IPv4 address of the network interface.
        /// </summary>
        public string IPv4Address { get; set; }

        /// <summary>
        /// Gets or sets the subnet mask of the network interface.
        /// </summary>
        public string SubnetMask { get; set; }

        /// <summary>
        /// Gets or sets the IPv6 address of the network interface.
        /// </summary>
        public string IPv6Address { get; set; }

        /// <summary>
        /// Gets or sets the gateway address of the network interface.
        /// </summary>
        public string Gateway { get; set; }

        /// <summary>
        /// Gets or sets the MAC address of the network interface.
        /// </summary>
        public string MacAddress { get; set; }

        /// <summary>
        /// Gets or sets the DNS servers of the network interface.
        /// </summary>
        public List<string> DnsServers { get; set; } = new List<string>();

        /// <summary>
        /// Gets the formatted speed of the network interface.
        /// </summary>
        public string FormattedSpeed
        {
            get
            {
                if (Speed < 1000)
                    return $"{Speed} bps";
                else if (Speed < 1000 * 1000)
                    return $"{Speed / 1000.0:F2} Kbps";
                else if (Speed < 1000 * 1000 * 1000)
                    return $"{Speed / (1000.0 * 1000.0):F2} Mbps";
                else
                    return $"{Speed / (1000.0 * 1000.0 * 1000.0):F2} Gbps";
            }
        }

        /// <summary>
        /// Returns a string representation of the network interface information.
        /// </summary>
        /// <returns>A string representation of the network interface information.</returns>
        public override string ToString()
        {
            string result = $"Interface: {Name}\n";
            result += $"Description: {Description}\n";
            result += $"Type: {Type}\n";
            result += $"Status: {Status}\n";
            result += $"Speed: {FormattedSpeed}\n";
            result += $"MAC Address: {MacAddress}\n";
            
            if (!string.IsNullOrEmpty(IPv4Address))
                result += $"IPv4 Address: {IPv4Address}\n";
            
            if (!string.IsNullOrEmpty(SubnetMask))
                result += $"Subnet Mask: {SubnetMask}\n";
            
            if (!string.IsNullOrEmpty(IPv6Address))
                result += $"IPv6 Address: {IPv6Address}\n";
            
            if (!string.IsNullOrEmpty(Gateway))
                result += $"Gateway: {Gateway}\n";
            
            if (DnsServers.Count > 0)
            {
                result += "DNS Servers:\n";
                foreach (var dns in DnsServers)
                {
                    result += $"  {dns}\n";
                }
            }
            
            return result;
        }
    }
}
