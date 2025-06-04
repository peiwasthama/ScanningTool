# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-06-04

### Added
- Completely refactored codebase with layered architecture
- AI-driven error analysis with OpenAI integration
- Configuration support via appSettings.json
- Secure storage of API keys with encryption
- Comprehensive logging and error handling
- Unit tests for all services
- Detailed documentation and user guide
- Settings interface for configuration
- GitHub portfolio files (README, LICENSE, CONTRIBUTING, etc.)

### Changed
- Improved user interface with clearer feedback
- Optimized performance for disk analysis and system error retrieval
- Enhanced security with validation of all external calls
- Updated to .NET 8.0 and C# 10.0
- Improved error handling with detailed error messages

### Fixed
- Removed hardcoded paths and configuration parameters
- Resolved memory leaks during extended use
- Fixed UI freezing during heavy operations with asynchronous programming
- Improved handling of down network interfaces
- Fixed issues with disk imaging on systems with limited access

## [0.9.0] - 2025-05-26

### Added
- Initial version of ScanningTool
- Basic disk health analysis
- System error retrieval
- Network information
- Disk imaging with DumpIt
