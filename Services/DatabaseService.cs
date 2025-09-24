using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BabyTime.Models;

namespace BabyTime.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "activities.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Activity>().Wait();
        }

        public Task<int> SaveActivityAsync(Activity activity)
        {
            activity.CreatedAt = DateTime.Now;
            return _database.InsertAsync(activity);
        }

        public Task<List<Activity>> GetActivitiesForDateAsync(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            return _database.Table<Activity>()
                .Where(a => a.DateTime >= startOfDay && a.DateTime <= endOfDay)
                .OrderBy(a => a.DateTime)
                .ToListAsync();
        }

        public Task<List<Activity>> GetActivitiesForDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date.AddDays(1).AddTicks(-1);

            return _database.Table<Activity>()
                .Where(a => a.DateTime >= start && a.DateTime <= end)
                .OrderBy(a => a.DateTime)
                .ToListAsync();
        }

        public Task<int> DeleteActivityAsync(Activity activity)
        {
            return _database.DeleteAsync(activity);
        }

        public Task<int> UpdateActivityAsync(Activity activity)
        {
            return _database.UpdateAsync(activity);
        }
    }
}