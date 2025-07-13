using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProducerConsumer.Domain.Config;
using ProducerConsumer.Domain.Enum;
using ProducerConsumer.Domain.Events;
using ProducerConsumer.Domain.Interfaces;
using ProducerConsumer.Domain.Models;

namespace ProducerConsumerSolution
{
    /// <summary>
    /// MainForm is the primary UI for the Producer-Consumer demo application.
    /// It allows users to configure, start, and stop the producer-consumer system,
    /// and displays real-time statistics, queue contents, and log output.
    /// </summary>
    public partial class MainForm : Form
    {
        // Business logic dependencies
        private readonly IProducerConsumerManager _manager;
        private readonly ILoggerService _logger;
        private readonly IQueueService _queueService;

        // UI Controls
        private Button btnStart, btnStop, btnClear;
        private NumericUpDown nudProducerDelay, nudConsumerDelay, nudConsumerCount;
        private ListBox lstQueue;
        private RichTextBox rtbLog;
        private Label lblQueueSize, lblProduced, lblConsumed, lblElapsedTime;
        private ProgressBar progressQueue;

        private const int MaxQueueDisplayItems = 20;

        /// <summary>
        /// Initializes the main form and its dependencies.
        /// </summary>
        /// <param name="manager">Producer-consumer manager instance.</param>
        /// <param name="logger">Logger service instance.</param>
        /// <param name="queueService">Queue service instance.</param>
        public MainForm(IProducerConsumerManager manager, ILoggerService logger, IQueueService queueService)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));

            InitializeComponent();
            InitializeUIControls();
            SetupEventHandlers();
            UpdateUI();
        }

        /// <summary>
        /// Sets up and arranges all UI controls on the form.
        /// </summary>
        private void InitializeUIControls()
        {
            // Main layout using TableLayoutPanel for better alignment and resizing
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(10),
                AutoSize = true
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Actions & Config
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Statistics
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Displays

            // --- Top Controls: Actions and Configuration ---
            var topPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Fill,
                AutoSize = true
            };

            btnStart = new Button { Text = "Start", Width = 80 };
            btnStop = new Button { Text = "Stop", Width = 80, Enabled = false };
            btnClear = new Button { Text = "Clear Log", Width = 100 };

            topPanel.Controls.Add(btnStart);
            topPanel.Controls.Add(btnStop);
            topPanel.Controls.Add(btnClear);

            // Configuration group
            var configGroup = new GroupBox
            {
                Text = "Configuration",
                AutoSize = true,
                Padding = new Padding(10)
            };
            var configLayout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 3,
                Dock = DockStyle.Fill,
                AutoSize = true
            };
            configLayout.Controls.Add(new Label { Text = "Producer Delay (ms):", AutoSize = true }, 0, 0);
            nudProducerDelay = new NumericUpDown { Minimum = 10, Maximum = 5000, Value = 500, Increment = 10, Width = 80 };
            configLayout.Controls.Add(nudProducerDelay, 1, 0);

            configLayout.Controls.Add(new Label { Text = "Consumer Delay (ms):", AutoSize = true }, 0, 1);
            nudConsumerDelay = new NumericUpDown { Minimum = 10, Maximum = 5000, Value = 500, Increment = 10, Width = 80 };
            configLayout.Controls.Add(nudConsumerDelay, 1, 1);

            configLayout.Controls.Add(new Label { Text = "Consumer Count:", AutoSize = true }, 0, 2);
            nudConsumerCount = new NumericUpDown { Minimum = 1, Maximum = 10, Value = 2, Width = 80 };
            configLayout.Controls.Add(nudConsumerCount, 1, 2);

            configGroup.Controls.Add(configLayout);
            topPanel.Controls.Add(configGroup);

            mainLayout.Controls.Add(topPanel, 0, 0);
            mainLayout.SetColumnSpan(topPanel, 2);

            // --- Statistics Group ---
            var statsGroup = new GroupBox
            {
                Text = "Statistics",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            var statsLayout = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 4,
                Dock = DockStyle.Fill,
                AutoSize = true
            };
            lblQueueSize = new Label { Text = "Queue Size: 0", AutoSize = true };
            lblProduced = new Label { Text = "Produced: 0", AutoSize = true };
            lblConsumed = new Label { Text = "Consumed: 0", AutoSize = true };
            lblElapsedTime = new Label { Text = "Elapsed: 00:00:00", AutoSize = true };

            statsLayout.Controls.Add(lblQueueSize, 0, 0);
            statsLayout.Controls.Add(lblProduced, 0, 1);
            statsLayout.Controls.Add(lblConsumed, 0, 2);
            statsLayout.Controls.Add(lblElapsedTime, 0, 3);

            statsGroup.Controls.Add(statsLayout);
            mainLayout.Controls.Add(statsGroup, 0, 1);

            // --- Queue and Log Display ---
            var displayLayout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 2,
                Dock = DockStyle.Fill,
                AutoSize = true
            };

            // Labels for displays
            var lblQueue = new Label { Text = "Queue", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, AutoSize = true };
            var lblLog = new Label { Text = "Log Output", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, AutoSize = true };

            displayLayout.Controls.Add(lblQueue, 0, 0);
            displayLayout.Controls.Add(lblLog, 1, 0);

            lstQueue = new ListBox { Dock = DockStyle.Fill, Width = 200, Height = 200 };
            rtbLog = new RichTextBox { Dock = DockStyle.Fill, ReadOnly = true, Width = 300, Height = 200 };

            displayLayout.Controls.Add(lstQueue, 0, 1);
            displayLayout.Controls.Add(rtbLog, 1, 1);

            mainLayout.Controls.Add(displayLayout, 1, 1);

            // --- Progress Bar ---
            progressQueue = new ProgressBar
            {
                Dock = DockStyle.Bottom,
                Height = 20,
                Minimum = 0,
                Maximum = 100
            };
            mainLayout.Controls.Add(progressQueue, 0, 2);
            mainLayout.SetColumnSpan(progressQueue, 2);

            // --- Add main layout to form ---
            Controls.Clear();
            Controls.Add(mainLayout);

            // Tooltips for usability
            var toolTip = new ToolTip();
            toolTip.SetToolTip(btnStart, "Start the producer/consumer system");
            toolTip.SetToolTip(btnStop, "Stop the system and show summary");
            toolTip.SetToolTip(btnClear, "Clear the log area");
            toolTip.SetToolTip(nudProducerDelay, "Delay between produced items (ms)");
            toolTip.SetToolTip(nudConsumerDelay, "Delay between consumed items (ms)");
            toolTip.SetToolTip(nudConsumerCount, "Number of consumers");
        }

        /// <summary>
        /// Wires up all event handlers for UI and business logic events.
        /// </summary>
        private void SetupEventHandlers()
        {
            btnStart.Click += BtnStart_Click;
            btnStop.Click += BtnStop_Click;
            btnClear.Click += BtnClear_Click;

            // Business Logic Event Handlers
            _logger.LogEntryAdded += Logger_LogEntryAdded;
            _manager.QueueStatisticsUpdated += Manager_QueueStatisticsUpdated;
            _manager.ProducerStatisticsUpdated += Manager_ProducerStatisticsUpdated;
            _manager.ConsumerStatisticsUpdated += Manager_ConsumerStatisticsUpdated;
        }

        /// <summary>
        /// Handles the Start button click event.
        /// </summary>
        private async void BtnStart_Click(object sender, EventArgs e)
        {
            await SafeRunAsync(StartSystem);
        }

        /// <summary>
        /// Handles the Stop button click event.
        /// </summary>
        private async void BtnStop_Click(object sender, EventArgs e)
        {
            await SafeRunAsync(StopSystem);
        }

        /// <summary>
        /// Handles the Clear Log button click event.
        /// </summary>
        private void BtnClear_Click(object sender, EventArgs e)
        {
            rtbLog.Clear();
        }

        /// <summary>
        /// Runs an async action safely, disabling controls and logging exceptions.
        /// </summary>
        private async Task SafeRunAsync(Func<Task> action)
        {
            try
            {
                SetControlsEnabled(false);
                await action();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                AddLogEntry($"Exception: {ex.Message}", Color.Red);
            }
            finally
            {
                SetControlsEnabled(true);
            }
        }

        /// <summary>
        /// Starts the producer-consumer system with the current configuration.
        /// </summary>
        private async Task StartSystem()
        {
            var config = new ProducerConsumerConfiguration
            {
                ProducerDelayMs = (int)nudProducerDelay.Value,
                ConsumerDelayMs = (int)nudConsumerDelay.Value,
                ConsumerCount = (int)nudConsumerCount.Value
            };

            await _manager.StartAsync(config);
            UpdateUIState(true);
        }

        /// <summary>
        /// Stops the producer-consumer system and updates the UI.
        /// </summary>
        private async Task StopSystem()
        {
            await _manager.StopAsync();
            UpdateUIState(false);
        }

        /// <summary>
        /// Updates the enabled state of controls based on whether the system is running.
        /// </summary>
        private void UpdateUIState(bool isRunning)
        {
            btnStart.Enabled = !isRunning;
            btnStop.Enabled = isRunning;
            nudProducerDelay.Enabled = !isRunning;
            nudConsumerDelay.Enabled = !isRunning;
            nudConsumerCount.Enabled = !isRunning;
        }

        /// <summary>
        /// Sets the enabled state of controls, considering the manager's running state.
        /// </summary>
        private void SetControlsEnabled(bool enabled)
        {
            btnStart.Enabled = enabled && !_manager.IsRunning;
            btnStop.Enabled = enabled && _manager.IsRunning;
            nudProducerDelay.Enabled = enabled && !_manager.IsRunning;
            nudConsumerDelay.Enabled = enabled && !_manager.IsRunning;
            nudConsumerCount.Enabled = enabled && !_manager.IsRunning;
        }

        /// <summary>
        /// Handles log entry events and appends them to the log display.
        /// </summary>
        private void Logger_LogEntryAdded(object sender, LogEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, LogEventArgs>(Logger_LogEntryAdded), sender, e);
                return;
            }

            Color color = e.Level switch
            {
                LogLevel.Error => Color.Red,
                LogLevel.Warning => Color.Orange,
                LogLevel.Info => Color.Green,
                LogLevel.Debug => Color.Blue,
                _ => Color.Black
            };

            AddLogEntry($"[{e.Level}] {e.Message}", color);
        }

        /// <summary>
        /// Handles queue statistics updates and refreshes the queue display.
        /// </summary>
        private void Manager_QueueStatisticsUpdated(object sender, QueueStatistics e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, QueueStatistics>(Manager_QueueStatisticsUpdated), sender, e);
                return;
            }

            lblQueueSize.Text = $"Queue Size: {e.CurrentSize}";
            UpdateQueueDisplay();
        }

        /// <summary>
        /// Handles producer statistics updates and refreshes the produced count.
        /// </summary>
        private void Manager_ProducerStatisticsUpdated(object sender, ProducerStatistics e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, ProducerStatistics>(Manager_ProducerStatisticsUpdated), sender, e);
                return;
            }
            lblProduced.Text = $"Produced: {GetTotalProduced()}";
        }

        /// <summary>
        /// Handles consumer statistics updates and refreshes the consumed count.
        /// </summary>
        private void Manager_ConsumerStatisticsUpdated(object sender, ConsumerStatistics e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, ConsumerStatistics>(Manager_ConsumerStatisticsUpdated), sender, e);
                return;
            }
            lblConsumed.Text = $"Consumed: {GetTotalConsumed()}";
        }

        /// <summary>
        /// Gets the total number of produced items from all producers.
        /// </summary>
        private int GetTotalProduced() => _manager.GetProducerStatistics().Sum(p => p.TotalProduced);

        /// <summary>
        /// Gets the total number of consumed items from all consumers.
        /// </summary>
        private int GetTotalConsumed() => _manager.GetConsumerStatistics().Sum(c => c.TotalConsumed);

        /// <summary>
        /// Updates the queue display with the latest items.
        /// </summary>
        private void UpdateQueueDisplay()
        {
            lstQueue.Items.Clear();
            var items = _queueService.GetCurrentItems(MaxQueueDisplayItems);

            for (int i = 0; i < items.Length; i++)
            {
                lstQueue.Items.Add($"[{i}] {items[i].Value} (by {items[i].ProducerId})");
            }
        }

        /// <summary>
        /// Appends a log entry to the log display with a timestamp and color.
        /// </summary>
        private void AddLogEntry(string message, Color color)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string fullMessage = $"[{timestamp}] {message}\n";

            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor = color;
            rtbLog.AppendText(fullMessage);
            rtbLog.SelectionColor = rtbLog.ForeColor;
            rtbLog.ScrollToCaret();
        }

        /// <summary>
        /// Resets the statistics labels to their initial state.
        /// </summary>
        private void UpdateUI()
        {
            lblQueueSize.Text = "Queue Size: 0";
            lblProduced.Text = "Produced: 0";
            lblConsumed.Text = "Consumed: 0";
        }
    }
}
