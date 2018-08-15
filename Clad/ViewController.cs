using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

using Foundation;
using UIKit;
using CoreGraphics;
using Clad.Models;
using Clad.Audio;
using Clad.Helpers;

namespace Clad
{
    public partial class ViewController : UIViewController, IUITableViewDelegate, IUIAlertViewDelegate
    {
        private BPMModel _bpmModel = new BPMModel(Settings.LastBPM, Settings.LastUpper, Settings.LastLower);

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

        private SetlistSource _setlistSource = new SetlistSource(new List<SetlistModel>());

        UIBarButtonItem _addNavButton;
        UIBarButtonItem _editNavButton;
        UIBarButtonItem _doneNavButton;

        AddPopupView _popView;
        UIViewController _addPopupController;
        UITapGestureRecognizer _bpmLabelTap;

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //Audio Manager
            AudioManager.Initialize(AudioManager.PadSounds.Classic, Settings.MasterVolume, Settings.PadVolume, Settings.ClickVolume);

            //ICloud
            Settings.SyncFromCloud();

            //Setup Views
            SetupNavBar();
            masterVolumeSlider.Value = Settings.MasterVolume;
            clickVolumeSlider.Value = Settings.ClickVolume;
            padVolumeSlider.Value = Settings.PadVolume;

            //Initialization logic
            bpmStepperControl.Value = BPM.CurrentBPM;
            bpmLabel.AttributedText = BPM.GetBPMAttributedString(BPM.CurrentBPM);
            upperStepperControl.Value = BPM.Upper;
            upperLabel.Text = BPM.Upper.ToString();
            lowerStepperControl.Value = BPM.Lower;
            lowerLabel.Text = BPM.Lower.ToString();
            setlistTable.Source = _setlistSource;
            setlistTable.Delegate = this;
            toggleClickButton.SetTitle("Start Click", UIControlState.Normal);
            toggleClickButton.SetTitle("Stop Click", UIControlState.Selected);
            _setlistSource.Items = LiteDbHelper.GetSetlists();

            //Style
            bpmTapButton.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
            bpmTapButton.SetTitleColor(UIColor.LightTextColor, UIControlState.Highlighted);

