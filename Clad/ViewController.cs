using System;
using System.Diagnostics;
using System.Collections.Generic;

using Foundation;
using UIKit;
using Clad.Models;

namespace Clad
{
    public partial class ViewController : UIViewController
    {
        private BPMModel _bpmModel = new BPMModel();

        [Export(nameof(BPM))]
        public BPMModel BPM
        {
            get => _bpmModel;
            set
            {
                WillChangeValue(nameof(BPM));
                _bpmModel = value;
                DidChangeValue(nameof(BPM));
            }
        }

        [Outlet]
        PadButton[] _padButtons { get; set; }

        IList<IDisposable> _observablesToDispose = new List<IDisposable>();

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //Initialization logic
            bpmStepperControl.Value = BPM.CurrentBPM;

            //Style
            bpmTapButton.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
            bpmTapButton.SetTitleColor(UIColor.LightTextColor, UIControlState.Highlighted);

            //Event Handles
            bpmStepperControl.ValueChanged += BpmStepperControl_ValueChanged;
            bpmTapButton.TouchDown += BpmTapButton_TouchDown;
            bpmTapButton.TouchUpInside += BpmTapButton_TouchUpInside;

            //Observers
            _observablesToDispose.Add(BPM.AddObserver(nameof(BPMModel.CurrentBPM), NSKeyValueObservingOptions.New, (observed) =>
            {
                int newValue = observed.NewValue.ToInt();
                Debug.WriteLine($"{nameof(BPMModel.CurrentBPM)} changed: {newValue}");
                var attributedString = new NSMutableAttributedString($"{newValue}bpm");
                attributedString.BeginEditing();
                attributedString.AddAttribute(UIStringAttributeKey.Font, UIFont.SystemFontOfSize(22, UIFontWeight.Regular), new NSRange(newValue.ToString().Length, 3));
                attributedString.EndEditing();
                bpmLabel.AttributedText = attributedString;
                bpmStepperControl.Value = newValue;
            }));
        }
        public override void ViewDidUnload()
        {
            base.ViewDidUnload();
            //Event Handlers
            bpmStepperControl.ValueChanged -= BpmStepperControl_ValueChanged;
            bpmTapButton.TouchDown -= BpmTapButton_TouchDown;
            bpmTapButton.TouchUpInside -= BpmTapButton_TouchUpInside;
            //Observers
            foreach (var observable in _observablesToDispose)
                observable?.Dispose();
            _observablesToDispose.Clear();
            _observablesToDispose = new List<IDisposable>();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use
        }

        public override bool PrefersStatusBarHidden() => true;

        #region Events
        void BpmTapButton_TouchDown(object sender, EventArgs e)
        {
            BPM.LastBPMTap = DateTime.UtcNow.Ticks;
            bpmTapButton.BackgroundColor = UIColor.FromRGB(97, 125, 138);
        }


        void BpmStepperControl_ValueChanged(object sender, EventArgs e)
        {
            UIStepper stepper = (UIStepper)sender;
            Debug.WriteLine($"BPM Stepper Change: {stepper.Value}");
            BPM.CurrentBPM = (int)stepper.Value;
        }

        partial void PadButton_Up(PadButton sender)
        {
            var key = sender.AccessibilityIdentifier;
            Debug.WriteLine($"Pad button tapped: {key}");

            if(sender.Selected)
            {
                sender.Reset();
                return;
            }

            //Reset
            foreach (var padButton in _padButtons)
                padButton.Reset();

            //Highlight
            sender.Play();
        }

        void BpmTapButton_TouchUpInside(object sender, EventArgs e)
        {
            bpmTapButton.BackgroundColor = UIColor.FromRGB(192, 203, 208);
        }
        #endregion
    }
}
