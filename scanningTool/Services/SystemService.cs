using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using scanningTool.Models;
using scanningTool.Helpers;

namespace scanningTool.Services
{
    /// <summary>
    /// Implementation of the system service interface with AI analysis integration.
    /// </summary>
    public class SystemService : ISystemService
    {
        private readonly IAIService _aiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemService"/> class.
        /// </summary>
        /// <param name="aiService">The AI service for error analysis.</param>
        public SystemService(IAIService aiService = null)
        {
            _aiService = aiService ?? new AIService();
            LoggingHelper.LogInfo("SystemService initialized with AI capabilities");
        }

        /// <summary>
        /// Gets system information asynchronously.
        /// </summary>
        /// <returns>A SystemInfo object containing system information.</returns>
        public async Task<Models.SystemInfo> GetSystemInfoAsync()
        {
            LoggingHelper.LogInfo("Getting system information");
            return await Task.Run(() => GetSystemInfo());
        }

        /// <summary>
        /// Gets system information synchronously.
        /// </summary>
        /// <returns>A SystemInfo object containing system information.</returns>
        private Models.SystemInfo GetSystemInfo()
        {
            Models.SystemInfo systemInfo = new Models.SystemInfo();

            try
            {
                // Get computer system information
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        systemInfo.ComputerName = obj["Name"]?.ToString() ?? "Unknown";
                        systemInfo.TotalPhysicalMemory = obj["TotalPhysicalMemory"] != null ? 
                            Convert.ToUInt64(obj["TotalPhysicalMemory"]) : 0;
                    }
                }

                // Get operating system information
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        systemInfo.OperatingSystem = obj["Caption"]?.ToString() ?? "Unknown";
                        systemInfo.AvailablePhysicalMemory = obj["FreePhysicalMemory"] != null ? 
                            Convert.ToUInt64(obj["FreePhysicalMemory"]) * 1024 : 0;
                        
                        // Get install date
                        if (obj["InstallDate"] != null)
                        {
                            string installDateString = obj["InstallDate"].ToString();
                            if (DateTime.TryParseExact(
                                installDateString.Substring(0, 14), 
                                "yyyyMMddHHmmss", 
                                System.Globalization.CultureInfo.InvariantCulture, 
                                System.Globalization.DateTimeStyles.None, 
                                out DateTime installDate))
                            {
                                systemInfo.InstallDate = installDate;
                            }
                        }

