using System;
using System.Collections.Generic;
using System.Linq;
using ButtonCircle.FormsPlugin.iOS;
using Foundation;
using Lottie.Forms.iOS.Renderers;
using UIKit;

namespace Tabs.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            // Code for starting up the Xamarin Test Cloud Agent
#if DEBUG
			Xamarin.Calabash.Start();
#endif
            AnimationViewRenderer.Init();
            ButtonCircleRenderer.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
