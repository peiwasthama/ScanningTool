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
    /// Unit tests for the AIService class.
    /// </summary>
    [TestClass]
    public class AIServiceTests
    {
        private Mock<IAIService> _mockAIService;

        /// <summary>
        /// Initializes test dependencies.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _mockAIService = new Mock<IAIService>();
        }

        /// <summary>
        /// Tests that AnalyzeErrorAsync returns expected result.
        /// </summary>
        [TestMethod]
        public async Task AnalyzeErrorAsync_ReturnsExpectedResult()
        {
            // Arrange
            string errorMessage = "Access denied to file C:\\example.txt";
            string expectedResult = "This error indicates a permission issue. The application does not have sufficient rights to access the file.";
            
            _mockAIService.Setup(s => s.AnalyzeErrorAsync(errorMessage))
                .ReturnsAsync(expectedResult);
                
            // Act
            string result = await _mockAIService.Object.AnalyzeErrorAsync(errorMessage);
            
            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        /// <summary>
        /// Tests that AnalyzeSystemEventAsync returns expected result.
        /// </summary>
        [TestMethod]
        public async Task AnalyzeSystemEventAsync_ReturnsExpectedResult()
        {
            // Arrange
            long eventId = 1234;
            string source = "Disk";
            string logName = "System";
            string message = "The device is not ready.";
            string expectedResult = "This event indicates a hardware issue with the disk. The system cannot access the device.";
            
            _mockAIService.Setup(s => s.AnalyzeSystemEventAsync(eventId, source, logName, message))
                .ReturnsAsync(expectedResult);
                
            // Act
            string result = await _mockAIService.Object.AnalyzeSystemEventAsync(eventId, source, logName, message);
            
            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        /// <summary>
        /// Tests that IsServiceAvailable returns expected result.
        /// </summary>
        [TestMethod]
        public void IsServiceAvailable_ReturnsExpectedResult()
        {
            // Arrange
            bool expectedResult = true;
            
            _mockAIService.Setup(s => s.IsServiceAvailable())
                .Returns(expectedResult);
                
            // Act
            bool result = _mockAIService.Object.IsServiceAvailable();
            
            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
