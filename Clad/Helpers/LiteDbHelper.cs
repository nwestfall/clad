using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

using LiteDB;
using Clad.Models;
using Foundation;

namespace Clad.Helpers
{
    /// <summary>
    /// Wrapper for <see cref=" LiteDB"/>
    /// </summary>
    public static class LiteDbHelper
    {
        private static readonly object _dbLock = new object();

        /// <summary>
        /// Saves the current setlist.
        /// </summary>
        /// <param name="setlists">Setlists.</param>
        public static void SaveCurrentSetlist(IList<SetlistModel> setlists)
        {
            Task.Run(() =>
            {
                lock (_dbLock)
                {
                    // Setup mapping
                    IList<BsonDocument> setlistDocs = new List<BsonDocument>();

                    // Set sequence and map
                    for (var i = 0; i < setlists.Count; i++)
                    {
                        setlists[i].Seq = i + 1;
                        setlistDocs.Add(new BsonDocument(new Dictionary<string, BsonValue>()
                        {
                            { "BPM", setlists[i].BPM },
                            { "Upper", setlists[i].Upper },
                            { "Lower", setlists[i].Lower },
                            { "Key", setlists[i].Key },
                            { "Seq", setlists[i].Seq }
                        }));
                    }

                    using (var instance = GetInstance())
                    {
                        var setlistCollection = instance.GetCollection(nameof(SetlistModel));
                        setlistCollection?.Delete(Query.All());
                        setlistCollection?.InsertBulk(setlistDocs);
                    }
                }
            });
        }

        /// <summary>
        /// Gets the setlists async.
        /// </summary>
        /// <returns>The setlists async.</returns>
        public static Task<List<SetlistModel>> GetSetlistsAsync()
        {
            return Task.Run(() =>
            {
                return GetSetlists();
            });
        }

        /// <summary>
        /// Gets the setlists.
        /// </summary>
        /// <returns>The setlists.</returns>
        public static List<SetlistModel> GetSetlists()
        {
            lock (_dbLock)
            {
                using (var instance = GetInstance())
                {
                    return instance.GetCollection<SetlistModel>().FindAll()?.OrderBy(s => s.Seq)?.ToList();
                }
            }
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>The instance.</returns>
        private static LiteDatabase GetInstance()
        {
            var connectionString = new ConnectionString()
            {
                Filename = Path.Combine(NSSearchPath.GetDirectories(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User)[0], "Clad.db"),
                Mode = LiteDB.FileMode.Exclusive
            };
            return new LiteDatabase(connectionString);
        }

        /// <summary>
        /// Lows the memory.
        /// </summary>
        public static void LowMemory()
        {
            // Don't need it yet
        }
    }
}
