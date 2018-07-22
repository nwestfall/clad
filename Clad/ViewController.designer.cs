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
        }
    }
}