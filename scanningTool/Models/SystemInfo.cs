using System;

namespace scanningTool.Models
{
    /// <summary>
    /// Class representing system information.
    /// </summary>
    public class SystemInfo
    {
        /// <summary>
        /// Gets or sets the computer name.
        /// </summary>
        public string ComputerName { get; set; }

        /// <summary>
        /// Gets or sets the operating system name.
        /// </summary>
        public string OperatingSystem { get; set; }

        /// <summary>
        /// Gets or sets the total physical memory in bytes.
        /// </summary>
        public ulong TotalPhysicalMemory { get; set; }

        /// <summary>
        /// Gets or sets the available physical memory in bytes.
        /// </summary>
        public ulong AvailablePhysicalMemory { get; set; }

        /// <summary>
        /// Gets or sets the processor information.
        /// </summary>
        public string ProcessorInfo { get; set; }

        /// <summary>
        /// Gets or sets the installation date of the operating system.
        /// </summary>
        public DateTime InstallDate { get; set; }

        /// <summary>
        /// Gets or sets the system uptime.
        /// </summary>
        public TimeSpan Uptime { get; set; }

        /// <summary>
        /// Gets the system age as a string.
        /// </summary>
        public string SystemAge
        {
            get
            {
                TimeSpan age = DateTime.Now - InstallDate;
                return $"{age.Days} days, {age.Hours} hours, {age.Minutes} minutes";
            }
        }

        /// <summary>
        /// Gets the formatted uptime as a string.
        /// </summary>
        public string FormattedUptime
        {
            get
            {
                return $"{Uptime.Days} days, {Uptime.Hours} hours, {Uptime.Minutes} minutes, {Uptime.Seconds} seconds";
            }
        }

        /// <summary>
        /// Gets the formatted total physical memory.
        /// </summary>
        public string FormattedTotalPhysicalMemory
        {
            get
            {
                if (TotalPhysicalMemory < 1024)
                    return $"{TotalPhysicalMemory} B";
                else if (TotalPhysicalMemory < 1024 * 1024)
                    return $"{TotalPhysicalMemory / 1024.0:F2} KB";
                else if (TotalPhysicalMemory < 1024 * 1024 * 1024)
                    return $"{TotalPhysicalMemory / (1024.0 * 1024.0):F2} MB";
                else if (TotalPhysicalMemory < 1024L * 1024L * 1024L * 1024L)
                    return $"{TotalPhysicalMemory / (1024.0 * 1024.0 * 1024.0):F2} GB";
                else
                    return $"{TotalPhysicalMemory / (1024.0 * 1024.0 * 1024.0 * 1024.0):F2} TB";
            }
        }

        /// <summary>
        /// Gets the formatted available physical memory.
        /// </summary>
        public string FormattedAvailablePhysicalMemory
        {
            get
            {
                if (AvailablePhysicalMemory < 1024)
                    return $"{AvailablePhysicalMemory} B";
                else if (AvailablePhysicalMemory < 1024 * 1024)
                    return $"{AvailablePhysicalMemory / 1024.0:F2} KB";
                else if (AvailablePhysicalMemory < 1024 * 1024 * 1024)
                    return $"{AvailablePhysicalMemory / (1024.0 * 1024.0):F2} MB";
                else if (AvailablePhysicalMemory < 1024L * 1024L * 1024L * 1024L)
                    return $"{AvailablePhysicalMemory / (1024.0 * 1024.0 * 1024.0):F2} GB";
                else
                    return $"{AvailablePhysicalMemory / (1024.0 * 1024.0 * 1024.0 * 1024.0):F2} TB";
            }
        }

        /// <summary>
        /// Gets the percentage of memory used.
        /// </summary>
        public double MemoryUsagePercentage
        {
            get
            {
                if (TotalPhysicalMemory == 0)
                    return 0;
                return (double)(TotalPhysicalMemory - AvailablePhysicalMemory) / TotalPhysicalMemory * 100;
            }
        }

        /// <summary>
        /// Returns a string representation of the system information.
        /// </summary>
        /// <returns>A string representation of the system information.</returns>
        public override string ToString()
        {
            string result = $"Computer Name: {ComputerName}\n";
            result += $"Operating System: {OperatingSystem}\n";
            result += $"Processor: {ProcessorInfo}\n";
            result += $"Total Memory: {FormattedTotalPhysicalMemory}\n";
            result += $"Available Memory: {FormattedAvailablePhysicalMemory} ({100 - MemoryUsagePercentage:F2}% free)\n";
            result += $"Installation Date: {InstallDate:yyyy-MM-dd}\n";
            result += $"System Age: {SystemAge}\n";
            result += $"Uptime: {FormattedUptime}\n";
            
            return result;
        }
    }
}
