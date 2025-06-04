using System;

namespace scanningTool.Models
{
    /// <summary>
    /// Class representing system event information.
    /// </summary>
    public class SystemEventInfo
    {
        /// <summary>
        /// Gets or sets the event ID.
        /// </summary>
        public long EventId { get; set; }

        /// <summary>
        /// Gets or sets the source of the event.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the log name.
        /// </summary>
        public string LogName { get; set; }

        /// <summary>
        /// Gets or sets the event message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the time the event was generated.
        /// </summary>
        public DateTime TimeGenerated { get; set; }

        /// <summary>
        /// Gets or sets the level of the event (Error, Warning, Information, etc.).
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Returns a string representation of the system event information.
        /// </summary>
        /// <returns>A string representation of the system event information.</returns>
        public override string ToString()
        {
            string result = $"Event ID: {EventId}\n";
            result += $"Source: {Source}\n";
            result += $"Log: {LogName}\n";
            result += $"Level: {Level}\n";
            result += $"Time: {TimeGenerated:yyyy-MM-dd HH:mm:ss}\n";
            result += $"Message: {Message}\n";
            
            return result;
        }
    }
}
