using Foundation;
using System;
using UIKit;

namespace Clad
{
    public partial class PadButton : UIButton
    {
        private UIColor _originalBackground;

        private bool _isPlaying;

        [Export(nameof(IsPlaying))]
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                WillChangeValue(nameof(IsPlaying));
                _isPlaying = value;
                DidChangeValue(nameof(IsPlaying));
            }
        }

        public PadButton (IntPtr handle) : base (handle) 
        {
            _originalBackground = BackgroundColor;
            SetTitleColor(UIColor.LightTextColor, UIControlState.Normal);
            SetTitleColor(UIColor.DarkGray, UIControlState.Selected);
        }

        public void Play()
        {
            IsPlaying = true;
            SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
            BackgroundColor = UIColor.FromRGB(207, 216, 220);
        }

        public void Reset()
        {
            IsPlaying = false;
            SetTitleColor(UIColor.LightTextColor, UIControlState.Normal);
            BackgroundColor = _originalBackground;
        }
    }
}