using BabyTime.Models;
using BabyTime.Services;

namespace BabyTime.Views
{
    public partial class InputPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public InputPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            InitializePickers();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateTimeToNow();
        }

        private void InitializePickers()
        {
            var hours = Enumerable.Range(0, 24).Select(h => h.ToString("00")).ToList();
            HourPicker.ItemsSource = hours;

            var minutes = Enumerable.Range(0, 60).Select(m => m.ToString("00")).ToList();
            MinutePicker.ItemsSource = minutes;

            // Add focus event handlers to ensure selection is visible
            HourPicker.Focused += OnPickerFocused;
            MinutePicker.Focused += OnPickerFocused;

            UpdateTimeToNow();
            ActivityPicker.SelectedIndex = 0; // Default to "Eat"
        }

        private async void OnPickerFocused(object sender, FocusEventArgs e)
        {
            if (sender is Picker picker)
            {
                // Small delay to ensure the picker is fully rendered
                await Task.Delay(50);

                // Ensure the selected item is properly set and visible
                var currentIndex = picker.SelectedIndex;
                if (currentIndex >= 0 && picker.ItemsSource != null && currentIndex < picker.ItemsSource.Count)
                {
                    picker.SelectedItem = picker.ItemsSource[currentIndex];
                }
            }
        }

        private void UpdateTimeToNow()
        {
            try
            {
                var sofiaTime = TimeZoneInfo.ConvertTime(DateTime.Now,
                    TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"));

                // Set the selected index which will automatically position the picker
                HourPicker.SelectedIndex = sofiaTime.Hour;
                MinutePicker.SelectedIndex = sofiaTime.Minute;

                // Force update the selected item to ensure it's properly set
                if (HourPicker.ItemsSource != null && HourPicker.ItemsSource.Count > sofiaTime.Hour)
                {
                    HourPicker.SelectedItem = HourPicker.ItemsSource[sofiaTime.Hour];
                }

                if (MinutePicker.ItemsSource != null && MinutePicker.ItemsSource.Count > sofiaTime.Minute)
                {
                    MinutePicker.SelectedItem = MinutePicker.ItemsSource[sofiaTime.Minute];
                }
            }
            catch
            {
                // Fallback if timezone not found
                var currentTime = DateTime.Now;
                HourPicker.SelectedIndex = currentTime.Hour;
                MinutePicker.SelectedIndex = currentTime.Minute;

                // Force update the selected item to ensure it's properly set
                if (HourPicker.ItemsSource != null && HourPicker.ItemsSource.Count > currentTime.Hour)
                {
                    HourPicker.SelectedItem = HourPicker.ItemsSource[currentTime.Hour];
                }

                if (MinutePicker.ItemsSource != null && MinutePicker.ItemsSource.Count > currentTime.Minute)
                {
                    MinutePicker.SelectedItem = MinutePicker.ItemsSource[currentTime.Minute];
                }
            }
        }

        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            if (HourPicker.SelectedItem == null || MinutePicker.SelectedItem == null || ActivityPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Please select all fields", "OK");
                return;
            }

            var hour = int.Parse(HourPicker.SelectedItem.ToString());
            var minute = int.Parse(MinutePicker.SelectedItem.ToString());
            var activityType = ActivityPicker.SelectedItem.ToString();

            DateTime baseTime;
            try
            {
                baseTime = TimeZoneInfo.ConvertTime(DateTime.Now,
                    TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"));
            }
            catch
            {
                // Fallback if timezone not found
                baseTime = DateTime.Now;
            }

            var activityDateTime = new DateTime(baseTime.Year, baseTime.Month, baseTime.Day, hour, minute, 0);

            var activity = new Activity
            {
                DateTime = activityDateTime,
                ActivityType = activityType
            };

            await _databaseService.SaveActivityAsync(activity);

            ShowSuccess();

            // Reset to current time for next entry
            UpdateTimeToNow();
        }

        private async void ShowSuccess()
        {
            SuccessBorder.IsVisible = true;
            await Task.Delay(5000);
            SuccessBorder.IsVisible = false;
        }
    }
}