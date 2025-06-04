using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using System.Linq;
using scanningTool.Helpers;

namespace scanningTool.Services
{
    /// <summary>
    /// Implementation of the AI service interface that uses OpenAI to analyze error logs and provide solutions.
    /// </summary>
    public class AIService : IAIService
    {
        private OpenAIService _openAIService;
        private bool _isConfigured = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="AIService"/> class.
        /// </summary>
        public AIService()
        {
            try
            {
                string apiKey = ConfigurationHelper.GetOpenAIApiKey();
                
                if (!string.IsNullOrEmpty(apiKey))
                {
                    _openAIService = new OpenAIService(new OpenAiOptions
                    {
                        ApiKey = apiKey
                    });
                    _openAIService.SetDefaultModelId(Models.ChatGpt3_5Turbo);
                    _isConfigured = true;
                    LoggingHelper.LogInfo("AI Service initialized successfully");
                }
                else
                {
                    LoggingHelper.LogWarning("OpenAI API key not found in configuration");
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error initializing AI Service");
            }
        }

        /// <summary>
        /// Analyzes an error message using AI and returns potential solutions.
        /// </summary>
        /// <param name="errorMessage">The error message to analyze.</param>
        /// <returns>A string containing the AI analysis and potential solutions.</returns>
        public async Task<string> AnalyzeErrorAsync(string errorMessage)
        {
            if (!_isConfigured)
            {
                return "AI analysis not available: Service not configured. Please add your OpenAI API key to the configuration.";
            }

            try
            {
                LoggingHelper.LogInfo($"Analyzing error with AI: {errorMessage.Substring(0, Math.Min(100, errorMessage.Length))}...");
                
                var messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem("You are a helpful system diagnostic assistant. Analyze the following error message and provide a clear explanation of what it means and suggest possible solutions."),
                    ChatMessage.FromUser(errorMessage)
                };

                var request = new ChatCompletionCreateRequest
                {
                    Messages = messages,
                    N = 1,
                    MaxTokens = 1000,
                    Temperature = 0.3f
                };

                var response = await _openAIService.ChatCompletion.CreateCompletion(request);

                if (response.Successful)
                {
                    var result = response.Choices.First().Message.Content;
                    LoggingHelper.LogInfo("AI analysis completed successfully");
                    return result;
                }
                else
                {
                    LoggingHelper.LogWarning($"AI analysis failed: {response.Error?.Message}");
                    return $"AI analysis failed: {response.Error?.Message}";
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error during AI analysis");
                return $"Error during AI analysis: {ex.Message}";
            }
        }

        /// <summary>
        /// Analyzes a system event and returns potential solutions.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="source">The event source.</param>
        /// <param name="logName">The log name.</param>
        /// <param name="message">The event message.</param>
        /// <returns>A string containing the AI analysis and potential solutions.</returns>
        public async Task<string> AnalyzeSystemEventAsync(long eventId, string source, string logName, string message)
        {
            if (!_isConfigured)
            {
                return "AI analysis not available: Service not configured. Please add your OpenAI API key to the configuration.";
            }

            try
            {
                LoggingHelper.LogInfo($"Analyzing system event with AI: Event ID {eventId} from {source}");
                
                var formattedEvent = $"Event ID: {eventId}\nSource: {source}\nLog: {logName}\nMessage: {message}";
                
                var messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem("You are a helpful system diagnostic assistant. Analyze the following Windows event log entry and provide a clear explanation of what it means, its potential impact on the system, and suggest possible solutions or actions to address it."),
                    ChatMessage.FromUser(formattedEvent)
                };

                var request = new ChatCompletionCreateRequest
                {
                    Messages = messages,
                    N = 1,
                    MaxTokens = 1000,
                    Temperature = 0.3f
                };

                var response = await _openAIService.ChatCompletion.CreateCompletion(request);

                if (response.Successful)
                {
                    var result = response.Choices.First().Message.Content;
                    LoggingHelper.LogInfo("AI analysis of system event completed successfully");
                    return result;
                }
                else
                {
                    LoggingHelper.LogWarning($"AI analysis of system event failed: {response.Error?.Message}");
                    return $"AI analysis failed: {response.Error?.Message}";
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error during AI analysis of system event");
                return $"Error during AI analysis: {ex.Message}";
            }
        }

        /// <summary>
        /// Checks if the AI service is properly configured and available.
        /// </summary>
        /// <returns>True if the service is available, false otherwise.</returns>
        public bool IsServiceAvailable()
        {
            return _isConfigured;
        }
    }
}
