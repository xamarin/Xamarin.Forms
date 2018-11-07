﻿using Foundation;
using UIKit;

namespace Xamarin.Forms.ControlGallery.iOS
{
	public class CustomApplication : UIApplication
	{
		public CustomApplication()
		{
			ApplicationSupportsShakeToEdit = true;
		}

		public override void MotionEnded(UIEventSubtype motion, UIEvent evt)
		{
			if(motion == UIEventSubtype.MotionShake)
			{
				(Delegate as AppDelegate)?.Reset(string.Empty);
			}
			base.MotionEnded(motion, evt);
		}
	}
}
