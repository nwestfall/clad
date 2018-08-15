using System;
using Foundation;
using UIKit;
using Clad.Helpers;

namespace Clad.Models
{
    [Register(nameof(BPMModel))]
    public class BPMModel : NSObject
    {
        public const long TICKS_PER_MINUTE = 60000 * 10000;
        public const long TICKS_PER_SIX_SECONDS = 6000 * 10000;

        private int _currentBpm = 125;

        [Export(nameof(CurrentBPM))]
        public int CurrentBPM
        {
            get => _currentBpm;
            set
            {
                WillChangeValue(nameof(CurrentBPM));
                _currentBpm = value;
                Settings.LastBPM = value;
                DidChangeValue(nameof(CurrentBPM));
            }
        }

        private long _lastBPMTap = 0;

        [Export(nameof(LastBPMTap))]
        public long LastBPMTap
        {
            get => _lastBPMTap;
            set
            {
                CalculateBPMFromTaps();
                WillChangeValue(nameof(LastBPMTap));
                _lastBPMTap = value;
                DidChangeValue(nameof(LastBPMTap));
            }
        }

        private int _upper = 4;

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

        private int _lower = 4;

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

        private void CalculateBPMFromTaps()
        {
            var now = DateTime.UtcNow.Ticks;
            var totalMilliseconds = now - _lastBPMTap;
            if(totalMilliseconds < TICKS_PER_SIX_SECONDS)
            {
                //In interval, check
                CurrentBPM = (int)(TICKS_PER_MINUTE / totalMilliseconds);
            }
        }

        /// <summary>
        /// Gets the BPMA ttributed string.
        /// </summary>
        /// <returns>The BPMA ttributed string.</returns>
        /// <param name="bpm">Bpm.</param>
        public NSAttributedString GetBPMAttributedString(int bpm, int fontSize = 22)
        {
            var attributedString = new NSMutableAttributedString($"{bpm}bpm");
            attributedString.BeginEditing();
            attributedString.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(fontSize, UIFontWeight.Regular), new NSRange(bpm.ToString().Length, 3));
            attributedString.EndEditing();
            return attributedString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Clad.Models.BPMModel"/> class.
        /// </summary>
        /// <param name="bpm">Bpm.</param>
        public BPMModel(int bpm = 125, int upper = 4, int lower = 4)
        {
            _currentBpm = bpm;
            _upper = upper;
            _lower = lower;
        }
    }
}
