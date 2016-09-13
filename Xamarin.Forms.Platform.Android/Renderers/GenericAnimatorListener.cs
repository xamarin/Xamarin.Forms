using System;
using Android.Animation;

namespace Xamarin.Forms.Platform.Android
{
	public class GenericAnimatorListener : AnimatorListenerAdapter
	{
		public Action<Animator> OnCancel { get; set; }

		public Action<Animator> OnEnd { get; set; }

		public Action<Animator> OnRepeat { get; set; }

		public override void OnAnimationCancel(Animator animation)
		{
			OnCancel?.Invoke(animation);
			base.OnAnimationCancel(animation);
		}

		public override void OnAnimationEnd(Animator animation)
		{
			OnEnd?.Invoke(animation);
			base.OnAnimationEnd(animation);
		}

		public override void OnAnimationRepeat(Animator animation)
		{
			OnRepeat?.Invoke(animation);
			base.OnAnimationRepeat(animation);
		}

		protected override void JavaFinalize()
		{
			OnCancel = OnRepeat = OnEnd = null;
			base.JavaFinalize();
		}
	}
}