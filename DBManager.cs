using LiteDB;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace TwitchDropFarmBot
{
    internal class DBManager
    {
        private static bool isInitialized = false;
        public static string databasePath = "steamers.db";
        private static LiteDatabase database;
        public DBManager(string databasePath)
        {
            if (!isInitialized)
            {
                database = new LiteDatabase(databasePath);
                isInitialized = true;
            }
        }

        public static void InsertStreamData(StreamerData streamData)
        {
            var collection = database.GetCollection<StreamerData>("streamData");
            collection.Insert(streamData);
        }
        public static void DeleteStreamData(int id)
        {
            var collection = database.GetCollection<StreamerData>("streamData");
            collection.Delete(id);
        }

        public static void UpdateWatchedTime(int id, int watchedMinutes)
        {
            var collection = database.GetCollection<StreamerData>("streamData");
            var streamData = collection.FindById(id);
            streamData.Watched = watchedMinutes;
            collection.Update(streamData);
        }
        public static void UpdateDone(int id, bool value)
        {
            var collection = database.GetCollection<StreamerData>("streamData");
            var streamData = collection.FindById(id);
            streamData.Done = value;
            collection.Update(streamData);
        }

        public static IEnumerable<StreamerData> GetAllStreamData()
        {
            var collection = database.GetCollection<StreamerData>("streamData");
            return collection.FindAll();
        }
    }

    public class StreamerData
    {
        [BsonId]
        public int Id { get; set; }
        public string StreamerName { get; set; }
        public bool SpecificGame { get; set; }
        public string SpecificGameName { get; set; }
        public int HowLongToWatch { get; set; }
        public int Watched { get; set; }
        public bool Done { get; set; }
    }
}
