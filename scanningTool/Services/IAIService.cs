using System;
using System.Threading.Tasks;

namespace scanningTool.Services
{
    /// <summary>
    /// Interface for AI service that analyzes error logs and provides solutions.
    /// </summary>
    public interface IAIService
    {
        /// <summary>
        /// Analyzes an error message using AI and returns potential solutions.
        /// </summary>
        /// <param name="errorMessage">The error message to analyze.</param>
        /// <returns>A string containing the AI analysis and potential solutions.</returns>
        Task<string> AnalyzeErrorAsync(string errorMessage);

        /// <summary>
        /// Analyzes a system event and returns potential solutions.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="source">The event source.</param>
        /// <param name="logName">The log name.</param>
        /// <param name="message">The event message.</param>
        /// <returns>A string containing the AI analysis and potential solutions.</returns>
        Task<string> AnalyzeSystemEventAsync(long eventId, string source, string logName, string message);

        /// <summary>
        /// Checks if the AI service is properly configured and available.
        /// </summary>
        /// <returns>True if the service is available, false otherwise.</returns>
        bool IsServiceAvailable();
    }
}
