using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using scanningTool.Helpers;
using scanningTool.Services;

namespace scanningTool.Views
{
    /// <summary>
    /// Form for configuring application settings.
    /// </summary>
    public class SettingsForm : Form
    {
        private readonly ConfigurationHelper _configHelper;
        private readonly IAIService _aiService;
        private TextBox txtDumpItPath;
        private TextBox txtOutputPath;
        private TextBox txtMaxEvents;
        private TextBox txtApiKey;
        private Button btnSave;
        private Button btnCancel;
        private Button btnBrowseDumpIt;
        private Button btnBrowseOutput;
        private CheckBox chkEncryptApiKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsForm"/> class.
        /// </summary>
        /// <param name="aiService">The AI service.</param>
        public SettingsForm(IAIService aiService = null)
        {
            _aiService = aiService ?? new AIService();
            InitializeComponent();
            LoadSettings();
        }

        /// <summary>
        /// Loads the current settings into the form.
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                var settings = ConfigurationHelper.GetAppSettings();
                txtDumpItPath.Text = settings.DumpItPath;
                txtOutputPath.Text = settings.OutputFolderPath;
                txtMaxEvents.Text = settings.MaxEvents.ToString();
                
                // Only show the last 4 characters of the API key if it exists
                if (!string.IsNullOrEmpty(settings.OpenAIApiKey))
                {
                    string apiKey = settings.OpenAIApiKey;
                    if (apiKey.Length > 4)
                    {
                        txtApiKey.Text = new string('*', apiKey.Length - 4) + apiKey.Substring(apiKey.Length - 4);
                    }
                    else
                    {
                        txtApiKey.Text = apiKey;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error loading settings");
                MessageBox.Show(
                    "Error loading settings. Default values will be used.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Saves the settings from the form.
        /// </summary>
        private async Task SaveSettingsAsync()
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(txtDumpItPath.Text))
                {
                    MessageBox.Show(
                        "DumpIt path cannot be empty.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtOutputPath.Text))
                {
                    MessageBox.Show(
                        "Output path cannot be empty.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (!int.TryParse(txtMaxEvents.Text, out int maxEvents) || maxEvents <= 0)
                {
                    MessageBox.Show(
                        "Max events must be a positive number.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Get current settings
                var settings = ConfigurationHelper.GetAppSettings();
                
                // Update settings
                settings.DumpItPath = txtDumpItPath.Text;
                settings.OutputFolderPath = txtOutputPath.Text;
                settings.MaxEvents = maxEvents;
                
                // Only update API key if it has been changed (not all asterisks)
                if (!string.IsNullOrWhiteSpace(txtApiKey.Text) && !txtApiKey.Text.StartsWith("*"))
                {
                    settings.OpenAIApiKey = txtApiKey.Text;
                }

                // Save settings
                ConfigurationHelper.SaveAppSettings(settings);
                
                // Test AI service if API key was provided
                if (!string.IsNullOrWhiteSpace(settings.OpenAIApiKey))
                {
                    this.Cursor = Cursors.WaitCursor;
                    
                    try
                    {
                        // Simple test to verify the API key works
                        string testResult = await _aiService.AnalyzeErrorAsync("This is a test message to verify the API key works.");
                        
                        if (!string.IsNullOrEmpty(testResult) && !testResult.Contains("API key"))
                        {
                            MessageBox.Show(
                                "Settings saved successfully and AI service is working.",
                                "Success",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(
                                "Settings saved but there may be an issue with the API key. Please verify it is correct.",
                                "Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggingHelper.LogException(ex, "Error testing AI service");
                        MessageBox.Show(
                            "Settings saved but there was an error testing the AI service. Please verify your API key.",
                            "Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Settings saved successfully. Note that AI analysis features will not be available without an OpenAI API key.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error saving settings");
                MessageBox.Show(
                    $"Error saving settings: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Initializes the form components.
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = "Settings";
            this.Size = new System.Drawing.Size(500, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = System.Drawing.Color.FromArgb(37, 37, 38);
            this.ForeColor = System.Drawing.Color.White;

            // Create labels
            Label lblDumpItPath = new Label
            {
                Text = "DumpIt Path:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(100, 20),
                ForeColor = System.Drawing.Color.White
            };

            Label lblOutputPath = new Label
            {
                Text = "Output Path:",
                Location = new System.Drawing.Point(20, 70),
                Size = new System.Drawing.Size(100, 20),
                ForeColor = System.Drawing.Color.White
            };

            Label lblMaxEvents = new Label
            {
                Text = "Max Events:",
                Location = new System.Drawing.Point(20, 120),
                Size = new System.Drawing.Size(100, 20),
                ForeColor = System.Drawing.Color.White
            };

            Label lblApiKey = new Label
            {
                Text = "OpenAI API Key:",
                Location = new System.Drawing.Point(20, 170),
                Size = new System.Drawing.Size(100, 20),
                ForeColor = System.Drawing.Color.White
            };

            // Create text boxes
            txtDumpItPath = new TextBox
            {
                Location = new System.Drawing.Point(130, 20),
                Size = new System.Drawing.Size(250, 20),
                BackColor = System.Drawing.Color.FromArgb(30, 30, 30),
                ForeColor = System.Drawing.Color.White
            };

            txtOutputPath = new TextBox
            {
                Location = new System.Drawing.Point(130, 70),
                Size = new System.Drawing.Size(250, 20),
                BackColor = System.Drawing.Color.FromArgb(30, 30, 30),
                ForeColor = System.Drawing.Color.White
            };

            txtMaxEvents = new TextBox
            {
                Location = new System.Drawing.Point(130, 120),
                Size = new System.Drawing.Size(100, 20),
                BackColor = System.Drawing.Color.FromArgb(30, 30, 30),
                ForeColor = System.Drawing.Color.White
            };

            txtApiKey = new TextBox
            {
                Location = new System.Drawing.Point(130, 170),
                Size = new System.Drawing.Size(250, 20),
                BackColor = System.Drawing.Color.FromArgb(30, 30, 30),
                ForeColor = System.Drawing.Color.White,
                PasswordChar = '*'
            };

            // Create browse buttons
            btnBrowseDumpIt = new Button
            {
                Text = "...",
                Location = new System.Drawing.Point(390, 20),
                Size = new System.Drawing.Size(30, 23),
                BackColor = System.Drawing.Color.FromArgb(45, 45, 48),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBrowseDumpIt.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(67, 67, 70);
            btnBrowseDumpIt.Click += BtnBrowseDumpIt_Click;

            btnBrowseOutput = new Button
            {
                Text = "...",
                Location = new System.Drawing.Point(390, 70),
                Size = new System.Drawing.Size(30, 23),
                BackColor = System.Drawing.Color.FromArgb(45, 45, 48),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBrowseOutput.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(67, 67, 70);
            btnBrowseOutput.Click += BtnBrowseOutput_Click;

            // Create checkbox for API key encryption
            chkEncryptApiKey = new CheckBox
            {
                Text = "Encrypt API Key",
                Location = new System.Drawing.Point(130, 200),
                Size = new System.Drawing.Size(150, 20),
                Checked = true,
                ForeColor = System.Drawing.Color.White
            };

            // Create buttons
            btnSave = new Button
            {
                Text = "Save",
                Location = new System.Drawing.Point(130, 250),
                Size = new System.Drawing.Size(100, 30),
                BackColor = System.Drawing.Color.FromArgb(45, 45, 48),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(67, 67, 70);
            btnSave.Click += async (sender, e) => await SaveSettingsAsync();

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new System.Drawing.Point(250, 250),
                Size = new System.Drawing.Size(100, 30),
                BackColor = System.Drawing.Color.FromArgb(45, 45, 48),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(67, 67, 70);
            btnCancel.Click += (sender, e) => Close();

            // Add controls to form
            Controls.Add(lblDumpItPath);
            Controls.Add(txtDumpItPath);
            Controls.Add(btnBrowseDumpIt);
            Controls.Add(lblOutputPath);
            Controls.Add(txtOutputPath);
            Controls.Add(btnBrowseOutput);
            Controls.Add(lblMaxEvents);
            Controls.Add(txtMaxEvents);
            Controls.Add(lblApiKey);
            Controls.Add(txtApiKey);
            Controls.Add(chkEncryptApiKey);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);

            // Set accept and cancel buttons
            AcceptButton = btnSave;
            CancelButton = btnCancel;
        }

        /// <summary>
        /// Handles the Click event of the Browse DumpIt button.
        /// </summary>
        private void BtnBrowseDumpIt_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";
                openFileDialog.Title = "Select DumpIt Executable";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtDumpItPath.Text = openFileDialog.FileName;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the Browse Output button.
        /// </summary>
        private void BtnBrowseOutput_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select Output Folder";

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    txtOutputPath.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }
    }
}
