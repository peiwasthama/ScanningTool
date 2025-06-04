using System;
using System.Collections.Generic;

namespace scanningTool.Models
{
    /// <summary>
    /// Class representing disk health information.
    /// </summary>
    public class DiskHealthInfo
    {
        /// <summary>
        /// Gets or sets the device ID.
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the interface type.
        /// </summary>
        public string InterfaceType { get; set; }

        /// <summary>
        /// Gets or sets the size in bytes.
        /// </summary>
        public ulong Size { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the list of partitions.
        /// </summary>
        public List<DiskPartitionInfo> Partitions { get; set; } = new List<DiskPartitionInfo>();

        /// <summary>
        /// Gets the formatted size.
        /// </summary>
        public string FormattedSize
        {
            get
            {
                if (Size < 1024)
                    return $"{Size} B";
                else if (Size < 1024 * 1024)
                    return $"{Size / 1024.0:F2} KB";
                else if (Size < 1024 * 1024 * 1024)
                    return $"{Size / (1024.0 * 1024.0):F2} MB";
                else if (Size < 1024L * 1024L * 1024L * 1024L)
                    return $"{Size / (1024.0 * 1024.0 * 1024.0):F2} GB";
                else
                    return $"{Size / (1024.0 * 1024.0 * 1024.0 * 1024.0):F2} TB";
            }
        }

        /// <summary>
        /// Returns a string representation of the disk health information.
        /// </summary>
        /// <returns>A string representation of the disk health information.</returns>
        public override string ToString()
        {
            string result = $"Disk: {Model}\n";
            result += $"Device ID: {DeviceID}\n";
            result += $"Interface: {InterfaceType}\n";
            result += $"Size: {FormattedSize}\n";
            result += $"Status: {Status}\n";
            
            if (Partitions.Count > 0)
            {
                result += "Partitions:\n";
                foreach (var partition in Partitions)
                {
                    result += $"  {partition}\n";
                }
            }
            
            return result;
        }
    }

    /// <summary>
    /// Class representing disk partition information.
    /// </summary>
    public class DiskPartitionInfo
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the drive letter.
        /// </summary>
        public string DriveLetter { get; set; }

        /// <summary>
        /// Gets or sets the size in bytes.
        /// </summary>
        public ulong Size { get; set; }

        /// <summary>
        /// Gets or sets the free space in bytes.
        /// </summary>
        public ulong FreeSpace { get; set; }

        /// <summary>
        /// Gets the formatted size.
        /// </summary>
        public string FormattedSize
        {
            get
            {
                if (Size < 1024)
                    return $"{Size} B";
                else if (Size < 1024 * 1024)
                    return $"{Size / 1024.0:F2} KB";
                else if (Size < 1024 * 1024 * 1024)
                    return $"{Size / (1024.0 * 1024.0):F2} MB";
                else if (Size < 1024L * 1024L * 1024L * 1024L)
                    return $"{Size / (1024.0 * 1024.0 * 1024.0):F2} GB";
                else
                    return $"{Size / (1024.0 * 1024.0 * 1024.0 * 1024.0):F2} TB";
            }
        }

        /// <summary>
        /// Gets the formatted free space.
        /// </summary>
        public string FormattedFreeSpace
        {
            get
            {
                if (FreeSpace < 1024)
                    return $"{FreeSpace} B";
                else if (FreeSpace < 1024 * 1024)
                    return $"{FreeSpace / 1024.0:F2} KB";
                else if (FreeSpace < 1024 * 1024 * 1024)
                    return $"{FreeSpace / (1024.0 * 1024.0):F2} MB";
                else if (FreeSpace < 1024L * 1024L * 1024L * 1024L)
                    return $"{FreeSpace / (1024.0 * 1024.0 * 1024.0):F2} GB";
                else
                    return $"{FreeSpace / (1024.0 * 1024.0 * 1024.0 * 1024.0):F2} TB";
            }
        }

        /// <summary>
        /// Gets the percentage of free space.
        /// </summary>
        public double FreeSpacePercentage
        {
            get
            {
                if (Size == 0)
                    return 0;
                return (double)FreeSpace / Size * 100;
            }
        }

        /// <summary>
        /// Returns a string representation of the disk partition information.
        /// </summary>
        /// <returns>A string representation of the disk partition information.</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(DriveLetter))
                return $"Partition: {Name}";
            
            return $"Drive {DriveLetter} - Size: {FormattedSize}, Free: {FormattedFreeSpace} ({FreeSpacePercentage:F2}%)";
        }
    }

    /// <summary>
    /// Class representing the result of a disk image operation.
    /// </summary>
    public class DiskImageResult
    {
        /// <summary>
        /// Gets or sets whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the result message.
        /// </summary>
        public string ResultMessage { get; set; }
    }
}
