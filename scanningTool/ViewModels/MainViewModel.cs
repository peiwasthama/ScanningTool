using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using scanningTool.Helpers;
using scanningTool.Models;
using scanningTool.Services;

namespace scanningTool.ViewModels
{
    /// <summary>
    /// ViewModel for the main form of the application.
    /// Implements INotifyPropertyChanged for UI binding.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IDiskService _diskService;
        private readonly INetworkService _networkService;
        private readonly ISystemService _systemService;
        private readonly IAIService _aiService;
        
        private string _outputText = string.Empty;
        private bool _isBusy = false;
        private string _aiAnalysisResult = string.Empty;
        private bool _isAnalyzing = false;
        
        /// <summary>
        /// Event that is fired when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// Gets or sets the output text displayed in the UI.
        /// </summary>
        public string OutputText
        {
            get => _outputText;
            set
            {
                if (_outputText != value)
                {
                    _outputText = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets whether the application is busy processing a request.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the AI analysis result.
        /// </summary>
        public string AIAnalysisResult
        {
            get => _aiAnalysisResult;
            set
            {
                if (_aiAnalysisResult != value)
                {
                    _aiAnalysisResult = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets whether the AI is analyzing.
        /// </summary>
        public bool IsAnalyzing
        {
            get => _isAnalyzing;
            set
            {
                if (_isAnalyzing != value)
                {
                    _isAnalyzing = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            // Initialize services
            _aiService = new AIService();
            _diskService = new DiskService();
            _networkService = new NetworkService();
            _systemService = new SystemService(_aiService);
            
            // Log application start
            LoggingHelper.LogInfo("Application started");
        }
        
        /// <summary>
        /// Checks disk health and updates the output text.
        /// </summary>
        public async Task CheckDiskHealthAsync()
        {
            IsBusy = true;
            OutputText = "Checking Hard Disk Health...\n";
            AIAnalysisResult = string.Empty;
            
            try
            {
                var disks = await _diskService.GetDiskHealthInfoAsync();
                
                foreach (var disk in disks)
                {
                    AppendOutput($"\n{disk}\n");
                }
                
                if (disks.Count == 0)
                {
                    AppendOutput("\nNo disks found.\n");
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error in CheckDiskHealthAsync");
                AppendOutput($"\nError checking hard disk health: {ex.Message}\n");
                ShowErrorMessage("Error checking disk health", ex);
                
                // Analyze error with AI
                await AnalyzeErrorWithAIAsync(ex.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        /// <summary>
        /// Finds PC age and updates the output text.
        /// </summary>
        public async Task FindPcAgeAsync()
        {
            IsBusy = true;
            OutputText = "Finding PC Age...\n";
            AIAnalysisResult = string.Empty;
            
            try
            {
                var systemInfo = await _systemService.GetSystemInfoAsync();
                
                AppendOutput($"\nComputer Name: {systemInfo.ComputerName}\n");
                AppendOutput($"Operating System: {systemInfo.OperatingSystem}\n");
                AppendOutput($"Installation Date: {systemInfo.InstallDate:yyyy-MM-dd}\n");
                AppendOutput($"System Age: {systemInfo.SystemAge}\n");
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error in FindPcAgeAsync");
                AppendOutput($"\nError finding PC age: {ex.Message}\n");
                ShowErrorMessage("Error finding PC age", ex);
                
                // Analyze error with AI
                await AnalyzeErrorWithAIAsync(ex.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        /// <summary>
        /// Checks system errors and updates the output text.
        /// </summary>
        public async Task CheckSystemErrorsAsync()
        {
            IsBusy = true;
            OutputText = "Checking System Errors...\n";
            AIAnalysisResult = string.Empty;
            
            try
            {
                int maxEvents = ConfigurationHelper.GetMaxEvents();
                var errors = await _systemService.GetSystemErrorsAsync(maxEvents);
                
                AppendOutput($"\nFound {errors.Count} system errors:\n");
                
                foreach (var error in errors)
                {
                    AppendOutput($"\n{error}\n");
                }
                
                if (errors.Count == 0)
                {
                    AppendOutput("\nNo system errors found.\n");
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error in CheckSystemErrorsAsync");
                AppendOutput($"\nError checking system errors: {ex.Message}\n");
                ShowErrorMessage("Error checking system errors", ex);
                
                // Analyze error with AI
                await AnalyzeErrorWithAIAsync(ex.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        /// <summary>
        /// Finds system warnings and updates the output text.
        /// </summary>
        public async Task FindWarningsAsync()
        {
            IsBusy = true;
            OutputText = "Finding System Warnings...\n";
            AIAnalysisResult = string.Empty;
            
            try
            {
                int maxEvents = ConfigurationHelper.GetMaxEvents();
                var warnings = await _systemService.GetSystemWarningsAsync(maxEvents);
                
                AppendOutput($"\nFound {warnings.Count} system warnings:\n");
                
                foreach (var warning in warnings)
                {
                    AppendOutput($"\n{warning}\n");
                }
                
                if (warnings.Count == 0)
                {
                    AppendOutput("\nNo system warnings found.\n");
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error in FindWarningsAsync");
                AppendOutput($"\nError finding system warnings: {ex.Message}\n");
                ShowErrorMessage("Error finding system warnings", ex);
                
                // Analyze error with AI
                await AnalyzeErrorWithAIAsync(ex.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        /// <summary>
        /// Finds network information and updates the output text.
        /// </summary>
        public async Task FindNetworkInfoAsync()
        {
            IsBusy = true;
            OutputText = "Finding Network Information...\n";
            AIAnalysisResult = string.Empty;
            
            try
            {
                var interfaces = await _networkService.GetNetworkInterfacesAsync();
                
                AppendOutput($"\nFound {interfaces.Count} network interfaces:\n");
                
                foreach (var networkInterface in interfaces)
                {
                    AppendOutput($"\n{networkInterface}\n");
                }
                
                // Check for down interfaces
                var downInterfaces = _networkService.GetDownInterfaces();
                
                if (downInterfaces.Count > 0)
                {
                    AppendOutput("\nInterfaces Down (Check these):\n");
                    
                    foreach (var downInterface in downInterfaces)
                    {
                        AppendOutput($"   {downInterface}\n");
                    }
                }
                
                if (interfaces.Count == 0)
                {
                    AppendOutput("\nNo network interfaces found.\n");
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error in FindNetworkInfoAsync");
                AppendOutput($"\nError finding network information: {ex.Message}\n");
                ShowErrorMessage("Error finding network information", ex);
                
                // Analyze error with AI
                await AnalyzeErrorWithAIAsync(ex.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        /// <summary>
        /// Creates a disk image and updates the output text.
        /// </summary>
        public async Task CreateDiskImageAsync()
        {
            IsBusy = true;
            OutputText = "Creating disk image... Please wait.\n";
            AIAnalysisResult = string.Empty;
            
            try
            {
                // Get paths from configuration
                string dumpItPath = ConfigurationHelper.GetDumpItPath();
                string outputFolderPath = ConfigurationHelper.GetOutputFolderPath();
                
                // Validate paths
                if (!System.IO.File.Exists(dumpItPath))
                {
                    throw new System.IO.FileNotFoundException($"DumpIt executable not found at {dumpItPath}");
                }
                
                var result = await _diskService.CreateDiskImageAsync(dumpItPath, outputFolderPath);
                
                if (result.Success)
                {
                    AppendOutput($"\nDisk image successfully saved:\n{result.ResultMessage}\n");
                }
                else
                {
                    AppendOutput($"\n{result.ResultMessage}\n");
                    
                    // Analyze error with AI if there was a problem
                    await AnalyzeErrorWithAIAsync(result.ResultMessage);
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error in CreateDiskImageAsync");
                AppendOutput($"\nError creating disk image: {ex.Message}\n");
                ShowErrorMessage("Error creating disk image", ex);
                
                // Analyze error with AI
                await AnalyzeErrorWithAIAsync(ex.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        /// <summary>
        /// Checks system uptime and updates the output text.
        /// </summary>
        public async Task CheckUptimeAsync()
        {
            IsBusy = true;
            OutputText = "Checking System Uptime...\n";
            AIAnalysisResult = string.Empty;
            
            try
            {
                var systemInfo = await _systemService.GetSystemInfoAsync();
                
                AppendOutput($"\nSystem Uptime: {systemInfo.FormattedUptime}\n");
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error in CheckUptimeAsync");
                AppendOutput($"\nError checking system uptime: {ex.Message}\n");
                ShowErrorMessage("Error checking system uptime", ex);
                
                // Analyze error with AI
                await AnalyzeErrorWithAIAsync(ex.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        /// <summary>
        /// Analyzes a system event with AI and updates the AI analysis result.
        /// </summary>
        /// <param name="eventInfo">The system event to analyze.</param>
        public async Task AnalyzeEventWithAIAsync(SystemEventInfo eventInfo)
        {
            if (!_aiService.IsServiceAvailable())
            {
                AIAnalysisResult = "AI analysis not available. Please configure the OpenAI API key in settings.";
                return;
            }
            
            IsAnalyzing = true;
            AIAnalysisResult = "Analyzing with AI...";
            
            try
            {
                string result = await _systemService.AnalyzeEventWithAIAsync(eventInfo);
                AIAnalysisResult = result;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error in AnalyzeEventWithAIAsync");
                AIAnalysisResult = $"Error during AI analysis: {ex.Message}";
            }
            finally
            {
                IsAnalyzing = false;
            }
        }
        
        /// <summary>
        /// Analyzes an error message with AI and updates the AI analysis result.
        /// </summary>
        /// <param name="errorMessage">The error message to analyze.</param>
        public async Task AnalyzeErrorWithAIAsync(string errorMessage)
        {
            if (!_aiService.IsServiceAvailable())
            {
                AIAnalysisResult = "AI analysis not available. Please configure the OpenAI API key in settings.";
                return;
            }
            
            IsAnalyzing = true;
            AIAnalysisResult = "Analyzing error with AI...";
            
            try
            {
                string result = await _systemService.AnalyzeErrorWithAIAsync(errorMessage);
                AIAnalysisResult = result;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error in AnalyzeErrorWithAIAsync");
                AIAnalysisResult = $"Error during AI analysis: {ex.Message}";
            }
            finally
            {
                IsAnalyzing = false;
            }
        }
        
        /// <summary>
        /// Sets the OpenAI API key in configuration.
        /// </summary>
        /// <param name="apiKey">The API key to set.</param>
        public void SetOpenAIApiKey(string apiKey)
        {
            try
            {
                ConfigurationHelper.SetOpenAIApiKey(apiKey);
                LoggingHelper.LogInfo("OpenAI API key updated");
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error setting OpenAI API key");
                ShowErrorMessage("Error setting OpenAI API key", ex);
            }
        }
        
        /// <summary>
        /// Appends text to the output.
        /// </summary>
        /// <param name="text">The text to append.</param>
        private void AppendOutput(string text)
        {
            OutputText += text;
        }
        
        /// <summary>
        /// Shows an error message to the user.
        /// </summary>
        /// <param name="title">The error title.</param>
        /// <param name="ex">The exception that occurred.</param>
        private void ShowErrorMessage(string title, Exception ex)
        {
            // This method will be called from the UI thread
            MessageBox.Show(
                $"An error occurred: {ex.Message}\n\nPlease check the log file for more details.",
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        
        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
