//using Microsoft.Maui.Essentials;

using FitnessTracker.Resources;

namespace FitnessTracker
{
    public partial class MainPage : ContentPage
    {
        bool isTracking = false;
        int stepCount = 0;
        int stepGoal = 0;

        public MainPage()
        {
            InitializeComponent();
            UpdateHistoryDisplay();
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
        }

        private void OnStartButtonClicked(object sender, EventArgs e)
        {
            isTracking = !isTracking;
            if (isTracking)
            {
                Accelerometer.Start(SensorSpeed.UI);

                StartButton.Text = "Stop Tracking";
                StartButton.BackgroundColor = Colors.Red;
            }
            else
            {
                Accelerometer.Stop();
                StartButton.Text = "Start Tracking";
                StartButton.BackgroundColor = Colors.Green;
                SaveDailySteps();
            }
        }

        private async void OnSetGoalClicked(object sender, EventArgs e)
        {
            
            string result = await DisplayPromptAsync(
                "Set Step Goal",
                "How many steps would you like to achieve?",
                keyboard: Keyboard.Numeric 
            );

            //valid number
            if (int.TryParse(result, out int newGoal) && newGoal > 0)
            {
                stepGoal = newGoal;
                GoalLabel.Text = $"Goal: {stepGoal} steps";  // Up display

                UpdateProgress();
            }
            else if (result != null) 
            {
                await DisplayAlert("Invalid Input", "Please enter a positive number", "OK");
            }
        }

        private void OnResetButtonClicked(object sender, EventArgs e)
        {
            isTracking = false;
            SaveDailySteps();
            stepCount = 0;
            stepGoal = 0;

            // Reset UI elements
            StartButton.Text = "Start Tracking";
            StartButton.BackgroundColor = Colors.Blue;
            StepsLabel.Text = "0";
            ProgressLabel.Text = "0%";
            GoalLabel.Text = "Goal: Not Set";

            StepProgressBar.Progress = 0;

            if (Accelerometer.IsMonitoring)
                Accelerometer.Stop();
        }

        private async void UpdateProgress()
        {
            StepsLabel.Text = $"{stepCount:N0}";

            if (stepGoal > 0)
            {
                double progress = (double)stepCount / stepGoal;
                double currentProgress = StepProgressBar.Progress;

                // Animate ProgressBar
                uint animationLength = 500;
                await StepProgressBar.ProgressTo(Math.Min(progress, 1.0), animationLength, Easing.CubicOut);

                // Update percentage
                ProgressLabel.Text = progress >= 1.0 ? "100%" : $"{progress * 100:F1}%";

                if (progress >= 1.0 && stepCount - 1 < stepGoal)
                {

                 
                    await DisplayAlert("Congratulations! ", "You've achieved your step goal!", "OK");

                    StepProgressBar.ProgressColor = Colors.Blue;

                    GoalLabel.Text = $"Goal Achieved: {stepGoal} steps";
                }
            }
        }


        private DateTime lastStepTime = DateTime.MinValue;

        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            const double stepThreshold = 1.0;
            TimeSpan debounceTime = TimeSpan.FromMilliseconds(300);

            if ((e.Reading.Acceleration.X > stepThreshold || e.Reading.Acceleration.Y > stepThreshold) &&
                DateTime.UtcNow - lastStepTime > debounceTime)
            {
                lastStepTime = DateTime.UtcNow;
                stepCount++;
                UpdateProgress();
            }
        }

        private async void SaveDailySteps()
        {
            if (stepCount > 0)
            {
                var history = new StepHistory
                {
                    Timestamp = DateTime.Now,
                    Steps = stepCount
                };

                await DBHelper.SaveStepHistoryAsync(history);

                // Debugging: Check data saved
                var allHistory = await DBHelper.GetStepHistoryAsync();
                System.Diagnostics.Debug.WriteLine($"Total records in DB: {allHistory.Count}");
            }

            stepCount = 0;
            UpdateHistoryDisplay();
        }

        private async void UpdateHistoryDisplay()
        {
            try
            {
                var history = await DBHelper.GetStepHistoryAsync();
                var formattedHistory = history.Select(h => new
                {
                    Date = h.Timestamp.ToString("MMMM dd, yyyy, HH:mm"),
                    Steps = $"{h.Steps:N0} steps"
                }).ToList();

                // Bind data to the ListView
                HistoryList.ItemsSource = formattedHistory;

                //data binding
                System.Diagnostics.Debug.WriteLine($"HistoryList Items Count: {formattedHistory.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading history: {ex.Message}");
            }
        }

    }

}
