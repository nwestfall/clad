// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Clad
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel bpmLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStepper bpmStepperControl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton bpmTapButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider clickVolumeSlider { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lowerLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStepper lowerStepperControl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider masterVolumeSlider { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UINavigationBar navBar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider padVolumeSlider { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView setlistTable { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton toggleClickButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel upperLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStepper upperStepperControl { get; set; }


        [Action ("PadButton_Up:")]
        partial void PadButton_Up (Clad.PadButton sender);

        [Action ("ActionButtons:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionButtons (UIKit.UIButton sender);

        [Action ("Settings_Action:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Settings_Action (UIKit.UIBarButtonItem sender);

        [Action ("Share_Action:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Share_Action (UIKit.UIBarButtonItem sender);

        [Action ("VolumeChange:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void VolumeChange (UIKit.UISlider sender);

        void ReleaseDesignerOutlets ()
        {
            if (bpmLabel != null) {
                bpmLabel.Dispose ();
                bpmLabel = null;
            }

            if (bpmStepperControl != null) {
                bpmStepperControl.Dispose ();
                bpmStepperControl = null;
            }

            if (bpmTapButton != null) {
                bpmTapButton.Dispose ();
                bpmTapButton = null;
            }

            if (clickVolumeSlider != null) {
                clickVolumeSlider.Dispose ();
                clickVolumeSlider = null;
            }

            if (lowerLabel != null) {
                lowerLabel.Dispose ();
                lowerLabel = null;
            }

            if (lowerStepperControl != null) {
                lowerStepperControl.Dispose ();
                lowerStepperControl = null;
            }

            if (masterVolumeSlider != null) {
                masterVolumeSlider.Dispose ();
                masterVolumeSlider = null;
            }

            if (navBar != null) {
                navBar.Dispose ();
                navBar = null;
            }

            if (padVolumeSlider != null) {
                padVolumeSlider.Dispose ();
                padVolumeSlider = null;
            }

            if (setlistTable != null) {
                setlistTable.Dispose ();
                setlistTable = null;
            }

            if (toggleClickButton != null) {
                toggleClickButton.Dispose ();
                toggleClickButton = null;
            }

            if (upperLabel != null) {
                upperLabel.Dispose ();
                upperLabel = null;
            }

            if (upperStepperControl != null) {
                upperStepperControl.Dispose ();
                upperStepperControl = null;
            }
        }
    }
}