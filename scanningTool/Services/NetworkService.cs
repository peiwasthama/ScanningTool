using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using scanningTool.Models;
using scanningTool.Helpers;

namespace scanningTool.Services
{
    /// <summary>
    /// Implementation of the network service interface.
    /// </summary>
    public class NetworkService : INetworkService
    {
        private List<string> _downInterfaces = new List<string>();

        /// <summary>
        /// Gets network interface information asynchronously.
        /// </summary>
        /// <returns>A list of NetworkInterfaceInfo objects.</returns>
        public async Task<List<NetworkInterfaceInfo>> GetNetworkInterfacesAsync()
        {
            LoggingHelper.LogInfo("Getting network interface information");
            return await Task.Run(() => GetNetworkInterfaces());
        }

        /// <summary>
        /// Gets network interface information synchronously.
        /// </summary>
        /// <returns>A list of NetworkInterfaceInfo objects.</returns>
        private List<NetworkInterfaceInfo> GetNetworkInterfaces()
        {
            List<NetworkInterfaceInfo> interfaces = new List<NetworkInterfaceInfo>();
            _downInterfaces.Clear();

            try
            {
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface ni in networkInterfaces)
                {
                    NetworkInterfaceInfo interfaceInfo = new NetworkInterfaceInfo
                    {
                        Name = ni.Name,
                        Description = ni.Description,
                        Type = ni.NetworkInterfaceType.ToString(),
                        Status = ni.OperationalStatus.ToString(),
                        Speed = ni.Speed
                    };

                    // Get IP addresses
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            interfaceInfo.IPv4Address = ip.Address.ToString();
                            interfaceInfo.SubnetMask = ip.IPv4Mask.ToString();
                        }
                        else if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        {
                            interfaceInfo.IPv6Address = ip.Address.ToString();
                        }
                    }

                    // Get gateway
                    foreach (GatewayIPAddressInformation gateway in ni.GetIPProperties().GatewayAddresses)
                    {
                        interfaceInfo.Gateway = gateway.Address.ToString();
                        break; // Just get the first gateway
                    }

                    // Get DNS servers
                    foreach (var dns in ni.GetIPProperties().DnsAddresses)
                    {
                        interfaceInfo.DnsServers.Add(dns.ToString());
                    }

                    // Get MAC address
                    interfaceInfo.MacAddress = string.Join("-", ni.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")));

                    // Check if interface is down
                    if (ni.OperationalStatus != OperationalStatus.Up && 
                        (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet || 
                         ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                    {
                        _downInterfaces.Add(ni.Name);
                    }

                    interfaces.Add(interfaceInfo);
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error getting network interface information");
                throw new Exception("Failed to retrieve network interface information", ex);
            }

            return interfaces;
        }

        /// <summary>
        /// Gets a list of network interfaces that are down.
        /// </summary>
        /// <returns>A list of interface names that are down.</returns>
        public List<string> GetDownInterfaces()
        {
            return _downInterfaces;
        }
    }
}
