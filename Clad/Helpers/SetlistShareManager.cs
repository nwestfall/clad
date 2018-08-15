using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Clad.Models;
using Foundation;

namespace Clad.Helpers
{
    /// <summary>
    /// Setlist share manager.
    /// Manages generating and reading setlist
    /// </summary>
    public static class SetlistShareManager
    {
        /// <summary>
        /// Generates the setlist share file.
        /// </summary>
        /// <returns>The setlist share file.</returns>
        /// <param name="setlists">Setlists.</param>
        public static SetlistFile GenerateSetlistShareFile(string name, IList<SetlistModel> setlists)
        {
            // Get temp file path
            var tmpPath = Path.GetTempPath();
            var tmpFile = Path.Combine(tmpPath, name + ".clad");

            // Build string for setlist in temp file
            using(var file = File.CreateText(tmpFile))
            {
                foreach (var item in setlists)
                {
                    file.WriteLine($"{item.Key},{item.BPM}");
                }
            }

            // Return as NSUrl
            return new SetlistFile(NSUrl.FromFilename(tmpFile));
        }

        /// <summary>
        /// Gets the setlists from file.
        /// </summary>
        /// <returns>The setlists from file.</returns>
        /// <param name="file">File.</param>
        public static IList<SetlistModel> GetSetlistsFromFile(string file)
        {
            IList<SetlistModel> setlist = new List<SetlistModel>();
            var fileContents = File.ReadAllLines(file);
            foreach(var line in fileContents)
            {
                var split = line.Split(',');
                if (split.Length < 2)
                    continue; // Skip cause it's too small

                setlist.Add(new SetlistModel()
                {
                    Key = split[0],
                    BPM = int.Parse(split[1])
                });
            }

            return setlist;
        }

        /// <summary>
        /// Setlist file.
        /// </summary>
        public class SetlistFile : IDisposable
        {
            /// <summary>
            /// Gets the file NSUrl.
            /// </summary>
            /// <value>The file NSUrl.</value>
            public NSUrl FileNSUrl { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Clad.Helpers.SetlistShareManager.SetlistFile"/> class.
            /// </summary>
            /// <param name="fileNsUrl">File ns URL.</param>
            public SetlistFile(NSUrl fileNsUrl)
            {
                this.FileNSUrl = fileNsUrl;
            }

            /// <summary>
            /// Releases all resource used by the <see cref="T:Clad.Helpers.SetlistShareManager.SetlistFile"/> object.
            /// </summary>
            /// <remarks>Call <see cref="Dispose"/> when you are finished using the
            /// <see cref="T:Clad.Helpers.SetlistShareManager.SetlistFile"/>. The <see cref="Dispose"/> method leaves
            /// the <see cref="T:Clad.Helpers.SetlistShareManager.SetlistFile"/> in an unusable state. After calling
            /// <see cref="Dispose"/>, you must release all references to the
            /// <see cref="T:Clad.Helpers.SetlistShareManager.SetlistFile"/> so the garbage collector can reclaim the
            /// memory that the <see cref="T:Clad.Helpers.SetlistShareManager.SetlistFile"/> was occupying.</remarks>
            public void Dispose()
            {
                if (!string.IsNullOrEmpty(FileNSUrl?.FilePathUrl?.AbsoluteString))
                {
                    if (File.Exists(FileNSUrl.FilePathUrl.AbsoluteString))
                    {
                        File.Delete(FileNSUrl.FilePathUrl.AbsoluteString);
                    }

                    FileNSUrl = null;
                }
            }
        }
    }
}
