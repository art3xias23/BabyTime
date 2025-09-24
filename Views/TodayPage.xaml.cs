using BabyTime.Models;
using BabyTime.Services;
using System.Globalization;

namespace BabyTime.Views
{
    public partial class TodayPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public TodayPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTodayActivities();
        }

        private async System.Threading.Tasks.Task LoadTodayActivities()
        {
            DateTime currentTime;
            try
            {
                currentTime = TimeZoneInfo.ConvertTime(DateTime.Now,
                    TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"));
            }
            catch
            {
                // Fallback if timezone not found
                currentTime = DateTime.Now;
            }

            DateLabel.Text = currentTime.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture);
            DayLabel.Text = currentTime.ToString("dddd", CultureInfo.InvariantCulture);

            var activities = await _databaseService.GetActivitiesForDateAsync(currentTime);

            // Debug: Check if activities have ActivityType
            var displayActivities = activities.Select(a => new ActivityDisplay
            {
                Id = a.Id,
                TimeString = a.DateTime.ToString("HH:mm"),
                EndTimeString = a.EndDateTime?.ToString("HH:mm") ?? "ongoing",
                ActivityType = string.IsNullOrEmpty(a.ActivityType) ? "Unknown" : a.ActivityType,
                DateTime = a.DateTime,
                EndDateTime = a.EndDateTime
            }).OrderBy(a => a.DateTime).ToList();

            ActivitiesCollectionView.ItemsSource = displayActivities;
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var activity = button?.CommandParameter as ActivityDisplay;

            if (activity == null) return;

            bool confirm = await DisplayAlert("Confirm Delete",
                $"Are you sure you want to delete the {activity.ActivityType} activity at {activity.TimeString}?",
                "Yes", "No");

            if (confirm)
            {
                // Find and delete the activity from database
                var activities = await _databaseService.GetActivitiesForDateAsync(activity.DateTime.Date);
                var activityToDelete = activities.FirstOrDefault(a =>
                    a.DateTime.Hour == activity.DateTime.Hour &&
                    a.DateTime.Minute == activity.DateTime.Minute &&
                    a.ActivityType == activity.ActivityType);

                if (activityToDelete != null)
                {
                    await _databaseService.DeleteActivityAsync(activityToDelete);
                    await LoadTodayActivities();
                    ShowSuccess();
                }
            }
        }

        private async void OnFinishClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var activity = button?.CommandParameter as ActivityDisplay;

            if (activity == null) return;

            var finishPage = new FinishActivityPage(activity);
            await Navigation.PushAsync(finishPage);

            // Refresh when returning
            finishPage.Disappearing += async (s, args) =>
            {
                await LoadTodayActivities();
            };
        }

        private async void ShowSuccess()
        {
            SuccessBorder.IsVisible = true;
            await Task.Delay(5000);
            SuccessBorder.IsVisible = false;
        }
    }
}