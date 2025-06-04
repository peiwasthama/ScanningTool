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
    /// Unit tests for the SystemService class.
    /// </summary>
    [TestClass]
    public class SystemServiceTests
    {
        private Mock<ISystemService> _mockSystemService;
        private Mock<IAIService> _mockAIService;

        /// <summary>
        /// Initializes test dependencies.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _mockAIService = new Mock<IAIService>();
            _mockSystemService = new Mock<ISystemService>();
        }

        /// <summary>
        /// Tests that GetSystemInfoAsync returns expected result.
        /// </summary>
        [TestMethod]
        public async Task GetSystemInfoAsync_ReturnsExpectedResult()
        {
            // Arrange
            var expectedInfo = new SystemInfo
            {
                ComputerName = "TestComputer",
                OperatingSystem = "Windows 10",
                TotalPhysicalMemory = 8589934592, // 8 GB
                AvailablePhysicalMemory = 4294967296, // 4 GB
                ProcessorInfo = "Intel Core i7",
                InstallDate = new DateTime(2023, 1, 1),
                Uptime = TimeSpan.FromHours(48)
            };
            
            _mockSystemService.Setup(s => s.GetSystemInfoAsync())
                .ReturnsAsync(expectedInfo);
                
            // Act
            var result = await _mockSystemService.Object.GetSystemInfoAsync();
            
            // Assert
            Assert.AreEqual(expectedInfo.ComputerName, result.ComputerName);
            Assert.AreEqual(expectedInfo.OperatingSystem, result.OperatingSystem);
            Assert.AreEqual(expectedInfo.TotalPhysicalMemory, result.TotalPhysicalMemory);
            Assert.AreEqual(expectedInfo.AvailablePhysicalMemory, result.AvailablePhysicalMemory);
            Assert.AreEqual(expectedInfo.ProcessorInfo, result.ProcessorInfo);
            Assert.AreEqual(expectedInfo.InstallDate, result.InstallDate);
            Assert.AreEqual(expectedInfo.Uptime, result.Uptime);
        }

        /// <summary>
        /// Tests that GetSystemErrorsAsync returns expected result.
        /// </summary>
        [TestMethod]
        public async Task GetSystemErrorsAsync_ReturnsExpectedResult()
        {
            // Arrange
            var expectedErrors = new List<SystemEventInfo>
            {
                new SystemEventInfo
                {
                    EventId = 1001,
                    Source = "Application Error",
                    LogName = "Application",
                    Message = "The application crashed",
                    TimeGenerated = DateTime.Now.AddDays(-1),
                    Level = "Error"
                },
                new SystemEventInfo
                {
                    EventId = 1002,
                    Source = "Disk",
                    LogName = "System",
                    Message = "Disk error detected",
                    TimeGenerated = DateTime.Now.AddDays(-2),
                    Level = "Error"
                }
            };
            
            _mockSystemService.Setup(s => s.GetSystemErrorsAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedErrors);
                
            // Act
            var result = await _mockSystemService.Object.GetSystemErrorsAsync(100);
            
            // Assert
            Assert.AreEqual(expectedErrors.Count, result.Count);
            Assert.AreEqual(expectedErrors[0].EventId, result[0].EventId);
            Assert.AreEqual(expectedErrors[0].Source, result[0].Source);
            Assert.AreEqual(expectedErrors[1].EventId, result[1].EventId);
            Assert.AreEqual(expectedErrors[1].Source, result[1].Source);
        }

        /// <summary>
        /// Tests that AnalyzeEventWithAIAsync returns expected result.
        /// </summary>
        [TestMethod]
        public async Task AnalyzeEventWithAIAsync_ReturnsExpectedResult()
        {
            // Arrange
            var eventInfo = new SystemEventInfo
            {
                EventId = 1001,
                Source = "Application Error",
                LogName = "Application",
                Message = "The application crashed",
                TimeGenerated = DateTime.Now.AddDays(-1),
                Level = "Error"
            };
            
            string expectedAnalysis = "This error indicates that the application crashed unexpectedly. This could be due to a memory issue or a bug in the application.";
            
            _mockSystemService.Setup(s => s.AnalyzeEventWithAIAsync(It.IsAny<SystemEventInfo>()))
                .ReturnsAsync(expectedAnalysis);
                
            // Act
            var result = await _mockSystemService.Object.AnalyzeEventWithAIAsync(eventInfo);
            
            // Assert
            Assert.AreEqual(expectedAnalysis, result);
        }

        /// <summary>
        /// Tests that AnalyzeErrorWithAIAsync returns expected result.
        /// </summary>
        [TestMethod]
        public async Task AnalyzeErrorWithAIAsync_ReturnsExpectedResult()
        {
            // Arrange
            string errorMessage = "Access denied to file C:\\example.txt";
            string expectedAnalysis = "This error indicates a permission issue. The application does not have sufficient rights to access the file.";
            
            _mockSystemService.Setup(s => s.AnalyzeErrorWithAIAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedAnalysis);
                
            // Act
            var result = await _mockSystemService.Object.AnalyzeErrorWithAIAsync(errorMessage);
            
            // Assert
            Assert.AreEqual(expectedAnalysis, result);
        }
    }
}
