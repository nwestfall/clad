using Foundation;
using System;
using System.Diagnostics;
using UIKit;

using Clad.Audio;

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

        public async void Play()
        {
            Debug.WriteLine($"Play Pad: {AccessibilityIdentifier}");
            IsPlaying = true;
            SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
            BackgroundColor = UIColor.FromRGB(207, 216, 220);
            await AudioManager.Instance.PlayAsync(AccessibilityIdentifier);
        }

        public async void Stop()
        {
            Debug.WriteLine($"Stop Pad: {AccessibilityIdentifier}");
            Reset();
            await AudioManager.Instance.StopAsync(AudioManager.SoundType.Pad);
        }

        public void Reset()
        {
            IsPlaying = false;
            SetTitleColor(UIColor.LightTextColor, UIControlState.Normal);
            BackgroundColor = _originalBackground;
        }
    }
}