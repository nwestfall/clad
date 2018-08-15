using System;

using Foundation;
using LiteDB;

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

        private int _upper;

        /// <summary>
        /// Gets or sets the upper.
        /// </summary>
        /// <value>The upper.</value>
        [Export(nameof(Upper))]
        public int Upper
        {
            get => _upper;
            set
            {
                WillChangeValue(nameof(Upper));
                _upper = value;
                DidChangeValue(nameof(Upper));
            }
        }

        private int _lower;

        /// <summary>
        /// Gets or sets the lower.
        /// </summary>
        /// <value>The lower.</value>
        [Export(nameof(Lower))]
        public int Lower
        {
            get => _lower;
            set
            {
                WillChangeValue(nameof(Lower));
                _lower = value;
                DidChangeValue(nameof(Lower));
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

        private int _seq;

        /// <summary>
        /// Gets or sets the seq.
        /// </summary>
        /// <value>The seq.</value>
        [Export(nameof(Seq))]
        public int Seq
        {
            get => _seq;
            set
            {
                WillChangeValue(nameof(Seq));
                _seq = value;
                DidChangeValue(nameof(Seq));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Clad.Models.SetlistModel"/> class.
        /// </summary>
        public SetlistModel() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Clad.Models.SetlistModel"/> class.
        /// </summary>
        /// <param name="bpm">Bpm.</param>
        /// <param name="upper">Upper.</param>
        /// <param name="lower">Lower.</param>
        /// <param name="key">Key.</param>
        public SetlistModel(int bpm, int upper, int lower, string key)
        {
            BPM = bpm;
            Upper = upper;
            Lower = lower;
            Key = key;
        }
    }
}
