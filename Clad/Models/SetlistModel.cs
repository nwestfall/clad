using System;

using Foundation;

namespace Clad.Models
{
    /// <summary>
    /// Setlist model.
    /// </summary>
    [Register(nameof(SetlistModel))]
    public class SetlistModel : NSObject
    {
        private int _bpm;

        /// <summary>
        /// Gets or sets the bpm.
        /// </summary>
        /// <value>The bpm.</value>
        [Export(nameof(BPM))]
        public int BPM
        {
            get => _bpm;
            set
            {
                WillChangeValue(nameof(BPM));
                _bpm = value;
                DidChangeValue(nameof(BPM));
            }
        }

        private string _key;

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        [Export(nameof(Key))]
        public string Key
        {
            get => _key;
            set
            {
                WillChangeValue(nameof(Key));
                _key = value;
                DidChangeValue(nameof(Key));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Clad.Models.SetlistModel"/> class.
        /// </summary>
        /// <param name="bpm">Bpm.</param>
        /// <param name="key">Key.</param>
        public SetlistModel(int bpm, string key)
        {
            BPM = bpm;
            Key = key;
        }
    }
}
