using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Management;
using System.IO;
using System.Diagnostics;
using scanningTool.Models;
using scanningTool.Helpers;

namespace scanningTool.Services
{
    /// <summary>
    /// Implementation of the disk service interface.
    /// </summary>
    public class DiskService : IDiskService
    {
        /// <summary>
        /// Gets disk health information asynchronously.
        /// </summary>
        /// <returns>A list of DiskHealthInfo objects.</returns>
        public async Task<List<DiskHealthInfo>> GetDiskHealthInfoAsync()
        {
            LoggingHelper.LogInfo("Getting disk health information");
            return await Task.Run(() => GetDiskHealthInfo());
        }

        /// <summary>
        /// Gets disk health information synchronously.
        /// </summary>
        /// <returns>A list of DiskHealthInfo objects.</returns>
        private List<DiskHealthInfo> GetDiskHealthInfo()
        {
            List<DiskHealthInfo> disks = new List<DiskHealthInfo>();

            try
            {
                // Get physical disk information
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive"))
                {
                    foreach (ManagementObject disk in searcher.Get())
                    {
                        DiskHealthInfo diskInfo = new DiskHealthInfo
                        {
                            DeviceID = disk["DeviceID"]?.ToString() ?? "Unknown",
                            Model = disk["Model"]?.ToString() ?? "Unknown",
                            InterfaceType = disk["InterfaceType"]?.ToString() ?? "Unknown",
                            Size = disk["Size"] != null ? Convert.ToUInt64(disk["Size"]) : 0,
                            Status = disk["Status"]?.ToString() ?? "Unknown"
                        };

                        // Get SMART data if available
                        try
                        {
                            using (ManagementObjectSearcher smartSearcher = new ManagementObjectSearcher(
                                "SELECT * FROM Win32_DiskDriveToDiskPartition WHERE Antecedent = '" + 
                                disk["DeviceID"].ToString().Replace("\\", "\\\\") + "'"))
                            {
                                foreach (ManagementObject partition in smartSearcher.Get())
                                {
                                    // Add partition information
                                    diskInfo.Partitions.Add(new DiskPartitionInfo
                                    {
                                        Name = partition["Dependent"]?.ToString() ?? "Unknown"
                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LoggingHelper.LogException(ex, "Error getting partition information");
                        }

                        disks.Add(diskInfo);
                    }
                }

                // Get logical disk information
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk WHERE DriveType = 3"))
                {
                    foreach (ManagementObject logicalDisk in searcher.Get())
                    {
                        string deviceID = logicalDisk["DeviceID"]?.ToString() ?? "Unknown";
                        ulong freeSpace = logicalDisk["FreeSpace"] != null ? Convert.ToUInt64(logicalDisk["FreeSpace"]) : 0;
                        ulong size = logicalDisk["Size"] != null ? Convert.ToUInt64(logicalDisk["Size"]) : 0;
                        
                        // Find the physical disk this logical disk belongs to
                        foreach (DiskHealthInfo disk in disks)
                        {
                            foreach (DiskPartitionInfo partition in disk.Partitions)
                            {
                                if (partition.Name.Contains(deviceID))
                                {
                                    partition.DriveLetter = deviceID;
                                    partition.FreeSpace = freeSpace;
                                    partition.Size = size;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error getting disk health information");
                throw new Exception("Failed to retrieve disk health information", ex);
            }

            return disks;
        }

        /// <summary>
        /// Creates a disk image using DumpIt.
        /// </summary>
        /// <param name="dumpItPath">Path to the DumpIt executable.</param>
        /// <param name="outputFolderPath">Path to the output folder.</param>
        /// <returns>A DiskImageResult object containing the result of the operation.</returns>
        public async Task<DiskImageResult> CreateDiskImageAsync(string dumpItPath, string outputFolderPath)
        {
            LoggingHelper.LogInfo($"Creating disk image using DumpIt at {dumpItPath}");
            return await Task.Run(() => CreateDiskImage(dumpItPath, outputFolderPath));
        }

        /// <summary>
        /// Creates a disk image using DumpIt synchronously.
        /// </summary>
        /// <param name="dumpItPath">Path to the DumpIt executable.</param>
        /// <param name="outputFolderPath">Path to the output folder.</param>
        /// <returns>A DiskImageResult object containing the result of the operation.</returns>
        private DiskImageResult CreateDiskImage(string dumpItPath, string outputFolderPath)
        {
            DiskImageResult result = new DiskImageResult();

            try
            {
                // Validate paths
                if (!File.Exists(dumpItPath))
                {
                    result.Success = false;
                    result.ResultMessage = $"DumpIt executable not found at {dumpItPath}";
                    LoggingHelper.LogError(result.ResultMessage);
                    return result;
                }

                // Ensure output directory exists
                if (!Directory.Exists(outputFolderPath))
                {
                    Directory.CreateDirectory(outputFolderPath);
                }

                // Create a unique filename for the output
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string outputFilePath = Path.Combine(outputFolderPath, $"DiskImage_{timestamp}.raw");

                // Create process start info
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = dumpItPath,
                    Arguments = $"/O {outputFilePath} /Q",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                // Start the process
                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.OutputDataReceived += (sender, e) => 
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            LoggingHelper.LogInfo($"DumpIt output: {e.Data}");
                        }
                    };
                    process.ErrorDataReceived += (sender, e) => 
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            LoggingHelper.LogError($"DumpIt error: {e.Data}");
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    // Check the exit code
                    if (process.ExitCode == 0)
                    {
                        result.Success = true;
                        result.ResultMessage = $"Disk image created successfully at {outputFilePath}";
                        LoggingHelper.LogInfo(result.ResultMessage);
                    }
                    else
                    {
                        result.Success = false;
                        result.ResultMessage = $"DumpIt process exited with code {process.ExitCode}";
                        LoggingHelper.LogError(result.ResultMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ResultMessage = $"Error creating disk image: {ex.Message}";
                LoggingHelper.LogException(ex, "Error creating disk image");
            }

            return result;
        }
    }
}