            //Event Handles
            bpmStepperControl.ValueChanged += BpmStepperControl_ValueChanged;
            upperStepperControl.ValueChanged += UpperStepperControl_ValueChanged;
            lowerStepperControl.ValueChanged += LowerStepperControl_ValueChanged;
            bpmTapButton.TouchDown += BpmTapButton_TouchDown;
            bpmTapButton.TouchUpInside += BpmTapButton_TouchUpInside;
            bpmLabel.UserInteractionEnabled = true;
            _bpmLabelTap = new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("BPM Label Tapped");
                var bpmPrompt = UIAlertController.Create("Enter BPM", "", UIAlertControllerStyle.Alert);
                bpmPrompt.Title = "Enter BPM";
                bpmPrompt.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                bpmPrompt.AddAction(UIAlertAction.Create("Set", UIAlertActionStyle.Default, (action) =>
                {
                    Debug.WriteLine("BPM Set action");
                    var textField = bpmPrompt.TextFields[0];
                    if (string.IsNullOrEmpty(textField.Text))
                        AlertMessage("Error changing BPM", "BPM field was left blank");
                    if (int.TryParse(textField.Text, out int result))
                    {
                        if (result < 1 || result > 500)
                            AlertMessage("Error changing BPM", "BPM must be between 0 and 501");
                        else
                        {
                            BPM.CurrentBPM = result;
                        }
                    }
                    else
                        AlertMessage("Error changing BPM", "BPM must be a number");
                }));
                bpmPrompt.AddTextField((textField) =>
                {
                    textField.KeyboardType = UIKeyboardType.NumberPad;
                    textField.KeyboardAppearance = UIKeyboardAppearance.Dark;
                    textField.ShouldChangeCharacters = (UITextField field, NSRange range, string replace) =>
                    {
                        return int.TryParse(replace, out int result);
                    };
                });
                PresentViewController(bpmPrompt, true, null);
            });
            bpmLabel.AddGestureRecognizer(_bpmLabelTap);

            //Observers
            _observablesToDispose.Add(BPM.AddObserver(nameof(BPMModel.CurrentBPM), NSKeyValueObservingOptions.New, (observed) =>
            {
                int newValue = observed.NewValue.ToInt();
                Debug.WriteLine($"{nameof(BPMModel.CurrentBPM)} changed: {newValue}");
                bpmLabel.AttributedText = BPM.GetBPMAttributedString(newValue);
                bpmStepperControl.Value = newValue;
            }));
            _observablesToDispose.Add(BPM.AddObserver(nameof(BPMModel.Upper), NSKeyValueObservingOptions.New, (observed) =>
            {
                int newValue = observed.NewValue.ToInt();
                Debug.WriteLine($"{nameof(BPMModel.Upper)} changed: {newValue}");
                upperLabel.Text = newValue.ToString();
                upperStepperControl.Value = newValue;
            }));
            _observablesToDispose.Add(BPM.AddObserver(nameof(BPMModel.Lower), NSKeyValueObservingOptions.New, (observed) =>
            {
                int newValue = observed.NewValue.ToInt();
                Debug.WriteLine($"{nameof(BPMModel.Lower)} changed: {newValue}");
                lowerLabel.Text = newValue.ToString();
                lowerStepperControl.Value = newValue;
            }));
        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();
            //Event Handlers
            bpmStepperControl.ValueChanged -= BpmStepperControl_ValueChanged;
            upperStepperControl.ValueChanged -= UpperStepperControl_ValueChanged;
            lowerStepperControl.ValueChanged -= LowerStepperControl_ValueChanged;
            bpmTapButton.TouchDown -= BpmTapButton_TouchDown;
            bpmTapButton.TouchUpInside -= BpmTapButton_TouchUpInside;
            bpmLabel.RemoveGestureRecognizer(_bpmLabelTap);
            _bpmLabelTap.Dispose();
            _bpmLabelTap = null;

            //Observers
            foreach (var observable in _observablesToDispose)
                observable?.Dispose();
            _observablesToDispose.Clear();
            _observablesToDispose = new List<IDisposable>();

            //ICloud
            Settings.SyncToCloud();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use
            if (_popView != null)
            {
                _popView.SetlistAdded -= _popView_SetlistAdded;
                _popView.Dispose();
                _popView = null;
            }
            if (_addPopupController != null)
            {
                _addPopupController?.Dispose();
                _addPopupController = null;
            }
            // Manage LiteDb
            LiteDbHelper.LowMemory();
        }

        public override bool PrefersStatusBarHidden() => true;

        void SetupNavBar()
        {
            _addNavButton = new UIBarButtonItem(UIBarButtonSystemItem.Add, (sender, e) =>
            {
                Debug.WriteLine("Add Action");

                if(_popView == null)
                {
                    _popView = new AddPopupView(BPM);
                    _popView.SetlistAdded += _popView_SetlistAdded;
                }

                if (_addPopupController == null)
                {
                    _addPopupController = new UIViewController()
                    {
                        ModalPresentationStyle = UIModalPresentationStyle.Popover,
                        PreferredContentSize = new CGSize(500, 200),
                        View = _popView
                    };
                }

                _addPopupController.PopoverPresentationController.BackgroundColor = UIColor.LightGray;
                _addPopupController.PopoverPresentationController.SourceRect = new CGRect(50, 50, 500, 300);
                _addPopupController.PopoverPresentationController.BarButtonItem = _addNavButton;
                _addPopupController.PopoverPresentationController.SourceView = View;

                PresentViewController(_addPopupController, true, null);
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

        [Export("tableView:didSelectRowAtIndexPath:")]
        public async void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            Debug.WriteLine($"Setlist Row Selected: {indexPath.Row}");
            var setlist = _setlistSource.Items[indexPath.Row];
            BPM.CurrentBPM = setlist.BPM;
            BPM.Upper = setlist.Upper;
            BPM.Lower = setlist.Lower;

            //Manage Pad Button
            foreach (var padButton in _padButtons)
            {
                if (padButton.AccessibilityIdentifier.Equals(setlist.Key, StringComparison.InvariantCultureIgnoreCase))
                {
                    padButton.Play();
                    continue;
                }

                if (padButton.IsPlaying)
                    padButton.Stop();
                else
                    padButton.Reset();
            }

            //Manage click
            await AudioManager.Instance.PlayAsync(ref _bpmModel);
            toggleClickButton.Selected = true;
        }

        void AlertMessage(string title, string message)
        {
            UIAlertController alertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            PresentViewController(alertController, true, null);
        }

        Task<string> PromptMessage(string title, string message, string action, string placeholder = "")
        {
            TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();
            UIAlertController alertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alertController.AddTextField((obj) =>
            {
                obj.Placeholder = placeholder;
            });
            alertController.AddAction(UIAlertAction.Create(action, UIAlertActionStyle.Default, (ui) => {
                var textField = alertController.TextFields[0];
                taskCompletionSource.TrySetResult(textField.Text);
            }));
            PresentViewController(alertController, true, null);
            return taskCompletionSource.Task;
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

            if (setlistTable.IndexPathForSelectedRow != null)
                setlistTable.DeselectRow(setlistTable.IndexPathForSelectedRow, true);
        }

        void UpperStepperControl_ValueChanged(object sender, EventArgs e)
        {
            UIStepper stepper = (UIStepper)sender;
            Debug.WriteLine($"Upper Stepper Change: {stepper.Value}");
            BPM.Upper = (int)stepper.Value;

            if (setlistTable.IndexPathForSelectedRow != null)
                setlistTable.DeselectRow(setlistTable.IndexPathForSelectedRow, true);
        }

        void LowerStepperControl_ValueChanged(object sender, EventArgs e)
        {
            UIStepper stepper = (UIStepper)sender;
            Debug.WriteLine($"Lower Stepper Change: {stepper.Value}");
            BPM.Lower = (int)stepper.Value;

            if (setlistTable.IndexPathForSelectedRow != null)
                setlistTable.DeselectRow(setlistTable.IndexPathForSelectedRow, true);
        }

        async partial void Share_Action(UIBarButtonItem sender)
        {
            Debug.WriteLine("Share Action");
            if (_setlistSource.Items.Count == 0)
                AlertMessage("Unable to share setlist", "You must add at least 1 item to your setlist before you can share it with someone");
            else
            {
                Debug.WriteLine($"Sharing ${_setlistSource.Items.Count} items from your setlist");
                Debug.WriteLine("Asking for name");
                var name = await PromptMessage("Setlist name", "Enter a name for the setlist", "OK");
                var tempFile = SetlistShareManager.GenerateSetlistShareFile(name, _setlistSource.Items);
                var activityViewController = new UIActivityViewController(new NSObject[1] { tempFile.FileNSUrl }, null);
                activityViewController.ModalPresentationStyle = UIModalPresentationStyle.Popover;
                activityViewController.PopoverPresentationController.BarButtonItem = sender;
                activityViewController.PopoverPresentationController.SourceView = View;
                activityViewController.ExcludedActivityTypes = new NSString[0];
                PresentViewController(activityViewController, true, () => 
                {
                    tempFile?.Dispose();
                });
            }
        }

        partial void Settings_Action(UIBarButtonItem sender)
        {
            Debug.WriteLine("Settings Action");
        }

        partial void PadButton_Up(PadButton sender)
        {
            var key = sender.AccessibilityIdentifier;
            Debug.WriteLine($"Pad button tapped: {key}");

            if (setlistTable.IndexPathForSelectedRow != null)
                setlistTable.DeselectRow(setlistTable.IndexPathForSelectedRow, true);

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

        async partial void ActionButtons(UIButton sender)
        {
            var action = sender.AccessibilityIdentifier;
            Debug.WriteLine($"{action} tapped");
            switch (action)
            {
                case "ToggleClick":
                    if (sender.Selected)
                    {
                        //Stop
                        await AudioManager.Instance.StopAsync(AudioManager.SoundType.Click);
                        sender.Selected = false;
                    }
                    else
                    {
                        //Start
                        await AudioManager.Instance.PlayAsync(ref _bpmModel);
                        sender.Selected = true;
                    }
                    break;
                case "StopAll":
                    await AudioManager.Instance.StopAsync(AudioManager.SoundType.All);
                    toggleClickButton.Selected = false; //Click button

                    //Stop all pads
                    foreach (var padButton in _padButtons)
                    {
                        padButton.Reset();
                    }

                    //Reset table
                    if (setlistTable.IndexPathForSelectedRow != null)
                        setlistTable.DeselectRow(setlistTable.IndexPathForSelectedRow, true);
                    break;
                default:
                    throw new NotSupportedException("This type of action isn't supported");
            }
        }

        partial void VolumeChange(UISlider sender)
        {
            var value = sender.Value;
            var slider = sender.AccessibilityIdentifier;
            Debug.WriteLine($"{slider} slider changed: {value}");
            switch(slider)
            {
                case "MasterVolume":
                    //TODO: Update both * master
                    AudioManager.Instance.MasterVolume = value;
                    Settings.MasterVolume = value;
                    break;
                case "ClickVolume":
                    //TODO: Update click * master
                    AudioManager.Instance.ClickVolume = value;
                    Settings.ClickVolume = value;
                    break;
                case "PadVolume":
                    //TODO: Update pad * master
                    AudioManager.Instance.PadVolume = value;
                    Settings.PadVolume = value;
                    break;
                default:
                    throw new NotSupportedException("This type of volume slider isn't supported");
            }
        }

        void _popView_SetlistAdded(object sender, SetlistEventArgs e)
        {
            _setlistSource.Items.Add(e.Setlist);
            LiteDbHelper.SaveCurrentSetlist(_setlistSource.Items);
            setlistTable.InsertRows(new NSIndexPath[] { NSIndexPath.Create(0, _setlistSource.Items.Count - 1) }, UITableViewRowAnimation.Automatic);
            _addPopupController.DismissViewController(true, () =>
            {
                Debug.WriteLine("Add Popup Dismissed");
                _popView.SetlistAdded -= _popView_SetlistAdded;
                _popView?.Dispose();
                _popView = null;
                _addPopupController?.Dispose();
                _addPopupController = null;
            });
        }
        #endregion
    }
}
