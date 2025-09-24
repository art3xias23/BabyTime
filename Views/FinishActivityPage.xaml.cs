using System;
using System.Linq;
using Microsoft.Maui.Controls;
using BabyTime.Models;
using BabyTime.Services;

namespace BabyTime.Views
{
    public partial class FinishActivityPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private Activity _activity;
        private ActivityDisplay _activityDisplay;

        public FinishActivityPage(ActivityDisplay activityDisplay)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _activityDisplay = activityDisplay;
            InitializePage();
        }

        private async void InitializePage()
        {
            // Get the full activity from database
            var activities = await _databaseService.GetActivitiesForDateAsync(_activityDisplay.DateTime.Date);
            _activity = activities.FirstOrDefault(a => a.Id == _activityDisplay.Id);

            if (_activity == null)
            {
                await DisplayAlert("Error", "Activity not found", "OK");
                await Navigation.PopAsync();
                return;
            }

            // Display activity info
            ActivityInfoLabel.Text = $"{_activity.ActivityType}";
            StartTimeLabel.Text = $"Started at {_activity.DateTime:HH:mm}";

            // Initialize time pickers
            var hours = Enumerable.Range(0, 24).Select(h => h.ToString("00")).ToList();
            HourPicker.ItemsSource = hours;

            var minutes = Enumerable.Range(0, 60).Select(m => m.ToString("00")).ToList();
            MinutePicker.ItemsSource = minutes;

            // Set current time or existing end time
            DateTime defaultTime;
            if (_activity.EndDateTime.HasValue)
            {
                defaultTime = _activity.EndDateTime.Value;
            }
            else
            {
                try
                {
                    defaultTime = TimeZoneInfo.ConvertTime(DateTime.Now,
                        TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"));
                }
                catch
                {
                    defaultTime = DateTime.Now;
                }
            }

            HourPicker.SelectedIndex = defaultTime.Hour;
            MinutePicker.SelectedIndex = defaultTime.Minute;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (HourPicker.SelectedItem == null || MinutePicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Please select time", "OK");
                return;
            }

            var hour = int.Parse(HourPicker.SelectedItem.ToString());
            var minute = int.Parse(MinutePicker.SelectedItem.ToString());

            var endDateTime = new DateTime(_activity.DateTime.Year, _activity.DateTime.Month,
                _activity.DateTime.Day, hour, minute, 0);

            // Validate end time is after start time
            if (endDateTime <= _activity.DateTime)
            {
                await DisplayAlert("Error", "End time must be after start time", "OK");
                return;
            }

            // Update the activity
            _activity.EndDateTime = endDateTime;
            await _databaseService.UpdateActivityAsync(_activity);

            await DisplayAlert("Success", "End time saved successfully", "OK");
            await Navigation.PopAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}