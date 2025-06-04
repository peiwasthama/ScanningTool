using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
using scanningTool.Helpers;
using scanningTool.ViewModels;
using scanningTool.Models;

namespace scanningTool.Views
{
    /// <summary>
    /// Main form of the scanning tool application.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly MainViewModel _viewModel;
        private SystemEventInfo _selectedEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            
            // Initialize ViewModel
            _viewModel = new MainViewModel();
            
            // Bind ViewModel properties to UI
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            
            // Log form initialization
            LoggingHelper.LogInfo("Main form initialized");
        }

        /// <summary>
        /// Handles property changed events from the ViewModel.
        /// </summary>
        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.OutputText))
            {
                textBoxOutput.Text = _viewModel.OutputText;
                textBoxOutput.SelectionStart = textBoxOutput.Text.Length;
                textBoxOutput.ScrollToCaret();
            }
            else if (e.PropertyName == nameof(MainViewModel.IsBusy))
            {
                UpdateButtonsEnabled(!_viewModel.IsBusy);
            }
            else if (e.PropertyName == nameof(MainViewModel.AIAnalysisResult))
            {
                textBoxAIAnalysis.Text = _viewModel.AIAnalysisResult;
                textBoxAIAnalysis.SelectionStart = textBoxAIAnalysis.Text.Length;
                textBoxAIAnalysis.ScrollToCaret();
            }
            else if (e.PropertyName == nameof(MainViewModel.IsAnalyzing))
            {
                btnAnalyzeWithAI.Enabled = !_viewModel.IsAnalyzing;
            }
        }

        /// <summary>
        /// Updates the enabled state of all buttons.
        /// </summary>
        /// <param name="enabled">Whether the buttons should be enabled.</param>
        private void UpdateButtonsEnabled(bool enabled)
        {
            btnCheckDiskHealth.Enabled = enabled;
            btnFindPcAge.Enabled = enabled;
            btnCheckSystemErrors.Enabled = enabled;
            btnFindWarnings.Enabled = enabled;
            btnFindNetworkInfo.Enabled = enabled;
            btnDiskImage.Enabled = enabled;
            btnCheckUptime.Enabled = enabled;
            btnAnalyzeWithAI.Enabled = enabled && _selectedEvent != null;
            btnSetApiKey.Enabled = enabled;
        }

        /// <summary>
        /// Handles the Click event of the Check Disk Health button.
        /// </summary>
        private async void btnCheckDiskHealth_Click(object sender, EventArgs e)
        {
            try
            {
                await _viewModel.CheckDiskHealthAsync();
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Unhandled exception in btnCheckDiskHealth_Click");
                MessageBox.Show(
                    $"An unexpected error occurred: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the Find PC Age button.
        /// </summary>
        private async void btnFindPcAge_Click(object sender, EventArgs e)
        {
            try
            {
                await _viewModel.FindPcAgeAsync();
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Unhandled exception in btnFindPcAge_Click");
                MessageBox.Show(
                    $"An unexpected error occurred: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the Check System Errors button.
        /// </summary>
        private async void btnCheckSystemErrors_Click(object sender, EventArgs e)
        {
            try
            {
                await _viewModel.CheckSystemErrorsAsync();
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Unhandled exception in btnCheckSystemErrors_Click");
                MessageBox.Show(
                    $"An unexpected error occurred: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the Find Warnings button.
        /// </summary>
        private async void btnFindWarnings_Click(object sender, EventArgs e)
        {
            try
            {
                await _viewModel.FindWarningsAsync();
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Unhandled exception in btnFindWarnings_Click");
                MessageBox.Show(
                    $"An unexpected error occurred: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the Find Network Info button.
        /// </summary>
        private async void btnFindNetworkInfo_Click(object sender, EventArgs e)
        {
            try
            {
                await _viewModel.FindNetworkInfoAsync();
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Unhandled exception in btnFindNetworkInfo_Click");
                MessageBox.Show(
                    $"An unexpected error occurred: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the Disk Image button.
        /// </summary>
        private async void btnDiskImage_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if DumpIt.exe exists
                string dumpItPath = ConfigurationHelper.GetDumpItPath();
                if (!File.Exists(dumpItPath))
                {
                    MessageBox.Show(
                        $"DumpIt.exe not found at {dumpItPath}. Please ensure the file exists.",
                        "File Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Confirm operation with user
                var result = MessageBox.Show(
                    "This operation will create a disk image and requires administrator privileges. Continue?",
                    "Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    await _viewModel.CreateDiskImageAsync();
                }
                else
                {
                    LoggingHelper.LogInfo("Disk image creation cancelled by user");
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Unhandled exception in btnDiskImage_Click");
                MessageBox.Show(
                    $"An unexpected error occurred: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the Check Uptime button.
        /// </summary>
        private async void btnCheckUptime_Click(object sender, EventArgs e)
        {
            try
            {
                await _viewModel.CheckUptimeAsync();
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Unhandled exception in btnCheckUptime_Click");
                MessageBox.Show(
                    $"An unexpected error occurred: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the Analyze with AI button.
        /// </summary>
        private async void btnAnalyzeWithAI_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedEvent != null)
                {
                    await _viewModel.AnalyzeEventWithAIAsync(_selectedEvent);
                }
                else
                {
                    MessageBox.Show(
                        "Please select an event to analyze.",
                        "No Event Selected",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Unhandled exception in btnAnalyzeWithAI_Click");
                MessageBox.Show(
                    $"An unexpected error occurred: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the Set API Key button.
        /// </summary>
        private void btnSetApiKey_Click(object sender, EventArgs e)
        {
            try
            {
                string apiKey = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter your OpenAI API Key:",
                    "API Key Configuration",
                    "",
                    -1,
                    -1);

                if (!string.IsNullOrWhiteSpace(apiKey))
                {
                    _viewModel.SetOpenAIApiKey(apiKey);
                    MessageBox.Show(
                        "API key has been saved. You may need to restart the application for changes to take effect.",
                        "API Key Saved",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Unhandled exception in btnSetApiKey_Click");
                MessageBox.Show(
                    $"An unexpected error occurred: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Creates a button with the specified text and position.
        /// </summary>
        /// <param name="text">The button text.</param>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>A new Button instance.</returns>
        private Button CreateButton(string text, int x, int y)
        {
            Button button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(250, 40),
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            
            button.FlatAppearance.BorderColor = Color.FromArgb(67, 67, 70);
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 64);
            
            Controls.Add(button);
            return button;
        }

        /// <summary>
        /// Initializes the form components.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            AutoScaleMode = AutoScaleMode.Font;
            Text = "Scanning Tool";
            ClientSize = new Size(1000, 600);
            BackColor = Color.FromArgb(37, 37, 38);
            
            // Create main layout panel
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 280));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            
            Controls.Add(mainLayout);
            
            // Create button panel
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30)
            };
            
            mainLayout.Controls.Add(buttonPanel, 0, 0);
            
            // Create output panel
            TableLayoutPanel outputPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            
            outputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            outputPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            outputPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            
            mainLayout.Controls.Add(outputPanel, 1, 0);
            
            // Create output text box
            textBoxOutput = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.LightGreen,
                Font = new Font("Consolas", 10F),
                ReadOnly = true
            };
            
            outputPanel.Controls.Add(textBoxOutput, 0, 0);
            
            // Create AI analysis panel
            Panel aiPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 5, 0, 0)
            };
            
            outputPanel.Controls.Add(aiPanel, 0, 1);
            
            // Create AI analysis label
            Label aiLabel = new Label
            {
                Text = "AI Analysis",
                Dock = DockStyle.Top,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Height = 25
            };
            
            aiPanel.Controls.Add(aiLabel);
            
            // Create AI analysis text box
            textBoxAIAnalysis = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.LightCyan,
                Font = new Font("Consolas", 10F),
                ReadOnly = true
            };
            
            aiPanel.Controls.Add(textBoxAIAnalysis);
            
            // Create buttons
            btnCheckDiskHealth = CreateButton("Check Disk Health", 15, 20);
            btnFindPcAge = CreateButton("Find PC Age", 15, 70);
            btnCheckSystemErrors = CreateButton("Check System Errors", 15, 120);
            btnFindWarnings = CreateButton("Find Warnings", 15, 170);
            btnFindNetworkInfo = CreateButton("Find Network Info", 15, 220);
            btnDiskImage = CreateButton("Create Disk Image", 15, 270);
            btnCheckUptime = CreateButton("Check Uptime", 15, 320);
            btnAnalyzeWithAI = CreateButton("Analyze with AI", 15, 390);
            btnSetApiKey = CreateButton("Set OpenAI API Key", 15, 440);
            
            // Add buttons to panel
            buttonPanel.Controls.Add(btnCheckDiskHealth);
            buttonPanel.Controls.Add(btnFindPcAge);
            buttonPanel.Controls.Add(btnCheckSystemErrors);
            buttonPanel.Controls.Add(btnFindWarnings);
            buttonPanel.Controls.Add(btnFindNetworkInfo);
            buttonPanel.Controls.Add(btnDiskImage);
            buttonPanel.Controls.Add(btnCheckUptime);
            buttonPanel.Controls.Add(btnAnalyzeWithAI);
            buttonPanel.Controls.Add(btnSetApiKey);
            
            // Wire up event handlers
            btnCheckDiskHealth.Click += btnCheckDiskHealth_Click;
            btnFindPcAge.Click += btnFindPcAge_Click;
            btnCheckSystemErrors.Click += btnCheckSystemErrors_Click;
            btnFindWarnings.Click += btnFindWarnings_Click;
            btnFindNetworkInfo.Click += btnFindNetworkInfo_Click;
            btnDiskImage.Click += btnDiskImage_Click;
            btnCheckUptime.Click += btnCheckUptime_Click;
            btnAnalyzeWithAI.Click += btnAnalyzeWithAI_Click;
            btnSetApiKey.Click += btnSetApiKey_Click;
            
            // Initially disable the Analyze with AI button until an event is selected
            btnAnalyzeWithAI.Enabled = false;
        }

        private System.ComponentModel.IContainer components = null;
        private TextBox textBoxOutput;
        private TextBox textBoxAIAnalysis;
        private Button btnCheckDiskHealth;
        private Button btnFindPcAge;
        private Button btnCheckSystemErrors;
        private Button btnFindWarnings;
        private Button btnFindNetworkInfo;
        private Button btnDiskImage;
        private Button btnCheckUptime;
        private Button btnAnalyzeWithAI;
        private Button btnSetApiKey;
        
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
