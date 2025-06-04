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
    /// Unit tests for the DiskService class.
    /// </summary>
    [TestClass]
    public class DiskServiceTests
    {
        private Mock<IDiskService> _mockDiskService;

        /// <summary>
        /// Initializes test dependencies.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _mockDiskService = new Mock<IDiskService>();
        }

        /// <summary>
        /// Tests that GetDiskHealthInfoAsync returns expected result.
        /// </summary>
        [TestMethod]
        public async Task GetDiskHealthInfoAsync_ReturnsExpectedResult()
        {
            // Arrange
            var expectedDisks = new List<DiskHealthInfo>
            {
                new DiskHealthInfo
                {
                    DeviceID = "\\\\.\\PHYSICALDRIVE0",
                    Model = "Samsung SSD 970 EVO 1TB",
                    InterfaceType = "SCSI",
                    Size = 1000204886016, // 1 TB
                    Status = "OK",
                    Partitions = new List<DiskPartitionInfo>
                    {
                        new DiskPartitionInfo
                        {
                            Name = "Disk #0, Partition #0",
                            DriveLetter = "C:",
                            Size = 999204886016, // ~999 GB
                            FreeSpace = 500204886016 // ~500 GB
                        }
                    }
                }
            };
            
            _mockDiskService.Setup(s => s.GetDiskHealthInfoAsync())
                .ReturnsAsync(expectedDisks);
                
            // Act
            var result = await _mockDiskService.Object.GetDiskHealthInfoAsync();
            
            // Assert
            Assert.AreEqual(expectedDisks.Count, result.Count);
            Assert.AreEqual(expectedDisks[0].DeviceID, result[0].DeviceID);
            Assert.AreEqual(expectedDisks[0].Model, result[0].Model);
            Assert.AreEqual(expectedDisks[0].Size, result[0].Size);
            Assert.AreEqual(expectedDisks[0].Partitions.Count, result[0].Partitions.Count);
            Assert.AreEqual(expectedDisks[0].Partitions[0].DriveLetter, result[0].Partitions[0].DriveLetter);
        }

        /// <summary>
        /// Tests that CreateDiskImageAsync returns expected result.
        /// </summary>
        [TestMethod]
        public async Task CreateDiskImageAsync_ReturnsExpectedResult()
        {
            // Arrange
            string dumpItPath = @"C:\Tools\DumpIt.exe";
            string outputFolderPath = @"C:\Output";
            
            var expectedResult = new DiskImageResult
            {
                Success = true,
                ResultMessage = "Disk image created successfully at C:\\Output\\DiskImage_20250530_092200.raw"
            };
            
            _mockDiskService.Setup(s => s.CreateDiskImageAsync(dumpItPath, outputFolderPath))
                .ReturnsAsync(expectedResult);
                
            // Act
            var result = await _mockDiskService.Object.CreateDiskImageAsync(dumpItPath, outputFolderPath);
            
            // Assert
            Assert.AreEqual(expectedResult.Success, result.Success);
            Assert.AreEqual(expectedResult.ResultMessage, result.ResultMessage);
        }

        /// <summary>
        /// Tests that CreateDiskImageAsync handles errors correctly.
        /// </summary>
        [TestMethod]
        public async Task CreateDiskImageAsync_HandlesErrorsCorrectly()
        {
            // Arrange
            string dumpItPath = @"C:\Tools\NonExistentFile.exe";
            string outputFolderPath = @"C:\Output";
            
            var expectedResult = new DiskImageResult
            {
                Success = false,
                ResultMessage = "DumpIt executable not found at C:\\Tools\\NonExistentFile.exe"
            };
            
            _mockDiskService.Setup(s => s.CreateDiskImageAsync(dumpItPath, outputFolderPath))
                .ReturnsAsync(expectedResult);
                
            // Act
            var result = await _mockDiskService.Object.CreateDiskImageAsync(dumpItPath, outputFolderPath);
            
            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(expectedResult.ResultMessage, result.ResultMessage);
        }
    }
}
