using System;

using Foundation;

namespace Clad.Models
{
    [Register(nameof(SetlistModel))]
    public class SetlistModel : NSObject
    {
        private int _bpm;

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

        public SetlistModel(int bpm, string key)
        {
            BPM = bpm;
            Key = key;
        }
    }
}
