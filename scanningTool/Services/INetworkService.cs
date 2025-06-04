using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using scanningTool.Models;

namespace scanningTool.Services
{
    /// <summary>
    /// Interface for network service operations.
    /// </summary>
    public interface INetworkService
    {
        /// <summary>
        /// Gets network interface information asynchronously.
        /// </summary>
        /// <returns>A list of NetworkInterfaceInfo objects.</returns>
        Task<List<NetworkInterfaceInfo>> GetNetworkInterfacesAsync();

        /// <summary>
        /// Gets a list of network interfaces that are down.
        /// </summary>
        /// <returns>A list of interface names that are down.</returns>
        List<string> GetDownInterfaces();
    }
}
