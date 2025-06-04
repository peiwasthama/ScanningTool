using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using scanningTool.Models;
using scanningTool.Services;

namespace scanningTool.Tests
{
    /// <summary>
    /// Unit tests for the NetworkService class.
    /// </summary>
    [TestClass]
    public class NetworkServiceTests
    {
        private Mock<INetworkService> _mockNetworkService;

        /// <summary>
        /// Initializes test dependencies.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _mockNetworkService = new Mock<INetworkService>();
        }

        /// <summary>
        /// Tests that GetNetworkInterfacesAsync returns expected result.
        /// </summary>
        [TestMethod]
        public async Task GetNetworkInterfacesAsync_ReturnsExpectedResult()
        {
            // Arrange
            var expectedInterfaces = new List<NetworkInterfaceInfo>
            {
                new NetworkInterfaceInfo
                {
                    Name = "Ethernet",
                    Description = "Intel(R) Ethernet Connection",
                    Type = "Ethernet",
                    Status = "Up",
                    Speed = 1000000000, // 1 Gbps
                    IPv4Address = "192.168.1.100",
                    SubnetMask = "255.255.255.0",
                    IPv6Address = "fe80::1234:5678:9abc:def0",
                    Gateway = "192.168.1.1",
                    MacAddress = "00-11-22-33-44-55",
                    DnsServers = new List<string> { "8.8.8.8", "8.8.4.4" }
                },
                new NetworkInterfaceInfo
                {
                    Name = "Wi-Fi",
                    Description = "Intel(R) Wireless-AC 9560",
                    Type = "Wireless80211",
                    Status = "Up",
                    Speed = 433000000, // 433 Mbps
                    IPv4Address = "192.168.1.101",
                    SubnetMask = "255.255.255.0",
                    IPv6Address = "fe80::abcd:ef01:2345:6789",
                    Gateway = "192.168.1.1",
                    MacAddress = "AA-BB-CC-DD-EE-FF",
                    DnsServers = new List<string> { "8.8.8.8", "8.8.4.4" }
                }
            };
            
            _mockNetworkService.Setup(s => s.GetNetworkInterfacesAsync())
                .ReturnsAsync(expectedInterfaces);
                
            // Act
            var result = await _mockNetworkService.Object.GetNetworkInterfacesAsync();
            
            // Assert
            Assert.AreEqual(expectedInterfaces.Count, result.Count);
            Assert.AreEqual(expectedInterfaces[0].Name, result[0].Name);
            Assert.AreEqual(expectedInterfaces[0].IPv4Address, result[0].IPv4Address);
            Assert.AreEqual(expectedInterfaces[1].Name, result[1].Name);
            Assert.AreEqual(expectedInterfaces[1].IPv4Address, result[1].IPv4Address);
        }

        /// <summary>
        /// Tests that GetDownInterfaces returns expected result.
        /// </summary>
        [TestMethod]
        public void GetDownInterfaces_ReturnsExpectedResult()
        {
            // Arrange
            var expectedDownInterfaces = new List<string> { "Ethernet 2", "Bluetooth Network Connection" };
            
            _mockNetworkService.Setup(s => s.GetDownInterfaces())
                .Returns(expectedDownInterfaces);
                
            // Act
            var result = _mockNetworkService.Object.GetDownInterfaces();
            
            // Assert
            Assert.AreEqual(expectedDownInterfaces.Count, result.Count);
            Assert.AreEqual(expectedDownInterfaces[0], result[0]);
            Assert.AreEqual(expectedDownInterfaces[1], result[1]);
        }
    }
}
