using System;
using System.Diagnostics;

using CoreGraphics;
using UIKit;
using Foundation;
using Clad.Models;
using Clad.Audio;

namespace Clad
{
    /// <summary>
    /// Add popup view.
    /// </summary>
    [Register(nameof(AddPopupView))]
	public class AddPopupView : UIView
    {
        UINavigationBar _navBar;
        UISegmentedControl _keySelection;
        UILabel _bpmDisplay;
        UIStepper _bpmStepper;
        UILabel _upperDisplay;
        UIStepper _upperStepper;
        UILabel _lowerDisplay;
        UIStepper _lowerStepper;

        BPMModel _bpmModel;

        /// <summary>
        /// Occurs when setlist added.
        /// </summary>
        public event EventHandler<SetlistEventArgs> SetlistAdded = delegate { };

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Clad.AddPopupView"/> class.
        /// </summary>
        public AddPopupView(BPMModel bpmModel)
        {
            _bpmModel = bpmModel;
            BackgroundColor = UIColor.FromRGB(97, 125, 138);
            UserInteractionEnabled = true;
        }

        /// <summary>
        /// Draw the specified rect.
        /// </summary>
        /// <param name="rect">Rect.</param>
        public override void Draw(CGRect rect)
        {
            // Header
            _navBar = new UINavigationBar(new CGRect(0, 0, 500, 30));
            _navBar.BackgroundColor = UIColor.FromRGB(26, 32, 35);
            _navBar.TintColor = UIColor.FromRGB(26, 32, 35);
            _navBar.BarTintColor = UIColor.LightTextColor;
            _navBar.Items = new UINavigationItem[]
            {
                new UINavigationItem()
                {
                    Title = "Add Preset",
                    RightBarButtonItem = new UIBarButtonItem("Add", UIBarButtonItemStyle.Plain, (object sender, System.EventArgs e) =>
                    {
                        Debug.WriteLine("Add Preset");
                        SetlistAdded?.Invoke(this, new SetlistEventArgs((int)_bpmStepper.Value, (int)_upperStepper.Value, (int)_lowerStepper.Value, AudioManager.KEYS[(int)_keySelection.SelectedSegment]));
                    })
                    {
                        TintColor = UIColor.DarkTextColor
                    }
                }
            };

            // Key selection
            _keySelection = new UISegmentedControl(AudioManager.KEYS.ToNSStringArray());
            _keySelection.Frame = new CGRect(10, 80, 480, 35);
            _keySelection.TintColor = UIColor.FromRGB(26, 32, 35);
            _keySelection.SelectedSegment = 0;

            // BPM display
            _bpmDisplay = new UILabel(new CGRect(10, 155, 100, 40));
            _bpmDisplay.TextColor = UIColor.DarkTextColor;
            _bpmDisplay.AttributedText = _bpmModel.GetBPMAttributedString(_bpmModel.CurrentBPM, 10);

            // BPM Steper
            _bpmStepper = new UIStepper(new CGRect(90, 155, 100, 35));
            _bpmStepper.TintColor = UIColor.FromRGB(26, 32, 35);
            _bpmStepper.MinimumValue = 1;
            _bpmStepper.MaximumValue = 500;
            _bpmStepper.StepValue = 1;
            _bpmStepper.Value = _bpmModel.CurrentBPM;

            // Upper display
            _upperDisplay = new UILabel(new CGRect(200, 155, 25, 40));
            _upperDisplay.TextColor = UIColor.DarkTextColor;
            _upperDisplay.Text = _bpmModel.Upper.ToString();

            // Upper stepper
            _upperStepper = new UIStepper(new CGRect(230, 155, 100, 35));
            _upperStepper.TintColor = UIColor.FromRGB(26, 32, 35);
            _upperStepper.MinimumValue = 1;
            _upperStepper.MaximumValue = 16;
            _upperStepper.StepValue = 1;
            _upperStepper.Value = _bpmModel.Upper;

            // Lower display
            _lowerDisplay = new UILabel(new CGRect(340, 155, 25, 40));
            _lowerDisplay.TextColor = UIColor.DarkTextColor;
            _lowerDisplay.Text = _bpmModel.Lower.ToString();

            // Lower stepper
            _lowerStepper = new UIStepper(new CGRect(370, 155, 100, 35));
            _lowerStepper.TintColor = UIColor.FromRGB(26, 32, 35);
            _lowerStepper.MinimumValue = 1;
            _lowerStepper.MaximumValue = 16;
            _lowerStepper.StepValue = 1;
            _lowerStepper.Value = _bpmModel.Lower;

            // Setup Listeners
            _bpmStepper.ValueChanged += _bpmStepper_ValueChanged;
            _upperStepper.ValueChanged += _upperStepper_ValueChanged;
            _lowerStepper.ValueChanged += _lowerStepper_ValueChanged;

            // Show the view
            AddSubviews(
                _navBar,
                new UILabel(new CGRect(10, 40, 100, 40))
                {
                    Text = "Select a Key",
                    TextColor = UIColor.DarkTextColor
                },
                _keySelection,
                new UILabel(new CGRect(10, 120, 150, 40))
                {
                    Text = "Select a Tempo",
                    TextColor = UIColor.DarkTextColor
                },
                _bpmDisplay,
                _bpmStepper,
                _upperDisplay,
                _upperStepper,
                _lowerDisplay,
                _lowerStepper
            );

            // Custom dimensions
            rect.Width = 500;
            rect.Height = 200;

            base.Draw(rect);
        }

        void _bpmStepper_ValueChanged(object sender, System.EventArgs e)
        {
            UIStepper stepper = (UIStepper)sender;
            Debug.WriteLine($"BPM Stepper Change in Pop Over: {stepper.Value}");
            _bpmDisplay.AttributedText = _bpmModel.GetBPMAttributedString((int)stepper.Value, 10);
        }

        void _upperStepper_ValueChanged(object sender, EventArgs e)
        {
            UIStepper stepper = (UIStepper)sender;
            Debug.WriteLine($"Upper Stepper Change in Pop Over: {stepper.Value}");
            _upperDisplay.Text = stepper.Value.ToString();
        }

        void _lowerStepper_ValueChanged(object sender, EventArgs e)
        {
            UIStepper stepper = (UIStepper)sender;
            Debug.WriteLine($"Lower Stepper Change in Pop Over: {stepper.Value}");
            _lowerDisplay.Text = stepper.Value.ToString();
        }
    }
}
