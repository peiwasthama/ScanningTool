using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using scanningTool.Models;

namespace scanningTool.Services
{
    /// <summary>
    /// Interface for disk service operations.
    /// </summary>
    public interface IDiskService
    {
        /// <summary>
        /// Gets disk health information asynchronously.
        /// </summary>
        /// <returns>A list of DiskHealthInfo objects.</returns>
        Task<List<DiskHealthInfo>> GetDiskHealthInfoAsync();

        /// <summary>
        /// Creates a disk image using DumpIt.
        /// </summary>
        /// <param name="dumpItPath">Path to the DumpIt executable.</param>
        /// <param name="outputFolderPath">Path to the output folder.</param>
        /// <returns>A DiskImageResult object containing the result of the operation.</returns>
        Task<DiskImageResult> CreateDiskImageAsync(string dumpItPath, string outputFolderPath);
    }
}
