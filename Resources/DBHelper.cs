using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessTracker.Resources
{
    internal class DBHelper
    {
        private static SQLiteAsyncConnection _database;

        public static async Task<SQLiteAsyncConnection> GetDatabaseAsync()
        {
            if (_database == null)
            {
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "step_history.db");
                _database = new SQLiteAsyncConnection(dbPath);
                await _database.CreateTableAsync<StepHistory>();
            }
            return _database;
        }

        public static async Task SaveStepHistoryAsync(StepHistory history)
        {
            var db = await GetDatabaseAsync();
            await db.InsertAsync(history);
        }

        public static async Task<List<StepHistory>> GetStepHistoryAsync()
        {
            var db = await GetDatabaseAsync();
            return await db.Table<StepHistory>().ToListAsync();
        }
    }
}

