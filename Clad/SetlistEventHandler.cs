using System;

using Clad.Models;

namespace Clad
{
    /// <summary>
    /// Setlist event handler.
    /// </summary>
    public class SetlistEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the setlist.
        /// </summary>
        /// <value>The setlist.</value>
        public SetlistModel Setlist { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Clad.SetlistEventArgs"/> class.
        /// </summary>
        /// <param name="bpm">Bpm.</param>
        /// <param name="key">Key.</param>
        public SetlistEventArgs(int bpm,string key)
        {
            Setlist = new SetlistModel(bpm, key);
        }
    }
}
