using System;
using System.IO;
using System.Text;
using System.Security;
using System.Security.Cryptography;

namespace scanningTool.Helpers
{
    /// <summary>
    /// Helper class for security-related operations.
    /// </summary>
    public static class SecurityHelper
    {
        private static readonly byte[] _entropy = new byte[] { 0x43, 0x87, 0x23, 0x72, 0x45, 0x56, 0x68, 0x14, 0x62, 0x84 };
        
        /// <summary>
        /// Encrypts sensitive data like API keys.
        /// </summary>
        /// <param name="plainText">The plain text to encrypt.</param>
        /// <returns>The encrypted data as a Base64 string.</returns>
        public static string EncryptString(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;
                
            try
            {
                // Convert the plaintext string to a byte array
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                
                // Encrypt the data
                byte[] encryptedBytes = ProtectedData.Protect(
                    plainTextBytes, 
                    _entropy,
                    DataProtectionScope.CurrentUser);
                    
                // Return as base64 string
                return Convert.ToBase64String(encryptedBytes);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error encrypting string");
                return string.Empty;
            }
        }
        
        /// <summary>
        /// Decrypts sensitive data like API keys.
        /// </summary>
        /// <param name="encryptedText">The encrypted data as a Base64 string.</param>
        /// <returns>The decrypted plain text.</returns>
        public static string DecryptString(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return string.Empty;
                
            try
            {
                // Convert the encrypted base64 string to a byte array
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                
                // Decrypt the data
                byte[] plainTextBytes = ProtectedData.Unprotect(
                    encryptedBytes,
                    _entropy,
                    DataProtectionScope.CurrentUser);
                    
                // Return the decrypted string
                return Encoding.UTF8.GetString(plainTextBytes);
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error decrypting string");
                return string.Empty;
            }
        }
        
        /// <summary>
        /// Validates if a file path is safe to access.
        /// </summary>
        /// <param name="filePath">The file path to validate.</param>
        /// <returns>True if the file path is safe, false otherwise.</returns>
        public static bool IsFilePathSafe(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;
                
            try
            {
                // Check if the path is absolute
                if (!Path.IsPathRooted(filePath))
                    return false;
                    
                // Check if the file exists
                if (!File.Exists(filePath))
                    return false;
                    
                // Additional security checks can be added here
                
                return true;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error validating file path");
                return false;
            }
        }
        
        /// <summary>
        /// Validates if a directory path is safe to access.
        /// </summary>
        /// <param name="directoryPath">The directory path to validate.</param>
        /// <returns>True if the directory path is safe, false otherwise.</returns>
        public static bool IsDirectoryPathSafe(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
                return false;
                
            try
            {
                // Check if the path is absolute
                if (!Path.IsPathRooted(directoryPath))
                    return false;
                    
                // Check if the directory exists
                if (!Directory.Exists(directoryPath))
                {
                    // Try to create the directory
                    Directory.CreateDirectory(directoryPath);
                }
                
                // Additional security checks can be added here
                
                return true;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error validating directory path");
                return false;
            }
        }
        
        /// <summary>
        /// Sanitizes a file name to remove invalid characters.
        /// </summary>
        /// <param name="fileName">The file name to sanitize.</param>
        /// <returns>The sanitized file name.</returns>
        public static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return "unnamed";
                
            // Remove invalid characters
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
            
            // Ensure the file name is not empty
            if (string.IsNullOrEmpty(sanitized))
                return "unnamed";
                
            return sanitized;
        }
    }
}
