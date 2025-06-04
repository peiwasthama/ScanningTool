using System;
using System.Threading.Tasks;
using scanningTool.Models;

namespace scanningTool.Services
{
    /// <summary>
    /// Interface for system service operations.
    /// </summary>
    public interface ISystemService
    {
        /// <summary>
        /// Gets system information asynchronously.
        /// </summary>
        /// <returns>A SystemInfo object containing system information.</returns>
        Task<SystemInfo> GetSystemInfoAsync();

        /// <summary>
        /// Gets system errors from the event log asynchronously.
        /// </summary>
        /// <param name="maxEvents">Maximum number of events to retrieve.</param>
        /// <returns>A list of system error events.</returns>
        Task<System.Collections.Generic.List<SystemEventInfo>> GetSystemErrorsAsync(int maxEvents = 100);

        /// <summary>
        /// Gets system warnings from the event log asynchronously.
        /// </summary>
        /// <param name="maxEvents">Maximum number of events to retrieve.</param>
        /// <returns>A list of system warning events.</returns>
        Task<System.Collections.Generic.List<SystemEventInfo>> GetSystemWarningsAsync(int maxEvents = 100);
        
        /// <summary>
        /// Analyzes a system event using AI and returns potential solutions.
        /// </summary>
        /// <param name="eventInfo">The system event to analyze.</param>
        /// <returns>A string containing the AI analysis and potential solutions.</returns>
        Task<string> AnalyzeEventWithAIAsync(SystemEventInfo eventInfo);
        
        /// <summary>
        /// Analyzes an error message using AI and returns potential solutions.
        /// </summary>
        /// <param name="errorMessage">The error message to analyze.</param>
        /// <returns>A string containing the AI analysis and potential solutions.</returns>
        Task<string> AnalyzeErrorWithAIAsync(string errorMessage);
    }
}
