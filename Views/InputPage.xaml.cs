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

        private void InitializePickers()
        {
            var hours = Enumerable.Range(0, 24).Select(h => h.ToString("00")).ToList();
            HourPicker.ItemsSource = hours;

            var minutes = Enumerable.Range(0, 60).Select(m => m.ToString("00")).ToList();
            MinutePicker.ItemsSource = minutes;

            try
            {
                var sofiaTime = TimeZoneInfo.ConvertTime(DateTime.Now,
                    TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"));

                HourPicker.SelectedIndex = sofiaTime.Hour;
                MinutePicker.SelectedIndex = sofiaTime.Minute;
            }
            catch
            {
                // Fallback if timezone not found
                var currentTime = DateTime.Now;
                HourPicker.SelectedIndex = currentTime.Hour;
                MinutePicker.SelectedIndex = currentTime.Minute;
            }

            ActivityPicker.SelectedIndex = 0; // Default to "Eat"
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
            InitializePickers();
        }

        private async void ShowSuccess()
        {
            SuccessBorder.IsVisible = true;
            await Task.Delay(5000);
            SuccessBorder.IsVisible = false;
        }
    }
}