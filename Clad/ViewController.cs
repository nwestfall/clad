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

        private SetlistSource _setlistSource = new SetlistSource(new List<SetlistModel>()
        {
            new SetlistModel(120, "C"),
            new SetlistModel(155, "D")
        });

        UIBarButtonItem _addNavButton;
        UIBarButtonItem _editNavButton;
        UIBarButtonItem _doneNavButton;

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //Audio Manager
            AudioManager.Initialize(AudioManager.PadSounds.Classic);

            SetupNavBar();

            //Initialization logic
            bpmStepperControl.Value = BPM.CurrentBPM;
            setlistTable.Source = _setlistSource;

            //Style
            bpmTapButton.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
            bpmTapButton.SetTitleColor(UIColor.LightTextColor, UIControlState.Highlighted);

            //Event Handles
            bpmStepperControl.ValueChanged += BpmStepperControl_ValueChanged;
            bpmTapButton.TouchDown += BpmTapButton_TouchDown;
            bpmTapButton.TouchUpInside += BpmTapButton_TouchUpInside;
            bpmLabel.UserInteractionEnabled = true;

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

        void SetupNavBar()
        {
            _addNavButton = new UIBarButtonItem(UIBarButtonSystemItem.Add, (sender, e) =>
            {
                Debug.WriteLine("Add Action");
            })
            {
                TintColor = UIColor.LightTextColor
            };
            _editNavButton = new UIBarButtonItem(UIBarButtonSystemItem.Edit, (sender, e) =>
            {
                Debug.WriteLine("Edit Action");
                ToggleSetlistEdit();
            })
            {
                TintColor = UIColor.LightTextColor
            };
            _doneNavButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (sender, e) =>
            {
                Debug.WriteLine("Done Action");
                ToggleSetlistEdit();
            })
            {
                TintColor = UIColor.LightTextColor
            };

            var item = navBar.Items;
            item[0].LeftBarButtonItems = new UIBarButtonItem[]
            {
                _addNavButton,
                _editNavButton
            };
        }

        void ToggleSetlistEdit()
        {
            UIBarButtonItem buttonToUse = null;
            if (setlistTable.Editing)
            {
                setlistTable.SetEditing(false, true);
                buttonToUse = _editNavButton;
            }
            else
            {
                setlistTable.SetEditing(true, true);
                buttonToUse = _doneNavButton;
            }

            var items = navBar.Items;
            items[0].LeftBarButtonItems = new UIBarButtonItem[]
            {
                _addNavButton,
                buttonToUse
            };
            navBar.Items = items;
        }

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

        partial void Settings_Action(UIBarButtonItem sender)
        {
            Debug.WriteLine("Settings Action");
        }

        partial void PadButton_Up(PadButton sender)
        {
            var key = sender.AccessibilityIdentifier;
            Debug.WriteLine($"Pad button tapped: {key}");

            if(sender.IsPlaying)
            {
                sender.Stop();
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