                        // Get last boot time to calculate uptime
                        if (obj["LastBootUpTime"] != null)
                        {
                            string lastBootString = obj["LastBootUpTime"].ToString();
                            if (DateTime.TryParseExact(
                                lastBootString.Substring(0, 14), 
                                "yyyyMMddHHmmss", 
                                System.Globalization.CultureInfo.InvariantCulture, 
                                System.Globalization.DateTimeStyles.None, 
                                out DateTime lastBoot))
                            {
                                systemInfo.Uptime = DateTime.Now - lastBoot;
                            }
                        }
                    }
                }

                // Get processor information
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        systemInfo.ProcessorInfo = obj["Name"]?.ToString() ?? "Unknown";
                        break; // Just get the first processor
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error getting system information");
                throw new Exception("Failed to retrieve system information", ex);
            }

            return systemInfo;
        }

        /// <summary>
        /// Gets system errors from the event log asynchronously.
        /// </summary>
        /// <param name="maxEvents">Maximum number of events to retrieve.</param>
        /// <returns>A list of system error events.</returns>
        public async Task<List<Models.SystemEventInfo>> GetSystemErrorsAsync(int maxEvents = 100)
        {
            LoggingHelper.LogInfo($"Getting system errors (max: {maxEvents})");
            return await Task.Run(() => GetSystemErrors(maxEvents));
        }

        /// <summary>
        /// Gets system errors from the event log synchronously.
        /// </summary>
        /// <param name="maxEvents">Maximum number of events to retrieve.</param>
        /// <returns>A list of system error events.</returns>
        private List<Models.SystemEventInfo> GetSystemErrors(int maxEvents = 100)
        {
            List<Models.SystemEventInfo> errors = new List<Models.SystemEventInfo>();

            try
            {
                EventLog systemLog = new EventLog("System");
                EventLog applicationLog = new EventLog("Application");

                // Get errors from system log
                foreach (EventLogEntry entry in systemLog.Entries)
                {
                    if (entry.EntryType == EventLogEntryType.Error)
                    {
                        errors.Add(new Models.SystemEventInfo
                        {
                            EventId = entry.InstanceId,
                            Source = entry.Source,
                            LogName = "System",
                            Message = entry.Message,
                            TimeGenerated = entry.TimeGenerated,
                            Level = "Error"
                        });

                        if (errors.Count >= maxEvents / 2)
                            break;
                    }
                }

                // Get errors from application log
                foreach (EventLogEntry entry in applicationLog.Entries)
                {
                    if (entry.EntryType == EventLogEntryType.Error)
                    {
                        errors.Add(new Models.SystemEventInfo
                        {
                            EventId = entry.InstanceId,
                            Source = entry.Source,
                            LogName = "Application",
                            Message = entry.Message,
                            TimeGenerated = entry.TimeGenerated,
                            Level = "Error"
                        });

                        if (errors.Count >= maxEvents)
                            break;
                    }
                }

                // Sort by time, newest first
                errors.Sort((a, b) => b.TimeGenerated.CompareTo(a.TimeGenerated));
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error getting system errors");
                throw new Exception("Failed to retrieve system errors", ex);
            }

            return errors;
        }

        /// <summary>
        /// Gets system warnings from the event log asynchronously.
        /// </summary>
        /// <param name="maxEvents">Maximum number of events to retrieve.</param>
        /// <returns>A list of system warning events.</returns>
        public async Task<List<Models.SystemEventInfo>> GetSystemWarningsAsync(int maxEvents = 100)
        {
            LoggingHelper.LogInfo($"Getting system warnings (max: {maxEvents})");
            return await Task.Run(() => GetSystemWarnings(maxEvents));
        }

        /// <summary>
        /// Gets system warnings from the event log synchronously.
        /// </summary>
        /// <param name="maxEvents">Maximum number of events to retrieve.</param>
        /// <returns>A list of system warning events.</returns>
        private List<Models.SystemEventInfo> GetSystemWarnings(int maxEvents = 100)
        {
            List<Models.SystemEventInfo> warnings = new List<Models.SystemEventInfo>();

            try
            {
                EventLog systemLog = new EventLog("System");
                EventLog applicationLog = new EventLog("Application");

                // Get warnings from system log
                foreach (EventLogEntry entry in systemLog.Entries)
                {
                    if (entry.EntryType == EventLogEntryType.Warning)
                    {
                        warnings.Add(new Models.SystemEventInfo
                        {
                            EventId = entry.InstanceId,
                            Source = entry.Source,
                            LogName = "System",
                            Message = entry.Message,
                            TimeGenerated = entry.TimeGenerated,
                            Level = "Warning"
                        });

                        if (warnings.Count >= maxEvents / 2)
                            break;
                    }
                }

                // Get warnings from application log
                foreach (EventLogEntry entry in applicationLog.Entries)
                {
                    if (entry.EntryType == EventLogEntryType.Warning)
                    {
                        warnings.Add(new Models.SystemEventInfo
                        {
                            EventId = entry.InstanceId,
                            Source = entry.Source,
                            LogName = "Application",
                            Message = entry.Message,
                            TimeGenerated = entry.TimeGenerated,
                            Level = "Warning"
                        });

                        if (warnings.Count >= maxEvents)
                            break;
                    }
                }

                // Sort by time, newest first
                warnings.Sort((a, b) => b.TimeGenerated.CompareTo(a.TimeGenerated));
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error getting system warnings");
                throw new Exception("Failed to retrieve system warnings", ex);
            }

            return warnings;
        }

        /// <summary>
        /// Analyzes a system event using AI and returns potential solutions.
        /// </summary>
        /// <param name="eventInfo">The system event to analyze.</param>
        /// <returns>A string containing the AI analysis and potential solutions.</returns>
        public async Task<string> AnalyzeEventWithAIAsync(SystemEventInfo eventInfo)
        {
            if (_aiService == null || !_aiService.IsServiceAvailable())
            {
                return "AI analysis not available. Please configure the OpenAI API key in settings.";
            }

            try
            {
                LoggingHelper.LogInfo($"Analyzing event with AI: {eventInfo.EventId} from {eventInfo.Source}");
                return await _aiService.AnalyzeSystemEventAsync(
                    eventInfo.EventId, 
                    eventInfo.Source, 
                    eventInfo.LogName, 
                    eventInfo.Message);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error analyzing event with AI");
                return $"Error during AI analysis: {ex.Message}";
            }
        }

        /// <summary>
        /// Analyzes an error message using AI and returns potential solutions.
        /// </summary>
        /// <param name="errorMessage">The error message to analyze.</param>
        /// <returns>A string containing the AI analysis and potential solutions.</returns>
        public async Task<string> AnalyzeErrorWithAIAsync(string errorMessage)
        {
            if (_aiService == null || !_aiService.IsServiceAvailable())
            {
                return "AI analysis not available. Please configure the OpenAI API key in settings.";
            }

            try
            {
                LoggingHelper.LogInfo("Analyzing error message with AI");
                return await _aiService.AnalyzeErrorAsync(errorMessage);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error analyzing error message with AI");
                return $"Error during AI analysis: {ex.Message}";
            }
        }
    }
}
